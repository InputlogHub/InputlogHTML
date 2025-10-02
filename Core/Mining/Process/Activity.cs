using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util;

namespace InputLog.Core.Mining.Process
{
    public class Activity
    {
        private ulong Timestamp;
        private Dictionary<string, string> Properties;

        public Activity(ulong timestamp)
        {
            Timestamp = timestamp;
            Properties = new Dictionary<string, string>();
        }

        public void SetProperty(string key, string val)
        {
            Properties.Add(key, val);
        }

        internal void ToXml(System.Xml.XmlWriter writer)
        {
            // Name
            writer.WriteStartElement("string");
            writer.WriteAttributeString("key", "concept:name");
            writer.WriteAttributeString("value", JoinProperties());
            writer.WriteEndElement();

            // Timestamp
            writer.WriteStartElement("date");
            writer.WriteAttributeString("key", "time:timestamp");
            writer.WriteAttributeString("value", DateTimeUtils.FromTimestamp(Timestamp).ToXESRoundTrip());
            writer.WriteEndElement();

            foreach (string key in Properties.Keys)
            {
                // Timestamp
                writer.WriteStartElement("string");
                writer.WriteAttributeString("key", key);
                writer.WriteAttributeString("value", Properties[key]);
                writer.WriteEndElement();
            }
        }

        private string JoinProperties()
        {
            return String.Join("-", Properties.Values);
        }
    }
}
