using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InputLog.Core.Plugin
{
    /// <summary>
    /// Util class that imports some Dll methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// If the specified window is a control, the text of the control is copied. However,
        /// GetWindowText cannot retrieve the text of a control in another application.
        /// (See http://msdn.microsoft.com/en-us/library/ms633520.aspx.)
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="lpString">The buffer that will receive the text.
        /// If the string is as long or longer than the buffer,
        /// the string is truncated and terminated with a null character.</param>
        /// <param name="nMaxCount">The maximum number of characters to copy to the buffer,
        /// including the null character. If the text exceeds this limit, it is truncated.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters,
        /// of the copied string, not including the terminating null character.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retursn the handle to the window currently on the foreground.
        /// (See http://msdn.microsoft.com/en-us/library/ms633505.aspx.)
        /// </summary>
        /// <returns>The handle to the window currently on the foreground.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
    }
}