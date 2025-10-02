using System.IO;
using InputLog.Core.Events.WordLog;

namespace InputLog.Core.IO.Txt.Output
{
    public class TxtWordLogWriter
    {
        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteInsert(Insert e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  insertion: {0}: \"{1}\" | \"{2}\"", e.Position, e.Before, e.After);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteKeypress(Keypress e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  position: {0} - replay: {1}", e.Position, e.IncludeInReplay);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteMouseEvent(MouseEvent e, StreamWriter writer)
        {
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteReplacement(Replacement e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  replacement: {0}-{1}: \"{2}\"", e.Start, e.End, e.NewText);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteSelectionChange(SelectionChange e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  selection: " + e.Start + " - " + e.End);
        }

        /// <summary>
        ///     Write the given EventPart.
        /// </summary>
        /// <param name="e">The EventPart to write.</param>
        /// <param name="writer"></param>
        public static void WriteStatistics(Statistics e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  statistics: ");
            writer.WriteLine("    Character count (excl spaces): {0}", e.CharCountWithoutSpaces);
            writer.WriteLine("    Character count (incl spaces): {0}", e.CharCountWithSpaces);
            writer.WriteLine("    FarEast char count: {0}", e.FarEastCharCount);
            writer.WriteLine("    Line count: {0}", e.LineCount);
            writer.WriteLine("    Page count: {0}", e.PageCount);
            writer.WriteLine("    Paragraph count: {0}", e.ParagraphCount);
            writer.WriteLine("    Word count: {0}", e.WordCount);
        }

        /// <summary>
        /// Writing the author comment.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="writer"></param>
        public static void WriteAuthorComment(AuthorComment e, StreamWriter writer)
        {
            writer.WriteLine("  WORD  comment: " + e.Comment);
        }
    }
}