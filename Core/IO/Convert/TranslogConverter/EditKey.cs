using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Converting a Translog edit event.
    /// </summary>
    public class EditKey : ILConvertible
    {
        #region Fields

        private readonly XPathNavigator Node;
        private readonly List<Event> EventList;
        private readonly TranslogReader.DocParam Parameter;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">A Translog xml element.</param>
        /// <param name="param">Document parameters: length, position, event id.</param>
        public EditKey(XPathNavigator node, TranslogReader.DocParam param)
        {
            Node = node;
            Parameter = param;
            EventList = new List<Event>();
        }

        /// <summary>
        /// Extracting Translog data element and conversion into an equivalent Inputlog event.
        /// </summary>
        /// <returns>An Inputlog event.</returns>
        public IEnumerable<Event> ConvertToEvent()
        {
            int block = 0;

            // Reading the Translog node content
            ulong time = System.Convert.ToUInt64(Node.GetAttribute("Time", ""));
            int cursor = System.Convert.ToInt32(Node.GetAttribute("Cursor", ""));
            string text = Node.GetAttribute("Text", "");
            
            if (text.Length > 0)
            {
                var tmpBlock = Node.GetAttribute("Block", "");
                block = tmpBlock.Equals(string.Empty) ? text.Length : System.Convert.ToInt32(tmpBlock);
            }
            string value = Node.GetAttribute("Value", "");
            string paste = Node.GetAttribute("Paste", "");
            if (paste.Length > 0)
            {
                var tmpBlock = Node.GetAttribute("Block", "");
                block = tmpBlock.Equals(string.Empty) ? paste.Length : System.Convert.ToInt32(tmpBlock);
            }

            // Conversion into new Inputlog events.
            Event thisEvent;

            if (value.Equals("[Ctrl+C]") || text.Length > 0)
            {
				Parameter.Position = cursor;

                thisEvent = new Event();
                thisEvent.Properties["type"] = "keyboard";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));
                var keyPress = new KeyPress
                {
                        StartTime = time, 
                        EndTime = time, 
                        Key = KeysEx.VK_C
                };
                keyPress.KeyboardState.Add(KeysEx.VK_LCONTROL);
                thisEvent.Parts.Add(keyPress);
                EventList.Add(thisEvent);
            }
            if (value.Equals("[Ctrl+V]") || paste.Length > 0)
            {
				Parameter.Position = cursor;

                thisEvent = new Event();
                thisEvent.Properties["type"] = "keyboard";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));
                var keyPress = new KeyPress
                {
                        StartTime = time, 
                        EndTime = time, 
                        Key = KeysEx.VK_V
                };
                keyPress.KeyboardState.Add(KeysEx.VK_LCONTROL);
                thisEvent.Parts.Add(keyPress);
                EventList.Add(thisEvent);

                thisEvent = new Event();
                thisEvent.Properties["type"] = "insert";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
				thisEvent.Parts.Add(new Insert(cursor, paste, ""));
                EventList.Add(thisEvent);
				
				Parameter.DocLength += block;
				Parameter.Position = cursor + block;

                thisEvent = new Event();
                thisEvent.Properties["type"] = "selection";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new SelectionChange(Parameter.Position, Parameter.Position));
                EventList.Add(thisEvent);
            }
            if (value.Equals("[Ctrl+X]"))
            {             
                Parameter.Position = cursor;

                thisEvent = new Event();
                thisEvent.Properties["type"] = "keyboard";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));
                var keyPress = new KeyPress
                {
                        StartTime = time, 
                        EndTime = time, 
                        Key = KeysEx.VK_X
                };
                keyPress.KeyboardState.Add(KeysEx.VK_LCONTROL);
                thisEvent.Parts.Add(keyPress);
                EventList.Add(thisEvent);

                if (text.Length > 0)
                {
                    Parameter.DocLength -= block;

                    thisEvent = new Event();
                    thisEvent.Properties["type"] = "replacement";
                    thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                    thisEvent.Parts.Add(new Replacement(cursor, cursor + block, ""));
                    EventList.Add(thisEvent);

                    thisEvent = new Event();
                    thisEvent.Properties["type"] = "selection";
                    thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                    thisEvent.Parts.Add(new SelectionChange(Parameter.Position, Parameter.Position));
                    EventList.Add(thisEvent);
                }
            }

            return EventList;
        }
    }
}