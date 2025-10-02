using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Converting a Translog return key event.
    /// </summary>
    public class ReturnKey : ILConvertible
    {
        #region Fields

        private readonly List<Event> EventList;
        private readonly XPathNavigator Node;
        private readonly TranslogReader.DocParam Parameter;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">A Translog xml element.</param>
        /// <param name="param">Document parameters: length, position, event id.</param>
        public ReturnKey(XPathNavigator node, TranslogReader.DocParam param)
        {
            Node = node;
            Parameter = param;
            EventList = new List<Event>();
        }

        /// <summary>
        /// Extracting Translog data element and conversion into an equivalent Inputlog event.
        /// </summary>
        /// <returns>List with return key events</returns>
        public IEnumerable<Event> ConvertToEvent()
        {
            // Reading the node content
            ulong time = System.Convert.ToUInt64(Node.GetAttribute("Time", ""));
			int cursor = System.Convert.ToInt32(Node.GetAttribute("Cursor", "")); 
			string text = Node.GetAttribute("Text", "");
            if (text.Length > 0)
			{
			    int block = System.Convert.ToInt32(Node.GetAttribute("Block", ""));
			    if (block == 0)
				{
					block = text.Length;
				}
			}

            // Conversion into new Inputlog events.
			if (text.Length > 0)
			{
				var thisEvent = new Event();
				thisEvent.Properties["type"] = "keyboard";
				thisEvent.Properties["id"] = (Parameter.Id++).ToString();
				thisEvent.Parts.Add(new Keypress(Parameter.Position, Parameter.DocLength, false));
				var keyPress = new KeyPress
				{
				        StartTime = time, 
                        EndTime = time, 
                        Value = "\n", 
                        Key = KeysEx.VK_RETURN
				};
				thisEvent.Parts.Add(keyPress);
				EventList.Add(thisEvent);

				Parameter.Position++;
				Parameter.DocLength = Parameter.DocLength - text.Length + 1;

				thisEvent = new Event();
				thisEvent.Properties["type"] = "replacement";
				thisEvent.Properties["id"] = (Parameter.Id++).ToString();
				thisEvent.Parts.Add(new Replacement(cursor, cursor + text.Length, "\n"));
				EventList.Add(thisEvent);

				thisEvent = new Event();
				thisEvent.Properties["type"] = "selection";
				thisEvent.Properties["id"] = (Parameter.Id++).ToString();
				thisEvent.Parts.Add(new SelectionChange(Parameter.Position, Parameter.Position));
				EventList.Add(thisEvent);
			}
			else
			{
				var thisEvent = new Event();
				thisEvent.Properties["type"] = "keyboard";
				thisEvent.Properties["id"] = (Parameter.Id++).ToString();
				thisEvent.Parts.Add(new Keypress(cursor, Parameter.DocLength++, true));
				Parameter.Position = cursor + 1;
				var keyPress = new KeyPress
				{
				        StartTime = time, 
                        EndTime = time, 
                        Value = "\n", 
                        Key = KeysEx.VK_RETURN
				};
				thisEvent.Parts.Add(keyPress);
				EventList.Add(thisEvent);
			}

            return EventList;
        }
    }
}
