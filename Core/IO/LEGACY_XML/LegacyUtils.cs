using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util.KeyConversion;
using System.Windows.Forms;
using InputLog.Core.Events.WinLog;
using log4net;

namespace InputLog.Core.IO.Legacy_Xml {

    // Provides some utility methods for reading in Legacy XML files.
    public static class LegacyUtils {

        // Log4Net MessageLogger.
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Converts strings to a KeysEx representation.
        /// Note: this method only does an (elaborate) attempt to find a good matching, that is, it does not garantuee that it will return a KeysEx for every string.
        /// 
        /// Some key-values are converted to qwerty related keysEx.
        /// For example, the underscore _ will be converted to a KeysEx.VK_OEM_MINUS because it can be found on the same key as the - (dash) on the qwerty keyboard.
        /// This is only done for frequently used keys, less frequently used key-values will return KeysEx.None.
        /// 
        /// </summary>
        /// <param name="value">Value to get the KeysEx representation for.</param>
        /// <returns>A KeysEx for the given value, or KeysEx.None if no KeysEx could be found that can be mapped to the given character.</returns>
        public static KeysEx StringToKeysEx(string value) {
            string originalValue = value;
            KeysEx returnKeysEx = KeysEx.NONE;
            value = value.Trim();
            try {
                if (value.Length == 1) {

                    switch (value) {
                        case ",": return KeysEx.VK_OEM_COMMA;
                        case ".": return KeysEx.VK_OEM_PERIOD; // it could be that the decimal was used => ignore this.
                        case "-": return KeysEx.VK_OEM_MINUS;
                        case "+": return KeysEx.VK_OEM_PLUS;
                        case "/": return KeysEx.VK_DIVIDE;
                        case ";": return (KeysEx)Keys.OemSemicolon;
                        case ":": return (KeysEx)Keys.OemSemicolon;
                        case "?": return (KeysEx)Keys.OemQuestion;
                        case "~": return (KeysEx)Keys.Oemtilde;
                        case "{": return (KeysEx)Keys.OemOpenBrackets;
                        case "}": return (KeysEx)Keys.OemCloseBrackets;
                        case "|": return (KeysEx)Keys.OemPipe;
                        case "_": return KeysEx.VK_OEM_MINUS;
                        case "'": return (KeysEx)Keys.OemQuotes;
                        case "\"": return (KeysEx)Keys.OemQuotes;
                        default: value = "VK_" + value.ToUpper(); break;
                    }
                    returnKeysEx = (KeysEx)Enum.Parse(typeof(KeysEx), value);

                }
                else {
                    // special keys, see legacy code 
                    // Project Integrate, InputLog.cpp, line 1950
                    // bool CXMLParser::fetchVKey(UINT paramL, UINT paramH, TCHAR keyDescription[]);
                    switch (value) {
                        case "NOTHING": return KeysEx.NONE;

                        case "BS": return KeysEx.VK_BACK;
                        case "TAB": return KeysEx.VK_TAB;
                        case "CLEAR": return KeysEx.VK_CLEAR;
                        case "ENTER": return KeysEx.VK_RETURN;
                        case "SHIFT": return KeysEx.VK_SHIFT;
                        case "CTRL": return KeysEx.VK_CONTROL;

                        case "ALT": return KeysEx.VK_MENU;
                        case "PAUSE": return KeysEx.VK_PAUSE;
                        case "CAPS LOCK": return KeysEx.VK_CAPITAL;
                        case "ESC": return KeysEx.VK_ESCAPE;
                        case "SPACE": return KeysEx.VK_SPACE;
                        case "PU": return KeysEx.VK_PRIOR;
                        case "PD": return KeysEx.VK_NEXT;
                        case "END": return KeysEx.VK_END;
                        case "HOME": return KeysEx.VK_HOME;
                        // arrow keys
                        case "LEFT": return KeysEx.VK_LEFT;
                        case "UP": return KeysEx.VK_UP;
                        case "RIGHT": return KeysEx.VK_RIGHT;
                        case "DOWN": return KeysEx.VK_DOWN;

                        case "SELECT": return KeysEx.VK_SELECT;
                        case "EXECUTE": return KeysEx.VK_EXECUTE;
                        case "PRT SCR": return KeysEx.VK_SNAPSHOT;
                        case "INS": return KeysEx.VK_INSERT;
                        case "DEL": return KeysEx.VK_DELETE;
                        case "HELP": return KeysEx.VK_HELP;
                        case "WinKey": return KeysEx.VK_LWIN; // always return left windows-key (theoretically, it could have been the right one too).
                        case "ContextKey": return KeysEx.VK_APPS;

                        // Numpad keys are handled seperately, see further
                        // F keys are handled seperately, see further
                        case "NUM LOCK": return KeysEx.VK_NUMLOCK;
                        case "SCROLL LOCK": return KeysEx.VK_NUMLOCK;
                        case "ATTN": return KeysEx.VK_ATTN;
                        case "CR SEL": return KeysEx.VK_CRSEL;
                        case "EX SEL": return KeysEx.VK_EXSEL;
                        case "ERASE EOF": return KeysEx.VK_EREOF;
                        case "PLAY": return KeysEx.VK_PLAY;
                        case "ZOOM": return KeysEx.VK_ZOOM;
                        case "PA1": return KeysEx.VK_PA1;
                    }
                    // Numpad keys
                    if (value.StartsWith("NUM ")) {
                        value = value.Remove(0, 4); // remove "NUM "
                        switch (value) {
                            case "*": return KeysEx.VK_MULTIPLY;
                            case "+": return KeysEx.VK_ADD;
                            case "SEPERATOR": return KeysEx.VK_SEPARATOR;
                            case "-": return KeysEx.VK_SUBTRACT;
                            case ".": return KeysEx.VK_DECIMAL;
                            case "/": return KeysEx.VK_DIVIDE;
                            default: value = "VK_NUMPAD" + value; break;
                        }
                        returnKeysEx = (KeysEx)Enum.Parse(typeof(KeysEx), value);
                    }
                    // F keys
                    if (value.StartsWith("F")) {
                        value = "VK_" + value;
                        returnKeysEx = (KeysEx)Enum.Parse(typeof(KeysEx), value);
                    }

                }

            }
            catch (ArgumentException e) {
                log.Debug("Unable to convert '" + originalValue + "' to a KeysEx", e);
            }

            if (returnKeysEx == KeysEx.NONE) {
                log.Debug("Unable to convert '" + originalValue + "' to a KeysEx");
            }

            return returnKeysEx;
        }


