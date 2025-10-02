using System;
using System.ComponentModel;
using System.Globalization;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;
using InputLog.Core.Analyses.Pause;

namespace GUI.Tabs.Analyze.AnalysesControls.Pause
{
    /// <summary>
    /// This class provides the controls for a PauseAnalysis.
    /// </summary>
    public partial class PauseAnalysisControl : AnalysisControl
    {
        #region Fields
        /// <summary>
        /// The type of pause analysis that will be performed.
        /// </summary>
        private PauseAnalysis.IntervalType PauseAnalysisType;

        /// <summary>
        /// Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
            public const string PAUSETHRESHOLD = "PauseThreshold";
            public const string PBURSTTHRESHOLD = "PBurstThreshold";
            public const string ANALYSIS_TYPE = "AnalysisType";

            #region Nested type: FIXED_INTERVAL_SIZE

            public static class FixedIntervalSize
            {
                public const string INTERVAL_SIZE = "FixedIntervalSize_IntervalSize";
            }

            #endregion

            #region Nested type: FIXED_NUMBER_OF_INTERVALS

            public static class FixedNumberOfIntervals
            {
                public const string NUMBER_OF_INTERVALS = "FixedNumberOfIntervals_NumberOfIntervals";
            }

            #endregion
        }

        protected override string NAME { get { return PauseAnalyzer.NAME; } }

        #endregion

        /// <summary>
        /// Constructs a PauseAnalysis.
        /// </summary>
        public PauseAnalysisControl()
        {
            InitializeComponent();
            UpdateAnalysisType(this, null);
            Init(NameLabel, MoreInfoLabel, PauseAnalyzer.ABBR);
            InitTooltip();
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new PauseAnalyzer(
                RunGetTS(GetIntervalParam),
                PauseAnalysisType,
                RunGetTS(() => (ulong) PauseThresholdField.Value),
                RunGetTS(() => (ulong) PBurstThresholdField.Value)
            );
        }

        /// <summary>
        ///     Initialize the tooltip information.
        /// </summary>
        private void InitTooltip()
        {
            PBurstTooltip.ShowAlways = true;
            PBurstTooltip.SetToolTip(label5, "Consult the Inputlog documentation about the difference\n" +
                                                           "between a threshold for pauses and one for p-bursts.");
        }

        /// <summary>
        /// Gets the intervalParameter from the GUI based on the selected analysis type.
        /// </summary>
        private ulong GetIntervalParam()
        {
            ulong result = 0;

            switch (PauseAnalysisType)
            {
                case PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS:
                    result = ulong.Parse(IntervalSizeField.Text)*1000;
                    break;

                case PauseAnalysis.IntervalType.FIXED_NUMBER_OF_INTERVALS:
                    result = ulong.Parse(NumberOfIntervalsField.Text);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Updates the UI and internal bookkeeping when the user switches between the different types of linear intervals.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateAnalysisType(object sender, EventArgs e)
        {
            if (FixedIntervalSizeRadioButton.Checked)
            {
                PauseAnalysisType = PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS;
                FixedNumberOfIntervalsPanel.Enabled = false;
                FixedIntervalSizePanel.Enabled = true;
            }
            else if (FixedNumberOfIntervalsRadioButton.Checked)
            {
                PauseAnalysisType = PauseAnalysis.IntervalType.FIXED_NUMBER_OF_INTERVALS;
                FixedIntervalSizePanel.Enabled = false;
                FixedNumberOfIntervalsPanel.Enabled = true;
            }
        }

        /// <summary>
        /// Validates the IntervalSizeField (checks whether the entered value is a ulong).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void IntervalSizeFieldValidating(object sender, CancelEventArgs e)
        {
            e.Cancel = !InputValidation.AssertParse<ulong>(IntervalSizeField.Text, 5, 600);
            IntervalSizeErrorProvider.SetError(FixedIntervalSizePanel, e.Cancel ?
                "The size of the interval in seconds must be >= 5 and <= 600." : "");
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

        /// <summary>
        /// Exports the configuration of this Analysis Control to a |AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration 
        /// (= the input fields of this control) of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);

            config.Parameters.Add(ConfigurationParameters.PAUSETHRESHOLD, PauseThresholdField.Value.ToString(CultureInfo.InvariantCulture));
            config.Parameters.Add(ConfigurationParameters.PBURSTTHRESHOLD, PBurstThresholdField.Value.ToString(CultureInfo.InvariantCulture));
            config.Parameters.Add(ConfigurationParameters.ANALYSIS_TYPE, PauseAnalysisType.ToString());
            config.Parameters.Add(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE, IntervalSizeField.Text);
            config.Parameters.Add(ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS,
                NumberOfIntervalsField.Text);

            return config;
        }

        /// <summary>
        /// Imports a configuration into this LinearAnalysis Control from an AnalysisConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration to import.</param>
        public override void Import(AnalysisConfiguration configuration)
        {
            // check correct radiobutton
            var analysisType = (PauseAnalysis.IntervalType) Enum.Parse(typeof (PauseAnalysis.IntervalType),
                configuration.TryGetParameter(ConfigurationParameters.ANALYSIS_TYPE,
                PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS.ToString()),true);
            switch (analysisType)
            {
                case PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS:
                    {
                        FixedIntervalSizeRadioButton.Checked = true;
                        break;
                    }
                case PauseAnalysis.IntervalType.FIXED_NUMBER_OF_INTERVALS:
                    {
                        FixedNumberOfIntervalsRadioButton.Checked = true;
                        break;
                    }
            }
            UpdateAnalysisType(this, null);

            string pauseThresholdStr = configuration.TryGetParameter(ConfigurationParameters.PAUSETHRESHOLD,
                PauseThresholdField.Value.ToString(CultureInfo.InvariantCulture));
            string pburstThresholdStr = configuration.TryGetParameter(ConfigurationParameters.PBURSTTHRESHOLD,
                PBurstThresholdField.Value.ToString(CultureInfo.InvariantCulture));
            PauseThresholdField.Value = (decimal) Convert.ChangeType(pauseThresholdStr, 
                PauseThresholdField.Value.GetType());
            IntervalSizeField.Text = configuration.TryGetParameter(ConfigurationParameters.FixedIntervalSize.INTERVAL_SIZE,
                IntervalSizeField.Text);
            NumberOfIntervalsField.Text = 
                configuration.TryGetParameter(ConfigurationParameters.FixedNumberOfIntervals.NUMBER_OF_INTERVALS,
                NumberOfIntervalsField.Text);
        }
    }
}