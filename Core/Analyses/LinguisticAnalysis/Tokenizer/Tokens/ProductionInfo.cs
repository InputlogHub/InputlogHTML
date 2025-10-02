using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.Revision.Revisions.Edits;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// Adds StartTime, EndTime, Action time and Pause time to each token.
    /// </summary>
    public class ProductionInfo
    {
        /// <summary>
        /// >Laps of time to subtract from the log start
        /// </summary>
        private static ulong NewStartOffset { get; set; }

        /// <summary>
        ///    Enrich the token objects with timed information.
        /// </summary>
        /// <param name="tokenDictionary">A collection of 'Token' objects extracted from the current idfx.</param>
        /// <param name="startOffset">Laps of time to subtract from the log start</param>
        public void AddProductionInfo(Dictionary<int, Token> tokenDictionary, ulong startOffset)
        {
            NewStartOffset = startOffset;
            // The startTime of the last event of the previous word.
            var previousStartTime = 0ul; // NewStartOffset;
            IEdit prevEdit = null;

            // Sorting the TokenDictionary on the index in the Token object.
            var sortedDict = tokenDictionary.OrderBy(entry => entry.Value.DictEntry);
            foreach (KeyValuePair<int, Token> pair in sortedDict)
            {
                Token token = pair.Value;
                var editList = token.EditList;

                // Console.WriteLine("**Word: " + token.Reconstruction + " #edits: " + editList.Count);

                // Number of chars without the middle dot (space).
                token.CharCount = token.TokenProcess.Equals(Token.ProcessType.REVISED)
                    ? Math.Max(RemoveBraces(token.Production).Length - 1, 0)
                    : Math.Max(token.Production.Length - 1, 0);       

                // Adding 'null' to empty or partially empty edit lists.
                if (editList.Count < token.CharCount)
                {
                    IEdit edit = null;
                    int toAdd = token.CharCount - editList.Count;
                    for (int i = 0; i < toAdd; i++)
                    {
                        editList.Add(edit);
                    }
                    if (toAdd == editList.Count)
                    {
                        continue;
                    }
                }

                var firstStart = 0ul;
                var lastEnd = 0ul;

                for (int index = 0; index < editList.Count; index++)
                {
                    IEdit edit = editList[index];

                    //Console.WriteLine("edit.StartTime: " + (edit.StartTime - NewStartOffset)
                    //        + " index: " + index + " count: " + editList.Count);
                    try
                    {
                        // WordPause: starttime last char - starttime previous (starting with first char after space)
                        if (index > 0)
                        {
                            if (index < editList.Count - 1)
                            {
                                if (prevEdit != null)
                                {
                                    if (edit.StartTime > prevEdit.StartTime)
                                    {
                                        token.WordPause += edit.StartTime - prevEdit.StartTime;
                                    }
                                    else
                                    {
                                        token.WordPause += 0; // prevEdit.StartTime - edit.StartTime;
                                        edit.StartTime = prevEdit.StartTime;
                                    }
                                    //Console.WriteLine(" WordPause for " + token.Reconstruction + " index: " + pair.Key
                                    //    + " - #edit: " + index 
                                    //    + " - edit.StartTime: " + (edit.StartTime - NewStartOffset)
                                    //    + " - prevEdit.StartTime: " + (prevEdit.StartTime - NewStartOffset)
                                    //    + " - WordPause: " + token.WordPause);
                                }
                                else
                                {
                                    token.WordPause += 0;
                                }
                            }
                        }
                        // A special arrangement: the word pause for one letter words.
                        else if (editList.Count <= 2 && index == 0)
                        {
                            if (prevEdit != null)
                            {
                                if (edit.StartTime > prevEdit.StartTime)
                                {
                                    token.WordPause = edit.StartTime - prevEdit.StartTime;
                                }
                                else
                                {
                                    token.WordPause = 0;
                                    edit.StartTime = prevEdit.StartTime;
                                }
                            }

                            else
                            {
                                token.WordPause += 0;
                            }

                            //Console.WriteLine(" WordPause for " + token.Reconstruction + " index: " + index
                            //          + " count: " + editList.Count
                            //          + " edit.StartTime: " + (edit.StartTime - NewStartOffset)
                            //          + " prevEdit.StartTime: " + (prevEdit.StartTime - NewStartOffset));
                        }

                        // Ignoring start and end time with zero values.
                        var startTime = edit?.StartTime ?? 0;
                        if (firstStart == 0 && startTime > 0)
                        {
                            firstStart = startTime;
                        }
                        var endTime = edit?.EndTime ?? 0;

                        // Ignore pause time for the space symbol.
                        if (endTime > 0 && endTime > lastEnd && index < editList.Count - 1)
                        {
                            lastEnd = endTime;
                        }

                        token.BeforeWord2 = previousStartTime;

                        // AfterWordPause: pause time between start time of last char and start time of first event following, 
                        // probably a space, or mouse movement.
                        token.AfterWordPause = 0;
                        if (edit != null && prevEdit != null)
                        {
                            if (edit.StartTime > prevEdit.StartTime)
                            {
                                if (index == 0)
                                {
                                    token.BeforeWord1 = edit.StartTime - prevEdit.StartTime;
                                }
                                if (index == editList.Count - 1)
                                {
                                    token.AfterWordPause = edit.StartTime - prevEdit.StartTime;
                                }
                            }
                            else
                            {
                                // Try to look for a token beyond the inserted word
                                if (pair.Key > 1)
                                {
                                    KeyValuePair<int, Token> olderPair = sortedDict.ElementAt(pair.Key - 2);
                                    Token olderToken = olderPair.Value;
                                    var olderEdits = olderToken.EditList;
                                    var olderEdit = olderEdits.LastOrDefault();
                                    if (index == 0)
                                    {
                                        token.BeforeWord1 = 0;
                                        if (edit.StartTime > olderEdit.StartTime)
                                        {
                                            token.BeforeWord1 = edit.StartTime - olderEdit.StartTime;
                                        }
                                    }
                                    if (index == editList.Count)
                                    {
                                        if (edit.StartTime > olderEdit.StartTime)
                                        {
                                            token.AfterWordPause = edit.StartTime - olderEdit.StartTime;
                                        }
                                        if (edit.StartTime > NewStartOffset)
                                        {
                                            token.AfterWordPause = edit.StartTime - NewStartOffset;
                                        }
                                    }
                                }
                            }

                            //Console.WriteLine(" (PrevEdit greater) AfterWordPause for " + token.Reconstruction
                            //    + " at index: " + index + " edit.StartTime: " + (edit.StartTime - NewStartOffset)
                            //    + " prevEdit.StartTime: " + (prevEdit.StartTime - NewStartOffset) + " AWP: " + token.AfterWordPause);
                        }

                        prevEdit = edit;
                    }
                    // Not a very kosher use of exception catching, but useful to debug otherwise hard to
                    // observe problems with list of edits.
                    catch (Exception)
                    {
                        //Console.WriteLine("Exception catched - trying to continue");
                    }
                }
                previousStartTime = prevEdit != null ? token.AfterWordPause : 0;

                // Adding the word production time.
                if (lastEnd == 0 || lastEnd < firstStart)
                {
                    token.WordProduction = 0;
                }
                else
                {
                    token.WordProduction = lastEnd - firstStart;
                }

                //Console.WriteLine("BeforeWord-2: " + token.BeforeWord2);
                //Console.WriteLine("BeforeWord-1: " + token.BeforeWord1);
                //Console.WriteLine("WordPause: " + token.WordPause);
                //Console.WriteLine("WordProduction: " + token.WordProduction);
                //Console.WriteLine("AfterWordPause: " + token.AfterWordPause);
            }
        }

        /// <summary>
        ///  Temporary removal of all braces and brackets.
        /// Characters inside a deletion are ignored.
        /// </summary>
        /// <param name="word">The string to process.</param>
        /// <returns>The word stripped from braces and brackets.</returns>
        private string RemoveBraces(string word)
        {
            int markUp = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in word)
            {
                switch (c)
                {
                    case '[':
                    {
                        markUp++;
                        continue;
                    }
                    case ']':
                    {
                        markUp--;
                        continue;
                    }

                    case '{':
                    case '}':
                    {
                        continue;
                    }
                }
                if (markUp == 0)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}