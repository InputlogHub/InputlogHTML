using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputLog.Core.Events;
using InputLog.Core.Events.DragonNS;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using log4net;

namespace InputLog.Core.Analyses.General
{
    /// <summary>
    /// A General Analysis.
    /// The general analysis collects basic information for each of the logged events.
    /// This basic info includes: event analysisType, output, pausetime, actiontime, start- and endtime, pauselocation, etc.
    /// </summary>
    public class GeneralAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// List of controlKeys that need to be taken into account.
        /// </summary>
        private readonly List<KeysEx> _controlKeys;

        /// <summary>
        /// Log4Net MessageLogger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// All event types we wish to do the analysis on (others are discarded).
        /// </summary>
        private static readonly string[] EventTypes =
        {
            EventType.KEYBOARD, EventType.FOCUS, EventType.MOUSE,
            EventType.INSERT, EventType.REPLACEMENT,
            EventType.EYETRACK, EventType.DRAGONNS
        };

        /// <summary>
        /// Inputlog (IPL) Event Types
        /// </summary>
        private static readonly string[] IplEventTypes =
        {
            EventType.FOCUS,
            EventType.INSERT,
            EventType.KEYBOARD,
            EventType.MOUSE,
            EventType.REPLACEMENT
        };

        /// <summary>
        ///  Flags to indicate if the previous or current output contains a composite key.
        /// </summary>
        private static bool _previousComposite;

        private static bool _currentIsComposite;

        /// <summary>
        /// Class variable to allow access to keyPress outside method AnalyzeKeyboardEvent 
        /// </summary>
        private static KeyPress _keyLogPart;

        // Last doclength seen.
        private static int _lastDoclength;
        private int? _previousDocLength;
        // Last position seen.
        private int _lastPosition;
        // The number of characters deleted.
        private int _deletedChars;
        // Characters replaced
        private int _replacedChars;
        // The length of the last insert.
        private static int _insertLength;
        // Index of an event in the summary list.
        private int _eventIndex;
        // The previous output event
        private static GeneralAnalysisSummary.GeneralAnalysisEvent _prevOutput;

        // The summary for this analysis.
        protected GeneralAnalysisSummary Summary;

        #endregion

        #region FixedSizeInterval

        /// <summary>
        /// The start time for the first event.
        /// </summary>
        private ulong _firstStartTime;

        private ulong _lastEndTime;

        /// <summary>
        /// Fixed number of intervals
        /// </summary>
        private int NumberOfIntervals;

        /// <summary>
        /// Size of an interval in msec, yielding 1 min. slots.
        /// </summary>
        private ulong IntervalSize;

        /// <summary>
        /// Remembers the how-mannieth interval this is.
        /// </summary>
        private int FixedNumberInterval { get; set; }

        private int FixedSizeInterval { get; set; }

        #endregion

        /// <summary>
        /// Constructs a new GeneralAnalysis.
        /// </summary>;
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="abbrv">The abbreviation of the analysis that ordered this analysis</param>
        /// <param name="numberOfIntervals">Partitioning of the General in a given number of intervals</param>
        /// <param name="intervalSize">Partitioning of the General in intervals of a size in minutes</param>
        /// Can be different from the full analysis name: e.g. the ProcesGraphAnalysis makes use of a 
        /// General Analysis but is different in that it publishes a visualization of the data (chart)
        /// and not an html-page with statistics. This GA will stay in memory, not written to disk.
        /// <param name="controlKeys">List of controlKeys that need to be taken into account.</param>
        public GeneralAnalysis(List<Event> events, SessionIdentification sessionID, string abbrv, int numberOfIntervals,
            ulong intervalSize, List<KeysEx> controlKeys = null)
            : base(abbrv, events, sessionID)
        {
            InputEvents = events.FilterWithoutAltering(new EventTypeFilter(EventTypes));
            _controlKeys = controlKeys;
            Summary = new GeneralAnalysisSummary();
            _lastDoclength = 0;
            NumberOfIntervals = numberOfIntervals;
            IntervalSize = intervalSize * 60000;
        }

        /// <summary>
        /// Performs the actual General Analysis on the events with which this Analysis was constructed.
        /// </summary>
        /// <returns>A list with analyzed events</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            // TOM 8/09/2013: Moved the pause location calculation code to the AnalyzeEvents method.
            // Determine some extra info / do some preprocessing
            if (_controlKeys != null)
            {
                RemoveDuplicateControlKeys(InputEvents, _controlKeys);
            }
            // Discover double clicks if the threshold > 0
            if (Settings.Analysis.DoubleClickThreshold > 0)
            {
                RecognizeDoubleClicks(InputEvents, Settings.Analysis.DoubleClickThreshold);
            }

