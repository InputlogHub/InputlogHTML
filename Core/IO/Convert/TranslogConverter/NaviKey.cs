using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Converting a Translog navigation key event.
    /// </summary>
    public class NaviKey : ILConvertible
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
        public NaviKey(XPathNavigator node, TranslogReader.DocParam param)
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
            // Reading the Translog node content
            ulong time = System.Convert.ToUInt64(Node.GetAttribute("Time", ""));
			int cursor = System.Convert.ToInt32(Node.GetAttribute("Cursor", "")); 
            string value = Node.GetAttribute("Value", "");
            string text = Node.GetAttribute("Text", "");
            int block = 0;
            if (text.Length > 0)
            {
                block = System.Convert.ToInt32(Node.GetAttribute("Block", ""));
                if (block == 0)
                {
                    block = text.Length;
                }
            }
 
            // Conversion into new Inputlog events.
            var ctrlKey = KeysEx.NONE;
            var naviKey = KeysEx.NONE;
            var thisEvent = new Event();
            thisEvent.Properties["type"] = "keyboard";
            thisEvent.Properties["id"] = (Parameter.Id++).ToString();
            thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));

            // Splitting the composed navigation keys in a control part (SHIFT/CTRL) and a navigation
            // part (LEFT/RIGHT/HOME/END..)
            value = value.Replace("[", "");
            value = value.Replace("]", "");
            string[] words = value.Split('+');
            foreach (var word in words)
            {
                switch (word)
                {
                    //NAVIKEYS
                    case "Left":
                        naviKey = KeysEx.VK_LEFT;
                        break;
                    case "Right":
                        naviKey = KeysEx.VK_RIGHT;
                        break;
                    case "Up":
                        naviKey = KeysEx.VK_UP;
                        break;
                    case "Down":
                        naviKey = KeysEx.VK_DOWN;
                        break;
                    case "Home":
                        naviKey = KeysEx.VK_HOME;
                        break;
                    case "End":
                        naviKey = KeysEx.VK_END;
                        break;
                    case "PageUp":
                        naviKey = KeysEx.VK_PRIOR;
                        break;
                    case "PageDown":
                        naviKey = KeysEx.VK_NEXT;
                        break;
                    // CTRLKEYS
                    case "Shift":
                        ctrlKey = KeysEx.VK_SHIFT;
                        break;
                    case "Ctrl":
                        ctrlKey = KeysEx.VK_CONTROL;
                        break;
                    case "Rshift":
                        ctrlKey = KeysEx.VK_RSHIFT;
                        break;
                    case "Lshift":
                        ctrlKey = KeysEx.VK_LSHIFT;
                        break;
                    case "Rctrl":
                        ctrlKey = KeysEx.VK_RCONTROL;
                        break;
                    case "Lctrl":
                        ctrlKey = KeysEx.VK_LCONTROL;
                        break;
                }
            }

            var keyPress = new KeyPress
            {
                    Key = naviKey, 
                    StartTime = time, 
                    EndTime = time,
            };
            keyPress.KeyboardState.Add(ctrlKey);
            thisEvent.Parts.Add(keyPress);
            EventList.Add(thisEvent);

			Parameter.Position = cursor;

            if (text.Length > 0)
            {
                thisEvent = new Event();
                thisEvent.Properties["type"] = "replacement";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Replacement(cursor, cursor + block, text));
				EventList.Add(thisEvent);

                thisEvent = new Event();
                thisEvent.Properties["type"] = "selection";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new SelectionChange(cursor, cursor + block));
                EventList.Add(thisEvent);
            }

            return EventList;
        }
    }
}