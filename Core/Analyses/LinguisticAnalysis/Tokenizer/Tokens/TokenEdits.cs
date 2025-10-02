using System;
using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Revisions.Edits;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// Getting the edits for every character of the targeted word in a TokenAnalysis.
    /// </summary>
    public class TokenEdits
    {
        #region Fields
        /// <summary>
        ///     The list of revisions & edits to link to the characters.
        /// </summary>
        private readonly LinkedList<SymbolNode> Symbols;

        /// <summary>
        ///     The current word position. May be different from the initial word position
        ///     when the word is extended with additional characters in NGramMaker.
        /// </summary>
        private int CurrentWordPosition { get; set; }
        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="symbols">LinkedList with all elements on which the text reconstruction is based.</param>
        public TokenEdits(LinkedList<SymbolNode> symbols)
        {
            Symbols = symbols;
        }

        /// <summary>
        ///     Collects the edits (IEdit) for every character of the argument.
        /// </summary>
        /// <param name="word">The string without markup symbols.</param>
        /// <param name="wordPosition">Start of the word in the text.</param>
        /// <returns>List with edits for every character of the 'word'.</returns>
        public List<IEdit> GetTokenEdits(string word, int wordPosition)
        {
            CurrentWordPosition = wordPosition;
            // The list with timed information.
            var times = new List<IEdit>();
            // Counting the number of symbols that have been matched already.
            int i = 0;

            // Holds the original value for the CurrentWordPosition. 
            // When getting a 'faulty match' we reset the wordPosition to the original position.
            int originalWordPosition = CurrentWordPosition;

            // Looking for the Symbols of the current word
            foreach (SymbolNode node in Symbols)
            {
                IEdit edit = node.Edit;
                if (edit != null)
                {
                    //Console.WriteLine("Word: " + word + " Word Pos: " + CurrentWordPosition 
                    //    + " Edit pos: " + edit.StartPos + " Symbol: " + node.Symbol);

                    // A little flexibility on the position. Sometimes the estimated position
                    // is at quite a distance from the real one.
                    if (edit.StartPos >= Math.Max(0, CurrentWordPosition - 20)
                        && edit.StartPos <= Math.Min(CurrentWordPosition + 20, Symbols.Count))
                    {
                        // Looking for the letters of the word.
                        string s = word[i].ToString();

                        //Console.WriteLine("Word " + word + " - '" + s + " node.Symbol "
                        //+ node.Symbol + "' CurrentPosition: " + CurrentWordPosition
                        //+ " node pos: " + node.Edit.StartPos); // + " - fail: " + Failure);

                        if (node.Symbol.Equals(s))
                        {
                            //Console.WriteLine("Word " + word + " equal chars");
                            //if (s.Equals("\u00B7")) Console.WriteLine("Word " + word + " space normal");

                            CurrentWordPosition = edit.StartPos + 1;
                            times.Add(edit);
                            i++;

                            if (i == word.Length) break;
                        }
                        else if (s.Equals("\u00B7"))
                        {
                            // 'Spaces' (middle-dot) without edit are captured here 
                            times.Add(null);
                            i++;
                            // Console.WriteLine("Word " + word + " space without edit" + " position " + CurrentWordPosition);  
                            if (i == word.Length) break;
                        }
                        else
                        {
                            // We were on a false trail. The previous match was a faulty one because 
                            // the next expected character did not appear. 
                            // 1. Clear the time list
                            // 2. Reset the word position to the originally expected wordposition.

                            /*
                             * QUIRK: The extension of the words to include one extra character
                             * do not take into account deleted characters. Thus if we get a missmatch
                             * on a deleted character (symbol with edit && positionCount == 0) we refrain
                             * from resetting the entire matching progress until we find a missmatch 
                             * from a non-deleted character.
                             * 
                             */
                            if (node.PositionCount > 0)
                            {
                                times.Clear();
                                CurrentWordPosition = originalWordPosition;
                                i = 0;
                            }
                        }
                    }
                }

                // Console.WriteLine("NO Edits " + " symbols count: " + Symbols.Count);
            }
            return times;
        }
    }
}