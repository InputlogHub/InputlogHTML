using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Preprocessing.Recode
{
    /// <summary>
    ///     It has been observed that in rare occasions the mouse is assigned to a thread, 
    ///     different from the main Inputlog thread.
    ///     This happens when the user opens a Windows IME feature (eg. Japanese) with a mouse click, 
    ///     prior to the opening of Inputlog. If, during the logging, the user goes back and forth 
    ///     from his Inputlog document to a Japansese dictionary a double and divergent time registration
    ///     is the result: one timeline for Inputlog events and one timeline for IME events (mouse movements).
    ///     When the time difference between keybord and mouse is negative we replace the start and endtime
    ///     of mouse movements and clicks with the end time of the preceding keyboard event.
    ///     The changed idfx is then saved with 'RECODE' added to its filename.
    /// </summary>
    public class NegativePauseRemover : Preprocessor
    {
        /// <summary>
        /// Start and end times are replaced with the end time of the previous event 
        /// if the pause time is negative.
        /// </summary>
        /// <param name="inputEvents"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)
        {
            ulong previousEndTime = 0;
            var recodedEventList = new List<Event>();

            foreach (var @event in inputEvents)
            {
                var newEvent = @event;
                ulong currentStartTime = 0;

                var timedEventPartCurrent = Event.GetFirstEventPart<TimedEventPart>(newEvent);
                if (timedEventPartCurrent != null)
                {
                    currentStartTime = timedEventPartCurrent.StartTime;
                }

                switch (newEvent.Type)
                {
                    case EventType.KEYBOARD:
                        if (timedEventPartCurrent != null)
                        {
                            previousEndTime = timedEventPartCurrent.EndTime;
                        }
                        recodedEventList.Add(newEvent);
                        break;

                    case EventType.MOUSE:

                        if (currentStartTime < previousEndTime)
                        {
                            if (!newEvent.Parts.OfType<MouseMovement>().IsNullOrEmpty())
                            {
                                MouseMovement moveLog = newEvent.Parts.OfType<MouseMovement>().FirstOrDefault();
                                int mIndex = newEvent.Parts.IndexOf(moveLog);
                                ((MouseMovement) newEvent.Parts[mIndex]).StartTime = previousEndTime;
                                ((MouseMovement) newEvent.Parts[mIndex]).EndTime = previousEndTime;
                            }
                            else if (!newEvent.Parts.OfType<Click>().IsNullOrEmpty())
                            {
                                Click clickLog = newEvent.Parts.OfType<Click>().FirstOrDefault();
                                int mIndex = newEvent.Parts.IndexOf(clickLog);
                                ((Click) newEvent.Parts[mIndex]).StartTime = previousEndTime;
                                ((Click) newEvent.Parts[mIndex]).EndTime = previousEndTime;
                            }
                            else if (!newEvent.Parts.OfType<Scroll>().IsNullOrEmpty())
                            {
                                Scroll scrollLog = newEvent.Parts.OfType<Scroll>().FirstOrDefault();
                                int mIndex = newEvent.Parts.IndexOf(scrollLog);
                                ((Scroll)newEvent.Parts[mIndex]).StartTime = previousEndTime;
                                ((Scroll)newEvent.Parts[mIndex]).EndTime = previousEndTime;
                            }
                        }
                        recodedEventList.Add(newEvent);
                        break;

                    case EventType.FOCUS:
                        if (currentStartTime < previousEndTime)
                        {
                            if (!newEvent.Parts.OfType<FocusChange>().IsNullOrEmpty())
                            {
                                FocusChange focusLog = newEvent.Parts.OfType<FocusChange>().FirstOrDefault();
                                int focusIndex = newEvent.Parts.IndexOf(focusLog);
                                ((FocusChange) newEvent.Parts[focusIndex]).StartTime = previousEndTime;
                                ((FocusChange) newEvent.Parts[focusIndex]).EndTime = previousEndTime;
                            }
                        }
                        recodedEventList.Add(newEvent);
                        break;
                    default:
                        recodedEventList.Add(newEvent);
                        break;
                }
            }
            return recodedEventList;
        }
    }
}