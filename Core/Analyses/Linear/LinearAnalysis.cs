using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using log4net;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Linear
{
    /// <summary>
    /// A Linear Analysis.
    /// The linear analysis gives a concise (linear) representation of the logged events, 
    /// partitioned in different intervals.
    /// The user must choose between either a fixed number of intervals or fixed length intervals.
    /// 
    /// This algorithm will add periods to a summary and periodEvents to the periods. 
    /// There are different types of periodEvents:
    /// * regular events: simple keyboard characters
    /// * special events: special keys (CTRL, SHIFT, etc), non-keyboard events.
    /// * pause events: when the user waits for a pause > pausethreshold, a pause event is added
    /// 
    /// Special events contain the both the value of the event as the count (that is, 
    /// if x same special events happen after each other, 
    /// only 1 special event will be added to the period, but the count of that 
    /// event will be x). Special events are typically represented differently in the analysis output.
    /// </summary>
    public class LinearAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// Different types of linear analysis.
        /// Primarily indicates how the interval size is determined.
        /// </summary>
        public enum TYPE
        {
            //INTERVAL_PER_EVENT, // LEGACY Didn't really find a good example of this, not sure whether this is ever used.
            FIXED_NUMBER_OF_INTERVALS, 
			FIXED_LENGTH_INTERVALS,
			FOCUS_INTERVALS, // Intervals by focus events
			REVISION_INTERVALS, // Intervals corresponding to the different revisions.
            PAUSE_INTERVALS,
        }

        /// <summary>
        /// The used pause threshold (how long the pause between 2 events should be before we actually
        ///  mark it as a pause).
        /// </summary>
        private readonly ulong PauseThreshold;

        /// <summary>
        /// The type of linear analysis that is performed (using a fixed number of intervals, fixed interval length, ...).
        /// </summary>
        private readonly TYPE ThisType;

        private string ThisCurrentSpecialKey;
        /// <summary>
        /// Sets/Gets the current special key.
        /// When setting the CurrentSpecialKey to the same value it already has, the CurrentSpecialKeyCount is increased.
        /// When setting it to a different value, the CurrentSpecialKeyCount is resetted to 1.
        /// </summary>
        private string CurrentSpecialKey
        {
            set
            {
                if (ThisCurrentSpecialKey == null)
                {
                    ThisCurrentSpecialKey = value;
                    CurrentSpecialKeyCount = 1;
                }
                else
                {
                    if (ThisCurrentSpecialKey == value)
                    {
                        CurrentSpecialKeyCount++;
                    }
                    else
                    {
                        ThisCurrentSpecialKey = value;
                        CurrentSpecialKeyCount = 1;
                    }
                }
            }
            get
            {
                return ThisCurrentSpecialKey;
            }
        }

        /// <summary>
        /// Number of times the CurrentSpecialKey was set to the same key.
        /// </summary>
        private ulong CurrentSpecialKeyCount;

        /// <summary>
        /// The start time for the first event.
        /// </summary>
        private ulong FirstStartTime;
        /// <summary>
        /// List of keys that is considered to be control keys (e.g. SHIFT, CTRL). 
        /// When a keyboard event contains one of these control keys, it can be represented differently in the summary.
        /// </summary>
        private readonly List<KeysEx> ControlKeys;

        /// <summary>
        /// Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of linear analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that interval size in msec.
        /// e.g. when the typed is "fixed number of intervals", the TypeParam will represent the number of intervals.
        /// </summary>
        private readonly ulong TypeParam;

		/// <summary>
		/// Dictionary containing extra, linearAnalysisType-depending parameters.
		/// </summary>
		private readonly Dictionary<string,object> Parameters;

        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event types we wish to do the analysis on (all other types are discarded).
        /// </summary>
        private static readonly string[] EventTypes = {EventType.KEYBOARD, EventType.MOUSE, EventType.INSERT, 
                                                 EventType.REPLACEMENT, EventType.FOCUS };

        /// <summary>
        /// Flag to indicate if the previous or current output contains a composite key
        /// </summary>
        private static bool _previousIsComposite;
        private static bool _currentIsComposite;
        #endregion

        /// <summary>
        /// Constructs a new LinearAnalysis. 
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
		/// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="linearAnalysisThisType">The type of linear analysis that is performed 
        /// (using a fixed number of intervals, fixed interval length, ...).</param>
        /// <param name="pauseThreshold">The used pause threshold (how long the pause between 
        /// 2 events should be before we actually mark it as a pause).</param>
        /// <param name="typeParam">Parameter that is used to initialize the analysis.
        /// This parameter is interpreted differently depending on the type of linear analysis.
        /// e.g. when the type is "fixed interval length", the TypeParam will represent that interval size in msec.
        /// e.g. when the typed is "fixed number of intervals",
        ///  the TypeParam will represent the number of intervals.</param>
        /// <param name="controlKeys">List of controlKeys that need to be taken into account 
        /// (controlkeys are displayed differently in the analysis).</param>
		/// <param name="extraParameters">Dictionary containing extra parameters depending on the linearAnalysisType</param>
        public LinearAnalysis(List<Event> events, SessionIdentification sessionID, TYPE linearAnalysisThisType, ulong pauseThreshold,
            ulong typeParam, List<KeysEx> controlKeys = null, Dictionary<string,object> extraParameters = null)
            : base("LA", events, sessionID)
        {
			if (linearAnalysisThisType == TYPE.REVISION_INTERVALS && extraParameters != null)
			{
				extraParameters.Add("unfiltered_events", events);
			} 
			else 
			{
				extraParameters = new Dictionary<string,object>();
			}
			Parameters = extraParameters;
			InputEvents = events.Filter(new EventTypeFilter(EventTypes));     
            PauseThreshold = pauseThreshold;
            ThisType = linearAnalysisThisType;
            TypeParam = typeParam;
            ControlKeys = controlKeys;
        }

        /// <summary>
        /// Performs the actual Linear Analysis on the events with which this Analysis was constructed.
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            Event prevEvent = null;
            var summary = new LinearAnalysisSummary();

            // Aborts process when TimedEventPart is missing.
            if (null != Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]))
            {
                FirstStartTime = Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]).StartTime;
            }
            else
            {
                return null;
            }

			Initialize();

            LinearAnalysisSummary.AbstractPeriod period = CreateInitialPeriod();
			
			// Boolean makes sure that the:
			// 1. The first period is added and not overwritten before it is saved
			// 2. The last period isn't saved two times.
			bool periodAlreadyAdded = false;

            // iterate over all events
            foreach (var even in InputEvents)
            {
                try
                {
                    var timedEventPart = Event.GetFirstEventPart<TimedEventPart>(even);

					// Add a pause event if the current event is timed, and the pause time is larger than
					// the pause treshold
					//
                    if (timedEventPart != null)
                    {
						// if the pauseTime is larger than the threshold, add a periodEvent with the pauseTime.
						var pauseTime = DeterminePauseTime(prevEvent, even);
						if (prevEvent == null)
						{
						    if (pauseTime > NewStartOffset)
						    {
						        pauseTime -= NewStartOffset;
						    }
						    else
						    {
                                pauseTime = Event.GetFirstEventPart<TimedEventPart>(InputEvents[0]).EndTime - NewStartOffset;
						    }
						}
						if (pauseTime >= PauseThreshold)
						{
							// first add the currentspecial key event, then add the pause event
							AddSpecialKeyIfNotNull(period);
							period.Add(new LinearAnalysisSummary.PauseEvent(pauseTime), 
                                timedEventPart.StartTime, timedEventPart.EndTime);
						}
					}
                    else
                    {
                        Console.WriteLine();
                    }

					// Check if the event belongs to the period, otherwise add new periods until we 
					// reach the interval the current event is part off.
					while(!period.BelongsTo(even))
					{
                        AddSpecialKeyIfNotNull(period);
						// If the current period hasn't been saved yet, save it before we start
						// overwriting it.
						if (!periodAlreadyAdded)
						{
							summary.Periods.Add(period);
						}

						period = period.CreateFollowingPeriod();
                        if (period == null)
                        {
                            break;
                        }
						summary.Periods.Add(period);

						periodAlreadyAdded = true;
					}
                    if (period == null)
                    {
                        break;
                    }
					// first add the currentspecial key event, then add the pause event
					AddSpecialKeyIfNotNull(period);
					ProcessEvent(even, period);

                    prevEvent = even;
                }
                catch (AnalysisException e)
                {
                    Log.Warn(e); // exception while analyzing an event, log it and skip it...
                }
            }
            // add last period to summary
			if (!periodAlreadyAdded)
			{
				summary.Periods.Add(period);
			}
            return summary;
        }

	
		/// <summary>
		/// Create the first period to add events too, this period is constructed depending on the 
		/// type of grouping is requested in the linear analysis. By Interval, Fixed FixedSizeInterval, focus
		/// or revisions. After the initial period has been created, subsequent periods can be 
		/// created based on each other to create a chain of periods.
		/// </summary>
		/// <returns>The first period for the linear analysis summary, its type depending on the
		/// requested sort of grouping for the linear parts.</returns>
		private LinearAnalysisSummary.AbstractPeriod CreateInitialPeriod()
		{

			switch (ThisType)
			{
				case TYPE.FIXED_LENGTH_INTERVALS:
				case TYPE.FIXED_NUMBER_OF_INTERVALS:

					// Find the last event that is a timedEventPart,
					TimedEventPart lastEventTimedPart = null;
					var i = InputEvents.Count - 1;
					while (i >= 0)
					{
						lastEventTimedPart = Event.GetFirstEventPart<TimedEventPart>(InputEvents[i]);
						if (lastEventTimedPart != null) break;
						i--;
					}
					// throw exception if there is not such event,
					if (lastEventTimedPart == null)
					{
						Exception e = new AnalysisException("No TimedEvent Part found in the given event list");
						Log.Warn(e);
						throw e;
					}
					var endTime = lastEventTimedPart.EndTime;

					// determine the intervalsize/number of intervals (depending on the type of linear analysis).
					switch (ThisType)
					{
						case TYPE.FIXED_LENGTH_INTERVALS:
                            return new LinearAnalysisSummary.TimeBasedPeriod(0, 
                                FirstStartTime, 0, 
								TypeParam, endTime, LinearAnalysisSummary.TimeBasedPeriod.TimeType.FIXED_INTERVAL);

						case TYPE.FIXED_NUMBER_OF_INTERVALS:
                            return new LinearAnalysisSummary.TimeBasedPeriod(0, FirstStartTime, TypeParam, 
								0, endTime, LinearAnalysisSummary.TimeBasedPeriod.TimeType.FIXED_TIME);
					}
					break;
				case TYPE.FOCUS_INTERVALS:
					return new LinearAnalysisSummary.FocusPeriod();

				case TYPE.REVISION_INTERVALS:
					return new LinearAnalysisSummary.RevisionPeriod((List<Event>)Parameters["unfiltered_events"],
                        SessionIdentification, Parameters["docPath"].ToString(), PauseThreshold, ControlKeys);

                case TYPE.PAUSE_INTERVALS:
                    return new LinearAnalysisSummary.PausePeriod(0);
			}
			return null;
		}

        /// <summary>
        /// Processing the events and removing superfluous special keys with CheckSpecialKeys
        /// </summary>
        /// <param name="even">An event</param>
        /// <param name="period">Period contains Events that represent a keypress or pause</param>
        private void ProcessEvent(Event even, LinearAnalysisSummary.AbstractPeriod period)
        {
            var timedPart = Event.GetFirstEventPart<TimedEventPart>(even);
            ulong startTime = 0;
            ulong endTime = 0;
            if (timedPart != null)
            {
                startTime = timedPart.StartTime;
                endTime = timedPart.EndTime;
            }
            switch (even.Type)
            {
                case EventType.KEYBOARD:
                    {
                        var keyPress = Event.GetFirstEventPart<KeyPress>(even);
                        if (keyPress == null)
                        {
                            Log.Warn("Found a keyboard event without a keyPress part, skipped it.");
                            return;
                        }
                        var keyPressRepresentation = EventToString.KeyPressToString(keyPress, ControlKeys);
                        // Gets the current composite status
                        _currentIsComposite = EventToString.IsComposite;

                        if (keyPressRepresentation.Count() == 1)
                        {
                            AddSpecialKeyIfNotNull(period);   
                            period.Add(new LinearAnalysisSummary.RegularEvent(keyPress.Value), startTime, endTime);
                            CheckSpecialKeys(period);
                        }
                        else
                        {
                            if (CurrentSpecialKey != keyPressRepresentation)
                            {
                                AddSpecialKeyIfNotNull(period);
                            }
                            CurrentSpecialKey = keyPressRepresentation;
                        }
                        break;
                    }
                case EventType.MOUSE:
                    {
                        AddSpecialKeyIfNotNull(period);
                        var mouseEvent = Event.GetFirstEventPart<AbstractMouseEvent>(even);
                        if (mouseEvent == null)
                        {
                            Log.Warn("Found a mouse event without a mouse part, skipped it.");
                            return;
                        }
                        CurrentSpecialKey = EventToString.MouseEventToString(mouseEvent);
                        AddSpecialKeyIfNotNull(period);
                        break;
                    }
                case EventType.REPLACEMENT:
                    {
                        foreach (var replacementPart in even.Parts.OfType<Replacement>())
                        {
                            period.Add(new LinearAnalysisSummary.ReplaceEvent("<" + replacementPart.NewText + ">"), startTime, endTime);
                        }
                        break;
                    }
                case EventType.INSERT:
                    {
                        foreach (var insertPart in even.Parts.OfType<Insert>())
                        {
                            period.Add(new LinearAnalysisSummary.RegularEvent("<" + insertPart.Before + ">"), startTime, endTime);
                        }
                        break;
                    }
				case EventType.FOCUS:
					{
						foreach (var focusPart in even.Parts.OfType<FocusChange>())
						{
                            period.Add(new LinearAnalysisSummary.FocusEvent(focusPart.WindowTitle, 
                                int.Parse(even.Properties["id"])), startTime, endTime);
						}
					}
					break;

                //FUTURE selection, Eyewrite, DNS: skip them for now
            }
        }

        /// <summary>
        /// Initializes the analysis (= set some members) based on the type of linear analysis.
        /// </summary>
        private void Initialize()
        {
            if (ControlKeys != null)
            {
                RemoveDuplicateControlKeys(InputEvents, ControlKeys);
            }

            // discover double clicks if the threshold > 0
            if (Settings.Analysis.DoubleClickThreshold > 0)
            {
                RecognizeDoubleClicks(InputEvents, Settings.Analysis.DoubleClickThreshold);
            }
        }

        /// <summary>
        /// Adds the current special key to a given analysis if the current special key is not null.
        /// After calling this method, the current special key is always set to null.
        /// </summary>
        /// <param name="period">Period to add the current special event to (if it is not null).</param>
        private void AddSpecialKeyIfNotNull(LinearAnalysisSummary.AbstractPeriod period)
        {
            if (CurrentSpecialKey != null)
            {
                var specialKeyEvent = new LinearAnalysisSummary.SpecialEvent(CurrentSpecialKey, CurrentSpecialKeyCount);
                period.Add(specialKeyEvent);
                CheckSpecialKeys(period);
            }
            CurrentSpecialKey = null;
            _currentIsComposite = false;
        }

        /// <summary>
        /// CheckSpecialKeys: removes any superfluous special key from the current period.
        /// No need to copy pauseTime from deleted periods. The consecutive PauseTimes are calculated directly 
        /// from the logged events, not from the linear analysis periods.
        /// </summary>
        /// <param name="period">period in view</param>
        private static void CheckSpecialKeys(LinearAnalysisSummary.AbstractPeriod period)
        {
            if (_currentIsComposite)
            {
                if (!_previousIsComposite && (period.Count - 1) >= 0)
                {
                    period.RemoveAt(period.Count - 1);
                }
                _currentIsComposite = false;
                _previousIsComposite = true;
            }
            else
            {
                _previousIsComposite = false;
            }
        }
    }
}