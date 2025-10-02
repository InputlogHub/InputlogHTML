using System;
using InputLog.Core.Analyses;

namespace GUI.Tabs.Analyze.AnalysesControls.WordPauses
{
    public class WordPausesAnalyzer : AbstractAnalyzer
    {
        #region Fields
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Word Pauses";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "WP";

        private readonly string[] TargetPaths;
        /// <summary>
        /// The Damerau-Levenshtein distance between taget and production.
        /// </summary>
        private readonly int WordDistance;
        #endregion

        public WordPausesAnalyzer(string targetPath, string participantPath, int wordDistance)
        {
            if (!targetPath.Equals(string.Empty) && !participantPath.Equals(string.Empty))
            {
                TargetPaths = new[] {targetPath , participantPath};
            }
            WordDistance = wordDistance;
        }

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            // Events from the logfile on which this analysis is performed.</param>
            // "OrgDocPath" the path of the document in the original state (before the logging happened).</param>
            // "SessionId", dictionary containing the session identificication for the logfile on which
            // this analysis is performed.
            // "ABBR", abbreviation for the analysis
            // TargetPath and ParticipantPath point to two optional csv files with specific words
            // that could be contained in the text.
            // Returns the analyzer that implements the actual analysis.
            return new InputLog.Core.Analyses.WordPauses.WordPausesAnalysis(Events, OrgDocPath, SessionId, ABBR, TargetPaths, WordDistance);
        }
    }
}
