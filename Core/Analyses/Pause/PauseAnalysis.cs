using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;
using log4net;

namespace InputLog.Core.Analyses.Pause
{
    /// <summary>
    /// A Pause Analysis.
    /// The pause analysis determines various statistics relating to pause times for a given log.
    /// The result of the analysis consists of 3 parts:
    /// * A General Overview of the pauses
    ///     In this case, the total number of pauses, total pause time, mean pause time, standard deviation, etc. 
    ///     of the log are determined
    /// 
    /// * The statistics per pause type (the terms pause type and pause location are both used to denote the same thing).
    ///     In this case, the number of pauses per pause type is counted, 
    ///     and from that some other statistics are determined
    ///     (the mean and median pause time, and standard deviation).
    ///     
    /// * The statistics per interval
    ///     In this case, the number of pauses within a given interval is counted, 
    ///     and from that some other statistics are determined (the mean and median pause time, and standard deviation).
    ///     The number of intervals and their startTimes are determined by parameters passed to the constructor.
    /// </summary>
    public class PauseAnalysis : Analysis
    {

        #region Fields

        /// <summary>
        /// Different types of pause analysis.
        /// Indicates how the interval size is determined.
        /// </summary>
        public enum IntervalType
        {
            FIXED_NUMBER_OF_INTERVALS,
            FIXED_LENGTH_INTERVALS
        }

        /// <summary>
        /// Event types we wish to do the analysis on (all other types are ignored).
        /// </summary>
        private static readonly string[] EventTypes =
        {
            EventType.KEYBOARD, EventType.MOUSE, EventType.FOCUS,
            EventType.REPLACEMENT, EventType.INSERT
        };

        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of pauseAnalysis analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that 
        /// interval size in msec.
        /// e.g. when the typed is "fixed number of intervals", the TypeParam will represent 
        /// the number of intervals.
        /// </summary>
        private readonly ulong _intervalParam;

        /// <summary>
        /// The used pause threshold (how long the pause between 2 events should be before we mark it as a pause).
        /// </summary>
        private ulong _currentPauseThreshold;

        /// <summary>
        /// The default pause threshold (from the GUI) 
        /// </summary>
        private readonly ulong _defaultPauseThreshold;

        /// <summary>
        /// The threshold used to calculate the P-Bursts as set in the GUI.
        /// </summary>
        private readonly ulong _pBurstThreshold;

        /// <summary>
        /// If different P-Burst thresholds are used in multiple analysis.
        /// </summary>
        private ulong _currentPBurstThreshold;

        /// <summary>
        /// Sets the initial time.
        /// </summary>
        private ulong _initialStartTime;

        private string mainDocTitle;

        private bool _fromMainDoc;
        // focus event 
        private Event _lastFocus;

        /// <summary>
        /// Dictionary mapping the different intervals to their statistics.
        /// </summary>
        private IDictionary<ulong, SingularPauseAnalysisSummary.IntervalStats> StatsPerInterval;

        private IDictionary<ulong, List<ulong>> PausesPerInterval;
        private IDictionary<ulong, List<double>> LogPausesPerInterval;

        /// <summary>
        /// Dictionary mapping the different pauseTypes (pauselocations) to their statistics.
        /// </summary>
        private IDictionary<PauseLocation, SingularPauseAnalysisSummary.CombinedStats> StatsCombinedTypes;

        private IDictionary<PauseLocation, SingularPauseAnalysisSummary.BetweenBeforeStats> StatsBeforeType;
        private IDictionary<PauseLocation, SingularPauseAnalysisSummary.BetweenAfterStats> StatsAfterType;
        private IDictionary<PauseLocation, SingularPauseAnalysisSummary.BaseStats> StatsPerBaseType;
        private IDictionary<PauseLocation, SingularPauseAnalysisSummary.MiscellaneousStats> StatsPerMiscellaneousType;
        private IDictionary<PauseLocation, List<ulong>> PausesPerBetweenType;
        private IDictionary<PauseLocation, List<ulong>> PausesPerBaseType;
        private IDictionary<PauseLocation, List<ulong>> PausesPerMiscellaneousType;
        private IDictionary<PauseLocation, List<ulong>> CombinedPauseList;
        private IDictionary<PauseLocation, List<double>> LogPausesPerBetweenType;
        private IDictionary<PauseLocation, List<double>> LogPausesPerBaseType;
        private IDictionary<PauseLocation, List<double>> LogPausesPerMiscellaneousType;
        private IDictionary<PauseLocation, List<double>> LogCombinedPauseList;

        /// <summary>
        /// The type of pause analysis that is performed (using a fixed number of intervals, 
        /// fixed interval length, ...).
        /// </summary>
        private readonly IntervalType Type;

        /// <summary>
        /// Counter indicating the current interval.
        /// </summary>
        private ulong CurrentInterval;

        /// <summary>
        /// Size of an interval.
        /// </summary>
        private ulong IntervalSize;

        /// <summary>
        /// Number of intervals in the given list of events.
        /// </summary>
        private ulong NumberOfIntervals;

        /// <summary>
        /// Total process time (= actual total logging time that is being considered).
        /// </summary>
        private ulong TotalProcessTime;

        /// <summary>
        /// Ratio total pause time on total process time
        /// </summary>
        private double PauseTimeProportion;

        /// <summary>
        /// A list of every pause needed to define the median value.
        /// </summary>
        private List<ulong> AllPauses;

        /// <summary>
        /// A list of the log of every pause needed to define mean statistics.
        /// </summary>
        private List<double> AllLogPauses;

        /// <summary>
        /// The event list with the pause location and filtered on the event types used in this analysis.
        /// </summary>
        private List<Pair<Event, PauseLocation>> PauseEventList;

        /// <summary>
        /// Combined pause field used to sum the different pause times between AFTER_ and BEFORE_
        /// </summary>
        private ulong _betweenPauseCollector;

        /// <summary>
        /// P-Burst statistics
        /// </summary>
        private List<ulong> PBProcessCharList;

        private List<ulong> PBPauseTimeList;
        private List<ulong> PBProcessTimeList;
        private ulong TotalPBProcessTime;
        // segment statistics
        private ulong TotalPBSegments;
        // The previous PBurst event
        private Event PrevPBEvent;
        private ulong CurrentPBChars;
        private ulong CurrentPBSegmentStartTime;

        // Debugging only
        //private int tmpCountInitial = 1;
        //private int tmpCountSecond = 1;
        //private int tmpCountCombined = 1;
        //private int tmpCountDiscarded = 1;
        //private int mouseCount = 0;
        //private ulong tmpTotalCombinationPause;

        /// <summary>
        /// Boolean that checks whether the pause analysis should be run multiple times
        /// or not.
        /// </summary>
        private readonly bool RunMultiple;

        #endregion

