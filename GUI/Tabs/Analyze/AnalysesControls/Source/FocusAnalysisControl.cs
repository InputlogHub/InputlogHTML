using System.ComponentModel;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Source
{

    /// <summary>
    /// This class provides the controls for a SourceAnalysis.
    /// InputFields:
    /// - destination path
    /// </summary>
    public partial class FocusAnalysisControl : AnalysisControl
    {

        #region Fields

        /// <summary>
        /// Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
            public const string NUMBER_OF_INTERVALS = "NumberOfIntervals";
        }

        protected override string NAME => FocusAnalyzer.NAME;

        #endregion

        /// <summary>
        /// Constructs a SourceAnalysis.
        /// </summary>
        public FocusAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, MoreInfoLabel, FocusAnalyzer.ABBR);
            InitTooltips();
        }

        /// <summary>
        ///     Initialize the tooltip information.
        /// </summary>
        private void InitTooltips()
        {
           PajekTooltip.ShowAlways = true;
           PajekTooltip.SetToolTip(AddPajekFileCBx,
                "This visualization file (with .net extension) allows you to represent " +
                "\nthe relations between the different sources as a network." +
                "\nThe nodes are the sources and the arrows represent the transition" +
                "\nfrom one source to the next." +
                "\nUse a network viewer that accepts the Pajek format (Pajek, Gephi,...).");
        }

        /// <summary>
        /// Gets the number of intervals from the GUI.
        /// </summary>
        private int GetIntervalParam()
        {
            return int.Parse(NumberOfIntervalsField.Text);
        }

        /// <summary>
        /// Validates the NumberOfIntervalsField (checks whether the entered value is a ulong).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NumberOfIntervalsFieldValidating(object sender, CancelEventArgs e)
        {
            e.Cancel = !InputValidation.AssertParse<ulong>(NumberOfIntervalsField.Text, 2, 100);
            IntervalNmbrErrorProvider.SetError(FixedNumberOfIntervalsPanel, e.Cancel ?
                "The number of intervals must be >= 2 and <= 100." : "");
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new FocusAnalyzer(RunGetTS(GetIntervalParam),
                RunGetTS(() => AddPajekFileCBx.Checked)
            );
        }

        /// <summary>
        /// Exports the configuration of this Analysis Control to a AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration
        ///  (= the input fields of this control) of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);
            config.Parameters.Add(ConfigurationParameters.NUMBER_OF_INTERVALS,
                NumberOfIntervalsField.Text);
            return config;
        }

        /// <summary>
        /// Imports a configuration into this Analysis Control from an AnalysisConfiguration.
        /// </summary>
        public override void Import(AnalysisConfiguration configuration)
        {
            NumberOfIntervalsField.Text =
               configuration.TryGetParameter(ConfigurationParameters.NUMBER_OF_INTERVALS,
               NumberOfIntervalsField.Text);
        }
    }
}