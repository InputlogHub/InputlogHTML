using System.Xml;
using System;

namespace InputLog.Core.Events.WordLog
{

    // FUTURE add markup logging?
    /// <summary>
    /// WordLog part for a KeyboardEvent, adds a position.
    /// </summary>
    public sealed class Keypress : IKeyboardEventPart
    {
        #region Fields
        /// <summary>
        /// Position in the document when the keypress took place.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The length of the document when the keypress took place.
        /// </summary>
        public int DocumentLength { get; set; }

        /// <summary>
        /// True if Replay should include this KeyPress,
        /// false if it should ignore it.
        /// (Typically keypresses will be ignored if they
        /// are accompanied by a Replacement event.)
        /// </summary>
        public bool IncludeInReplay { get; set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">Position in the document when the keypress took place.</param>
        /// <param name="documentLength">The length of the document when the keypress took place.</param>
        /// <param name="includeInReplay">True if Replay should include this KeyPress, false if
        /// it should ignore it. (Typically keypresses will be ignored if they are accompanied
        /// by a Replacement event.)</param>
        public Keypress(int position, int documentLength, bool includeInReplay)
        {
            Position = position;
            DocumentLength = documentLength;
            IncludeInReplay = includeInReplay;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Keypress()
        {
        }

        #region Xml Serialization Infrastructure
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Position", Position.ToString());
            writer.WriteElementString("DocumentLength", DocumentLength.ToString());
            writer.WriteElementString("IncludeInReplay", IncludeInReplay.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            Position = Int32.Parse(reader.ReadElementString("Position"));
            DocumentLength = Int32.Parse(reader.ReadElementString("DocumentLength"));
            IncludeInReplay = Boolean.Parse(reader.ReadElementString("IncludeInReplay"));
        }
        #endregion
    }
}