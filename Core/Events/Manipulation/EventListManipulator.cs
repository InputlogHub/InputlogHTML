using System.Collections.Generic;

namespace InputLog.Core.Events.Manipulation
{
    public abstract class EventListManipulator
    {
        #region Fields

        /// <summary>
        /// The events that are being manipulated.
        /// </summary>
        protected List<Event> Events;

        #endregion

        protected EventListManipulator(List<Event> events)
        {
            Events = events;
        }

        public abstract List<Event> Run();
    }
}