using System;

namespace InputLog.Core.Analyses.PauseParser
{
    public interface IEventLookup
    {
            Func<bool> FindPredicate(string name, int peekPosition = 0);
    }
}
