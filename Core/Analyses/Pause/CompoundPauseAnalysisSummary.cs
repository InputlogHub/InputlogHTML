using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting;
using InputLog.Core.Reporting.Graphs;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Pause
{
    /// <summary>
    ///     Summary of the Pause analysis. Compound pause analysis can contain a few different
    ///     summaries of pause analyses run with different parameters. The standard case will be that it only
    ///     contains one summary, the default one. However, for reporting purposes, it may contain a few
    ///     different analyses results.
    /// </summary>
    public class CompoundPauseAnalysisSummary : AbstractAnalysisSummary
    {
        /// <summary>
        ///     Dictionary that maps an integer to a singular summary.
        /// </summary>
        public Dictionary<string, SingularPauseAnalysisSummary> Summaries;

        // Keys used to index the singular pause analysis summaries.
        public const string TRESHOLD_30 = "30";
        public const string TRESHOLD_DEFAULT = "DEFAULT";
        public const string TRESHOLD_1000 = "1000";
        public const string TRESHOLD_2000 = "2000";
        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

        public CompoundPauseAnalysisSummary()
        {
            Summaries = new Dictionary<string, SingularPauseAnalysisSummary>(4);
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

        #region Reporting

        /*
         * The pause analysis may run the pause analysis four times using four different threshold
         * variables when run for the reporting functionality.
         * This results in a lot of copy/pasting of the target methods.
         */

        private readonly CultureInfo culture = new CultureInfo("en-US", false);

        /*
         * PauseTime
         */

        private string report_PauseTime(SingularPauseAnalysisSummary summary)
        {
            return DateTimeUtils.MsecToClockString(summary.GeneralInformation.TotalPauseTime, false);
        }


        public ReportValue report_PauseTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseTime_30()
        {
            const string RESOURCE_ID = "Pause_PauseTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseTime_1000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseTime_2000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime(Summaries[TRESHOLD_2000])
            );
        }

        /*
         * Total Process Time
         */

        private string report_ProcessTime(SingularPauseAnalysisSummary summary)
        {
            return DateTimeUtils.MsecToClockString(summary.GeneralInformation.TotalProcessTime, false);
        }

        public ReportValue report_ProcessTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_ProcessTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_ProcessTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_ProcessTime_30()
        {
            const string RESOURCE_ID = "Pause_ProcessTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_ProcessTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_ProcessTime_1000()
        {
            const string RESOURCE_ID = "Pause_ProcessTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_ProcessTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_ProcessTime_2000()
        {
            const string RESOURCE_ID = "Pause_ProcessTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_ProcessTime(Summaries[TRESHOLD_2000])
            );
        }

        /* 
         * Total Pauses
         */

        private string report_TotalPauses(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.TotalNumberOfPauses.ToString();
        }

        public ReportValue report_TotalPauses_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_TotalPauses_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_TotalPauses(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_TotalPauses_30()
        {
            const string RESOURCE_ID = "Pause_TotalPauses_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_TotalPauses(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_TotalPauses_1000()
        {
            const string RESOURCE_ID = "Pause_TotalPauses_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_TotalPauses(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_TotalPauses_2000()
        {
            const string RESOURCE_ID = "Pause_TotalPauses_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_TotalPauses(Summaries[TRESHOLD_2000])
            );
        }

        /* 
         * Active Time
         */

        private string report_ActiveTime(SingularPauseAnalysisSummary summary)
        {
            return DateTimeUtils.MsecToClockString(Convert.ToUInt64(summary.GeneralInformation.TotalWritingTime), false);
        }

        public ReportValue report_ActiveTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_ActiveTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_ActiveTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_ActiveTime_30()
        {
            const string RESOURCE_ID = "Pause_ActiveTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_ActiveTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_ActiveTime_1000()
        {
            const string RESOURCE_ID = "Pause_ActiveTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_ActiveTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_ActiveTime_2000()
        {
            const string RESOURCE_ID = "Pause_ActiveTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_ActiveTime(Summaries[TRESHOLD_2000])
            );
        }

        /*
         * Pause to Active TIme proportionally
         */

        private string report_PauseToActiveTime_Proportionally(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.PauseTimeProportion.ToString("P", culture.NumberFormat);
        }

        public ReportValue report_PauseToActiveTime_Proportionally_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseToActiveProp_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseToActiveTime_Proportionally(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseToActiveTime_Proportionally_30()
        {
            const string RESOURCE_ID = "Pause_PauseToActiveProp_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseToActiveTime_Proportionally(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseToActiveTime_Proportionally_1000()
        {
            const string RESOURCE_ID = "Pause_PauseToActiveProp_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseToActiveTime_Proportionally(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseToActiveTime_Proportionally_2000()
        {
            const string RESOURCE_ID = "Pause_PauseToActiveProp_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseToActiveTime_Proportionally(Summaries[TRESHOLD_2000])
            );
        }

        /*
         * Pause geo mean
         */

        private string report_PauseGeoMean(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.GeoMeanPauseTime.ToString("F0", culture.NumberFormat);
        }


        public ReportValue report_PauseGeoMean_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_GeoMean_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseGeoMean(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseGeoMean_30()
        {
            const string RESOURCE_ID = "Pause_GeoMean_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseGeoMean(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseGeoMean_1000()
        {
            const string RESOURCE_ID = "Pause_GeoMean_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseGeoMean(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseGeoMean_2000()
        {
            const string RESOURCE_ID = "Pause_GeoMean_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseGeoMean(Summaries[TRESHOLD_2000])
            );
        }

        /* 
         * Pause CoV
         */

        private string report_PauseCOV(SingularPauseAnalysisSummary summary)
        {
            double cov = summary.GeneralInformation.CoefVar*100;
            return cov.ToString("F", culture.NumberFormat) + "%";

        }

        public ReportValue report_PauseCOV_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_COV_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseCOV(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseCOV_30()
        {
            const string RESOURCE_ID = "Pause_COV_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseCOV(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseCOV_1000()
        {
            const string RESOURCE_ID = "Pause_COV_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseCOV(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseCOV_2000()
        {
            const string RESOURCE_ID = "Pause_COV_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseCOV(Summaries[TRESHOLD_2000])
            );
        }

        /* 
         * Pause First Key
         */

        private string report_PauseFirstKey(SingularPauseAnalysisSummary summary)
        {
            return DateTimeUtils.MsecToClockString(summary.GeneralInformation.FirstStartKeyTime, false, true);
        }

        public ReportValue report_PauseFirstKey_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_FirstKey_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseFirstKey(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseFirstKey_30()
        {
            const string RESOURCE_ID = "Pause_FirstKey_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseFirstKey(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseFirstKey_1000()
        {
            const string RESOURCE_ID = "Pause_FirstKey_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseFirstKey(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseFirstKey_2000()
        {
            const string RESOURCE_ID = "Pause_FirstKey_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseFirstKey(Summaries[TRESHOLD_2000])
            );
        }

        /*
         * PauseTime between words avg
         */

        private string report_PauseTime_BetweenWords_Avg(SingularPauseAnalysisSummary summary)
        {
            var between_words = summary.CombinedTypes[PauseLocation.BEFORE_WORDS];
            return between_words.CombinedMean.ToString("0", culture.NumberFormat);
        }

        public ReportValue report_PauseTime_BetweenWords_Avg_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Avg_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Avg(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Avg_30()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Avg_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Avg(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Avg_1000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Avg_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Avg(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Avg_2000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Avg_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Avg(Summaries[TRESHOLD_2000])
            );
        }

        /*
         * pause time between words stdev
         */

        private string report_PauseTime_BetweenWords_Stdev(SingularPauseAnalysisSummary summary)
        {
            var between_words = summary.CombinedTypes[PauseLocation.BEFORE_WORDS];
            return between_words.CombinedStDev.ToString("F", culture.NumberFormat);
        }

        public ReportValue report_PauseTime_BetweenWords_Stdev_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Stdev_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Stdev(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Stdev_30()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Stdev_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Stdev(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Stdev_1000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Stdev_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Stdev(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseTime_BetweenWords_Stdev_2000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_Stdev_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_Stdev(Summaries[TRESHOLD_2000])
            );
        }

        // Combined Geomean

        private string report_PauseTime_BetweenWords_CombinedGeoMean(SingularPauseAnalysisSummary summary)
        {
            var between_words = summary.CombinedTypes[PauseLocation.BEFORE_WORDS];
            return between_words.CombinedGeoMean.ToString("F", culture.NumberFormat);
        }

        public ReportValue report_PauseGeoMean_BetweenWords_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_GeoMean_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedGeoMean(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseGeoMean_BetweenWords_30()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_GeoMean_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedGeoMean(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseGeoMean_BetweenWords_1000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_GeoMean_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedGeoMean(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseGeoMean_BetweenWords_2000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_GeoMean_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedGeoMean(Summaries[TRESHOLD_2000])
            );
        }

        // Combined CoV

        private string report_PauseTime_BetweenWords_CombinedCoefVar(SingularPauseAnalysisSummary summary)
        {
            var between_words = summary.CombinedTypes[PauseLocation.BEFORE_WORDS];
            return between_words.CombinedCoefVar.ToString("F", culture.NumberFormat);

        }

        public ReportValue report_PauseCOV_BetweenWords_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_COV_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedCoefVar(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseCOV_BetweenWords_30()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_COV_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedCoefVar(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseCOV_BetweenWords_1000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_COV_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedCoefVar(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseCOV_BetweenWords_2000()
        {
            const string RESOURCE_ID = "Pause_PauseTime_BetweenWords_COV_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_PauseTime_BetweenWords_CombinedCoefVar(Summaries[TRESHOLD_2000])
            );
        }


        /*
         * Pause Levels box plot
         */

        private MemoryStream report_PauseLevels_BoxPlot(SingularPauseAnalysisSummary summary)
        {
            BoxPlot plot = new BoxPlot();
            var within_words = summary.BasePauseTypes[PauseLocation.WITHIN_WORDS];
            plot.AddSeries(
                "Within Words",
                within_words.BoxPlotMin,
                within_words.BoxPlotMax,
                within_words.FirstQuartile,
                within_words.ThirdQuartile,
                within_words.MeanPauseTime,
                within_words.MedianPauseTime,
                null
            );

            var between_words = summary.CombinedTypes[PauseLocation.BEFORE_WORDS];
            plot.AddSeries(
                "Between Words",
                between_words.CombinedBoxPlotMin,
                between_words.CombinedBoxPlotMax,
                between_words.CombinedFirstQuartile,
                between_words.CombinedThirdQuartile,
                between_words.CombinedMean,
                between_words.CombinedMedian,
                null
            );

            var between_sentences = summary.CombinedTypes[PauseLocation.BEFORE_SENTENCES];
            plot.AddSeries(
                "Between sentences",
                between_sentences.CombinedBoxPlotMin,
                between_sentences.CombinedBoxPlotMax,
                between_sentences.CombinedFirstQuartile,
                between_sentences.CombinedThirdQuartile,
                between_sentences.CombinedMean,
                between_sentences.CombinedMedian,
                null
            );

            plot.SetYAxisName("Pause Time");
            plot.SetSize(800, 500);
            MemoryStream image = new MemoryStream();
            plot.SaveToStream(image);

            return image;
        }

        public ReportValue report_PauseLevels_BoxPlot_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_PauseLevels_BoxPlot_DEFAULT";
            return new GraphValue(
                RESOURCE_ID,
                report_PauseLevels_BoxPlot(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_PauseLevels_BoxPlot_30()
        {
            const string RESOURCE_ID = "Pause_PauseLevels_BoxPlot_30";
            return new GraphValue(
                RESOURCE_ID,
                report_PauseLevels_BoxPlot(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_PauseLevels_BoxPlot_1000()
        {
            const string RESOURCE_ID = "Pause_PauseLevels_BoxPlot_1000";
            return new GraphValue(
                RESOURCE_ID,
                report_PauseLevels_BoxPlot(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_PauseLevels_BoxPlot_2000()
        {
            const string RESOURCE_ID = "Pause_PauseLevels_BoxPlot_2000";
            return new GraphValue(
                RESOURCE_ID,
                report_PauseLevels_BoxPlot(Summaries[TRESHOLD_2000])
            );
        }


        /// <summary>
        /// Interval statistics
        /// </summary>
        private List<double> _timeCount;
        private List<ulong> _pauseCount;
        private List<double> _pauseAvg;
        private List<double> _timePerct;

        /// <summary>
        /// 
        /// </summary>
        private void IntervalStats()
        {
            _timeCount = new List<double>();
            _pauseCount = new List<ulong>();
            _pauseAvg = new List<double>();
            _timePerct = new List<double>();
            try
            {
                if (Summaries[TRESHOLD_2000].IntervalInfo.Count > 3)
                {
                    MessageBox.Show(
                        "Variable: 'IntervalStats'\nThe number of intervals used (" +
                        Summaries[TRESHOLD_2000].IntervalInfo.Count +
                        ") is larger than expected by the report (3).\nThe program will continue but the results may be wrong.",
                        "Number of Intervals in the Pause Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                double totalCount = 0.0;
                foreach (var field in Summaries[TRESHOLD_2000].IntervalInfo)
                {
                    var statistics = field.Value;
                    _timeCount.Add(statistics.MedianPauseTime);
                    totalCount += statistics.MedianPauseTime;
                    _pauseCount.Add(statistics.NumberOfPauses);
                    _pauseAvg.Add(statistics.MeanPauseTime);
                }
                _timeCount.Add(totalCount);
                _timePerct.AddRange(_timeCount.Select(time => time/totalCount));
                _timePerct[3] = 1;
            }
            catch (Exception e)
            {
                throw new AnalysisException("Exception in 'Intervalstats' " + e.StackTrace);
            }
        }

        /// <summary>
        /// Total time in an interval is the same for every interval because a fixed number (3) of intervals is used.
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        private ulong IntervalTime(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.IntervalLength;
        }


        public ReportValue report_PausePerct_Int1()
        {
            if(_timeCount == null)
            {
                IntervalStats();
            } 
            const string RESOURCE_ID = "Pause_PausePerct_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                _timePerct[0].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_RelativePause_Int1()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativePause_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                (_timeCount[0]/IntervalTime(Summaries[TRESHOLD_2000])).ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_RelativeWriting_Int1()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativeWriting_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                ((IntervalTime(Summaries[TRESHOLD_2000]) -_timeCount[0])/IntervalTime(Summaries[TRESHOLD_2000])).ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_PausePerMin_Int1()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerMin_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                (_pauseCount[0]/ (IntervalTime(Summaries[TRESHOLD_2000])/60000)).ToString("00.00", culture.NumberFormat)
            );
        }

        public ReportValue report_AvgPause_Int1()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_AvgPause_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                DateTimeUtils.MsecToClockString((ulong) _pauseAvg[0], false)
            );
        }


        public ReportValue report_PausePerct_Int2()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerct_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                _timePerct[1].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_RelativePause_Int2()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativePause_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                (_timeCount[1]/IntervalTime(Summaries[TRESHOLD_2000])).ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_RelativeWriting_Int2()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativeWriting_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                ((IntervalTime(Summaries[TRESHOLD_2000]) - _timeCount[1]) / IntervalTime(Summaries[TRESHOLD_2000])).ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_PausePerMin_Int2()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerMin_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                (_pauseCount[1]/(IntervalTime(Summaries[TRESHOLD_2000])/60000)).ToString("00.00", culture.NumberFormat)
            );
        }

        public ReportValue report_AvgPause_Int2()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_AvgPause_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                DateTimeUtils.MsecToClockString((ulong)_pauseAvg[1], false)
            );
        }

        public ReportValue report_PausePerct_Int3()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerct_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                _timePerct[2].ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_RelativePause_Int3()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativePause_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                (_timeCount[2]/IntervalTime(Summaries[TRESHOLD_2000])).ToString("P", culture.NumberFormat)
            );
        }


        public ReportValue report_RelativeWriting_Int3()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_RelativeWriting_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                ((IntervalTime(Summaries[TRESHOLD_2000]) - _timeCount[2])/IntervalTime(Summaries[TRESHOLD_2000]))
                    .ToString("P", culture.NumberFormat)
            );
        }

        public ReportValue report_PausePerMin_Int3()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerMin_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                (_pauseCount[2]/(IntervalTime(Summaries[TRESHOLD_2000])/60000)).ToString("00.00", culture.NumberFormat)
            );
        }

        public ReportValue report_AvgPause_Int3()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_AvgPause_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                DateTimeUtils.MsecToClockString((ulong)_pauseAvg[2], false)
            );
        }

        public ReportValue report_PausePerct_Tot()
        {
            if (_timeCount == null)
            {
                IntervalStats();
            }
            const string RESOURCE_ID = "Pause_PausePerct_Tot";
            return new LabeledValue(
                RESOURCE_ID,
                _timePerct[3].ToString("P", culture.NumberFormat)
            );
        }

        #region PBurst
        // P-Burst statistics. A p-burst is an action time from the previous key-up to current key-down, not a pause.
        // Important: the THRESHOLD values mentioned here are for the pause statistics, not for p-bursts.
        // However, the p-bursts are calculated with their appropriate values, as defined in
        // the PauseAnalysis. The correct value for both the pause and the p-burst threshold 
        // is reported in the PauseAnalysis xml as a meta field.
        public string report_Pburst_Threshold(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.PBurstTreshold.ToString("F", Nfi);
        }

        public string report_Pburst_Nr(SingularPauseAnalysisSummary summary)
        {
           return summary.GeneralInformation.NumberOfSegments.ToString("F", Nfi);
        }

        public string report_Pburst_AvgTime(SingularPauseAnalysisSummary summary)
        {
            return (summary.GeneralInformation.AvgProcessTime / 1000.0).ToString("F", Nfi);
        }

        public string report_Pburst_MedianTime(SingularPauseAnalysisSummary summary)
        {          
            return (summary.GeneralInformation.MedianProcessTime / 1000.0).ToString("F", Nfi);
        }

        public string report_Pburst_PerMinute(SingularPauseAnalysisSummary summary)
        {         
            return summary.GeneralInformation.PBurstsPerMinute.ToString("F", Nfi);
        }

        public string report_Pburst_StdevTime(SingularPauseAnalysisSummary summary)
        {          
            return (summary.GeneralInformation.StandardDeviation / 1000.0).ToString("F", Nfi);
        }

        public string report_Pburst_AvgProd(SingularPauseAnalysisSummary summary)
        {           
            return (summary.GeneralInformation.AvgProcessChars / 1000.0).ToString("F", Nfi);
        }

        public string report_Pburst_MedianProd(SingularPauseAnalysisSummary summary)
        {          
            return summary.GeneralInformation.MedianProcessChars.ToString("F", Nfi);
        }

        public string report_Pburst_StdevProd(SingularPauseAnalysisSummary summary)
        {
            return summary.GeneralInformation.StandardDeviationChars.ToString("F", Nfi);
        }

        public ReportValue report_Pburst_Threshold_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_Threshold_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Threshold(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_Nr_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_Nr_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Nr(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_AvgTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_AvgTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_MedianTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_MedianTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_PerMinute_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_PerMinute_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_PerMinute(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_StdevTime_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevTime_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevTime(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_AvgProd_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgProd_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_AvgProd(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_MedianProd_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianProd_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_MedianProd(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_StdevProd_DEFAULT()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevProd_DEFAULT";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevProd(Summaries[TRESHOLD_DEFAULT])
            );
        }

        public ReportValue report_Pburst_Threshold_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_Threshold_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Threshold(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_Nr_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_Nr_30";
            return new LabeledValue(
                RESOURCE_ID,
               report_Pburst_Nr(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_AvgTime_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_AvgTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_MedianTime_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_MedianTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_PerMinute_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_PerMinute_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_PerMinute(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_StdevTime_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevTime_30";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevTime(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_AvgProd_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgProd_30";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_AvgProd(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_MedianProd_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianProd_30";
            return new LabeledValue(
                RESOURCE_ID,
               report_Pburst_MedianProd(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_StdevProd_30()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevProd_30";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevProd(Summaries[TRESHOLD_30])
            );
        }

        public ReportValue report_Pburst_Threshold_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_Threshold_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Threshold(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_Nr_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_Nr_1000";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_Nr(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_AvgTime_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_AvgTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_MedianTime_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_MedianTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_PerMinute_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_PerMinute_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_PerMinute(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_StdevTime_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevTime_1000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_StdevTime(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_AvgProd_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgProd_1000";
            return new LabeledValue(
                RESOURCE_ID,
               report_Pburst_AvgProd(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_MedianProd_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianProd_1000";
            return new LabeledValue(
                RESOURCE_ID,
               report_Pburst_MedianProd(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_StdevProd_1000()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevProd_1000";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevProd(Summaries[TRESHOLD_1000])
            );
        }

        public ReportValue report_Pburst_Threshold_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_Threshold_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Threshold(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_Nr_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_Nr_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_Nr(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_AvgTime_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_AvgTime(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_MedianTime_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_MedianTime(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_PerMinute_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_PerMinute_2000";
            return new LabeledValue(
                RESOURCE_ID,
                  report_Pburst_PerMinute(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_StdevTime_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevTime_2000";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevTime(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_AvgProd_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_AvgProd_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_AvgProd(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_MedianProd_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_MedianProd_2000";
            return new LabeledValue(
                RESOURCE_ID,
                report_Pburst_MedianProd(Summaries[TRESHOLD_2000])
            );
        }

        public ReportValue report_Pburst_StdevProd_2000()
        {
            const string RESOURCE_ID = "Pause_Pburst_StdevProd_2000";
            return new LabeledValue(
                RESOURCE_ID,
                 report_Pburst_StdevProd(Summaries[TRESHOLD_2000])
            );
        }
        #endregion
        #endregion
    }
}


