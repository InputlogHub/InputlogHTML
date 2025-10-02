using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace InputLog.Core.Util.KeyConversion
{
    /// <summary>
    /// Represents a keyboard loaded from a keyboard layout and contains
    /// all the Virtal Keys found in it.
    /// </summary>
    public class Keyboard
    {
        #region Fields
        /// <summary>
        /// Empty state, i.e. a state where no keys are pressed.
        /// </summary>
        private static readonly KeysEx[] NullKeyState = new KeysEx[256];

        /// <summary>
        /// Maps each encountered DeadKeys while scanning the keyboard to its character.
        /// </summary>
        private readonly Dictionary<char, DeadKey> DeadKeys = new Dictionary<char, DeadKey>();

        /// <summary>
        /// Contains the different Keys in the keyboard.
        /// </summary>
        private readonly Dictionary<KeysEx, VirtualKey> KeysDictionary = new Dictionary<KeysEx, VirtualKey>(255);

        /// <summary>
        /// Handler to the keyboard layout.
        /// </summary>
        private readonly IntPtr Layout;

        public Dictionary<KeysEx, VirtualKey> Keys { get { return KeysDictionary; } }

        /// <summary>
        /// Contains the key that functions as a special shift key, or KeysEx.None if
        /// there is no special shift state.
        /// </summary>
        public KeysEx XxxxVk { get; private set; }

        /// <summary>
        /// Returns the maximum shift state.
        /// </summary>
        private ShiftState MaxShiftState
        {
            get { return (XxxxVk == KeysEx.NONE ? ShiftState.SHFT_MENU_CTRL : ShiftState.SHFT_XXXX); }
        }
        #endregion

        /// <summary>
        /// Constructor, constructs an instance using CurrentInputLanguage.Handle as
        /// keyboard layout handle.
        /// </summary>
        public Keyboard() : this(InputLanguage.CurrentInputLanguage.Handle) { }

        /// <summary>
        /// Constructor, constructs an instance using the given keyboard layout handle.
        /// (The keyboard should be loaded.)
        /// </summary>
        /// <param name="layout">The handle to the keyboard layout.</param>
        private Keyboard(IntPtr layout)
        {
            Layout = layout;
            ScanKeys();
            ProcessKeys();
        }

        /// <summary>
        /// Wrapper around NativMethods.ToUnicodeEx.
        /// </summary>
        /// <param name="key">The VirtualKey to translate to Unicode.</param>
        /// <param name="keyState">The state of the keyboard while translating.</param>
        /// <param name="buf">The buffer used for storing the result.</param>
        /// <returns>The result of NativeMethods.ToUnicodeEx</returns>
        private int ToUnicode(VirtualKey key, KeysEx[] keyState, StringBuilder buf)
        {
            return NativeMethods.ToUnicodeEx(key.VkCode, key.ScanCode, keyState, buf, buf.Capacity, 0, Layout);
        }

        /// <summary>
        /// Scans the keyboard for all available keys and adds them to this.Keys.
        /// It also checks whether there is a special shift state added and sets
        /// this.XxxxVk to the key that activates that state (or to KeysEx.None
        /// if there is no special shift state).
        /// </summary>
        private void ScanKeys()
        {
            // Scroll through the Scan Code (SC) values and get the valid Virtual Key (VK)
            // value for it. If the SC has a valid VK, store the constructed VK in this.Keys
            // so it can act as a flag that the VK is valid.
            for (uint sc = 0x01; sc <= 0x7f; sc++)
            {
                var key = new VirtualKey(Layout, sc);
                if (key.VkCode != 0)
                {
                    KeysDictionary[key.Key] = key;
                }
            }

            // Add the special keys that do not get added from the code above
            for (var ke = KeysEx.VK_NUMPAD0; ke <= KeysEx.VK_NUMPAD9; ke++)
            {
                KeysDictionary[ke] = new VirtualKey(Layout, ke);
            }
            KeysDictionary[KeysEx.VK_DIVIDE] = new VirtualKey(Layout, KeysEx.VK_DIVIDE);
            KeysDictionary[KeysEx.VK_CANCEL] = new VirtualKey(Layout, KeysEx.VK_CANCEL);
            KeysDictionary[KeysEx.VK_DECIMAL] = new VirtualKey(Layout, KeysEx.VK_DECIMAL);

            // See if there is a special shift state added
            XxxxVk = KeysEx.NONE; // Reset to empty value
            for (var vk = KeysEx.NONE; vk <= KeysEx.VK_OEM_CLEAR; vk++)
            {
                var sc = NativeMethods.MapVirtualKeyEx((uint)vk, 0, Layout);
                var vkL = NativeMethods.MapVirtualKeyEx(sc, 1, Layout);
                var vkR = NativeMethods.MapVirtualKeyEx(sc, 3, Layout);

                if ((vkL != vkR) && ((uint)vk != vkL))
                {
                    switch (vk)
                    {
                        case KeysEx.VK_LCONTROL:
                        case KeysEx.VK_RCONTROL:
                        case KeysEx.VK_LSHIFT:
                        case KeysEx.VK_RSHIFT:
                        case KeysEx.VK_LMENU:
                        case KeysEx.VK_RMENU:
                            break;
                        default:
                            XxxxVk = vk;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the keys STORED in this.Keys, filling in their result values
        /// as if they were pressed under different keyboard states and looks for DeadKeys.
        /// </summary>
        private void ProcessKeys()
        {
            var keystate = new KeysEx[256];

            foreach (var key in KeysDictionary.Values)
            {
                for (var ss = ShiftState.BASE; ss <= MaxShiftState; ss++)
                {
                    if (ss == ShiftState.MENU || ss == ShiftState.SHFT_MENU)
                    {
                        // Alt and Shift+Alt don't work, so skip them
                        continue;
                    }

                    for (var caps = 0; caps <= 1; caps++)
                    {
                        ClearKeyboardBuffer();
                        FillKeyState(keystate, ss, caps != 0);
                        var buf = new StringBuilder(10);

                        var rc = ToUnicode(key, keystate, buf);
                        if (rc > 0)
                        {
                            if (buf.Length == 0)
                            {
                                // Someone defined NULL on the keyboard; let's coddle them
                                key.SetValue(ss, "\u0000", caps != 0);
                            }
                            else
                            {
                                if ((rc == 1) &&
                                    (ss == ShiftState.CTRL || ss == ShiftState.SHFT_CTRL) &&
                                    ((int)key.VkCode == ((uint)buf[0] + 0x40)))
                                {
                                    // ToUnicodeEx has an internal knowledge about those 
                                    // VK_A ~ VK_Z keys to produce the control characters, 
                                    // when the conversion rule is not provided in keyboard 
                                    // layout files
                                    continue;
                                }

                                key.SetValue(ss, buf.ToString().Substring(0, rc), caps != 0);
                            }
                        }
                        else if (rc < 0)
                        {
                            // It's a dead key
                            var deadChar = buf.ToString()[0];
                            key.SetValue(ss, deadChar.ToString(), caps != 0);

                            // It's a dead key; let's flush out whats stored in the keyboard state.
                            ClearKeyboardBuffer();

                            // Only process found dead key if we did not process it yet
                            if (!DeadKeys.ContainsKey(deadChar))
                            {
                                DeadKeys.Add(deadChar, ProcessDeadKey(key, ss, keystate, caps != 0));
                            }

                            key.SetValue(ss, caps != 0, DeadKeys[deadChar]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes the given dead key, creating a new instance of DeadKey and looking for any
        /// possible combinations with it.
        /// </summary>
        /// <param name="dKey">The dead key.</param>
        /// <param name="shiftstate">The shiftstate under which the given key returns a dead character.</param>
        /// <param name="dKeyState">The key state  under which the given key returns a dead character.</param>
        /// <param name="capslock">Was the caps lock key on?</param>
        /// <returns>An instance of DeadKey containing the results.</returns>
        private DeadKey ProcessDeadKey(VirtualKey dKey, ShiftState shiftstate, KeysEx[] dKeyState, bool capslock)
        {
            var keyState = new KeysEx[256];
            var deadKey = new DeadKey(dKey.GetValue(shiftstate, capslock)[0]);

            // Test each possible combination
            foreach (var key in KeysDictionary.Values)
            {
                var buf = new StringBuilder(10);

                for (var ss = ShiftState.BASE; ss <= MaxShiftState; ss++)
                {
                    var rc = 0;
                    if (ss == ShiftState.MENU || ss == ShiftState.SHFT_MENU)
                    {
                        // Alt and Shift+Alt don't work, so skip them
                        continue;
                    }

                    for (var caps = 0; caps <= 1; caps++)
                    {
                        // First the dead key
                        while (rc >= 0)
                        {
                            // We expect a dead key (otherwise this fct would not be called)
                            // => rc should be equal to -1.
                            // If no dead key received, state was messed up and we run again
                            // to clear it up. Risk is technically an infinite loop but per
                            // Hiroyama that should be impossible here.
                            rc = ToUnicode(dKey, dKeyState, buf);
                        }

                        // Now fill the key state for the potential base character
                        FillKeyState(keyState, ss, caps != 0);

                        buf = new StringBuilder(10);
                        rc = ToUnicode(key, keyState, buf);
                        if (rc == 1)
                        {
                            // That was indeed a base character for our dead key and we now have
                            // a composite character. Let's run through one more time to get the
                            // actual base character that made it all possible?
                            var combchar = buf[0];

                            buf = new StringBuilder(10);
                            rc = ToUnicode(key, keyState, buf);

                            var basechar = buf[0];

                            // If combined char same as dead key, we must clear out the keyboard buffer
                            if (deadKey.DeadCharacter == combchar)
                            {
                                ClearKeyboardBuffer();
                            }

                            if (((ss == ShiftState.CTRL || ss == ShiftState.SHFT_CTRL)
                                 && (char.IsControl(basechar))) || basechar.Equals(combchar))
                            {
                                // ToUnicodeEx has an internal knowledge about those VK_A ~ VK_Z
                                // keys to produce the control characters, when the conversion rule
                                // is not provided in keyboard layout files.
                                // Additionally, dead key state is lost for some of these character
                                // combinations, for unknown reasons.
                                // Therefore, if the base character and combining are equal, and its
                                // a CTRL or CTRL+SHIFT state, and a control character is returned,
                                // then we do not add this "dead key" (which is not really a dead key).
                                continue;
                            }

                            if (!deadKey.ContainsBaseCharacter(basechar))
                            {
                                deadKey.AddBaseChar(basechar, combchar);
                            }
                        }
                        else if (rc > 1)
                        {
                            // Not a valid dead key combination, just ignore it.
                        }
                        else if (rc < 0)
                        {
                            // It's another dead key, so we ignore it but flush it from the state.
                            ClearKeyboardBuffer();
                        }
                    }
                }
            }

            return deadKey;
        }

        /// <summary>
        /// Clears the keyboard buffer, removing any dead characters residing within.
        /// </summary>
        private void ClearKeyboardBuffer()
        {
            var sb = new StringBuilder(10);
            var rc = 0;
            while (rc < 0)
            {
                rc = ToUnicode(KeysDictionary[KeysEx.VK_DECIMAL], NullKeyState, sb);
            }
        }

        /// <summary>
        /// Fills keyState so it contains the state of the keyboard representing
        /// the given ShiftState - capslock combination.
        /// </summary>
        /// <param name="keyState">The array to make represent the keyboard state.</param>
        /// <param name="ss">The ShiftState.</param>
        /// <param name="capsLock">The capslock state. (True = on, false = off.)</param>
        private void FillKeyState(IList<KeysEx> keyState, ShiftState ss, bool capsLock)
        {
            keyState[(int)KeysEx.VK_SHIFT] = (((ss & ShiftState.SHFT) != 0) ? (KeysEx)0x80 : 0x00);
            keyState[(int)KeysEx.VK_CONTROL] = (((ss & ShiftState.CTRL) != 0) ? (KeysEx)0x80 : 0x00);
            keyState[(int)KeysEx.VK_MENU] = (((ss & ShiftState.MENU) != 0) ? (KeysEx)0x80 : 0x00);

            if (XxxxVk != KeysEx.NONE)
            {
                // The Xxxx key has been assigned, so let's include it
                keyState[(int)XxxxVk] = (((ss & ShiftState.XXXX) != 0) ? (KeysEx)0x80 : 0x00);
            }

            keyState[(int)KeysEx.VK_CAPITAL] = (capsLock ? (KeysEx)0x01 : 0x00);
        }
    }
}