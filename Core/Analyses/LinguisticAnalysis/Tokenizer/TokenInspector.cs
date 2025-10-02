using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// Second step in the linguistic processing: inspecting the string
    /// for special constructs such as e-mail addresses, dates, hyphenated words. 
    /// The main purpose is to locate all dots and other symbols that may confuse 
    /// a part-of-speech (PoS) tagger.
    /// </summary>
    public class TokenInspector : Inspector
    {
        #region Fields
        // List of properties attached to the markup-tokens.
        private readonly IList<RegexDefinition> TokenDefinitions = new List<RegexDefinition>();

        // General End Of Line (EOL) regex
        readonly Regex EOLRegex = new Regex(@"\r\n?|\n|\f", RegexOptions.Compiled);

        // Regex match location
        private int CurrentIndex;
        private int CurrentLine;

        // Table to hold four versions of the input text (final product, reconstructed,
        // inserts in context, deletions in context)
        private DataTable TextTable;
        //// The table with the details for every token in the input text.
        //private DataTable AnalysisTable;
        #endregion

        /// <summary>
        /// Processing the regex definitions on the input string.
        /// </summary>
        /// <param name="linguisticProcess">A list with two DataTables: one with three version
        /// of the original input string and one with the revision number of the deletions 
        /// and the index of the markup in the original text.</param>
        /// <returns>A list with two DataTables. Endpoints are added to the cleaned string. 
        /// Special tokens are collected.</returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working with the TokenInspector"));
            AddRegexDefinition();

            // Loads the content of the DataTables into the local tables.
            TextTable = linguisticProcess.Tables["textStrings"];
            var row = TextTable.Rows[0];
            var wnotation = row.ItemArray[1].ToString();
            //AnalysisTable = linguisticProcess.Tables["processes"];

            // Splitting the input into properly formatted lines.
            var newText = GetTextLines(wnotation);

            // Replaces the original W-Notation with the updated version.
            TextTable.Rows.RemoveAt(0);
            var r = TextTable.NewRow();
            r["TextType"] = "Reconstruct";
            r["Text"] = newText.ToString();
            TextTable.Rows.InsertAt(r, 0);

            // Updating the DataSet
            UpdateDataSet(linguisticProcess);

            // Back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// Constructs text lines based on a EOL marker and looks for special tokens.
        /// </summary>
        /// <param name="wnotation">The orginal input, with all markup symbols removed.</param>
        /// <returns></returns>
        private StringBuilder GetTextLines(string wnotation)
        {
            var endOfLineMatch = EOLRegex.Matches(wnotation);
            var start = 0;
            string line;
            var newText = new StringBuilder();

            // Searching for EOL markers.
            foreach (Match eol in endOfLineMatch)
            {
                foreach (Capture capture in eol.Captures)
                {
                    CurrentLine += 1;
                    line = wnotation.Substring(start, capture.Index - start);
                    newText.Append(EndDot(line));
                   // FillTable(FindTokens(line));
                    start = capture.Index + 1;
                }
            }
            // Handles a possible leftover string from the input.
            if (start < wnotation.Length)
            {
                CurrentLine += 1;
                line = wnotation.Substring(start, wnotation.Length - start);
              //  newText.Append(EndDot(line));
              //  FillTable(FindTokens(line));
            }
            return newText;
        }

        ///// <summary>
        ///// Adds to the AnalysisTable every final word that is left in the text 
        ///// after the special tokens have been removed. 
        ///// Split uses the middle dot, tab, and new line to separate the words.
        ///// </summary>
        ///// <param name="txt">the cleaned final text</param>
        //protected virtual void FillTable(object txt)
        //{
        //    var delimiters = new[] { '\u00B7', '\t', '\r' };
        //    var words = txt.ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        //    //foreach (var word in words)
        //    //{
        //    //    AnalysisTable.Rows.Add(0, 0, word, "", "");
        //    //}
        //}

        /// <summary>
        /// Adding regex definitions.
        /// The third (boolean) argument indicates whether this regex should be executed (false) 
        /// or ignored (true). The different regex machines (the 'rules') will run in order over the string.
        /// </summary>
        public override void AddRegexDefinition()
        {
            TokenDefinitions.Add(new RegexDefinition("ShortAbbr", TokenRegex.SHORT_ABBREVIATION, true));
            TokenDefinitions.Add(new RegexDefinition("Email", TokenRegex.EMAIL_ADDRESS, false));
            TokenDefinitions.Add(new RegexDefinition("Web", TokenRegex.INTERNET_ADDRESS, false));
            TokenDefinitions.Add(new RegexDefinition("EuDate", TokenRegex.EU_DATE, false));
            TokenDefinitions.Add(new RegexDefinition("NormDate", TokenRegex.NORMALIZED_DATE, true));
            TokenDefinitions.Add(new RegexDefinition("UsDate", TokenRegex.US_DATE, true));
            TokenDefinitions.Add(new RegexDefinition("Abbr", TokenRegex.ABBREVIATION, false));
            TokenDefinitions.Add(new RegexDefinition("Hyph", TokenRegex.HYPHENATED_WORD, false));
        }

        /// <summary>
        /// Checks if a line has an endpoint, if not one is added. Precedes all endpoints with a space (middle dot) 
        /// to accomodate the shallow parser.
        /// </summary>
        /// <param name="line">Line based on a EOL marker</param>
        private static string EndDot(string line)
        {
            return line.IndexOf('.') < line.Length
                ? line.Insert(line.Length, ((Char)183 + ".")) :
                line.Insert(line.Length - 1, ((Char)183).ToString());
        }


        /// <summary>
        /// Runs the regexes over the strings and collects special tokens that answer the definition.
        /// When a special word is found it's added to the analysis table and removed from the string.
        /// </summary>
        /// <param name="line">The string to examine.</param>
        /// <returns>A string with all special tokens removed.</returns>
        private string FindTokens(string line)
        {
            var s = line;
            foreach (var def in TokenDefinitions)
            {
                if (def.IsIgnored) continue;
                var matches = def.Regex.Matches(line);

                foreach (var special in matches)
                {
                    CurrentIndex = line.IndexOf(special.ToString(), StringComparison.Ordinal);
               //     AnalysisTable.Rows.Add(0, CurrentIndex, special.ToString(), "", "");
                    // Removes all occurrences of the special token.
                    s = line.Replace(special.ToString(), "\u00B7");
                }
                line = s;
            }
            return s;
        }

        /// <summary>
        /// Replaces the DataTables in the DataSet with updated versions.
        /// </summary>
        /// <param name="linguisticProcess">The DataSet</param>
        protected virtual void UpdateDataSet(DataSet linguisticProcess)
        {
            // Removes the old tables.
            linguisticProcess.Tables.Remove("textStrings");
           // linguisticProcess.Tables.Remove("processes");
            // Replaces original TextTable with the updated version.
            linguisticProcess.Tables.Add(TextTable);
            //// Replaces original AnalysisTable with the updated version.
            //linguisticProcess.Tables.Add(AnalysisTable);
        }
    }
}
