using System;
using System.IO;
using System.Windows.Forms;
using GUI.Session;
using InputLog.Core.Util;
using CoreSettings = InputLog.Core.Util.Settings;

namespace GUI.Tabs.Record.Plugin.WordLog
{
    /// <summary>
    /// Options corresponding to the WordLog plugin
    /// </summary>
    public partial class WordLogOptions : PluginOptions
    {
        #region Fields
        /// <summary>
        /// Path the the previously logged document. (The document that was logged during the previous session.)
        /// </summary>
        public string PreviousLoggedDocument
        {
            private get { return _previousDoc; }
            set
            {
                PreviousDocTextBox.Text = value;
                PreviousDocTextBox.Select(PreviousDocTextBox.Text.Length, 0);
                if (!PreviousDocTextBox.Text.IsNullOrEmpty())
                    _previousDoc = PreviousDocTextBox.Text;
            }
        }

        /// <summary>
        /// If the user chooses a document to be opened, this property returns the path of it.
        /// If the user chooses to create a new document, this property will return null.
        /// </summary>
        public string SelectedDocument
        {
            get
            {
                if (WordNewDoc.Checked)
                {
                    return null;
                }
                if (OpenDocBtn.Checked)
                {
                    if (_fileField.Length > 1) ExistingDocTextField.Text = _fileField;
                    ExistingDocTextField.Select(ExistingDocTextField.Text.Length, 0);
                    return ExistingDocTextField.Text;
                }
                return PreviousDocBtn.Checked ? PreviousDocTextBox.Text : null;
            }
        }

        /// <summary>
        /// Returns true if the selected document should be updated, false if not.
        /// (The selected document should be updated if it is either a new one or the option is enabled in the settings.)
        /// </summary>
        public bool UpdateDocument => WordNewDoc.Checked || CoreSettings.WordLogOverrideDoc;

        /// <summary>
        /// The file that is selected to work on.
        /// </summary>
       private static string _fileField;

       private static string _previousDoc;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public WordLogOptions()
        {
            InitializeComponent();
            _fileField = string.Empty;
            ExistingDocTextField.Text = Path.Combine(Properties.Settings.Default.Workspace, "WordLog.docx");
            ExistingDocTextField.Select(ExistingDocTextField.Text.Length, 0);
            PreviousDocTextBox.Text = PreviousLoggedDocument;
            PreviousDocTextBox.Select(PreviousDocTextBox.Text.Length, 0);
            // Update the interface to resemble the selected choice.
            OpenDocBtn.Checked = false;
            PreviousDocBtn.Checked = false;
            ExistingDocTextField.Enabled = false;
            PreviousDocTextBox.Enabled = false;

            // Subscribe to Path Change Handler
            Gui.PathChangeHandler += ChangePath;
        }

        /// <summary>
        /// Callback from the Gui with the latest document path.
        /// </summary>
        /// <param name="sender">Gui</param>
        /// <param name="e">path to the most recent selected document</param>
        private static void ChangePath(Gui sender, Gui.PathChanged e)
        {
            _fileField = e.ThisPath;
        }

        /// <summary>
        /// Callback for when the Browse-button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Empty.</param>
        private void ExistingDocBrowseButtonClick(object sender, EventArgs e)
        {
            try
            {
                OpenDocBtn.Checked = true;
                if (_fileField.Length > 1) ExistingDocTextField.Text = _fileField;
                WordDocDialog.InitialDirectory = Path.GetDirectoryName(ExistingDocTextField.Text);
                WordDocDialog.FileName = Path.GetFileName(ExistingDocTextField.Text);
                if (WordDocDialog.ShowDialog() == DialogResult.OK)
                {
                    _fileField = WordDocDialog.FileName;
                    ExistingDocTextField.Text = _fileField;
                    ExistingDocTextField.Select(ExistingDocTextField.Text.Length, 0);
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Modifies the properties (and the GUI) so that it reflects the metadata contained in the given RecordSession.
        /// Subclasses should override this method if the empty implementation is not wanted.
        /// <param name="session">The session data.</param>
        /// </summary>
        public override void SetSessionData(RecordSession session)
        {
            PreviousLoggedDocument = session.WordLogDoc;
            PreviousDocTextBox.Select(PreviousDocTextBox.Text.Length, 0);
        }

        /// <summary>
        /// Adds the session information currently stated in the GUI to the given RecordSession.
        /// </summary>
        /// <param name="session">The object in which the session data should be saved.</param>
        public override void GetSessionInfo(RecordSession session)
        {
            if (_fileField == "")
            {
                _fileField = PreviousLoggedDocument;
            }
            session.WordLogDoc = _fileField;
        }

        private void OpenDocBtnCheckedChanged(object sender, EventArgs e)
        {
            PreviousDocTextBox.Enabled = false;
            ExistingDocTextField.Enabled = true;
            if (_fileField.Length > 1) ExistingDocTextField.Text = _fileField;
            ExistingDocTextField.Select(ExistingDocTextField.Text.Length, 0);
        }

        private void PreviousDocBtnCheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PreviousDocTextBox.Text))
            {
                ExistingDocTextField.Enabled = false;
                PreviousDocTextBox.Enabled = true;
            }
            else
            {
                MessageBox.Show("No previous document available");
            }
        }

        private void WordNewDocCheckedChanged(object sender, EventArgs e)
        {
            PreviousDocTextBox.Enabled = false;      
            ExistingDocTextField.Enabled = false;
        }
    }
}