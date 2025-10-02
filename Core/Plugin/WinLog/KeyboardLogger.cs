using System.Collections.Generic;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Plugin.WinLog
{
    /// <summary>
    /// WinLogger for keyboard eventz.
    /// </summary>
    public class KeyboardLogger : AbstractPlugin
    {
        #region Fields
        /// <summary>
        /// Contains the keys currently pressed and not yet released and maps them to their alias
        /// and their event part, waiting for their endtime to be set.
        /// </summary>
        private readonly IDictionary<KeysEx, Pair<string, KeyPress>> KeyboardState = 
            new Dictionary<KeysEx, Pair<string, KeyPress>>();

        /// <summary>
        /// When the user checked the restricted write button in the GUI options, 
        /// keyboard logging outside the main word document will not be recorded.
        /// </summary>
        public bool IsRestricted { set; private get; }
        #endregion

        /// <summary>
        /// Callback for keyboard eventz.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        /// <param name="unicode">The unicode representation of the key (if there is one), taking into account
        /// previously pressed dead keys. (A maximum of one dead key is taken into account, ie chaining
        /// of dead keys is not supported.)</param>
        /// <param name="alias">The alias corresponding to the event.</param>
        /// <param name="kbState">The keyboard state.</param>
        private void KeyboardCallback(object sender, KeyboardEvent e, string unicode, string alias, ICollection<KeysEx> kbState)
        {
            lock (KeyboardState)
            {
                // KeyboardState contains key, then key was already down => write & end currently stored event
                Pair<string, KeyPress> pair;
                if (KeyboardState.TryGetValue(e.Key, out pair))
                {
                    try
                    {
                        // If new event is keyup, this is the keyup that corresponds to the keydown
                        // of which the event was stored in KeyboardState.
                        if (e.Type == KeyboardMessages.WM_KEYUP || e.Type == KeyboardMessages.WM_SYSKEYUP)
                        {
                            pair.Second.EndTime = e.Time; // endtime of a keyboardevent == time of keyup

                            // If it was keyup, key is no longer pressed => remove it from KeyboardState
                            KeyboardState.Remove(e.Key);
                        }

                        // Write & End the event
                        if (IsRestricted) SysLog.RestrictedWrite(pair.Second, pair.First);
                        else SysLog.Write(pair.Second, pair.First);
                    }
                    finally
                    {
                        SysLog.EndEvent(pair.First);
                    }
                }

                // If it was a keydown, generate a new keyboard event
                if (e.Type == KeyboardMessages.WM_KEYDOWN || e.Type == KeyboardMessages.WM_SYSKEYDOWN)
                {
                    KeyboardState[e.Key] = new Pair<string, KeyPress>(alias,
                        new KeyPress(e.Key, unicode, e.Time, kbState));

                    // We are not done processing this event, we're waiting for the corresponding
                    // KEYUP event => DO NOT call EndEvent yet
                }
                else
                {
                    SysLog.EndEvent(alias);
                }
            }
        }

        /// <summary>
        /// Launches the KeyboardLogger, logging keyboard input from now on.
        /// </summary>
        protected override void Start()
        {
            SysLog.KeyboardEvent += KeyboardCallback;
        }

        /// <summary>
        /// Terminates the KeyboardLogger.
        /// </summary>
        protected override void Stop()
        {
            SysLog.KeyboardEvent -= KeyboardCallback;
        }
    }
}