using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.AnalysesControls.Fluency;
using GUI.Util;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using CoreSettings = InputLog.Core.Util.Settings;

namespace GUI.Settings.Tabs
{
    /// <summary>
    /// Tab containing settings for the Analyses.
    /// </summary>
    public partial class Analyses : BaseSettingsTab
    {
        /// <summary>
        /// Constant indicating that the double click threshold has not been set yet.
        /// </summary>
        private const long UNSET_DOUBLECLICK_THRESHOLD = -1;

        /// <summary>
        /// Constructs the Analyses Settings tab.
        /// </summary>
        public Analyses()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loading Event-Callback.
        /// Populates the fields of the tab with the previously saved settings.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AnalysesLoad(object sender, EventArgs e)
        {
            ReloadSettings();
        }

        /// <summary>
        /// Reload the new settings.
        /// </summary>
        public override void ReloadSettings()
        {
            RecognizeDoubleClicksCheckbox.Checked = Settings.Analysis_RecognizeDoubleClicks;
            if (Settings.Analysis_DoubleClickThreshold == UNSET_DOUBLECLICK_THRESHOLD)
            {
                // set the double click threshold to the system's threshold if it has not been set yet.
                RecognizeDoubleClicksUpDown.Value = SystemInformation.DoubleClickTime;
            }
            else
            {
                RecognizeDoubleClicksUpDown.Value = Settings.Analysis_DoubleClickThreshold;
            }
            RecognizeDoubleClicksUpPanel.Enabled = Settings.Analysis_RecognizeDoubleClicks;
            // use thread to fill lists (otherwise, there is a noticable slowdown)
            Invoke((MethodInvoker)delegate
            {
                GroupedKeys.Enabled = false;
                AvailableKeys.Enabled = false;
                // first populate the grouped key list, since the available key list will see which keys are already present in the 
                // grouped key list (those are no longer displayed in the available key list).
                PopulateGroupedKeysList();
                PopulateAvailableKeysList();
                GroupedKeys.Enabled = true;
                AvailableKeys.Enabled = true;
                disablePersMax.Checked = !Settings.StorePersonalMaxima;
            });

            //Fluency analysis settings
            personalMax.Value = Convert.ToInt32(RegistryTools.GetSetting(RegistryTools.DEFAULT_APP_NAME, 
                "PersonalFluencyMaximumm", FluencyAnalysis.DEFAULT_ABSOLUTE_MAXIMUM-20));
            absoluteMax.Value = Convert.ToInt32(RegistryTools.GetSetting(RegistryTools.DEFAULT_APP_NAME, 
                "AbsoluteFluencyMaximum", FluencyAnalysis.DEFAULT_ABSOLUTE_MAXIMUM));
            //absoluteOpt.Enabled = false;
        }

        /// <summary>
        /// Populates the grouped keys list with the keys store in the settings.
        /// </summary>
        private void PopulateGroupedKeysList()
        {
            GroupedKeys.Items.Clear();
            if (Settings.GroupedKeysList != null)
            {
                foreach (var key in SettingsManipulation.DeserializeGroupedKeyList(Settings.GroupedKeysList))
                {
                    GroupedKeys.Items.Add(new KeyValuePair<string, KeysEx>(EventToString.KeyToString(key), key));
                }
            }
        }

        /// <summary>
        /// Populates the Available keys list with a set of predefined keys.
        /// If one of these keys is already present in the groupedkeyslist, it won't be added to the available keys list anymore.
        /// </summary>
        private void PopulateAvailableKeysList()
        {
            AvailableKeys.Items.Clear();
            var keysEx = Enum.GetValues(typeof(KeysEx));

            var controlKeys = new List<KeysEx>(new[] {
				KeysEx.VK_LSHIFT, KeysEx.VK_LCONTROL, KeysEx.VK_RSHIFT, 
				KeysEx.VK_RCONTROL, KeysEx.VK_SHIFT, KeysEx.VK_CONTROL,
				KeysEx.VK_LMENU, KeysEx.VK_MENU, KeysEx.VK_LEFT, KeysEx.VK_RIGHT, 
                KeysEx.VK_UP, KeysEx.VK_DOWN, KeysEx.VK_HOME, KeysEx.VK_END, KeysEx.VK_PAUSE,
                KeysEx.VK_ESCAPE, KeysEx.VK_PRINT, KeysEx.VK_NUMLOCK, KeysEx.VK_SCROLL, KeysEx.VK_CAPITAL });
            //VK_CAPITAL = CAPS LOCK

            var specialKeys = new List<KeysEx>(new[] {
				KeysEx.VK_TAB, KeysEx.VK_RETURN, KeysEx.VK_SPACE, KeysEx.VK_BACK, KeysEx.VK_DELETE});

            foreach (KeysEx key in keysEx)
            {
                var listItem = new KeyValuePair<string,KeysEx>(EventToString.KeyToString(key), key);
                if (GroupedKeys.Items.Contains(listItem))
                {
                    continue;
                }
                if (controlKeys.Contains(key) || specialKeys.Contains(key) || IsSupportedKey(key))
                {
                    AvailableKeys.Items.Add(listItem);
                }
            }
        }

