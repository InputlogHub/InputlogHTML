using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Session;
using GUI.Settings;
using GUI.Tabs.Record.Plugin.WordLog;
using GUI.Tabs.Replay;
using InputLog.Core;
using InputLog.Core.IO;
using InputLog.Core.IO.Xml;
using InputLog.Core.Plugin.WordLog;
using InputLog.Core.Util;

namespace GUI.Tabs.Record
{
    public partial class Record : UserControl
    {
        #region Fields

        /// <summary>
        /// Array of two-letter code text languages.
        /// </summary>
        public static readonly object[] LANGUAGES = AvailableLanguages.Languages;
        /// <summary>
        /// URL to the copy tasks on the inputlog website.
        /// </summary>
        private const string COPY_TASK_URL = "https://inputlog01.uantwerpen.be/Website/copyTask/tasks.html";           
        /// <summary>
        /// The language presented as a default value in the GUI.
        /// </summary>
        public const int DEFAULT_LANG = 0;

        /// <summary>
        /// The default rows that are contained by the session identification.
        /// </summary>
        private readonly DataGridViewCell[][] _sessionIDDefaults = {
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_PARTICIPANTKEY}, 
                new DataGridViewTextBoxCell { Value = "" }}, // Should stay at index 0
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_LANGUAGE}, 
                new DataGridViewComboBoxCell() }, // Should stay at index 1
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_AGEKEY}, 
                new DataGridViewTextBoxCell { Value = "" }},
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_GENDERKEY}, 
                new DataGridViewTextBoxCell { Value = "" }},
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_SESSIONKEY}, 
                new DataGridViewTextBoxCell { Value = "" }},
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_GROUPKEY},
                new DataGridViewTextBoxCell { Value = "" }},
            new DataGridViewCell[] { new DataGridViewTextBoxCell { Value = SessionIdentification.SESSION_EXPERIENCEKEY}, 
                new DataGridViewTextBoxCell { Value = "" }}
        };

        /// <summary>
        /// Reference to the SystemLogger.
        /// </summary>
        private readonly SystemLogger _sysLog = SystemLogger.SysLog;

        /// <summary>
        /// Data about the session (age of participant, ...).
        /// </summary>
        private readonly SessionIdentification _result;

        /// <summary>
        /// Reference to the AnalyzeTab.
        /// </summary>
        public Analyze.Analyze AnalyzeTab;

        /// <summary>
        /// Reference to the PlayTab.
        /// </summary>
        public Play PlayTab;

        /// <summary>
        /// The GUI of which this tab is part of.
        /// </summary>
        public Gui GUI;

        /// <summary>
        /// True if in recording state, false if not.
        /// If this value is set, the GUI is updated to resemble the new value of this property
        /// (the text of the record button is modified, the GUI is hidden, ...).
        /// This value is in direct contact with the Enabled property of the RecordPanel.
        /// When the state is recording, this panel should be disabled and when not recording,
        /// the panel should be enabled.
        /// </summary>
        private bool _thisRecording;
        private bool Recording
        {
            get => _thisRecording;
            set
            {
                if (value == _thisRecording || RecordStateChanged == null) return;
                _thisRecording = value;
                RecordStateChanged(this, new RecordStateChangedEventArgs(value));
            }
        }

        /// <summary>
        /// Data about the session (age of participant, ...).
        /// </summary>
        private SessionIdentification SessionID
        {
            get
            {
                foreach (DataGridViewRow row in RecordSessionIdentification.Rows)
                {
                    var key = row.Cells[0].Value;
                    var value = row.Cells[1].Value;
                    // Older files used 'Sex' instead of 'Gender'.
                    if (key == null || key.Equals("Sex")) continue;
                    // If key exists, remove it to prevent adding twice the same value
                    _result.GetSessionInfo().Remove(key.ToString());
                    // Do not add rows with empty key
                    _result.GetSessionInfo().Add(key.ToString(), value?.ToString() ?? "");
                }
                // No key logging outside the main Word document, when the 'WordLog Restricted' option is 'true'.
                _result.SetSessionLogging(InputLog.Core.Util.Settings.WinLogRestricted ?
                    "No keystroke logging outside the main Word document." : string.Empty);
                _result.SetLogVersion(Application.ProductVersion);

                return _result;
            }
        }

        /// <summary>
        /// The current output directory.
        /// </summary>
        private DirectoryInfo OutputDir { get; set; }

        /// <summary>
        /// Path to the output file.
        /// </summary>
        private string OutputPath { get; set; }

        /// <summary>
        /// Event that will be triggered whenever the recording is turned on
        /// (and it wasn't yet on) or of (when it was on).
        /// </summary>
        public event RecordStateChanged RecordStateChanged;

        private RecordSettings _settings;

        /// <summary>
        /// Checks whether this tab is logging restricted or not.
        /// </summary>
        private bool _isLoggingRestricted;
        #endregion

        /// <summary>
        /// Constructs the Record component.
        /// </summary>
        public Record()
        {
            InitializeComponent();

            LoadRecordSettings();
            Properties.Settings.Default.SettingsSaving += (o, e) => LoadRecordSettings();

            _result = new SessionIdentification();
            Recording = false;

            // session identification default fields
            foreach (var row in _sessionIDDefaults)
            {
                var gridrow = new DataGridViewRow();
                gridrow.Cells.AddRange(row);
                RecordSessionIdentification.Rows.Add(gridrow);
            }
            RecordSessionIdentification.Rows[0].Cells[0].ReadOnly = true; // Participant field should not be changed
            RecordSessionIdentification.Rows[1].Cells[0].ReadOnly = true;
            var combobox = (DataGridViewComboBoxCell)RecordSessionIdentification.Rows[1].Cells[1];

            combobox.FlatStyle = FlatStyle.Flat;
            combobox.Items.AddRange(LANGUAGES);
            combobox.Value = combobox.Items[DEFAULT_LANG];

            RecordStateChanged += OnRecordStateChanged;
            _isLoggingRestricted = InputLog.Core.Util.Settings.WinLogRestricted;
        }

        private void LoadRecordSettings()
        {
            _settings = new RecordSettings(this);
            RecordPluginOptions.Controls.Clear();
            foreach (int i in _settings.PluginSelection)
            {
                RecordPluginOptions.Controls.Add(_settings.Plugins[i].Options);
            }
        }

        /// <summary>
        /// Updates OutputPath and OutputDir according to the participant's name.
        /// </summary>
        /// <param name="name">The name of the participant.</param>
        /// <returns>True if the function succeeded, false if the user
        /// has canceled or an exception was thrown.</returns>
        private bool UpdateOutputPath(string name)
        {
            var success = true;
            Exception exc = null;

            try
            {
                name = PathSanitizer.Sanitize(name);
                var workspace = Properties.Settings.Default.Workspace;

                var userDir = new DirectoryInfo(Path.Combine(workspace, name));
                var x = GetDirectoryNumber(userDir);
                var date = DateTime.Now.ToString("yyyy-MM-dd");

                OutputDir = userDir.CreateSubdirectory(date + "_" + x);
                if (!OutputDir.Exists)
                {
                    OutputDir.Create();
                }

                OutputPath = Path.Combine(OutputDir.FullName, name + "_" + x + ".idfx"); // TODO extract

                if (File.Exists(OutputPath))
                {
                    //TODO extract string
                    var res = MessageBox.Show("The destination file '" + OutputPath
                        + "' already exists, do you want overwrite the file?",
                        "InputLog - Overwrite file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (DialogResult.Yes == res)
                    {
                        File.Delete(OutputPath);
                        File.Create(OutputPath).Close();
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    File.Create(OutputPath).Close();
                }
            }
            catch (Exception e)
            {
                success = false;
                exc = e;
            }

            if (!success)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to create output directory");
            }

            return success;
        }

        /// <summary>
        /// Scans the given directory for subdirectories and returns x such that x is
        /// one greater than the highest number that occurs after the '_' sign in the subdirectories.
        /// If the given directory does not exists, x will be 0 and the directory will be created.
        /// </summary>
        /// <param name="dir">The directory to scan.</param>
        /// <returns>
        /// The number that is one greater than the highest number that appears in the name of the
        /// subdirectories. (The numbers should appear after the '_' character before they are considered.)
        /// </returns>
        private static int GetDirectoryNumber(DirectoryInfo dir)
        {
            var x = 0;

            if (dir.Exists)
            {
                foreach (var subdir in dir.EnumerateDirectories())
                {
                    var name = subdir.Name;
                    var parts = name.Split('_');
                    if (parts.Length <= 1) continue;
                    int y;
                    if (int.TryParse(parts[parts.Length - 1], out y))
                    {
                        x = Math.Max(x, y + 1);
                    }
                }
            }
            else
            {
                dir.Create();
            }
            return x;
        }

        /// <summary>
        /// Check that a participant name is provided and that a valid output
        /// path can be configured
        /// </summary>
        /// <returns>true if the preconditions were all valid, false if not.</returns>
        private bool ValidPreconditionsForLogging()
        {
            var participant = SessionID.GetParticipant();
            if (string.IsNullOrEmpty(participant)) {
                MessageBox.Show("Please fill in the participant's name", "InputLog: Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //... update path before validating the plugins, they may need the path...
            if (!UpdateOutputPath(participant))
            {
                return false;
            }

            // No validation is actually performed at this time. 
            // Plugin validations always returns true
            return _settings.PluginSelection.Aggregate(true,
                (current, iPlug) => current && _settings.Plugins[iPlug].Validate());
        }

        /// <summary>
        /// Starts the logging. Assumes preconditions have been correctly set.
        /// </summary>
        private void StartLogging()
        {
            try
            {
                foreach (int iPlug in _settings.PluginSelection)
                {
                    _settings.Plugins[iPlug].Launch(OutputDir.FullName, SessionID);
                }
                SessionID.SetMainDocument(WordLog.MainDocTitle);
                _sysLog.Start(OutputPath, _settings.LoggingFormat, SessionID, _settings.HookKeyboard, _settings.HookMouse);
                Recording = true;
            }
            catch (Exception e)
            {
                // If an exception was thrown, just stop logging and throw on.
                StopLogging();

                // TODO if Word is already open, we get an exception here ...
                MessageBox.Show("Unable to start logging: " + e, "InputLog: Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stops the logging.
        /// </summary>
        private void StopLogging()
        {
            if (!Recording)
            {
                return;
            }
            var updateReplay = false;
            // First stop plugins, then stop SystemLogger
            foreach (int iPlug in _settings.PluginSelection)
            {
                _settings.Plugins[iPlug].Terminate();
                if (_settings.Plugins[iPlug] is WordLogFrontend)
                {
                    updateReplay = true;
                }
            }         
            _sysLog.Stop();
            Recording = false;

            // Update Replay and Analyze tab, switch to the latter
            var orgDocPath = "";
            if (updateReplay)
            {
                var wrdLog = _settings.Plugins.OfType<WordLogFrontend>().First();
                orgDocPath = wrdLog.OrigDocPath;
                PlayTab.SetReplaySources(OutputPath, orgDocPath);
            }
            AnalyzeTab.Activate(OutputPath, orgDocPath);
            GUI.OpenFile(InputlogDocument.LoggedDocPath);

            if (InputLog.Core.Util.Settings.Segmentation_KeyDelimiterActive)
            {
                var seg = new IdfxSegmenter(
                    InputLog.Core.Util.Settings.Segmentation_KeyDelimiter,
                    InputLog.Core.Util.Settings.Segmentation_IncludeInitialPause
                );
                int c = seg.Segment(OutputPath);
                MessageBox.Show("Output file was split into " + c + " segments", "Segmenting Finished",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Click-listener of the record-button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void RecordButtonClick(object sender, EventArgs e)
        {
            if (IsNormalLogging())
            {
                HandleNormalRecordingClick();
            }
            else
            {
                HandleAlternativeRecordingClick();
            }
        }

        /// <summary>
        /// Handle the clicking of the record button while 'alternative 
        /// logging' methods have been selected.
        /// </summary>
        private void HandleAlternativeRecordingClick()
        {
            if (AL_CopytaskRBTN.Checked)
            {
                System.Diagnostics.Process.Start(COPY_TASK_URL);
            }
        }

        ///// <summary>
        ///// Check if the Chinese SogouPinyin is installed as keyboard.
        ////  Only useful in the Chinese Inputlog version.
        ///// </summary>
        ///// <returns></returns>
        //private static bool RequirementsPresent()
        //{
        //    return RegistryTools.FoundSogouInputRegistryKey();
        //}

        /// <summary>
        /// Start / stop recording when we are in recording mode
        /// </summary>
        private void HandleNormalRecordingClick()
        {
            try
            {
                if (Recording)
                {
                    AllowInterfaceRecordingChanges(true);
                    StopLogging();
                }
                else
                {
                    if (ValidPreconditionsForLogging())
                    {
                        AllowInterfaceRecordingChanges(false);
                        StartLogging();
                    }
                }
            }
            catch (Exception exc)
            {
                AllowInterfaceRecordingChanges(true);
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Allows or blocks changes to the information about logging options
        /// and/or session information. If it is not allowed then these areas are disabled.
        /// </summary>
        /// <param name="allow">True if changes are allowed, false if not.</param>
        private void AllowInterfaceRecordingChanges(bool allow)
        {
            ALGroupbox.Enabled = allow;
            RecordPluginOptions.Enabled = allow;
            RecordSessionIdentification.Enabled = allow;
        }

        /// <summary>
        /// Callback for when the recording state has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRecordStateChanged(object sender, RecordStateChangedEventArgs e)
        {
            if (e.Recording)
            {
                RecordButton.Text = "Stop Recording";
                GUI.ContextMenuRecordItem.Text = "Stop Recording";
                if (Properties.Settings.Default.AutoHide)
                {
                    GUI.Hidden = true;
                }
            }
            else
            {
                RecordButton.Text = "Record";
                GUI.ContextMenuRecordItem.Text = "Record";
                if (Properties.Settings.Default.AutoHide)
                {
                    GUI.Hidden = false;
                }
            }
            RecordPanel.Enabled = !e.Recording;
        }

        /// <summary>
        /// Creates and returns a RecordSession object containing the meta information from the GUI.
        /// </summary>
        /// <returns>A RecordSession object containing the meta information from the GUI.</returns>
        public RecordSession GetSessionInfo()
        {
            var session = new RecordSession(SessionID.GetSessionInfo(), _settings.HookFocus, _settings.HookKeyboard, _settings.HookMouse,
                _settings.LoggingFormat, _settings.PluginSelection.Contains(0), _settings.PluginSelection.Contains(1));

            foreach (var plugin in _settings.Plugins)
            {
                plugin.Options.GetSessionInfo(session);
            }

            return session;
        }

        /// <summary>
        /// Modifies the properties (and the GUI) such that it reflects the metadata contained in the given RecordSession.
        /// </summary>
        public void SetSessionData(RecordSession session)
        {
            LoadSessionIDInformation(session.SessionID);

            _settings.HookFocus = session.HookFocus;
            _settings.HookKeyboard = session.HookKeyboard;
            _settings.HookMouse = session.HookMouse;
            _settings.LoggingFormat = session.LogFormat;
            if (session.WinLog) _settings.PluginSelection.Add(0); else _settings.PluginSelection.Remove(0);
            if (session.WordLog) _settings.PluginSelection.Add(1); else _settings.PluginSelection.Remove(1);

            foreach (var plugin in _settings.Plugins)
            {
                plugin.Options.SetSessionData(session);
            }
        }

        /// <summary>
        /// Load a dictionary of session information into the session datagrid.
        /// </summary>
        /// <param name="session">Dictionary containing the data to be 
        /// added to the session datagrid.</param>
        private void LoadSessionIDInformation(IDictionary<string, string> session)
        {
            foreach (var pair in session)
            {
                if (pair.Key.Equals("Restricted Logging") && pair.Value.Equals(""))
                {
                    continue;
                }

                bool foundKey = false;
                for (int i = 0; i < RecordSessionIdentification.Rows.Count && !foundKey; i++)
                {
                    if (RecordSessionIdentification.Rows[i].Cells[0].Value != null &&
                        RecordSessionIdentification.Rows[i].Cells[0].Value.Equals(pair.Key))
                    {
                        RecordSessionIdentification.Rows[i].Cells[1].Value = pair.Value;
                        foundKey = true;
                    }
                }

                if (!foundKey) // We looped over all the rows and still didn't find the right key => add a new row
                {
                    RecordSessionIdentification.Rows.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Depending on whether WinLogRestricted option has been activated or not (currently
        /// in the options > logging window) this will add the WinLogRestricted entry or remove
        /// it from the session information that is displayed in the Record tab.
        /// </summary>
        internal void UpdateWinLogRestricted()
        {
            bool currentRestrictionStatus = InputLog.Core.Util.Settings.WinLogRestricted;
            if (_isLoggingRestricted != currentRestrictionStatus)
            {
                _isLoggingRestricted = currentRestrictionStatus;
                // Session ID automatically updates the WinLogRestricted info.
                SessionIdentification newSession = SessionID;
                LoadSessionIDInformation(newSession.GetSessionInfo());

                // When logging is not currently restricted, make sure that if the session ID 
                // still shows the restricted information, it is removed from the session id datagrid.
                if (!_isLoggingRestricted)
                {
                    RemoveRestrictedLogging();
                }
            }

        }

        /// <summary>
        /// Removes 'Restricted Logging' from the Session Identification table
        /// when the user unselected 'WordLog Restricted' in the File Options. 
        /// </summary>
        private void RemoveRestrictedLogging()
        {
            foreach (DataGridViewRow row in RecordSessionIdentification.Rows)
            {
                var key = row.Cells[0].Value;
                if (key == null || !key.Equals("Restricted Logging")) continue;
                if (!row.IsNewRow)
                    RecordSessionIdentification.Rows.Remove(row);
            }
        }

        private void AL_NoneRBTN_CheckedChanged(object sender, EventArgs e)
        {
            HandleAlternativeLoggingSelectionChange(); 

        }

        private void AL_CopytaskRBTN_CheckedChanged(object sender, EventArgs e)
        {
            HandleAlternativeLoggingSelectionChange();
        }

        /// <summary>
        /// If an alternative logging method other than 'None' has been selected
        /// we disable the other Documents' options, and the SessionInformation 
        /// table.
        /// </summary>
        private void HandleAlternativeLoggingSelectionChange()
        {
            NormalLogging_ToggleEnabled(AL_NoneRBTN.Checked);
        }

        /// <summary>
        /// GUI elements that should be enabled for normal logging and disabled ofr
        /// 'alternative logging' such as copyTask logging.
        /// </summary>
        /// <param name="enabled"></param>
        private void NormalLogging_ToggleEnabled(bool enabled)
        {
            RecordPluginOptions.Enabled = enabled;
            RecordSessionIdentification.Enabled = enabled;
        }

        /// <summary>
        /// Returns true if we are currently in normal logging mode,
        /// false if we are in alternative logging modes.
        /// </summary>
        /// <returns></returns>
        private bool IsNormalLogging()
        {
            return AL_NoneRBTN.Checked;
        }
    }

    /// <summary>
    /// Class that contains the event arguments passed whenever the recording
    /// state has changed.
    /// </summary>
    public class RecordStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recording">True if Inputlog is currently logging, false if not.</param>
        public RecordStateChangedEventArgs(bool recording)
        {
            Recording = recording;
        }

        /// <summary>
        /// Returns true if Inputlog is currently logging the input, false if not.
        /// </summary>
        public bool Recording { get; }
    }

    /// <summary>
    /// A delegate so listeners can register themselves for events whenever the recording state
    /// has changed (from not recording to recording and vice versa).
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void RecordStateChanged(object sender, RecordStateChangedEventArgs e);
}