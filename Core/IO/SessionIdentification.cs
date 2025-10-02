using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Linq;
using InputLog.Core.Util;

namespace InputLog.Core.IO
{
    /// <summary>
    /// SessionIdentification collects a number of attributes linked to a specific logging session
    /// as identified by a unique id (META_GUID).
    /// The SessionIdentification attributes make up the header section of an ifx-file.
    /// Some of these will be reported on the analysis sheet (SESSION_), others are used 
    /// during the analysis, notably the META_LOGRELATIVECREATIONTIME or are necessary to
    /// perform pre- or post processing.
    /// </summary>
    public sealed class SessionIdentification: CSV.CSVItem
    {
        private const string DATE_FORMAT = "dd/MM/yy HH:mm:ss.FFF";

        private const string OLD_META_PREFIX = "Old";

        public const string META_LOGFILE = "__LogFileName";
        public const string META_LOGCREATIONDATE = "__LogCreationDate";
        private const string META_LOGCREATIONTIMESTAMP = "__LogCreationTimeStamp";
        private const string META_LOGRELATIVECREATIONTIME = "__LogRelativeCreationDate";
        private const string META_CONVERSION_FROM_FORMAT = "__ConversionFromFormat";
        private const string META_CONVERSION_TO_FORMAT = "__ConversionToFormat";
        private const string META_CONVERTED_FROM = "__ConvertedFrom";
        private const string META_GUID = "__GUID";
        private const string META_LOGPROGRAMVERSION = "__LogProgramVersion";
        private const string META_ANALYSISPROGRAMVERSION = "__AnalysisProgramVersion";
        private const string META_MAINDOCUMENT = "__MainDocument";
        private const string SESSION_RESTR_LOGGING = "Restricted Logging";

        public const string META_COPYTASK = "__copytask";
        public const string SESSION_PARTICIPANTKEY = "Participant";
        public const string SESSION_AGEKEY = "Age";
        public const string SESSION_GENDERKEY = "Gender";
        public const string SESSION_SESSIONKEY = "Session";
        public const string SESSION_GROUPKEY = "Group";
        public const string SESSION_EXPERIENCEKEY = "Experience";
        public const string SESSION_LANGUAGE = "Text Language";
        public const string SESSION_LITE_ACCOUNT = "Account";
        public const string SESSION_KEYBOARD = "Keyboard";

        public readonly Dictionary<string, string> SessionInfo;
        public readonly Dictionary<string, string> MetaInfo;

        public SessionIdentification()
        {
            SessionInfo = new Dictionary<string, string>();
            MetaInfo = new Dictionary<string, string>();
        }

        public void ReadXml(XmlReader reader)
        {
            bool endMi = !reader.IsEmptyElement;
            reader.ReadStartElement("meta");
            while (reader.IsStartElement("entry"))
            {
                reader.ReadStartElement("entry");
                reader.ReadStartElement("key");
                string key = reader.ReadContentAsString();
                reader.ReadEndElement(); // Reads end element and then: Whitespace, value = \n

                /*
                 * This code is not very readable, admittedly. 
                 * But, it's one of the few ways to actually be able to get the entire
                 * copyTask xml as a string from the content of the IDFX file.
                 * 
                 * Using ReadStartElement method etc, followed by reader.ReadInnerXML()
                 * returns empty strings as values.
                 */
                reader.Read(); // Reads Element, name = value
                string value = reader.ReadInnerXml(); // Reads Text, value = 6.1.2.0 OR value = e.g. the entire copyTask XML!
                reader.Read(); // Reads EndElement, name = value
                reader.Read(); // Reads Whitespace, value = \n
                MetaInfo[key] = value;
                reader.Read(); // Reads up to the next Element (finishing </entry>)
            }
            if (endMi)
                reader.ReadEndElement();
            bool endSi = !reader.IsEmptyElement;
            reader.ReadStartElement("session");
            while (reader.IsStartElement("entry"))
            {
                reader.ReadStartElement("entry");
                string key = reader.ReadElementString("key");
                string value = reader.ReadElementString("value");
                SessionInfo[key] = value;
                reader.ReadEndElement();
            }
            if (endSi)
                reader.ReadEndElement();
        }

