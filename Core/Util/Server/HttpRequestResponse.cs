using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;

namespace InputLog.Core.Util.Server
{
    /// <summary>
    /// Facilitates requesting and getting response via HTTP
    /// </summary>
    class HttpRequestResponse
    {
        private readonly string URL;

        public HttpRequestResponse(String url)
        {
            URL = url;
        }

        /// <summary>
        ///This public interface receives the request and sends the response of type string.
        /// </summary>
        public XmlDocument SendRequest(Dictionary<String, String> parameters)
        {
            String getURL = URL;
            bool first = true;
            foreach (KeyValuePair<String, String> pair in parameters)
            {
                if (first)
                {
                    first = false;
                    getURL = getURL + String.Format("?{0}={1}", pair.Key, pair.Value);
                }
                else
                {
                    getURL = getURL + String.Format("&{0}={1}", pair.Key, pair.Value);
                }
            }
            WebRequest wrGeturl = WebRequest.Create(getURL);
            Stream result = wrGeturl.GetResponse().GetResponseStream();
            var doc = new XmlDocument();
            if (result != null) doc.Load(result);
            return doc;
        } 

        private WebException CatchHttpExceptions(string errMsg)
        {
            errMsg = "Error During Web Interface. Error is: " + errMsg;
            return new WebException(errMsg);
        }
    }
}