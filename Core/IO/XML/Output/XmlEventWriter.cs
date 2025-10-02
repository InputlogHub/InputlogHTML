using System.IO;
using System.Text;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Output;

namespace InputLog.Core.IO.Xml.Output
{
    /// <summary>
    ///     Eventwriter using xml.
    /// </summary>
    public class XmlEventWriter : AbstractEventWriter
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="stream">The stream to which the output will be sent.</param>
        public XmlEventWriter(Encoding encoding, Stream stream)
            : base(LogFormat.XML)
        {
            Append = false;
            var settings = new XmlWriterSettings
                           {
                               Indent = true,
                               Encoding = encoding,
                               CheckCharacters = false,
                               CloseOutput = true
                           };
            Writer = XmlWriter.Create(stream, settings);
        }

        /// <summary>
        ///     Constructor to be used if the XmlEventWriter should be appending
        ///     to an already created XML.
        /// </summary>
        /// <param name="writer"><b>XmlTextWriter</b> to be used for appending XML.</param>
        public XmlEventWriter(XmlWriter writer)
            : base(LogFormat.XML)
        {
            Append = true;
            Writer = writer;
        }

        /// <summary>
        ///     Prepares the Writer for writing eventz by writing a header containing the information
        ///     stored in SessionIdentification.
        /// </summary>
        /// <param name="sessionIdentification">Some data about the session.</param>
        public override void Start(SessionIdentification sessionIdentification)
        {
            if (!Append)
            {
                Writer.WriteStartDocument();
            }
            Writer.WriteStartElement(XmlElements.Log.TAG); //TODO decide on exact XML structure

            Writer.WriteStartElement(XmlElements.Log.Meta.TAG);
            foreach (var entry in sessionIdentification.GetMetaInfo())
            {
                Writer.WriteStartElement(XmlElements.Log.Meta.Entry.TAG);
                Writer.WriteElementString(XmlElements.Log.Meta.Entry.Key.TAG, entry.Key);
                Writer.WriteElementString(XmlElements.Log.Meta.Entry.Value.TAG, entry.Value ?? "");
                Writer.WriteEndElement(); // </entry>
            }

            Writer.WriteEndElement(); // </meta>
            Writer.WriteStartElement(XmlElements.Log.Session.TAG);

            foreach (var entry in sessionIdentification.GetSessionInfo())
            {
                Writer.WriteStartElement(XmlElements.Log.Session.Entry.TAG);
                Writer.WriteElementString(XmlElements.Log.Session.Entry.Key.TAG, entry.Key);
                Writer.WriteElementString(XmlElements.Log.Session.Entry.Value.TAG, entry.Value);
                Writer.WriteEndElement(); // </entry>
            }

            Writer.WriteEndElement(); // </session>

            Writer.Flush();
        }

        protected override object GetWriter()
        {
            return Writer;
        }

        /// <summary>
        ///     Flushes and closes the Writer, after this call, the object is disposed, if and only if
        ///     the constructor was not handed the writer to be used for writing the xml file.
        ///     If this class got passed a writer from the outside, it is not disposed, and the
        ///     writer is not closed.
        /// </summary>
        public override void Stop()
        {
            if (!Append)
            {
                Writer.WriteEndDocument();
                base.Stop();
            }
            else
            {
                Writer.WriteEndElement();
                Writer.Flush();
            }
        }

        /// <summary>
        ///     Opens an event by writing a header.
        ///     This method is called before each part of it is written (using the EventPartWriters).
        /// </summary>
        /// <param name="even">The event where to write the header for.</param>
        protected override void OpenEvent(Event even)
        {
            if (Writer.WriteState == WriteState.Error)
            {
                //TODO do something to fix the error
                Writer.Flush();
            }
            Writer.WriteStartElement(XmlElements.Log.Events.Event.TAG);

            foreach (var attribute in even.Properties)
            {
                Writer.WriteAttributeString(attribute.Key, attribute.Value);
            }

            foreach (var label in even.Labels)
            {
                Writer.WriteStartElement("label");
                Writer.WriteAttributeString("key", label.Key);
                Writer.WriteString(label.Value);
                Writer.WriteEndElement();
            }
        }

        /// <summary>
        ///     Closes the last opened event by writing a footer.
        ///     This method is called after all parts of the current event is written (using the EventPartWriters).
        /// </summary>
        protected override void CloseEvent()
        {
            Writer.WriteEndElement();

            // Flush in order to anticipate sudden crashes:
            // It's better to have a partial (non closed) xml than none at all!
           // Writer.Flush();
        }

        /// <summary>
        ///     Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                Writer.Flush();
                Writer.Close();
            }
            Disposed = true;
        }

        #region Fields

        /// <summary>
        ///     The writer used for writing to the output stream.
        /// </summary>
        private readonly XmlWriter Writer;

        /// <summary>
        ///     Boolean to see whether we are writing a new XML
        ///     file or appending to an existing one.
        /// </summary>
        private readonly bool Append;

        #endregion
    }
}