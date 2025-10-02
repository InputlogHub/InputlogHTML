using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Bigram
{
    /// <summary>
    /// Analysis XMLWriter for the Linear Analysis.
    /// </summary>
    public class BigramAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {

        #region Constants

        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "bigram_analysis.xsl";
        private const string PERIOD_TAG = "Period";
        private const string PERIOD_EVENT_TAG = "PeriodEvent";

        // Period/Event attributes
        private const string PERIOD_ID_ATTRIBUTE = "period_id";
        private const string PERIOD_TIME_ATTRIBUTE = "periodTime";
        private const string LINK_ATTRIBUTE = "link";

        public const string TASK_MAXIMUM_MODE_PARAMETER = "Task Maximum Mode";
        public const string TYPE_PARAMETER = "Fluency Analysis Type";

        public const string SPACE = "SPACE";
        public const string DELETE = "DELETE";
        public const string RIGHT = "RIGHT";
        public const string BACK = "BACK";
        public const string LEFT = "LEFT";

        public const string ABSOLUTE_MAXIMUM_TAG = "AbsMaximum";
        public const string PERSONAL_MAXIMUM_TAG = "PersonalMaximum";
        public const string TASK_MAXIMUM_TAG = "TaskLMaximum";
        public const string TASK_MAXIMUM_PERIOD = "TaskMaximumPeriod";
        public const string TASK_MAXIMUM_WINDOW = "TaskOptimumWindow";
        public const string TASK_MAXIMUM_LOCATION = "TaskOptimumLocation";

        public const string AVERAGE_SPM_TITLE = "Average Strokes per Minute";
        public const string STDDEV_SPM_TITLE = "Standard Deviation";
        public const string INTERVAL_LENGTH = "Interval Length";
        public const string TOTAL_LENGTH = "Total Length";
        public const string TOTAL_STROKES = "Total Strokes";

        public const string NR_STROKES_TAG = "NrStrokes";
        public const string STROKES_PER_MIN_TAG = "StrokesPerMin";
        public const string LENGTH_TAG = "Length";
        public const string ABSOLUTE_PERCENTAGE_TAG = "AbsolutePercentage";
        public const string TASK_PERCENTAGE_TAG = "TaskPercentage";
        public const string PERSONAL_PERCENTAGE_TAG = "PersonalPercentage";

        private BigramAnalysisSummary Summary;

        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        /// <summary>
        /// Constructs a LinearAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public BigramAnalysisXMLWriter(string destinationFilePath)
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
        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string,
            IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is BigramAnalysisSummary)) throw new AnalysisWriterException("Given summary is not of type" +
                " BigramAnalysisSummary");
            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 1;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary((BigramAnalysisSummary)summary);
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_CSS, COMMON_XSL }, new[] { INPUTLOG_LOGO });
            CopyText(HTML_README);
            XMLWriter.Close();
        }

        /// <summary>
        /// Writes the summary to the stream.
        /// Loops over all the periods in the stream, writes out the start and periodtime for each of them.
        /// Then loops over all the events within a period and writes out the events (based on there type, 
        /// they are represented differently).
        /// </summary>
        /// <param name="summary">The analysis summary</param>
        private void WriteSummary(BigramAnalysisSummary summary)
        {
           Summary = summary;
           WriteModule("Summary", delegate
                {
                    WriteModuleBlock("Summary", delegate
                    {
                        WriteModuleElement("Count", summary.GetCount().ToString());
                        WriteModuleElement("Mean", summary.GetMean().ToString("F",Nfi));
                        WriteModuleElement("StdDev", summary.GetStdDev().ToString("F", Nfi));
                        WriteModuleElement("Median", StringUtils.DoubleIfNeeded(summary.GetMedian(), "F", Nfi));
                        WriteModuleElement("Min", summary.GetMin().ToString());
                        WriteModuleElement("Max", summary.GetMax().ToString());
                        double mean = summary.GetMean();
                        double shift = summary.Get95Pct();
                        WriteModuleElement("95Pct", "[" + (mean - shift).ToString("F", Nfi) + "  "
                            + (mean + shift).ToString("F", Nfi) + "]");

                        WriteModuleElement("Top5Fastest", string.Join("; ", 
                            summary.Top5Fastest.Select(i => string.Format("{0}: {1}", 
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                        WriteModuleElement("Top5Slowest", string.Join("; ", 
                            summary.Top5Slowest.Select(i => string.Format("{0}: {1}",
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                        WriteModuleElement("Top5Freq", string.Join("; ", 
                            summary.Top5Freq.Select(i => string.Format("{0}: {1}", 
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                        WriteModuleElement("Bot5Freq", string.Join("; ", 
                            summary.Bot5Freq.Select(i => string.Format("{0}: {1}", 
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                        WriteModuleElement("Top5Freq2", string.Join("; ", 
                            summary.Top5Freq2.Select(i => string.Format("{0}: {1}",
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                        WriteModuleElement("Bot5Freq2", string.Join("; ", 
                            summary.Bot5Freq2.Select(i => string.Format("{0}: {1}", 
                                i.Key, i.GetMean().ToString("F", Nfi)))));
                    });
                });

           WriteModule("Bigram Classes", delegate
           {
               WriteBlocks(summary.ClassGrams);
           });

           WriteModule("Alphabet Bigrams", delegate
           {
               WriteBlocks(summary.AlphaGrams);
           });

           WriteModule("Non-Alpha Bigrams", delegate
           {
               WriteBlocks(summary.SpecialGrams);
           });

           WriteModule("Top5Fastest", delegate
           {
               WriteTop5Blocks(summary.Top5Fastest);
           });

           WriteModule("Top5Slowest", delegate
           {
               WriteTop5Blocks(summary.Top5Slowest);
           });

           WriteModule("Top5Freq", delegate
           {
               WriteTop5Blocks(summary.Top5Freq2);
           });

           WriteModule("Top5Nonfreq", delegate
           {
               WriteTop5Blocks(summary.Bot5Freq2);
           });

           WriteModule("Top5Freq (TFIDF)", delegate
           {
               WriteTop5Blocks(summary.Top5Freq);
           });

           WriteModule("Top5Nonfreq (TFIDF)", delegate
           {
               WriteTop5Blocks(summary.Bot5Freq);
           });
        }

        private void WriteTop5Blocks(List<BigramAnalysisSummary.AlphaBigramInfo> list)
        {
            WriteModuleBlock("Bigram", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetKey());
                    c++;
                }
            });

            WriteModuleBlock("Count", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetCount().ToString());
                    c++;
                }
            });

            WriteModuleBlock("Mean", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetMean().ToString("F", Nfi));
                    c++;
                }
            });

            WriteModuleBlock("StdDev", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetStdDev().ToString("F", Nfi));
                    c++;
                }
            });

            WriteModuleBlock("Median", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), StringUtils.DoubleIfNeeded(b.GetMedian(), "F", Nfi));
                    c++;
                }
            });

            WriteModuleBlock("Min", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetMin().ToString());
                    c++;
                }
            });

            WriteModuleBlock("Max", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    WriteModuleElement(c.ToString(), b.GetMax().ToString());
                    c++;
                }
            });

            WriteModuleBlock("95PctIntervalLo", delegate
            {
               
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    // var intervals = b.GetInterval();
                    // WriteModuleElement(c.ToString(), Convert.ToInt32(intervals.Item1).ToString());
                    WriteModuleElement(c.ToString(), Convert.ToInt32(b.Get95PctLow()).ToString());
                    c++;
                }
            });

            WriteModuleBlock("95PctIntervalHi", delegate
            {
                int c = 1;
                foreach (BigramAnalysisSummary.AlphaBigramInfo b in list)
                {
                    // var intervals = b.GetInterval();
                    // WriteModuleElement(c.ToString(), Convert.ToInt32(intervals.Item3).ToString());
                    WriteModuleElement(c.ToString(), Convert.ToInt32(b.Get95PctHigh()).ToString());
                    c++;
                }
            });
        }

        private void WriteBlocks<T>(Dictionary<string, T> grams) where T : BigramAnalysisSummary.SpecialBigramInfo
        {
            WriteModuleBlock("Count", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, grams[s].GetCount().ToString());
                }
            });

            WriteModuleBlock("Mean", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, grams[s].GetMean().ToString("F", Nfi));
                }
            });

            WriteModuleBlock("StdDev", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, grams[s].GetStdDev().ToString("F", Nfi));
                }
            });

            WriteModuleBlock("Median", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, StringUtils.DoubleIfNeeded(grams[s].GetMedian(), "F", Nfi));
                }
            });

            WriteModuleBlock("Min", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, grams[s].GetMin().ToString());
                }
            });

            WriteModuleBlock("Max", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, grams[s].GetMax().ToString());
                }
            });

            WriteModuleBlock("95PctIntervalLo", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, Convert.ToInt32(grams[s].Get95PctLow()).ToString());
                }
            });

            WriteModuleBlock("95PctIntervalHi", delegate
            {
                foreach (string s in grams.Keys)
                {
                    WriteModuleElement(s, Convert.ToInt32(grams[s].Get95PctHigh()).ToString());
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
