using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Analyses.WordPauses;
using InputLog.Core.Util;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    // Used by the Linguistic Analysis and Word Pauses Analysis
    // Tokenizing Text with ICU4j's RuleBasedBreakIterator
    // file:///C:/Users/EVH/Documents/InputlogDoc/Tokenizer/Salmon%20Run%20Tokenizing%20Text%20with%20ICU4j's%20RuleBasedBreakIterator.htm
    internal class WordsReconstruction : Inspector
    {
        #region Fields

        // List of properties attached to the markup-tokens.
        private readonly IList<RegexDefinition> _regexList = new List<RegexDefinition>();
        // Collection of 'Token' objects extracted from the current idfx.
        private readonly Dictionary<int, Token> _tokenDictionary = new Dictionary<int, Token>();
        // Collection of revisions in this session with their revision type (immediate, delayed, or none) .
        private SortedDictionary<int, Token.RevisionType> _revisionTypes;
        // The intial W-Notation string with the analysis markup symbols.
        private readonly string _wNotation;
        private static string _cleanTxt;
        // The full text reconstructed by MakupRemover.
        private static string ReconstructedText { get; set; }
        // The abbreviation of the analysis that ordered this analysis.
        private readonly string _analysisAbbr;
        //>The paths to optional cvs files with target words and the participant-target assignment.
        private readonly string[] _targetPaths;
        // The optional id of the author of this idfx.
        private readonly string _participant;
        // The Damerau-Levenshtein distance between taget and production.
        private readonly int _wordDistance;
        // This table holds four versions of the input text: final product from the Word document,
        // reconstructed, inserts in context, and deletions in context.
        private DataTable _textTable;
        // Full words that have been deleted. Tab separated string added to the reconstructed text.
        private string DeletedWords { get; set; }
        // The index of the TokenDictionary.
        private int _tokenIndex;
        // The position of the previous token in the TokenDictionary.
        private int _prevPosition;
        // The language of the text to analyze.
        private readonly string _language;
        // Checking if all words were correctly recognized and provided with the right timing info.
        // Debugging
        public static bool Succeeded { get; private set; }
        // Empty token to add at the front of a normal.
        private bool _addEmptyToken;
        // Adding a column with the target 
        private bool _addTarget;
        //WordEdits class gets the edits and pause information for every character of a word.
        private readonly WordEdits _wordPauseEdits;
        /// <summary>
        /// >Laps of time to subtract from the log start
        /// </summary>
        private static ulong NewStartOffset { get; set; }
        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="text">The full text found in the *.idfx, rendered in W-Notation.</param>
        /// <param name="symbols">LinkedList of elements on which the text reconstruction is based.</param>
        /// <param name="startOffset">Laps of time to subtract from the log start.</param>
        /// <param name="abbrv">Optional abbreviation of the analysis that ordered these methods</param>
        /// <param name="language">The language of this session</param>
        /// <param name="targets">The paths to optional cvs files with target words and the participant-target assignment</param>
        /// <param name="participant">The optional id of the author of this idfx.</param>
        /// <param name="distance"> The Damerau-Levenshtein distance between taget and production.</param>
        public WordsReconstruction(string text, LinkedList<SymbolNode> symbols, ulong startOffset, 
            string language, string abbrv = "", string[] targets = null, string participant = "", int distance = 0)
        {
            _wNotation = text;
            _analysisAbbr = abbrv;
            _revisionTypes = new SortedDictionary<int, Token.RevisionType>();
            NewStartOffset = startOffset;
            _language = language;
            _wordDistance = distance;
            _wordPauseEdits = new WordEdits(RemoveStringControls(symbols));
            _targetPaths = targets;
            _participant = participant;
        }

        /// <summary>
        /// A series of processes will transform the W-Notation text into a dataTable
        /// with words and their revisions.
        /// </summary>
        /// <param name="linguisticProcess">A DataSet holding two DataTables, one prepared by
        /// MarkupRemover and one empty at this point.</param>
        /// <returns>
        ///     A DataSet with two DataTables: one with three versions of the original input string 
        ///     and one with edits for every word.
        /// </returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            if (!_analysisAbbr.Equals("LG") && !_analysisAbbr.Equals("WP"))
            {
                throw new AnalysisException("The given task is not a 'Linguistic Analysis' nor a 'Word Pause Analysis'");
            }

            ReportProgress(this, new ProgressEventArgs("Working on the Token Reconstruction"));

            // Loading the content of the DataTables into the local tables.
            _textTable = linguisticProcess.Tables["textStrings"];

            // General analysis of the revision types.
            DefineRevisionType();

            // Activating the necessary regexes.
            AddRegexDefinition();

            // Extracting tokens with their timed information from the symbol node list.
            CollectTokens();

            // Adding missing brackets and braces.
            MatchBraces();

            // Adding a reconstructed string to the Token object and finding its position
            // in the reconstructed text.
            Reconstruct();

            // Instantiates a class that adds Action time and Pause time to each token.
            var productionInfo = new ProductionInfo();
            productionInfo.AddProductionInfo(_tokenDictionary, NewStartOffset);

            // Setting the target words for this session if the optional path is given.
            if (!_targetPaths.IsNullOrEmpty())
            {
                var target = new WordPauseTargets(_tokenDictionary, _wordDistance);
                target.SetTargetToken(_targetPaths, _participant);
                _addTarget = true;
            }

            // Inserting the data into a DataTable.
            var table = new InfoTable();
            linguisticProcess.Tables.Add(table.UpdateDataTable(_tokenDictionary, NewStartOffset, _analysisAbbr, _addTarget));

            // TODO implement adding deleted words in a tab separated string to the reconstructed text.
            // These deletions will also receive linguistic information.
            if (DeletedWords.Length > 0)
            {
                ReconstructedText = ReconstructedText + '\n' + DeletedWords;
            }

            // Replaces the original with the updated version.
            _textTable.Rows.RemoveAt(0);
            var r = _textTable.NewRow();
            r["TextType"] = "Reconstruct";
            r["Text"] = ReconstructedText;
            _textTable.Rows.InsertAt(r, 0);
            _textTable.AcceptChanges();

            // Putting everything back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// Extracts tokens out of a string formatted according to the W-Notation conventions.
        /// </summary>
        private void CollectTokens()
        {
            //The postion of the previous token in the TokenDictionary.
            _prevPosition = 0;

            string txt2 = _cleanTxt;

            // Next, removing all break markups.
            txt2 = Regex.Replace(txt2, TokenRegex.BREAK_MARKUP, string.Empty, RegexOptions.Compiled);

            // Catching deletions and temporary replacing any punctuation symbols inside a deletion with a white space
            // so that they will not be mistaken for word boundaries.
            MatchCollection deletions = Regex.Matches(txt2, TokenRegex.DELETE_MARKUP, RegexOptions.Compiled);
            foreach (Match match in deletions)
            {
                foreach (Capture capture in match.Captures)
                {
                    var tt = Regex.Replace(capture.Value, TokenRegex.PUNCT, "\u0020", RegexOptions.Compiled);
                    txt2 = txt2.Replace(capture.Value, tt);
                }
            }

            // First rough step: a token is every sequence of characters bounded by a middle dot (space replacement).
            MatchCollection tokenMatches = Regex.Matches(txt2, TokenRegex.TOKEN, RegexOptions.Compiled);
            foreach (Match match in tokenMatches)
            {
                foreach (Capture capture in match.Captures)
                {
                    string t = capture.Value;

                    //Console.WriteLine("Raw candidate: " + t + " idx: " + TokenIndex);

                    // Forcing a word boundary on a string with a punctuation mark but without middle dots.
                    // Single and double quotes are ignored.
                    var tokenBis = Regex.Match(t, TokenRegex.TOKEN_BIS, RegexOptions.Compiled);
                    if (tokenBis.Success)
                    {
                        Group g = tokenBis.Groups[1];
                        CaptureCollection cc = g.Captures;
                        Capture c = cc[0];

                        // First part from begin to the punctuation symbol.
                        string t1 = tokenBis.Value.Substring(0, c.Index);
                        // Second part from the punctuation symbol to the end.
                        string t2 = t.Substring(c.Index + 1);
                        // The punctuation symbol between the two parts.
                        string punct = tokenBis.Value.Substring(c.Index, 1);

                        //Console.WriteLine("tokenBis Length: " + tokenBis.Value.Length + " t1-Length: "
                        //    + t1.Length + " c-Index: " + c.Index + " t1: " + t1 + " t2: " + t2 + " punct: " + punct);

                        // If t2 has only one character that is not a middle dot + optionally a white space, add it to t1 and add the
                        // punctuation symbol at the end but before the middle dot boundary.
                        if (t2 != string.Empty && t2.Length < 3 && !t2.Contains("\u00B7"))
                        {
                            ProcessTokenString(t1 + punct + t2[0] + "\u00B7");
                        }
                        else
                        {
                            var hasLInsertion = (Regex.Match(t2, TokenRegex.LEFT_INSERT_MARKUP, RegexOptions.Compiled));
                            var hasRInsertion = (Regex.Match(t2, TokenRegex.RIGHT_INSERT_MARKUP, RegexOptions.Compiled));
                            if(!hasLInsertion.Success && hasRInsertion.Success)
                            {
                                ProcessTokenString(t1 + punct + hasRInsertion.Value + "\u00B7");  
                            }
                            else
                            {
                                ProcessTokenString(t1 + punct);// + "\u00B7");
                            }
                            ProcessTokenString(t2);
                        }
                    }
                    else
                    {
                        ProcessTokenString(t);
                    }
                }
            }
        }

        /// <summary>
        /// Main processing method creating a token object, adding pauze time information and the processing type.
        /// Punctuation is separated from the word and saved as an additional token.
        /// </summary>
        /// <param name="t">The token string to process.</param>
        private void ProcessTokenString(string t)
        {
            MatchCollection process = new Regex(TokenRegex.MARKUP, RegexOptions.Compiled).Matches(t);
            var token = CreateToken();

            // Defining the token ProcessType and getting the revisions.
            // Initally every token gets the 'NormalProductionRevision' indication
            // with a 'NONE' revision type.
            token.TokenRevisions.Add(0, Token.RevisionType.NONE);

            // Adding revision numbers when appropriate.
            if (process.Count > 0)
            {
                // Special case: the previous word has a separation dot
                // encased in an insert brace. We remove the first brace
                // and its accessory markup index.
                Regex rgx1 = new Regex(@"}");
                Regex rgx2 = new Regex(TokenRegex.MARKUP);
                string[] text1 = {};
                string[] text2 = {};
                int removedKey = -1;

                if (t[0].Equals('}'))
                {
                    // We have to remember the number of that markup key.
                    t = rgx1.Replace(t, string.Empty, 1);
                    if (!t.IsNullOrEmpty()) text1 = Regex.Split(t, @"\D+");
                    t = rgx2.Replace(t, string.Empty, 1);
                    if (!t.IsNullOrEmpty()) text2 = Regex.Split(t, @"\D+");
                    if (text1.Length > 0 && text2.Length > 0)
                    {
                        var diff = text1.Except(text2);
                        removedKey = Convert.ToInt32(diff.First());
                    }

                    // We assume that this word is a normal production
                    // preceded by an insert if this is the only markup.
                    if (process.Count == 1)
                    {
                        token.TokenProcess = Token.ProcessType.NORMAL;
                    }
                    else
                    {
                        token.TokenProcess = Token.ProcessType.REVISED;
                        CollectRevisionNumber(process, token);
                        // Removing from the revision list the now redundant 
                        // markup key linked to the first brace.
                        token.TokenRevisions.Remove(removedKey);
                    }
                }
                else
                {
                    token.TokenProcess = Token.ProcessType.REVISED;
                    CollectRevisionNumber(process, token);
                }
            }
            else
            {
                token.TokenProcess = Token.ProcessType.NORMAL;
            }
            // Replaces a blank with the middle dot character.
            token.Production = t.Replace('\u0020', '\u00B7');

            // Removes remaining closing symbols.
            t = Regex.Replace(t, TokenRegex.REMAINS, string.Empty, RegexOptions.Compiled);

            // If a token has only middlde dots or contains only a deletion, the current token is 
            // added to the next token together with all related revisions. The present token is then discarded.
            if (IsMiddleDotOnly(t) || IsEmpty(t))
            {
                _addEmptyToken = true;
                _prevPosition = _tokenIndex;
                _tokenIndex++;
                return;
            }

            if (_addEmptyToken)
            {
                Token prevToken = _tokenDictionary[_prevPosition];
                foreach (KeyValuePair<int, Token.RevisionType> r in token.TokenRevisions)
                {
                    if (prevToken.TokenRevisions.ContainsKey(r.Key)) continue;
                    prevToken.TokenRevisions.Add(r.Key, r.Value);
                    if(token.TokenProcess != Token.ProcessType.NORMAL)
                    {
                        prevToken.TokenProcess = Token.ProcessType.REVISED;
                    }
                }

            //  Console.WriteLine(" ** PrevToken: " + prevToken.Production + " and t: " + t);
                prevToken.Production = prevToken.Production + t;                
                // Remove the current token, it is now added to the empty previous one.
                _tokenDictionary.Remove(_tokenIndex);
                t = prevToken.Production;
                token = prevToken;
                _addEmptyToken = false;
            }
            
             //Console.WriteLine("New Candidate: " + t);

            // Collecting the edits for this token.
            token.EditList.AddRange(_wordPauseEdits.GetAllEdits(t, token.TokenRevisions));
            token.ResultString = _wordPauseEdits.ResultString;
            _tokenIndex++;

            // Checking if this word contains a punctuation symbol at the end.
            // If so, the punctuation is separated and saved as an additional token.
            FindPunctuation(t, token);
        }

        /// <summary>
        ///  Creating a new empty token entry in the TokenDictionary.
        /// </summary>
        /// <returns>A new Token object.</returns>
        private Token CreateToken()
        {
            var token = new Token();
            _tokenDictionary[_tokenIndex] = token;
            token.DictEntry = _tokenIndex;
            return token;
        }

        /// <summary>
        /// Replacing line feeds, tabs, etc first with a 'middle dot'.
        /// </summary>
        /// <param name="nodes">The node list to clean</param>
        private static LinkedList<SymbolNode> RemoveStringControls(LinkedList<SymbolNode> nodes)
        {
            var sb = new StringBuilder();
            LinkedList < SymbolNode > cleanSymbols = new LinkedList<SymbolNode>();

            foreach (SymbolNode thisNode in nodes)
            {
                if (thisNode.Symbol.Length > 1)
                {
                    cleanSymbols.AddLast(thisNode);
                    sb.Append(thisNode.Symbol);
                }
                else
                {
                    char c = thisNode.SymbolChar;
                    var cat = char.GetUnicodeCategory(c);
                    //Console.WriteLine("UnicodeCategory detected : " + cat + " - " + c);
                    if (cat.Equals(UnicodeCategory.Control)
                        || cat.Equals(UnicodeCategory.LineSeparator)
                        || cat.Equals(UnicodeCategory.ParagraphSeparator)
                        || cat.Equals(UnicodeCategory.SpaceSeparator))
                    {
                        thisNode.Symbol = "\u00B7";
                        cleanSymbols.AddLast(thisNode);
                        sb.Append("\u00B7");
                        //Console.WriteLine("CONTROL: " + sb + " idx: " + index);
                    }
                    else
                    {
                        cleanSymbols.AddLast(thisNode);
                        sb.Append(thisNode.Symbol);
                    }
                }
            }
            // Ensuring that every string ends with a middle dot.
            if (!sb.ToString().EndsWith("\u00B7"))
            {
                sb.Append("\u00B7");
            }
            //Console.WriteLine("String controls removed: " + sb);
            _cleanTxt = sb.ToString();
            return cleanSymbols;
        }

        /// <summary>
        /// Checking if this string contains only middle dots.
        /// </summary>
        /// <param name="t">The string to test.</param>
        /// <returns>'true'if string contains middle dot only, false otherwise.</returns>
        private static bool IsMiddleDotOnly(string t)
        {
            var t2 = Regex.Replace(t, TokenRegex.DELETE_MARKUP, string.Empty, RegexOptions.Compiled);
            t2 = Regex.Replace(t2, TokenRegex.MARKUP, string.Empty, RegexOptions.Compiled);
            t2 = RemoveBraces(t2).Trim();
            t2 = t2.Replace('\u0020', '\u00B7');
            return t2.All(c => c.Equals('\u00B7'));
        }

        /// <summary>
        /// Checking if this string is empty after deletion.
        /// </summary>
        /// <param name="t">The string to test</param>
        /// <returns>'true'if an empty string is returned.</returns>
        private static bool IsEmpty(string t)
        {
            var t2 = Regex.Replace(t, TokenRegex.DELETE_MARKUP, string.Empty, RegexOptions.Compiled);
            t2 = Regex.Replace(t2, TokenRegex.MARKUP, string.Empty, RegexOptions.Compiled);
            t2 = t2.Replace('\u00B7', '\u0020');
            t2 = RemoveBraces(t2).Trim();
            return string.IsNullOrEmpty(t2);
        }

        /// <summary>
        /// The linguistic analysis expects all punctuation symbols in front 
        /// or after a word to be detached and saved as a separate token.
        /// </summary>
        /// <param name="t">The 'word'.</param>
        /// <param name="token">The Token object of the word</param>
        private void FindPunctuation(string t, Token token)
        {
            // Looking for punctuation at the beginning of a word.
            var front = t.Substring(0, Math.Min(3, t.Length));

            //Console.WriteLine("front: " + front);

            MatchCollection punctMatch = Regex.Matches(front, TokenRegex.OPEN_PUNCT, RegexOptions.Compiled);
            if (punctMatch.Count > 0 && front.Length > 2)
            {
                CreatePunctuation(token, punctMatch, 0);
            }

            // Looking for punctuation at the back of a word, 
            // only if this string is different from the front.
            var back = t.Substring(Math.Max(0, t.Length - 3));
            if (front.Equals(back)) return;
            var revStr = new string(back.ToCharArray().Reverse().ToArray());

            //Console.WriteLine("back: " + back);

            punctMatch = Regex.Matches(revStr, _language.Equals("FR") ? TokenRegex.FRENCH_PUNCT 
                : TokenRegex.CLOSE_PUNCT, RegexOptions.Compiled);
            if (punctMatch.Count > 0 && back.Length > 2)
            {
                CreatePunctuation(token, punctMatch, 1);
            }
        }

        /// <summary>
        ///  Creating an entry in the TokenDictionary if a punctuation symbol was found.
        /// </summary>
        /// <param name="token">The token object</param>
        /// <param name="punctMatch">Matching chars.</param>else
        /// <param name="pos">Position of de punctuation: at the front of the word ('0') or at the back ('1').</param>
        private void CreatePunctuation(Token token, MatchCollection punctMatch, int pos)
        {
            int i = punctMatch.Count;
            // Making a new token for each punctuation symbol found.
            foreach (Match match in punctMatch)
            {          
                var listIdx = token.EditList.Count - i;
                var newToken = CreateToken();
                newToken.Production = match.ToString() + '\u0020';
                newToken.ResultString = match.ToString() + '\u0020';
                newToken.TokenProcess = Token.ProcessType.PUNCT;

                // Collecting the edit for this punctuation, adding it to the new EditList
                // and removing it from the original token.
                IEdit newEdit = token.EditList[listIdx];
                newToken.EditList.Add(newEdit);             
                _tokenIndex++;

                var len = token.Production.Length;
                if (len > 3 && punctMatch.Count == 1)
                {
                    token.Production = token.Production.Replace(match.ToString(), string.Empty);
                  //  token.Production = token.Production.Remove(token.Production.Length -1);
                    token.EditList.RemoveAt(listIdx);
                }
                --i;

                //Console.WriteLine(" CLOSE_PUNCT token: " + token.Production + " match: "
                // + match + " new token: " + newToken.Production);

                // When a punctuation symbol was found in front of the word, the content 
                // of token and newToken (but not their indexes) are swapped.
                // Reason: the new token for the punctuation symbol gets an index higher than the index
                // for the original word and is consequently positioned behind that word. This routine
                // puts the symbol back at the front of the word.
                if (pos != 0) continue;
                var tmpToken = token;
                var tmpIdx1 = token.DictEntry;
                var tmpIdx2 = newToken.DictEntry;
                token = newToken;
                newToken = tmpToken;
                token.DictEntry = tmpIdx1;
                newToken.DictEntry = tmpIdx2;

            }
            // Remove dictionary entry if the token.EditList is empty after copying 
            // all the punctuation edits to the new tokens.
            if (token.EditList.Count == punctMatch.Count)
            {
                _tokenDictionary.Remove(token.DictEntry);
                _tokenIndex--;
            }
                // Console.WriteLine(" token: " + token + "\n new token: " + newToken);         
        }

        /// <summary>
        /// Collecting the revision numbers, then removing them from the string.
        /// </summary>
        /// <param name="type">Token types: REVISED, NORMAL, NONE.</param>
        /// <param name="token">a Token instance</param>
        private void CollectRevisionNumber(MatchCollection type, Token token)
        {
            foreach (Capture c in type)
            {
                string revision = Regex.Replace(c.Value, TokenRegex.UNDERSCORE, string.Empty, RegexOptions.Compiled);
                int revKey = Convert.ToInt32(revision);

                if (token.TokenRevisions.ContainsKey(revKey)) continue;
                token.TokenRevisions.Add(revKey, _revisionTypes[revKey]);
            }
        }

        /// <summary>
        ///     Adding the missing opening or closing braces and brackets on word level.
        /// These are missing because insertions or deletions may extend over
        /// many events in one text string and words are now a separate entities.
        /// </summary>
        private void MatchBraces()
        {
            foreach (var pair in _tokenDictionary)
            {
                Token token = pair.Value;
                if (token.TokenProcess == Token.ProcessType.REVISED)
                {
                    var sb = new StringBuilder();
                    int openBrckt = 0;
                    int openBrce = 0;
                    string t = token.Production;

                    foreach (char chr in t)
                    {
                        switch (chr)
                        {
                            case '[':
                                openBrckt++;
                                break;
                            case ']':
                                openBrckt--;
                                break;
                            case '{':
                                openBrce++;
                                break;
                            case '}':
                                openBrce--;
                                break;
                        }
                    }
                    if (openBrce < 0)
                    {
                        for (int e = 0; e > openBrce; e--)
                        {
                            sb.Append('{');
                        }
                        sb.Append(t);
                    }
                    else if (openBrce > 0)
                    {
                        sb.Append(t);
                        for (int e = 0; e < openBrce; e++)
                        {
                            sb.Append('}');
                        }
                    }
                    if (openBrckt < 0)
                    {
                        for (int e = 0; e > openBrckt; e--)
                        {
                            sb.Append('[');
                        }
                        sb.Append(t);
                    }
                    else if (openBrckt > 0)
                    {
                        sb.Append(t);
                        for (int e = 0; e < openBrckt; e++)
                        {
                            sb.Append(']');
                        }
                    }
                    if (sb.Length == 0)
                    {
                        sb.Append(t);
                    }
                    token.Production = sb.ToString();
                }
            }
        }

        /// <summary>
        ///     Reconstructing the final word as it was produced from the 'process' string 
        ///     by deleting the deletions and inserting the insertions.
        ///     Finally, adding the position of the reconstructed word in the text.
        /// </summary>
        private void Reconstruct()
        {
            var deletionSb = new StringBuilder();
            var fullTextSb = new StringBuilder();
            var check = string.Empty;
            var start = 0;
            var checkCount = 0;

            // Sorting the TokenDictionary on the index in the Token object.
            var sortedDict = _tokenDictionary.OrderBy(entry => entry.Value.DictEntry);
            foreach (Token token in sortedDict.Select(pair => pair.Value))
            {
                if (token.TokenProcess == Token.ProcessType.REVISED)
                {
                   // Console.WriteLine(" ** Token Production before : " + token.Production);
                    
                    string t = token.Production;

                    foreach (RegexDefinition regEx in _regexList)
                    {
                        MatchCollection matches = regEx.Regex.Matches(t);
                        foreach (Match markup in matches)
                        {
                            foreach (Capture capture in markup.Captures)
                            {
                                if (t.Length <= 0) continue;
                                t = t.Replace(capture.Value, string.Empty);

                                // Console.Write("  Token cleaned : " + t + " Regex: " + regEx.Type);
                            }
                        }
                    }

                   // Console.WriteLine("\n ** Final Token Production after : " + t);

                    token.Reconstruction = t;

                    // The production string shows the inserted and deleted characters with 
                    // the corresponding brackets and braces but without the revision indices.
                    token.Production = Regex.Replace(token.Production, TokenRegex.MARKUP, string.Empty,
                                                     RegexOptions.Compiled);
                    check = RemoveBraces(token.Production);

                   // Console.WriteLine(" ** Reconstruction: " + token.Reconstruction + " Check: " + check);

                    if (token.Reconstruction.Length < 1 || IsEmpty(token.Reconstruction))
                    {
                        token.TokenProcess = Token.ProcessType.DELETED;
                        token.Reconstruction = " # ";
                        //deletionSb.Append(token.Production).Append('\t');
                    }
                }
                else if (token.Production.Length > 0)
                {
                    token.Reconstruction = token.Production;
                    check = token.Production;
                }

                // A check on the synchronization with the revision data. The tokens reconstructed 
                // with WordsReconstruction and WordEdit should be identical.
                // checkCount should be zero.
                char[] rslt = null;
                if (token.ResultString != null)
                {
                     rslt = token.ResultString.Where(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c)).ToArray();
                }
                char[] chck = check.Where(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c)).ToArray();
                var rsltStr = new string(rslt);
                var chckStr = new string(chck);
                if (rsltStr.Equals(string.Empty))
                {
                    continue;
                }
                if (!rsltStr.Equals(chckStr))
                {
                    checkCount++;
                   // Console.WriteLine("** Result: " + rsltStr + " Check: " + chckStr + " Count: " + checkCount);
                }

                // Adding the reconstructed word without middle dots to 
                // the full text string to be used by the linguistic analyzer.
                fullTextSb.Append(token.Reconstruction.Replace('\u00B7', ' '));

                // Getting the position of this word in the reconstructed text.
                token.Position = start;
                start = start + token.Reconstruction.Length;
            }

            // If checkCount is zero all words were synchronized with the right timing info.
            // This information is passed to the XMLWriter to inform the user.
            Succeeded = checkCount == 0;

            // ** Not implemented yet **
            // Constructing a tab separated string with deleted words.
            DeletedWords = deletionSb.ToString();

            // Collecting reconstructed words into one string.
            ReconstructedText = fullTextSb.ToString().Trim();
        }

        /// <summary>
        /// Defines for every revision if it is 'immediate' or 'delayed'.
        /// An immediate revision happens between words, a delayed revision
        /// has one or more words between the break and the actual deletion or
        /// insertion. A SortedDictionary collects the revision number as key and
        /// a RevisionType as value.
        /// </summary>
        private void DefineRevisionType()
        {
            _revisionTypes = new SortedDictionary<int, Token.RevisionType>();
            _regexList.Add(new RegexDefinition("RDel", TokenRegex.RREVISION_DEL, false));
            _regexList.Add(new RegexDefinition("RIns", TokenRegex.RREVISION_INS, false));
            _regexList.Add(new RegexDefinition("LDel", TokenRegex.LREVISION_DEL, false));
            _regexList.Add(new RegexDefinition("LIns", TokenRegex.LREVISION_INS, false));
            _regexList.Add(new RegexDefinition("Break", TokenRegex.BREAK_MARKUP, false));

            foreach (RegexDefinition regEx in _regexList)
            {
                var matches = regEx.Regex.Matches(_wNotation);
                foreach (Match markup in matches)
                {
                    foreach (Capture capture in markup.Captures)
                    {
                        var digits = new HashSet<string>();
                        MatchCollection match = Regex.Matches(capture.Value, @"(\d+)", RegexOptions.Compiled);
                        foreach (Match match1 in match)
                        {
                            digits.Add(match1.ToString());
                        }

                        int pos1 = 0;
                        int pos2 = 0;
                        foreach (string digit in digits)
                        {
                            if (regEx.Type.Equals("RDel") || regEx.Type.Equals("RIns"))
                            {
                                pos1 = capture.Value.IndexOf(@"_" + digit + @"_", StringComparison.Ordinal);
                                pos2 = capture.Value.LastIndexOf(@"^_" + digit + @"_", StringComparison.Ordinal);
                            }
                            if (regEx.Type.Equals("LDel") || regEx.Type.Equals("LIns"))
                            {
                                pos1 = capture.Value.IndexOf(@"^_" + digit + @"_", StringComparison.Ordinal);
                                pos2 = capture.Value.LastIndexOf(@"_" + digit + @"_", StringComparison.Ordinal);
                            }

                            // When both pos1 and pos2 are -1, this digit is not a markup symbol
                            // but a plain number in the text.
                            if (pos1 == -1 && pos2 == -1) continue;
                            if (regEx.Type.Equals("Break"))
                            {
                                if (_revisionTypes.ContainsKey(Convert.ToInt32(digit))) continue;
                                _revisionTypes.Add(Convert.ToInt32(digit), Token.RevisionType.DELAYED);
                            }
                            if (Math.Abs(pos1 - pos2) > (digit.Length + 10) || pos2 == -1)
                            {
                                if (_revisionTypes.ContainsKey(Convert.ToInt32(digit))) continue;
                                _revisionTypes.Add(Convert.ToInt32(digit), Token.RevisionType.DELAYED);
                            }
                            else
                            {
                                if (_revisionTypes.ContainsKey(Convert.ToInt32(digit))) continue;
                                _revisionTypes.Add(Convert.ToInt32(digit), Token.RevisionType.IMMEDIATE);
                            }
                        }
                    }
                }
            }
            // Clears the regex list for use with other regexes.
            _regexList.Clear();
        }


        /// <summary>
        ///  Adding regex definitions based on the W-Notation convention: deletion, insertion, break.
        /// </summary>
        public override void AddRegexDefinition()
        {
            _regexList.Add(new RegexDefinition("FullDel", TokenRegex.DELETE_MARKUP, false));
            _regexList.Add(new RegexDefinition("NestedDel", TokenRegex.NESTEDDELETE, false));
            _regexList.Add(new RegexDefinition("ShortDel", TokenRegex.SHORTDELETE, false));
            _regexList.Add(new RegexDefinition("LInsert", TokenRegex.LEFT_INSERT_MARKUP, false));
            _regexList.Add(new RegexDefinition("RInsert", TokenRegex.RIGHT_INSERT_MARKUP, false));
            _regexList.Add(new RegexDefinition("LDelete", TokenRegex.SLDELETE, false));
            _regexList.Add(new RegexDefinition("RDelete", TokenRegex.SRDELETE, false));
            _regexList.Add(new RegexDefinition("LBrace", TokenRegex.LBRACE, false));
            _regexList.Add(new RegexDefinition("RBrace", TokenRegex.RBRACE, false));
            _regexList.Add(new RegexDefinition("RBracket", TokenRegex.RBRACKET, false));
            _regexList.Add(new RegexDefinition("Markup", TokenRegex.MARKUP, false));
        }

        /// <summary>
        ///  Temporary removal of all braces and brackets.
        /// </summary>
        /// <param name="word">The string to process.</param>
        /// <returns>The word stripped from braces and brackets.</returns>
        private static string RemoveBraces(string word)
        {
            word = Regex.Replace(word, @"[\[\]{}]", string.Empty, RegexOptions.Compiled);
            return word;
        }
    }
}