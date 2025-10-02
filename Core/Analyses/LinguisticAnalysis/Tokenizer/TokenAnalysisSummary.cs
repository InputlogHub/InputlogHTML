using System.Collections.Generic;
using System.Data;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    public class TokenAnalysisSummary : WNotationSummary
    {
        #region Fields
        public readonly DataSet LinguisticProcessResults;
        private readonly WNotationSummary WSumm;

        #endregion

        /// <summary>
        ///// Change W-Notation markup symbols into S-Notation to enhance readability in the W-Notation report.
        /// </summary>
        public IEnumerable<SymbolNode> SNotations
        {
            get { return WSumm.SNotationList; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="summ">Summary</param>
        /// <param name="linguisticProcessResults"></param>
        public TokenAnalysisSummary(WNotationSummary summ, DataSet linguisticProcessResults)
            : base(summ)
        {
            WSumm = summ;
            LinguisticProcessResults = linguisticProcessResults;
        }
    }
}
