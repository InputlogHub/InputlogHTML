using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;

namespace InputLog.Core.IO.Basic.Input
{

	/// <summary>
	/// An EventLogReader that can read Logged eventz in an ordered way with the help of an EventReader.
	/// </summary>
	public class EventLogReader
	{
		/// <summary>
		/// Reader used for reading the events.
		/// </summary>
		private readonly AbstractEventReader EventReader;

		/// <summary>
		/// Constructs an EventLofReader
		/// </summary>
		/// <param name="eventReader">EventReader used to read the actual events.</param>
		public EventLogReader(AbstractEventReader eventReader)
		{
			EventReader = eventReader;
		}

		/// <summary>
		/// Retrieve the SessionIdentification (=information that identifies the logsession) from the EventStream.
		/// </summary>
		/// <returns>Session information.</returns>
		public SessionIdentification ReadSessionIdentification()
		{
			return EventReader.ReadHeader();
		}

		/// <summary>
		/// Reads the eventz from the EventStream.
		/// </summary>
		/// <returns>A list of Events that were read from the EventStream.</returns>
		public List<Event> ReadEvents()
		{
			// 21-5-2013: Preprocess on the read event list that swaps focus and mouse/keyboard events that 
			// have the same start time. As a focus event always appears after the first event in the window, 
			// while in reality the event has happened after the focus event.
			// Reason for this: focus change gets checked by the logger on keyboard/mouse-events, not on 
			// the 'actual' changing of the window.
			// 13-06-2013: Current StartTime < Previous StartTime patch. Situation encountered in 
			// a Dutch school logging session, no clue as why and how this happened.
			// 2015-07-07 Swapping is used to order the events by their start time.
			List<Event> events = EventReader.ReadEvents();
			Event previousEvent = null;
			int count = 0;
			while (count < events.Count)
			{
                Event currentEvent = events[count];

                // Outcommented 
                //// Removing events with a timed part but with no activity. 
                //if (null != Event.GetFirstEventPart<TimedEventPart>(currentEvent) && currentEvent.Parts.Count == 1)
                //{
                //    TimedEventPart part = Event.GetFirstEventPart<TimedEventPart>(currentEvent);
                //    // If it is a focus event though, we do keep it.
                //    if (!(part is FocusChange))
                //    {
                //        events.RemoveAt(count);
                //        continue;
                //    }
                //}

                // Mouse events with a timed part but with no activity are removed.
                var evenPart = currentEvent.Parts.OfType<KeyPress>().FirstOrDefault();
                if (null != evenPart)
                {
                    if (EventType.MOUSE == currentEvent.Type && Math.Abs((long)(evenPart.EndTime - evenPart.StartTime)) < 3)
                    {
                        events.RemoveAt(count);
                    }
                }

				// Fix copyTask faulty logging of VK_SPACE
				if (currentEvent.Type == EventType.KEYBOARD)
				{
					var keypress = currentEvent.GetKeypressFromEvent();
					if (keypress != null && keypress.Key == Util.KeyConversion.KeysEx.VK_SPACE)
					{
						keypress.Value = " ";
					}
				}

				if (null != previousEvent)
				{
					var currentTime = (TimedEventPart) currentEvent.Parts.Find(part => part is TimedEventPart);
					var previousTime = (TimedEventPart) previousEvent.Parts.Find(part => part is TimedEventPart);

					if (null != currentTime && null != previousTime)
					{
						if (currentTime.StartTime < previousTime.StartTime)
						{
							SwapEvents(events, currentEvent, previousEvent, count);
						}
						if (currentEvent.Type == EventType.FOCUS && currentTime.StartTime == previousTime.StartTime)
						{
							SwapEvents(events, currentEvent, previousEvent, count);
						}
					}
				}
				previousEvent = events[count];
				count++;
			}
			Close();
			return events;
		}

		/// <summary>
		/// Swap the two events in the eventList. Their ID's will be swapped as well.
		/// </summary>
		/// <param name="events">The list of events.</param>
		/// <param name="current">The current event, to be swapped with the previous one.</param>
		/// <param name="previous">The previous event, to be swapped with the current event.</param>
		/// <param name="count">The index in the events list of the current event.</param>
		private static void SwapEvents(IList<Event> events, Event current, Event previous, int count)
		{
			events.Insert(count - 1, current);
			events.RemoveAt(count + 1);

			string tmp = previous.Properties["id"];
			previous.Properties["id"] = current.Properties["id"];
			current.Properties["id"] = tmp;
		}

		public void Close()
		{
			EventReader.Close();
		}
	}
}