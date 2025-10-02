using System;
using System.Drawing;
using System.Windows.Forms;
using InputLog.Core.Util.Progress;
using GUI.Util;
using InputLog.Core.Util;

namespace GUI.Flow
{
	/// <summary>
	/// <para>
	/// A Wizard-like GUI that 'flows' between pages. This GUI part is only the encapsulating
	/// GUI which provides navigation options, can display different pages, information, preprocessing
	/// progress and progress while processing data.
	/// </para>
	/// <para>
	/// Every FlowGUI must have an AbstractFlowController handed to it upon construction. This
	/// FlowController is the class that gathers data from the different pages, and gives pages
	/// to the FlowGUI to render. 
	/// </para>
	/// </summary>
	public partial class FlowGUI : Form
	{
		#region private_fields
		/// <summary>
		/// The flow controller associated with this instance of the flowGUI.
		/// The flow controller hands the flowGUI the pages that have to be shown
		/// and handles the data and the processing.
		/// </summary>
		private readonly AbstractFlowController _flowController;

		/// <summary>
		/// True if there is another page after the current page. False if not.
		/// </summary>
		private bool _hasNext;

		/// <summary>
		/// True if there is a previous page to go back to, false if not.
		/// </summary>
		private bool _hasPrevious;

		/// <summary>
		/// Counts how many steps have already been completed in the process so far.
		/// </summary>
		private int _stepCounter;

		/// <summary>
		/// The Usercontrol that is currently being displayed in the FlowGUI
		/// </summary>
		private UserControl _activePageControl;

		/// <summary>
		/// Should we show the progressbar at this point?
		/// </summary>
		private volatile bool _showProgressBar;

		/// <summary>
		/// The old text of the next button.
		/// </summary>
		private string _oldNextButtonText;

		/// <summary>
		/// Bool is set to true from the moment we know the GUI will be closed.
		/// </summary>
		private volatile bool _closed = false;
		#endregion

		/// <summary>
		/// Create the FlowGUI with associated FlowController.
		/// </summary>
		/// <param name="fController">FlowController associated with this GUI.</param>
		public FlowGUI(AbstractFlowController fController)
		{
			InitializeComponent();
			
			// The flowcontroller
			_flowController = fController;
			_flowController.InfoUpdated += HandleInfoUpdate;
			_flowController.PageUpdated += HandlePageUpdate;
			_flowController.ProcessListeners += HandleProcessProgress;
			_flowController.PreprocessListeners += HandlePreprocessProgress;
			_flowController.FlowClose += OnFlowClose;

			// Booleans
			_hasNext = (fController.NumberOfPages > 1);
			_hasPrevious = false;

			// Set buttons initial values
			PreviousButton.Enabled = false;
			NextButton.Enabled = true;
			NextButton.Text = (_hasNext) ? "Next" : "Finish";
			_oldNextButtonText = NextButton.Text;

			// Hide progress bar.
			ProgressBar.Visible = false;
			InfoLbl.Visible = false;
			ProcessLbl.Visible = false;
			ProcessInfoLbl.Visible = false;

			// The initial control to display.
			if (fController.CurrentDisplay != null)
			{
				this.Layout.Controls.Add(fController.CurrentDisplay, 0, 1);
				_activePageControl = fController.CurrentDisplay;
				_activePageControl.Visible = true;
				_activePageControl.Dock = DockStyle.Fill;
			}
		
			// Abstract flow controller listens to the form closing event.
			FormClosing += fController.OnFormClose;
		}

		// 
		// Flow control buttons
		//

		/// <summary>
		/// The previous button has been clicked. Notify the flowcontroller
		/// that we wish to load the previous page.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters</param>
		private void PreviousButton_Click(object sender, EventArgs e)
		{
			try
			{
				NextButton.SetPropertyThreadSafe(() => NextButton.Text, "Next");
				_hasNext = true;
				_hasPrevious = _flowController.Previous();
				ChangeNavigationButtonsEnabled(true);
			}
			catch (Exception exc)
			{
				// Anything that had to be handled has already been handled.
				// This is just a shortcircuit, so the navigationbuttons will not be altered.
				MessageLogger.CatchException(this, exc, Severity.ERROR,
					"Could not load previous page: \"" + exc.Message + "\"");
			}
		}

