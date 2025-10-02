using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.Util;

namespace GUI.Visualization
{
    /// <summary>
    /// Displays a chart containing logged process data from a General Analysis.
    /// Series can be toggled on or off
    /// </summary>
    public partial class FluencyGraph : Visualization
    {
        private const double X_AXIS_PIXELS = 885.0;

        protected enum MaximumTYPE
        {
            ABSOLUTE, PERSONAL, TASK
        };

        private static readonly Color[] Colors = 
        { 
            Color.Blue, Color.Red, Color.DarkGreen, Color.DarkMagenta, Color.Orange, 
            Color.Turquoise, Color.Gray, Color.Navy, Color.DarkRed, Color.Olive
        };

        protected readonly Font ThisFont;
        private readonly Dictionary<string, bool> SeriesShown;
        private readonly Dictionary<string, int> SeriesTrendLineDegrees;
        private readonly Dictionary<Tuple<MaximumTYPE, string>, Series> SeriesCache;
        private FluencyAnalysisSummary Summary;
        protected MaximumTYPE Maximum;
        protected int Count;
        private double MaxY;
        private double MaxX;
        protected LinearAnalysis.TYPE? CurrentLinearType;
        protected int LegendsPerSeries = 2;
        private int TrendLineDegree;

        /// <summary>
        /// Constructor
        /// </summary>
        protected FluencyGraph() : this(3)
        {

        }

        public FluencyGraph(int trendLineDegree)
        {
            ThisFont = new Font("Microsoft sans serif", 8.25F, FontStyle.Regular);
            InitializeComponent();
            SeriesShown = new Dictionary<string, bool>();
            SeriesTrendLineDegrees = new Dictionary<string, int>();
            Count = 0;
            maxTypeSelect.Items.Add("Absolute");
            maxTypeSelect.Items.Add("Task");
            maxTypeSelect.Items.Add("Personal");
            SeriesCache = new Dictionary<Tuple<MaximumTYPE, string>, Series>();
            TrendLineDegree = trendLineDegree;
        }

        protected override Chart GetChart()
        {
            return chart1;
        }

        public void ChangeTrendLineDegree(int newDegree)
        {
            TrendLineDegree = newDegree;
        }

        public virtual void Draw()
        {
            maxTypeSelect.Visible = false;
            maxTypeLabel.Visible = false;
            RedrawSeries();
        }

        /// <summary>
        /// Reads Visualization data from a FluencyAnalysisSummary and displays them.
        /// </summary>
        /// <param name="summary"></param>
        public void Process(FluencyAnalysisSummary summary)
        {
            CurrentLinearType = summary.LinearType;
            Summary = summary;
            InitSeries("Absolute");
            InitSeries("Personal");
            InitSeries("Task");
            InitTaskMaximumZone("Task Maximum Zone");
            maxTypeSelect.SelectedItem = "All";
            ProcessTimeLbl.Text = TimeSpan.FromMilliseconds(Summary.TotalLength).ToString(@"hh\:mm\:ss");
            TaskLbl.Text = Summary.TaskLMaximum.ToString();
            PersonalLbl.Text = Summary.PersonalMaximum.ToString();
        }

        /// <summary>
        /// Format X-axis labels
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pId"></param>
        protected virtual void AddLabel(int pos, string pId)
        {
            CustomLabel cl = new CustomLabel {FromPosition = pos - 1, ToPosition = pos, Text = pId};
            chart1.ChartAreas["MainArea"].AxisX.CustomLabels.Add(cl);
        }

        protected virtual void RedrawSeries()
        {
            Count = 0;          
            chart1.Series.Clear();
            Maximum = MaximumTYPE.ABSOLUTE;
            AddSeries("Absolute");
            AddSeriesPoints("Absolute");
            Maximum = MaximumTYPE.PERSONAL;
            AddSeries("Personal");
            AddSeriesPoints("Personal");
            Maximum = MaximumTYPE.TASK;
            AddSeries("Task");
            AddSeriesPoints("Task");
            AddTaskMaximumZone("Task Maximum Zone");
            AddTaskMaximumZonePoints("Task Maximum Zone");
            chart1.ChartAreas["MainArea"].AxisY.LabelStyle.Font = ThisFont;
            chart1.ChartAreas["MainArea"].AxisX.LabelStyle.Font = ThisFont;
        }

        private void AddTaskMaximumZone(string name)
        {
            AddTaskMaximumZone(name, Summary);
        }

        protected void AddTaskMaximumZone(string name, FluencyAnalysisSummary summary)
        {
            Series maxSeries = chart1.Series.Add(name);
            maxSeries.Color = Color.FromArgb(80, Colors[Count-1]);
            maxSeries.ChartType = SeriesChartType.Column;
            maxSeries["DrawSideBySide"] = "False";
        }

