using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Focus;

namespace GUI.Tabs.Analyze.AnalysesControls.Source
{
    public class FocusAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Source";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "SO";

        private readonly bool AddPajek;

        /// <summary>
        /// FixedNumberOfIntervals: the number of intervals 
        /// </summary>
        private readonly int _fixedNumberOfIntervals;

        public FocusAnalyzer(int intervalParam, bool addPajek)
        {
            _fixedNumberOfIntervals = intervalParam;
            AddPajek = addPajek;
        }
        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            return new FocusAnalysis(Events, SessionId, _fixedNumberOfIntervals);
        }

        protected override bool BeforeWrite(IAnalysisWriter writer)
        {
            FocusAnalysisXMLWriter.WritePajekFile = AddPajek;
            return true;
        }
    }
}
