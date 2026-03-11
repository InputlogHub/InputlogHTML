using System.Xml;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using FocusChangePart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.FocusChange.WinLog;
using KeyboardPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Keyboard.WinLog;
using MousePart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Mouse.WinLog;
using Part = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part;

namespace InputLog.Core.IO.Xml.Output
{

    public class XmlWinLogWriter
    {
        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteFocusChange(FocusChange e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog); // LinearAnalysisType="winlog"
            writer.WriteElementString(FocusChangePart.Title.TAG, e.WindowTitle.Trim()); // <title>
            writer.WriteElementString(KeyboardPart.StartTime.TAG, e.StartTime.ToString()); // <startTime>
            writer.WriteElementString(KeyboardPart.EndTime.TAG, e.EndTime.ToString()); // <endTime>

            writer.WriteEndElement(); // </core>
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteKeyPress(KeyPress e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog); // LinearAnalysisType="winlog"

            writer.WriteElementString(KeyboardPart.StartTime.TAG, e.StartTime.ToString()); // <startTime>
            writer.WriteElementString(KeyboardPart.EndTime.TAG, e.EndTime.ToString()); // <endTime>
            writer.WriteElementString(KeyboardPart.Key.TAG, e.Key.ToString());
            writer.WriteElementString(KeyboardPart.Value.TAG, StringUtils.Escape(e.Value)); // <value>
            writer.WriteStartElement(KeyboardPart.KeyboardState.TAG); // <keyboardstate
            foreach (KeysEx key in e.KeyboardState)
            {
                writer.WriteElementString(KeyboardPart.KeyboardState.Key.TAG, key.ToString()); // <key>
            }
            writer.WriteEndElement(); // </keyboardstate>

            writer.WriteEndElement(); // </part>
        }

        /// <summary>
        /// Writes out common mouse information that is shared between different mouse-eventz (all eventz derived from AbstractMouseEvent).
        /// </summary>
        /// <param name="e">The mouse-event for which to write out the common info.</param>
        /// <param name="writer"></param>
        private static void WriteCommonMouseInfo(AbstractMouseEvent e, XmlWriter writer)
        {
            writer.WriteElementString(MousePart.StartTime.TAG, e.StartTime.ToString()); // <startTime>
            writer.WriteElementString(MousePart.EndTime.TAG, e.EndTime.ToString()); // <endTime>
            writer.WriteElementString(MousePart.X.TAG, e.X.ToString()); // <x>
            writer.WriteElementString(MousePart.Y.TAG, e.Y.ToString()); // <y>
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteClick(Click e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog); // LinearAnalysisType="winlog"

            WriteCommonMouseInfo(e, writer);
            writer.WriteElementString(MousePart.Type.TAG, MousePart.Type.VALUES.CLICK);
            writer.WriteElementString(MousePart.Click.Button.TAG, e.Button.ToString());
            //writer.WriteStartElement(KeyboardPart.KeyboardState.TAG); // <keyboardstate
            //foreach (Keys key in e.KeyboardState) {
            //    writer.WriteElementString(KeyboardPart.KeyboardState.Key.TAG, key.ToString()); // <key>
            //}
            //writer.WriteEndElement(); // </keyboardstate>

            writer.WriteEndElement(); // </part>
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteMouseMovement(MouseMovement e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog); // LinearAnalysisType="winlog"

            WriteCommonMouseInfo(e, writer);
            writer.WriteElementString(MousePart.Type.TAG, MousePart.Type.VALUES.MOVEMENT);
            //writer.WriteStartElement(KeyboardPart.KeyboardState.TAG); // <keyboardstate
            //foreach (Keys key in e.KeyboardState) {
            //    writer.WriteElementString(KeyboardPart.KeyboardState.Key.TAG, key.ToString()); // <key>
            //}
            //writer.WriteEndElement(); // </keyboardstate>

            writer.WriteEndElement(); // </part>
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteScroll(Scroll e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog); // LinearAnalysisType="winlog"

            WriteCommonMouseInfo(e, writer);
            writer.WriteElementString(MousePart.Type.TAG, MousePart.Type.VALUES.SCROLL);
            writer.WriteElementString(MousePart.Scroll.Orientation.TAG, e.Orientation.ToString());
            writer.WriteElementString(MousePart.Scroll.Delta.TAG, e.Delta.ToString());
            //writer.WriteStartElement(KeyboardPart.KeyboardState.TAG); // <keyboardstate
            //foreach (Keys key in e.KeyboardState) {
            //    writer.WriteElementString(KeyboardPart.KeyboardState.Key.TAG, key.ToString()); // <key>
            //}
            //writer.WriteEndElement(); // </keyboardstate>

            writer.WriteEndElement(); // </part>
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteQuestions(Questions e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>        
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WinLog);

            writer.WriteElementString("handedness", e.Handedness);
            writer.WriteElementString("computer", e.Computer);
            writer.WriteElementString("keyboard", e.Keyboard);
            writer.WriteElementString("browser", e.Browser);
            writer.WriteElementString("language", e.Language);
            writer.WriteElementString("disorder", e.Disorder.ToString());
            writer.WriteElementString("education", e.Education);
            writer.WriteElementString("repetition", e.Repetition.ToString());

            writer.WriteEndElement(); // </part>
        }
    }
}