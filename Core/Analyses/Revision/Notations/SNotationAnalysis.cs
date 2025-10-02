using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Events;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class SNotationAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// List of input events on which to perform the analysis.
        /// </summary>
        private readonly IList<IRevision> _revisions;

        /// <summary>
        /// Used for building the representation.
        /// </summary>
        private readonly NotationBuilder _builder;
        #endregion

        /// <summary>
        /// SNotationAnalysis Constructor
        /// </summary>
        /// <param name="events"></param>
		/// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="docpath">The path of the document in the original state (before the logging happened). 
        /// If the file pointed to by the given path does not exists, an empty document will be used.</param>
        /// <param name="pauseThreshold">The used pause threshold</param>
        public SNotationAnalysis(List<Event> events, SessionIdentification sessionID, string docpath, ulong pauseThreshold)
            : base("SA", events, sessionID)
        {
			var revAnalysis = new RevisionAnalysis.RevisionAnalysis(events, sessionID, docpath, pauseThreshold);
            var revisionSummary = (RevisionAnalysisSummary)revAnalysis.DoAnalysis();
			_revisions = revisionSummary.Revisions;

            _builder = new NotationBuilder(new SNotationMarkup(), sessionID.GetFileName(), revAnalysis.OriginalContent);
        }

        /// <summary>
        /// Loops over the revisions and constructs a representation according to
        /// the S-Notation or the W-Notation convention.Returns a list with the symbols 
        /// and text from a revision analysis
        /// Called by GUI.Tabs.AnalysesControls.AnalysisControl
        /// </summary>
        /// <returns>A summary (list) with symbols and text.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            foreach (var revision in _revisions)
            {
                var rev = revision as InsertRevision;
                if (rev != null)
                {
                    _builder.InsertText(rev);
                }
                else
                {
                    var deleteRevision = revision as DeleteRevision;
                    if (deleteRevision != null)
                    {
                        try
                        {
                            _builder.DeleteRange(deleteRevision);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
            return new SNotationSummary(_builder.Symbols, _builder.Syntax);
        }
    }
}