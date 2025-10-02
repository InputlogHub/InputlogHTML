using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.Preprocessing;
using InputLog.Core.Reporting;
using InputLog.Core.Reporting.Output;
using InputLog.Core.Reporting.Report;
using InputLog.Core.Reporting.ReportTemplate;
using InputLog.Core.Util;
using InputLog.Core.IO.HTML;


namespace GUI.Tabs.Analyze
{
    /// <summary>
    /// Business logic for the Analyze tab.
    /// </summary>
    public class AnalyzeModel
    {
        private readonly List<string> _thesePaths = new List<string>();
        private readonly List<string> _theOriginalPaths = new List<string>();
        private string _myDestinationPath = Properties.Settings.Default.Workspace;
        private static string _currentSourcePath;

        /// <summary>
        ///     Maps paths to idfx files to the session id information
        ///     of that idfx file.
        /// </summary>
        private readonly Dictionary<string, SessionIdentification> _sessionIDs =
            new Dictionary<string, SessionIdentification>();

        public IList<string> SourcePaths
        {
            get => _thesePaths.AsReadOnly();
            set
            {
                _thesePaths.Clear();
                _theOriginalPaths.Clear();
                DestinationPath = null;

                if (value == null || !value.Any()) return;
                _thesePaths.AddRange(value.Where(File.Exists));

                // Search for an original document in the directory of each source file.
                foreach (string path in _thesePaths)
                {
                    string[] docs = Directory.GetFiles(Path.GetDirectoryName(path) ?? path, "*_original.docx", SearchOption.TopDirectoryOnly);
                    _theOriginalPaths.Add((docs.Any()) ? docs[0] : "");
                }

                // Suggest a destination path if all source files are in the same directory.
                if (!_thesePaths.Any()) return;
                string sourceDir = Path.GetDirectoryName(_thesePaths.First()) ?? _thesePaths.First();
                DestinationPath = (_thesePaths.All(src => string.Equals(Path.GetDirectoryName(src), sourceDir)))
                    ? Path.Combine(sourceDir, Properties.Settings.Default.Analysis_DestinationSubdirectory)
                    : null;
            }
        }

        public IList<string> OriginalDocumentPaths
        {
            get => _theOriginalPaths;
            set
            {
                _theOriginalPaths.Clear();
                _theOriginalPaths.AddRange(value.Where(File.Exists));
            }
        }

        public string DestinationPath
        {
            get => _myDestinationPath;
            set
            {
                _myDestinationPath = value ?? Properties.Settings.Default.Workspace;

                if (Path.HasExtension(_myDestinationPath))
                    _myDestinationPath = Path.GetDirectoryName(_myDestinationPath);
            }
        }

        /// <summary>
        ///     If we are in reporting mode this will contain our SelectedTemplate.
        /// </summary>
        public ReportTemplate ActiveTemplate;

        /// <summary>
        ///     Output formats for the reporting.
        /// </summary>
        public IEnumerable<string> ReportOutputFormats;

        /// <summary>
        /// A copyTask analysis needs a proper idfx.
        /// </summary>
        private bool _isCopytaskAnalyzer;

        /// <summary>
        /// Reads events and session identification from source file.
        /// All source files must be re-read for every analysis,
        /// since the list of events may be modified by event manipulators.
        /// Maintaining a deep copy of the event list does not suffice,
        /// since this could double memory usage.
        ///
        /// </summary>
        public List<Event> ReadEvents(string sourcePath, out SessionIdentification sessionId)
        {
            try
            {
                var eventLogReader = EventLogFactory.CreateFileEventLogReader(sourcePath, LogFormat.XML);
                sessionId = eventLogReader.ReadSessionIdentification();
                return eventLogReader.ReadEvents();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR,
                    "Unable to read events from file. Are you sure you provided a valid logfile?");
                sessionId = null;
                return null;
            }
        }

        /// <summary>
        /// Preprocesses the events to do some automatic conversions for older logging files.
        /// This differs from the preprocessing steps specified by the user.
        /// </summary>
        public static bool PreprocessEvents(SessionIdentification sessionId, List<Event> events)
        {
            int versionNumber = sessionId.GetVersionNumber();
            bool invalidBackspaceSequence = Analysis.PreprocessEvents(events, versionNumber);
            RevisionAnalysis.PreprocessEvents(events, versionNumber);
            return invalidBackspaceSequence;
        }

