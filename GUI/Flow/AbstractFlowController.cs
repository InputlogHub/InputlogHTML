using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using InputLog.Core.Util.Progress;

namespace GUI.Flow
{
	#region public_delegates
	/// <summary>
	/// A PageUpdated delegate function gets called when a page is updated.
	/// The receiver of the corresponding event should then update its views
	/// to represent the new page.
	/// </summary>
	/// <param name="control">The page to be displayed.</param>
	public delegate void UpdatePage(UserControl control);
	#endregion

	/// <summary>
	/// <para>
	/// An abstract flow controller is the class keeping track of all the pages that have to be 
	/// displayed and in which order it should be done. Preprocessing amongst pages, the processing,
	/// the handling of data is all done by a FlowController.
	/// </para>
	/// <para>
	/// The abstract flow controller is the abstract base class for FlowController classes. It provides
	/// some of the basic shared functionality among the flowcontrollers.
	/// </para>
	/// </summary>
	public abstract class AbstractFlowController: IProcessTrackableAction, IPreprocessTrackableAction
	{
		#region events_and_delegates
		/// <summary>
		/// Events gets called when the Page should be updated to represent
		/// the new page to be displayed.
		/// </summary>
		public event UpdatePage PageUpdated;

		/// <summary>
		/// Event gets called when the info message of the GUI should be updated.
		/// </summary>
		public event UpdateInfo InfoUpdated;

		/// <summary>
		/// Event that gets called when the processing of a page has updates to be passed.
		/// </summary>
		public event ProgressListener ProcessListeners;

		/// <summary>
		/// Event used for tracking the progress of preprocessing actions of a page.
		/// </summary>
		public event ProgressListener PreprocessListeners;

		/// <summary>
		/// Event that gets called if from the AbstractFlowController out form 
		/// should be closed in its entirety. This may be caused by a severe error
		/// in one of the flowPages.
		/// </summary>
		public event EventHandler FlowClose;
		#endregion

		#region private_fields

		/// <summary>
		/// Index of the page that should be displayed upon a successful 
		/// update of the display
		/// </summary>
		private int UpdateIndex;
		#endregion

		#region protected_fields
		/// <summary>
		/// List of all the Pages to be diplayed by the flow controller, in order.
		/// </summary>
		protected LinkedList<AbstractFlowPage> Pages;

		/// <summary>
		/// Index of the current page in the Pages Array.
		/// </summary>
		protected int CurrentPageIndex;

		/// <summary>
		/// Returns the currently active page.
		/// </summary>
		protected AbstractFlowPage CurrentPage
		{
			get 
			{ 
				return (CurrentPageIndex < Pages.Count) ? Pages.ElementAt(CurrentPageIndex) : null; 
			}
		}
		#endregion

		#region public_fields
		/// <summary>
		/// Returns the Control for the currently active page.
		/// </summary>
		public UserControl CurrentDisplay
		{ 
			get 
			{
				return (CurrentPageIndex < Pages.Count) ? Pages.ElementAt(CurrentPageIndex).Control : null;
			}
		}

		/// <summary>
		/// Flag that is true if the CurrentPage is busy Preprocessing, false if it is not doing so.
		/// </summary>
		public bool IsPreprocessing
		{
			get 
			{ 
				return CurrentPage.IsCurrentlyPreprocessing; 
			}
		}

		/// <summary>
		/// Flag that is true if the CurrentPage is busy Processing, flase if it is not doing so.
		/// </summary>
		public bool IsProcessing
		{
			get 
			{
				return CurrentPage.IsCurrentlyProcessing;
			}
		}

		/// <summary>
		/// Flag that is true if the current page has completed the preprocessing.
		/// </summary>
		public bool PreprocessingCompleted
		{
			get
			{
				return CurrentPage.PreprocessingCompleted;
			}
		}

		/// <summary>
		/// Flags that is trueif hte current page has completed the processing.
		/// </summary>
		public bool ProcessingCompleted
		{
			get
			{
				return CurrentPage.ProcessingCompleted;
			}
		}

		/// <summary>
		/// Return the number of pages in the flow controller.
		/// </summary>
		public int NumberOfPages
		{
			get
			{
				return Pages.Count;
			}
		}
		#endregion

		/// <summary>
		/// Construct the abstract flow controller. We pass along all the pages
		/// that should be displayed and the order in which they are to be displayed.
		/// </summary>
		/// <param name="pages">The pages to be displayed, in the order as given.</param>
		public AbstractFlowController(LinkedList<AbstractFlowPage> pages)
		{
			if (pages == null || pages.Count == 0)
			{
				throw new ArgumentException("A FlowController must have at least one page!", "pages");
			}

			Pages = pages;
			CurrentPageIndex = 0;

			// Set listeners on initial page.
			CurrentPage.ProcessListeners += ProcessProgressTracking;
			CurrentPage.PreprocessListeners += PreprocessProgressTracking;

			// Listen to the pages' events.
			Array.ForEach<AbstractFlowPage>(pages.ToArray(), page => page.CloseFlow += OnFlowClose);

			// Start the first pages Processing tasks.
			Pages.First.Value.EnablePageProcesses();

			// Subscribe to the first pages info updates
			Pages.First.Value.InfoUpdated += OnInfoUpdated;
		}

