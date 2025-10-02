using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Events;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class WNotationAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// List of input events on which to perform the analysis.
        /// </summary>
        private readonly IList<IRevision> Revisions;

        /// <summary>
        /// Used for building the representation.
        /// </summary>
        private readonly NotationBuilder Builder;

        #endregion

        /// <summary>
        /// WNotationAnalysis Constructor
        /// </summary>
        /// <param name="events">Events from the logfile on which this analysis is performed.</param>
        /// <param name="docPath">The path of the document in the original state (before the logging happened). 
        /// If the file pointed to by the given path does not exists, an empty document will be used.</param>
        /// <param name="pauseThreshold">The used pause threshold</param>
        /// <param name="sessionIdentification"></param>
        public WNotationAnalysis(List<Event> events, string docPath, ulong pauseThreshold, 
            SessionIdentification sessionIdentification)
            : base("WN", events, sessionIdentification)
        {
            var revAnalysis = new RevisionAnalysis.RevisionAnalysis(events, sessionIdentification, 
                docPath,pauseThreshold);
            var revisionSummary = (RevisionAnalysisSummary)revAnalysis.DoAnalysis();
            Builder = new NotationBuilder(new WNotationMarkup(), sessionIdentification.GetFileName(), revAnalysis.OriginalContent);
            Revisions = revisionSummary.Revisions;
        }

        /// <summary>
        /// Loops over the revisions and constructs a representation according to
        /// the W-Notation convention. Returns a list with the symbols and text from a revision analysis.
        /// </summary>
        /// <returns>A summary (list) with symbols and text.</returns>
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

            return new WNotationSummary(Builder.Symbols, new SNotationMarkup(), Builder.ToString());
        }
    }
}