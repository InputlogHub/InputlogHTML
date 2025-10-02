using System.Linq;
using InputLog.Core.Events;
using System.Collections.Generic;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Preprocessing.Filter
{

    /// <summary>
    /// Event Filter that filters events based on their type.
    /// </summary>
    public class EventTypeFilter : EventFilter
    {
        #region
        /// <summary>
        /// Whether to remove matched events (true), or to keep matched events and remove all others (false).
        /// </summary>
        private readonly bool Remove;

        /// <summary>
        /// The event types to match (filter).
        /// </summary>
        private readonly string[] EventTypes;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventTypes"></param>
        public EventTypeFilter(string[] eventTypes)
        {
            EventTypes = eventTypes;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name for this filter</param>
        /// <param name="eventTypes">The event types to match (filter).</param>
        /// <param name="remove">Whether to remove matched events (true), 
        /// or to keep matched events and remove all others (false).</param>
        public EventTypeFilter(string name, string[] eventTypes, bool remove = false)
        {
            PreprocessorName = name;
            Remove = remove;
            EventTypes = eventTypes;
        }

        /// <summary>
        /// Verifies whether the passed event is of a predefined type (the acceptable types are set in the ctor). 
        /// If so, this event is either kept (if the 'remove' parameter in the ctor was false) or 
        /// removed (if the 'remove' parameter in the ctor was true).
        /// If not, the event is either kept (if the 'remove' parameter in the ctor was true) or 
        /// removed (if the 'remove' parameter in the ctor was false).
        /// </summary>
        /// <param name="even">Event to verify</param>
        /// <returns>True if the event is to kept, false otherwise.</returns>
		public override bool Check(Event even)
        {
            if (EventTypes.Contains(even.Type))
            {
                if (EventTypes.Contains(even.Type))
                {
                    return !Remove;
                }
            }
            return Remove;
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
