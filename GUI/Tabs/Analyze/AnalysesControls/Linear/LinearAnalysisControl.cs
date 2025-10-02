using System;
using System.ComponentModel;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;
using LinearAnalysisType = InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE;

namespace GUI.Tabs.Analyze.AnalysesControls.Linear
{
    /// <summary>
    ///     This class provides the controls for a LinearAnalysis.
    /// </summary>
    public partial class LinearAnalysisControl : AnalysisControl
    {
        /// <summary>
        ///     Constructs a LinearAnalysis.
        /// </summary>
        public LinearAnalysisControl()
        {
            InitializeComponent();
            UpdateAnalysisType(this, null);
            Init(NameLabel, MoreInfoLabel, LinearAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new LinearAnalyzer(
                RunGetTS(GetIntervalParam),
                LinearAnalysisType,
                RunGetTS(delegate { return (ulong) PauseThresholdField.Value; }),
                RunGetTS(delegate { return SpecialCBx.Checked; })
                );
        }

        /// <summary>
        ///     Gets the intervalParameter from the GUI based on the selected analysis type.
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
            else if (PauseIntervals.Checked)
            {
                LinearAnalysisType = LinearAnalysisType.PAUSE_INTERVALS;
                FixedIntervalSizePanel.Enabled = false;
                FixedNumberOfIntervalsPanel.Enabled = false;
            }
        }

        /// <summary>
        ///     Validates the IntervalSizeField (checks whether the entered value is a ulong).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void IntervalSizeFieldValidating(object sender, CancelEventArgs e)
        {
            e.Cancel = !InputValidation.AssertParse<ulong>(IntervalSizeField.Text, 5, 600);
            IntervalSizeErrorProvider.SetError(FixedIntervalSizePanel, e.Cancel
                ? "The size of the interval in seconds must be >= 5 and <= 600."
                : "");
        }

        /// <summary>
        ///     Validates the NumberOfIntervalsField (checks whether the entered value is a ulong).
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

            config.Parameters.Add(ConfigurationParameters.PAUSETHRESHOLD, PauseThresholdField.Value.ToString());
            config.Parameters.Add(ConfigurationParameters.ANALYSIS_TYPE, LinearAnalysisType.ToString());
            config.Parameters.Add(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE, IntervalSizeField.Text);
            config.Parameters.Add(ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS,
                NumberOfIntervalsField.Text);

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
            IntervalSizeField.Text =
                configuration.TryGetParameter(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE,
                    IntervalSizeField.Text);
            NumberOfIntervalsField.Text = configuration.TryGetParameter
                (ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS, NumberOfIntervalsField.Text);
        }

        #region Fields

        /// <summary>
        ///     The type of linear analysis that will be performed.
        /// </summary>
        private LinearAnalysisType LinearAnalysisType;

        /// <summary>
        ///     Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
            public const string PAUSETHRESHOLD = "PauseThreshold";
            public const string ANALYSIS_TYPE = "AnalysisType";

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
            get { return LinearAnalyzer.NAME; }
        }

        #endregion
    }
}