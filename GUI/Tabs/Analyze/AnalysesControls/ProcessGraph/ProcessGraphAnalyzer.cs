using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GUI.Util;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.General;
using InputLog.Core.Analyses.ProcessGraphAnalysis;
using InputLog.Core.IO;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace GUI.Tabs.Analyze.AnalysesControls.ProcessGraph
{
    public class ProcessGraphAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Process Graph";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "PG";

        private readonly string GraphImageExtension;

        private readonly ulong PauseThreshold;

        private readonly List<string> VisibleSeries;

        public Visualization.ProcessGraph Graph;

        private ProcessGraphAnalysisSummary Summary;

        private List<KeysEx> CtrlKeys;

        public string ImageFilename;

        public ProcessGraphAnalyzer(string graphImageExtension, ulong pauseThreshold, List<string> visibleSeries,
            List<KeysEx> ctrlKeys)
        {
            PauseThreshold = pauseThreshold;
            GraphImageExtension = graphImageExtension;
            VisibleSeries = visibleSeries;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new ProcessGraphAnalysis(Events, SessionId, CtrlKeys);
        }

        protected override void AfterAnalysis(Analysis analysis, IAnalysisSummary summary)
        {
            Summary = (ProcessGraphAnalysisSummary)summary;
            GeneralAnalysisSummary gaSumm = Summary.GASummary;
            Graph = new Visualization.ProcessGraph();
            var y1 = VisibleSeries.FirstOrDefault(s => s.Contains("Y1"));
            var y2 = VisibleSeries.FirstOrDefault(s => s.Contains("Y2"));
            if (y1 != null) Graph.FixedY1Max = System.Convert.ToInt32(y1.Substring(3));
            if (y2 != null) Graph.FixedY2Max = System.Convert.ToInt32(y2.Substring(3));
            Graph.Process(gaSumm, (int)PauseThreshold,  analysis.MainDocument);
            if (!VisibleSeries.Contains("Process")) Graph.ToggleProcess();
            if (!VisibleSeries.Contains("Product")) Graph.ToggleProduct();
            if (!VisibleSeries.Contains("Position")) Graph.TogglePosition();
            if (!VisibleSeries.Contains("Pauses")) Graph.TogglePauses();
            if (!VisibleSeries.Contains("Focus")) Graph.ToggleFocus();
            if (!VisibleSeries.Contains("Outliers")) Graph.ToggleOutliers();

            // Save graph in image format to an in-memory stream in the ProgressGraphAnalysisSummary.
            Graph.SaveToStream(this.Summary.ImageBuffer, ChartImageUtils.StrToImageFormat(GraphImageExtension));
        }

        protected override bool BeforeWrite(IAnalysisWriter writer)
        {
            ImageFilename = PathSanitizer.Uniquify(Path.ChangeExtension(OutputFilePath, GraphImageExtension));
            //Graph.Save(ImageFilename, ChartImageUtils.StrToImageFormat(GraphImageExtension));

            if (Summary != null)
            {
                var fStream = new FileStream(ImageFilename, FileMode.CreateNew);
                Summary.ImageBuffer.Seek(0, SeekOrigin.Begin);
                Summary.ImageBuffer.CopyTo(fStream);
                fStream.Close();
            }
            
            return false;
        }

        /// <summary>
        /// Given the destination folder and a source (log) file, this method returns a
        /// full path of the file where the result of the analysis should be stored. This pathname may contain 
        /// place holders for session identification information using the ${key} notation.
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
            buf.Append(Path.GetExtension(name));

            return buf.ToString();
        }
    }
}
