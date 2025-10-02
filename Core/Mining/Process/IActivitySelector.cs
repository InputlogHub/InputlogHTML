using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;

namespace InputLog.Core.Mining.Process
{
    public interface IActivitySelector
    {
        List<Activity> Select(List<Event> events);
    }
}
