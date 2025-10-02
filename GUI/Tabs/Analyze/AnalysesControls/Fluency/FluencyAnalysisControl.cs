using System;
using System.ComponentModel;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.Util;
using LinearAnalysisType = InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE;

namespace GUI.Tabs.Analyze.AnalysesControls.Fluency
{
    /// <summary>
    ///     This class provides the controls for a LinearAnalysis.
    /// </summary>
    public partial class FluencyAnalysisControl : AnalysisControl
    {
        /// <summary>
        ///     Constructs a FluencyAnalysis.
        /// </summary>
        public FluencyAnalysisControl()
        {
            InitializeComponent();
            UpdateAnalysisType(this, null);
            Init(NameLabel, MoreInfoLabel, FluencyAnalyzer.ABBR);

            // Fetch personal maximum
            const int defPersMax = FluencyAnalysis.DEFAULT_ABSOLUTE_MAXIMUM - 20;
            var oPersMax = RegistryTools.GetSetting(RegistryTools.DEFAULT_APP_NAME,
                "PersonalFluencyMaximum", defPersMax).ToString();
            personalMax.Value = int.Parse(oPersMax);
            FileTypeList.SelectedItem = "PNG";
            trendLineDegree.SelectedItem = "3";
            TaskMaximumTypeSelect.Items.Add(FluencyAnalysis.TaskMaximumMode.BASIC);
            TaskMaximumTypeSelect.Items.Add(FluencyAnalysis.TaskMaximumMode.INTERVAL_DEPENDENT);
            TaskMaximumTypeSelect.SelectedItem = FluencyAnalysis.TaskMaximumMode.BASIC;
        }

        protected override void ProduceAnalyzer()
        {
            IsMultipleFileAnalysis = SourceCount > 1;

            FluencyAnalyzer = new FluencyAnalyzer(
                RunGetTS(GetIntervalParam),
                LinearAnalysisType,
                RunGetTS(delegate { return (ulong) PauseThresholdField.Value; }),
                RunGetTS(delegate { return (FluencyAnalysis.TaskMaximumMode) TaskMaximumTypeSelect.SelectedIndex; }),
                RunGetTS(delegate { return CharProd.Checked; }),
                RunGetTS(delegate { return Convert.ToInt32(personalMax.Value); }),
                IsMultipleFileAnalysis,
                RunGetTS(delegate { return PersMaxRBtn.Checked; }),
                RunGetTS(delegate { return MultigrphCBx.Checked; }),
                SourceCount
                );
            FluencyAnalyzer.ChangeGraphImageExtension(
                RunGetTS(delegate { return FileTypeList.SelectedItem.ToString(); })
                );
            FluencyAnalyzer.ChangeTrendLineDegree(
                RunGetTS(delegate { return int.Parse(trendLineDegree.SelectedItem.ToString()); })
                );
            Analyzer = FluencyAnalyzer;
        }

