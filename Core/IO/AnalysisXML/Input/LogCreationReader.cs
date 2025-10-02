using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    /// <summary>
    /// Returns the creation date and time of a log file.
    /// </summary>
    public class LogCreationReader
    {
        private readonly IFormatProvider Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

        public DateTime GetCreationTime(string filePath)
        {
            try
            {
                string logTime = "";
                XDocument doc = XDocument.Load(new StreamReader(filePath));
                XElement session = doc.Element("session");
                if (session != null)
                {
                    XElement meta = session.Element("meta");
                    if (meta != null)
                    {
                        IEnumerable<XElement> entries = meta.Elements("entry");
                        foreach (var xElement in entries.
                            Where(xElement => xElement.FirstAttribute.Value.Equals("LogCreationDate")))
                        {
                            logTime = xElement.LastAttribute.Value;
                        }
                        return DateTime.Parse(logTime, Culture, System.Globalization.DateTimeStyles.AssumeLocal);
                    }
                }
                return DateTime.Now;
            }
            catch (Exception)
            {
                Util.MessageLogger.LogMessage(this, "Reading Error",
                                              "Improperly constructed file " +
                                              "[" + filePath + "]", Util.Severity.ERROR);
            }
            return DateTime.Now;
        }
    }
}
