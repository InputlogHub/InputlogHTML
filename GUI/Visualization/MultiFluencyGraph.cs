using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using GUI.Tabs.Analyze.AnalysesControls.Fluency;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.Analyses.Linear;

namespace GUI.Visualization
{
    /// <summary>
    /// Displays a chart containing logged process data from a General Analysis.
    /// Series can be toggled on or off
    /// </summary>
    public class MultiFluencyGraph : FluencyGraph
    {
        private readonly Dictionary<string, FluencyAnalysisSummary> Summaries;
        private int MultiCount;

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiFluencyGraph()
        {
            Summaries = new Dictionary<string, FluencyAnalysisSummary>();
            LegendsPerSeries = 3;
            Count = 0;
            MultiCount = 0;
        }

        public MultiFluencyGraph(int trendLineDegree) : base(trendLineDegree) 
        {
            Summaries = new Dictionary<string, FluencyAnalysisSummary>();
            LegendsPerSeries = 3;
            Count = 0;
            MultiCount = 0;
        }

        public bool IsReady()
        {
            return MultiCount == FluencyAnalyzer.FileCount;
        }

        public override void Draw()
        {   
            maxTypeSelect.SelectedItem = "Absolute";
            RedrawSeries();
        }

        /// <summary>
        /// Reads Visualization data from a FluencyAnalysisSummary and displays them.
        /// </summary>
        /// <param name="idfx"></param>
        /// <param name="summary"></param>
        public void Process(string idfx, FluencyAnalysisSummary summary)
        {
            CurrentLinearType = summary.LinearType;
            string name = Path.GetFileNameWithoutExtension(idfx);
            if (Summaries.Keys.Contains(name))
            {
                Process(name + "_(2)", summary);
                return;
            }
            if (name != null)
            {
                Summaries[name] = summary;
                InitSeries(name);
                InitTaskMaximumZone(name + " TOZ");
            }
            Count++;
            MultiCount++;
        }

        protected override void RedrawSeries()
        {
            chart1.Series.Clear();
            Count = 0;
            foreach (string idfx in Summaries.Keys)
            {
                AddSeries(idfx);
                AddSeriesPoints(idfx, Summaries[idfx]);
                AddTaskMaximumZone(idfx + " TOZ", Summaries[idfx]);
                AddTaskMaximumZonePoints(idfx + " TOZ", Summaries[idfx]);
            }
            chart1.ChartAreas["MainArea"].AxisY.LabelStyle.Font = ThisFont;
        }

        protected override void AddLabel(int pos, string pId)
        {
            if (CurrentLinearType == LinearAnalysis.TYPE.FIXED_NUMBER_OF_INTERVALS)
            {
                var cl = new CustomLabel {FromPosition = pos - 1, ToPosition = pos, Text = pos.ToString()};
                chart1.ChartAreas["MainArea"].AxisX.CustomLabels.Add(cl);
            }
            else
            {
                base.AddLabel(pos, pId);
            }
        }
    }
}