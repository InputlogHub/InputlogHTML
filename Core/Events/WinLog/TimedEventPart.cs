using System.Xml;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Represents a part that contains a start and an endtime.
    /// </summary>
    public class TimedEventPart: IEventPart
    {
        /// <summary>
        /// Start time of the event (timestamp in msec = msecs passed since start of system).
        /// </summary>
        public ulong StartTime { get; set; }

        /// <summary>
        /// End time of the event (timestamp in msec = msecs passed since start of system).
        /// </summary>
        public ulong EndTime { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        public TimedEventPart(ulong start, ulong end)
        {
            StartTime = start;
            EndTime = end;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public TimedEventPart()
        {
        }

        public void Shift(ulong amount)
        {
            StartTime += amount;
            EndTime += amount;
        }

        #region Xml Serialization Infrastructure
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("StartTime", StartTime.ToString());
            writer.WriteElementString("EndTime", EndTime.ToString());
        }

        public virtual void ReadXml(XmlReader reader)
        {
            StartTime = ulong.Parse(reader.ReadElementString("StartTime"));
            EndTime = ulong.Parse(reader.ReadElementString("EndTime"));
        }
        #endregion
    }
}