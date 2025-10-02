using System;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Event arguments for a PauselocationEvent.
    /// </summary>
    public class PauseLocationEventArgs : EventArgs
    {

        /// <summary>
        /// Event that triggered the PauseLocationEvent.
        /// </summary>
        public Events.Event Event
        {
            private set;
            get;
        }

        /// <summary>
        /// PauseLocation that belongs to the Event that triggered the PauseLocationEvent.
        /// </summary>
        public PauseLocation PauseLocation
        {
            private set;
            get;
        }

        /// <summary>
        /// Constructs a PauseLocationEventArgs.
        /// </summary>
        /// <param name="pEvent">Event that is part the PauseLocationEventArgs
        ///  (=event that triggered the PauseLocationEvent).</param>
        /// <param name="pauseLocation">Pauselocation that belongs to this event.</param>
        public PauseLocationEventArgs(Events.Event pEvent, PauseLocation pauseLocation)
        {
            Event = pEvent;
            PauseLocation = pauseLocation;
        }

        /// <summary>
        /// Constructs a PauseLocationEventArgs.
        /// </summary>
        /// <param name="eventPausePair">Pair containing an event (=the event that triggered the PauseLocationEvent) 
        /// and its corresponding pauselocation.</param>
        public PauseLocationEventArgs(Pair<Events.Event, PauseLocation> eventPausePair)
            : this(eventPausePair.First, eventPausePair.Second)
        {
        }
    }
}
