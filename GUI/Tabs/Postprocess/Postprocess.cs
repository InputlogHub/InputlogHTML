using System;
using System.Windows.Forms;
using GUI.Tabs.Postprocess.WizardPages.Page;
using GUI.Wizard;

namespace GUI.Tabs.Postprocess
{
    public partial class Postprocess : UserControl
    {
        /// <summary>
        /// The GUI of which this tab is part of.
        /// </summary>
        public Gui GUI;

        public Postprocess()
        {
            InitializeComponent();

            ConvertProgressBar.Minimum = 0;
            ConvertProgressBar.Maximum = 100;
            ConvertProgressMessage.Text = "";
        }

        private void ConvertDEV_Load(object sender, EventArgs e)
        {
        }

        private void MergeAnalysis_CheckedChanged(object sender, EventArgs e)
        {
            MergeAnalysis.Checked = true;
            MergeOther.Checked = false;

            var wizard = new WizardSkeleton("Merge Analysis Files");
            wizard.StartPage = new MergeAnalysis_Intro();
            wizard.ShowDialog();

            MergeAnalysis.CheckedChanged -= MergeAnalysis_CheckedChanged;
            MergeAnalysis.Checked = false;
            MergeAnalysis.CheckedChanged += MergeAnalysis_CheckedChanged;
        }
    }
}