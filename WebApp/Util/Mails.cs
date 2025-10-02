using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Postal;
using InputLog.Core.Util;

namespace WebApp.Util
{
    public static class Mails
    {
        public static void SendWrapped(string to, string content, string subject, List<Pair<string,string>> attach = null)
        {
            dynamic email = new Email("DefaultWrapper");
            email.To = to;
            email.Subject = subject;
            email.Content = content;
            var stream = new WebClient().OpenRead(ConfigurationManager.AppSettings["SiteURL"] + "Content/DescriptionHeader.PNG");
            Attachment header = new Attachment(stream ?? throw new InvalidOperationException(), "DescriptionHeader.PNG");
            email.Attach(header);
            email.cid = header.ContentId;
            if (attach != null)
            {
                foreach (var a in attach)
                {
                    stream = new WebClient().OpenRead(a.Second);
                    Attachment att = new Attachment(stream ?? throw new InvalidOperationException(), a.First);
                    email.Attach(att);
                }
            }
            email.Send();
        }
    }
}