        #region creation date
        /// <summary>
        ///     Removing the creation time of the current session at the end of a logging session
        ///     in order to solve a problem with the pause time calculation when recording immediately
        ///     a new session without closing Inputlog first.
        /// </summary>
        public void RemoveCreationDate()
        {
            if (MetaInfo.ContainsKey(META_LOGCREATIONDATE)) MetaInfo[META_LOGCREATIONDATE] = null;
            if (MetaInfo.ContainsKey(META_LOGRELATIVECREATIONTIME)) MetaInfo[META_LOGRELATIVECREATIONTIME] = null;
            if (MetaInfo.ContainsKey(META_LOGCREATIONTIMESTAMP)) MetaInfo[META_LOGCREATIONTIMESTAMP] = null;
        }

        public void SetCreationDate(DateTime dateTime)
        {
            DateTime now = DateTime.Now;
            MetaInfo[META_LOGCREATIONDATE] = now.ToString(DATE_FORMAT);
            MetaInfo[META_LOGCREATIONTIMESTAMP] = now.ConvertToTimestamp().ToString();
        }

        public void SetCreationDate(string s)
        {
            MetaInfo[META_LOGCREATIONDATE] = s;
        }

        public ulong GetCreationTimestamp()
        {
            return MetaInfo.ContainsKey(META_LOGCREATIONTIMESTAMP)
                ? ulong.Parse(MetaInfo[META_LOGCREATIONTIMESTAMP])
                : GetCreationDate().ConvertToTimestamp();
        }

        public DateTime GetCreationDate()
        {
            if (MetaInfo.ContainsKey(META_LOGCREATIONTIMESTAMP))
            {
                return DateTimeUtils.FromTimestamp(ulong.Parse(MetaInfo[META_LOGCREATIONTIMESTAMP]));
            }
            DateTime creationDate;
            string creationDateString = MetaInfo[META_LOGCREATIONDATE];
            DateTime.TryParse(creationDateString, out creationDate);

            return creationDate;
        }

        public bool HasCreationDate()
        {
            return MetaInfo.ContainsKey(META_LOGCREATIONDATE);
        }

        public string GetCreationDateString()
        {
            var d = GetCreationDate();
            return d.ToString(DATE_FORMAT);
        }
        #endregion

        #region filename
        public string GetFileName()
        {
            return MetaInfo[META_LOGFILE];
        }

        public void SetFileName(string sourcePath)
        {
            MetaInfo[META_LOGFILE] = sourcePath;
        }

        #endregion

        #region mainDocument

