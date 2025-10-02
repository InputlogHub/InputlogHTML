using System.Collections.Generic;

namespace InputLog.Core.Util.KeyConversion
{
    /// <summary>
    /// Converts virtual key codes to Unicode strings,
    /// taking into account previousle entered dead keys.
    /// </summary>
    internal class KeyConverter
    {
        #region Fields

        /// <summary>
        /// A model for the keyboard from which the virtual keys are received.
        /// </summary>
        private readonly Keyboard Keyboard = new Keyboard();

        /// <summary>
        /// If the last entered key resulted in a dead key,
        /// this buffer is used for combining it with the
        /// next entered key.
        /// </summary>
        private DeadKey Buffer;

        #endregion

        /// <summary>
        /// Convert the given virtual key to its string value.
        /// This method takes dead keys into account in order to combine them with the given key.
        /// If the given key results in a deadkey using the given keyboardstate
        /// and capslock state and the buffer does not yet contain a deadkey the given key is stored 
        /// in the buffer and an empty string is returned.
        /// If the buffer does contain a deadkey, that deadkey will be combined with
        /// this deadkey and the resulting string will be returned (which consistst
        /// of both dead characters appended in chronologically entered order).
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <param name="keyboardstate">Set with the keys that are currently pressed.</param>
        /// <returns>The resulting value.</returns>
        public string Convert(KeysEx key, HashSet<KeysEx> keyboardstate)
        {
            lock (Keyboard)
            {
                var result = "";
                var capslock = (NativeMethods.GetKeyState((int) KeysEx.VK_CAPITAL) & 0x01) != 0;
                var state = ConvertKeyboardState(keyboardstate);

                if (Keyboard.Keys.ContainsKey(key))
                {
                    var vk = Keyboard.Keys[key];

                    if (Buffer != null)
                    {
                        // There is a dead key in the buffer, we need to combine it with this key
                        result = Buffer.GetCombinedCharacter(vk.GetValue(state, capslock)[0]);

                        // Dead char in buffer is used => reset it to null so it can't be used twice
                        Buffer = null;
                    }
                    else
                    {
                        // Buffer is still empty => if given key result in a dead key, put it in the buffer,
                        // otherwise, just return its value
                        if (vk.IsDead(state, capslock))
                        {
                            Buffer = vk.GetDeadKey(state, capslock);
                        }
                        else
                        {
                            result = vk.GetValue(state, capslock);
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Converts the given keyboardstate to a ShiftState.
        /// </summary>
        /// <param name="state">The keyboardstate to convert.</param>
        /// <returns></returns>
        private ShiftState ConvertKeyboardState(ICollection<KeysEx> state)
        {
            var result = ShiftState.BASE;

            if (state.Contains(KeysEx.VK_SHIFT) || state.Contains(KeysEx.VK_LSHIFT) || state.Contains(KeysEx.VK_RSHIFT))
            {
                result |= ShiftState.SHFT;
            }

            if (state.Contains(KeysEx.VK_CONTROL) || state.Contains(KeysEx.VK_LCONTROL) || state.Contains(KeysEx.VK_RCONTROL))
            {
                result |= ShiftState.CTRL;
            }

            if (state.Contains(KeysEx.VK_MENU) || state.Contains(KeysEx.VK_LMENU) || state.Contains(KeysEx.VK_RMENU))
            {
                result |= ShiftState.MENU;
            }

            if (state.Contains(Keyboard.XxxxVk))
            {
                // Reset result in order to stay within possibilities of ShiftState
                // (e.g. ctrl+Xxxx is not supported).
                result = ShiftState.XXXX;

                if (state.Contains(KeysEx.VK_SHIFT) || state.Contains(KeysEx.VK_LSHIFT) || state.Contains(KeysEx.VK_RSHIFT))
                {
                    result |= ShiftState.SHFT;
                }
            }

            return result;
        }
    }
}