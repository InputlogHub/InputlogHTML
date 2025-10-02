using System;
using System.Collections.Generic;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Plugin.WordLog.Keys
{
    /// <summary>
    /// A key is a lot like a WinLog.KeyPress except that the timed component is irrelevant.
    /// Concluding, the Key has a key being pressed and an optionally empty current KeyState.
    /// </summary>
    public class Key: IEquatable<Key>
    {
        #region Fields
        /// <summary>
        /// The pressed key.
        /// </summary>
        public KeysEx KeyPressed { get; set; }

        /// <summary>
        /// The state of the keyboard at the time of the keypress.
        /// </summary>
        public ICollection<KeysEx> KeyState { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Key() { }

        /// <summary>
        /// Initialize the key.
        /// </summary>
        /// <param name="key">Key being pressed</param>
        /// <param name="keyState">List of keys currently being pressed on the keyboard, may be null</param>
        public Key(KeysEx Key, List<KeysEx> KeyState)
        {
            this.KeyPressed = Key;
            this.KeyState = (KeyState == null) ? new List<KeysEx>() : KeyState;
        }

        /// <summary>
        /// Checks whether two keys are equal or not. In order to be equal they must have the same KeyPressed
        /// and the same KeyState
        /// TODO: Only check for equal control keys in the keystate? Random characters that are still pressed but
        /// are not control keys could perhaps be ignored, and thus the two keys may still be perceived as being
        /// equal? E.g. when typing very quickly.
        /// </summary>
        /// <param name="Other">The other key</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public bool Equals(Key Other)
        {
            // Create two sets with all the keys in the hashset.
            HashSet<KeysEx> ownKeys = new HashSet<KeysEx>();
            HashSet<KeysEx> othersKeys = new HashSet<KeysEx>();

            ownKeys.Add(this.KeyPressed);
            foreach(KeysEx key in this.KeyState)
            {
                ownKeys.Add(key);
            }

            othersKeys.Add(Other.KeyPressed);
            foreach (KeysEx key in Other.KeyState)
            {
                othersKeys.Add(key);
            }

            // If the two sets contain the same items the two keys are perceived as equal.
            if (ownKeys.Count != othersKeys.Count) return false;
            ownKeys.UnionWith(othersKeys);
            return ownKeys.Count == othersKeys.Count;
        }
    }
}
