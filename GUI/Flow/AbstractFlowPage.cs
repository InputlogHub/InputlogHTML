using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util;
using System.Drawing;

namespace GUI.Flow
{
	#region public_delegates
	/// <summary>
	/// Data in a control has changed. The areaOfChange specifies what type of control has changed, the
	/// general type of control, and the sort of data we should try checking for changes.
	/// </summary>
	/// <typeparam name="Area">The type for the areaOfChange, this could be an Enum with zones defined for example.</typeparam>
	/// <param name="areaOfChange">The area of change is an identifiable area where the change occurred.</param>
	public delegate void ControlDataChange<Area>(Area areaOfChange);

	/// <summary>
	/// An InfoUpdated delegate functions gets called when the information
	/// message should be updated. 
	/// </summary>
	/// <param name="infoParams">The parameters for the info update event.</param>
	public delegate void UpdateInfo(InfoParameters infoParams);

	/// <summary>
	/// A class of parameters for an infoUpdated event. Parameters are:
	/// - Message: the actual message to display.
	/// - Bold: true/false
	/// - Color of the text
	/// - Show info message: true/false
	/// - Duration (ms): How long to show the message for, if show info = true, in milli-seconds.
	/// </summary>
	public class InfoParameters
	{
		/// <summary>
		/// Set the info message bold or not.
		/// </summary>
		public bool Bold { get; private set; }

		/// <summary>
		/// Set the color of the info text message
		/// This is standard black. If the user wants to change it the set 
		/// method has to be explicitely called with the new color.
		/// </summary>
		public Color InfoColor { get; set; }

		/// <summary>
		/// Show the info message, or hide it?
		/// </summary>
		public bool ShowInfo { get; private set; }

		/// <summary>
		/// If we have to show the info message, how long do we show it for.
		/// A value of -1 means that the info message is only removed when loading a new page
		/// or until an InfoEvent with ShowInfo = false event arrives.
		/// </summary>
		public long Duration { get; private set; }

		/// <summary>
		/// The info message to be displayed.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Create a new InfoParams class. The default color is black and can not be changed
		/// through the constructor but must be changed explicitely through the set function.
		/// </summary>
		/// <param name="message">The message to be displayed. (Default = "")</param>
		/// <param name="showInfo">Whether to show or hide the message on the GUI. (Default = false)</param>
		/// <param name="duration">How long should the message be displayed, in milli-seconds. Use -1 to display the 
		/// message until a new page is loaded or until the info message is explicitely removed. (Default = -1)</param>
		/// <param name="bold">Should the message be displayed in bold or not? (Default = false)</param>
		public InfoParameters(string message = "", bool showInfo = false, long duration = -1, bool bold = false)
		{
			Message = message;
			ShowInfo = showInfo;
			Duration = duration;
			Bold = bold;
			InfoColor = Color.Black;
		}
	}
	#endregion

	/// <summary>
	/// An abstract flow page. Flow pages inheriting from AbstractFlowPage will implement the 
	/// functionality associated with the UserControl they are displaying. 
	/// </summary>
	public abstract class AbstractFlowPage: IPreprocessTrackableAction, IProcessTrackableAction
	{
		#region public_fields
		/// <summary>
		/// The UserControl associated with this FlowPage, it contains the display elements
		/// for the logic behind the page.
		/// </summary>
		public UserControl Control { get; private set; }

		/// <summary>
		/// True if the page is currently preprocessing or has a preprocessing task ready
		/// to be handled as soon as possible. Otherwise, this bool returns false.
		/// </summary>
		private bool _isCurrentlyPreprocessing;
		public bool IsCurrentlyPreprocessing
		{
			get { return _isCurrentlyPreprocessing; }
			set { _isCurrentlyPreprocessing = value; }
		}

		/// <summary>
		/// True if the page is currently processing, or has a processing task ready to be
		/// handled as soon as possible. Otherwise this bool returns false.
		/// </summary>
		public bool IsCurrentlyProcessing { get; private set; }

		/// <summary>
		/// True if this is a page that has preprocessing tasks, regardless of whether
		/// they are currently running or not.
		/// </summary>
		public bool IsPreprocessingPage { get; private set; }

