using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using InputLog.Core.Analyses.General;
using InputLog.Core.Reporting;

namespace GUI.Visualization
{
    /// <summary>
    /// Displays a chart containing logged process data from a General Analysis.
    /// Series can be toggled on or off
    /// </summary>
    public partial class ProcessGraph : Visualization, IReportData
    {
        private Series FocusSeries;
        private bool FocusShown;
        private string MainDocument;
        private bool PauseShown = true;
        private int PauseThreshold = 1000;
        private List<KeyValuePair<DateTime, int>> Pauses;
        private DateTime BaseDate;
        private Series PausesSeries;
        private Series PositionSeries;
        private bool PositionShown = true;
        private Series ProcessSeries;
        private bool ProcessShown = true;
        private Series ProductSeries;
        private bool ProductShown = true;
        private Font ThisFont;
        private Point? PrevPosition;
        private List<ProcessGraphDataPoint> Points;
        public double FixedY1Max { private get; set; }     
        public int FixedY2Max { private get; set; }
        private double Y1Min;
        private int PauseCount;
        private int OutlierLimit;
        private int OutlierMax;
        private int OutlierCount;
        private double OutlierAvrge;
        private bool OutlierShown = true;
        private readonly ToolTip ThisTooltip = new ToolTip();

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessGraph()
        {
            InitializeComponent();        
            ThisTooltip.InitialDelay = 50;
            chart1.MouseMove += Chart1MouseMove;
            OutlierLimit = 3000;
            OutlierLimitTxtBx.Text = OutlierLimit.ToString();
            BaseDate = DateTime.Today;
        }

        protected override Chart GetChart()
        {
            return chart1;
        }

        /// <summary>
        /// TODO: Implement / Refactor
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ReportMethod> GetBoundReportTargets()
        {
            return new Dictionary<string, ReportMethod>();
        }

        /// <summary>
        /// Reads Visualization data from a General Analysis and displays them.
        /// </summary>
        /// <param name="summ">The list with analysis results</param>
        /// <param name="pauseThreshold"></param>
        /// <param name="mainDoc">The name of the main document in this analysis</param>
        public void Process(GeneralAnalysisSummary summ, int pauseThreshold, string mainDoc)
        {
            PauseThreshold = pauseThreshold;
            MainDocument = mainDoc.Equals(string.Empty) ? "Wordlog" : mainDoc;
            Points = ProcessGraphDataPoint.ReadFromGeneralAnalysisSummary(summ, MainDocument);
            DrawPoints(Points);
        }

