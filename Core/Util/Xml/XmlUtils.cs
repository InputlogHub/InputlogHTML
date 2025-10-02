using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InputLog.Core.Util
{
    /// <summary>
    /// Simple utilities for helping in reading XML data
    /// </summary>
    public static class XmlUtils
    {
        public static void WriteAttributeElement(this XmlWriter writer, string elName, string attrName, string attrValue)
        {
            writer.WriteStartElement(elName);
            writer.WriteAttributeString(attrName, attrValue);
            writer.WriteEndElement();
        }

        public static bool ReadIntIfPresent(XmlReader Reader, String Element, out int Result)
        {
            String S = Reader.ReadElementString(Element);
            if (!String.IsNullOrEmpty(S))
            {
                Result = Int32.Parse(S);
                return true;
            }
            else
            {
                Result = 0;
                return false;
            }
        }

        public static bool ReadUlongIfPresent(XmlReader Reader, String Element, out ulong Result)
        {
            String S = Reader.ReadElementString(Element);
            if (!String.IsNullOrEmpty(S))
            {
                Result = ulong.Parse(S);
                return true;
            }
            else
            {
                Result = 0;
                return false;
            }
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static string SanitizeXmlString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public static XmlElement GetElementByAttrValue(this XmlNode node, string attr, string val)
        {
            if (node is XmlElement)
            {
                XmlElement elem = (XmlElement)node;
                if (elem.HasAttribute(attr) && elem.GetAttribute(attr).Equals(val))
                {
                    return elem;
                }
            }
            foreach (XmlNode child in node.ChildNodes)
            {
                XmlElement res = child.GetElementByAttrValue(attr,val);
                if (res != null) return res;
            }
            return null;
        }

    }
}