		/// <summary>
		/// True if this is a page that has processing tasks, regardless of whether
		/// they are currently running or not.
		/// </summary>
		public bool IsProcessingPage { get; private set; }

		/// <summary>
		/// Bool that is set true if the preprocessing step has been completed.
		/// </summary>
		private bool _preprocessingCompleted;
		public bool PreprocessingCompleted
		{
			get
			{
				return _preprocessingCompleted || !IsPreprocessingPage;
			}
			set
			{
				_preprocessingCompleted = value;
			}
		}

		/// <summary>
		///  Bool that is set true if the processing has been completed. If the
		///  preprocessing step has to be repeated, the processing step automatically
		///  has to be repeated as well.
		/// </summary>
		private bool _processingCompleted;
		public bool ProcessingCompleted
		{
			get
			{
				return _processingCompleted || !IsProcessingPage;
			}
			set
			{
				_processingCompleted = value;
			}
		}
		#endregion

		#region protected_fields
		#endregion

		#region private_fields
		/// <summary>
		/// The thread used for preprocessing, and the task doing the work.
		/// </summary>
		private Pair<Thread, ProcessTask> PreprocessWorker;

		/// <summary>
		/// The thread used for processing and the task doing the processing.
		/// </summary>
		private Pair<Thread, ProcessTask> ProcessWorker;

		/// <summary>
		/// Thread that starts threads when they're created, and
		/// stops the threads when they are called to stop from the outside.
		/// </summary>
		private Thread ThreadStarter;

		/// <summary>
		/// Bool that is set to true if all the processing of this page should be 
		/// stopped immediately.
		/// </summary>
		private volatile bool AllProcessingStopped;

		/// <summary>
		/// Bool that is set to true if the preprocessing should be done anew.
		/// Note that if this happens during the processing, the processing will be 
		/// aborted first, so that the preprocessing can then be started anew. After which
		/// the processing will start again as well.
		/// </summary>
		private volatile bool PreprocessingReset;

		/// <summary>
		/// Preprocessing Lock
		/// </summary>
		private object PreLock = new Object();

		/// <summary>
		/// Processing lock.
		/// </summary>
		private object ProLock = new Object();

		/// <summary>
		/// Duration for how long to show the initial page instruction.
		/// </summary>
		private const int PageInstructDuration = -1;
		#endregion

		#region public_events

		/// <summary>
		/// Event that notifies the listeners of the preprocessing progress of
		/// this page.
		/// </summary>
		public event ProgressListener ProcessListeners;

		/// <summary>
		/// Event that notifies the listeners of the processing progress
		/// of this page.
		/// </summary>
		public event ProgressListener PreprocessListeners;

		/// <summary>
		/// Event that notifies the listener that the flow page should
		/// be closed in its entirety. This is most likely caused by a severe
		/// error or such.
		/// </summary>
		public event EventHandler CloseFlow;

		/// <summary>
		/// Event gets called when the info message of the GUI should be updated.
		/// </summary>
		public event UpdateInfo InfoUpdated;
		#endregion

		#region protected_fields
		protected ProcessTask PreprocessTask
		{
			get
			{
				if (PreprocessWorker != null)
				{
					return PreprocessWorker.Second;
				}
				else
				{
					return null;
				}
			}
		}

		protected ProcessTask ProcessTask
		{
			get
			{
				if (ProcessWorker != null)
				{
					return ProcessWorker.Second;
				}
				else
				{
					return null;
				}
			}
		}

		protected volatile bool Closed = false;
		#endregion

		/// <summary>
		/// Create a flow page. 
		/// </summary>
		/// <param name="control">The UserControl for this page in the flow.</param>
		/// <param name="prepocessingPage">Is this a page with a preprocessing task?</param>
		/// <param name="processingPage">Is this a page with a processing task?</param>
		/// <param name="pageInstruction">An instruction for using the page that should be displayed
		/// to the user upon loading the page.</param>
		public AbstractFlowPage(UserControl control, bool prepocessingPage, bool processingPage, string pageInstruction = "")
		{
			IsPreprocessingPage = prepocessingPage;
			IsProcessingPage = processingPage;

			Control = control;
			control.Visible = false;

			AllProcessingStopped = false;
			PreprocessingReset = false;
			ThreadStarter = new Thread(StartThreadControl);

			Control.VisibleChanged += (sender, args) =>
			{
				if (Control.Visible && !string.IsNullOrEmpty(pageInstruction))
				{
					InfoParameters inf = new InfoParameters(pageInstruction, true, PageInstructDuration, false);
					OnUpdateInfo(inf);
				}
			};
		}


