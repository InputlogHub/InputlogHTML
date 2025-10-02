using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUI.Flow;
using InputLog.Core.Util.Matching;
using InputLog.Core.Util.Matching.TraversalPolicies;
using InputLog.Core.Util.Matching.MatchPolicies;
using System.Threading;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util;
using InputLog.Core.Util.Validation;
using System.Windows.Forms;
using GUI.Util;
using System.Drawing;
using InputLog.Core.Merging.Sources.ProcessTasks;
using InputLog.Core.Merging;
using System.Text.RegularExpressions;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	class FileSelectAndMatchFlowPage<V>: AbstractFlowPage
        where V : IValidator<string>, new()
	{
		#region private_fields
		// Extensions used for file matching
		private string EyeTrackExt;
		private string IdfxExt;

		/// <summary>
		/// A thread used for background work. 
		/// </summary>
		private Thread Worker;

		/// <summary>
		/// The Specific UserControl used in this flow page.
		/// </summary>
		private FileSelectAndMatch SpecificControl
		{
			get
			{
				return (FileSelectAndMatch)Control;
			}
		}

		/// <summary>
		/// List of all the matches on the page.
		/// </summary>
		private List<IMatch<string>> Matches;

		/// <summary>
		/// The preprocess task.
		/// </summary>
		private ProcessTask _PreprocessTaskInstance;

		/// <summary>
		/// The process task.
		/// </summary>
		private ProcessTask _ProcessTaskInstance;
		#endregion

		#region public_fields

		public delegate void HandleOffsetsChanged(Dictionary<IMatch<string>, int> offsets);
		public event HandleOffsetsChanged OffsetsChanged;

		/// <summary>
		/// The calculated offsets, matched to their 'Match'. 
		/// The match index is the largest common path of a match set.
		/// </summary>
		private Dictionary<IMatch<string>, int> _offsetsByMatch;
		public Dictionary<IMatch<string>, int> OffsetsByMatch
		{
			get
			{
				return _offsetsByMatch;
			}
			private set
			{
				if (value != _offsetsByMatch)
				{
					_offsetsByMatch = value;

					if (OffsetsChanged != null)
					{
						OffsetsChanged(_offsetsByMatch);
					}
				}
			}
		}
		#endregion


		public FileSelectAndMatchFlowPage(string otherExt, string unselectPattern = "^$", string idfxExt = ".idfx"): 
			base(new FileSelectAndMatch(unselectPattern), true, true, 
			"Select the directory containing the files to be merged. " + 
			"Top level directories should hold the " + otherExt + " file, their subdirectories .idfx files.")
		{
			// Listen to data changes in the control.
			SpecificControl.DataChanged += HandleDataChanged;
			SpecificControl.SelectionChanged += HandleSelectionChanged;
            IdfxExt = idfxExt;
            EyeTrackExt = otherExt;
		}

		/// <summary>
		/// Destructor. Closes off any working threads still working.
		/// </summary>
		~FileSelectAndMatchFlowPage()
		{
			StopAllProcessing();
			if (Worker != null && Worker.IsAlive)
			{
				Worker.Abort();
			}
		}

		//
		// Preprocessing and processing
		//
		#region processing_methods
		/// <summary>
		/// Get the preprocess task of this page.
		/// </summary>
		/// <returns>The preprocess task, or null if no such task is defined for this page.</returns>
		protected override InputLog.Core.Util.Progress.ProcessTask GetPreprocessTask()
		{
			if (_PreprocessTaskInstance == null)
			{
				_PreprocessTaskInstance = new FileValidator<V>(new V());
				_PreprocessTaskInstance.ProcessListeners += HandlePreprocessUpdate;
			}
			return _PreprocessTaskInstance;
		}

		/// <summary>
		/// Get the process task for this page.
		/// </summary>
		/// <returns>the process task, or null if no such task is defined for this page.</returns>
		protected override InputLog.Core.Util.Progress.ProcessTask GetProcessTask()
		{
			// We create a new task for this each time we start the processing.
			_ProcessTaskInstance = OffsetCalculatorFactory.Create(Matches);
			_ProcessTaskInstance.ProcessListeners += HandleProcessUpdate;
			return _ProcessTaskInstance;
		}
		#endregion

		//
		// GUI Event Handling
		//
		#region directory_changes
		private void HandleDataChanged(FileSelectAndMatch.Zones zone)
		{
			switch (zone)
			{
				case FileSelectAndMatch.Zones.SELECTED_DIRECTORY:
					// The directory has changed. Redo the initial input file matching.
					DirectoryChanged();
					break;
			}
		}

		/// <summary>
		/// The selected directory has changed.
		/// </summary>
		private void DirectoryChanged()
		{
			// Check if the worker thread is still working, if it is we kill it first.
			if (Worker != null && Worker.IsAlive)
			{
				Worker.Abort();
			}

			DocumentMatchFinder<RecursiveFolderTraversal, ParentSubDirExtensionMatching> matchFinder =
				new DocumentMatchFinder<RecursiveFolderTraversal, ParentSubDirExtensionMatching>(
					new RecursiveFolderTraversal(),
					new ParentSubDirExtensionMatching()
				);

			matchFinder.Matcher.InitializeChildExtensions(new HashSet<string>(new string[] { IdfxExt }), true);
			matchFinder.Matcher.InitializeParentExtensions(new HashSet<string>(new string[] { EyeTrackExt }), true);

			// Find the matches in a separate thread.
			Worker = new Thread(delegate()
			{
				try
				{
					SpecificControl.ClearMatches();
					Matches = matchFinder.GetMatches(SpecificControl.SelectedPath);
					SpecificControl.NewMatches(Matches);
					if (Matches != null)
					{
						if (IsCurrentlyPreprocessing)
						{
							ResetThePreprocessing();

						}
						else
						{
							Preprocess();
						}

						List<string> files = new List<string>();
						Matches.ForEach(match => files.AddRange(match.Items()));
						((FileValidator<V>)_PreprocessTaskInstance).AddFiles(new List<string>(files));
					}
				}
				catch (ThreadAbortException)
				{
					// Reset the matches of the control and don't do anything else.
					SpecificControl.ClearMatches();
				}
				catch (Exception exc)
				{
					SpecificControl.ClearMatches();
					MessageLogger.CatchException(this, exc, Severity.ERROR, "Could not generate matches for specified folder. Please try again.");
				}
			});
			Worker.Start();
		}
		#endregion

		// 
		// Handling file selection changes
		//
		#region file_selection_changes
		/// <summary>
		/// Handle the selection changes of files in the GUI.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="selected">True if files are being selected, false if they are being deselected</param>
		/// <param name="files">List of files that is affected by the change, or null if it involves all the files.</param>
		private void HandleSelectionChanged(object sender, bool selected, List<string> files)
		{
			if (files != null)
			{
				foreach (string file in files)
				{
					List<IMatch<string>> affectedMatches = Matches.FindAll(match => match.Items().Contains(file));
					affectedMatches.ForEach(match => match.ChangeItemSelection(file, selected));
				}
			}
			else
			{
                Matches.ForEach(match => match.Items().ToList().ForEach(file => match.ChangeItemSelection(file, selected)));
			}
		}
		#endregion

		//
		// Handling Processtasks progress
		//
		#region process_task_progresstracking
		/// <summary>
		/// Handle updates from the preprocessing task.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		[STAThread]
		private void HandlePreprocessUpdate(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.DONE:
				case ProgressEventArgs.ProgressCode.FAILED:

					Dictionary<string, List<FileValidator<V>.FileIssue>> issues = 
						((FileValidator<V>)sender).GetIssues();

					if (issues.Count > 0)
					{
						ControlDialog dialog = new ControlDialog();
						dialog.ExplanationLbl.Text = 
							"These files could not be used for merging, " +
							"they will be automatically unselected.";
						TableLayoutPanel layout = dialog.Controls.OfType<TableLayoutPanel>().First();
						ListView listView = layout.Controls.OfType<ListView>().First();
						PopulateIssueListView(listView, issues);

						// Show dialog until it is closed.
						// But, wait in this thread until the dialog is closed.
						dialog.OkButton.Click += new EventHandler(delegate
						{
							dialog.Close();
						});
						dialog.ShowDialog();

						while (dialog.Visible && !Closed)
						{
							Thread.Sleep(200);
						}


						if (!Closed)
						{
							// Deselect the issue-files in the flow page GUI
							SpecificControl.MarkProblemFiles(issues.Keys.ToList());
						}
					}


					break;
				default:
					break;
			}
		}

		private void PopulateIssueListView(ListView list,
			Dictionary<string, List<FileValidator<V>.FileIssue>> issues)
		{
			string longestFile = ""; ;

			foreach (string file in issues.Keys)
			{
				var fileIssues = issues[file];
				foreach (var fileIssue in fileIssues)
				{
					ListViewItem item = new ListViewItem(new string[] {
						fileIssue.File,
						fileIssue.Description,
						((fileIssue.Exc != null) ? fileIssue.Exc.StackTrace : "")
					});
					list.Items.Add(item);

					longestFile = (longestFile.Length >= fileIssue.File.Length) ? longestFile : fileIssue.File;
				}
			}

			Graphics g = list.CreateGraphics();
			SizeF size = g.MeasureString(longestFile, new Font("Microsoft Sans Serif",8.25f));
			list.Columns[0].Width = (int)Math.Floor(size.Width + 0.5f);
		}

		/// <summary>
		/// Handle updates from the processing task.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleProcessUpdate(object sender, ProgressEventArgs args)
		{
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.DONE:
					var offsetsDict = new Dictionary<IMatch<string>, int>();
					int?[] offsets = ((IOffsetCalculator)sender).GetOffsets();

					for (int i = 0; i < Matches.Count; i++)
					{
						// Don't add matches that have been completely unselected.
						if (Matches[i].SelectedItems().Count > 0)
						{
							offsetsDict.Add(Matches[i], offsets[i].Value);
						}
					}
					// This triggers the OffsetsChanged event.
					OffsetsByMatch = offsetsDict;

					break;
				case ProgressEventArgs.ProgressCode.FAILED:
				case ProgressEventArgs.ProgressCode.STARTED:
				default:
					OffsetsByMatch = null;
					break;
			}
		}
		#endregion

	}
}
