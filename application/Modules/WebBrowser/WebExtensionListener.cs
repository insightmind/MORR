using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MORR.Shared.Utility;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     The <see cref="WebExtensionListener" /> is responsible for maintaining the connection to the WebBrowser(s).
    ///     It will answer incoming requests based on the recording state and receive incoming event data.
    /// </summary>
    internal class WebExtensionListener : IWebBrowserEventObservable
    {
        private const string serializedTypeField = "type";

        private const int
            ERROR_OPERATION_ABORTED = 995; //errorcode thrown by the async function when listener is stopped

        private const int ERROR_ALREADY_EXISTS = 183;

        //deliberately don't use IList, as RemoveAll function is used later
        private readonly Dictionary<EventLabel, List<IWebBrowserEventObserver>> observers;

        //i am not 100% positive that i need a threadsafe collection here
        //depends on the way the asynchronous BeginGetContext is handled internally
        private readonly ConcurrentQueue<HttpListenerResponse> startQueue;
        private readonly ConcurrentQueue<HttpListenerResponse> stopQueue;

        private HttpListener? listener;

        private readonly Uri listenerPrefix;
        private bool recordingActive;

        /// <summary>
        ///     Create a new WebExtensionListener, listening on localhost with port and optionally a directory determined by
        ///     urlSuffix
        /// </summary>
        /// <param name="urlSuffix">
        ///     the url suffix consisting of port number and optional directory. Must end in a slash '/'
        ///     character.
        /// </param>
        /// <exception cref="HttpListenerException">
        ///     If a listener already listens on the given port (might also be another
        ///     application).
        /// </exception>
        /// <exception cref="UriFormatException">If the urlSuffix is invalid.</exception>
        public WebExtensionListener(string urlSuffix)
        {
            listenerPrefix = new Uri(URLPREFIX + urlSuffix);
            startQueue = new ConcurrentQueue<HttpListenerResponse>();
            stopQueue = new ConcurrentQueue<HttpListenerResponse>();
            observers = new Dictionary<EventLabel, List<IWebBrowserEventObserver>>();
        }

        public bool RecordingActive
        {
            get => recordingActive;
            set => Utility.SetAndDispatch(ref recordingActive, value, Start, Stop);
        }

        /// <summary>
        ///     Start listening and handling requests.
        /// </summary>
        public void StartListening()
        {
            if (listener == null || !listener.IsListening)
            {
                if (listener == null)
                {
                    listener = new HttpListener();
                    listener.Prefixes.Add(listenerPrefix.ToString());
                }

                try
                {
                    listener.Start();
                    listener.BeginGetContext(RetrieveRequest, null);
                }
                catch (HttpListenerException ex)
                {
                    if (ex.ErrorCode == ERROR_ALREADY_EXISTS)
                    {
                        StopForeignInstance();
                        listener = new HttpListener();
                        listener.Prefixes.Add(listenerPrefix.ToString());
                        listener.Start();
                        listener.BeginGetContext(RetrieveRequest, null);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        ///     Stop listening and handling requests.
        /// </summary>
        public void StopListening()
        {
            StopListening(null);
        }

        //Stop listening, but respond to context just before stopping
        private void StopListening(HttpListenerContext? context)
        {
            if (listener == null)
            {
                return;
            }

            if (listener.IsListening)
            {
                //inform all the connected applications about the listener having stopped
                foreach (var response in startQueue)
                {
                    AnswerRequest(response, new WebBrowserResponse("Quit"));
                }

                foreach (var response in stopQueue)
                {
                    AnswerRequest(response, new WebBrowserResponse("Quit"));
                }

                if (context != null)
                {
                    AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.POSITIVERESPONSE));
                }

                listener.Stop();
                startQueue.Clear();
                stopQueue.Clear();
                listener = null;
            }
        }

        #region Private helper classes

        //a private helper class to parse and identify the request
        private class WebBrowserRequest
        {
            public string Request { get; set; } = "";
            public JsonElement? Data { get; set; }
        }

        //a private helper class to build the response
        private class WebBrowserResponse
        {
            public WebBrowserResponse(string response, string? config = null)
            {
                this.response = response;
                this.config = config;
            }

            //this is only here to the JsonSerializer adds it to the response
            //DO NOT change the names of these
#pragma warning disable IDE1006
            // ReSharper disable once InconsistentNaming
            public string application { get; } = "MORR";

            // ReSharper disable once InconsistentNaming
            public string response { get; }

            // ReSharper disable once InconsistentNaming
            public string? config { get; }
#pragma warning restore IDE1006
        }

        //the possible incoming requests
        private enum WebBrowserRequestType
        {
            CONNECT,
            CONFIG,
            START,
            SENDDATA,
            WAITSTOP,
            STOPLISTENING
        }

        #endregion

        #region constants

        //the possible responses to send
        private sealed class ResponseStrings
        {
            public const string POSITIVERESPONSE = "Ok";
            public const string NEGATIVERESPONSE = "Invalid Request";
            public const string STARTRESPONSE = "Start";
            public const string STOPRESPONSE = "Stop";
            public const string REQUESTDENIED = "Denied";
        }

        private static readonly string URLPREFIX = "http://localhost:";

        #endregion

        #region Private methods

        /// <summary>
        ///     Signal that the recording starts.
        /// </summary>
        private void Start()
        {
            foreach (var response in startQueue)
            {
                AnswerRequest(response, new WebBrowserResponse(ResponseStrings.STARTRESPONSE));
            }

            startQueue.Clear();
        }

        /// <summary>
        ///     Signal that the recording stops.
        /// </summary>
        private void Stop()
        {
            foreach (var response in stopQueue)
            {
                AnswerRequest(response, new WebBrowserResponse(ResponseStrings.STOPRESPONSE));
            }

            stopQueue.Clear();
        }

        //retrieve and parse another incoming request
        private void RetrieveRequest(IAsyncResult result)
        {
            HttpListenerContext? context = null;
            try
            {
                if (listener != null)
                {
                    context = listener.EndGetContext(result);
                }
            }
            catch (HttpListenerException ex)
            {
                //if the following condition is correct, the listener has been stopped, which is handled by simply cancelling the operation.
                if (ex.NativeErrorCode == ERROR_OPERATION_ABORTED)
                {
                    return;
                }

                throw ex;
            }

            if (context == null)
            {
                return;
            }

            var request = context.Request;

            //get post data and decode it (will come in URL encoding)
            string decodedRequest;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                decodedRequest = DecodeUrlString(reader.ReadToEnd());
            }

            listener?.BeginGetContext(RetrieveRequest, null); //get ready for next request

            //try parsing the request
            WebBrowserRequest webBrowserRequest;
            try
            {
                webBrowserRequest =
                    JsonSerializer.Deserialize<WebBrowserRequest>(decodedRequest,
                                                                  new JsonSerializerOptions
                                                                      { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                AnswerInvalid(context.Response);
                return;
            }

            HandleRequest(webBrowserRequest, context);
        }

        //handle a retrieved and parsed request
        private void HandleRequest(WebBrowserRequest request, HttpListenerContext context)
        {
            if (!Enum.TryParse(request.Request, true, out WebBrowserRequestType type))
            {
                AnswerInvalid(context.Response);
            }
            else
            {
                switch (type)
                {
                    case WebBrowserRequestType.CONNECT:
                        AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.POSITIVERESPONSE));
                        break;
                    case WebBrowserRequestType.CONFIG:
                        AnswerRequest(context.Response,
                                      new WebBrowserResponse(ResponseStrings.POSITIVERESPONSE,
                                                             "undefined")); //only stub-implementation in case it should find usage later
                        break;
                    case WebBrowserRequestType.START:
                        if (recordingActive)
                        {
                            AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.STARTRESPONSE));
                        }
                        else
                        {
                            startQueue.Enqueue(context.Response);
                        }

                        break;
                    case WebBrowserRequestType.SENDDATA:
                        if (!recordingActive)
                        {
                            //this code is only reached if the recording has been stopped in MORR,
                            //but the webextension has not yet been informed.
                            AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.STOPRESPONSE));
                        }
                        else
                        {
                            if (request.Data != null && DeserializeEventAndBroadcast(request))
                            {
                                AnswerRequest(context.Response,
                                              new WebBrowserResponse(ResponseStrings.POSITIVERESPONSE));
                            }
                            else
                            {
                                AnswerInvalid(context.Response);
                            }
                        }

                        break;
                    case WebBrowserRequestType.WAITSTOP:
                        if (!recordingActive)
                        {
                            AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.STOPRESPONSE));
                        }
                        else
                        {
                            stopQueue.Enqueue(context.Response);
                        }

                        break;
                    case WebBrowserRequestType.STOPLISTENING:
                        if (!recordingActive)
                        {
                            //stop listening to allow other instance to record on the specified port
                            StopListening(context);
                        }
                        else
                        {
                            //do not terminate if a recording is currently active
                            AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.REQUESTDENIED));
                        }

                        break;
                    default:
                        //theoretically unreachable, as Enum.TryParse should have failed in this case
                        AnswerInvalid(context.Response);
                        break;
                }
            }
        }

        //inform the sender that its request was invalid
        private void AnswerInvalid(HttpListenerResponse response)
        {
            AnswerRequest(response, new WebBrowserResponse(ResponseStrings.NEGATIVERESPONSE));
        }

        //send an answer to the sender
        private void AnswerRequest(HttpListenerResponse response, WebBrowserResponse answer)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(answer));
            response.ContentType = "Application/json";
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        //decode an URL-encoded string to UTF8
        private static string DecodeUrlString(string url)
        {
            string newUrl;
            //url is not guaranteed to be decoded within a single call
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
            {
                url = newUrl;
            }

            return newUrl;
        }

        private bool DeserializeEventAndBroadcast(WebBrowserRequest request)
        {
            if (request.Data == null)
            {
                return false;
            }

            var parsed = request.Data.Value;
            if (!Enum.TryParse(parsed.GetProperty(serializedTypeField).ToString(), true, out EventLabel label))
            {
                return false;
            }

            NotifyAll(parsed, label);
            return true;
        }

        //send stoplistening-request to the specified address, attempting to shut down another instance of webextension listener.
        private void StopForeignInstance()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = listenerPrefix;
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
                var response = SendHTTPMessage(new { Request = "STOPLISTENING" }, client);
                if (!response.TryGetProperty("application", out var applicationId) ||
                    !applicationId.ValueEquals("MORR"))
                {
                    throw new InvalidDataException("HTTP response did not contain a valid 'application' identifier.");
                }

                if (!response.TryGetProperty("response", out var answer))
                {
                    throw new InvalidDataException("HTTP response did not contain a 'response' field.");
                }

                if (answer.ValueEquals(ResponseStrings.POSITIVERESPONSE)) { }
                else if (answer.ValueEquals(ResponseStrings.REQUESTDENIED))
                {
                    throw new InvalidOperationException(
                        "Another instance of MORR is actively recording on this system.");
                }
                else
                {
                    throw new InvalidDataException("Received unexpected answer: " + answer);
                }
            }
        }

        private JsonElement SendHTTPMessage(object data, HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress");
            request.Content = new StringContent(JsonSerializer.Serialize(data),
                                                Encoding.UTF8,
                                                "application/json"); //CONTENT-TYPE header

            var response = client.PostAsync(listenerPrefix, request.Content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            return GetJsonFromString(result);
        }

        private static JsonElement GetJsonFromString(string data)
        {
            return JsonDocument.Parse(data).RootElement;
        }

        #endregion

        #region Observer pattern implementation

        public void Subscribe(IWebBrowserEventObserver observer, params EventLabel[] labels)
        {
            foreach (var label in labels)
            {
                if (!observers.ContainsKey(label)) //create list for label is nonexistent
                {
                    observers.Add(label, new List<IWebBrowserEventObserver>());
                }

                if (!observers[label].Contains(observer))
                {
                    observers[label].Add(observer);
                }
            }
        }

        public void Unsubscribe(IWebBrowserEventObserver observer, params EventLabel[] labels)
        {
            foreach (var label in labels)
            {
                if (observers.ContainsKey(label))
                {
                    observers[label].Remove(observer);
                    if (!observers[label].Any()) //clean up empty lists
                    {
                        observers.Remove(label);
                    }
                }
            }
        }

        private void NotifyAll(JsonElement eventJson, EventLabel label)
        {
            foreach (var observer in observers[label])
            {
                observer.Notify(eventJson);
            }
        }

        #endregion
    }
}