		/// <summary>
		/// The next button has been clicked. Notifiy the flowcontroller 
		/// that we wish to load the next page.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters</param>
		private void NextButton_Click(object sender, EventArgs e)
		{
			try
			{
				// disable buttons until we receive a pageUpdate event.
				ChangeNavigationButtonsEnabled(false);
				_hasPrevious = true;
				_oldNextButtonText = NextButton.Text;

				_hasNext = _flowController.Next();
				NextButton.SetPropertyThreadSafe(() => NextButton.Text, (_hasNext) ? "Next" : "Finish");
			}
			catch (Exception exc)
			{
				// Anything that had to be handled has already been handled.
				// This is just a shortcircuit, so the navigationbuttons will not be altered.
				MessageLogger.CatchException(this, exc, Severity.ERROR,
					"Could not load next page: \"" + exc.Message + "\"");
			}
		}

		// 
		// Visual updates
		//

		/// <summary>
		/// Change the enabled status of the navigation buttons
		/// </summary>
		/// <param name="enabled">True to enable the navigation buttons, false to 
		/// disable them.</param>
		private void ChangeNavigationButtonsEnabled(bool enabled)
		{
			NextButton.SetPropertyThreadSafe(() => NextButton.Enabled, enabled);
			PreviousButton.SetPropertyThreadSafe(() => PreviousButton.Enabled, enabled && _hasPrevious);
		}


		// 
		// AbstractFlowController event handlers
		//
		#region flow_controller_event_handlers

		public void HandlePageUpdate(UserControl control)
		{
			// We have no more pages to display, so just close the form.
			if (control == null)
			{
				lock (this)
				{
					/*
					Closed = true;
					Thread.Sleep(1000);
					this.BeginInvoke((MethodInvoker)(() => this.Close()));
					 */
					MakePageCloseReady();
				}
				return;
			}

			PageUpdateWork(control);
			ChangeNavigationButtonsEnabled(true);
		}

		private delegate void MakePageCloseReadyDelegate();
		private void MakePageCloseReady()
		{
			if (this.InvokeRequired)
			{
				MakePageCloseReadyDelegate del = new MakePageCloseReadyDelegate(MakePageCloseReady);
				this.Invoke(del);
			}
			else
			{
				ChangeNavigationButtonsEnabled(false);
				NextButton.Enabled = true;
				PreviousButton.Visible = false;
				NextButton.Click += new EventHandler(delegate(object sender, EventArgs args)
				{
					_closed = true;
					this.BeginInvoke((MethodInvoker)(() => this.Close()));
				});
			}
		}

		private delegate void PageUpdateDelegate(UserControl control);
		private void PageUpdateWork(UserControl control)
		{
			if (InvokeRequired)
			{
				PageUpdateDelegate del = new PageUpdateDelegate(PageUpdateWork);
				Invoke(del, new object[] { control });
			}
			else
			{
				//ProgressBar.Visible = false;
				NextButton.Text = (_hasNext) ? "Next" : "Finish";

				_activePageControl.Visible = false;
				this.Layout.Controls.Remove(control);

				_activePageControl = control;
				_activePageControl.Dock = DockStyle.Fill;
				this.Layout.Controls.Add(control, 0, 1);
				_activePageControl.Visible = true;
			}
		}

