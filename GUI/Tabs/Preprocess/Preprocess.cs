using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GUI.Flow;
using GUI.Tabs.Preprocess.Conversion;
using GUI.Tabs.Preprocess.Filters;
using GUI.Tabs.Preprocess.Merge;
using GUI.Tabs.Preprocess.Merge.FlowPages;
using GUI.Tabs.Preprocess.Recoders;
using GUI.Util;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.CSV;
using InputLog.Core.IO.Xml;
using InputLog.Core.Preprocessing.Recode;
using InputLog.Core.Util;
using Convert = GUI.Tabs.Preprocess.Conversion.Convert;
using EventType = GUI.Tabs.Preprocess.Filters.EventType;

namespace GUI.Tabs.Preprocess
{
    public sealed partial class Preprocess : GuiPathChangedEvent // UserControl
    {
        public Preprocess()
        {
            InitializeComponent();
            _model = new PreprocessModel();
            _destinationDialog = new FolderBrowserDialog {ShowNewFolderButton = true};

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 1000;
            ProcessStarted += StartProcess;
            ProcessEnded += EndProcess;
            StepCompleted += CompleteStep;

            _convertControl = new Convert {Dock = DockStyle.Fill};
            _mergeControl = new MergeSelector {Dock = DockStyle.Fill};

            segmentKeyDelimiters.DataSource = new BindingSource(PreprocessModel.Keys, null);
            segmentKeyDelimiters.DisplayMember = "Key";
            segmentKeyDelimiters.ValueMember = "Value";

            //if (Directory.Exists("DNSOld"))
            _mergeInputFileTypes.Add("Dragon Naturally Speaking (*.dat)", typeof (DragonFlowController));
            _mergeInputFileTypes.Add("Tobii/Eyelink files (*.tsv)", typeof (EyetrackFlowController));
            //InputFilesDD.Items.AddRange(MergeInputFileTypes.Keys.ToArray());
            InputFilesDD.SelectedIndex = _mergeInputFileTypes.Count - 1;

            _convertControl.Visible = false;
            _mergeControl.Visible = false;

            RecodePanel.SetAvailableManipulators(
                new Dictionary<string, Type>
                {
                    {FocusRewrite.NAME, typeof (FocusRewrite)},
                    {IdfxRecoder.NAME, typeof (IdfxRecoder)},
                    {NegtivePauseRemover.NAME, typeof (NegtivePauseRemover)},
                    {RemoveTaskbar.NAME, typeof(RemoveTaskbar)}
                }
                );

            FilterPanel.SetAvailableManipulators(
                new Dictionary<string, Type>
                {
                    {EventType.NAME, typeof (EventType)},
                    {Time.NAME, typeof (Time)},
                    {Window.NAME, typeof (Window)}
                }
                );

            FilterRadioEnableSubParts(false);
            RecodeRadioEnableSubParts(false);
            SegmentRadioEnableSubParts(false);
            MergeRadioEnableSubParts(false);

            FilterFilesUpdated += FilterPanel.UpdateSelectedFiles;
            RecodeFilesUpdated += RecodePanel.UpdateSelectedFiles;

            UpdateInfo();
            InitTooltips();
            UpdateSelectedFileInfo();
        }

        /// <summary>
        ///     Sets all radio buttons to unchecked, except for the active radio button.
        ///     This is not done automatically as the three radio buttons are spread over
        ///     two different parent components.
        /// </summary>
        /// <param name="activeRadio">The active radio button.</param>
        private void UpdateTopLevelRadio(ToplevelRadio activeRadio)
        {
            FileLevelRadio.Checked = (activeRadio == ToplevelRadio.FILE_LEVEL);
            FilterRadio.Checked = (activeRadio == ToplevelRadio.FILTER);
            RecodeRadio.Checked = (activeRadio == ToplevelRadio.RECODE);
            SegmentationRadio.Checked = (activeRadio == ToplevelRadio.SEGMENT);
            MergeRadio.Checked = (activeRadio == ToplevelRadio.MERGE);

            ProcessButton.SetPropertyThreadSafe(() => ProcessButton.Enabled, !FileLevelRadio.Checked);
        }