        /// <summary>
        /// Checks whether a given key is a supported key or not.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>true if the key is supported, false otherwise.</returns>
        private static bool IsSupportedKey(KeysEx key)
        {
            var name = key.ToString().Replace("VK_", "");
            if (name.Length == 1)
            {
                return true;
            }
            if (name.StartsWith("NUMPAD"))
            {
                return true;
            }
            return name.StartsWith("F") && name.Length <= 3;
        }

        /// <summary>
        /// Applies the new settings.
        /// </summary>
        public override void ApplySettings()
        {
            var keys = (from KeyValuePair<string,KeysEx> item in GroupedKeys.Items select item.Value).ToList();
            Settings.GroupedKeysList = SettingsManipulation.SerializeGroupedKeyList(keys);
            if (RecognizeDoubleClicksCheckbox.Checked)
            {
                CoreSettings.Analysis.DoubleClickThreshold = (ulong)RecognizeDoubleClicksUpDown.Value;
                Settings.Analysis_DoubleClickThreshold = (long)RecognizeDoubleClicksUpDown.Value;
            }
            else
            {
                CoreSettings.Analysis.DoubleClickThreshold = 0;
            }

            Settings.Analysis_RecognizeDoubleClicks = RecognizeDoubleClicksCheckbox.Checked;

            Settings.StorePersonalMaxima = !disablePersMax.Checked;

            // Fluency analysis settings
            RegistryTools.SaveSetting(RegistryTools.DEFAULT_APP_NAME, "PersonalFluencyMaximum", personalMax.Value.ToString());
            RegistryTools.SaveSetting(RegistryTools.DEFAULT_APP_NAME, "AbsoluteFluencyMaximum", absoluteMax.Value.ToString());
        }

        /// <summary>
        /// Event callback for the click event of the AvailableToGroupedButton.
        /// Removes the selected key from the available keys list and adds it to the grouped keys list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AvailableToGroupedButtonClick(object sender, EventArgs e)
        {
            if (AvailableKeys.SelectedItem == null) return;
            GroupedKeys.Items.Add(AvailableKeys.SelectedItem);
            var removedIndex = AvailableKeys.SelectedIndex;
            AvailableKeys.Items.RemoveAt(removedIndex);
            AvailableKeys.SelectedIndex = Math.Min(AvailableKeys.Items.Count - 1, removedIndex);
        }
        /// <summary>
        /// Event callback for the click event of the GroupAllBtn.
        /// Removes all keys from the available keys list and adds it to the grouped keys list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void GroupAllButtonClick(object sender, EventArgs e)
        {
            if (AvailableKeys.Items.Count == 0) return;
            GroupedKeys.Items.AddRange(AvailableKeys.Items);
            AvailableKeys.Items.Clear();
        }

        /// <summary>
        /// Event callback for the click event of the RemoveAllBtn.
        /// Removes all keys from the grouped keys list and adds it to the available keys list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RemoveAllButtonClick(object sender, EventArgs e)
        {
            if (GroupedKeys.Items.Count == 0) return;
            AvailableKeys.Items.AddRange(GroupedKeys.Items);
            GroupedKeys.Items.Clear();
        }

        /// <summary>
        /// Event callback for the click event of the GroupedToAvailableButton.
        /// Removes the selected key from the grouped keys list and adds it to the available keys list.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void GroupedToAvailableButtonClick(object sender, EventArgs e)
        {
            if (GroupedKeys.SelectedItem == null) return;
            AvailableKeys.Items.Add(GroupedKeys.SelectedItem);
            var removedIndex = GroupedKeys.SelectedIndex;
            GroupedKeys.Items.RemoveAt(removedIndex);
            GroupedKeys.SelectedIndex = Math.Min(GroupedKeys.Items.Count - 1, removedIndex);
        }

        /// <summary>
        /// Event callback for the click event of the RecognizeDoubleClicksCheckbox.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RecognizeDoubleClicksCheckboxCheckedChanged(object sender, EventArgs e)
        {
            RecognizeDoubleClicksUpPanel.Enabled = RecognizeDoubleClicksCheckbox.Checked;
        }

        private void resetPersOptima_Click(object sender, EventArgs e)
        {
            string s = FluencyAnalyzer.GetPersonalMaximaMatrixDir();
            File.WriteAllText(s, "");
        }

    }
}