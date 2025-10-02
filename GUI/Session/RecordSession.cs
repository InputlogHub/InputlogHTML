using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GUI.Session
{
    /// <summary>
    /// Class that contains the meta-data about a record/logging session.
    /// This data includes the SessionID-table, the selected plugins, ...
    /// </summary>
    [Serializable]
    public class RecordSession
    {
        #region Fields
        /// <summary>
        /// Contains the session identification data (participant's name, age, ...).
        /// </summary>
        public IDictionary<string, string> SessionID { get; private set; }

        /// <summary>
        /// True iff focus event logging is enabled.
        /// </summary>
        public bool HookFocus { get; private set; }

        /// <summary>
        /// True iff keyboard event logging is enabled.
        /// </summary>
        public bool HookKeyboard { get; private set; }

        /// <summary>
        /// True iff mouse event logging is enabled.
        /// </summary>
        public bool HookMouse { get; private set; }

        /// <summary>
        /// Format of the output file.
        /// </summary>
        public string LogFormat { get; private set; }

        /// <summary>
        /// True iff WinLog is activated.
        /// </summary>
        public bool WinLog { get; private set; }

        /// <summary>
        /// True iff WordLog is activated.
        /// </summary>
        public bool WordLog { get; private set; }

        /// <summary>
        /// The path to the logged document of WordLog (if WordLog is activated).
        /// </summary>
        public string WordLogDoc { get; set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sessionID">The session's metadata (name of the participant, ...).</param>
        /// <param name="hookFocus">True iff focus event logging is enabled.</param>
        /// <param name="hookKeyboard">True iff keyboard event logging is enabled.</param>
        /// <param name="hookMouse">True iff mouse event logging is enabled.</param>
        /// <param name="logformat">Format of the output file.</param>
        /// <param name="winlog">True iff WinLog is activated.</param>
        /// <param name="wordlog">True iff WordLog is activated.</param>
        public RecordSession(IDictionary<string, string> sessionID, bool hookFocus, bool hookKeyboard, bool hookMouse,
                             string logformat, bool winlog, bool wordlog)
        {
            SessionID = new Dictionary<string, string>(sessionID);
            HookFocus = hookFocus;
            HookKeyboard = hookKeyboard;
            HookMouse = hookMouse;
            LogFormat = logformat;
            WinLog = winlog;
            WordLog = wordlog;
        }

        /// <summary>
        /// Tries to read a RecordSession object from the file located on the given path.
        /// If no such file exists or the file is corrupted, null will be returned.F
        /// </summary>
        /// <param name="path">The path to the file where the RecordSession should be deserialized from.</param>
        /// <returns>Null if the file could not be read/is corrupt, 
        /// the deserialized RecordSession if it succeeded.</returns>
        public static RecordSession OpenSession(string path)
        {
            RecordSession session = null;
            var info = new FileInfo(path);

            if (info.Exists)
            {
                Stream stream = info.Open(FileMode.Open);
                try
                {
                    var bFormatter = new BinaryFormatter();
                    session = (RecordSession)bFormatter.Deserialize(stream);
                }
                catch (Exception)
                {
                    // Fail silently, just start with an empty session
                }
                finally
                {
                    stream.Close();
                }
            }

            return session;
        }

        /// <summary>
        /// Saves the instance to the given file and hides the file.
        /// </summary>
        /// <param name="path">The path of the file where to save the session data to.</param>
        public void Save(string path)
        {
            if (File.Exists(path))
            {
                // first check if sessionstate file exists (File.Delete throws an exception if the file does not exist).
                File.Delete(path);
            }
            // create the directory if it does not exist yet
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Stream stream = File.Open(path, FileMode.Create);
            try
            {
                var bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, this);
            }
            finally
            {
                stream.Close();
                File.SetAttributes(path, FileAttributes.Hidden);
            }
        }
    }
}