        /// <summary>
        /// Constructs a new PauseAnalysis. 
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events</param>
        /// <param name="pauseAnalysisType">The type of pause analysis that is performed 
        /// (using a fixed number of intervals, fixed interval length, ...).</param>
        /// <param name="intervalParam">Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of pauseAnalysis analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that interval size in msec.
        /// e.g. when the typed is "fixed number of intervals", the TypeParam will represent the number of intervals.</param>
        /// <param name="pauseThreshold">The used pause threshold 
        /// (how long the pause between 2 events should be before we mark it as a pause).</param>
        /// <param name="pBurstThreshold">The pause time between two events used to calculate a P-Burst</param>
        /// <param name="runMultiple">True if the pause analysis should be ran multiple times and therefore
        /// a compound summary containing multiple singular pause analysis summaries.</param>
        public PauseAnalysis(List<Event> events, SessionIdentification sessionID,
            IntervalType pauseAnalysisType, ulong intervalParam, ulong pauseThreshold, ulong pBurstThreshold, bool runMultiple)
            : base("PA", events, sessionID)
        {
            InputEvents = events.Filter(new EventTypeFilter(EventTypes));
            _intervalParam = intervalParam;
            Type = pauseAnalysisType;
            _defaultPauseThreshold = pauseThreshold;
            _pBurstThreshold = pBurstThreshold;
            RunMultiple = runMultiple;
            PBProcessCharList = new List<ulong>();
            PBPauseTimeList = new List<ulong>();
            PBProcessTimeList = new List<ulong>();
            // Do we have a standard main document?
            mainDocTitle = sessionID.GetMainDocument().ToLower();
            _fromMainDoc = true;
        }

        /// <summary>
        /// Resets the pause analysis to its initial state.
        /// </summary>
        private void Reset()
        {
            StatsPerInterval = new Dictionary<ulong, SingularPauseAnalysisSummary.IntervalStats>();
            PausesPerInterval = new Dictionary<ulong, List<ulong>>();
            LogPausesPerInterval = new Dictionary<ulong, List<double>>();

            StatsPerBaseType = new Dictionary<PauseLocation, SingularPauseAnalysisSummary.BaseStats>();
            StatsPerMiscellaneousType = new Dictionary<PauseLocation, SingularPauseAnalysisSummary.MiscellaneousStats>();
            StatsCombinedTypes = new Dictionary<PauseLocation, SingularPauseAnalysisSummary.CombinedStats>();
            StatsBeforeType = new Dictionary<PauseLocation, SingularPauseAnalysisSummary.BetweenBeforeStats>();
            StatsAfterType = new Dictionary<PauseLocation, SingularPauseAnalysisSummary.BetweenAfterStats>();
            PausesPerBaseType = new Dictionary<PauseLocation, List<ulong>>();
            PausesPerBetweenType = new Dictionary<PauseLocation, List<ulong>>();
            PausesPerMiscellaneousType = new Dictionary<PauseLocation, List<ulong>>();
            CombinedPauseList = new Dictionary<PauseLocation, List<ulong>>();
            LogPausesPerBaseType = new Dictionary<PauseLocation, List<double>>();
            LogPausesPerBetweenType = new Dictionary<PauseLocation, List<double>>();
            LogPausesPerMiscellaneousType = new Dictionary<PauseLocation, List<double>>();
            LogCombinedPauseList = new Dictionary<PauseLocation, List<double>>();

            AllPauses = new List<ulong>();
            AllLogPauses = new List<double>();
            TotalProcessTime = 0;
            CurrentInterval = 0;

            // P-Burst
            // Setting all times/counts to 0.
            PBProcessCharList = new List<ulong>();
            PBPauseTimeList = new List<ulong>();
            PBProcessTimeList = new List<ulong>();
            TotalPBSegments = 0;
            TotalPBProcessTime = 0;
            CurrentPBChars = 0;
            PrevPBEvent = null;
            CurrentPBSegmentStartTime = 0;
        }

        /// <summary>
        /// Performs a Pause Analysis on the events with which this Analysis was constructed.
        /// When multiple runs are executed, the appropriate p-burst threshold is taken from 
        /// the pBurstThresholds array (set default to three time the same value).
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            CompoundPauseAnalysisSummary summary = new CompoundPauseAnalysisSummary();

            _currentPauseThreshold = _defaultPauseThreshold;
            _currentPBurstThreshold = _pBurstThreshold;
            var singleSummary = RunSinglePauseAnalysis();
            summary.Summaries[CompoundPauseAnalysisSummary.TRESHOLD_DEFAULT] = singleSummary;

            if (RunMultiple)
            {
                ulong[] pauseThresholds = {30, 1000, 2000};
                ulong[] pBurstThresholds = {2000, 2000, 2000};
                for (int index = 0; index < pauseThresholds.Length; index++)
                {
                    ulong threshold = pauseThresholds[index];
                    _currentPauseThreshold = threshold;
                    _currentPBurstThreshold = pBurstThresholds[index];
                    singleSummary = RunSinglePauseAnalysis();
                    summary.Summaries[threshold.ToString()] = singleSummary;
                }
            }

            return summary;
        }

        private SingularPauseAnalysisSummary RunSinglePauseAnalysis()
        {
            Reset();

            var pauseLocationMarker = new PauseLocationMarker();

            // Registering the count of chars in the PauseLocationMarker
            pauseLocationMarker.EventEventHandler += BeginEvent;
            pauseLocationMarker.FocusChangeEventHandler += UpdateFocusChange;

            // Runs the PauseLocationMarker.         
            PauseEventList = pauseLocationMarker.Start(InputEvents);

            // Prepares pause type-event pairs.
            InitializeAnalysis();

            // Executing the actual analysis.
            DeterminePauseStatistics();
            FixEndEvent();

            // Creates a summary.
            return CreateSummary();
        }

