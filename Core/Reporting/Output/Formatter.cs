using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InputLog.Core.IO;
using InputLog.Core.Reporting.ReportTemplate;

namespace InputLog.Core.Reporting.Output
{
    /// <summary>
    /// Abstract base class that can be inherited from to implement
    /// different formatters. Formatters can define the format in which
    /// they save a report.
    /// </summary>
    public abstract class Formatter
    {
        public Formatter()
        {
            _buffer = new StringBuilder();
        }

        #region Structural methods
        public virtual void StartReport(ReportTemplate.ReportTemplate report) { }
        public virtual void EndReport() { }

        public virtual void StartBlocks(int nrOfBlocks) { }
        public virtual void StartBlock(BlockTemplate template) { }
        public virtual void EndBlock() { }
        public virtual void EndBlocks() { }

        public virtual void StartElements(int nrOfElements) { }
        public virtual void StartElement(ElementTemplate template) { }
        public virtual void EndElement() { }
        public virtual void EndElements() { }

        public virtual void StartValues(int nrOfValues) { }
        public virtual void StartValue(ValueTemplate template) { }
        public virtual void EndValue() { }
        public virtual void EndValues() { }
        #endregion

        #region Content methods
        /// <summary>
        /// Add a labeled value to the report. The labels and texts given here are 
        /// only used when the template for this element does not specify any differen texts.
        /// </summary>
        /// <param name="label">Label for the value</param>
        /// <param name="value">Value</param>
        /// <param name="intro">Default introduction for the element - not from template</param>
        public abstract void AddLabeledValue(string label, string value, string intro = null);

        /// <summary>
        /// Add a labeled image to the report. The labels and texts specified here
        /// are only used when the template for this element does not specify
        /// any different texts
        /// </summary>
        /// <param name="label">Label for the image</param>
        /// <param name="imageStream">Stream containing the image</param>
        /// <param name="intro">Default introduction for the element - not from template</param>
        /// <param name="fullPage">Should the image be printed full page or just smaller? True = full page</param>
        public abstract void AddLabeledImage(string label, Stream imageStream, string intro = null, bool fullPage = false);
        #endregion

        #region Output
        public abstract void WriteToStream(System.IO.Stream stream);
        public abstract void WriteToFile(string filepath);
        public abstract string GetExtension();
        public abstract string GetAffix();
        #endregion


        #region Session info parsing
        /// <summary>
        ///     Reference to a sessionIdentification object
        ///     that will be used to pull the sessionValues from that
        ///     are found in the templates.
        /// </summary>
        protected SessionIdentification _sessionId;

        /// <summary>
        /// Matches session id replaceable
        /// </summary>
        protected const string PATTERN = @"(?<all>#__SESSION_(?<key>\w+))";
        private const string ALL = "all";
        private const string KEY = "key";
        private const string KEY_DATE = "Date";
        private const string KEY_TIME = "Time";
        private readonly string[] SPECIAL_KEYS = new string[] { KEY_DATE, KEY_TIME };

        /// <summary>
        ///     Contains the string while it's being parsed
        ///     for session information.
        /// </summary>
        private StringBuilder _buffer;

        /// <summary>
        ///     Set the session identification object that will be used to 
        ///     fill the session-values found in the ReportTemplate
        /// </summary>
        /// <param name="sessionId">Session identification object.</param>
        public void SetSessionID(SessionIdentification sessionId)
        {
            _sessionId = sessionId;
        }

        /// <summary>
        ///     Parse a string and replace any session info placeholders
        ///     within the string by their corresponding session info.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The input string with placeholders replaced by 
        /// actual values.</returns>
        public string ParseForSessionData(string input)
        {
            if (string.IsNullOrEmpty(input) || _sessionId == null)
            {
                return input;
            }

            this._buffer.Clear();
            int currentIndex = 0;
            foreach (Match match in Regex.Matches(input, PATTERN))
            {
                string key = match.Groups[KEY].Value;
                // Keys such as Text_Language correspond to session key: Text Language
                key = key.Replace('_', ' ');
                string replacementValue;

                if (_sessionId.SessionInfo.ContainsKey(key))
                {
                    replacementValue = _sessionId.SessionInfo[key];
                }
                else if (this.SPECIAL_KEYS.Contains(key))
                {
                    replacementValue = _getSessionDataSpecial(key);
                }
                else
                {
                    replacementValue = match.Groups[ALL].Value;
                }

                // Add text from current to beginning of match
                int distanceCurrentToBegin = match.Groups[ALL].Index - currentIndex;
                _buffer.Append(input.Substring(currentIndex, distanceCurrentToBegin));

                // Add match replacementValue
                _buffer.Append(replacementValue);

                // Set currentIndex to first index after match. 
                // (currentIndex might point to out of bounds location here (+1))
                currentIndex = match.Groups[ALL].Index + match.Groups[ALL].Length;
            }

            // Add left overs
            if (currentIndex < input.Length)
            {
                int remainingLength = input.Length - currentIndex;
                _buffer.Append(input.Substring(currentIndex, remainingLength));
            }

            return _buffer.ToString();
        }

        /// <summary>
        ///     Special function that handles "session" data that is not 
        ///     strictly speaking session data, such as the date. Only the date and
        ///     time will be using different keys and can be retrieve separately.
        /// </summary>
        /// <param name="key">Key of the special data that is requested.</param>
        /// <returns></returns>
        private string _getSessionDataSpecial(string key)
        {
            string timeString = _sessionId.MetaInfo[SessionIdentification.META_LOGCREATIONDATE];
            DateTime time = DateTime.Parse(timeString, CultureInfo.CreateSpecificCulture("nl-be").DateTimeFormat);

            if (key == KEY_DATE)
            {
                return time.ToShortDateString();
            }

            if (key == KEY_TIME)
            {
                return time.ToShortTimeString();
            }

            return null;
        }
        #endregion

    }
}
