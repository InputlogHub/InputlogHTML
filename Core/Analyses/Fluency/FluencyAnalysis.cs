using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using log4net;

namespace InputLog.Core.Analyses.Fluency
{
    /// <summary>
    /// A Fluency Analysis.
    /// Measures Fluency based on strokes per minute during intervals, compared to maximum performance.
    /// </summary>
    public class FluencyAnalysis : Analysis
    {
        public enum TaskMaximumMode
        {
            BASIC,
            INTERVAL_DEPENDENT,
        }
     
        #region Utility Subclasses
        /// <summary>
        /// Computes and contains stats for a Fluency Analysis period
        /// </summary>
        public class PeriodStats
        {
            public readonly int NrStrokes;
            public readonly int StrokesPerMin;
            public readonly double AbsolutePercentage;
            public readonly ulong Length;
            public readonly double TaskPercentage;
            public readonly double PersonalPercentage;

            /// <summary>
            /// A few numbers on the writing activity in this period.
            /// </summary>
            /// <param name="periodCount">Strokes in this period</param>
            /// <param name="intervalSize">The length of an interval (ms)</param>
            /// <param name="aMax">The absolute maximum</param>
            /// <param name="tMax">The task maximum</param>
            /// <param name="pMax">The personal maximum</param>
            public PeriodStats(int periodCount, ulong intervalSize, double aMax, double tMax, double pMax)
            {
                NrStrokes = periodCount; 
                StrokesPerMin = StrokesPerMinute(periodCount, intervalSize);
                Length = intervalSize;
                AbsolutePercentage = StrokesPerMin / aMax;
                TaskPercentage = StrokesPerMin / tMax;
                PersonalPercentage = StrokesPerMin / pMax;
            }
        }

        /// <summary>
        /// Describes the period in which the Task Maximum was reached
        /// </summary>
        public class TaskMaximumPeriod
        {
            public readonly ulong WindowLength;
            public readonly int Window;
            public readonly ulong PeriodStart;
            public readonly ulong PeriodEnd;

            public TaskMaximumPeriod(int window, ulong windowLength, ulong periodEnd)
            {
                Window = window;
                WindowLength = windowLength;

                // 20150530 We've seen periodEnds that are smaller than the window * windowLength offset,
                // resulting in a TimeSpan overflow downstream, because of a 'negative' ulong.
                var periodOffset = (ulong)window * windowLength;
                if (periodOffset < periodEnd)
                {
                    PeriodStart = periodEnd - periodOffset;
                }
                else if (windowLength < periodEnd)
                {
                    PeriodStart = periodEnd - windowLength;
                }
                else
                {
                    PeriodStart = periodEnd;
                }
                PeriodEnd = periodEnd;
            }
        }
        #endregion

        #region Fields copied from linear
        /// <summary>
        /// The used pause threshold (how long the pause between 2 events should be before we actually
        ///  mark it as a pause).
        /// </summary>
        private readonly ulong PauseThreshold;

        /// <summary>
        /// List of keys that is considered to be control keys (e.g. SHIFT, CTRL). 
        /// When a keyboard event contains one of these control keys, it can be represented differently in the summary.
        /// </summary>
        private readonly List<KeysEx> ControlKeys;

        /// <summary>
        /// Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of linear analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that interval size in msec.
        /// e.g. when the typed is "fixed number of intervals", the TypeParam will represent the number of intervals.
        /// </summary>
        private readonly ulong TypeParam;

        /// <summary>
        /// Dictionary containing extra, linearAnalysisType-depending parameters.
        /// </summary>
        private Dictionary<string, object> ExtraParameters { get; set; }

        /// <summary>
        /// Type of linear analysis on which this Fluency analysis is based
        /// </summary>
        private readonly LinearAnalysis.TYPE LinearType;

        /// <summary>
        /// Summary of the linear analysis on which this Fluency analysis is based
        /// </summary>
        private LinearAnalysisSummary LinearSummary;
        #endregion

        #region Fields
        /// <summary>
        /// Value of the personal fluency maximum to be used, set in constructor
        /// </summary>
        private readonly double PersonalMaximum;

        /// <summary>
        /// Value of the personal task maximum to be used, determined during analysis
        /// </summary>
        private double TaskMaximum;

        /// <summary>
        /// Value of the absolute fluency maximum to be used, set in constructor
        /// </summary>
        private readonly double AbsoluteMaximum;

        /// <summary>
        /// Minimum size of periods in order to be included in FluencyAnalysisSummary
        /// </summary>
        public const ulong PERIOD_MIN_SIZE = 10000;

