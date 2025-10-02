using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.IO.Xml.Input
{
    /// <summary>
    /// EventPartReader that can read Windows-parts from XML.
    /// </summary>
    public class XmlWinLogReader
    {
        /// <summary>
        /// Reads a Windows KeyboardEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created KeyBoardEventPart.</returns>
        public static IKeyboardEventPart ReadKeyboard(XmlElement xmlElement)
        {
            ulong startTime = 0;
            ulong endTime = 0;
            string rawValue = "";
            string key = "";

            var element = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.StartTime.TAG];
            if (element != null) startTime = ulong.Parse(element.InnerText);
            var xmlElement1 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.EndTime.TAG];
            if (xmlElement1 != null) endTime = ulong.Parse(xmlElement1.InnerText);
            var element1 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.Value.TAG];
            if (element1 != null) rawValue = element1.InnerText;
            var value = StringUtils.Unescape(rawValue);
            if (value.Equals(@"\w")) value = " ";
            if (value.Equals(@"BACKSPACE")) value = new String((char)8, 1);
            var xmlElement2 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.Key.TAG];
            if (xmlElement2 != null) key = xmlElement2.InnerText;

            // HACK by JR: newlines aren't read by xmlreader, so we add them manually as the value
            if (key.Equals("VK_RETURN") || value.Equals(@"NEWLINE"))
            {
                value = "\r\n";
            }
            var keyboardState = new List<KeysEx>();
            XmlNodeList xmlKeyboardKeys = null;
            var element2 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.KeyboardState.TAG];
            if (element2 != null) xmlKeyboardKeys = 
                element2.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Keyboard.WinLog.KeyboardState.Key.TAG);
            if (xmlKeyboardKeys != null)
                keyboardState.AddRange(from XmlNode xmlKeyboardKey in xmlKeyboardKeys 
                                       select (KeysEx) Enum.Parse(typeof (KeysEx), xmlKeyboardKey.InnerText));
            try
            {
                return new KeyPress((KeysEx)Enum.Parse(typeof(KeysEx), key), value, startTime, endTime, keyboardState);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads a Windows MouseEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created MouseEventPart.</returns>
        public static IMouseEventPart ReadMouse(XmlElement xmlElement)
        {
            int x = 0;
            int y = 0;
            ulong startTime = 0;
            ulong endTime = 0;

            var element = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.X.TAG];
            if (element != null) x = int.Parse(element.InnerText);
            var xmlElement1 = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.Y.TAG];
            if (xmlElement1 != null) y = int.Parse(xmlElement1.InnerText);
            var element1 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.StartTime.TAG];
            if (element1 != null) startTime = ulong.Parse(element1.InnerText);
            var xmlElement2 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.EndTime.TAG];
            if (xmlElement2 != null) endTime = ulong.Parse(xmlElement2.InnerText);

            IMouseEventPart mouseEventPart = null;

            var element3 = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.Type.TAG];
            if (element3 != null) switch (element3.InnerText)
                {
                    case XmlElements.Log.Events.Event.Part.Mouse.WinLog.Type.VALUES.MOVEMENT:
                        {
                            mouseEventPart = new MouseMovement(x, y, startTime, endTime);
                            break;
                        }
                    case XmlElements.Log.Events.Event.Part.Mouse.WinLog.Type.VALUES.CLICK:
                        {
                            string mouseButton = "";
                            var element2 = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.Click.Button.TAG];
                            if (element2 != null) mouseButton= element2.InnerText;
                            mouseEventPart = new Click(x, y, (Buttons)Enum.Parse(typeof(Buttons),
                                                                                 mouseButton.ToUpper()), startTime, endTime);
                            break;
                        }
                    case XmlElements.Log.Events.Event.Part.Mouse.WinLog.Type.VALUES.SCROLL:
                        {
                            int delta = 0;

                            var element2 = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.Scroll.Delta.TAG];
                            if (element2 != null) delta = int.Parse(element2.InnerText);
                            var xmlElement3 = xmlElement[XmlElements.Log.Events.Event.Part.Mouse.WinLog.Scroll.Orientation.TAG];
                            if (xmlElement3 != null)
                            {
                                var orientation = (Scroll.OrientationType)Enum.Parse(typeof(Scroll.OrientationType),
                                                                                     xmlElement3.InnerText.ToUpper());
                                mouseEventPart = new Scroll(x, y, delta, orientation, startTime, endTime);
                            }
                            break;
                        }
                }
            return mouseEventPart;
        }

        /// <summary>
        /// Reads an Windows FocusChangeEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created FocusChangeEventPart.</returns>
        public static IFocusChangeEventPart ReadFocus(XmlElement xmlElement)
        {
            var title = "";
            ulong startTime = 0;
            ulong endTime = 0;

            var element = xmlElement[XmlElements.Log.Events.Event.Part.FocusChange.WinLog.Title.TAG];
            if (element != null) title = element.InnerText;

            var xmlElement1 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.StartTime.TAG];
            if (xmlElement1 != null) startTime = ulong.Parse(xmlElement1.InnerText);

            var element1 = xmlElement[XmlElements.Log.Events.Event.Part.Keyboard.WinLog.EndTime.TAG];
            if (element1 != null) endTime = ulong.Parse(element1.InnerText);

            return new FocusChange(title, startTime, endTime);
         }

        /// <summary>
        /// Reads an WinLog QuestionsEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created Questions part.</returns>
        public static Questions ReadQuestions(XmlElement xmlElement)
        {
            var questions = new Questions();

            questions.Handedness = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Handedness.TAG].IsNullOrEmpty()
                    ? "0.0"
                    : (xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Handedness.TAG].InnerText);
            questions.Computer = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Computer.TAG].InnerText;
            questions.Keyboard = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Keyboard.TAG].InnerText;
            questions.Browser = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Browser.TAG].InnerText;
            questions.Language = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Language.TAG].InnerText;
            questions.Disorder = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Disorder.TAG].IsNullOrEmpty()
                    ? false
                    : bool.Parse(xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Disorder.TAG].InnerText);
            questions.Education = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Education.TAG].InnerText;
            questions.Disorder = xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Repetition.TAG].IsNullOrEmpty()
                    ? false
                    : bool.Parse(xmlElement[XmlElements.Log.Events.Event.Part.Questions.WinLog.Repetition.TAG].InnerText);

            return questions;
        }
    }
}