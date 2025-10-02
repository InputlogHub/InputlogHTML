using InputLog.Core.Analyses;
using InputLog.Core.IO;

namespace GUI.Tabs.Analyze.AnalysesControls.Token
{
    public class TokenAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Token Analyzer";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "TA";

        private readonly string CsvPath;

        public TokenAnalyzer(string csvPath)
        {
            CsvPath = csvPath;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.TokenAnalysis(Events, OrgDocPath,
               SessionId, ABBR, CsvPath);
        }

        /// <summary>
        /// Given the destination folder and a source (log) file, this method returns a
        /// full path of the file where the result of the analysis should be stored. 
        /// </summary>
        /// <param name="sessionId">The session identification.</param>
        /// <param name="srcFile">The source (log) file</param>
        /// <param name="destDir">The destination directory path.</param>
        /// <returns>String with full file path of the file where the result of the analysis should be stored</returns>
        protected override string ExtendFileName(SessionIdentification sessionId, string srcFile, string destDir)
        {
            var name = base.ExtendFileName(sessionId, srcFile, destDir);
            return name;
        }
    }
}
