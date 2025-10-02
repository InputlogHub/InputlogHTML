using System.Collections.Generic;
using System.Text;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Hooks.Mouse;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Plugin.WinLog
{
    /// <summary>
    /// WinLogger for focus changes.
    /// </summary>
    public class FocusLogger : AbstractPlugin
    {
        #region Fields
     
        /// <summary>
        /// Previous window title.
        /// </summary>
        private string _oldTitle = "";
        private ulong _oldTime;
        // The name of the main logging document as recorded in the SessionIdentification Meta
        public static string FocusMainDoc { get; set; }

        /// <summary>
        /// Property that returns the value of the window currently
        /// having focus.
        /// </summary>
        private static string CurrentTitle
        {          
            get
            {
                var sb = new StringBuilder(500);
                var hwnd = NativeMethods.GetForegroundWindow();
                var result = NativeMethods.GetWindowText(hwnd, sb, sb.Capacity);
                var title = sb.ToString().Substring(0, result);

                if (!title.Contains("docx")) return title;

                // Microsoft may append '- Word' or other markers to a Word document.
                // We don't want this as it complicates comparing document titles.
                var end = title.LastIndexOf('.') + 5;
                var newTitle = title.Substring(0, end);
                // If the title of a Word document in focus equals the name of the main document
                // in the session identification, we simplify it to 'WordLog MainDoc'.
                // This solves issues with templates that are not recognized as the main logging doc.
                return FocusMainDoc.Contains(newTitle) ? "WordLog MainDoc" : newTitle;
            }
        }

        /// <summary>
        /// When the user checked the restricted write button in the GUI options, 
        /// keyboard logging outside the main word document will not be recorded.
        /// </summary>
        public bool IsRestricted { set; private get; }

        #endregion

        /// <summary>
        /// Callback for mouse eventz, just calls HandleEvent with the alias.
        /// </summary>
        /// <param name="sender">The originator of the event.</param>
        /// <param name="e">Contains the data of the event.</param>
        /// <param name="alias">The alias corresponding to the event.</param>
        /// <param name="kbState">The keyboard state.</param>
        private void MouseCallback(object sender, MouseEvent e, string alias, ICollection<KeysEx> kbState)
        {
            HandleEvent(alias, e.Time);
        }

        /// <summary>
        /// Callback for keyboard events, just calls HandleEvent with the alias.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        /// <param name="unicode">The unicode representation of the key (if there is one), taking into account
        /// previously pressed dead keys. A maximum of one dead key is taken into account, ie chaining
        /// of dead keys is not supported.</param>
        /// <param name="alias">The alias corresponding to the event.</param>
        /// <param name="kbState">The keyboard state.</param>
        private void KeyboardCallback(object sender, KeyboardEvent e, string unicode, string alias, ICollection<KeysEx> kbState)
        {
            HandleEvent(alias, e.Time);
        }

        /// <summary>
        /// The actual handling of the eventz: logs a focus change if
        /// the current window title differs from the previous one.
        /// A focus takes its start and end time from the previous event.
        /// </summary>
        /// <param name="alias">The alias of the event that was passed on.</param>
        /// <param name="time"></param>
        private void HandleEvent(string alias, ulong time)
        {
            if (_oldTime == 0)
            {
                _oldTime = time;
            }
            try
            {
                string newTitle = CurrentTitle;
                if (newTitle.Equals(_oldTitle)) return;
                var name = SysLog.StartEvent("focus");
                if (IsRestricted) SysLog.RestrictedWrite(new FocusChange(newTitle, _oldTime, _oldTime), name);
                else SysLog.Write(new FocusChange(newTitle, _oldTime, _oldTime), name);
                SysLog.EndEvent(name);
                _oldTitle = newTitle;
            }
            finally
            {
                SysLog.EndEvent(alias);
                _oldTime = time;
            }
        }

        /// <summary>
        /// Launches the FocusLogger, logging focus changes from now on.
        /// </summary>
        protected override void Start()
        {
            SysLog.KeyboardEvent += KeyboardCallback;
            SysLog.MouseEvent += MouseCallback;
        }

        /// <summary>
        /// Terminates the FocusLogger.
        /// </summary>
        protected override void Stop()
        {
            SysLog.KeyboardEvent -= KeyboardCallback;
            SysLog.MouseEvent -= MouseCallback;
        }

        /// <summary>
        /// Resets oldTime, oldTitle at the start of a new session.
        /// </summary>
        public void Reset()
        {
            _oldTitle = "";
            _oldTime = 0;
        }
    }
}