        /// <summary>
        /// Adds a given list of VisualizationDataPoints to the chart.
        /// </summary>
        private void DrawPoints(List<ProcessGraphDataPoint> points)
        {
            ThisFont = new Font("Microsoft sans serif", 8.25F, FontStyle.Regular);
            OutlierMax = 0;
            OutlierAvrge = 0.0;
            OutlierCount = 0;
            PauseCount = 0;
            double y1Max = 0;
            Y1Min = 0;
            int y2Max = 0;

            // Copies the max. value as set by the user to the outlier maximum.
            if (FixedY1Max > 0)
            {
                OutlierLimitTxtBx.Text= Convert.ToInt16(FixedY1Max).ToString();
            }

            ProcessSeries = chart1.Series["Process"];
            PositionSeries = chart1.Series["Cursor Position"];
            ProductSeries = chart1.Series["Product"];
            PausesSeries = chart1.Series["Pauses"];
            FocusSeries = chart1.Series["Focus"];

            // X axis labels.
            SetInterval(BaseDate.AddMilliseconds(points.First().StartTime),
                BaseDate.AddMilliseconds(points.Last().StartTime));

            Pauses = new List<KeyValuePair<DateTime, int>>();
			string lastFocus = "";

            foreach (ProcessGraphDataPoint point in points)
            {
                var x = BaseDate.AddMilliseconds(point.StartTime);

                // DocLength should not be greater than the number of characters produced.
                // This happens at the start where the doc has already 1 char and the
                // process is still at 0.
                if (point.DocLength > point.Characters)
                {
                    point.DocLength = point.Characters;
                }
                ProductSeries.Points.AddXY(x, point.DocLength);
                ProcessSeries.Points.AddXY(x, point.Characters);
                PositionSeries.Points.AddXY(x, point.Position);

                // Adding focus points.
                DataPoint p;
                // TODO: check with lead and discuss compatibility with previous versions
                if (point.Focused)
                {
                    p = new DataPoint
                        {
                            XValue = x.ToOADate(),
                            YValues = new[] {(double) 0}
                        };
                }
                else
                {
                    p = new DataPoint
                        {
                            XValue = x.ToOADate(),
                            YValues = new[] {(double) 1},
                            Tag = point.Focus,
                            MarkerStyle = MarkerStyle.Circle,
                            MarkerSize = 4,
                            MarkerColor = Color.Brown
                        };
                }

                FocusShown = true;
                // FocusSeries.Points.Add(p);   
                if (lastFocus != point.Focus)
                {
                    FocusSeries.Points.Add(p); // only add if different from lastFocus    
                }
                lastFocus = point.Focus;				

                // Calculates the x,y coordinates for the pause time series and some outlier statistics. 
                Pauses.Add(new KeyValuePair<DateTime, int>(x, point.PauseTime));

                if (point.PauseTime < OutlierLimit)
                {
                    PauseCount++;
                    if (point.PauseTime > y1Max) y1Max = point.PauseTime;
                }
                else
                {
                    OutlierCount++;
                    OutlierAvrge += point.PauseTime;
                    if (point.PauseTime > OutlierMax) OutlierMax = point.PauseTime;
                }

                if (point.Characters > y2Max) y2Max = point.Characters;
            }

            var outlierPrct = (double) (OutlierCount*100)/(PauseCount + OutlierCount);
            OutlierCountLbl2.Text = OutlierCount + " (" + Math.Round(outlierPrct, 1) + "%)";
            if (OutlierCount > 0)
            {
                OutlierAvrgLbl2.Text = string.Format("{0:0,0.00}", (OutlierAvrge/OutlierCount)/1000);
            }
            OutlierMaxLbl2.Text = string.Format("{0:0,0.00}", OutlierMax/1000);

            // The lower boundary cannot be > than the maximum value.
            // If so, we fall back on a minimum pause time.
            if (PauseThreshold >= y1Max)
            {
                PauseThreshold = 10;
            }

            Y1Min = PauseThreshold;

            // The overridden values should be greater than the calculated values.
            // If not, we discard the manually set value.
            if (FixedY1Max <= y1Max)
            {
                FixedY1Max = 0;
            }

            if (FixedY2Max <= y2Max || FixedY2Max == 0)
            {
                FixedY2Max = 0;
                y2Max = VisualizationExtensions.RoundTo(y2Max/5, 5)*5;
            }
            else
            {
                FixedY2Max = VisualizationExtensions.RoundTo(FixedY2Max/5, 5)*5;
            }

            chart1.ChartAreas["MainArea"].AxisY.Minimum = Y1Min;
            chart1.ChartAreas["MainArea"].AxisY.Maximum = FixedY1Max.Equals(0) ? y1Max : FixedY1Max;
            chart1.ChartAreas["MainArea"].AxisY.LabelStyle.Format = "###,###";
            chart1.ChartAreas["MainArea"].AxisY2.Maximum = FixedY2Max == 0 ? y2Max : FixedY2Max;
            chart1.ChartAreas["MainArea"].AxisY2.LabelStyle.Format = "###,###";
            chart1.ChartAreas["FocusArea"].AxisY2.Minimum = 0;
            chart1.ChartAreas["FocusArea"].AxisY2.Maximum = 1;
            chart1.ChartAreas["FocusArea"].AxisY2.CustomLabels.Add(0, 0.1, " Main Document");
            chart1.ChartAreas["FocusArea"].AxisY2.CustomLabels.Add(0.9, 1, " Sources");

            VisPauseThreshold.Value = PauseThreshold;
        }

