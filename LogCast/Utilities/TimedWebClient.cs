using System;
using System.Net;

namespace LogCast.Utilities
{
    public class TimedWebClient : WebClient
    {
        static TimedWebClient()
        {
            //workaround to override limit of parallel connections per host (2 by default)
            if (ServicePointManager.DefaultConnectionLimit < 10)
                ServicePointManager.DefaultConnectionLimit = 100;
        }

        private readonly int _timeout;

        public TimedWebClient(int timeout)
        {
            _timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var request = (HttpWebRequest)base.GetWebRequest(uri);

            if (request != null)
            {
                request.ContentType = "application/json";
                request.Timeout = _timeout;
            }

            return request;
        }
    }
}