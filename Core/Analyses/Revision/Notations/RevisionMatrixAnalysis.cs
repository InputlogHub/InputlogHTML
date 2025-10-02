using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Summary;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class RevisionMatrixAnalysis : Analysis
    {
        /// <summary>
        ///     Revision Matrix Analysis Constructor
        /// </summary>
        /// <param name="events">The list of events to analyze</param>
        /// <param name="sessionID"></param>
       /// <param name="hasReport">Boolean whether this analysis is used in a report (true) or as a stand alone analysis (false).</param>
        /// <param name="docpath">
        ///     Path to the original document, if no path is specificed or the path
        ///     does not exist, an empty docuemnt will be used.
        /// </param>
        /// <param name="pauseTreshold"> The used pause treshold</param>
        public RevisionMatrixAnalysis(List<Event> events, SessionIdentification sessionID, string docpath, bool hasReport, ulong pauseTreshold = 0ul) :
            base("RM", events, sessionID)
        {
            InputEvents = events;
            _revSummary = (RevisionAnalysisSummary) new RevisionAnalysis.RevisionAnalysis(events, sessionID,
                docpath, pauseTreshold).DoAnalysis();

            // We need the total process time as calculated by the SummaryAnalysis.
            _sumAnalysis = new SummaryAnalysis(InputEvents, sessionID);
        }


        /// <summary>
        ///     Creates a Revision Matrix analysis of the given events etc.
        /// </summary>
        /// <returns>The analysis summary.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            var summary = new RevisionMatrixSummary();
            // Gets the TotalProcessTime in minutes and millisecs.
            if (_sumAnalysis != null)
            {
                _sumSummary = (SummaryAnalysisSummary) _sumAnalysis.DoAnalysis();
                TotalProcessTime = _sumSummary.ProcessTimeInMinutes;
                _totalProcessTime = (ulong) _sumSummary.ProcessMillis;
            }

            // Temporary values for a Normal Production Revision
            var previousRevisionIsNormal = false;
            RevisionMatrixEntry tempEntry = new RevisionMatrixEntry();

            foreach (var revision in _revSummary.Revisions)
            {
                var entry = new RevisionMatrixEntry(revision);

                if (revision is NormalProductionRevision)
                {
                    // A new Normal Producton, the previous was something else.
                    if (!previousRevisionIsNormal)
                    {
                        tempEntry.TypeOfRevision = entry.RevisionTypeString();
                        tempEntry.Edits = entry.NumberOfEdits();
                        tempEntry.PosStart = entry.StartPosition();
                        tempEntry.TimeStart = entry.StartTime();
                        tempEntry.PosEnd = entry.EndPosition();
                        tempEntry.TimeEnd = entry.EndTime();
                        tempEntry.Chars = entry.Characters();
                        tempEntry.CharsWithoutSpace = entry.CharWithoutSpace();
                        tempEntry.RevisionDuration = entry.Duration();
                        tempEntry.NumberOfWords = entry.Words();
                        tempEntry.SizeOfRevision = entry.RevisionSize();
                        tempEntry.Text = entry.Content();
                    }
                    else
                    {
                        // If this is a Normal Production following a previous Normal Production, 
                        // we aggregate their data
                        tempEntry.Edits += entry.NumberOfEdits();
                        tempEntry.PosEnd = entry.EndPosition();
                        tempEntry.TimeEnd = entry.EndTime();
                        tempEntry.Chars += entry.Characters();
                        tempEntry.CharsWithoutSpace += entry.CharWithoutSpace();
                        tempEntry.RevisionDuration += entry.Duration();
                        tempEntry.NumberOfWords += entry.Words();
                        tempEntry.SizeOfRevision += entry.RevisionSize();
                        tempEntry.Text += entry.Content();
                    }
                    previousRevisionIsNormal = true;
                }
                else
                {
                    // Copying the content of the temporary Normal Production values
                    if (previousRevisionIsNormal)
                    {
                        summary.Add(tempEntry);
                        tempEntry = new RevisionMatrixEntry();                      
                        summary.Add(entry);
                        previousRevisionIsNormal = false;
                    }
                    else
                    {
                        summary.Add(entry);
                    }
                }     
            }
            // Adding the last Normal Production.
            if (!tempEntry.TypeOfRevision.IsNullOrEmpty())
            {
                summary.Add(tempEntry);
            }

            Summmarize(summary);
            CalculateRBursts(summary);

            return summary;
        }

        private void CalculateRBursts(RevisionMatrixSummary summary)
        {
            var totalTime = _allTimeSpent.Sum(Convert.ToInt32)/1000;
            var totalChar = _allCharsProduced.Sum(Convert.ToInt32);

            summary.NumberOfBursts = _totalNormalCount;
            summary.MeanRBurstTime = (double) totalTime/_totalNormalCount;

            var medianTime = MathExt.GetMedian(_allTimeSpent.Select<ulong, double>(x => x).ToList());
            summary.MedianRBurstTime = medianTime/1000;
            summary.StdevRburstTime = MathExt.StandardDeviation(_totalNormalCount, _totalSquaredTime, totalTime);

            summary.MeanRburstChars = (double) totalChar/_totalNormalCount;
            summary.MedianRburstChars = MathExt.GetMedian(_allCharsProduced.Select<ulong, double>(x => x).ToList());
            summary.StdevRburstChars = MathExt.StandardDeviation(_totalNormalCount, _totalSquaredChar, totalChar);
        }

        private void Summmarize(RevisionMatrixSummary summary)
        {
            var vals = new Dictionary<string, int[]>{ {TotalProcess, new[] 
            { 0, 0, (int) _totalProcessTime, 0, 0, 0, 0 }},{AllRevisions, new int[7]} };

            foreach (var entry in summary.Entries)
            {           
                string t = entry.RevisionTypeString();

                // Collecting R-Burst data.
                // Time in seconds
                if (entry.TypeOfRevision != null && entry.TypeOfRevision.Equals("Normal Production"))
                {
                    t = "Normal Production";
                    _allCharsProduced.Add((ulong) entry.Chars);
                    _allTimeSpent.Add(entry.RevisionDuration);
                    _totalSquaredChar += Math.Pow(entry.Chars, 2);
                    _totalSquaredTime += Math.Pow((double) entry.RevisionDuration/1000, 2);                  
                    _thisEdits = entry.Edits;
                    _thisDuration = entry.RevisionDuration;
                    _thisRevisionSize = entry.SizeOfRevision;
                    _thisChars = entry.Chars;
                    _thisCharWithoutSpace = entry.CharsWithoutSpace;
                    _thisWords = entry.NumberOfWords;
                    ++_totalNormalCount;
                }
                else if(entry.Revision != null)
                {
                    _thisEdits = entry.NumberOfEdits();
                    _thisDuration = entry.Duration(); 
                    _thisRevisionSize = entry.RevisionSize();
                    _thisChars = entry.Characters();
                    _thisCharWithoutSpace = entry.CharsWithoutSpace;
                    _thisWords = entry.Words();
                }

                if (!vals.ContainsKey(t))
                {
                    vals.Add(t, new int[7]);
                }
                AddToSummary(vals[t]);
                AddToSummary(vals[AllRevisions]);
            }
            var totTime = new KeyValuePair<string, int[]>();
            var prodRev= new KeyValuePair<string, int[]>();
            List<KeyValuePair<string, int[]>> sumList = vals.ToList();
            foreach (var pair in vals)
            {
                if (pair.Key.Contains("Total Processing Time"))
                {
                    totTime = pair;
                    sumList.Remove(pair);
                }
                if (pair.Key.Contains("Production + Revisions"))
                {
                    prodRev = pair;
                    sumList.Remove(pair);
                }
            }
            sumList.Add(prodRev);
            sumList.Add(totTime);
            var dict = sumList.ToDictionary(keyItem => keyItem.Key, valueItem => valueItem.Value);
            summary.Summaries = dict;
        }

        private void AddToSummary(int[] vs)
        {
            vs[0]++;
            vs[1] = vs[1] + _thisEdits;
            vs[2] = vs[2] + (int) _thisDuration;
            vs[3] = vs[3] + _thisRevisionSize;
            vs[4] = vs[4] + _thisChars;
            vs[5] = vs[5] + _thisCharWithoutSpace;
            vs[6] = vs[6] + _thisWords;
        }

        #region Fields

        /// <summary>
        ///     The revision summary created by the RevisionAnalysis.
        /// </summary>
        private readonly RevisionAnalysisSummary _revSummary;

        // R-Burst members
        private readonly List<ulong> _allCharsProduced = new List<ulong>();
        private readonly List<ulong> _allTimeSpent = new List<ulong>();
        private int _totalNormalCount;
        private double _totalSquaredChar;
        private double _totalSquaredTime;
        private ulong _totalProcessTime;

        // Summary members
        private int _thisEdits;
        private ulong _thisDuration;
        private int _thisRevisionSize;
        private int _thisChars;
        private int _thisCharWithoutSpace;
        private int _thisWords;

        // Summary indices
        public const int RevisionsNr = 0;
        public const int Edits = 1;
        public const int Duration = 2;
        public const int Length = 3;
        public const int Chars = 4;
        public const int CharsWithoutSpace = 5;
        public const int Words = 6;
        private const string AllRevisions = "Production + Revisions";
        private const string TotalProcess = "Total Processing Time";

        // Reporting members
        public static double TotalProcessTime { get; private set; }
        private readonly SummaryAnalysis _sumAnalysis;
        private SummaryAnalysisSummary _sumSummary;

        #endregion
    }
}