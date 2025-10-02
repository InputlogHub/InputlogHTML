using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace InputLog.Core.Util.Server
{
    public class ServerAnalysisException : Exception, IXmlSerializable
    {
        public string Filename;
        public new string Message;

        public ServerAnalysisException(string filename)
        {
            Filename = filename;
            Message = "An unknown error has occurred";
        }

        public ServerAnalysisException(string filename, string msg)
        {
            Filename = filename;
            Message = msg;
        }

        // Parametersless constructor for use with Xml serialization
        public ServerAnalysisException()
        {
        }

        public override string ToString()
        {
            return "An exception occured while processing file " + Filename + " on the Inputlog server: " + Message;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Filename = reader.ReadElementString("Filename");
            Message = reader.ReadElementString("Message");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Filename", Filename);
            writer.WriteElementString("Message", Message);
        }
    }

    public class RemoteCallException : Exception
    {
        public string Error;

        public RemoteCallException(string error)
        {
            Error = error;
        }

    }

    public class ServerDownException : Exception
    {

    }
}