        /// <summary>
        /// Default value for absolute fluency maximum 
        /// </summary>
        public const int DEFAULT_ABSOLUTE_MAXIMUM = 400;

        /// <summary>
        /// If true, only read character production events are counted, otherwise every stroke counts
        /// </summary>
        private readonly bool OnlyCharProduction;

        private LinearAnalysisSummary SmallChunkSummary;
        private double SmallChunkMean;
        private double SmallChunkStdDev;

        private readonly TaskMaximumMode TaskMaxMode;
        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// Constructs a new FluencyAnalysis. 
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="pauseThreshold">The used pause threshold (how long the pause between 
        /// 2 events should be before we actually mark it as a pause).</param>
        /// <param name="linearType"></param>
        /// <param name="absoluteMax">Absolute fluency maximum to be used for PeriodStats</param>
        /// <param name="personalMax">Personal fluency maximum to be used for PeriodStats</param>
        /// <param name="typeParam">Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of linear analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that interval size in msec.
        /// e.g. when the typed is "fixed number of intervals",
        ///  the TypeParam will represent the number of intervals.</param>
        /// <param name="onlyCharProduction">Bool indicating whether each keystroke should be counted, 
        /// or only character production</param>
        /// <param name="tMaxMode"></param>
        /// <param name="controlKeys">List of controlKeys that need to be taken into account 
        /// (controlkeys are displayed differently in the analysis).</param>
        /// <param name="extraParameters">Dictionary containing extra parameters depending on the linearAnalysisType</param>
        public FluencyAnalysis(List<Event> events, SessionIdentification sessionID, LinearAnalysis.TYPE linearType,
            int absoluteMax, int personalMax, ulong pauseThreshold, ulong typeParam, bool onlyCharProduction,
            TaskMaximumMode tMaxMode, List<KeysEx> controlKeys = null, Dictionary<string,object> extraParameters = null)
            : base("FLUA", events, sessionID)
        {
            ExtraParameters = extraParameters;
            TypeParam = typeParam;
            ControlKeys = controlKeys;
            PauseThreshold = pauseThreshold;
            PersonalMaximum = personalMax;
            AbsoluteMaximum = absoluteMax;
            LinearType = linearType;
            OnlyCharProduction = onlyCharProduction;
            TaskMaxMode = tMaxMode;
        }

        /// <summary>
        /// Performs the actual Fluency Analysis on the periods found in the linear analysis
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            var summary = new FluencyAnalysisSummary {LinearType = LinearType};
            try
            {
                // Perform linear analysis as base
                DoLinearAnalysis();
                TaskMaximumPeriod maxPeriod = FindTaskMaximum();
                summary.AddMaxima(AbsoluteMaximum, PersonalMaximum, TaskMaximum, maxPeriod);

                // Compute stats for each period > PERIOD_MIN_SIZE
                foreach (var period in LinearSummary.Periods)
                { 
                    ulong periodLength = period.Length();
                    if (periodLength >= PERIOD_MIN_SIZE)
                    {
                        var periodCount = EventCount(period);
                        var ps = new PeriodStats(periodCount, periodLength, AbsoluteMaximum, TaskMaximum, PersonalMaximum);
                        summary.AddPeriod(period, ps);
                    }
                }
                ComputeStdDev();
                summary.AddStdDev(SmallChunkStdDev);
                summary.ComputeStats();
            }
            catch (AnalysisException e)
            {
                Log.Warn(e); // exception while analyzing an event, log it and skip it...
            }
            return summary;
        }

        /// <summary>
        /// Finds and sets TaskMaximum, and returns the period where maximum was found
        /// See developer documentation for algorithm to find task maximum
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        private TaskMaximumPeriod FindTaskMaximum()
        {
            switch (TaskMaxMode)
            {
                case TaskMaximumMode.BASIC:
                    return FindTaskMaximumBasic();
                case TaskMaximumMode.INTERVAL_DEPENDENT:
                    return FindTaskMaximumIntervalDependent();
            }
            return null;
        }

        private TaskMaximumPeriod FindTaskMaximumGeneric(ulong chunkLength, int window)
        {
            SmallChunkSummary = GetSmallChunkSummary(chunkLength);

            // Determine task maximum via Rolling Average
            var avg = new RollingAverage(window);
            TaskMaximum = 0;
            ulong maxPeriodEnd = 0;
            ulong rollingEnd = 0;
            SmallChunkMean = 0;
            foreach (var period in SmallChunkSummary.Periods)
            {
                int ec = EventCount(period);
                SmallChunkMean += ec;
                rollingEnd += period.Length();
                avg.Add(StrokesPerMinute(ec, period.Length()));
                double cAvg = avg.GetAverage();
                if (cAvg > TaskMaximum)
                {
                    TaskMaximum = cAvg;
                    maxPeriodEnd = rollingEnd;
                }
            }
            TaskMaximum = Math.Ceiling(TaskMaximum);
            SmallChunkMean = SmallChunkMean / SmallChunkSummary.Periods.Count;
            return new TaskMaximumPeriod(window, chunkLength, maxPeriodEnd);
        }