        /// <summary>
        ///     Makes sure that only one 'radio button' (it are actually checkboxes) is
        ///     checked on the file level at a single time.
        /// </summary>
        /// <param name="activeRadio"></param>
        private void UpdateFileLevelRadio(FilelevelRadio activeRadio)
        {
            FileLevelRadioMerge.Checked = (activeRadio == FilelevelRadio.MERGE);
            FileLevelRadioConvert.Checked = (activeRadio == FilelevelRadio.CONVERT);

            ProcessButton.SetPropertyThreadSafe(() => ProcessButton.Enabled, FileLevelRadioConvert.Checked);
        }

        /// <summary>
        ///     Updating info tooltips, messages etc to reflect the current situation
        ///     of the preprocess tab.
        /// </summary>
        private void UpdateInfo()
        {
            MightDisplayWarningMessage(RecodePanel, TooltipWarningRecode);
            MightDisplayWarningMessage(FilterPanel, TooltipWarningFilter);
        }

        /// <summary>
        ///     Initializes the tooltip information.
        /// </summary>
        private void InitTooltips()
        {
            TooltipInfo.ShowAlways = true;

            TooltipInfo.SetToolTip(FileLevelRadio,
                "Preprocessing steps that automatically affect whole files.");
            TooltipInfo.SetToolTip(RecodeRadio,
                "Preprocessing steps that alter, combine and/or recode specified information in the file(s).");
            TooltipInfo.SetToolTip(FilterRadio,
                "Preprocessing steps that filter (remove) certain data from the scope of the file(s).");

            TooltipInfo.SetToolTip(FileLevelRadioMerge,
                "Merge idfx files with files from different applications.");
            TooltipInfo.SetToolTip(FileLevelRadioConvert,
                "Convert between different versions of inputlog logging files.");

            TooltipWarningRecode.SetToolTip(RecodePanel,
                "Only one type of preprocessors can be executed \n" +
                "at the same time. The currently specified recode \n" +
                "preprocessors will not be executed upon preprocessing.");
            TooltipWarningFilter.SetToolTip(FilterPanel,
                "Only one type of preprocessors can be executed \n" +
                "at the same time. The currently specified filter \n" +
                "preprocessors will not be executed upon preprocessing.");
        }

        /// <summary>
        ///     The file level radio button has been selected or unselected.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void FileLevelRadioCheckedChanged(object sender, EventArgs e)
        {
            if (FileLevelRadio.Checked)
            {
                UpdateTopLevelRadio(ToplevelRadio.FILE_LEVEL);
                FileLevelRadioConvert.Enabled = true;
                FileLevelRadioMerge.Enabled = true;
                InputFilesDD.Enabled = true;
            }
            else
            {
                FileLevelRadioConvert.Enabled = false;
                FileLevelRadioMerge.Enabled = false;
                InputFilesDD.Enabled = false;
            }

            UpdateInfo();
        }

        /// <summary>
        ///     The filters radio button has been selected or unselected.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void FilterRadioCheckedChanged(object sender, EventArgs e)
        {
            if (FilterRadio.Checked)
            {
                FilterPanel.ValidatePaths();
                UpdateTopLevelRadio(ToplevelRadio.FILTER);
                FilterRadioEnableSubParts(true);
            }
            else
            {
                FilterRadioEnableSubParts(false);
            }

            UpdateInfo();
        }

        /// <summary>
        ///     Enables or disables the sub parts of the filter radio level preprocessors.
        /// </summary>
        /// <param name="enable">
        ///     True if we need to enable the subparts, false if we need to
        ///     disable them.
        /// </param>
        private void FilterRadioEnableSubParts(bool enable)
        {
            FilterPanel.Enabled = enable;
        }

        /// <summary>
        ///     The recode radio button has been selected or unselected.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RecodeRadioCheckedChanged(object sender, EventArgs e)
        {
            if (RecodeRadio.Checked)
            {
                UpdateTopLevelRadio(ToplevelRadio.RECODE);
                RecodeRadioEnableSubParts(true);
                RecodePanel.ValidatePaths();
            }
            else
            {
                RecodeRadioEnableSubParts(false);
            }

            UpdateInfo();
        }

