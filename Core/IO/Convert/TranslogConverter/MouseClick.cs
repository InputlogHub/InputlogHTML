using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Converting a Translog mouse click event.
    /// </summary>
    public class MouseClick : ILConvertible
    {
        #region Fields

        private readonly XPathNavigator Node;
        private readonly XPathNavigator PrevNode;
        private ulong DownTime;
        private readonly List<Event> EventList;
        private readonly TranslogReader.DocParam Parameter;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">A Translog xml element.</param>
        /// <param name="previous">The previous mouse click.</param>
        /// <param name="param">Document parameters: length, position, event id.</param>
        public MouseClick(XPathNavigator node, XPathNavigator previous, TranslogReader.DocParam param)
        {
            Node = node;
            PrevNode = previous;
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
            bool hasText = false;

            // Reading the Translog node content
            ulong upTime = System.Convert.ToUInt64(Node.GetAttribute("Time", ""));     
            int cursor = System.Convert.ToInt32(Node.GetAttribute("Cursor", "")); 
            string text = Node.GetAttribute("Text", "");
            if (text.Length > 0)
            {
                hasText = true;
                block = System.Convert.ToInt32(Node.GetAttribute("Block", ""));
                if (block == 0)
                {
                    block = text.Length;
                }
            }
            // When we have a previous node, it certainly was a mouse'down'.
            if (null != PrevNode)
            {
                DownTime = System.Convert.ToUInt64(PrevNode.GetAttribute("Time", ""));
            }
            // ... but if we have a situation where the mouse 'down' went missing, 
            // we use the mouse 'up' time as DownTime.
            else if (Node.GetAttribute("Value", "").Equals("Up"))
            {
                DownTime = upTime - 10;
            }  

            // Conversion into new Inputlog events.
            var thisEvent = new Event();
            thisEvent.Properties["type"] = "mouse";
            thisEvent.Properties["id"] = (Parameter.Id++).ToString();
            thisEvent.Parts.Add(new Click(0, 0, Hooks.Mouse.MouseMessages.WM_LBUTTONDOWN, DownTime, upTime));
			EventList.Add(thisEvent);

            Parameter.Position = cursor;

			if (hasText)
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
			else
			{
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