        private TaskMaximumPeriod FindTaskMaximumBasic()
        {
            return FindTaskMaximumGeneric(10000, 3);
        }

        private TaskMaximumPeriod FindTaskMaximumIntervalDependent()
        {
            // Determine size of small chunks: window is determined via reverse factorial,
            // chunk size is minimum interval size / window size
            ulong minPeriodLength = GetMinimumPeriodLength();
            int window = RollingAverageWindow(minPeriodLength);
            ulong chunkLength = minPeriodLength / (ulong)window;
            return FindTaskMaximumGeneric(chunkLength, window);
        }

        /// <summary>
        /// Performs linear analysis used as base for the fluency analysis
        /// </summary>
        private void DoLinearAnalysis()
        {
            LinearAnalysis linear = new LinearAnalysis(InputEvents, SessionIdentification, LinearType,
                                                       PauseThreshold, TypeParam, ControlKeys, ExtraParameters);
            LinearSummary = (LinearAnalysisSummary)linear.DoAnalysis();
        }

        /// <summary>
        /// Performs linear analysis with smaller chunks used for finding task maximum
        /// </summary>
        private LinearAnalysisSummary GetSmallChunkSummary(ulong chunkLength)
        {
            LinearAnalysis smallChunkLinear = new LinearAnalysis(InputEvents, SessionIdentification,
                LinearAnalysis.TYPE.FIXED_LENGTH_INTERVALS, PauseThreshold, chunkLength, ControlKeys, ExtraParameters);
            return (LinearAnalysisSummary)smallChunkLinear.DoAnalysis();
        }

        /// <summary>
        /// Returns the size of the rolling average window for the given minPeriodLength
        /// </summary>
        private static int RollingAverageWindow(ulong minPeriodLength)
        {
            ulong xDiv30 = minPeriodLength / 30000;
            return InverseFactorial(xDiv30) + 1;
        }

        /// <summary>
        /// Hardcoded reverse factorial for certain values
        /// </summary>
        private static int InverseFactorial(ulong xDiv30)
        {
            if (xDiv30 >= 720) return 6;
            if (xDiv30 >= 120) return 5;
            if (xDiv30 >= 24) return 4;
            if (xDiv30 >= 6) return 3;
            if (xDiv30 < 2) return 1;
            return 2;
        }

        /// <summary>
        /// Computes strokes per minute from a given keystroke count and interval length
        /// </summary>
        private static int StrokesPerMinute(int periodCount, ulong length)
        {
            return Convert.ToInt32(Math.Ceiling((double)(periodCount * 60000) / length));
        }

        /// <summary>
        /// Returns the length of the smallest period > PERIOD_MIN_SIZE
        /// </summary>
        private ulong GetMinimumPeriodLength()
        {
            ulong minLength = ulong.MaxValue;
            foreach (var period in LinearSummary.Periods)
            {
                ulong periodLength = period.Length();
                if (periodLength >= PERIOD_MIN_SIZE && periodLength < minLength)
                {
                    minLength = periodLength;
                }
            }
            return minLength;
        }

        /// <summary>
        /// Returns the Task Maximum
        /// </summary>
        public double GetTaskMaximum()
        {
            return TaskMaximum;
        }

        /// <summary> Counts the number of keystrokes in a given period
        /// TODO: use Pause Treshold to filter non-events (events with very small pause time)
        /// </summary>
        private int EventCount(LinearAnalysisSummary.AbstractPeriod period)
        {
            int periodCount = 0;
            foreach (var periodEvent in period)
            {
                if (OnlyCharProduction)
                {
                    if (periodEvent is LinearAnalysisSummary.RegularEvent)
                    {
                        periodCount++;
                    }
                }
                else if (!(periodEvent is LinearAnalysisSummary.PauseEvent))
                {
                    periodCount++;
                }
            }
            return periodCount;
        }

        private void ComputeStdDev()
        {
            var variance = SmallChunkSummary.Periods.Select(p => EventCount(p) - SmallChunkMean).Select(diff => (diff*diff)).Sum();
            SmallChunkStdDev = Math.Sqrt(variance / SmallChunkSummary.Periods.Count);
        }
    }
}