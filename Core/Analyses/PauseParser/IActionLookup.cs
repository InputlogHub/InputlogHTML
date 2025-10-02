using System;

namespace InputLog.Core.Analyses.PauseParser
{
    public interface IActionLookup
    {
            Action FindAction(string name);
    }
}
