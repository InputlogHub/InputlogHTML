using System;
using System.Runtime.InteropServices;

namespace InputLog.Core.Hooks.Mouse
{
    /// <summary>
    /// Listener LinearAnalysisType for mouse eventz
    /// </summary>
    /// <param name="sender">The originater of the event.</param>
    /// <param name="e">Contains the data of the event.</param>
    public delegate void MouseListener(object sender, MouseEvent e);

    /// <summary>
    /// Class for processing the mouse eventz from Windows.
    /// </summary>
    public class MouseEventListener : WindowsEventListener
    {
        #region Fields
        /// <summary>
        /// Constants for mouse hook types (see http://msdn.microsoft.com/en-us/library/ms644990.aspx).
        /// </summary>
        private const int WH_MOUSE = 7;
        private const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Event that will be triggered whenever a low level mouse event is detected.
        /// </summary>
        public event MouseListener MouseEvent;
        #endregion

        /// <summary>
        /// Constructor. Use low level hook because this is the only global hook allowed from C#.
        /// </summary>
        internal MouseEventListener() : base(WH_MOUSE_LL) { }

        /// <summary>
        /// Processes the event by converting the input parameters and passing them to the registered listeners.
        /// The parameters are the ones that Windows passed to the callback method.
        /// </summary>
        /// <param name="nCode">nCode as passed by Windows to callback.</param>
        /// <param name="wParam">wParam as passed by Windows to callback.</param>
        /// <param name="lParam">lParam as passed by Windows to callback.</param>
        override protected void ProcessEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Convert parameters to mouse struct
            MSLLHOOKSTRUCT data = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

            // Pass on to listeners if there are any
            if (this.MouseEvent != null)
            {
                this.MouseEvent(this, new MouseEvent(data, wParam));
            }
        }
    }
}