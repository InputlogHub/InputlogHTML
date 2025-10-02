using System.IO;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Plugin.WordLog;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Revision
{
    public class RevisionMatrixAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Whether a heat map should be produced
        /// </summary>
        private readonly bool _includeHeatmap;

        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Revision";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "RM";

        /// <summary>
        /// Path for the heat map docx file
        /// </summary>
        public string HMPath { get; private set; }

        public RevisionMatrixAnalyzer(bool includeHeatmap)
        {
            _includeHeatmap = includeHeatmap;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new RevisionMatrixAnalysis(Events, SessionId, OrgDocPath, IsReportGenerator);
        }

        protected override void AfterWrite(IAnalysisWriter writer)
        {
            if (!_includeHeatmap) return;
            string tmp = Path.GetFileNameWithoutExtension(OutputFilePath);
            HMPath = PathSanitizer.Uniquify(GetDestinationDir() + "/" + tmp + "_HM.docx");
            InputlogDocument doc;
            if (File.Exists(OrgDocPath))
            {
                // Copy original document and add heat map at the end
                File.Copy(OrgDocPath, HMPath, true);
                doc = new InputlogDocument(HMPath, true);
            }
            else
            {
                // Create document to start heat map
                doc = InputlogDocument.Create(HMPath);
            }
            doc.Word.Visible = false;
            var hm = new Heatmap((RevisionMatrixSummary)CurrentSummary, doc);
            hm.Process();
        }
    }

}
