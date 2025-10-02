using System;
using System.IO;
using System.Windows.Forms;
using GUI.Session;
using GUI.Tabs.Record.Plugin.WordLog;
using InputLog.Core;
using InputLog.Core.IO;
using System.Collections.Generic;
using InputLog.Core.Plugin.WordLog;

namespace GUI.Tabs.Record
{
    public class Recorder
    {
        /// <summary>
        /// Reference to the SystemLogger.
        /// </summary>
        private readonly SystemLogger SysLog = SystemLogger.SysLog;

        /// <summary>
        /// Data about the session (age of participant, ...).
        /// </summary>
        private readonly SessionIdentification Result;

        /// <summary>
        /// True if in recording state, false if not.
        /// If this value is set, the GUI is update to resemble the new value of this property
        /// (the text of the record button is modified, the GUI is hidden, ...).
        /// This value is in direct contact with the Enabled property of the RecordPanel as
        /// when the state is recording, this panel should be disabled and when not recording,
        /// the panel should be enabled.
        /// </summary>
        private bool ThisRecording;
        public bool Recording
        {
            get { return ThisRecording; }
            private set
            {
                if (value != ThisRecording)
                    ThisRecording = value;
                if (RecordStateChanged != null)
                    RecordStateChanged(this, new RecordStateChangedEventArgs(value));
            }
        }

        /// <summary>
        /// Data about the session (age of participant, ...).
        /// </summary>
        public SessionIdentification SessionID
        {
            get
            {
                Result.SetLogVersion(Application.ProductVersion);
                Result.SetSessionLogging(InputLog.Core.Util.Settings.WinLogRestricted
                                 ? "Inputlog did not log key strokes outside the main Word document." : "");
                return Result;
            }
        }

        /// <summary>
        /// Event that will be triggered whenever the recording is turned on (and it wasn't yet
        /// on) or of (when it was on).
        /// </summary>
        public event RecordStateChangedEvent RecordStateChanged;

        private readonly RecordSettings Settings;

        public bool UpdateReplay;

        public Recorder(Record rec)
        {
            Settings = new RecordSettings(rec);

            Result = new SessionIdentification();
            Recording = false;
            UpdateReplay = false;
        }

        public Recorder(RecordSettings settings)
        {
            Settings = settings;

            Result = new SessionIdentification();
            Recording = false;
            UpdateReplay = false;
        }

        /// <summary>
        /// Starts the logging.
        /// </summary>
        public void StartLogging(string outputPath, string idfxpath=null)
        {
            Recording = true;
            if (idfxpath == null)
                idfxpath = outputPath;
            foreach (int iPlug in Settings.PluginSelection)
            {
                Settings.Plugins[iPlug].Launch(Path.GetDirectoryName(outputPath), SessionID);
            }
            SessionID.SetMainDocument(WordLog.MainDocTitle);
            SysLog.Start(idfxpath, Settings.LoggingFormat, SessionID, Settings.HookKeyboard, Settings.HookMouse);
        }

        /// <summary>
        /// Stops the logging.
        /// </summary>
        public void StopLogging()
        {
            UpdateReplay = false;
            // First stop plugins, then stop SystemLogger
            foreach (int iPlug in Settings.PluginSelection)
            {
                Settings.Plugins[iPlug].Terminate();
                if (Settings.Plugins[iPlug] is WordLogFrontend)
                {
                    UpdateReplay = true;
                }
            }
            SysLog.Stop();
            Recording = false;
        }

        /// <summary>
        /// Creates and returns a RecordSession object containing the meta information of the GUI.
        /// </summary>
        /// <returns>A RecordSession object containing the meta information of the GUI.</returns>
        public RecordSession GetSessionInfo()
        {
            Dictionary<string, string> sessionInfo = new Dictionary<string, string>();
            var session = new RecordSession(sessionInfo, Settings.HookFocus, Settings.HookKeyboard, Settings.HookMouse,
                Settings.LoggingFormat, Settings.PluginSelection.Contains(0), Settings.PluginSelection.Contains(1));

            foreach (var plugin in Settings.Plugins)
            {
                plugin.Options.GetSessionInfo(session);
            }

            SessionID.AddSessionInfo(sessionInfo);
            return session;
        }

        /// <summary>
        /// Class that contains the event arguments passed whenever the recording
        /// state is chagned.
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
            /// Returns true if the Inputlog is currently logging the input, false if not.
            /// </summary>
            private bool Recording { get; set; }
        }

        /// <summary>
        /// A delegate so listeners can register themselves for events whenever the recording state
        /// is changed (from not recording to recording and vice versa).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void RecordStateChangedEvent(object sender, RecordStateChangedEventArgs e);

        public bool ValidatePlugins()
        {
            bool valid = true;
            foreach (int iPlug in Settings.PluginSelection)
            {
                valid = valid && Settings.Plugins[iPlug].Validate();
            }
            return valid;
        }
    }
}
