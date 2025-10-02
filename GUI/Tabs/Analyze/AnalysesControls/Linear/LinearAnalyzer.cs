using System.Collections.Generic;
using System.IO;
using System.Text;
using GUI.Util;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.IO;
using LinearAnalysisType = InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE;

namespace GUI.Tabs.Analyze.AnalysesControls.Linear
{

    public class LinearAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Extra parameter used by the analysis. When the intervaltype is:
        ///  - FixedNumberOfIntervals, this parameter represents the number of parameters
        ///  - FixedIntervalLength, this parameter represents the interval length
        /// </summary>
        private readonly ulong IntervalParam;

        /// <summary>
        /// The type of linear analysis that will be performed.
        /// </summary>
        private readonly LinearAnalysisType LinearAnalysisType;

        /// <summary>
        /// The value of the pause threshold of the last returned Analyser (using the GetAnalyser method).
        /// </summary>
        private readonly ulong PauseThreshold;

        private readonly bool Condensed;

        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Linear";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "LA";

        public LinearAnalyzer(ulong intervalParam, LinearAnalysisType linearType, ulong pauseThreshold, bool condensed)
        {
            IntervalParam = intervalParam;
            LinearAnalysisType = linearType;
            PauseThreshold = pauseThreshold;
            Condensed = condensed;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            // Extra parameters, the docpath is required for the revision grouping of the linear analysis.
            var extraParameters = new Dictionary<string, object> { { "docPath", OrgDocPath } };

            // List of keys that act as control keys (these should be represented with '+' when 
            // they are present in the keyboard state).
            // e.g if LSHIFT is part of ControlKeys, then pressing LSHIFT and 'a' at the same time 
            // will result in LSHIFT + A in the representation. If LSHIFT is not part of ControlKeys, then pressing LSHIFT
            //  and  'a' at the same time will result in 2 different keystrokes.
            var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);
            return new LinearAnalysis(Events, SessionId, LinearAnalysisType,
                PauseThreshold, IntervalParam, controlKeys, extraParameters);
        }

        protected override bool BeforeWrite(IAnalysisWriter writer)
        {
            // 'True' if a special condensed linear analysis is to be added to the regular linear analysis.
            LinearAnalysisXMLWriter.IsSpecialLinear = Condensed;
            return true;
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

            var buf = new StringBuilder(Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name)));
            buf.Append("_PT" + PauseThreshold);
            switch (LinearAnalysisType)
            {
                case LinearAnalysisType.FIXED_NUMBER_OF_INTERVALS:
                    buf.Append("_FN" + IntervalParam);
                    break;

                case LinearAnalysisType.FIXED_LENGTH_INTERVALS:
                    buf.Append("_FL" + (IntervalParam / 1000));
                    break;

                case LinearAnalysisType.FOCUS_INTERVALS:
                    buf.Append("_FI");
                    break;

                case LinearAnalysisType.REVISION_INTERVALS:
                    buf.Append("_RI");
                    break;

                case LinearAnalysisType.PAUSE_INTERVALS:
                    buf.Append("_PI");
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
            parameters[LinearAnalysisXMLWriter.PAUSE_THRESHOLD_PARAMETER] = PauseThreshold;
            switch (LinearAnalysisType)
            {
                case LinearAnalysis.TYPE.FIXED_LENGTH_INTERVALS:
                    parameters[LinearAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Length Intervals";
                    parameters["Length of Interval (sec)"] = IntervalParam / 1000;
                    break;
                case LinearAnalysis.TYPE.FIXED_NUMBER_OF_INTERVALS:
                    parameters[LinearAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Number of Intervals";
                    parameters["Number of Intervals"] = IntervalParam;
                    break;
                case LinearAnalysis.TYPE.FOCUS_INTERVALS:
                    parameters[LinearAnalysisXMLWriter.TYPE_PARAMETER] = "Focus-Based Intervals";
                    break;
                case LinearAnalysis.TYPE.REVISION_INTERVALS:
                    parameters[LinearAnalysisXMLWriter.TYPE_PARAMETER] = "Revision-Based Intervals";
                    break;
                case LinearAnalysis.TYPE.PAUSE_INTERVALS:
                    parameters[LinearAnalysisXMLWriter.TYPE_PARAMETER] = "Pause-Based Intervals";
                    break;

            }
            return dict;
        }
    }
}
