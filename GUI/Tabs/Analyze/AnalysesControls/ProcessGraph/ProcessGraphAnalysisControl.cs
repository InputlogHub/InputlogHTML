using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.ProcessGraph
{
    /// <summary>
    ///     This class provides the controls for a ProcessGraphAnalysis.
    /// </summary>
    public partial class ProcessGraphAnalysisControl : AnalysisControl
    {
        /// <summary>
        ///     Constructs a GeneralAnalysis.
        /// </summary>
        public ProcessGraphAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, MoreInfoLabel, ProcessGraphAnalyzer.ABBR);
            FileTypeList.SelectedItem = "PNG";
            InitTooltips();
        }

        /// <summary>
        ///     Initialize the tooltip information.
        /// </summary>
        private void InitTooltips()
        {
            OverrideY1Tip.ShowAlways = true;
            OverrideY2Tip.ShowAlways = true;

            OverrideY1Tip.SetToolTip(OverrideY1TxtBx,
                "Enter a maximum value in milliseconds \nfor the primary Y-axis (left)." +
                "\nThe settings are applied to all the source files of this session.");
            OverrideY2Tip.SetToolTip(OverrideY2TxtBx,
                "Enter a maximum value in number of characters \nfor the secondary Y-axis (right)." +
                "\nThe settings are applied to all the source files of this session.");
        }

        protected override void ProduceAnalyzer()
        {
            var graphSettings = new List<string>();
            if (RunGetTS(() => IncludeProduct.Checked)) graphSettings.Add("Product");
            if (RunGetTS(() => IncludeProcess.Checked)) graphSettings.Add("Process");
            if (RunGetTS(() => IncludePosition.Checked)) graphSettings.Add("Position");
            if (RunGetTS(() => IncludePauses.Checked)) graphSettings.Add("Pauses");
            if (RunGetTS(() => IncludeFocus.Checked)) graphSettings.Add("Focus");
            if (RunGetTS(() => IncludeOutlierBx.Checked)) graphSettings.Add("Outliers");
            if (_overrideIsValid)
            {
                graphSettings.Add("Y1:" + RunGetTS(() => OverrideY1TxtBx.Text.Trim()));
                graphSettings.Add("Y2:" + RunGetTS(() => OverrideY2TxtBx.Text.Trim()));
            }
            else
            {
                graphSettings.Add("Y1:0");
                graphSettings.Add("Y2:0");
            }
            // List of keys that act as control keys (these should be represented with '+' 
            // when they are present in the keyboard state).
            // e.g if LSHIFT is part of ControlKeys, then pressing LSHIFT and 'a' at the same time 
            // will result in LSHIFT + A in the representation. If LSHIFT is not part of ControlKeys, then pressing LSHIFT
            //  and  'a' at the same time will result in 2 different keystrokes.
            var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);
            Analyzer = new ProcessGraphAnalyzer(
                RunGetTS(() => FileTypeList.SelectedItem.ToString()),
                RunGetTS(() => (ulong) PauseThreshold.Value),
                graphSettings,
                controlKeys
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
        ///     Imports a configuration into this Control from an AnalysisConfiguration.
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
            Control fileLink;
            if (DynamicControls.TryGetValue(FILE_LINK_ID, out fileLink))
            {
                RemoveClickEvent(fileLink);
                fileLink.Text = "Show graph";
                fileLink.Click += delegate { ((ProcessGraphAnalyzer) Analyzer).Graph.Show(); };
            }
        }

        private static void RemoveClickEvent(IDisposable b)
        {
            var f1 = typeof (Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            if (f1 == null) return;
            var obj = f1.GetValue(b);
            var pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (EventHandlerList) pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }

        private void ValidateInputY1Y2(object sender, EventArgs e)
        {
            var checkInput = AxisGrpBx.Controls.OfType<MaskedTextBox>();
            int num;
            _overrideIsValid = checkInput.Any(axis => int.TryParse(OverrideY1TxtBx.Text.Trim(), out num)
                                                     && int.TryParse(OverrideY2TxtBx.Text.Trim(), out num));

            if (!_overrideIsValid)
            {
                MessageBox.Show("When you consider to override the calculated maxima of this graph" +
                                " you should enter a value for both the primary and the secondary axis ",
                    "Overriding Calculated Values of the Process Graph", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        #region Fields

        /// <summary>
        ///     Configuration parameter names of this analysis.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string DESTINATION = "Destination";
        }

        protected override string NAME => ProcessGraphAnalyzer.NAME;

        private bool _overrideIsValid;

        #endregion
    }
}