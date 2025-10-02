using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.General
{
    /// <summary>
    ///     This class provides the controls for a GeneralAnalysis.
    ///     InputFields:
    ///     - destination path
    ///     - generate csv file
    ///     - add revision data to the output
    ///     - set fixed number of intervals
    ///     - set interval size in minutes
    /// </summary>
    public partial class GeneralAnalysisControl : AnalysisControl
    {
        /// <summary>
        ///     Constructs a GeneralAnalysis.
        /// </summary>
        public GeneralAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, MoreInfoLabel, GeneralAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);
            Analyzer = new GeneralAnalyzer(RunGetTS(CsvChecked), RunGetTS(RevisionChecked), IntervalSize(), FixedIntervals(),controlKeys);
        }

        protected bool CsvChecked()
        {
            return CsvCBx.Checked;
        }

        protected bool RevisionChecked()
        {
            return RevisionCBX.Checked;
        }

        /// <summary>
        /// Variables to mark an logging interval based on a period in minutes (IntervalLength) or based on
        /// a partition of the logging in a fixed number of intervals (NumberOfIntervals)
        /// </summary>
        /// <returns></returns>
        private ulong FixedIntervals()
        {
            return (ulong) fixedInterval.Value;
        }

        private int IntervalSize()
        {
            return (int) numberOfIntervals.Value;
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
        ///     Imports a configuration into this GeneralAnalysis Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
        }

        #region Fields

        /// <summary>
        ///     Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
        }

        protected override string NAME
        {
            get { return GeneralAnalyzer.NAME; }
        }

        #endregion
    }
}