        private void AddTaskMaximumZonePoints(string name)
        {
            AddTaskMaximumZonePoints(name, Summary);
        }

        protected void AddTaskMaximumZonePoints(string name, FluencyAnalysisSummary summary)
        {
            Series maxSeries = chart1.Series[name];
            bool seriesShown = SeriesShown[name];
            if (!seriesShown) return;
            Series cache = GetCached(name);
            if (cache != null)
            {
                maxSeries.Points.AddAll(cache.Points);
                return;
            }
            ulong taskMaximumPeriodBegin = summary.TaskMaximumPeriod.PeriodStart;
            ulong taskMaximumPeriodEnd = summary.TaskMaximumPeriod.PeriodEnd;
            ulong rollingLength = 0;
            double maxBegin = -1.0;
            double maxEnd = -1.0;
            int c = 0;
            foreach (var p in summary.Periods)
            {
                c++;
                LinearAnalysisSummary.AbstractPeriod period = p.Item1;
                ulong periodEnd = rollingLength + period.Length();
                if (maxBegin < 0 && periodEnd > taskMaximumPeriodBegin)
                {
                    double maxPartInThisPeriod = taskMaximumPeriodBegin - rollingLength;
                    maxBegin = c - 1 + (maxPartInThisPeriod / period.Length());
                }
                if (maxEnd < 0 && periodEnd > taskMaximumPeriodEnd)
                {
                    double maxPartInThisPeriod = taskMaximumPeriodEnd - rollingLength;
                    maxEnd = c - 1 + maxPartInThisPeriod / period.Length();
                }
                else if (maxEnd < 0 && c == summary.Periods.Count)
                {
                    maxEnd = c;
                }

                rollingLength += period.Length();
            }
            int pixelsPerInterval = Convert.ToInt32(Math.Ceiling(X_AXIS_PIXELS / MaxX));
            maxSeries["PixelPointWidth"] = Convert.ToInt32(Math.Ceiling((pixelsPerInterval * (maxEnd - maxBegin)))).ToString();
            AddPoint((maxEnd + maxBegin) / 2, chart1.ChartAreas["MainArea"].AxisY.Maximum, maxSeries);
        }

        private void AddSeriesPoints(string name)
        {
            AddSeriesPoints(name, Summary);
        }

        protected void AddSeriesPoints(string name, FluencyAnalysisSummary summary)
        {
            bool seriesShown = SeriesShown[name];
            string trendName = name + " Trend";
            bool trendSeriesShown = SeriesShown[trendName];
            int trendLineDegree = SeriesTrendLineDegrees[trendName];

            Series cache = GetCached(name);
            if (seriesShown && cache != null)
            {
                seriesShown = false;
                chart1.Series[name] = cache;
            }
            Series trendCache = GetCached(trendName);
            if (trendSeriesShown && trendCache != null)
            {
                trendSeriesShown = false;
                chart1.Series[trendName].Points.AddAll(trendCache.Points);
            }
            // Check whether we need to compute ...
            if (!trendSeriesShown && !seriesShown) return;

            Series series = chart1.Series[name];
            Series trendSeries = chart1.Series[trendName];
            if (seriesShown) AddToCache(name, series);
            if (trendSeriesShown) AddToCache(trendName, trendSeries);

            double[] xs = new double[summary.Periods.Count];
            double[] ys = new double[summary.Periods.Count];

            int c = 0;
            double midpoint = 0;
            double cumMidpoint = 0;
            foreach (var p in summary.Periods)
            {
                c++;
                FluencyAnalysis.PeriodStats ps = p.Item2;
                LinearAnalysisSummary.AbstractPeriod period = p.Item1;
                if (c == 1)
                {
                    midpoint = ((Convert.ToDouble(period.EndTime) - Convert.ToDouble(period.StartTime))/2);
                    cumMidpoint = midpoint;
                    IntervalLbl.Text = TimeSpan.FromMilliseconds(cumMidpoint * 2).ToString(@"hh\:mm\:ss");
                }
                double y = Math.Round(GetPercentage(ps) * 100, 2);
                if (y > MaxY) MaxY = y;
                if (seriesShown)
                {
                    AddPoint(c - 0.5, y, series, true);
                }
                xs[c - 1] = c - 0.5;
                ys[c - 1] = y;

                if (c > MaxX)
                {
                    AddLabel(c, TimeSpan.FromMilliseconds(cumMidpoint).ToString(@"hh\:mm\:ss"));
                    cumMidpoint += midpoint * 2;   
                }
            }
            if (c > MaxX)
            {
                MaxX = c;
                chart1.ChartAreas["MainArea"].AxisX.Maximum = c;
            }
	        double axisMax = Math.Max(100, (MaxY - (MaxY % 10) + 10));
            chart1.ChartAreas["MainArea"].AxisY.Maximum = axisMax;
            double[] trendParameters = MathExt.PolynomialFit(xs, ys, trendLineDegree);
            for (int i = 0; i < c; i++)
            {
                double x = i + 0.5;
                double y = MathExt.ComputePolynomial(x, trendParameters);
                AddPoint(x, y, trendSeries);
            }
        }

