using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using GUI.Tabs.Record;

namespace GUI.Settings.Tabs
{
    /// <summary>
    /// Tab containing general settings for Inputlog.
    /// </summary>
    public partial class General : BaseSettingsTab
    {
        /// <summary>
        /// Constructs the General settings tab.
        /// </summary>
        public General()
        {
            InitializeComponent();
            ReloadSettings();
        }

        /// <summary>
        /// Loading Event-Callback.
        /// Populates the fields of the tab with the previously saved settings.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void GeneralLoad(object sender, EventArgs e)
        {
            ReloadSettings();
        }

        /// <summary>
        /// Callback for the Workspace Browse button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WorkspaceBrowseButtonClick(object sender, EventArgs e)
        {
            WorkspaceBrowserDialog.SelectedPath = WorkspaceTextField.Text;

            if (WorkspaceBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                WorkspaceTextField.Text = WorkspaceBrowserDialog.SelectedPath;
            }
        }

        private static string CheckedIndexesToString(IEnumerable cic)
        {
            var sb = new StringBuilder();
            bool notFirst = false;
            foreach (int i in cic)
            {
                if (notFirst)
                {
                    sb.Append(",");
                }
                else
                {
                    notFirst = true;
                }
                sb.Append(i.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Applies the new settings.
        /// </summary>
        public override void ApplySettings()
        {
            var reloadSession = !Settings.Workspace.Equals(WorkspaceTextField.Text);
            if (reloadSession)
            {
                GUI.SaveRecordSession();
            }

            Settings.Workspace = WorkspaceTextField.Text;
            Settings.Record_EventSelection = CheckedIndexesToString(RecordHookSelectionBox.CheckedIndices);
            Settings.Record_PluginSelection = CheckedIndexesToString(RecordPluginSelectionBox.CheckedIndices);
            //Settings.Record_LogFormat = RecordLoggingFormatList.SelectedIndex;  
            Settings.Save();

            if (reloadSession)
            {
                GUI.UpdateRecordSession();
            }
        }

        /// <summary>
        /// Reload the new settings.
        /// </summary>
        public sealed override void ReloadSettings()
        {
            WorkspaceTextField.Text = Settings.Workspace;
            var settings = new RecordSettings();
            foreach (int i in settings.PluginSelection)
            {
                RecordPluginSelectionBox.SetItemChecked(i, true);
            }
            foreach (int i in settings.HookSelection)
            {
                RecordHookSelectionBox.SetItemChecked(i, true);
            }
            RecordLoggingFormatList.SelectedIndex = settings.LoggingFormatID;
        }
    }
}