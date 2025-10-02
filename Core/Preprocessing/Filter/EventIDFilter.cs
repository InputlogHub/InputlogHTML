using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Preprocessing.Filter
{
    /// <summary>
    /// Event filter based on the ID of the event. It allows to change the starting and stopping moments of an analysis.
    /// </summary>
    public class EventIDFilter : EventFilter
    {
        #region Fields

        /// <summary>
        /// Whether to keep matched events (true), or remove (false).
        /// </summary>
        private const bool KEEP = true;

        /// <summary>
        /// 'True' if an EventValue is defined.
        /// </summary>
        private readonly bool _hasFilterValue;
        
        /// <summary>
        /// Starting and ending parameters.
        /// </summary>
        private int NewStartID { get; set; }
        private int NewStopID { get; set; }
        private bool ResetTime { get; }
        private string EventValue { get; }
        private TimedEventPart CurrentTimedPart { get; set; }
        // The last TimedEventPart before the ID cutoff.
        private TimedEventPart LastTimedPart { get; set; }
        // The first TimedEventPart after the ID cutoff.
        private TimedEventPart FirstTimedPart { get; set; }

        /// <summary>
        /// Author identification
        /// </summary>
        public static string Author { get; private set; }
        public static string IdfxID { get; private set; }

        /// <summary>
        /// Offset to in case of a time reset.
        /// </summary>
        private ulong Offset { get; set; }

        /// <summary>
        /// Bool to track whether the offset has already been set or not.
        /// </summary>
        private bool OffsetSet { get; set; }

        /// <summary>
        /// Inserting a focus event prior to the start id cutoff.
        /// </summary>
        private FocusChange _focusStart;
        private Event _insertFocus;
        private readonly ArrayList _filterEntry;

        /// <summary>
        /// Event filters
        /// </summary>
        private bool IsFixedStartFilter { get; }
        private bool IsFixedEndFilter { get; }
        private EventTypeFilter FocusTypeFilter { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventIds">List with params to manipulate events.</param>
        public EventIDFilter(IList<string> eventIds)
        {
            PreprocessorName = eventIds[0];
            if (!eventIds[1].Equals("fix"))
            {
                NewStartID = Convert.ToInt32(eventIds[1]);
                IsFixedStartFilter = false;
            }
            else
            {
                IsFixedStartFilter = true;
            }
            if (!eventIds[2].Equals("fix"))
            {
                NewStopID = Convert.ToInt32(eventIds[2]);
                IsFixedEndFilter = false;
            }
            else
            {
                IsFixedEndFilter = true;
            }

            ResetTime = Convert.ToBoolean(eventIds[3]);
            EventValue = eventIds[4];
            if (!EventValue.IsNullOrEmpty())
            {
                _hasFilterValue = true;
            }
            Author = eventIds[5];
            IdfxID = eventIds[6];
            OffsetSet = false;
            _focusStart = new FocusChange();
            // The time at the filter event. If no filter event was found
            // it is the time of the last event with timed information,
            // i.e. equal to the time in _filterEntry[2].
            // The last timed information found in the event list.
            _filterEntry = new ArrayList {IdfxID, 0ul, 0ul};
        }

        /// <summary>
        /// Removes events with an ID smaller than the NewStartID or greater than the NewEndID.
        /// </summary>
        /// <param name="even">The event to check</param>
        /// <returns>Boolean whether to remove (false) or keep (true) this event</returns>
        public override bool Check(Event even)
        {
            var thisID = Convert.ToInt32(even.Properties["id"]);
            CurrentTimedPart = Event.GetFirstEventPart<TimedEventPart>(even);

            // In the initial case we get the new offset.
            if (NewStartID == thisID && !OffsetSet)
            {
                if (CurrentTimedPart != null)
                {
                    Offset = CurrentTimedPart.StartTime;
                    OffsetSet = true;
                }
                // If the event at the start has no TimedEventPart (e.g. a Replacement),
                // we use the time from the last event having a TimedEventPart before the cutoff point.
                else if (LastTimedPart != null)
                {
                    Offset = LastTimedPart.StartTime;
                    OffsetSet = true;
                }
            }

            if (NewStartID == thisID && thisID != 0 && _focusStart != null)
            {
                if (ResetTime)
                {
                    _focusStart.StartTime = _focusStart.EndTime = 0;
                }
                else
                {
                    if (CurrentTimedPart != null)
                    {
                        _focusStart.StartTime = _focusStart.EndTime = CurrentTimedPart.StartTime;
                    }
                }
                if (_focusStart.WindowTitle.IsNullOrEmpty())
                {
                    _focusStart.WindowTitle = "UNKNOWN";
                }

                _insertFocus = new Event { Type = "focus" };
                _insertFocus.Parts.Add(_focusStart);
                _insertFocus.ChangeId(NewStartID - 1);
            }

            // All following events will have their times updated.
            if (thisID >= NewStartID && ResetTime)
            {
                // Adjusting the Offset in case the very first TimedEventPart is larger than the Offset.
                // This can happen when the LastTimedPart was used for the Offset.
                if (FirstTimedPart == null && CurrentTimedPart != null)
                {
                    FirstTimedPart = CurrentTimedPart;
                    if(FirstTimedPart.StartTime > Offset)
                        Offset = FirstTimedPart.StartTime;
                }

                if (CurrentTimedPart != null)
                {
                    if (CurrentTimedPart.StartTime >= Offset)
                    {
                        CurrentTimedPart.StartTime -= Offset;
                    }
                   
                    if (CurrentTimedPart.EndTime < Offset)
                    {
                        CurrentTimedPart.EndTime = CurrentTimedPart.StartTime;
                    }
                    else
                    {
                        CurrentTimedPart.EndTime -= Offset;
                    }
                }
            }

            if (thisID < NewStartID)
            {
                // Looking for the last "focus" event before the NewStartID.
                if (even.Type == "focus")
                {
                    _focusStart = even.Parts.OfType<FocusChange>().Single();
                }
                // Preserving the last TimedEventPart before the cutoff point. 
                if (CurrentTimedPart != null) LastTimedPart = CurrentTimedPart;

                return !KEEP;
            }
            // Keeping 'authorcomment" appearing as an event after the statistics, if it exists.
            if (thisID > NewStopID && (!even.Properties["type"].Equals("statistics") || !even.Properties["type"].Equals("authorcomment")))
            {
                return !KEEP;
            }
            return KEEP;
        }

        /// <summary>
        /// Processes the eventList.
        /// </summary>
        /// <param name="inputEvents">Events to process</param>
        /// <param name="sessionId">SessionIdentification data such as the name of the main document.</param>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)
        {
            // If only focus events are needed.
            FocusTypeFilter = new EventTypeFilter(new[] { "focus" });

            if (IsFixedStartFilter)
            {
                NewStartID = Event.SkipToFirstKeypress(inputEvents).First;
            }
            if(IsFixedEndFilter)
            {
                NewStopID = SkipToLastKeypress(inputEvents, sessionId).First;
            }

            int i = 0;
            var list = new List<Event>(inputEvents);

            while (i < list.Count)
            {
                if (Check(list[i]))
                {
                    if (_insertFocus != null)
                    {
                        list.Insert(i, _insertFocus);
                        _insertFocus = null;
                        _focusStart = null;
                    }
                    i++;
                }
                else
                {
                    list.RemoveAt(i);
                }
            }
            Offset = 0;
            OffsetSet = false;
            return list;
        }

        /// <summary>
        /// Iterates backwards over the event list locating the last keyboard event
        /// produced in the main document. If the mainDoc is unknown at this point, an 
        /// attempt is made to find one in the event list.
        /// </summary>
        /// <param name="events">The events logged in this session.</param>
        /// <param name="sessionId">Meta information concerning the file in view</param>
        /// <returns>Returns the id and the starting time of the last keypress in the main document</returns>
        private Pair<int, ulong> SkipToLastKeypress(List<Event> events, SessionIdentification sessionId)
        {
            //The name of the main document, it can be an empty string.
            string doc = sessionId.GetMainDocument();
            //Filters a copy of the event list on focus events.
            var sourceEvents = new List<Event>(events);
            sourceEvents.Filter(FocusTypeFilter);
            int count = sourceEvents.Count < 6 ? sourceEvents.Count : 5;
            // If there isn't a mainDoc in the SessionIdentification, we have to guess.
            var docName = doc.IsNullOrEmpty() ? GetMainDoc(sourceEvents, count) : doc.ToLower();
            // Stripping any extension
            var idx = docName.LastIndexOf('.');
            var mainDoc = docName.Substring(0, idx);
            // Character count
            var charCount = 0;

            Pair<int, ulong> lastKeyPress = null;

            for (int i = events.Count - 1; i >= 0; i--)
            {
                var e = events[i];

                string windowTitle = e.GetWindowTitle(e).ToLower();

                // Any last timed information.
                if (Convert.ToUInt64(_filterEntry[2]) <= 0ul)
                {
                    var t = Event.GetFirstEventPart<TimedEventPart>(e);
                    if (t != null)
                    {
                        _filterEntry[1] = t.EndTime;
                        _filterEntry[2] = t.EndTime;
                    }
                }

                // The last timed keyboard event.
                if (lastKeyPress == null && EventType.KEYBOARD.Equals(e.Type))
                {
                    var t = Event.GetFirstEventPart<TimedEventPart>(e);
                    lastKeyPress = new Pair<int, ulong>(e.GetId(), t.StartTime);
                }

                // We count the keyPresses and consider only the last main document focus
                // if it contains characters
                if (!e.GetKeypressValue().Equals(string.Empty))
                {
                    charCount++;
                }
                // When the last main document focus is found by going backwards, 
                // we need to extract the last keyboard event inside this focus
                // by going forward again.
                if (windowTitle.Contains(mainDoc) && charCount > 1)
                {
                    for (int j = i + 1; j < events.Count; j++)
                    {
                        var ev = events[j];

                        // Looking for a keyboard event as long 
                        // as no new window title is found...
                        if (ev.GetWindowTitle(ev).IsNullOrEmpty())
                        {
                            var tmpKeyPress = ev.GetStartKeyboardEventID(ev);
                            if (tmpKeyPress != null)
                            {              
                                // We look for the last keyboard event as defined by its value (if any).         
                                if (_hasFilterValue)
                                {
                                    if (ev.GetKeypressValue().Equals(EventValue))
                                    {
                                        lastKeyPress = tmpKeyPress;
                                        _filterEntry[1] = lastKeyPress.Second;
                                    }
                                }
                                else
                                {
                                    lastKeyPress = tmpKeyPress;
                                    _filterEntry[1] = lastKeyPress.Second;
                                }                               
                            }
                        }
                    }

                    break;
                }
            }
            return lastKeyPress;
        }

        /// <summary>
        /// When IsFixedEndFilter = true, a _filterEntry is filled.
        /// _filterEntry[0] contains the IdfxID;
        /// _filterEntry[1] contains the time at the filter event. If no filter event was found, 
        /// the entry has the time of the last event with timed information, i.e. it's equal to _filterEntry[2].
        /// _filterEntry[2] contains the very last timed information found in the event list.
        /// </summary>
        /// <returns>ArrayList with three entries: a string and two ulong.</returns>
        public override ArrayList GetFilterResult()
        {
            return _filterEntry;
        }

        /// <summary>
        /// When the session identification doesn't have the name of the main document,
        /// we assume that it is the first focus event with a window title that is 
        /// not 'TASKBAR', 'Inputlog', a pdf or a browser.
        /// </summary>
        /// <param name="events">Focus events logged in this session.</param>
        /// <param name="count"></param>
        /// <returns>The window title of the main document.</returns>
        private string GetMainDoc(List<Event> events, int count)
        {
            List<string> titleList = new List<string>();
           
            foreach (Event e in events)
            {
                string mainTitle = e.FindMainTitle(e.GetWindowTitle(e), titleList, count);
                if (!mainTitle.Equals("UNKNOWN"))
                {
                    return mainTitle;
                }
            }
            return "UNKNOWN";
        }
    }
}