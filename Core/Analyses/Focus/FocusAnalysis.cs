using System;
using System.Collections.Generic;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;
using InputLog.Core.IO;
using log4net;
using EventTypeFilter = InputLog.Core.Preprocessing.Filter.EventTypeFilter;

namespace InputLog.Core.Analyses.Focus
{
    /// <summary>
    /// A Focus Analysis.
    /// The focus Analysis determines information about the user's actions within different windows (after focus changes)
    /// and about the interaction between the different windows. 
    /// IMPORTANT: any idfx contains also 'false' focus changes in the form of 'TASKBAR and 'Switch between tasks' 
    /// (translated according the local language settings of the user). These are 'false' because there is no real content involved. 
    /// A preprocessing tool 'RemoveTaskbar' should first filter out all 'TASKBAR and 'Switch between tasks' events by replacing them
    /// with the first real source that follows. The concerned real focus change is then removed. An idfx thus recoded has
    /// the mention 'REMOVED' at the end of its file name. These events could also be collected into an 'IGNORED' group in
    /// the Recode/Sources preprocessing step with the same effect.
    /// </summary>
    public class FocusAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// Event types we wish to do the analysis on (all other types are discarded).
        /// </summary>
        private static readonly string[] EventTypes = { EventType.KEYBOARD, EventType.FOCUS, EventType.MOUSE };

        /// <summary>
        /// Log4Net MessageLogger.
        /// </summary>
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Number of intervals in the given list of events.
        /// </summary>
        private readonly int _numberOfIntervals;

        /// <summary>
        /// Total process time (= actual total logging time that is being considered).
        /// </summary>
        private ulong _totalIntervalTime;

        /// <summary>
        /// Duration (time) of an interval.
        /// </summary>
        private ulong _intervalSize;

        /// <summary>
        /// Remembers the how-mannieth interval this is.
        /// </summary>
        private int IntervalNumber { get; set; }

        /// <summary>
        /// '_focusOffset' is the start time of the first timed event. 
        /// It is *not* the start time of the logging which is the NewStartOffset as calculated
        /// by the Analysis base class.
        /// </summary>
        private ulong _focusOffset;

        /// <summary>
        /// The main document of this session.
        /// </summary>
        private readonly string _mainDocument;

        /// <summary>
        /// Dictionaries mapping the different intervals to their statistics.
        /// </summary>
        private IDictionary<int, FocusAnalysisSummary.IntervalStats> _intervalObject;
        private IDictionary<string, IDictionary<string, int>> _intervalWindowTransitions;
        private Dictionary<string, FocusAnalysisSummary.WindowStatistics> _intervalFocusStats;
        private FocusAnalysisSummary.IntervalSummary _intervalTotals;

        #endregion

        /// <summary>
        /// Constructs a new FocusAnalysis.
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="fixedIntervals">The number of intervals dividing the focus events</param>
        public FocusAnalysis(List<Event> events, SessionIdentification sessionID, int fixedIntervals)
            : base("FA", events, sessionID)
        {
            InputEvents = events.Filter(new EventTypeFilter(EventTypes));
            _numberOfIntervals = fixedIntervals;
            _mainDocument = MainDocument;
        }

