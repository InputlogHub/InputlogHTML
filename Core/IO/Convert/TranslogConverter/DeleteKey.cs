using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Converting a Translog delete event.
    /// </summary>
    public class DeleteKey : ILConvertible
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
        public DeleteKey(XPathNavigator node, TranslogReader.DocParam param)
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

            // Conversion into new Inputlog events.
            Event thisEvent;

            if (value.Equals("[Back]"))
            {
                // Simple one char deletion.
                if (text.Length <= 1)
                {
                    thisEvent = new Event();
                    thisEvent.Properties["type"] = "keyboard";
                    thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                    thisEvent.Parts.Add(new Keypress(Parameter.Position--, Parameter.DocLength--, true));
                    var keyPress = new KeyPress
                    {
                        StartTime = time,
                        EndTime = time,
                        Value = "\u0008",
                        Key = KeysEx.VK_BACK
                    };
                    thisEvent.Parts.Add(keyPress);
                    EventList.Add(thisEvent);
                }
                // Deletion of a selection with 'back'
                if (text.Length > 1)
                {
                    thisEvent = new Event();
                    thisEvent.Properties["type"] = "keyboard";
                    thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                    thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));
                    var keyPress = new KeyPress
                    {
                        StartTime = time,
                        EndTime = time,
                        Value = "\u0008",
                        Key = KeysEx.VK_BACK
                    };
                    thisEvent.Parts.Add(keyPress);
                    EventList.Add(thisEvent);

                    thisEvent = new Event();
                    thisEvent.Properties["type"] = "replacement";
                    thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                    thisEvent.Parts.Add(new Replacement(cursor, cursor + text.Length, ""));
                    EventList.Add(thisEvent);

					Parameter.Position = cursor;
					Parameter.DocLength -= text.Length;
                }
            }

            if (value.Equals("[Delete]"))
            {
                thisEvent = new Event();
                thisEvent.Properties["type"] = "keyboard";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Keypress(Parameter.Position = cursor, Parameter.DocLength, false));
                var keyPress = new KeyPress
                {
                        StartTime = time,
                        EndTime = time,
                        Value = "",
                        Key = KeysEx.VK_DELETE
                };
                thisEvent.Parts.Add(keyPress);
                EventList.Add(thisEvent);

				Parameter.DocLength -= text.Length;

                thisEvent = new Event();
                thisEvent.Properties["type"] = "replacement";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new Replacement(cursor, cursor + text.Length, ""));
                EventList.Add(thisEvent);

                thisEvent = new Event();
                thisEvent.Properties["type"] = "selection";
                thisEvent.Properties["id"] = (Parameter.Id++).ToString();
                thisEvent.Parts.Add(new SelectionChange(cursor, cursor));
                EventList.Add(thisEvent);
            }

            return EventList;
        }
    }
}