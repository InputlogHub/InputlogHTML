using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.AnalysesControls;
using GUI.Tabs.Preprocess;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.ImportExport
{
    /// <summary>
    /// Dialog that allows the user to export a configuration of the analysis tab.
    /// This includes all analyses, filters, sources and destination paths that have been specified.
    /// </summary>
    public partial class ExportAnalysisConfiguration : Form
    {
        #region Fields

        /// <summary>
        /// List of analysisControls that needs to be exported.
        /// </summary>
        private readonly List<AnalysisControl> _analysisControls;

        /// <summary>
        /// List of filterControls that needs to be exported.
        /// </summary>
        private readonly List<ProcessControl> _filterControls;

        ///// <summary>
        ///// List of source files that needs to be exported.
        ///// </summary>
        private readonly List<String> _sources;

        /// <summary>
        /// The path to the Export folder.
        /// </summary>
        private String _exportFilePath;

        private readonly String _exportDir;

        #endregion

        /// <summary>
        /// Default ctor (needed for the C# Form designer).
        /// </summary>
        public ExportAnalysisConfiguration()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs an ExportAnalyisConfiguration Dialog.
        /// </summary>
        /// <param name="analysisControls">Analysis controls that need to be exported.</param>
        /// <param name="filterControls">Filter controls that need to be exported.</param>
        /// <param name="sources">Input Sources that need to be exported.</param>
        /// <param name="destination">Destination that needs to be exported.</param>
        public ExportAnalysisConfiguration(List<AnalysisControl> analysisControls,
            List<ProcessControl> filterControls, List<String> sources, String destination)
        {
            InitializeComponent();
            StatusLabel.Text = String.Empty;
            _analysisControls = analysisControls;
            _filterControls = filterControls;
            _sources = sources;

            if (destination.Length > 1)
            {
                _exportDir = destination;
                DestinationFolderTB.Text = destination;
                DestinationFolderTB.SelectionStart = DestinationFolderTB.Text.Length;
                DestinationFolderTB.ScrollToCaret();
            }
        }

        /// <summary>
        /// Click listener for the ExportButton button.
        /// Exports the current configuration of the analyze tab taking into account the export 
        /// settings that were specified in this dialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            // Validates that the destination field is a valid file.
            if (String.IsNullOrWhiteSpace(DestinationFolderTB.Text) || String.IsNullOrWhiteSpace(DestinationFileTB.Text))
            {
                MessageBox.Show("Please provide a name\nfor the destination file.", "Invalid destination file",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            _exportFilePath = Path.Combine(_exportDir, DestinationFileTB.Text + ".iafx");

            if (File.Exists(_exportFilePath))
            {
                DialogResult result = MessageBox.Show(
                    "Destination file already exists. Do you want to overwrite it?",
                    "Do you want to overwrite the destination file?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (DialogResult.No.Equals(result)) return;
            }

            if (!Directory.Exists(_exportDir))
                Directory.CreateDirectory(Path.GetDirectoryName(_exportDir));

            Enabled = false;
            StatusLabel.Text = @"Exporting configuation files";

            // Starts actual export logic.
            var exportOptions = new ExportOptions();
            var filterOptions = new FilterExportOptions();
            exportOptions.IncludeDestination = SaveDstCheckbox.Checked;

            var serializer = new AnalysisConfigurationXMLSerializer(_exportFilePath);
            try
            {
                // Checks first if a Time Filter is not linked to multiple sources. This is not allowed, because the parameters
                // of the filter are derived from a specific source and cannot be applied to other files.
                if (_filterControls.Any(control => control.Name.Equals("Time")) 
                    && (SaveSrcCheckbox.Checked && _sources.Count > 1)
                    && (SaveFiltersCheckbox.Checked = true))
                {
                    MessageBox.Show("It is not possible to link a Time Filter to multiple sources." +
                                    "\nEither remove the Time Filter, or save the one source \nthat goes with this filter. ",
                                    "Time Filter with Multiple Sources", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check on the presence of a source file when saving the Time filter
                if (_filterControls.Any(control => control.Name.Equals("Time")) 
                    && (SaveSrcCheckbox.Checked == false) 
                    && (SaveFiltersCheckbox.Checked = true))
                {
                    MessageBox.Show("You're about to save Time Filter without a source." +
                                    "\nEither remove the Time Filter,\nor check the 'Include source file paths'-box",
                                    "Time Filter without Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                serializer.WriteHeader();

                // Exporting analysis control configurations.
                if (SaveAnalysisCheckBox.Checked && _analysisControls.Count > 0)
                    serializer.SerializeAnalysisConfiguration(_analysisControls, exportOptions);

                // Exporting filter configurations.
                if (SaveFiltersCheckbox.Checked && _filterControls.Count > 0)
                    serializer.SerializeFilterConfiguration(_filterControls, filterOptions);

                // Exporting source paths.
                if (SaveSrcCheckbox.Checked && _sources.Count > 0)
                    serializer.SerializeSource(_sources);

                // Exporting destination.
                if (SaveDstCheckbox.Checked)
                    serializer.SerializeDestination(_exportDir);

                // Finalize export
                serializer.WriteFooter();

                StatusLabel.Text = @"Done";
                MessageBox.Show("The configuration was successfully exported.", "Export successful",
                                MessageBoxButtons.OK);
                Close();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
                StatusLabel.Text = @"An error occured during export.";
            }
            finally
            {
                serializer.Dispose();
                Enabled = true;
            }
        }

        /// <summary>
        /// Click event for the "Browse" button with the destination folder text field.
        /// Shows a FolderSelectionDialog.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (destinationFolderDialog.ShowDialog() == DialogResult.OK)
                DestinationFolderTB.Text = destinationFolderDialog.SelectedPath;
        }
    }
}