        /// <summary>
        /// Performs the actual Focus Analysis on the events with which this Analysis was constructed.
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            _intervalObject = new Dictionary<int, FocusAnalysisSummary.IntervalStats>();
            var summary = new FocusAnalysisSummary();
            DetermineWindowStatisticsAndTransitions(summary);
            return summary;
        }

        /// <summary>
        /// Determines event statistics for each window. 
        /// These statistics include:
        /// - Total elapsed time
        /// - Relative elapsed time
        /// - Total key presses
        /// - Relative key presses
        /// </summary>
        /// <param name="summary">Summary in which to store the statistics.</param>
        private void DetermineWindowStatisticsAndTransitions(FocusAnalysisSummary summary)
        {
            FocusAnalysisSummary.WindowStatistics currentWindowStats = null;
            FocusAnalysisSummary.WindowStatistics focusStats = null;
            ulong totalKeypressEvents = 0;
            string prevWindowTitle = null;
            bool firstFocus = true;
            bool skipOnce = false;
            ulong timeBeforeFirstWindow = 0;
            ulong delta = 0ul;
            int currentInterval = 0;
            var currentWindowTitle = string.Empty;
            var emptyIntervalList = new List<Pair<int, Pair<string, int>>>();

            // 'focusOffset' is the start time of the first timed event.
            _focusOffset = Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]).StartTime;
            ulong currentFocusStartTime = _focusOffset;
            // The start time of the current focus in an interval.
            // The first start time is the _focusOffset.
            ulong currentIntervalFocusStartTime = _focusOffset;

            // We don't include the time before the first timed event to define the total process time.
            // The last event with a TimedEventPart marks the end of the analysis.
            var endTimedEventPart = GetLastTimedEvent();
            // Adding one ms to avoid the edge case where the very last event starts a new interval.
            _totalIntervalTime = endTimedEventPart.EndTime + 1 - _focusOffset;
            _intervalSize = (ulong)Math.Ceiling((decimal)_totalIntervalTime / _numberOfIntervals);

            // Initializing the Interval stats
            for (int i = 0; i < _numberOfIntervals; i++)
            {
                _intervalFocusStats = new Dictionary<string, FocusAnalysisSummary.WindowStatistics>();
                _intervalTotals = new FocusAnalysisSummary.IntervalSummary();
                _intervalWindowTransitions = new Dictionary<string, IDictionary<string, int>>();
                _intervalObject[i] = new FocusAnalysisSummary.IntervalStats(i + 1, _intervalSize * (ulong)i,
                    _intervalFocusStats, _intervalTotals, _intervalWindowTransitions);
            }

            // Iterates over all events counting the number of keypresses, the time spent in a window and the switching between windows.
            // Window switches occur on focus events. A new interval occurs when the time allocated to that interval has passed.
            foreach (Event even in InputEvents)
            {
                try
                {
                    // New interval statistics are created at the change of the interval number.
                    int newInterval = GetIntervalNumber(even);
                    if (newInterval > currentInterval)
                    {
                        // Changing the current window title if there is one at the interval transition.
                        string thisWindowTitle;
                        if (!even.GetWindowTitle(even).Equals(string.Empty))
                        {
                            thisWindowTitle = even.GetWindowTitle(even);
                            if (currentWindowTitle.Equals(string.Empty))
                            {
                                currentWindowTitle = thisWindowTitle;
                            }
                        }
                        else thisWindowTitle = currentWindowTitle;

                        // If the current window title equals the previous title at an interval transition, 
                        // we first finalize the total time of that focus, before opening a new one with the same window title.
                        // This new window contains the data overflow, i.e. the part of the window in focus that was cut by
                        // the interval boundary.
                        if (thisWindowTitle.Equals(prevWindowTitle))
                        {
                            focusStats.TotalTime += _intervalObject[currentInterval].IntervalStart + _intervalSize
                                                    - (currentIntervalFocusStartTime - _focusOffset);  // _intervalTotals.TotalTime;
                            // The interval boundary is the start time for the new focus statistics containing the rest of
                            // the window in focus.
                            currentIntervalFocusStartTime = _intervalObject[currentInterval].IntervalStart
                                                            + _intervalSize + _focusOffset;
                        }

                        var previousInterval = currentInterval;
                        currentInterval = newInterval;

                        // Looking for empty intervals prior to the current segment.
                        // Collecting three things: as key the segment number following after the one or more empty interval;
                        // as value: the name of the current window and the number of preceding empty intervals.
                        int emptyIntervals = currentInterval - previousInterval - 1;
                        if (emptyIntervals >= 1)
                        {
                            var emptyWindows = new Pair<string, int> { First = thisWindowTitle, Second = emptyIntervals };
                            var intervl = new Pair<int, Pair<string, int>> { First = currentInterval + 1, Second = emptyWindows };
                            emptyIntervalList.Add(intervl);

                            for (int i = 1; i <= emptyIntervals; i++)
                            {
                                focusStats = new FocusAnalysisSummary.WindowStatistics();
                                _intervalObject[previousInterval + i].IntervalFocusStats[thisWindowTitle] = focusStats;
                                // The segment numbering starts at 1.                             
                                focusStats.Index = _intervalObject[previousInterval + i].IntervalFocusStats.Count;
                                focusStats.TotalTime = _intervalSize;
                            }
                        }

                        // Total time in the current interval
                        var totalInterval = 0ul;
                        FocusAnalysisSummary.IntervalStats interval = _intervalObject[previousInterval];
                        foreach (KeyValuePair<string, FocusAnalysisSummary.WindowStatistics> stats in interval.IntervalFocusStats)
                        {
                            totalInterval += stats.Value.TotalTime;
                        }

                        // New focus statistics and a new intervalObject
                        // Fixing the time of the open currentInterval before starting a new one.
                        // To prevent counting this time twice, we skip it later.
                        FocusChange focusEventPart = Event.GetFirstEventPart<FocusChange>(even);
                        if (focusEventPart != null && focusStats.TotalTime == 0)
                        {
                            var thisFocusTime = focusEventPart.StartTime - currentIntervalFocusStartTime;
                            focusStats.TotalTime += focusEventPart.StartTime - currentIntervalFocusStartTime;

                            // If this time is added to the interval we may overshoot the interval threshold.
                            // When that is the case, 'delta' is used to create a new focus in the next interval.
                            if (thisFocusTime + totalInterval > _intervalSize)
                            {
                                focusStats.TotalTime = thisFocusTime - (thisFocusTime + totalInterval - _intervalSize);
                                delta = thisFocusTime - focusStats.TotalTime;
                            }
                            skipOnce = true;
                        }

                        focusStats = new FocusAnalysisSummary.WindowStatistics();
                        _intervalObject[currentInterval].IntervalFocusStats[thisWindowTitle] = focusStats;
                        focusStats.Index = _intervalObject[currentInterval].IntervalFocusStats.Count;
                    }

                    // New focus event, change current statistics
                    if (even.Type.Equals(EventType.FOCUS))
                    {
                        FocusChange focusEventPart = Event.GetFirstEventPart<FocusChange>(even);
                        if (focusEventPart == null) continue;

                        // Create a bogus statistics to report the time before the first real focus event.
                        if (firstFocus)
                        {
                            currentWindowStats = new FocusAnalysisSummary.WindowStatistics();
                            summary.WindowStats["* Time before the first window *"] = currentWindowStats;
                            currentWindowStats.Index = summary.WindowStats.Count;
                            // When 'before the first window', we want to know the time before processing really started.
                            currentWindowStats.TotalTime = focusEventPart.StartTime - NewStartOffset;
                            timeBeforeFirstWindow = currentWindowStats.TotalTime;

                            firstFocus = false;
                        }

                        // Updating time inside a focus
                        if (currentWindowStats != null)
                        {
                            if (focusEventPart.StartTime == 0 || currentFocusStartTime > focusEventPart.StartTime)
                            {
                                if (currentWindowStats.TotalTime > 0) continue;
                                currentWindowStats.TotalTime = 0;
                            }
                            else
                            {
                                currentWindowStats.TotalTime += focusEventPart.StartTime - currentFocusStartTime;
                            }
                        }

                        // Updating the time inside a focus of the current interval at a focus change.
                        // The focusEventPart.StartTime is the start of the next focus window to be created,
                        // it is taken as the end of the current focus.
                        if (focusStats != null)
                        {
                            if (focusEventPart.StartTime == 0 || currentIntervalFocusStartTime > focusEventPart.StartTime)
                            {
                                if (focusStats.TotalTime > 0) continue;
                                focusStats.TotalTime = 0;
                            }
                            else
                            {
                                // This time was added already at the change of an interval.
                                if (skipOnce)
                                {
                                    // If there is a leftover we put it in an new intervalStat
                                    if (delta > 0)
                                    {
                                        focusStats = new FocusAnalysisSummary.WindowStatistics();
                                        _intervalObject[currentInterval].IntervalFocusStats[currentWindowTitle] = focusStats;
                                        focusStats.Index = _intervalObject[currentInterval].IntervalFocusStats.Count;
                                        focusStats.TotalTime += delta;
                                    }
                                    skipOnce = false;
                                }
                                else
                                {
                                    focusStats.TotalTime += focusEventPart.StartTime - currentIntervalFocusStartTime;
                                }
                            }
                        }

                        currentWindowTitle = focusEventPart.WindowTitle;
                        currentFocusStartTime = focusEventPart.StartTime;
                        currentIntervalFocusStartTime = focusEventPart.StartTime;

                        // A new windowTitle triggers a new general WindowStats.
                        if (!summary.WindowStats.TryGetValue(currentWindowTitle, out currentWindowStats))
                        {
                            currentWindowStats = new FocusAnalysisSummary.WindowStatistics();
                            summary.WindowStats[currentWindowTitle] = currentWindowStats;
                            currentWindowStats.Index = summary.WindowStats.Count;
                        }

                        // A new windowTitle triggers a new intervalStats in the current intervalObject. 
                        if (!_intervalObject[currentInterval].IntervalFocusStats.TryGetValue(currentWindowTitle,
                            out focusStats))
                        {
                            focusStats = new FocusAnalysisSummary.WindowStatistics();
                            _intervalObject[currentInterval].IntervalFocusStats[currentWindowTitle] = focusStats;
                            focusStats.Index = _intervalObject[currentInterval].IntervalFocusStats.Count;
                        }

                        // Defining the main document
                        if (currentWindowTitle.ToLower().Contains(_mainDocument.ToLower()))
                        {
                            summary.MainDoc = currentWindowTitle;
                        }

                        // Updating the window transitions.
                        if (prevWindowTitle != null)
                        {
                            // Not the first window.
                            if (!summary.WindowTransitionCounts.TryGetValue(prevWindowTitle, out var transitions))
                            {
                                transitions = new Dictionary<string, int>();
                                summary.WindowTransitionCounts[prevWindowTitle] = transitions;
                            }

                            if (!transitions.ContainsKey(currentWindowTitle))
                            {
                                transitions[currentWindowTitle] = 1;
                            }
                            else
                            {
                                transitions[currentWindowTitle]++;
                            }

                            summary.TotalWindowTransitions++;

                            // Updating window transitions inside an interval.
                            if (!_intervalObject[currentInterval].IntervalWindowTransitions.TryGetValue(prevWindowTitle, 
                                out var intervalTransitions))
                            {
                                intervalTransitions = new Dictionary<string, int>();
                                _intervalObject[currentInterval].IntervalWindowTransitions[prevWindowTitle] = intervalTransitions;
                            }

                            if (!intervalTransitions.ContainsKey(currentWindowTitle))
                            {
                                intervalTransitions[currentWindowTitle] = 1;
                            }
                            else
                            {
                                intervalTransitions[currentWindowTitle]++;
                            }
                        }

                        prevWindowTitle = currentWindowTitle;
                    }
                    // Not a focusEvent
                    else
                    {
                        // Updating the existing statistics.
                        if (currentWindowStats != null)
                        {
                            if (EventType.KEYBOARD.Equals(even.Type))
                            {
                                currentWindowStats.TotalKeyPresses++;
                                totalKeypressEvents++;
                            }
                        }

                        if (focusStats == null) continue;
                        if (EventType.KEYBOARD.Equals(even.Type))
                        {
                            focusStats.TotalKeyPresses++;
                        }
                    }
                }
                catch (AnalysisException e)
                {
                    Log.Warn(e); // Exception while analyzing an event, log it and skip it...
                }
            }

            // Going backwards to find the last event with a TimedEventPart.
            TimedEventPart timedEventPart = null;
            var j = InputEvents.Count - 1;
            while (j >= 0)
            {
                timedEventPart = Event.GetFirstEventPart<TimedEventPart>(InputEvents[j]);
                if (timedEventPart != null) break;
                j--;
            }

            if (timedEventPart == null || currentWindowStats == null) return;
            // Ensuring that the window stats is not zero (this can happen when a filter is used in preprocessing).
            var endTime = timedEventPart.EndTime > 0 ? timedEventPart.EndTime : timedEventPart.StartTime;

            // Fixing the total time for the window that gained the last focus.
            currentWindowStats.TotalTime += endTime - currentFocusStartTime;
            focusStats.TotalTime += endTime - currentFocusStartTime;

            // Updating statistics to include relative stats.
            var totalTime = endTime - _focusOffset + timeBeforeFirstWindow;

            foreach (var stats in summary.WindowStats.Values) //.Values)
            {
                if (totalTime > 0)
                {
                    stats.TotalTimeRelative = stats.TotalTime / (double)totalTime;
                }
                summary.RelativeTimeTotal += stats.TotalTimeRelative;

                if (totalKeypressEvents > 0)
                {
                    stats.TotalKeyPressesRelative = stats.TotalKeyPresses / (double)totalKeypressEvents;
                }
                summary.RelativeKeypressTotal += stats.TotalKeyPressesRelative;
            }

            // Interval statistics.
            foreach (KeyValuePair<int, FocusAnalysisSummary.IntervalStats> interval in _intervalObject)
            {
                _intervalTotals = interval.Value.IntervalTotals;
                var segment = interval.Value.IntervalSegment;
                var intervalStats = interval.Value.IntervalFocusStats;

                // If a segment follows after one or more empty intervals, as reported in the emptyIntervalList,
                // the total time of a currentWindow in this segment is reduced to compensate for the time in
                // the preceding empty interval(s), where an empty focus with the same windowTitle was appended
                // as a placeholder for the idle time in that segment.
                // The time correction is always a multiple (1 or more, depending on the number of empty intervals)
                // of the intervalSize.
                foreach (var pair in emptyIntervalList)
                {
                    if (pair.First.Equals(segment))
                    {
                        var emptyWindow = pair.Second;
                        string windowTitle = emptyWindow.First;
                        var nmbr = (ulong)emptyWindow.Second;
                        foreach (var stats in intervalStats)
                        {
                            if (stats.Key.Equals(windowTitle))
                            {
                                stats.Value.TotalTime -= nmbr * _intervalSize;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, FocusAnalysisSummary.WindowStatistics> stats in
                    interval.Value.IntervalFocusStats)
                {
                    _intervalTotals.TotalTime += stats.Value.TotalTime;
                    _intervalTotals.TotalKeyPresses += stats.Value.TotalKeyPresses;
                }

                foreach (KeyValuePair<string, FocusAnalysisSummary.WindowStatistics> stats in
                    interval.Value.IntervalFocusStats)
                {
                    if (_intervalTotals.TotalTime > 0)
                    {
                        stats.Value.TotalTimeRelative = stats.Value.TotalTime / (double)_intervalTotals.TotalTime;
                    }

                    _intervalTotals.RelativeTimeTotal += stats.Value.TotalTimeRelative;

                    if (_intervalTotals.TotalKeyPresses > 0)
                    {
                        stats.Value.TotalKeyPressesRelative = stats.Value.TotalKeyPresses /
                                                              (double)_intervalTotals.TotalKeyPresses;
                    }
                    _intervalTotals.RelativeKeypressTotal += stats.Value.TotalKeyPressesRelative;
                }

                foreach (IDictionary<string, int> pair in interval.Value.IntervalWindowTransitions.Values)
                {
                    foreach (KeyValuePair<string, int> p in pair)
                    {
                        _intervalTotals.TotalWindowTransitions += (ulong)p.Value;
                    }
                }

                summary.IntervalInfo[interval.Key] = new FocusAnalysisSummary.IntervalStats(interval.Value.IntervalSegment,
                        interval.Value.IntervalStart, interval.Value.IntervalFocusStats, interval.Value.IntervalTotals,
                        interval.Value.IntervalWindowTransitions);
            }

            summary.TotalTime = totalTime;
            summary.TotalKeyPresses = totalKeypressEvents;
        }

        /// <summary>
        /// Returns the interval number for this event.
        /// </summary>
        /// <param name="even">an Event</param>
        /// <returns>The interval number for this event</returns>
        private int GetIntervalNumber(Event even)
        {
            if (null == Event.GetFirstEventPart<TimedEventPart>(even)) return IntervalNumber;
            ulong startTime = Event.GetFirstEventPart<TimedEventPart>(even).StartTime;
            if (_focusOffset > startTime)
            {
                return IntervalNumber;
            }
            var interval = (startTime - _focusOffset) / ((double)_intervalSize);
            return IntervalNumber = (int)Math.Floor(interval);
        }
    }
}