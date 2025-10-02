using System.Xml;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util;
using InsertionPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Insert.WordLog;
using KeyboardPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Keyboard.WordLog;
using Part = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part;
using ReplacementPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Replacement.WordLog;
using SelectionPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.SelectionChange.WordLog;
using StatisticsPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Statistics.WordLog;
using AuthorCommentPart = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.AuthorComment.WordLog;

namespace InputLog.Core.IO.Xml.Output
{

    public class XmlWordLogWriter
    {
        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteInsert(Insert e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);

            writer.WriteElementString(InsertionPart.Position.TAG, e.Position.ToString()); // <position>
            writer.WriteElementString(InsertionPart.Before.TAG, StringUtils.Escape(e.Before)); // <before>
            writer.WriteElementString(InsertionPart.After.TAG, StringUtils.Escape(e.After)); // <after>

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteKeypress(Keypress e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG); // <part>
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);
                // LinearAnalysisType="wordlog"

            writer.WriteElementString(KeyboardPart.Position.TAG, e.Position.ToString()); // <position>
            writer.WriteElementString(KeyboardPart.DocumentLength.TAG, e.DocumentLength.ToString()); // <documentlength>
            writer.WriteElementString(KeyboardPart.IncludeInReplay.TAG, e.IncludeInReplay.ToString()); // <replay>

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteMouseEvent(MouseEvent e, XmlWriter writer)
        {
            // no extra info for mouse events in word => nothing to write
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteReplacement(Replacement e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);

            writer.WriteElementString(ReplacementPart.Start.TAG, e.Start.ToString()); // <start>
            writer.WriteElementString(ReplacementPart.End.TAG, e.End.ToString()); // <end>
            writer.WriteElementString(ReplacementPart.NewText.TAG, StringUtils.Escape(e.NewText)); // <newtext>

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteSelectionChange(SelectionChange e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);

            writer.WriteElementString(SelectionPart.Start.TAG, e.Start.ToString()); // <start>
            writer.WriteElementString(SelectionPart.End.TAG, e.End.ToString()); // <end>

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteStatistics(Statistics e, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);

            writer.WriteElementString(StatisticsPart.CharCountWithoutSpaces.TAG, e.CharCountWithoutSpaces.ToString());
            writer.WriteElementString(StatisticsPart.CharCountWithSpaces.TAG, e.CharCountWithSpaces.ToString());
            writer.WriteElementString(StatisticsPart.FarEastCharCount.TAG, e.FarEastCharCount.ToString());
            writer.WriteElementString(StatisticsPart.LineCount.TAG, e.LineCount.ToString());
            writer.WriteElementString(StatisticsPart.PageCount.TAG, e.PageCount.ToString());
            writer.WriteElementString(StatisticsPart.ParagraphCount.TAG, e.ParagraphCount.ToString());
            writer.WriteElementString(StatisticsPart.WordCount.TAG, e.WordCount.ToString());
            writer.WriteElementString(StatisticsPart.StartCharCountWithoutSpaces.TAG,
                e.StartCharCountWithoutSpaces.ToString());
            writer.WriteElementString(StatisticsPart.StartCharCountWithSpaces.TAG, e.StartCharCountWithSpaces.ToString());
            writer.WriteElementString(StatisticsPart.StartFarEastCharCount.TAG, e.StartFarEastCharCount.ToString());
            writer.WriteElementString(StatisticsPart.StartLineCount.TAG, e.StartLineCount.ToString());
            writer.WriteElementString(StatisticsPart.StartPageCount.TAG, e.StartPageCount.ToString());
            writer.WriteElementString(StatisticsPart.StartParagraphCount.TAG, e.StartParagraphCount.ToString());
            writer.WriteElementString(StatisticsPart.StartWordCount.TAG, e.StartWordCount.ToString());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the AuthorComment EventPart.
        /// </summary>
        /// <param name="ac">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteAuthorComment(AuthorComment ac, XmlWriter writer)
        {
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.WordLog);
            writer.WriteElementString(AuthorCommentPart.Comment.TAG, ac.Comment);
            writer.WriteEndElement();
        }
    }
}