		protected void OnUpdateInfo(InfoParameters infoParams)
		{
			if (InfoUpdated != null)
			{
				InfoUpdated(infoParams);
			}
		}

		/// <summary>
		/// Method gets called when the form is closed and the page 
		/// should also terminate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnFormClose(object sender, EventArgs args)
		{
			Closed = true;
			StopAllProcessing();
		}

		//
		// Functions that manage the higher level processing threads.
		// These functions stop all processing, reset it. Make sure that the 
		// threads are executed in correct order etc...
		//
		#region Processing_thread_master_functions

		/// <summary>
		/// Immediately stop all the processing that he page was currently doing.
		/// This might lead to data loss.
		/// </summary>
		public void StopAllProcessing()
		{
			AllProcessingStopped = true;

			// Reset the 'tracking' booleans
			IsCurrentlyPreprocessing = false;
			IsCurrentlyProcessing = false;
			PreprocessingCompleted = false;
			ProcessingCompleted = false;

			if (!Closed)
			{
				ResetThePreprocessing();
			}
		}

		/// <summary>
		/// Reset the preprocessing. This might be required after relevant data on the page has 
		/// been changed.
		/// </summary>
		public void ResetThePreprocessing()
		{
			PreprocessingReset = true;
			PreprocessingCompleted = false;
			ProcessingCompleted = false;
			PreprocessWorker = null;
			ProcessWorker = null;
		}

		/// <summary>
		/// Enable the preprocessing and processing functions of the page. This is only 
		/// required if the page is a preprocessingpage, a processingpage, or both. <br />
		/// Calling this function starts the auto-processing functions of the page. It is impossible
		/// to do any processing without first enabling the page processes!
		/// </summary>
		public void EnablePageProcesses()
		{
			if (ThreadStarter == null)
			{
				ThreadStarter = new Thread(StartThreadControl);
			}

			PreprocessWorker = null;
			ProcessWorker = null;
			AllProcessingStopped = false;

			lock (ThreadStarter)
			{
				if (!ThreadStarter.IsAlive)
				{
					if (ThreadStarter.ThreadState != ThreadState.Unstarted)
					{
						ThreadStarter = new Thread(StartThreadControl);
					}
					ThreadStarter.Start();
				}
			}
		}

		/// <summary>
		/// Function that handles the threads on a high level.
		/// It stops threads when so asked through outside functions. It restarts preprocessing
		/// if required. And automatically starts the processing thread if it is ready to be 
		/// started after the preprocessing has been succesfully completed.
		/// </summary>
		private void StartThreadControl()
		{
			while (!AllProcessingStopped)
			{
				if (IsPreprocessingPage && PreprocessingReset)
				{
					lock (PreLock) {
					lock (ProLock) {
						// Are we processing anything right now??
						if (IsCurrentlyPreprocessing)
						{
							// Stop preprocessing, and restart it.
							StopPreprocessing();
							if (!AllProcessingStopped)
							{
								Preprocess();
							}
							else
							{
								return;
							}
						}
						else if (IsCurrentlyProcessing)
						{
							// Stop processing and restart the preprocessing and the processing.
							StopProcessing();
							if (!AllProcessingStopped)
							{
								Preprocess();
								Process();
							}
							else
							{
								return;
							}
						}
						else if (!AllProcessingStopped)
						{
							Preprocess();
						}
						else
						{
							return;
						}
					}
					}

					// reset the bool to its initial state.
					PreprocessingReset = false;
				}

				lock (PreLock)
				{
					// Start a (pre)processing thread, if possible.
					if (PreprocessWorker != null && IsPreprocessingPage && !AllProcessingStopped)
					{
						// If the thread is not alive but still needs to preprocess.
						if (!PreprocessWorker.First.IsAlive && !PreprocessingCompleted)
						{
							PreprocessWorker.First.Start();
						}
					}
				}

				lock (ProLock)
				{
					// Start the processing thread, if possible.
					if (ProcessWorker != null && IsProcessingPage && !AllProcessingStopped)
					{
						if (!ProcessWorker.First.IsAlive && PreprocessingCompleted && !ProcessingCompleted)
						{
							ProcessWorker.First.Start();
						}
					}
				}

				// Wait for 100 milliseconds before we try processing again.
				Thread.Sleep(500);
			}

			// Stop the threads.
			lock (PreLock)
			{
				StopPreprocessing();
			}
			lock (ProLock)
			{
				StopProcessing();
			}

			// reset the bool to its initial state.
			AllProcessingStopped = false;
		}

