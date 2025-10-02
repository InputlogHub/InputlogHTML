using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// A token is an object representing a string between word bounderies. It has a position, type, 
    ///  a collection of revision numbers, and different forms of realization: 
    /// * TargetWord - the word in its correct orthography (the cannonical form);
    /// * Reconstruction - the production stripped from all deletions and augmented with all insertions; 
    /// * Production - a simplified S-Notation showing the normal production with all the inserts and deletions. 
    /// If there is a 'TargetWord' the token also has a Dictionary with n-grams.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// A word is either the result from a normal production, or has been revised (characters added or removed),
        /// or has been deleted all together, or was copied from another location.
        /// </summary>
        public enum ProcessType { NORMAL, REVISED, DELETED, COPIED, PUNCT }

        /// <summary>
        ///  A revision is 'immediate' when it occurs between the revised word and the next word, i.e. only
        /// spaces and puntuation is allowed between a revision and its break symbol. In a 'delayed' revision
        /// there are one or more words between the break symbol and the revision. The 'NONE' type is for
        /// the zero-revision.
        /// </summary>
        public enum RevisionType { IMMEDIATE, DELAYED, NONE } 

        #region Properties
        /// <summary>
        /// Index of this token in the TokenDictionary.
        /// </summary>
        public int DictEntry { get; set; }
        /// <summary>
        ///  Start position of this token in the reconstructed text.
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// The different process types are 'normal', deleted and 'revised'.
        /// </summary>
        public ProcessType TokenProcess { get; set; }
        /// <summary>
        /// A targetWord is the 'official' orthography of this token. Used by 'Token Analysis' and 'WordPause Analysis'.
        /// </summary>
        public string TargetWord { get; set; }
        /// <summary>
        /// The token in its final state after deleting and inserting the revisions.
        /// </summary>
        public string Reconstruction { get; set; }
        /// <summary>
        /// Set of revisions numbers and the associated revision type related to the production of this token.
        /// </summary>
        public SortedDictionary<int, RevisionType> TokenRevisions { get; private set; }
        /// <summary>
        /// The token with its process info (S-Notation without the numbers).
        /// </summary>
        public string Production { get; set; }
        /// <summary>
        /// N-gram information: a token split into character n-grams (currently hard coded as a bigram)
        /// with for each n-gram its pause time.
        /// </summary>
        public Dictionary<int, Pair<string, ulong>> NGrams { get; private set; }
        /// <summary>
        /// ArrayList with the edits for this token.
        /// </summary>
        public List<IEdit> EditList { get; private set; }
        /// <summary>
        /// The string constructed with the Symbol nodes from the Revision Analysis.
        /// It should be the same as Production string without the WNotation markups.
        /// </summary>
        public string ResultString { get; set; }
        /// <summary>
        /// The number characters produced in this token.
        /// </summary>
        public int CharCount { get; set; }
        /// <summary>
        /// The PauseTime after the last character of the previous token.
        /// </summary>
        public ulong BeforeWord2 { get; set; }
        /// <summary>
        /// The PauseTime before the first character of the token.
        /// </summary>
        public ulong BeforeWord1 { get; set; }
        /// <summary>
        /// The production time for the whole token.
        /// </summary>
        public ulong WordProduction { get; set; }
        /// <summary>
        /// The pause time for the whole token.
        /// </summary>
        public ulong WordPause { get; set; }
        /// <summary>
        /// The PauseTime for the last previous event.
        /// </summary>
        public ulong AfterWordPause { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Token()
        {
            TokenRevisions = new SortedDictionary<int, RevisionType>();
            NGrams = new Dictionary<int, Pair<string, ulong>>();
            EditList = new List<IEdit>();
            TargetWord = "";
            ResultString = "";
        }

        /// <summary>
        /// Verbose description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Idx: {0}, Process Type: \"{1}\", Target: \"{2}\", Reconstructed: \"{3}\", " +
                                   "Produced: \"{4}\", Result: \"{5}\", Revisions: \"{6}\", Position: {7}," +
                                 " #Edits: {8}, #Chars: {9}",
                                   DictEntry, TokenProcess, TargetWord, Reconstruction, Production, ResultString,
                                   string.Join(", ", TokenRevisions.Keys), Position, EditList.Count, CharCount);
        }
    }
}
