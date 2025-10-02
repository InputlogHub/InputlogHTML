using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GUI.Tabs.Preprocess.Filters.TimeFilterHelp;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess.Filters
{
    /// <summary>
    /// This filter allows changing the start and/or end point of an analysis by changing 
    /// the ID of the first and/or last Event. 
    /// The user can override the start time of the (new) first event with a zero time stamp.
    /// </summary>
    public partial class Time : ProcessControl
    {
        #region fields

        /// <summary>
        /// Name of this Filter.
        /// </summary>
        public const string NAME = "ID & Time Filter";

        /// <summary>
        /// The dialog to configure the timeFilter.
        /// </summary>
        public TimeConfiguration ConfigurationDialog { get; private set; }

        private Dictionary<string, string> Configurations;

        /// <summary>
        /// The 6 different modes for the time filter.
        /// Keeping the start time - using time parameter as filter
        /// Keeping the start time - using the id parameter as filter
        /// Keeping the start time - using the first and last keyboard event as filter
        /// Resetting the start time - using the time parameter as filter
        /// Resetting the start time - using the id parameter as filter
        /// Resetting the start time - using the first and last keyboard event as filter
        /// 
        /// Initial content...
        /// Config: reusing saved settings.
        /// </summary>
        private enum Mode
        {
            KEEP_TIME,
            KEEP_ID,
            KEEP_TIME_FIRST_KEY,
            RESET_TIME_FIRST_KEY,
            RESET_TIME,
            RESET_ID,          
            INITIAL,
            CONFIG,
            MANUAL_CONFIG,
            AUTO_CONFIG
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Time()
        {
            InitializeComponent();
            FilterName = "Time Filter";
            FilterAbbreviation = "time";          
            ConfigurationDialog = new TimeConfiguration();
            LoadConfiguration(Mode.INITIAL);
            InitHelp(MoreInfoLabel, FilterAbbreviation);
        }

        /// <summary>
        /// Initial settings of the user interface. 
        /// Manual settings for one file only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualBtnClick(object sender, EventArgs e)
        {
            if (FilePaths.Count > 1)
            {
                {
                    MessageBox.Show("Multiple files selected.\n" +
                                    "Manual processing is only possible for one file at a time. ",
                        "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            AutoBtn.Enabled = false;
            InitialIDPanel.SendToBack();
            LoadConfiguration(Mode.MANUAL_CONFIG);
        }

        /// <summary>
        /// Initial settings of the user interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoBtnClick(object sender, EventArgs e)
        {
            ManualBtn.Enabled = false;
            InitialIDPanel.SendToBack();
            ConfigurationDialog.FixedStartEnd = true;
            LoadConfiguration(Mode.AUTO_CONFIG);
        }

        /// <summary>
        /// Translate the information in the configuration dialog
        /// into a singular mode for the summary.
        /// </summary>
        /// <returns>The identified mode in the configuration dialog.</returns>
        private Mode CurrentMode()
        {
            if (ConfigurationDialog.FixedStartEnd)
            {
                return ConfigurationDialog.ResetTime ? Mode.RESET_TIME_FIRST_KEY : Mode.KEEP_TIME_FIRST_KEY;
            }
            if (ConfigurationDialog.ResetTime)
            {
                return ConfigurationDialog.IDBased ? Mode.RESET_ID : Mode.RESET_TIME;
            }
            return ConfigurationDialog.IDBased ? Mode.KEEP_ID : Mode.KEEP_TIME;
        }

        /// <summary>
        /// Load the user-entered configuration in the summary display.
        /// The user configuration is entered in the Time_Configuration class.
        /// 
        /// After it is entered the summary is updated with the newly specified
        /// values.
        /// </summary>
        /// <param name="mode">The mode of the configuration. Keeping or resetting start 
        /// time. Is the filtering time or id-based?</param>
        private void LoadConfiguration(Mode mode)
        {
            switch (mode)
            {
                case Mode.KEEP_ID:
                    Summary.SetTitle("Keep the initial start time.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    break;
                case Mode.KEEP_TIME:
                    Summary.SetTitle("Keep the initial start time.");
                    Summary.SetLeftLabels("Start Time:", "End Time:");
                    break;
                case Mode.RESET_ID:
                    Summary.SetTitle("Reset the initial start time.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    break;
                case Mode.RESET_TIME:
                    Summary.SetTitle("Reset the initial start time.");
                    Summary.SetLeftLabels("Start Time:", "End Time:");
                    break;
                case Mode.KEEP_TIME_FIRST_KEY:
                    Summary.SetTitle("Fixed Start && End ID - Keeping the start time.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    break;
                case Mode.RESET_TIME_FIRST_KEY:
                    Summary.SetTitle("Fixed Start && End ID - Start time is set to zero.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    break;
                case Mode.CONFIG:
                    string startId;
                    string endId;
                    Configurations.TryGetValue("StartID", out startId);
                    ConfigurationDialog.NewStartID = startId;
                    Configurations.TryGetValue("EndID", out endId);
                    ConfigurationDialog.NewStopID = endId;
                    Summary.SetTitle("Parameters from timeConfig.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    Summary.SetLeftValues(startId, endId);
                    Summary.SetRightLabels(ConfigurationDialog.GetStartLabel(),
                        ConfigurationDialog.GetEndLabel());
                    break;
                case Mode.MANUAL_CONFIG:
                    Summary.BringToFront();
                    EditConfiguration.BringToFront();
                    openConfigFile.BringToFront();
                    configSaveBtn.BringToFront();
                    break;
                case Mode.AUTO_CONFIG:                  
                    Summary.SendToBack();
                    EditConfiguration.SendToBack();
                    openConfigFile.SendToBack();
                    configSaveBtn.SendToBack();
                    ConfigurationDialog.FixedStartEnd = true;
                    ConfigurationDialog.Show();                 
                    ConfigurationDialog.ShowFixedIDPanel();
                    AutoBtn.Enabled = false;
                    break;
                default:
                    Summary.SetTitle("Configure filter to adjust parameters.");
                    Summary.SetLeftLabels("Start ID:", "End ID:");
                    Summary.SetRightLabels("Start Time: n/a", "End Time: n/a");
                    Summary.SetLeftValues("n/a", "n/a");
                    break;
            }

            if (mode == Mode.INITIAL || mode == Mode.CONFIG)
            {
                return;
            }
            if (mode == Mode.RESET_TIME_FIRST_KEY || mode == Mode.KEEP_TIME_FIRST_KEY)
            {
                Summary.SetLeftValues("to be defined", "to be defined");
                Summary.SetRightLabels(string.Empty, string.Empty);
                return;
            }

            Summary.SetLeftValues(
                ConfigurationDialog.GetStartBox(),
                ConfigurationDialog.GetEndBox());

            Summary.SetRightLabels(
                ConfigurationDialog.GetStartLabel(),
                ConfigurationDialog.GetEndLabel());
        }

        /// <summary>
        /// Returns the event filter that is represented by this filter control.
        /// </summary>
        /// <returns>The event filter.</returns>
        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            // The author and his idfx
            var author = "";
            var idfxID = "";
            if (sessionID != null)
            {
                author = sessionID.SessionInfo[SessionIdentification.SESSION_PARTICIPANTKEY].ToLower();
                idfxID = Path.GetFileNameWithoutExtension(sessionID.MetaInfo[SessionIdentification.META_LOGFILE].ToLower());
            }
            if (ConfigurationDialog.FixedStartEnd)
            {
                author = "Fixed_Start_End_ID";
            }

            // Events preceding the Start ID or following the Stop ID are always replaced:
            // being the first and last event everything stays the same when they were unchanged. 
            var eventIDs = new List<string>
                           {
                               NAME,
                               ConfigurationDialog.NewStartID,
                               ConfigurationDialog.NewStopID,
                               ConfigurationDialog.ResetTime.ToString(),
                               ConfigurationDialog.EventValue,
                               author,
                               idfxID                            
                           };
            return new EventIDFilter(eventIDs);
        }

        /// <summary>
        /// Configuration parameter names of this filter.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string NEWSTART_ID = "StartID";
            public const string NEWSTOP_ID = "EndID";
            public const string RESET_TIME = "ResetTime";
            public const string AUTHOR = "Author";
            public const string IDFX_ID = "IdfxID";
        }

        /// <summary>
        /// Returns whether this process control can handle multiple files at the same
        /// time or can only process one time at a time.
        /// </summary>
        public override bool MultipleFileCompatible => true;

        /// <summary>
        /// Exports the configuration of this EventID Control to a FilterConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A FilterConfiguration that defines the configuration (= the input fields) of this filter control.</returns>
        public override FilterConfiguration Export(FilterExportOptions options)
        {
            var config = new FilterConfiguration(NAME);
            config.Parameters.Add(
                ConfigurationParameters.NEWSTART_ID,
                ConfigurationDialog.NewStartID);
            config.Parameters.Add(
                ConfigurationParameters.NEWSTOP_ID,
                ConfigurationDialog.NewStopID);
            config.Parameters.Add(
                ConfigurationParameters.RESET_TIME,
                ConfigurationDialog.ResetTime.ToString());
            config.Parameters.Add(
                ConfigurationParameters.AUTHOR, EventIDFilter.Author);
            config.Parameters.Add(
                ConfigurationParameters.IDFX_ID, EventIDFilter.IdfxID);
            return config;
        }

        /// <summary>
        /// Imports a configuration into this EventID Control from an FilterConfiguration.
        /// </summary>
        public override void Import(FilterConfiguration configuration)
        {
            configuration.TryGetParameter(ConfigurationParameters.NEWSTART_ID, "");
            configuration.TryGetParameter(ConfigurationParameters.NEWSTOP_ID, "");
            configuration.TryGetParameter(ConfigurationParameters.RESET_TIME, "");
        }

        /// <summary>
        /// Show the time_configuration dialog form to specify the parameters
        /// of the time filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditConfigurationButtonClick(object sender, EventArgs e)
        {
            if (FilePaths == null || FilePaths.Count < 1) return;
            ConfigurationDialog.ShowVariableIDPanel();
            if (ConfigurationDialog.FixedStartEnd)
            {
                 LoadConfiguration(Mode.KEEP_TIME_FIRST_KEY);
            }
            if (ConfigurationDialog.ShowDialog() == DialogResult.OK)
                LoadConfiguration(CurrentMode());
        }

        /// <summary>
        /// Callback method for when the file selection has been changed.
        /// </summary>
        /// <param name="filePaths"></param>
        public override void OnFileSelectionChange(List<string> filePaths)
        {
            base.OnFileSelectionChange(filePaths);
            ConfigurationDialog.OnSelectedFilesChanged();

            if (!ConfigurationDialog.FixedStartEnd)
            {
                ConfigurationDialog.ReadEvents(filePaths[0]);
            }

            LoadConfiguration(Mode.INITIAL);
        }

        /// <summary>
        /// Reading Time filter parameter settings from disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConfigFileClick(object sender, EventArgs e)
        {
            if (configFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                Configurations = new Dictionary<string, string>();
                using (var reader = new StreamReader(configFileDialog.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        if (!line.Contains(",")) continue;
                        var data = line.Split(',');
                        var key = data[0];
                        var value = data[1];
                        Configurations.Add(key, value);
                    }
                }

                LoadConfiguration(Mode.CONFIG);
            }
            catch (IOException ioException)
            {
                MessageLogger.CatchException(this, ioException, Severity.ERROR);
            }
        }

        /// <summary>
        /// Writing Time filter parameter settings to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSaveBtnClick(object sender, EventArgs e)
        {
            if (saveConfigDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                var configuration = Export(null);
                using (var writer = new StreamWriter(saveConfigDialog.FileName))
                {
                    writer.WriteLine(configuration.FilterName);
                    foreach (var param in configuration.Parameters)
                    {
                        writer.WriteLine("{0},{1}", param.Key, param.Value);
                    }
                }
            }
            catch (IOException ioException)
            {
                MessageLogger.CatchException(this, ioException, Severity.ERROR);
            }
        }
    }
}