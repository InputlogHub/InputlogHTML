using System;
using System.Windows.Forms;

namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    public partial class ProcessPage : UserControl
    {
        #region Fields

        #region Nested type: AppendLineCallback

        /// <summary>
        /// Delegate function for threadsafe appending of lines to the output
        /// textbox.
        /// </summary>
        /// <param name="line">Line to append to the textbox</param>
        private delegate void AppendLineCallback(string line);

        #endregion

        #region Nested type: EnabledCallback

        /// <summary>
        /// Delegate function for thread safe changing of the enabled status
        /// of a control
        /// </summary>
        /// <param name="control">control to alter</param>
        /// <param name="enabled">new enabled value</param>
        private delegate void EnabledCallback(Control control, bool enabled);

        #endregion

        #region Nested type: ProgressMaximumCallback

        /// <summary>
        /// Delegate function for threadsafe changing of the maximum value
        /// of the progress bar.
        /// </summary>
        /// <param name="max">Maximum value of the progress bar</param>
        private delegate void ProgressMaximumCallback(int max);

        #endregion

        #region Nested type: ProgressValueCallback

        /// <summary>
        /// Delegate function for threadsafe changing of the value 
        /// of the progress bar.
        /// </summary>
        /// <param name="value">Value of the progress bar</param>
        private delegate void ProgressValueCallback(int value);

        #endregion

        #region Nested type: VisibilityCallback

        /// <summary>
        /// Delegate function for threadsafe changing of the visibility
        /// of a control.
        /// </summary>
        /// <param name="control">Control to change visibility of</param>
        /// <param name="visibility">new visibility for the control.</param>
        private delegate void VisibilityCallback(Control control, bool visibility);

        #endregion

        #endregion

        //---------------------------------------------------------------------
        // Construction 
        // 

        protected ProcessPage()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------
        // Helper functions for thread safety
        //

        /// <summary>
        /// Thread safe method to append a line to the output textbox.
        /// </summary>
        /// <param name="line">Line to append to the textbox.</param>
        public void AppendLine(String line)
        {
            if (Output.InvokeRequired)
            {
                AppendLineCallback appendCallback = AppendLine;
                Invoke(appendCallback, line);
            }
            else
            {
                if (Output.Text != "")
                {
                    Output.AppendText(Environment.NewLine + line);
                }
                else
                {
                    Output.AppendText(line);
                }
            }
        }

        /// <summary>
        /// Thread safe method to set the maximum value of the progress bar
        /// </summary>
        /// <param name="max">Maximum value of the progress bar.</param>
        public void SetProgressMaximum(int max)
        {
            if (ProgressBar.InvokeRequired)
            {
                ProgressMaximumCallback maxCallback = SetProgressMaximum;
                Invoke(maxCallback, max);
            }
            else
            {
                ProgressBar.Maximum = max;
            }
        }

        /// <summary>
        /// Thread safe method for setting the value of the progress bar.
        /// </summary>
        /// <param name="value">Value of the progress bar.</param>
        public void SetProgressValue(int value)
        {
            if (ProgressBar.InvokeRequired)
            {
                ProgressValueCallback valueCallback = SetProgressValue;
                Invoke(valueCallback, value);
            }
            else
            {
                ProgressBar.Value = value;
            }
        }

        /// <summary>
        /// Thread safe method for setting visibility of a control
        /// </summary>
        /// <param name="control">control to change</param>
        /// <param name="visibility">new visibility value</param>
        public void SetVisibility(Control control, bool visibility)
        {
            if (control.InvokeRequired)
            {
                VisibilityCallback callback = SetVisibility;
                Invoke(callback, control, visibility);
            }
            else
            {
                control.Visible = visibility;
            }
        }

        public void SetEnabled(Control control, bool enabled)
        {
            if (control.InvokeRequired)
            {
                EnabledCallback callback = SetEnabled;
                Invoke(callback, control, enabled);
            }
            else
            {
                control.Enabled = enabled;
            }
        }
    }
}