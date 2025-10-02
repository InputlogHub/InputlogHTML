using System;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;

namespace GUI.Tabs.Analyze.AnalysesControls.Linguistic
{
    public sealed partial class LinguisticAnalysisControl : ServerAnalysisControl
    {
        protected override String NAME { get { return LinguisticAnalyzer.NAME; } }

        /// <summary>
        /// Constructs a Linguistic Analysis.
        /// </summary>
        public LinguisticAnalysisControl()
        {
            InitializeThisComponent();
            Init(NameLabel, MoreInfoLabel, LinguisticAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new LinguisticAnalyzer();
        }

        /// <summary>
        /// Exports the configuration of this WNotation Control to an AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration 
        /// (= the input fields of this control) of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);
            //config.Parameters.Add(ConfigurationParameters.PAUSETHRESHOLD, PauseThresholdField.Value.ToString());
            return config;
        }

        /// <summary>
        /// Imports a configuration into this WNotation Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
            // var pauseThresholdStr = configuration.TryGetParameter(ConfigurationParameters.PAUSETHRESHOLD,
            // PauseThresholdField.Value.ToString());
            // PauseThresholdField.Value = (decimal)Convert.ChangeType(pauseThresholdStr, PauseThresholdField.Value.GetType());
        }

        protected override void AddOpenContainingFolderLink()
        {
            base.AddOpenContainingFolderLink();
            DynamicControls[FILE_LINK_ID].Visible = false;
        }

        public override ProgressBar GetProgressBar()
        {
            return progressBar1;
        }

        protected override Label GetStatusLabel()
        {
            return StatusLabel;
        }

        protected override Label GetIDLabel()
        {
            return IDLabel;
        }
    }

}
