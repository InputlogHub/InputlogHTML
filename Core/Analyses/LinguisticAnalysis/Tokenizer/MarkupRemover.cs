using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// First step in the tokenize process is to transform all the deleted material and all the markup symbols with 
    /// the use of a set of regex definitions.
    /// </summary>
    public class MarkupRemover : Inspector
    {
        #region Fields
        // List of properties attached to the markup-tokens.
        private readonly IList<RegexDefinition> RegexList = new List<RegexDefinition>();

        // The intial W-Notation string with the analysis markup symbols.
        private readonly WNotationSummary WNotation;

        // Three variants of the input string with the markup symbols partially removed
        private string Reconstruction;
        private string DeleteInContext;
        private string InsertInContext;

        // Table to hold three versions of the input text (reconstruction, inserts in context, deletions in context)
        private DataTable TextTable;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="summary">The initial W-Notation prepared by the Revision Analysis.</param>
        public MarkupRemover(WNotationSummary summary)
        {
            WNotation = summary;
        }

        /// <summary>
        /// Processing the regex definitions on the input string to extract the markup symbol. Deleted and inserted parts 
        /// are collected for use later in the analysis process.
        /// </summary>
        /// <param name="linguisticProcess">The DataSet will hold two DataTables but is empty at this point.</param>
        /// <returns>A DataSet with a DataTables with three version of the original input string.</returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working with the MarkupRemover"));

            // Text version initialization.
            Reconstruction = WNotation.RevisionTxt;
            DeleteInContext = WNotation.RevisionTxt;
            InsertInContext = WNotation.RevisionTxt;
            // Activating the necessary regexes.
            AddRegexDefinition();
            // Retrieving tokens and substrings. 
            FillTable(WNotation);
            // Preparing DataTable with the three text versions.
            GenerateTextTable(Reconstruction, DeleteInContext, InsertInContext);

            // Updating the DataSet
            UpdateDataSet(linguisticProcess);

            // Putting everything back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// Retrieves tokens and substrings based on the results of a regex operation on the input.
        /// </summary>
        /// <param name="txt">The input string with a W-Notation markup.</param>
        private void FillTable(object txt)
        {
           foreach (var regEx in RegexList)
            {
                if (regEx.IsIgnored) continue;
                var transform = GetTransformType(regEx);

                // We keep information on the deleted and inserted parts, the rest of the markups is removed.
                if (transform > 0)
                {
                    var matches = regEx.Regex.Matches(txt.ToString());
                    foreach (Match markup in matches)
                    {
                        foreach (Capture capture in markup.Captures)
                        {
                            // Stripping the markups from the delete string.
                            if (transform == 1)
                            {
                                RemoveInternalMarkup(1, capture.Value, '[', ']');
                            }
                            // Stripping the markups from the insertion string
                            if (transform == 2)
                            {
                                RemoveInternalMarkup(2, capture.Value, '{', '}');
                            }
                        }
                    }
                }

                // Removes selected markup symbols from the input string.
                if (transform != 2) StringCleanup(regEx);
            }
        }

        /// <summary>
        /// Defines the kind of symbols to transform.
        /// </summary>
        /// <param name="regEx">The regex related to this definition.</param>
        /// <returns>a flag: 0 = no action; 1 = delete; 2 = insert.</returns>
        private static int GetTransformType(RegexDefinition regEx)
        {
            var transform = 0;
            if (regEx.Type.Equals("Delete")) transform = 1;
            if (regEx.Type.Equals("Insert")) transform = 2;
            return transform;
        }

        /// <summary>
        /// Adding regex definitions based on the W-Notation convention: deletion, insertion, break.
        /// The third (boolean) argument indicates whether this regex should be executed (false) 
        /// or ignored (true). The different regex machines (the 'rules') will run in order over the string.
        /// </summary>
        public override void AddRegexDefinition()
        {
            RegexList.Add(new RegexDefinition("Delete", TokenRegex.DELETE_MARKUP, false));
            RegexList.Add(new RegexDefinition("Insert", TokenRegex.INSERT_MARKUP, false));
            RegexList.Add(new RegexDefinition("Break", TokenRegex.BREAK_MARKUP, false));
            RegexList.Add(new RegexDefinition("LDelete", TokenRegex.LEFT_DELETE_MARKUP, false));
            RegexList.Add(new RegexDefinition("RDelete", TokenRegex.RIGHT_DELETE_MARKUP, false));
            RegexList.Add(new RegexDefinition("LInsert", TokenRegex.LEFT_INSERT_MARKUP, false));
            RegexList.Add(new RegexDefinition("RInsert", TokenRegex.RIGHT_INSERT_MARKUP, false));
            RegexList.Add(new RegexDefinition("WhiteDot", TokenRegex.WHITE_SPACE_DOT, true));
        }

        /// <summary>
        /// Strips markup symbols from the input text, removes deletions but includes the insertion string.
        /// Produces three variants to show insertions and deletions in context.
        /// </summary>
        /// <param name="regexDef">a RegexDefinition based on a regex (Deletion, Insertion, Break)</param>
        private void StringCleanup(RegexDefinition regexDef)
        {
            // String without any markup, deletions removed, insertions included.
            Reconstruction = Regex.Replace(Reconstruction, regexDef.Rule, string.Empty, RegexOptions.Compiled);
            // String without markup, except for deletions, insertions included.
            if (!regexDef.Type.Equals("LDelete") 
                && !regexDef.Type.Equals("RDelete") 
                && !regexDef.Type.Equals("Delete"))
                DeleteInContext = Regex.Replace(DeleteInContext, regexDef.Rule, string.Empty, RegexOptions.Compiled);
            // String without markup except for insertions, deletions removed.
            if (!regexDef.Type.Equals("LInsert") 
                && !regexDef.Type.Equals("RInsert")
                && !regexDef.Type.Equals("Insert"))
                InsertInContext = Regex.Replace(InsertInContext, regexDef.Rule, string.Empty, RegexOptions.Compiled);
        }

        /// <summary>
        /// Removes the markup symbols inside a captured regex group.
        /// </summary>
        /// <param name="transform">Defines the kind of symbols to transform.</param>
        /// <param name="s">The input string.</param>
        /// <param name="open">The 'open' bracket around an insert or deletion.</param>
        /// <param name="close">The 'close' bracket around an insert or deletion.</param>
        /// <returns>The characters from the input without the markup symbols.</returns>
        private void RemoveInternalMarkup(int transform, string s, char open, char close)
        {
            s = (s.Substring(s.IndexOf(open) + 1));
            s = s.Remove(s.IndexOf(close));

            // 'transform == 1' a deletion: return the empty string.
            if (transform == 1) 
            {
                return;
            }

            // 'transform == 2' an insert: transform nested deletions and markup symbols.
            var startIndex = 0;
            while (startIndex < s.Length)
            {
                var matchPosition = s.Length;
                foreach (var regEx in RegexList)
                {
                    if (regEx.IsIgnored) continue;
                    var regexStart = startIndex;
                    foreach (Match match in regEx.Regex.Matches(s, regexStart))
                    {
                        foreach (Capture capture in match.Captures)
                        {
                            if (regEx.Rule.Equals("Insert")) continue;
                            try
                            {
                                s = s.Replace(capture.Value, string.Empty);
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        }
                        matchPosition = match.Index;
                    }
                }
                startIndex = matchPosition;
            }
        }

        /// <summary>
        /// Generates a named table ("textStrings") with four versions of the input.
        /// </summary>
        /// <param name="reconstruct">Text with all markup and deletions removed.</param>
        /// <param name="deletions">Text with deletions in context.</param>
        /// <param name="inserts">Text with inserts in context.</param>
        private void GenerateTextTable(string reconstruct, string deletions, string inserts)
        {
            TextTable = new DataTable("textStrings");
            TextTable.Columns.Add("TextType", typeof(string));
            TextTable.Columns.Add("Text", typeof(string));
            TextTable.Rows.Add("Reconstruct", reconstruct);
            TextTable.Rows.Add("Inserts", inserts);
            TextTable.Rows.Add("Deletions", deletions);
        }

        /// <summary>
        /// Puts the DataTables in the DataSet.
        /// Replaces original TextTable with the updated version.
        /// </summary>
        /// <param name="linguisticProcess">The DataSet</param>
        private void UpdateDataSet(DataSet linguisticProcess)
        { 
            linguisticProcess.Tables.Add(TextTable);
        }
    }
}
