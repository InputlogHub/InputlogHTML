using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Mining.Process
{
    public class Case
    {
        private string ID;
        private List<Activity> Activities;

        public Case(string id)
        {
            ID = id;
            Activities = new List<Activity>();
        }

        public void AddActivity(Activity act)
        {
            Activities.Add(act);
        }

        internal void ToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("string");
            writer.WriteAttributeString("key", "concept:name");
            writer.WriteAttributeString("value", ID);
            writer.WriteEndElement();
            foreach (var act in Activities)
            {
                writer.WriteStartElement("event");
                act.ToXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}
