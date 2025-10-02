using InputLog.Core.Events.WinLog;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Pseudo event class representing a double mouse click.
    /// This event is added during the analysis (multiple single clicks are merged in a double click).
    /// </summary>
    public class DoubleClick : Click
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="firstClick"></param>
        /// <param name="secondClick"></param>
        public DoubleClick(Click firstClick, TimedEventPart secondClick)
            :base(firstClick.X, firstClick.Y, firstClick.Button, firstClick.StartTime, secondClick.EndTime)
        {

        }
    }
}