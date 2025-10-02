using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;
using System.Reflection;
using InputLog.Core.Analyses.PauseParser;
using System;
using System.IO;
using System.Windows.Forms;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Signature of the callback delegate that can be installed on one of the eventlisteners of the PauseLocationMarker.
    /// </summary>
    /// <param name="sender">Sender of the event (will always be an instance of PauseLocationMarker)</param>
    /// <param name="e">Event Arguments</param>
    public delegate void Callback(object sender, PauseLocationEventArgs e);


    #region Comparison

    public abstract class ACompareEvent : IComparer<Event>
    {
        public abstract int Compare(Event evt1, Event evt2);

        protected static object GetPublicProperty(object obj, string propName)
        {
            if (obj != null)
            {
                PropertyInfo propInfo = obj.GetType().GetProperty(propName);
                if (propInfo != null)
                {
                    return propInfo.GetValue(obj, null);
                }
            }
            return null;
        }

        protected static bool HasPublicProperty(object obj, string propName)
        {
            if (obj != null)
            {
                PropertyInfo propInfo = obj.GetType().GetProperty(propName);
                if (propInfo != null)
                {
                    return true;
                }
            }
            return false;
        }

        protected static object GetPublicField(object obj, string fieldName)
        {
            if (obj != null)
            {
                FieldInfo fieldInfo = obj.GetType().GetField(fieldName);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(obj);
                }
            }
            return null;
        }

        protected static bool HasPublicField(object obj, string fieldName)
        {
            if (obj != null)
            {
                FieldInfo propInfo = obj.GetType().GetField(fieldName);
                if (propInfo != null)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CompareEventTime : ACompareEvent
    {
        public override int Compare(Event evt1, Event evt2)
        {
            if (HasStartTime(evt1) && HasStartTime(evt2))
            {
                if (GetStartTime(evt1) > GetStartTime(evt2))
                {
                    return 1;
                }
                if (GetStartTime(evt1) < GetStartTime(evt2))
                {
                    return -1;
                }
                return 0;
            }
            if (HasEndTime(evt1) && HasEndTime(evt2))
            {
                if (GetEndTime(evt1) > GetEndTime(evt2))
                {
                    return 1;
                }
                if (GetEndTime(evt1) < GetEndTime(evt2))
                {
                    return -1;
                }
                return 0;
            }
            //Give up and say they are equal (i.e. keep same position)
            return 0;
        }

        private static bool HasStartTime(Event evt)
        {
            const string name = "StartTime";
            return HasPublicProperty(evt, name) || HasPublicField(evt, name);
        }

        private static ulong GetStartTime(Event evt)
        {
            const string name = "StartTime";
            if (HasPublicProperty(evt, name))
            {
                return (ulong)GetPublicProperty(evt, name);
            }
            if (HasPublicField(evt, name))
            {
                return (ulong)GetPublicField(evt, name);
            }
            return 0UL;
        }

        private static bool HasEndTime(Event evt)
        {
            const string name = "EndTime";
            return HasPublicProperty(evt, name) || HasPublicField(evt, name);
        }

        private static ulong GetEndTime(Event evt)
        {
            const string name = "EndTime";
            if (HasPublicProperty(evt, name))
            {
                return (ulong)GetPublicProperty(evt, name);
            }
            if (HasPublicField(evt, name))
            {
                return (ulong)GetPublicField(evt, name);
            }
            return 0UL;
        }
    }

    public class CompareEventPosition : CompareEventTime
    {
        public override int Compare(Event evt1, Event evt2)
        {
            if (HasPosition(evt1) && HasPosition(evt2))
            {
                return GetPosition(evt1) - GetPosition(evt2);
            }
            return base.Compare(evt1, evt2);
        }

        private static bool HasPosition(Event evt)
        {
            const string name = "Position";
            return HasPublicProperty(evt, name) || HasPublicField(evt, name);
        }

        private static int GetPosition(Event evt)
        {
            const string name = "Position";
            if (HasPublicProperty(evt, name))
            {
                return (int)GetPublicProperty(evt, name);
            }
            if (HasPublicField(evt, name))
            {
                return (int)GetPublicField(evt, name);
            }
            return -1;
        }
    }

    #endregion

    /// <summary>
    /// Determines the pauselocations in a given list of events.
    /// 
    /// PauseLocations are locations in a text that are have some kind of pattern to which 
    /// a certain type of pause is assigned to.
    /// e.g. the space between 2 words in a text is typically assigned the pauselocation BETWEEN_WORDS,
    /// while the characters in the word itself are (obviously) marked as WITHIN_WORD.
    /// For the different type of pauselocations, have a look at the PauseLocation class.
    /// 
    /// This class basically implements a Finite State Machine according to the rules that 
    /// are defined in Docs/PauseLocationRules.xls.
    /// </summary>
    public class PauseLocationMarker
    {
        #region Fields
        /// <summary>
        /// Current number of events that are matched as a character that might be part of a paragraph.
        /// </summary>
        private uint NbParagraphChars1;

        /// <summary>
        /// Current number of events that are matched as a character that might be part of a sentence.
        /// </summary>
        private uint NbSentenceChars1;

        /// <summary>
        /// The length of the current insert or non-empty replacement.
        /// </summary>
       // public static int InsertLength { get; private set; }

        /// <summary>
        /// Ensures that at the end of the text an "After Paragraphs" marker is placed in the absence of
        /// further keyPresses.
        /// </summary>
        private static bool HasBeforeParagraph { get;  set; }

        /// <summary>
        /// Current number of events that are matched as a character that might be part of a paragraph.
        /// </summary>
        public uint NbParagraphChars
        {
            get { return NbParagraphChars1; }
            set { NbParagraphChars1 = value; }
        }

        /// <summary>
        /// Current number of events that are matched as a character that might be part of a sentence.
        /// </summary>
        public uint NbSentenceChars
        {
            get { return NbSentenceChars1; }
            set { NbSentenceChars1 = value; }
        }

        /// <summary>
        /// Counter of already finished Events
        /// </summary>
        private int EventCounter { get; set; }

        /// <summary>
        /// Eventhandlers
        /// </summary>
        public event Callback EventEventHandler;
        private bool NotifiedWord;
        public event Callback WordEventHandler;
        private bool NotifiedSentence;
        public event Callback SentenceEventHandler;
        private bool NotifiedParagraph;
        public event Callback ParagraphEventHandler;
        private bool NotifiedWordChar;
        public event Callback WordCharEventHandler;
        private bool NotifiedSentenceChar;
        public event Callback SentenceCharEventHandler;
        private bool NotifiedParagraphChar;
        public event Callback ParagraphCharEventHandler;
        public event Callback FocusChangeEventHandler;
        public event Callback InsertEventHandler;

        /// <summary>
        /// List of currently handled events
        /// </summary>
        private List<Event> CurrentEventList;

        /// <summary>
        /// Index of the currently handled event in CurrentEventList
        /// </summary>
        private int CurrentEventIndex;
        private Pair<Event, PauseLocation> CurrentEventPausePair;
        private List<int> PastIndices;

        /// <summary>
        /// Indices of events in the resorted array in the original list
        /// </summary>
        private int[] CurrentReorderIndices;

        /// <summary>
        /// Finite state machines to detect the pause locations
        /// The more at the front an FSM is, the higher its priority.
        /// </summary>
        private readonly List<DeterministicFiniteStateMachine<ISet<string>>> FSMList;

        #region FSMInteractionFields

        /// <summary>
        /// Signal to determine whether the pause location is generated or not
        /// </summary>
        /// <remarks>This is primarily intended for use in the finite state machine itself.</remarks>
        private static bool _pauseLocationReady;

        /// <summary>
        /// The PauseLocation for the current handled Event, as generated by the finite state machine
        /// </summary>
        /// <remarks>This is primarily intended for use in the finite state machine itself.</remarks>
        private static PauseLocation _generatedPauseLocation;

        /// <summary>
        /// FSM which is momentarily transitioning
        /// </summary>
        private static AFiniteStateMachine<ISet<string>> _currentFSM;
        #endregion

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configFile">Configuration files for the finite state machine rules</param>
        public PauseLocationMarker(string configFile = @"\Analyses\PauseParser\FSMRules.txt")
        {
            string fullPath = Path.GetDirectoryName(Application.ExecutablePath) + configFile;
            NbSentenceChars1 = 0;
            NbParagraphChars1 = 0;
            CurrentEventList = null;
            CurrentEventIndex = -1;
            EventCounter = -1;

            using (var configStream = new StreamReader(fullPath))
            {
                var actionLookup = new PauseLocationActionLookup(this);
                var eventLookup = new PauseLocationEventLookup(this);

                AFSMFactory<string> factory = new StringFSMFactory(actionLookup, eventLookup);
                IRuleFactory<string> ruleFactory = new StringRuleFactory();

                var parser = new Parser(new Scanner(configStream.BaseStream), factory, ruleFactory);
                FSMList = parser.GetDeterministicFiniteStateMachines();
                ForeachFSM(fsm => fsm.BuildCache());
            }
        }

        private static int[] Reorder(List<Event> eventList, out List<Event> sortedList)
        {
            //Order in 2 passes
            List<Event> positionOrdered;
            int[] eventSortIndices = Sort.MergeSort(eventList, out positionOrdered, new CompareEventPosition());
            int[] timeSortIndices = Sort.MergeSort(positionOrdered, out sortedList, new CompareEventTime());

            //Fix indexes
            int[] indices = eventSortIndices;//Just initialisation, we'll overwrite it anyway
            for(int i = 0; i < eventSortIndices.Length; i++)
            {
                int timeSortIndex = timeSortIndices[i];
                int eventSortIndex = eventSortIndices[timeSortIndex];
                indices[i] = eventSortIndex;
            }

            return indices;
        }

        private void InsertEventPausePair(IList<Pair<Event, PauseLocation>> eventPauseList, 
            int i, Pair<Event, PauseLocation> eventPausePair)
        {
            eventPauseList[i] = eventPausePair;
            EventCounter++;
        }

        public List<Pair<Event, PauseLocation>> Start(List<Event> events)
        {
            var eventPauseList = new List<Pair<Event, PauseLocation>>(events.Count);
            if (events.Count == 0)
            {
                return eventPauseList; // fail safety: Don't run the algorithm if the list is empty
            }

            CurrentReorderIndices = Reorder(events, out CurrentEventList);

            //Initialise list, making sure all possible indices already exist for (re)inserting the pairs in correct order
            eventPauseList.AddRange(events.Select(t => new Pair<Event, PauseLocation>(t, PauseLocation.UNDETERMINED)));

            //Debug.WriteLine("*Start* ");
            CurrentEventIndex = -1;
            PastIndices = new List<int>();
            CurrentEventPausePair = ReadNextPair(eventPauseList);
            while (CurrentEventPausePair != null)
            {
                Event evt = CurrentEventPausePair.First;
                PauseLocationEventLookup.SetValues(evt);
                ResetNotifications();

                // notify listeners
                NotifyEventHandler(CurrentEventPausePair);
                switch (evt.Type)
                {
                    case EventType.KEYBOARD:
                    case EventType.PLACEHOLDER:
                        {
                            try
                            {
                                CurrentEventPausePair.Second = GetPauseLocation();
                                //Debug.Write(" Event added to list: '" + evt.GetKeypressKey() + "'; ");

                                // If there is a Keypress, the insert is taken care of.
                                TakeAction(CurrentEventPausePair);
                                //InsertLength = 0;
                            }
                            catch (AnalysisException)
                            {
                                // Event does not contain a keypress: skip event 
                            }
                            break;
                        }
                    case EventType.MOUSE:
                        {
                            var mouseEventPart = Event.GetFirstEventPart<AbstractMouseEvent>(evt);
                            if (mouseEventPart != null && !(mouseEventPart is MouseMovement))
                            {
                                CurrentEventPausePair.Second = GetPauseLocation();

                                //Debug.Write(" Mouse event added; ");
                                TakeAction(CurrentEventPausePair);
                            }
                            else // if (mouseEventPart != null && mouseEventPart is MouseMovement)
                            {
                                int previousEventIndex = (PastIndices.Count > 0)? 
                                    CurrentReorderIndices[PastIndices[PastIndices.Count-1]] : -1;
                                if (previousEventIndex >= 0 
                                    && eventPauseList[previousEventIndex].Second == PauseLocation.WITHIN_WORDS)
                                {
                                    CurrentEventPausePair.Second = PauseLocation.WITHIN_WORDS;
                                }
                            }
                            break;
                        }
                }
                InsertEventPausePair(eventPauseList, CurrentReorderIndices[CurrentEventIndex], CurrentEventPausePair);

                 //Debug.WriteLine(" Final state for this " + CurrentEventPausePair.First.GetKeypressValue() 
                // + "-type: " + CurrentEventPausePair.Second);
                CurrentEventPausePair = ReadNextPair(eventPauseList);
            }

            FixStartEndPauseLocation(eventPauseList);

            //Debug.WriteLine(" *END*");
            return eventPauseList;
        }

        private static void FixStartEndPauseLocation(List<Pair<Event, PauseLocation>> eventPausePairList)
        {
            var filteredEventPausePairList = eventPausePairList.FindAll(Analysis.DropEventPauseTypeFilter(new[] { EventType.FOCUS }));

            if (filteredEventPausePairList.IsNullOrEmpty() || filteredEventPausePairList.First() == eventPausePairList.Last())
            {
                return;
            }
           
            // Prevents BEFORE_SENTENCES being overwritten
            if (!filteredEventPausePairList.First().Second.Equals(PauseLocation.BEFORE_SENTENCES))
            {
                filteredEventPausePairList.First().Second = PauseLocation.INITIAL;
            }

            //Debug.Write(" Initial state: " + filteredEventPausePairList.First().Second);

            if (filteredEventPausePairList.Count < 1 || filteredEventPausePairList[1] == eventPausePairList.Last())
                return;

            if (filteredEventPausePairList.First().First.Type.Equals(EventType.MOUSE))
            {
                var mouseEvent =
                    Event.GetFirstEventPart<AbstractMouseEvent>(filteredEventPausePairList.First().First);

                if (!filteredEventPausePairList[1].Second.Equals(PauseLocation.BEFORE_SENTENCES))
                {
                    if (mouseEvent is MouseMovement) filteredEventPausePairList[1].Second = PauseLocation.INITIAL;
                }

                //Debug.Write(" Initial state: " + filteredEventPausePairList[1].Second);
            }

            Pair<Event, PauseLocation> tmp = filteredEventPausePairList.Last();
            tmp.Second = PauseLocation.END;

            Debug.Write(" Final state:  " + tmp.Second);

            // LEGACY The last 4 events are marked as end pauses here.
            // This is done because in the legacy version of InputLog, closing InputLog required 4 actions:
            // 1. A mouse movement to the system tray
            // 2. A right click on the InputLog icon ( this only works prior to Windows 7, 
            //    since Windows 7 collapses the tray icons: extra clicks are required).
            // 3. A movement to the 'Stop Recording' part of the right mouse menu
            // 4. A left click on 'Stop Recording'
            for (int i = filteredEventPausePairList.Count - 2; i > filteredEventPausePairList.Count - 6; i--)
            {
                if (tmp == filteredEventPausePairList.First()) return;
                tmp = filteredEventPausePairList[i];

                // Inserting a final AFTER_PARAGRAPHS if an earlier BEFORE_PARAGRAPHS remained open.
                if (HasBeforeParagraph)
                {
                    tmp.Second = PauseLocation.AFTER_PARAGRAPHS;
                    HasBeforeParagraph = false;
                    break;
                }

                if (tmp.First.Type != EventType.MOUSE) { break; }
                tmp.Second = PauseLocation.END;

                Debug.Write(" Final state: END_PAUSE ");
            }

            // Finding the last keyboard event and mark it AFTER_SENTENCES if it was WITHIN_WORDS.
            for (int i = filteredEventPausePairList.Count - 1; i > 0; i--)
            {
                tmp = filteredEventPausePairList[i];
                if (tmp.First.Type != EventType.KEYBOARD) continue;
                if (tmp.Second == PauseLocation.WITHIN_WORDS)
                    tmp.Second = PauseLocation.AFTER_SENTENCES;
                break;
            }

            // Finding the last keyboard event and marking it AFTER_SENTENCES if it was WITHIN_WORDS.
            // Inserting a final AFTER_PARAGRAPHS if an earlier BEFORE_PARAGRAPHS remained open.
            for (int i = filteredEventPausePairList.Count - 1; i > 0; i--)
            {
                tmp = filteredEventPausePairList[i];
                if (tmp.First.Type != EventType.KEYBOARD || tmp.Second.Equals(PauseLocation.COMBINATION_KEY)) continue;
                
                if (tmp.Second.Equals(PauseLocation.AFTER_SENTENCES)
                    || (tmp.Second.Equals(PauseLocation.AFTER_PARAGRAPHS))) 
                    break;
                if (tmp.Second.Equals(PauseLocation.WITHIN_WORDS) 
                    || tmp.Second.Equals(PauseLocation.AFTER_WORDS)
                    || tmp.Second.Equals(PauseLocation.BEFORE_WORDS)
                    || tmp.Second.Equals(PauseLocation.UNDETERMINED)
                    || tmp.Second.Equals(PauseLocation.COMBINATION_KEY)
                    || tmp.Second.Equals(PauseLocation.UNKNOWN))
                    tmp.Second = HasBeforeParagraph ? PauseLocation.AFTER_PARAGRAPHS : PauseLocation.AFTER_SENTENCES;
                break;
            }
        }

        /// <summary>
        /// Handling the shift keys and mouse events.
        /// </summary>
        /// <param name="currentEventPausePair">Currently inspected Event-PauseLocation pair </param>
        private void TakeAction(Pair<Event, PauseLocation> currentEventPausePair)
        {
            var keyPress = currentEventPausePair.First.GetKeypressFromEvent();
            if (keyPress == null) return;

            if (Lexical.IsWithinWordChar(keyPress.Value)
                || Lexical.IsSentencePrecedingReadingMark(keyPress.Value))
            {
                // Notify listeners and increase character counts.
                //CallCallback(currentEventPausePair, WordCharEventHandler);
                NotifyWordCharListeners(currentEventPausePair);
                NbSentenceChars1++;

                //CallCallback(currentEventPausePair, SentenceCharEventHandler);
                NotifySentenceCharListeners(currentEventPausePair);
                NbParagraphChars1++;

                //CallCallback(currentEventPausePair, ParagraphCharEventHandler);
                NotifyParagraphCharListeners(currentEventPausePair);

                //   Debug.Write(" NbSentenceChars " + NbSentenceChars + "  NbParagraphChars " + NbParagraphChars);
            }

            if (Lexical.IsWordReadingMark(keyPress.Value))
            {
                // Notify listeners and increase character counts.
                NbSentenceChars1++;
                NotifySentenceCharListeners(currentEventPausePair);
                NotifyParagraphCharListeners(currentEventPausePair);
                //    Debug.Write(" NbSentenceChars " + NbSentenceChars);
            }

            if (Lexical.IsSentenceReadingMark(keyPress.Value))
            {
                // Notify listeners and increase character counts.
                NbParagraphChars1++;
                NotifyParagraphCharListeners(currentEventPausePair);
                //   Debug.Write(" NbParagraphChars " + NbParagraphChars);
            }
        }

        #region FSMInteraction

        private void ForeachFSM(Action<AFiniteStateMachine<ISet<string>>> fsmAction)
        {
            foreach (var fsm in FSMList)
            {
                fsmAction(fsm);
            }
        }

        /// <summary>
        /// Peek at the next (interesting) event in the list.
        /// </summary>
        /// <param name="nextPosition">Amount of positions to look further in the list</param>
        /// <returns>Event at </returns>
        /// <remarks>Unconsidered events are skipped.</remarks>
        public Event Peek(int nextPosition)
        {
            if (nextPosition >= 0)
            {
                int position = 0;
                for (int i = CurrentEventIndex; i < CurrentEventList.Count; i++)
                {
                    Event evt = CurrentEventList[i];
                    switch (evt.Type)
                    {
                        case EventType.KEYBOARD:
                        case EventType.PLACEHOLDER:
                        case EventType.MOUSE:
                            {
                                //Don't skip these ones
                                if (position == nextPosition)//If we are at the requested event's position
                                {
                                    return CurrentEventList[i];
                                }
                                position++;
                                break;
                            }
                    }
                    //Debug.Write(" " + EventCounter + " ");
                }
            }
            else // nextPosition < 0
            {
                int n = PastIndices.Count;
                if (nextPosition >= -n)
                {
                    return CurrentEventList[PastIndices[n+nextPosition]];
                }
            }
            return null;
        }

        /// <summary>
        /// Progresses to next event, effectively skipping all ignored events.
        /// </summary>
        /// <param name="eventList">Currently inspected list of events</param>
        /// <param name="currentIndex">Index of currently considered event</param>
        /// <param name="eventPauseList">Current output list</param>
        /// <returns>Index of next event to consider</returns>
        private int NextEventIndex(IList<Event> eventList, int currentIndex, IList<Pair<Event, PauseLocation>> eventPauseList)
        {
            if (0 <= currentIndex && currentIndex < eventList.Count)
                PastIndices.Add(currentIndex);

            for (int i = currentIndex + 1; i < eventList.Count; i++)
            {
                var evt = eventList[i];
                var eventPausePair = new Pair<Event, PauseLocation>(evt, PauseLocation.UNDETERMINED);
                //var insertLength = 0;

                switch (evt.Type)
                {
                    case EventType.KEYBOARD:
                        var keyPress = evt.GetKeypressFromEvent();

                        // Can be 'null' when WinLog is switched off by the user in the GUI-options.
                        if (keyPress == null)
                        {
                            eventPausePair.Second = PauseLocation.UNKNOWN;
                            InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                            NotifyEventHandler(eventPausePair);
                            break;
                        }
                        if (Lexical.IsCombinationKey(keyPress))
                        {
                            eventPausePair.Second = PauseLocation.COMBINATION_KEY;
                            InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                            NotifyEventHandler(eventPausePair);
                            break;
                        }
                        if (Lexical.HasRevisionKey(keyPress))
                        {
                            eventPausePair.Second = PauseLocation.REVISION;
                            InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                            NotifyEventHandler(eventPausePair);
                            break;
                        }
                        return i;
                    case EventType.MOUSE:
                        eventPausePair.Second = PauseLocation.MOUSE;
                        InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                        NotifyEventHandler(eventPausePair);
                        break;
                    case EventType.PLACEHOLDER:
                        return i;
                    case EventType.INSERT:
                        eventPausePair.Second = PauseLocation.CHANGE;
                        //Debug.Write(" Event added to list: 'INSERT';");
                        InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                        NotifyInsertListeners(eventPausePair);
                        break;
                    case EventType.SELECTION:
                    case EventType.REPLACEMENT:
                        try
                        {                          
                            eventPausePair.Second = PauseLocation.CHANGE;
                            InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                            NotifyInsertListeners(eventPausePair);;
                            //Debug.Write(" Event added to list: 'REPLACEMENT';");                           
                        }
                        catch (AnalysisException) {}
                        break;
                    case EventType.FOCUS:
                        eventPausePair.Second = PauseLocation.CHANGE;
                        InsertEventPausePair(eventPauseList, CurrentReorderIndices[i], eventPausePair);
                        // Because we only want to process data inside the main document, we call
                        // the word, sentence and paragraph listners before notifying a focus change.
                        NotifyWordListeners(eventPausePair);
                        NotifySentenceListeners(eventPausePair);
                        NotifyParagraphListeners(eventPausePair);

                        NotifyFocusChangeListeners(eventPausePair);                      
                        break;
                }
                //Debug.Write(" " + EventCounter + " ");
            }
            // Out of range on purpose.
            return eventPauseList.Count;
        }

        /// <summary>
        /// Go to inspect the next Event in the list.
        /// </summary>
        /// <param name="eventPauseList">Currently inspected Event-PauseLocation list</param>
        /// <returns>Next pair in the list</returns>
        /// <remarks>Note that this method skips ignored events. See also "NextEventIndex"</remarks>
        private Pair<Event, PauseLocation> ReadNextPair(List<Pair<Event, PauseLocation>> eventPauseList)
        {
            if (CurrentEventList != null)
            {
                var nextEventIndex = NextEventIndex(CurrentEventList, CurrentEventIndex, eventPauseList);
                if (0 <= nextEventIndex && nextEventIndex < CurrentEventList.Count - 1)
                {
                    CurrentEventIndex = nextEventIndex;
                    return new Pair<Event, PauseLocation>(CurrentEventList[CurrentEventIndex], PauseLocation.UNDETERMINED);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the PauseLocation for the current Event.
        /// </summary>
        /// <returns>PauseLocation as generated by the finite state machine, 
        /// or UNKNOWN when the operation timed out.</returns>
        /// <seealso cref="Timeout"/>
        /// <seealso>
        ///     <cref>SleepTime</cref>
        /// </seealso>
        private PauseLocation GetPauseLocation()
        {
            _pauseLocationReady = false;
            _generatedPauseLocation = PauseLocation.UNDETERMINED;
            
            ForeachFSM(fsm => { _currentFSM = fsm; _currentFSM.Start(); _currentFSM.PerformTransition(); });
           
            if (_pauseLocationReady)
            {
                // Keeping track of unclosed paragraphs at the end of the document.
                switch (_generatedPauseLocation)
                {
                    case PauseLocation.BEFORE_PARAGRAPHS:
                        HasBeforeParagraph = true;
                        break;
                    case PauseLocation.AFTER_PARAGRAPHS:
                        HasBeforeParagraph = false;
                        break;
                 }
                return _generatedPauseLocation;
            }

            Reset();
            return PauseLocation.UNKNOWN;
        }

        /// <summary>
        /// Erase _all _state of this pause location marker.
        /// </summary>
        private void Reset()
        {
            ForeachFSM(fsm => fsm.Reset());
        }

        /// <summary>
        /// Reset currently considered finite state machine.
        /// </summary>
        public static void ResetCurrentFSM()
        {
            _currentFSM.Reset();
        }

        /// <summary>
        /// Sends this PauseLocationMarker a message that the PauseLocation that has been generated.
        /// </summary>
        /// <param name="pauseLoc">Generated PauseLocation for the current Event</param>
        /// <remarks>Main goal of this is encapsulation (to avoid making GeneratedPauseLocation a public field). 
        /// Please do not call this method, except when setting a PauseLocation for some Event.</remarks>
        public void GeneratePauseLocation(PauseLocation pauseLoc)
        {
            if (!_pauseLocationReady)//Don't overwrite things
            {
                _generatedPauseLocation = pauseLoc;
                _pauseLocationReady = true;
            }
        }
        #endregion

        #region Callback

        private void ResetNotifications()
        {
            NotifiedWord = false;
            NotifiedWordChar = false;
            NotifiedSentence = false;
            NotifiedSentenceChar = false;
            NotifiedParagraph = false;
            NotifiedParagraphChar = false;
        }
        
        private void CallCallback(Pair<Event, PauseLocation> eventPausePair, Callback callback)
        {
            if (callback != null)
            {
                if (eventPausePair == null)
                {
                    eventPausePair = CurrentEventPausePair;
                }
                callback(this, new PauseLocationEventArgs(eventPausePair));
            }
        }

        public void NotifyWordListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedWord)
            {
                CallCallback(currentEventPausePair, WordEventHandler);
                NotifiedWord = true;
            }
        }

        public void NotifyWordCharListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedWordChar)
            {
                CallCallback(currentEventPausePair, WordCharEventHandler);
                NotifiedWordChar = true;
            }
        }

        public void NotifySentenceListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedSentence)
            {
                CallCallback(currentEventPausePair, SentenceEventHandler);
                NotifiedSentence = true;
            }
        }

        public void NotifySentenceCharListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedSentenceChar)
            {
                CallCallback(currentEventPausePair, SentenceCharEventHandler);
                NotifiedSentenceChar = true;
            }
        }

        public void NotifyParagraphListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedParagraph)
            {
                CallCallback(currentEventPausePair, ParagraphEventHandler);
                NotifiedParagraph = true;
            }
        }

        public void NotifyParagraphCharListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            if (!NotifiedParagraphChar)
            {
                CallCallback(currentEventPausePair, ParagraphCharEventHandler);
                NotifiedParagraphChar = true;
            }
        }

        private void NotifyFocusChangeListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            CallCallback(currentEventPausePair, FocusChangeEventHandler);
        }

        private void NotifyInsertListeners(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            CallCallback(currentEventPausePair, InsertEventHandler);
        }


        private void NotifyEventHandler(Pair<Event, PauseLocation> currentEventPausePair = null)
        {
            CallCallback(currentEventPausePair, EventEventHandler);
        }
        #endregion

        /// <summary>
        /// Returns a string representation of this PauseLocationMarker.
        /// </summary>
        /// <returns>String representation of the mechanism of this PauseLocationMarker</returns>
        public override string ToString()
        {
            string descr = string.Empty;
            Func<ISet<string>,string> stateToStringFunc = FSMUtil.IEnumerableToString;
            ForeachFSM(fsm => {descr += fsm.CurrentStateToString(stateToStringFunc) 
                + Environment.NewLine + fsm.ToString(stateToStringFunc) + Environment.NewLine + Environment.NewLine;});
            return descr;
        }

    }
}