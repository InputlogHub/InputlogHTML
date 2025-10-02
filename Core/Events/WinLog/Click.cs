using InputLog.Core.Hooks.Mouse;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// The buttons that can be clicked.
    /// </summary>
    public enum Buttons
    {
        UNKNOWN,
        LEFT,
        MIDDLE,
        RIGHT,
        X
    }

    /// <summary>
    /// Represents a click of a mouse button.
    /// </summary>
    public class Click : AbstractMouseEvent
    {
        #region Fields

        /// <summary>
        /// The button that was clicked.
        /// </summary>
        public Buttons Button { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate of the mouse.</param>
        /// <param name="y">Y coordinate of the mouse.</param>
        /// <param name="button">The button that was clicked</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public Click(int x, int y, Buttons button, ulong start, ulong end = 0ul)
            : base(x, y, start, end)
        {
            Button = button;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Click()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate of the mouse.</param>
        /// <param name="y">Y coordinate of the mouse.</param>
        /// <param name="button">The button that was clicked
        /// (should be one of WM_*BUTTONDOWN or WM_*BUTTONUP)</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public Click(int x, int y, MouseMessages button, ulong start, ulong end = 0ul)
            : base(x, y, start, end)
        {
            switch (button)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                case MouseMessages.WM_LBUTTONUP:
                case MouseMessages.WM_LBUTTONDBLCLK:
                    Button = Buttons.LEFT;
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                case MouseMessages.WM_RBUTTONUP:
                case MouseMessages.WM_RBUTTONDBLCLK:
                    Button = Buttons.RIGHT;
                    break;
                case MouseMessages.WM_MBUTTONDOWN:
                case MouseMessages.WM_MBUTTONUP:
                case MouseMessages.WM_MBUTTONDBLCLK:
                    Button = Buttons.MIDDLE;
                    break;
                case MouseMessages.WM_XBUTTONDOWN:
                case MouseMessages.WM_XBUTTONUP:
                case MouseMessages.WM_XBUTTONDBLCLK:
                    Button = Buttons.X;
                    break;

                default:
                    Button = Buttons.UNKNOWN;
                    break;
            }
        }
    }
}