using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util;

namespace InputLog.Core.Events
{
    /// <summary>
    ///     Represents any kind of keyboard-, mouse-, focus-, ... event.
    ///     An event consists of EventParts that can be added/removed using the Parts property.
    /// </summary>
    public class Event
    {
        #region Fields

        /// <summary>
        ///     Special properties that are internally used by inputLog.
        /// </summary>
        private const string TYPE = "type";

        /// <summary>
        ///     Contains the parts of which this event exists.
        /// </summary>
        public List<IEventPart> Parts { get; private set; }

        /// <summary>
        ///     Properties of the event that can be set.
        /// </summary>
        public IDictionary<string, string> Properties { get; private set; }

		/// <summary>
		///		Labels that can be added to the event.
		/// </summary>
		public IDictionary<string, string> Labels { get; private set; }

        /// <summary>
        ///     The type of the event (this is a property of the Event).
        ///     Note: by setting this.Properties.[TYPE] ="foo" you overwrite the type property.
        /// </summary>
        public string Type
        {
            get
            {
                string returnVal;
                Properties.TryGetValue(TYPE, out returnVal);
                return returnVal ?? "";
            }
            set { Properties[TYPE] = value; }
        }

        #endregion

        /// <summary>
        ///     Constructs a new event with no event parts and no properties set.
        /// </summary>
        public Event()
        {
            Properties = new Dictionary<string, string>();
            Parts = new List<IEventPart>();
            Labels = new Dictionary<string, string>();
        }

        /// <summary>
        /// Provides a deep copy of this event.
        /// </summary>
        /// <returns></returns>
        public Event GetEventCopy()
        {
            var thisEvent = new Event {Properties = {["type"] = Type, ["id"] = GetId().ToString()}};
            if (Parts.Count > 0)
            {
                foreach (IEventPart part in Parts)
                {
                    thisEvent.Parts.Add(part);
                }
            }
            if (Labels.Count > 0)
            {
                foreach (var labelPair in Labels)
                {
                    thisEvent.Labels.Add(labelPair);
                }
            }
            return thisEvent;
        }

        /// <summary>
        ///     Provides a string representation of an event.
        /// </summary>
        /// <returns>String representation of an event.</returns>
        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("[Event type:");
            strBuilder.Append(Type);
            if (Parts.Count > 0)
            {
                strBuilder.Append(" {");
                foreach (IEventPart part in Parts)
                {
                    strBuilder.Append(part);
                    strBuilder.Append(" ,");
                }
                strBuilder.Remove(strBuilder.Length - 2, 2); // remove last ",[space]"
                strBuilder.Append("}");
            }
			if (Labels.Count > 0)
			{
				foreach (var labelPair in Labels)
				{
					strBuilder.Append("{");
					strBuilder.Append(labelPair.Key);
					strBuilder.Append(": ");
					strBuilder.Append(labelPair.Value);
					strBuilder.Append("}, ");
				}
				strBuilder.Remove(strBuilder.Length - 2, 2); // remove last ",[space]"
				strBuilder.Append("}");
			}
            strBuilder.Append("]");
            return strBuilder.ToString();
        }

        public KeyPress GetKeypressFromEvent()
        {
            return Parts.OfType<KeyPress>().FirstOrDefault();
        }

        public string GetKeypressKey()
        {
            var keyPress = GetKeypressFromEvent();
            return keyPress?.Key.ToString() ?? string.Empty;
        }
        
        public string GetKeypressValue()
        {
            var keyPress = GetKeypressFromEvent();
            return keyPress != null ? keyPress.Value : string.Empty;
        }

        public void ChangeId(int to)
        {
            Properties["id"] = to.ToString();
        }

        public int GetId()
        {
            return int.Parse(Properties["id"]);
        }

        /// <summary>
        ///     Returns the first EventPart of a certain AnalysisType T.
        /// </summary>
        /// <param name="even">Event for which to return the first EventPart</param>
        /// <returns>
        ///     The first EventPart of AnalysisType T of the given event,
        ///     or default(T) if there is no such part.
        /// </returns>
        public static T GetFirstEventPart<T>(Event even)
        {
            if (even == null)
            {
                return default;
            }
            foreach (T eventPart in even.Parts.OfType<T>())
            {
                return eventPart;
            }
            return default;
        }

