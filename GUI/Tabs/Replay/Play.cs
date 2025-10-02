 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EmbeddedWord;
using GUI.Properties;
using InputLog.Core.Analyses.Revision.Replay;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.Util;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace GUI.Tabs.Replay
{
    /// <summary>
    /// Play tab and its controls.
    /// </summary>
    public partial class Play : GuiPathChangedEvent //UserControl
    {
        #region Fields

        /// <summary>
        /// The engine used for the replay.
        /// </summary>
        private PlaybackEngine Engine;

        /// <summary>
        /// Reference to the GUI.
        /// </summary>
        public Gui GUI;

        /// <summary>
        /// True if there is currently a document being replayed.
        /// </summary>
        private bool Replaying;

        /// <summary>
        /// Full path to the source file. 
        /// </summary>
        private string SourceFile;

        /// <summary>
        /// The current RevisionAnalysisSummary which is being replayed.
        /// Null if not replaying anything.
        /// </summary>
        private RevisionAnalysisSummary Summary;

        /// <summary>
        /// The EmbededWordComponent that is embedded in EmbeddedWord.
        /// </summary>
        private EmbeddedWordComponent Word
        {
            get { return EmbeddedWord.Word; }
        }

        private Application app;
        private Document doc;

        /// <summary>
        /// The settings of the application.
        /// </summary>
        private Properties.Settings Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <summary>
        /// Returns whether or not the embedded word component integrated successfully.
        /// ---
        /// HACK 12 sept. 2012: 
        /// This property is added to work around a bug that disables the entire Play tab in the
        /// after the call in Gui.cs to ComponentResourceManager.ApplyResources to apply the 
        /// resources to the play tab.
        /// I have not found the actual reason why this call suddenly starts disabling the 
        /// play tab and this datamember is merely part of a simple work around!
        /// </summary>
        public bool EmbeddingSuccessful
        {
            get { return EmbeddedWord.Exception == null; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Play()
        {
            InitializeComponent();

            if (EmbeddedWord.Exception == null)
            {
                // Every tick of the timer should advance to the next edit, which is what NextButton_Click does.
                PlayTimer.Tick += PlayTimerTick;

                // Update the EmbeddedWord object properties
                //var aDoc = EmbeddedWord.Word.ActiveDocument as Microsoft.Office.Interop.Word.Document;
                // TODO: Change the settings so that you can scroll in embedded word but nothing else!

                // More initialization
                PlaySpeedBar.Value = Settings.PlayBarValue;
                Replaying = false;
                UpdatePlaybackControls();
            }
            else
            {
                string msg = string.Format("A COMException was caught during the initialization of the Play tab." 
                    + "(DSOFramer.ocx is probably not registered.) As a consequence, the play tab is disabled.\nMessage:\n{0}", 
                    EmbeddedWord.Exception.Message);
                ErrorLabel.Text = msg;
                ErrorLabel.Visible = true;
                if (SrcFilePanel != null)
                {
                    SrcFilePanel.Visible = false;
                    SrcFilePanel.SendToBack();
                }
                if (EmbeddedWord != null) EmbeddedWord.Visible = false;
                if (ControlPanel != null) ControlPanel.Visible = false;
                Enabled = false;
            }
        }

        /// <summary>
        /// Callback for when the play tab is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayLoad(object sender, EventArgs e)
        {
            if (GUI != null) // Hack/workaround for visual studio designer
                GUI.FormClosing += GUIFormClosing;
        }

        /// <summary>
        /// Callback for when the GUI is closing.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void GUIFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Replaying)
            {
                StopReplay();
            }
        }

        /// <summary>
        /// Sets the source loggg file and the source document fields to the given values.
        /// </summary>
        public void SetReplaySources(string logFile, string startDoc)
        {
            SrcFileTextField.Text = logFile;
            SrcFileTextField.Select(SrcFileTextField.Text.Length, 0);
            SrcDocTextField.Text = startDoc;
            SrcDocTextField.Select(SrcDocTextField.Text.Length, 0);
        }

        /// <summary>
        /// Click event for the "Browse" button with the source text field.
        /// Shows a FileSelectionDialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SrcFileButtonClick(object sender, EventArgs e)
        {
            try
            {
                FileDialog.Filter = "Log files (*.idfx)|*.idfx";
                FileDialog.DefaultExt = "idfx";
                DialogResult result = FileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    SrcFileTextField.Text = FileDialog.FileName;
                    SrcFileTextField.Select(SrcFileTextField.Text.Length, 0);
                    SourceFile = FileDialog.FileName;
                    OnPathChanged(new PathChangedEventArgs(new List<string> { SourceFile }));
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        public void SelectFile(string file)
        {
            SrcFileTextField.Text = file;
            SrcFileTextField.Select(SrcFileTextField.Text.Length, 0);
            FileDialog.InitialDirectory = Path.GetDirectoryName(file);
        }

        /// <summary>
        /// Click event for the "Browse" button with the source text field.
        /// Shows a FileSelectionDialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SrcDocButtonClick(object sender, EventArgs e)
        {
            try
            {
                FileDialog.Filter = "Doc files (*.docx;*.doc)|*.docx;*.doc";
                FileDialog.DefaultExt = "docx";
                DialogResult result = FileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    SrcDocTextField.Text = FileDialog.FileName;
                    SrcDocTextField.Select(SrcDocTextField.Text.Length, 0);
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Callback for when the start button was cliced.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void StartButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Replaying)
                {
                    StopReplay();
                }
                else
                {
                    StartReplay();
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Starts the replay by opening the source logging file etc.
        /// </summary>
        private void StartReplay()
        {

            {
                MessageBox.Show(
                    "Although utmost care has been taken to ensure the accuracy and reliability of Inputlog we cannot guarantee an error free replay of your process file. " +
                    "When logging data in the idfx, the program might have missed a minor feature of the MS Word environment." +
                    "This may or may not have an effect on what you see on the screen." +
                    "\nIf your research relies on the replay, we advise you to use a screencam recorder, e.g., Camstudio or Camtasia.",
                    "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // validate that srcfield is a valid file
            if (string.IsNullOrWhiteSpace(SrcFileTextField.Text))
            {
                MessageBox.Show("Please provide a source file.", "Invalid source file", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(SrcFileTextField.Text))
            {
                MessageBox.Show("The source file does not exist.", "Invalid source file",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // read events (and session identification) from src file
            try
            {
                EventLogReader eventLogReader =
                    EventLogFactory.CreateFileEventLogReader(SrcFileTextField.Text, LogFormat.XML);
                SessionIdentification sessionIdentification = eventLogReader.ReadSessionIdentification();
                List<Event> events = eventLogReader.ReadEvents();
                try
                {
                    Summary = (RevisionAnalysisSummary) new RevisionAnalysis(events, sessionIdentification,
                        SrcDocTextField.Text).DoAnalysis();
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR,
                        string.Format("Could not construct revision analysis of source file: {0}", SrcFileTextField.Text));
                    return;
                }

                // TODO clean?
                SourceFile = SrcDocTextField.Text;

                try
                {
                    Word.Visible = false;
                    if (string.IsNullOrWhiteSpace(SourceFile) || !File.Exists(SourceFile))
                    {
                        Word.CreateNew("Word.Document");
                    }
                    else
                    {
                        Word.Open(SourceFile, true, "Word.Document", Type.Missing, Type.Missing);
                    }

                    // Try and set the options of the embedded word document.
                    doc = (Document) Word.ActiveDocument;
                    app = doc.Application;
                    WordDocumentTools.DisableUnwantedOptions(app);

                    double version;
                    if (double.TryParse(((Document) Word.ActiveDocument).Application.Version, out version))
                    {
                        // Word 2003 does not support the Final property and for some reason its version number is 110.0
                        if (version > 110.0)
                        {
                            ((Document) Word.ActiveDocument).Final = true;
                        }
                    }

                    Word.Visible = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Unable to open Word document {0}\n{1}", SourceFile, e.Message),
                                    "Unable to open document", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Engine = new PlaybackEngine(Summary, (Document) Word.ActiveDocument);
                Word.Activate();

                // Disable unwanted options in Word

                StartButton.Text = "Stop";
                Replaying = true;
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to start the replay. " 
                    + "Are you sure you provided a valid logfile - document combination?");

                // Stop replaying
                StopReplay();
            }
        }

        /// <summary>
        /// Stops the replay and disposes the engine.
        /// </summary>
        private void StopReplay()
        {
            Summary = null;
            PlayTimer.Stop();
            if (Engine != null)
            {
                Engine.Dispose();
            }
            Engine = null;
            if (Word != null && Word.ActiveDocument != null)
            {
                Word.Close();
                app.Quit();
            }
            StartButton.Text = "Start";
            Replaying = false;
        }

        /// <summary>
        /// Click event for the Next button, advances to the next edit.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NextButtonClick(object sender, EventArgs e)
        {
            try
            {
                Engine.ToNext();
                EmbeddedWord.Focus();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Click event for the Next Revision button, advances to the next revision.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NextRevisionButtonClick(object sender, EventArgs e)
        {
            try
            {
                Engine.ToNextRevision();
                EmbeddedWord.Focus();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Updates the playback controls, enabling/disabling them whenever it is 
        /// possible/not possible to advance in that direction.
        /// </summary>
        private void UpdatePlaybackControls()
        {
            try
            {
                if (Replaying)
                {
                    RevisionBox.Text = Engine.CurrentRevisionNumber.ToString();
                    EditsBox.Text = Engine.CurrentEditNumber.ToString();
                    PositionBox.Text = Engine.CurrentPosition.ToString();
                    DocLengthBox.Text = Engine.CurrentDocLength.ToString();
                    ControlPanel.Enabled = true;

                    PlaySpeedBar.Enabled = !PlayTimer.Enabled;
                    ToBeginningButton.Enabled = Engine.HasPrevious;
                    PreviousRevisionButton.Enabled = Engine.HasPrevious;
                    PreviousButton.Enabled = Engine.HasPrevious;
                    PlayPauseButton.Enabled = Engine.HasNext;
                    NextButton.Enabled = Engine.HasNext;
                    NextRevisionButton.Enabled = Engine.HasNext;
                    ToEndButton.Enabled = Engine.HasNext;
                }
                else
                {
                    ControlPanel.Enabled = false;
                    RevisionBox.Text = "";
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Click event for the Next button, advances to the next edit.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PreviousButtonClick(object sender, EventArgs e)
        {
            try
            {
                Engine.ToPrevious();
                EmbeddedWord.Focus();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Click event for the Next button, advances to the next edit.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PreviousRevisionButtonClick(object sender, EventArgs e)
        {
            try
            {
                Engine.ToPreviousRevision();
                EmbeddedWord.Focus();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Click event for the ToBeginning button, goes back to the original document state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ToBeginningButtonClick(object sender, EventArgs e)
        {
            try
            {
                Engine.Dispose();

                if (string.IsNullOrWhiteSpace(SourceFile) || !File.Exists(SourceFile))
                {
                    Word.CreateNew("Word.Document");
                }
                else
                {
                    Word.Open(SrcDocTextField.Text, true, "Word.Document", Type.Missing, Type.Missing);
                }

                Engine = new PlaybackEngine(Summary, (Document) Word.ActiveDocument);
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Click event for the ToEnd button, advances to the end.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ToEndButtonClick(object sender, EventArgs e)
        {
            try
            {
                while (Engine.HasNext)
                {
                    Engine.ToNextRevision();
                }
                EmbeddedWord.Focus();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Click event for the PlayPause button, plays or pauses the replay.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PlayPauseButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (PlayTimer.Enabled)
                {
                    PlayTimer.Stop();
                    PlayPauseButton.Image = Resources.media_playback_start;
                }
                else
                {
                    // Save & set (rescaled) interval value
                    Settings.PlayBarValue = PlaySpeedBar.Value;
                    Settings.Save();
                    double rescaled = ((double) (PlaySpeedBar.Maximum - PlaySpeedBar.Value - PlaySpeedBar.Minimum))
                                      /(PlaySpeedBar.Maximum - PlaySpeedBar.Minimum)*
                                      (Settings.PlayIntervalMax - Settings.PlayIntervalMin)
                                      + Settings.PlayIntervalMin;
                    PlayTimer.Interval = (int) rescaled;
                    PlayPauseButton.Image = Resources.media_playback_pause;
                    PlayTimer.Start();
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Callback for a PlayTimer tick.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PlayTimerTick(object sender, EventArgs e)
        {
            try
            {
                if (Engine.HasNext) // If we can advance, simulate a next button click
                {
                    NextButtonClick(sender, e);
                }
                else // If we cannot any more, simulate a pause button click
                {
                    PlayPauseButtonClick(sender, e);
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);

                // Stop replaying
                StopReplay();
            }
            finally
            {
                UpdatePlaybackControls();
            }
        }

        /// <summary>
        /// Updates the original doc field to the document with the "_original" appendix in the same
        /// directory (if such a file exists).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arugments.</param>
        private void SrcFileTextFieldTextChanged(object sender, EventArgs e)
        {
            SrcFileTextField.Select(SrcFileTextField.Text.Length, 0);
            var dir = new DirectoryInfo(Path.GetDirectoryName(SrcFileTextField.Text));
            if (dir.Exists)
            {
                FileInfo[] docs = dir.GetFiles("*" + Settings.Original_doc_appendix + "*", SearchOption.TopDirectoryOnly);
                if (docs.Any())
                {
                    // Prefer docx over doc
                    FileInfo doc = docs.FirstOrDefault(fi => ".docx".Equals(fi.Extension)) ??
                                   docs.FirstOrDefault(fi => ".doc".Equals(fi.Extension));

                    if (doc != null)
                    {
                        SrcDocTextField.Text = doc.FullName;
                        SrcDocTextField.Select(SrcDocTextField.Text.Length, 0);
                    }
                    else
                    {
                        SrcDocTextField.Text = "";
                    }
                }
            }
        }

        private void EmbeddedWord_Load(object sender, EventArgs e)
        {

        }
    }
}