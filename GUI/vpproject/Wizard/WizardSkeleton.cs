using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using InputLog.Core.Util;

namespace GUI.Wizard
{
    /// <summary>
    /// Skeleton for a Wizard class. This class creates the a navigatable window with 
    /// next and previous buttons, and a finish button. Upon pressing the finish button
    /// the last page in the form will be rendered, while the processor is executing the 
    /// wizards functionality. <br />
    /// So, the 'last page' in the wizard, where the user can provide information is 
    /// page before the last page. The very last page in the form is the page that is rendered
    /// during the processing by the wizard.
    /// </summary>
    public sealed partial class WizardSkeleton : Form
    {
        //---------------------------------------------------------------------
        // Delegates for thread safety
        //

        //---------------------------------------------------------------------
        // Data members
        //

        //---------------------------------------------------------------------
        // Constructing the wizard
        //

        /// <summary>
        /// Construct the wizard skeleton.
        /// </summary>
        public WizardSkeleton(string wizardTitle)
        {
            // Initialize GUI.
            InitializeComponent();

            // Set the window title.
            Text = wizardTitle;
        }

        //---------------------------------------------------------------------
        // Rendering
        //

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Current = StartPage;
            NavigateToPage();
        }

        private void NavigateToPage()
        {
            // First we set both buttons back to the start state.
            SetVisibility(PreviousButton, true);
            SetEnabled(PreviousButton, true);
            SetVisibility(NextButton, true);
            SetEnabled(NextButton, true);

            // Now we change the state of the buttons according to the pages we are on.
            if (Current.Previous == null)
            {
                SetEnabled(PreviousButton, false);
            }
            else
            {
                SetEnabled(PreviousButton, true);
            }

            // Change next button text
            if (Current.Next == null)
            {
                SetText(NextButton, ENDTEXT);
            }
            else
            {
                SetText(NextButton, NEXTTEXT);
            }

            // Special case for when its a processing page.
            if (Current.IsProcessingPage())
            {
                // The page has processed its information and finished successfully.
                if (Current.ProcessedCorrectly())
                {
                    SetVisibility(PreviousButton, false);
                }
            }

            LoadActivePage();
        }

        private void LoadActivePage()
        {
            // Load the correct page.
            if (!PageControls.Controls.Contains(Current.Control))
            {
                PageControls.Controls.Add(Current.Control);
            }

            foreach (UserControl control in PageControls.Controls)
            {
                if (control != Current.Control)
                {
                    SetVisibility(control, false);
                }
                else
                {
                    SetVisibility(control, true);
                }
            }

            // If the active page is the processing page. We register to the processing events.
            ProcessPage();
        }

        //---------------------------------------------------------------------
        // Special processing of the page, if required.
        //

        /// <summary>
        /// If the page is a processing page we do whatever processing is required
        /// by the page. The user can not continue the wizard until the processing has 
        /// been completed.
        /// </summary>
        private void ProcessPage()
        {
            if (Current.IsProcessingPage() && !Current.ProcessedCorrectly())
            {
                // We register as a listener to the process events of the current page.
                Current.ProcessStatusEvent += ProcessStatusListener;
                RunningThread = new Thread(Current.Process);
                SetEnabled(NextButton, false);
                SetEnabled(PreviousButton, false);
                RunningThread.Start();
            }
        }

        /// <summary>
        /// Listens to processing events of the current page if it is currently processing.
        /// Upon a process event it redraws the window accordingly and might display a message
        /// box concerning the process activity.
        /// </summary>
        /// <param name="sender">Sender of the event. (Current Page)</param>
        /// <param name="e">Proces status change event.</param>
        public void ProcessStatusListener(Object sender, ProcessStatusChanged e)
        {
            Debug.Print("Event: {0}", e.Status.ToString());
            switch (e.Status)
            {
                case ProcessCode.PROCESS_SUCCESS:
                    //Reconstruct the buttons on the page, and redraw the page.
                    NavigateToPage();
                    Refresh();

                    // We unregister from ProcessStatus events.
                    Current.ProcessStatusEvent -= ProcessStatusListener;
                    break;

                case ProcessCode.PROCESS_ERROR:
                case ProcessCode.PROCESS_FAIL:
                    NavigateToPage();
                    Refresh();
                    MessageLogger.LogMessage(this, Text + " - Failed", e.Message, Severity.WARNING);

                    // We unregister from ProcessStatus events.
                    Current.ProcessStatusEvent -= ProcessStatusListener;
                    break;
            }
        }