        /// <summary>
        /// Analyzes all sources files, given an analyzer and a list of preprocessors.
        ///
        /// NOTE: THE SERVERANALYSISLAUNCHER COPIES PARTS OF THIS LOGIC (in the Analyze method)!!
        /// CHANGES HERE MUST BE MADE TO BOTH THE SERVERANALYSISLAUNCHER AS HERE, OR IT SHOULD BE
        /// REFACTORED TO NOT BE SUCH A BLOODY MESS!!
        /// 
        /// </summary>
        public void Analyze(AbstractAnalyzer analyzer, List<Preprocessor> preprocessors,
            bool preprocessOnly = false)
        {
            OriginalDocumentPaths.Add("");
            _isCopytaskAnalyzer = analyzer.GetType().Name.Equals("CopytaskAnalyzer");

            // Iterate over all source files and their respective original documents.
            foreach (var it in SourcePaths.Zip(OriginalDocumentPaths,
                (s, o) => new { SourcePath = s, OriginalDocumentPath = o }))
            {
                try
                {
                    var events = ReadEvents(it.SourcePath, out var sessionId);
                    if (sessionId == null || events == null) continue;
                    // Performing a copyTask analysis with the wrong idfx file generates a breaking error.
                    if (_isCopytaskAnalyzer && !sessionId.MetaInfo.ContainsKey("__copytask"))
                    {
                        MessageBox.Show("The file '" + Path.GetFileName(it.SourcePath) + "' is not a copyTask idfx." +
                                        "\nThe Copytask Analysis will be skipped.",
                            "InputLog - Copytask Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }
                    _currentSourcePath = it.SourcePath;
                    PreprocessEvents(sessionId, events);

                    analyzer.StartSession(sessionId, events, IsReportGeneration);
                    analyzer.SetPaths(it.SourcePath, DestinationPath, it.OriginalDocumentPath);
                    FileInfo idfxFileInfo = new FileInfo(it.SourcePath);
                    _sessionIDs[idfxFileInfo.Name] = sessionId;

                    if (preprocessOnly) continue;

                    analyzer.DoAnalysis();
                    DifferenceForReportGeneration(it.SourcePath, analyzer);
                }
                catch (Exception exc)
                {
                    // TODO Make the messageLogger.CatchException just store exceptions in memory until
                    // a call is made to display the exceptions that have been caught. That way we can prevent
                    // Inputlog from spamming the user with exception dialogs if the user is running an analysis
                    // that just can not be executed correctly, on a large batch of files!
                    if (exc.Message.Contains("Empty path"))
                    {
                        MessageLogger.CatchException(this, exc, Severity.ERROR, "Path not found.");
                    }
                    else if (exc.Message.Contains("The process"))
                    {
                        MessageLogger.CatchException(this, exc, Severity.ERROR, "The file is being used by another process.");
                    }
                    else
                    {
                        string message = "The analysis could not be executed due to an error.";
                        if (!string.IsNullOrEmpty(RevisionAnalysis.BadEventIds)) 
                        {
                            message += "Bad events detected with ids [" + RevisionAnalysis.BadEventIds + "]";
                        }
                        MessageLogger.CatchException(this, 
                            new Exception(message, exc),
                            Severity.ERROR
                        );
                    }
                }
            }
        }

        /// <summary>
        ///     Write away the reports - this is after all the reports have
        ///     been constructed in-memory (after the analyses)
        /// </summary>
        public void WriteReports()
        {
            foreach (KeyValuePair<string, Report> reportPair in Reports)
            {
                string sourceIdfx = reportPair.Key;
                Report report = reportPair.Value;

                foreach (string format in ReportOutputFormats)
                {
                    try
                    {
                        Formatter formatter = FormatterFactory.CreateFormatter(format);
                        formatter.SetSessionID(_sessionIDs[sourceIdfx]);
                        report.Format(formatter);
                        string outputPath = DetermineReportSavePath(
                            sourceIdfx,
                            formatter.GetAffix(),
                            formatter.GetExtension()
                        );
                        formatter.WriteToFile(outputPath);
                    }
                    catch (Exception e)
                    {
                        MessageLogger.CatchException(this, e, Severity.ERROR, 
                            "Could not write report (format: \"" + format + "\" idfx: \"" + sourceIdfx + "\")"
                        );
                    }
                }
            }
            Process.Start(DestinationPath);
        }

        private string DetermineReportSavePath(string sourceIdfx, string affix, string extension)
        {
            FileInfo fInfo = new FileInfo(sourceIdfx);
            string filenameWithoutExtension = fInfo.Name + (affix ?? string.Empty);
            string nameWithExtension = filenameWithoutExtension + '.' + extension;

            string outputPath = Path.Combine(DestinationPath, nameWithExtension);
            outputPath = PathSanitizer.Uniquify(outputPath);
            return outputPath;
        }

        /// <summary>
        ///     Handling of what happens after an analysis has been completed for a given
        ///     idfx file. In case of normal handling the results of the analyis are written
        ///     using the analyzer. 
        ///     When we are in 'reportHandling' mode however, the reportingTargets for the summary
        ///     are executed and the data retrieved by the ReportingMethods are added to the
        ///     report.
        /// </summary>
        /// <param name="idfxPath">Path of the analyzed idfx</param>
        /// <param name="analyzer">The analyzer used for the analysis.</param>
        private void DifferenceForReportGeneration(string idfxPath, AbstractAnalyzer analyzer)
        {
            if (IsReportGeneration)
            {
                FileInfo fInfo = new FileInfo(idfxPath);
                string sourceFile = fInfo.Name;

                // Get the report associated with this sourceFile
                // If it exists already.
                Report currentReport;
                if (Reports.ContainsKey(sourceFile)) 
                {
                    currentReport = Reports[sourceFile];
                }
                else 
                {
                    currentReport = new Report(ActiveTemplate);
                    Reports.Add(sourceFile, currentReport);
                }

                AbstractAnalysisSummary result = (AbstractAnalysisSummary)analyzer.CurrentSummary;
                AddTargetsToReports(result, analyzer.Name, currentReport);
            }
            else
            {
                analyzer.WriteAnalysis();
                string outputPath = Path.Combine(Path.GetDirectoryName(idfxPath), "output");
                XmlToHtmlTranslator.Transform(outputPath);
            }

        }

        /// <summary>
        ///     Get all the selected targets for the given Analysis/AnalysisSummary and
        ///     get their data from the Summary to add to the report.
        /// </summary>
        /// <param name="result">The analysis summary</param>
        /// <param name="analyzerName">The name of the analyzer, used to find the analysisName</param>
        /// <param name="report">The report to which we add the targets.</param>
        private void AddTargetsToReports(AbstractAnalysisSummary result, string analyzerName, Report report)
        {
            string analysisName = GetAnalysisNameFromAnalyzerName(analyzerName);
            Dictionary<string, ReportMethod> boundTargets = result.GetBoundReportTargets();

            // Get all targets for this analysis.
            if (!report.TargetsPerAnalysis.ContainsKey(analysisName))
            {
                return;
            }

            foreach (string targetId in report.TargetsPerAnalysis[analysisName])
            {
                Report.ReportResource resource = Report.GetResource(targetId);
                ReportMethod boundTarget = boundTargets[resource.Method];
                ReportValue element = boundTarget.Invoke();

                report.AddValue(targetId, element);
            }
        }

        private string GetAnalysisNameFromAnalyzerName(string analyzerName)
        {
            KeyValuePair<string, string> analysisToAnalyzer =
                AnalysisMap.ANALYSIS_NAME_TO_ANALYZER_NAME.Single(pair => pair.Value == analyzerName);
            return analysisToAnalyzer.Key;

        }

        public static string GetCurrentSourcePath()
        {
            return _currentSourcePath;
        }

        public AnalysisConfigurationXMLDeserializer ImportConfiguration(string fileName)
        {
            var deserializer = new AnalysisConfigurationXMLDeserializer(fileName);
            SourcePaths = deserializer.DeserializeSources();
            DestinationPath = deserializer.DeserializeDestination();
            return deserializer;
        }

        public bool IsReportGeneration = false;
        public Dictionary<string, Report> Reports;
    }
}