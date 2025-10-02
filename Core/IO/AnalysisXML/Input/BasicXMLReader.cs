using System.Collections.Generic;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
using System.Xml;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    using XML;

    public abstract class BasicXMLReader
    {
        public delegate Header ReadHeaderMethod(XmlDocument xmlDoc, string timeStamp);

        public delegate List<ExtraInfo> ReadExtraInfoMethod(XmlDocument xmlDoc);

        public delegate List<Event> ReadEventsMethod(XmlDocument xmlDoc);

        public delegate List<Module> ReadModulesMethod(XmlDocument xmlDoc);

        protected static Header Header(XmlDocument xmlDoc, string timeStamp)
        {
            var header = new Header();
            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session == null) return header;
            header.Title = "Session Information";
            header.Properties.Add("LogCreationDate", timeStamp);
            XmlElement sessionId = session[AnalysisXML.Session.SessionIdentification.TAG];
            if (sessionId == null) return header;
            foreach (XmlElement entry in sessionId.GetElementsByTagName(AnalysisXML.Session.SessionIdentification.Entry.TAG))
            {
                KeyValuePair<string, string> pair = Entry(entry);
                header.Properties[pair.Key] = pair.Value;
            }
            return header;
        }

        protected static List<ExtraInfo> ExtraInfo(XmlDocument xmlDoc)
        {
            var extras = new List<ExtraInfo>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session == null) return extras;
            foreach (XmlElement xmlExtra in session.GetElementsByTagName(AnalysisXML.Session.ExtraInfo.TAG))
            {
                var extra = new ExtraInfo{Title = xmlExtra.GetAttribute(AnalysisXML.Session.ExtraInfo.ATTRIBUTES.TITLE)};
                foreach (XmlElement xmlEntry in xmlExtra.GetElementsByTagName(AnalysisXML.Session.ExtraInfo.Entry.TAG))
                {
                    var entry = Entry(xmlEntry);
                    extra.Properties[entry.Key] = entry.Value;
                }
                extras.Add(extra);
            }
            return extras;
        }

        protected static List<Event> Events(XmlDocument xmlDoc)
        {
            var events = new List<Event>();

            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session == null) return events;
            foreach (XmlElement xmlEvent in session.GetElementsByTagName(AnalysisXML.Session.Event.TAG))
            {
                var ev = new Event();
                // Type and Id and Output
                XmlNodeList elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.Type.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.Type.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.Type.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.Id.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.Id.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.Id.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.Output.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.Output.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.Output.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.CharProduction.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.CharProduction.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.CharProduction.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.ActionTime.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.ActionTime.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.ActionTime.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.StartClock.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.StartClock.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.StartClock.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.EndClock.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.EndClock.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.EndClock.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.StartTime.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.StartTime.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.StartTime.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.EndTime.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.EndTime.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.EndTime.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.PauseLocation.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.PauseLocation.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.PauseLocation.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.PauseLocationFull.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.PauseLocationFull.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.PauseLocationFull.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.PauseTime.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.PauseTime.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.PauseTime.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.FixedNumberInterval.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.FixedNumberInterval.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.FixedNumberInterval.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.FixedSizeInterval.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.FixedSizeInterval.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.FixedSizeInterval.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.X.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.X.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.X.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.Y.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.Y.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.Y.TAG].InnerText;
                }

                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.Position.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.Position.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.Position.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.PositionFull.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.PositionFull.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.PositionFull.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.DocLength.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.DocLength.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.DocLength.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.DocLengthFull.TAG);
                if (elements.Count > 0)
                {
                    ev.Properties[AnalysisXML.Session.Event.DocLengthFull.TAG] =
                        xmlEvent[AnalysisXML.Session.Event.DocLengthFull.TAG].InnerText;
                }
                elements = xmlEvent.GetElementsByTagName(AnalysisXML.Session.Event.RevisionInfo.TAG);
                if (elements.Count > 0)
                {
                    var infoElement = xmlEvent[AnalysisXML.Session.Event.RevisionInfo.TAG];
                    string nrText = infoElement[AnalysisXML.Session.Event.RevisionInfo.NR_TAG].InnerText;
                    ev.Properties[AnalysisXML.Session.Event.RevisionInfo.NR_TAG] = "'" + nrText + "'";

                    string posText = infoElement[AnalysisXML.Session.Event.RevisionInfo.POS_TAG].InnerText;
                    ev.Properties[AnalysisXML.Session.Event.RevisionInfo.POS_TAG] = "'" + posText + "'";

                    ev.Properties[AnalysisXML.Session.Event.RevisionInfo.TYPE_TAG] =
                        infoElement[AnalysisXML.Session.Event.RevisionInfo.TYPE_TAG].InnerText;
                } 
                events.Add(ev);
            }
            return events;
        }

        protected static List<Module> Modules(XmlDocument xmlDoc)
        {
            var modules = new List<Module>();
            XmlElement session = xmlDoc[AnalysisXML.Session.TAG];
            if (session == null) return modules;
            foreach (XmlElement xmlModule in session.GetElementsByTagName(AnalysisXML.Session.Module.TAG))
            {
                var module = new Module {Title = xmlModule.GetAttribute(AnalysisXML.Session.Module.ATTRIBUTES.NAME)};
                foreach (var o in xmlModule.Attributes)
                {
                    XmlAttribute attr = (XmlAttribute)o;
                    module.Attributes[attr.Name] = attr.Value;
                }
                foreach (XmlElement xmlBlock in xmlModule.GetElementsByTagName(AnalysisXML.Session.Module.Block.TAG))
                {
                    var block = new Block{Title = xmlBlock.GetAttribute(AnalysisXML.Session.Module.Block.ATTRIBUTES.NAME),
                                    Value = xmlBlock.GetAttribute(AnalysisXML.Session.Module.Block.ATTRIBUTES.VALUE)};

                    foreach (var o in xmlBlock.Attributes)
                    {
                        XmlAttribute attr = (XmlAttribute)o;
                        block.Attributes[attr.Name] = attr.Value;
                    }

                    string prefix = "";
                    foreach (XmlElement xmlElement in xmlBlock.GetElementsByTagName(AnalysisXML.Session.Module.Block.Element.TAG))
                    {
                        KeyValuePair<string, string> element = Element(xmlElement);

                        // The Miscellaneous Pauses block contains an extra level not defined as an XML-element.
                        // Manually added prefixes prevent block properties with the same key to be overwritten.                     
                        if (block.Title.Contains("Miscellaneous"))
                        {
                            switch (element.Key)
                            {
                                case "INITIAL PAUSES":
                                    prefix = "IN_";
                                    break;
                                case "END PAUSES":
                                    prefix = "EN_";
                                    break;
                                case "COMBINATION KEY PAUSES":
                                    prefix = "CO_";
                                    break;
                                case "REVISION PAUSES":
                                    prefix = "RE_";
                                    break;
                                case "CHANGE PAUSES":
                                    prefix = "CH_";
                                    break;
                                case "UNKNOWN & UNDETERMINED PAUSES":
                                    prefix = "UN_";
                                    break;
                            }
                        }

                        var propertyKey = prefix + element.Key;
                        if (block.Properties.ContainsKey(propertyKey))
                        {
                            element = new KeyValuePair<string, string>(propertyKey + ":", element.Value);
                        }
                        block.Properties[propertyKey] = element.Value;
                    }
                    module.Children.Add(block);
                }
                modules.Add(module);
            }
            return modules;
        }

        //---------------------------------------------------------------------
        // Private helper functions
        //

        /// <summary>
        /// This method reads an entry of the SessionIdentification part of an AnalysisXML file.
        /// Most likely it will be able to read the entries in other parts just as well, as they 
        /// are currently (19-02-2012) identical. But if this changes in the future, this method
        /// should not be used for reading changed entries.
        /// </summary>
        /// <param name="entry">XMLElement of an entry, that we wish to read.</param>
        /// <returns>A keyvalue pair where the Key is the value the name attribute, and the Value is
        /// the value of the Value attribute of an entry.</returns>
        private static KeyValuePair<string, string> Entry(XmlElement entry)
        {
            string name = entry.GetAttribute(AnalysisXML.Session.SessionIdentification.Entry.ATTRIBUTES.NAME);
            string value = entry.GetAttribute(AnalysisXML.Session.SessionIdentification.Entry.ATTRIBUTES.VALUE);
            var pair = new KeyValuePair<string, string>(name, value);
            return pair;
        }

        /// <summary>
        /// This method reads an element of the Block part of an AnalysisXML file.
        /// </summary>
        /// <param name="entry">XMLElement of an element, that we wish to read.</param>
        /// <returns>A keyvalue pair where the Key is the value the name attribute, and the Value is
        /// the value of the Value attribute of an element.</returns>
        private static KeyValuePair<string, string> Element(XmlElement entry)
        {
            string name = entry.GetAttribute(AnalysisXML.Session.Module.Block.Element.ATTRIBUTES.NAME);
            string value = entry.GetAttribute(AnalysisXML.Session.Module.Block.Element.ATTRIBUTES.VALUE);
            var pair = new KeyValuePair<string, string>(name, value);
            return pair;
        }

        /// <summary>
        /// Null functions: 
        /// These functions are stubs for functionality that is not supported
        /// by a certain type of AnalysisXML file.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>A delegate that allways returns null.</returns>
        protected static Header NullHeader(XmlDocument xmlDoc, string timestamp)
        {
            return new Header();
        }

        protected static List<ExtraInfo> NullExtraInfo(XmlDocument xmlDoc)
        {
            return new List<ExtraInfo>();
        }

        protected static List<Event> NullEvents(XmlDocument xmlDoc)
        {
            return new List<Event>();
        }

        protected static List<Module> NullModules(XmlDocument xmlDoc)
        {
            return new List<Module>();
        }

        /// <summary>
        /// Reading methods
        /// </summary>
        /// <returns>The correct delegate functions in the
        /// subclasses of BasicXMLReader.</returns>
        public abstract ReadHeaderMethod ReadHeader();
        public abstract ReadExtraInfoMethod ReadExtraInfo();
        public abstract ReadEventsMethod ReadEvents();
        public abstract ReadModulesMethod ReadModules();
    }
}