        /// <summary>
        ///     Returns the first EventPart of a certain AnalysisType T that occurs in the list,
        ///     i.e. of the first Event that contains a part of this type
        /// </summary>
        /// <param name="events">Event for which to return the first EventPart</param>
        /// <returns>
        ///     The first EventPart of AnalysisType T of the given event,
        ///     or default(T) if there is no such part.
        /// </returns>
        public static T GetFirstEventPart<T>(IEnumerable<Event> events)
        {
            foreach (T eventPart in events.Where(even => even != null).SelectMany(even => even.Parts).OfType<T>())
            {
                return (eventPart);
            }
            return default;
        }

        /// <summary>
        ///  Returns the id and the starting time of the first keypress
        /// </summary>
        /// <param name="el">The events logged in this session.</param>
        /// <returns></returns>
        public static Pair<int, ulong> SkipToFirstKeypress(IEnumerable<Event> el)
        {
            return (from e in el
                where EventType.KEYBOARD.Equals(e.Type)
                let t = GetFirstEventPart<TimedEventPart>(e)
                where t != null
                select new Pair<int, ulong>(e.GetId(), t.StartTime)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the startTime of an event or null.
        /// </summary>
        /// <param name="ev">Event in view.</param>
        /// <returns>Returns a Pair with key: event.ID, value: event.StartTime></returns>
        public Pair<int, ulong> GetStartKeyboardEventID(Event ev)
        {
            Pair<int, ulong> keyPress = null;
            if (EventType.KEYBOARD.Equals(ev.Type))
            {
                var t = GetFirstEventPart<TimedEventPart>(ev);
                if (t != null)
                {
                    keyPress = new Pair<int, ulong>(ev.GetId(), t.StartTime);
                }
            }
            return keyPress;
        }

        /// <summary>
        /// TODO This is a dirty hack that should be removed and replaced by first identifying the main document
        /// at the start in the InputlogDocument and then setting MainDocument property of the SessionIdentification.
        /// 
        /// Finding out if this window title could be the main document of a logging session.
        /// </summary>
        /// <param name="aName">A document name found in one event.</param>
        /// <param name="titleList">The main document is assumed to have the highest frequency in this list.</param>
        /// <param name="count">The number of items to collect in the title list.</param>
        /// <returns>Probably the main document name</returns>
        public string FindMainTitle(string aName, List<string> titleList, int count)
        {
            var title = aName.ToLower();
            // Skipping over a few unusable titles.
            // 'TASKBAR' is the default name given by FocusChange.cs 
            // to an event with an unknown window title.
            if (title.IsNullOrEmpty()
                // no clickable components with this 'title'
                || title.Contains("taskbar")
                || title.Contains("inputlog")
                // no external documents
                || title.Contains("pdf")
                // no browser pages
                || title.Contains("chrome")
                || title.Contains("firefox")
                || title.Contains("safari")
                || title.Contains("opera")
                || title.Contains("mozilla")
                || title.Contains("explorer"))
            {
                return "UNKNOWN";
            }
            // This could be a candidate...
            if (title.Contains("wordlog"))
            {
                return aName;
            }
            // Still guessing? We take the document with
            // the highest frequency in this little title list.
            titleList.Add(aName);
            if (titleList.Count >= count)
            {
                return titleList.GroupBy(x => x)
                    .OrderByDescending(x => x.Count())
                    .First().Key;
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Returns a window title if this event has one.
        /// </summary>
        /// <param name="even">The event to check.</param>
        /// <returns>Empty string or window title.</returns>
        public string GetWindowTitle(Event even)
        {
            if (!EventType.FOCUS.Equals(even.Type)) return string.Empty;
            var focusEventParts = even.Parts.OfType<FocusChange>();
            var focusChanges = focusEventParts as FocusChange[] ?? focusEventParts.ToArray();
            if (focusChanges.Any())
            {
                return focusChanges.First().WindowTitle;
            }
            return string.Empty;
        }
    }
}