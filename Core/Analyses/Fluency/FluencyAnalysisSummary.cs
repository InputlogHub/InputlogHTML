using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.Reporting;
using InputLog.Core.Reporting.Report;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Fluency
{
    /// <summary>
    ///     Summary of the Fluency analysis.
    ///     Contains a list of Periods. Each Period contains Events that represent a keypress or pause.
    /// </summary>
    public class FluencyAnalysisSummary : AbstractAnalysisSummary
    {
        #region fields

        public LinearAnalysis.TYPE LinearType;
        public readonly List<Tuple<LinearAnalysisSummary.AbstractPeriod, FluencyAnalysis.PeriodStats>> Periods;
        public double AbsMaximum;
        public double PersonalMaximum;
        public double TaskLMaximum;
        public double AverageSPM;
        public double StdDev;
        public ulong TotalLength;
        public int TotalStrokes;
        private int NrPeriods;
        private double SpmSum;

        public double AbsCoefVar;
        public double AbsMeanInterval;
        public double AbsMedian;
        public double AbsMaxStdev;
        public double TaskCoefVar;
        public double TaskMeanInterval;
        public double TaskMedian;
        public double TaskMaxStdev;
        public double PersCoefVar;
        public double PersMeanInterval;
        public double PersMedian;
        public double PersMaxStdev;
        public double StrokesCoefVar;
        public double StrokesMeanInterval;
        public double StrokesMedian;
        public double StrokesMaxStdev;
        public double StrokesMinCoefVar;
        public double StrokesMinInterval;
        public double StrokesMinMedian;
        public double StrokesMinMaxStdev;

        public FluencyAnalysis.TaskMaximumPeriod TaskMaximumPeriod;
        public double SmallChunkStdDev;

        public MemoryStream GraphImage;
        private CultureInfo culture;

        #endregion

        /// <summary>
        ///     Construct the summary.
        /// </summary>
        public FluencyAnalysisSummary()
        {
            Periods = new List<Tuple<LinearAnalysisSummary.AbstractPeriod, FluencyAnalysis.PeriodStats>>();
            this.GraphImage = new MemoryStream();
            this.culture = new CultureInfo("en-US");
        }

        public void AddPeriod(LinearAnalysisSummary.AbstractPeriod p, FluencyAnalysis.PeriodStats ps)
        {
            Periods.Add(new Tuple<LinearAnalysisSummary.AbstractPeriod, FluencyAnalysis.PeriodStats>(p, ps));
            SpmSum += ps.StrokesPerMin;
            NrPeriods++;
            TotalLength += ps.Length;
            TotalStrokes += ps.NrStrokes;
        }

        public void AddMaxima(double max, double pMax, double tMax, FluencyAnalysis.TaskMaximumPeriod period)
        {
            AbsMaximum = max;
            TaskLMaximum = tMax;
            PersonalMaximum = pMax;
            TaskMaximumPeriod = period;
        }

        public void ComputeStats()
        {
            AverageSPM = SpmSum/NrPeriods;
            double variance = (from p in Periods
                select p.Item2
                into ps
                select ps.StrokesPerMin - AverageSPM
                into diff
                select (diff*diff)).Sum();
            StdDev = Math.Sqrt(variance/NrPeriods);

            IEnumerable<double> maxPerct = from p in Periods select p.Item2.AbsolutePercentage;
            IEnumerable<double> taskPerct = from p in Periods select p.Item2.TaskPercentage;
            IEnumerable<double> persPerct = from p in Periods select p.Item2.PersonalPercentage;
            IEnumerable<int> strokes = from p in Periods select p.Item2.NrStrokes;
            IEnumerable<int> strokesMin = from p in Periods select p.Item2.StrokesPerMin;
            List<double> maxPerctList = maxPerct.ToList();
            List<double> taskPerctList = taskPerct.ToList();
            List<double> persPerctList = persPerct.ToList();
            var doubleArray = Array.ConvertAll(strokes.ToArray(), num => (double) num);
            List<double> strokesList = doubleArray.ToList();
            doubleArray = Array.ConvertAll(strokesMin.ToArray(), num => (double) num);
            List<double> strokesMinList = doubleArray.ToList();

            // Returns a tuple with as first element the low interval boundary (double),
            // as second the (geometric) mean, thirdly the high interval boundary (double), and
            // as fourth element the coefficient of variation.
            Tuple<double, double, double, double> intervals = MathExt.CalculateInterval(maxPerctList);
            AbsCoefVar = intervals.Item4;
            AbsMeanInterval = MathExt.MeanFromList(maxPerctList);
            AbsMedian = MathExt.GetMedian(maxPerctList);
            AbsMaxStdev = MathExt.StDevFromList(maxPerctList);

            intervals = MathExt.CalculateInterval(taskPerctList);
            TaskCoefVar = intervals.Item4;
            TaskMeanInterval = MathExt.MeanFromList(taskPerctList);
            TaskMedian = MathExt.GetMedian(taskPerctList);
            TaskMaxStdev = MathExt.StDevFromList(taskPerctList);

            intervals = MathExt.CalculateInterval(persPerctList);
            PersCoefVar = intervals.Item4;
            PersMeanInterval = MathExt.MeanFromList(persPerctList);
            PersMedian = MathExt.GetMedian(persPerctList);
            PersMaxStdev = MathExt.StDevFromList(persPerctList);

            intervals = MathExt.CalculateInterval(strokesList);
            StrokesCoefVar = intervals.Item4;
            StrokesMeanInterval = MathExt.MeanFromList(strokesList);
            StrokesMedian = MathExt.GetMedian(strokesList);
            StrokesMaxStdev = MathExt.StDevFromList(strokesList);

            intervals = MathExt.CalculateInterval(strokesMinList);
            StrokesMinCoefVar = intervals.Item4;
            StrokesMinInterval = MathExt.MeanFromList(strokesMinList);
            StrokesMinMedian = MathExt.GetMedian(strokesMinList);
            StrokesMinMaxStdev = MathExt.StDevFromList(strokesMinList);
        }

        public void AddStdDev(double smallChunkStdDev)
        {
            SmallChunkStdDev = smallChunkStdDev;
        }


        /*
         * IMPORTANT NOTE: 
         * These methods are being referenced in the reporting functionality, by name!
         * Do not change these method names without changing the references
         * in the required resource files as well.
         * 
         * Resource File: Core.Reporting.Resources.ReportMappingResources.resx
         * 
         * Note: The report_ prefix must be kept! The methods are also reflectively discovered
         * in the GetBoundTargets() method of the AbstractAnalysisSummary class!
         */

        #region Reporting methods

        public ReportValue report_Fluency_Graph()
        {
            const string RESOURCE_ID = "Fluency_FluencyGraph";
            GraphValue value = new GraphValue(
                RESOURCE_ID,
                this.GraphImage
            );
            value.IsFullPageImage = true;
            return value;
        }

        public ReportValue report_AvgSPM()
        {
            const string RESOURCE_ID = "Fluency_Avg_StrokesPerMinute";
            return new LabeledValue(
                RESOURCE_ID,
                this.AverageSPM.ToString("0", culture.NumberFormat)
            );
        }

        public ReportValue report_StdevSPM()
        {
            const string RESOURCE_ID = "Fluency_Stdev_StrokesPerMinute";
            return new LabeledValue(
                RESOURCE_ID,
                this.StdDev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_AbsCoefVar()
        {
            const string RESOURCE_ID = "Fluency_COV_PerctMaximum";
            return new LabeledValue(
                RESOURCE_ID,
                this.AbsCoefVar.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_AbsMeanInterval()
        {
            const string RESOURCE_ID = "Fluency_Mean_PerctMaximum";
            return new LabeledValue(
                RESOURCE_ID,
                this.AbsMeanInterval.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_AbsMedian()
        {
            const string RESOURCE_ID = "Fluency_Median_PerctMaximum";
            return new LabeledValue(
                RESOURCE_ID,
                this.AbsMedian.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_AbsMaxStdev()
        {
            const string RESOURCE_ID = "Fluency_Stdev_PerctMaximum";
            return new LabeledValue(
                RESOURCE_ID,
                this.AbsMaxStdev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_TaskCoefVar()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_TaskCoefVar";
            return new LabeledValue(
                RESOURCE_ID,
                this.TaskCoefVar.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_TaskMeanInterval()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_TaskMeanInterval";
            return new LabeledValue(
                RESOURCE_ID,
                this.TaskMeanInterval.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_TaskMedian()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_TaskMedian";
            return new LabeledValue(
                RESOURCE_ID,
                this.TaskMedian.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_TaskMaxStdev()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_TaskMaxStdev";
            return new LabeledValue(
                RESOURCE_ID,
                this.TaskMaxStdev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesCoefVar()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesCoefVar";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesCoefVar.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMeanInterval()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMeanInterval";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMeanInterval.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMedian()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMedian";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMedian.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMaxStdev()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMaxStdev";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMaxStdev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMinCoefVar()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMinCoefVar";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMinCoefVar.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMinInterval()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMinInterval";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMinInterval.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMinMedian()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMinMedian";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMinMedian.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TaskMaximum_StrokesMinMaxStdev()
        {
            const string RESOURCE_ID = "Fluency_TaskMaximum_StrokesMinMaxStdev";
            return new LabeledValue(
                RESOURCE_ID,
                this.StrokesMinMaxStdev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_PersMaximum_PersCoefVar()
        {
            const string RESOURCE_ID = "Fluency_PersMaximum_PersCoefVar";
            return new LabeledValue(
                RESOURCE_ID,
                this.PersCoefVar.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_PersMaximum_PersMeanInterval()
        {
            const string RESOURCE_ID = "Fluency_PersMaximum_PersMeanInterval";
            return new LabeledValue(
                RESOURCE_ID,
                this.PersMeanInterval.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_PersMaximum_PersMedian()
        {
            const string RESOURCE_ID = "Fluency_PersMaximum_PersMedian";
            return new LabeledValue(
                RESOURCE_ID,
                this.PersMedian.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_PersMaximum_PersMaxStdev()
        {
            const string RESOURCE_ID = "Fluency_PersMaximum_PersMaxStdev";
            return new LabeledValue(
                RESOURCE_ID,
                this.PersMaxStdev.ToString("F", culture.NumberFormat)
            );
        }


        public ReportValue report_TaskMaximum_SPM()
        {
            int max = Periods.Select(tuple => tuple.Item2.StrokesPerMin).Max();

            const string RESOURCE_ID = "Fluency_TaskMaximum_SPM";
            return new LabeledValue(
                RESOURCE_ID,
                max.ToString()
            );
        }

        public ReportValue report_TaskMaximum()
        {
            double max = this.TaskLMaximum;

            const string RESOURCE_ID = "Fluency_TaskMaximum";
            return new LabeledValue(
                RESOURCE_ID,
                max.ToString("0", culture.NumberFormat)
            );
        }

        /// <summary>
        /// Keystrokes per minute per interval.
        /// </summary>
        private List<int> StrokesPerMinute
        {
            get
            {
                
                List<int> timeCount = new List<int>();
                try
                {
                    if (Periods.Count > 3)
                    {
                        MessageBox.Show("Variable: 'StrokesPerMinute'\nThe number of intervals used ("
                                        + Periods.Count + ") is larger than expected by the report (3).\n" +
                                        "The program will continue but the result may be wrong.",
                            "Number of Intervals in the Fluency Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    int totalCount = 0;
                    foreach (var interval in Periods)
                    {
                        timeCount.Add(interval.Item2.StrokesPerMin);
                        totalCount += interval.Item2.StrokesPerMin;
                    }
                    timeCount.Add(totalCount);
                }
                catch (Exception e)
                {
                    throw new AnalysisException("Exception in 'StrokesPerMinute' " + e.StackTrace);
                }
                return timeCount;
            }
        }

        public ReportValue report_KeystrokesMin_Int1()
        {
            if (_strokesPerMinute == null)
            {
                _strokesPerMinute = StrokesPerMinute;
            }
            const string RESOURCE_ID = "KeystrokesMin_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerMinute[0].ToString("0", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesMin_Int2()
        {
            if (_strokesPerMinute == null)
            {
                _strokesPerMinute = StrokesPerMinute;
            }
            const string RESOURCE_ID = "KeystrokesMin_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerMinute[1].ToString("0", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesMin_Int3()
        {
            if (_strokesPerMinute == null)
            {
                _strokesPerMinute = StrokesPerMinute;
            }
            const string RESOURCE_ID = "KeystrokesMin_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerMinute[2].ToString("0", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesMin_Tot()
        {
            if (_strokesPerMinute == null)
            {
                _strokesPerMinute = StrokesPerMinute;
            }
            const string RESOURCE_ID = "KeystrokesMin_Tot";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerMinute[3].ToString("0", culture.NumberFormat)
            );
        }

        private List<int> _strokesPerMinute;

        /// <summary>
        /// Percentage strokes per interval
        /// </summary>
        private List<double> StrokesPerInterval
        {
            get
            {             
                List<int> strokesPerInterval = new List<int>();
                List<double> strokesPerct = new List<double>();
       
                try
                {
                    if (Periods.Count > 3)
                    {
                        MessageBox.Show("Variable: 'StrokesPerInterval'\nThe number of intervals used (" + Periods.Count
                                        +
                                        ") is larger than expected by the report (3).\nThe program will continue but the result may be wrong.",
                            "Number of Intervals in the Fluency Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    int totalCount = 0;
                    foreach (var interval in Periods)
                    {                      
                        strokesPerInterval.Add(interval.Item2.NrStrokes);
                        totalCount += interval.Item2.NrStrokes;
                    }
                    strokesPerInterval.Add(totalCount);
                    strokesPerct.AddRange(strokesPerInterval.Select(intervals => (double) intervals/totalCount));
                }
                catch (Exception e)
                {
                    throw new AnalysisException("Exception in 'StrokesPerMinute' " + e.StackTrace);
                }
                return strokesPerct;
            }
        }

        public ReportValue report_KeystrokesPerct_Int1()
        {
            if (_strokesPerInterval == null)
            {
                _strokesPerInterval = StrokesPerInterval;
            }
            const string RESOURCE_ID = "KeystrokesPerct_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerInterval[0].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesPerct_Int2()
        {
            if (_strokesPerInterval == null)
            {
                _strokesPerInterval = StrokesPerInterval;
            }
            const string RESOURCE_ID = "KeystrokesPerct_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerInterval[1].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesPerct_Int3()
        {
            if (_strokesPerInterval == null)
            {
                _strokesPerInterval = StrokesPerInterval;
            }
            const string RESOURCE_ID = "KeystrokesPerct_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerInterval[2].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_KeystrokesPerct_Tot()
        {
            if (_strokesPerInterval == null)
            {
                _strokesPerInterval = StrokesPerInterval;
            }
            const string RESOURCE_ID = "KeystrokesPerct_Tot";
            return new LabeledValue(
                RESOURCE_ID,
                _strokesPerInterval[3].ToString("P", culture.NumberFormat)
            );
        }

        private List<double> _strokesPerInterval;

        #endregion
    }
}