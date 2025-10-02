using System.IO;
using InputLog.Core.Analyses.General;
using InputLog.Core.Reporting;

namespace InputLog.Core.Analyses.ProcessGraphAnalysis
{
    /// <summary>
    /// Summary of the ProcessGraphAnalysis, contains the results
    /// of performing the analysis.
    /// </summary>
    public class ProcessGraphAnalysisSummary: AbstractAnalysisSummary
    {
        public GeneralAnalysisSummary GASummary;
        public MemoryStream ImageBuffer;

        public ProcessGraphAnalysisSummary() 
        {
            this.ImageBuffer = new MemoryStream();
        }

        /*
         * IMPORTANT NOTE: 
         * These methods are being referenced in the reporting functionality, by name!
         * Do not change these method names without changing the references
         * in the required resource files as well.
         * 
         * Resource File: Core.Reporting.Resources.ReportMappingResources.resx
         * 
         * Note: The report_ prefix must be kept! The methods are also reflectively discovered
         * in the GetBoundTargets() method of the AbstractAnalysisSummary class!
         */
        #region Reporting 
        public ReportValue report_Process_Graph()
        {
            const string RESOURCE_ID = "Process_ProcessGraph";
            GraphValue value = new GraphValue(
                RESOURCE_ID,
                this.ImageBuffer
            );
            value.IsFullPageImage = true;
            return value;
        }
        #endregion

    }
}
