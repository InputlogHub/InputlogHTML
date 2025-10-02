using System.Collections.Generic;
using System.IO;
using System.Text;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Pause;
using InputLog.Core.IO;

namespace GUI.Tabs.Analyze.AnalysesControls.Pause
{
    public class PauseAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Pause";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "PA";

        /// <summary>
        /// Extra parameter used by the analysis. When the intervaltype is:
        ///  - FixedNumberOfIntervals, this parameter represents the number of parameters
        ///  - FixedIntervalLength, this parameter represents the interval length
        /// </summary>
        private readonly ulong IntervalParam;

        /// <summary>
        /// The type of pause analysis that will be performed.
        /// </summary>
        private readonly PauseAnalysis.IntervalType PauseAnalysisType;

        /// <summary>
        /// The value of the pause threshold of the last returned Analyser (using the GetAnalyser method).
        /// </summary>
        private readonly ulong PauseThreshold;
        /// <summary>
        /// The value of the P-Burst threshold of the last returned Analyser (using the GetAnalyser method).
        /// </summary>
        private readonly ulong PBurstThreshold;

        /// <summary>
        /// Set this boolean to true if this analyzer is run for report generation and should
        /// run 4 pause analyses instead of just one pause analysis.
        /// This mode is remembered until it is reset!
        /// </summary>
        public static bool RunMultiple = false;

        public PauseAnalyzer(ulong intervalParam, PauseAnalysis.IntervalType pauseType, ulong pauseThreshold, ulong pburstThreshold)
        {
            IntervalParam = intervalParam;
            PauseAnalysisType = pauseType;
            PauseThreshold = pauseThreshold;
            PBurstThreshold = pburstThreshold;
        }

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new PauseAnalysis(Events, SessionId, PauseAnalysisType, IntervalParam, PauseThreshold, PBurstThreshold, RunMultiple);
        }

        /// <summary>
        /// Given the destination folder and a source (log) file, this method returns a
        /// full path of the file where the result of the analysis should be stored. This pathname may contain 
        /// place holders for sessionidentification information using the ${key} notation.
        /// </summary>
        /// <param name="sessionId">The session identification.</param>
        /// <param name="srcFile">The source (log) file</param>
        /// <param name="destDir">The destination directory path.</param>
        /// <returns>String with full file path of the file where the result of the analysis should be stored</returns>
        protected override string ExtendFileName(SessionIdentification sessionId, string srcFile, string destDir)
        {
            string name = base.ExtendFileName(sessionId, srcFile, destDir);

            var buf = new StringBuilder(Path.Combine(Path.GetDirectoryName(name),
                Path.GetFileNameWithoutExtension(name)));
            buf.Append("_PT" + PauseThreshold);
            switch (PauseAnalysisType)
            {
                case PauseAnalysis.IntervalType.FIXED_NUMBER_OF_INTERVALS:
                    buf.Append("_FN" + IntervalParam);
                    break;

                case PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS:
                    buf.Append("_FL" + (IntervalParam / 1000));
                    break;
            }
            buf.Append(Path.GetExtension(name));

            return buf.ToString();
        }

        /// <summary>
        /// Creates the dictionary of parameters that should be passed to the analysiswriter.
        /// </summary>
        /// <returns>A dictionary containing the various parameters.</returns>
        protected override IDictionary<string, IDictionary<string, object>> GetParameters()
        {
            var dict = new Dictionary<string, IDictionary<string, object>>();
            var parameters = new Dictionary<string, object>();
            dict["Parameters"] = parameters;

            parameters[PauseAnalysisXMLWriter.PAUSE_THRESHOLD_PARAMETER] = PauseThreshold;
            parameters[PauseAnalysisXMLWriter.PBURST_THRESHOLD_PARAMETER] = PBurstThreshold;
            switch (PauseAnalysisType)
            {
                case PauseAnalysis.IntervalType.FIXED_LENGTH_INTERVALS:
                    parameters[PauseAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Length Intervals";
                    parameters["Length of Interval (sec)"] = IntervalParam / 1000;
                    break;
                case PauseAnalysis.IntervalType.FIXED_NUMBER_OF_INTERVALS:
                    parameters[PauseAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Number of Intervals";
                    parameters["Number of Intervals"] = IntervalParam;
                    break;
            }
            return dict;
        }
    }
}
