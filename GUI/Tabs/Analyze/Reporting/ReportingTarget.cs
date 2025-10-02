using System;
namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    /// Class that holds information about which a specific reporting target with
    /// specified identifier, and any extra information the user has indicated
    /// that is important about how the target should be reported on. This includes
    /// information such as blocks.
    /// </summary>
    [Obsolete("Use Templates instead", false)]
    public class ReportingTarget
    {
        /// <summary>
        /// Analysis that should be run in order to get the information
        /// for this specific reporting target.
        /// </summary>
        public string Analysis
        {
            get;
            set;
        }
        /// <summary>
        /// The target report ID that needs to be run after the analysis
        /// has been completed. This is the actual resource ID of the reporting method.
        /// </summary>
        public string TargetID
        {
            get;
            private set;
        }

        /// <summary>
        /// The block in which this element should be added under in the 
        /// report. 
        /// </summary>
        public string Block
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new reporting target keeping information about how a 
        /// target should be added to the report, and what the exact target
        /// of the element is.
        /// </summary>
        /// <param name="target">The exact name of the target to be executed.
        /// This should be the exact method name.</param>
        /// <param name="block">The block the target should be added in when
        /// the report is outputted.</param>
        public ReportingTarget(string target, string block)
        {
            TargetID = target;
            Block = block;
        }

        /// <summary>
        /// Create a new reporting target keeping information about how a 
        /// target should be added to the report, and what the exact target
        /// of the element is.
        /// </summary>
        /// <param name="analysis">The analysis that supports the specified 
        /// reporting target </param>
        /// <param name="target">The exact name of the target to be executed.
        /// This should be the exact method name.</param>
        /// <param name="block">The block the target should be added in when
        /// the report is outputted.</param>
        public ReportingTarget(string analysis, string target, string block)
        {
            Analysis = analysis;
            TargetID = target;
            Block = block;
        }
    }
}
