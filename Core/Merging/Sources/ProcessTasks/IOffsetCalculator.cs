using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util.Matching;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
    public abstract class IOffsetCalculator : ProcessTask
    {
        public abstract int?[] GetOffsets();
    }
}
