using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Analyses.Revision.Revisions.Edits;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// A DataTable filled with linguistic information and process data for every word in a text production.
    /// </summary>
    public class InfoTable
    {
         #region Fields
        /// <summary>
        /// The DataTable.
        /// </summary>
        private DataTable AnalysisTable { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public InfoTable()
        {
            AnalysisTable = new DataTable();
        }

        /// <summary>
        /// Moving the information from the TokenDictionary to the AnalysisTable.
        /// </summary>
        public DataTable UpdateDataTable(Dictionary<int, Token> dictionary, ulong startOffset, string abbr, bool addTarget)
        {
            // Creating the table. 
            var colCount = 0;
            if (abbr.Equals("LG"))
            {
                colCount = 27;
            } 
            if (abbr.Equals("WP"))
            {
                colCount = 14;
            }
            AnalysisTable.TableName = "infoTable";
            AnalysisTable.AcceptChanges();

            // Creating the columns
            // First part: word forms and pause time
            AnalysisTable.Columns.Add("Revisions", typeof (string));        //0
            AnalysisTable.Columns.Add("S-Notation", typeof (string));       //1
            AnalysisTable.Columns.Add("CharsProduced", typeof(int));        //2
            AnalysisTable.Columns.Add("Token1", typeof(string));            //3
            AnalysisTable.Columns.Add("StartID", typeof (int));             //4
            AnalysisTable.Columns.Add("EndID", typeof (int));               //5
            AnalysisTable.Columns.Add("StartTime", typeof (ulong));         //6
            AnalysisTable.Columns.Add("EndTime", typeof (ulong));           //7
            AnalysisTable.Columns.Add("BeforeWord2", typeof(ulong));        //8     
            AnalysisTable.Columns.Add("BeforeWord1", typeof (ulong));       //9
            AnalysisTable.Columns.Add("BetweenPause", typeof(ulong));       //10
            AnalysisTable.Columns.Add("Production", typeof (ulong));        //11
            AnalysisTable.Columns.Add("WordPause", typeof (ulong));         //12
            AnalysisTable.Columns.Add("AfterWordPause", typeof(ulong));     //13
            if (addTarget)
            {
                AnalysisTable.Columns.Add("Target", typeof(string));        //14 (skip this if Analysis is Linguistic)
                colCount = 15;
            }

            // Second part: linguistic data
            if (abbr.Equals("LG"))
            {
                AnalysisTable.Columns.Add("Token2", typeof(string));        //14 (no Target in a Linguistic Analysis)
                AnalysisTable.Columns.Add("PoSA", typeof(string));          //15
                AnalysisTable.Columns.Add("PoSB", typeof(string));          //16
                AnalysisTable.Columns.Add("PoS-Prob", typeof(string));      //17
                AnalysisTable.Columns.Add("Lemma", typeof(string));         //18
                AnalysisTable.Columns.Add("Lemma-Prob", typeof(string));    //19
                AnalysisTable.Columns.Add("ChunkA", typeof(string));        //20
                AnalysisTable.Columns.Add("ChunkB", typeof(string));        //21
                AnalysisTable.Columns.Add("NE", typeof(string));            //22
                AnalysisTable.Columns.Add("NE-Prob", typeof(string));       //23
                AnalysisTable.Columns.Add("LogFreq", typeof(string));       //24
                AnalysisTable.Columns.Add("RelFreq", typeof(string));       //25
                AnalysisTable.Columns.Add("Syllable", typeof(string));      //26
            }

            // Iterating over the TokenDictionary after sorting the dictionary 
            // on the index in the Token object.
            var sortedDict = dictionary.OrderBy(entry => entry.Value.DictEntry);

            // Debug: console row counter
            //int j = 0;

            foreach (KeyValuePair<int, Token> pair in sortedDict)
            {
                var token = pair.Value;

                // No entry for deleted tokens.
                if (token.TokenProcess == Token.ProcessType.DELETED)
                {
                    continue;
                }

                // Data array with space for the token timed and linguistic info.
                var dataArray = new object[colCount];

                // Filling the array.
                // Revision number with abbreviated types ('I' = IMMEDIATE, 'D' = DELAYED)
                // Type 'NONE' is skipped.
                var sb = new StringBuilder();
                foreach (var revision in token.TokenRevisions)
                {
                    if (revision.Value.Equals(Token.RevisionType.NONE)) continue;
                    var type = revision.Value.ToString();
                    sb.Append(revision.Key).Append("-").Append(type.Substring(0, 1)).Append(" ");
                }
                dataArray[0] = sb.ToString();

                // Word with its deletions and insertions if any.
                dataArray[1] = token.Production;
                dataArray[2] = token.CharCount;
                dataArray[3] = token.ResultString.Replace('\u00B7', ' ');

                // Timed info.
                var firstEdit = token.EditList.FirstOrDefault();
                // Skipping the space after the chars.
                IEdit lastEdit = null;
                if (token.CharCount > 0)
                {
                    var listCount = token.CharCount - 1;
                    lastEdit = token.EditList[listCount];
                }            
                dataArray[4] = firstEdit?.StartPos ?? 0;
                dataArray[5] = lastEdit?.EndPos ?? dataArray[4];
                if (firstEdit != null)
                {
                    if (firstEdit.StartTime > startOffset)
                    {
                        dataArray[6] = firstEdit.StartTime - startOffset;
                    }
                    else
                    {
                        dataArray[6] = firstEdit.StartTime;
                    }
                }
                else
                {
                    dataArray[6] = 0;
                }

                if (lastEdit != null)
                {
                    if (lastEdit.EndTime > startOffset)
                    {
                        dataArray[7] = lastEdit.EndTime- startOffset;
                    }
                    else
                    {
                        dataArray[7] = lastEdit.EndTime;
                    }
                }
                else
                {
                    dataArray[7] = dataArray[6];
                }
                dataArray[8] = token.BeforeWord2;
                dataArray[9] = token.BeforeWord1;
                dataArray[10] = token.BeforeWord1 + token.BeforeWord2;
                dataArray[11] = token.WordProduction;
                dataArray[12] = token.WordPause;
                dataArray[13] = token.AfterWordPause;
                if (addTarget)
                {
                    dataArray[14] = token.TargetWord;
                }

                // Prefilling the linguistic data slots with placeholders
                if (abbr.Equals("LG"))
                {
                    dataArray[14] = "-";    // Token from the linguistic processor
                    dataArray[15] = "-";    // Part of Speech A
                    dataArray[16] = "-";    // Part of Speech B (second part)
                    dataArray[17] = 0.0;    // Part of Speech probability
                    dataArray[18] = "-";    // Lemma
                    dataArray[19] = 0.0;    // Lemma probability
                    dataArray[20] = "-";    // Chunk A
                    dataArray[21] = "-";    // Chunk B (second part)
                    dataArray[22] = "-";    // Named Entity (NE)
                    dataArray[23] = 0.0;    // Named Entity probability
                    dataArray[24] = 0;      // Log of Word Frequency
                    dataArray[25] = 0.0;    // Relative frequency
                    dataArray[26] = "-";    // Syllables
                }

                // Adding the array to the dataTable
                AnalysisTable.Rows.Add(dataArray);

                //try
                //{
                //    Debug
                //    if (abbr.Equals("LG"))
                //    {
                //        Console.WriteLine("*Row " + j + ": " + dataArray[0] + " " + dataArray[1] + " " + dataArray[2] + " " +
                //                          dataArray[3] + " " + dataArray[4] + " " + dataArray[5] + " " + dataArray[6] + " " + dataArray[7] + " " +
                //                          dataArray[8] + " " + dataArray[9] + " " + dataArray[10] + " " + dataArray[11] + " " +
                //                          dataArray[12] + " " + dataArray[13] + " " + dataArray[14] + " " + dataArray[15] + " " +
                //                          dataArray[16] + " " + dataArray[17] + " " + dataArray[18] + " " + dataArray[19] + " " +
                //                          dataArray[20] + " " + dataArray[21] + " " + dataArray[22] + " " + dataArray[23] + " " +
                //                          dataArray[24] + " " + dataArray[25]);
                //    }
                //    else
                //    {
                //        Console.WriteLine("*Row " + j + ": " + dataArray[0] + " " + dataArray[1] + " " + dataArray[2] + " " +
                //                          dataArray[3] + " " + dataArray[4] + " " + dataArray[5] + " " + dataArray[6] + " " +
                //                          dataArray[7] + " " + dataArray[8] + " " + dataArray[9] + " " + dataArray[10] + " " +
                //                          dataArray[11] + " " + dataArray[12]);
                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("Exception " + e.StackTrace);
                //    throw;
                //}

                //j++;
            }

            // Committing the changes.
            AnalysisTable.AcceptChanges();
            return AnalysisTable;
        }

        /// <summary>
        /// Returns the last edit with useful data.
        /// </summary>
        /// <param name="editList">The list with edits for this word.</param>
        /// <returns>The index in the list</returns>
        private static int GetLastIndex(IList<IEdit> editList)
        {
            int firstPos = 0;
            if (editList == null || !editList.Any())
            {
                return 0;
            }
            var anEdit = editList.ElementAt(0);

            if (anEdit != null) firstPos = anEdit.EndPos;

            // Hack to step over the curious line breaks introduced earlier 
            // in the analysis process and pushed to the end of this document.
            for (int i = editList.Count; i-- > 0; )
            {
                var edit = editList[i];

                if(edit != null && edit.StartPos < firstPos) continue;
                if (edit != null && edit.EndPos > 0)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
