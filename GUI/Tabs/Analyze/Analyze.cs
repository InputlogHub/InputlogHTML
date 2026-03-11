using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.AnalysesControls;
using GUI.Tabs.Analyze.AnalysesControls.Pause;
using GUI.Tabs.Analyze.ImportExport;
using GUI.Tabs.Preprocess;
using GUI.Tools.Reporting;
using InputLog.Core.Reporting.Report;
using InputLog.Core.Reporting.ReportTemplate;
using InputLog.Core.Util;
using InputLog.Core.Util.Progress;
using ReportingFormat = InputLog.Core.Reporting.Output;

namespace GUI.Tabs.Analyze
{
    /// <summary>
    ///     Analyze tab and its components.
    /// </summary>
    public partial class Analyze : GuiPathChangedEvent // UserControl
    {
        private const string LOAD_TEMPLATE_FROM_FILE = "<load template from file>";
        private const string OPEN_TEMPLATE_EDITOR = "<open template editor>";
		
        public Analyze()
        {
            IsDebugging();
            InitializeComponent();

            ProgressLabel.Text = "";
            NumberSelectedSourceFilesLabel.Text = "";
            AnalysesList.Items.AddRange(AnalysesNameToControl.Keys.ToArray());
            AnalysesRadioBttn_CheckedChanged(this, null);

            _preprocessorControls = new List<ProcessControl>();
            _model = new AnalyzeModel();
            _worker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _worker.DoWork += RunAnalyses;

            var formats = ReportingFormat.FormatterFactory.FORMATS;
            foreach (var availableFormat in formats)
                ReportOutputCList.Items.Add(availableFormat.ToString());
            ReportOutputCList.CheckBoxItems.Single(
                format => format.Text == ReportingFormat.FormatterFactory.Format.PDF.ToString()).Checked = true;

            ReportTemplateManager.TemplateInstance.TemplateListUpdated += TemplateListUpdatedHandler;

            RefreshForm();
        }

        /// <summary>
        /// When debugging we skip an EDU/LIFT report test. This test redirects the user to
        /// a dedicated EDU/LIFT version of Inputlog, but in debugging mode we want to be 
        /// able to check all the reporting templates, including those for EDU/LIFT>>.
        /// </summary>
        private static bool _isDebug;
        [Conditional("DEBUG")]
        private static void IsDebugging()
        {
            _isDebug = true;
        }
        private bool InReportGenerationMode => ReportRadioBttn.Checked;

        /// <summary>
        ///     Resets and activates the analysisPanel.
        /// </summary>
        public void Activate(string srcPaths = "", string origDocPaths = "")
        {
            // Modify this when the order of the tabs changes!
            Gui.Tabs.SelectedIndex = 2;
            _model.SourcePaths = StringUtils.SplitPaths(srcPaths);
            _model.OriginalDocumentPaths = StringUtils.SplitPaths(origDocPaths);
            ClearAnalysesButtonClick(this, null);
            RefreshForm();
        }

        /// <summary>
        ///     Refreshes the form based on the values stored in the model.
        /// </summary>
        private void RefreshForm()
        {
            SrcFileTextField.Text = StringUtils.JoinPaths(_model.SourcePaths);
            DstFileTextField.Text = _model.DestinationPath;

            OrgDocTextField.Text = _model.OriginalDocumentPaths.Any(doc => doc != null)
                ? StringUtils.JoinPaths(_model.OriginalDocumentPaths)
                : "";

            // Scroll each text field to the right, showing the end of each path.
            foreach (var field in new[] {SrcFileTextField, DstFileTextField, OrgDocTextField})
                field.Select(field.Text.Length, 0);

            _countedFiles = _model.SourcePaths.Count;

            NumberSelectedSourceFilesLabel.Text = _model.SourcePaths.Any()
                ? $"{_countedFiles} source file{(_countedFiles > 1 ? "s" : "")} specified"
                : "";

            // When starting Inputlog directly from an idfx-file the Analyze window is activated
            // before the Gui is built, resulting in a NullPointer exception on Gui.MyRecentFiles.
            if (null == Gui.MyRecentFiles)
                return;

            foreach (var sourcePath in _model.SourcePaths)
                Gui.MyRecentFiles.AddFile(sourcePath);
        }

