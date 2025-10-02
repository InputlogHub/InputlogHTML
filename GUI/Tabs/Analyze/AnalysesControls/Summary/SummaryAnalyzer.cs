using System.Collections.Generic;
using System.IO;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Summary;
using InputLog.Core.IO;

namespace GUI.Tabs.Analyze.AnalysesControls.Summary
{
    public class SummaryAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Summary";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "SU";

        private readonly ulong PauseThreshold;

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new SummaryAnalysis(Events, SessionId);
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
            parameters[SummaryAnalysisXMLWriter.PAUSE_THRESHOLD_PARAMETER] = PauseThreshold;

            return dict;
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
            var name = base.ExtendFileName(sessionId, srcFile, destDir);
            return string.Concat(Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name)),
                "_PT" + PauseThreshold, Path.GetExtension(name));
        }
    }
}
