using InputLog.Core.Analyses;

namespace GUI.Tabs.Analyze.AnalysesControls.SNotation
{
    public class SNotationAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "S-Notation";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "SN";

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            // PauseThreshold = (ulong)PauseThresholdField.Value;
            // Debug: commented out RevisionAnalysis and controlKeys
            // List of keys that act as control keys (these should be represented with '+' 
            // when they are present in the keyboard state), e.g if LSHIFT is part of ControlKeys, 
            // then pressing LSHIFT and 'a' at the same time will result in LSHIFT + A.
            // var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);
            //return new InputLog.Core.Analyses.Revision.RevisionAnalysis.RevisionAnalysis(events, eventFilters, "", PauseThreshold,
            //    controlKeys, startTimeOffset);
            return new InputLog.Core.Analyses.Revision.Notations.SNotationAnalysis(Events, SessionId, OrgDocPath, 0);
        }
    }
}
