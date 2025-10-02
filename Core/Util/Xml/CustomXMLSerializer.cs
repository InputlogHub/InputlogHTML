using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Linq;

namespace InputLog.Core.Util.Xml
{
    /// <summary>
    /// Utility class for serializing/deserializing an object with its full type name included.
    /// Uses default ReadXml/WriteXml methods for actual serialization of the object.
    /// </summary>
    public static class CustomXMLSerializer
    {
        private static string PublicKeyToken = null;


        /// <summary>
        /// Returns the object that was serialized to InFile
        /// </summary>
        /// <param name="inFile">Filename of the serialized object</param>
        /// <returns></returns>
        public static IXmlSerializable Deserialize(FileStream inFile)
        {
            // If the PublicKeyToken isn't set yet. Set it first.
            if (PublicKeyToken == null)
            {
                AssemblyName executingAssemblyName = Assembly.GetExecutingAssembly().GetName();
                byte[] publicKeyTokenBytes = executingAssemblyName.GetPublicKeyToken();
                PublicKeyToken = String.Concat(publicKeyTokenBytes.Select(aByte => aByte.ToString("x2")));
            }

			var settings = new XmlReaderSettings { CheckCharacters = false };
            var reader = new XmlTextReader(inFile);
            while (!reader.IsStartElement("Object"))
            {
                reader.Read();
            }
            reader.ReadStartElement("Object");
            var typeName = reader.ReadElementString("FullType");
            string alteredTypeName = PublicKeyTokenTampering(typeName);

            var T = Type.GetType(alteredTypeName);
            if (T != null)
            {
                var result = (IXmlSerializable) Activator.CreateInstance(T);
                result.ReadXml(reader);
                return result;
            }
            return null;
        }

        /// <summary>
        /// This method tampers with the PublicKeyToken in the given typename. It extracts the 
        /// publicKeyToken, and if it is different from this Assembly's PublicKeyToken it then
        /// it changes the original token with this Assembly's token. 
        /// 
        /// This is done so that even though older versions of the InputLogCore might get different
        /// PublicKeyTokens when serializing the object - if there haven't been any large changes
        /// to the serialized classes, they may still be deserialized by a newer assembly.
        /// </summary>
        /// <param name="orignalType">The original, full type name.</param>
        /// <returns>The tampered full type name - which has this assembly's PublicKeyToken.</returns>
        private static string PublicKeyTokenTampering(string originalType)
        {
            const string pktokenTag = "PublicKeyToken=";

            // Get the original PublicKeyToken
            int pktokenIndex = originalType.IndexOf(pktokenTag);
            string originalPKToken = originalType.Substring(pktokenIndex + pktokenTag.Length);

            if (originalPKToken != PublicKeyToken)
            {
                // Create a new TypeString with altered PublicKeyToken
                string newType = originalType.Substring(0, pktokenIndex + pktokenTag.Length) + PublicKeyToken;

                Console.WriteLine("[PublicKeyTokenTampering] Changing token from \"" +
                    originalPKToken + "\" to \"" + PublicKeyToken + "\"");
                Console.WriteLine("[PublicKeyTokenTampering] Original Type: " + originalType);
                Console.WriteLine("[PublicKeyTokenTampering] New Type: " + newType);

                return newType;
            }
            return originalType;
        }

        /// <summary>
        /// Serializes the given object to Outfile
        /// </summary>
        /// <param name="outFile">File to serialize to</param>
        /// <param name="Object">Object to serialize</param>
        public static void Serialize(FileStream outFile, IXmlSerializable Object)
        {
            var settings = new XmlWriterSettings {Indent = true, CheckCharacters = false, CloseOutput = true};
            var writer = XmlWriter.Create(outFile, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Object");
            writer.WriteElementString("FullType", Object.GetType().AssemblyQualifiedName);
            Object.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }
}
