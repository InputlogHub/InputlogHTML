using System;
using InputLog.Core.Util.Server;
using InputLog.Core.be.ugent.lt3serv;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Webservice
{
    public class LinguisticWebserviceHandler
    {
        private string _id;
        private string _status;
        private readonly PreProWS PPWS = new();

        public string CallAndWait(string lang, string text)
        {
            Submit(lang, text);
            int interval = 1000 + (text.Length / 1000);
            while (true)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(interval));
                CheckStatus();
                if (!(StatusProcessing() || StatusWaiting())) break;
            }
            string res = GetResult();
            if (res.Substring(0, 7).ToLower().StartsWith("error"))
            {
                throw new RemoteCallException(res);
            }
            return res;
        }

        private void Submit(string lang, string text)
        {
            _id = PPWS.submit(lang, text);
        }

        private void CheckStatus()
        {
            _status = PPWS.check(_id);
        }

        private string GetResult()
        {
            return PPWS.acquire(_id);
        }

        private bool StatusWaiting()
        {
            return _status.Equals("waiting");
        }

        private bool StatusProcessing()
        {
            return _status.Equals("processing");
        }

        public bool StatusDone()
        {
            return _status.Equals("done");
        }

        public bool StatusError()
        {
            return _status.Equals("error");
        }
    }
}