            // Process actual events (+ write to analysis-document).
            return AnalyzeEvents();
        }

        /// <summary>
        /// This method performs the actual analysis on a given list of events (and their pause location).
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        private GeneralAnalysisSummary AnalyzeEvents()
        {
            /*
			 * TOM 8/9/2013:
			 * In order to correctly determine pause times while the inputEvent list is 'muddled' 
			 * with DNS and EYETRACK events, we will ignore these events when calculating pause times.
			 * The pause list resulting from this will contain the correct pause times calculated
			 * on only the native inputlog events. This list will then be enrichted by adding all the 
			 * missing DNS and/or EYETRACK events in the correct location, but with a pause location
			 * defined as UNKNOWN.
			 */

            /////////////////////////////////////////////////////////////////////
            // START OF PAUSELIST ALTERING CODE:
            /////////////////////////////////////////////////////////////////////

            List<Event> iplOnlyEvents = InputEvents.FilterWithoutAltering(new EventTypeFilter(IplEventTypes));

            // Determine the pauselocations of the native inputlog events.
            var pauseLocationMarker = new PauseLocationMarker();
            Pair<Event, PauseLocation>[] tmpEventPauseList = pauseLocationMarker.Start(iplOnlyEvents).ToArray();

            // Enrich this eventPauseList with the events we have excluded from the pause location.
            var eventPauseList = new List<Pair<Event, PauseLocation>>();
            int tmpPauseListCounter = 0;

            foreach (Event @event in InputEvents)
            {
                // If the next inputEvent is an event that has already had its pause calculated:
                if (tmpPauseListCounter < tmpEventPauseList.Length &&
                    tmpEventPauseList[tmpPauseListCounter].First.Properties["id"] == @event.Properties["id"])
                {
                    // copy it to the resultList and increment counter
                    eventPauseList.Add(tmpEventPauseList[tmpPauseListCounter]);
                    tmpPauseListCounter++;
                }
                else
                {
                    // Add the @event to the resultList as a pair with an unknown pause time.
                    if (@event.Type.Equals("eyetrack"))
                    {
                        eventPauseList.Add(new Pair<Event, PauseLocation>(@event, PauseLocation.EYETRACK));
                    }
                    else if (@event.Type.Equals("dragonns"))
                    {
                        eventPauseList.Add(new Pair<Event, PauseLocation>(@event, PauseLocation.SPEECH));
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////
            // END OF PAUSELIST ALTERING CODE:
            /////////////////////////////////////////////////////////////////////

            Event prevEvent = null;

            // Getting first and last timed event to calculate the intervals
            if (null != Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]))
            {
                FixedNumberInterval = 1;
                FixedSizeInterval = 1;
                _firstStartTime = Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]).StartTime - NewStartOffset;
                // Going backwards to find the last event that is a timedEventPart,
                TimedEventPart lastEventTimedPart = null;
                var i = InputEvents.Count - 1;
                while (i >= 0)
                {
                    lastEventTimedPart = Event.GetFirstEventPart<TimedEventPart>(InputEvents[i]);
                    if (lastEventTimedPart != null) break;
                    i--;
                }
                // throw exception if there is no such event,
                if (lastEventTimedPart == null)
                {
                    Exception e = new AnalysisException("No TimedEvent Part found in the given event list");
                    Log.Warn(e);
                    throw e;
                }
                _lastEndTime = lastEventTimedPart.EndTime - NewStartOffset;
            }

            // If this GeneralAnalysis is used to produce data for a chart by the ProcesGraphAnalysis, 
            // it will be written to memory and not to disk.
            GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent = null;

            // Looping over all events
            foreach (var eventPair in eventPauseList)
            {
                var even = eventPair.First;

                // 1. Copy pauselocation (the pauselocation has already been determined by the PauseLocationMarker).
                // 2. Only start a new outputEvent if the previous one has been completed (and thus
                //     added to the analysisSummary).
                if (outputEvent == null || outputEvent.IsCompleted)
                {
                    outputEvent = Summary.GetAnalysisEvent();
                    outputEvent.PauseLocation = eventPair.Second;
                    outputEvent.NewStartOffset = NewStartOffset;
                }

                // Events get an id (sequential number)
                outputEvent.Id = even.Properties["id"];

                // Determine the pauseTime.
                outputEvent.PauseTime = DeterminePauseTime(prevEvent, even);

                // Getting the interval for this event.
                outputEvent.FixedNumberInterval = GetFixedNumberInterval(even);
                outputEvent.FixedSizeInterval = GetFixedSizeInterval(even);

                // Delegate the events based on their analysisType.
                outputEvent.Type = even.Type;

                switch (even.Type)
                {
                    case EventType.REPLACEMENT:
                    {
                        // Remember the position of this event in the summary list.
                        _eventIndex = Summary.Events.Count - 1;
                        AnalyzeReplacementEvent(even, outputEvent);
                        _previousDocLength = _lastDoclength;
                        break;
                    }
                    case EventType.INSERT:
                    {
                        // Remember the position of this event in the summary list.
                        _eventIndex = Summary.Events.Count - 1;
                        AnalyzeInsertEvent(even, outputEvent);
                        break;
                    }
                    case EventType.SELECTION:
                    {
                        // Remember the position of this event in the summary list.
                        _eventIndex = Summary.Events.Count - 1;
                        AnalyzeSelectionEvent(even, outputEvent);
                        break;
                    }
                    case EventType.KEYBOARD:
                    {
                        AnalyzeKeyboardEvent(even, outputEvent);
                        break;
                    }
                    case EventType.FOCUS:
                    {
                        AnalyzeFocusEvent(even, outputEvent);
                        break;
                    }
                    case EventType.MOUSE:
                    {
                        AnalyzeMouseEvent(even, outputEvent);
                        break;
                    }
                    case EventType.DRAGONNS:
                    {
                        AnalyzeDragonEvent(even, outputEvent);
                        break;
                    }
                    // Duplicate keys are removed and replaced by a placeholder indicating 
                    // the number of removed keys with timing information (begin, endtime, 
                    // action and pause)
                    case EventType.PLACEHOLDER:
                    {
                        AnalyzeKeyboardEvent(even, outputEvent);
                        break;
                    }
                }

                outputEvent.CharProduction = _lastDoclength + _deletedChars + _replacedChars;

                // Check for an eyetrackPart... if there is one, add the info to the outputEvent.
                var eyetrackPart = Event.GetFirstEventPart<EyetrackPart>(even);
                if (eyetrackPart != null)
                {
                    AnalyzeEyetrackEvent(even, outputEvent);
                }

                // Handles keys used to form a capital letter
                CheckSpecialKeys(outputEvent, Summary);

                // The EventIndex has the position of some previous insert, replacement 
                // or selection in the summary list. It can be retrieved to add
                // the position and document length of the current event.
                if (_eventIndex > 0 && outputEvent.Position > 0)
                {
                    try
                    {
                        var passedEvent = Summary.Events[_eventIndex];
                        passedEvent.Position = outputEvent.Position;
                        passedEvent.DocLength = outputEvent.DocLength;

                        if (passedEvent.Type.Equals("replacement"))
                        {
                            if (_previousDocLength != outputEvent.DocLength)
                            {
                                var passedEv = eventPauseList[_eventIndex];
                                var passedPart = passedEv.First;
                                var extraChars = 0;
                                foreach (var replacePart in passedPart.Parts.OfType<Replacement>())
                                {
                                    if (replacePart.Length > 0 && replacePart.NewText.Length > 0)
                                    {
                                        extraChars = replacePart.NewText.Length;
                                        var nettoRange = replacePart.Length - extraChars;
                                        var docLengthChange =
                                            Math.Abs(_previousDocLength.Value - outputEvent.DocLength.Value);
                                        if (nettoRange == docLengthChange)
                                        {
                                            _replacedChars += extraChars;
                                        }
                                        else
                                        {
                                            extraChars = 0;
                                        }
                                    }
                                }

                                // We have to add the extra replacement chars to the character count of the events 
                                // that are already written to the Summary since the 'EventIndex'.
                                if (extraChars > 0)
                                {
                                    for (int i = _eventIndex; i < Summary.Events.Count; i++)
                                    {
                                        passedEvent = Summary.Events[i];
                                        passedEvent.CharProduction += extraChars;
                                    }
                                    // The current outputEvent is not yet added to the Summary,
                                    // so we correct it here.
                                    outputEvent.CharProduction += extraChars;
                                }
                            }
                        }
                        _eventIndex = 0;
                    }
                    catch
                    {
                        // Ignore and continue.
                    }
                }

                // The previous event & outputEvent
                // Don't update in case of e.g. DNS or EYETRACK event types.
                // We don't want the pause-time calculation to be altered by these types of events.
                if (IplEventTypes.Contains(even.Type))
                {
                    prevEvent = even;
                    _prevOutput = outputEvent;
                }

                // Adds GeneralAnalysisEvent to the summary - if they have been completed.
                if (!outputEvent.IsCompleted) continue;
                Summary.Events.Add(outputEvent);
                outputEvent = null;
            }

            // If the last event has been processed but the outputEvent has not been closed off yet,
            // we forcibly close off the event and add it to the summary.
            if (outputEvent == null) return Summary;
            outputEvent.IsCompleted = true;
            Summary.Events.Add(outputEvent);

            return Summary;
        }

        /// <summary>
        /// CheckSpecialKeys: removes a superfluous control key used to form a capital letter 
        /// and recalculates the pause and action time.
        /// </summary>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        /// <param name="summary">A list with analyzed output events</param>
        private static void CheckSpecialKeys(GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent,
            GeneralAnalysisSummary summary)
        {
            if (_currentIsComposite)
            {
                if (!_previousComposite && summary.Events.Count > 0)
                {
                    var lastElement = summary.Events.Count - 1;
                    var lastEvent = summary.Events.ElementAt(lastElement);
                    if (lastEvent.StartTime != null && outputEvent.EndTime != null)
                    {
                        // ActionTime is an unsigned long and returns error when result is negative
                        if (outputEvent.EndTime == 0 || outputEvent.EndTime < lastEvent.StartTime)
                        {
                            outputEvent.ActionTime = 0;
                        }
                        else
                        {
                            outputEvent.ActionTime = (ulong) outputEvent.EndTime - (ulong) lastEvent.StartTime;
                        }
                    }
                    outputEvent.PauseTime = lastEvent.PauseTime;
                    // 20130926 EVH The relevant pause location is not that from the control key but from the actual character.
                    // outputEvent.PauseLocation = lastEvent.PauseLocation;
                    summary.Events.RemoveAt(lastElement);
                }
                _currentIsComposite = false;
                _previousComposite = true;
            }
            else
            {
                _previousComposite = false;
            }
        }

        /// <summary>
        /// Adds the eyetrack information to the output event. Call this only after all the other
        /// information has been set, as otherwise this method might overwrite some values.
        /// </summary>
        /// <param name="even">The event that contains the eyetrack information.</param>
        /// <param name="outputEvent">The general analysis output event for the corresponding ipl event.</param>
        protected virtual void AnalyzeEyetrackEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            outputEvent.Eyetrack = Event.GetFirstEventPart<EyetrackPart>(even);

            // If there's no timing information in the outputEvent yet, add it from the eyetrack event.
            var time = outputEvent.Eyetrack.GetFirstSubPart<IPLTimePart>();

            if (!outputEvent.StartTime.HasValue && !outputEvent.EndTime.HasValue)
            {
                ulong eyetrackStartTime = time.InterpretData().StartTimeIplReferenced;
                ulong eyetrackEndTime = time.InterpretData().EndTimeIplReferenced;

                outputEvent.RawStartTime = eyetrackStartTime;
                outputEvent.RawEndTime = eyetrackEndTime;
                outputEvent.StartTime = eyetrackStartTime >= NewStartOffset
                    ? eyetrackStartTime - NewStartOffset
                    : eyetrackStartTime;
                outputEvent.EndTime = eyetrackEndTime >= NewStartOffset
                    ? eyetrackEndTime - NewStartOffset
                    : eyetrackEndTime;
            }

            var gaze = outputEvent.Eyetrack.GetFirstSubPart<GazeEventPart>();

            if (!outputEvent.ActionTime.HasValue)
            {
                outputEvent.ActionTime = (ulong) gaze.InterpretData().GazeEventDuration;
            }

            // If there's no output set, then we just put the Eyetracktype, and it's duration as output.
            if (string.IsNullOrEmpty(outputEvent.Output))
            {
                outputEvent.Output = "[" + gaze.GazeEventType + ": " + gaze.GazeEventDuration + " ms]";
            }

            // Convert the SegmentIPLTime to the SegmentIPLTime - StartOffset
            var segment = outputEvent.Eyetrack.GetFirstSubPart<SegmentPart>();
            if (segment != null)
            {
                segment.InterpretData().SceneSegmentStartIPLReferenced =
                    (segment.InterpretData().SceneSegmentStartIPLReferenced.HasValue
                        ? segment.InterpretData().SceneSegmentStartIPLReferenced - NewStartOffset
                        : null);
                segment.InterpretData().SceneSegmentEndIPLReferenced =
                    (segment.InterpretData().SceneSegmentEndIPLReferenced.HasValue
                        ? segment.InterpretData().SceneSegmentEndIPLReferenced - NewStartOffset
                        : null);
                segment.InterpretData().SegmentStartIPLReferenced =
                    (segment.InterpretData().SegmentStartIPLReferenced.HasValue
                        ? segment.InterpretData().SegmentStartIPLReferenced - NewStartOffset
                        : null);
                segment.InterpretData().SegmentEndIPLReferenced =
                    (segment.InterpretData().SegmentEndIPLReferenced.HasValue
                        ? segment.InterpretData().SegmentEndIPLReferenced - NewStartOffset
                        : null);
            }

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Prepares a replacement event
        /// Shows the replacement with its start and end position.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeReplacementEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            foreach (var replacementPart in even.Parts.OfType<Replacement>())
            {
                outputEvent.ActionTime = 0;
                outputEvent.Output = "[" + replacementPart.Start + ":" + replacementPart.End + "] " +
                                     replacementPart.NewText;
            }

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        ///  Prepares an insert event. 
        ///  Capturing the length of the most recent insert. 
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeInsertEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            // Text Before
            string insertTxtB = string.Empty;
            // text After - not used 
            // string insertTxtA = string.Empty;

            foreach (var insertPart in even.Parts.OfType<Insert>())
            {
                outputEvent.ActionTime = 0;
                //outputEvent.Output = "BEFORE:[" + insertPart.Position + "] " + insertPart.Before + " LENGTH:[" +
                //                     insertPart.Before.Length + "]";
                _insertLength = insertPart.Length;
                insertTxtB = insertPart.Before;
               // insertTxtA = insertPart.After;
            }
            outputEvent.DocLength += _insertLength;

            // We recuperate DocLength in the case of an insert in the previous event and when the actual DocLength 
            // is not reflecting this because only mouse movements followed the insert event. When there is no keyPress
            // there is no update of the DocLength. The actual docLength is updated here.
            if (_prevOutput != null && _prevOutput.DocLength == _lastDoclength && _insertLength > 0)
            {
                _lastDoclength += _insertLength;
                outputEvent.DocLength = _lastDoclength;                
            }
            if (_prevOutput != null) outputEvent.Position = _prevOutput.DocLength + 1;
            outputEvent.Output = "[" + insertTxtB + "]";
            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Prepares a selection event
        /// Returns the first and the last char position of the selection.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeSelectionEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            foreach (var selectionPart in even.Parts.OfType<SelectionChange>())
            {
                outputEvent.ActionTime = 0;
                outputEvent.Output = selectionPart.Start + ":" + selectionPart.End;
            }

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Writes out timing information (start time, end time, action time, start clock, end clock)
        /// to the analysis document. This information is shared by all TimedEventParts.
        /// </summary>
        /// <param name="evenPart">EventPart for which to write out the time information.</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeTimedEventPart(TimedEventPart evenPart,
            GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            outputEvent.RawStartTime = evenPart.StartTime;
            outputEvent.RawEndTime = evenPart.EndTime;

            outputEvent.StartTime = evenPart.StartTime >= NewStartOffset
                ? evenPart.StartTime - NewStartOffset
                : evenPart.StartTime;
            outputEvent.EndTime = evenPart.EndTime >= NewStartOffset
                ? evenPart.EndTime - NewStartOffset
                : evenPart.EndTime;

            if (evenPart.EndTime == 0)
            {
                outputEvent.EndTime = outputEvent.StartTime; // + outputEvent.PauseTime;
            }
            outputEvent.ActionTime = outputEvent.EndTime - outputEvent.StartTime;
        }

        /// <summary>
        /// Composite state of the current key captured.
        /// Analyzes a keyboard event and writes the results to the analysis document.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private void AnalyzeKeyboardEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            foreach (var eventPart in even.Parts)
            {
                // Mind: WINLOG is 'KeyPress' with a capital 'P'
                if (eventPart is KeyPress part)
                {
                    _keyLogPart = part;
                    if (outputEvent.Type.Equals("placeholder"))
                    {
                        outputEvent.Output = "[" + EventToString.KeyPressToString(_keyLogPart, _controlKeys)
                                             + " #" + even.Properties["counter"] + "]";
                    }
                    else
                    {
                        outputEvent.Output = EventToString.KeyPressToString(_keyLogPart, _controlKeys);
                    }
                    _currentIsComposite = EventToString.IsComposite;
                    AnalyzeTimedEventPart(_keyLogPart, outputEvent);
                }
                else
                {
                    // Mind: WORDLOG is 'Keypress' with a small 'p'!
                    if (!(eventPart is Keypress keypress)) continue;
                    var wordLogPart = keypress;
                    outputEvent.Position = wordLogPart.Position;
                    outputEvent.DocLength = wordLogPart.DocumentLength;

                    // Keeping track of characters that might have been deleted.
                    if (_lastDoclength > outputEvent.DocLength)
                    {
                        _deletedChars += (int) (_lastDoclength - outputEvent.DocLength);
                    }
                    _lastPosition = (int) outputEvent.Position;
                    _lastDoclength = (int) outputEvent.DocLength;
                }
            }

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Analyzes a mouse event and writes the results to the analysis document.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeMouseEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            foreach (var winlogPart in even.Parts.OfType<AbstractMouseEvent>())
            {
                outputEvent.Output = EventToString.MouseEventToString(winlogPart);
                outputEvent.X = winlogPart.X;
                outputEvent.Y = winlogPart.Y;
                AnalyzeTimedEventPart(winlogPart, outputEvent);
            }

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Analyzes a Focus event and writes the results to the analysis document.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <param name="outputEvent">The output to the summary with additional information</param>
        private static void AnalyzeFocusEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            var eventPart = Event.GetFirstEventPart<FocusChange>(even);
            if (eventPart == null) return;
            outputEvent.Output = eventPart.WindowTitle;
            AnalyzeTimedEventPart(eventPart, outputEvent);

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        private void AnalyzeDragonEvent(Event even, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
        {
            var eventPart = Event.GetFirstEventPart<DragonNSPart>(even);
            if (eventPart == null) return;
            outputEvent.Output = eventPart.Text;
            _lastPosition += eventPart.Text.Length;
            _lastDoclength += eventPart.Text.Length;
            outputEvent.DocLength = _lastDoclength;
            outputEvent.Position = _lastPosition;
            outputEvent.CharProduction = _lastDoclength + _deletedChars + _replacedChars;
            outputEvent.Resource = eventPart.WavePath;
            AnalyzeTimedEventPart(eventPart, outputEvent);

            // Complete the outputEvent
            outputEvent.IsCompleted = true;
        }

        /// <summary>
        /// Gives the fixed number interval for this event.
        /// The events are put into a fixed number of slots, eg 10.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <returns>The interval slot for this event</returns>
        private int GetFixedNumberInterval(Event even)
        {
            if (null == Event.GetFirstEventPart<TimedEventPart>(even)) return FixedNumberInterval;
            ulong startTime = Event.GetFirstEventPart<TimedEventPart>(even).StartTime;
            if (NewStartOffset > startTime)
            {
                return FixedNumberInterval;
            }
            var interval = (startTime - NewStartOffset)/((double) _lastEndTime/NumberOfIntervals);
            return FixedNumberInterval = (int) Math.Ceiling(interval);
        }

        /// <summary>
        /// Returns the the fixed size interval for this event.
        /// The events are put into slots of a fixed size, eg. 1 min.
        /// </summary>
        /// <param name="even">The first part of an event pair (the logged event proper)</param>
        /// <returns>The interval slot for this event</returns>
        private int GetFixedSizeInterval(Event even)
        {
            if (null == Event.GetFirstEventPart<TimedEventPart>(even)) return FixedSizeInterval;
            ulong startTime = Event.GetFirstEventPart<TimedEventPart>(even).StartTime;
            if (NewStartOffset > startTime)
            {
                return FixedSizeInterval;
            }
            if (startTime - NewStartOffset <= _firstStartTime + IntervalSize*(ulong) FixedSizeInterval)
            {
                return FixedSizeInterval;
            }
            return FixedSizeInterval++;
        }
    }
}