        private void AddToCache(string name, Series series)
        {
            Tuple<MaximumTYPE, string> key = new Tuple<MaximumTYPE, string>(Maximum, name);
            SeriesCache[key] = series;
        }

        private Series GetCached(string name)
        {
            Tuple<MaximumTYPE, string> key = new Tuple<MaximumTYPE, string>(Maximum, name);
            if (SeriesCache.ContainsKey(key))
            {
                return SeriesCache[key];
            }
            return null;
        }

        private void AddPoint(double x, double y, Series series, bool label = false)
        {
            var point = new DataPoint {XValue = x, YValues = new[] {y}};
            if (label) point.ToolTip = "(" + y + "%)";
            series.Points.Add(point);
        }

        private double GetPercentage(FluencyAnalysis.PeriodStats ps)
        {
            switch (Maximum)
            {
                case MaximumTYPE.ABSOLUTE:
                    return ps.AbsolutePercentage;
                case MaximumTYPE.PERSONAL:
                    return ps.PersonalPercentage;
                case MaximumTYPE.TASK:
                    return ps.TaskPercentage;
                default:
                    return 0;
            }
        }

        protected void InitSeries(string name)
        {
            string trendName = name + " Trend";
            SeriesShown[name] = true;
            SeriesShown[trendName] = true;
            SeriesTrendLineDegrees[trendName] = TrendLineDegree;
            AddSeries(name, trendName);
            LinkLabel toggleLink = AddToggleLink(1);
            toggleLink.Click += delegate
            {
                Toggle(name, toggleLink);
            };
            LinkLabel trendToggleLink = AddToggleLink(2);
            trendToggleLink.Click += delegate
            {
                Toggle(trendName, trendToggleLink);
            };
        }

        protected void InitTaskMaximumZone(string name)
        {
            SeriesShown[name] = true;
            AddTaskMaximumZone(name);
            LinkLabel optToggleLink = AddToggleLink(3);
            optToggleLink.Click += delegate
            {
                Toggle(name, optToggleLink);
            };
        }

        protected void AddSeries(string name)
        {
            string trendName = name + " Trend";
            AddSeries(name, trendName);
        }

        private void AddSeries(string name, string trendName)
        {
            Color color = Colors[Count];
            Series series = chart1.Series.Add(name);
            series.ChartType = SeriesChartType.Line;
            series.MarkerStyle = MarkerStyle.Diamond;
            series.MarkerSize = 10;
            series.Color = color;

            Series trendSeries = chart1.Series.Add(trendName);
            trendSeries.ChartType = SeriesChartType.Spline;
            trendSeries.Color = Color.FromArgb(215, color);
            trendSeries.BorderDashStyle = ChartDashStyle.Dash;

            Count++;
        }

        private void Toggle(string name, LinkLabel lab)
        {
            if (SeriesShown[name])
            {
                SeriesShown[name] = false;
                lab.Text = "(Show)";
            }
            else
            {
                SeriesShown[name] = true;
                lab.Text = "(Hide)";
            }
            RedrawSeries();
        }

        private LinkLabel AddToggleLink(int offset)
        {
            LinkLabel toggleLink = new LinkLabel();
            Controls.Add(toggleLink);
            int x = dummyToggleLink.Location.X;
            int y = dummyToggleLink.Location.Y + ((dummyToggleLink.Height + 1) 
                * ((LegendsPerSeries * (Count - 1)) + offset)) + (Count - 5);
            toggleLink.Text = "(Hide)";
            toggleLink.Location = new Point(x, y);
            toggleLink.BringToFront();
            toggleLink.BackColor = Color.White;
            toggleLink.Font = new Font("Microsoft sans serif", 7F, FontStyle.Regular);
            toggleLink.Size = new Size(40, 14);
            return toggleLink;
        }

        private void OptTypeSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (maxTypeSelect.SelectedItem.ToString())
            {
                case "Absolute":
                    Maximum = MaximumTYPE.ABSOLUTE;
                    break;
                case "Personal":
                    Maximum = MaximumTYPE.PERSONAL;
                    break;
                case "Task":
                    Maximum = MaximumTYPE.TASK;
                    break;
                default:
                    Maximum = MaximumTYPE.ABSOLUTE;
                    break;
            }
            RedrawSeries();
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