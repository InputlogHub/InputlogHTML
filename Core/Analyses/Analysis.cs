using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses
{
    public abstract class Analysis
    {
        #region Fields

        /// <summary>
        /// A coordinate - threshold that indicates when a double click will no longer be recognized as a double click.
        /// That is, if a double click is recognized, but if the difference between x or y coordinates of the different clicks
        /// is larger than this constant, the consecutive clicks will no longer be recognized as a double click.
        /// </summary>
        private const int DOUBLE_CLICK_COORDINATE_DIFFERENCE_THRESHOLD = 10;

        /// <summary>
        /// List of filters that are applied on the input events before the event list is analyzed. 
        /// We keep this list purely for referential purposes, as the filters itself are directly 
        /// applied in the constructor.
        /// </summary>
        protected List<EventFilter> EventFilters;

        /// <summary>
        /// List of input events on which to perform the analysis.
        /// </summary>
        protected List<Event> InputEvents;

        /// <summary>
        /// Session identification for the list of events.
        /// </summary>
        protected readonly SessionIdentification SessionIdentification;

        /// <summary>
        /// Laps of time to subtract from the log start
        /// </summary>
        public static ulong NewStartOffset { get; set; }

        /// <summary>
        /// The main document of this analysis
        /// </summary>
        public string MainDocument { get; private set; }
  

        ///// <summary>
        ///// The short name of the analysis (abbreviation) 
        ///// </summary>
        public string Abbrv;

        /// <summary>
        /// Remembers the last meaningful current and previous non zero start time, 
        /// during pause time calculation.
        /// </summary>
        // private ulong LastCurrent;
        protected ulong LastCurrent;
        protected ulong LastPrevious;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="abbrv">The short name of the analysis (abbreviation). Can be null </param>
        /// <param name="events">The events to be processed in the analysis</param>
        /// <param name="sessionID">The session identification for the list of events.</param>
        protected Analysis(string abbrv, List<Event> events, SessionIdentification sessionID)
        {
            Abbrv = abbrv;
            InputEvents = events;
            SessionIdentification = sessionID;
            NewStartOffset = SetTimeOffset();  
            MainDocument = SessionIdentification.GetMainDocument();
        }

        /// <summary>
        /// Startoffset according to the session meta info. This is the time elapsed in millisecs from
        /// the internal clock since the system has started. It can be null if no such information is available.
        /// </summary>
        /// <returns>The offset (ulong) to be subtracted to get the real start time of inputlog.</returns>
        private ulong SetTimeOffset()
        {
            ulong startTimeOffset; //= SessionIdentification.HasRelativeCreationTime() ? SessionIdentification.GetRelativeCreationTime() : 0;

            // If the first event has a startTime == 0, where first_event.startTime != startTimeOffset, this means
            // that the start time of the list of events has been reset, and the startTimeOffset should be == 0.
            var firstTimedEvent = FindFirstTimedEvent();
            if (firstTimedEvent != null && firstTimedEvent.StartTime == 0)
            {
                startTimeOffset = 0;
            }
            else
            {
                startTimeOffset = firstTimedEvent.StartTime;
            }
            return startTimeOffset;
        }

        /// <summary>
        /// Finds the first event in the list that has timing information.
        /// </summary>
        /// <returns>Returns the timing information of the first event with such information.</returns>
        protected TimedEventPart FindFirstTimedEvent()
        {
            // Going over events from first to last.
            return (from e in InputEvents 
                    select e.Parts.OfType<TimedEventPart>() 
                    into timing 
                    select timing as TimedEventPart[] ?? timing.ToArray() 
                    into timedEventParts 
                    where timedEventParts.Any() 
                    select timedEventParts.First()).FirstOrDefault();
        }

        /// <summary>
        /// Returns the last event from the event list that has a timeEvent part 
        /// It checks whether the returned TimedEvent is larger than the following parts.
        /// This is what one would expect, but cases exists where the last mouse or focus events have
        /// smaller time stamps than the preceding keyboard events
        /// </summary>
        /// <returns></returns>
        protected TimedEventPart GetLastTimedEvent()
        {
            TimedEventPart firstLastTimedEventPart = null;
            ulong keyboardEndTime = 0;
            ulong lastEndTime = 0;

            int i = InputEvents.Count - 1;
            while (i >= 0)
            {
                Event currEvent = InputEvents[i];
                var lastTimedEventPart = Event.GetFirstEventPart<TimedEventPart>(currEvent);
                if (lastTimedEventPart != null && lastTimedEventPart.EndTime == 0)
                {
                    lastTimedEventPart = null;
                }

                if (currEvent.Type.Equals(EventType.KEYBOARD) && lastTimedEventPart != null)
                {
                    keyboardEndTime = lastTimedEventPart.EndTime;
                }
                else
                {
                    if (firstLastTimedEventPart == null && lastTimedEventPart != null)
                    {
                        firstLastTimedEventPart = lastTimedEventPart;
                        lastEndTime = firstLastTimedEventPart.EndTime;
                    }
                }

                if (keyboardEndTime > lastEndTime)
                {
                    return lastTimedEventPart;
                }
                if (keyboardEndTime > 0 && lastEndTime > 0)
                {
                    return firstLastTimedEventPart;
                }
                i--;
            }
            return null;
        }

        /// <summary>
        /// Performs the actual Analysis on the events with which this Analysis was constructed.
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public abstract IAnalysisSummary DoAnalysis();

        /// <summary>
        /// A pause time is the time in millsecs elapsed between the start time of an event (e.g. key down)
        /// and the start time of the next event (e.g. key down).
        /// 20150611 refactored by E.
        /// </summary>
        /// <param name="prevEvent">the previous event</param>
        /// <param name="currEvent">the current event</param>
        /// <returns></returns>
        protected ulong DeterminePauseTime(Event prevEvent, Event currEvent)
        {
            // Preliminary check. Return 0 if both events are null or 
            // throw an exception when the current event is null.
            if (currEvent == null)
            {
                if (prevEvent == null)
                {
                    return 0;
                }
                throw new AnalysisException("Current event shouldn't be null. Previous event ID: " + prevEvent.GetId());
            }

            // First, getting the TimedEventPart and start time of the current event for event
            // types KEYBOARD, MOUSE, PLACEHOLDER, DRAGONNS. The other types FOCUS, REPLACEMENT, 
            // SELECTION are skipped and getting a zero pause time.
            var timedEventPartCurrent = Event.GetFirstEventPart<TimedEventPart>(currEvent);
            if (timedEventPartCurrent != null)
            {
                switch (currEvent.Type)
                {
                    case EventType.KEYBOARD:
                    case EventType.MOUSE:
                    case EventType.PLACEHOLDER:
                    case EventType.DRAGONNS:
                        if (timedEventPartCurrent.StartTime > 0)
                        {
                            LastCurrent = timedEventPartCurrent.StartTime;
                        }
                        break;
                    default:
                        return 0;
                }
            }

            // If PrevEvent == null we are at the beginning of the eventList and
            // the pauseTime there is the startTime - startOffset.         
            if (prevEvent == null)
            {
                // Special case. When a Time Filter removes events at the start of the list,
                // a Focus event is added up front, it's getting the StartTime of the
                // first real event and a zero PauseTime.
                if (currEvent.GetId() > 0 && currEvent.Type.Equals(EventType.FOCUS))
                {
                    return 0;
                }

                // LastCurrent >= NewStartOffset: ulong should never be negative.
                if (LastCurrent >= NewStartOffset)
                {
                    return LastCurrent - NewStartOffset;
                }
                return LastCurrent;
            }

            // Secondly, we take the TimedEventPart and start time of the previous event for certain types.
            // If the EventType is MOUSE or DRAGONNS we use the end time.
            // REPLACEMENT, SELECTION are skipped and getting a zero pause time.
            var timedEventPartPrev = Event.GetFirstEventPart<TimedEventPart>(prevEvent);
            if (timedEventPartPrev != null)
            {
                switch (prevEvent.Type)
                {
                    case EventType.KEYBOARD:
                    case EventType.PLACEHOLDER:
                    case EventType.FOCUS:
                       if (timedEventPartPrev.StartTime > 0)
                       {
                           LastPrevious = timedEventPartPrev.StartTime;
                       }
                       break;
                    case EventType.MOUSE:
                    case EventType.DRAGONNS:
                        if (timedEventPartPrev.EndTime > 0)
                        {
                            LastPrevious = timedEventPartPrev.EndTime;
                        }
                        break;
                    default:
                        return 0;
                }
            }

            // 20140316 E. It happens with SHIFT or CLICK that the previous event is logged after
            // the startTime of the current although it was pressed earlier (hence the '>' check below).
            // In such a case we take the absolute diff between previous and current rather than returning a zero.
            if (LastPrevious > LastCurrent)
            {
                //Debug
                //Console.WriteLine("Current < Previous - ID: " + currEvent.GetId());
                return LastPrevious - LastCurrent;
            }

            // The pause time for the felicitous general case.
            return LastCurrent - LastPrevious;
        }

        public static Predicate<Pair<Event, PauseLocation>> DropEventPauseTypeFilter(IEnumerable<string> eventTypes)
        {
            return pair => (!eventTypes.Contains(pair.First.Type));
        }

        protected static Predicate<Pair<Event, PauseLocation>> EventPauseTypeFilter(IEnumerable<string> eventTypes)
        {
            return pair => (eventTypes.Contains(pair.First.Type));
        }

        /// <summary>
        /// Rationale: Old logging files may have incorrect positions logged when users hold
        /// down the backspace to delete a series of characters.
        /// Effect: Attempts to arbitrarily detect these sequences and automatically fix the
        /// positions during the holding down of a backspace event.
        /// </summary>
        /// <param name="events">The list of events</param>
        /// <param name="versionNumber">The versionNumber of the idfx that logged the events.</param>
        /// <returns>Returns true if an invalid backspace sequence was detected and/or restored, 
        /// false if no such sequence was detected</returns>
        public static bool PreprocessEvents(IEnumerable<Event> events, int versionNumber)
        {
            // This was somewhere during or before this version, so this is a safe margin.
            if (versionNumber > 50126)
            {
                return false;
            }

            // Variables needed
            bool previousEventWasBackspace = false;
            int previousDocLength = -1;
            int previousPosition = -1;
            bool detectedBackspaceSequence = false;

            // Cycle over all events.
            foreach (Event e in events)
            {
                // Only for keyboard events.
                if (e.Type == EventType.KEYBOARD)
                {
                    KeyPress winLog = e.Parts.OfType<KeyPress>().FirstOrDefault();
                    Keypress wordLog = e.Parts.OfType<Keypress>().FirstOrDefault();

                    // Only in Word && with a winLog part, obviously.
                    if (winLog != null && wordLog != null)
                    {
                        // If this key is an actual backspace without any fancy stuff. (CTRL+Backspace, etc...)
                        if (winLog.Key == KeysEx.VK_BACK && wordLog.IncludeInReplay)
                        {
                            // 1. The previous event was not a backspace:
                            // > This is the first backspace of a possible sequence, we 
                            // > Save the position and DocumentLength
                            if (!previousEventWasBackspace)
                            {
                                previousPosition = wordLog.Position;
                                previousDocLength = wordLog.DocumentLength;
                            }
                                // 2. This is a backspace in a sequence
                                // > Set it's position to previousPosition-1
                                // > Set documentLength to previousLength-1
                            else
                            {
                                detectedBackspaceSequence = true;

                                // Update current event with new position/length
                                int wordLogIndex = e.Parts.IndexOf(wordLog);
                                ((Keypress) e.Parts[wordLogIndex]).DocumentLength = --previousDocLength;
                                ((Keypress) e.Parts[wordLogIndex]).Position = --previousPosition;
                            }
                            previousEventWasBackspace = true;
                        }
                        else
                        {
                            previousEventWasBackspace = false;
                        }
                    }
                    else
                    {
                        previousEventWasBackspace = false;
                    }
                }
                else
                {
                    previousEventWasBackspace = false;
                }
            }
            return detectedBackspaceSequence;
        }

        #region DuplicateRemoval

        /// <summary>
        /// Recognizes Double Clicks in a list of events.
        /// This is done by iterating over the given event list and searching for a double click. When a double click is found, 
        /// the last of the 2 click events is replaced by a new pseudo event; 
        /// the DoubleClick event (pseudo since this event didn't actually occur, but is a preprocessing step for later analysis).
        /// A "double click" is defined as:
        /// 2 consecutive mouse click events of the same mouse button which have a paustime that is less than the given threshold
        /// and occur at more or less the same point on the screen (a small deviation is allowed).
        /// </summary>
        /// <param name="events">The list of events in which to find double clicks.</param>
        /// <param name="threshold">The double click pause threshold (max time that can occur for consecutive clicks 
        /// to be recognized as a double click).</param>
        protected void RecognizeDoubleClicks(List<Event> events, ulong threshold)
        {
            Event prevClickEvent = null;
            int prevClickLocation = -1;
            for (int i = 0; i < events.Count; i++)
            {
                Event even = events[i];
                if (even.Type.Equals(EventType.MOUSE))
                {
                    var click = Event.GetFirstEventPart<Click>(even);
                    if (click != null)
                    {
                        if (prevClickEvent != null)
                        {
                            var prevClick = Event.GetFirstEventPart<Click>(prevClickEvent);
                            ulong pauseTime = DeterminePauseTime(prevClickEvent, even);
                            // A double click are 2 consecutive mouse click events of the same mouse button
                            // which have a paustime that is less than a given theshold, and occur at more or less the same
                            // point on the screen (a small deviation is allowed).
                            if (prevClick.Button == click.Button && pauseTime <= threshold)
                            {
                                // found Double click
                                if (Math.Abs(click.X - prevClick.X) <= DOUBLE_CLICK_COORDINATE_DIFFERENCE_THRESHOLD
                                    &&
                                    Math.Abs(click.Y - prevClick.Y) <= DOUBLE_CLICK_COORDINATE_DIFFERENCE_THRESHOLD)
                                {
                                    // create new 'fake' double click event 
                                    var doubleClickEvent = new Event {Type = EventType.MOUSE};
                                    // CHANGED TOM: 16-12-2011: Store doubleClickEvent ID as well!
                                    // Set ID to the ID of the first click in the doubleClick
                                    doubleClickEvent.Properties["id"] = prevClickEvent.Properties["id"];
                                    doubleClickEvent.Parts.Add(new DoubleClick(prevClick, click));
                                    events[i] = doubleClickEvent;
                                    events.RemoveAt(prevClickLocation);
                                    i--;
                                }
                            }
                        }
                        prevClickEvent = even;
                        prevClickLocation = i;
                    }
                }
                else
                {
                    // Event was not a mouse event, set prevClickEvent to null
                    // make an exception for focus events:
                    // if you change window by double clicking on that window, windows will first register a single click, then
                    // register the focus events for the window on which was clicked, and then register the next click.
                    // This prevents us from recognizing the double click. This is fixed by ignoring focus events.
                    if (!even.Type.Equals(EventType.FOCUS))
                    {
                        prevClickEvent = null;
                        prevClickLocation = -1;
                    }
                }
            }
			Reset();
        }

		/// <summary>
		/// This method resets the the Analysis to its original state, where needed
		/// </summary>
		private void Reset()
		{
		    LastCurrent = 0;
		    LastPrevious = 0;
		}

        /// <summary>
        /// Removes duplicate controlkeys from the eventlist.
        /// Duplicate control keys are those keys for which windows has generated multiple keydown events but
        /// only a single keyup event. E.g. when LSHIFT is pressed for a longer time, windows will generate multiple 
        /// keydown events, but only a single keyup event.
        /// Our core lib already notices this and sets the endTime of the those duplicate keys to 0.
        /// This method will search2 the list of inputevents and delete any controlkey for which the endTime is set to 0.
        /// By doing this, only the last keystroke of the duplicates is left. This method also edits the startTime 
        /// of that last event so that its startTime is set to the startTime of the first duplicate key in the range.
        /// Example:
        /// input:
        /// -----------------------------
        /// event   startTime   endTime
        /// -----------------------------
        /// LSHIFT  1234        0
        /// LSHIFT  1254        0
        /// LSHIFT  1274        0
        /// LSHIFT  1294        1314
        /// 
        /// output:
        /// LSHIFT  1234        1314
        /// </summary>
        /// <param name="eventList">List of events from which to remove the duplicate control keys.</param>
        /// <param name="controlKeys">List of keys that are considered to be control keys 
        /// (duplicates of these keys in the eventlist will be removed).</param>
        protected static void RemoveDuplicateControlKeys(List<Event> eventList, List<KeysEx> controlKeys)
        {
            // A buffer is used to store the positions of the duplicate events.
            // We don't really need this, since the positions of the duplicate events will always follow each other
            // however, using a buffer to store all duplicates makes the code slightly simpler and is helpfull when debugging
            // (you can print out the current buffer).
            var duplicateBuff = new List<int>();
            var currentKey = KeysEx.NONE;
            var currentTime = (ulong) 0;
            // iterate over all events
            for (int i = 0; i < eventList.Count; i++)
            {
                Event even = eventList[i];
                // if the event is a keyboard event, search2 for the keypress part
                var keyPress = Event.GetFirstEventPart<KeyPress>(even);
                if (keyPress != null)
                {
                    //DEBUG: Console.WriteLine("keyPress:" + keyPress.Key + "(i:" + i + ")");
                    if (keyPress.Key == currentKey)
                    {
                        if (controlKeys.Contains(keyPress.Key))
                        {
                            if (keyPress.EndTime == currentTime)
                            {
                                //Console.WriteLine("Adding " + keyPress.Key + " to the buffer.");
                                // the key of this keypress is the same as the current duplicate key, 
                                // add the index of this event to the list of duplicate events
                                duplicateBuff.Add(i);
                            }
                            // 20111124 EVH  Ticket #18 (duplicate control keys appeared anyway) resolved
                            //i -= CompactEventRangeHelper(duplicateBuff, eventList);
                        }
                    }
                    else
                    {
                        // the key of this keypress is different from the current duplicate key.
                        // compact the events that are in the current event buffer make the current key the new duplicate key.
                        i -= CompactEventRangeHelper(duplicateBuff, eventList);
                        currentKey = keyPress.Key;
                        currentTime = keyPress.EndTime;
                        duplicateBuff.Add(i);
                    }
                }
                else
                {
                    // other event than keypress event (e.g. a mouse event), compact the current buffer
                    i -= CompactEventRangeHelper(duplicateBuff, eventList);
                }
            }
            // iterated over all events, compact any events still in the buffer 
            // (needed when the last the duplicate events occur at the end of the logfile).
            CompactEventRangeHelper(duplicateBuff, eventList);
        }

        /// <summary>
        /// Helper method for the RemoveDuplicateControlKeys method.
        /// This method just calls the CompactEventRange() method using the duplicates buffer provided 
        /// by the RemoveDuplicateControlKeys() method.
        /// </summary>
        /// <param name="positions">List of Positions of the duplicate events.</param>
        /// <param name="eventList">List of events from which to compact a certain range.</param>
        /// <returns>The number of items that have been compacted (= the same as count) or 0 if count small/equal to 1</returns>
        private static int CompactEventRangeHelper(ICollection<int> positions, List<Event> eventList)
        {
            if (positions.Any())
            {
                int returnVal = CompactEventRange(positions.First(), positions.Count(), eventList);
                positions.Clear();
                return returnVal;
            }
            positions.Clear();
            return 0;
        }

        /// <summary>
        /// Helper method for the RemoveDuplicateControlKeys method.
        /// Compacts a part of a given list of events.
        /// This is done by first removing the range of events from the list 
        /// (the range is determined by a given startposition and the size of the range.)
        /// Furthermore, the startTime of the event that follows the last event that is part of the deleted range, 
        /// is set to the start time of the first event of the range. In other words, this method does the actual 
        /// duplicate-removing. The RemoveDuplicateControlKeys() method determines the duplicate keys and then calls 
        /// this method to do the acual removing.
        /// </summary>
        /// <param name="startPosition">StartPosition of the range that is to be compacted in the eventlist.</param>
        /// <param name="count">Number of events in the range to compact.</param>
        /// <param name="eventList">List of events from which to compact a certain range.</param>
        /// <returns>The number of items that have been compacted (= the same as count) or 0 if count is small/equal to 1</returns>
        private static int CompactEventRange(int startPosition, int count, List<Event> eventList)
        {
            if (count <= 1) return 0;
            // if duplicateBuff contains more than a single element: fix the startTime of last duplicate event 
            // + remove duplicate eventz from events list; set startTime of last duplicate event to startTime of
            // first duplicate event.
            Event placeholder = eventList[startPosition];
            placeholder.Type = "placeholder";
            // The number of duplicate events represented by this placeholder
            placeholder.Properties.Add("counter", count.ToString());
            var eventPart = Event.GetFirstEventPart<TimedEventPart>(placeholder);
            if (eventPart != null)
            {
                var eventPart2 = Event.GetFirstEventPart<TimedEventPart>(eventList[startPosition + count]);
                if (eventPart2 != null) eventPart.EndTime = eventPart2.EndTime;
            }
            // Remove duplicate events from the eventslist.
            // Keep the first as placeholder and diminish the count by one.
            eventList.RemoveRange(startPosition + 1, count - 1);
            return count - 1;
        }

        #endregion
    }
}