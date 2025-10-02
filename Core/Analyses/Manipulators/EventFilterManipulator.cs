using System.Collections.Generic;
using InputLog.Core.Events.Manipulation;
using InputLog.Core.Events;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Manipulators {

    public class EventFilterManipulator : EventListManipulator {

        #region

        private readonly EventFilter _filter;

        #endregion

        public EventFilterManipulator(List<Event> events, EventFilter filter)
            : base(events)
        {
            _filter = filter;
        }

        public override List<Event> Run()
        {
            return Events.Filter(_filter);
        }
    }
}
