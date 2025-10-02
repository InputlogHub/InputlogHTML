using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InputLog.Core.IO.CSV
{
    /// <summary>
    /// This class is meant for reading CSV files and returning the output. The output may be returned as a 
    /// simple dictionary, or may be mapped onto an item class (one instance of the class for each row).
    /// This class is not safe for multithreading.
    /// </summary>
    public class CSVReader<T> where T : CSVItem, new()
    {
        #region Constants
        /// <summary>
        /// QUOTE is what encompasses a singel value, e.g. double quotes: "value1"
        /// </summary>
        private readonly char[] QUOTE;
        private bool USES_QUOTE
        {
            get { return this.QUOTE != null; }
        }

        /// <summary>
        /// SEPARATOR is what separates different values from one another, e.g. semicolon:
        /// value1;value2
        /// </summary>
        private readonly char[] SEPARATOR;
        #endregion

        #region Fields - Error Tracking & Reporting
        /// <summary>
        ///  A collection holding all the errors encountered so far while processing
        ///  the CSV file.
        /// </summary>
        private List<Error> _errors;
        public IEnumerable<Error> Errors
        {
            get
            {
                return this._errors;
            }
        }

        /// <summary>
        /// Flag stating whether any errors have been encountered while
        /// processing the CSV file.
        /// </summary>
        public bool ErrorsEncountered
        {
            get
            {
                return (Errors != null) && (Errors.Count() > 0);
            }
        }

        /// <summary>
        /// Returns the highest severity of errors encountered in while processing
        /// the CSV file.
        /// </summary>
        public SeverityLevel? ErrorHighestSeverity
        {
            get
            {
                if (!this.ErrorsEncountered)
                {
                    return null;
                }

                SeverityLevel highestSeverity = this.Errors.Max(error => error.Severity);
                return highestSeverity;
            }
        }
        #endregion

        /// <summary>
        /// Create a new CSV reader and specify the format of the text to be read. The default is a 
        /// comma separated, non-quoted CSV file.
        /// </summary>
        /// <param name="separator">The separator string for values</param>
        public CSVReader(char separator = ',')
        {
            this.SEPARATOR = new char[] { separator };
            this.QUOTE = null;
        }

        /// <summary>
        /// Create a new CSV reader and specify the format of the text to be read.
        /// </summary>
        /// <param name="separator">The separator string for values</param>
        /// <param name="quote">The string used to quote the values.</param>
        public CSVReader(char separator, char quote)
        {
            this.SEPARATOR = new char[] { separator };
            this.QUOTE = new char[] { quote };
        }

        /// <summary>
        /// Read a CSV file at the specified path. Returns the information found in the CSV.
        /// </summary>
        /// <param name="path">Path to the CSV file to be read.</param>
        /// <returns>A processed CSV file containing all the information.</returns>
        public CSVFileContent<T> ReadFile(string path)
        {
            // Read the file.
            if (!File.Exists(path))
            {
                this.AddError(new Error(
                    0,
                    "File at \"" + path + "\" not found.",
                    SeverityLevel.FATAL
                ));
                return null;
            }

            int currentLineNo = 1;
            CSVFileContent<T> content = this.GetFileContainer();
            try
            {
                using (StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8))
                {
                    string headerLine = reader.ReadLine();
                    string[] headers = this.ParseLine(headerLine);
                    // Check header for possible errors
                    this.CheckHeader(currentLineNo, headers);
                    content.SetHeaders(headers);

                    // Process following lines.
                    currentLineNo += 1;
                    string line = reader.ReadLine();
                    while (!String.IsNullOrEmpty(line))
                    {
                        string[] lineParts = this.ParseLine(line);

                        // Check line for possible errors.
                        Error? error = this.CatchErrorsInLine(currentLineNo, lineParts, headers.Length);
                        if (error.HasValue && error.Value.Severity >= SeverityLevel.FATAL)
                        {
                            break;
                        }
                        else if (!error.HasValue || error.Value.Severity <= SeverityLevel.INFO)
                        {
                            try
                            {
                                // Lines that have an error SeverityLevel higher than Info - such as "Warning", or "Error" - 
                                // are considered wrongly processed and will not be added to the list of items.
                                content.AddItem(currentLineNo, lineParts);
                            }
                            catch (Exception e)
                            {
                                // Add an error for the Exception, ignore the line and continue processing.
                                // The line should not have been added to the list of Lines, the throwing
                                // of the exception should have prevented that.

                                this.AddError(new Error(
                                    currentLineNo,
                                    "Line could not be processed correctly: " + e.Message,
                                    SeverityLevel.ERROR,
                                    e
                                ));
                            }
                        }

                        // Process next line.
                        currentLineNo += 1;
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                this.AddError(new Error(
                    currentLineNo,
                    e.Message,
                    SeverityLevel.FATAL
                ));
            }
            return content;
        }


        /// <summary>
        /// Get a storage container to store the information read
        /// from the CSV file.
        /// </summary>
        /// <typeparam name="T">The items to which the information will be saved.</typeparam>
        /// <returns>A storage container to add information to that has
        /// been read from the CSV file</returns>
        protected virtual CSVFileContent<T> GetFileContainer()
        {
            CSVFileContent<T> content = new CSVFileContent<T>();
            return content;
        }


        #region Error handling while parsing CSV file.
        /// <summary>
        /// Checks whether there is any cause to believe the line is incorrect. If there is
        /// any cause to believe it is incorrect an appropriate error is added to the error list and the error flag
        /// is set.
        /// </summary>
        /// <param name="currentLineNo">Current line number</param>
        /// <param name="lineParts">The line found, after processing.</param>
        /// <param name="headerSize">The size of the header for this CSV file.</param>
        /// <returns>An error, if one has been encountered, null if no errros occurred.</returns>
        private Error? CatchErrorsInLine(int currentLineNo, string[] lineParts, int headerSize)
        {
            Error? error = null;

            if (lineParts.Length != headerSize)
            {
                string issue = (lineParts.Length > headerSize) ?
                    "Line contains more data fields than has been announced in the header. Line has been ignored." :
                    "Line contains fewer data fields than has been announced in the header. Line has been ignored.";

                error = new Error(
                    currentLineNo,
                    issue + "\n" +
                        "Header size = " + headerSize + "; line size = " + lineParts.Length,
                    SeverityLevel.ERROR
                );

                this.AddError(error.Value);
                return error;
            }

            return error;
        }

        /// <summary>
        /// Checks whether there is any cause to believe the header is incorrect. If there is
        /// any cause to believe it is incorrect a warning message is added and the error flag
        /// is set.
        /// </summary>
        /// <param name="currentLineNo">Current line number</param>
        /// <param name="headers">The headers found, after processing.</param>
        /// <returns>An error, if one has been encountered, null if no errros occurred.</returns>
        private Error? CheckHeader(int currentLineNo, string[] headers)
        {
            Error? error = null;

            if (headers.Length == 0)
            {
                error = new Error(
                    currentLineNo,
                    "Header has length 0, please check file formatting. CSV Settings used: \n" +
                        "\t-Separators: " + String.Join("\t", this.SEPARATOR) + "\n" +
                        "\t-Value quotation: '" + ((this.USES_QUOTE) ? String.Join("\t", this.QUOTE) : "'") + "\n",
                    SeverityLevel.WARNING
                );
                this.AddError(error.Value);
                return error;
            }

            if (headers.Length == 1)
            {
                error = new Error(
                    currentLineNo,
                    "Header has length 1, If your CSV is intented to just have 1 column then there is no cause for worry. " +
                    "CSV Settings used: \n" +
                        "\t-Separators: " + String.Join("\t", this.SEPARATOR) + "\n" +
                        "\t-Value quotation: '" + ((this.USES_QUOTE) ? String.Join("\t", this.QUOTE) : "'") + "\n",
                    SeverityLevel.INFO
                );
                this.AddError(error.Value);
                return error;
            }
            return error;
        }

        /// <summary>
        /// Add an error item to the list of errors encountered.
        /// </summary>
        /// <param name="error">error to add.</param>
        private void AddError(Error error)
        {
            if (this._errors == null)
            {
                this._errors = new List<Error>();
            }
            this._errors.Add(error);
        }
        #endregion

        /// <summary>
        /// Parses a single line from a CSV file, returning all the separate elements in 
        /// this line in an array of strings. Order in which the elements are retrieved are
        /// the same as the order they have on the line.
        /// </summary>
        /// <param name="headerLine"></param>
        /// <returns></returns>
        private string[] ParseLine(string headerLine)
        {
            string[] parts = headerLine.Split(this.SEPARATOR, System.StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                bool emptyPart = String.IsNullOrEmpty(parts[i]);
                if (!emptyPart && this.USES_QUOTE)
                {
                    parts[i] = parts[i].Trim(this.QUOTE);
                }
            }
            return parts;
        }

        #region CSVFileContent definition
        /// <summary>
        /// Class representing the contents of a CSV file.
        /// </summary>
        public class CSVFileContent<T> where T : CSVItem, new()
        {
            /// <summary>
            /// The headers of the CSV file in order of appearance.
            /// </summary>
            public string[] Headers;
            private bool _isHeaderSet;

            /// <summary>
            /// Keeps indices of those headers that have delegate functions
            /// attached to them (for a class T), and on the opposite side, the headers that do
            /// not have delegates associated with them.
            /// </summary>
            private HashSet<int> _headersWithDelegate;
            private HashSet<int> _headersWithoutDelegate;

            /// <summary>
            /// The items found in the CSV file.
            /// </summary>
            private List<T> _items;
            public virtual IReadOnlyCollection<T> Items
            {
                get
                {
                    return _items.AsReadOnly();
                }
            }

            /// <summary>
            /// Create a new CSVFileContent class.
            /// </summary>
            public CSVFileContent()
            {
                this._headersWithDelegate = new HashSet<int>();
                this._headersWithoutDelegate = new HashSet<int>();
                this._isHeaderSet = false;
                this._items = new List<T>();
            }


            /// <summary>
            /// Specify the headers. Headers must be set before you start adding items.
            /// </summary>
            /// <param name="headers">The headers read from the CSV file.</param>
            internal virtual void SetHeaders(string[] headers)
            {
                Debug.Assert(headers != null);

                this.Headers = headers;
                T dummy = new T();

                this._headersWithDelegate.Clear();
                this._headersWithoutDelegate.Clear();

                for (int i = 0; i < headers.Length; i++)
                {
                    // If there is a delegate function for header at position i ->
                    if (dummy.SetDelegates.Keys.Contains(headers[i]))
                    {
                        this._headersWithDelegate.Add(i);
                    }
                    else
                    {
                        this._headersWithoutDelegate.Add(i);
                    }
                }
                this._isHeaderSet = true;
            }

            /// <summary>
            /// Add an item to the CSV File Contents. Items can only be added after the headers
            /// have been set.
            /// </summary>
            /// <param name="currentLineNo">Line number at which the item has been found</param>
            /// <param name="lineParts">The line, procesed into parts. The indices of the items
            /// should correspond to the indices of the headers.</param>
            internal virtual void AddItem(int currentLineNo, string[] lineParts)
            {
                T item = new T();
                FillItemWithLine(currentLineNo, lineParts, item);
                this._items.Add(item);
            }

            /// <summary>
            /// Fill an item T with the information found on the given line.
            /// </summary>
            /// <param name="currentLineNo">The line number of the line</param>
            /// <param name="lineParts">The information on the line, separated
            /// in its different parts.</param>
            /// <param name="item">The item to which the information of the 
            /// given line will be added.</param>
            protected void FillItemWithLine(int currentLineNo, string[] lineParts, T item)
            {
                Debug.Assert(this._isHeaderSet);
                Debug.Assert(lineParts.Length == this.Headers.Length);

                item.LineNo = currentLineNo;
                foreach (int currentIndex in this._headersWithDelegate)
                {
                    // For each header that has a delegate attached, call the delegate
                    // associated with that header, to set the associated data.
                    string currentHeader = this.Headers[currentIndex];
                    string currentValue = lineParts[currentIndex];
                    item.SetDelegates[currentHeader](currentValue);
                }

                foreach (int leftoverIndex in this._headersWithoutDelegate)
                {
                    string leftoverData = lineParts[leftoverIndex];
                    string leftoverHeader = this.Headers[leftoverIndex];
                    item.AddLeftoverData(leftoverHeader, leftoverData);
                }
            }
        }

        #endregion


        #region Define error struct
        /// <summary>
        /// Holds information about an ecountered error while processing
        /// the CSV file.
        /// </summary>
        public struct Error
        {
            /// <summary>
            /// Line number of the occurrence
            /// </summary>
            public int Line
            {
                get;
                private set;
            }

            /// <summary>
            /// Informative message describing the error.
            /// </summary>
            public string Message
            {
                get;
                private set;
            }

            /// <summary>
            /// Severity of the error. Fatal implies that the processing
            /// was stopped.
            /// </summary>
            public SeverityLevel Severity
            {
                get;
                private set;
            }

            /// <summary>
            /// The exception that caused this error, if any.
            /// </summary>
            public Exception Exception
            {
                get;
                private set;
            }

            /// <summary>
            /// Construct an error struct
            /// </summary>
            /// <param name="line">line number of the error occurrence</param>
            /// <param name="message">message describing the problem</param>
            /// <param name="severity">severity level of the error</param>
            /// <param name="e">The exception that caused this error, if any.</param>
            public Error(int line, string message, SeverityLevel severity, Exception e = null)
                : this()
            {
                this.Line = line;
                this.Message = message;
                this.Severity = severity;
                this.Exception = e;
            }
        }

        /// <summary>
        ///  Severity level of an error struct
        /// </summary>
        public enum SeverityLevel
        {
            INFO = 10,
            WARNING = 20,
            ERROR = 30,
            FATAL = 40,
        }
        #endregion
    
    }

    /// <summary>
    ///     Exception thrown by a CSVReader
    /// </summary>
    public class CSVReaderException : Exception
    {
        public CSVReaderException(string message, Exception inner) : base(message, inner) { }
    }

}
