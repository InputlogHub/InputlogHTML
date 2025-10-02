using InputLog.Core.Plugin.WinLog;
using InputLog.Core.Hooks.Keyboard;
using CoreSettings = InputLog.Core.Util.Settings;

namespace GUI.Tabs.Record.Plugin.WinLog
{
    /// <summary>
    /// Implementation for plugins and holder of the PluginOptions.
    /// </summary>
    public class WinLog : AbstractPlugin
    {
        #region Fields

        /// <summary>
        /// The logger responsible for the focus events.
        /// </summary>
        private readonly FocusLogger _focusLogger = new FocusLogger();

        /// <summary>
        /// The logger responsible for the keyboard events.
        /// </summary>
        private readonly KeyboardLogger _keyLogger = new KeyboardLogger();

        /// <summary>
        /// The logger responsible for the mouse events.
        /// </summary>
        private readonly MouseLogger _mouseLogger = new MouseLogger();

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings"></param>
        public WinLog(RecordSettings settings)
            : base(new PluginOptions(), settings) { }

        /// <summary>
        /// Validates the configuration entered in the GUI, returns true if everything is OK, false if not.
        /// </summary>
        /// <returns>True if the entered configuration is OK, false if there are conflicts, invalid information, ...
        /// and the plugin will not be able to start/function properly.</returns>
        public override bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Launches the WinLog plugin.
        /// </summary>
        protected override void InternalLaunch()
        {
            KeyboardEvent.HighPrecision = RecordSettings.HighPrecision;
            if (RecordSettings.HookFocus)
            {
                _focusLogger.Running = true;
                _focusLogger.Reset();
            }
            if (RecordSettings.HookKeyboard)
                _keyLogger.Running = true;
            if (RecordSettings.HookMouse)
                _mouseLogger.Running = true;
            if (!CoreSettings.WinLogRestricted) return;
            _focusLogger.IsRestricted = true;
            _keyLogger.IsRestricted = true;
            _mouseLogger.IsRestricted = true;
        }

        /// <summary>
        /// Terminates the WinLogger.
        /// </summary>
        protected override void InternalTerminate()
        {
            _focusLogger.Running = false;
            _keyLogger.Running = false;
            _mouseLogger.Running = false;
            _focusLogger.IsRestricted = false;
            _keyLogger.IsRestricted = false;
            _mouseLogger.IsRestricted = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources
        /// should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            _focusLogger.Dispose();
            _keyLogger.Dispose();
            _mouseLogger.Dispose();
        }
    }
}