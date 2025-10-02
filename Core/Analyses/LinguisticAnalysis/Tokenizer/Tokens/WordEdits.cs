using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// Getting the edits for every character of a word in a Word Pause or Linguistic Analysis.
    /// </summary>
    public class WordEdits
    {
        #region Fields

        /// <summary>
        ///     The list of revisions & edits to link to the characters.
        /// </summary>
        private LinkedList<SymbolNode> Symbols { get; set; }

        /// <summary>
        ///     The current word position in the list of Symbols.
        /// </summary>
        private int CurrentSymbolPosition { get; set; }

        /// <summary>
        /// String constructed with Symbol nodes. The ResultString should be
        /// identical to the word made in the WordsReconstruction class.
        /// </summary>
        public string ResultString { get; private set; }

        /// <summary>
        /// List of regexes to detect the markup-tokens.
        /// </summary>
        private readonly IList<RegexDefinition> _regexList = new List<RegexDefinition>();

        /// <summary>
        /// The regexes for a 'word', brackets, braces and break.
        /// </summary>
        private readonly Regex _alphaRegex = new Regex(TokenRegex.WORD);

        private readonly Regex _leftBracketRegex = new Regex(TokenRegex.SHORTL_DELETE_MARKUP);
        private readonly Regex _rightBracketRegex = new Regex(TokenRegex.SHORTR_DELETE_MARKUP);
        private readonly Regex _rightInsertRegex = new Regex(TokenRegex.RIGHT_INSERT_MARKUP);
        private readonly Regex _regexBreak = new Regex(TokenRegex.BREAK_MARKUP);

        /// <summary>
        /// In deletions an extra space to be deleted is added when no such thing has been logged.
        /// We count them as more than one may be present.
        /// </summary>
        private int _isSuspiciousSpace;

        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="symbols">LinkedList with all elements on which the text reconstruction is based.</param>
        public WordEdits(LinkedList<SymbolNode> symbols)
        {
            var node = symbols.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value.Edit != null && node.Value.Edit.Type == EditType.Skip)
                {
                    symbols.Remove(node);
                }
                node = nextNode;
            }
            Symbols = symbols;
            _regexList.Add(new RegexDefinition("LInsert", TokenRegex.LEFT_INSERT_MARKUP, false));
            _regexList.Add(new RegexDefinition("RInsert", TokenRegex.RIGHT_INSERT_MARKUP, false));
            _regexList.Add(new RegexDefinition("LDelete", TokenRegex.SHORTL_DELETE_MARKUP, false));
            _regexList.Add(new RegexDefinition("RDelete", TokenRegex.SHORTR_DELETE_MARKUP, false));
        }

        /// <summary>
        /// This IEnumerable skips the SymbolNodes already visited.
        /// </summary>
        /// <param name="word">The input string.</param>
        /// <returns>Array with SymbolNodes starting from the CurrentSymbolPosition.</returns>
        private IEnumerable<SymbolNode> NodesLeft(string word)
        {
            // Synchronizes the Symbols list with the input from the token 
            SymbolNode currentSymbol = Symbols.ElementAt(CurrentSymbolPosition);
            if (currentSymbol.SymbolChar.Equals('\u00B7') &&
                Symbols.ElementAt(CurrentSymbolPosition + 1).Symbol.Equals(word[0].ToString()))
            {
                ++CurrentSymbolPosition;
                currentSymbol = Symbols.ElementAt(CurrentSymbolPosition);
            }
            SymbolNode prevSymbol = null;
            if (CurrentSymbolPosition > 0) prevSymbol = Symbols.ElementAt(CurrentSymbolPosition - 1);
            if (IsBreak(currentSymbol))
            {
                if (prevSymbol != null && prevSymbol.Symbol[0].Equals(word[0]))
                {
                    return Symbols.Skip(--CurrentSymbolPosition);
                }
            }
            return Symbols.Skip(CurrentSymbolPosition);
        }

        /// <summary>
        ///  Collecting the edits (IEdit) from the SymbolNodes for every character in a word.
        ///  We will taking into account only IMMEDIATE deletions. Inserts are always implemented.
        /// </summary>
        /// <param name="word">The 'word' string.</param>
        /// <param name="revisions">The type of revisions for this word (immediate or delayed)</param>
        /// <returns>List with edits for every character in the arg.</returns>
        public IEnumerable<IEdit> GetAllEdits(string word, SortedDictionary<int, Token.RevisionType> revisions)
        {
            // List to fill with edits.
            var allEdits = new List<IEdit>();

            // Collecting characters
            var result = new StringBuilder();
            // Managing deletions
            int markUp = 0;
            IEdit lastDelEdit = null;
            ulong firstIntialDelCharTime = 0;
            ulong lastInitialDelCharTime = 0;
            bool useFirstDeletedCharTime = false;
            bool isLastDeletion = false;
            // Managing insertions
            bool isLastInsertion = false;
            IEdit insertedEdit = null;
            int listCounter = 0;

            // The location of the revisions performed on this word, and their type, either IMMEDIATE or DELAYED.
            SortedDictionary<int, Token.RevisionType> thisRevisions = revisions;
            // Array containing the nodes not seen yet.
            SymbolNode[] nodeArray = NodesLeft(word).ToArray();

            // Ensures that each word ends with a middle dot, and only one.
            var input = word.TrimEnd('\u00B7') + '\u00B7';

            // When the length of the trimmed word is different from the original,
            // we add the diff to the listCounter to stay in sync with the symbol list.
            var diff = word.Length - input.Length;

            var inputList = new List<string>();
            inputList.AddRange(FindMarkupSymbols(input));

            // Reverse loop. Checking if this word ends with a deletion or an insertion.
            // Whatever comes first breaks the iteration.
            for (var i = inputList.Count; i-- > 0;)
            {
                var item = inputList[i];
                // Insertion at the end
                if (_rightInsertRegex.IsMatch(item))
                {
                    isLastInsertion = true;
                    break;
                }
                // Deletion at the end.
                if (_rightBracketRegex.IsMatch(item))
                {
                    isLastDeletion = true;
                    break;
                }
                // Any TypeChar
                if (_alphaRegex.IsMatch(item))
                {
                    break;
                }
            }

            try
            {
                // A counter to dynamically change the iteration if needed.
                listCounter = inputList.Count;
                listCounter += diff;
                for (int i = 0; i < listCounter; i++)
                {
                    // i must be smaller than the length of the nodeArray,
                    // otherwise we reached the end of the symbols.
                    // This should not happen.                
                    if (i >= nodeArray.Length)
                    {
                        break;
                    }

                    SymbolNode node = nodeArray[i];
                    string listNode = string.Empty;
                    if (i < inputList.Count)
                    {
                        listNode = inputList[i];
                    }

                    // If we know that this node is a TypeChar we do not have to check
                    // again for the deletion and insertion symbols.
                    bool isNotTypeChar;
                    if (node.Edit != null && node.Edit.Type == EditType.TypeChar)
                    {
                        isNotTypeChar = false;
                    }
                    else
                    {
                        isNotTypeChar = true;
                    }

                    // Incrementing the Symbol position by 1 each time a break symbol
                    // is encountered, then taking the next symbol and adjusting the counter.
                    // Break symbols ("^" or caret in the W-Notation) are seen in the Symbols list 
                    // but not in the inputList. We add a caret to the inputList to keep it synchronized.
                    if (isNotTypeChar)
                    {
                        while (IsBreak(node))
                        {
                            node = nodeArray[++i];
                            listCounter++;
                            if (inputList.Count > i - 1)
                            {
                                inputList.Insert(i - 1, "^");
                            }
                        }
                    }

                    // Ignoring all edits that are deletions.
                    // Except if the first or the last character is deleted, then we
                    // use the start time (first) or the end time (last) of the deleted char
                    // to get a correct start and end of the word in view.
                    // However, we only consider deletions that are an 'IMMEDIATE' revision,
                    // i.e. deletions that happened while producing the word. 'DELAYED' revisions
                    // made later in the writing process are ignored.
                    if (isNotTypeChar && IsDeletion(node, ref markUp))
                    {
                        // Initial deletion, do we take account?
                        // 'DELAYED' revisions are ignored.
                        if (i == 0)
                        {
                            string[] rev = Regex.Split(node.Symbol, @"\D+");
                            useFirstDeletedCharTime = thisRevisions[Convert.ToInt32(rev[1])] ==
                                                      Token.RevisionType.IMMEDIATE;
                        }
                        continue;
                    }
                    if (markUp > 0)
                    {
                        // i is not 0 anymore, but this is the first char from the initial deletion.
                        if (useFirstDeletedCharTime)
                        {
                            if (node.Edit != null)
                            {
                                firstIntialDelCharTime = node.Edit.StartTime;
                                useFirstDeletedCharTime = false;
                            }
                        }
                        // We don't know how many deletes may follow, so we trace them all.
                        // Overwriting the previous ensures we catch the final node.
                        // We might need it to adjust the time of the final word boundary even 
                        // if the delete revision itself is not taken into account because 'delayed'.
                        else if (isLastInsertion || isLastDeletion)
                        {
                            if (node.Edit != null)
                            {
                                lastDelEdit = node.Edit;
                                lastInitialDelCharTime = lastDelEdit.EndTime;
                            }
                        }
                        // If this symbol happens to be a suspicious space, 
                        // we remove the _isSuspiciousSpace flag.
                        if (listNode.Contains('\u01C2'))
                        {
                            _isSuspiciousSpace--;
                        }
                        continue;
                    }

                    // When we encounter an insert inside a word of revision type 'IMMEDIATE',
                    // we take its time data to overwrite the final normal char.
                    if (isNotTypeChar && isLastInsertion == false && IsInsert(node))
                    {
                        string[] rev = Regex.Split(node.Symbol, @"\D+");
                        if (thisRevisions[Convert.ToInt32(rev[1])] == Token.RevisionType.IMMEDIATE)
                        {
                            insertedEdit = nodeArray[i - 1].Edit;
                        }
                    }

                    // Ignoring anything not alfanumeric.
                    if (i == 0 && !node.Symbol.All(char.IsLetterOrDigit) && !node.Symbol.All(char.IsPunctuation))
                        // (node.Symbol.Contains('\n') || node.Symbol.Contains('\u00B7')))
                    {
                        continue;
                    }

                    var edit = node.Edit;
                    if (edit == null) continue;

                    if (firstIntialDelCharTime > 0)
                    {
                        edit.StartTime = firstIntialDelCharTime;
                        firstIntialDelCharTime = 0;
                    }

                    allEdits.Add(edit);
                    result.Append(node.Symbol);
                }

                CurrentSymbolPosition += listCounter;
            }
            catch (IndexOutOfRangeException e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR, "Index out of range");
            }

            // Postprocessing the edits
            // If there was an initial delete, the start time of the word boundary (final white space, mouse, ect.)
            // needs to be adjusted, because the boundary comes after the deletion
            // but before the new chars, what would result in a faulty AfterWord Pause.
            //
            // If there was an insert it will overwrite the time data of the last normal char.
            // This time may be overwritten again by last deleted edit.
            //
            // If there was a deletion at the end, the last produced char gets the time information 
            // from the last deleted character.

            var editCounter = allEdits.Count - 1;

            if (lastInitialDelCharTime > 0 && !isLastDeletion)
            {
                ulong delta = 0;
                // The very last edit holds the word boundary (space, mouse)
                var boundary = allEdits[editCounter];
                if (boundary.Type == EditType.TypeChar)
                {
                    if (lastInitialDelCharTime < boundary.StartTime)
                    {
                        delta = boundary.StartTime - lastInitialDelCharTime;
                    }
                }
                var lastChar = allEdits[editCounter - 1];
                if (lastChar != null && lastChar.Type == EditType.TypeChar)
                {
                    if (lastDelEdit != null)
                    {
                        boundary.StartTime = lastChar.EndTime + delta;
                    }
                }
            }

            if (insertedEdit != null)
            {
                // The last but one  edit is the final character of this word.
                var lastChar = allEdits[editCounter - 1];
                if (lastChar != null && lastChar.Type == EditType.TypeChar)
                {
                    lastChar.StartTime = insertedEdit.StartTime;
                    lastChar.EndTime = insertedEdit.EndTime;
                }
            }
            if (isLastDeletion)
            {
                // The last but one edit is the final character of this word.
                if (editCounter - 1 > allEdits.Count)
                {
                    var lastChar = allEdits[editCounter - 1];
                    if (lastChar != null && lastChar.Type == EditType.TypeChar)
                    {
                        if (lastDelEdit != null)
                        {
                            lastChar.StartTime = lastDelEdit.StartTime;
                            lastChar.EndTime = lastDelEdit.EndTime;
                        }
                    }
                }
            }

            // Removing superfluous edits resulting from overshooting the inital input list.
            var countBack = 0;
            // If there are one or more 'suppiciousSpace' left, the length of the result set is too long.
            // We find out by looking at the last char of the result: it should be a middle dot and not
            // the char of the next word.
            if (_isSuspiciousSpace > 0)
            {
                char ed = '\0';
                if (result.Length >= 2)
                {
                    ed = result[result.Length - 1];
                }
                countBack = _isSuspiciousSpace;
                _isSuspiciousSpace = 0;
                if (ed != '\u00B7' && !char.IsPunctuation(ed))
                {
                    result = RollBack(allEdits, result, countBack);
                }
            }

            // Check if the middle dot word terminator or a punctuation symbol exists in the nodeArray 
            // at the end of the current input, if not our input.Length is one character too long.
            try
            {
                SymbolNode c = nodeArray[listCounter - 1];
                SymbolNode d = nodeArray[listCounter];
                if (!c.Symbol.Equals("\u00B7") && !char.IsPunctuation(c.SymbolChar) && result.Length > 0)
                {
                    if (d.Symbol.Equals("\u00B7") || char.IsPunctuation(d.SymbolChar))
                    {
                        ResultString = result.ToString();
                    }
                    else
                    {
                        countBack++;
                        ResultString = RollBack(allEdits, result, countBack).ToString();
                    }
                }
                else
                {
                    ResultString = result.ToString();
                }
            }
            catch (Exception)
            {
                ResultString = result.ToString();
            }

            // Back to the WordsReconstruction with an EditList.
            return allEdits;
        }

        /// <summary>
        /// RollBack removes edits resulting from overshooting the inital input list.
        /// </summary>
        /// <param name="allEdits">All edits collected for the input</param>
        /// <param name="result">Reconstructed word</param>
        /// <param name="countBack">NUlber of steps to go back</param>
        /// <returns></returns>
        private StringBuilder RollBack(List<IEdit> allEdits, StringBuilder result, int countBack)
        {
                CurrentSymbolPosition -= countBack;
                int remove = Math.Min(allEdits.Count, countBack);
                allEdits.RemoveRange(allEdits.Count - countBack, remove);
                result = result.Remove(result.Length - countBack, countBack);         

            return result;
        }

        /// <summary>
        /// Content inside a deletion is skipped.
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <param name="markUp">Counting the deletion symbols</param>
        /// <returns>'true' if inside a deletion</returns>
        private bool IsDeletion(SymbolNode node, ref int markUp)
        {
            if (_leftBracketRegex.IsMatch(node.Symbol))
            {
                markUp++;
            }
            if (_rightBracketRegex.IsMatch(node.Symbol))
            {
                markUp--;
            }
            return markUp > 0;
        }

        /// <summary>
        /// If we encounter a break symbol, the CurrentSymbolCounter is
        /// incremented by one, in order to keep the Inputlist syncronized.
        /// Break symbols are in the Symbols list but not in the word representation.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true if a break symbol</returns>
        private bool IsBreak(SymbolNode node)
        {
            return _regexBreak.Match(node.Symbol).Success;
        }

        /// <summary>
        /// We take the time data of the last inserted char to replace
        /// the time info of the last normal char. 
        /// An insert comes by definition after the last normal production.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsInsert(SymbolNode node)
        {
            return _rightInsertRegex.IsMatch(node.Symbol);
        }

        /// <summary>
        /// Putting all markup symbols forming one unit into one string so as to align with
        /// the SymbolNode array where one markup with all its digits, braces and brackets is one node.
        /// </summary>
        /// <param name="word">The word to handle.</param>
        /// <returns>List of strings with the relevant markup symbols grouped in one string.</returns>
        private IEnumerable<string> FindMarkupSymbols(string word)
        {
            List<KeyValuePair<int, string>> nodeList = new List<KeyValuePair<int, string>>();
            foreach (RegexDefinition regEx in _regexList)
            {
                MatchCollection matches = regEx.Regex.Matches(word);
                foreach (Match markup in matches)
                {
                    nodeList.AddRange(from Capture capture in markup.Captures
                        where word.Length > 0
                        select new KeyValuePair<int, string>(capture.Index, capture.Value));
                }
            }

            char[] charArray = word.ToCharArray();
            var charList = new List<KeyValuePair<int, string>>();
            for (int index = 0; index < charArray.Length; index++)
            {
                var c = charArray[index];

                // We expect a middle dot as a placeholder for spaces. When an unmarked space is observed,
                // there might be a misalignment between the nodeList and the Symbols with a result string that is too long
                // because this unmarked space will be overlooked.
                if (c.ToString().Equals(" "))
                {
                    // Replace space with a double pipe so as to recognize it later
                    c = '\u01C2';
                    // We'll check in a postprocessing step if there really was a space too much.
                    _isSuspiciousSpace++;
                }
                charList.Add(new KeyValuePair<int, string>(index, c.ToString()));
            }

            foreach (KeyValuePair<int, string> p in nodeList)
            {
                var p1 = p;
                var targetIdx = charList.FindIndex(n => n.Key.Equals(p1.Key));
                if (targetIdx != -1)
                {
                    charList[targetIdx] = p;
                    charList.RemoveRange(targetIdx + 1, p.Value.Length - 1);
                }
                else
                {
                    charList.Add(p);
                }
            }
            charList.Sort(Compare);

            // Returning only the string not the keyValue pair.
            return charList.Select(d => d.Value).ToList();
        }

        // Comparing on the index of the keyValue pairs.
        private static int Compare(KeyValuePair<int, string> a, KeyValuePair<int, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }
    }
}