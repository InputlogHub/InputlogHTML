using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using InputLog.Core.Analyses;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Util;
using InputLog.Core.Util.Progress;

namespace GUI.Tabs.Analyze
{
    public abstract class AbstractAnalyzer : ProgressTrackableAction
    {
        #region fields

        /// <summary>
        ///     Collection of all the files to which this analyzer has written.
        /// </summary>
        public readonly List<string> DestinationFiles = new List<string>();

        public IAnalysisSummary CurrentSummary;

        private string _destDir;

        private IDictionary<string, IDictionary<string, object>> _extraInfo;

        /// <summary>
        ///     Path to original output.
        /// </summary>
        protected string OrgDocPath;

        /// <summary>
        ///     Path to the final output of this analysis.
        /// </summary>
        public string OutputFilePath  { get; set; }

        public SessionIdentification SessionId { get; private set; }

        private Analysis CurrentAnalysis { get; set; }

        public List<Event> Events { get; private set; }

        public bool IsReportGenerator { get; private set; }

        /// <summary>
        ///     Name of the analysis
        /// </summary>
        public string Name
        {
            get
            {
                Type type = GetType();
                FieldInfo info = type.GetField("NAME");

                // Using the PropertyInfo to retrieve the value from the type by not passing in an instance.
                object value = info.GetValue(null);
                return (string) value;
            }
        }

        /// <summary>
        ///     Abbreviation for the analysis
        /// </summary>
        private string Abbr
        {
            get
            {
                Type t = GetType();
                FieldInfo propertyInfo = t.GetField("ABBR",
                                                    BindingFlags.Public | BindingFlags.Static |
                                                    BindingFlags.FlattenHierarchy);

                // Using the PropertyInfo to retrieve the value from the type by not passing in an instance.
                if (propertyInfo == null) return "";
                object value = propertyInfo.GetValue(null);
                return (string) value;
            }
        }

        /// <summary>
        ///     Path to a source file (*.idfx).
        /// </summary>
        public string SourcePath { get; private set; }

        #endregion

        public void StartSession(SessionIdentification sessionId, List<Event> events, bool isReportGeneration)
        {
            SessionId = sessionId;
            Events = events;
            SessionId.SetAnalysisVersion(Application.ProductVersion);
            _extraInfo = GetParameters();
            IsReportGenerator = isReportGeneration;
        }

        protected virtual IDictionary<string, IDictionary<string, object>> GetParameters()
        {
            return new Dictionary<string, IDictionary<string, object>>();
        }

        public void SetPaths(string srcFile, string dstDir, string orgPath)
        {
            SourcePath = srcFile;
            _destDir = dstDir;
            OrgDocPath = orgPath ?? "";

            Directory.CreateDirectory(_destDir);
            var tmpPath = PathSanitizer.Sanitize(ExtendFileName(SessionId, SourcePath, _destDir));
            OutputFilePath = PathSanitizer.Uniquify(tmpPath);
            SessionId.SetFileName(Path.GetFileName(SourcePath));

            // Looking for existing files so that they don't get overwritten.
            DestinationFiles.Add(OutputFilePath);
        }

        /// <summary>
        ///     This method should be implemented by the subclasses so that it returns the correct analysis
        ///     for performing the actual analysis.
        /// </summary>
        /// <returns>The analyzer that implements the actual analysis.</returns>
        protected virtual Analysis GetAnalysis()
        {
            throw new NotImplementedException();
        }


        public virtual AnalysisDescription GetDescription(string workDir)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Given the destination folder and a source (log) file, this method returns a
        ///     full path of the file where the result of the analysis should be stored. This pathname may contain
        ///     place holders for sessionidentification information using the ${key} notation.
        /// </summary>
        /// <param name="sessionId">The session identification.</param>
        /// <param name="srcFile">The source (log) file</param>
        /// <param name="destDir">The destination directory path.</param>
        /// <returns>String with full file path of the file where the result of the analysis should be stored</returns>
        protected virtual string ExtendFileName(SessionIdentification sessionId, string srcFile, string destDir)
        {
            var part = sessionId.GetParticipant();
            var dateStr = sessionId.GetCreationDateString();
            try
            {
                var newDate = dateStr.Replace('-', '/');
                var dateTime = DateTime.ParseExact(newDate, "dd/MM/yy HH:mm:ss.FFF", null);
                dateStr = dateTime.ToString("yyyyMMdd");
            }
            catch (Exception)
            {
                var sb = new StringBuilder(8);
                foreach (char c in dateStr.Where(char.IsLetterOrDigit))
                {
                    sb.Append(c);
                    if (sb.Length == 8)
                    {
                        break;
                    }
                }
                dateStr = sb.ToString();
            }

            string logVersion = Path.GetFileNameWithoutExtension(srcFile);
            if (logVersion != null)
            {
                int lastIndexLocation = logVersion.LastIndexOf("_", StringComparison.Ordinal);
                logVersion = lastIndexLocation > 0 ? logVersion.Substring(lastIndexLocation + 1) : string.Empty;
            }
            return Path.Combine(destDir, $"{part}_{dateStr}_{logVersion}_{Abbr}.xml");
        }

        protected string GetDestinationDir()
        {
            if (_destDir == null)
            {
                IEnumerable<IGrouping<string, string>> destinationDirs = DestinationFiles.GroupBy(Path.GetDirectoryName);
                _destDir = destinationDirs.First().First();
            }

            return _destDir;
        }

        private IAnalysisWriter GetWriter(Analysis analysis, string outputFilePath)
        {
            return new AnalysisWriterFactory().Create(analysis, outputFilePath);
        }

        public bool DoAnalysis()
        {
            CurrentAnalysis = GetAnalysis();
           
            ReportProgress(this, new ProgressEventArgs("Analysis started"));

            if (BeforeAnalysis())
            {
                CurrentSummary = CurrentAnalysis.DoAnalysis();
                AfterAnalysis(CurrentAnalysis, CurrentSummary);

                ReportProgress(this, new ProgressEventArgs("Analysis finished, creating report"));
                return true;
            }
            else
            {
                ReportProgress(this, new ProgressEventArgs("Analysis aborted, creating report"));
                return false;
            }
        }

        public bool WriteAnalysis()
        {
            using (IAnalysisWriter writer = GetWriter(CurrentAnalysis, OutputFilePath))
            {
                if (BeforeWrite(writer))
                {
                    writer.WriteDocument(SessionId, _extraInfo, CurrentSummary);
                    AfterWrite(writer);
                    ReportProgress(this, new ProgressEventArgs("Done"));
                    return true;
                }
                else
                {
                    writer.Abort();
                    ReportProgress(this, new ProgressEventArgs("Aborted"));
                    return false;
                }
            }
        }

        private static bool BeforeAnalysis()
        {
            return true;
        }

        protected virtual void AfterAnalysis(Analysis analysis, IAnalysisSummary summary)
        {
        }

        protected virtual bool BeforeWrite(IAnalysisWriter writer)
        {
            return true;
        }

        protected virtual void AfterWrite(IAnalysisWriter writer)
        {
        }
    }
}