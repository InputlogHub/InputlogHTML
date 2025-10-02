using System.Collections.Generic;
using System.Data;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;

namespace InputLog.Core.Analyses.WordPauses
{
    public class WordPausesAnalysisSummary : WNotationSummary
    {
        #region Fields
        public readonly DataSet WordPauseResults;
        private readonly WNotationSummary WSumm;
        #endregion

        /// <summary>
        ///// Changes W-Notation markup symbols into S-Notation to enhance human readability in the report.
        /// </summary>
        public IEnumerable<SymbolNode> SNotations
        {
            get { return WSumm.SNotationList; }
        }

        /// <summary>
        /// The Word Pause Analysis is similar to the Linguistic Analysis, save for the linguistic parts.
        /// This analysis shows only the revisions and pauses for the different tokens in the text.
        /// </summary>
        /// <param name="summ">Summary</param>
        /// <param name="wordPauseResults"></param>
        public WordPausesAnalysisSummary(WNotationSummary summ, DataSet wordPauseResults)
            : base(summ)
        {
            WSumm = summ;
            WordPauseResults = wordPauseResults;
        }
    }
}