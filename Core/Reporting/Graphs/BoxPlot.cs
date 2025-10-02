using System;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace InputLog.Core.Reporting.Graphs
{
    /// <summary>
    ///     This creates an in-memory representation of a box plot with
    ///     a variable number of series in it.
    /// </summary>
    public class BoxPlot: ReportChart
    {
        

        //
        // Identifies series elements
        //
        private const int LOWER_WHISKER = 0;
        private const int UPPER_WHISKER = 1;
        private const int LOWER_BOX = 2;
        private const int UPPER_BOX = 3;
        private const int AVERAGE = 4;
        private const int MEDIAN = 5;
        private const int UNUSUAL_POINTS = 6;

        private const int NR_OF_VALUES = 6;

        private int seriesCounter = 0;
        private const string DATA_PREFIX = "data-series-";

        public BoxPlot() 
        {
            this._area.AxisX.MajorGrid.Enabled = false;
        }

        //public void AddSeries(string name,
        //    double[] values,
        //    int confidenceInterval = 5,
        //    bool showAverage = true,
        //    bool showMedian = true,
        //    bool showUnusual = false
        //) {
        //    // If the series doesn't exist yet, add it. Otherwise update it.
        //    if (!this._plot.Series.Any(series => series.Name == name))
        //    {
        //        Series series = new Series(name);
        //        series.ChartType = SeriesChartType.BoxPlot;
        //        series.ChartArea = "Default";
        //        this._plot.Series.Add(series);
        //    }

        //    string dataId = DATA_PREFIX + seriesCounter.ToString();
        //    this._plot.Series.Add(new Series(dataId));
        //    this._plot.Series[dataId].Points.DataBindY(values);

        //    this._plot.Series[name]["BoxPlotSeries"] = dataId;
        //    this._plot.Series[name]["BoxPlotWhiserPercentile"] = confidenceInterval.ToString();
        //    this._plot.Series[name]["BoxPlotShowAverage"] = showAverage.ToString(); ;
        //    this._plot.Series[name]["BoxPlotShowMedian"] = showMedian.ToString();
        //    this._plot.Series[name]["BoxPlotShowUnusualValues"] = showUnusual.ToString();
        //    this._plot.Series[name].Label = name;

        //    seriesCounter += 1;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="firstQ"></param>
        /// <param name="thirdQ"></param>
        /// <param name="mean"></param>
        /// <param name="median"></param>
        /// <param name="unusual_points"></param>
        public void AddSeries(
            string name,
            double min,
            double max,
            double firstQ,
            double thirdQ,
            double mean,
            double median,
            double[] unusual_points = null
        )
        {
            // If the series doesn't exist yet, add it. Otherwise update it.
            if (!this._plot.Series.Any(series => series.Name == name))
            {
                Series series = new Series(name);
                series.ChartType = SeriesChartType.BoxPlot;
                series.ChartArea = "Default";
                this._plot.Series.Add(series);
            }

            int nrExtraValues = (unusual_points != null) ? unusual_points.Length : 0;
            double[] values = new double[NR_OF_VALUES + nrExtraValues];
            values[LOWER_WHISKER] = min;
            values[UPPER_WHISKER] = max;
            values[LOWER_BOX] = firstQ;
            values[UPPER_BOX] = thirdQ;
            values[AVERAGE] = mean;
            values[MEDIAN] = median;

            int extraIndex = UNUSUAL_POINTS;
            for (int i = 0; i < nrExtraValues; i++)
            {
                values[extraIndex + i] = unusual_points[i];
            }

            this._plot.Series[name].Points.Add(new DataPoint(seriesCounter, values));
            this._plot.Series[name].Points[0].AxisLabel = name;

            System.Drawing.Color seriesColor = this.GetInputlogColor(this.seriesCounter);
            if (seriesColor != System.Drawing.Color.Empty)
            {
                this._plot.Series[name].Color = GetInputlogColor(this.seriesCounter);
            }

            this.seriesCounter += 1;
        }

        private System.Drawing.Color GetInputlogColor(int index)
        {
            int mod = index % 3;
            switch (mod)
            {
                case 0:
                    return System.Drawing.ColorTranslator.FromHtml("#333366");
                case 1:
                    return System.Drawing.ColorTranslator.FromHtml("#993300");
                case 2:
                    return System.Drawing.ColorTranslator.FromHtml("#5F3B42");
                case 3:
                    return System.Drawing.ColorTranslator.FromHtml("#D9E3ED");
                case 4:
                    return System.Drawing.ColorTranslator.FromHtml("#45556A");
                case 5:
                    return System.Drawing.ColorTranslator.FromHtml("#C1B39E");
                default:
                    return System.Drawing.Color.Empty;
            }

        }
    }
}
