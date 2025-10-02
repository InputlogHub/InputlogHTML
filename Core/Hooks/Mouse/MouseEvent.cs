using System;
using System.Runtime.InteropServices;

namespace InputLog.Core.Hooks.Mouse
{

    #region Mouse Structs

    /// <summary>
    /// Struct containing the x y position of the mouse.
    /// (See http://msdn.microsoft.com/en-us/library/dd162805.aspx.)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// X Coordinate.
        /// </summary>
        public int x { get; private set; }

        /// <summary>
        /// Y Coordinate.
        /// </summary>
        public int y { get; private set; }
    }

    /// <summary>
    /// Struct passed as lParam of the callback if the hook is registered as WH_MOUSE_LL.
    /// (See http://msdn.microsoft.com/en-us/library/ms644970.aspx.)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    #endregion

    /// <summary>
    /// Different mouse messages, listed under wParam on http://msdn.microsoft.com/en-us/library/ms644986.aspx.
    /// </summary>
    public enum MouseMessages
    {
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_LBUTTONDBLCLK = 0x203,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_RBUTTONDBLCLK = 0x206,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MBUTTONDBLCLK = 0x209,
        WM_MOUSEWHEEL = 0x20A,
        WM_XBUTTONDOWN = 0x20B,
        WM_XBUTTONUP = 0x20C,
        WM_XBUTTONDBLCLK = 0x20D,
        WM_MOUSEHWHEEL = 0x20E
    }

    /// <summary>
    /// Represents a mouse event and aggregates all the information received from Windows about it in one class.
    /// Whenever an instance of MouseEventListiner notices a mouse event, it will generate an event
    /// of this kind and pass it on to the registered listeners.
    /// </summary>
    public class MouseEvent : BasicEvent
    {
        #region Fields

        /// <summary>
        /// The x- and y-coordinates of the cursor, in screen coordinates.
        /// </summary>
        public POINT Point { get; private set; }

        /// <summary>
        /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta.
        /// The low-order word is reserved. A positive value indicates that the wheel was rotated forward,
        /// away from the user; a negative value indicates that the wheel was rotated backward,
        /// toward the user. One wheel click is defined as WHEEL_DELTA, which is 120.
        /// 
        /// If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN,
        /// WM_NCXBUTTONUP, or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button
        /// was pressed or released, and the low-order word is reserved. This value can be one
        /// or more of the following values. Otherwise, mouseData is not used.
        /// </summary>
        public uint MouseData { get; private set; }

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
        /// The LinearAnalysisType of the mouse message.
        /// </summary>
        public MouseMessages Type { get; private set; }

        /// <summary>
        /// Returns the wheel delta (see http://msdn.microsoft.com/en-us/library/ms644970.aspx).
        /// This Propertie is only useful if LinearAnalysisType == MouseMessages.WM_MOUSEWHEEL.
        /// </summary>
        public int WheelDelta
        {
            get { return (int) MouseData >> 16; }
        }

        #endregion

        /// <summary>
        /// Constructor, constructs a new MouseEvent with the given parameters.
        /// </summary>
        /// <param name="data">The struct as passed by Windows when the event was generated.</param>
        /// <param name="wParam">wParam as passed by Windows, defines the LinearAnalysisType of mouse message.</param>
        public MouseEvent(MSLLHOOKSTRUCT data, IntPtr wParam)
        {
            Point = data.pt;
            MouseData = data.mouseData;
            Flags = data.flags;
            Time = data.time;
            DWExtraInfo = data.dwExtraInfo;
            Type = (MouseMessages) wParam;
        }
    }
}