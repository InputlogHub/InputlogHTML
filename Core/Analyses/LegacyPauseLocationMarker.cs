using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Signature of the callback delegate that can be installed on one of the eventlisteners of the PauseLocationMarker.
    /// </summary>
    /// <param name="sender">Sender of the event (will always be an instance of PauseLocationMarker)</param>
    /// <param name="e">Event Arguments</param>
    public delegate void Callback(object sender, PauseLocationEventArgs e);

    /**
     * **************************************************************
     *   Important to anyone making modifications to this class.
     * **************************************************************
     * Joris Roovers - 31/08/2010
     *  
     * This class basically implements a Finite State Machine according to the rules that are defined in Docs/PauseLocationRules.xls.
     * 
     * The rules in itself are mostly pretty straightforward, but the implementation of the ruleset (provided by this class) is 
     * a lot more complex.
     *     
     * The code in this class is a port of the original C++ code (PauseLocationMarker.cpp, from now referred to as the "legacy code")
     * in C#.
     * Because of the complexity of the original code, it was decided to do a literal translation of the code in order to preserve the
     * behavior of the code. Although some modifications have been made in order to make the code more accessible, some study will
     * be needed in order to fully understand what the code actually does.
     *
     * It would be nice if a better port of this code could be made in the future (starting from this code).
     * In my opinion, the best option would be to completely rewrite the functionality in such a way that the ruleset can be extracted
     * such that existing rules can be reviewed and tweaked more easily and new rules can be added without much trouble.
     * Also, I'm not convinced that both a callback mechanism and a "return" mechanism are needed, I think one of both should suffice 
     * (if this sound rather vague, compare how the PauseLocationMarker is used in GeneralAnalysis and in PauseAnalysis).
     * If someone would make such a new implementation at some point, decent (unit) tests are a neccessity to garantuee 
     * output behaviour.
     * 
     * In some parts of the code I've added a comment proceeded with the word LEGACY, meaning that the comment concerns something
     * about the legacy code. This is typically done if some part is implemented differently than in the legacy code, 
     * or if some parts of the legacy code have been dropped (most notably assert() statements).
     * 
     * As a last remark: please ignore any strange quirks in the code below; most (if not all) are artifacts from the legacy code
     * (they do implement some kind of behavior, so don't just delete them!).
     */


    /// <summary>
    /// Determines the pauselocations in a given list of events.
    /// 
    /// PauseLocations are locations in a text that are have some kind of pattern to which a certain type of pause is assigned to.
    /// e.g. the space between 2 words in a text is typically assigned the pauselocation BETWEEN_WORDS, while the characters in the
    /// word itself are (obviously) marked as WITHIN_WORD.
    /// For the different type of pauselocations, have a look at the PauseLocation class.
    /// 
    /// This class basically implements a Finite State Machine according to the rules that are defined in Docs/PauseLocationRules.xls.
    /// </summary>
    public class PauseLocationMarker
    {
        #region Fields

        /// <summary>
        /// List of events that fall under the rule that is currently being matched.
        /// When action is taken based on the current rule (see the different that are named "apply..."), 
        /// then typically all the events in this list receive the same pauselocation (some exceptions are possible, 
        /// most notably, the first and last event in the list).
        /// </summary>
        private readonly List<Pair<Event, PauseLocation>> CurrentRuleEvents;

        /// <summary>
        /// Number of characters that already need to be matched for a event to be marked as part of a paragraph.
        /// For example, if there is a single charachter between 2 paragraphs and the ParagraphCharsThreshold is set to 3 (=default)
        /// then, this space will never be marked as a state that represents an event within a paragraph, 
        /// also the ParagraphEventHandler will not be called.
        /// </summary>
        private readonly uint ParagraphCharsThreshold;

        /// <summary>
        /// Number of characters that already need to be matched for a event to be marked as part of a sentence.
        /// For example, if there is a single space between 2 sentences and the SentenceCharsTreshold is set to 3 (=default)
        /// then, this space will never be marked as a state that represents an event within a sentence, 
        /// also the SentenceEventHandler will not be called.
        /// </summary>
        private readonly uint SentenceCharsTreshold;

        /// <summary>
        /// Current state of the FSM.
        /// </summary>
        private State CurrentState;

        /// <summary>
        /// Current number of events that are matched as a character that might be part of a paragraph.
        /// </summary>
        private uint NbParagraphChars;

        /// <summary>
        /// Current number of events that are matched as a character that might be part of a sentence.
        /// </summary>
        private uint NbSentenceChars;
        /// <summary>
        /// The length of the last insert. Occurs when the last event was an insert not followed by a KeyPress.
        /// The inserted characters remain unaccounted for unless they are picked up by the concerned analyses.
        /// </summary>
        public static int InsertLength { get; private set; }

        /// <summary>
        /// Eventhandlers
        /// </summary>
        public event Callback EventEventHandler;
        public event Callback WordEventHandler;
        public event Callback SentenceEventHandler;
        public event Callback ParagraphEventHandler;
        public event Callback WordCharEventHandler;
        public event Callback SentenceCharEventHandler;
        public event Callback ParagraphCharEventHandler;

        /// <summary>
        /// The states where the PauseLocationMarker can be in.
        /// </summary>
        private enum State
        {
            // START STATES
            START,
            TABSPACE,
            ENTER,
            SENTENCE_READING_MARK,
            WORD_READING_MARK,
            ALPHA_NUMERIC_OR_WITHIN_WORD,
            BINDING_CHARACTER,
            ANY_CHARACTER,
           
            // reachable from ENTER
            ENTER_TAB,
            ENTER_SPACE,
            ENTER_ENTER,

            // reachable from ENTER_TAB
            ENTER_TAB2,
            ENTER_X_BACKSPACE,

            // reachable from ENTER_SPACE
            ENTER_SPACE2,

            // reachable from CHARACTER
            CHARACTER_TABSPACE,
            CHARACTER_ENTER,

            // reachable from CHARACTER_ENTER
            CHARACTER_ENTER_SPACE,

            // reachable from SENTENCE_READING_MARK:
            SENTENCE_READING_MARK_TABSPACE,
            SENTENCE_READING_MARK_ENTER,
            SENTENCE_READING_MARK_NOT_CHARACTER,

            // reachable from WORD_READING_MARK
            WORD_READING_MARK_TABSPACE,
            WORD_READING_MARK_ENTER,

            // reachable from ALPHA_NUMERIC_OR_WITHIN_WORD
            ALPHANUMERIC_WITHINWORD_CHAR_DOT,
            ALPHANUMERIC_OR_WITHINWORD_TABSPACE,
            ALPHANUMERIC_OR_WITHINWORD_WORD_READING_MARK,
            ALPHANUMERIC_WITHINWORD_CTRL_BACK,

            // reachable from ALPHANUMERIC_WITHINWORD_CHAR_DOT
            ALPHANUMERIC_WITHINWORD_CHAR_DOT_ALPHANUMERIC_WITHINWORD_CHAR,

            // reachable from ALPHANUMERIC_WITHINWORD_CHAR_DOT_ALPHANUMERIC_WITHINWORD_CHAR
            ALPHANUMERIC_OR_WITHINWORD_CTRL_BACKSPACE,

            // reachable from ALPHANUMERIC_OR_WITHINWORD_TABSPACE
            ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING,

            // reachable from ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING
            ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING_TABSPACE
        }

        #endregion

        public PauseLocationMarker(uint sentenceCharsTreshold = 3u, uint paragraphCharsThreshold = 3u)
        {
            CurrentRuleEvents = new List<Pair<Event, PauseLocation>>();
            //TODO extract these numbers, use config/settings file
            NbSentenceChars = 0;
            NbParagraphChars = 0;
            SentenceCharsTreshold = sentenceCharsTreshold;
            ParagraphCharsThreshold = paragraphCharsThreshold;
        }

        private int EventCounter = -1;

        public List<Pair<Event, PauseLocation>> Start(List<Event> eventz)
        {
            var events = new List<Pair<Event, PauseLocation>>(eventz.Count);
            if (eventz.Count == 0) return events; // fail safety: Don't run the algorithm is the list is empty

            CurrentState = State.START;
            Debug.WriteLine("*Start* ");
            foreach (Event even in eventz)
            {
                var eventPausePair = new Pair<Event, PauseLocation>(even, PauseLocation.UNDETERMINED);
                events.Add(eventPausePair);
                EventCounter++;
                Debug.Write(" " + EventCounter + " ");

                // notify listeners
                CallCallback(eventPausePair, EventEventHandler);

                switch (even.Type)
                {
                    case EventType.KEYBOARD:
                    case EventType.PLACEHOLDER:
                        {
                            try
                            {
                                CurrentState = DoTransition(eventPausePair);
                                CurrentRuleEvents.Add(eventPausePair);
                                Debug.Write(" Event added to list: '" + GetKeypressKey(even) + "'; ");
                                // If there is a Keypress, the insert is taken care of.
                                TakeAction(eventPausePair);
                                InsertLength = 0;
                            }
                            catch (AnalysisException)
                            {
                                Debug.WriteLine("AnalysisException while notifying listeners " +
                                                "of EventType.KEYBOARD or EventType.PLACEHOLDER");
                            }
                            break;
                        }
                    case EventType.INSERT:
                        {
                            // Capturing the length of the most recent insert. 
                            foreach (var insertPart in even.Parts.OfType<Insert>())
                            {
                                InsertLength = insertPart.Before.Length;
                                eventPausePair.Second = PauseLocation.TRANSITION;
                                Debug.Write(" Event added to list: 'INSERT';");
                            }
                            break;
                        }
                    case EventType.REPLACEMENT:
                        {
                            try
                            {
                                CurrentRuleEvents.Add(eventPausePair);
                                eventPausePair.Second = PauseLocation.TRANSITION;
                                Debug.Write(" Event added to list: 'REPLACEMENT';");
                                InsertLength = 0;
                            }
                            catch (AnalysisException)
                            {
                                Debug.WriteLine("AnalysisException while notifying listeners of EventType.REPLACEMENT");
                            }
                            break;
                        }
                    case EventType.MOUSE:
                        {
                            var mouseEventPart = Event.GetFirstEventPart<AbstractMouseEvent>(even);
                            if (mouseEventPart != null && !(mouseEventPart is MouseMovement))
                            {
                                CurrentState = DoTransition(eventPausePair);
                                CurrentRuleEvents.Add(eventPausePair);
                                Debug.Write(" Mouse event added; ");
                                TakeAction(eventPausePair);
                            }
                            else // if (mouseEventPart != null && mouseEventPart is MouseMovement)
                            {
                                if (CurrentState == State.ALPHA_NUMERIC_OR_WITHIN_WORD)
                                {
                                    eventPausePair.Second = PauseLocation.WITHIN_WORDS;
                                }
                            }
                            break;
                        }
                    case EventType.FOCUS:
                        {
                            eventPausePair.Second = PauseLocation.TRANSITION;
                            break;
                        }
                }
                // If we were unable to determined to pauselocation, mark it as an unknown pause
                if (eventPausePair.Second == PauseLocation.UNDETERMINED)
                {
                    eventPausePair.Second =  PauseLocation.UNKNOWN;
                }
                Debug.WriteLine(" Final state for this " + even.Type + "-type: " + eventPausePair.Second);
            }
            FixStartEndPauseLocation(events);
            Debug.WriteLine(" *END*");
            return events;
        }


        private static void FixStartEndPauseLocation(List<Pair<Event, PauseLocation>> eventPausePairList)
        {
            List<Pair<Event, PauseLocation>> filteredEventPausePairList =
                eventPausePairList.FindAll(Analysis.DropEventPauseTypeFilter(new[] {EventType.FOCUS}));

            if (filteredEventPausePairList.First() == eventPausePairList.Last())
                return;
            filteredEventPausePairList.First().Second = PauseLocation.INITIAL;
            Debug.Write(" Final state: INITIAL_PAUSE ");
            if (filteredEventPausePairList.Count < 1 || filteredEventPausePairList[1] == eventPausePairList.Last())
                return;

            if (filteredEventPausePairList.First().First.Type == EventType.MOUSE)
            {
                var mouseEvent =
                    Event.GetFirstEventPart<AbstractMouseEvent>(filteredEventPausePairList.First().First);
                if (mouseEvent is MouseMovement) filteredEventPausePairList[1].Second = PauseLocation.INITIAL;
                Debug.Write(" Final state: INITIAL_PAUSE ");
            }


            Pair<Event, PauseLocation> tmp = filteredEventPausePairList.Last();
            tmp.Second = PauseLocation.END;
            Debug.Write(" Final state: END_PAUSE ");
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
                if (tmp.First.Type != EventType.MOUSE) break;
                tmp.Second = PauseLocation.END;
                Debug.Write(" Final state: END_PAUSE ");
            }
        }

        /// <summary>
        /// Handling the shift keys and mouse events.
        /// </summary>
        /// <param name="currentEventPausePair"></param>
        private void TakeAction(Pair<Event, PauseLocation> currentEventPausePair)
        {
            KeyPress keyPress = GetKeypressFromEvent(CurrentRuleEvents.Last().First);
            if (keyPress != null)
            {
                if (Lexical.IsAlphaNumeric(keyPress.Value) 
                    || Lexical.IsWithinWordChar(keyPress.Value)
                    || Lexical.IsSentencePrecedingReadingMark(keyPress.Value))
                {
                    // Notify listeners and increase counts.
                    CallCallback(currentEventPausePair, WordCharEventHandler);
                    NbSentenceChars++;
                    CallCallback(currentEventPausePair, SentenceCharEventHandler);
                    NbParagraphChars++;
                    CallCallback(currentEventPausePair, ParagraphCharEventHandler);
                 //   Debug.Write(" NbSentenceChars " + NbSentenceChars + "  NbParagraphChars " + NbParagraphChars);
                }
                if (Lexical.IsWordReadingMark(keyPress.Value))
                {
                    // Notify listeners and increase counts.
                    NbSentenceChars++;
                    CallCallback(currentEventPausePair, SentenceCharEventHandler);
                    CallCallback(currentEventPausePair, ParagraphCharEventHandler);
                //    Debug.Write(" NbSentenceChars " + NbSentenceChars);
                }
                if (Lexical.IsSentenceReadingMark(keyPress.Value))
                {
                    // Notify listeners and increase counts.
                    NbParagraphChars++;
                    CallCallback(currentEventPausePair, ParagraphCharEventHandler);
                 //   Debug.Write(" NbParagraphChars " + NbParagraphChars);
                }
            }
        }

        private State DoTransition(Pair<Event, PauseLocation> eventPausePair)
        {
            var value = "";
            var keyPress = GetKeypressFromEvent(eventPausePair.First);
            if (keyPress != null)
            {
                value = keyPress.Value;

                // Debug only
                if (Lexical.IsSpace(value))
                {
                    Debug.Write("'SPACE'");
                }
                if (Lexical.IsEnter(value))
                {
                    Debug.Write("'ENTER'");
                }
                if (Lexical.IsBackSpace(value))
                {
                    Debug.Write("'BACK'");
                }
                if (Lexical.IsTab(value))
                {
                    Debug.Write("'TAB'");
                }
                else
                {
                    Debug.Write("'" + value + "'");
                }
                // End Debug.

                if (keyPress.KeyboardState.Count != 0 && Lexical.HasCombinationKey(keyPress))
                {
                    eventPausePair.Second = PauseLocation.COMBINATION_KEY;

                    if (Lexical.HasShiftKey(keyPress))
                    {
                        ApplyRuleC5(eventPausePair);
                    }
                }
                if (Lexical.HasRevisionKey(keyPress))
                {
                    eventPausePair.Second = PauseLocation.REVISION;
                }
            }

            if (CurrentRuleEvents.Count > 1 
                && CurrentRuleEvents[1].Second == PauseLocation.TRANSITION
                || CurrentState == State.START)
            {
                ApplyRuleC5(eventPausePair);
            }

            if (CurrentState != State.BINDING_CHARACTER && SkipKey(keyPress))
            {
                Debug.Write(" CurrentState B: " + CurrentState + "; ");
                return CurrentState;
            }
            Debug.Write(" Previous State: " + CurrentState + "; ");

            switch (CurrentState)
            {
                case State.START:
                    var nextState = DetermineNextStartState(value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                    // ENTER - TAB - SPACE (Priority P)
                case State.TABSPACE:
                    nextState = MatchTabSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER:
                    nextState = MatchEnter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_TAB:
                    nextState =  MatchEnterTab(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_TAB2:
                    nextState = MatchEnterTab2(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_X_BACKSPACE:
                    nextState = MatchEnterXBackSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_SPACE:
                    nextState = MatchEnterSpace(value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_SPACE2:
                    nextState = MatchEnterSpace2(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ENTER_ENTER:
                    nextState = MatchEnterEnter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;

                case State.ANY_CHARACTER:
                    nextState = MatchAnyCharacter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.CHARACTER_TABSPACE:
                    nextState = MatchCharacterTabSpace(value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.CHARACTER_ENTER:
                    nextState = MatchCharacterEnter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.CHARACTER_ENTER_SPACE:
                    nextState = MatchCharacterEnterSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.SENTENCE_READING_MARK:
                    nextState = MatchSentenceReadingMark(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.SENTENCE_READING_MARK_TABSPACE:
                    nextState = MatchSentenceReadingMarkTabSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.SENTENCE_READING_MARK_ENTER:
                    nextState = MatchSentenceReadingMarkEnter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.SENTENCE_READING_MARK_NOT_CHARACTER:
                    nextState = MatchSentenceReadingMarkNotCharacter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;

                case State.WORD_READING_MARK:
                    nextState = MatchWordReadingMark(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.WORD_READING_MARK_TABSPACE:
                    nextState = MatchWordReadingMarkTabSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.WORD_READING_MARK_ENTER:
                    nextState = MatchWordReadingMarkEnter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHA_NUMERIC_OR_WITHIN_WORD:
                    nextState = MatchAlphaNumericOrWithinWord(eventPausePair, value, keyPress);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_WITHINWORD_CHAR_DOT:
                    nextState = MatchAlphaNumericWithinWordCharDot(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_WITHINWORD_CHAR_DOT_ALPHANUMERIC_WITHINWORD_CHAR:
                    nextState = MatchAlphaNumericWithinWordCharDotAlphaNumericWithinWordChar(eventPausePair, value, keyPress);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.BINDING_CHARACTER:
                    nextState = MatchBindingCharacter(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE:
                    nextState = MatchAlphaNumericOrWithinWordTabSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING:
                    nextState = MatchAlphaNumericOrWithinWordTabSpaceBinding(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING_TABSPACE:
                    nextState = MatchAlphaNumericOrWithinWordTabSpaceBindingTabSpace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_OR_WITHINWORD_WORD_READING_MARK:
                    nextState = MatchAlphaNumericOrWithinWordWordReadingMark(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                case State.ALPHANUMERIC_OR_WITHINWORD_CTRL_BACKSPACE:
                    nextState = MatchAlphaNumericOrWhithinWordCtrlBackspace(eventPausePair, value);
                    Debug.Write(" Next state: " + nextState + "; ");
                    return nextState;
                default:
                    {
                        //TODO this should not happen! Log this!
                        Debug.WriteLine("Default State: this should not happen!");
                        return State.START;
                    }
            }
        }

        private static bool SkipKey(KeyPress keyPress)
        {
            if (keyPress == null) return false;
            return keyPress.Key == KeysEx.VK_DELETE;
        }

        private State DetermineNextStartState(string value)
        {
            Debug.Write(CurrentRuleEvents.Count() + " previous events in CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
            // Test added
            if (value.Length == 0) return State.START;

            if (NbParagraphChars >= ParagraphCharsThreshold && Lexical.IsTabOrSpace(value))
            {
                return State.TABSPACE;
            }
            if (NbParagraphChars >= ParagraphCharsThreshold && Lexical.IsEnter(value))
            {
                return State.ENTER;
            }
            if (Lexical.IsSentenceReadingMark(value))
            {
                return State.SENTENCE_READING_MARK;
            }
            if (Lexical.IsWordReadingMark(value))
            {
                return State.WORD_READING_MARK;
            }
            if (Lexical.IsAlphaNumeric(value) 
                || Lexical.IsWithinWordChar(value)
                || Lexical.IsSentencePrecedingReadingMark(value))
            {
                
                return State.ALPHA_NUMERIC_OR_WITHIN_WORD;
            }
            if (Lexical.IsBindingChar(value))
            {
                return State.BINDING_CHARACTER;
            }
            if (Lexical.IsCharacter(value) && NbSentenceChars >= SentenceCharsTreshold)
            {
                return State.ANY_CHARACTER;
            }
            return State.START;
        }


        private void CallCallback(Pair<Event, PauseLocation> eventPausePair, Callback callback)
        {
            if (callback != null)
            {
                callback(this, new PauseLocationEventArgs(eventPausePair));
            }
        }

        #region Applyrules

        private void ApplyRuleP1P2P3P4(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // Not much to do:
            // all events in myCurrentRuleEvents should be marked with BP,
            // and the current event should also be marked with BP.

            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];

                if (i == 0)
                {
                    even.Second = PauseLocation.AFTER_PARAGRAPHS;
                    Debug.Write(" RuleP1P2P3P4 for '" + GetKeypressKey(even.First) + "': AFTER_PARAGRAPHS; ");
                }
                even.Second = i == count - 1 ? PauseLocation.BEFORE_PARAGRAPHS : PauseLocation.AFTER_PARAGRAPHS;
                Debug.Write(" RuleP1P2P3P4 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
            }
            currentEventPausePair.Second = PauseLocation.AFTER_PARAGRAPHS;
            Debug.Write(" RuleP1P2P3P4 for '" + GetKeypressKey(currentEventPausePair.First) + "': AFTER_PARAGRAPHS; ");
  
            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            CallCallback(currentEventPausePair, SentenceEventHandler);
            CallCallback(currentEventPausePair, ParagraphEventHandler);
            Debug.Write(" RuleP1P2P3P4 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleS1S2(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // All events in myCurrentRuleEvents should be marked with BS.
            // The current event should also be marked with BS.
            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];

                if (i == 0)
                {
                    even.Second = PauseLocation.BEFORE_SENTENCES;
                    Debug.Write(" RuleS1S2 for '" + GetKeypressKey(even.First) + "': BEFORE_SENTENCES; ");
                }
                even.Second = i == count - 1 ? PauseLocation.BEFORE_SENTENCES : PauseLocation.AFTER_SENTENCES;
                Debug.Write(" RuleS1S2 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
            }
            currentEventPausePair.Second = PauseLocation.BEFORE_SENTENCES;
            Debug.Write(" RuleS1S2 for '" + GetKeypressKey(currentEventPausePair.First) + "': BEFORE_SENTENCES; ");

            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            CallCallback(currentEventPausePair, SentenceEventHandler);
            Debug.Write(" RuleS1S2 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }


        private void ApplyRuleS4(Pair<Event, PauseLocation> currentEventPausePair)
        {
            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];

                if (i == 0)
                {
                    even.Second = PauseLocation.AFTER_SENTENCES;
                    Debug.Write(" RuleS4 for '" + GetKeypressKey(even.First) + "': AFTER_SENTENCES; ");
                }
                even.Second = i == count - 1 ? PauseLocation.BEFORE_SENTENCES : PauseLocation.AFTER_SENTENCES;
                Debug.Write(" RuleS4 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
            }
            currentEventPausePair.Second = PauseLocation.AFTER_SENTENCES;
            Debug.Write(" RuleS4 for '" + GetKeypressKey(currentEventPausePair.First) + "': AFTER_SENTENCES; ");

            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            CallCallback(currentEventPausePair, SentenceEventHandler);
            Debug.Write(" RuleS4 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }


        private void ApplyRuleS3(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // Not much to do:
            // all events in myCurrentRuleEvents should be marked with BS
            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];

                if (i == 0)
                {
                    even.Second = PauseLocation.AFTER_SENTENCES;
                    Debug.Write(" RuleS3 for '" + GetKeypressKey(even.First) + "': AFTER_SENTENCES; ");
                }
                even.Second = i == count - 1 ? PauseLocation.BEFORE_SENTENCES : PauseLocation.AFTER_SENTENCES;
                Debug.Write(" RuleS3 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
            }

            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            CallCallback(currentEventPausePair, SentenceEventHandler);
            Debug.Write(" RuleS3 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleC2C3(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // The first event should _not_ be marked. Usually there will be no
            // events left. Then the current event should be marked.
            // However often there are "DEL" events following the alnum event.
            // These should also be marked with WW.
            if (CurrentRuleEvents.Count > 0)
            {
                Debug.Write(" RuleC2C3 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
                CurrentRuleEvents.RemoveAt(0);
                foreach (var even in CurrentRuleEvents)
                {
                    even.Second = PauseLocation.WITHIN_WORDS;
                    Debug.Write(" RuleC2C3 for '" + GetKeypressKey(even.First) + "': WITHIN_WORDS; ");
                }
            }
            currentEventPausePair.Second = PauseLocation.WITHIN_WORDS;
            Debug.Write(" RuleC2C3 for '" + GetKeypressKey(currentEventPausePair.First) + "': WITHIN_WORDS; ");
            Debug.Write(" RuleC2C3 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleC4(Pair<Event, PauseLocation> currentEventPausePair)
        {
            Debug.Write(" RuleC4 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);
            bool foundDot = false;

            for (int i = 0; i < CurrentRuleEvents.Count; i++)
            {
                Pair<Event, PauseLocation> eventPair = CurrentRuleEvents[i];
                KeyPress currentKeyPress = GetKeypressFromEvent(eventPair.First);
                if (!foundDot && currentKeyPress.Value == ".")
                {
                    foundDot = true;
                    eventPair.Second = PauseLocation.WITHIN_WORDS;
                    Debug.Write(" RuleC4 for '" + GetKeypressKey(eventPair.First) + "': WITHIN_WORDS; ");
                    continue;
                }
                if (SkipKey(currentKeyPress))
                {
                    eventPair.Second = PauseLocation.WITHIN_WORDS;
                    Debug.Write(" RuleC4 for '" + GetKeypressKey(eventPair.First) + "': WITHIN_WORDS; ");
                    Debug.Write(" RuleC4 remove next: '" + GetKeypressKey(CurrentRuleEvents[i].First) + "'; ");
                    CurrentRuleEvents.RemoveAt(i);
                    i--;
                        // decrease i, as to make sure that in the next iteration, we stay at the same position (= new element)
                    continue;
                }
                if (foundDot &&
                    (Lexical.IsAlphaNumeric(currentKeyPress.Value) 
                    || Lexical.IsWithinWordChar(currentKeyPress.Value)
                    || Lexical.IsSentencePrecedingReadingMark(currentKeyPress.Value)))
                {
                    //foundAlnumWWC = true;            
                    eventPair.Second = PauseLocation.WITHIN_WORDS;
                    Debug.Write(" RuleC4 for '" + GetKeypressKey(eventPair.First) + "': WITHIN_WORDS; ");
                }
            }

            currentEventPausePair.Second = PauseLocation.WITHIN_WORDS;
            Debug.Write(" RuleC4 for '" + GetKeypressKey(currentEventPausePair.First) + "': WITHIN_WORDS; ");

            // Remove the dot, keep the AlphaNumericWithingWordChar
            Debug.Write(" RuleC4 remove finally: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentEventPausePair"></param>
        private void ApplyRuleC5(Pair<Event, PauseLocation> currentEventPausePair)
        {
            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];
                if (i == 0 || i == count - 1 )
                {
                    even.Second = PauseLocation.BEFORE_WORDS;
                    Debug.Write(" RuleC5 for '" + GetKeypressKey(even.First) + "': BEFORE_WORDS; ");
                }
                else
                {
                    even.Second = Lexical.IsSentenceReadingMark(GetKeypressValue(currentEventPausePair.First)) 
                        ? PauseLocation.AFTER_SENTENCES : PauseLocation.WITHIN_WORDS;
                    Debug.Write(" RuleC5 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
                }
            }
                currentEventPausePair.Second = PauseLocation.WITHIN_WORDS;
                Debug.Write(" RuleC5 for current '" + GetKeypressKey(currentEventPausePair.First) + "': WITHIN_WORDS; ");
        }

        private void ApplyRuleW1W2W6(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // The first event should _not_ be marked. Any remaining eventz,
            // if any, should be "DEL" eventz. The current event should be marked with BW.
            if (!Lexical.IsSpace(GetKeypressValue(currentEventPausePair.First)) 
                && !CurrentState.Equals(State.SENTENCE_READING_MARK))
            {
                Debug.Write(" RuleW1W2W6 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
                CurrentRuleEvents.RemoveAt(0);
            }

            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];
                even.Second = i == count - 1 ? PauseLocation.AFTER_WORDS : PauseLocation.BEFORE_WORDS;
                Debug.Write(" RuleW1W2W6 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
                CallCallback(even, WordEventHandler);
            }

            currentEventPausePair.Second = PauseLocation.WITHIN_WORDS;
            //  currentEventPausePair.Second = PauseLocation.BEFORE_WORDS;
            Debug.Write(" RuleW1W2W6 for current " + GetKeypressKey(currentEventPausePair.First) + ": WITHIN_WORDS; ");
            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            Debug.Write(" RuleW1W2W6 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleW3(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // The first event should _not_ be marked. All others from myCurrentRuleEvents
            // should be marked with BW. The current event should also be marked.
            Debug.Write(" RuleW3 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);

            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];
                even.Second = i == count - 1 ? PauseLocation.AFTER_WORDS : PauseLocation.BEFORE_WORDS;
                Debug.Write(" RuleW3 for '" + GetKeypressKey(even.First) + "': " + even.Second +"; ");
            }

            currentEventPausePair.Second = PauseLocation.BEFORE_WORDS;
            Debug.Write(" RuleW3 for current " + GetKeypressKey(currentEventPausePair.First) + ": BEFORE_WORDS; ");
            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            Debug.Write(" RuleW3 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleW4W5(Pair<Event, PauseLocation> currentEventPausePair)
        {
            // The first event should _not_ be marked. Only one other event left.
            Debug.Write(" RuleW4W5 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);
            try
            {
                while (GetKeypressFromEvent(CurrentRuleEvents.First().First).Key == KeysEx.VK_DELETE)
                {
                    CurrentRuleEvents.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION found in 'ApplyRuleW4W5'. Rule skipped " + e);
            }
            CurrentRuleEvents.First().Second = PauseLocation.AFTER_WORDS;
            Debug.Write(" RuleW4W5 for '" + GetKeypressKey(CurrentRuleEvents.First().First)+ "': AFTER_WORDS; ");
            Debug.Write(" RuleW4W5 remove next: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);
            foreach (var eventPair in CurrentRuleEvents)
            {
                eventPair.Second = PauseLocation.AFTER_WORDS;
                Debug.Write(" RuleW4W5 for " + GetKeypressKey(eventPair.First) + ": AFTER_WORDS; ");
            }

            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            Debug.Write(" RuleW4W5 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        private void ApplyRuleW7(Pair<Event, PauseLocation> currentEventPausePair)
        {
            Debug.Write(" RuleW7 remove first: '" + GetKeypressKey(CurrentRuleEvents[0].First) + "'; ");
            CurrentRuleEvents.RemoveAt(0);

            var count = CurrentRuleEvents.Count;
            for (var i = 0; i < count; i++)
            {
                var even = CurrentRuleEvents[i];
                even.Second = i == count - 1 ? PauseLocation.AFTER_WORDS : PauseLocation.BEFORE_WORDS;
                Debug.Write(" RuleW7 for '" + GetKeypressKey(even.First) + "': " + even.Second + "; ");
            }

            currentEventPausePair.Second = PauseLocation.AFTER_WORDS;
            Debug.Write(" RuleW7 for current " + GetKeypressKey(currentEventPausePair.First) + ": AFTER_WORDS; ");
            // notify listeners
            CallCallback(currentEventPausePair, WordEventHandler);
            Debug.Write(" RuleW7 finally: all CurrentRuleEvents removed; ");
            CurrentRuleEvents.Clear();
        }

        #endregion

        #region Matches

        private State MatchAlphaNumericOrWhithinWordCtrlBackspace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            // Rule W5
            Debug.Write("Match AlphaNumeric Or WhithinWord CtrlBackspace: ApplyRuleW4W5; ");
            ApplyRuleW4W5(eventPausePair);
           // eventPausePair.Second = PauseLocation.AFTER_WORDS;
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericOrWithinWordWordReadingMark(Pair<Event, PauseLocation> eventPausePair,string value)
        {
            if (Lexical.IsEnter(value))
            {
                return State.WORD_READING_MARK_ENTER;
            }
            // Rule W4
            Debug.Write("Match AlphaNumeric Or WithinWord-Word ReadingMark: ApplyRuleW4W5; ");
            ApplyRuleW4W5(eventPausePair);
            if (Lexical.IsSpace(value))
            {
                eventPausePair.Second = PauseLocation.BEFORE_WORDS;
            }
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericOrWithinWordTabSpaceBindingTabSpace(Pair<Event, PauseLocation> eventPausePair,string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING_TABSPACE;
            // Rule W7
            Debug.Write("Match AlphaNumeric Or WithinWord Tab Space Binding Tab Space: ApplyRuleW7; ");
            ApplyRuleW7(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericOrWithinWordTabSpaceBinding(Pair<Event, PauseLocation> eventPausePair,
                                                                   string value)
        {
            if (Lexical.IsBindingChar(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING;
            if (Lexical.IsTabOrSpace(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING_TABSPACE;
            // Rule W3
            // Delete the BINDING events
            for (int i = 0; i < CurrentRuleEvents.Count;)
            {
                KeyPress currentKeyPress = GetKeypressFromEvent(CurrentRuleEvents[i].First);
                if (Lexical.IsBindingChar(currentKeyPress.Value))
                {
                    CurrentRuleEvents.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            Debug.Write("Match AlphaNumeric Or WithinWord Tab Space Binding: ApplyRuleW3; ");
            ApplyRuleW3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericOrWithinWordTabSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE;
            if (Lexical.IsEnter(value))
                return State.CHARACTER_ENTER;
            if (Lexical.IsBindingChar(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE_BINDING;
            // Rule W3
            Debug.Write("Match AlphaNumeric Or WithinWord Tab Space: ApplyRuleW3; ");
            ApplyRuleW3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchBindingCharacter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            Debug.Write("Match Binding Character: ApplyRuleC2C3; ");
            ApplyRuleC2C3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericWithinWordCharDotAlphaNumericWithinWordChar(Pair<Event, PauseLocation> eventPausePair,
            string value, KeyPress keyPress)
        {
            ApplyRuleC4(eventPausePair);
            // We're back at ALNUMORWITHINWORD
            if (value == ".")
                return State.ALPHANUMERIC_WITHINWORD_CHAR_DOT;
            if (Lexical.IsTabOrSpace(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE;
            if (Lexical.IsWordReadingMark(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_WORD_READING_MARK;
            if (IsCtrlBackSpace(keyPress))
                return State.ALPHANUMERIC_OR_WITHINWORD_CTRL_BACKSPACE;
            if (Lexical.IsEnter(value))
                return State.CHARACTER_ENTER;
            Debug.Write("Match AlphaNumeric WithinWord Char Dot AlphaNumeric Within WordChar: ApplyRuleC2C3; ");
            ApplyRuleC2C3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private static bool IsCtrlBackSpace(KeyPress keyPress)
        {
            if (keyPress == null) return false;
            var key = keyPress.Key;
            // OR you press a controlkey (left or right) + BACKSPACE
            if ((key == KeysEx.VK_LCONTROL || key == KeysEx.VK_RCONTROL) &&
                keyPress.KeyboardState.Contains(KeysEx.VK_BACK))
            {
                return true;
            }
            // OR you press BACKSPACE + controlkey (left or right)
            if (key == KeysEx.VK_BACK && (keyPress.KeyboardState.Contains(KeysEx.VK_LCONTROL) ||
                                          keyPress.KeyboardState.Contains(KeysEx.VK_RCONTROL)))
            {
                return true;
            }
            return false;
        }

        private State MatchAlphaNumericWithinWordCharDot(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsAlphaNumeric(value) 
                || Lexical.IsWithinWordChar(value)
                || Lexical.IsSentencePrecedingReadingMark(value))
                return State.ALPHANUMERIC_WITHINWORD_CHAR_DOT_ALPHANUMERIC_WITHINWORD_CHAR;

            // Didn't match C4. Still have to count the "ALPHANUMERIC". (Rule C2)
            // Remove any DEL and "." at the end.
            try
            {
                while (GetKeypressFromEvent(CurrentRuleEvents.Last().First).Key == KeysEx.VK_DELETE)
                {
                    CurrentRuleEvents.RemoveAt(CurrentRuleEvents.Count - 1);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION found in 'MatchAlphaNumericWithinWordCharDot'. Rule skipped. " + e);
            }
            Pair<Event, PauseLocation> lastEvent = CurrentRuleEvents.Last();
            // Joris: By first removing and then adding again, we make sure that the last event is not manipulated by
            // applyRuleC2C3(...)
            CurrentRuleEvents.RemoveAt(CurrentRuleEvents.Count - 1);
            Debug.Write("Match AlphaNumeric WithinWord Char Dot: ApplyRuleC2C3; ");
            ApplyRuleC2C3(lastEvent);
            CurrentRuleEvents.Add(lastEvent);

            // Now make it SRM.
            if (NbSentenceChars >= SentenceCharsTreshold && Lexical.IsTabOrSpace(value))
                return State.SENTENCE_READING_MARK_NOT_CHARACTER;
            if (Lexical.IsEnter(value))
                return State.SENTENCE_READING_MARK_ENTER;
            if (NbSentenceChars >= SentenceCharsTreshold && !Lexical.IsCharacter(value))
                return State.SENTENCE_READING_MARK_NOT_CHARACTER;
            if (Lexical.IsSentenceReadingMark(value))
                return State.SENTENCE_READING_MARK;
            // Rule W1
            Debug.Write("Match AlphaNumeric WithinWord Char Dot: ApplyRuleW1W2W6; ");
            ApplyRuleW1W2W6(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchAlphaNumericOrWithinWord(Pair<Event, PauseLocation> eventPausePair, string value,KeyPress keyPress)
        {
            if (value == ".")
                return State.ALPHANUMERIC_WITHINWORD_CHAR_DOT;
            if (Lexical.IsTabOrSpace(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_TABSPACE;
            if (Lexical.IsWordReadingMark(value))
                return State.ALPHANUMERIC_OR_WITHINWORD_WORD_READING_MARK;
            if (IsCtrlBackSpace(keyPress))
                return State.ALPHANUMERIC_WITHINWORD_CTRL_BACK;
            if (Lexical.IsEnter(value))
                return State.CHARACTER_ENTER;
            if (keyPress != null && Lexical.HasShiftKey(keyPress)) return State.CHARACTER_ENTER;
            Debug.Write("Match AlphaNumeric Or Within Word: ApplyRuleC2C3; ");
            ApplyRuleC2C3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchWordReadingMarkEnter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            // Rule S1
            Debug.Write("Match Word ReadingMark Enter: ApplyRuleS1S2; ");
            ApplyRuleS1S2(eventPausePair);
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchWordReadingMarkTabSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            // We don't know yet: either we match rule S1/S2 or we match W2.
            if (Lexical.IsTabOrSpace(value))
                return State.WORD_READING_MARK_TABSPACE;
            if (Lexical.IsEnter(value))
                return State.CHARACTER_ENTER;
            // Rule W2
            // Only keep the first event
            CurrentRuleEvents.RemoveRange(1, CurrentRuleEvents.Count - 1);
            Debug.Write("Match Word ReadingMark Tab Space: ApplyRuleW1W2W6; ");
            ApplyRuleW1W2W6(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchWordReadingMark(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.WORD_READING_MARK_TABSPACE;
            if (Lexical.IsEnter(value))
                return State.WORD_READING_MARK_ENTER;
                //return State.CHARACTER_ENTER;
            // Rule W2
            Debug.Write("Match Word ReadingMark: ApplyRuleW1W2W6; ");
            ApplyRuleW1W2W6(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchSentenceReadingMarkNotCharacter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            // Rule S3
            Debug.Write("Match Sentence ReadingMark Not Character: ApplyRuleS3; ");
            ApplyRuleS3(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchSentenceReadingMarkEnter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnterOrBack(value))
                return State.SENTENCE_READING_MARK_ENTER;

            // Rule P4
            Debug.Write("Match Sentence ReadingMark Enter: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchSentenceReadingMarkTabSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.SENTENCE_READING_MARK_TABSPACE;
            // If it's an ENTER then we matched rule P4
            if (Lexical.IsEnter(value))
                return State.SENTENCE_READING_MARK_ENTER;
            // Otherwise we apply rule S4.
            Debug.Write("Match Sentence ReadingMark Tab Space: ApplyRuleS4; ");
            ApplyRuleS4(eventPausePair);
            return DetermineNextStartState(value);
        }

        private State MatchSentenceReadingMark(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            // Rule W1
            // We want to keep the last SENTENCE_READING_MARK event in the list.
            Pair<Event, PauseLocation> lastEventPausePair = CurrentRuleEvents.Last();
            Debug.Write("Match Sentence ReadingMark: ApplyRuleW1W2W6; ");
            ApplyRuleW1W2W6(eventPausePair);
            CurrentRuleEvents.Add(lastEventPausePair);

            if (Lexical.IsSentenceReadingMark(value))
                return State.SENTENCE_READING_MARK;
            if (NbSentenceChars >= SentenceCharsTreshold && Lexical.IsTabOrSpace(value))
            {
                CurrentRuleEvents.RemoveRange(0, CurrentRuleEvents.Count - 1); // remove all by the last SRM
                return State.SENTENCE_READING_MARK_TABSPACE;
            }
            if (NbParagraphChars >= ParagraphCharsThreshold && Lexical.IsEnter(value))
            {
                CurrentRuleEvents.RemoveRange(0, CurrentRuleEvents.Count - 1); // remove all by the last SRM
                return State.SENTENCE_READING_MARK_ENTER;
            }
            if (NbSentenceChars > SentenceCharsTreshold && !Lexical.IsCharacter(value))
            {
                CurrentRuleEvents.RemoveRange(0, CurrentRuleEvents.Count - 1); // remove all by the last SRM
                return State.SENTENCE_READING_MARK_NOT_CHARACTER;
            }

            return DetermineNextStartState(value);
        }

        private State MatchCharacterEnterSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnter(value))
            {
                PopAllFromCurrentEventListExceptTabSpaceEnter();
                // Rule P3
                return State.ENTER_SPACE2;
            }

            // Rule S2
            Debug.Write(" Match Character Enter Space: ApplyRuleS1S2; ");
            ApplyRuleS1S2(eventPausePair);
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchCharacterEnter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTab(value))
            {
                PopAllFromCurrentEventListExceptTabSpaceEnter();
                return State.ENTER_TAB;
            }
            if (Lexical.IsEnter(value))
            {
                PopAllFromCurrentEventListExceptTabSpaceEnter();
                return State.ENTER_ENTER;
            }
            if (Lexical.IsSpace(value))
            {
                return State.CHARACTER_ENTER_SPACE;
            }

            // Rule S1
            Debug.Write(" Match Character Enter: ApplyRuleS1S2; ");
            ApplyRuleS1S2(eventPausePair);
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private void PopAllFromCurrentEventListExceptTabSpaceEnter()
        {
            // Pop all characters from the front, only keeping
            // the TABs and SPACEs or ENTERs.
            try
            {
                while (CurrentRuleEvents.Count > 0)
                {
                    var value = GetKeypressFromEvent(CurrentRuleEvents[0].First).Value;
                    if (Lexical.IsTabOrSpaceOrEnter(value))
                    {
                        break;
                    }
                    CurrentRuleEvents.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION found in 'PopAllFromCurrentEventListExceptTabSpaceEnter'. Rule skipped " + e);
            }
        }

        private State MatchCharacterTabSpace(string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.CHARACTER_TABSPACE;
            return Lexical.IsEnter(value) ? State.CHARACTER_ENTER : DetermineNextStartState(value);
        }

        private State MatchAnyCharacter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpace(value))
            {
                return State.CHARACTER_TABSPACE;
            }
            if (Lexical.IsEnter(value))
            {
                return State.CHARACTER_ENTER;
            }
            State state = DetermineNextStartState(value);
            if (state == State.ALPHA_NUMERIC_OR_WITHIN_WORD)
            {
                Debug.Write(" Match TabSpace: ApplyRuleC2C3; ");
                ApplyRuleC2C3(eventPausePair);
            }
            return state;

        }

        private State MatchEnterEnter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnterOrBack(value))
            {
                return State.ENTER_ENTER;
            }
            // Rule P1 - P4
            Debug.Write(" Match Enter Enter: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            NbParagraphChars = 0;
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchEnterSpace2(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnter(value))
            {
                return State.ENTER_SPACE2;
            }
            // Rule P1 - P4
            Debug.Write(" Match Enter Space2: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            NbParagraphChars = 0;
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchEnterSpace(string value)
        {
            return Lexical.IsTabOrSpaceOrEnter(value) ? State.ENTER_SPACE2 : DetermineNextStartState(value);
        }

        private State MatchEnterXBackSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnterOrBack(value))
            {
                return State.ENTER_X_BACKSPACE;
            }

            // Rule P1 - P4
            Debug.Write(" Match Enter XBackSpace: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            NbParagraphChars = 0;
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchEnterTab2(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnterOrBack(value))
                return State.ENTER_TAB2;
            // Rule P2
            Debug.Write(" Match Enter Tab2: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            NbParagraphChars = 0;
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchEnterTab(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpaceOrEnter(value))
                return State.ENTER_TAB2;
            if (Lexical.IsBackSpace(value))
                return State.ENTER_X_BACKSPACE;

            // Rule P2
            Debug.Write(" Match Enter Tab: ApplyRuleP1P2P3P4; ");
            ApplyRuleP1P2P3P4(eventPausePair);
            NbParagraphChars = 0;
            NbSentenceChars = 0;
            return DetermineNextStartState(value);
        }

        private State MatchTabSpace(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTabOrSpace(value))
                return State.TABSPACE;
            if (Lexical.IsEnter(value))
                return State.ENTER;
            State state =  DetermineNextStartState(value);
            if (state == State.ALPHA_NUMERIC_OR_WITHIN_WORD)
            {
                Debug.Write(" Match TabSpace: ApplyRuleC2C3; ");
                ApplyRuleC2C3(eventPausePair);
            }
            return state;
        }

        private State MatchEnter(Pair<Event, PauseLocation> eventPausePair, string value)
        {
            if (Lexical.IsTab(value))
                return State.ENTER_TAB;
            if (Lexical.IsSpace(value))
                return State.ENTER_SPACE;
            if (Lexical.IsEnter(value))
                return State.ENTER_ENTER;

            // Rule W6 (single enter followed by anything): delete any non-enter eventz.
            while (true)
            {
                if (CurrentRuleEvents.Count == 0)
                    return State.START; // TODO this should NOT happen! log this
                var eventKeyPress = GetKeypressFromEvent(CurrentRuleEvents.First().First);
                if (eventKeyPress == null)
                    throw new AnalysisException("Found an event without a keypress in the CurrentRuleEvents list.");

                if (Lexical.IsEnter(eventKeyPress.Value))
                {
                    break;
                }
                CurrentRuleEvents.RemoveAt(0); // remove first element
            }
            Debug.Write(" Match Enter: ApplyRuleW1W2W6; ");
            ApplyRuleW1W2W6(eventPausePair);
            return DetermineNextStartState(value);
        }

        private static KeyPress GetKeypressFromEvent(Event even)
        {
            return even.Parts.OfType<KeyPress>().FirstOrDefault();
        }

        private static string GetKeypressKey(Event even)
        {
            var keyPress = GetKeypressFromEvent(even);
            return keyPress != null ? keyPress.Key.ToString() : string.Empty;
        }

        private static string GetKeypressValue(Event even)
        {
            var keyPress = GetKeypressFromEvent(even);
            return keyPress != null ? keyPress.Value : string.Empty;
        }
        #endregion
    }
}