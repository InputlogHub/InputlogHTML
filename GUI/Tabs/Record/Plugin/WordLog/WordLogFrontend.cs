
using System;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.Util;
using WLog = InputLog.Core.Plugin.WordLog.WordLog;

namespace GUI.Tabs.Record.Plugin.WordLog
{
    /// <summary>
    /// InputLog plugin that captures extra information from word.
    /// </summary>
    public class WordLogFrontend : AbstractPlugin
    {
        #region Fields
        /// <summary>
        /// Reference to the core WordLog plugin used for the actual logging.
        /// </summary>
        private readonly WLog _backend = new WLog();

        /// <summary>
        /// Path where the original version of the opened document is saved
        /// or where an empty file was created when a new document was created
        /// during the last logging session.
        /// </summary>
        public string OrigDocPath => _backend.OriginalDocPath;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings"></param>
        public WordLogFrontend(RecordSettings settings) 
            : base(new WordLogOptions(), settings) { }

        /// <summary>
        /// Validates the configuration entered in the GUI, returns true if everything is OK, false if not.
        /// </summary>
        /// <returns>True if the entered configuration is OK, false if there are conflicts, invalid information, ...
        /// and the plugin will not be able to start/function properly.</returns>
        public override bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Launches the WordLog plugin.
        /// </summary>
        protected override void InternalLaunch()
        {
            var options = (WordLogOptions)Options;

            // After logging the selected doc, it will be the previous logged document
            var doc = string.IsNullOrWhiteSpace(options.SelectedDocument) ? null : PathSanitizer.Sanitize(options.SelectedDocument);
            options.PreviousLoggedDocument = doc;
           
            // Push configuration to backend
            _backend.Template = doc;

            // If no template name was given, the document name is a general specifier.
            var workdocA = string.IsNullOrWhiteSpace(doc) ? "WordLog.docx" : Path.GetFileName(doc);

            // Second part of the document name with the identification of the user and the production date.       
            string workdocB;
            var participant = SessionID.GetParticipant();
            var fileExt = Path.GetExtension(workdocA);
            var dateString = DateTime.Now.ToString("s");

            // We do not add the same participant twice, only change the timestamp.
            if (workdocA.Contains(participant))
            {   
                var pIdx = workdocA.IndexOf(participant, StringComparison.CurrentCulture) + participant.Length + 1;
                var subA = workdocA.Substring(0, pIdx);
                workdocB = subA + PathSanitizer.KeepDigits(dateString) + fileExt;
            }
            else
            {
                workdocB = Path.GetFileNameWithoutExtension(workdocA)
                    + "_" + participant
                    + "_" + PathSanitizer.KeepDigits(dateString)
                    + fileExt;
            }

            // Full path
            _backend.WorkPath = PathSanitizer.Sanitize(Path.Combine(OutputDir, Path.GetFileName(workdocB)));
            _backend.UpdateTemplate = options.UpdateDocument;
            options.PreviousLoggedDocument = _backend.WorkPath;

            // Then start the plugin
            _backend.Running = true;
        }

        /// <summary>
        /// Terminates the WordLog plugin.
        /// </summary>
        protected override void InternalTerminate()
        {
            if (_backend.LoggedDocHasUnsavedChanges)
            {
                if (MessageBox.Show("Your document was saved automatically.\n" +
                                    "Do you want to save an additional copy?",
                    "Inputlog Additional Save Option", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,  MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    _backend.Save();
                }
            }
            _backend.Running = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources
        /// should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _backend.Dispose();
            }
        }
    }
}