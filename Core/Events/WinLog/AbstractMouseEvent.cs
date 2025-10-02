using System.Xml;
using System;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Core mouse information (received from Windows).
    /// @date July 06, 2010
    /// </summary>
    public abstract class AbstractMouseEvent : TimedEventPart, IMouseEventPart
    {
        /// <summary>
        ///  X coordinate of the mouse pointer.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y coordinate of the mouse pointer.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Constructs a MouseEventPart.
        /// </summary>
        /// <param name="x">x coordinate of the mouse pointer.</param>
        /// <param name="y">y coordinate of the mouse pointer.</param>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        protected AbstractMouseEvent(int x, int y, ulong start, ulong end = 0ul)
            : base(start, end)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public AbstractMouseEvent()
        {
        }
    }
}