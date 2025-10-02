using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    // Used by the Token Analysis: compares process and production with words from a given 'target' collection.
    internal class TokenReconstructor : Inspector
    {
        #region Fields

        // List of properties attached to the markup-tokens.
        private readonly IList<RegexDefinition> _regexList = new List<RegexDefinition>();
        // Each node contains either a character or a markup symbol in order of appearence in the S-Notation.
        private readonly LinkedList<SymbolNode> _symbols;
        // Collection of 'Token' objects extracted from the current idfx.
        private readonly Dictionary<int, Token> _tokenDictionary = new Dictionary<int, Token>();
        // Collection of revisions in this session with their revision type (immediate, delayed, or none) .
        private SortedDictionary<int, Token.RevisionType> _revisionTypes;
        // The intial W-Notation string with the analysis markup symbols.
        private readonly string _wNotation;
        // The abbreviation of the analysis that ordered this analysis.
        private readonly string _analysisAbbr;
        // The language of the text to analyze.
        private string _language;
        // Path to the csv-file with the target words.
        private readonly string _csvPath;
        // The index of the TokenDictionary.
        private int _tokenIndex;
        // The position of the previous token in the TokenDictionary.
        private int _prevPosition;

        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="text">The full text found in the *.idfx, rendered in W-Notation.</param>
        /// <param name="symbols">LinkedList of elements on which the text reconstruction is based.</param>
        /// <param name="language">The language of the text</param>
        /// <param name="abbrv">Optional abbreviation of the analysis that ordered these methods</param>
        /// <param name="csvPath">Optional path to the CSV-file with the target words for a Token Analyser.</param>
        public TokenReconstructor(string text, LinkedList<SymbolNode> symbols, string language, 
            string abbrv = "", string csvPath = "")
        {
            _wNotation = text;
            _symbols = symbols;
            _language = language;
            _analysisAbbr = abbrv;
            _csvPath = csvPath;
            _revisionTypes = new SortedDictionary<int, Token.RevisionType>();
        }

        /// <summary>
        /// A series of processes will transform the W-Notation text into a dataTable
        /// with targeted words and their revisions, and bigrams with the associated pause time.
        /// </summary>
        /// <param name="linguisticProcess">A DataSet holding two DataTables, one prepared by
        /// MarkupRemover and one empty at this point.</param>
        /// <returns>
        ///     A DataSet with two DataTables: one with three version of the original input string 
        ///     and one with edits for every word.
        /// </returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            if (!_analysisAbbr.Equals("TA"))
            {
                throw new AnalysisException("The given analysis is not of type 'Token Analysis'");
            }

            ReportProgress(this, new ProgressEventArgs("Working on the Token Reconstruction"));

            // General analysis of the revision types.
            DefineRevisionType();

            // Activating the necessary regexes.
            AddRegexDefinition();

            // Retrieving tokens and substrings. 
            CollectTokens(_wNotation);

            // Adding missing brackets and braces.
            MatchBraces();

            // Adding a reconstructed string to the Token object and finding its position
            // in the reconstructed text.
            Reconstruct();

            // Setting the target words for this session.
            var target = new TokenTarget(_tokenDictionary);
            target.SetTargetToken(_csvPath);

            // Instantiates a class for making bigrams out of a word and adding a pause time to every bigram.
            var bigrams = new NGramMaker(_symbols);

            // Converts a token production into bigrams and adds the associated pause time.
            // WordIndex can be set as an argument when there are more words to choose from. 
            bigrams.GetNGrams(_tokenDictionary);

            // Class inserts the data into a DataTable.
            var table = new BigramTable();
            linguisticProcess.Tables.Add(table.UpdateDataTable(_tokenDictionary));

            // Putting everything back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// Extracts tokens out of a string formatted according to the W-Notation conventions.
        /// </summary>
        /// <param name="txt">the W-Notation string.</param>
        private void CollectTokens(object txt)
        {
            //The postion of the previous token in the TokenDictionary.
            _prevPosition = 0;

            // Replacing line feeds, tabs, etc first with a 'middle dot'.
            string txt2 = RemoveStringControls(txt.ToString());

            // Next, removing all break markups.
            txt2 = Regex.Replace(txt2, TokenRegex.BREAK_MARKUP, string.Empty, RegexOptions.Compiled);

            // Catching deletions and temporary replacing any punctuation symbols with a white space
            // they will not be considered word boundaries.
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

                    // Imposing a word boundary when there is no space after a punctuation mark without middle dots.
                    try
                    {
                        var tokenBis = Regex.Match(t, TokenRegex.TOKEN_BIS, RegexOptions.Compiled);

                        if (tokenBis.Success)
                        {
                            Group g = tokenBis.Groups[1];
                            CaptureCollection cc = g.Captures;
                            Capture c = cc[0];

                            // First part includes the punctuation symbol.
                            string t1 = tokenBis.Value.Substring(0, c.Index + 2);
                            // Second part from the punctuation symbol to the end.
                            string t2 = t.Substring(c.Index + 3);
                            ProcessTokenString(t1 + "\u00B7");
                            ProcessTokenString(t2);
                        }

                        else
                        {
                            ProcessTokenString(t);
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        //Ignore System.ArgumentOutOfRangeException in this specific case.
                    }
                }
            }
        }

        /// <summary>
        /// Main processing method creating a token object, adding pauze time information and the processing type.
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
                // encased in an insert brace. 
                if (t[0].Equals('}'))
                {
                    t = t.Replace("}", string.Empty);
                    t = Regex.Replace(t, TokenRegex.MARKUP, string.Empty, RegexOptions.Compiled);

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

            // Removing remaining brackets and braces.
            t = Regex.Replace(t, TokenRegex.REMAINS, string.Empty, RegexOptions.Compiled);

            // If a token has only middlde dots or contains only a deletion, the current token is 
            // added to the previous together with its related revisions. 
            // The present token is then discarded.
            if (IsMiddleDotOnly(t) || IsEmpty(t))
            {
                Token prevToken = _tokenDictionary[_prevPosition];
                foreach (KeyValuePair<int, Token.RevisionType> r in token.TokenRevisions)
                {
                    if (prevToken.TokenRevisions.ContainsKey(r.Key)) continue;
                    prevToken.TokenRevisions.Add(r.Key, r.Value);
                }

                //Console.WriteLine(" ** PrevToken: " + prevToken.Production + " and t: " + t);

                prevToken.Production = prevToken.Production + t;

                // Previous token inherits the 'revised' process type from t.
                if (token.TokenProcess.Equals(Token.ProcessType.REVISED))
                {
                    prevToken.TokenProcess = token.TokenProcess;
                }
                return;
            }

            //Console.WriteLine("Candidate: " + t);

            _prevPosition = _tokenIndex;
            _tokenIndex++;
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
        /// <param name="txt">The text to clean.</param>
        private static string RemoveStringControls(string txt)
        {
            var index = 0;
            var sb = new StringBuilder();
            foreach (char c in txt)
            {
                var cat = Char.GetUnicodeCategory(c);

                //Console.WriteLine("UnicodeCategory detected : " + cat + " - " + c);
                if (cat.Equals(UnicodeCategory.Control)
                    || cat.Equals(UnicodeCategory.LineSeparator)
                    || cat.Equals(UnicodeCategory.ParagraphSeparator)
                    || cat.Equals(UnicodeCategory.SpaceSeparator))
                {
                    if (index == 0)
                    {
                        index++;
                        sb.Append("\u00B7");
                        //Console.WriteLine("CONTROL: " + sb + " idx: " + index);
                    }
                }
                else
                {
                    index = 0;
                    sb.Append(c);
                }
            }
            // Ensuring that every string ends with a middle dot.
            if (!sb.ToString().EndsWith("\u00B7"))
            {
                sb.Append("\u00B7");
            }
            //Console.WriteLine("String controls removed: " + sb);
            return sb.ToString();
        }

        /// <summary>
        /// Checking if this string contains only middle dots.
        /// </summary>
        /// <param name="t">The string to test.</param>
        /// <returns>'true'if string contains middle dot only, false otherwise.</returns>
        private bool IsMiddleDotOnly(string t)
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
            var fullTextSb = new StringBuilder();
            var start = 0;

            // Sorting the TokenDictionary on the index in the Token object.
            var sortedDict = _tokenDictionary.OrderBy(entry => entry.Value.DictEntry);
            foreach (Token token in sortedDict.Select(pair => pair.Value))
            {
                if (token.TokenProcess == Token.ProcessType.REVISED)
                {
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
                            }
                        }
                    }

                    token.Reconstruction = t;

                    // The production string shows the inserted and deleted characters with 
                    // the corresponding brackets and braces but without the revision indices.
                    token.Production = Regex.Replace(token.Production, TokenRegex.MARKUP, string.Empty,
                                                     RegexOptions.Compiled);

                    //Console.WriteLine(" ** Token Production: " + token.Production + " Reconstruction: " +
                    //                  token.Reconstruction + " Check: " + check);

                    if (token.Reconstruction.Length <= 1 || IsEmpty(token.Reconstruction))
                    {
                        token.TokenProcess = Token.ProcessType.DELETED;
                        token.Reconstruction = " # ";
                        //deletionSb.Append(token.Production).Append('\t');
                    }
                }
                else if (token.Production.Length > 0)
                {
                    token.Reconstruction = token.Production;
                }

                // Adding the reconstructed word without middle dots to 
                // the full text string to be used by the linguistic analyzer.
                fullTextSb.Append(token.Reconstruction.Replace('\u00B7', ' '));

                // Getting the position of this word in the reconstructed text.
                token.Position = start;
                start = start + token.Reconstruction.Length;
            }
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