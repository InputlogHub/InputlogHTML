using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace InputLog.Core.Util
{
    /// <summary>
    /// The class StringUtils has 'escape' and 'unescape' methods to handle special kinds of white space. 
    /// In an earlier version every whitespace was transformed into a space, which gave problems with the revision analysis 
    /// if other white spaces where used (\ v, \ r, ...). The StringUtils encode and decode the special white spaces
    /// (but not the real space) into their corresponding Unicode formats and handles the backslash that precedes 
    /// the Unicode notation.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Regex that matches whitespace.
        /// We use this pattern such that the space is not replaced but the '\' is
        /// </summary>
        private static readonly Regex WhiteSpaces = new Regex(@"\\\f\n\r\t\v");

        /// <summary>
        /// Regex that matches unicode sequences.
        /// </summary>
        private static readonly Regex UnicodeSequences = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})");

        /// <summary>
        /// Character used to separate file paths.
        /// </summary>
        private const string PATH_SEPARATOR = ";";

        /// <summary>
        /// A stringBuilder that can be reused without having to recreate
        /// it for every operation.
        /// </summary>
        private static readonly StringBuilder Sb = new StringBuilder();

        /// <summary>
        /// The maxlength for a filename on Windows FAT system (=255), NTFS maxlength = 256,
        /// Window maxlength = 260. We use the smallest maxlength.
        /// </summary>
        public static readonly int FILENAME_MAX_LENGTH = 255;

        /// <summary>
        /// Replaces every \\\f\n\r\t\v characters that occurs in the given string by its unicode sequence.
        /// </summary>
        /// <param name="input">The string in which to replace the characters.</param>
        /// <returns>The resulting string.</returns>
        public static string Escape(string input)
        {
            return input == null ? null : WhiteSpaces.Replace(input, ToUnicodeEscapeChar);
        }

        /// <summary>
        /// Un-escapes the given string, replacing any unicode sequences by its corresponding character counterparts.
        /// </summary>
        /// <param name="input">The string in which the replacing should be done.</param>
        /// <returns>The resulting string.</returns>
        public static string Unescape(string input)
        {
            return input == null ? null : UnicodeSequences.Replace(input, FormUnicodeEscapeSequence);
        }

        /// <summary>
        /// Replaces every character in the given match by its unicode sequence.
        /// </summary>
        /// <param name="input">The match of which the characters should be replaced.</param>
        /// <returns>The unicode sequences corresponding to the characters in the given match.</returns>
        private static string ToUnicodeEscapeChar(Match input)
        {
            var buf = new StringBuilder(input.Length*6);
            foreach (var c in input.Value)
            {
                buf.Append(@"\u" + ((int) c).ToString("x4"));
            }
            return buf.ToString();
        }

        /// <summary>
        /// Replaces the match, which should be a match of a unicode sequence in which the numeric value is a
        /// subgroup called "Value", by its unicode character.
        /// </summary>
        /// <param name="input">Match which should be converted to its unicode counterpart.</param>
        /// <returns>The unicode character represented by the match.</returns>
        private static string FormUnicodeEscapeSequence(Match input)
        {
            return ((char) Int32.Parse(input.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
        }

        /// <summary>
        /// Filteres a string to be a valid SPSS variable name. This fucntion removes
        /// any non alphanumeric character from a string and replaces it by an underscore
        /// </summary>
        /// <param name="input">string to be cleaned</param>
        /// <param name="sep">String separator</param>
        /// <returns>cleaned string</returns>
        public static string FilterSPSSInvalid(string input, string sep)
        {
            if (input.Contains("row"))
            {
                var i = input.IndexOf("row", StringComparison.Ordinal);
                var s1 = input.Substring(0, i);
                var s2 = input.Substring(i + 4);
                input = s1 + s2;
            }

            var condensed = new StringBuilder();
            string[] parts = Regex.Split(input, sep);
            for (int i = 0; i < parts.Length - 1; i++)
            {
                string[] pps = parts[i].Split(' ');
                foreach (string p in pps)
                {
                    try
                    {
                        if (p.Length == 2 && i == 1)
                        {
                            condensed.Append(p[0].ToString().ToUpper());
                            condensed.Append(p[1].ToString().ToUpper());
                        }
                        else
                        {
                            condensed.Append(p.First().ToString().ToUpper());
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore empty string and continue
                    }
                }
                condensed.Append("_");
            }
            condensed.Append(parts[parts.Length - 1]);

            if (parts.Length < 3 && !string.IsNullOrWhiteSpace(parts[parts.Length - 1]))
            {
                string[] pps = parts[parts.Length - 1].Split(' ');

                condensed.Append(" (");
                foreach (string p in pps)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        condensed.Append(p.First().ToString().ToUpper());
                    }
                }
                condensed.Append(")");
            }

            string cond = condensed.ToString().Replace(sep, "_");
            var filtered = new StringBuilder();
            foreach (var t in cond)
            {
                filtered.Append(char.IsLetterOrDigit(t) ? t : '_');
            }
            return filtered.ToString();
        }

        /// <summary>
        /// Creates an acronym from upper case letters in a string.
        /// A second lower case character is added after the upper case.
        /// </summary>
        /// <param name="text">input</param>
        /// <returns></returns>
        public static string GetAcronym(string text)
        {
            var result = new StringBuilder();

            for (int index = 0; index < text.Length - 1; index++)
            {
                char c = text[index];
                if (char.IsUpper(c))
                {
                    result.Append(c);
                    result.Append(char.ToLower(text[index + 1]));
                }
            }
            return (result.Length == 0) ? text : result.ToString();
        }


        /// <summary>
        /// Changes line separators to line feeds, paragraph separators to line feeds and
        /// space characters to 'middle dot's. 
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>String with the above mentioned characters replaced.</returns>
        public static string ConvertToReadable(string input)
        {
            Sb.Clear();
            foreach (var chr in input)
            {
                var charCat = char.GetUnicodeCategory(chr);
                switch (charCat)
                {
                    // Convert all line separators into a line feed.
                    case UnicodeCategory.LineSeparator:
                        Sb.Append((char) 10);
                        break;
                    // Convert all paragraph separators into a line feed.
                    case UnicodeCategory.ParagraphSeparator:
                        Sb.Append((char) 10);
                        break;
                    // Change space representations into 'middle dot'
                    case UnicodeCategory.SpaceSeparator:
                        Sb.Append((char) 183);
                        break;
                    // Convert e.g. the formfeed &#xc; character 'middle-dot'.
                    // Control code character, with a Unicode value of U+007F 
                    // or in the range U+0000 through U+001F or U+0080 through U+009F. 
                    // Signified by the Unicode designation "Cc" (other, control). The value is 14. 
                    case UnicodeCategory.Control:
                        Sb.Append((char) 183);
                        break;
                    default:
                        Sb.Append(chr);
                        break;
                }
            }
            return Sb.ToString();
        }

        /// <summary>
        /// Non-printable ASCII characters cause trouble in the HTML page. 
        /// They have a byte value below 32 and 127 (DELETE) and can be removed
        /// and replaced by a middle dot.
        /// </summary>
        /// <param name="s">input </param>
        /// <returns></returns>
        public static string ReplaceNonPrintableCharacters(IEnumerable<char> s)
        {
            const char replaceWith = (char) 183;
            if (s == null) return null;
            var result = new StringBuilder();
            foreach (var cr in s)
            {
                var b = (int) cr;
                result.Append((b < 32 || b == 127) ? replaceWith : cr);
            }
            return result.ToString();
        }

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx
        /// </summary>
        /// <param name="pathname">The pathname to shorten.</param>
        /// <param name="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three ellipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        public static string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname == null)
                return "";

            if (pathname.Length <= maxLength)
            {
                return pathname;
            }

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
            {
                root += Path.DirectorySeparatorChar;
            }

            string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    return pathname.Substring(0, maxLength - 3) + "...";
                }
            }
            // pathname is just a root and filename
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength)
            {
                root += "...\\";
                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string
                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathname;
        }

        public static string DoubleIfNeeded(double d, string f, NumberFormatInfo nfi)
        {
            var i = Convert.ToInt32(d);
            return d - i > 0.00001 ? d.ToString(f, nfi) : i.ToString();
        }

        /// <summary>
        /// Converts a clock string with date and hh:mm:ss.thousands to a unix style date stamp.
        /// This conversion is done according to the date time locale information... (Culture specific).
        /// </summary>
        /// <param name="cstring">the clock style string.</param>
        /// <returns>The unix timestamp for the given time-string.</returns>
        public static ulong ClockStringToMsec(string cstring)
        {
            var ticks = (ulong) (DateTime.UtcNow.Ticks - DateTime.Parse(cstring).Ticks);
            return ticks/10000; // convert to milliseconds.
        }

        /// <summary>
        /// Return the largest common path in a list of file paths.
        /// </summary>
        /// <param name="paths">The different paths to find the common path in.</param>
        /// <returns>The largest common path. The common path is the path that starts at the highest hierarchy, which is common
        /// to as low a level as possible.</returns>
        public static string FindCommonPath(IEnumerable<string> paths)
        {
            var enumerable = paths as string[] ?? paths.ToArray();
            var matches =
                from length in Enumerable.Range(0, enumerable.Min(s => s.Length)).Reverse()
                let possibleMatch = enumerable.First().Substring(0, length)
                where enumerable.All(f => f.StartsWith(possibleMatch)
                        && possibleMatch.EndsWith(Path.DirectorySeparatorChar.ToString()))
                select possibleMatch;

            return matches.First();
        }

        /// <summary>
        /// Splitting a string with several concatenated paths on the separator character 
        /// and returning a list of paths.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> SplitPaths(string paths)
        {
            return paths.Split(PATH_SEPARATOR.ToCharArray()).ToList();
        }

        /// <summary>
        /// Concatenating a collection of paths into one string, 
        /// each separated by a separator character. 
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string JoinPaths(IEnumerable<string> paths)
        {
            return string.Join(PATH_SEPARATOR, paths);
        }

        /// <summary>
        /// Converts all non-ascii characters in a string to their ascii form.<br />
        /// For example ë > e. <br />
        /// Or á > a. <br/>
        /// etc.<br />
        /// Source: http://stackoverflow.com/questions/140422/how-do-i-translate-8bit-characters-into-7bit-characters-i-e-to-u/10036907#10036907
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>A normalized, ascii string.</returns>
        public static string LatinToAscii(string input)
        {
            StringBuilder newStringBuilder = new StringBuilder();
            newStringBuilder.Append(input.Normalize(NormalizationForm.FormKD)
                .Where(x => x < 128)
                .ToArray());
            return newStringBuilder.ToString();
        }

        public static string JoinF<T>(string sep, IEnumerable<T> objs, Func<T, string> f)
        {
            var sb = new StringBuilder();
            bool notFirst = false;
            foreach (T o in objs)
            {
                if (notFirst)
                {
                    sb.Append(sep);
                }
                else
                {
                    notFirst = true;
                }
                sb.Append(f(o));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// Invalid characters will be replaced with an underscore. Invalid/reserved words will be replaced by
        /// _reservedWord_ 
        /// </summary>
        /// <param name="filename">filename that needs to be sanitized.</param>
        /// <param name="extensionLength">Length of the extension, if it is not yet included in the fileName, excluding
        /// the extension separator character.</param>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string CoerceValidFileName(string filename, int extensionLength = 0)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = $@"[{invalidChars}]+";

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitizedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = $"^{reservedWord}\\.";
                sanitizedNamePart = Regex.Replace(sanitizedNamePart, reservedWordPattern, "_reservedWord_.",
                    RegexOptions.IgnoreCase);
            }

            return sanitizedNamePart.Truncate(FILENAME_MAX_LENGTH - (extensionLength + 1));
        }

        /// <summary>
        /// Truncate a string to maxlength.
        /// </summary>
        /// <param name="value">String to be truncated</param>
        /// <param name="maxLength">Max length for the string. The minimum length is 0.</param>
        /// <returns>The original string, truncated to max length if it was too long. Or the original string, unaltered,
        /// if it did not need to be truncated.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        ///     Converts a string to subscript characters. This is only possible for certain characters.
        ///     Allowed characters are 0-9, +, -, =, (, and ), and a, e, o, and x
        /// </summary>
        /// <param name="value">The string</param>
        /// <returns>The string in subscript, for the characters that were valid. Invalid characters 
        /// are ignored.</returns>
        public static string ToUnicodeSubScript(this string value)
        {
            StringBuilder unicodeRep = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        unicodeRep.Append("\\u208");
                        unicodeRep.Append(c);
                        break;

                    case '+':
                        unicodeRep.Append("\\u208A");
                        break;
                    case '-':
                        unicodeRep.Append("\\u208B");
                        break;
                    case '=':
                        unicodeRep.Append("\\u208C");
                        break;
                    case '(':
                        unicodeRep.Append("\\u208D");
                        break;
                    case ')':
                        unicodeRep.Append("\\u208E");
                        break;

                    case 'a':
                        unicodeRep.Append("\\u2090");
                        break;
                    case 'e':
                        unicodeRep.Append("\\u2091");
                        break;
                    case 'o':
                        unicodeRep.Append("\\u2092");
                        break;
                    case 'x':
                        unicodeRep.Append("\\u2093");
                        break;
                }
            }
            StringBuilder result = new StringBuilder();
            foreach (Match match in Regex.Matches(unicodeRep.ToString(), @"\\u(?<Value>[a-zA-Z0-9]{4})"))
            {
                var unicodeChar = (char) int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
                result.AppendFormat("{0}", unicodeChar);
            }

            return result.ToString();
        }

        /// <summary>
        ///     Converts a string to superscript characters. This is only possible for certain characters.
        ///     Allowed characters are 0-9, +, -, =, (, and ), n, i
        /// </summary>
        /// <param name="value">The string</param>
        /// <returns>The string in superscript, for the characters that were valid. Invalid characters 
        /// are ignored.</returns>
        public static string ToUnicodeSuperScript(this string value)
        {
            StringBuilder unicodeRep = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    // Superscript 1, 2, 3 have been added to standard ASCII range
                    case '1':
                        unicodeRep.Append("\\u00B9");
                        break;
                    case '2':
                        unicodeRep.Append("\\u00B2");
                        break;
                    case '3':
                        unicodeRep.Append("\\u00B3");
                        break;

                    // Unicode range
                    case '0':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        unicodeRep.Append("\\u207");
                        unicodeRep.Append(c);
                        break;

                    case '+':
                        unicodeRep.Append("\\u207A");
                        break;
                    case '-':
                        unicodeRep.Append("\\u207B");
                        break;
                    case '=':
                        unicodeRep.Append("\\u207C");
                        break;
                    case '(':
                        unicodeRep.Append("\\u207D");
                        break;
                    case ')':
                        unicodeRep.Append("\\u207E");
                        break;
                    case 'n':
                        unicodeRep.Append("\\u207F");
                        break;
                    case 'i':
                        unicodeRep.Append("\\u2071");
                        break;
                }
            }
            StringBuilder result = new StringBuilder();
            foreach (Match match in Regex.Matches(unicodeRep.ToString(), @"\\u(?<Value>[a-zA-Z0-9]{4})"))
            {
                Char unicodeChar = (Char) int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
                result.AppendFormat("{0}", unicodeChar);
            }

            return result.ToString();
        }
    }

    public static class StringExtensions
    {
        public static int IndexOfOccurrence(this string s, string match, int occurrence)
        {
            int i = 1;
            int index = -1;

            while (i <= occurrence && (index = s.IndexOf(match, index + 1, StringComparison.Ordinal)) != -1)
            {
                if (i == occurrence)
                {
                    return index;
                }
                i++;
            }
            return -1;
        }
    }
}