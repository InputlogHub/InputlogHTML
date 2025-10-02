using System.Collections.Generic;

namespace WebApp.Models
{
    public class AnalysisIndexViewModel
    {
        public readonly List<AnalysisModel> Analyses;

        public AnalysisIndexViewModel(List<AnalysisModel> analyses)
        {
            Analyses = analyses;
        }
    }

}