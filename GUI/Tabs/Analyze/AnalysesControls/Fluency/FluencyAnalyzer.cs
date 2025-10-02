using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GUI.Util;
using GUI.Visualization;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.IO;
using InputLog.Core.Util;
using LinearAnalysisType = InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE;
using TaskMaximumMode = InputLog.Core.Analyses.Fluency.FluencyAnalysis.TaskMaximumMode;

namespace GUI.Tabs.Analyze.AnalysesControls.Fluency
{
    public class FluencyAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Fluency";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "FLUA";

        /// <summary>
        /// Extra parameter used by the analysis. When the intervaltype is:
        ///  - FixedNumberOfIntervals, this parameter represents the number of parameters
        ///  - FixedIntervalLength, this parameter represents the interval length
        /// -Copy from Linear-
        /// </summary>
        private readonly ulong IntervalParam;

        /// <summary>
        ///  Directory to the personal fluency optimum matrix.
        /// </summary>
        private string MatrixDir = "";

        /// <summary>
        /// The type of linear analysis that will be performed.
        /// -Copy from Linear-
        /// </summary>
        private readonly LinearAnalysisType LinearAnalysisType;

        /// <summary>
        /// The Fluency Graph to be shown when user clicks the "Show Graph" link
        /// </summary>
        public static FluencyGraph FluencyGraph;

        private static FluencyGraph _tmpFluencyGraph;

        /// <summary>
        /// Summary of the previous fluency analysis, needed for showing more than 1 in a graph
        /// </summary>
        private static FluencyAnalysisSummary _prevFluencySummary;

        /// <summary>
        /// Source file of the previous fluency analysis, needed for showing more than 1 in a graph
        /// </summary>
        private static string _prevSrcFile;

        private static int _prevTrendLineDegree;

        private readonly int DefaultPersonalMaximum;

        private readonly TaskMaximumMode TaskMaxMode;

        /// <summary>
        /// Parameters used in a multiple file analysis
        /// </summary>
        private readonly bool IsMultiFileAnalysis;

        private readonly bool UsePersonalDefault;
        private readonly bool MakeMultigraph;
        private bool DrawMultiGraph;

        /// <summary>
        /// Indicates whether the analysis was completed successfully
        /// </summary>
        public bool Success;

        private readonly bool OnlyCharProduction;

        /// <summary>
        /// The value of the pause threshold.
        /// </summary>
        private readonly ulong PauseThreshold;
        /// <summary>
        /// The number of files to process.
        /// </summary>
        public static int FileCount;

        //private FluencyStdDevGraph StdDevGraph = new FluencyStdDevGraph();

        private Dictionary<string, string> PersonalMaxima;

        private int TrendLineDegree;

        private string GraphExtension;

        /// <summary>
        /// Analyzes the writing fluency of one or more log files.
        /// </summary>
        /// <param name="intervalParam"></param>
        /// <param name="linearAnalysisType"></param>
        /// <param name="pauseThreshold"></param>
        /// <param name="taskMaximumMode"></param>
        /// <param name="onlyCharProduction"></param>
        /// <param name="defaultPersonalMaximum"></param>
        /// <param name="isMultiFileAnalysis">True if more than 1 file is selected</param>
        /// <param name="usePersMax">True if each personal maximum should be saved</param>
        /// <param name="multigraph">True if a multiple fluency graph should be generated</param>
        /// <param name="fileCount">The number of files to process</param>
        public FluencyAnalyzer(ulong intervalParam, LinearAnalysisType linearAnalysisType, ulong pauseThreshold,
            TaskMaximumMode taskMaximumMode, bool onlyCharProduction, int defaultPersonalMaximum,
            bool isMultiFileAnalysis, bool usePersMax, bool multigraph, int fileCount)
        {
            IntervalParam = intervalParam;
            LinearAnalysisType = linearAnalysisType;
            PauseThreshold = pauseThreshold;
            TaskMaxMode = taskMaximumMode;
            OnlyCharProduction = onlyCharProduction;
            DefaultPersonalMaximum = defaultPersonalMaximum;
            IsMultiFileAnalysis = isMultiFileAnalysis;
            UsePersonalDefault = !usePersMax; // When not using personal maximum, use default.
            MakeMultigraph = multigraph;
            FileCount = fileCount;
            ReadPersonalMaxima();
        }

