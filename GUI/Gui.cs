using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GUI.Session;
using GUI.Settings;
using GUI.Tabs.Record;
using GUI.Util;
using InputLog.Core.Util;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using InputLog.Core.Util.Server;
using GUI.Tools;
using GUI.Tools.Copytask_Creator;
using GUI.Tools.Reporting;
using System.Threading;

namespace GUI
{
	/// <summary>
	/// Graphical user interface for InputLog.
	/// </summary>
	public partial class Gui : Form
	{
		#region Fields
		/// <summary>
		/// The application Settings.
		/// </summary>
		private readonly Properties.Settings Settings = Properties.Settings.Default;

		/// <summary>
		/// Returns the path to the session state file in the currrent workspace directory.
		/// </summary>
		private string SessionSavePath
		{
			get { return Path.Combine(Settings.Workspace, Settings.SessionStateFileName); }
		}

		/// <summary>
		/// Gets and sets the document selected in the open file dialog or in the recent files list in the menu bar.
		/// See: WordLogOptions.
		/// </summary>
		public class PathChanged : EventArgs
		{
			public string ThisPath { get; set; }
		}

		/// <summary>
		/// Do not remove.
		/// PathChangeHandler is subscribed to by WordLogOptions.
		/// </summary>
		public static event PathHandler PathChangeHandler;
		public delegate void PathHandler(Gui gui, PathChanged e);


		/// <summary>
		/// The current output directory.
		/// </summary>
		public DirectoryInfo OutputDir
		{
			get { return OutputDir; }
			private set
			{
				//TODO !!!
				//this.Record.
			}
		}

		/// <summary>
		/// Gets or sets a boolean that defines whether the GUI is minimized to the system tray or not.F
		/// </summary>
		public bool Hidden
		{
			private get { return !ShowInTaskbar; }
			set
			{
				if (value)
				{
					Hide();
				}
				else
				{
					Show();
					Activate();
				}

				ShowInTaskbar = !value;
			}
		}

		/// <summary>
		/// The Recent Files list manager
		/// </summary>
		public static RecentFilesList MyRecentFiles;

		/// <summary>
		/// The different tabnames, in an enumeration.
		/// </summary>
		public enum TabNames
		{
			RECORD,
			PREPROCESS,
			ANALYZE,
			POSTPROCESS,
			PLAY,
		}

		/// <summary>
		/// Delegate method that allows you to select a tab by
		/// giving the tabname.
		/// </summary>
		/// <param name="tab">Name of the tab.</param>
		private delegate void TabSelected(TabNames tab);

		/// <summary>
		/// The instance of the GUI.
		/// </summary>
		private static Gui _guiInstance;

		/// <summary>
		/// Key used for locking when getting the instance
		/// </summary>
		private static readonly object Key = new object();

		#endregion

		/// <summary>
		/// Get the instance of the gui.
		/// </summary>
		/// <param name="args">Command line arguments passed through the GUI</param>
		/// <returns>The single instance of the GUI.</returns>
		public static Gui GetInstance(string[] args = null)
		{
			lock (Key)
			{
				if (_guiInstance == null)
				{
					_guiInstance = new Gui(args);
				}
			}
			return _guiInstance;
		}

		/// <summary>
		/// Constructor, private because we use the Singleton pattern.
		/// </summary>
		/// <param name="args">The CLI arguments.</param>
		private Gui(string[] args)
		{
			InitializeComponent();
			Tabs.SelectedIndex = 0;

			// HACK 12 sept. 2012: 
			// This if-structure has been added to work around a bug that disables the entire Play tab in the
			// after the call in Gui.cs to ComponentResourceManager.ApplyResources to apply the 
			// resources to the play tab.
			// I have not found the actual reason why this call suddenly starts disabling the 
			// play tab and this check is merely part of a simple work around!
			// 
			if (PlayTab.EmbeddingSuccessful)
			{
				PlayTab.Enabled = true;
			}

			RecordTab.GUI = this;
			RecordTab.AnalyzeTab = AnalyzeTab;
			RecordTab.PlayTab = PlayTab;
		   
			RecordTab.RecordStateChanged += OnRecordStateChanged;
			AnalyzeTab.Gui = this;
			PlayTab.GUI = this;
			ServerUtils.Initialize();
			MessageLogger.ExceptionCaught += ExceptionCaught;
			MessageLogger.MessageReceived += MessageReceived;

			try
			{
				if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Workspace))
				{
					Settings.Workspace = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InputLog");
					Settings.Save();
				}

