using GUI.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.General
{
    public class GeneralEyetrackAnalysisControl : GeneralAnalysisControl
    {
        public GeneralEyetrackAnalysisControl()
        {
            NameLabel.Text = "General (Eyetracking)";
        }

        protected override string NAME
        {
            get { return GeneralEyetrackAnalyzer.NAME; }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GeneralEyetrackAnalysis
            // 
            Name = "GeneralEyetrackAnalysis";
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void ProduceAnalyzer()
        {
            var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);
            Analyzer = new GeneralEyetrackAnalyzer(
                RunGetTS(CsvChecked),
                RunGetTS(RevisionChecked),
                controlKeys
                );
        }
    }
}