using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Focus
{
    /// <summary>
    ///     Summary of the Focus analysis.
    /// </summary>
    public class FocusAnalysisSummary : AbstractAnalysisSummary
    {
        /// <summary>
        ///     Dictionary that maps a window title to a dictionary that maps other window titles to counts.
        ///     In essence, this dictionary stores the counts of transitions between windows.
        ///     For example:
        ///     title A => title B => 10
        ///     title C => 2
        ///     title B => title A => 3
        ///     title C => 7
        ///     Note that the order of mapping is important, as title A => title B represents a switch form window A to window B
        ///     and title B => title A represents the reverse switch.
        /// </summary>
        public readonly IDictionary<string, IDictionary<string, int>> WindowTransitionCounts;

        /// <summary>
        /// The main document used for logging this session.
        /// </summary>
        public string MainDoc { private get; set; }

        /// <summary>
        ///     The total amount of key presses.
        /// </summary>
        public ulong TotalKeyPresses;

        /// <summary>
        ///     The total amount of logging time.
        /// </summary>
        public ulong TotalTime;

        /// <summary>
        ///     The total number of transitions/switches between windows.
        /// </summary>
        public ulong TotalWindowTransitions;

        /// <summary>
        ///     Dictionary of statistics of the events happening in a specific window.
        ///     Maps the window title the statistics of the window.
        /// </summary>
        public IDictionary<string, WindowStatistics> WindowStats { get; }

        public double RelativeTimeTotal;
        public double RelativeKeypressTotal;

        /// <summary>
        ///     Dictionary mapping the different intervals to their window-in-focus and transition statistics.
        /// </summary>
        public IDictionary<int, IntervalStats> IntervalInfo { get; }

        /// <summary>
        ///     Visual representation of the focus analysis.
        /// </summary>
        public Bitmap VisualRepresentation;


        /// <summary>
        ///     Constructs a new FocusAnalysis Summary.
        /// </summary>
        public FocusAnalysisSummary()
        {
            WindowStats = new Dictionary<string, WindowStatistics>();
            WindowTransitionCounts = new Dictionary<string, IDictionary<string, int>>();
            IntervalInfo = new Dictionary<int, IntervalStats>();
            TotalWindowTransitions = 0;
            TotalTime = 0;
            TotalKeyPresses = 0;
            MainDoc = string.Empty;
        }

        /// <summary>
        ///     The number of transitions per minute
        /// </summary>
        private double TransitionsPerMinute
        {
            get
            {
                var timeInMinutes = TotalTime/60000.0;
                var tpm = TotalWindowTransitions/timeInMinutes;
                return tpm;
            }
        }

        /// <summary>
        ///     Mean transition length in seconds
        /// </summary>
        private double MeanTransitionLength
        {
            get
            {
                if (TotalWindowTransitions == 0)
                    return double.NaN;
                return TotalTime/1000.0/TotalWindowTransitions;
            }
        }

        /// <summary>
        ///     The relative time spent in the main document.
        /// </summary>
        private double RelativeTimeInMainDoc
        {
            get
            {
                var ratio = 0.0;
                try
                {
                    WindowStatistics stats;
                    if (WindowStats.TryGetValue(MainDoc, out stats))
                    {
                        ratio = stats.TotalTimeRelative;
                    }
                }
                catch (KeyNotFoundException k)
                {
                    throw new KeyNotFoundException("MainDoc not found" + k.StackTrace);
                }
                return ratio;
            }
        }

        /// <summary>
        ///     The relative time spent in the sources (not maindoc).
        /// </summary>
        private double RelativeTimeInSources
        {
            get
            {
                var ratio = 0.0;
                try
                {
                    WindowStatistics stats;
                    if (WindowStats.TryGetValue(MainDoc, out stats))
                    {
                        ratio = 1 - stats.TotalTimeRelative;
                    }
                }
                catch (KeyNotFoundException k)
                {
                    throw new KeyNotFoundException("Data not found" + k.StackTrace);
                }
                return ratio;
            }
        }

        /// <summary>
        /// Counting the number of transitions from the main document to other sources and vice versa,
        /// with the average number of transitions per minute per interval and the total over the intervals.
        /// The number of intervals is expected to be 3.
        /// </summary>
        private List<Pair<int, double>> SwitchesBetweenMainAndSources
        {
            get
            {
                var timeInMinutes = TotalTime / 60000.0;
                var intervalTime = timeInMinutes / 3;
                var totalCount = 0;
                List<Pair<int,double>> switchCount = new List<Pair<int, double>>();
                try
                {
                    if (IntervalInfo.Count > 3)
                    {
                        MessageBox.Show("Variable: 'SwitchesBetweenMainAndSources'\nThe number of intervals used (" + IntervalInfo.Count
                            + ") is larger than expected by the report (3).\nThe program will continue but the result may be wrong.",
                                "Number of Intervals in the Source Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    foreach (var stat in IntervalInfo)
                    {
                        int count = 0;
                        foreach (var transitions in stat.Value.IntervalWindowTransitions)
                        {
                            if (transitions.Key.Equals(MainDoc))
                            {
                                foreach (var nextTransitions in transitions.Value)
                                {
                                     count += nextTransitions.Value;
                                }
                            }
                            else
                            {
                                foreach (var nextTransitions in transitions.Value)
                                {
                                    if (nextTransitions.Key.Equals(MainDoc))
                                    {
                                        count += nextTransitions.Value;
                                    }
                                }
                            }
                        }

                        var switchTime = new Pair<int, double>
                        {
                            First = count,
                            Second = count / intervalTime
                        };
                        switchCount.Add(switchTime);
                        totalCount += count;
                    }
                    var totalSwitchTime = new Pair<int, double>
                    {
                        First = totalCount,
                        Second = totalCount / timeInMinutes
                    };
                    switchCount.Add(totalSwitchTime);
                }
                catch (Exception e)
                {
                    throw new AnalysisException("Exception in 'SwitchesBetweenMainAndSources' " + e.StackTrace);
                }
                return switchCount;
            }
        }

        /// <summary>
        ///  Counting the number of transitions between all sources and the time per switch.
        /// The number of intervals is expected to be 3.
        /// </summary>
        private List<Pair<int, double>> SwitchesBetweenSources
        {
            get
            {
                var timeInMinutes = TotalTime/60000.0;
                var intervalTime = timeInMinutes/3;
                var totalCount = 0;
                List<Pair<int, double>> switchCount = new List<Pair<int, double>>();
                try
                {
                    if (IntervalInfo.Count > 3)
                    {
                        MessageBox.Show("Variable: 'SwitchesBetweenSources'\nThe number of intervals used (" + IntervalInfo.Count 
                            + ") is larger than expected by the report (3).\nThe program will continue but the result may be wrong.",
                                "Number of Intervals in the Source Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    foreach (var stat in IntervalInfo)
                    {
                        int count = 0;
                        foreach (var transitions in stat.Value.IntervalWindowTransitions)
                        {
                            count += transitions.Value.Sum(nextTransitions => nextTransitions.Value);
                        }
                        var switchTime = new Pair<int, double>
                        {
                            First = count,
                            Second = count / intervalTime
                        };
                        switchCount.Add(switchTime);
                        totalCount += count;
                    }
                    var totalSwitchTime = new Pair<int, double>
                    {
                        First = totalCount,
                        Second = totalCount / timeInMinutes
                    };
                    switchCount.Add(totalSwitchTime);
                }
                catch (Exception e)
                {
                    throw new AnalysisException("Exception in 'SwitchesBetweenMSources' " + e.StackTrace);
                }
                return switchCount;
            }
        }

        /// <summary>
        /// Time per interval spent in sources, not in the main document, by subtracting the 
        /// time in the main document from the total time in the interval.
        /// The number of intervals is expected to be 3.
        /// </summary>
        private List<double> TimeInSources
        {
            get
            {
                double totalCount = 0;
                double total = 0;
                List<double> timeCount = new List<double>();
                List<double> timePerct = new List<double>();
                try
                {
                    if (IntervalInfo.Count > 3)
                    {
                        MessageBox.Show("Variable: 'TimeInSources'\nThe number of intervals used (" + IntervalInfo.Count 
                            + ") is larger than expected by the report (3).\nThe program will continue but the result may be wrong.",
                                "Number of Intervals in the Source Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    foreach (var stats in IntervalInfo)
                    {
                        var totalTime = stats.Value.IntervalTotals.TotalTime;
                        total += totalTime;
                        var maindocTime = 0ul;
                        foreach (var doc in stats.Value.IntervalFocusStats)
                        {
                            if (doc.Key.Equals(MainDoc))
                            {
                                maindocTime += doc.Value.TotalTime;
                            }
                        }
                        timeCount.Add(totalTime - maindocTime);
                        totalCount += totalTime - maindocTime;
                    }
                    timePerct.AddRange(timeCount.Select(time => time / totalCount));
                    timePerct.Add(totalCount / total);
                }
                catch (Exception e)
                {
                    throw new AnalysisException("Exception in 'TimeInSources' " + e.StackTrace);
                }
                return timePerct;
            }
        } 

        /// <summary>
        ///     Class representing some statistics of the events that occurred within a window.
        /// </summary>
        public class WindowStatistics
        {
            /// <summary>
            ///     The index of this stats entry.
            /// </summary>
            public int Index;

            /// <summary>
            ///     The total amount of keypresses a user made within a specific window.
            /// </summary>
            public ulong TotalKeyPresses;

            /// <summary>
            ///     The relative amount of keypresses a user made within a specific window.
            /// </summary>
            public double TotalKeyPressesRelative;

            /// <summary>
            ///     The total amount of time the user spent in a specific window.
            /// </summary>
            public ulong TotalTime;

            /// <summary>
            ///     The relative amount of time the user spent in a specific window.
            /// </summary>
            public double TotalTimeRelative;
        }

        /// <summary>
        /// Totals per interval
        /// </summary>
        public class IntervalSummary
        {
            /// <summary>
            ///     The total amount of key presses.
            /// </summary>
            public ulong TotalKeyPresses;
            /// <summary>
            ///     The total amount of logging time.
            /// </summary>
            public ulong TotalTime;
            /// <summary>
            ///     The total number of transitions/switches between windows.
            /// </summary>
            public ulong TotalWindowTransitions;
            /// <summary>
            /// 
            /// </summary>
            public double RelativeTimeTotal;
            /// <summary>
            /// 
            /// </summary>
            public double RelativeKeypressTotal;
        }


        /// <summary>
        ///  Per interval, the time spent in a window-in-focus and the number of keystrokes. 
        /// </summary>
        public class IntervalStats : IEnumerable
        {
            // Start time of an interval
            public readonly ulong IntervalStart;
            // Interval sequence number
            public readonly int IntervalSegment;
            public readonly IDictionary<string, WindowStatistics> IntervalFocusStats;
            public readonly IntervalSummary IntervalTotals;
            public readonly IDictionary<string, IDictionary<string, int>> IntervalWindowTransitions;

            /// <summary>
            /// Window-in-focus and transition statistics per interval
            /// </summary>
            /// <param name="intervalSegment"></param>
            /// <param name="intervalStart"></param>
            /// <param name="focusStats">For every window (string) an instance of WindowStatistics</param>
            /// <param name="intervalTotals">The totals of the window statistics per interval</param>
            /// <param name="intervalWindowTransitions">>First key is the 'from' window, in the second dict key is the 'to' window, int is the transition count</param>
            public IntervalStats(int intervalSegment, ulong intervalStart, IDictionary<string, WindowStatistics> focusStats,
                IntervalSummary intervalTotals, IDictionary<string, IDictionary<string, int>> intervalWindowTransitions)
            {
                IntervalSegment = intervalSegment;
                IntervalStart = intervalStart;
                IntervalFocusStats = focusStats;
                IntervalTotals = intervalTotals;
                IntervalWindowTransitions = intervalWindowTransitions;
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
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

        private readonly CultureInfo _culture = new CultureInfo("en-US");

        public ReportValue report_NumberOfSources()
        {
            const string RESOURCE_ID = "Focus_Nr";
            return new LabeledValue(
                RESOURCE_ID,
                WindowTransitionCounts.Count.ToString()
            );
        }

        public ReportValue report_TransitionsPerMinute()
        {
            const string RESOURCE_ID = "Focus_TransitionsPerMinute";
            return new LabeledValue(
                RESOURCE_ID,
                TransitionsPerMinute.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionLength_Avg()
        {
            const string RESOURCE_ID = "Focus_TransitionLength_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                MeanTransitionLength.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_RelativeTime_Sources()
        {
            const string RESOURCE_ID = "Focus_RelativeTime_Sources";
            return new LabeledValue(
                RESOURCE_ID,
                RelativeTimeInSources.ToString("P", _culture.NumberFormat)
            );
        }

        public ReportValue report_RelativeTime_MainDoc()
        {
            const string RESOURCE_ID = "Focus_RelativeTime_MainDoc";
            return new LabeledValue(
                RESOURCE_ID,
                RelativeTimeInMainDoc.ToString("P", _culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionMain_Sources_Int1()
        {
            if (_mainDocTransitions == null)
            {
                _mainDocTransitions = SwitchesBetweenMainAndSources;
            }
            const string RESOURCE_ID = "TransitionMain_Sources_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                _mainDocTransitions[0].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionMain_Sources_Int2()
        {
            if (_mainDocTransitions == null)
            {
                _mainDocTransitions = SwitchesBetweenMainAndSources;
            }
            const string RESOURCE_ID = "TransitionMain_Sources_Int2";
            return new LabeledValue(
                RESOURCE_ID,
                _mainDocTransitions[1].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionMain_Sources_Int3()
        {
            if (_mainDocTransitions == null)
            {
                _mainDocTransitions = SwitchesBetweenMainAndSources;
            }
            const string RESOURCE_ID = "TransitionMain_Sources_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                _mainDocTransitions[2].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionMain_Sources_Tot()
        {
            if (_mainDocTransitions == null)
            {
                _mainDocTransitions = SwitchesBetweenMainAndSources;
            }
            const string RESOURCE_ID = "TransitionMain_Sources_Tot";
            return new LabeledValue(
                RESOURCE_ID,
                _mainDocTransitions[3].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Transition_Sources_Int1()
        {
            if (_sourceTransitions == null)
            {
                _sourceTransitions = SwitchesBetweenSources;
            }
            const string RESOURCE_ID = "Transition_Sources_Int1";
            return new LabeledValue(
                RESOURCE_ID,
                _sourceTransitions[0].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Transition_Sources_Int2()
        {
            if (_sourceTransitions == null)
            {
                _sourceTransitions = SwitchesBetweenSources;
            }
            const string RESOURCE_ID = "Transition_Sources_Int2";
            return new LabeledValue(
                RESOURCE_ID,
               _sourceTransitions[1].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Transition_Sources_Int3()
        {
            if (_sourceTransitions == null)
            {
                _sourceTransitions = SwitchesBetweenSources;
            }
            const string RESOURCE_ID = "Transition_Sources_Int3";
            return new LabeledValue(
                RESOURCE_ID,
                _sourceTransitions[2].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Transition_Sources_Tot()
        {
            if (_sourceTransitions == null)
            {
                _sourceTransitions = SwitchesBetweenSources;
            }
            const string RESOURCE_ID = "Transition_Sources_Tot";
            return new LabeledValue(
                RESOURCE_ID,
               _sourceTransitions[3].Second.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_TimeInSources_Int1()
        {
            if (_timeInSources == null)
            {
                _timeInSources = TimeInSources;
            }
            const string RESOURCE_ID = "TimeInSources_Int1";
            return new LabeledValue(
                RESOURCE_ID,
               _timeInSources[0].ToString("P", _culture.NumberFormat)
            );
        }

        public ReportValue report_TimeInSources_Int2()
        {
            if (_timeInSources == null)
            {
                _timeInSources = TimeInSources;
            }
            const string RESOURCE_ID = "TimeInSources_Int2";
            return new LabeledValue(
                RESOURCE_ID,
               _timeInSources[1].ToString("P", _culture.NumberFormat)
            );
        }

        public ReportValue report_TimeInSources_Int3()
        {
            if (_timeInSources == null)
            {
                _timeInSources = TimeInSources;
            }
            const string RESOURCE_ID = "TimeInSources_Int3";
            return new LabeledValue(
                RESOURCE_ID,
               _timeInSources[2].ToString("P", _culture.NumberFormat)
            );
        }

        public ReportValue report_TimeInSources_Tot()
        {
            if (_timeInSources == null)
            {
                _timeInSources = TimeInSources;
            }
            const string RESOURCE_ID = "TimeInSources_Tot";
            return new LabeledValue(
                RESOURCE_ID,
                _timeInSources[3].ToString("P", _culture.NumberFormat)
            );
        }

        /// <summary>
        /// Containers with the results of a transition count.
        /// </summary>
        private List<Pair<int, double>> _mainDocTransitions;
        private List<Pair<int, double>> _sourceTransitions;
        private List<double> _timeInSources;

        #endregion
    }
}