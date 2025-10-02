using System.Collections.Generic;
using InputLog.Core.Analyses.General;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses.ProcessGraphAnalysis
{
    /// <summary>
    /// Analysis that runs a general analysis and subsequently creates a process
    /// graph based on the results from the general analysis.
    /// </summary>
    public class ProcessGraphAnalysis: Analysis
    {
        #region Fields
        /// <summary>
        /// A reference to the general analysis that needs to be run
        /// in order to construct the process graph
        /// </summary>
        private GeneralAnalysis General;

        /// <summary>
        /// Abbreviation for a ProgressGraphAnalysis.
        /// </summary>
        private const string ABBR = "PG";

        #endregion

        /// <summary>
        /// Create a new process graph analysis.
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="controlKeys">List of controlKeys that need to be taken into account.</param>
        public ProcessGraphAnalysis(List<Event> events, SessionIdentification sessionID, List<KeysEx> controlKeys = null)
            : base(ABBR, events, sessionID)
        {
            int numberOfIntervals = 0;
            ulong intervalSize = 0;
            this.General = new GeneralAnalysis(events, sessionID, ABBR, numberOfIntervals, intervalSize, controlKeys);
        }

        /// <summary>
        /// Perform the process graph analysis.
        /// </summary>
        /// <returns></returns>
        public override IAnalysisSummary DoAnalysis()
        {
            GeneralAnalysisSummary gaSummary = (GeneralAnalysisSummary)this.General.DoAnalysis();
            ProcessGraphAnalysisSummary pgSummary = new ProcessGraphAnalysisSummary();
            pgSummary.GASummary = gaSummary;
            return pgSummary;
        }
        
    }
}
