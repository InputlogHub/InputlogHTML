using InputLog.Core.IO.AnalysisXML.XML.Parts;
using System.Xml;
using System.Collections.Generic;
namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;
    using System;
    internal class LinearAnalysisReader : BasicXMLReader
    {
        public override ReadHeaderMethod ReadHeader()
        {
            return Header;
        }

        public override ReadExtraInfoMethod ReadExtraInfo()
        {
            return ExtraInfo;
        }

        public override ReadEventsMethod ReadEvents()
        {
            return this.ReadPeriods;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }

        protected List<Event> ReadPeriods(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session != null)
            {
                bool condensed = false;
                foreach (XmlElement xmlEvent in session.GetElementsByTagName(AnalysisXML.Session.Period.TAG))
                {
                    var ev = new Event();
                    ev.Properties[AnalysisXML.Session.Period.PeriodID.TAG] = xmlEvent.Attributes[AnalysisXML.Session.Period.PeriodID.TAG].InnerText;
                    var periodTime = xmlEvent.Attributes[AnalysisXML.Session.Period.PeriodTime.TAG];
                    ev.Properties[AnalysisXML.Session.Period.PeriodTime.TAG] = periodTime != null ? periodTime.InnerText : "";
                    if (periodTime == null) condensed = true;
                    ev.Properties[AnalysisXML.Session.Period.Condensed.TAG] = condensed.ToString();
                    XmlNodeList elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Period.PeriodEvent.TAG);
                    if (elements.Count > 0)
                    {
                        string[] pEvents = new string[elements.Count];
                        int i = 0;
                        foreach (XmlElement e in elements)
                        {
                            pEvents[i] = e.Attributes[AnalysisXML.Session.Period.PeriodEvent.Value.TAG].InnerText;
                            i++;
                        }
                        ev.Properties[AnalysisXML.Session.Period.PeriodEvent.TAG] = String.Join(";", pEvents);
                    }
                    else
                    {
                        ev.Properties[AnalysisXML.Session.Period.PeriodEvent.TAG] = "";
                    }
                    events.Add(ev);
                }
            }

            return events;
        }
    }
}