		/// <summary>
		/// Hanlde the update of info
		/// </summary>
		/// <param name="infoParams"></param>
		public void HandleInfoUpdate(InfoParameters infoParams)
		{
			InfoLbl.SetPropertyThreadSafe(() => InfoLbl.Text, infoParams.Message);
			if (infoParams.ShowInfo)
			{
				InfoLbl.SetPropertyThreadSafe(() => InfoLbl.Visible, true);
				InfoLbl.SetPropertyThreadSafe(() => InfoLbl.Font, 
					(infoParams.Bold ? new Font(InfoLbl.Font,FontStyle.Bold) : new Font(InfoLbl.Font, FontStyle.Regular)));

				InfoLbl.SetPropertyThreadSafe(() => InfoLbl.ForeColor, infoParams.InfoColor);

				if (infoParams.Duration != -1)
				{
					System.Timers.Timer t = new System.Timers.Timer(infoParams.Duration);
					t.Elapsed += (sender, args) =>
					{
						t.Stop();
						t.Close();

						// Only hide the label if the original text is still being displayed.
						// And if the window has not been destroyed yet.
						lock (this)
						{
							if (!_closed && InfoLabel_TextRead_TS() == infoParams.Message)
							{
								InfoLbl.SetPropertyThreadSafe(() => InfoLbl.Visible, false);
							}
						}
					};
					t.Start();
				}
			}
			else
			{
				InfoLbl.SetPropertyThreadSafe(() => InfoLbl.Visible, false);
			}
		}

		/// <summary>
		/// Handle a preprocess update.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandlePreprocessProgress(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.STARTED:
					_stepCounter = 0;
					UpdatePreprocessLabels(args);
					ProcessInfoLbl.SetPropertyThreadSafe(() => ProcessInfoLbl.Visible, true);
					ProcessLbl.SetPropertyThreadSafe(() => ProcessLbl.Visible, true);

					break;
				case ProgressEventArgs.ProgressCode.STEP_COMPLETED:
					_stepCounter++;
					UpdatePreprocessLabels(args);

					break;
				case ProgressEventArgs.ProgressCode.DONE:
					UpdatePreprocessLabels(args, 3000, "100%");

					break;
				case ProgressEventArgs.ProgressCode.FAILED:
					UpdatePreprocessLabels(args, 3000, "");
					ChangeNavigationButtonsEnabled(true);
					InfoParameters ipm = new InfoParameters("Preprocess failed: " + args.Message, true, 5000, false);
					ipm.InfoColor = Color.Red;
					HandleInfoUpdate(ipm);

					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Update the status of the preprocess labels.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="displayTime">How long should the labels be visible? In milliseconds</param>
		/// <param name="percentage">An arbitrary completion percentage to display, or a calculated
		/// one if the parameter is null (default value).</param>
		private void UpdatePreprocessLabels(ProgressEventArgs args, int displayTime = 0, string percentage = null)
		{
			ProcessInfoLbl.SetPropertyThreadSafe(() => ProcessInfoLbl.Text, args.Message);
			ProcessLbl.SetPropertyThreadSafe(() => ProcessLbl.Text, (percentage == null) ? PreprocessPercentage() : percentage);

			if (displayTime > 0)
			{
				System.Timers.Timer t = new System.Timers.Timer(displayTime);
				t.Elapsed += delegate
				{
					t.Stop();
					t.Dispose();

					// Only hide the label if the text has not changed since it has been set.
					// And if the window hasn't been destoryed yet.
					lock (this)
					{
						if (!_closed && args.Message == ProcessInfoLabel_TextRead_TS())
						{
							ProcessInfoLbl.SetPropertyThreadSafe(() => ProcessInfoLbl.Visible, false);
							ProcessLbl.SetPropertyThreadSafe(() => ProcessLbl.Visible, false);
						}
					}
				};
				t.Start();
			}
		}