		#endregion

		//
		// Public methods for making the flow page (pre)process.
		//

		public void Preprocess()
		{
			// If this is not a preprocessing page, just leave.
			if (!IsPreprocessingPage)
			{
				return;
			}

			lock (PreLock)
			{
				try
				{
					// We set the preprocessing bool.
					IsCurrentlyPreprocessing = true;
					PreprocessingCompleted = false;

					// Create the thread.
					ProcessTask task = GetPreprocessTask();
					Thread preprocessThread = new Thread(task.Run);
					preprocessThread.Name = "PreprocessThread";
					PreprocessWorker = new Pair<Thread, ProcessTask>(preprocessThread, task);

					// Register to the events.
					task.ProcessListeners += PreprocessProgressHandler;
				}
				catch (Exception)
				{
					IsCurrentlyPreprocessing = false;
					PreprocessWorker = null;

					throw;
				}
			}

			// We do not start the task. This is done by the thread master function.
			// in ThreadStarter -> StartThreadControl
			// The 'thread master' is started by calling EnablePageProcesses()
		}

		public void Process()
		{
			// If this is not a preprocessing page, just leave.
			if (!IsProcessingPage)
			{
				return;
			}

			lock (ProLock)
			{
				try
				{
					// We set the preprocessing bool.
					IsCurrentlyProcessing = true;
					ProcessingCompleted = false;

					// Create the thread.
					ProcessTask task = GetProcessTask();
					Thread processThread = new Thread(task.Run);
					processThread.Name = "ProcessThread";
					ProcessWorker = new Pair<Thread, ProcessTask>(processThread, task);

					task.ProcessListeners += ProcessProgressHandler;
				}
				catch (Exception)
				{
					IsCurrentlyProcessing = false;
					ProcessWorker = null;
					throw;
				}
			}

			// We do not start the task. This is done by the thread master function.
			// in ThreadStarter -> StartThreadControl
			// This method is started by calling EnablePageProcesses()
		}

		//
		// Protected methods that get called by public preprocess() and process()
		// methods in order to do the actual processing work.
		//

		protected abstract ProcessTask GetPreprocessTask();
		protected abstract ProcessTask GetProcessTask();

		//
		// Handling of the report progress feedback we get from our own tasks.
		//
		#region progress_feedback_handling
		/// <summary>
		/// Handle the feedback we get of our own preprocess task.
		/// </summary>
		/// <param name="sender">Sender of the feedback.</param>
		/// <param name="args">Progress arguments.</param>
		private void PreprocessProgressHandler(object sender, ProgressEventArgs args)
		{
			try
			{
				lock (PreLock)
				{
					// In some cases we have to do some internal management before we pass
					// along the event.
					switch (args.Code)
					{
						case ProgressEventArgs.ProgressCode.DONE:
							IsCurrentlyPreprocessing = false;
							PreprocessingCompleted = true;

							break;
						case ProgressEventArgs.ProgressCode.FAILED:
							IsCurrentlyPreprocessing = false;
							PreprocessingCompleted = false;

							// Make sure thread is stopped, and stop listenening.
							StopPreprocessing();

							break;
						case ProgressEventArgs.ProgressCode.STARTED:
							IsCurrentlyPreprocessing = true;
							PreprocessingCompleted = false;
							ProcessingCompleted = false;

							break;
						default:
							// Code.NOT_SET || Code.STEP_COMPLETED
							break;
					};
				}
			}
			finally
			{
				// Refire the event for our own listeners.
				((IPreprocessTrackableAction)this).ReportProgress(sender, args);
			}
		}

