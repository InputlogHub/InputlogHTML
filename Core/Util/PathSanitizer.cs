using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Util
{
    /// <summary>
    /// Contains methods to sanitize and uniquify a file path.
    /// </summary>
    public static class PathSanitizer
    {
        /// <summary>
        /// The maximum length of a path in Microsoft's OS.
        /// http://www.pinvoke.net/default.aspx/Constants/MAX_PATH.html
        /// </summary>
        private const int MAX_PATH = 260;

        /// <summary>
        /// Sanitizes the given path, replacing every unallowed filename character by a "_" character.
        /// 20110330 added normalization of directory separator chars EVH
        /// </summary>
        /// <param name="path">The path to sanitize.</param>
        /// <returns>The sanitized version of the path.</returns>
        public static string Sanitize(string path)
        {
            try
            {
                if (path.IsNullOrEmpty())
                {
                    throw new ArgumentException("Path is an empty or null string.");
                }
                var invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
                var invalidStr = $"[{invalidChars}]";
                var dir = Regex.Replace(Path.GetDirectoryName(path) ?? throw new InvalidOperationException(), invalidStr, "_");
                dir = Regex.Replace(dir, Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString());

                invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
                invalidStr = $"[{invalidChars}]";
                var file = Regex.Replace(Path.GetFileName(path), invalidStr, "_");
            
                var fullPath = Path.Combine(dir, file);
                if (fullPath.Length > MAX_PATH)
                {
                    return StringUtils.ShortenPathname(fullPath, MAX_PATH-5);
                  //  throw new PathTooLongException("File path length exceeds the allowed maximum of " + MaxPath + " characters.");
                }
                return fullPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Could not create sanitized version of path: " + path, e);
            }
        }

        /// <summary>
        /// Makes a unique filepath out of filepath by appending numbers between brackets.
        /// Example: if filepath = "C:\\Users\JohnDoe\Test" and the directory "C:\\Users\JohnDoe\"
        /// contains files named "Test.ext", "Test (2).ext", "Test (3).ext", this method returns
        /// "C:\\Users\JohnDoe\Test (4).ext"
        /// </summary>
        /// <param name="filepath">The path of the file that needs to be unique.</param>
        /// <returns>A path to a not yet existing file as described above.</returns>
        public static string Uniquify(string filepath)
        {
            string folder = Path.GetDirectoryName(filepath);
            string fileName = Path.GetFileName(filepath);
            const int maxAttempts = 1024;

            // Gets filename base and extension.
            var fileBase = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            // Builds hash set of filenames for performance.
            if (folder == null) throw new Exception("Invalid filepath: " + filepath);
            var files = new HashSet<string>(Directory.GetFileSystemEntries(folder));

            for (var index = 0; index < maxAttempts; index++)
            {
                // Trying with the original filename first, then incrementally adding an index.
                var name = (index == 0)
                    ? fileName
                    : $"{fileBase} ({index}){ext}";

                // Checks if path exists and is not too long. Shorten it if necessary
                var fullPath = Path.Combine(folder, name);
                if (fullPath.Length > MAX_PATH)
                {
                    return StringUtils.ShortenPathname(fullPath, MAX_PATH - 5);
                    //throw new PathTooLongException("File path length exceeds the allowed maximum of " + MaxPath + " characters.");
                }

                if (!files.Contains(fullPath))
                {
                    return fullPath;
                }
            }

            throw new Exception("Could not create unique filename in " + maxAttempts + " attempts");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string KeepDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            string cleaned = new string(s.Where(char.IsDigit).ToArray());
            return cleaned;
        }
    }
}