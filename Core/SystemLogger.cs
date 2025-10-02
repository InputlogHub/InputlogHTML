using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using InputLog.Core.Events;
using InputLog.Core.Hooks;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Hooks.Mouse;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using EventLog = InputLog.Core.IO.Basic.Output.EventLogWriter;

namespace InputLog.Core
{
    #region Delegates

    /// <summary>
    /// Delegate for keyboard event callbacks.
    /// </summary>
    /// <param name="sender">The originator of the event.</param>
    /// <param name="e">The data of the event, mostly a wrapper around the information provided by Windows.</param>
    /// <param name="unicode">The unicode representation of the key (if there is one), taking into account
    /// previously pressed dead keys. (A maximum of one dead key is taken into account, ie chaining
    /// of dead keys is not supported.)</param>
    /// <param name="alias">The alias used for referencing the event in the Log.</param>
    /// <param name="kbState">Enumeration containing all the keyboard buttons currently pressed - excluding the button
    /// which lead to the callback.</param>
    public delegate void KeyboardDelegate(object sender, KeyboardEvent e, string unicode,
                                          string alias, ICollection<KeysEx> kbState);

    /// <summary>
    /// Delegate for mouse event callbacks.
    /// </summary>
    /// <param name="sender">The originator of the event.</param>
    /// <param name="e">The data of the event, mostly a wrapper around the information provided by Windows.</param>
    /// <param name="alias">The alias used for referencing the event in the Log.</param>
    /// <param name="kbState">Enumeration containing all the keyboard buttons currently pressed - excluding the button
    /// which lead to the callback.</param>
    public delegate void MouseDelegate(object sender, MouseEvent e, string alias, ICollection<KeysEx> kbState);

    #endregion

    /// <summary>
    /// Class that logs the different eventz it receives.
    /// Before any events are given to it, it should be activated.
    /// </summary>
    public class SystemLogger : IDisposable
    {
        #region Fields
        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly SystemLogger Instance = new SystemLogger();

        /// <summary>
        /// This will convert the pressed key into its Unicode value
        /// (taking up to one previously entered dead key into account).
        /// </summary>
        private readonly KeyConverter _converter = new KeyConverter();

        /// <summary>
        /// Contains the keys currently pressed and not yet released and maps them to their alias
        /// and their event part, waiting for their endtime to be set.
        /// </summary>
        private readonly HashSet<KeysEx> _keyboardState = new HashSet<KeysEx>();

        /// <summary>
        /// Used for creating the EventLog to which received eventz will be pushed.
        /// </summary>
        public readonly EventLogFactory LogFactory = new EventLogFactory();

        /// <summary>
        /// Maps each alias to its number of current processors.
        /// Whenever EndEvent is called, which decreases this number by one, and the
        /// number of current processors reach 0, the event will be closed in the Log.
        /// </summary>
        private readonly IDictionary<string, int> _processors = new Dictionary<string, int>();

        /// <summary>
        /// Counts the eventz added by AddEvent in order to keep the generated aliases unique.
        /// Whenever the callback is finished processing the eventz, it should call EndEvent on Log
        /// so the event is closed if all the different callbacks are called and have finished processing.
        /// </summary>
        private ulong _anonymousEventCounter;

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The log to which the received eventz will be pushed, null if the SystemLogger isn't active.
        /// </summary>
        private EventLog _log;

        /// <summary>
        /// Reference to the single instance of this class.
        /// </summary>
        public static SystemLogger SysLog => Instance;

        /// <summary>
        /// Event triggered whenever a keyboard event is received.
        /// Whenever the callback is finished processing the event, it should call EndEvent
        /// so the SystemLogger knows the callback is finished.
        /// </summary>
        public event KeyboardDelegate KeyboardEvent;

        /// <summary>
        /// Event triggered whenever a mouse event is received.
        /// Whenever the callback is finished processing the event, it should call EndEvent
        /// so the SystemLogger knows the callback is finished.
        /// </summary>
        public event MouseDelegate MouseEvent;

        private SessionIdentification _sessionId;

        /// <summary>
        /// Blocks keylogging outside the main document.
        /// </summary>
        private bool _allowKeyLogging = true;
        #endregion

