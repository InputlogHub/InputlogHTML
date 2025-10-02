using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.Util.KeyConversion;
using InputLog.Core.IO.Xml;
using System;
using InputLog.Core.IO.Legacy_Xml;

namespace InputLog.Core.IO.LegacyXML {

    /// <summary>
    /// EventReader that can read the old (legacy) InputLog extended XML format.
    /// </summary>
    public class LegacyXMLEventReader : AbstractEventReader {
        #region Fields
        //Used tags, attributes and values.

        // TAGS
        private const string SESSION_INFO_TAG = "SessionInfo";
        private const string LOCAL_TIME_TAG = "LocalTime";
        private const string EPOCH_TIME_TAG = "EpochTime";

        private const string EVENT_TAG = "Event";
        private const string OUTPUT_TAG = "output";
        private const string START_TIME_TAG = "startTime";
        private const string END_TIME_TAG = "endTime";
        private const string X_TAG = "x";
        private const string Y_TAG = "y";

        // ATTRIBUTES
        private const string WM_ATTRIBUTE = "wm";

        // TAG INNERTEXT
        private const string MOUSE_OUTPUT_MOVEMENT = "Movement";
        private const string MOUSE_OUTPUT_LM = "Left Button";
        private const string MOUSE_OUTPUT_RM = "Right Button";

        // WM (writing modes)
        private const string WM_KEYBOARD = "1";
        private const string WM_MOUSE = "2";
        private const string WM_DNS = "3"; // Dragon Naturally Speaking
        private const string WM_EYEWRITE = "4";
        private const string WM_FOCUSCHANGE = "5";

        // Stream to read eventz from.
        private Stream stream;

        // XML Document used to parse the XML from the stream.
        private XmlDocument xmlDoc;

        private bool Disposed = false;

        #endregion

        /// <summary>
        /// Constructs a new LegacyXMLEventReader.
        /// </summary>
        /// <param name="stream">Stream to read events from (=the EventStream).</param>
        public LegacyXMLEventReader(Stream stream)
            : base(LogFormat.LEGACY_XML.ToString()) {
            this.stream = stream;
            this.xmlDoc = new XmlDocument();
            xmlDoc.Load(this.stream);
        }

        /// <summary>
        /// Reads eventz from the stream.
        /// </summary>
        /// <returns>A list of Events read from the stream.</returns>
        public override List<Event> ReadEvents() {
            List<Event> events = new List<Event>();

            XmlNodeList xmlEvents = xmlDoc.GetElementsByTagName(EVENT_TAG);

            foreach (XmlNode xmlEvent in xmlEvents) {
                if (xmlEvent is XmlElement) {
                    XmlElement xmlEventElement = (XmlElement)xmlEvent;
                    Event even = new Event();
                    // every event has a start and end time
                    ulong startTime = ulong.Parse(xmlEventElement[START_TIME_TAG].InnerText);
                    ulong endTime = ulong.Parse(xmlEventElement[END_TIME_TAG].InnerText);

                    // process different LinearAnalysisType of events
                    string wm = xmlEventElement.GetAttribute(WM_ATTRIBUTE);
                    switch (wm) {
                        case WM_KEYBOARD: {
                                even.Properties["type"] = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.KEYBOARD;
                                even.Parts.Add(LegacyUtils.CreateKeyPress(xmlEventElement[OUTPUT_TAG].InnerText, startTime, endTime));
                                break;
                            }
                        case WM_MOUSE: {
                                even.Properties["type"] = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.MOUSE;
                                string output = xmlEventElement[OUTPUT_TAG].InnerText;
                                int x = int.Parse(xmlEventElement[X_TAG].InnerText);
                                int y = int.Parse(xmlEventElement[Y_TAG].InnerText);

                                // process different LinearAnalysisType of mouse eventz
                                switch (output) {
                                    case MOUSE_OUTPUT_LM: {
                                            even.Parts.Add(new Click(x, y, Hooks.Mouse.MouseMessages.WM_LBUTTONDOWN, startTime, endTime));
                                            break;
                                        }
                                    case MOUSE_OUTPUT_RM: {
                                            even.Parts.Add(new Click(x, y, Hooks.Mouse.MouseMessages.WM_RBUTTONDOWN, startTime, endTime));
                                            break;
                                        }
                                    case MOUSE_OUTPUT_MOVEMENT: {
                                            even.Parts.Add(new MouseMovement(x, y, startTime, endTime));
                                            break;
                                        }
                                }
                                break;
                            }
                        case WM_FOCUSCHANGE: {
                                even.Properties["type"] = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.FOCUSCHANGE;
                                even.Parts.Add(new FocusChange(xmlEventElement[OUTPUT_TAG].InnerText, startTime, endTime));
                                break;
                            }
                        case WM_EYEWRITE: {
                                //TODO not yet supported
                                break;
                            }
                        case WM_DNS: {
                                //TODO not yet supported
                                break;
                            }
                    }
                    events.Add(even);
                }
            }

            return events;
        }

        /// <summary>
        /// Reads the header of the log from the stream.
        /// </summary>
        /// <returns>A Dictionary containing session identification info.</returns>
        public override SessionIdentification ReadHeader() {
            SessionIdentification sessionIdentification = new SessionIdentification();

            foreach (XmlNode sessionInfo in xmlDoc.GetElementsByTagName(SESSION_INFO_TAG)) {
                foreach (XmlNode child in sessionInfo.ChildNodes) {
                    if (child.NodeType == XmlNodeType.Element) {
                        // use SessionIdentification[key] = value, instead of SessionIdentification.Add() in 
                        // order to avoid problems with duplicate keys in the Dictionary (not allowed by the Add() method).

                        if (child.Name == LOCAL_TIME_TAG) {
                            sessionIdentification.SetCreationDate(child.InnerText);
                        }
                        else if (child.Name == EPOCH_TIME_TAG && !sessionIdentification.HasCreationDate()) {
                            // fallback to epochTime if no local time is given.
                            // FUTURE test this properly + convert epoch to timestring.
                            sessionIdentification.SetCreationDate(child.InnerText);
                        }
                        else {
                            sessionIdentification.AddSessionInfo(child.Name, child.InnerText);
                        }


                    }
                }
            }

            return sessionIdentification;
        }

        /// <summary>
        /// Reads the footer of the log from the stream.
        /// </summary>
        public override void ReadFooter() {
            // there is no footer in the legacy xml
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected void Dispose(bool disposing) {
            if (!this.Disposed) {
                if (disposing) {
                    this.stream.Close();
                }
                Disposed = true;
            }
        }
    }
}