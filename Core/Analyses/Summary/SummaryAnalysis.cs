using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Summary
{
    /// <summary>
    /// A Summary Analysis.
    /// The summary analysis gives an overview of the logging file by providing several statistics of the file.
    /// Example statistics: number of words, avg chars/word, standard deviation chars/word, avg sentences/paragraph, etc.
    /// Besides this, the analysis also determines the different writing clusters and segments within the text and calculates
    /// some pausetime statistics for these clusters and segments.
    /// </summary>
    public class SummaryAnalysis : Analysis
    {
        #region Fields
        #region StatisticalFields
        // character statistics
        private readonly IDictionary<Tuple<string, string>, ulong> EventTypeSwitches;
        private readonly IDictionary<string, ulong> SquaredEventTypeClusterTime;
        private readonly IDictionary<string, ulong> SquaredEventTypeSegmentTime;
        private readonly IDictionary<string, ulong> TotalEventTypeClusterTime;
        private readonly IDictionary<string, ulong> TotalEventTypeClusters;
        private readonly IDictionary<string, ulong> TotalEventTypeSegmentTime;
        private readonly IDictionary<string, ulong> TotalEventTypeSegments;
        private ulong CurrentClusterStartTime;
        private ulong CurrentParagraphCharLength;
        private ulong CurrentParagraphSentenceLength;
        private ulong CurrentParagraphWordLength;
        private ulong CurrentProcessStartTime;
        private ulong CurrentSegmentStartTime;
        private ulong CurrentSentenceCharLength;
        private ulong CurrentSentenceWordLength;
        private ulong CurrentWordLength;
        private int PreviousDocLength;

        private ulong SquaredParagraphCharLength;
        private ulong SquaredParagraphSentenceLength;
        private ulong SquaredParagraphWordLength;
        // pause statistics
        private ulong SquaredProcessTime;
        private ulong SquaredSentenceCharLength;
        private ulong SquaredSentenceWordLength;
        private ulong SquaredWordLength;
        private ulong TotalCharsInSentences;
        private ulong TotalCharsWithoutSpaces;
        private ulong TotalParagraphs;
        private ulong TotalPauses;
        private ulong TotalSentences;
        private ulong TotalSpaces;
        private ulong TotalWordLength;
        // Lists to calculate the median.
        private readonly List<ulong> WordLengthList;
        private readonly List<ulong> SentenceCharList;
        private readonly List<ulong> SentenceWordList;
        private readonly List<ulong> ParagraphCharList;
        private readonly List<ulong> ParagraphWordList;
        private readonly List<ulong> ParagraphSentenceList;
        private readonly List<ulong> PauseTimeList;
        private readonly List<ulong> ProcessTimeList;
        private readonly List<ulong> ProcessCharList;
        private readonly IDictionary<string, List<ulong>> ClusterTime;
        private readonly IDictionary<string, List<ulong>> SegmentTime;
        // pburst statistics
        private ulong SquaredPBurstChars;
        private ulong CurrentPBurstChars;

        // segment statistics
        private ulong TotalWritingSegments;
        // word statistics
        private ulong TotalWords;
        private ulong TotalFormattingKeys;
        private int LastDocLength;
        private int TotalInsert;
        private int ReplacedLength;
        private int TotalReplaced;
        private bool FromMainDoc;
  
        // focus event 
        private Event _lastFocus;
        #endregion

        // Various temporary variables that are needed to do the calculations.
        /// <summary>
        /// Events that are taken in consideration when performing the analyses (other event types are ignored).
        /// </summary>
        private static readonly string[] EventTypes = { EventType.KEYBOARD, EventType.MOUSE, EventType.FOCUS, EventType.SELECTION,
                                                          EventType.INSERT, EventType.REPLACEMENT, EventType.STATISTICS };

        /// <summary>
        /// The types of the events that are analyzed for the writing modes module.
        /// </summary>
        private static readonly string[] WritingModeEventTypes = { EventType.KEYBOARD, EventType.MOUSE };

        /// <summary>
        /// The previous event that was handled by the BeginEvent() callback.
        /// Initialized to null, since obviously no event has been handled when the SummaryAnalysis is constructed.
        /// </summary>
        private Event PrevEvent;

        // Index of an event in the summary list.
        private int ReplacementIndex;

        #endregion

        /// <summary>
        /// Constructs a new SummaryAnalysis.
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification for the list of events.</param>
        public SummaryAnalysis(List<Event> events, SessionIdentification sessionID)
            : base("SA", events, sessionID)
        {
			InputEvents = events.Filter(new EventTypeFilter(EventTypes));
            LastDocLength = 0;
            // Constructing dictionaries (segments and clusters).
            TotalEventTypeSegments = new Dictionary<string, ulong>();
            TotalEventTypeSegmentTime = new Dictionary<string, ulong>();
            SquaredEventTypeSegmentTime = new Dictionary<string, ulong>();

            TotalEventTypeClusters = new Dictionary<string, ulong>();
            TotalEventTypeClusterTime = new Dictionary<string, ulong>();
            SquaredEventTypeClusterTime = new Dictionary<string, ulong>();

            WordLengthList = new List<ulong>();       
            SentenceCharList = new List<ulong>();
            SentenceWordList = new List<ulong>();
            ParagraphCharList = new List<ulong>();
            ParagraphWordList = new List<ulong>();
            ParagraphSentenceList = new List<ulong>();
            PauseTimeList = new List<ulong>();
            ProcessTimeList = new List<ulong>();
            ProcessCharList = new List<ulong>();
            ClusterTime = new Dictionary<string, List<ulong>>();
            SegmentTime = new Dictionary<string, List<ulong>>();

            // Setting all times/counts to 0.
            foreach (string eventType in EventTypes)
            {
                TotalEventTypeSegments[eventType] = 0;
                TotalEventTypeSegmentTime[eventType] = 0;
                SquaredEventTypeSegmentTime[eventType] = 0;

                TotalEventTypeClusters[eventType] = 0;
                TotalEventTypeClusterTime[eventType] = 0;
                SquaredEventTypeClusterTime[eventType] = 0;
                ClusterTime[eventType] = new List<ulong> {0};
                SegmentTime[eventType] = new List<ulong> {0};
            }

            EventTypeSwitches = new Dictionary<Tuple<string, string>, ulong>();
            // Do we have a standard main document?
            string docTitle = sessionID.GetMainDocument().ToLower();
            FromMainDoc = docTitle.Equals(MainDocument.ToLower()) || docTitle.Contains("wordlog") || docTitle.Contains("maindoc");
        }

        /// <summary>
        /// Performs the actual Summary Analysis on the events with which this Analysis was constructed.
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            var pauseLocationMarker = new PauseLocationMarker();

            // Adding event handlers to the PauseLocationMarker.
            pauseLocationMarker.WordEventHandler += UpdateTotalWords;
            pauseLocationMarker.SentenceEventHandler += UpdateTotalSentences;
            pauseLocationMarker.ParagraphEventHandler += UpdateTotalParagraphs;
            pauseLocationMarker.WordCharEventHandler += UpdateTotalWordChars;
            pauseLocationMarker.SentenceCharEventHandler += UpdateTotalSentenceChars;
            pauseLocationMarker.ParagraphCharEventHandler += UpdateTotalParagraphChars;
            pauseLocationMarker.FocusChangeEventHandler += UpdateFocusChange;
            pauseLocationMarker.InsertEventHandler += UpdateInsert;
            pauseLocationMarker.EventEventHandler += BeginEvent;

            // Running the PauseLocationMarker.
            var pauseEventList = pauseLocationMarker.Start(InputEvents);
            if (pauseEventList == null) throw new NotImplementedException();
            FixEndEvent();

            // Creating and returning the Summary Analysis.
            return CreateSummary();
        }

        /// <summary>
        /// Applies a fix using the last event of the list of inputevents.
        /// This is necessary to have correct results for the last segment/cluster and eventype cluster/segment.
        /// The last event holds the'Word Statistics'.
        /// </summary>
        private void FixEndEvent()
        {
            if (InputEvents.Count <= 0) return;
            var lastEvent = GetCurrentLastTimedEvent();
            var lastTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(lastEvent);
            // We assume that the last events are from the main document.
            FromMainDoc = true;
            UpdateTotalWords(this, new PauseLocationEventArgs(null, PauseLocation.UNDETERMINED));
            UpdateTotalSentences(this, new PauseLocationEventArgs(null, PauseLocation.UNDETERMINED));
            UpdateTotalParagraphs(this, new PauseLocationEventArgs(null, PauseLocation.UNDETERMINED));

            ulong clusterActionTime = 0;
            ulong segmentActionTime = 0;

            if (lastTimedEventPart != null)
            {
                clusterActionTime = lastTimedEventPart.EndTime - CurrentClusterStartTime;
                segmentActionTime = lastTimedEventPart.EndTime - CurrentSegmentStartTime;
            }

            TotalWritingSegments++;
            TotalEventTypeClusters[lastEvent.Type]++;
            TotalEventTypeSegments[lastEvent.Type]++;

            TotalEventTypeClusterTime[lastEvent.Type] += clusterActionTime;
            ClusterTime[lastEvent.Type].Add(clusterActionTime);
            SquaredEventTypeClusterTime[lastEvent.Type] += (ulong)Math.Pow(clusterActionTime, 2);

            TotalEventTypeSegmentTime[lastEvent.Type] += segmentActionTime;
            SegmentTime[lastEvent.Type].Add(segmentActionTime);
            SquaredEventTypeSegmentTime[lastEvent.Type] += (ulong)Math.Pow(segmentActionTime, 2);

            if (lastTimedEventPart != null && lastTimedEventPart.EndTime >= CurrentProcessStartTime)

            {
                var actionTime = lastTimedEventPart.EndTime - CurrentProcessStartTime;
                SquaredProcessTime += (ulong)Math.Pow(actionTime, 2);
            }
        }

        #region SummaryCreation
        /// <summary>
        /// Creates a summary from the gathered information.
        /// </summary>
        /// <returns>A summary of the analysis that was performed.</returns>
        private IAnalysisSummary CreateSummary()
        {
            var summary = new SummaryAnalysisSummary();
            AddProductInformation(summary);
            AddProcessInformation(summary);
            AddProductProcessRatio(summary);
            AddProcessTimeInformation(summary);
            AddEventTypeInformation(summary);
            return summary;
        }

        /// <summary>
        /// Helper method of CreateSummary().
        /// Adds the global product information to a given SummaryAnalysisSummary.
        /// This information is extracted from the statistics event in the idfx.
        /// </summary>
        /// <param name="summary">Summary to add the information to.</param>
        private void AddProductInformation(SummaryAnalysisSummary summary)
        {
            var even = InputEvents.LastOrDefault(ev => ev.Type.Equals(EventType.STATISTICS));
            if (even == null) return;
            var stats = even.Parts.OfType<Statistics>().First();
            summary.ProductInformation.StatsFound = true;

            summary.ProductInformation.CharCountWithoutSpaces = stats.CharCountWithoutSpaces; 
            summary.ProductInformation.CharCountWithSpaces = stats.CharCountWithSpaces; 
            summary.ProductInformation.LineCount = stats.LineCount;
            summary.ProductInformation.PageCount = stats.PageCount;
            summary.ProductInformation.ParagraphCount = stats.ParagraphCount;
            summary.ProductInformation.WordCount = stats.WordCount;
            // Additonal information on the document before logging started. Fields may be empty.
            summary.ProductInformation.StartCharCountWithoutSpaces = stats.StartCharCountWithoutSpaces;
            summary.ProductInformation.StartCharCountWithSpaces = stats.StartCharCountWithSpaces;
            summary.ProductInformation.StartWordCount = stats.StartWordCount;
        }

        /// <summary>
        /// Helper method of CreateSummary().
        /// Adds the global process information to a given SummaryAnalysisSummary.
        /// </summary>
        /// <param name="summary">Summary to add the information to.</param>
        private void AddProcessInformation(SummaryAnalysisSummary summary)
        {
            // Subtracting one char that may have been added artificially at the opening of a loggedDocument.
            // See:  InputLog.Core.Plugin.WordLog/InputlogDocument.cs line 125
            var totalCharsWithSpaces = TotalCharsWithoutSpaces + TotalSpaces;
            var avgWordLength = TotalWords == 0 ? 0 : TotalWordLength / (double)TotalWords;
            
            var avgSentenceCharLength = TotalSentences == 0 ? 0 : totalCharsWithSpaces / (double)TotalSentences;
            var avgSentenceWordLength = TotalSentences == 0 ? 0 : TotalWords / (double)TotalSentences;
            var avgParagraphCharLength = TotalParagraphs == 0 ? 0 : totalCharsWithSpaces / (double)TotalParagraphs;
            var avgParagraphWords = TotalParagraphs == 0 ? 0 : TotalWords / (double)TotalParagraphs;
            var avgParagraphSentences = TotalParagraphs == 0 ? 0 : TotalSentences / (double)TotalParagraphs;

            var medianWordLength = TotalWords == 0 ? 0
                : MathExt.GetMedian(WordLengthList.Select<ulong, double>(i => i).ToList());
            var medianSentenceCharLength = TotalSentences == 0 ? 0 :
                MathExt.GetMedian(SentenceCharList.Select<ulong, double>(i => i).ToList());
            var medianSentenceWordLength = TotalSentences == 0 ? 0
                : MathExt.GetMedian(SentenceWordList.Select<ulong, double>(i => i).ToList());
            var medianParagraphCharLength = TotalParagraphs == 0 ? 0
                : MathExt.GetMedian(ParagraphCharList.Select<ulong, double>(i => i).ToList());
            var medianParagraphWords = TotalParagraphs == 0 ? 0
                : MathExt.GetMedian(ParagraphWordList.Select<ulong, double>(i => i).ToList());
            var medianParagraphSentences = TotalParagraphs == 0 ? 0
                : MathExt.GetMedian(ParagraphSentenceList.Select<ulong, double>(i => i).ToList());

            var sdWordCharLength = MathExt.StandardDeviation(TotalWords, SquaredWordLength, TotalWordLength);
            var sdSentenceCharLength = MathExt.StandardDeviation(TotalSentences,
                SquaredSentenceCharLength,TotalCharsInSentences);
            var sdSentenceWordLength = MathExt.StandardDeviation(TotalSentences, SquaredSentenceWordLength,
                TotalWords);
            var sdParagraphCharLength = MathExt.StandardDeviation(TotalParagraphs, SquaredParagraphCharLength,
                TotalCharsWithoutSpaces);
            var sdParagraphWordLength = MathExt.StandardDeviation(TotalParagraphs, SquaredParagraphWordLength,
                TotalWords);
            var sdParagraphSentenceLength = MathExt.StandardDeviation(TotalParagraphs,
                SquaredParagraphSentenceLength, TotalSentences);

            // Populating the summary fields. 
            // ProcessInformation
            summary.ProcessInformation.Words.TotalCharsProduced = Convert.ToInt32(totalCharsWithSpaces) + Convert.ToInt32(TotalFormattingKeys) + TotalInsert + TotalReplaced;
            if (TotalInsert < 0)
            {
                summary.ProcessInformation.Words.TotalCharsCopied = 0;
                summary.ProcessInformation.Words.TotalCharsProduced = Convert.ToInt32(totalCharsWithSpaces) + Convert.ToInt32(TotalFormattingKeys);
            }
            else
            {
                summary.ProcessInformation.Words.TotalCharsCopied = TotalInsert;
            }
            summary.ProcessInformation.Words.TotalCharsReplaced = TotalReplaced;
            summary.ProcessInformation.Words.TotalCharactersWithoutSpaces = TotalCharsWithoutSpaces;
           
            summary.ProcessInformation.Words.TotalCharactersWithSpaces = totalCharsWithSpaces;
            
            // Formatting keys are included in the Inputlog process information, but not are not
            // counted as characters by Microsoft Word. 
            summary.ProcessInformation.Words.FormattingKeys = Convert.ToInt32(TotalFormattingKeys);

            summary.ProcessInformation.Words.TotalWords = TotalWords;
            summary.ProcessInformation.Words.AvgWordLength = avgWordLength;
            summary.ProcessInformation.Words.MedianWordLength = medianWordLength;
            summary.ProcessInformation.Words.StandardDeviation = sdWordCharLength;
            // Sentences
            summary.ProcessInformation.Sentences.TotalSentences = TotalSentences;
            summary.ProcessInformation.Sentences.AvgCharactersPerSentence = avgSentenceCharLength;
            summary.ProcessInformation.Sentences.MedianCharactersPerSentence = medianSentenceCharLength;
            summary.ProcessInformation.Sentences.StandardDeviationCs = sdSentenceCharLength;
            summary.ProcessInformation.Sentences.AvgWordsPerSentence = avgSentenceWordLength;
            summary.ProcessInformation.Sentences.MedianWordsPerSentence = medianSentenceWordLength;
            summary.ProcessInformation.Sentences.StandardDeviationWs = sdSentenceWordLength;
            // Paragraphs
            summary.ProcessInformation.Paragraphs.TotalParagraphs = TotalParagraphs;
            summary.ProcessInformation.Paragraphs.AvgCharactersPerParagraph = avgParagraphCharLength;
            summary.ProcessInformation.Paragraphs.MedianCharactersPerParagraph = medianParagraphCharLength;
            summary.ProcessInformation.Paragraphs.StandardDeviationCp = sdParagraphCharLength;
            summary.ProcessInformation.Paragraphs.AvgWordsPerParagraph = avgParagraphWords;
            summary.ProcessInformation.Paragraphs.MedianWordsPerParagraph = medianParagraphWords;
            summary.ProcessInformation.Paragraphs.StandardDeviationWp = sdParagraphWordLength;
            summary.ProcessInformation.Paragraphs.AvgSentencePerParagraph = avgParagraphSentences;
            summary.ProcessInformation.Paragraphs.MedianSentencePerParagraph = medianParagraphSentences;
            summary.ProcessInformation.Paragraphs.StandardDeviationSp = sdParagraphSentenceLength;
        }

        /// <summary>
        /// Helper method of CreateSummary().
        /// Adds the process product ratio to a given SummaryAnalysisSummary.
        /// </summary>
        /// <param name="summary">Summary to add the information to.</param>
        private void AddProductProcessRatio(SummaryAnalysisSummary summary)
        {
            if (!summary.ProductInformation.StatsFound) return;

            summary.ProductProcessRatio.CharWithoutSpacesRatio = TotalCharsWithoutSpaces == 0 ? double.NaN
                : summary.ProductInformation.CharCountWithoutSpaces / ((double) summary.ProcessInformation.Words.TotalCharactersWithoutSpaces);

            summary.ProductProcessRatio.CharWithSpacesRatio = (TotalCharsWithoutSpaces + TotalSpaces) == 0 ? double.NaN
                : summary.ProductInformation.CharCountWithSpaces / (double)summary.ProcessInformation.Words.TotalCharactersWithSpaces;

            summary.ProductProcessRatio.ProdWithSpacesRatio = summary.ProcessInformation.Words.TotalCharsProduced == 0 ? double.NaN
                : (summary.ProductInformation.CharCountWithSpaces + Convert.ToInt32(TotalFormattingKeys))/ (double)summary.ProcessInformation.Words.TotalCharsProduced;

            summary.ProductProcessRatio.WordRatio = summary.ProcessInformation.Words.TotalWords == 0 ? double.NaN
                : summary.ProductInformation.WordCount / (double)summary.ProcessInformation.Words.TotalWords;
        }

        /// <summary>
        /// Helper method of CreateSummary().
        /// Adds the global process time information to a given SummaryAnalysisSummary.
        /// </summary>
        /// <param name="summary">Summary to add the information to.</param>
        private void AddProcessTimeInformation(SummaryAnalysisSummary summary)
        {
            var lastEvent = GetCurrentLastTimedEvent();
            var lastTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(lastEvent);
            var totalProcessTime = lastTimedEventPart?.EndTime - NewStartOffset ?? 0;
            var avgProcessTime = TotalWritingSegments == 0 ? 0 : totalProcessTime / TotalWritingSegments;
            var medianProcessTime = TotalWritingSegments == 0 ? 0 
                : MathExt.GetMedian(ProcessTimeList.Select<ulong, double>(i => i).ToList());
            var medianProcessChars = TotalWritingSegments == 0 ? 0
                : MathExt.GetMedian(ProcessCharList.Select<ulong, double>(i => i).ToList());
            var sdProcessTime = MathExt.StandardDeviation(TotalWritingSegments, SquaredProcessTime, totalProcessTime);

            // ProcessTime
            // General
            summary.ProcessTime.General.TotalProcessTime = totalProcessTime;
            summary.ProcessTime.General.NumberOfSegments = TotalWritingSegments;
            summary.ProcessTime.General.PBurstsPerMinute = (double) totalProcessTime / TotalWritingSegments / 60000;
            summary.ProcessTime.General.AvgProcessTime = avgProcessTime;
            summary.ProcessTime.General.MedianProcessTime = medianProcessTime;
            summary.ProcessTime.General.StandardDeviation = (ulong)sdProcessTime;
            summary.ProcessTime.General.AvgProcessChars = TotalPauses == 0 ? 0
                : (double) summary.ProcessInformation.Words.TotalCharactersWithSpaces / TotalPauses;
            summary.ProcessTime.General.MedianProcessChars = medianProcessChars;
            summary.ProcessTime.General.StandardDeviationChars = MathExt.StandardDeviation(TotalPauses, 
                SquaredPBurstChars, summary.ProcessInformation.Words.TotalCharactersWithSpaces);

        }

        /// <summary>
        /// Returns the last event from the event list that has a timeEvent part 
        /// and adapting the CurrentProcessStartTime to the new position in the event list.
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
                    CurrentProcessStartTime = prevTimedEventPart.EndTime;
                    if (lastTimedEventPart != null && lastTimedEventPart.EndTime == 0)
                    {
                        lastTimedEventPart = null;
                    }
                }
                i--;
            }
            return currentEvent;
        }

        /// <summary>
        /// Helper method of CreateSummary().
        /// Adds information per eventType to a given SummaryAnalysisSummary.
        /// </summary>
        /// <param name="summary">Summary to add the information to.</param>
        private void AddEventTypeInformation(SummaryAnalysisSummary summary)
        {
            // EventTypes
            foreach (var eventType in WritingModeEventTypes)
            {
                var totalClusters = TotalEventTypeClusters[eventType];
                var avgClusterTime = totalClusters == 0 ? 0 : TotalEventTypeClusterTime[eventType] / totalClusters;
                int medianClusterTime = (int)(totalClusters == 0 ? 0 : 
                    MathExt.GetMedian(ClusterTime[eventType].Select<ulong, double>(i => i).ToList()));
                var sdClusterTime = (ulong)MathExt.StandardDeviation(totalClusters,
                    SquaredEventTypeClusterTime[eventType], TotalEventTypeClusterTime[eventType]);

                var totalSegments = TotalEventTypeSegments[eventType];
                var avgSegmentTime = totalSegments == 0 ? 0 : TotalEventTypeSegmentTime[eventType] / totalSegments;
                int medianSegmentTime = (int)(totalSegments == 0 ? 0
                    : MathExt.GetMedian(SegmentTime[eventType].Select<ulong, double>(i => i).ToList()));

                var sdSegmentTime = (ulong)MathExt.StandardDeviation(totalSegments,
                    SquaredEventTypeSegmentTime[eventType], TotalEventTypeSegmentTime[eventType]);
                var wmEntry = new SummaryAnalysisSummary.WritingModeClass.WritingModeEntryClass
                {
                    TotalTime = TotalEventTypeClusterTime[eventType],
                    NumberOfClusters = totalClusters,
                    AvgTimeClusters = avgClusterTime,
                    MedianTimeClusters = medianClusterTime,
                    StandardDeviationClusters = sdClusterTime,
                    NumberOfSegments = totalSegments,
                    AvgTimeSegments = avgSegmentTime,
                    MedianTimeSegments = medianSegmentTime,
                    StandardDeviationSegments = sdSegmentTime
                };

                // Switches between eventtypes.
                foreach (var eventType2 in WritingModeEventTypes)
                {
                    if (eventType == eventType2) continue;
                    ulong numberOfSwitches;
                    var pair = new Tuple<string, string>(eventType, eventType2);
                    if (EventTypeSwitches.TryGetValue(pair, out numberOfSwitches))
                    {
                        wmEntry.Switches[eventType2] = numberOfSwitches;
                    }
                }
                summary.WritingMode.Entries[eventType] = wmEntry;
            }
        }
        #endregion

        #region EventHandlers
        /// <summary>
        /// EventEventHandler.
        /// This callback is called after every handled event (by the PauseLocationMarker).
        /// It updates segments, clusters,  global process and pausetimes if necessary.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void BeginEvent(object sender, PauseLocationEventArgs eventArgs)
        {
            var currentEvent = eventArgs.Event;

            var keyPress = Event.GetFirstEventPart<KeyPress>(currentEvent);
            var wordKeyPress = Event.GetFirstEventPart<Keypress>(currentEvent);
            //var insert = Event.GetFirstEventPart<Insert>(currentEvent);

            UpdateEventTypeSwitches(PrevEvent, currentEvent);
            UpdateTotalSegments(PrevEvent, currentEvent);

            CountDocLength(wordKeyPress);

            // If a replacement event was seen earlier, we check if the docLength at that moment
            // is different from the current doclength. If that is the case, the length of the replaced text
            // is added to the TotalReplaced counter.
            if (ReplacementIndex > 0)
            {
                if (LastDocLength != PreviousDocLength)
                {
                    TotalReplaced += ReplacedLength;
                }
                ReplacementIndex = 0;
            }

            if (keyPress != null && FromMainDoc)
            {
                // Counting the formatting keys
                if (Lexical.HasCombinationKey(keyPress))
                {
                    TotalFormattingKeys++;
                }
                // White Space has a broader definition, not just the empty string.
                if (Lexical.IsWhiteSpace(keyPress.Value)) 
                {
                    TotalSpaces++;
                    CurrentPBurstChars++;
                }
                    // Checking both that the count == 1 and that we don't have a tab, space or enter.
                    // this extra check (tab, space, enter) is necessary because in the new representation these keys also
                    // have a single character representation (' ', '\t', '\n' and '\r'), while in the legacy code these keys
                    // had a multiple character representation ('SPACE', 'TAB', 'ENTER'), 
                    // and thus are never matched by the count == 1.
                else if (keyPress.Value.Length == 1
                    && (Lexical.IsAlphaNumeric(keyPress.Value)
                    || Lexical.IsUnicodePunctuation(keyPress.Value)))
                {                 
                    TotalCharsWithoutSpaces++;                   
                    CurrentPBurstChars++;
                }
            }
            PrevEvent = currentEvent;
        }

        /// <summary>
        /// WordCharEventHandler.
        /// Updates the total number of characters in the current word.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalWordChars(object sender, PauseLocationEventArgs eventArgs)
        {
            CurrentWordLength++;
        }

        /// <summary>
        /// SentenceCharEventHandler.
        /// Updates the total number of characters within the current sentence.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalSentenceChars(object sender, PauseLocationEventArgs eventArgs)
        {
            CurrentSentenceCharLength++;
        }

        /// <summary>
        /// ParagraphCharEventHandler.
        /// Updates the total number of characters within the current paragraph.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalParagraphChars(object sender, PauseLocationEventArgs eventArgs)
        {
            CurrentParagraphCharLength++;
        }

        /// <summary>
        /// Creates a new segment if the pauseTime between the 2 given events (the previous event and the current event) 
        /// exceeds the pauseThreshold.
        /// Also updates the global pause statistics (total pause time, total squared pause time, etc) 
        /// and the process statistics.
        /// </summary>
        /// <param name="pEvent">The previous event.</param>
        /// <param name="cEvent">The current event.</param>
        private void UpdateTotalSegments(Event pEvent, Event cEvent)
        {
            // EVH 20170417 The PauseLocationMarker delegates a Focus event only to the FocusChangeEventHandler.
            // Hence, between the current event (cEvent) and the previous (pEvent) there is an event missing when the 
            // real previous event was a Focus change, resulting in a faulty pauseTime calculation.
            // Therefore, if the previous id (not the pEvent) belongs to a Focus change we return a pauseTime of zero.
            var pauseTime = _lastFocus != null && (cEvent.GetId() - 1 == _lastFocus.GetId())
                ? 0ul
                : DeterminePauseTime(pEvent, cEvent);

            var prevTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(pEvent);
            var currentTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(cEvent);
            // Increases total pauses.
            TotalPauses++;
            PauseTimeList.Add(pauseTime);
            ProcessCharList.Add(CurrentPBurstChars);
            SquaredPBurstChars += (CurrentPBurstChars * CurrentPBurstChars);
            CurrentPBurstChars = 0;

            if (prevTimedEventPart != null && currentTimedEventPart != null)
            {
                // Only update totalWritingSegments if it's not the first event.
                // Then we can just add one in the end. Otherwise you get an extra segment
                // for the initial pause.
                TotalWritingSegments++;

                // Legacy: 2 keyboard events may overlap: the second has an endtime before the endtime of the first.
                if(prevTimedEventPart.EndTime > CurrentProcessStartTime && CurrentProcessStartTime > 0)
                {
                    var processTime = prevTimedEventPart.EndTime - CurrentProcessStartTime;
                    ProcessTimeList.Add(processTime);
                    SquaredProcessTime += (ulong)Math.Pow(processTime, 2);
                }
                CurrentProcessStartTime = prevTimedEventPart.EndTime;

                var actionTime = prevTimedEventPart.EndTime < CurrentSegmentStartTime ? 0 : 
                    (prevTimedEventPart.EndTime - CurrentSegmentStartTime);

                if (pEvent != null)
                {
                    if (pEvent.Type == cEvent.Type && pauseTime > 0)
                    {
                        TotalEventTypeSegments[cEvent.Type]++;
                        TotalEventTypeSegmentTime[cEvent.Type] += actionTime;
                        SquaredEventTypeSegmentTime[cEvent.Type] += (ulong)Math.Pow(actionTime, 2);

                        ProcessTimeList.Add(actionTime);
                        SquaredProcessTime += (ulong)Math.Pow(actionTime, 2);
                    }
                }
            }
            if (currentTimedEventPart != null)
            {
                CurrentSegmentStartTime = currentTimedEventPart.StartTime;
            }
        }

        /// <summary>
        /// Updates the EventType Switches if the 2 given events (the previous event and the current event) are of 
        /// a different type.
        /// This method also updates the current cluster and segment if the types of the events are different.
        /// 
        /// Important: Method should be called for _every_ event.
        /// </summary>
        /// <param name="pEvent">The previous event.</param>
        /// <param name="cEvent">The current event.</param>
        private void UpdateEventTypeSwitches(Event pEvent, Event cEvent)
        {
            var prevTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(pEvent);
            var currentTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(cEvent);
            if (currentTimedEventPart == null) return;
            if (prevTimedEventPart != null)
            {
                // If the current event is from an other type than the previous event
                // => update some values.
                if (pEvent.Type != cEvent.Type)
                {
                    // Hack to prevent underflow of ulong. 
                    if (CurrentSegmentStartTime > prevTimedEventPart.EndTime)
                    {
                        CurrentSegmentStartTime = prevTimedEventPart.EndTime;
                    }
                    if (CurrentClusterStartTime > prevTimedEventPart.EndTime)
                    {
                        CurrentClusterStartTime = prevTimedEventPart.EndTime;
                    }
                    // Updating and closing the previous cluster/segment, add new cluster/segment.
                    var clusterActionTime = prevTimedEventPart.EndTime - CurrentClusterStartTime;
                    var segmentActionTime = prevTimedEventPart.EndTime - CurrentSegmentStartTime;

                    CurrentClusterStartTime = currentTimedEventPart.StartTime;
                    CurrentSegmentStartTime = currentTimedEventPart.StartTime;

                    // We switched of event type => increase the switchescounter of 
                    // the pair(PrevEvent.Type, CurrentEvent.Type).
                    var key = new Tuple<string, string>(pEvent.Type, cEvent.Type);
                    if (EventTypeSwitches.ContainsKey(key))
                    {
                        EventTypeSwitches[key]++;
                    }
                    else
                    {
                        EventTypeSwitches[key] = 1;
                    }

                    TotalEventTypeClusters[pEvent.Type]++;
                    TotalEventTypeSegments[pEvent.Type]++;
                    TotalEventTypeClusterTime[pEvent.Type] += clusterActionTime;
                    ClusterTime[pEvent.Type].Add(clusterActionTime);
                    SquaredEventTypeClusterTime[pEvent.Type] += (ulong) Math.Pow(clusterActionTime, 2);
                    TotalEventTypeSegmentTime[pEvent.Type] += segmentActionTime;
                    SegmentTime[pEvent.Type].Add(segmentActionTime);
                    SquaredEventTypeSegmentTime[pEvent.Type] += (ulong) Math.Pow(segmentActionTime, 2);
                }
            }
            else
            {
                // First event.
                CurrentClusterStartTime = currentTimedEventPart.StartTime;
                CurrentSegmentStartTime = currentTimedEventPart.StartTime;
            }
        }

        /// <summary>
        /// WordEventHandler.
        /// Records a new word if CurrentWordLength > 0.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalWords(object sender, PauseLocationEventArgs eventArgs)
        {
            if (CurrentWordLength <= 0 || !FromMainDoc)
            {
                CurrentWordLength = 0;
                return;
            }
            TotalWords++;
            CurrentSentenceWordLength++;
            CurrentParagraphWordLength++;
            SquaredWordLength += (ulong)Math.Pow(CurrentWordLength, 2);
            TotalWordLength += CurrentWordLength;
            WordLengthList.Add(CurrentWordLength);
            CurrentWordLength = 0;
        }

        /// <summary>
        /// SentenceEventHandler.
        /// Records a new sentence if CurrentSentenceCharLength > 0.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalSentences(object sender, PauseLocationEventArgs eventArgs)
        {
            if (CurrentSentenceCharLength <= 0 || !FromMainDoc)
            {
                CurrentSentenceCharLength = 0;
                CurrentSentenceWordLength = 0;
                return;
            }
            TotalCharsInSentences += CurrentSentenceCharLength;
            SentenceCharList.Add(CurrentSentenceCharLength);
            SentenceWordList.Add(CurrentSentenceWordLength);
            TotalSentences++;
            CurrentParagraphSentenceLength++;
            SquaredSentenceCharLength += (ulong)Math.Pow(CurrentSentenceCharLength, 2);
            SquaredSentenceWordLength += (ulong)Math.Pow(CurrentSentenceWordLength, 2);
            CurrentSentenceCharLength = 0;
            CurrentSentenceWordLength = 0;
        }

        /// <summary>
        /// ParagraphEventHandler.
        /// Records a new paragraph if CurrentParagraphCharLength > 0.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker).</param>
        /// <param name="eventArgs">Event arguments passed by the sender.</param>
        private void UpdateTotalParagraphs(object sender, PauseLocationEventArgs eventArgs)
        {
            if (CurrentParagraphCharLength <= 0 || !FromMainDoc)
            {
                CurrentParagraphCharLength = 0;
                CurrentParagraphSentenceLength = 0;
                CurrentParagraphWordLength = 0; 
                return;
            }
            TotalParagraphs++;
            SquaredParagraphCharLength += (ulong)Math.Pow(CurrentParagraphCharLength, 2);
            SquaredParagraphWordLength += (ulong)Math.Pow(CurrentParagraphWordLength, 2);
            ParagraphCharList.Add(CurrentParagraphCharLength);
            ParagraphWordList.Add(CurrentParagraphWordLength);
            ParagraphSentenceList.Add(CurrentParagraphSentenceLength);
            SquaredParagraphSentenceLength += (ulong)Math.Pow(CurrentParagraphSentenceLength, 2);
            CurrentParagraphCharLength = 0;
            CurrentParagraphSentenceLength = 0;
            CurrentParagraphWordLength = 0;    
        }

        /// <summary>
        /// Notifies a focus change and checks if the main document has the current focus.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker)</param>
        /// <param name="eventArgs">PauseLocation args</param>
        private void UpdateFocusChange(object sender, PauseLocationEventArgs eventArgs)
        {
            _lastFocus = eventArgs.Event;
            string docTitle = eventArgs.Event.GetWindowTitle(eventArgs.Event).ToLower();
            FromMainDoc = docTitle.Equals(MainDocument.ToLower()) || docTitle.Contains("wordlog") || docTitle.Contains("maindoc"); //|| docTitle.Contains("microsoft");
        }

        /// <summary>
        /// Notifies text inserts from 'Insert' and 'Replacement' events.
        /// </summary>
        /// <param name="sender">Sender of the event (=PauseLocationMarker)</param>
        /// <param name="eventArgs"></param>
        private void UpdateInsert(object sender, PauseLocationEventArgs eventArgs)
        {
            // Collecting the length of the inserts. 
            foreach (var insertPart in eventArgs.Event.Parts.OfType<Insert>())
            {
                TotalInsert += insertPart.Length;     // Math.Max(insertPart.After.Length, insertPart.Before.Length);                       
            }

            // Remembering the id of this event.
            if (eventArgs.Event.Type.Equals("replacement"))
            {
                ReplacementIndex = eventArgs.Event.GetId();
                PreviousDocLength = LastDocLength;

                foreach (var replacePart in eventArgs.Event.Parts.OfType<Replacement>())
                {
                    if (replacePart.Length > 0 && replacePart.NewText.Length > 0)
                    {
                        ReplacedLength = replacePart.NewText.Length;
                    }
                }  
            }
        }

        #endregion

        /// <summary>
        /// Counting the difference between the previous document length and the current,
        ///  to count the deleted characters or to observe a document length increase.
        /// </summary>
        /// <param name="wordLogPart">Part of the event that contains document length</param>
        private void CountDocLength(Keypress wordLogPart)
        {
            if (null == wordLogPart) return;
            if(LastDocLength > wordLogPart.DocumentLength)
            {
            }
            LastDocLength = wordLogPart.DocumentLength;         
        }
    }
}