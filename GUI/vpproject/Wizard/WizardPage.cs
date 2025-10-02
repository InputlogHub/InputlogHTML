using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.Wizard
{
    /// <summary>
    /// Wrapper around a System.Windows.Forms.UserControl, so that it provides
    /// an extra method for getting all the information out of the form, after
    /// it has been filled in.
    /// </summary>
    public abstract class WizardPage
    {
        #region fields

        #region Delegates

        public delegate void ProcessStatusChange(Object send, ProcessStatusChanged e);

        #endregion

        /// <summary>
        /// Dictionary to store the data gathered in the wizard pages.
        /// </summary>
        protected Dictionary<string, Object> Data;

        /// <summary>
        /// Boolean to keep track of whether we should stop the processing or not.
        /// </summary>
        protected volatile bool Stop;

        /// <summary>
        /// Storage for the last error triggered on this page.
        /// </summary>
        private string Error;

        /// <summary>
        /// The controls that should be displayed on 
        /// this page.
        /// </summary>
        public UserControl Control { protected set; get; }

        /// <summary>
        /// Get the next page in the wizard. This must be the actual instance of the
        /// previous page, and not just another instance of the same page.
        /// </summary>
        public WizardPage Next { get; set; }

        /// <summary>
        /// Get the previous page in the wizard. This must be the actual instance of the
        /// previous page, and not just another instance of the same page.
        /// </summary>
        public WizardPage Previous { get; set; }

        /// <summary>
        /// Boolean keeping track of whether or not we have already processed the page or not.
        /// </summary>
        public ProcessCode Status { get; protected set; }

        /// <summary>
        /// Returns the last error set on the page, and resets the 
        /// error after it has been retrieved.
        /// </summary>
        public string LastError
        {
            protected set { Error = value; }
            get
            {
                string tmp = Error;
                Error = null;
                return tmp;
            }
        }

        public event ProcessStatusChange ProcessStatusEvent;

        #endregion fields

        //---------------------------------------------------------------------
        // Page Construction
        //

        protected WizardPage()
        {
            Next = null;
            Previous = null;
            Status = ProcessCode.NOT_PROCESSED;
            Stop = false;
            Data = new Dictionary<string, Object>();
        }

        //---------------------------------------------------------------------
        // Processing functionality
        // 

        /// <summary>
        /// Get the information entered in the wizard page. This gets all the data
        /// from the previous page as well (recursively, through all the previous pages).
        /// Note that if you want to get all data from the current page and the previous page,
        /// the data from the current page must be entered before calling this getData() method.
        /// </summary>
        protected virtual Dictionary<string, Object> GetData()
        {
            if (Previous != null)
            {
                Data = Data.Union(Previous.GetData()).ToDictionary(key => key.Key, value => value.Value);
            }
            return Data;
        }

        /// <summary>
        /// Returns whether the page has already correctly processed her
        /// information the information.
        /// </summary>
        /// <returns>True if this page has correctly processed her information, false if not.
        /// Note that if this page is not a process page, it will allways return false</returns>
        public bool ProcessedCorrectly()
        {
            return Status == ProcessCode.PROCESS_SUCCESS && IsProcessingPage();
        }

        /// <summary>
        /// Returns whether this Wizard page is a processing page or not. 
        /// A processing page is a page that executes whatever the wizard
        /// is supposed to do.
        /// </summary>
        /// <returns>Returns true if the page is a processing page, false if not.</returns>
        public virtual bool IsProcessingPage()
        {
            return false;
        }

        /// <summary>
        /// Checks whether or not the page has been correctly filled in. If it has been
        /// correctly filled in it returns true, otherwise it returns false.
        /// </summary>
        /// <returns>Returns true if the page has been correclty filled in, false if not.</returns>
        public virtual bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Process this page. Generally a page will not have to process data.
        /// </summary>
        public virtual void Process()
        {
            Status = ProcessCode.NOT_PROCESSED;
        }

        /// <summary>
        /// Tell the wizard page to stop whatever processing the page might be doing.
        /// </summary>
        public void StopProcessing()
        {
            Stop = true;
        }

        //---------------------------------------------------------------------
        // Event functions
        //

        /// <summary>
        /// Fires a ProcessStatusEvent event upon the starting of the processing.
        /// </summary>
        /// <param name="message">Optional message to include in the event.</param>
        protected virtual void OnProcessStart(string message = null)
        {
            if (ProcessStatusEvent != null)
            {
                Status = ProcessCode.PROCESS_START;
                var e = new ProcessStatusChanged(ProcessCode.PROCESS_START, message);
                ProcessStatusEvent(this, e);
            }
        }

        /// <summary>
        /// Fires a ProcessStatusEvent event upon successfully finishing the processing
        /// </summary>
        /// <param name="message">Optional message to include in the event.</param>
        protected virtual void OnProcessSuccess(string message = null)
        {
            if (ProcessStatusEvent != null)
            {
                Status = ProcessCode.PROCESS_SUCCESS;
                var e = new ProcessStatusChanged(ProcessCode.PROCESS_SUCCESS, message);
                ProcessStatusEvent(this, e);
            }
        }

        /// <summary>
        /// Fires a ProcessStatusEvent event upon finishing the processing unsuccessfully.
        /// </summary>
        /// <param name="message">Optional message to include in the event.</param>
        protected virtual void OnProcessFail(string message = null)
        {
            if (ProcessStatusEvent != null)
            {
                Status = ProcessCode.PROCESS_FAIL;
                var e = new ProcessStatusChanged(ProcessCode.PROCESS_FAIL, message);
                ProcessStatusEvent(this, e);
            }
        }

        /// <summary>
        /// Fires a ProcessStatusEvent event upon failure of completing the processing due to
        /// an error.
        /// </summary>
        /// <param name="message">Optional message to include in the event.</param>
        protected virtual void OnProcessError(string message = null)
        {
            if (ProcessStatusEvent != null)
            {
                Status = ProcessCode.PROCESS_ERROR;
                var e = new ProcessStatusChanged(ProcessCode.PROCESS_ERROR, message);
                ProcessStatusEvent(this, e);
            }
        }
    }
}

/// <summary>
/// The different possible process statusses. <br />
/// PROCESS_START: The processing has been initiated. <br />
/// PROCESS_SUCCESS: The processing has finished and was successful. <br />
/// PROCESS_FAIL: The processing has finished but failed. <br />
/// PROCESS_ERROR: The processing was stopped because of an error. <br />
/// </summary>
public enum ProcessCode
{
    NOT_PROCESSED,
    PROCESS_START,
    PROCESS_SUCCESS,
    PROCESS_FAIL,
    PROCESS_ERROR,
} ;


/// <summary>
/// An event class for process status change events.
/// </summary>
public class ProcessStatusChanged : EventArgs
{
    /// <summary>
    /// Construct the class.
    /// </summary>
    /// <param name="status">Status of the process</param>
    /// <param name="message">Optional message containing more information</param>
    public ProcessStatusChanged(ProcessCode status, string message = null)
    {
        Status = status;
        Message = message;
    }

    /// <summary>
    /// Status of the process
    /// </summary>
    public ProcessCode Status { get; private set; }

    /// <summary>
    /// Message containing some more information. This is optional.
    /// </summary>
    public string Message { get; private set; }
}