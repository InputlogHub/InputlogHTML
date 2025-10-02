using System.Collections.Generic;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.Util;
using System;
using System.Globalization;

namespace InputLog.Core.Analyses.Fluency
{
    /// <summary>
    /// Analysis XMLWriter for the Linear Analysis.
    /// </summary>
    public class FluencyAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {

        #region Constants

        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "fluency_analysis.xsl";
        private const string PERIOD_TAG = "Period";
        private const string PERIOD_EVENT_TAG = "PeriodEvent";

        // Period/Event attributes
        private const string PERIOD_ID_ATTRIBUTE = "period_id";
        private const string PERIOD_TIME_ATTRIBUTE = "periodTime";
        private const string LINK_ATTRIBUTE = "link";

        public const string TASK_MAXIMUM_MODE_PARAMETER = "Task Maximum Mode";
        public const string TYPE_PARAMETER = "Fluency Analysis Type";
        public const string RESTRICTED_LOGGING = "Restricted Logging";

        public const string SPACE = "SPACE";
        public const string DELETE = "DELETE";
        public const string RIGHT = "RIGHT";
        public const string BACK = "BACK";
        public const string LEFT = "LEFT";

        public const string ABSOLUTE_MAXIMUM_TAG = "AbsMaximum";
        public const string PERSONAL_MAXIMUM_TAG = "PersonalMaximum";
        public const string TASK_MAXIMUM_TAG = "TaskLMaximum";
        public const string TASK_MAXIMUM_PERIOD = "TaskMaximumPeriod";
        public const string TASK_MAXIMUM_WINDOW = "TaskMaximumWindow";
        public const string TASK_MAXIMUM_LOCATION = "TaskMaximumLocation";

        private const string AVERAGE_SPM_TITLE = "Average Strokes per Minute (10 intervals)";
        private const string STDDEV_SPM_TITLE = "Standard Deviation (Intervals)";
        private const string STDDEV_OVERALL_TITLE = "Standard Deviation (Overall)";
        private const string INTERVAL_LENGTH = "Interval Length";
        private const string TOTAL_LENGTH = "Total Length";
        private const string TOTAL_STROKES = "Total Strokes";

        public const string NR_STROKES_TAG = "NrStrokes";
        public const string STROKES_PER_MIN_TAG = "StrokesPerMin";
        public const string LENGTH_TAG = "Length";
        public const string ABSOLUTE_PERCENTAGE_TAG = "AbsolutePercentage";
        public const string TASK_PERCENTAGE_TAG = "TaskPercentage";
        public const string PERSONAL_PERCENTAGE_TAG = "PersonalPercentage";

        private const string COV_PERCT_MAX = "Coefficient of Variation of % Abs.Maximum";
        private const string MEAN_PERCT_MAX = "Mean of % Abs. Maximum";
        private const string MEDIAN_PERCTMAX = "Median of % Abs. Maximum";
        private const string STDEV_PERCT_MAX = "Standard Deviation of % Abs. Maximum";
        private const string TASKMAX_TASK_COEFVAR = "Coefficient of Variation of % Task Maximum";
        private const string TASKMAX_TASK_MEANINTERVAL = "Mean of % Task Maximum";
        private const string TASKMAX_TASK_MEDIAN = "Median of % Task Maximum";
        private const string TASKMAX_TASK_MAXSTDEV = "Standard Deviation of % Task Maximum";
        private const string TASKMAX_STROKES_COEFVAR = "Coefficient of Variation of Strokes/Interval";
        private const string TASKMAX_STROKES_MEANINTERVAL = "Mean of Strokes/Interval ";
        private const string TASKMAX_STROKES_MEDIAN = "Median of Strokes/Interval";
        private const string TASKMAX_STROKES_MAXSTDEV = "Standard Deviation of Strokes/Interval";
        private const string TASKMAX_STROKES_MIN_COEFVAR = "Coefficient of Variation of Strokes/Min.";
        private const string TASKMAX_STROKES_MIN_INTERVAL = "Mean of Strokes/Min.";
        private const string TASKMAX_STROKES_MIN_MEDIAN = "Median of Strokes/Min.";
        private const string TASKMAX_STROKES_MIN_MAXSTDEV = "Standard Deviation of Strokes/Min.";
        private const string PERSMAX_PERS_COEFVAR = "Coefficient of Variation of % Pers.Maximum";
        private const string PERSMAX_PERS_MEANINTERVAL = "Mean of % Pers.Maximum";
        private const string PERSMAX_PERS_MEDIAN = "Median of % Pers.Maximum";
        private const string PERSMAX_PERS_MAXSTDEV = "Standard Deviation of % Pers.Maximum";