        public string GetMainDocument()
        {
            if (!MetaInfo.ContainsKey(META_MAINDOCUMENT)) return string.Empty;
            var value = MetaInfo[META_MAINDOCUMENT];
            if (value.IsNullOrEmpty())
            {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Default value "WordLog.docx"
        /// </summary>
        /// <returns></returns>
        public void SetMainDocument(string mainDoc)
        {
            if (string.IsNullOrEmpty(mainDoc))
            {
                MetaInfo[META_MAINDOCUMENT] = "WordLog.docx";
            }
            else
            {
                MetaInfo[META_MAINDOCUMENT] = mainDoc;
            }
        }
        #endregion

        #region relative creation time
        public ulong GetRelativeCreationTime()
        {
            return ulong.Parse(MetaInfo[META_LOGRELATIVECREATIONTIME]);
        }

        public bool HasRelativeCreationTime()
        {
            if (!MetaInfo.ContainsKey(META_LOGRELATIVECREATIONTIME)) return false;
            var value = MetaInfo[META_LOGRELATIVECREATIONTIME];
            return value != null;
        }

        public void SetRelativeCreationTime(ulong val)
        {
            MetaInfo[META_LOGRELATIVECREATIONTIME] = val.ToString();
        }
        #endregion

        #region language
        /// Array of two-letter code text languages according to ISO 639-1.
        /// Default value 'EN' for English.
        /// See : https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        public string GetLanguage()
        {
            if (!SessionInfo.ContainsKey(SESSION_LANGUAGE)) return "EN";
            var lang = SessionInfo[SESSION_LANGUAGE];
            return string.IsNullOrEmpty(lang) ? "EN" : lang;
        }

        /// <summary>
        /// A useful language symbol is a two-letter code according to ISO 639-1.
        /// It excludes not only the empty string but also the 'other' label.
        /// </summary>
        /// <returns>bool true if language code has two letters. </returns>
        public bool HasLanguage()
        {
            SessionInfo.TryGetValue(SESSION_LANGUAGE, out var lang);
            return lang != null && lang.Length == 2;
        }
        /// Array of two-letter code text languages according to ISO 639-1.
        public void SetLanguage(string lang)
        {
            SessionInfo[SESSION_LANGUAGE] = lang;
        }
        #endregion

        #region sessionlogging
        public void SetSessionLogging(string s)
        {
            SessionInfo[SESSION_RESTR_LOGGING] = s;
        }

        /// <summary>
        /// Returns true when the option box 'no logging outside the main document' is checked
        /// </summary>
        /// <returns></returns>
        public bool IsRestrictedLogging()
        {
            return SessionInfo[SESSION_RESTR_LOGGING].Contains("No keystroke");
        }

        #endregion

        #region participant
        public string GetParticipant()
        {
            return SessionInfo[SESSION_PARTICIPANTKEY];
        }

        public void SetParticipant(string p)
        {
            SessionInfo[SESSION_PARTICIPANTKEY] = p;
        }
        #endregion

        #region guid
        public void GenerateGuid()
        {
            MetaInfo[META_GUID] = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return MetaInfo.ContainsKey(META_GUID) ? MetaInfo[META_GUID] : null;
        }
        #endregion

        #region conversion
        public void SetConversion(string from)
        {
            MetaInfo[META_CONVERTED_FROM] = from;
        }

        public void SetConversionFormats(string inputFormat, string outputFormat)
        {
            MetaInfo[META_CONVERSION_FROM_FORMAT] = inputFormat;
            MetaInfo[META_CONVERSION_TO_FORMAT] = outputFormat;
        }

        public string GetConversion()
        {
            return MetaInfo.ContainsKey(META_CONVERTED_FROM) ? MetaInfo[META_CONVERTED_FROM] : null;
        }

        public bool IsConverted()
        {
            return MetaInfo.ContainsKey(META_CONVERTED_FROM);
        }
        #endregion

        #region versions
        public void SetAnalysisVersion(string version)
        {
            MetaInfo[META_ANALYSISPROGRAMVERSION] = version;
        }

        public void SetLogVersion(string version)
        {
            MetaInfo[META_LOGPROGRAMVERSION] = version;
        }

        public string GetAnalysisVersion()
        {
            return MetaInfo.ContainsKey(META_ANALYSISPROGRAMVERSION) ? MetaInfo[META_ANALYSISPROGRAMVERSION] : null;
        }

        public string GetLogVersion()
        {
            return MetaInfo.ContainsKey(META_LOGPROGRAMVERSION) ? MetaInfo[META_LOGPROGRAMVERSION] : null;
        }

        public int GetVersionNumber()
        {
            string versionString = MetaInfo.ContainsKey(META_LOGPROGRAMVERSION)
                ? MetaInfo[META_LOGPROGRAMVERSION] : "0.0.0.0";

            var sb = new StringBuilder(versionString.Length);
            foreach (char c in versionString.Where(char.IsDigit))
                sb.Append(c);

            return int.Parse(sb.ToString());
        }
        #endregion

        #region dictionaries
        public void AddSessionInfo(Dictionary<string, string> sessionInfo)
        {
            foreach (var pair in sessionInfo)
            {
                SessionInfo[pair.Key] = pair.Value;
            }
        }

        public void AddSessionInfo(string key, string val)
        {
            SessionInfo[key] = val;
        }

        public Dictionary<string, string> GetSessionInfo()
        {
            return SessionInfo;
        }

        public Dictionary<string, string> GetMetaInfo()
        {
            return MetaInfo;
        }
        #endregion

        public void CopyOldInfo()
        {
            if (MetaInfo.ContainsKey(META_LOGCREATIONDATE))
                MetaInfo[OLD_META_PREFIX + META_LOGCREATIONDATE] = MetaInfo[META_LOGCREATIONDATE];

            if (MetaInfo.ContainsKey(META_GUID))
                MetaInfo[OLD_META_PREFIX + META_GUID] = MetaInfo[META_GUID];
        }

        #region Implement CSV Item delegate methods.
        protected override void InitSetDelegates()
        {
            /* Has no set delegates */
        }

        protected override void InitGetDelegates()
        {
            _getDelegates.Add("logfile", GetFileName);
            _getDelegates.Add("participant", GetParticipant);
            _getDelegates.Add("creation_date", GetCreationDateString);
        }

        #endregion
    }
}