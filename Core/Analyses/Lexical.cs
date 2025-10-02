using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// The Lexical class provides some helper methods to easily categorize characters.
    /// </summary>
    public static class Lexical
    {
        /// <summary>
        ///  List of keys (SHIFT, LSHIFT & RSHIFT) used to form a capital letter.
        /// </summary>
        private static readonly List<KeysEx> CapControls = new List<KeysEx>(new[] { KeysEx.VK_LSHIFT, 
            KeysEx.VK_RSHIFT, KeysEx.VK_SHIFT, KeysEx.VK_CAPITAL });

        /// <summary>
        /// List of Control keys.
        /// </summary>
        private static readonly List<KeysEx> CTRLControls = new List<KeysEx>(new[] {KeysEx.VK_LCONTROL, 
            KeysEx.VK_RCONTROL, KeysEx.VK_CONTROL});

        /// <summary>
        /// List of ALT keys.
        /// </summary>
        private static readonly List<KeysEx> ALTControls = new List<KeysEx>(new[] {KeysEx.VK_LMENU, 
            KeysEx.VK_MENU, KeysEx.VK_RMENU});

        /// <summary>
        /// List of revision keys.
        /// </summary>
        private static readonly List<KeysEx> RevisionKeys = new List<KeysEx>(new[] {KeysEx.VK_BACK, KeysEx.VK_DELETE, 
            KeysEx.VK_CLEAR, KeysEx.VK_ESCAPE});

        /// <summary>
        /// Determines whether a given string represents an alphanumeric character.
        /// It uses IsNumber and not the more common IsDigit method.
        /// See: msdn.microsoft.com/en-us/library/yk2b3t2y.aspx.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if the string represents an alphanumeric character, false otherwise.</returns>
        public static bool IsAlphaNumeric(string str)
        {
            if (str == null || str.Count() != 1) return false;
            // characters with punctuation are included
            // The IsDigit method determines whether a Char is a radix-10 digit while 
            // IsNumber will return true for digits, or for things that look like
            // numbers (fractions, Roman numerals, etc.).
            return char.IsLetter(str.First()) || char.IsNumber(str.First());
        }

        /// <summary>
        /// This method uses the Unicode categories ConnectorPunctuation, DashPunctuation, 
        /// OpenPunctuation, ClosePunctuation, InititalQuotePunctuation, FinalQuotePunctuation, 
        /// or OtherPunctuation. See: msdn.microsoft.com/en-us/library/aa311603(v=vs.71).aspx
        /// This method is more robust (culture indifferent) than the other checks defined in this class. 
        /// For instance 'IsSentenceReadingMark' does not include the inverted question mark 
        /// that precedes a question in Spanish.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if string is a punctuation mark as defined in a UnicodeCategory</returns>
        public static bool IsUnicodePunctuation(string str)
        {
            return char.IsPunctuation(str.First());
        }

        /// <summary>
        /// Determines whether a given string is a reading mark.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str == "." or str == "?" or str == "!", false otherwise.</returns>
        public static bool IsSentenceReadingMark(string str)
        {
            return ".".Equals(str) || "?".Equals(str) || "!".Equals(str);
        }

        /// <summary>
        /// Determines whether a given string is a reading mark preceding a sentence (Spanish).
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str == "¡" or str == "¿" , false otherwise.</returns>
        public static bool IsSentencePrecedingReadingMark(string str)
        {
            return "¡".Equals(str) || "¿".Equals(str);
        }

        /// <summary>
        /// Determines whether a given string is a binding charachter.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if (str == "-"), false otherwise.</returns>
        public static bool IsBindingChar(string str)
        {
            return "-".Equals(str);
        }

        /// <summary>
        /// Determines whether a given string is a word reading mark.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is one of & | \ " ( § ^ { }  / > , ; : = + , false otherwise.</returns>
        public static bool IsWordReadingMark(string str)
        {
            return ("&".Equals(str)
                || "|".Equals(str)
                || "\\".Equals(str)  /* escaped backslash */
                || "\"".Equals(str)  /* escaped double quote */
         //       || "\'".Equals(str)  /* escaped single quote */
                || "(".Equals(str)
                || "§".Equals(str)
                || "^".Equals(str)
                || "{".Equals(str)
                || "}".Equals(str)
                || "<".Equals(str)
                || "/".Equals(str)
                || ">".Equals(str)
                || ",".Equals(str)
                || ";".Equals(str)
                || ":".Equals(str)
                || "=".Equals(str)
                || "+".Equals(str)
                || "%".Equals(str) 
                || "*".Equals(str));
        }

        /// <summary>
        /// Determines whether a given string is a character within a word.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is alphanumeric or one of ' ` @ _ , false otherwise.</returns>
        public static bool IsWithinWordChar(string str)
        {
            return (IsAlphaNumeric(str)
                || "'".Equals(str) 
                || "`".Equals(str) 
                || "@".Equals(str) 
                || "_".Equals(str)
                || "~".Equals(str));
        }

        /// <summary>
        /// Determines whether a given string is a digit.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is a digit [0-9]</returns>
        public static bool IsDigit(string str)
        {
            if (str != null && str.Count() != 1)
            {
                return false;
            }
            return str != null && char.IsDigit(str.First());
        }

        /// <summary>
        /// Checks if an addition, subtraction, multiplication or division symbol is used in simple
        /// arithmetic operations. The pause parser will check if a digit precedes and follows this 
        /// symbol in order to disambiguate between other uses such as alphanumeric or word reading mark.
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns>true if string is a simple arithmetic symbol</returns>
        public static bool IsArithmeticOperation(string str)
        {
            return "+".Equals(str) || "-".Equals(str)
                   || "x".Equals(str) || @"\u00B7".Equals(str) // unicode middle dot (·) used as multiplication sign
                   || "*".Equals(str)
                   || "/".Equals(str) || @"\u00F7".Equals(str) // unicode division sign: ÷
                   || "=".Equals(str); 
        }

        /// <summary>
        /// Determines whether a string is a single character.
        /// 
        /// Note: rather strange rule, but taken over from legacy code.
        /// Still in use to make sure that we have the same behaviour as in the legacy code.
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns>true if str != null && str.Count() == 1</returns>
        public static bool IsCharacter(string str)
        {
            return str != null && str.Count() == 1;
        }

        public static bool IsCapitalLetter(string str)
        {
            return !string.IsNullOrEmpty(str) && char.IsUpper(str.First());
        }

        /// <summary>
        /// Determines whether a given string is a tab character.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is a tab "\t"</returns>
        public static bool IsTab(string str)
        {
            return str == "\t";
        }

        /// <summary>
        /// Determines whether a given string is a space character.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is a space " "</returns>
        public static bool IsSpace(string str)
        {
            return str == " ";
        }

        /// <summary>
        /// Legacy
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
         public static bool IsEnter(string str)
         {
             return IsReturn(str);
         }

        /// <summary>
        /// Determines whether a given string is a return.
        /// </summary>
        /// <param name="str">the string to check: new line, return, form feed, page break.</param>
        /// <returns>true if str is "\n", "\r", "\r\n" or "\f" or "\f\r"</returns>
        public static bool IsReturn(string str)
        {
            return str == "\n" || str == "\r" || str == "\r\n" || str == "\f" || str == "\f\r";
        }

        /// <summary>
        /// Determines whether a given string is a backspace.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if str is a tab "&#x8;" (=unicode for backspace)</returns>
        public static bool IsBackSpace(string str)
        {
            return str.Equals("&#x8;");
        }

        /// <summary>
        /// Determines whether a given string is a tab, space or enter.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if Lexical.isTabOrSpace(str) || Lexical.isEnter(str) is true</returns>
        public static bool IsTabOrSpaceOrEnter(string str)
        {
            return IsWhiteSpace(str) || IsReturn(str);
        }

        /// <summary>
        /// Determines whether a given string is a tab, space, enter or backspace.
        /// </summary>
        /// <param name="str">the string to check.</param>
        /// <returns>true if Lexical.isTabOrSpaceOrEnter(str) || Lexical.isBackSpace(str) is true</returns>
        public static bool IsTabOrSpaceOrEnterOrBack(string str)
        {
            return IsTabOrSpaceOrEnter(str) || IsBackSpace(str);
        }

        /// <summary>
        /// Determines whether a given string is a shift key.
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns></returns>
        public static bool IsShiftKey(string str)
        {
            return (str.Equals("LSHIFT") || str.Equals("RSHIFT") || str.Equals("SHIFT"));
        }

        /// <summary>
        /// Determines whether a given string is a control (CTRL) key.
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns></returns>
        public static bool IsCTRLKey(string str)
        {
            return (str.Equals("VK_LCTRL") || str.Equals("VK_RCTRL") || str.Equals("VK_CTRL"));
        }

        /// <summary>
        /// Determines whether a given string is an ALT key.
        /// </summary>
        /// <param name="str">the string to check</param>
        /// <returns></returns>
        public static bool IsALTKey(string str)
        {
            return (str.Equals("VK_MENU") || str.Equals("VK_RMENU") || str.Equals("VK_LMENU"));
        }

        /// <summary>
        /// Determines whether a given KeyPress is SHIFT, ALT, CTRL, diacritic or if
        /// the KeyboardState of an empty key contains these controls.
        /// </summary>
        /// <param name="keyPress">the key to check</param>
        /// <returns></returns>
        public static bool IsCombinationKey(KeyPress keyPress)
        {
            return CapControls.Contains(keyPress.Key)
                   || ALTControls.Contains(keyPress.Key)
                   || CTRLControls.Contains(keyPress.Key)
                   || string.IsNullOrEmpty(keyPress.Value)
                   && HasCombinationKey(keyPress);
        }

        /// <summary>
        /// Determines whether a given KeyBoardState contains a SHIFT, an ALT, or a CTRL key.
        /// </summary>
        /// <param name="keyPress">the key to check</param>
        /// <returns></returns>
        public static bool HasCombinationKey(KeyPress keyPress)
        {
            return (keyPress.KeyboardState.Any(stateKey => CapControls.Contains(stateKey))
                    || keyPress.KeyboardState.Any(stateKey => ALTControls.Contains(stateKey))
                    || keyPress.KeyboardState.Any(stateKey => CTRLControls.Contains(stateKey)));
        }

        /// <summary>
        /// Determines whether a given Key is a BackSpace, a DELETE, CLEAR, or ESCAPE key.
        /// </summary>
        /// <param name="keyPress">the key to check</param>
        /// <returns></returns>
        public static bool HasRevisionKey(KeyPress keyPress)
        {
            return RevisionKeys.Contains(keyPress.Key);
        }

        /// <summary>
        /// Determines whether a given KeyBoardState contains a shift key.
        /// </summary>
        /// <param name="keyPress">the key to check</param>
        /// <returns>Returns true if key is LSHIFT, RSHIFT or SHIFT</returns>
        public static bool HasShiftKey(KeyPress keyPress)
        {
            return keyPress.KeyboardState.Any(stateKey => CapControls.Contains(stateKey));
        }

        /// <summary>
        /// Returns whether Control and Backspace combinations are pressed.
        /// </summary>
        /// <returns>True if both Control (left or right) and Backspace are pressed</returns>
        public static bool IsCtrlBackSpace(KeyPress keyPress)
        {
            // OR controlkey (left or right) is pressed + BACKSPACE
            if (CTRLControls.Contains(keyPress.Key) 
                && keyPress.KeyboardState.Any(stateKey => RevisionKeys.Contains(stateKey)))
            {
                return true;
            }
            // OR BACKSPACE is pressed first + controlkey (left or right)
            return RevisionKeys.Contains(keyPress.Key) 
                && keyPress.KeyboardState.Any(stateKey => CTRLControls.Contains(stateKey));
        }

        /// <summary>
        /// Uses unicode categories. It includes SpaceSeparator category, LineSeparator category, 
        /// and ParagraphSeparator category.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsWhiteSpace(string str)
        {
            try
            {
                if (str.Any(char.IsWhiteSpace))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}