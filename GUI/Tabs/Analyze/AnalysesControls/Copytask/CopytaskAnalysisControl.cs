using GUI.Tabs.Analyze.ImportExport;

namespace GUI.Tabs.Analyze.AnalysesControls.Copytask
{
    public partial class CopytaskAnalysisControl : AnalysisControl
    {
        public CopytaskAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, MoreInfoLabel, CopytaskAnalyzer.ABBR);
        }

        // Name of analysis
        protected override string NAME
        {
            get { return CopytaskAnalyzer.NAME; }
        }


        protected override void ProduceAnalyzer()
        {
            Analyzer = new CopytaskAnalyzer(cbRaw.Checked);
        }

        // Import export functionality

        #region Import/Export

        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);
            return config;
        }

        public override void Import(AnalysisConfiguration configuration)
        {
            // No import functionality
        }

        #endregion
    }
}