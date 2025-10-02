using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// Committing the results of the bigram construction into a DataTable.
    /// Only bigrams/digraphs with a 'target' word are saved.
    /// </summary>
    public class BigramTable
    {
        #region Fields
        /// <summary>
        /// The DataTable to hold all bigrams.
        /// </summary>
        private DataTable AnalysisTable { get; set; }
        // The set with missed targets.
        private static IEnumerable<string> Missed { get { return TokenTarget.MissedTargets; } }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BigramTable()
        {
            AnalysisTable = new DataTable();
        }

        /// <summary>
        /// Moving all the data from the TokenDictionary to the AnalysisTable.
        /// </summary>
        public DataTable UpdateDataTable( Dictionary<int, Token> dictionary)
        {
            // Change name of this table.      
            AnalysisTable.TableName = "bigrams";
            AnalysisTable.AcceptChanges();

            // Finding the maximum bigram length.
            int maxBigrams = dictionary.
                Select(pair => pair.Value).
                Select(token => token.NGrams.Count).
                Concat(new[] { 0 }).Max();
            // Size of the data array.
            var countMax = maxBigrams * 2;

            // Creating the fixed columns
            AnalysisTable.Columns.Add("Name", typeof(string));
            AnalysisTable.Columns.Add("Target" , typeof(string));
            AnalysisTable.Columns.Add("Produced", typeof(string));
            AnalysisTable.Columns.Add("S-Notation", typeof(string));
            AnalysisTable.Columns.Add("Revisions", typeof(string));

            // Iterating over the TokenDictionary.
            foreach (KeyValuePair<int, Token> pair in dictionary)
            {
                var token = pair.Value;

                // Only proceeding when a target word exists.
                if (token.TargetWord.Equals("")) continue;

                var bigrams = token.NGrams;

                // Data array of the size of the number of bigrams for this word, 
                // plus extra space for additional information.
                var dataArray = new object[countMax + 5];

                // Preparing the bigram columns and their headers.
                int k = 0;
                for (var i = 0; i < countMax; i++)
                {
                    if (!MathExt.IsOdd(i))
                    {
                        if (AnalysisTable.Columns.Contains("digr_" + k)) continue;
                        AnalysisTable.Columns.Add("digr_" + k, typeof(string));
                    }
                    else
                    {
                        if (AnalysisTable.Columns.Contains("pause_" + k)) continue;
                        AnalysisTable.Columns.Add("pause_" + k, typeof(string));
                        k++;
                    }
                }

                // Filling the array.
                dataArray[0] = "name";
                dataArray[1] = token.TargetWord;
                dataArray[2] = token.Reconstruction;
                dataArray[3] = token.Production;

                // Revision number with abbreviated types ('I' = IMMEDIATE, 'D' = DELAYED)
                // Type 'NONE' is skipped.
                var sb = new StringBuilder();
                foreach (var revision in token.TokenRevisions)
                {
                    if (revision.Value.Equals(Token.RevisionType.NONE)) continue;
                    var type = revision.Value.ToString();
                    sb.Append(revision.Key).Append("-").Append(type.Substring(0, 1)).Append(" ");
                }
                dataArray[4] = sb.ToString();

                int j = 0;
                int biCount = (bigrams.Count * 2) + 5;

                // Putting the bigrams into the array.
                for (var i = 5; i < biCount; )
                {
                    var bigram = bigrams[j];
                    dataArray[i++] = bigram.First;
                    dataArray[i++] = bigram.Second;
                    j++;
                }

                // Adding the array to the dataTable
                AnalysisTable.Rows.Add(dataArray);
            }

            // Adding empty rows with the missing target string.
            foreach (var missingTarget in Missed )
            {
                var dataArray = new Object[countMax + 5];
                dataArray[0] = "name";
                dataArray[1] = missingTarget;

                // Adding the array to the dataTable
                AnalysisTable.Rows.Add(dataArray);
            }

            // Committing the changes.
            AnalysisTable.AcceptChanges();
            return AnalysisTable;
        }
    }
}