        private void ReadPersonalMaxima()
        {
            PersonalMaxima = new Dictionary<string, string>();
            MatrixDir = GetPersonalMaximaMatrixDir();
            string s = string.Empty;
            if (!Directory.Exists(Path.GetDirectoryName(MatrixDir)))          
            {
                Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.Workspace, "FluencySettings"));
                File.Create(MatrixDir);
                s = "";
            }
            else if(!File.Exists(MatrixDir))
            {
                 File.Create(MatrixDir);
            }
            else s = File.ReadAllText(MatrixDir);
            PersonalMaxima.FromSerialString(s);
        }

        public static string GetPersonalMaximaMatrixDir()
        {
            return Path.Combine(Properties.Settings.Default.Workspace, "FluencySettings/PersonalMaxima.txt");
        }

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct analysis 
        /// for performing the actual analysis.
        /// 
        /// </summary>
        protected override Analysis GetAnalysis()
        {
            #region copy from linear

            // Extra parameters, the docpath is required for the revision grouping of the linear analysis.
            var extraParameters = new Dictionary<string, object> {{"docPath", OrgDocPath}};

            // List of keys that act as control keys (these should be represented with '+' when 
            // they are present in the keyboard state).
            // e.g if LSHIFT is part of ControlKeys, then pressing LSHIFT and 'a' at the same time 
            // will result in LSHIFT + A in the representation. If LSHIFT is not part of ControlKeys, then pressing LSHIFT
            //  and  'a' at the same time will result in 2 different keystrokes.
            var controlKeys = SettingsManipulation.DeserializeGroupedKeyList(Properties.Settings.Default.GroupedKeysList);

            #endregion

            // Read absolute maximum from registry
            const int defAbsMax = FluencyAnalysis.DEFAULT_ABSOLUTE_MAXIMUM;
            var absoluteMaximum = Int32.Parse(RegistryTools.GetSetting(RegistryTools.DEFAULT_APP_NAME,
                "AbsoluteFluencyMaximum", defAbsMax).ToString());

            var author = SessionId.GetParticipant().ToLower();

            int persMaximum;
            // With the multiple file analysis the user can set the default maximum as the personal value for every author,
            if (IsMultiFileAnalysis && UsePersonalDefault)
            {
                persMaximum = DefaultPersonalMaximum;
            }
            // ...in all other cases, multiple or not, the saved personal maximum is used (if it exists). 
            else
            {
                persMaximum = PersonalMaxima.ContainsKey(author)
                ? Int32.Parse(PersonalMaxima[author])
                : DefaultPersonalMaximum;
            }

            return new FluencyAnalysis(Events, SessionId, LinearAnalysisType, absoluteMaximum,
                persMaximum, PauseThreshold, IntervalParam, OnlyCharProduction,
                TaskMaxMode, controlKeys, extraParameters);
        }

        /// <summary>
        /// Executes a fluency analysis and saves the graph image
        /// </summary>
        protected override void AfterAnalysis(Analysis analysis, IAnalysisSummary summary)
        {
            var fluencyAnalysis = (FluencyAnalysis) analysis;
            var fluencySummary = (FluencyAnalysisSummary) summary;
            _tmpFluencyGraph = new FluencyGraph(TrendLineDegree);
            var author = SessionId.GetParticipant().ToLower();

            if (fluencySummary.Periods.Count > 0)
            {
                var taskMaximum = Convert.ToInt32(fluencyAnalysis.GetTaskMaximum());

                // Storing the PersonalMaxima was allowed in the Options Analyses Settings of Inputlog.
                if (Properties.Settings.Default.StorePersonalMaxima)
                {
                    // If this is a multiple file analysis,
                    // and we have used an existing personal maximum,
                    if (IsMultiFileAnalysis && !UsePersonalDefault)
                    {
                        // ...then leave the maximum as it is.
                        // If there is none, save the current task maximum.
                        if (!PersonalMaxima.ContainsKey(author))
                        {
                            PersonalMaxima.Add(author, taskMaximum.ToString());
                        }
                    }
                    // If this is a single file analysis,
                    if (!IsMultiFileAnalysis)
                    {
                        // ...and if a personal maximum exists for this author,
                        if (PersonalMaxima.ContainsKey(author))
                        {
                            // ...and if the current task maximum exceeds his saved personal maximum, 
                            // ask the user if the new task maximum should overwrite the personal maximum.
                            if (Int32.Parse(PersonalMaxima[author]) < taskMaximum)
                            {
                                var msg = "Task maximum exceeds personal maximum for "
                                          + author + ". Do you wish to store new personal maximum?";
                                var dialogResult = MessageBox.Show(msg, "New personal maximum?",
                                    MessageBoxButtons.YesNo);
                                if (dialogResult == DialogResult.Yes)
                                {
                                    PersonalMaxima[author] = taskMaximum.ToString();
                                }
                            }
                        }
                        // If no personal maximum exists, one is created.
                        else
                        {
                            PersonalMaxima.Add(author, taskMaximum.ToString());
                        }
                    }
                    // Any new or changed personal maximum is written to the PersonalMaxima collection.
                    using (var newTask = new StreamWriter(MatrixDir, false))
                    {
                        newTask.WriteLine(PersonalMaxima.ToSerialString());
                    }
                }

                // Create graph with only this analysis and save it
                _tmpFluencyGraph.Process(fluencySummary);

                // Save the _tmpFluencyGraph as an image to a stream.
                _tmpFluencyGraph.Draw();
                _tmpFluencyGraph.SaveToStream(
                    fluencySummary.GraphImage,
                    System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png
                );

                // Different intervals for different idfxs -> no possibility of plotting graphs together
                if (MakeMultigraph && (LinearAnalysisType == LinearAnalysisType.REVISION_INTERVALS
                                       || LinearAnalysisType == LinearAnalysisType.FOCUS_INTERVALS))
                {
                    if (FluencyGraph != null)
                    {
                        FluencyGraph.Dispose();
                    }
                    FluencyGraph = _tmpFluencyGraph;
                }
                else // Same intervals for different idfxs, plot them together if more than one arrives
                {
                    if (FluencyGraph == null)
                    {
                        FluencyGraph = _tmpFluencyGraph;
                        _prevFluencySummary = fluencySummary;
                        _prevSrcFile = SourcePath;
                        _prevTrendLineDegree = TrendLineDegree;
                    }
                    else if (MakeMultigraph)
                    {
                        if (!(FluencyGraph is MultiFluencyGraph))
                        {
                            FluencyGraph.Dispose();
                            // Plot previous idfx as well
                            FluencyGraph = new MultiFluencyGraph(_prevTrendLineDegree);
                            ((MultiFluencyGraph) FluencyGraph).Process(_prevSrcFile, _prevFluencySummary);
                            FluencyGraph.ChangeTrendLineDegree(TrendLineDegree);
                        }
                        ((MultiFluencyGraph) FluencyGraph).Process(SourcePath, fluencySummary);
                        if (((MultiFluencyGraph) FluencyGraph).IsReady()) DrawMultiGraph = true;
                    }
                }
                Success = true;

            }
            else
            {
                const ulong minSize = FluencyAnalysis.PERIOD_MIN_SIZE;
                Success = false;
                MessageBox.Show("Analysis failed. Intervals were too small to process. Minimum interval size: "
                                + DateTimeUtils.MsecToClockString(minSize, false, false),
                    "Analysis failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool BeforeWrite(IAnalysisWriter writer)
        {
            return Success;
        }

        protected override void AfterWrite(IAnalysisWriter writer)
        {
            string fileName = OutputFilePath.Substring(0, OutputFilePath.Length - 4) + "." + GraphExtension;

            // Replace the author name in the path when the file is a multigraph.
            if (DrawMultiGraph)
            {
                var firstIdx = fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                var lastIdx = fileName.IndexOf('_', firstIdx);
                var author = fileName.Substring(firstIdx, (lastIdx - firstIdx));
                fileName = fileName.Replace(author, "MULTIGRAPH");
                FluencyGraph.Draw();
                FluencyGraph.Save(fileName, ChartImageUtils.StrToImageFormat(GraphExtension));
                return;
            }

            _tmpFluencyGraph.Draw();
            _tmpFluencyGraph.Save(fileName, ChartImageUtils.StrToImageFormat(GraphExtension));
        }

        public void ChangeTrendLineDegree(int p)
        {
            if (FluencyGraph != null) FluencyGraph.ChangeTrendLineDegree(p);
            TrendLineDegree = p;
        }

        public void ChangeGraphImageExtension(string ext)
        {
            GraphExtension = ext;
        }

        /// <summary>
        /// Given the destination folder and a source (log) file, this method returns a
        /// full path of the file where the result of the analysis should be stored. This pathname may contain 
        /// place holders for sessionidentification information using the ${key} notation.
        /// -copy from linear-
        /// </summary>
        /// <param name="sessionId">The session identification.</param>
        /// <param name="srcFile">The source (log) file</param>
        /// <param name="destDir">The destination directory path.</param>
        /// <returns>String with full file path of the file where the result of the analysis should be stored</returns>
        protected override string ExtendFileName(SessionIdentification sessionId, string srcFile, string destDir)
        {
            var name = base.ExtendFileName(sessionId, srcFile, destDir);

            var buf =
                new StringBuilder(Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name)));
            switch (LinearAnalysisType)
            {
                case LinearAnalysisType.FIXED_NUMBER_OF_INTERVALS:
                    buf.Append("_FN" + IntervalParam);
                    break;

                case LinearAnalysisType.FIXED_LENGTH_INTERVALS:
                    buf.Append("_FL" + (IntervalParam/1000));
                    break;
                case LinearAnalysisType.FOCUS_INTERVALS:
                    buf.Append("_FI");
                    break;

                case LinearAnalysisType.REVISION_INTERVALS:
                    buf.Append("_RI");
                    break;
            }
            switch (TaskMaxMode)
            {
                case TaskMaximumMode.BASIC:
                    break;
                case TaskMaximumMode.INTERVAL_DEPENDENT:
                    buf.Append("_ID");
                    break;
            }
            buf.Append(Path.GetExtension(name));

            return buf.ToString();
        }


        /// <summary>
        /// Creates the dictionary of parameters that should be passed to the analysiswriter. 
        /// -copy from linear-
        /// </summary>
        /// <returns>A dictionary containing the various parameters.</returns>
        protected override IDictionary<string, IDictionary<string, object>> GetParameters()
        {
            var dict = new Dictionary<string, IDictionary<string, object>>();
            var parameters = new Dictionary<string, object>();
            dict["Parameters"] = parameters;
            switch (LinearAnalysisType)
            {
                case InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE.FIXED_LENGTH_INTERVALS:
                    parameters[FluencyAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Length Intervals";
                    parameters["Length of Interval (sec)"] = IntervalParam/1000;
                    break;
                case InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE.FIXED_NUMBER_OF_INTERVALS:
                    parameters[FluencyAnalysisXMLWriter.TYPE_PARAMETER] = "Fixed Number of Intervals";
                    parameters["Number of Intervals"] = IntervalParam;
                    break;
                case InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE.FOCUS_INTERVALS:
                    parameters[FluencyAnalysisXMLWriter.TYPE_PARAMETER] = "Focus-Based Intervals";
                    break;
                case InputLog.Core.Analyses.Linear.LinearAnalysis.TYPE.REVISION_INTERVALS:
                    parameters[FluencyAnalysisXMLWriter.TYPE_PARAMETER] = "Revision-Based Intervals";
                    break;
            }
            switch (TaskMaxMode)
            {
                case TaskMaximumMode.BASIC:
                    parameters[FluencyAnalysisXMLWriter.TASK_MAXIMUM_MODE_PARAMETER] = "Basic";
                    break;
                case TaskMaximumMode.INTERVAL_DEPENDENT:
                    parameters[FluencyAnalysisXMLWriter.TASK_MAXIMUM_MODE_PARAMETER] = "Interval Dependent";
                    break;
            }
            parameters["Only Character production"] = OnlyCharProduction ? "Yes" : "No";
            return dict;
        }

        /// <summary>
        /// Resets the graph, happens automatically when intervals are changed
        /// </summary>
        public static void ResetGraph()
        {
            if (FluencyGraph != null)
            {
                FluencyGraph.Dispose();
                FluencyGraph = null;
            }
            if (_tmpFluencyGraph == null) return;
            _tmpFluencyGraph.Dispose();
            _tmpFluencyGraph = null;
        }
    }
}