        //---------------------------------------------------------------------
        // Navigating the wizard

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (Current.Validate())
            {
                SetText(Error, "");
                if (Current.Next != null)
                {
                    Current = Current.Next;
                }
                else if (NextButton.Text == ENDTEXT)
                {
                    Close();
                }
                NavigateToPage();
            }
            else
            {
                SetText(Error, Current.LastError);
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (Current.Previous != null)
            {
                Current = Current.Previous;
                SetText(Error, "");
            }
            NavigateToPage();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // If the thread is still alive we are still processing. Ask
            // whether the user really wants to stop now.
            if (RunningThread != null && RunningThread.IsAlive)
            {
                if (MessageBox.Show(
                    "This window is still completing tasks, closing this window will terminate \n" +
                    "those tasks and possibly involve a loss of data.\n Are you sure you want to close this window?",
                    "Really Cancel?",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Current.StopProcessing();
                    Thread.Sleep(250);
                    if (RunningThread.IsAlive)
                    {
                        try
                        {
                            RunningThread.Abort();
                        }
                        catch (ThreadAbortException)
                        {
                            // We don't process this exception, as we knew this could happen...
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            // We unregister from ProcessStatus events.
            Current.ProcessStatusEvent -= ProcessStatusListener;

            base.OnClosing(e);
        }

        #region HelperFunctions

        /// <summary>
        /// Thread safe function for changing the visibility of the buttons
        /// in our wizard form, as the buttons can be altered from an 'eventThread'
        /// as well.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="visibility">Value for the visibility attribute of the button</param>
        private void SetVisibility(Control control, bool visibility)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (control.InvokeRequired)
            {
                SetVisibilityCallback threadSafeCallback = SetVisibility;
                Invoke(threadSafeCallback, new object[] {control, visibility});
            }
            else
            {
                control.Visible = visibility;
            }
        }

        /// <summary>
        /// Thread safe function for changing the enabled attribute of the buttons
        /// in our wizard form, as the buttons can be altered from an 'eventThread'
        /// as well.
        /// </summary>
        private void SetEnabled(Control control, bool enabled)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (control.InvokeRequired)
            {
                SetEnabledCallback threadSafeCallback = SetEnabled;
                Invoke(threadSafeCallback, new object[] {control, enabled});
            }
            else
            {
                control.Enabled = enabled;
            }
        }

        /// <summary>
        /// Thread safe function for changing the text of the buttons
        /// in our wizard form, as the buttons can be altered from an 'eventThread'
        /// as well.
        /// </summary>
        private void SetText(Control control, string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (control.InvokeRequired)
            {
                SetTextCallback threadSafeCallback = SetText;
                Invoke(threadSafeCallback, new object[] {control, text});
            }
            else
            {
                control.Text = text;
            }
        }

        /// <summary>
        /// Override of Refresh function to make it threadsafe.
        /// </summary>
        public override void Refresh()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (InvokeRequired)
            {
                RefreshCallback threadSafeCallback = Refresh;
                Invoke(threadSafeCallback, new object[] {});
            }
            else
            {
                base.Refresh();
            }
        }

        #endregion

        #region Nested type: RefreshCallback

        /// <summary>
        /// Delegate method for threadsafe refreshing of the GUI.
        /// </summary>
        private delegate void RefreshCallback();

        #endregion

        #region Fields 

        /// <summary>
        /// Text to be rendered on the next button when the processing step 
        /// has been completed, and clicking this button will close the wizard.
        /// </summary>
        private const string ENDTEXT = "Finish";

        /// <summary>
        /// Text to be rendered on the next button if we can just go to the
        /// next page, without anything special happening there.
        /// </summary>
        private const string NEXTTEXT = "Next";

        /// <summary>
        /// Keeps track of the current page of the wizard.
        /// </summary>
        private WizardPage Current;

        /// <summary>
        /// Thread used to run the processing function of a WizardPage in.
        /// Used so that the UI does not lock up while the processing is in action.
        /// </summary>
        private Thread RunningThread;

        /// <summary>
        /// The list of pages the wizard consists off.
        /// </summary>
        public WizardPage StartPage { get; set; }

        #endregion

        #region Nested type: SetEnabledCallback

        /// <summary>
        /// Delegate method for threadsafe changing of the enabled status of the given control.
        /// </summary>
        private delegate void SetEnabledCallback(Control control, bool enabled);

        #endregion

        #region Nested type: SetTextCallback

        /// <summary>
        /// Delegate method for threadsafe changing of the Text of the given control.
        /// </summary>
        private delegate void SetTextCallback(Control control, string text);

        #endregion

        #region Nested type: SetVisibilityCallback

        /// <summary>
        /// Delegate method for threadsafe changing of the visibility of the given control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="visibility">Visibility value you wish to assign to the control</param>
        private delegate void SetVisibilityCallback(Control control, bool visibility);

        #endregion

        //---------------------------------------------------------------------
        // Change button text/visibility/enabled in thread-safe way
        //
    }
}