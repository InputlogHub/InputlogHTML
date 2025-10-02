using System;
using System.IO;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;
using InputLog.Core.IO.Basic;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.WordPauses
{
    public partial class WordPausesAnalysisControl : AnalysisControl
    {
        #region Fields
        private string TargetPath = "";
        private string ParticipantPath = "";
        #endregion

        protected override string NAME { get { return WordPausesAnalyzer.NAME; } }

        /// <summary>
        /// Constructs a Word Pauzes Analysis.
        /// </summary>
        public WordPausesAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, moreInfoLabel, WordPausesAnalyzer.ABBR);
            InitTooltip();
        }


        /// <summary>
        ///     Initialize the tooltip information.
        /// </summary>
        private void InitTooltip()
        {
            TargetTip.ShowAlways = true;
            TargetTip.SetToolTip(TargetField, "The selection of these target files is optional.");
        }

        public override bool CheckPreconditions()
        {
            // Target checking is skipped when no paths were given, but that's ok.
            if (TargetPath.Length == 0 && ParticipantPath.Length == 0)
            {
                return true;
            }

            // If  one path is given, the other should also be present.
            if ((TargetPath.Length == 0 && ParticipantPath.Length > 0) || (TargetPath.Length > 1  && ParticipantPath.Length == 0))
            {
                RunTS(delegate
                {
                    MessageBox.Show(this, "You forgot to select a CSV-file.",
                        "Word Pause Analysis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
                return false;
            }
            return true;
        }


        protected override void ProduceAnalyzer()
        {
            Analyzer = new WordPausesAnalyzer(TargetPath, ParticipantPath, (int) DistanceUpDown.Value);
        }

        /// <summary>
        /// Selecting a target-file to set a 'target' wordlist in the WordPauseAnalysis class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetSelectionBtnClick(object sender, EventArgs e)
        {
            try
            {
                var result = SelectTargetDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (File.Exists(SelectTargetDialog.FileName))
                    {
                        TargetPath = SelectTargetDialog.FileName;
                        TargetField.Text = SelectTargetDialog.FileName;
                        TargetField.Select(TargetField.Text.Length, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Selecting a participant-file to set a 'participant-target' list in the WordPauseAnalysis class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParticipantSelectionBtnClick(object sender, EventArgs e)
        {
            try
            {
                var result = SelectParticipantDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (File.Exists(SelectParticipantDialog.FileName))
                    {
                        ParticipantPath = SelectParticipantDialog.FileName;
                        ParticipantField.Text = SelectParticipantDialog.FileName;
                        ParticipantField.Select(ParticipantField.Text.Length, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
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

            return config;
        }

        /// <summary>
        /// Imports a configuration into this Analysis Control from an AnalysisConfiguration.
        /// </summary>
        public override void Import(AnalysisConfiguration configuration) { }
    }
}
