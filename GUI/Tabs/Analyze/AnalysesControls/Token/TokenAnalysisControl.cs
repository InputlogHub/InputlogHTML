using System;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Token
{
    public partial class TokenAnalysisControl : AnalysisControl
    {
        #region Fields
        private string CsvPath = "";

        protected override string NAME { get { return TokenAnalyzer.NAME; } }
        #endregion

        /// <summary>
        /// Constructs a TokenAnalysis.
        /// </summary>
        public TokenAnalysisControl()
        {
            InitializeComponent();
            Init(NameLabel, moreInfoLabel, TokenAnalyzer.ABBR);
        }

        public override bool CheckPreconditions()
        {
            if (CsvPath.Length == 0)
            {
                RunTS(delegate
                {
                    MessageBox.Show(this, "You forgot to select a CSV-file.",
                        "Token Analysis", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                });
                return false;
            }
            return true;
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new TokenAnalyzer(CsvPath);
        }

        /// <summary>
        /// Selecting a csv-file to set a 'target' word in the TokenTarget class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CSVSelectionBtnClick(object sender, EventArgs e)
        {
            try
            {
                var result = SelectCSVDialog.ShowDialog();
               
                if (result == DialogResult.OK)
                {
                    if (File.Exists(SelectCSVDialog.FileName))
                    {
                        CsvPath = SelectCSVDialog.FileName;
                        CSVField.Text = SelectCSVDialog.FileName;
                        CSVField.Select(CSVField.Text.Length, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }
    }
}