        /// <summary>
        ///     Enables or disables the sub parts of the Recode radio level preprocessors.
        /// </summary>
        /// <param name="enable">
        ///     True if we need to enable the subparts, false if we need to
        ///     disable them.
        /// </param>
        private void RecodeRadioEnableSubParts(bool enable)
        {
            RecodePanel.Enabled = enable;
            NrSrcFiles.Enabled = enable;
        }

        /// <summary>
        ///     Making sure that only one checkbox is selected at a time, on the
        ///     file level. The checkboxes function as radio boxes.
        ///     If the radio conversion is checked we load the conversion usercontrol,
        ///     if not we unload the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioVersionConvertCheckedChanged(object sender, EventArgs e)
        {
            if (FileLevelRadio.Checked)
            {
                if (FileLevelRadioConvert.Checked)
                {
                    UpdateFileLevelRadio(FilelevelRadio.CONVERT);
                    _mergeControl.Visible = false;

                    var form = new ConversionForm(_model.SourcePaths.Any() ? _model.SourcePaths[0] : "");
                    form.Show();
                    form.BringToFront();
                    form.Closed += delegate
                                   {
                                       BringToFront();
                                       _convertControl = form.ConvertControl;
                                   };
                }
                else
                {
                    _convertControl.Visible = false;
                }
            }

            UpdateInfo();
        }

        /// <summary>
        ///     Tooltip that shows when the mouse appears over a disabled
        ///     EventProcessor when the mouse hovers over the control.
        /// </summary>
        /// <param name="control">Control to display warning message on.</param>
        /// <param name="toolTip">
        ///     The tooltip that will be activated in order to
        ///     display the warning message.
        /// </param>
        private static void MightDisplayWarningMessage(EventProcessor control, ToolTip toolTip)
        {
            if (control == null) return;

            if (!control.Enabled && control.HasContent)
            {
                var toolTipString = toolTip.GetToolTip(control);
                toolTip.Show(toolTipString, control, control.Width/10, control.Height/5, 5000);
            }
            else if (toolTip.Active)
            {
                toolTip.Hide(control);
            }
        }

        /// <summary>
        ///     Selecting the files to be preprocessed by the filters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSelectButtonFilterClick(object sender, EventArgs e)
        {
            if (SelectFileDialog.ShowDialog() == DialogResult.OK)
            {
                _model.SourcePaths = SelectFileDialog.FileNames;
                OnPathChanged(new PathChangedEventArgs(_model.SourcePaths));
            }
            UpdateSelectedFileInfo();
        }

        public void SelectFile(string path)
        {
            _model.SourcePaths = StringUtils.SplitPaths(path); 
            UpdateSelectedFileInfo();
        }

        /// <summary>
        ///     Update the label for the selected file and the tooltip
        ///     message for the label.
        /// </summary>
        private void UpdateSelectedFileInfo()
        {
            FilterFilesUpdated?.Invoke(_model.SourcePaths);
            RecodeFilesUpdated?.Invoke(_model.SourcePaths);

            SrcFileTextField.Text = StringUtils.JoinPaths(_model.SourcePaths);
            SrcFileTextField.Select(SrcFileTextField.Text.Length, 0);

            NrSrcFiles.Text = _model.SourcePaths.Count + " File(s)";
            var caption = "";

            if (_model.SourcePaths.Any())
            {
                caption = _model.SourcePaths.Aggregate(caption, 
                    (current, filePath) => current + (StringUtils.ShortenPathname(filePath, 100) + "\n"));
            }

            FileListTooltip.SetToolTip(NrSrcFiles, caption);
            ProgressBar.Maximum = _model.SourcePaths.Count * 3;
        }

