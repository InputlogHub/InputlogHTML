using System.Collections.Generic;
using System.Data;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Pipes;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// The Token Analysis allows the researcher to compare production and process data with 
    /// words provided by a list of target words.
    /// </summary>
    public class TokenAnalysis : Analysis
    {
        #region Fields
        /// <summary>
        /// List of input events on which to perform the analysis.
        /// </summary>
        private readonly IList<IRevision> Revisions;

        /// <summary>
        /// Path to the CSV-file with the target words.
        /// </summary>
        private readonly string CsvPath;

        /// <summary>
        /// Used for building the representation.
        /// </summary>
        private readonly NotationBuilder Builder;
        private readonly string AnalysisAbbr;
        private readonly string Language;

        #endregion

        /// <summary>
        /// TokenAnalysis Constructor
        /// </summary>
        /// <param name="events">Events from the logfile on which this analysis is performed.</param>
        /// <param name="docPath">The path of the document in the original state (before the logging happened). 
        ///     If the file pointed to by the given path does not exists, an empty document will be used.</param>
        /// <param name="sessionIdentification"></param>
        /// <param name="abbrv">The abbreviation of the analysis that ordered this analysis.</param>
        /// <param name="csvFile"> Path to the CSV-file with the target words.</param>
        public TokenAnalysis(List<Event> events, string docPath, SessionIdentification sessionIdentification, 
            string abbrv, string csvFile)
            : base(abbrv, events, sessionIdentification)
        {
           
            CsvPath = csvFile;
            var revAnalysis = new RevisionAnalysis(events, sessionIdentification, docPath);
            var revisionSummary = (RevisionAnalysisSummary) revAnalysis.DoAnalysis();
            Builder = new NotationBuilder(new WNotationMarkup(), sessionIdentification.GetFileName(), revAnalysis.OriginalContent);
            Revisions = revisionSummary.Revisions;
            AnalysisAbbr = abbrv;
            Language = sessionIdentification.GetLanguage();
        }

        public override IAnalysisSummary DoAnalysis()
        {
            foreach (var revision in Revisions)
            {
                var rev = revision as InsertRevision;
                if (rev != null)
                {
                    Builder.InsertText(rev);
                }
                else
                {
                    var deleteRevision = revision as DeleteRevision;
                    if (deleteRevision != null)
                    {
                        Builder.DeleteRange(deleteRevision);
                    }
                }
            }
            var thisSummary = new WNotationSummary(Builder.Symbols, new SNotationMarkup(), Builder.ToString());

            var pipeline = new ProcessPipeline();
            var processResults = new DataSet("linguisticProcess");

            {
                pipeline.Register(new MarkupRemover(thisSummary)).
                         Register(new TokenReconstructor(thisSummary.RevisionTxt, thisSummary.SymbolList, 
                             Language, AnalysisAbbr, CsvPath)).
                         Execute(processResults);
                processResults = pipeline.Output;
                return new TokenAnalysisSummary(thisSummary, processResults);
            }
        }
    }
}