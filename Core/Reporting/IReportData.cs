using System.Collections.Generic;

namespace InputLog.Core.Reporting
{
    public interface IReportData
    {
        /// <summary>
        /// Returns the reporting targets available in whoever implements this interface.
        /// That the ReportTargets are bound means that they are bound to and will be executed
        /// on the _instance_ that has supplied the ReportTargets
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ReportMethod> GetBoundReportTargets();
    }
}
