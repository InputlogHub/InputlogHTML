using System.Collections.Generic;
using System.Xml;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;
    internal class RevisionAnalysisReader : BasicXMLReader
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
            return ReadRevisions;
        }

        public override ReadModulesMethod ReadModules()
        {
            return Modules;
        }

        private static List<Event> ReadRevisions(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session == null) return events;
            foreach (XmlElement xmlEvent in session.GetElementsByTagName(AnalysisXML.Session.Revision.TAG))
            {
                var ev = new Event();
                // Type and Id and Output
                XmlNodeList elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.RevisionNumber.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.RevisionNumber.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.RevisionNumber.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Type.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Type.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Type.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Content.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Content.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Content.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Edits.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Edits.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Edits.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Start.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Start.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Start.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.End.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.End.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.End.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Duration.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Duration.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Duration.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Length.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Length.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Length.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.BeginPos.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.BeginPos.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.BeginPos.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.EndPos.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.EndPos.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.EndPos.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Chars.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Chars.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Chars.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.CharWithoutSpace.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.CharWithoutSpace.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.CharWithoutSpace.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Revision.Words.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Revision.Words.TAG] =
                        xmlEvent[AnalysisXML.Session.Revision.Words.TAG].InnerText;
                }

                events.Add(ev);
            }

            return events;
        }
    }
}