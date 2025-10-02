using System;
using System.Runtime.InteropServices;

namespace InputLog.Core.Hooks.Keyboard
{
    /// <summary>
    /// Listener LinearAnalysisType for keyboard eventz
    /// </summary>
    /// <param name="sender">The originater of the event.</param>
    /// <param name="e">Contains the data of the event.</param>
    public delegate void KeyboardListener(object sender, KeyboardEvent e);

    /// <summary>
    /// Class for processing the keyboard eventz from Windows.
    /// </summary>
    public class KeyboardEventListener : WindowsEventListener
    {
        #region Fields
        /// <summary>
        /// Constant for keyboard hooks. (See http://msdn.microsoft.com/en-us/library/ms644990.aspx.)
        /// </summary>
        //private const int WH_KEYBOARD = 2;
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Event that will be triggered whenever a low level keyboard event is detected.
        /// </summary>
        public event KeyboardListener KeyboardEvent;
        #endregion

        /// <summary>
        /// Constructor. Use low level hook because this is the only global hook allowed from C#.
        /// </summary>
        internal KeyboardEventListener() : base(WH_KEYBOARD_LL) { }

        /// <summary>
        /// Processes the event by converting the input parameters and passing them to the registered listeners.
        /// The parameters are the ones that Windows passed to the callback method.
        /// </summary>
        /// <param name="nCode">nCode as passed by Windows to callback.</param>
        /// <param name="wParam">wParam as passed by Windows to callback.</param>
        /// <param name="lParam">lParam as passed by Windows to callback.</param>
        protected override void ProcessEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Convert parameters to keyboard struct
            var ptrToStructure = Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if (ptrToStructure == null) return;
            KBDLLHOOKSTRUCT data = (KBDLLHOOKSTRUCT)ptrToStructure;

            // Pass on to listeners if there are any
            KeyboardEvent?.Invoke(this, new KeyboardEvent(data, wParam));
        }
    }
}