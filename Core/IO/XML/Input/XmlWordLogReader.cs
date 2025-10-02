using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util;
using Part = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part;

namespace InputLog.Core.IO.Xml.Input
{
    /// <summary>
    /// EventPartReader that can read Windows-parts from XML.
    /// </summary>
    public class XmlWordLogReader
    {
        /// <summary>
        /// Reads a Windows KeyboardEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created KeyBoardEventPart.</returns>
        public static IKeyboardEventPart ReadKeyboard(XmlElement xmlElement)
        {
            int position = int.Parse(xmlElement[Part.Keyboard.WordLog.Position.TAG].InnerText);
            int documentLength = int.Parse(xmlElement[Part.Keyboard.WordLog.DocumentLength.TAG].InnerText);
            bool includeInReplay = bool.Parse(xmlElement[Part.Keyboard.WordLog.IncludeInReplay.TAG].InnerText);
            return new Keypress(position, documentLength, includeInReplay);
        }

        /// <summary>
        /// Reads a Windows MouseEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created MouseEventPart.</returns>
        public static IMouseEventPart ReadMouse(XmlElement xmlElement)
        {
            return null;
        }

        /// <summary>
        /// Reads an Windows FocusChangeEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created FocusChangeEventPart.</returns>
        public static IFocusChangeEventPart ReadFocus(XmlElement xmlElement)
        {
            return null;
        }

        /// <summary>
        /// Reads an Word ReplacementEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created replacementEventPart.</returns>
        public static Replacement ReadReplacement(XmlElement xmlElement)
        {
            int start = int.Parse(xmlElement[Part.Replacement.WordLog.Start.TAG].InnerText);
            int end = int.Parse(xmlElement[Part.Replacement.WordLog.End.TAG].InnerText);
            string text = StringUtils.Unescape(xmlElement[Part.Replacement.WordLog.NewText.TAG].InnerText);
            return new Replacement(start, end, text);
        }

        /// <summary>
        /// Reads an Word InsertionEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created insertionEventPart.</returns>
        public static Insert ReadInsert(XmlElement xmlElement)
        {
            int position = int.Parse(xmlElement[Part.Insert.WordLog.Position.TAG].InnerText);
            string before = StringUtils.Unescape(xmlElement[Part.Insert.WordLog.Before.TAG].InnerText);
            string after = StringUtils.Unescape(xmlElement[Part.Insert.WordLog.After.TAG].InnerText);
            return new Insert(position, before, after);
        }

        /// <summary>
        /// Reads an Word SelectionChangeEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created SelectionChangeEventPart.</returns>
        public static SelectionChange ReadSelection(XmlElement xmlElement)
        {
            int start = int.Parse(xmlElement[Part.SelectionChange.WordLog.Start.TAG].InnerText);
            int end = int.Parse(xmlElement[Part.SelectionChange.WordLog.End.TAG].InnerText);
            return new SelectionChange(start, end);
        }

        /// <summary>
        /// Reads an Word StatisticsEventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement">XmlElement to read the part from.</param>
        /// <returns>The created StatisticsEventPart.</returns>
        public static Statistics ReadStatistics(XmlElement xmlElement)
        {
            int charCountWithoutSpaces = xmlElement[Part.Statistics.WordLog.CharCountWithoutSpaces.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.CharCountWithoutSpaces.TAG].InnerText);
            int charCountWithSpaces = xmlElement[Part.Statistics.WordLog.CharCountWithSpaces.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.CharCountWithSpaces.TAG].InnerText);
            int farEastCharCount = xmlElement[Part.Statistics.WordLog.FarEastCharCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.FarEastCharCount.TAG].InnerText);
            int lineCount = xmlElement[Part.Statistics.WordLog.LineCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.LineCount.TAG].InnerText);
            int pageCount = xmlElement[Part.Statistics.WordLog.PageCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.PageCount.TAG].InnerText);
            int paragraphCount = xmlElement[Part.Statistics.WordLog.ParagraphCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.ParagraphCount.TAG].InnerText);
            int wordCount = xmlElement[Part.Statistics.WordLog.WordCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.WordCount.TAG].InnerText);

            int startCharCountWithoutSpaces =
                xmlElement[Part.Statistics.WordLog.StartCharCountWithoutSpaces.TAG].IsNullOrEmpty() ? -1
                    : int.Parse(xmlElement[Part.Statistics.WordLog.StartCharCountWithoutSpaces.TAG].InnerText);
            int startCharCountWithSpaces =
                xmlElement[Part.Statistics.WordLog.StartCharCountWithSpaces.TAG].IsNullOrEmpty() ? -1
                    : int.Parse(xmlElement[Part.Statistics.WordLog.StartCharCountWithSpaces.TAG].InnerText);
            int startFarEastCharCount = xmlElement[Part.Statistics.WordLog.StartFarEastCharCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.StartFarEastCharCount.TAG].InnerText);
            int startLineCount = xmlElement[Part.Statistics.WordLog.StartLineCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.StartLineCount.TAG].InnerText);
            int startPageCount = xmlElement[Part.Statistics.WordLog.StartPageCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.StartPageCount.TAG].InnerText);
            int startParagraphCount = xmlElement[Part.Statistics.WordLog.StartParagraphCount.TAG].IsNullOrEmpty() ? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.StartParagraphCount.TAG].InnerText);
            int startWordCount = xmlElement[Part.Statistics.WordLog.StartWordCount.TAG].IsNullOrEmpty()? -1
                : int.Parse(xmlElement[Part.Statistics.WordLog.StartWordCount.TAG].InnerText);

            return new Statistics(charCountWithoutSpaces, charCountWithSpaces, farEastCharCount, lineCount, pageCount, paragraphCount, wordCount,
            startCharCountWithoutSpaces, startCharCountWithSpaces, startFarEastCharCount, startLineCount, startPageCount, startParagraphCount, startWordCount);
        }

        /// <summary>
        /// Reads an AuthorComment EventPart from an XmlElement.
        /// </summary>
        /// <param name="xmlElement"></param>
        /// <returns></returns>
        public static AuthorComment ReadAuthorComment(XmlElement xmlElement)
        {
            var element = xmlElement[Part.AuthorComment.WordLog.Comment.TAG];
            if (element != null)
            {
                var comment = StringUtils.Unescape(element.InnerText);
                return new AuthorComment(comment);
            }
            return null;
        }
    }
}