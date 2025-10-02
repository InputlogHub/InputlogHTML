using System.Windows.Forms;
using System;

namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public partial class MergeAnalysis_Intro_Form : UserControl
    {
        /// <summary>
        /// 'true' is vertical merge; 'false' is horizontal merge
        /// </summary>
        public static bool MergeDirection { private set; get; }

        public event ChangedEventHandler Changed;

        public MergeAnalysis_Intro_Form()
        {
            InitializeComponent();
            MergeDirection = true;
            RadioMergeVertical.Checked = true;
        }

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Returns whether or not the Vertical merging option is checked or not.
        /// </summary>
        /// <returns>True if vertical merging is selected, false if not.</returns>
        public bool MergeVerticalIsChecked()
        {
            return RadioMergeVertical.Checked;
        }

        /// <summary>
        /// Returns whether or not the Horizontal merging option is checked or not
        /// </summary>
        /// <returns>True if horizontal merging is selected, false if not.</returns>
        public bool MergeHorizontalIsChecked()
        {
            return RadioMergeHorizontal.Checked;
        }

        /// <summary>
        /// Eventhandler on the radiobutton 'vertical merge'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioMergeVertical_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!MergeDirection) OnChanged(EventArgs.Empty);
            MergeDirection = true;
        }

        /// <summary>
        ///  Eventhandler on the radiobutton 'horizontal merge'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioMergeHorizontal_CheckedChanged(object sender, System.EventArgs e)
        {
            if (MergeDirection) OnChanged(EventArgs.Empty);
            MergeDirection = false;
        }
    }
}