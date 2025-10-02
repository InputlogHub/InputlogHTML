using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using InputLog.Core.Analyses.Bigram;
using System.Drawing;

namespace GUI.Visualization
{
    /// <summary>
    /// Displays a chart containing logged process data from a General Analysis.
    /// Series can be toggled on or off
    /// </summary>
    public partial class BigramGraph : Visualization
    {
        private IEnumerable<BigramAnalysisSummary.AlphaBigramInfo> Bigrams;
        private Series MeanSeries;
        private Series MedianSeries;
        private BigramAnalysis.Average ActiveSeries;
        private int XInt;
        private int YInt;
        private double YIntMult;

        /// <summary>
        /// Constructor
        /// </summary>
        public BigramGraph(BigramAnalysis.Average avgStat)
        {
            InitializeComponent();
            ActiveSeries = avgStat;
            ValueComboBox.Items.Add(BigramAnalysis.Average.MEAN);
            ValueComboBox.Items.Add(BigramAnalysis.Average.MEDIAN);
            ValueComboBox.SelectedItem = avgStat;
        }

        protected override Chart GetChart()
        {
            return chart1;
        }

        /// <summary>
        /// Reads Visualization data from a General Analysis and displays them.
        /// </summary>
        public void Process(IEnumerable<BigramAnalysisSummary.AlphaBigramInfo> bigrams)
        {
            Bigrams = bigrams;
            MeanSeries = chart1.Series["Mean"];
            MedianSeries = chart1.Series["Median"];
            MeanSeries.MarkerColor = Color.Teal;
            MedianSeries.MarkerColor = Color.Orange;
            DrawPoints();
        }

        /// <summary>
        /// Adds a given list of VisualizationDataPoints to the chart.
        /// </summary>
        private void DrawPoints()
        {
            int yMax = 0;
            double xMaxD = 0.0;

            foreach (BigramAnalysisSummary.AlphaBigramInfo big in Bigrams)
            {
                int y = big.Count;
                if (y > yMax) yMax = y;
                double x1 = big.GetMean();
                double x2 = big.GetMedian();
                if (x1 > xMaxD) xMaxD = x1;
                if (x2 > xMaxD) xMaxD = x2;
                var p1 = new DataPoint(x1, y) {ToolTip = big.Key + ": " + "(" + x1.ToString("0") + ";" + y + ")"};
                //p1.Label = big.Key;
                MeanSeries.Points.Add(p1);
                var p2 = new DataPoint(x2, y) {ToolTip = big.Key + ": " + "(" + x2.ToString("0") + ";" + y + ")"};
                //p2.Label = big.Key;
                MedianSeries.Points.Add(p2);
            }
            int xMax = Convert.ToInt32(Math.Ceiling(xMaxD));
            XInt = VisualizationExtensions.RoundTo(xMax / 10, 10);
            YInt = VisualizationExtensions.RoundTo(yMax / 5, 5);
            YIntMult = Math.Ceiling((double)(yMax + 1) / YInt);
            chart1.ChartAreas["MainArea"].AxisY.Minimum = 0;
            chart1.ChartAreas["MainArea"].AxisY.Interval = YInt;
            chart1.ChartAreas["MainArea"].AxisY.Maximum = YInt * YIntMult;
            chart1.ChartAreas["MainArea"].AxisX.Minimum = 0;
            chart1.ChartAreas["MainArea"].AxisX.Interval = XInt;
            chart1.ChartAreas["MainArea"].AxisX.Maximum = XInt * 10;
        }

        public void Draw()
        {
            chart1.Series.Clear();
            chart1.ChartAreas["MainArea"].AxisY.Minimum = 0;
            chart1.ChartAreas["MainArea"].AxisY.Interval = YInt;
            chart1.ChartAreas["MainArea"].AxisY.Maximum = YInt * YIntMult;
            chart1.ChartAreas["MainArea"].AxisX.Minimum = 0;
            chart1.ChartAreas["MainArea"].AxisX.Interval = XInt;
            chart1.ChartAreas["MainArea"].AxisX.Maximum = XInt * 10;
            if (ActiveSeries == BigramAnalysis.Average.MEAN)
                chart1.Series.Add(MeanSeries);
            else if (ActiveSeries == BigramAnalysis.Average.MEDIAN)
                chart1.Series.Add(MedianSeries);
        }

        private void ValueComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            var newType = (BigramAnalysis.Average)ValueComboBox.SelectedIndex;
            if (newType == ActiveSeries) return;
            ActiveSeries = newType;
            Draw();
        }

        protected override SaveFileDialog GetSaveDialog()
        {
            return VisualSaveFileDialog;
        }

        private void VisualSaveButtonClick(object sender, EventArgs e)
        {
            SelectFormat();
        }

    }
}