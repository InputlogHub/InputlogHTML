using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;

namespace InputLog.Core.Mining.Process
{
    public interface ICaseSplitter
    {
        Dictionary<string, List<Event>> Split(List<Event> events);
    }
}