        /// <summary>
        ///     Gets the intervalParameter from the GUI based on the selected analysis type.
        ///     -copy from linear-
        /// </summary>
        private ulong GetIntervalParam()
        {
            ulong result = 0;

            switch (LinearAnalysisType)
            {
                case LinearAnalysisType.FIXED_LENGTH_INTERVALS:
                    result = ulong.Parse(IntervalSizeField.Text)*1000;
                    break;

                case LinearAnalysisType.FIXED_NUMBER_OF_INTERVALS:
                    result = ulong.Parse(NumberOfIntervalsField.Text);
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Updates the UI and internal bookkeeping when the user switches between the different types of linear intervals.
        ///     -copy from linear-
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateAnalysisType(object sender, EventArgs e)
        {
            if (FixedIntervalSizeRadioButton.Checked)
            {
                LinearAnalysisType = LinearAnalysisType.FIXED_LENGTH_INTERVALS;
                FixedNumberOfIntervalsPanel.Enabled = false;
                FixedIntervalSizePanel.Enabled = true;
            }
            else if (FixedNumberOfIntervalsRadioButton.Checked)
            {
                LinearAnalysisType = LinearAnalysisType.FIXED_NUMBER_OF_INTERVALS;
                FixedIntervalSizePanel.Enabled = false;
                FixedNumberOfIntervalsPanel.Enabled = true;
            }
            else if (RevisionIntervals.Checked)
            {
                LinearAnalysisType = LinearAnalysisType.REVISION_INTERVALS;
                FixedIntervalSizePanel.Enabled = false;
                FixedNumberOfIntervalsPanel.Enabled = false;
            }
            else if (FocusIntervals.Checked)
            {
                LinearAnalysisType = LinearAnalysisType.FOCUS_INTERVALS;
                FixedIntervalSizePanel.Enabled = false;
                FixedNumberOfIntervalsPanel.Enabled = false;
            }
            if (FluencyAnalyzer != null) FluencyAnalyzer.ResetGraph();
        }

        /// <summary>
        ///     Validates the IntervalSizeField (checks whether the entered value is a ulong).
        ///     -copy from linear-
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void IntervalSizeFieldValidating(object sender, CancelEventArgs e)
        {
            e.Cancel = !InputValidation.AssertParse<ulong>(IntervalSizeField.Text, 5, 3600);
            IntervalSizeErrorProvider.SetError(FixedIntervalSizePanel, e.Cancel
                ? "The size of the interval in seconds must be >= 10 and <= 3600."
                : "");
        }

        /// <summary>
        ///     Validates the NumberOfIntervalsField (checks whether the entered value is a ulong).
        ///     -copy from linear-
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NumberOfIntervalsFieldValidating(object sender, CancelEventArgs e)
        {
            e.Cancel = !InputValidation.AssertParse<ulong>(NumberOfIntervalsField.Text, 1, 100);
            IntervalNmbrErrorProvider.SetError(FixedNumberOfIntervalsPanel, e.Cancel
                ? "The number of intervals must be >= 1 and <= 100."
                : "");
        }

        /// <summary>
        ///     Validates the the number of graphs to include in a multigraph.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void MultigraphValidating(object sender, CancelEventArgs e)
        {
            var validated = SourceCount > 1 && SourceCount <= 4;
            if (validated)
            {
                MultiGraphErrorProvider.SetError(MultigrphCBx, string.Empty);
                MultiGraphErrorProvider.Clear();
            }
            else
            {
                MultigrphCBx.Checked = false;
                MultiGraphErrorProvider.SetError(MultigrphCBx, "The number of source files must be > 1 and <= 4.");
            }
        }

        /// <summary>
        ///     Exports the configuration of this LinearAnalysis Control to an AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>
        ///     An analysis configuration that defines the configuration (= the input fields) of
        ///     this analysis control.
        /// </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);

            config.Parameters.Add(ConfigurationParameters.ANALYSIS_TYPE, LinearAnalysisType.ToString());
            config.Parameters.Add(ConfigurationParameters.PAUSETHRESHOLD, PauseThresholdField.Value.ToString());
            config.Parameters.Add(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE, IntervalSizeField.Text);
            config.Parameters.Add(ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS,
                NumberOfIntervalsField.Text);
            config.Parameters.Add(ConfigurationParameters.ONLY_CHAR_PRODUCTION, CharProd.Checked ? "1" : "0");
            config.Parameters.Add(ConfigurationParameters.TASK_MAXIMUM_MODE,
                TaskMaximumTypeSelect.SelectedIndex.ToString());
            return config;
        }

        /// <summary>
        ///     Imports a configuration into this LinearAnalysis Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
            // check correct radiobutton
            var analysisType = (LinearAnalysisType) Enum.Parse(typeof (LinearAnalysisType),
                configuration.TryGetParameter(ConfigurationParameters.ANALYSIS_TYPE,
                    LinearAnalysisType.FIXED_LENGTH_INTERVALS.ToString()), true);
            switch (analysisType)
            {
                case LinearAnalysisType.FIXED_LENGTH_INTERVALS:
                {
                    FixedIntervalSizeRadioButton.Checked = true;
                    break;
                }
                case LinearAnalysisType.FIXED_NUMBER_OF_INTERVALS:
                {
                    FixedNumberOfIntervalsRadioButton.Checked = true;
                    break;
                }
            }
            UpdateAnalysisType(this, null);
            var pauseThresholdStr = configuration.TryGetParameter(ConfigurationParameters.PAUSETHRESHOLD,
                PauseThresholdField.Value.ToString());
            PauseThresholdField.Value =
                (decimal) Convert.ChangeType(pauseThresholdStr, PauseThresholdField.Value.GetType());
            TaskMaximumTypeSelect.SelectedIndex = int.Parse(ConfigurationParameters.TASK_MAXIMUM_MODE);

            IntervalSizeField.Text =
                configuration.TryGetParameter(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE,
                    IntervalSizeField.Text);
            NumberOfIntervalsField.Text = configuration.TryGetParameter
                (ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS, NumberOfIntervalsField.Text);
            var sCharProd = configuration.TryGetParameter(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE, "0");
            CharProd.Checked = sCharProd.Equals("1");
        }

