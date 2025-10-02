using GUI.Tabs.Analyze.ImportExport;

namespace GUI.Tabs.Analyze.AnalysesControls.SNotation
{
    public partial class SNotationAnalysisControl : AnalysisControl
    {
        #region Fields
        /// <summary>
        /// Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            // public const String PAUSETHRESHOLD = "PauseThreshold";
        }

        protected override string NAME { get { return SNotationAnalyzer.NAME; } }
        #endregion

        public SNotationAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, moreInfoLabel, SNotationAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new SNotationAnalyzer();
        }

        /// <summary>
        /// Exports the configuration of this SNotation Control to an AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration (= the input fields of this control) 
        /// of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);
            return config;
        }

        /// <summary>
        /// Imports a configuration into this SNotation Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
        }
    }
}