using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Uploader4PDA
{
    public class MyWebClient : WebClient
    {
        public MyWebClient() : base()
        {
            container = new CookieContainer();
        }
        public MyWebClient(CookieContainer container) : base()
        {
            this.container = container;
        }
        public MyWebClient(int timeout) : this()
        {
            Timeout = timeout;
        }
        public MyWebClient(CookieContainer container, int timeout) : this(container)
        {
            Timeout = timeout;
        }

        private CookieContainer container;

        public CookieContainer CookieContainer
        {
            get { return container; }
            set { container = value; }
        }


        public int Timeout { get; set; }
        private void ReadCookies(WebResponse r)
        {
            if (r is HttpWebResponse response)
            {
                CookieCollection cookies = response.Cookies;
                container?.Add(cookies);
            }
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var webRequest = base.GetWebRequest(uri);
            if (webRequest is HttpWebRequest request)
            {
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
                if (container != null)
                {
                    request.CookieContainer = container;
                }
                if (Timeout > 0)
                {
                    request.Timeout = Timeout;
                    request.ContinueTimeout = Timeout;
                    request.ReadWriteTimeout = Timeout;
                }
            }
            return webRequest;
        }
    }
}
