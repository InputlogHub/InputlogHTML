using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GUI.Tabs.Record.Plugin;
using GUI.Tabs.Record.Plugin.WinLog;
using GUI.Tabs.Record.Plugin.WordLog;
using InputLog.Core.IO.Basic;

namespace GUI.Tabs.Record
{
    public class RecordSettings
    {
        /// <summary>
        /// Array with known plugins.
        /// </summary>
        public AbstractPlugin[] Plugins;

        /// <summary>
        /// Different log formats.
        /// </summary>
        private readonly string[] _formats = { LogFormat.XML, LogFormat.TXT };

        public int LoggingFormatID;

        /// <summary>
        /// Format of the output file.
        /// </summary>
        public string LoggingFormat
        {
            get => _formats[LoggingFormatID];
            set { for (int i = 0; i < _formats.Length; i++) if (value.Equals(_formats[i])) LoggingFormatID = i; }
        }

        /// <summary>
        /// Returns true if focus events should be logged.
        /// </summary>
        public bool HookFocus
        {
            get => HookSelection.Contains(0);
            set { if (value) HookSelection.Add(0); else HookSelection.Remove(0); }
        }

        /// <summary>
        /// Returns true if keyboard events should be logged.
        /// </summary>
        public bool HookKeyboard
        {
            get => HookSelection.Contains(1);
            set { if (value) HookSelection.Add(1); else HookSelection.Remove(1); }
        }

        /// <summary>
        /// Returns true if mouse events should be logged.
        /// </summary>
        public bool HookMouse
        {
            get => HookSelection.Contains(2);
            set { if (value) HookSelection.Add(2); else HookSelection.Remove(2); }
        }

        public readonly HashSet<int> HookSelection = new HashSet<int>();
        public readonly HashSet<int> PluginSelection = new HashSet<int>();

        /// <summary>
        ///  Returns true if high precision event timing should be used.
        /// </summary>
        public readonly bool HighPrecision;

        public RecordSettings(Record rec) : this()
        {
            Plugins = new AbstractPlugin[] { new WinLog(this), new WordLogFrontend(this) };
        }

        public void RegisterPlugins(AbstractPlugin[] plugs)
        {
            Plugins = plugs;
        }

        public RecordSettings()
        {
            try
            {
                string[] splits = Properties.Settings.Default.Record_EventSelection.Split(',');
                if (splits.Length > 0)
                {
                    foreach (string s in splits)
                    {
                        HookSelection.Add(int.Parse(s));
                    }
                }

                splits = Properties.Settings.Default.Record_PluginSelection.Split(',');
                if (splits.Length > 0)
                {
                    foreach (string s in splits)
                    {
                        PluginSelection.Add(int.Parse(s));
                    }
                }

                HighPrecision = Properties.Settings.Default.HighPrecision;
                LoggingFormatID = Properties.Settings.Default.Record_LogFormat;
            }
            catch (FormatException)
            {
                MessageBox.Show("An exception occured while loading application settings. " +
                                "Please visit File > Options > General and check your settings.");
            }
        }
    }
}