		/// <summary>
		/// Stop the preprocessing thread and make sure we are unregistered
		/// from the tasks' progress events.
		/// </summary>
		private void StopPreprocessing()
		{
			lock (PreLock)
			{
				IsCurrentlyPreprocessing = false;
				PreprocessingCompleted = false;
				PreprocessingReset = true;

				// Nothing to do here in this case.
				if (PreprocessWorker == null)
				{
					return;
				}

				try
				{
					if (PreprocessWorker.First.IsAlive)
					{
						PreprocessWorker.First.Abort();
					}
				}
				finally
				{
					PreprocessWorker.Second.ProcessListeners -= PreprocessProgressHandler;
					PreprocessWorker = null;
				}
			}
		}

		/// <summary>
		/// Handle the feedback we get of our own process task.
		/// </summary>
		/// <param name="sender">Sender of the feedback.</param>
		/// <param name="args">Progress arguments.</param>
		private void ProcessProgressHandler(object sender, ProgressEventArgs args)
		{
			try
			{
				lock (ProLock)
				{
					switch (args.Code)
					{
						case ProgressEventArgs.ProgressCode.DONE:
							IsCurrentlyProcessing = false;
							ProcessingCompleted = true;

							break;
						case ProgressEventArgs.ProgressCode.FAILED:
							lock (this)
							{
								IsCurrentlyProcessing = false;
								ProcessingCompleted = false;

								// Make sure thread is stopped, and stop listenening.
								StopProcessing();
							}
							break;
						case ProgressEventArgs.ProgressCode.STARTED:
							IsCurrentlyProcessing = true;
							ProcessingCompleted = false;

							break;
						default:
							// Code.NOT_SET || Code.STEP_COMPLETED
							break;
					};
				}
			}
			finally
			{
				// Refire the event for our own listeners.
				((IProcessTrackableAction)this).ReportProgress(sender, args);
			}
		}

		/// <summary>
		/// Stop the processing thread and make sure we are unregistered
		/// from the tasks' progress events.
		/// </summary>
		private void StopProcessing()
		{
			lock (ProLock)
			{
				IsCurrentlyProcessing = false;
				ProcessingCompleted = false;

				// Nothing to do here in this case.
				if (ProcessWorker == null)
				{
					return;
				}

				try
				{
					if (ProcessWorker.First.IsAlive)
					{
						ProcessWorker.First.Abort();
					}
				}
				finally
				{
					ProcessWorker.Second.ProcessListeners -= ProcessProgressHandler;
					ProcessWorker = null;
				}
			}
		}
		#endregion

		//
		// IProcessTrackableAction
		//
		#region IProcessTrackableAction Members

		/// <summary>
		/// The (approximate) number of steps it will take for the action to finish.
		/// This can be used to estimate how many times the ProgressListener-callback will be called 
		/// (e.g. to initialize a progressbar).
		/// </summary>
		int IProcessTrackableAction.NumberOfSteps
		{
			get 
			{
				ProcessTask processTask = GetProcessTask();
				return (processTask != null) ? processTask.NumberOfSteps : 0;
			}
		}

		/// <summary>
		/// Reports progress to the ProcessListeners.
		/// </summary>
		/// <param name="sender">Sender of the ProcessEvent.</param>
		/// <param name="args">Arguments of the ProcessEvent.</param>
		void IProcessTrackableAction.ReportProgress(object sender, ProgressEventArgs args)
		{
			if (ProcessListeners != null)
			{
				ProcessListeners(sender, args);
			}
		}

		#endregion

		//
		// IPreprocessTrackableAction
		//
		#region IPreprocessTrackableAction Members

		/// <summary>
		/// The (approximate) number of steps it will take for the action to finish.
		/// This can be used to estimate how many times the ProgressListener-callback will be called 
		/// (e.g. to initialize a progressbar).
		/// </summary>
		int IPreprocessTrackableAction.NumberOfSteps
		{
			get 
			{
				ProcessTask preprocessTask = GetPreprocessTask();
				return (preprocessTask != null) ? preprocessTask.NumberOfSteps : 0;
			}
		}

		/// <summary>
		/// Reports progress to the PreprocessListeners.
		/// </summary>
		/// <param name="sender">Sender of the PreprocessEvent.</param>
		/// <param name="args">Arguments of the PreprocessEvent.</param>
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
