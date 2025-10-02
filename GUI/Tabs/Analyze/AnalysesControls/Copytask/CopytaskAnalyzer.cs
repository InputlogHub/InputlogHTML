using System.Diagnostics;
using System.IO;
using InputLog.Core.Analyses.Copytask;
using InputLog.Core.IO.CSV;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Copytask
{
    internal class CopytaskAnalyzer : AbstractAnalyzer
    {
        // Constants
        public const string NAME = "Copy task";
        public const string ABBR = "CT";
        private bool OutputRawData;

        public CopytaskAnalyzer(bool outputRawData)
        {
            this.OutputRawData = outputRawData;
        }

        /// <summary>
        /// Get the copyTask analysis that will be used to analyze the event data.
        /// </summary>
        /// <returns>The copyTask analysis to be used for analyzing the data.</returns>
        protected override InputLog.Core.Analyses.Analysis GetAnalysis()
        {
            return new CopytaskAnalysis(ABBR, Events, SessionId);
        }

        protected override void AfterAnalysis(
            InputLog.Core.Analyses.Analysis analysis,
            InputLog.Core.Analyses.IAnalysisSummary summary)
        {
            if (!OutputRawData)
            {
                return;
            }

            CopytaskAnalysisSummary summ = summary as CopytaskAnalysisSummary;
            Debug.Assert(summ != null);

            var csvOutput = OutputFilePath;
            csvOutput = Path.ChangeExtension(csvOutput, ".csv");
            csvOutput = PathSanitizer.Uniquify(csvOutput);

            var writer = GetCSVWriter(summ);
            writer.WriteToFile(csvOutput);
        }

        private CSVMergeWriter GetCSVWriter(CopytaskAnalysisSummary summ)
        {
            ICSVLineGetter rawBigrams = new CSVLineGetter(summ.RawBigrams.AsReadOnly());
            ICSVLineGetter sessionInfo = new CSVRepeatGetter(SessionId);

            var writer = new CSVMergeWriter(new ICSVLineGetter[] {rawBigrams, sessionInfo});
            return writer;
        }
    }
}
