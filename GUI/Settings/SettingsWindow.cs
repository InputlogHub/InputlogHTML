using System;
using System.IO;
using System.Windows.Forms;
using CoreSettings = InputLog.Core.Util.Settings;

namespace GUI.Settings
{
    /// <summary>
    /// Form showing different settings the user can change (and persist).
    /// </summary>
    public partial class SettingsWindow : Form
    {
        /// <summary>
        /// SettingsWindow class to save the settings to.
        /// </summary>
        private readonly Properties.Settings settings = Properties.Settings.Default;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsWindow(Gui gui)
        {
            InitializeComponent();

            GeneralSettings.GUI = gui;
            LoggingSettings.GUI = gui;
            AnalysesSettings.GUI = gui;
        }

        /// <summary>
        /// Callback for the OK button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OKButtonClick(object sender, EventArgs e)
        {
            Tabs.Enabled = false;
            StatusLabel.Text = @"Saving...";
            GeneralSettings.ApplySettings();
            LoggingSettings.ApplySettings();
            AnalysesSettings.ApplySettings();
            StatusLabel.Text = @"Done";

            CoreSettings.Save();
            settings.Save();
            Close();
        }

        /// <summary>
        /// Callback for the Cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Callback for the ResetToDefault button.
        /// Shows a confirmation dialog. If confirmed, all settings are reset.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ResetToDefaultButtonClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                @"Are you sure you wish to reset all settings to the default settings?", 
                @"Are you sure you want to reset?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // use thread to make sure UI does not freeze (not entirely fixed yet).
                StatusLabel.Text = @"Resetting...";
                StatusLabel.Refresh(); // needed, otherwise the text is not shown
                Invoke((MethodInvoker) delegate
                                           {
                                               Tabs.Enabled = false;
                                               ButtonPanel.Enabled = false;
                                               settings.Reset();
                                               CoreSettings.Reset();
                                               settings.Workspace = Path.Combine(Environment.GetFolderPath
                                                   (Environment.SpecialFolder.MyDocuments),"InputLog");
                                               settings.Save();
                                               CoreSettings.Save();
                                               GeneralSettings.ReloadSettings();
                                               LoggingSettings.ReloadSettings();
                                               AnalysesSettings.ReloadSettings();
                                               ButtonPanel.Enabled = true;
                                               Tabs.Enabled = true;
                                               StatusLabel.Text = @"Done";
                                           });
            }
        }
    }
}