        /// <summary>
        ///     The filtering button. This processes the selected files and does whatever
        ///     conversions/filtering or recoding is requested by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessButtonClick(object sender, EventArgs e)
        {
            if (!FileLevelRadio.Checked && !FilterRadio.Checked && !RecodeRadio.Checked
                && !SegmentationRadio.Checked && !MergeRadio.Checked)
            {
                MessageBox.Show("No preprocessing step has been specified. " +
                                "First specify and configure the preprocessing, then try again.",
                    "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (FileLevelRadio.Checked) HandleFileProcessing();
            else if (FilterRadio.Checked) HandleFilterProcessing();
            else if (RecodeRadio.Checked) HandleRecodeProcessing();
            else if (SegmentationRadio.Checked) HandleSegmentProcessing();
            else if (MergeRadio.Checked) HandleMergeProcessing();
        }

        /// <summary>
        ///     Handling the recoding.
        /// </summary>
        private void HandleRecodeProcessing()
        {
            HandleEventProcessing(RecodePanel);
        }

        /// <summary>
        ///     Handling the filtering.
        /// </summary>
        private void HandleFilterProcessing()
        {
            HandleEventProcessing(FilterPanel);
        }

        /// <summary>
        ///     Handling the processing (recoding/filtering) on event level.
        ///     This is the same for filtering and recoding, only
        ///     the panel the controls are read from is different.
        /// </summary>
        /// <param name="eventPanel">Panel to read the processors from.</param>
        private void HandleEventProcessing(EventProcessor eventPanel)
        {
            var erroneousFiles = new List<string>();
            var caughtExceptions = new List<Exception>();
           
            var processerList = eventPanel.GetActiveProcessers();
            if (!processerList.Any())
            {
                MessageBox.Show("You must add at least one preprocessor in order to proceed.",
                    "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (!_model.SourcePaths.Any())
            {
                MessageBox.Show("No source path was given.",
                    "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // A set of different files might hav been saved in the focus log file,
            // they need reprocessing.
            if (FocusRewrite.LoadFromLogFile)
            {
                _model.SourcePaths = FocusRewrite.SavedFiles;
                ProgressBar.Maximum = _model.SourcePaths.Count * 3;
            }

            ProcessStarted?.Invoke("Preprocessing", _model.SourcePaths.Count * processerList.Count);

            var filterControl = processerList.Find(x => x.FilterAbbreviation == "time");
            // Identifying RemoveTaskbar. May be removed together with the temporary Remove Taskbar recoding.
            if (processerList.Find(x => x.Name == "RemoveTaskbar") != null) _recodeName = "REMOVED";

            var time = filterControl as Time;
            if (time != null)
            {
                if (time.ConfigurationDialog.NewStartID.Equals(time.ConfigurationDialog.InitStartID) &&
                    time.ConfigurationDialog.NewStopID.Equals(time.ConfigurationDialog.InitStopID))
                {
                    MessageBox.Show("The Time Filter was not used,\nplease, remove it or change its settings.",
                        "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            _destinationDialog.SelectedPath = Path.GetDirectoryName(_model.SourcePaths[0]);
            if (_destinationDialog.ShowDialog() != DialogResult.OK) return;

            // Collecting the filterResults for every file passing through the EvenIDFilter.
            FilterResultList = new List<ArrayList>();


            foreach (var sourcePath in _model.SourcePaths)
            {
                try
                {
                    var eventLogReader = EventLogFactory.CreateFileEventLogReader(sourcePath, LogFormat.XML);
                    SessionIdentification sessionId = eventLogReader.ReadSessionIdentification();
                    sessionId.SetFileName(sourcePath);
                    var events = eventLogReader.ReadEvents();
                    var processedList = new List<Event>();

                    // If an EventIDFilter resets the start time, then
                    // the LogRelativeCreationDate in sessionId must be 0.
                    if (filterControl != null && time.ConfigurationDialog.ResetTime)
                    {
                        sessionId.SetRelativeCreationTime(0);
                    }

                    FocusRewriter focus = null;

                    foreach (var processerControl in processerList)
                    {
                        try
                        {
                            var processor = processerControl.GetPreprocessor(sessionId);
                            processedList = processor.Process(events, sessionId);

                            // When IsFixedEndFilter = true, a _filterEntry is made in EventIDFilter.
                            // _filterEntry[0] contains the IdfxID;
                            // _filterEntry[1] contains the time at the filter event. If no filter event was found it is the time 
                            // of the last event with timed information, i.e. equal to the time in _filterEntry[2].
                            // _filterEntry[2] contains the very last timed information found in the event list.
                            var filterResult = processor.GetFilterResult();
                            // Collecting all filter results.
                            FilterResultList.Add(filterResult);

                            var rewriter = processor as FocusRewriter;
                            if (rewriter != null && focus == null)
                                focus = rewriter;
                        }
                        catch (Exception excProcess)
                        {
                            erroneousFiles.Add(sourcePath);
                            caughtExceptions.Add(excProcess);
                        }

                        StepCompleted();
                    }

                    var lastSlash = sourcePath.LastIndexOf(Path.DirectorySeparatorChar);
                    var fileName = sourcePath.Substring(lastSlash + 1);
                    var destinationPath = PathSanitizer.Uniquify(
                        ExtendFileName(fileName, _destinationDialog.SelectedPath));

                    var logWriter = EventLogFactory.CreateFileEventLogWriter(destinationPath, LogFormat.XML);
                    logWriter.WriteExistingFile(sessionId, processedList);

                    focus?.ToXml(Path.ChangeExtension(destinationPath, ".log.xml"));

                    if (_model.SourcePaths.Count == 1)
                        Gui.MyRecentFiles.AddFile(destinationPath);
                }
                catch (Exception exc)
                {
                    erroneousFiles.Add(sourcePath);
                    caughtExceptions.Add(exc);

                    for (var i = 0; i < processerList.Count; i++)
                        StepCompleted();
                }
            }

            if (!FilterResultList.All(x => x.IsNullOrEmpty()))
            {
                var writer = new CSVTextWriter();
                var date = DateTime.Now.ToString("yyyyMMdd");
                var filePath = PathSanitizer.Uniquify(Path.Combine(_destinationDialog.SelectedPath, $"FilterResult_{date}.csv"));
                writer.WriteToFile(FilterResultList, filePath, ";");
            }
            ProcessEnded();

            // TODO: Log all errors and exceptions.
            //MessageLogger.CatchException(this, exc, Severity.ERROR,
            //    "Unable to read events from file. Are you sure you provided a valid logfile?");
        }

        /// <summary>
        ///     Handling the processing of files. Converting or DNS.
        /// </summary>
        private void HandleFileProcessing()
        {
            if (ProcessStarted != null) ProcessStarted("Conversion", 1);
            _convertControl.DoConversion();
            if (ProcessEnded != null)
            {
                ProcessEnded();
            }

            if (!(_convertControl.ThisLogFormat.Equals(LogFormat.TRANSLOG_XML)))
            {
                MessageBox.Show("Please note that the only supported analysis type for older, " +
                                "converted files is the General Analysis.", "Converted File Limitations",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        ///     Enables or disables the sub parts of the segment radio level preprocessors.
        /// </summary>
        /// <param name="enable">
        ///     True if we need to enable the subparts, false if we need to
        ///     disable them.
        /// </param>
        private void SegmentRadioEnableSubParts(bool enable)
        {
            segmentInitialPauseCBX.Enabled = enable;
            segmentKeyDelimiters.Enabled = enable;
        }

        /// <summary>
        ///     Enables or disables the sub parts of the merge radio level preprocessors.
        /// </summary>
        /// <param name="enable">
        ///     True if we need to enable the subparts, false if we need to
        ///     disable them.
        /// </param>
        private void MergeRadioEnableSubParts(bool enable)
        {
            mergeIncludePause.Enabled = enable;
        }

        /// <summary>
        ///     Handles the segmentation of IDFX's
        /// </summary>
        private void HandleSegmentProcessing()
        {
            if (!ValidatePaths()) return;

            if (ProcessStarted != null) ProcessStarted("Segmentation", 1);
            var segmenter = new IdfxSegmenter(
                segmentKeyDelimiters.SelectedValue.ToString(),
                segmentInitialPauseCBX.Checked);
            var numSegments = segmenter.Segment(_model.SourcePaths.ToArray());
            if (ProcessEnded != null)
            {
                ProcessEnded();
            }

            MessageBox.Show(string.Format("Finished segmenting {0} file(s) into {1} segments",
                _model.SourcePaths.Count(), numSegments), "Segmenting Finished",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     Handles the merging of IDFX files.
        /// </summary>
        private void HandleMergeProcessing()
        {
            if (!ValidatePaths()) return;

            if (ProcessStarted != null) ProcessStarted("Merging", 1);
            _model.MergeIdfx(mergeIncludePause.Checked);
            if (ProcessEnded != null)
            {
                ProcessEnded();
            }

            MessageBox.Show("Finished merging " + _model.SourcePaths.Count() + " idfx file(s)",
                "IDFX Merging Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     Structure of a callback method for the start of a process.
        /// </summary>
        /// <param name="proces">Name of the process</param>
        /// <param name="steps">Number of steps in the process</param>
        private void StartProcess(string proces, int steps)
        {
            ProgressBar.Minimum = steps;
            ProgressBar.Value = ProgressBar.Minimum;
        }

        /// <summary>
        ///     Structure of a callback method to be called when a step in the
        ///     current process has been completed.
        /// </summary>
        private void CompleteStep()
        {
            ProgressBar.Value++;
        }

        /// <summary>
        ///     Updating the progressBar value thread-safely.
        /// </summary>
        /// <param name="value">Value for the progressbar.</param>
        private void SetProgressBarValue(int value)
        {
            if (ProgressBar.InvokeRequired)
            {
                var changeFunction = new ChangeProgressBarValue(SetProgressBarValue);
                Invoke(changeFunction, value);
            }
            else
            {
                ProgressBar.Value = value;
            }
        }

        /// <summary>
        ///     Updating the processbutton text thread-safely.
        /// </summary>
        /// <param name="txt">New processbutton text.</param>
        private void SetProcessButtonText(string txt)
        {
            if (ProcessButton.InvokeRequired)
            {
                var changeFunction = new ChangeProcessButtonText(SetProcessButtonText);
                Invoke(changeFunction, txt);
            }
            else
            {
                ProcessButton.Text = txt;
            }
        }

        /// <summary>
        ///     Structure of a callback method to be called when all the current
        ///     process has been ended, succesfully or unsucessfully.
        ///     After ending the process we switch to the analyze tab.
        /// </summary>
        /// <summary>
        ///     Structure of a callback method to be called when all the current
        ///     process has been ended, succesfully or unsucessfully.
        ///     After ending the process we switch to the analyze tab.
        /// </summary>
        private void EndProcess()
        {
            var t = new Thread(delegate()
                               {
                                   var txt = ProcessButton.Text;
                                   SetProgressBarValue(ProgressBar.Maximum);
                                   SetProcessButtonText("Done...");
                                   Thread.Sleep(2000);
                                   SetProgressBarValue(ProgressBar.Minimum);
                                   SetProcessButtonText(txt);
                               });

            t.Start();

            // TODO: Enter the resulting files as source files in the analyze tab?
        }

        private void SegmentRadioCheckedChanged(object sender, EventArgs e)
        {
            if (SegmentationRadio.Checked)
            {
                UpdateTopLevelRadio(ToplevelRadio.SEGMENT);
                SegmentRadioEnableSubParts(true);
            }
            else
            {
                SegmentRadioEnableSubParts(false);
            }

            UpdateInfo();
        }

        private void MergeRadioCheckedChanged(object sender, EventArgs e)
        {
            if (MergeRadio.Checked)
            {
                UpdateTopLevelRadio(ToplevelRadio.MERGE);
                MergeRadioEnableSubParts(true);
            }
            else
            {
                MergeRadioEnableSubParts(false);
            }

            UpdateInfo();
        }

        private string ExtendFileName(string srcFile, string destDir)
        {          
            var srcBase = Path.GetFileNameWithoutExtension(srcFile);
            var date = DateTime.Now.ToString("yyyyMMdd");
            var abbr = (FilterRadio.Checked) ? "FILTER" : (RecodeRadio.Checked) ? _recodeName : "";
            return Path.Combine(destDir, string.Format("{0}_{1}_{2}.idfx", srcBase, date, abbr));
        }

        private void FileLevelRadioMergeClick(object sender, EventArgs e)
        {
            var flowController = (AbstractFlowController) Activator.CreateInstance(
                _mergeInputFileTypes[(string) InputFilesDD.SelectedItem]);
            var flowWindow = new FlowGUI(flowController);
            flowWindow.Show();
        }

        private bool ValidatePaths()
        {
            if (_model.SourcePaths.Any()) return true;
            MessageBox.Show("You must specify at least 1 file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        #region events_delegates

        /// <summary>
        ///     Delegate to update the processbutton text form another thread.
        /// </summary>
        /// <param name="txt">Text to change the button's text too.</param>
        private delegate void ChangeProcessButtonText(string txt);

        /// <summary>
        ///     Delegate to update the progressbar value from another thread
        ///     than the GUI thread.
        /// </summary>
        /// <param name="value">Value to update the progressbar too.</param>
        private delegate void ChangeProgressBarValue(int value);

        /// <summary>
        ///     Structure of a callback method to be called when a step in the
        ///     current process has been completed.
        /// </summary>
        public delegate void CompleteStepDelegate();

        /// <summary>
        ///     Structure of a callback method to be called when all the current
        ///     proces has been ended, succesfully or unsucessfully.
        /// </summary>
        public delegate void EndProcessDelegate();

        /// <summary>
        ///     Structure of a callback method for the start of a proces.
        /// </summary>
        /// <param name="proces">Name of the process</param>
        /// <param name="steps">Number of steps in the process</param>
        public delegate void StartProcessDelegate(string proces, int steps);

        /// <summary>
        ///     Delegate function for when the selected files have been updated.
        /// </summary>
        /// <param name="filePaths">The list of all filepahts of all selected files.</param>
        public delegate void UpdateSelectedFiles(IList<string> filePaths);

        /// <summary>
        ///     Event that gets called when the number of files for the
        ///     filter preprocessing has been updated.
        /// </summary>
        public event UpdateSelectedFiles FilterFilesUpdated;

        /// <summary>
        ///     Event that gets called when the number of files for the
        ///     recode preprocessing has been updated.
        /// </summary>
        public event UpdateSelectedFiles RecodeFilesUpdated;

        /// <summary>
        ///     Event to be called when a process has been started.
        /// </summary>
        public event StartProcessDelegate ProcessStarted;

        /// <summary>
        ///     Event to be called when as step in the process has been completed.
        /// </summary>
        public event CompleteStepDelegate StepCompleted;

        /// <summary>
        ///     Event to be called when a process has ended.
        /// </summary>
        public event EndProcessDelegate ProcessEnded;

        #endregion

        #region fields

        private string _recodeName = "RECODE";

        /// <summary>
        ///     Business logic for the Preprocess tab.
        /// </summary>
        private readonly PreprocessModel _model;

        /// <summary>
        ///     Dialog that lets the user select the folder to save the preprocessed files.
        /// </summary>
        private readonly FolderBrowserDialog _destinationDialog;

        /// <summary>
        ///     The conversion user control.
        /// </summary>
        private Convert _convertControl;

        /// <summary>
        ///     The merging user control.
        /// </summary>
        private readonly MergeSelector _mergeControl;

        /// <summary>
        /// List with the filterResult arrayLists.
        /// A filterResult contains a _filterEntry made in EventIDFilter for every file passing.
        /// </summary>
        public List<ArrayList> FilterResultList;

        /// <summary>
        ///     Specifies the two types of file level 'radio buttons'.
        /// </summary>
        private enum FilelevelRadio
        {
            MERGE,
            CONVERT
        }

        /// <summary>
        ///     Specifies the three types of top level radio buttons
        /// </summary>
        private enum ToplevelRadio
        {
            FILE_LEVEL,
            FILTER,
            RECODE,
            SEGMENT,
            MERGE
        }

        private readonly Dictionary<String, Type> _mergeInputFileTypes = new Dictionary<string, Type>();

        #endregion
    }
}