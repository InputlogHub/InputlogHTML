using System;
using System.Xml;

namespace InputLog.Core.Events.WordLog {

	/// <summary>
	/// An EventPart that represents the insertion of a text in the logged
	/// Document at some position.
	/// As we cannot know during the logging whether the text that was inserted
	/// was inserted before or after the cursor, we include both possibilities
	/// and leave it up to Replay to figure this out.
	/// </summary>
    public sealed class Insert : IEventPart {
		#region Fields
		/// <summary>
		/// The position where the text is either inserted before or after.
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// If the inserted text was inserted before the position, it is this text.
		/// </summary>
		public string Before { get; private set; }

		/// <summary>
		/// If the inserted text was after before the position, it is this text.
		/// </summary>
		public string After { get; private set; }

        /// <summary>
        /// Returns the length of the inserted text 'before'.
        /// </summary>
        public int Length
        {
            get { return Before.Length; }
        }
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="position">The position where the text is inserted.</param>
		/// <param name="before">The text that was inserted if the text was inserted before the cursor.</param>
		/// <param name="after">The text that was inserted if the text was inserted after the cursor.</param>
		public Insert(int position, string before, string after)
        {
			Position = position;
			Before = before;
			After = after;
		}

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Insert()
        {
        }

        #region Xml Serialization Infrastructure
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Position", Position.ToString());
            writer.WriteElementString("Before", Before);
            writer.WriteElementString("After", After);
        }

        public void ReadXml(XmlReader reader)
        {
            Position = Int32.Parse(reader.ReadElementString("Position"));
            Before = reader.ReadElementString("Before");
            After = reader.ReadElementString("After");
        }
        #endregion
	}
}