				UpdateRecordSession();

				// Parse arguments (only one argument is supported now)
				if (args != null && args.Length > 0)
				{
					var fi = new FileInfo(args[0]);
					// Use substring to remove the dot.
					if (Properties.Settings.Default.Logfile_extension.Equals(fi.Extension.Substring(1)))
					{
						PlayTab.SetReplaySources(fi.FullName, ""); // Where is the original doc located?
						AnalyzeTab.Activate(fi.FullName);
					}
				}
			}
			catch (Exception ex)
			{
				MessageLogger.CatchException(this, ex, Severity.ERROR, "Unable to load GUI:\r\n" + ex);
			}

			// Subscribe to the Preprocess Path Change Handler
			PreprocessTab.PathChanged += ChangePath;
			// Subscribe to the Analyze Path Change Handler
			AnalyzeTab.PathChanged += ChangePath;
			// Subscribe to the Play Change Handler
			PlayTab.PathChanged += ChangePath;
		}

		/// <summary>
		/// Callback from the tabs Preprocess, Analyzer, and Play with the latest document path.
		/// </summary>
		/// <param name="sender">GuiPathChangedEvent</param>
		/// <param name="e">path to the most recent selected document(s)</param>
		private void ChangePath(object sender, GuiPathChangedEvent.PathChangedEventArgs e)
		{
			OpenFile(StringUtils.JoinPaths(e.ThisSourcePaths));
		}

		/// <summary>
		/// Updates the record tab to the session saved in the workspace directory 
		/// (if such a saved session is found).
		/// </summary>
		public void UpdateRecordSession()
		{
			RecordSession session = RecordSession.OpenSession(SessionSavePath);

			if (session != null)
			{
				RecordTab.SetSessionData(session);
			}
		}

		/// <summary>
		/// Callback for the account button in the Tools menu.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The event arguments.</param>
		private void AccountSettingsButtonClick(object sender, EventArgs e)
		{
			var options = new InputLog.Core.Util.Server.AccountSettingsWindow();
			try
			{
				options.ShowDialog();
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR);
			}
			finally
			{
				options.Dispose();
			}
		}

		/// <summary>
		/// Callback for the close button in the toolstrip menu.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">Empty</param>
		private void QuitButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Drag event handler.
		/// Allows the user to drag files to the GUI (UI automatically switches to the correct tab using the filetype).
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event Arguments.</param>
		private void GuiDragEnter(object sender, DragEventArgs e)
		{
			try
			{
				// if we are dropping a file, check the extension and switch tab accordingly.
				if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
				{
					var files = (string[])e.Data.GetData(DataFormats.FileDrop);
					if (files.Length >= 1)
					{
						// FUTURE support batch processing
						string extension = Path.GetExtension(files[0]);
						switch (extension)
						{
							// IDFX file, switch to analyze tab
							case ".idfx":
								{
									e.Effect = DragDropEffects.All;
									// switch tab if the convert tab is not selected.
									// If the convert tab is selected, keep it selected ==> 
									// it is possible to convert idfx to an other format.
									if (Tabs.SelectedIndex != 3)
									{
										Tabs.SelectedIndex = 2;
									}
									break;
								}
							// XML OR IDF file, switch to convert tab
							case ".xml":
							case ".idf":
								{
									e.Effect = DragDropEffects.All;
									Tabs.SelectedIndex = 3;
									break;
								}
						}
					}
				}
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR);
			}
		}

		/// <summary>
		/// Callback for whenever an exception is caught.
		/// </summary>
		/// <param name="sender">The sender of the exception.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void ExceptionCaught(object sender, ExceptionEventArgs eventArgs)
		{
            Thread dialogThread = new Thread(new ThreadStart(() => {
                ExceptionMessageBox.Show(eventArgs.Exception, eventArgs.Severity, "Inputlog: Error", eventArgs.Message);
            }));
            dialogThread.Name = "ExceptionDialogThread";
            dialogThread.Start();
		}

		/// <summary>
		/// Callback for whenever a log message is received.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void MessageReceived(object sender, MessageEventArgs eventArgs)
		{
			var icon = ToolTipIcon.None;
			switch (eventArgs.Severity)
			{
				case Severity.DEBUG:
					// Leave icon as is
					break;
				case Severity.INFO:
					icon = ToolTipIcon.Info;
					break;
				case Severity.WARNING:
					icon = ToolTipIcon.Warning;
					break;
				case Severity.ERROR:
					icon = ToolTipIcon.Error;
					break;
				case Severity.FATAL:
					icon = ToolTipIcon.Error;
					break;
			}

			if (!string.IsNullOrEmpty(eventArgs.Title))
			{
				NotifyIcon.ShowBalloonTip(Properties.Settings.Default.BalloonTipTimeOut,
										  eventArgs.Title, eventArgs.Message, icon);
			}
		}

		/// <summary>
		/// Callback when the GUI is closed, deregisters the GUI as an exceptionlistener.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void GuiFormClosed(object sender, FormClosedEventArgs e)
		{
			MessageLogger.ExceptionCaught -= ExceptionCaught;
		}

		/// <summary>
		/// Click-handler for the Record-item of the contextmenu,
		/// just passes it on to the RecordButton of the Record tab.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void ContextMenuRecordItemClick(object sender, EventArgs e)
		{
			RecordTab.RecordButtonClick(sender, e);
		}

		/// <summary>
		/// Callback for when the recording state has chagned.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void OnRecordStateChanged(object sender, RecordStateChangedEventArgs e)
		{
			if (e.Recording)
			{
				ContextMenuRecordItem.Text = "Stop Recording";
				if (Properties.Settings.Default.AutoHide)
				{
					Hidden = true;
				}
			}
			else
			{
				ContextMenuRecordItem.Text = "Record";
				if (Properties.Settings.Default.AutoHide)
				{
					Hidden = false;
				}
				//else
				//{
				//    WindowState = FormWindowState.Minimized;
				//}
			}
		}

		/// <summary>
		/// Callback when the form is about to close, saves the session state.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void GuiFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveRecordSession();
		}

		/// <summary>
		/// Saves the current state (meta data) of the record session.
		/// </summary>
		public void SaveRecordSession()
		{
			try
			{
				RecordSession session = RecordTab.GetSessionInfo();
				session.Save(SessionSavePath);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.WARNING, "Unable to save session data.");
			}
		}

		/// <summary>
		/// Callback when the notify icon is double clicked, shows or hide the GUI.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void NotifyIconMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Hidden = !Hidden;
			}
		}

		// Show application version in the form border and recently opened files in the File Menu
		private void GuiLoad(object sender, EventArgs e)
		{
			Text = Application.ProductName + " " + Application.ProductVersion;
			MyRecentFiles = new RecentFilesList("Inputlog", RecentFilesButton, 5);
			MyRecentFiles.FileSelected += MyRecentFilesFileSelected;
		}

		private void MyAccountToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputLogServer"] + "Task/");
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to the InputLog server.");
			}
		}

		/// <summary>
		/// File Open button clicked - Starts file dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenButtonClick(object sender, EventArgs e)
		{
			var thisOpenFileDialog = new OpenFileDialog 
			{
				InitialDirectory = SessionSavePath,
				Filter =
				"idfx files(*.idfx)|*.idfx|Doc Files(*.doc;*.docx;*.docm)" 
					+ "|*.doc; *.docx; *. docm|All files(*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};
			if (thisOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = thisOpenFileDialog.FileName;
				OpenFile(fileName);
			}
		}

		/// <summary>
		///  Open a file and add it to the Recent File list.
		/// </summary>
		/// <param name="fileName"></param>
		public void OpenFile(string fileName)
		{
			try
			{
				if (fileName == null) return;

				if (fileName.EndsWith("idfx"))
				{
					// One or more selected files possible in Preprocess or Analyze.
					// Preprocess and Analyze expect a list of file paths.
					PreprocessTab.SelectFile(fileName); 
					AnalyzeTab.SelectSourceFile(fileName);    
			   
					// RecentFiles and Play expect a string with one path.
					// They get the last file when many were selected.
					List<string> thisFiles = StringUtils.SplitPaths(fileName);
					if (thisFiles.Count > 1)
					{
						MyRecentFiles.AddFile(thisFiles.Last());
						PlayTab.SelectFile(thisFiles.Last());   
					}
					else
					{
                        MyRecentFiles.AddFile(fileName);
                        PlayTab.SelectFile(fileName);
                    }
                }
			}
			catch (Exception ex)
			{
			   // Remove the file from the Recent File list.
			   MyRecentFiles.RemoveFile(fileName);
			   MessageLogger.CatchException(this, ex, Severity.WARNING, 
				   string.Format("File {0} doesn't exist. Its name is removed from the Recent File list.", fileName));
			}
		}

		/// <summary>
		///  Open a file selected from the Recent File list.
		/// </summary>
		/// <param name="fileName"></param>
		private void MyRecentFilesFileSelected(string fileName)
		{
			OpenFile(fileName);
		}

		/// <summary>
		/// Callback for the options button in the File menu.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OptionsButtonClick(object sender, EventArgs e)
		{
			var options = new SettingsWindow(this);
			try
			{
				options.ShowDialog();
				RecordTab.UpdateWinLogRestricted();
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR);
			}
			finally
			{
				options.Dispose();
			}
		}

		/// <summary>
		/// Redirecting to the relevant Inputlog web page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InputlogHelpToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogHelpURL"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}
		
		/// <summary>
		/// Redirecting to the relevant Inputlog web page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InputlogTourToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogTourURL"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}

		/// <summary>
		/// Redirecting to the relevant Inputlog web page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InputlogOnTheWebToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogWebURL"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}

		/// <summary>
		/// Redirecting to the relevant Inputlog web page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AboutInputlogToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogAboutURL"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}

		/// <summary>
		/// Redirecting to the relevant Inputlog web page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckForUpdatesToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogDownloadURL"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}

		/// <summary>
		/// Open the tab with given tabName. If the
		/// the tab is already selected, this method does nothing.
		/// 
		/// This method is thread safe!
		/// </summary>
		/// <param name="tab">Identifier of the tab to be selected.</param>
		public void SelectTab(TabNames tab)
		{
			if (Tabs.InvokeRequired)
			{
				TabSelected delegateMethod = SelectTab;
				Invoke(delegateMethod, tab);
			}
			else
			{
				// Select the correct tab.
				switch (tab)
				{
					case TabNames.RECORD:
						Tabs.SelectedIndex = 0;
						break;
					case TabNames.PREPROCESS:
						Tabs.SelectedIndex = 1;
						break;
					case TabNames.ANALYZE:
						Tabs.SelectedIndex = 2;
						break;
					case TabNames.POSTPROCESS:
						Tabs.SelectedIndex = 3;
						break;
					case TabNames.PLAY:
						Tabs.SelectedIndex = 4;
						break;
				}
			}
		}

		private void InputlogManualpdfToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Process.Start(ConfigurationManager.AppSettings["InputlogManualPdf"]);
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to connect to InputLog website.");
			}
		}

		private void CopytaskIDFXManToolStripMenuItemClick(object sender, EventArgs e)
		{
			CopytaskIDFXManipulator idfxManipulator = new CopytaskIDFXManipulator();
			idfxManipulator.ShowDialog();
		}

		private void CopytaskCreatorToolStripMenuItemClick(object sender, EventArgs e)
		{
			CopyTaskCreator ctCreator = new CopyTaskCreator();
			ctCreator.Show();
		}

        private void LogfileNameRestoreToolStripMenuItemClick(object sender, EventArgs e)
        {
            CopytaskFixFilenames nameFixer = new CopytaskFixFilenames();
            nameFixer.Show();
        }

        private void editDefaultValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceEditor editor = new ResourceEditor();
            if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
            else
            {

            }

        }

        private void inspectTemplateErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateErrorReporter errorReporter = new TemplateErrorReporter();
            errorReporter.ShowDialog();
        }
	}
}