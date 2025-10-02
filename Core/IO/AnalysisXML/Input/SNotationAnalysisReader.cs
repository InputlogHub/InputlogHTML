using InputLog.Core.IO.AnalysisXML.XML.Parts;
using System.Xml;
using System.Collections.Generic;
namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;
    internal class SNotationAnalysisReader : BasicXMLReader
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
            return this.ReadRevisions;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }

        protected List<Event> ReadRevisions(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session != null)
            {
                foreach (XmlElement xmlEvent in session.GetElementsByTagName(AnalysisXML.Session.SNotation.TAG))
                {
                    var ev = new Event();
                    // Type and Id and Output
                    ev.Properties[AnalysisXML.Session.SNotation.TAG] =
                        xmlEvent.InnerText;
 
                    events.Add(ev);
                }
            }

            return events;
        }
    }
}