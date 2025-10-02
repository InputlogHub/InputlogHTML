using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Pause;

namespace InputLog.Core.Reporting
{
    public class ReportController
    {
        public static readonly string REPORT_PREFIX = "report_";

        /// <summary>
        /// Returns a list of names of reporting targets that have been identified on the 
        /// type supplied as parameter. 
        /// A ReportTarget is valid if it's a method that:
        /// 1. Starts with prefix `REPORT_PREFIX` in its name
        /// 2. Takes no parameters
        /// 3. Returns a ReportElement as its result
        /// 
        /// Note: These reporting targets are not bound to any object on which they can be 
        /// executed. These are only the names of the reporting methods that have been
        /// identified on the `reporter`.
        /// </summary>
        /// <param name="reporter">The type of the class which we want to inspect for the 
        /// presence of Reporting Targets.</param>
        /// <returns></returns>
        [Obsolete("IdentifyReportTargets is deprecated, please use Report.Resources instead.")]
        public static List<string> IdentifyReportTargets(Type reporter)
        {
            List<string> identifiedTargets = new List<string>();
            string REPORT_PREFIX = "report_";

            MethodInfo[] methodInfos = reporter.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            foreach (MethodInfo mInfo in methodInfos) 
            {
                // The only valid reporting methods are methods with:
                // 1. The report_ prefix
                // 2. ReportElement returntype or a subclass/assignable replacement
                // 3. Takes no parameters
                if (mInfo.Name.StartsWith(REPORT_PREFIX) && 
                    typeof(ReportValue).IsAssignableFrom(mInfo.ReturnType) &&
                    mInfo.GetParameters().Length == 0)
                {
                    sb.Append(mInfo.Name);
                    identifiedTargets.Add(sb.ToString());
                    sb.Clear();
                }
            }

            return identifiedTargets;
        }
    }
}