        /// <summary>
        ///     Stores selected personal fluency maximum
        /// </summary>
        private void SavePersOptLinkClicked(object sender, EventArgs e)
        {
            RegistryTools.SaveSetting(RegistryTools.DEFAULT_APP_NAME, "PersonalFluencyMaximum",
                personalMax.Value.ToString());
        }

        /// <summary>
        ///     Shows save-link next to personal maximum as soon as its value is changed
        /// </summary>
        private void PersonalMaxValueChanged(object sender, EventArgs e)
        {
            //SavePersOptLink.Visible = true;
        }

        /// <summary>
        ///     Adapts the UI so to represent that the analysis has been finished.
        ///     FluencyAnalysisControl adds a "Show Graph" link in the GUI
        ///     If analysis was unsuccessful (probably no intervals found), UI is *not* changed
        /// </summary>
        protected override void ChangeUItoFinished()
        {
            if (!FluencyAnalyzer.Success) return;
            base.ChangeUItoFinished();
            AddGraphLink(delegate
                         {
                             FluencyAnalyzer.FluencyGraph.Draw();
                             FluencyAnalyzer.FluencyGraph.Show();
                         });
        }

        private void ResetGraph(object sender, EventArgs e)
        {
            FluencyAnalyzer.ResetGraph();
        }

        public override void Close()
        {
            base.Close();
            if (FluencyAnalyzer != null) FluencyAnalyzer.ResetGraph();
        }

        private void ValidateMultipleFiles(object sender, EventArgs e)
        {
            if (!IsMultipleFileAnalysis)
            {
                MessageBox.Show("To use this option you should select a least 2 source files.",
                    "Processing Multiple Files", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        #region Fields

        private FluencyAnalyzer FluencyAnalyzer;

        /// <summary>
        ///     The type of linear analysis that will be performed.
        ///     -Copy from Linear-
        /// </summary>
        private LinearAnalysisType LinearAnalysisType;

        /// <summary>
        ///     Multiple files are processed.
        /// </summary>
        private bool IsMultipleFileAnalysis { get; set; }

        /// <summary>
        ///     Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string ANALYSIS_TYPE = "AnalysisType";
            public const string ONLY_CHAR_PRODUCTION = "OnlyCharProduction";
            public const string PAUSETHRESHOLD = "PauseThreshold";

            public const string TASK_MAXIMUM_MODE = "TaskMaximumMode";

            public static class FixedIntervalSize
            {
                public const string INTERVAL_SIZE = "FixedIntervalSize_IntervalSize";
            }

            public static class FixedNumberOfIntervals
            {
                public const string NUMBER_OF_INTERVALS = "FixedNumberOfIntervals_NumberOfIntervals";
            }
        }

        protected override string NAME
        {
            get { return FluencyAnalyzer.NAME; }
        }

        #endregion
    }
}