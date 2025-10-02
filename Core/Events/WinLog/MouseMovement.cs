using System.Xml;
namespace InputLog.Core.Events.WinLog
{
    public sealed class MouseMovement : AbstractMouseEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate of the mouse.</param>
        /// <param name="y">Y coordinate of the mouse.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public MouseMovement(int x, int y, ulong start, ulong end = 0ul) : base(x, y, start, end) { }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public MouseMovement()
        {
        }
    }
}