using System.Collections.Generic;
using System.Text;

namespace InputLog.Core.Events.WordLog
{
    /// <summary>
    /// An EventPart that represents the replacement of a range of text in the logged
    /// Document by some new text (possibly empty if the range was only deleted).
    /// </summary>
    public sealed class Replacement : IEventPart
    {
        #region Fields

        /// <summary>
        /// The start point of the range that is replaced.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// The end point of the range that is replaced.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Returns the length of the range to be replaced.
        /// </summary>
        public int Length
        {
            get { return End - Start + 1; }
        }

        /// <summary>
        /// The text by which [Start End] is replaced in the Document.
        /// </summary>
        public string NewText { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">The start point of the range being replaced.</param>
        /// <param name="end">The end point of the range being replaced.</param>
        /// <param name="text">The text that is inserted over the given range or the empty string
        /// if the range was only deleted (this is the default value).</param>
        public Replacement(int start, int end, IEnumerable<char> text = null)
        {
            Start = start;
            End = end;
			
			//NewText = (text == null) ? '' : text;// ReplaceNonPrintableCharacters(text, ' ');
			NewText = "";

			var sb = new StringBuilder();
			if (text != null)
			{
				foreach(char c in text)
				{
					sb.Append(c);
				}
				NewText = sb.ToString();
			}

            //Debug.WriteLine("Replacement class: {0}-{1}: \"{2}\"", start, end, text);
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Replacement()
        {
        }
    }
}