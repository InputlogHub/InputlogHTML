using System;
using InputLog.Core.IO;

namespace GUI.Tabs.Record.Plugin
{
    /// <summary>
    /// Base class for the plugins.
    /// </summary>
    public abstract class AbstractPlugin : IDisposable
    {
        #region Fields

        /// <summary>
        /// Options panel that is to be shown when the plugin is selected from the list.
        /// </summary>
        public PluginOptions Options { get; private set; }

        /// <summary>
        /// True if the plugin is running, false if not.
        /// </summary>
        private bool Running { get; set; }

        /// <summary>
        /// Reference to the recordsettings for extra config.
        /// </summary>
        protected RecordSettings RecordSettings { get; private set; }

        /// <summary>
        /// SettingsWindow instance containing the options entered in the options menu.
        /// </summary>
        protected Properties.Settings Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        private bool Disposed { get; set; }

        /// <summary>
        /// Output directory of logged idfx
        /// </summary>
        protected string OutputDir { get; private set; }

        /// <summary>
        /// Meta data about the session (age of participant, ...).
        /// </summary>
        protected SessionIdentification SessionID { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Panel containing the options of the Plugin.</param>
        /// <param name="settings"></param>
        protected AbstractPlugin(PluginOptions options, RecordSettings settings)
        {
            Options = options;
            RecordSettings = settings;
            Disposed = false;
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this LinearAnalysisType implements a finalizer.
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                Terminate();
                Options.Dispose();
                Options = null;
            }

            Disposed = true;
        }

        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// Do not provide any destructors in types derived from this LinearAnalysisType.
        /// </summary>
        ~AbstractPlugin()
        {
            Dispose(false);
        }

        /// <summary>
        /// Launches the plugin.
        /// </summary>
        /// 
        public void Launch(string outputDir, SessionIdentification session)
        {
            OutputDir = outputDir;
            SessionID = session;
            if (Running) return;
            InternalLaunch();
            Running = true;
        }

        /// <summary>
        /// Terminates the plugin.
        /// </summary>
        public void Terminate()
        {
            if (!Running) return;
            InternalTerminate();
            Running = false;
        }

        /// <summary>
        /// Validates the configuration entered in the GUI, returns true if everything is OK, false if not.
        /// </summary>
        /// <returns>True if the entered configuration is OK, false if there are conflicts, invalid information, ...
        /// and the plugin will not be able to start/function properly.</returns>
        public abstract bool Validate();

        /// <summary>
        /// Launches the plugin.
        /// </summary>
        protected abstract void InternalLaunch();

        /// <summary>
        /// Terminates the plugin.
        /// </summary>
        protected abstract void InternalTerminate();
    }
}