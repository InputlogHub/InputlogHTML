using System;
using System.Xml;

namespace InputLog.Core.Events.WordLog
{

    /// <summary>
    /// Event that represent the change of the selection in Word.
    /// </summary>
    public sealed class SelectionChange : IEventPart
    {
        #region Fields
        /// <summary>
        /// Start of the selection, i.e. the position of the first character contained by the selection.
        /// (Counting starts with 0.)
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// End of the selection, i.e. the position of the last character contained by the selection.
        /// (Counting starts with 0.)
        /// </summary>
        public int End { get; private set; }

        /// <summary>
        /// Returns the length of the selected range.
        /// </summary>
        public int Length
        {
            get { return End - Start + 1; }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Position of the first character included in the selection.</param>
        /// <param name="end">Position of the last character included in the selection.</param>
        public SelectionChange(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public SelectionChange()
        {
        }

        #region Xml Serialization Infrastructure
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Start", Start.ToString());
            writer.WriteElementString("End", End.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            Start = Int32.Parse(reader.ReadElementString("Start"));
            End = Int32.Parse(reader.ReadElementString("End"));
        }
        #endregion
    }
}