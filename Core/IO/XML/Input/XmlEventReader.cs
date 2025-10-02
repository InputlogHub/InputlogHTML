using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.Util;

namespace InputLog.Core.IO.Xml.Input
{

    /// <summary>
    /// EventReader that can read the new InputLog XML format.
    /// </summary>
    public class XmlEventReader : AbstractEventReader
    {
        // TODO: Validating reader / XML schema?
        /// <summary>
        /// Stream containing the log (header + events + footer).
        /// </summary>
        private readonly Stream _stream;

        private readonly XmlReader _reader;
        /// <summary>
        /// Constructs an XmlEventReader.
        /// </summary>
        /// <param name="stream">Stream to read events from.</param>
        public XmlEventReader(Stream stream) : base("Xml")
        {
            _stream = stream;
            _reader = XmlReader.Create(new XmlSanitizingStream(stream),
                new XmlReaderSettings { CheckCharacters = false, IgnoreWhitespace = false });
        }

        /// <summary>
        /// Reads events from the stream.
        /// </summary>
        /// <returns>A list of Events read from the stream.</returns>
        public override List<Event> ReadEvents()
        {
            var events = new List<Event>();
            _reader.ReadToFollowing("event");

            while (_reader.IsStartElement("event"))
            {
                bool isFaultyEvent = false;

                try
                {
                    var e = new Event();

                    while (_reader.MoveToNextAttribute())
                        e.Properties[_reader.Name] = _reader.Value;

                    _reader.MoveToElement();
                    _reader.ReadStartElement("event");

                    while (_reader.IsStartElement("part") || _reader.IsStartElement("label"))
                    {
                        if (_reader.IsStartElement("label"))
				        {
                            string labelKey = _reader.GetAttribute("key");
                            string labelValue = _reader.ReadElementString("label");
					        e.Labels.Add(labelValue, labelKey);
				        }

                        if (!_reader.IsStartElement("part")) continue;

                        string partType = _reader.GetAttribute(XmlElements.Log.Events.Event.Part.ATTRIBUTES.Type.KEY);
                        IEventPart eventPart = ReadEventPart(partType, e.Type);
                        if (eventPart != null)
                        {
                            e.Parts.Add(eventPart);
                        }
                        else
                        {
                            // I gracefully ignore the fact that authorcomment is labeled 'faulty'. 
                            // I cannot see why. Perhaps you can!
                            if (!e.Type.Equals("authorcomment"))
                            {
                                // If the eventPart returned is null, that means the eventPart,
                                // could not be processed correctly. Let's log this as an exception
                                // and then skip the event.
                                isFaultyEvent = true;
                                MessageLogger.LogMessage(this,
                                    "Faulty event detected",
                                    "The event with id \"" + e.GetId() + "\" could not be processed correctly. " +
                                    "This event has been ignored, the file rest will continue to be processed.",
                                    Severity.ERROR
                                );
                            }
                        }

                        _reader.ReadEndElement(); // part;
                    }

                    _reader.ReadEndElement(); // event;
                    if (!isFaultyEvent)
                    {
                        events.Add(e);
                    }
                }
                catch (Exception e)
                {
                    MessageLogger.CatchException(this, e, Severity.WARNING);

                    while (!_reader.IsStartElement("event")
                        && !_reader.EOF && _reader.Depth > 1 && _reader.Read()) {}

                    try { _reader.ReadEndElement(); }
                    catch (Exception) {}
                }
            }
            return events;
        }

        public IEventPart ReadEventPart(string partType, string eventType)
        {
            // Read this eventpart subtree into XmlElement
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            XmlReader st = _reader.ReadSubtree();
            doc.Load(st);
            XmlElement elem = (XmlElement)doc.FirstChild;

            // Invoke appropriate reader method
            IEventPart res = base.ReadEventPart(partType, eventType, elem);

            // Make sure entire subtree has been processed
            while (st.Read()) { }
            st.Close();

            return res;
        }

        /// <summary>
        /// Reads the header of the log from the stream.
        /// </summary>
        /// <returns>Session identification info.</returns>
        public override SessionIdentification ReadHeader()
        {
            var sessionIdentification = new SessionIdentification();
            _reader.Read();
            _reader.ReadStartElement("log");
            sessionIdentification.ReadXml(_reader);
            return sessionIdentification;
        }

        public override void Close()
        {
            try
            {
                _stream.Close();
                _reader.Close();
            }
            catch (Exception) {}
        }
    }
}