		/// <summary>
		/// Make sure that all processes stop their running threads.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnFormClose(object sender, EventArgs args)
		{
			foreach (AbstractFlowPage page in Pages)
			{
				page.OnFormClose(sender, args);
			}
		}

		/// <summary>
		/// Fires the FlowClose event which requests the listeners to close the abstract
		/// flow controller and all of its pages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnFlowClose(object sender, EventArgs args)
		{
			if (FlowClose != null)
			{
				FlowClose(sender, args);
			}
		}

		//
		// Flow logic: 
		// Navigation next, previous, close.
		//
		#region flow_logic

		/// <summary>
		/// Requests to load the next page when possible. Returns true when there is still a next page, 
		/// or false if the current page is the last page and the setup may be closed after it is done processing.
		/// </summary>
		/// <returns>True, if there's still pages to be handled after this page, or false if the current page is the
		/// last page to be displayed.</returns>
		public virtual bool Next()
		{
			// The page might still be processing, or preprocessing... Or might require to do both still.
			if (CurrentPage.IsPreprocessingPage && !(IsPreprocessing || CurrentPage.PreprocessingCompleted))
			{
				try
				{
					CurrentPage.Preprocess();
				}
				catch (Exception e)
				{
					((IPreprocessTrackableAction)this).ReportProgress(this, new ProgressEventArgs("Could not properly construct the process task " +
						"for this page: Could not properly construct the process task for this page: \"" + e.Message + "\"", ProgressEventArgs.ProgressCode.FAILED));
					throw;
				}
			}

			if (CurrentPage.IsProcessingPage && !(IsProcessing || CurrentPage.ProcessingCompleted))
			{
				try
				{
					CurrentPage.Process();
				}
				catch (Exception e)
				{
					((IProcessTrackableAction)this).ReportProgress(this, new ProgressEventArgs("Could not properly construct the process task " +
						"for this page: Could not properly construct the process task for this page: \"" + e.Message + "\"", ProgressEventArgs.ProgressCode.FAILED));
					throw;
				}
			}

			// Try updating the display. 
			UpdateDisplay(CurrentPageIndex + 1);

			// There's still a page to be displayed after current page.
			return UpdateIndex < Pages.Count - 1;
		}

		/// <summary>
		/// Requests to load the previous page when possible. Returns false if the current page is 
		/// the first page. In case the current page is not the first page, this method will request
		/// the update. Returning to the previous page stops all processes currently running.
		/// </summary>
		/// <returns></returns>
		public virtual bool Previous()
		{
			// if this is the first page return false, and we are done.
			if (CurrentPageIndex == 0)
			{
				return false;
			}

			// The page might still be busy processing, or preprocessing... We stop
			// all running processes and return to the previous page.
			CurrentPage.StopAllProcessing();
			UpdateDisplay(CurrentPageIndex - 1);

			return CurrentPageIndex > 0;
		}

		/// <summary>
		/// Handles the updating of the display. This takes into account whether
		/// or not the pages are currently processing or not, whether it is the last 
		/// page in the Flow or not and whether or not there has been a PageFlow defined
		/// to proceed.
		/// </summary>
		/// <param name="pageIndex">Index of the page that should be displayed
		/// upon update. Default value is -1. If a value &lt; 0 is passed as parameter
		/// this is regarded as an update to the same display as the previous call has attempted 
		/// to update too.</param>
		private void UpdateDisplay(int pageIndex = -1)
		{
			// The UpdateIndex should be changed
			if (pageIndex >= 0)
			{
				UpdateIndex = pageIndex;
			}

			// Page is ready to be updated
			if (!IsPreprocessing && !IsProcessing &&
				(CurrentPageIndex > UpdateIndex || (PreprocessingCompleted && ProcessingCompleted)))
			{

				// Fire the update event.
				if (UpdateIndex != Pages.Count)
				{
					LoadNewPage(UpdateIndex);
					OnPageUpdated(CurrentPage.Control);
				}
				else
				{
					// This was the last page to display. The flow has ended.
					// To signal the end of the flow of pages we fire an update
					// event with a null UserControl.
					OnPageUpdated(null);
				}
			}
		}

		/// <summary>
		/// Fires the PageUpdated event.
		/// </summary>
		/// <param name="control">Control to update the display too.</param>
		private void OnPageUpdated(UserControl control)
		{
			if (PageUpdated != null)
			{
				PageUpdated(control);
			}
		}

