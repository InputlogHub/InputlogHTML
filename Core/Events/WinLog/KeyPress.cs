using System.Collections.Generic;
using InputLog.Core.Util.KeyConversion;
using System.Xml;
using System;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Core keyboard information (received from Windows).
    /// </summary>
    public sealed class KeyPress : TimedEventPart, IKeyboardEventPart
    {
        #region Fields
        /// <summary>
        /// The pressed key.
        /// </summary>
        public KeysEx Key { get; set; }

        /// <summary>
        /// Value resulting from this keyPress.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The keys pressed on the moment this event was generated.
        /// </summary>
        public List<KeysEx> KeyboardState { get; private set; }
        #endregion

        /// <summary>
        /// Constructs a CoreKeyboardEventPart.
        /// </summary>
        /// <param name="key">The pressed key.</param>
        /// <param name="value">The resulting input value.</param>
        /// <param name="startTime">start time of the event (timestamp in msec)</param>
        /// <param name="endTime"> end time of the event (timestamp in msec)</param>
        /// <param name="kbState">The keys pressed on the moment this event was generated.</param>
        public KeyPress(KeysEx key, string value, ulong startTime, ulong endTime, IEnumerable<KeysEx> kbState)
            : base(startTime, endTime)
        {
            Key = key;
            Value = value;
            KeyboardState = new List<KeysEx>(kbState);
        }

        /// <summary>
        /// Constructs a KeyPress.
        /// </summary>
        /// <param name="key">The pressed key.</param>
        /// <param name="value">The resulting input value.</param>
        /// <param name="startTime">start time of the event (timestamp in msec)</param>
        /// <param name="kbState">The keys pressed on the moment this event was generated.</param>
        public KeyPress(KeysEx key, string value, ulong startTime, ICollection<KeysEx> kbState)
            : this(key, value, startTime, 0, kbState)
        {

        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public KeyPress()
        {
            KeyboardState = new List<KeysEx>();
        }

        ///// <summary>
        ///// Returns whether Control and Backspace are pressed.
        ///// </summary>
        ///// <returns>True if both Control (left or right) and Backspace are pressed</returns>
        //public bool IsCtrlBackSpace()
        //{
        //    // OR you press a controlkey (left or right) + BACKSPACE
        //    if ((Key == KeysEx.VK_LCONTROL || Key == KeysEx.VK_RCONTROL) &&
        //        KeyboardState.Contains(KeysEx.VK_BACK))
        //    {
        //        return true;
        //    }
        //    // OR you press BACKSPACE + controlkey (left or right)
        //    return Key == KeysEx.VK_BACK && (KeyboardState.Contains(KeysEx.VK_LCONTROL) 
        //        || KeyboardState.Contains(KeysEx.VK_RCONTROL));
        //}

        #region Xml Serialization Infrastructure
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Key", Key.ToString());
            writer.WriteElementString("Value", Value);
            if (KeyboardState != null && KeyboardState.Count > 0)
            {
                writer.WriteStartElement("KeyBoardState");
                foreach (KeysEx key in KeyboardState)
                {
                    writer.WriteElementString("Key", key.ToString());
                }
                writer.WriteEndElement();
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Key = (KeysEx)Enum.Parse(typeof(KeysEx), reader.ReadElementString("Key"));
            Value = reader.ReadElementString("Value");
            if (reader.IsStartElement("KeyBoardState"))
            {
                reader.ReadStartElement("KeyBoardState");
                while (reader.IsStartElement("Key"))
                {
                    var k = (KeysEx)Enum.Parse(typeof(KeysEx), reader.ReadElementString("Key"));
                    KeyboardState.Add(k);
                }
                reader.ReadEndElement();
            }
        }
        #endregion
    }
}