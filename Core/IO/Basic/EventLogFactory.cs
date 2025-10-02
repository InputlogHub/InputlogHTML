using System.IO;
using System.Text;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.IO.Xml.Input;
using InputLog.Core.IO.Xml.Output;
using System;

namespace InputLog.Core.IO.Basic
{

    /// <summary>
    /// Factory that can be used to create EventLog(Writer)'s (to write eventz to e.g. a file) 
    /// or EventLog(Reader)'s (to read eventz from e.g an existing file).
    /// </summary>
    public class EventLogFactory
    {
        /// <summary>
        /// Creates an EventLog(Writer) that writes to file.
        /// </summary>
        /// <param name="filepath">Path where the file where the EventLog(Writer) will write to is located.</param>
        /// <param name="logFormat">LogFormat used while logging eventz to the file.</param>
        /// <returns>An EventLog(Writer) that can be used to write eventz to the file.</returns>
        public static EventLogWriter CreateFileEventLogWriter(string filepath, string logFormat)
        {
            switch (logFormat)
            {
                case LogFormat.XML:

                    var stream = File.Open(filepath, FileMode.Create);
                    return new EventLogWriter(new XmlEventWriter(Encoding.UTF8, stream));
                default:
                    throw new Exception("No registered writer for format " + logFormat);
            }
        }

        /// <summary>
        /// Creates an EventLog(Reader) that reads from file.
        /// </summary>
        /// <param name="filepath">Path where the file where the EventLog(Reader) will read from is located.</param>
        /// <param name="logFormat">LogFormat used by the logfile where the eventz are read from.</param>
        /// <returns>An EventLog(Reader) that can be used to read eventz from the file.</returns>
        public static EventLogReader CreateFileEventLogReader(string filepath, string logFormat)
        {
            switch (logFormat)
            {
                case LogFormat.XML:
                    var stream = File.Open(filepath, FileMode.Open);
                    return new EventLogReader(new XmlEventReader(stream));
                default:
                    throw new Exception("No registered reader for format " + logFormat);
            }
        }
    }
}