        /// <summary>
        /// Initializes the analysis using a given list of event-pause pairs.
        /// </summary>
        private void InitializeAnalysis()
        {
            if (PauseEventList != null)
            {
                LastCurrent = 0;
                LastPrevious = 0;
                // Finding initial start and end time.
                var startTimedEventPart = FindFirstTimedEvent();
                var endTimedEventPart = GetLastTimedEvent();

                // List without events with start and end time: abort.
                if (endTimedEventPart == null || startTimedEventPart == null)
                {
                    throw new AnalysisException(
                        "Unable to create a Pause Analysis. The IDFX does not contain timed information");
                }

                if (startTimedEventPart.EndTime != endTimedEventPart.EndTime)
                {
                    // The NewStartOffset is the machine start time to be subtracted from the logging
                    // start time to get the relative time.
                    _initialStartTime = startTimedEventPart.StartTime;
                    TotalProcessTime = endTimedEventPart.EndTime - NewStartOffset;

                    // Determines the interval size and the number of intervals
                    switch (Type)
                    {
                        case IntervalType.FIXED_LENGTH_INTERVALS:
                        {
                            IntervalSize = _intervalParam;
                            NumberOfIntervals = (ulong) Math.Ceiling((decimal) TotalProcessTime/IntervalSize);
                            break;
                        }
                        case IntervalType.FIXED_NUMBER_OF_INTERVALS:
                        {
                            NumberOfIntervals = _intervalParam;
                            IntervalSize = (ulong) Math.Ceiling((decimal) TotalProcessTime/NumberOfIntervals);
                            break;
                        }
                    }
                }
            }

            // Initializes the Interval stats
            for (ulong i = 0; i < NumberOfIntervals; i++)
            {
                StatsPerInterval[i] = new SingularPauseAnalysisSummary.IntervalStats("#1", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                PausesPerInterval[i] = new List<ulong>();
                LogPausesPerInterval[i] = new List<double>();
            }

            // Initializes Pause Location stats. Remark: MOUSE events are not reported.
            foreach (PauseLocation pauseType in Enum.GetValues(typeof(PauseLocation)))
            {
                switch (pauseType)
                {
                    // Within word pauses
                    case PauseLocation.WITHIN_WORDS:
                        StatsPerBaseType[PauseLocation.WITHIN_WORDS] =
                            new SingularPauseAnalysisSummary.BaseStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerBaseType[PauseLocation.WITHIN_WORDS] = new List<ulong>();
                        LogPausesPerBaseType[PauseLocation.WITHIN_WORDS] = new List<double>();
                        break;

                    // Miscellaneous pauses.
                    case PauseLocation.INITIAL:
                        StatsPerMiscellaneousType[PauseLocation.INITIAL] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.INITIAL] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.INITIAL] = new List<double>();
                        break;
                    case PauseLocation.END:
                        StatsPerMiscellaneousType[PauseLocation.END] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.END] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.END] = new List<double>();
                        break;
                    case PauseLocation.COMBINATION_KEY:
                        StatsPerMiscellaneousType[PauseLocation.COMBINATION_KEY] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.COMBINATION_KEY] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.COMBINATION_KEY] = new List<double>();
                        break;
                    case PauseLocation.REVISION:
                        StatsPerMiscellaneousType[PauseLocation.REVISION] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.REVISION] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.REVISION] = new List<double>();
                        break;
                    case PauseLocation.CHANGE:
                        StatsPerMiscellaneousType[PauseLocation.CHANGE] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.CHANGE] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.CHANGE] = new List<double>();
                        break;
                    case PauseLocation.UNKNOWN:
                    case PauseLocation.UNDETERMINED:
                        StatsPerMiscellaneousType[PauseLocation.UNKNOWN] =
                            new SingularPauseAnalysisSummary.MiscellaneousStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerMiscellaneousType[PauseLocation.UNKNOWN] = new List<ulong>();
                        LogPausesPerMiscellaneousType[PauseLocation.UNKNOWN] = new List<double>();
                        break;

                    // Before - After Pauses Also used to calculate the BETWEEN stats called Combined Pauses.
                    case PauseLocation.BEFORE_PARAGRAPHS:
                    case PauseLocation.AFTER_PARAGRAPHS:
                    case PauseLocation.BEFORE_SENTENCES:
                    case PauseLocation.AFTER_SENTENCES:
                    case PauseLocation.BEFORE_WORDS:
                    case PauseLocation.AFTER_WORDS:
                        StatsCombinedTypes[pauseType] =
                            new SingularPauseAnalysisSummary.CombinedStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        StatsBeforeType[pauseType] =
                            new SingularPauseAnalysisSummary.BetweenBeforeStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        StatsAfterType[pauseType] =
                            new SingularPauseAnalysisSummary.BetweenAfterStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        PausesPerBetweenType[pauseType] = new List<ulong>();
                        CombinedPauseList[pauseType] = new List<ulong>();
                        LogPausesPerBetweenType[pauseType] = new List<double>();
                        LogCombinedPauseList[pauseType] = new List<double>();
                        break;
                }
            }
        }

        /// <summary>
        /// Executes the actual Pause Analysis given a list of pause type-event pairs.
        /// </summary>
        private void DeterminePauseStatistics()
        {
            List<ulong> baseTypePauses = null;
            List<ulong> combinedPauses = null;
            List<ulong> miscellaneousPauses = null;
            List<ulong> betweenTypePauses = null;
            List<double> logBaseTypePauses = null;
            List<double> logCombinedPauses = null;
            List<double> logMiscellaneousPauses = null;
            List<double> logBetweenTypePauses = null;

            Event prevEvent = null;
            ulong prevPauseTime = 0;
            var prevPauseLocation = (PauseLocation) 0;
            //ulong previousEndTime = 0;

            for (int i = 0; i < PauseEventList.Count; i++)
            {
                int currentIndex = i;
                var eventPausePair = PauseEventList[currentIndex];
                try
                {
                    var currentEvent = eventPausePair.First;
                    // The pause location for this event.
                    var pauseLocation = eventPausePair.Second;

                    ulong currentStartTime = 0;
                    ulong currentEndTime = 0;
                    var timedEventPartCurrent = Event.GetFirstEventPart<TimedEventPart>(currentEvent);
                    if (timedEventPartCurrent != null)
                    {
                        currentStartTime = timedEventPartCurrent.StartTime;
                        currentEndTime = timedEventPartCurrent.EndTime;
                    }

                    // Commenting out the following method. Reason: the DeterminePauseTime method in the Analysis base class
                    // already handles the situation described here. Doing it twice generates faulty results.
                    // 
                    // Hack to catch strange behavior: a late transmission of a focus change or mouse event.
                    // The focusChange or Mouse event gets the time of the previous event.
                    //if (currentEvent.Type.Equals(EventType.FOCUS) || currentEvent.Type.Equals(EventType.MOUSE))
                    //{
                    //    if (currentStartTime < previousEndTime)
                    //    {
                    //        if (currentEvent.Type.Equals(EventType.FOCUS))
                    //        {
                    //            FocusChange focusLog = currentEvent.Parts.OfType<FocusChange>().FirstOrDefault();
                    //            int focusIndex = currentEvent.Parts.IndexOf(focusLog);
                    //            ((FocusChange) currentEvent.Parts[focusIndex]).StartTime = previousEndTime;
                    //            ((FocusChange) currentEvent.Parts[focusIndex]).EndTime = previousEndTime;
                    //        }

                    //        // We capture the duration (delta) between original but faulty endTime and startTime, adding it 
                    //        // to the previousTime, so as not to lose that information.
                    //        else
                    //        {
                    //            if (!currentEvent.Parts.OfType<MouseMovement>().IsNullOrEmpty())
                    //            {
                    //                MouseMovement moveLog = currentEvent.Parts.OfType<MouseMovement>().FirstOrDefault();
                    //                int moveIndex = currentEvent.Parts.IndexOf(moveLog);
                    //                var delta = ((MouseMovement) currentEvent.Parts[moveIndex]).EndTime -
                    //                            ((MouseMovement) currentEvent.Parts[moveIndex]).StartTime;
                    //                ((MouseMovement) currentEvent.Parts[moveIndex]).StartTime = previousEndTime;
                    //                ((MouseMovement) currentEvent.Parts[moveIndex]).EndTime = previousEndTime + delta;
                    //            }
                    //            else if (!currentEvent.Parts.OfType<Click>().IsNullOrEmpty())
                    //            {
                    //                Click clickLog = currentEvent.Parts.OfType<Click>().FirstOrDefault();
                    //                int clickIndex = currentEvent.Parts.IndexOf(clickLog);
                    //                var delta = ((Click) currentEvent.Parts[clickIndex]).EndTime -
                    //                            ((Click) currentEvent.Parts[clickIndex]).StartTime;
                    //                ((Click) currentEvent.Parts[clickIndex]).StartTime = previousEndTime;
                    //                ((Click) currentEvent.Parts[clickIndex]).EndTime = previousEndTime + delta;
                    //            }
                    //        }
                    //    }
                    //}

                    // If an event happens to be the very last of the idfx (this can happen after filtering), the interval boundaries don't
                    // fit and we give the time of the last event to the previous one.
                    // Reason: the time boundary of the intervals is calculated with the time of the last event.
                    if (currentIndex == PauseEventList.Count - 1)
                    {
                        var prevTimedEvent = Event.GetFirstEventPart<TimedEventPart>(prevEvent);
                        if (prevTimedEvent != null)
                        {
                            prevTimedEvent.StartTime = currentStartTime;
                            prevTimedEvent.EndTime = currentEndTime;
                        }
                    }

                    //previousEndTime = currentEndTime;

                    // Calculates the pause time for this event.
                    ulong pauseTime = DeterminePauseTime(prevEvent, currentEvent);
                    // Time counted twice and to be subtracted from the total pause time
                    // See the next comment for its use and reason why.
                    ulong doubleTimeCount = 0;

                    // To calculate the Combined Pause Locations (BETWEEN WORDS/SENTENCES/PARAGRAPHS), we take the sum
                    // of the current pauseTime and that of the preceding COMBINATION_KEY.
                    // Reason: the writing process starts cognitively when the user presses e.g. a shift key in order
                    // to generate a capital letter 'A', or any other combination key to obtain a combined result.
                    // We used to use only the pause time of the main key 'a', which is often very short and is
                    // consequently discarded because it falls under the pause threshold, missing a number of combined constructs.
                    if (prevPauseLocation == PauseLocation.COMBINATION_KEY && (pauseLocation == PauseLocation.BEFORE_PARAGRAPHS
                                                                           || pauseLocation == PauseLocation.AFTER_PARAGRAPHS
                                                                           || pauseLocation == PauseLocation.BEFORE_SENTENCES
                                                                           || pauseLocation == PauseLocation.AFTER_SENTENCES
                                                                           || pauseLocation == PauseLocation.BEFORE_WORDS
                                                                           || pauseLocation == PauseLocation.AFTER_WORDS))
                    {
                        // Debug
                        //Console.WriteLine("*** Count: " + tmpCountInitial + " - Previous PauseLocation: COMBINATION_KEY " + " - Previous event id: " 
                        //                  + prevEvent.GetId() + " - Previous pause time: " + prevPauseTime + "\n    Current PauseLocation: "
                        //                  + pauseLocation + " - Current event id: " + currentEvent.GetId() + " - Current pause time: " + pauseTime
                        //                 + "\n    New Pause Time: " + (prevPauseTime + pauseTime));
                        //tmpCountInitial++;

                        pauseTime = prevPauseTime + pauseTime;
                        doubleTimeCount = prevPauseTime;

                        //Debug
                        //Console.WriteLine("Current event id: " + currentEvent.GetId() + " doubleTime: " + doubleTimeCount);
                    }

                    switch (pauseLocation)
                    {
                        case PauseLocation.WITHIN_WORDS:
                            baseTypePauses = PausesPerBaseType[pauseLocation];
                            logBaseTypePauses = LogPausesPerBaseType[pauseLocation];
                            break;
                        case PauseLocation.INITIAL:
                            miscellaneousPauses = PausesPerMiscellaneousType[PauseLocation.INITIAL];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[PauseLocation.INITIAL];
                            break;
                        case PauseLocation.END:
                            miscellaneousPauses = PausesPerMiscellaneousType[PauseLocation.END];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[PauseLocation.END];
                            break;
                        case PauseLocation.REVISION:
                            miscellaneousPauses = PausesPerMiscellaneousType[PauseLocation.REVISION];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[PauseLocation.REVISION];
                            break;
                        case PauseLocation.COMBINATION_KEY:
                            miscellaneousPauses = PausesPerMiscellaneousType[pauseLocation];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[pauseLocation];
                            break;
                        case PauseLocation.CHANGE:
                            miscellaneousPauses = PausesPerMiscellaneousType[PauseLocation.CHANGE];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[PauseLocation.CHANGE];
                            break;
                        case PauseLocation.UNKNOWN:
                        case PauseLocation.UNDETERMINED:
                            miscellaneousPauses = PausesPerMiscellaneousType[PauseLocation.UNKNOWN];
                            logMiscellaneousPauses = LogPausesPerMiscellaneousType[PauseLocation.UNKNOWN];
                            break;
                        case PauseLocation.BEFORE_PARAGRAPHS:
                        case PauseLocation.AFTER_PARAGRAPHS:
                        case PauseLocation.BEFORE_SENTENCES:
                        case PauseLocation.AFTER_SENTENCES:
                        case PauseLocation.BEFORE_WORDS:
                        case PauseLocation.AFTER_WORDS:
                            combinedPauses = CombinedPauseList[pauseLocation];
                            betweenTypePauses = PausesPerBetweenType[pauseLocation];
                            logCombinedPauses = LogCombinedPauseList[pauseLocation];
                            logBetweenTypePauses = LogPausesPerBetweenType[pauseLocation];
                            break;
                    }

                    List<ulong> currentPauses = PausesPerInterval[CurrentInterval];
                    List<double> currentLogPauses = LogPausesPerInterval[CurrentInterval];

                    // Combined pauses are the sum of the certain event pauses between an 'AFTER_' and a 'BEFORE_'.
                    // The pauseTime may contain a 'doubleTimeCount' from a possible COMBINATION_KEY pause preceding this event.
                    // If there was no COMBINATION_KEY the value of the doubleTimeCount is zero.
                    // 'currentEvent.GetId()' is for debug purposes only. 
                    GetCombinedPause(currentEvent.GetId(), pauseLocation, pauseTime, combinedPauses, logCombinedPauses, doubleTimeCount);

                    // After handling the combined pauses we revert to using the normal pauseTime without the added COMBINATION_KEY pause.
                    pauseTime -= doubleTimeCount;

                    // Handling the other cases.
                    if (pauseTime >= _currentPauseThreshold)
                    {
                        // Changes statistics only if pause is larger than pause threshold.
                        AllPauses.Add(pauseTime);
                        AllLogPauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                        currentPauses.Add(pauseTime);
                        currentLogPauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));

                        switch (pauseLocation)
                        {
                            case PauseLocation.WITHIN_WORDS:
                                baseTypePauses.Add(pauseTime);
                                logBaseTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.CHANGE:
                            case PauseLocation.UNDETERMINED:
                            case PauseLocation.UNKNOWN:
                            case PauseLocation.REVISION:
                            case PauseLocation.COMBINATION_KEY:
                            case PauseLocation.END:
                            case PauseLocation.INITIAL:
                                miscellaneousPauses.Add(pauseTime);
                                logMiscellaneousPauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.BEFORE_WORDS:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.AFTER_WORDS:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.BEFORE_SENTENCES:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.AFTER_SENTENCES:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.BEFORE_PARAGRAPHS:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                            case PauseLocation.AFTER_PARAGRAPHS:
                                betweenTypePauses.Add(pauseTime);
                                logBetweenTypePauses.Add(pauseTime == 0 ? 0 : Math.Log(Convert.ToDouble(pauseTime)));
                                break;
                        }
                    }
                    // Debug only - listing the pause locations with pauseTime < _currentPauseThreshold
                    //else
                    //{
                    //    //if (pauseLocation == PauseLocation.MOUSE)
                    //        if (pauseLocation != PauseLocation.CHANGE &&
                    //            pauseLocation != PauseLocation.MOUSE &&
                    //            pauseLocation != PauseLocation.UNKNOWN &&
                    //            pauseLocation != PauseLocation.REVISION)
                    //        {
                    //        Console.WriteLine(pauseLocation + "\t" + currentEvent.GetId() + "\t" + pauseTime);
                    //    }
                    //}

                    // In earlier versions the previous event was defined on line xxx (right before the 'catch' clause),
                    // but on two occasions (line xxx and line xxx) the loop statement could pass control to the next iteration,
                    // skipping the 'prevEvent = currentEvent' expression.
                    // If that was the case, the next pause time is wrong because the available 'previous event' 
                    // is not the real 'previous'.
                    prevEvent = currentEvent;
                    prevPauseTime = pauseTime;
                    prevPauseLocation = pauseLocation;

                    // 20150528 Bug resolved: the previous version checked for null, but assigned '0' to the
                    // currentEventStartTime when the TimedEventPart was null, which resulted in an endless loop 
                    // in the 'while' function below and finally in an exception because the CurrentInterval grew too large.
                    var currentEventTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(currentEvent);
                    if (null == currentEventTimedEventPart)
                    {
                        // Ignore this event
                        continue;
                    }
                    var currentEventStartTime = currentEventTimedEventPart.StartTime;
                    if (currentEventStartTime == 0)
                    {
                        // Ignore this event
                        continue;
                    }

                    // Adding new intervals/periods until we reach the interval of which the current event is part of.
                    // One millisec is added to 'IntervalSize' to resolve the case where the IntervalSize is exactly on the
                    // border of a new interval.
                    while (currentEventStartTime - _initialStartTime >= IntervalSize*(CurrentInterval + 1) + 1)
                    {
                        try
                        {
                            CurrentInterval++;
                            StatsPerInterval[CurrentInterval].IntervalStart = IntervalSize*CurrentInterval;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine();
                        }
                    }
                }
                catch (AnalysisException e)
                {
                    Log.Warn(e); // exception while analyzing an event, log it and skip it...
                }
            }
            ////Debug
            //Console.WriteLine("MOUSE count: " + mouseCount);
        }

        /// <summary>
        /// A 'combined pause' is the sum the pause time of an AFTER pause location with the pause time
        /// of a BEFORE(WORDS/SENTENCES/PARAGRAPHS) that follows and everything between them.
        /// </summary>
        /// <param name="index">For debug purposes only. The id of the current event</param>
        /// <param name="pauseLocation">the current pause location</param>
        /// <param name="pauseTime">the pause time of the current location augmented with the pause time
        /// of the COMBINATION_KEY if it preceded this event.</param>
        /// <param name="combinedPauses">the list collecting the combined pause time for a given AFTER-BEFORE sequence</param>
        /// <param name="logCombinedPauses">the list collecting the log of the combined pause time for a given AFTER-BEFORE sequence</param>
        private void GetCombinedPause(int index, PauseLocation pauseLocation, ulong pauseTime, List<ulong> combinedPauses,
            List<double> logCombinedPauses, ulong combinationPause)
        {
            // The "AFTER" pause time is saved before returning. We expect the next event to be 'BEFORE.
            if (pauseLocation.ToString().Contains("AFTER"))
            {
                // We save the pauseTime for the current AFTER. We do not accumulate, so that in the case of
                // several consecutive AFTER only the last one counts.
                _betweenPauseCollector = pauseTime;

                // Debug
                //tmpTotalCombinationPause = combinationPause;
                //if (tmpCountInitial > 1)
                //{
                //    tmpCountInitial = 0;
                //    Console.WriteLine("Another'AFTER' encountered. BETWEEN calculation restarts at index: " + index + " - count: 1");
                //}
                //else
                //{
                //    Console.WriteLine(pauseLocation + " encountered at index: " + index + " - pauseTime:  " + _betweenPauseCollector + " - count: 1");
                //}
                //tmpCountInitial++;

                return;
            }

            // We have seen 'AFTER' but not yet 'BEFORE'. We inspect the pause time of the intermediate pause locations.
            // Certain pause locations break the 'BETWEEN' routine by setting de betweenPauseCollector to zero.
            // We list them explicitly for clarity. Consequently only the pause time of COMBINATION KEYS will be allowed
            // between an AFTER and a BEFORE. 
            if (!pauseLocation.ToString().Contains("BEFORE") && _betweenPauseCollector > 0)
            {
                if (pauseLocation == PauseLocation.WITHIN_WORDS
                    || pauseLocation == PauseLocation.REVISION 
                    || pauseLocation == PauseLocation.MOUSE
                    || pauseLocation == PauseLocation.CHANGE
                    || pauseLocation == PauseLocation.UNDETERMINED
                    || pauseLocation == PauseLocation.UNKNOWN
                    || pauseLocation == PauseLocation.END
                    || pauseLocation == PauseLocation.INITIAL)
                {
                    _betweenPauseCollector = 0;

                    // Debug
                    //tmpTotalCombinationPause = 0;
                    //Console.WriteLine("BETWEEN calculation ended at index: " + index + " - Reason: " + pauseLocation);
                    //tmpCountSecond = 1;
                    //tmpCountInitial = 1;

                    return;
                }
                _betweenPauseCollector += pauseTime;

                // Debug
                //tmpTotalCombinationPause += combinationPause;
                //tmpCountSecond++;
                //if (tmpTotalCombinationPause > 0)
                //{
                //    Console.WriteLine("Not yet at 'BEFORE' at index: " + index + " - " + pauseLocation + " - this pauseTime: "
                //                      + pauseTime + " - pauseTime collected so far:  " + _betweenPauseCollector +
                //                      " - total combinationPauses collected: " + tmpTotalCombinationPause + " - count: " + tmpCountSecond);
                //}
                //else
                //{
                //    Console.WriteLine("Not yet at 'BEFORE' at index: " + index + " - " + pauseLocation + " - this pauseTime: "
                //                      + pauseTime + " - pauseTime collected so far:  " + _betweenPauseCollector + " - count: " + tmpCountSecond);
                //}

                return;
            }
            // These are the BEFORE pause locations we are looking for.
            switch (pauseLocation)
            {
                case PauseLocation.BEFORE_WORDS:
                    if (_betweenPauseCollector == 0)
                    {
                        return;
                    }
                    _betweenPauseCollector += pauseTime;

                    // Adding the 'Between Words' stats
                    // if the combined pause is larger than pause threshold
                    if (_betweenPauseCollector >= _currentPauseThreshold)
                    {
                        combinedPauses.Add(_betweenPauseCollector);
                        logCombinedPauses.Add(_betweenPauseCollector == 0 ? 0 : Math.Log(Convert.ToDouble(_betweenPauseCollector)));
                    }

                    // Debug
                    //tmpTotalCombinationPause += combinationPause;
                    //tmpCountCombined = tmpCountSecond + 1;
                    //if (tmpTotalCombinationPause > 0)
                    //{
                    //    Console.WriteLine("'BEFORE WORDS' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector + " without Combinationkey pauses: "
                    //                      + (_betweenPauseCollector - tmpTotalCombinationPause) + " - count: " + tmpCountCombined + "\n");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("'BEFORE WORDS' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector  + " - count: " + tmpCountCombined + "\n");
                    //}
                    //tmpCountCombined = 0;
                    //tmpCountInitial = 1;
                    //tmpCountSecond = 1;
                    //tmpTotalCombinationPause = 0;

                    _betweenPauseCollector = 0;
                    break;

                case PauseLocation.BEFORE_SENTENCES:
                    if (_betweenPauseCollector == 0)
                    {
                        return;
                    }
                    _betweenPauseCollector += pauseTime;

                    // Adding the 'Between Sentences' stats
                    // if the combined pause is larger than pause threshold
                    if (_betweenPauseCollector >= _currentPauseThreshold)
                    {
                        combinedPauses.Add(_betweenPauseCollector);
                        logCombinedPauses.Add(_betweenPauseCollector == 0 ? 0 : Math.Log(Convert.ToDouble(_betweenPauseCollector)));
                    }

                    // Debug
                    //tmpTotalCombinationPause += combinationPause;
                    //tmpCountCombined = tmpCountSecond + 1;
                    //if (tmpTotalCombinationPause > 0)
                    //{
                    //    Console.WriteLine("'BEFORE SENTENCES' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector + " without Combinationkey pauses: "
                    //                      + (_betweenPauseCollector - tmpTotalCombinationPause) + " - count: " + tmpCountCombined + "\n");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("'BEFORE SENTENCES' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector + " - count: " + tmpCountCombined + "\n");
                    //}
                    //tmpCountCombined = 0;
                    //tmpCountInitial = 1;
                    //tmpCountSecond = 1;
                    //tmpTotalCombinationPause = 0;

                    _betweenPauseCollector = 0;
                    break;

                case PauseLocation.BEFORE_PARAGRAPHS:
                    if (_betweenPauseCollector == 0)
                    {
                        return;
                    }
                    _betweenPauseCollector += pauseTime;

                    // Adding the 'Between Paragraphs' stats
                    // if the combined pause is larger than pause threshold
                    if (_betweenPauseCollector >= _currentPauseThreshold)
                    {
                        combinedPauses.Add(_betweenPauseCollector);
                        logCombinedPauses.Add(_betweenPauseCollector == 0 ? 0 : Math.Log(Convert.ToDouble(_betweenPauseCollector)));
                    }

                    // Debug
                    //tmpTotalCombinationPause += combinationPause;
                    //tmpCountCombined = tmpCountSecond + 1;
                    //if (tmpTotalCombinationPause > 0)
                    //{
                    //    Console.WriteLine("'BEFORE PARAGRAPHS' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector + " without Combinationkey pauses: "
                    //                      + (_betweenPauseCollector - tmpTotalCombinationPause) + " - count: " + tmpCountCombined + "\n");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("'BEFORE PARAGRAPHS' - reached at index: " + index + " - Total pauseTime:  "
                    //                      + _betweenPauseCollector + " - count: " + tmpCountCombined + "\n");
                    //}
                    //tmpCountCombined = 0;
                    //tmpCountInitial = 1;
                    //tmpCountSecond = 1;
                    //tmpTotalCombinationPause = 0;

                    _betweenPauseCollector = 0;
                    break;
            }
        }

        #region P-Burst

        /// <summary>
        /// A p-burst is an action time from the previous key-up to current key-down, not a pause.
        /// Notifies a focus change and checks if the main document has the current focus.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker)</param>
        /// <param name="eventArgs">PauseLocation args</param>
        private void UpdateFocusChange(object sender, PauseLocationEventArgs eventArgs)
        {
            _lastFocus = eventArgs.Event;
            string docTitle = eventArgs.Event.GetWindowTitle(eventArgs.Event).ToLower();
            _fromMainDoc = docTitle.Equals(mainDocTitle);
        }

        /// <summary>
        /// EventEventHandler.
        /// This callback is called after every handled event (by the PauseLocationMarker).
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void BeginEvent(object sender, PauseLocationEventArgs eventArgs)
        {
            var currentPBEvent = eventArgs.Event;
            var keyPress = Event.GetFirstEventPart<KeyPress>(currentPBEvent);
            // First event
            if (CurrentPBSegmentStartTime == 0ul)
            {
                var currentTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(currentPBEvent);
                CurrentPBSegmentStartTime = currentTimedEventPart.StartTime;
            }
            if (PrevPBEvent != null)
            {
                UpdatePBSegments(PrevPBEvent, currentPBEvent);
            }

            if (keyPress != null && _fromMainDoc)
            {
                // White Space has a broader definition, not just the empty string.
                if (Lexical.IsWhiteSpace(keyPress.Value))
                {
                    CurrentPBChars++;
                }
                // Checking that the count == 1 and that we don't have a tab, space or enter.
                // This extra check (tab, space, enter) is necessary because in the new representation these keys also
                // have a single character representation (' ', '\t', '\n' and '\r'), while in the legacy code these keys
                // had a multiple character representation ('SPACE', 'TAB', 'ENTER'), 
                // and thus are never matched by the count == 1.
                else if (keyPress.Value.Length == 1
                    && (Lexical.IsAlphaNumeric(keyPress.Value)
                    || Lexical.IsUnicodePunctuation(keyPress.Value)))
                {
                    CurrentPBChars++;
                }
            }
            PrevPBEvent = currentPBEvent;
        }

        /// <summary>
        /// Applies a fix using the last event of the list of input events.
        /// This is necessary to have correct results for the last segment and even type cluster/segment.
        /// The last event may hold 'Word Statistics' and possibly an 'author comment' or both.
        /// </summary>
        private void FixEndEvent()
        {
            if (InputEvents.Count <= 0) return;
            var lastEvent = GetCurrentLastTimedEvent();
            var lastTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(lastEvent);
            // We assume that the last events are from the main document.
            _fromMainDoc = true;        
            TotalPBSegments++;
            if (lastTimedEventPart != null && lastTimedEventPart.StartTime >= CurrentPBSegmentStartTime)
            {
                var actionTime = lastTimedEventPart.EndTime - CurrentPBSegmentStartTime;
                var pauseTimePB = CurrentPBChars > 0 ? DeterminePauseTime(PrevPBEvent, lastEvent) : 0ul;             
                TotalPBProcessTime += actionTime;
                PBProcessTimeList.Add(actionTime);
                PBProcessCharList.Add(CurrentPBChars);
                PBPauseTimeList.Add(pauseTimePB);
            }
        }

        /// <summary>
        /// Creates a new segment if the pauseTime between the 2 given events (the previous event and the current event) 
        /// exceeds the PBurst threshold.
        /// 
        /// </summary>
        /// <param name="pEvent">The previous event.</param>
        /// <param name="cEvent">The current event.</param>
        private void UpdatePBSegments(Event pEvent, Event cEvent)
        {
            // The PauseLocationMarker delegates a Focus event only to the FocusChangeEventHandler.
            // Hence, between the current event (cEvent) and the previous (pEvent) there is an event missing when the 
            // real previous event was a Focus change, resulting in a faulty pauseTime calculation.
            // Therefore, if the previous id (not the pEvent) belongs to a Focus change we return a pauseTime of zero.
            var pauseTimePB = _lastFocus != null && (cEvent.GetId() - 1 == _lastFocus.GetId())
                ? 0ul
                : DeterminePauseTime(pEvent, cEvent);

            var prevTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(pEvent);
            var currentTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(cEvent);

            // If the pauseTime is larger than the PBurst threshold, we have a new segment.
            // Updating the statistics and creating a new segment.
            if (pauseTimePB < _currentPBurstThreshold) return;
            // Increases total pauses.
            PBPauseTimeList.Add(pauseTimePB);
            PBProcessCharList.Add(CurrentPBChars);
            CurrentPBChars = 0;

            if (prevTimedEventPart == null || currentTimedEventPart == null) return;
            // Only update total segments if it's not the first event.
            // Then we can just add one in the end. Otherwise we're getting an extra segment
            // for the initial pause.
            TotalPBSegments++;
            if (prevTimedEventPart.EndTime <= CurrentPBSegmentStartTime || CurrentPBSegmentStartTime <= 0) return;
            var actionTime = prevTimedEventPart.EndTime - CurrentPBSegmentStartTime;
            TotalPBProcessTime += actionTime;
            PBProcessTimeList.Add(actionTime);
            CurrentPBSegmentStartTime = currentTimedEventPart.StartTime;
        }

        /// <summary>
        /// Returns the last event from the event list that has a timeEvent part 
        /// and adapting the CurrentPBProcessStartTime to the new position in the event list.
        /// </summary>
        /// <returns></returns>
        private Event GetCurrentLastTimedEvent()
        {
            Event currentEvent = null;
            TimedEventPart lastTimedEventPart = null;
            int i = InputEvents.Count - 1;
            while (lastTimedEventPart == null && i >= 1)
            {
                currentEvent = InputEvents[i];
                var previousEvent = InputEvents[i - 1];
                lastTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(currentEvent);
                var prevTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(previousEvent);
                if (prevTimedEventPart != null)
                {
                    if (lastTimedEventPart != null && lastTimedEventPart.EndTime == 0)
                    {
                        lastTimedEventPart = null;
                    }
                }
                i--;
            }
            return currentEvent;
        }
        #endregion

        /// <summary>
        /// Creates a summary from the gathered information.
        /// </summary>
        /// <returns>A Pause Analysis Summary.</returns>
        private SingularPauseAnalysisSummary CreateSummary()
        {
            var summary = new SingularPauseAnalysisSummary();

            var minMax = MathExt.GetMinMax(AllPauses);
            var pauseList = AllPauses.Select<ulong, double>(i => i).ToList();
            var totalPauseTime = (ulong)pauseList.Sum();
            var pausesCount = Convert.ToUInt64(pauseList.Count);
            var meanPauseTime = MathExt.MeanFromList(pauseList);
            var intervals = MathExt.CalculateInterval(AllLogPauses);
            var standardDeviation = MathExt.StDevFromList(pauseList);

            var quartiles = MathExt.GetBoxPlotValues(pauseList);
            var boxplotMin = quartiles.Item1;
            var median = quartiles.Item3;
            var firstQuartile = quartiles.Item2;
            var thirdQuartile = quartiles.Item4;
            var boxplotMax = quartiles.Item5;

            summary.GeneralInformation.AnalysisType = Type;
            summary.GeneralInformation.TotalProcessTime = TotalProcessTime;         
            summary.GeneralInformation.TotalPauseTime = totalPauseTime;
            summary.GeneralInformation.TotalWritingTime = TotalProcessTime > totalPauseTime ? TotalProcessTime - totalPauseTime : 0;            
            summary.GeneralInformation.PauseTimeProportion = (double)totalPauseTime / TotalProcessTime;
            summary.GeneralInformation.IntervalLength = IntervalSize;
            summary.GeneralInformation.NumberOfIntervals = NumberOfIntervals;
            summary.GeneralInformation.TotalNumberOfPauses = pausesCount;
            summary.GeneralInformation.MeanPauseTime = meanPauseTime;
            summary.GeneralInformation.CI95L = Math.Exp(intervals.Item1);
            summary.GeneralInformation.GeoMeanPauseTime = Math.Exp(intervals.Item2);
            summary.GeneralInformation.CI95H = Math.Exp(intervals.Item3);
            summary.GeneralInformation.CoefVar = intervals.Item4;
            summary.GeneralInformation.MinPause = minMax.First;
            summary.GeneralInformation.MaxPause = minMax.Second;
            summary.GeneralInformation.MedianPauseTime = median;
            summary.GeneralInformation.StDev = standardDeviation;

            var firstKeyboard = Event.SkipToFirstKeypress(InputEvents);
            if (firstKeyboard != null)
            {
                summary.GeneralInformation.FirstKeyPressId = firstKeyboard.First;
                summary.GeneralInformation.FirstStartKeyTime = firstKeyboard.Second - NewStartOffset;
            }

            // P-Burst statistics
            summary.GeneralInformation.PBurstTreshold = _currentPBurstThreshold;
            summary.GeneralInformation.NumberOfSegments = TotalPBSegments;
            summary.GeneralInformation.AvgProcessTime = PBProcessTimeList.Count == 0 ? 0 :
             TotalPBProcessTime / (ulong) PBProcessTimeList.Count;
            summary.GeneralInformation.StandardDeviation = MathExt.StDevFromList(PBProcessTimeList.Select<ulong, double>(i => i).ToList());
            var charSum = PBProcessCharList.Sum(Convert.ToDouble);
            summary.GeneralInformation.AvgProcessChars = PBProcessCharList.Count == 0 ? 0
                :  charSum / PBProcessCharList.Count;
            summary.GeneralInformation.StandardDeviationChars = MathExt.StDevFromList(PBProcessCharList.Select<ulong, double>(i => i).ToList());
            summary.GeneralInformation.MedianProcessTime = PBProcessTimeList.Count == 0 ? 0 :
                MathExt.GetMedian(PBProcessTimeList.Select<ulong, double>(i => i).ToList());
            summary.GeneralInformation.MedianProcessChars = PBProcessCharList.Count == 0 ? 0
                : MathExt.GetMedian(PBProcessCharList.Select<ulong, double>(i => i).ToList());
            summary.GeneralInformation.PBurstsPerMinute = (double) TotalPBSegments / TotalProcessTime * 60000;

            // 'BETWEEN - BEFORE' pause type info
            foreach (var entry in StatsBeforeType)
            {
                switch (entry.Key)
                {
                    case PauseLocation.BEFORE_WORDS:
                        summary.BetweenBeforeTypes[PauseLocation.BEFORE_WORDS] = GetBeforeSummary(entry.Key);
                        break;
                    case PauseLocation.BEFORE_SENTENCES:
                        summary.BetweenBeforeTypes[PauseLocation.BEFORE_SENTENCES] = GetBeforeSummary(entry.Key);
                        break;
                    case PauseLocation.BEFORE_PARAGRAPHS:
                        summary.BetweenBeforeTypes[PauseLocation.BEFORE_PARAGRAPHS] = GetBeforeSummary(entry.Key);
                        break;
                }
            }

            // 'BETWEEN - AFTER' pause type info
            foreach (var entry in StatsAfterType)
            {
                switch (entry.Key)
                {
                    case PauseLocation.AFTER_WORDS:
                        summary.BetweenAfterTypes[PauseLocation.AFTER_WORDS] = GetAfterSummary(entry.Key);
                        break;
                    case PauseLocation.AFTER_SENTENCES:
                        summary.BetweenAfterTypes[PauseLocation.AFTER_SENTENCES] = GetAfterSummary(entry.Key);
                        break;
                    case PauseLocation.AFTER_PARAGRAPHS:
                        summary.BetweenAfterTypes[PauseLocation.AFTER_PARAGRAPHS] = GetAfterSummary(entry.Key);
                        break;
                }
            }

            // The miscellaneous pauses
            foreach (var entry in StatsPerMiscellaneousType)
            {
                switch (entry.Key)
                {
                    case PauseLocation.INITIAL:
                        summary.MiscellaneousTypes[PauseLocation.INITIAL] = GetMiscSummary(entry.Key);
                        break;
                    case PauseLocation.END:
                        summary.MiscellaneousTypes[PauseLocation.END] = GetMiscSummary(entry.Key);
                        break;
                    case PauseLocation.COMBINATION_KEY:
                        summary.MiscellaneousTypes[PauseLocation.COMBINATION_KEY] = GetMiscSummary(entry.Key);
                        break;
                    case PauseLocation.REVISION:
                        summary.MiscellaneousTypes[PauseLocation.REVISION] = GetMiscSummary(entry.Key);
                        break;
                    case PauseLocation.CHANGE:
                        summary.MiscellaneousTypes[PauseLocation.CHANGE] = GetMiscSummary(entry.Key);
                        break;
                    case PauseLocation.UNKNOWN:
                        summary.MiscellaneousTypes[PauseLocation.UNKNOWN] = GetMiscSummary(entry.Key);
                        break;
                }
            }

            // Returns the Combined Pause Statistics
            foreach (var entry in StatsCombinedTypes)
            {
                minMax = MathExt.GetMinMax(CombinedPauseList[entry.Key]);
                pauseList = CombinedPauseList[entry.Key].Select<ulong, double>(i => i).ToList();
                pausesCount = Convert.ToUInt64(pauseList.Count);
                meanPauseTime = MathExt.MeanFromList(pauseList);
                intervals = MathExt.CalculateInterval(LogCombinedPauseList[entry.Key]);
                standardDeviation = MathExt.StDevFromList(pauseList);
                quartiles = MathExt.GetBoxPlotValues(pauseList);
                boxplotMin = quartiles.Item1;
                firstQuartile = quartiles.Item2;
                median = quartiles.Item3;
                thirdQuartile = quartiles.Item4;
                boxplotMax = quartiles.Item5;

                summary.CombinedTypes[entry.Key] = new SingularPauseAnalysisSummary.
                    CombinedStats(pausesCount, meanPauseTime, Math.Exp(intervals.Item2),
                    Math.Exp(intervals.Item1), Math.Exp(intervals.Item3), minMax.First,
                    minMax.Second, intervals.Item4, median, standardDeviation, firstQuartile, thirdQuartile,
                    boxplotMin, boxplotMax);
            }

            // Base types - only 'WITHIN' is reported
            foreach (var entry in StatsPerBaseType)
            {
                switch (entry.Key)
                {
                    case PauseLocation.WITHIN_WORDS:

                        minMax = MathExt.GetMinMax(PausesPerBaseType[entry.Key]);
                        pauseList = PausesPerBaseType[entry.Key].Select<ulong, double>(i => i).ToList();
                        pausesCount = Convert.ToUInt64(pauseList.Count);
                        meanPauseTime = MathExt.MeanFromList(pauseList);
                        intervals = MathExt.CalculateInterval(LogPausesPerBaseType[entry.Key]);
                        //median = MathExt.GetMedian(pauseList);
                        standardDeviation = MathExt.StDevFromList(pauseList);

                        quartiles = MathExt.GetBoxPlotValues(pauseList);
                        boxplotMin = quartiles.Item1;
                        firstQuartile = quartiles.Item2;
                        median = quartiles.Item3;
                        thirdQuartile = quartiles.Item4;
                        boxplotMax = quartiles.Item5;

                        summary.BasePauseTypes[entry.Key] = new SingularPauseAnalysisSummary.BaseStats(pausesCount,
                            meanPauseTime, Math.Exp(intervals.Item2), Math.Exp(intervals.Item1),
                            Math.Exp(intervals.Item3), minMax.First, minMax.Second, intervals.Item4,
                            median, standardDeviation, firstQuartile, thirdQuartile, boxplotMin, boxplotMax);
                        break;
                }
            }

            // Interval statistics
            foreach (var entry in StatsPerInterval)
            {
                minMax = MathExt.GetMinMax(PausesPerInterval[entry.Key]);
                pauseList = PausesPerInterval[entry.Key].Select<ulong, double>(i => i).ToList();
                pausesCount = Convert.ToUInt64(pauseList.Count);
                meanPauseTime = MathExt.MeanFromList(pauseList);
                intervals = MathExt.CalculateInterval(LogPausesPerInterval[entry.Key]);
                median = MathExt.GetMedian(pauseList);
                standardDeviation = MathExt.StDevFromList(pauseList);

                summary.IntervalInfo[entry.Key] = new SingularPauseAnalysisSummary.
                    IntervalStats("#" + (entry.Key + 1), entry.Value.IntervalStart, pausesCount,
                    meanPauseTime, Math.Exp(intervals.Item2), Math.Exp(intervals.Item1),
                    Math.Exp(intervals.Item3), minMax.First, minMax.Second, intervals.Item4,
                    median, standardDeviation);
            }
            return summary;
        }

        /// <summary>
        /// Returns the Between Before Pause Statistics
        /// </summary>
        /// <param name="key">The PauseLocation in view</param>
        /// <returns></returns>
        private SingularPauseAnalysisSummary.BetweenBeforeStats GetBeforeSummary(PauseLocation key)
        {
            var minMax = MathExt.GetMinMax(PausesPerBetweenType[key]);
            var pauseList = PausesPerBetweenType[key].Select<ulong, double>(i => i).ToList();
            var pausesCount = Convert.ToUInt64(pauseList.Count);
            var meanPauseTime = MathExt.MeanFromList(pauseList);
            var intervals = MathExt.CalculateInterval(LogPausesPerBetweenType[key]);
            var median = MathExt.GetMedian(pauseList);
            var standardDeviation = MathExt.StDevFromList(pauseList);

            return new SingularPauseAnalysisSummary.BetweenBeforeStats(pausesCount, meanPauseTime,
                Math.Exp(intervals.Item2), Math.Exp(intervals.Item1), Math.Exp(intervals.Item3),
                minMax.First, minMax.Second, intervals.Item4, median, standardDeviation);
        }

        /// <summary>
        /// Returns the Between After Pause Statistics
        /// </summary>
        /// <param name="key">The PauseLocation in view</param>
        /// <returns></returns>
        private SingularPauseAnalysisSummary.BetweenAfterStats GetAfterSummary(PauseLocation key)
        {
            var minMax = MathExt.GetMinMax(PausesPerBetweenType[key]);
            var pauseList = PausesPerBetweenType[key].Select<ulong, double>(i => i).ToList();
            var pausesCount = Convert.ToUInt64(pauseList.Count);
            var meanPauseTime = MathExt.MeanFromList(pauseList);
            var intervals = MathExt.CalculateInterval(LogPausesPerBetweenType[key]);
            var median = MathExt.GetMedian(pauseList);
            var standardDeviation = MathExt.StDevFromList(pauseList);

            return new SingularPauseAnalysisSummary.BetweenAfterStats(pausesCount, meanPauseTime,
                Math.Exp(intervals.Item2), Math.Exp(intervals.Item1), Math.Exp(intervals.Item3),
                minMax.First, minMax.Second, intervals.Item4, median, standardDeviation);
        }

        /// <summary>
        /// Returns the Miscellaneous Pause Statistics
        /// </summary>
        /// <param name="key">The PauseLocation in view</param>
        /// <returns></returns>
        private SingularPauseAnalysisSummary.MiscellaneousStats GetMiscSummary(PauseLocation key)
        {
            var minMax = MathExt.GetMinMax(PausesPerMiscellaneousType[key]);
            var pauseList = PausesPerMiscellaneousType[key].Select<ulong, double>(i => i).ToList();
            var pausesCount = Convert.ToUInt64(pauseList.Count);
            var meanPauseTime = MathExt.MeanFromList(pauseList);
            var intervals = MathExt.CalculateInterval(LogPausesPerMiscellaneousType[key]);
            var median = MathExt.GetMedian(pauseList);
            var stDev = MathExt.StDevFromList(pauseList);
            return new SingularPauseAnalysisSummary.MiscellaneousStats(pausesCount, meanPauseTime,
                Math.Exp(intervals.Item2), Math.Exp(intervals.Item1), Math.Exp(intervals.Item3),
                minMax.First, minMax.Second, intervals.Item4, median, stDev);
        }
    }
}