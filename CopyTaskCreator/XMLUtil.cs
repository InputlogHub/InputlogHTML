using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CopyTaskCreator
{
    class XMLUtil
    {
        /// <summary>
        /// Tries to retrieve attribute from xmlNode. If attribute doesn't exist, returns empty string.
        /// </summary>
        public static string GetAttributeValue(XmlNode xNode, string attributeToFind)
        {
            string returnValue = string.Empty;
            XmlElement ele = xNode as XmlElement;

            if (ele.HasAttribute(attributeToFind))
                returnValue = ele.GetAttribute(attributeToFind);

            return returnValue;
        }

        /// <summary>
        /// Tries to retrieve subnode inner text from xmlNode. If subnode doesn't exist, returns empty string.
        /// </summary>
        public static string GetChildNodeValue(XmlNode xNode, string nodeToFind)
        {
            string returnValue = string.Empty;

            XmlNode childNode =  xNode[nodeToFind];
            if (childNode != null)
                returnValue = childNode.InnerText;

            return returnValue;
        }


    }
}
