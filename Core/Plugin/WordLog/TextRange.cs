using Word = Microsoft.Office.Interop.Word;

namespace InputLog.Core.Plugin.WordLog
{
    /// <summary>
    /// A text range that copies necessary data out of a given
    /// range in order to compare it for equality, log it, ...
    /// </summary>
    public class TextRange
    {
        public enum OverlapTYPE
        {
            DISJUNCT_BEFORE,
            DISJUNCT_AFTER,
            CONTAINED,
            OVERLAP_BEGIN,
            OVERLAP_END
        }

        #region Fields
        /// <summary>
        /// Start character position of the range.
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// End character position of the range.
        /// </summary>
        public int End { get; private set; }

        /// <summary>
        /// Returns the length of the selection.
        /// </summary>
        public int Length
        {
            get { return End - Start; }
        }

        /// <summary>
        /// The text contained by the range.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The font of the range.
        /// </summary>
        private Word.Font Font { get; set; }

        /// <summary>
        /// The originating range.
        /// </summary>
        public readonly Word.Range Range;
        ///// <summary>
        ///// The style of the range.
        ///// </summary>
        //public Style Style { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="range">The range where to copy the data from.</param>
        public TextRange(Word.Range range)
        {
            Start = range.Start;
            End = range.End;
            Text = range.Text;
            Font = range.Font;
            Range = range;
            //this.Style = range.Style;
        }

        /// <summary>
        /// Constructor using a start position a length and a Word Document.
        /// </summary>
        /// <param name="start">The start position of the range in de Document.</param>
        /// <param name="end">The end position of the range in the Document.</param>
        /// <param name="doc">The Word Document where to get the range from.</param>
        public TextRange(int start, int end, Word.Document doc)
            : this(doc.Range(start, end)) { }

        /// <summary>
        /// 20130117 E. This approach seems to decrease significantly the number of 'out-of-range' exceptions
        /// when making a Revision Matrix.
        /// Constructor using the range as returned by the Content of a Word Document.
        /// </summary>
        /// <param name="doc">The Word Document where to get the range from.</param>
        public TextRange(Word.Document doc)
            : this(doc.Content) { }

        public TextRange(int start, int end)
        {
            if (start <= end)
            {
                Start = start;
                End = end;
            }
            else
            {
                Start = end;
                End = start;
            }
            Text = "";
        }

        /// <summary>
        /// Checks for equality.
        /// Two TextRanges are equal if their Start, End and Text are equal.
        /// </summary>
        /// <param name="obj">The object where to compare this instance to.</param>
        /// <returns>True if obj is a TextRange with equal Start, End and Text properties.</returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            var textRange = obj as TextRange;
            if (textRange != null)
            {
                var range = textRange;

                result = Start == range.Start && End == range.End
                         && Text.Equals(range.Text); // && this.Font.Equals(range.Font);
            }

            return result;
        }

        /// <summary>
        /// Returns the hash code of the object.
        /// </summary>
        /// <returns>The hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode() + Text.GetHashCode();
        }

        /// <summary>
        /// Compares the contents of the two textranges, returning true if the
        /// contained text and formatting is equal, false if not.
        /// </summary>
        /// <param name="text">The textrange to compare its content to this instance.</param>
        /// <returns>True if the contained text and formatting is equal, false if not.</returns>
        public bool HasSameContentAs(TextRange text)
        {
            // TODO formatting!
            bool result = false;

            if (Text == null && text.Text == null)
            {
                // Either both  are false
                result = true;
            }
            else if (Text != null && text.Text != null)
            {
                // Or both are not null
                result = Text.Equals(text.Text);
            }

            return result;
        }

        internal OverlapTYPE Overlap(TextRange r)
        {
            if (r.Start >= End) return OverlapTYPE.DISJUNCT_AFTER;
            if (r.End <= Start) return OverlapTYPE.DISJUNCT_BEFORE;
            if (r.Start >= Start && r.End <= End) return OverlapTYPE.CONTAINED;
            if (r.Start < Start && r.End <= End) return OverlapTYPE.OVERLAP_BEGIN;
            if (r.Start >= Start && r.End > End) return OverlapTYPE.OVERLAP_END;
            return OverlapTYPE.DISJUNCT_AFTER;
        }

        internal void Shift(int s, int e)
        {
            Start += s;
            End += e;
        }

        internal int StartOffset(TextRange r)
        {
            return Start - r.Start;
        }

        internal int EndOffset(TextRange r)
        {
            return End - r.Start;
        }
    }
}