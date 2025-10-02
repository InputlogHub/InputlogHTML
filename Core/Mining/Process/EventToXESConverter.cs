using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;
using System.IO;
using System.Xml;

namespace InputLog.Core.Mining.Process
{
    public class EventToXESConverter
    {
        private List<Event> Events;
        private ICaseSplitter CaseSplitter;
        private IActivitySelector ActivitySelector;
        private DateTime Date;

        public EventToXESConverter(List<Event> events, ICaseSplitter caseSpl, IActivitySelector acSel)
        {
            Events = events;
            CaseSplitter = caseSpl;
            ActivitySelector = acSel;
        }

        public void Convert(string outFile)
        {
            var splits = CaseSplitter.Split(Events);
            List<Case> cases = new List<Case>();
            foreach (string key in splits.Keys)
            {
                Case cas = new Case(key);
                var acts = ActivitySelector.Select(splits[key]);
                foreach (var act in acts)
                {
                    cas.AddActivity(act);
                }
                cases.Add(cas);
            }
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(outFile, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("XES version 2.0");
                writer.WriteComment("Created by Inputlog (http://www.inputlog.net)");
                writer.WriteStartElement("log", "http://www.xes-standard.org/");
                writer.WriteAttributeString("xes.version", "2.0");

                // Concept extension
                writer.WriteStartElement("extension");
                writer.WriteAttributeString("name", "Concept");
                writer.WriteAttributeString("prefix", "concept");
                writer.WriteAttributeString("uri", "http://www.xes-standard.org/concept.xesext");
                writer.WriteEndElement();

                // Time extension
                writer.WriteStartElement("extension");
                writer.WriteAttributeString("name", "Time");
                writer.WriteAttributeString("prefix", "time");
                writer.WriteAttributeString("uri", "http://www.xes-standard.org/time.xesext");
                writer.WriteEndElement();

                // Trace ID
                writer.WriteStartElement("global");
                writer.WriteAttributeString("scope", "trace");
                writer.WriteStartElement("string");
                writer.WriteAttributeString("key", "concept:name");
                writer.WriteAttributeString("value", "Inputlog Event Log");
                writer.WriteEndElement();
                writer.WriteEndElement();

                // Global Event Config
                writer.WriteStartElement("global");
                writer.WriteAttributeString("scope", "event");
                writer.WriteStartElement("string");
                writer.WriteAttributeString("key", "concept:name");
                writer.WriteAttributeString("value", "name");
                writer.WriteEndElement();
                writer.WriteStartElement("date");
                writer.WriteAttributeString("key", "time:timestamp");
                writer.WriteAttributeString("value", "1970-01-01T00:00:00.000+00:00");
                writer.WriteEndElement();
                writer.WriteEndElement();

                foreach (Case c in cases)
                {
                    writer.WriteStartElement("trace");
                    c.ToXml(writer);
                    writer.WriteEndElement();
                }

                // Close <log> and document
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
