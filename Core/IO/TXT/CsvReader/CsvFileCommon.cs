namespace InputLog.Core.IO.Txt.CsvReader
{
    /// <summary>
    ///     Common base class for CSV reader and writer classes.
    /// </summary>
    public abstract class CsvFileCommon
    {
        // Indexes into SpecialChars for characters with specific meaning
        private const int DELIMITER_INDEX = 0;
        private const int QUOTE_INDEX = 1;

        /// <summary>
        ///     These are special characters in CSV files. If a column contains any
        ///     of these characters, the entire column is wrapped in double quotes.
        /// </summary>
        private readonly char[] SpecialChars = new[] {';', '"', '\r', '\n'};

        /// <summary>
        ///     Gets/sets the character used for column delimiters.
        /// </summary>
        protected char Delimiter
        {
            get { return SpecialChars[DELIMITER_INDEX]; }
            set { SpecialChars[DELIMITER_INDEX] = value; }
        }

        /// <summary>
        ///     Gets/sets the character used for column quotes.
        /// </summary>
        protected char Quote
        {
            get { return SpecialChars[QUOTE_INDEX]; }
            set { SpecialChars[QUOTE_INDEX] = value; }
        }
    }
}