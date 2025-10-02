using System.Collections.Generic;
using InputLog.Core.Analyses;
using InputLog.Core.Util.KeyConversion;

namespace GUI.Tabs.Analyze.AnalysesControls.General
{
    public class GeneralEyetrackAnalyzer : GeneralAnalyzer
    {
        public new const string NAME = "General - Condensed Eyetrack";
        public new const string ABBR = "GEA";
        static int numberOfIntervals = 0;
        static ulong intervalSize = 0;
        public GeneralEyetrackAnalyzer(bool includeCsv, bool includeRevisions, List<KeysEx> ctrlKeys)
            : base(includeCsv, includeRevisions, numberOfIntervals, intervalSize, ctrlKeys){}

        protected override Analysis GetAnalysis()
        {
            return new InputLog.Core.Analyses.GeneralEyetrack.GeneralEyetrackAnalysis(Events, SessionId, ABBR, CtrlKeys);
        }
    }
}
