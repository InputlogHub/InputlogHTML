using System.Collections.Generic;
using InputLog.Core.Events;

namespace InputLog.Core.IO.Convert
{
    /// <summary>
    /// An event that is convertible into another equivalent event.
    /// </summary>
    public interface ILConvertible 
    {
        /// <summary>
        /// The conversion produces a list of converted elements.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Event> ConvertToEvent();
    }
}
