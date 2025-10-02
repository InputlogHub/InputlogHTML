using System;

namespace InputLog.Core.Plugin
{
    /// <summary>
    /// Abstract base class for all plugins.
    /// </summary>
    public abstract class AbstractPlugin : IDisposable
    {
        #region Fields

        private bool _isRunning;

        /// <summary>
        /// Reference to System MessageLogger.
        /// </summary>
        protected static SystemLogger SysLog => SystemLogger.SysLog;

        /// <summary>
        /// Returns true if the plugin is running, false if it is not.
        /// Setting this value will change start or stop the plugin if
        /// the new state differs from the old.
        /// (Flipping it from false to true will start it and flipping
        /// it back will stop it. Setting it to true while it was already
        /// true will have no effect and the same goes when setting it to
        /// false if it was already false.)
        /// </summary>
        public bool Running
        {
            private get { return _isRunning; }
            set
            {
                if (_isRunning == value) return;
                // Only do stuff if state changes
                _isRunning = value; // Adjust state
                if (value)
                {
                    // Start or stop, depending on new value
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        private bool Disposed { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractPlugin()
        {
            Disposed = false;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        private void Dispose(bool disposing)
        {
            Running = false; // Stop running
            Disposed = true;
        }

        /// <summary>
        /// Starts the plugin.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Stops the plugin.
        /// </summary>
        protected abstract void Stop();

        /// <summary>
        /// Destructor.
        /// </summary>
        ~AbstractPlugin()
        {
            Dispose(false);
        }

        /// <summary>
        /// Checks if a change to the settings of the plugin is allowed or not.
        /// If no change is allowed, it throws a ChangeNotAllowedException.
        /// </summary>
        /// <param name="msg">A message to append to the default message.</param>
        protected void ChangeAllowed(string msg = "")
        {
            if (Running)
            {
                throw new ChangeNotAllowedException(msg);
            }
        }
    }
}