		/// <summary>
		/// Creates the percentage of completion string for the preprocessing label.
		/// </summary>
		/// <returns></returns>
		private string PreprocessPercentage()
		{
			int NrSteps = Math.Min(
					(int)((double)_stepCounter / 
					Math.Max(((IPreprocessTrackableAction)_flowController).NumberOfSteps, 1) * 100),
				100);
			return NrSteps.ToString() + "%";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleProcessProgress(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.STARTED:
					_stepCounter = 0;
					this.Invoke((MethodInvoker) delegate { ProgressBar.Value = ProgressBar.Minimum; });
					this.Invoke((MethodInvoker)delegate { ProgressBar.Maximum = ((IProcessTrackableAction)_flowController).NumberOfSteps; });
					ProgressBar.SetPropertyThreadSafe(() => ProgressBar.Visible, true);
					_showProgressBar = true;

					break;
				case ProgressEventArgs.ProgressCode.DONE:
					if (!IsDisposed)
					{
						_showProgressBar = false;
						this.Invoke((MethodInvoker)delegate { ProgressBar.Value = ProgressBar.Maximum; });
						ProgressBar.SetPropertyThreadSafe(() => ProgressBar.Visible, true);
						HandleInfoUpdate(new InfoParameters(args.Message, true, 5000, false));
						System.Timers.Timer t = new System.Timers.Timer(3000);
						t.Elapsed += delegate
						{
							t.Stop();
							t.Dispose();

							lock (this)
							{
								if (!_closed && !_showProgressBar)
								{
									ProgressBar.SetPropertyThreadSafe(() => ProgressBar.Visible, false);
								}
							}
						};
						t.Start();
					}
					break;

				case ProgressEventArgs.ProgressCode.FAILED:
					_showProgressBar = false;
					ProgressBar.SetPropertyThreadSafe(() => ProgressBar.Visible, true);
					InfoParameters ipm = new InfoParameters("Processing failed: " + args.Message, true, 3000, false);
					ipm.InfoColor = Color.Red;
					HandleInfoUpdate(ipm);
					System.Timers.Timer t2 = new System.Timers.Timer(3000);
					t2.Elapsed += delegate
					{
						t2.Stop();
						t2.Dispose();

						lock (this)
						{
							if (!_closed && !_showProgressBar)
							{
								ProgressBar.SetPropertyThreadSafe(() => ProgressBar.Visible, false);
							}
						}
					};
					t2.Start();
					NextButton.SetPropertyThreadSafe(() => NextButton.Text, _oldNextButtonText);
					ChangeNavigationButtonsEnabled(true);

					break;
				case ProgressEventArgs.ProgressCode.STEP_COMPLETED:
					_stepCounter++;
					this.Invoke((MethodInvoker)delegate { ProgressBar.Maximum = ((IProcessTrackableAction)_flowController).NumberOfSteps; });
					this.Invoke((MethodInvoker) delegate { ProgressBar.Value = Math.Min(_stepCounter, ProgressBar.Maximum); });

					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Handles the FlowClose event of the AbstractFlowController.
		/// In case the AbstractFlowController requests a close of the flow, we close down
		/// the flowGUI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnFlowClose(object sender, EventArgs args)
		{
			this.Close();
		}

		#endregion

		#region Thread_safe_helper_methods
		/// <summary>
		/// Read the text from a label, thread safe.
		/// </summary>
		private delegate string LabelTextReadDelegate();
		private string ProcessInfoLabel_TextRead_TS()
		{
			if (InvokeRequired)
			{
				LabelTextReadDelegate del = new LabelTextReadDelegate(ProcessInfoLabel_TextRead_TS);
				return (string)Invoke(del);
			}
			else
			{
				return ProcessInfoLbl.Text;
			}
		}

		private string InfoLabel_TextRead_TS()
		{
			if (InvokeRequired)
			{
				LabelTextReadDelegate del = new LabelTextReadDelegate(InfoLabel_TextRead_TS);
				return (string)Invoke(del);
			}
			else
			{
				return InfoLbl.Text;
			}
		}

		private delegate IntPtr HandleReadDelegate();
		private IntPtr ReadHandleValue_TS()
		{
			if (InvokeRequired)
			{
				HandleReadDelegate del = new HandleReadDelegate(ReadHandleValue_TS);
				return (IntPtr)Invoke(del);
			}
			else
			{
				return Handle;
			}
		}
		#endregion
	}
}
