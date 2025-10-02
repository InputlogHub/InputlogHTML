using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Utility class that provides method to easily get string representations for a keypress.
    /// </summary>
    public static class EventToString
    {
        /// <summary>
        /// Flag to indicate a compound keyRepresentation (controlKey + other keyPress)
        /// </summary>
        public static bool IsComposite { get; set; }

        /// <summary>
        /// TODO This list should be in a config file, not here
        /// List of control keys (SHIFT, LSHIFT & RSHIFT) used in combination with a letter key to form a capital letter.
        /// These control keys may not be shown in the analysis output ('Grouped Keys' in user settings).
        /// </summary>
        public static List<KeysEx> CapControls = new List<KeysEx>(new[] { KeysEx.VK_LSHIFT, KeysEx.VK_RSHIFT, KeysEx.VK_SHIFT });

        /// <summary>
        /// Determines a string representation for a keyPress.
        /// If the keyPress has a Value of size 1, this value is used as key representation 
        /// (except when that character is a control character).
        /// Otherwise, the virtual key code (stripped from the "VK_"-prefix) is used.
        /// If any control key was pressed at the same time (= if any control key is part of keyPress.KeyboardState),
        /// the returned string will will include these keys (e.g. LSHIFT + A). 
        /// Shift key is removed when the main key is not a control key
        /// </summary>
        /// <param name="keyPress">KeyPress for which to provide a string representation.</param>
        /// <param name="controlKeys">(Optional) List of keys that are considered control keys.</param>
        /// <returns>string representation for a KeyPress.</returns>
        public static string KeyPressToString(KeyPress keyPress, List<KeysEx> controlKeys = null)
        {
            var keyRepresentation = "";
            IsComposite = false;

            if (controlKeys != null)
            {
                foreach (var stateKey in
                    keyPress.KeyboardState.Where(stateKey => controlKeys.Contains(stateKey) && stateKey != keyPress.Key))
                {
                    if (!CapControls.Contains(stateKey) || (CapControls.Contains(stateKey) && controlKeys.Contains(keyPress.Key)))
                    {
                        keyRepresentation += KeyToString(stateKey) + " + ";
                    }
                    IsComposite = true;
                }
            }

            if (keyPress.Value.Count() == 1)
            {
                var character = keyPress.Value.First();
                string tmp;
                switch (character)
                {
                    case '\n':
                        tmp = "ENTER";
                        break;
                    case '\t':
                        tmp = "TAB";
                        break;
                    case ' ':
                        tmp = "SPACE";
                        break;
                    default:
                        {
                            if (Char.IsControl(character))
                            {
                                tmp = StripVKFromKeyCode(keyPress.Key.ToString());
                            }
                            else
                            {
                                var charRepresentation = character.ToString();
                                tmp = charRepresentation;
                            }
                            break;
                        }
                }
                keyRepresentation += tmp;
            }
            else
            {
                keyRepresentation += KeyToString(keyPress.Key);
            }
            //DEBUG: Console.WriteLine(" Key IsComposite " + IsComposite);
            return keyRepresentation;
        }

        /// <summary>
        /// Strips the string "VK_" from a given (keycode) string if the string starts with "VK_".
        /// </summary>
        /// <param name="keyCode">The string from which to strip "VK_"</param>
        /// <returns>the string without "VK_"</returns>
        public static string StripVKFromKeyCode(string keyCode)
        {
            return keyCode.StartsWith("VK_") ? keyCode.Substring(3) : keyCode;
        }

        /// <summary>
        /// Returns a string presentation that for a given key (only to be used in the output of analyses).
        /// </summary>
        /// <param name="key">Key for which to return a string representation.</param>
        /// <returns>a string representation for the given key</returns>
        public static string KeyToString(KeysEx key)
        {
            switch (key)
            {
                case KeysEx.VK_MENU:
                    return "ALT";
                case KeysEx.VK_LMENU:
                    return "LALT";
                case KeysEx.VK_RMENU:
                    return "RALT";
                case KeysEx.VK_CONTROL:
                    return "CTRL";
                case KeysEx.VK_LCONTROL:
                    return "LCTRL";
                case KeysEx.VK_RCONTROL:
                    return "RCTRL";
                case KeysEx.VK_CAPITAL:
                    return "CAPS LOCK";
                default:
                    return StripVKFromKeyCode(key.ToString());
            }
        }

        /// <summary>
        /// Determines a string representation for a mouseEvent.
        /// </summary>
        /// <param name="e">The mouseEvent to get the string representation for.</param>
        /// <returns>a string representation for the given mouseEvent.</returns>
        public static string MouseEventToString(AbstractMouseEvent e)
        {
            if (e is MouseMovement)
            {
                return "Movement";
            }
            if (e is DoubleClick)
            { // first check for DoubleClick and then for click since DoubleClick is inherited from Click
                return "Double " + ((Click)e).Button + " Click";
            }
            if (e is Click)
            {
                return ((Click)e).Button + " Click";
            }
            if (e is Scroll)
            {
                return "Scroll";
            }
            return "";
        }
    }
}