using System.Collections.Generic;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
using System.Xml;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;

    internal class LinguisticAnalysisReader : BasicXMLReader
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
            return LinguisticEvents;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }

        public List<Event> LinguisticEvents(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session != null)
            {
                foreach (XmlElement lingProc in session.GetElementsByTagName(AnalysisXML.Session.Linguistic.TAG))
                {
                    foreach (XmlElement xmlEvent in lingProc.GetElementsByTagName(AnalysisXML.Session.Linguistic.InfoTable.TAG))
                    {
                        var ev = new Event();
                        // Type and Id and Output
                        string[] tags = new string[] {
                            AnalysisXML.Session.Linguistic.InfoTable.Revisions.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.SNotation.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.CharsProduced.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.Token.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.PoSA.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.PoSB.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.PoSProb.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.Lemma.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.LemmaProb.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.ChunkA.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.ChunkB.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.NE.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.NEProb.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.LogFreq.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.RelFreq.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.Syllable.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.StartID.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.EndID.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.StartTime.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.EndTime.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.BeforeWord2.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.BeforeWord1.TAG,
                             AnalysisXML.Session.Linguistic.InfoTable.BetweenPause.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.Production.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.WordPause.TAG,
                            AnalysisXML.Session.Linguistic.InfoTable.AfterWordPause.TAG,
                        };
                        foreach (string tag in tags)
                        {
                            XmlNodeList elements = xmlEvent.GetElementsByTagName(tag);
                            if (elements.Count > 0)
                            {
                                ev.Properties[tag] = xmlEvent[tag].InnerText;
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