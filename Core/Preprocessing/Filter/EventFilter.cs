using InputLog.Core.Events;
using InputLog.Core.Util;

namespace InputLog.Core.Preprocessing.Filter
{

    /// <summary>
    /// Abstract class that implements common functionality of event filters. An event filter is a statefull predicate 
    /// that verifies a condition on an event. The idea is to use these filters on event lists to filter out certain events.
    /// </summary>
	public abstract class EventFilter : Preprocessor, IStatefullPredicate<Event>
    {   
        /// <summary>
        /// Method that verifies whether a given event fulfills a certain condition (condition itself is 
        /// obviously implementation specific). 
        /// 
        /// (Note that the outcome of this method can be dependent on previous invocations as this class 
        /// can maintain state between calls to Check()).
        /// </summary>
        /// <param name="even">Object on which to verify a condition.</param>
        /// <returns>true if the condition is met, false otherwise</returns>
        public abstract bool Check(Event even);

    }
}