        private FluencyAnalysisSummary Summary;

        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        /// <summary>
        /// Constructs a LinearAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public FluencyAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
        }

        /// <summary>
        /// Writes out the analysis document using a given FluencyAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which 
        /// the Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is FluencyAnalysisSummary)) throw new AnalysisWriterException("Given summary is not of type" +
                " FluencyAnalysisSummary");
            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 2;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary((FluencyAnalysisSummary)summary);
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_CSS, COMMON_XSL }, new[] { INPUTLOG_LOGO });
        }

        /// <summary>
        /// Writes the summary to the stream.
        /// Loops over all the periods in the stream, writes out the start and periodtime for each of them.
        /// Then loops over all the events within a period and writes out the events (based on there type, 
        /// they are represented differently).
        /// </summary>
        /// <param name="summary">The analysis summary</param>
        private void WriteSummary(FluencyAnalysisSummary summary)
        {
            Summary = summary;
            var absMaxParams = new Dictionary<string, string> 
            { 
                { "Maximum", summary.AbsMaximum.ToString(CultureInfo.InvariantCulture) },                               
                { COV_PERCT_MAX, summary.AbsCoefVar.ToString("F", Nfi) },
                { MEAN_PERCT_MAX, summary.AbsMeanInterval.ToString("F", Nfi) },
                { MEDIAN_PERCTMAX, summary.AbsMedian.ToString("F", Nfi)},
                { STDEV_PERCT_MAX, summary.AbsMaxStdev.ToString("F", Nfi)}
            };

            WriteModule("Statistics", delegate
                {
                    WriteModuleBlock("Statistics", delegate
                        {
                            WriteModuleElement(AVERAGE_SPM_TITLE, summary.AverageSPM.ToString("0"));
                            WriteModuleElement(STDDEV_SPM_TITLE, summary.StdDev.ToString("F", Nfi));
                            WriteModuleElement(STDDEV_OVERALL_TITLE, summary.SmallChunkStdDev.ToString("F", Nfi));
                            WriteModuleElement(TOTAL_LENGTH, 
                                DateTimeUtils.MsecToClockString(summary.TotalLength, false, false));
                            WriteModuleElement(TOTAL_STROKES, summary.TotalStrokes.ToString());
                            if (summary.LinearType == LinearAnalysis.TYPE.FIXED_LENGTH_INTERVALS 
                                || summary.LinearType == LinearAnalysis.TYPE.FIXED_NUMBER_OF_INTERVALS)
                            {
                                WriteModuleElement(INTERVAL_LENGTH, 
                                    DateTimeUtils.MsecToClockString(Summary.Periods[0].Item2.Length, false, false));
                            }
                        });
                });

            //var absMaxParams = new Dictionary<string, string> { { "Maximum", summary.AbsMaximum.ToString() } };
            WriteModule("Absolute Maximum", () => 
            {
                WriteParams("Maximum", absMaxParams); 
                WriteFluencyBlocks(ps => ps.AbsolutePercentage); 
            });

            string a = DateTimeUtils.MsecToClockString(summary.TaskMaximumPeriod.PeriodStart, false, false);
            string b = DateTimeUtils.MsecToClockString(summary.TaskMaximumPeriod.PeriodEnd, false, false);
            var taskMaxParams = new Dictionary<string, string>
            {
                    { "Maximum", summary.TaskLMaximum.ToString() },
                    { "PeriodSize", DateTimeUtils.MsecToClockString(summary.TaskMaximumPeriod.WindowLength, false, false) },
                    { "WindowSize", summary.TaskMaximumPeriod.Window.ToString() },
                    { "MaximumLocation", a + "  " + b },

                    { TASKMAX_TASK_COEFVAR, summary.TaskCoefVar.ToString("F", Nfi) },
                    { TASKMAX_TASK_MEANINTERVAL, summary.TaskMeanInterval.ToString("F", Nfi) },
                    { TASKMAX_TASK_MEDIAN, summary.TaskMedian.ToString("F", Nfi)},
                    { TASKMAX_TASK_MAXSTDEV, summary.TaskMaxStdev.ToString("F", Nfi)},

                    { TASKMAX_STROKES_COEFVAR, summary.StrokesCoefVar.ToString("F", Nfi) },
                    { TASKMAX_STROKES_MEANINTERVAL, summary.StrokesMeanInterval.ToString("F", Nfi) },
                    { TASKMAX_STROKES_MEDIAN, summary.StrokesMedian.ToString("F", Nfi)},
                    { TASKMAX_STROKES_MAXSTDEV, summary.StrokesMaxStdev.ToString("F", Nfi)},

                    { TASKMAX_STROKES_MIN_COEFVAR, summary.StrokesMinCoefVar.ToString("F", Nfi) },
                    { TASKMAX_STROKES_MIN_INTERVAL, summary.StrokesMinInterval.ToString("F", Nfi) },
                    { TASKMAX_STROKES_MIN_MEDIAN, summary.StrokesMinMedian.ToString("F", Nfi)},
                    { TASKMAX_STROKES_MIN_MAXSTDEV, summary.StrokesMinMaxStdev.ToString("F", Nfi)}
                };

            WriteModule("Task Maximum", () => 
            { 
                WriteParams("Maximum", taskMaxParams); 
                WriteFluencyBlocks(ps => ps.TaskPercentage); 
            });

            var persMaxParams = new Dictionary<string, string>
            {
                { "Maximum", summary.PersonalMaximum.ToString() },
                { PERSMAX_PERS_COEFVAR, summary.PersCoefVar.ToString("F", Nfi) },
                { PERSMAX_PERS_MEANINTERVAL, summary.PersMeanInterval.ToString("F", Nfi) },
                { PERSMAX_PERS_MEDIAN, summary.PersMedian.ToString("F", Nfi)},
                { PERSMAX_PERS_MAXSTDEV , summary.PersMaxStdev.ToString("F", Nfi)},
            
            };
            WriteModule("Personal Maximum", () => 
            {
                WriteParams("Maximum", persMaxParams); 
                WriteFluencyBlocks(ps => ps.PersonalPercentage); 
            });
        }

        private void WriteParams(string name, Dictionary<string, string> absMaxParams)
        {
            WriteModuleBlock(name, delegate
            {
                foreach (string k in absMaxParams.Keys)
                {
                    WriteModuleElement(k, absMaxParams[k]);
                }
            });
        }

        private void WriteFluencyBlocks(Func<FluencyAnalysis.PeriodStats, double> percentageFunc)
        {
            WriteModuleBlock("ID", delegate
            {
                int c = 1;
                foreach (var p in Summary.Periods)
                {
                    WriteModuleElement("Interval_" + c, p.Item1.ID);
                    c++;
                }
            });

            WriteModuleBlock("Total Strokes", delegate
                {
                    int c = 1;
                    foreach (var p in Summary.Periods)
                    {
                        WriteModuleElement("Interval_" + c, p.Item2.NrStrokes.ToString());
                        c++;
                    }
                });

            WriteModuleBlock("Strokes per Minute", delegate
            {
                int c = 1;
                foreach (var p in Summary.Periods)
                {
                    WriteModuleElement("Interval_" + c, p.Item2.StrokesPerMin.ToString());
                    c++;
                }
            });

            WriteModuleBlock("Percentage of Maximum", delegate
            {
                int c = 1;
                foreach (var p in Summary.Periods)
                {
                    WriteModuleElement("Interval_" + c, (percentageFunc(p.Item2) * 100).ToString("F", Nfi) + " %");
                    c++;
                }
            });

            WriteModuleBlock("Period Length", delegate
            {
                int c = 1;
                foreach (var p in Summary.Periods)
                {
                    WriteModuleElement("Interval_" + c, DateTimeUtils.MsecToClockString(p.Item2.Length, false, false));
                    c++;
                }
            });
        }


        /// <summary>
        /// Empty abstract method implementation
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }
    }
}
