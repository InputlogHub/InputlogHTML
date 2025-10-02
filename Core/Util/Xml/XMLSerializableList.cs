using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace InputLog.Core.Util
{
    public class XMLSerializableList<T> : List<T>, IXmlSerializable where T : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("SerializedList");
            while (reader.IsStartElement("Exception"))
            {
                reader.ReadStartElement("Exception");
                var typeName = reader.ReadElementString("FullType");
                var ct = Type.GetType(typeName);
                if (ct != null)
                {
                    var result = (IXmlSerializable)Activator.CreateInstance(ct);
                    T exc = (T)result;
                    result.ReadXml(reader);
                    Add(exc);
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("SerializedList");
            ForEach((x) =>
            {
                writer.WriteStartElement("Exception");
                writer.WriteElementString("FullType", x.GetType().AssemblyQualifiedName);
                x.WriteXml(writer);
                writer.WriteEndElement();
            });
            writer.WriteEndElement();
        }
    }
}
