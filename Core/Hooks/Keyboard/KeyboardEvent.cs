using System;
using System.Runtime.InteropServices;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Hooks.Keyboard
{

    #region Keyboard Structs

    /// <summary>
    /// Struct passed as lParam of the callback if the hook is registered as WH_KEYBOARD_LL.
    /// (See http://msdn.microsoft.com/en-us/library/ms644967.aspx.)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public readonly uint vkCode;
        public readonly uint scanCode;
        public readonly uint flags;
        public readonly uint time;
        public IntPtr dwExtraInfo;
    }

    #endregion

    /// <summary>
    /// Different keyboard messages, listed under wParam on http://msdn.microsoft.com/en-us/library/ms644985.aspx.
    /// </summary>
    public enum KeyboardMessages
    {
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105
    }

    /// <summary>
    /// Represents a keyboard event and aggregates all the information received from Windows about it in one class.
    /// Whenever an instance of KeyboardEventListiner notices a keyboard event, it will generate an event
    /// of this kind and pass it on to the registered listeners.
    /// </summary>
    public class KeyboardEvent : BasicEvent
    {
        #region Fields

        /// <summary>
        /// The pressed key that lead to the construction of this event.
        /// </summary>
        public KeysEx Key { get; private set; }

        /// <summary>
        /// The Char representation of the pressed key.
        /// </summary>
        public char Character { get; private set; }

        /// <summary>
        /// Virtual Key code, ranges from 1 to 254.
        /// </summary>
        public uint VKCode { get; private set; }

        /// <summary>
        /// Hardware scan code for the key.
        /// </summary>
        public uint ScanCode { get; private set; }

        /// <summary>
        /// Extended-key flag.
        /// </summary>
        public uint Flags { get; private set; }

        /// <summary>
        /// Time stamp for the message.
        /// </summary>
        public uint Time { get; private set; }

        /// <summary>
        /// Additional information associated with the message.
        /// </summary>
        public IntPtr DWExtraInfo { get; private set; }

        /// <summary>
        /// The LinearAnalysisType of the keyboard message.
        /// </summary>
        public KeyboardMessages Type { get; private set; }

        public static bool HighPrecision;
        private static bool _first = true;
        private static uint _timeOffset;
        #endregion

        /// <summary>
        /// Constructor, constructs a new KeyboardEvent with the given parameters.
        /// </summary>
        /// <param name="data">The struct as passed by Windows when the event was generated.</param>
        /// <param name="wParam">wParam as passed by Windows, defines the LinearAnalysisType of keyboard message.</param>
        public KeyboardEvent(KBDLLHOOKSTRUCT data, IntPtr wParam)
        {
            if (HighPrecision)
            {
                if (_first)
                {
                    Time = data.time;
                    _timeOffset = PCTimestamp() - data.time;
                    _first = false;
                }
                else
                {
                    Time = PCTimestamp() - _timeOffset;
                }
            }
            else
            {
                Time = data.time;
            }
            Key = (KeysEx) data.vkCode;
            unchecked
            {
                Character = (char) NativeMethods.MapVirtualKey(data.vkCode, NativeMethods.MAPVK_VK_TO_CHAR);
            }
            VKCode = data.vkCode;
            ScanCode = data.scanCode;
            Flags = data.flags;
            DWExtraInfo = data.dwExtraInfo;
            Type = (KeyboardMessages)wParam;
        }

        private static uint PCTimestamp()
        {
            long l1; long l2;
            QueryPerformanceCounter(out l1);
            QueryPerformanceFrequency(out l2);
            return (uint)(l1 / (l2 / 1000.0));
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}