		/// <summary>
		/// Load a new active page in the abstract flow controller.
		/// </summary>
		/// <param name="pageIndex">Index of the page.</param>
		private void LoadNewPage(int pageIndex)
		{
			// Stop listening to the old page.
			CurrentPage.ProcessListeners -= ProcessProgressTracking;
			CurrentPage.PreprocessListeners -= PreprocessProgressTracking;
			CurrentPage.InfoUpdated -= OnInfoUpdated;
			CurrentPage.StopAllProcessing();

			CurrentPageIndex = pageIndex;
			// Enable the processing of the new page.
			CurrentPage.ResetThePreprocessing();
			CurrentPage.EnablePageProcesses();

			// Listen to new page.
			CurrentPage.ProcessListeners += ProcessProgressTracking;
			CurrentPage.PreprocessListeners += PreprocessProgressTracking;
			CurrentPage.InfoUpdated += OnInfoUpdated;
		}

		#endregion

		//
		// Our own update functions for receiving updates of our pages' updates.
		//
		#region page_progress_tracking 

		/// <summary>
		/// Handle the updating of the preprocessing progress of the active page.
		/// Events are first handled locally and then forwarded to our own subscribers.
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="args">Progress status of the event.</param>
		private void PreprocessProgressTracking(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.DONE:
					// Try updating the display, this will not work if the page is still
					// processing.
					UpdateDisplay();
					break;

				case ProgressEventArgs.ProgressCode.FAILED:
					// Stop all processing on the page and restart it.
					CurrentPage.StopAllProcessing();
					CurrentPage.EnablePageProcesses();
					break;

				default:
					break;
			}

			// Update our own listeners
			((IPreprocessTrackableAction)this).ReportProgress(sender, args);
		}

		/// <summary>
		/// Fires the info updated event
		/// </summary>
		/// <param name="prms">Event parameters</param>
		private void OnInfoUpdated(InfoParameters prms)
		{
			if (InfoUpdated != null)
			{
				InfoUpdated(prms);
			}
		}

		/// <summary>
		/// Handle the updating of the processing progress of the active page.
		/// Events are first handled locally and then forwarded to our own subscribers.
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="args">Progress status of the event.</param>
		private void ProcessProgressTracking(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.DONE:
					// Update our own listeners
					((IProcessTrackableAction)this).ReportProgress(sender, args);

					// Try updating the display.
					UpdateDisplay();

					break;
				case ProgressEventArgs.ProgressCode.FAILED:
					// Stop all processing on the page and restart it.
					CurrentPage.StopAllProcessing();
					CurrentPage.EnablePageProcesses();

					// Update our own listeners
					((IProcessTrackableAction)this).ReportProgress(sender, args);

					break;
				default:


					// Update our own listeners
					((IProcessTrackableAction)this).ReportProgress(sender, args);
					break;
			}
		}

		#endregion

		// 
		// Reporting our process progress to our listeners
		//
		#region IProcessTrackableAction
		/// <summary>
		/// Returns the number of steps of this page has to do in the preprocessing step,
		/// or returns 0 if this page has no preprocessing step.
		/// </summary>
		int IProcessTrackableAction.NumberOfSteps
		{
			get 
			{ 
				return CurrentPage.IsProcessingPage ? ((IProcessTrackableAction)CurrentPage).NumberOfSteps : 0;
			}
		}

		/// <summary>
		/// Report the progress of the currents page process actions.
		/// </summary>
		/// <param name="sender">Sender of the update.</param>
		/// <param name="args">Arguments of the event</param>
		void IProcessTrackableAction.ReportProgress(object sender, ProgressEventArgs args)
		{
			if (ProcessListeners != null)
			{
				ProcessListeners(sender, args);
			}
		}
		#endregion

		// 
		// Reporting our preprocess progress to our listeners
		//
		#region IPreprocessTrackableAction
		/// <summary>
		/// Returns the number of steps of this page has to do in the processing step,
		/// or returns 0 if this page has no processing step.
		/// </summary>
		int IPreprocessTrackableAction.NumberOfSteps
		{
			get 
			{
				return CurrentPage.IsPreprocessingPage ? ((IPreprocessTrackableAction)CurrentPage).NumberOfSteps : 0;
			}
		}

		/// <summary>
		/// Report the progress of the currents page preprocess actions.
		/// </summary>
		/// <param name="sender">Sender of the update.</param>
		/// <param name="args">Arguments of the event</param>
		void IPreprocessTrackableAction.ReportProgress(object sender, ProgressEventArgs args)
		{
			if (PreprocessListeners != null)
			{
				PreprocessListeners(sender, args);
			}
		}
		#endregion
	}
}