        /// <summary>
        /// Converts values from their legacy representation to their new representation.
        /// </summary>
        /// <param name="oldValue">The legacy representation of the value.</param>
        /// <returns>The new representation of the value.</returns>
        public static string OldToNewValue(string oldValue) {
            string[] emptyNewValues = { "ALT", "SHIFT", "CTRL", "DOWN", "UP", "LEFT", "RIGHT" };
            if (emptyNewValues.Contains(oldValue)) return "";
            if (oldValue.StartsWith("NUM ")) return oldValue.Replace("NUM ", "");
            switch (oldValue) {
                case "ENTER": return "\n";
                case "SPACE": return " ";
                case "TAB": return "\t";
            }
            return oldValue.Trim(); // trim spaces from characters
        }


        /// <summary>
        /// Creates a keypress from a given string, starttime and endtime.
        /// The value should be the legacy-XML representation of the key.
        /// 
        /// If the value contains one or more "+", it will be splitted and the different parts of the string will be added to the keyboard-state. 
        /// The part after the last "+" will be taken as keypress value.
        /// 
        /// This method uses LegacyUtils.StringToKeysEx(value) to do the actual string to KeysEx conversion.
        /// 
        /// Note: this method does currently NOT create keyboardstates that are not explicitely indicated in value by '+'.
        /// For example, when you type a " (double quote) on a qwerty-keyboard, you need to press a SHIFT key and the '-key.
        /// If the given value is ", this method will only return ' as the pressed key, not the SHIFT key as part of the keyboard!
        /// (The value for the keypress still stays " of course).
        /// This behaviour is a balance between keyboard-indepence and usability. That is, most values will return a keypress with a keysEx,
        /// but some of those keypresses are qwerty-related. Implicit keyboard states are not added to avoid complete qwerty dependence.
        /// 
        /// 
        /// </summary>
        /// <param name="value">String to create a keypress for.</param>
        /// <param name="startTime">StartTime of the keypress.</param>
        /// <param name="endTime">EndTime of the keypress.</param>
        /// <returns>A Keypress representing the key that was pressed when the original value was typed.</returns>
        public static KeyPress CreateKeyPress(string value, ulong startTime, ulong endTime) {
            if (value == null) return null;
            KeyPress returnKeyPress = null;
            List<KeysEx> keyboardState = new List<KeysEx>();

            // if the value contains a '+', loop over all but the last part and add them to the keyboard state.
            if (value.Contains('+')) {
                string[] values = value.Split('+');
                value = LegacyUtils.OldToNewValue(values[values.Length - 1]);
                for (int i = 0; i < values.Length - 1; i++) {
                    KeysEx key = LegacyUtils.StringToKeysEx(values[i]);
                    keyboardState.Add(key);
                }
            }


            // add shift on upper case? => uncomment code below
            //if (value.Length == 1) {
            //    if (Char.IsUpper(value.First())) {
            //        keyboardState.Add(KeysEx.VK_SHIFT);
            //    }
            //}

            // keyboard states could (if necessary) be added like shown in the commented code below.
            //switch (value) {
            //    case "_": {
            //            keyboardState.Add(KeysEx.VK_SHIFT);
            //        }
            //}

            returnKeyPress = new KeyPress(LegacyUtils.StringToKeysEx(value), LegacyUtils.OldToNewValue(value), startTime, endTime, keyboardState);

            return returnKeyPress;
        }




    }
}
