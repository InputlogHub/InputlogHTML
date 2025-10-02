using System.Collections.Generic;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
using System.Xml;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;
    using System.Collections;

    internal class GeneralEyetrackAnalysisReader : BasicXMLReader
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
            return ReadEyetrack;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }

        protected List<Event> ReadEyetrack(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session != null)
            {
                foreach (XmlElement xmlEvent in session.GetElementsByTagName(AnalysisXML.Session.Event.TAG))
                {
                    var ev = new Event();
                    IEnumerator ienum = xmlEvent.GetEnumerator();
                    XmlNode node;
                    while (ienum.MoveNext())
                    {
                        node = (XmlNode)ienum.Current;
                        ev.Properties[node.Name] = node.InnerText;
                    }
                    events.Add(ev);
                }
            }

            return events;
        }
    }
}