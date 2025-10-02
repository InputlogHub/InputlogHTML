using System;
using GUI.Tabs.Analyze.ImportExport;
using InputLog.Core.Analyses.Bigram;

namespace GUI.Tabs.Analyze.AnalysesControls.Bigram
{
    /// <summary>
    ///     This class provides the controls for a BigramAnalysis.
    ///     InputFields:
    ///     - destination path
    /// </summary>
    public partial class BigramAnalysisControl : AnalysisControl
    {
        /// <summary>
        ///     Constructs a BigramAnalysis.
        /// </summary>
        public BigramAnalysisControl()
        {
            InitializeComponent();            
            Init(NameLabel, MoreInfoLabel, BigramAnalyzer.ABBR);
            FileTypeList.SelectedItem = "PNG";
            AvgStatistic.Items.Add(BigramAnalysis.Average.MEDIAN);
            AvgStatistic.Items.Add(BigramAnalysis.Average.MEAN);
            AvgStatistic.SelectedItem = BigramAnalysis.Average.MEDIAN;
        }

        protected override void ProduceAnalyzer()
        {
            var extension = RunGetTS(delegate { return FileTypeList.SelectedItem.ToString(); });
            Analyzer = new BigramAnalyzer(extension,
                RunGetTS(delegate { return (BigramAnalysis.Average) AvgStatistic.SelectedItem; }),
                RunGetTS(delegate { return Convert.ToInt32(PauseThresholdField.Value); }),
                null
                );
        }

        /// <summary>
        ///     Exports the configuration of this GeneralAnalysis Control to a AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>
        ///     A dictionary containing key-value pairs that define the configuration
        ///     (= the input fields of this control) of this analysis control.
        /// </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);

            return config;
        }

        /// <summary>
        ///     Imports a configuration into this BigramAnalysis Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
        }

        /// <summary>
        ///     Adapts the UI so to represent that the analysis has been finished.
        /// </summary>
        protected override void ChangeUItoFinished()
        {
            base.ChangeUItoFinished();
            AddGraphLink(delegate { ((BigramAnalyzer) Analyzer).Graph.Show(); });
        }

        #region Fields

        /// <summary>
        ///     Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public static string DESTINATION = "Destination";
        }

        protected override string NAME
                {
            get { return BigramAnalyzer.NAME; }
        }

        #endregion
    }
}