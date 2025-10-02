using System;
using System.Windows.Forms;
using GUI.Tabs.Preprocess;
using CoreSettings = InputLog.Core.Util.Settings;

namespace GUI.Settings.Tabs
{
    /// <summary>
    /// Tab containing general settings for Inputlog.
    /// </summary>
    public partial class Logging : BaseSettingsTab
    {
        /// <summary>
        /// Constructs the Logging settings tab.
        /// </summary>
        public Logging()
        {
            InitializeComponent();

            keyDelimiters.DataSource = new BindingSource(PreprocessModel.Keys, null);
            keyDelimiters.DisplayMember = "Key";
            keyDelimiters.ValueMember = "Value";
            keyDelimitersCBX.Checked = false;
            UserActionDB.Items.Insert(0, "");
            UserActionDB.Items.Add("esc");
            //UserActionDB.Items.Add("Ctrl+C");
            //UserActionDB.Items.Add("Ctrl+V");
            //UserActionDB.Items.Add("Ctrl+P");
            
        }

        /// <summary>
        /// Loading Event-Callback.
        /// Populates the fields of the tab with the previously saved settings.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LoggingLoad(object sender, EventArgs e)
        {
            ReloadSettings();
        }

        /// <summary>
        /// Callback for the SeperateMouseMovementsCheckbox button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SeperateMouseMovementsCheckboxCheckedChanged(object sender, EventArgs e)
        {
            MouseMovementPauseThresholdPanel.Enabled = SeperateMouseMovementsCheckbox.Checked;
        }

        private void KeyDelimitersSelectedIndexChanged(object sender, EventArgs e)
        {
            keyDelimitersCBX.Checked = true;
        }

        /// <summary>
        /// Interval setting for saving the current document in the background.
        /// It's either time based or action based (key presses) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimebaseIntervalChanged(object sender, EventArgs e)
        {
            if (TimeIntervalUD.Value > 0)
            {
                UserActionDB.SelectedItem = UserActionDB.Items[0];
            }
        }
        private void ActionIntervalChanged(object sender, EventArgs e)
        {
            if (!UserActionDB.GetItemText(UserActionDB.SelectedItem).Equals(string.Empty))
            {
                TimeIntervalUD.Value = 0;
            }       
        }

        /// <summary>
        /// Applies the new settings.
        /// </summary>
        public override void ApplySettings()
        {
            CoreSettings.WordLogOverrideDoc = WLOverride.Checked;
            CoreSettings.WordLogDisableWordAddins = WLDisableAddins.Checked;
            CoreSettings.StopTimeout = (int) TimOutUpDown.Value;
            CoreSettings.WinLogRestricted = WLRestrictedLogging.Checked;
            CoreSettings.Segmentation_IncludeInitialPause = segmentInitialPauseCBX.Checked;
            CoreSettings.Segmentation_KeyDelimiterActive = keyDelimitersCBX.Checked;
            CoreSettings.TimebasedIntervalSave = (ulong) TimeIntervalUD.Value;
            CoreSettings.UserActionIntervalSave = UserActionDB.GetItemText(UserActionDB.SelectedItem);
            CoreSettings.AddComment = AuthorCommentCBX.Checked;

            // Selecting the keyboard characters used to segment the idfx.
            if (CoreSettings.Segmentation_KeyDelimiterActive)
            {
                if (keyDelimiters.SelectedValue != null)
                {
                    CoreSettings.Segmentation_KeyDelimiter = keyDelimiters.SelectedValue.ToString();
                }
                else
                {
                    CoreSettings.Segmentation_KeyDelimiterActive = false;
                }
            }
            else
            {
                CoreSettings.Segmentation_KeyDelimiter = "";
            }

            // Time based interval setting
            if (CoreSettings.TimebasedIntervalSave > 0)
            {
                UserActionDB.SelectedItem = UserActionDB.Items[0];
            }

            // User action interval setting
            if (!UserActionDB.GetItemText(UserActionDB.SelectedItem).Equals(string.Empty))
            {
                CoreSettings.TimebasedIntervalSave = 0;
            }

            if (SeperateMouseMovementsCheckbox.Checked)
            {
                CoreSettings.WinLogMouseMovementPauseThreshold = (ulong) MouseMovementPauseThresholdUpDown.Value;
                Settings.MouseMovementPauseThreshold = CoreSettings.WinLogMouseMovementPauseThreshold;
            }
            else
            {
                CoreSettings.WinLogMouseMovementPauseThreshold = 0;
            }
            Settings.SeperateMouseMovements = SeperateMouseMovementsCheckbox.Checked;

            Settings.AutoHide = AutoHide.Checked;
            Settings.HighPrecision = HighPrecision.Checked;

        }

        /// <summary>
        /// Reload the new settings.
        /// </summary>
        public override void ReloadSettings()
        {
            WLOverride.Checked = CoreSettings.WordLogOverrideDoc;
            WLDisableAddins.Checked = CoreSettings.WordLogDisableWordAddins;
            WLRestrictedLogging.Checked = CoreSettings.WinLogRestricted;
            TimOutUpDown.Value = CoreSettings.StopTimeout;
            SeperateMouseMovementsCheckbox.Checked = Settings.SeperateMouseMovements;
            MouseMovementPauseThresholdUpDown.Value = Settings.MouseMovementPauseThreshold;
            AutoHide.Checked = Settings.AutoHide;
            HighPrecision.Checked = Settings.HighPrecision;
            segmentInitialPauseCBX.Checked = CoreSettings.Segmentation_IncludeInitialPause;
            keyDelimiters.SelectedValue = CoreSettings.Segmentation_KeyDelimiter;
            keyDelimitersCBX.Checked = CoreSettings.Segmentation_KeyDelimiterActive;
            TimeIntervalUD.Value = CoreSettings.TimebasedIntervalSave;
            UserActionDB.SelectedItem = CoreSettings.UserActionIntervalSave;
            AuthorCommentCBX.Checked = CoreSettings.AddComment;
        }
    }
}