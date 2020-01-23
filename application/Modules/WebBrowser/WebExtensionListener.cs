using System;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Text.Json;

namespace MORR.Modules.WebBrowser
{
    class WebExtensionListener
    {
        private class WebBrowserRequest
        {
            public string Request { get; set; }
            public string? Data { get; set; }
        }

        private class WebBrowserResponse
        {
            //this is only here to the JsonSerializer adds it to the response
            public string Application { get; } = "MORR";
            public string Response { get; }
            public string? Config { get; }
            public WebBrowserResponse(string response, string? config = null)
            {
                this.Response = response;
                this.Config = config;
            }
        }

        private enum WebBrowserRequestType
        {
            CONNECT,
            CONFIG,
            START,
            SENDDATA,
            WAITSTOP
        }

        private static readonly string URLPREFIX = "http://localhost:";
        bool running = false;
        //i am not 100% positive that i need a threadsafe collection here
        //depends on the way the asynchronous BeginGetContext is handled internally
        private ConcurrentQueue<HttpListenerResponse> startQueue;
        private ConcurrentQueue<HttpListenerResponse> stopQueue;
        private readonly HttpListener listener;

        /// <summary>
        /// Create a new WebExtensionListener, listening on localhost with port and optionally a directory determined by urlSuffix
        /// </summary>
        /// <param name="urlSuffix">the urlSuffic consisting of port number and optional directory. Must end in a slash '/' character.</param>
        ///<exception cref = "HttpListenerException" >If a listener already listens on the given port (might also be another application).</exception>
        ///<exception cref = "UriFormatException" >If the urlSuffix is invalid.</exception>
        public WebExtensionListener(Uri urlSuffix) 
        {
            listener = new HttpListener();
            Uri connectionUri = new Uri(URLPREFIX + urlSuffix);
            //this might throw the HttpListenerException
            listener.Prefixes.Add(URLPREFIX + urlSuffix);
            startQueue = new ConcurrentQueue<HttpListenerResponse>();
            stopQueue = new ConcurrentQueue<HttpListenerResponse>();
            //the is always running while the morr application is running to answer requests.
            //state is handled by giving different answers based on a recording being active or not
            listener.Start();
            listener.BeginGetContext(RetrieveRequest, null);
        }

        public void Start()
        {
            foreach (HttpListenerResponse response in startQueue)
            {
                AnswerRequest(response, new WebBrowserResponse("Start"));
            }
            startQueue.Clear();
        }

        public void Stop()
        {
            foreach (HttpListenerResponse response in stopQueue)
            {
                AnswerRequest(response, new WebBrowserResponse("Stop"));
            }
            stopQueue.Clear();
        }

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
                webBrowserRequest = JsonSerializer.Deserialize<WebBrowserRequest>(decodedRequest);
            }
            catch (JsonException)
            {
                AnswerInvalid(context.Response);
                return;
            }
            HandleRequest(webBrowserRequest, context);
        }

        private void HandleRequest(WebBrowserRequest request, HttpListenerContext context)
        {
            WebBrowserRequestType type;
            if (!Enum.TryParse(request.Request, true, out type))
            {
                AnswerInvalid(context.Response);
            }
            else
            {
                switch (type)
                {
                    case (WebBrowserRequestType.CONNECT):
                        AnswerRequest(context.Response, new WebBrowserResponse("Ok"));
                        break;
                    case (WebBrowserRequestType.CONFIG):
                        AnswerRequest(context.Response, new WebBrowserResponse("Ok", "")); //TODO: retrieve and send config
                        break;
                    case (WebBrowserRequestType.START):
                        if (running)
                            AnswerRequest(context.Response, new WebBrowserResponse("Start"));
                        else
                            startQueue.Enqueue(context.Response);
                        break;
                    case (WebBrowserRequestType.SENDDATA):
                        break;
                    case (WebBrowserRequestType.WAITSTOP):
                        if (!running)
                            AnswerRequest(context.Response, new WebBrowserResponse("Stop"));
                        else
                            stopQueue.Enqueue(context.Response);
                        break;
                    default:
                        //theoretically unreachable, as Enum.TryParse should have failed in this case
                        AnswerInvalid(context.Response);
                        break;
                }
            }
        }

        private void AnswerInvalid(HttpListenerResponse response)
        {
            AnswerRequest(response, new WebBrowserResponse("Invalid Request"));
        }

        private void AnswerRequest(HttpListenerResponse response, WebBrowserResponse answer)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize((answer)));
            response.ContentType = "Application/json";
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            //url is not guaranteed to be decoded within a single call
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
    }
}
