using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GUI.Util;
using GUI.Visualization;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Bigram;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Bigram
{
    public class BigramAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Bigram";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "BA";

        public BigramGraph Graph;

        private readonly string GraphImageExtension;

        private readonly BigramAnalysis.Average AvgStat;

        private readonly int PauseThreshold;

        private string BigramInfoPath;

        public BigramAnalyzer(string graphImageExtension, BigramAnalysis.Average avgStat, int pauseThreshold, string bigramInfoPath)
        {
            GraphImageExtension = graphImageExtension;
            AvgStat = avgStat;
            PauseThreshold = pauseThreshold;
            BigramInfoPath = bigramInfoPath;
        }

        private string GetBigramInfoPath()
        {
            const string defaultPath = @"/Analyses/Bigram/Default.xml";
            string dirPath = Path.GetDirectoryName(Application.ExecutablePath);
            string lang = SessionId.GetLanguage();
            if (lang == null || lang.Equals("Other"))
            {
                return dirPath + defaultPath;
            }
            string filename = @"/Analyses/Bigram/" + lang + @".xml";
            if (!File.Exists(filename)) return dirPath + defaultPath;
            return dirPath + filename;
        }

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new BigramAnalysis(Events, SessionId, AvgStat, PauseThreshold, BigramUtils.ReadBasicBigramInfo(BigramInfoPath));
        }

        protected override void AfterAnalysis(Analysis analysis, IAnalysisSummary isummary)
        {
            var summary = (BigramAnalysisSummary)isummary;
            Graph = new BigramGraph(AvgStat);
            Graph.Process(summary.AlphaGrams.Values);
        }

        protected override void AfterWrite(IAnalysisWriter writer)
        {
            string fileName = OutputFilePath.Substring(0, OutputFilePath.Length - 4) + "." + GraphImageExtension;
            Graph.Draw();
            Graph.Save(fileName, ChartImageUtils.StrToImageFormat(GraphImageExtension));
        }

        /// <summary>
        /// Creates the dictionary of parameters that should be passed to the analysiswriter. 
        /// -copy from linear-
        /// </summary>
        /// <returns>A dictionary containing the various parameters.</returns>
        protected override IDictionary<string, IDictionary<string, object>> GetParameters()
        {
            var dict = new Dictionary<string, IDictionary<string, object>>();
            var parameters = new Dictionary<string, object>();
            dict["Parameters"] = parameters;
            switch (AvgStat)
            {
                case BigramAnalysis.Average.MEAN:
                    parameters["Average statistic"] = "Mean";
                    break;
                case BigramAnalysis.Average.MEDIAN:
                    parameters["Average statistic"] = "Median";
                    break;
            }
            parameters["Pause threshold"] = PauseThreshold;
            if (BigramInfoPath == null) BigramInfoPath = GetBigramInfoPath();
            parameters["Bigram info file"] = BigramInfoPath;
            return dict;
        }
    }
}
