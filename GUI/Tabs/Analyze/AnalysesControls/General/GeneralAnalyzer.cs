using System;
using System.Collections.Generic;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.General;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Util.KeyConversion;

namespace GUI.Tabs.Analyze.AnalysesControls.General
{
    public class GeneralAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Whether a csv file should be produced
        /// </summary>
        private readonly bool IncludeCsv;

        /// <summary>
        /// Whether revision info should be included
        /// </summary>
        private readonly bool IncludeRevisions;

        /// <summary>
        /// List of keys that act as control keys 
        /// (these should be represented with '+' when they are present in the keyboard state).
        /// e.g if LSHIFT is part of ControlKeys, then pressing LSHIFT and 'a' at the same time 
        /// will result in LSHIFT + A in the representation. If LSHIFT is not part of ControlKeys, then pressing LSHIFT
        ///  and  'a' at the same time will result in 2 different keystrokes.
        /// </summary>
        protected readonly List<KeysEx> CtrlKeys;

        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "General";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "GA";

        /// <summary>
        /// Variables to mark an logging interval based on a period in minutes (FixedIntervals) or based on
        /// a partition of the logging in a certain number of intervals (NumberIntervals)
        /// </summary>
        public ulong FixedIntervals;
        public int NumberIntervals;

        private List<RevisionMatrixEntry> Revisions;

        public GeneralAnalyzer(bool includeCsv, bool includeRevisions, int numberIntervals, ulong fixedIntervals, List<KeysEx> ctrlKeys)
        {
            IncludeCsv = includeCsv;
            IncludeRevisions = includeRevisions;
            CtrlKeys = ctrlKeys;
            FixedIntervals = fixedIntervals;
            NumberIntervals = numberIntervals;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new GeneralAnalysis(Events, SessionId, ABBR, NumberIntervals, FixedIntervals, CtrlKeys);
        }

        protected override void AfterAnalysis(Analysis analysis, IAnalysisSummary summary)
        {
            try
            {
                if (IncludeRevisions)
                {
                    var revAnalysis = new RevisionMatrixAnalysis(Events, SessionId, OrgDocPath, false);
                    var revSummary = (RevisionMatrixSummary)revAnalysis.DoAnalysis();
                    Revisions = revSummary.Entries;
                }
            }
            catch (Exception)
            {
                // TODO proper exception handling
                Revisions = new List<RevisionMatrixEntry>();
            }
        }

        protected override bool BeforeWrite(IAnalysisWriter writer)
        {
            if (IncludeCsv) GeneralAnalysisXMLWriter.GenerateCSV = true;
            if (IncludeRevisions)
            {
                ((GeneralAnalysisXMLWriter)writer).AddRevisions(Revisions);
            }
            return true;
        }
    }

}
