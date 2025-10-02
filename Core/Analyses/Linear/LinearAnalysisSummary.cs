using System;
using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Linear
{
    /// <summary>
    ///     Summary of the Linear analysis.
    ///     Contains a list of Periods. Each Period contains Events that represent a keypress or pause.
    /// </summary>
    public class LinearAnalysisSummary : AbstractAnalysisSummary
    {
        /// <summary>
        ///     Construct the summary.
        /// </summary>
        public LinearAnalysisSummary()
        {
            Periods = new List<AbstractPeriod>();
        }

        /// <summary>
        ///     List of periods contained within the summary.
        /// </summary>
        public List<AbstractPeriod> Periods { private set; get; }

        /// <summary>
        ///     Abstract period is the basic type of period within a linear analysis.
        ///     The type will vary depending on how the analysis will be divided
        ///     into periods.
        /// </summary>
        public abstract class AbstractPeriod
        {
            #region fields

            /// <summary>
            ///     List of events that occured within this period.
            /// </summary>
            private readonly List<Event> PeriodEvents;

            /// <summary>
            ///     The number of events currently in this period.
            /// </summary>
            public int Count
            {
                get { return PeriodEvents.Count; }
            }

            /// <summary>
            ///     Identifier for the period.
            /// </summary>
            public virtual string ID
            {
                get { return ""; }
            }

            /// <summary>
            ///     Start time of the period.
            /// </summary>
            public virtual ulong PeriodStartTime
            {
                get { return 0; }
            }

            /// <summary>
            ///     Link to the start of the period in the General Analaysis file (currently not used).
            ///     FUTURE: implement this
            /// </summary>
            public virtual string Link
            {
                get { return ""; }
            }

            public ulong StartTime;
            public ulong EndTime;
            public bool First;

            #endregion

            /// <summary>
            ///     Construct an abstract period.
            /// </summary>
            protected AbstractPeriod()
            {
                First = true;
                StartTime = 0;
                EndTime = 0;
                PeriodEvents = new List<Event>();
            }

            /// <summary>
            ///     Creates a new Period that is the followup period after this one.
            ///     It is based on the content within this period.
            /// </summary>
            /// <returns>
            ///     A new period object that represents the new period following
            ///     up this period.
            /// </returns>
            public abstract AbstractPeriod CreateFollowingPeriod();

            /// <summary>
            ///     Checks whether a certain event still belongs to a period or
            ///     whether it does not belong to this period.
            /// </summary>
            /// <param name="e">Event that might belong to this period.</param>
            /// <returns>True if 'e' belongs to this period, false if it does not.</returns>
            public abstract bool BelongsTo(Events.Event e);

            ////////////////////////////////////////////////////////////////////////////
            // List functions
            //

            /// <summary>
            ///     Add an event to the period. Focus Events are not added
            ///     to the list of events in a period and are ignored.
            ///     Override this method to change this behaviour.
            /// </summary>
            /// <param name="even">Event to add to the period.</param>
            public virtual void Add(Event even)
            {
                switch (even.GetType().Name)
                {
                    case "CountableEvent":
                    case "PauseEvent":
                    case "SpecialEvent":
                    case "RegularEvent":
                        PeriodEvents.Add(even);
                        break;

                  // FocusEvents and unknown events are not added to the 
                  // PeriodEvents list but ignored.
                }
            }

            public void Add(Event even, ulong startTime, ulong endTime)
            {
                if (First) //No times set yet
                {
                    First = false;
                    StartTime = startTime;
                }
                EndTime = endTime;
                Add(even);
            }

            /// <summary>
            ///     Remove event at given position.
            /// </summary>
            /// <param name="index">Index of event to remove.</param>
            public void RemoveAt(int index)
            {
                PeriodEvents.RemoveAt(index);
            }

            /// <summary>
            ///     Periods are enumerable. If you wish to loop over all of the events of a
            ///     period you can just use it in a foreach loop.
            /// </summary>
            /// <returns>The current event in the list.</returns>
            public IEnumerator<Event> GetEnumerator()
            {
                return ((IEnumerable<Event>) PeriodEvents).GetEnumerator();
            }

            public virtual ulong Length()
            {
                if (EndTime < StartTime)
                    return 1;
                return EndTime - StartTime;
            }


        }

        /// <summary>
        ///     A Countable event is an event that occured multiple times in the logging file.
        ///     (e.g. an key that was pressed multiple times).
        /// </summary>
        public class CountableEvent : Event
        {
            /// <summary>
            ///     Constructs the CountableEvent.
            /// </summary>
            /// <param name="count">Count of the event (=how many times it occured).</param>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            protected CountableEvent(ulong count, int id = -1) :
                base(id)
            {
                Count = count;
            }

            /// <summary>
            ///     Count of the event (=how many times it occured).
            /// </summary>
            public ulong Count { private set; get; }
        }

        /// <summary>
        ///     ABC for events.
        ///     NOTE: If you add a new event type make sure to change the case in the Add() method
        ///     of the AbstractBase class, as it checks on type to check whether or not to add
        ///     events of this sort to the PeriodEvents list.
        /// </summary>
        public abstract class Event
        {
            #region public_datamembers

            /// <summary>
            ///     The inputlog eventID of the original event.
            /// </summary>
            private int EventID;

            #endregion

            /// <summary>
            ///     Construct an event.
            /// </summary>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            protected Event(int id)
            {
                EventID = id;
            }
        }

        /// <summary>
        ///     A focus in normal periods is not added to the event list. But it does help
        ///     the periods to divide other events based on focus grouping.
        /// </summary>
        public class FocusEvent : Event
        {
            /// <summary>
            ///     Create a focus event.
            /// </summary>
            /// <param name="currentWindow">Title of the current window in focus</param>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            public FocusEvent(string currentWindow, int id = -1) :
                base(id)
            {
                CurrentWindow = currentWindow;
            }

            /// <summary>
            ///     Title of the currently active window, that is the window that is
            ///     selected in this focus event.
            /// </summary>
            public String CurrentWindow { get; private set; }
        }

        /// <summary>
        ///     A focus period is a period that groups all the events within a single focus
        ///     window. As soon as a focus to another window occurs we enter a new 'period'.
        /// </summary>
        public class FocusPeriod : AbstractPeriod
        {
            #region public_datamembers

            /// <summary>
            ///     The title of the current focus window.
            ///     If the windowTitle is null
            /// </summary>
            private string CurrentWindow { get; set; }

            /// <summary>
            ///     Returns the ID of the current focus period. This is the title
            ///     of the focus window.
            /// </summary>
            public override string ID
            {
                get { return CurrentWindow; }
            }

            #endregion

            #region protected_datamembers

            /// <summary>
            ///     Holds the title of the next focus window.
            /// </summary>
            private string TitleOfNextWindow { get; set; }

            #endregion

            /// <summary>
            ///     Create a focusPeriod. If the currentWindowTitle is not yet known,
            ///     as will most likely be the case for the first FocusPeriod, it doesn't
            ///     have to be passed. It will be set as soon as the first focus period
            ///     is added to the period.
            /// </summary>
            /// <param name="currentWindow">
            ///     The windowtitle of the currently active window, if known,
            ///     if it is not known, this parameter may be null.
            /// </param>
            public FocusPeriod(string currentWindow = null)
            {
                CurrentWindow = currentWindow;
            }

            /// <summary>
            ///     Create a new FocusPeriod based on the previous focus period.
            ///     This will automatically have the correct window title set for
            ///     this FocusPeriod.
            /// </summary>
            /// <param name="previousPeriod">The previous FocusPeriod</param>
            private FocusPeriod(FocusPeriod previousPeriod)
            {
                CurrentWindow = previousPeriod.TitleOfNextWindow;
            }

            /// <summary>
            ///     Creates a new Period that is the followup period after this one.
            ///     It is based on the content within this period.
            /// </summary>
            /// <returns>
            ///     A new period object that represents the new period following
            ///     up this period.
            /// </returns>
            public override AbstractPeriod CreateFollowingPeriod()
            {
                return new FocusPeriod(this);
            }

            /// <summary>
            ///     Checks whether an event belongs to a FocusPeriod, this is always the case
            ///     except when the event itself is a new focus event, this announces the beginning
            ///     of a new focusPeriod, unless this is the first FocusPeriod and no focus event
            ///     has yet passed. This would mean that the CurrentWindowTitle is still not set.
            /// </summary>
            /// <param name="e">Event that might belong to this period.</param>
            /// <returns>True if 'e' belongs to this period, false if it does not.</returns>
            public override bool BelongsTo(Events.Event e)
            {
                if (e.Type != EventType.FOCUS ||
                    (e.Type == EventType.FOCUS && CurrentWindow == null))
                {
                    return true;
                }

                // We have to set the NextWindowTitle, this will be the windowTitle of this
                // new focus event. 
                var focusPart = Events.Event.GetFirstEventPart<FocusChange>(e);
                if (focusPart.WindowTitle == CurrentWindow)
                {
                    return true;
                }

                TitleOfNextWindow = focusPart.WindowTitle;
                return false;
            }

            /// <summary>
            ///     Add an event to the FocusPeriod, this event can only be a focus event in one case:
            ///     this is the first FocusPeriod and it does not yet have a WindowTitle set, in all other
            ///     cases, a focusEvent can never be added to the a FocusPeriod.
            /// </summary>
            /// <param name="even">Event to be added to the FocusPeriod</param>
            public override void Add(Event even)
            {
                var @event = even as FocusEvent;
                if (@event != null)
                {
                    // Case 1: This is the first focus period, but we didn't find a focus event
                    // yet, hence we didn't know which window title to use yet, however, this one
                    // is the first focus event in the log. We use this focus events title as 
                    // the title of this period's windowFocus.
                    if (CurrentWindow == null)
                    {
                        CurrentWindow = @event.CurrentWindow;
                    }
                    else
                    {
                        if (CurrentWindow != @event.CurrentWindow)
                        {
                            throw new ArgumentException("Added FocusEvent does not belong in this FocusPeriod!");
                        }
                        // Else we just ignore the event. We don't add focus events.
                    }
                }
                else
                {
                    base.Add(even);
                }
            }
        }

        /// <summary>
        ///     PauseEvent represents a pause between 2 events in the logfile that was larger than a given pausethreshold.
        /// </summary>
        public class PauseEvent : Event
        {
            /// <summary>
            ///     Constructs a PauseEvent.
            /// </summary>
            /// <param name="pauseTime">The actual pausetime.</param>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            public PauseEvent(ulong pauseTime, int id = -1) :
                base(id)
            {
                PauseTime = pauseTime;
            }

            /// <summary>
            ///     The actual pausetime.
            /// </summary>
            public ulong PauseTime { private set; get; }
        }

        /// <summary>
        ///     A regular event (most events are regular events).
        ///     This are typically normal keystrokes, e.g.: a, b, c, d, SPACE, etc
        /// </summary>
        public class RegularEvent : Event
        {
            /// <summary>
            ///     Constructs a RegularEvent.
            /// </summary>
            /// <param name="value">The string representation of the event.</param>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            public RegularEvent(IEnumerable<char> value, int id = -1) :
                base(id)
            {
                Value = StringUtils.ReplaceNonPrintableCharacters(value);
            }

            /// <summary>
            ///     The string representation of the event.
            /// </summary>
            public String Value { private set; get; }
        }

        /// <summary>
        ///     This event represents a replacement. Replacements are not shown in the
        ///     condensed format of the Linear Analysis, but Inserts are, hence the need
        ///     for this special class.
        /// </summary>
        public class ReplaceEvent : Event
        {
            public ReplaceEvent(IEnumerable<char> value, int id = -1)
                : base(id)
            {
                Value = StringUtils.ReplaceNonPrintableCharacters(value);
            }

            /// <summary>
            ///     The string representation of the event.
            /// </summary>
            public String Value { private set; get; }
        }

        public class RevisionPeriod : AbstractPeriod
        {
            #region public_datamembers

            /// <summary>
            ///     Returns the ID of the current period. This is the type of the revision
            ///     that constitutes this period.
            /// </summary>
            public override string ID
            {
                get
                {
                    return Revisions[CurrentRevisionIndex].RevisionNumber + ": " +
                           Revisions[CurrentRevisionIndex].Type.ToString();
                }
            }

            #endregion

            #region protected_datamembers

            /// <summary>
            ///     The revisions of the idfx file. These will be used to map events to a revision
            /// </summary>
            private readonly List<IRevision> Revisions;

            #endregion

            #region private_datamembers

            /// <summary>
            ///     The index of the current revision in the revisions list.
            /// </summary>
            private readonly int CurrentRevisionIndex;

            /// <summary>
            ///     The id of the first event in the current revision
            /// </summary>
            private int LowerBoundID;

            /// <summary>
            ///     The id of the last event in the current revision.
            /// </summary>
            private int UpperBoundID;

            #endregion

            /// <summary>
            ///     Create a new RevisionPeriod, this requires some extensive information to be able to firstly perform
            ///     a revisionAnalysis
            /// </summary>
            /// <param name="unfilteredEvents">The list of events, without any additional non-user defined filtering executed on it.</param>
            /// <param name="sessionID">Session information pertaining to the list of events.</param>
            /// <param name="docPath">Path to the original document</param>
            /// <param name="pauseTreshold">Pause Treshold</param>
            /// <param name="controlKeys">List of all special control keys, they may be rendered differently</param>
            public RevisionPeriod(List<Events.Event> unfilteredEvents, SessionIdentification sessionID, string docPath = null,
                                  ulong pauseTreshold = 0ul, List<KeysEx> controlKeys = null)
            {
                try
                {
                    var analysis = new RevisionAnalysis(unfilteredEvents, sessionID, docPath, pauseTreshold, controlKeys);
                    Revisions = ((RevisionAnalysisSummary) analysis.DoAnalysis()).Revisions;
                }
                catch (Exception)
                {
                    throw new AnalysisException(
                        "Could not construct revision analysis of document. Therefore we can not construct" +
                        "a revision based linear analysis of this logging file.");
                }
                CurrentRevisionIndex = 0;
                Initialize();
            }

            /// <summary>
            ///     Construct a new RevisionPeriod from the preceding RevisionPeriod.
            /// </summary>
            /// <param name="previous">The preceding RevisionPeriod</param>
            private RevisionPeriod(RevisionPeriod previous)
            {
                Revisions = previous.Revisions;
                CurrentRevisionIndex = previous.CurrentRevisionIndex + 1;

                Initialize();
            }

            /// <summary>
            ///     Initialize this period with the
            /// </summary>
            private void Initialize()
            {
                // Load initial revision information
                if (Revisions.Count > CurrentRevisionIndex)
                {
                    IRevision currentRev = Revisions[CurrentRevisionIndex];
                    LowerBoundID = currentRev.Edits[0].Id;
                    UpperBoundID = currentRev.Edits[currentRev.Edits.Count - 1].Id;
                }
                else
                {
                    if (Revisions.Count == 0)
                    {
                        throw new AnalysisException(
                            "Can not perform a LinearAnalysis of RevisionType if the logging file has no revisions.");
                    }
                    throw new AnalysisException("Can not create more RevisionPeriods in a LinearAnalysis file " +
                                                "than the number of revisions performed in the logging file.");
                }
            }

            /// <summary>
            ///     Creates a new Period that is the followup period after this one.
            ///     It is based on the content within this period.
            /// </summary>
            /// <returns>
            ///     A new period object that represents the new period following
            ///     up this period.
            /// </returns>
            public override AbstractPeriod CreateFollowingPeriod()
            {
                return new RevisionPeriod(this);
            }

            /// <summary>
            ///     Checks whether a certain event still belongs to a period or
            ///     whether it does not belong to this period.
            /// </summary>
            /// <param name="e">Event that might belong to this period.</param>
            /// <returns>True if 'e' belongs to this period, false if it does not.</returns>
            public override bool BelongsTo(Events.Event e)
            {
                int eventId = int.Parse(e.Properties["id"]);
                return eventId >= LowerBoundID && eventId <= UpperBoundID;
            }
        }

        /// <summary>
        ///     A special event (determined by the user). E.g. Certain control keys are typically considered
        ///     to be special events (RETURN, CTRL, LSHIFT, etc).
        ///     A special always has a count.
        /// </summary>
        public class SpecialEvent : CountableEvent
        {
            /// <summary>
            ///     The string representation of the event. (e.g.: RETURN).
            /// </summary>
            public readonly string Value;

            /// <summary>
            ///     Constructs a SpecialEvent.
            /// </summary>
            /// <param name="value">The string representation of the event..</param>
            /// <param name="count">The count of the event.</param>
            /// <param name="id">The id of the corresponding inputlog event.</param>
            public SpecialEvent(string value, ulong count, int id = -1)
                : base(count, id)
            {
                Value = value;
            }
        }

        /// <summary>
        ///     Period within the analysis.
        /// </summary>
        public class TimeBasedPeriod : AbstractPeriod
        {
            #region public_data members

            /// <summary>
            ///     Types of TimeBasedPeriods. Either the length of each interval
            ///     is fixed, or it is decided based on the endTime of the last event
            ///     and the number of intervals, which in that case is the fixed variable.
            /// </summary>
            public enum TimeType
            {
                FIXED_INTERVAL,
                FIXED_TIME
            };

            /// <summary>
            ///     Identifier string for the TimePeriod.
            /// </summary>
            public override string ID
            {
                get { return DateTimeUtils.MsecToClockString(StartTime, false); }
            }

            /// <summary>
            ///     The start time of the period for a TimeBasedPeriod corresponds to the
            ///     actual starting time of the period, given by the StartTime datamember.
            /// </summary>
            public override ulong PeriodStartTime
            {
                get { return StartTime; }
            }

            /// <summary>
            ///     The size of a single interval.
            /// </summary>
            private ulong IntervalSize { get; set; }

            #endregion

            #region protected_data members

            /// <summary>
            ///     The start offset for the logging time in the log file.
            /// </summary>
            private ulong StartOffset { get; set; }

            /// <summary>
            ///     The number of intervals in total, based on endtime.
            /// </summary>
            private ulong NumberOfIntervals { get; set; }

            /// <summary>
            ///     Remebers the how-mannieth interval this is.
            /// </summary>
            private ulong CurrentInterval { get; set; }

            #endregion

            /// <summary>
            ///     Constructs a new Period with the basic information. If this is a period that follows an already
            ///     existing period, use the other constructor.
            /// </summary>
            /// <param name="startTime">Starttime of this period.</param>
            /// <param name="startOffset">Offset in time before the logging actually starts.</param>
            /// <param name="numberOfIntervals">The number of intervals, this may be 0 if the the intervalSize
            ///  is given and fixed.</param>
            /// <param name="intervalSize">The size of a single interval, this may be 0 if the number of intervals
            ///  is given and fixed.</param>
            /// <param name="endTime">Endtime of the last timedEvent within the event list.</param>
            /// <param name="type">Type of timebased periods we are using, either fixedIntervalTime, or fixedNumberOfIntervals</param>
            /// <param name="link">Link to the start of the period in the General Analaysis file (currently not used).</param>
            public TimeBasedPeriod(ulong startTime, ulong startOffset, ulong numberOfIntervals, ulong intervalSize,
                                   ulong endTime, TimeType type, string link = "")
            {
                StartTime = startTime;
                StartOffset = startOffset;
                CurrentInterval = 0;

                // Determine the rest of the information based on the TimeBasedPeriod.TimeType.
                switch (type)
                {
                    case TimeType.FIXED_INTERVAL:
                        IntervalSize = intervalSize;
                        if (IntervalSize == 0) NumberOfIntervals = 0;
                        else NumberOfIntervals = (ulong) Math.Ceiling((endTime - StartOffset)/(double) IntervalSize);
                        break;

                    case TimeType.FIXED_TIME:
                        NumberOfIntervals = numberOfIntervals;
                        IntervalSize = NumberOfIntervals == 0 ? 0 : 
                            Convert.ToUInt64(Math.Ceiling((endTime - StartOffset)/(double)NumberOfIntervals));
                        break;

                    default:
                        throw new ArgumentException("TimeBasedPeriod type not recognized.");
                }
            }

            /// <summary>
            ///     Construct a new 'follow up' TimeBasedPeriod based on the preceding
            ///     TimeBasedPeriod. Relevant information is taken over, and other information
            ///     is altered to correctly make this the period following the previous TimeBasedPeriod.
            /// </summary>
            /// <param name="previous">The preceding TimeBasedPeriod.</param>
            private TimeBasedPeriod(TimeBasedPeriod previous)
            {
                // Copy information
                StartTime = previous.StartTime + previous.IntervalSize; // =follow-up interval
                StartOffset = previous.StartOffset;
                CurrentInterval = previous.CurrentInterval + 1; // =follow-up interval.
                IntervalSize = previous.IntervalSize;
                NumberOfIntervals = previous.NumberOfIntervals;
            }

            public override ulong Length()
            {
                return IntervalSize;
            }

            /// <summary>
            ///     Creates a new Period that is the followup period after this one.
            ///     It is based on the content within this period.
            /// </summary>
            /// <returns>
            ///     A new period object that represents the new period following
            ///     up this period.
            /// </returns>
            public override AbstractPeriod CreateFollowingPeriod()
            {
                if (CurrentInterval == NumberOfIntervals-1) return null;
                return new TimeBasedPeriod(this);
            }

            /// <summary>
            ///     Checks whether a certain event still belongs to a period or
            ///     whether it does not belong to this period.
            /// </summary>
            /// <param name="e">Event that might belong to this period.</param>
            /// <returns>True if 'e' belongs to this period, false if it does not.</returns>
            public override bool BelongsTo(Events.Event e)
            {
                var timedEventPart = Events.Event.GetFirstEventPart<TimedEventPart>(e);
                if (timedEventPart == null)
                {
                    return true;
                }
                return (timedEventPart.EndTime == 0 || (timedEventPart.StartTime - StartOffset) <
                        ((CurrentInterval + 1)*IntervalSize));
            }
        }

        /// <summary>
        ///     A focus period is a period that groups all the events within a single focus
        ///     window. As soon as a focus to another window occurs we enter a new 'period'.
        /// </summary>
        public class PausePeriod : AbstractPeriod
        {
            #region datamembers

            private bool PauseEncountered;

            /// <summary>
            ///     Identifier string for the PausePeriod.
            /// </summary>
            public override string ID
            {
                get { return "Pause " + Nr + " (" + Time + " ms)"; }
            }

            private readonly int Nr;
            private ulong Time;

            #endregion

            /// <summary>
            ///     Create a focusPeriod. If the currentWindowTitle is not yet known,
            ///     as will most likely be the case for the first FocusPeriod, it doesn't
            ///     have to be passed. It will be set as soon as the first focus period
            ///     is added to the period.
            /// </summary>
            public PausePeriod(int nr)
            {
                PauseEncountered = false;
                Nr = nr;
            }

            /// <summary>
            ///     Creates a new Period that is the followup period after this one.
            ///     It is based on the content within this period.
            /// </summary>
            /// <returns>
            ///     A new period object that represents the new period following
            ///     up this period.
            /// </returns>
            public override AbstractPeriod CreateFollowingPeriod()
            {
                return new PausePeriod(Nr+1);
            }

            /// <summary>
            ///     Checks whether an event belongs to a FocusPeriod, this is always the case
            ///     except when the event itself is a new focus event, this announces the beginning
            ///     of a new focusPeriod, unless this is the first FocusPeriod and no focus event
            ///     has yet passed. This would mean that the CurrentWindowTitle is still not set.
            /// </summary>
            /// <param name="e">Event that might belong to this period.</param>
            /// <returns>True if 'e' belongs to this period, false if it does not.</returns>
            public override bool BelongsTo(Events.Event e)
            {
                return !PauseEncountered;
            }

            /// <summary>
            ///     Add an event to the FocusPeriod, this event can only be a focus event in one case:
            ///     this is the first FocusPeriod and it does not yet have a WindowTitle set, in all other
            ///     cases, a focusEvent can never be added to the a FocusPeriod.
            /// </summary>
            /// <param name="even">Event to be added to the FocusPeriod</param>
            public override void Add(Event even)
            {
                var @event = even as PauseEvent;
                if (@event != null)
                {
                    PauseEncountered = true;
                    Time = @event.PauseTime;
                }
                else
                {
                    base.Add(even);
                }
            }
        }
    }
}