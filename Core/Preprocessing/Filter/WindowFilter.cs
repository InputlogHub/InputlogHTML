using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Preprocessing.Filter
{

    /// <summary>
    /// Filter that filters out events that happen in windows with specific titles 
    /// (only those events are kept, all other are dropped). The titles are specified by means of a regular expression.
    /// 
    /// </summary>
    public class WindowFilter : EventFilter
    {

        #region Fields
        /// <summary>
        /// Regular expression that specifies which events to keep (that is, it specifies the titles of the windows 
        /// in which events should be kept).
        /// </summary>
        private readonly Regex WindowTitleRegex;

        /// <summary>
        /// Whether to keep current events or not. Boolean is toggled based on titles of focus events.
        /// </summary>
        private bool KeepCurrentEvents;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name for this filter</param>
        /// <param name="windowTitleRegex">Regular expression indicating the titles of the windows 
        /// in which events should be kept).</param>
        public WindowFilter(string name, string windowTitleRegex)
        {
            PreprocessorName = name;
            WindowTitleRegex = new Regex(windowTitleRegex);
            KeepCurrentEvents = false;
        }

        /// <summary>
        /// Verifies whether an event occurs within a window of which the title matches the 
        /// regular expression specified in the ctor of this filter.
        /// </summary>
        /// <param name="even">Event to verify</param>
        /// <returns>True if the event is to kept, false otherwise.</returns>
		public override bool Check(Event even)
        {
            // Window switch, check whether we want these events
            if (EventType.FOCUS.Equals(even.Type))
            {
                var focusEventParts = even.Parts.OfType<FocusChange>();
                var focusChanges = focusEventParts as FocusChange[] ?? focusEventParts.ToArray();
                if (focusChanges.Any())
                {
                    var currentWindowTitle = focusChanges.First().WindowTitle;
                    KeepCurrentEvents = WindowTitleRegex.IsMatch(currentWindowTitle);
                }
            }
            return KeepCurrentEvents;

        }

        /// <summary>
        /// Process the eventList. This renames the value of any focus events according
        /// to the mapping rules specified by the user in the preprocessor (as per
        /// grouping).
        /// </summary>
        /// <param name="inputEvents">Events to process</param>
        /// <param name="sessionId"></param>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)
		{
			return inputEvents.Filter(this);
		}

    }
}
