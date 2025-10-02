using System.Collections.Generic;
using System.Xml;
using InputLog.Core.IO.AnalysisXML.XML.Parts;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;

    internal class WordPausesAnalysisReader : BasicXMLReader
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
            return WordPausesEvents;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }

        private List<Event> WordPausesEvents(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session != null)
            {
                foreach (XmlElement wpElement in session.GetElementsByTagName(AnalysisXML.Session.WordPauses.TAG))
                {
                    foreach (XmlElement xmlEvent in wpElement.GetElementsByTagName(AnalysisXML.Session.WordPauses.InfoTable.TAG))
                    {
                        var ev = new Event();
                        // Type and Id and Output
                        string[] tags = {
                            AnalysisXML.Session.WordPauses.InfoTable.Revisions.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.SNotation.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.CharsProduced.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.Token.TAG,                           
                            AnalysisXML.Session.WordPauses.InfoTable.StartID.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.EndID.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.StartTime.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.EndTime.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.BeforeWord2.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.BeforeWord1.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.BetweenPause.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.Production.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.WordPause.TAG,
                            AnalysisXML.Session.WordPauses.InfoTable.AfterWordPause.TAG,
                             AnalysisXML.Session.WordPauses.InfoTable.Target.TAG
                        };
                        foreach (string tag in tags)
                        {
                            XmlNodeList elements = xmlEvent.GetElementsByTagName(tag);
                            if (elements.Count > 0)
                            {
                                var xmlElement = xmlEvent[tag];
                                if (xmlElement != null) ev.Properties[tag] = xmlElement.InnerText;
                            }
                        }

                        events.Add(ev);
                    }
                }
            }

            return events;
        }
    }
}
