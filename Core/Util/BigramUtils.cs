using System.Collections.Generic;
using System.Xml;

namespace InputLog.Core.Util
{
    public static class BigramUtils
    {

        public static Dictionary<string, Dictionary<string, string>> ReadBasicBigramInfo(string file)
        {
            Dictionary<string, Dictionary<string, string>> info = new Dictionary<string, Dictionary<string, string>>();
            XmlReader reader = XmlReader.Create(file);
            reader.ReadStartElement("bigrams");
            while (reader.IsStartElement("bigram"))
            {
                reader.ReadStartElement("bigram");
                string id = reader.ReadElementString("id");
                Dictionary<string, string> thisInfo = new Dictionary<string, string>();
                thisInfo.Add("id", id);
                info.Add(id, thisInfo);
                while (reader.IsStartElement())
                {
                    string name = reader.Name;
                    string val = reader.ReadElementString();
                    thisInfo.Add(name, val);
                }
                reader.ReadEndElement(); //"bigram"
            }
            reader.ReadEndElement(); //"bigrams"
            return info;
        }
    }
}
