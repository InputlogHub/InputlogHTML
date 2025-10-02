using System;
using GUI.Tabs.Analyze.ImportExport;

namespace GUI.Tabs.Analyze.AnalysesControls.Summary
{
    /// <summary>
    /// This class provides the controls for a SummaryAnalysis.
    /// </summary>
    public partial class SummaryAnalysisControl : AnalysisControl
    {
        #region Fields

        /// <summary>
        /// Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
        }

        protected override string NAME { get { return SummaryAnalyzer.NAME; } }
        #endregion

        /// <summary>
        /// Constructs a SummaryAnalysis.
        /// </summary>
        public SummaryAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, MoreInfoLabel, SummaryAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new SummaryAnalyzer();          
        }

        /// <summary>
        /// Exports the configuration of this Control to a AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration 
        /// (= the input fields of this control) of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);
            return config;
        }

        /// <summary>
        /// Imports a configuration into this Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
        }
    }
}