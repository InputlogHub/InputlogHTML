using InputLog.Core.Hooks.Mouse;
using System;
using System.Xml;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Represents scrolling with the middle mouse button.
    /// </summary>
    public sealed class Scroll : AbstractMouseEvent
    {
        #region Fields
        /// <summary>
        /// The amount of scrolling done.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// The orientation of the scrolling.
        /// </summary>
        public OrientationType Orientation { get; private set; }
        #endregion

        /// <summary>
        /// LinearAnalysisType denoting whether it is a horizontal or a vertical scroll movement.
        /// </summary>
        public enum OrientationType
        {
            VERTICAL,
            HORIZONTAL,
            UNKNOWN
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Current x position of mouse.</param>
        /// <param name="y">Current y position of mouse.</param>
        /// <param name="delta">Amount of scrolling done.</param>
        /// <param name="orientation">The orientation of the scrolling.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public Scroll(int x, int y, int delta, OrientationType orientation, ulong start, ulong end = 0)
            : base(x, y, start, end)
        {
            this.Delta = delta;
            this.Orientation = orientation;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Scroll()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">Current x position of mouse.</param>
        /// <param name="y">Current y position of mouse.</param>
        /// <param name="delta">Amount of scrolling done.</param>
        /// <param name="LinearAnalysisType">Either WM_MOUSEWHEEL or WM_MOUSEHWHEEL.
        /// The orientation of the scrolling (horizonatal or vertical) will be deducted from this.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public Scroll(int x, int y, int delta, MouseMessages type, ulong start, ulong end = 0)
            : base(x, y, start, end)
        {
            this.Delta = delta;
            switch (type)
            {
                case MouseMessages.WM_MOUSEWHEEL:
                    this.Orientation = OrientationType.VERTICAL;
                    break;
                case MouseMessages.WM_MOUSEHWHEEL:
                    this.Orientation = OrientationType.HORIZONTAL;
                    break;
                default:
                    this.Orientation = OrientationType.UNKNOWN;
                    break;
            }
        }
    }
}