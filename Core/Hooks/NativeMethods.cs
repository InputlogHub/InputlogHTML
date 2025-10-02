using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputLog.Core.Hooks
{
    /// <summary>
    /// Delegate for Windows hooks. (See http://msdn.microsoft.com/en-us/library/ms644990(VS.85).aspx.)
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    internal delegate IntPtr WindowsHook(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Util class that imports some Dll methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Handle to dll (self).
        /// </summary>
        internal static readonly IntPtr DllHandle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

        /// <summary>
        /// See http://msdn.microsoft.com/en-us/library/ms683199.aspx.
        /// </summary>
        /// <param name="lpModuleName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// See http://msdn.microsoft.com/en-us/library/ms644990.aspx.
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hMod"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, WindowsHook lpfn, IntPtr hMod,
            uint dwThreadId);

        /// <summary>
        /// See  http://msdn.microsoft.com/en-us/library/ms644993.aspx.
        /// </summary>
        /// <param name="hhk"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// See http://msdn.microsoft.com/en-us/library/ms644974.aspx.
        /// </summary>
        /// <param name="hhk"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// Used for converting a virtual key code to char.
        /// See http://msdn.microsoft.com/en-us/library/ms646306.aspx.
        /// </summary>
        public const uint MAPVK_VK_TO_CHAR = 2;
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyW", ExactSpelling = true)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
}