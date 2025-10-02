using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    ///     Makes n-grams out of a 'production' word, i.e. the string that includes 
    ///     the normal production, and eventually all deletions and insertions. 
    ///     Pause times are added. 
    /// </summary>
    public class NGramMaker
    {
        #region Fields
        /// <summary>
        /// The current word position may be different from the initial word position
        /// because the word is extended with additional characters.
        /// </summary>
        private int CurrentWordPosition { get; set; }
        private int OrgWordPosition { get; set; }
        // Counting the number of failed tries in the search2 for a match with the Symbols.
        private int Failure;
        // Getting the edits for every character of a targeted word.
        private readonly TokenEdits TimeMaker;
        // Length of the n-gram, hard coded as a bigram for now.
        private const int N_GRAM = 2;
        #endregion

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="symbols">LinkedList with all elements on which the text reconstruction is based.</param>
        public NGramMaker(LinkedList<SymbolNode> symbols)
        {
            TimeMaker = new TokenEdits(symbols);
        }

        /// <summary>
        ///     The word associated with a 'target' word from the TokenDictionary is converted
        ///     into character n-grams. A pause time is associated with each n-gram.
        /// </summary>
        /// <param name="tokenDictionary">A collection of 'Token' objects extracted from the current idfx.</param>
        public void GetNGrams(Dictionary<int, Token> tokenDictionary)
        {

            // Keeping track of the position of the actual token
            // Sorting the TokenDictionary on the index in the Token object.
            var sortedDict = tokenDictionary.OrderBy(entry => entry.Value.DictEntry);
            foreach (var pair in sortedDict)
            {
                Token token = pair.Value;
                Failure = 0;

                // Skip if there is no token.
                if (token.TokenProcess == Token.ProcessType.DELETED 
                    || token.TokenProcess == Token.ProcessType.PUNCT)
                {
                    continue;
                }
                // Skip if target word is empty.
                if (token.TargetWord.Length == 0)
                {
                    continue;
                }

                // Getting the Production word together with all its deletions
                // and insertions (if any) only if there exists a Target.
                // The 'word' is extended to include the last character from
                // the preceding word and the spaces/punctuation between.
                string word = token.Production;

                // The start position of the word.
                // An exception occurs when no position was found.
                try
                {
                    CurrentWordPosition = token.Position;
                }
                catch (Exception)
                {
                    Debug.WriteLine("Position not found for: '" + word +
                        "'. Revision: " + string.Join(", ", token.TokenRevisions));
                }
  
                // The word before applying the extension.
                var orgWord1 = word;
                var orgWord = RemoveBraces(word);
                OrgWordPosition = CurrentWordPosition;

                // Start of the word in the reconstructed text.
                int prevIndex = 0;
                var prevPair = new KeyValuePair<int, Token>();
                foreach (var tokenPair in tokenDictionary)
                {
                    if (tokenPair.Value == token)
                    {
                        prevIndex = prevPair.Key;
                        break;
                    }
                    prevPair = tokenPair;
                }
                var prevWord = tokenDictionary[prevIndex].Production;

                // Extend the word in view with the last character of the previous word.
                if (CurrentWordPosition > 3)
                {
                    var sb = new StringBuilder();
                    for (int i = prevWord.Length - 1; i >= 0; i--)
                    {
                        char s = prevWord[i];
                        if (!Char.IsLetterOrDigit(s))
                        {
                            CurrentWordPosition--;
                            sb.Append(s);         
                        }
                        else
                        {
                            CurrentWordPosition--;
                            sb.Append(s);
                            break;
                        }
                    }
                    char[] arr = sb.ToString().ToCharArray();
                    Array.Reverse(arr);
                    word = new string(arr) + word;

                    // Exporting the extended word to Token.
                    token.Production = word;
                }

                // We try to get timed info on the revisions of this word, 
                // but obviously not on its markup symbols.
                if (token.TokenProcess.Equals(Token.ProcessType.REVISED))
                {
                    word = RemoveBraces(word);
                }

                // Getting the edits for each character.
                var timeEdits = TimeMaker.GetTokenEdits(word, CurrentWordPosition);

                // Retrying without the extensions when failing to find the matching edits. 
                // We do this only once.
                if (timeEdits.Count == 0 && Failure == 0)
                {
                    Failure++;
                    CurrentWordPosition = OrgWordPosition;
                    token.Production = orgWord1;
                    word = orgWord;
                    timeEdits = TimeMaker.GetTokenEdits(orgWord, CurrentWordPosition);
                }

                // Constructing the n-grams and adding their pause time.    
                var pauses = CalculatePauseTime(timeEdits, word);
   
                int arrLength = word.Length - (N_GRAM - 1);
                for (int i = 0; i < arrLength; i++)
                {
                    token.NGrams.Add(i, new Pair<string, ulong>
                    {
                        First = (word.Substring(i, N_GRAM)),
                        Second = pauses[i]
                    });
                }
            }
        }

        /// <summary>
        ///  Calculates the difference between the start time of the last character and
        ///  the start time of the first (including normal, deletions and insertions).
        /// </summary>
        /// <param name="editList">List with edits (IEdit) for every charcacter.</param>
        /// <param name="word">the word string.</param>
        /// <returns>List (ulong) with the StartTime for every char of word.</returns>
        private List<ulong> CalculatePauseTime(List<IEdit> editList, string word)
        {
            // Filling missing value slots with null.
            if (editList.Count < word.Length)
            {
                int toAdd = (word.Length - editList.Count);
                for (int i = 0; i < toAdd; i++)
                {
                    editList.Add(null);
                }
            }
            var pauses = new List<ulong>();

            for (int j = 0; j < (editList.Count - 1); j++)
            {
                ulong pause;
                if (null == editList[j] || null == editList[j + 1] 
                    || editList[j].StartTime > editList[j + 1].StartTime)
                {
                    pause = 0;
                }
                else
                {
                    pause = editList[j + 1].StartTime - editList[j].StartTime; 
                }
                pauses.Add(pause);
            }
            return pauses;
        }

        /// <summary>
        ///  Temporary removal of all braces and brackets.
        /// </summary>
        /// <param name="word">The string to process.</param>
        /// <returns>The word stripped from braces and brackets.</returns>
        private string RemoveBraces(string word)
        {
            word = Regex.Replace(word, @"[\[\]{}]", "");
            return word;
        }
    }
}