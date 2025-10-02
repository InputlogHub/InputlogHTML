using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace InputLog.Core.Reporting.Graphs
{
    /// <summary>
    ///     A chart that can be added to reports.
    /// </summary>
    public class ReportChart
    {
        /// <summary>
        ///     Reference to the in-memory chart
        /// </summary>
        protected Chart _plot;

        /// <summary>
        ///     The area of the plot. Only has one area.
        /// </summary>
        protected ChartArea _area;

        /// <summary>
        ///     Instantiate a new report chart
        /// </summary>
        public ReportChart()
        {
            this._plot = new Chart();
            this._area = new ChartArea("Default");
            this._plot.ChartAreas.Add(this._area);
            this._area.AxisX2.Enabled = AxisEnabled.False;
            this._area.AxisY2.Enabled = AxisEnabled.False;
        }

        /// <summary>
        ///     Set the name of the Y axis.
        /// </summary>
        /// <param name="name">Name of the y axis</param>
        public void SetYAxisName(string name)
        {
            this._area.AxisY.Title = name;
        }

        /// <summary>
        ///     Set the height of the plot in pixels.
        /// </summary>
        /// <param name="height">Height of plot</param>
        /// <param name="width">Width of plot</param>
        public void SetSize(int width, int height)
        {
            Debug.Assert(height > 0, "Negative height dimensions for boxplot");
            Debug.Assert(width > 0, "Negative width dimensions for boxplot");
            this._plot.Size = new System.Drawing.Size(width, height);
        }

        /// <summary>
        ///     Specify the minimum value on the y axis and 
        ///     the maximum value.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetYAxisScale(int min, int max)
        {
            Debug.Assert(min < max);
            this._area.AxisY.Minimum = min;
            this._area.AxisY.Maximum = max;
        }

        /// <summary>
        ///     Saves the graph to a steam
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToStream(Stream stream)
        {
            this._plot.SaveImage(stream, ChartImageFormat.Png);
        }
    }
}
