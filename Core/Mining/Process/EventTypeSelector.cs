using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;

namespace InputLog.Core.Mining.Process
{
    public class EventTypeSelector : IActivitySelector
    {
        private ulong StartOffset;

        public EventTypeSelector(DateTime startOffset)
        {
            StartOffset = startOffset.ConvertToTimestamp();
        }

        public List<Activity> Select(List<Event> events)
        {
            List<Activity> acts = new List<Activity>();
            string lastType = "";
            foreach (Event e in events)
            {
                var type = e.Type;
                var time = Event.GetFirstEventPart<TimedEventPart>(e);
                if (time != null && !type.Equals(lastType))
                {
                    var newAct = new Activity(StartOffset + time.StartTime);
                    newAct.SetProperty("Activity", type);
                    acts.Add(newAct);
                }
                lastType = type;
            }
            return acts;
        }
    }
}