        /// <summary>
        /// Constructor, constructs an inactive SystemLogger.
        /// Private due to singleton.
        /// </summary>
        private SystemLogger()
        {

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
        /// Creates a sessionIdentification for the SystemLogger and starts the logging.
        /// </summary>
        /// <param name="path">Path to the file where the output will be saved.</param>
        /// <param name="format">Format of the output.</param>
        /// <param name="sessionID">Contains the session identifaction data.</param>
        /// <param name="keyboard">True if keyboard eventz should be logged, false if not.</param>
        /// <param name="mouse">True if mouse eventz should be logged, false if not.</param>
        public void Start(string path, string format, SessionIdentification sessionID, bool keyboard, 
            bool mouse)
        {
            try
            {
                Debug.Assert(_log == null, "Start cannot be called twice.");
                _sessionId = sessionID;               
                _log = EventLogFactory.CreateFileEventLogWriter(path, format);
                _log.Start(_sessionId);
                InstallHooks(keyboard, mouse);
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Stops the logging.
        /// </summary>
        public void Stop()
        {
            try
            {
                Debug.Assert(_log != null, "Stop cannot be called if the SystemLogger is not started first.");

                // First uninstall hooks, then stop Log (this way no eventz are received after Log has been terminated).
                SystemMonitor.KeyboardEvent -= KeyboardCallback;
                SystemMonitor.MouseEvent -= MouseCallback;
                SystemMonitor.Uninstall();
                _log.Stop();
                _log = null; // Assign null so it is clear we are in the inactive state
                // Removing the creation time of the current session at the end of a 
                // session in order to solve a problem with the pause time calculation.
                _sessionId.RemoveCreationDate();
                // Clean up state
                lock (_keyboardState)
                {
                    _keyboardState.Clear();
                }
                
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Installs the hooks.
        /// </summary>
        /// <param name="keyboard">True if keyboard eventz should be logged, false if not.</param>
        /// <param name="mouse">True if mouse eventz should be logged, false if not.</param>
        private void InstallHooks(bool keyboard, bool mouse)
        {
            SystemMonitor.Install();
            if (keyboard)
            {
                SystemMonitor.KeyboardEvent += KeyboardCallback;
            }
            if (mouse)
            {
                SystemMonitor.MouseEvent += MouseCallback;
            }
        }

        /// <summary>
        /// Adds an event to the log and returns the alias of the newly created event.
        /// The event will be closed if EndEvent is called once.
        /// </summary>
        /// <param name="type">The value of the LinearAnalysisType property of the event
        ///  (or null if it does not need to be set).</param>
        /// <returns>The alias of the newly created event.</returns>
        public string StartEvent(string type)
        {
            Debug.Assert(_log != null, "An event cannot be started if the SystemLogger does not have an active log",
                         "Is SysLog.Start() called?");

            string alias = "__" + _anonymousEventCounter;

            AddEvent(alias, type, 1); // Anonymous eventz have 1 processor: the entity that added the event
            _anonymousEventCounter++;

            return alias;
        }

        /// <summary>
        /// Sets a property existing of a key and a value on the event corresponding to the given alias.
        /// </summary>
        /// <param name="alias">The alias of the event of which to set the property.</param>
        /// <param name="key">The id of the property.</param>
        /// <param name="value">The value of the property.</param>
        private void SetProperty(string alias, string key, object value)
        {
            Debug.Assert(_log != null, "A property cannot be set if the SystemLogger does not have an active log",
                         "Is SysLog.Start() called?");

            _log.SetProperty(alias, key, value.ToString());
        }

        /// <summary>
        /// Adds the given EventPart to the event with the given alias.
        /// </summary>
        /// <param name="e">The EventPart to add to the event.</param>
        /// <param name="alias">The alias of the event where to add 'e' to.</param>
        public void Write(IEventPart e, string alias)
        {
            Debug.Assert(_log != null, "An eventpart cannot be written if the SystemLogger does not have an active log",
                         "Is SysLog.Start() called?");
            _log.Write(e, alias);
        }

        /// <summary>
        /// Restricted logging when 'WordLog Restricted' is 'true' in Options/Logging tab.
        /// Adds the given EventPart to the event with the given alias, only if the event is created inside
        /// a Microsoft Word document or if it is a window title after a focus change.
        /// </summary>
        /// <param name="e">The EventPart to add to the event.</param>
        /// <param name="alias">The alias of the event where to add 'e' to.</param>
        public void RestrictedWrite(IEventPart e, string alias)
        {
            // Is this a focus change? 
            if (e.ToString().Contains("[WinLog.FocusChange"))
            { 
              // There was often no match because of main doc naming. The IEventPart should contain 'WordLog'
              // to be considered as 'main document'.
              //  if (Regex.IsMatch(e.ToString(), _sessionId.GetMainDocument(), RegexOptions.Compiled))
              if (e.ToString().Contains("WordLog")) 
              {
                  _allowKeyLogging = true;
              }
              else
              {
                  _allowKeyLogging = false;
                  _log.Write(e, alias);
              } 
            }
            if (_allowKeyLogging) _log.Write(e, alias);
        }

        /// <summary>
        /// Writes a whole event.
        /// </summary>
        /// <param name="even">The complete event to write.</param>
        public void Write(Event even)
        {
            Debug.Assert(_log != null, "An event cannot be written if the SystemLogger does not have an active log",
                         "Is SysLog.Start() called?");
            _log.Write(even);
        }

        /// <summary>
        /// Notifies the SystemLogger that one of the processors of the event with alias the given alias has finished.
        /// This method should be called whenever an entity has finished processing the event with the given alias such
        /// that the SystemLogger knows when there are no more current processors and the corresponding event can be
        /// closed in the Log.
        /// The event will be marked as closes whenever the SystemLogger sees that the event does not have any more
        /// active processors.
        /// </summary>
        /// <param name="alias">The alias of the event.</param>
        public void EndEvent(string alias)
        {
            Debug.Assert(_log != null, "An event cannot be started if the SystemLogger does not have an active log",
                "Is SysLog.Start() called?");

            lock (_processors)
            {
                // Decrease number of known processors
                _processors[alias]--;

                if (_processors[alias] > 0) return;
                _log.EndEvent(alias); // Mark as closed

                // Remove
                _processors.Remove(alias);
            }
        }

        /// <summary>
        /// Creates an event with the given alias in Log if nrOfProcessors > 0.
        /// </summary>
        /// <param name="alias">The alias to use when referring to the event.</param>
        /// <param name="type">The value of the AnalysisType property of the event
        /// (or null if it does not need to be set).</param>
        /// <param name="nrOfProcessors">The number of external processors.</param>
        private void AddEvent(string alias, string type, int nrOfProcessors)
        {
            if (nrOfProcessors <= 0) return;
            // Only add an event if someone is processing it.
            lock (_processors)
            {
                _processors.Add(alias, nrOfProcessors);
            }

            _log?.StartEvent(alias);
            if (type != null)
            {
                SetProperty(alias, "type", type);
            }
        }

        /// <summary>
        /// Callback for mouse eventz.
        /// </summary>
        /// <param name="sender">The originater of the event.</param>
        /// <param name="e">Contains the data of the event.</param>
        private void MouseCallback(object sender, MouseEvent e)
        {
            try
            {
                string alias = e.SequenceNumber.ToString();
                int nrOfProcessors = MouseEvent?.GetInvocationList().Length ?? 0;
                AddEvent(alias, "mouse", nrOfProcessors);
                MouseEvent?.Invoke(this, e, alias, new HashSet<KeysEx>(_keyboardState));
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Callback for keyboard eventz.
        /// </summary>
        /// <param name="sender">The originator of the event.</param>
        /// <param name="e">Contains the data of the event.</param>
        private void KeyboardCallback(object sender, KeyboardEvent e)
        {
            try
            {
                string alias = e.SequenceNumber.ToString();
                int nrOfProcessors = KeyboardEvent?.GetInvocationList().Length ?? 0;
                ICollection<KeysEx> keys = new HashSet<KeysEx>(_keyboardState);

                lock (_keyboardState)
                {
                    if (e.Type == KeyboardMessages.WM_KEYUP || e.Type == KeyboardMessages.WM_SYSKEYUP)
                    {
                        // Key is no longer pressed => remove it from the KeyboardState
                        _keyboardState.Remove(e.Key);
                    }
                    else
                    {
                        // Key is pressed => add it to KeyboardState
                        _keyboardState.Add(e.Key);
                    }
                }

                AddEvent(alias, "keyboard", nrOfProcessors);
                KeyboardEvent?.Invoke(this, e, _converter.Convert(e.Key, _keyboardState), alias, keys);
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _log?.Stop();

                SystemMonitor.KeyboardEvent -= KeyboardCallback;
                SystemMonitor.MouseEvent -= MouseCallback;
            }
            _disposed = true;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~SystemLogger()
        {
            Dispose(false);
        }
    }
}