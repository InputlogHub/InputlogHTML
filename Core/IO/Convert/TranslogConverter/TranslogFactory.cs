using System.Collections.Generic;
using System.Xml.XPath;
using InputLog.Core.Events;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    ///   Conversion factory for the different Translog events.
    /// </summary>
    public class TranslogFactory
    {
        /// <summary>
        /// The previous mouse click (Mouse "down").
        /// </summary>
        private XPathNavigator PreviousClick;

        /// <summary>
        /// Iterating over the list with Translog events and sending the xml nodes to the
        /// appropriate conversion class.
        /// </summary>
        /// <param name="nodes">the Translog events.</param>
        /// <param name="docParam">Utility class holding document length, cursor position, and event id </param>
        /// <returns>List with Inputlog events.</returns>
        public IEnumerable<Event> GetObject(XPathNavigator nodes, TranslogReader.DocParam docParam)
        {
            var inputlogEventList = new List<Event>();
            var param = docParam;

            foreach (XPathNavigator node in nodes.Select("//Events/*"))
            {
                if (node.Name.Equals("Mouse"))
                {
                    if (node.GetAttribute("Value", "").Equals("Up"))
                    {
                        inputlogEventList.AddRange(new MouseClick(node, PreviousClick, param).ConvertToEvent());
                        PreviousClick = null;
                    }

                    if (node.GetAttribute("Value", "").Equals("Down"))
                    {
                        PreviousClick = node;
                    }
                }
                else if (node.Name.Equals("Key"))
                {
                    var typeNode = node.GetAttribute("Type", "");
                    if (!typeNode.Equals(string.Empty))
                    {
                        string type = typeNode;
                        switch (type)
                        {
                            case "insert":
                                inputlogEventList.AddRange(new InsertKey(node, param).ConvertToEvent());
                                break;
                            case "delete":
                                inputlogEventList.AddRange(new DeleteKey(node, param).ConvertToEvent());
                                break;
                            case "navi":
                                inputlogEventList.AddRange(new NaviKey(node, param).ConvertToEvent());
                                break;
                            case "edit":
                                inputlogEventList.AddRange(new EditKey(node, param).ConvertToEvent());
                                break;
                            case "return":
                                inputlogEventList.AddRange(new ReturnKey(node, param).ConvertToEvent());
                                break;
                        }
                    }
                }
            }

            var finalText = nodes.SelectSingleNode("//FinalText");
            if (null != finalText) inputlogEventList.AddRange(new TextStatistics(finalText.Value, param).ConvertToEvent());

            return inputlogEventList;
        }
    }
}