        /// <summary>
        ///     The number of files to analyze as selected by the user.
        /// </summary>
        /// <returns>int</returns>
        public static int GetSourceCount()
        {
            return _countedFiles;
        }

        /// <summary>
        ///     Checks if at least one source file and a destination are given.
        /// </summary>
        private bool ValidatePaths()
        {
            if (!_model.SourcePaths.Any() || _model.SourcePaths.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Please provide at least one source file.", "Invalid source file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!_model.SourcePaths.All(File.Exists))
            {
                MessageBox.Show("Not all selected source files exist.", "Invalid source file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_model.DestinationPath))
            {
                MessageBox.Show("Please provide a destination directory.", "Invalid destination directory",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Add new analyses to the panel of selected analyses. The analyses to be added
        ///     are the ones that have been selected in the AnalysesList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButtonClick(object sender, EventArgs e)
        {
            foreach (var checkbox in AnalysesList.CheckBoxItems.Where(c => c.Checked))
            {
                var analyzerName = checkbox.Text;
                AddAnalysisToActiveAnalyses(analyzerName, RemoveAnalysisWrapper);
                checkbox.Checked = false;
            }
        }

        /// <summary>
        ///     Adds a new analysis with its control to the list of currently active analyses.
        ///     The analysis is added in a wrapper that controls some layout functionality.
        /// </summary>
        /// <param name="analyzerName">Name of the analyzer linked to the analysis.</param>
        /// <param name="removeAnalysisEventHandler">
        ///     The event handler that handles that gets called
        ///     when the user tries to remove the analysis from the list of active analyses.
        /// </param>
        private void AddAnalysisToActiveAnalyses(string analyzerName, EventHandler removeAnalysisEventHandler)
        {
            var analysisControl = (AnalysisControl) Activator.CreateInstance(AnalysesNameToControl[analyzerName]);
            var wrapper = new AnalysisWrapper(SelectedAnalysesPanel, analysisControl, removeAnalysisEventHandler);

            wrapper.SuspendLayout();
            SelectedAnalysesPanel.Controls.Add(wrapper);
            SelectedAnalysesPanel.ScrollControlIntoView(wrapper);
            wrapper.ResumeLayout();
        }

        /// <summary>
        ///     EventHandler that gets called when the user wishes to remove an analysis from
        ///     the list of active analyses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void RemoveAnalysisWrapper(object sender, EventArgs eventArgs)
        {
            var wrapper = (AnalysisWrapper) sender;
            wrapper.Control.Close();
            SelectedAnalysesPanel.Controls.Remove(wrapper);
            SelectedAnalysesPanel.PerformLayout();
            wrapper.Dispose();
        }

        private void ClearAnalysesButtonClick(object sender, EventArgs e)
        {
            ProgressLabel.Text = "";
            ProgressBar.Value = 0;

            foreach (var wrapper in SelectedAnalysesPanel.Controls.OfType<AnalysisWrapper>().ToList())
            {
                var activeAnalysis = wrapper.Control;
                activeAnalysis.Close();
                SelectedAnalysesPanel.Controls.Remove(wrapper);
            }

            if (InReportGenerationMode)
            {
                // TODO Unload template
            }
        }

        private void UpdateProgressBar(object sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) delegate { UpdateProgressBarThreadSafe(e); }, e);
            else
                UpdateProgressBarThreadSafe(e);
        }

        private void UpdateProgressBarThreadSafe(ProgressEventArgs e)
        {
            Application.EnableVisualStyles();
            ProgressLabel.Text = e.Message;
            ProgressLabel.Refresh();
            if (e.Code != ProgressEventArgs.ProgressCode.STARTED)
                ProgressBar.PerformStep();
        }

        private void AnalyzeButtonClick(object sender, MouseEventArgs e)
        {
            // Report generation can make the pause analysis run multiple pause analyses
            // behind the screens. This is done by a setting a static flag in the PauseAnalyzer.
            // We should never do this for a regular analysis, so explicitely set the flag to 
            // false here.
            PauseAnalyzer.RunMultiple = false;

            // Stops the analysis from running multiple times on double and tripple clicks.
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void RunAnalyses(object sender, DoWorkEventArgs args)
        {
            if (!ValidatePaths()) return;
            _model.IsReportGeneration = false;

            var analysisControls = SelectedAnalysesPanel.Controls.
                OfType<AnalysisWrapper>().Select(w => w.Control).ToList();

            if (analysisControls.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one analysis to execute.",
                    "No analyses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (!analysisControls.All(c => c.CheckPreconditions())) return;

            AllowInterfaceChangesToAnalyses(false);
            ResetProgressBar(analysisControls.Count);

            // Execute all selected analyses.
            foreach (var control in analysisControls)
            {
                var currentAnalysisName = control.AnalysisName;
                UpdateProgressBar(this, new ProgressEventArgs($"{currentAnalysisName}: {"Started"}",
                    ProgressEventArgs.ProgressCode.STARTED
                ));

                try
                {
                    control.Analyze(_model);
                    UpdateProgressBar(this,
                        new ProgressEventArgs($"{currentAnalysisName}: {"Done"}",
                            ProgressEventArgs.ProgressCode.DONE
                        ));
                }
                catch (Exception exc)
                {
                    UpdateProgressBar(this, new ProgressEventArgs(exc.Message,
                        ProgressEventArgs.ProgressCode.FAILED));
                    MessageLogger.CatchException(this, exc, Severity.ERROR);
                }
            }

            AllowInterfaceChangesToAnalyses(true);
        }

        #region Fields

        /// <summary>
        ///     The GUI of which this tab is part of.
        /// </summary>
        public Gui Gui;

        /// <summary>
        ///     The event filter controls that have been specified in the Filter dialog.
        /// </summary>
        private List<ProcessControl> _preprocessorControls;

        /// <summary>
        ///     Business logic for the Analyze tab.
        /// </summary>
        private readonly AnalyzeModel _model;

        private static int _countedFiles;

        /// <summary>
        ///     Suppported Analyses.
        ///     If a new analysis is added, the only place that should be updated is the list in the
        ///     AnalysisMap.
        /// </summary>
        private static readonly IDictionary<string, Type> AnalysesNameToControl =
            AnalysisMap.ANALYZER_NAME_TO_ANALYZER_CONTROL;

        /// <summary>
        ///     Runs the analyses and keeps the interface responsive.
        /// </summary>
        private readonly BackgroundWorker _worker;


        /// <summary>
        ///     Returns true if there are currently analyses added to the selectedAnalyses
        ///     Panel, false if it is empty.
        /// </summary>
        private bool HasCurrentlySelectedAnalyses => SelectedAnalysesPanel.Controls.OfType<AnalysisWrapper>().Any();

        #endregion

        #region FileSelection and Handling

        public void SelectSourceFile(string sourceFile)
        {
            _model.SourcePaths = StringUtils.SplitPaths(sourceFile);
            RefreshForm();
        }

        private void SrcFileButtonClick(object sender, EventArgs e)
        {
            if (SrcFileDialog.ShowDialog() != DialogResult.OK) return;
            _model.SourcePaths = SrcFileDialog.FileNames.ToList();
            OnPathChanged(new PathChangedEventArgs(_model.SourcePaths));
            RefreshForm();
        }

        private void SrcFileTextFieldLeave(object sender, EventArgs e)
        {
            _model.SourcePaths = StringUtils.SplitPaths(SrcFileTextField.Text);
            RefreshForm();
        }

        private void OrgDocButtonClick(object sender, EventArgs e)
        {
            if (OrgDocFileDialog.ShowDialog() != DialogResult.OK) return;
            _model.OriginalDocumentPaths = OrgDocFileDialog.FileNames.ToList();
            RefreshForm();
        }

        private void OrgDocTextFieldLeave(object sender, EventArgs e)
        {
            _model.OriginalDocumentPaths = StringUtils.SplitPaths(OrgDocTextField.Text);
            RefreshForm();
        }

        private void DstDirectoryBrowseButtonClick(object sender, EventArgs e)
        {
            if (DstDirectoryDialog.ShowDialog() != DialogResult.OK) return;
            _model.DestinationPath = DstDirectoryDialog.SelectedPath;
            RefreshForm();
        }

        private void DstFileTextFieldLeave(object sender, EventArgs e)
        {
            _model.DestinationPath = DstFileTextField.Text;
            RefreshForm();
        }

        #endregion

        #region Import And Export

        private void ExportConfigurationButtonClick(object sender, EventArgs e)
        {
            if (!ValidatePaths()) return;

            ImportConfigurationButton.Enabled = false;
            ExportConfigurationButton.Enabled = false;
            AnalyzeButton.Enabled = false;

            var analysisControls = SelectedAnalysesPanel.Controls.
                OfType<AnalysisWrapper>().Select(wrapper => wrapper.Control).ToList();

            var exportWindow = new ExportAnalysisConfiguration(analysisControls,
                _preprocessorControls, _model.SourcePaths.ToList(), _model.DestinationPath);

            exportWindow.ShowDialog();
            exportWindow.Dispose();

            ImportConfigurationButton.Enabled = true;
            ExportConfigurationButton.Enabled = true;
            AnalyzeButton.Enabled = true;
        }

        private void ImportConfigurationButtonClick(object sender, EventArgs e)
        {
            ImportConfigurationButton.Enabled = false;
            ExportConfigurationButton.Enabled = false;
            AnalyzeButton.Enabled = false;

            AnalysisConfigurationXMLDeserializer deserializer = null;

            try
            {
                if (ImportConfigurationDialog.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(ImportConfigurationDialog.FileName)) return;

                ClearAnalysesButtonClick(this, null);
                Enabled = false;

                deserializer = _model.ImportConfiguration(ImportConfigurationDialog.FileName);
                ImportAnalysisConfigurations(deserializer);
                ImportFilterConfigurations(deserializer);
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
            finally
            {
                deserializer?.Dispose();
                ImportConfigurationButton.Enabled = true;
                ExportConfigurationButton.Enabled = true;
                AnalyzeButton.Enabled = true;
                Enabled = true;
                RefreshForm();
            }
        }

        private void ImportAnalysisConfigurations(AnalysisConfigurationXMLDeserializer deserializer)
        {
            foreach (var analysisConfig in deserializer.DeserializeAnalysisConfiguration())
            {
                Type analysisType;
                if (AnalysesNameToControl.TryGetValue(analysisConfig.AnalysisName, out analysisType))
                {
                    var analysisControl = (AnalysisControl) Activator.CreateInstance(analysisType);
                    SelectedAnalysesPanel.Controls.Add(new AnalysisWrapper(SelectedAnalysesPanel,
                        analysisControl, RemoveAnalysisWrapper));
                    analysisControl.Import(analysisConfig);
                }
                else
                {
                    MessageBox.Show(this, "The analysis type '" + analysisConfig.AnalysisName +
                                          "' is unkown by this version of Inputlog.",
                        "Unknown Analysis Configuration",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void ImportFilterConfigurations(AnalysisConfigurationXMLDeserializer deserializer)
        {
            var filterConfigs = deserializer.DeserializeFilterConfiguration();
            if (!filterConfigs.Any()) return;

            var filterDialog = new PostProcessDialog(_preprocessorControls);

            foreach (var config in filterConfigs)
            {
                Type filterType;
                if (filterDialog.AvailableManipulators.TryGetValue(
                    config.FilterName, out filterType))
                {
                    if (config.FilterName.Equals("Time Filter")
                        && !_model.SourcePaths.Any())
                        MessageBox.Show(this,
                            "Attention: the source file attached to this filter is missing!\n" +
                            "This may lead to unpredictable results. Please remove the filter.",
                            "Missing Source File",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    var filterControl = (ProcessControl) Activator.CreateInstance(filterType);
                    filterDialog.SelectedManipulatorsPanel.Controls.
                        Add(new FilterWrapper(filterDialog.SelectedManipulatorsPanel,
                            filterControl, filterDialog.RemoveManipulatorWrapper));
                    filterControl.Click += filterDialog.ManipulatorSelectionListener;
                    filterControl.Import(config);
                }
                else
                {
                    MessageBox.Show(this, "The filter type '" + config.FilterName +
                                          "' is unkown by this version of Inputlog.",
                        "Unknown Filter Configuration",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            filterDialog.ShowDialog();
            _preprocessorControls = filterDialog.GetManipulatorControls();
        }

        #endregion

        #region Report Generation

        private ReportTemplateManager _reportTemplateManager;

        /// <summary>
        ///     Returns the ReportTemplate associated with the
        ///     currently selected reportListing in the GUI.
        /// </summary>
        private ReportTemplate SelectedReportTemplate
        {
            get
            {
                if (ReportsList.SelectedItem != null)
                {
                    var reportListing = (string) ReportsList.SelectedItem;
                    if (!reportListing.Contains('|'))
                        return null;
                    var reportId = reportListing.Split('|')[1];
                    reportId = reportId.Trim();
                    if (_reportTemplateManager.Contains(reportId))
                        return _reportTemplateManager[reportId];
                }
                return null;
            }
        }

        /// <summary>
        ///     Variable holding the currently active template. This may
        ///     be the SelectedReportTemplate, or their might not be any
        ///     template selected and instead a template has been loaded
        ///     through the GUI.
        /// </summary>
        private ReportTemplate _activeTemplate;

        /// <summary>
        ///     Returns true if the user has selected the option
        ///     to load a new template from a file for the reporting.
        /// </summary>
        private bool LoadNewTemplate
        {
            get
            {
                if (ReportsList.SelectedItem == null) return false;
                var item = (string)ReportsList.SelectedItem;
                return item == LOAD_TEMPLATE_FROM_FILE;
            }
        }

        /// <summary>
        ///     Returns true if the user has selected the option 
        ///     to open the template editor.
        /// </summary>
        private bool OpenTemplateEditor
        {
            get 
            {
                if (ReportsList.SelectedItem != null)
                {
                    var item = (string)ReportsList.SelectedItem;
                    return item == OPEN_TEMPLATE_EDITOR;
                }
                return false;
            }
        }

        /// <summary>		 				 				 
        ///     Get the selected output formats for the reporting
        /// </summary>
        private IEnumerable<string> ReportingOutputFormats
        {
            get
            {
                var outputs = ReportOutputCList.CheckBoxItems.Where(item => item.Checked).Select(item => item.Text);
                return outputs;
            }
        }

        /// <summary>
        ///     Returns true if the output formats for the
        ///     report generation have been selected, false if
        ///     no output formats have been specified.
        /// </summary>
        private bool HasReportFormatsSelected
        {
            get
            {
                var formats = ReportingOutputFormats;
                return (formats != null) && (formats.Any());
            }
        }

        /// <summary>
        ///     The user wishes to generate reports. Clicking this button will allow
        ///     the user to select which elements to add to the report, and the structure
        ///     of the report. When these changes are accepted the analyses that are required
        ///     for the information on the report are run for each idfx file that has been
        ///     selected.
        ///     The requested information is extracted from the analyses results per idfx file
        ///     and a separate report is generated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddReportClick(object sender, EventArgs e)
        {
            ClearAnalysesButtonClick(sender, e);
            if (LoadNewTemplate)
            {
                _activeTemplate = null;
                if (OpenTemplateDialog.ShowDialog() == DialogResult.OK)
                {
                    var templatePath = OpenTemplateDialog.FileName;
                    var errors =
                        _reportTemplateManager.OpenTemplate(templatePath, out var templateId);
                    if ((errors != null) && (errors.Any()))
                    {
                        // Show errors!
                        var errorReporter = new TemplateErrorReporter(templatePath);
                        errorReporter.ShowDialog();
                        return;
                    }
                    _activeTemplate = _reportTemplateManager[templateId];
                    ReportsList.SelectedItem = GetTemplateIdentifierString(_activeTemplate);
                }
                else
                {
                    return;
                }
            }								
			else if (OpenTemplateEditor)
            {
                var templatebuilder = new TemplateBuilder.TemplateBuilder();
                templatebuilder.Show();
            }																 		 
            else if (SelectedReportTemplate == null)
            {
                MessageBox.Show("Please select the report type you would like to generate.",
                    "Select report template",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
            else
            {
                _activeTemplate = SelectedReportTemplate;
            }


            if (!_checkHasReportFormatsSelected())
                return;

            if (_activeTemplate != null)
            {
                var targets = _activeTemplate.GetReportTargets();

                var groupByAnalysis = new FilteredGroupedListAccumulator<Report.ReportResource, string>(
                    resource => resource.Analysis
                );
                groupByAnalysis.Accumulate(targets);
                AddAnalysesForReporting(groupByAnalysis.Keys);
            }
        }

        /// <summary>
        ///     Shows an error if there is no reporting output format selected
        /// </summary>
        /// <returns>True if there are output formats selected, false if not.</returns>
        private bool _checkHasReportFormatsSelected()
        {
            if (!HasReportFormatsSelected)
                MessageBox.Show("Please select the desired output formats for the reports.",
                    "Select output format",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            return HasReportFormatsSelected;
        }

        /// <summary>
        ///     Add the corresponding analysisControl to the list of analyses to be run
        ///     for each analysis that is required.
        /// </summary>
        /// <param name="requiredAnalyses">
        ///     The list of analyses that needs
        ///     to be run in order to provide all the information needed for the
        ///     reports.
        /// </param>
        private void AddAnalysesForReporting(IEnumerable<string> requiredAnalyses)
        {
            if (!_isDebug)
            {
                if (_activeTemplate.ID.ToLowerInvariant().Contains("lift")
                    || _activeTemplate.ID.ToLowerInvariant().Contains("edu"))
                {
                    MessageBox.Show(
                        "Please use the Inputlog Edu/LiFT version with this template.",
                        "Wrong Inputlog Version",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            MessageBox.Show("Do not forget to check the number of intervals in the selected analyses.\n" +
                            "The intervals in the analyses may differ from what the report assumes.\n" +
                            "They should be the same. The assumed default interval value is 3",
                            "Check the Number of Intervals", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
            foreach (var analysisName in requiredAnalyses)
                AddAnalysisToActiveAnalyses(AnalysisMap.ANALYSIS_NAME_TO_ANALYZER_NAME[analysisName],
                    null
                );
        }

        /// <summary>
        ///     Method that gets called when we're in 'Report Generation' mode and the
        ///     analyze button gets clicked. That means we want to generate the reports.
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void AnalyzeButtonForReportGenerationClick(object send, MouseEventArgs e)
        {
            // NOTE: THIS IS MOSTLY COPIED FROM ANALYZEBUTTON_CLICK:
            // TODO: CHANGE THE LOGIC SO THE CODE CAN BE REUSED.
            //
            if (!ValidatePaths()) return;
            if (!_checkHasReportFormatsSelected()) return;
            // Report generation can make the pause analysis run multiple pause analyses
            // behind the screens. This is done by a setting a static flag in the PauseAnalyzer.
            // This should always be done for report generation, so set the flag for that here
            // explicitly to true
            PauseAnalyzer.RunMultiple = true;

            _model.IsReportGeneration = true;
            _model.Reports = new Dictionary<string, Report>();
            if (_activeTemplate != null)
            {
                _model.ActiveTemplate = _activeTemplate;
            }
            else
            {
                MessageBox.Show(
                    "Please click the 'Add' button to activate the template.",
                    "Add Template",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            _model.ReportOutputFormats = ReportingOutputFormats;

            var selectedAnalysisControls =
                SelectedAnalysesPanel.Controls.OfType<AnalysisWrapper>().Select(w => w.Control);
            var analysisControls = selectedAnalysisControls as AnalysisControl[] ?? selectedAnalysisControls.ToArray();
            if (!analysisControls.All(c => c.CheckPreconditions())) return;

            AllowInterfaceChangesToAnalyses(false);
            ResetProgressBar(analysisControls.Length);

            // Execute all selected analyses.
            foreach (var control in analysisControls)
            {
                var currentAnalysisName = control.AnalysisName;
                UpdateProgressBar(this,
                    new ProgressEventArgs($"{currentAnalysisName}: {"Started"}",
                        ProgressEventArgs.ProgressCode.STARTED
                    ));

                try
                {
                    control.Analyze(_model);

                    UpdateProgressBar(this,
                        new ProgressEventArgs($"{currentAnalysisName}: {"Done"}",
                            ProgressEventArgs.ProgressCode.DONE
                        ));
                }
                catch (Exception exc)
                {
                    UpdateProgressBar(this,
                        new ProgressEventArgs(exc.Message,
                            ProgressEventArgs.ProgressCode.FAILED
                        ));
                    MessageLogger.CatchException(this, exc, Severity.ERROR);
                }
            }
            if (_model.IsReportGeneration)
                _model.WriteReports();

            AllowInterfaceChangesToAnalyses(true);
        }

        private void ResetProgressBar(int max)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) delegate { ResetProgressBarThreadSafe(max); }, max);
            else
                ResetProgressBarThreadSafe(max);
        }

        private void ResetProgressBarThreadSafe(int max)
        {
            ProgressBar.Value = 0;
            ProgressBar.Step = 1;
            ProgressBar.Maximum = max;
        }

        /// <summary>
        ///     Toggles whether the interface is allowing changes to be made to the
        ///     current values of the controls, or not.
        /// </summary>
        /// <param name="changesAllowed">
        ///     True if changes are now allowed,
        ///     false if they are not.
        /// </param>
        private void AllowInterfaceChangesToAnalyses(bool changesAllowed)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) delegate { AllowInterfaceChangesToAnalysisThreadSafe(changesAllowed); },
                    changesAllowed);
            else
                AllowInterfaceChangesToAnalysisThreadSafe(changesAllowed);
        }

        private void AllowInterfaceChangesToAnalysisThreadSafe(bool changesAllowed)
        {
            AnalyzeButton.Enabled = changesAllowed;
            DisablePanel.Enabled = changesAllowed;
            SelectedAnalysesPanel.Enabled = changesAllowed;
            ClearAnalysesButton.Enabled = changesAllowed;
            ImportConfigurationButton.Enabled = changesAllowed;
            ExportConfigurationButton.Enabled = changesAllowed;

            if (InReportGenerationMode)
            {
                ReportOutputCList.Enabled = changesAllowed;
                ReportsList.Enabled = changesAllowed;
            }
            else
            {
                AnalysesList.Enabled = changesAllowed;
            }
        }

        private void AnalysesRadioBttn_CheckedChanged(object sender, EventArgs e)
        {
            // otherwise this is not a real event. It's just an initialization.
            if (e != null)
            {
                var newButton =
                    AnalysesRadioBttn.Checked
                        ? AnalysesRadioBttn
                        : ReportRadioBttn;
                var oldButton =
                    AnalysesRadioBttn != newButton
                        ? AnalysesRadioBttn
                        : ReportRadioBttn;
                if (!ConfirmChangeToDifferentMode(newButton, oldButton))
                    return;
            }


            if (AnalysesRadioBttn.Checked)
            {
                AnalysesList.Enabled = true;
                AddAnalysesButton.Enabled = true;

                // Remove handlers before adding to make sure we don't add the handler an extra time instead
                // of just replacing it.
                AnalyzeButton.MouseClick -= AnalyzeButtonClick;
                AnalyzeButton.MouseClick += AnalyzeButtonClick;
                AnalyzeButton.Text = "Analyze";
                ClearAnalysesButton.Text = "Clear";

                // Disable reporting stuff
                ReportsList.SelectedItem = null;
                ReportsList.Enabled = false;
                ReportOutputCList.Enabled = false;
                ReportOutputCList.SelectedItem = null;
                AddReportButton.Enabled = false;
                AnalyzeButton.MouseClick -= AnalyzeButtonForReportGenerationClick;
            }
            else
            {
                AddAnalysesButton.Enabled = false;
                AnalysesList.Enabled = false;
                AnalyzeButton.MouseClick -= AnalyzeButtonClick;
                ReportsList.Enabled = true;
                ReportOutputCList.Enabled = true;
                AddReportButton.Enabled = true;
                LoadReportTemplates();


                // Remove handlers before adding to make sure we don't add the handler an extra time instead
                // of just replacing it.
                AnalyzeButton.MouseClick -= AnalyzeButtonForReportGenerationClick;
                AnalyzeButton.MouseClick += AnalyzeButtonForReportGenerationClick;
                AnalyzeButton.Text = "Generate Reports";
                ClearAnalysesButton.Text = "Cancel";
            }
        }

        /// <summary>
        ///     Allows the user to confirm the change to the new mode (analyses from reporting,
        ///     or to reporting from analyses) if the 'selected analyses' field is currently
        ///     not empty.
        ///     Confirming the change will empty the currently selected analyes (and their
        ///     parameters).
        /// </summary>
        /// <param name="newActiveRadio">Radio button that will be set to active.</param>
        /// <param name="oldActiveRadio">Radio button that will be set to inactive..</param>
        /// <returns>True if the change is confirmed, false if the user wishes to cancel the change.</returns>
        private bool ConfirmChangeToDifferentMode(RadioButton newActiveRadio, RadioButton oldActiveRadio)
        {
            if (HasCurrentlySelectedAnalyses)
            {
                var mode = newActiveRadio == AnalysesRadioBttn ? "analyses" : "reporting";
                var result = MessageBox.Show(
                    "Are you sure you want change to " + mode +
                    " mode? Doing so will clear all selected analyses and their parameters.",
                    "Please confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    ClearAnalysesButtonClick(this, null);
                    return true;
                }
                AnalysesRadioBttn.CheckedChanged -= AnalysesRadioBttn_CheckedChanged;
                newActiveRadio.Checked = false;
                oldActiveRadio.Checked = true;
                AnalysesRadioBttn.CheckedChanged += AnalysesRadioBttn_CheckedChanged;
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Load the templates into the ReportsTemplate list.
        /// </summary>
        private void LoadReportTemplates()
        {
            ReportsList.Items.Clear();
            _reportTemplateManager = ReportTemplateManager.TemplateInstance;
            object[] templates = new object[_reportTemplateManager.TemplateIDs.Count()];

            var index = 0;
            foreach (var templateId in _reportTemplateManager.TemplateIDs)
            {
                var template = _reportTemplateManager[templateId];
                var templateString = GetTemplateIdentifierString(template);
                templates[index] = templateString;
                index += 1;
            }
            ReportsList.Items.AddRange(templates);
            ReportsList.Items.Add(LOAD_TEMPLATE_FROM_FILE);
			ReportsList.Items.Add(OPEN_TEMPLATE_EDITOR);
            if (ReportsList != null) ReportsList.SelectedIndex = 1;
            if (_reportTemplateManager.FoundFaultyTemplates && !TemplateErrorReporter.IsOpened)
            {
                var errorReporter = new TemplateErrorReporter();
                errorReporter.ShowDialog();
            }
        }

        private string GetTemplateIdentifierString(ReportTemplate template)
        {
            return template.Localization + " | " + template.ID;
        }

        private void TemplateListUpdatedHandler(object sender, EventArgs args)
        {
            LoadReportTemplates();
        }

        #endregion
    }
}