        /// <summary>
        /// Determines the parameters for the X-axis: time span, labels, format.
        /// </summary>
        /// <param name="first">The start time of the first event</param>
        /// <param name="last">The start time of the last event</param>
        private void SetInterval(DateTime first, DateTime last)
        {
            chart1.ChartAreas["MainArea"].AxisX.Minimum = first.ToOADate();
            chart1.ChartAreas["MainArea"].AxisX.Maximum = last.ToOADate();
            chart1.ChartAreas["MainArea"].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            chart1.ChartAreas["MainArea"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart1.ChartAreas["MainArea"].AxisX.LabelStyle.Font = ThisFont;
            chart1.ChartAreas["MainArea"].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart1.ChartAreas["MainArea"].AxisX.MajorTickMark.Enabled = true;
            chart1.ChartAreas["MainArea"].AxisX.MinorTickMark.Enabled = true;

            chart1.ChartAreas["FocusArea"].AxisX.Minimum = first.ToOADate();
            chart1.ChartAreas["FocusArea"].AxisX.Maximum = last.ToOADate();
            chart1.ChartAreas["FocusArea"].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            chart1.ChartAreas["FocusArea"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        }

        private void RedrawSeries()
        {
            chart1.Series.Clear();
            chart1.ChartAreas["MainArea"].AxisY.Minimum = Y1Min;
            AddProcessSeries();
            AddProductSeries();
            AddPositionSeries();
            AddPausesSeries();
            AddFocusSeries();
            AddOutlierBox();
        }

        private void AddProductSeries()
        {
            if (ProductShown)
            {
                chart1.Series["Product"] = ProductSeries;
            }
            else
            {
                chart1.Series.Add("Product");
                chart1.Series["Product"].ChartType = SeriesChartType.Line;
                chart1.Series["Product"].XValueType = ChartValueType.Time;
                chart1.Series["Product"].Color = Color.Green;
                chart1.Series["Product"].YAxisType = AxisType.Secondary;
            }
        }

        private void AddPositionSeries()
        {
            if (PositionShown)
            {
                chart1.Series["Cursor Position"] = PositionSeries;
            }
            else
            {
                chart1.Series.Add("Cursor Position");
                chart1.Series["Cursor Position"].ChartType = SeriesChartType.Line;
                chart1.Series["Cursor Position"].XValueType = ChartValueType.Time;
                chart1.Series["Cursor Position"].Color = Color.DarkGreen;
                chart1.Series["Cursor Position"].BorderDashStyle = ChartDashStyle.Dash;
                chart1.Series["Cursor Position"].YAxisType = AxisType.Secondary;
            }
        }

        private void AddProcessSeries()
        {
            if (ProcessShown)
            {
                chart1.Series["Process"] = ProcessSeries;
            }
            else
            {
                chart1.Series.Add("Process");
                chart1.Series["Process"].ChartType = SeriesChartType.Line;
                chart1.Series["Process"].XValueType = ChartValueType.Time;
                chart1.Series["Process"].Color = Color.Blue;
                chart1.Series["Process"].YAxisType = AxisType.Secondary;
            }
        }

        private void AddPausesSeries()
        {
            if (PauseShown)
            {
                chart1.Series["Pauses"] = PausesSeries;
            }
            else
            {
                chart1.Series.Add("Pauses");
                chart1.Series["Pauses"].XValueType = ChartValueType.Time;
                chart1.Series["Pauses"].MarkerStyle = MarkerStyle.Circle;
                chart1.Series["Pauses"].Color = Color.Orange;
                chart1.Series["Pauses"].YAxisType = AxisType.Primary;
            }
        }

        private void AddFocusSeries()
        {
            if (FocusShown)
            {
                chart1.Series["Focus"] = FocusSeries;
            }
            else
            {
                chart1.Series.Add("Focus");
                chart1.Series["Focus"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Focus"].XValueType = ChartValueType.Time;      
                chart1.Series["Focus"].Color = Color.Chocolate;
                chart1.Series["Focus"].YAxisType = AxisType.Primary;
            }
        }

        private void AddOutlierBox()
        {
            if (OutlierShown)
            {
                OutlierBx.Show();
            }
            else
            {
                OutlierBx.Hide();
            }
        }

        private void ProcessToggleClick(object sender, EventArgs e)
        {
            ToggleProcess();
        }

        public void ToggleProcess()
        {
            if (ProcessShown)
            {
                ProcessShown = false;
                ProcessToggle.Text = "(Show)";
                ProcessToggle.ForeColor = Color.LightGray;
            }
            else
            {
                ProcessShown = true;
                ProcessToggle.Text = "(Hide)";
                ProcessToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        private void PositionToggleClick(object sender, EventArgs e)
        {
            TogglePosition();
        }

        public void TogglePosition()
        {
            if (PositionShown)
            {
                PositionShown = false;
                PositionToggle.Text = "(Show)";
                PositionToggle.ForeColor = Color.LightGray;
            }
            else
            {
                PositionShown = true;
                PositionToggle.Text = "(Hide)";
                PositionToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        private void ProductToggleClick(object sender, EventArgs e)
        {
            ToggleProduct();
        }

        public void ToggleProduct()
        {
            if (ProductShown)
            {
                ProductShown = false;
                ProductToggle.Text = "(Show)";
                ProductToggle.ForeColor = Color.LightGray;
            }
            else
            {
                ProductShown = true;
                ProductToggle.Text = "(Hide)";
                ProductToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        private void PausesToggleClick(object sender, EventArgs e)
        {
            TogglePauses();
        }

        public void TogglePauses()
        {
            if (PauseShown)
            {
                PauseShown = false;
                PausesToggle.Text = "(Show)";
                PausesToggle.ForeColor = Color.LightGray;
            }
            else
            {
                PauseShown = true;
                PausesToggle.Text = "(Hide)";
                PausesToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        private void FocusToggleClick(object sender, EventArgs e)
        {
            ToggleFocus();
        }

        public void ToggleFocus()
        {
            if (FocusShown)
            {
                FocusShown = false;
                FocusToggle.Text = "(Show)";
                FocusToggle.ForeColor = Color.LightGray;
            }
            else
            {
                FocusShown = true;
                FocusToggle.Text = "(Hide)";
                FocusToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        private void OutlierToggleClick(object sender, EventArgs e)
        {
            ToggleOutliers();
        }

        public void ToggleOutliers()
        {
            if (OutlierShown)
            {
                OutlierShown = false;
                OutlierToggle.Text = "(Show Outliers)";
                OutlierToggle.ForeColor = Color.LightGray;
            }
            else
            {
                OutlierShown = true;
                OutlierToggle.Text = "(Hide)";
                OutlierToggle.ForeColor = Color.Black;
            }
            RedrawSeries();
        }

        /// <summary>
        /// Recalculating and redrawing the distribution of pause points when the user 
        /// changes the minimum pause threshold in the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisPauseThresholdValueChanged(object sender, EventArgs e)
        {
            PauseThreshold = Convert.ToInt32(VisPauseThreshold.Value);
            if (!PauseShown) return;
            chart1.Series.Remove(PausesSeries);
            chart1.Series.Add("Pauses");
            chart1.Series["Pauses"].ChartType = SeriesChartType.Point;
            chart1.Series["Pauses"].XValueType = ChartValueType.Time;
            chart1.Series["Pauses"].MarkerStyle = MarkerStyle.Circle;
            chart1.Series["Pauses"].Color = Color.Orange;
            chart1.Series["Pauses"].YAxisType = AxisType.Primary;
            PausesSeries = chart1.Series["Pauses"];
            Y1Min = PauseThreshold;
            foreach (var point in Pauses)
            {
                if (point.Value < PauseThreshold || point.Value > OutlierLimit) continue;               
                var p = new DataPoint
                        {
                            XValue = point.Key.ToOADate(),
                            YValues = new[] {(double) point.Value, 1},
                            ToolTip = $"{point.Value:0,0}" + " ms"
                        };
                PausesSeries.Points.Add(p);
            }
            RedrawSeries();
        }

        private void RedrawBtnClick(object sender, EventArgs e)
        {
            RedrawSeries();
        }

        /// <summary>
        /// Captures mouse movements close to FocusArea DataPoints and shows a tooltip with
        /// the focus document name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart1MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (PrevPosition.HasValue && pos == PrevPosition.Value)
            {
                return;
            }
            ThisTooltip.RemoveAll();
            PrevPosition = pos;
            var results = chart1.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType != ChartElementType.DataPoint) continue;
                if (chart1.Series["Focus"].Points.Count <= result.PointIndex) continue;
                var point = chart1.Series["Focus"].Points[result.PointIndex];
                if (point == null) continue;
                var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(point.XValue);
                var pointYPixel = result.ChartArea.AxisY2.ValueToPixelPosition(point.YValues[0]);

                // Returns a ToolTip when the cursor is close to the Focus DataPoint (2 pixels around)
                if (Math.Abs(pos.X - pointXPixel) <= 2 && Math.Abs(pos.Y - pointYPixel) <= 5)
                {
                    if (null != point.Tag && point.Tag.ToString() != string.Empty)
                    {
                        ThisTooltip.Show(point.Tag.ToString(), chart1, pos.X + 2, pos.Y + 2);
                    }
                }
            }
        }

        /// <summary>
        /// The user changed the outlier limit value and the data points are recalculated.
        /// The limit must be larger than the minimum of the Y1 axis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitChanged(object sender, EventArgs e)
        {
            int num;
            // Changing the outlier maximum in the Process Graph overrides the value set by user in the Analyze Panel.
            FixedY1Max = 0;
            int.TryParse(OutlierLimitTxtBx.Text.Trim(), out num);
            if (num < 5 || num < Y1Min + (Y1Min/5))
            {
                MessageBox.Show("The outlier limit must be larger than: " + Math.Max(Y1Min + (Y1Min/5), 5) +
                                ", \nthe minimum value of the left Y-axis.",
                    "Pause Outlier Limit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OutlierLimitTxtBx.Text = OutlierLimit.ToString();
            }
            else
            {
                OutlierLimit = num;
                foreach (var series in chart1.Series.Where(series => !series.Name.Equals("Pauses")))
                {
                    series.Points.Clear();
                }
                Pauses.Clear();
                DrawPoints(Points);
                RedrawSeries();
            }
        }

        /// <summary>
        /// Manually saving the chart. The chart was already saved automatically upon creation.
        /// </summary>
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