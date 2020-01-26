using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Utility;

namespace MORR.Modules.WebBrowser
{
    /// <summary>
    ///     The <see cref="WebExtensionListener" /> is responsible for maintaining the connection to the WebBrowser(s).
    ///     It will answer incoming requests based on the recording state and receive incoming event data.
    /// </summary>
    internal class WebExtensionListener : IWebBrowserEventObservable
    {
        private readonly HttpListener listener;
        private const string serializedTypeField = "type";

        //deliberately don't use IList, as RemoveAll function is used later
        private readonly List<Tuple<IWebBrowserEventObserver, Type>> observers;

        //i am not 100% positive that i need a threadsafe collection here
        //depends on the way the asynchronous BeginGetContext is handled internally
        private readonly ConcurrentQueue<HttpListenerResponse> startQueue;
        private readonly ConcurrentQueue<HttpListenerResponse> stopQueue;

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
            listener = new HttpListener();
            //this might throw the HttpListenerException
            listener.Prefixes.Add(new Uri(URLPREFIX + urlSuffix)
                                      .ToString()); //the short conversion to Uri is just to check URL validity
            startQueue = new ConcurrentQueue<HttpListenerResponse>();
            stopQueue = new ConcurrentQueue<HttpListenerResponse>();
            observers = new List<Tuple<IWebBrowserEventObserver, Type>>();
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
            if (!listener.IsListening)
            {
                listener.Start();
                listener.BeginGetContext(RetrieveRequest, null);
            }
        }

        /// <summary>
        ///     Stop listening and handling requests.
        /// </summary>
        public void StopListening()
        {
            if (listener.IsListening)
            {
                listener.Stop();
                //inform all the connected applications about the listener having stopped
                foreach (var response in startQueue)
                {
                    AnswerRequest(response, new WebBrowserResponse("Quit"));
                }

                foreach (var response in stopQueue)
                {
                    AnswerRequest(response, new WebBrowserResponse("Quit"));
                }
            }
        }

        #region Private helper classes

        //a private helper class to parse and identify the request
        private class WebBrowserRequest
        {
            public string Request { get; set; }
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
            WAITSTOP
        }

        #endregion

        #region constants

        //the possible responses to send
        private sealed class ResponseStrings
        {
            public static readonly string POSITIVERESPONSE = "Ok";
            public static readonly string NEGATIVERESPONSE = "Invalid Request";
            public static readonly string STARTRESPONSE = "Start";
            public static readonly string STOPRESPONSE = "Stop";
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
            var context = listener.EndGetContext(result);
            var request = context.Request;

            //get post data and decode it (will come in URL encoding)
            string decodedRequest;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                decodedRequest = DecodeUrlString(reader.ReadToEnd());
            }

            listener.BeginGetContext(RetrieveRequest, null); //get ready for next request

            //try parsing the request
            WebBrowserRequest webBrowserRequest;
            try
            {
                webBrowserRequest =
                    JsonSerializer.Deserialize<WebBrowserRequest>(decodedRequest,
                                                                  new JsonSerializerOptions
                                                                      { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
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
                                                             "undefined")); //TODO: retrieve and send config
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
                        if (request.Data != null && DeserializeEventAndBroadcast(request))
                        {
                            AnswerRequest(context.Response, new WebBrowserResponse(ResponseStrings.POSITIVERESPONSE));
                        }
                        else
                        {
                            AnswerInvalid(context.Response);
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

        //decode an URL-encoded string to UTF8 (i think)
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

            WebBrowserEvent @event;
            //choose event class based on type label
            //there may be some smoother way to map from string to class
            switch (label)
            {
                case EventLabel.BUTTONCLICK:
                    @event = new ButtonClickEvent();
                    break;
                case EventLabel.CLOSETAB:
                    @event = new CloseTabEvent();
                    break;
                case EventLabel.OPENTAB:
                    @event = new OpenTabEvent();
                    break;
                case EventLabel.SWITCHTAB:
                    @event = new SwitchTabEvent();
                    break;
                case EventLabel.NAVIGATION:
                    @event = new NavigationEvent();
                    break;
                case EventLabel.TEXTINPUT:
                    @event = new TextInputEvent();
                    break;
                case EventLabel.TEXTSELECTION:
                    @event = new TextSelectionEvent();
                    break;
                case EventLabel.DOWNLOAD:
                    @event = new FileDownloadEvent();
                    break;
                case EventLabel.HOVER:
                    @event = new HoverEvent();
                    break;
                default:
                    return false;
            }

            try
            {
                @event.Deserialize(parsed);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            NotifyAll(@event);
            return true;
        }

        #endregion

        #region Observer pattern implementation

        public void Subscribe(IWebBrowserEventObserver observer, Type eventType)
        {
            observers.RemoveAll(tuple => tuple.Item1 == observer);
            observers.Add(new Tuple<IWebBrowserEventObserver, Type>(observer, eventType));
        }

        public void Unsubscribe(IWebBrowserEventObserver observer)
        {
            observers.RemoveAll(tuple => tuple.Item1 == observer);
        }

        private void NotifyAll(WebBrowserEvent @event)
        {
            foreach (var tuple in observers.FindAll(tuple => tuple.Item2 == @event.GetType()))
            {
                tuple.Item1.Notify(@event);
            }
        }

        #endregion
    }
}