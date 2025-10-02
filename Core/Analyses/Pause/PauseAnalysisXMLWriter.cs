using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Pause
{
    /// <summary>
    /// Analysis XMLWriter for the Pause Analysis.
    /// </summary>
    public class PauseAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants

        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "pause_analysis.xsl";
        private const string MODULE_TAG = "module";
        private const string BLOCK_TAG = "block";
        private const string ELEMENT_TAG = "element";

        // Module names
        private const string GENERAL_INFORMATION = "General Information";
        private const string PAUSE_LOCATION = "Pause Location";
        private const string COMBINEDPAUSE_LOCATION = "Combined Pause Location";
        private const string SUMMARY_PER_INTERVAL = "Summary per Interval";

        // Block names Within Word Pauses
        private const string WITHIN_WORDS = "Within Words";

        // Block names Combined Pauses
        private const string BETWEEN_WORDS = "Between Words";
        private const string BETWEEN_SENTENCES = "Between Sentences";
        private const string BETWEEN_PARAGRAPHS = "Between Paragraphs";

        // Block names Before - After Pauses
        private const string BEFORE_WORDS = "Before Words";

        private const string AFTER_WORDS = "After Words";
        private const string BEFORE_SENTENCES = "Before Sentences";
        private const string AFTER_SENTENCES = "After Sentences";
        private const string BEFORE_PARAGRAPHS = "Before Paragraphs";
        private const string AFTER_PARAGRAPHS = "After Paragraphs";

        // Miscellaneous Pauses.
        private const string MISC_PAUSES = "Miscellaneous Pauses";
        private const string INITIAL_PAUSES = "INITIAL PAUSES";
        private const string END_PAUSES = "END PAUSES";
        private const string COMBINATIONKEY_PAUSES = "COMBINATION KEY PAUSES";
        private const string REVISION_PAUSES = "REVISION PAUSES";
        private const string CHANGE_PAUSES = "CHANGE PAUSES";
        private const string UNKNOWN_PAUSES = "UNKNOWN & UNDETERMINED PAUSES";

        // Intervals
        private const string INTERVAL = "Interval ";
        private const string INTERVAL_START = "Start Time";

        // Element names
        private const string TOTAL_PROCESS_TIME = "Total Process Time";
        private const string TOTAL_WRITING_TIME = "Total Active Writing Time";
        private const string TOTAL_WRITING_TIME_S = "Total Active Writing Time (s)";
        private const string TOTAL_NUMBER_OF_PAUSES = "Total Number of Pauses";
        private const string TOTAL_PAUSE_TIME = "Total Pause Time";
        private const string PAUSE_TIME_PROP = "Proportion of Pause Time";
        private const string TOTAL_PAUSE_TIME_S = "Total Pause Time (s)";
        private const string TOTAL_PROCESS_TIME_S = "Total Process Time (s)";
        private const string NUMBER_OF_PAUSES = "Number of Pauses";
        private const string MEAN_PAUSE_TIME = "Arithmetic Mean of Pauses (s)";
        private const string GEOMEAN_PAUSE_TIME = "Geometric Mean of Pauses (s)";
        private const string CI_L = "95% CI Log-Transformed - Low Boundary (s)";
        private const string CI_H = "95% CI Log-Transformed - High Boundary (s)";
        private const string CoV = "Coefficient of Variation";
        private const string MEDIAN_PAUSE_TIME = "Median Pause Time (s)";
        private const string STANDARD_DEVIATION = "Standard Deviation (s)";
        private const string INITIAL_ID = "Id of the First Key Event";
        private const string INITIAL_START_TIME = "Start Time of the First Key Event (ms)";

        private const string NUMBER_OF_PBURSTS = "Number of P-Bursts";
        private const string PBURST_MIN = "Number of P-Bursts per min.";
        private const string AVG_PROCESS_TIME = "Mean Process Time P-Bursts (s)";
        private const string MEDIAN_PROCESS_TIME = "Median Process Time P-Bursts (s)";
        private const string STANDARD_DEVIATION_PROCESS_TIME = "Standard Deviation P-Bursts (s)";
        private const string AVG_PROCESS_CHARS = "Mean Typed In P-Bursts (chars)";
        private const string MEDIAN_PROCESS_CHARS = "Median Typed In P-Bursts (chars)";
        private const string STANDARD_DEVIATION_PROCESS_CHARS = "Standard Deviation P-Bursts (chars)";

        // Meta Parameters
        public const string PAUSE_THRESHOLD_PARAMETER = "Pause Threshold (ms)";
        public const string PBURST_THRESHOLD_PARAMETER = "P-Burst Threshold (ms)";
        public const string TYPE_PARAMETER = "Pause Analysis Interval Type";

        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo( "en-US", false ).NumberFormat;
       
        #endregion

        /// <summary>
        /// Constructs a PauseAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public PauseAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
        }

        /// <summary>
        /// Writes out the analysis document using a given PauseAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which 
        /// the Pause Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is CompoundPauseAnalysisSummary))
                throw new AnalysisWriterException("Given Summary is not a Pause Analysis");

            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 3;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";

            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary((CompoundPauseAnalysisSummary) summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] {STYLESHEET_HREF, COMMON_CSS, COMMON_XSL}, new[] {INPUTLOG_LOGO});
        }

        /// <summary>
        /// Writes pause summary analysis 
        /// </summary>
        /// <param name="compoundSummary"></param>
        private void WriteSummary(CompoundPauseAnalysisSummary compoundSummary)
        {
            SingularPauseAnalysisSummary analysisSummary = 
                compoundSummary.Summaries[CompoundPauseAnalysisSummary.TRESHOLD_DEFAULT];

            // General Information
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, GENERAL_INFORMATION);

            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, "Overview");

            WriteElement(TOTAL_PROCESS_TIME,
                DateTimeUtils.MsecToClockString(Convert.ToUInt64(analysisSummary.GeneralInformation.TotalProcessTime), false));
            WriteElement(TOTAL_PAUSE_TIME,
                DateTimeUtils.MsecToClockString(Convert.ToUInt64(analysisSummary.GeneralInformation.TotalPauseTime), false));
            WriteElement(TOTAL_WRITING_TIME, 
                DateTimeUtils.MsecToClockString(Convert.ToUInt64(analysisSummary.GeneralInformation.TotalWritingTime), false));
            WriteElement(TOTAL_PROCESS_TIME_S, (analysisSummary.GeneralInformation.TotalProcessTime / 1000.0).ToString("F", Nfi));
            WriteElement(TOTAL_PAUSE_TIME_S, (analysisSummary.GeneralInformation.TotalPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(TOTAL_WRITING_TIME_S, (analysisSummary.GeneralInformation.TotalWritingTime / 1000.0).ToString("F", Nfi));
            WriteElement(PAUSE_TIME_PROP, (analysisSummary.GeneralInformation.PauseTimeProportion * 100).ToString("F", Nfi) + " %");
            XMLWriter.WriteEndElement(); // end of block

            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, "General");
            WriteElement(TOTAL_NUMBER_OF_PAUSES, analysisSummary.GeneralInformation.TotalNumberOfPauses.ToString());
            WriteElement(MEAN_PAUSE_TIME, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2 ? ""
                : (analysisSummary.GeneralInformation.MeanPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PAUSE_TIME, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2 ? ""
                : (analysisSummary.GeneralInformation.MedianPauseTime / 1000.0).ToString("F", Nfi));            
            WriteElement(GEOMEAN_PAUSE_TIME, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2 
                ? "" : (analysisSummary.GeneralInformation.GeoMeanPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(CI_L, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2
                ? "" : ((analysisSummary.GeneralInformation.CI95L/1000.0).ToString("F", Nfi)));
            WriteElement(CI_H, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2
                ? "" : ((analysisSummary.GeneralInformation.CI95H/1000.0).ToString("F", Nfi)));
            //+ " (ms min. " + analysisSummary.GeneralInformation.MinPause + " - ms max. " 
            //+ analysisSummary.GeneralInformation.MaxPause + ")"));
            WriteElement(CoV, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2 ? ""
                : (analysisSummary.GeneralInformation.CoefVar * 100).ToString("F", Nfi) + " %");
            WriteElement(STANDARD_DEVIATION, analysisSummary.GeneralInformation.TotalNumberOfPauses < 2 ? ("") 
                : (analysisSummary.GeneralInformation.StDev/1000.0).ToString("F", Nfi));
            WriteElement(INITIAL_ID, analysisSummary.GeneralInformation.FirstKeyPressId.ToString());
            WriteElement(INITIAL_START_TIME, analysisSummary.GeneralInformation.FirstStartKeyTime.ToString("F", Nfi));
            XMLWriter.WriteEndElement(); // end of block

            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, "P-Bursts at " + analysisSummary.GeneralInformation.PBurstTreshold + " ms");
            WriteElement(NUMBER_OF_PBURSTS, analysisSummary.GeneralInformation.NumberOfSegments.ToString("F", Nfi));
            WriteElement(PBURST_MIN, analysisSummary.GeneralInformation.PBurstsPerMinute.ToString("F", Nfi));
            WriteElement(AVG_PROCESS_TIME, (analysisSummary.GeneralInformation.AvgProcessTime / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PROCESS_TIME, (analysisSummary.GeneralInformation.MedianProcessTime / 1000.0).ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_PROCESS_TIME, analysisSummary.GeneralInformation.StandardDeviation.Equals(0)
                ? "" : (analysisSummary.GeneralInformation.StandardDeviation / 1000.0).ToString("F", Nfi));
            WriteElement(AVG_PROCESS_CHARS, analysisSummary.GeneralInformation.AvgProcessChars.ToString("F", Nfi));
            WriteElement(MEDIAN_PROCESS_CHARS, (analysisSummary.GeneralInformation.MedianProcessChars).ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_PROCESS_CHARS, analysisSummary.GeneralInformation.StandardDeviationChars.Equals(0)
                ? "" : analysisSummary.GeneralInformation.StandardDeviationChars.ToString("F", Nfi));
            XMLWriter.WriteEndElement();

         //   XMLWriter.WriteEndElement(); // end of block
            XMLWriter.WriteEndElement(); // end of module

            // Pause Location module tag
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PAUSE_LOCATION);

            // WITHIN_WORDS 
            foreach (var entry in analysisSummary.BasePauseTypes)
            {
                string blockName = null;
                switch (entry.Key)
                {
                    case PauseLocation.WITHIN_WORDS:
                        blockName = WITHIN_WORDS;
                        break;
                }
                if (blockName == null) continue;

                XMLWriter.WriteStartElement(BLOCK_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, blockName);
                switch (blockName)
                {
                    case WITHIN_WORDS:
                        WriteElement(NUMBER_OF_PAUSES, entry.Value.NumberOfPauses.ToString());
                        WriteElement(MEAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? "" 
                            : (entry.Value.MeanPauseTime / 1000.0).ToString("F", Nfi));
                        WriteElement(MEDIAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? ""
                            : (entry.Value.MedianPauseTime / 1000.0).ToString("F", Nfi));
                        WriteElement(GEOMEAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? "" : (entry.Value.GeoMeanPauseTime / 1000.0).ToString("F", Nfi));
                        WriteElement(CI_L, entry.Value.NumberOfPauses < 2
                            ? "" : ((entry.Value.CI95L/1000.0).ToString("F", Nfi)));
                        WriteElement(CI_H, entry.Value.NumberOfPauses < 2
                            ? "" : ((entry.Value.CI95H/1000.0).ToString("F", Nfi)));
                         //   + " (ms min. " + entry.Value.MinPause + " - ms max. " + entry.Value.MaxPause + ")"));
                        WriteElement(CoV, entry.Value.NumberOfPauses < 2 ? ""
                            : (entry.Value.CoefVar * 100).ToString("F", Nfi) + " %");
                        WriteElement(STANDARD_DEVIATION, entry.Value.NumberOfPauses < 2 ? ("") 
                            : (entry.Value.StDev / 1000.0).ToString("F", Nfi));
                        break;
                }
                XMLWriter.WriteEndElement();
            }

            // BEFORE - AFTER statistics
            foreach (var entry in analysisSummary.BetweenBeforeTypes)
            {
                switch (entry.Key)
                {
                    case PauseLocation.BEFORE_WORDS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BEFORE_WORDS);
                        WriteBefore(entry);
                        XMLWriter.WriteEndElement();
                        break;
                    case PauseLocation.BEFORE_SENTENCES:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BEFORE_SENTENCES);
                        WriteBefore(entry);
                        XMLWriter.WriteEndElement();
                        break;
                    case PauseLocation.BEFORE_PARAGRAPHS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BEFORE_PARAGRAPHS);
                        WriteBefore(entry);
                        XMLWriter.WriteEndElement();
                        break;
                }
            }

            // BEFORE - AFTER statistics
            foreach (var entry in analysisSummary.BetweenAfterTypes)
            {
                switch (entry.Key)
                {
                    case PauseLocation.AFTER_WORDS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, AFTER_WORDS);
                        WriteAfter(entry);
                        XMLWriter.WriteEndElement();
                        break;
                    case PauseLocation.AFTER_SENTENCES:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, AFTER_SENTENCES);
                        WriteAfter(entry);
                        XMLWriter.WriteEndElement();
                        break;
                    case PauseLocation.AFTER_PARAGRAPHS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, AFTER_PARAGRAPHS);
                        WriteAfter(entry);
                        XMLWriter.WriteEndElement();
                        break;
                }
            }

            // Miscellaneous Information
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, MISC_PAUSES);

            foreach (var entry in analysisSummary.MiscellaneousTypes)
            {
                switch (entry.Key)
                {
                    case PauseLocation.INITIAL:
                        WriteElement(INITIAL_PAUSES, "");
                        WritePause(entry);
                        break;
                    case PauseLocation.END:
                        WriteElement(END_PAUSES, "");
                        WritePause(entry);
                        break;
                    case PauseLocation.COMBINATION_KEY:
                        WriteElement(COMBINATIONKEY_PAUSES, "");
                        WritePause(entry);
                        break;
                    case PauseLocation.REVISION:
                        WriteElement(REVISION_PAUSES, "");
                        WritePause(entry);
                        break;
                    case PauseLocation.CHANGE:
                        WriteElement(CHANGE_PAUSES, "");
                        WritePause(entry);
                        break;
                    case PauseLocation.UNKNOWN:
                        WriteElement(UNKNOWN_PAUSES, "");
                        WritePause(entry);
                        break;
                }
            }
            XMLWriter.WriteEndElement();  // end of block
            XMLWriter.WriteEndElement(); // end of module

            // Combined Pause Location module tag
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, COMBINEDPAUSE_LOCATION);

            // BETWEEN statistics
            foreach (var entry in analysisSummary.CombinedTypes)
            {
                switch (entry.Key)
                {
                    case PauseLocation.BEFORE_WORDS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BETWEEN_WORDS);
                        WriteCombinedPauses(entry);
                        XMLWriter.WriteEndElement(); 
                        break;
                    case PauseLocation.BEFORE_SENTENCES:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BETWEEN_SENTENCES);
                        WriteCombinedPauses(entry);
                        XMLWriter.WriteEndElement(); 
                        break;
                    case PauseLocation.BEFORE_PARAGRAPHS:
                        XMLWriter.WriteStartElement(BLOCK_TAG);
                        XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, BETWEEN_PARAGRAPHS);
                        WriteCombinedPauses(entry);
                        XMLWriter.WriteEndElement(); 
                        break;
                }
            }

            XMLWriter.WriteEndElement(); // end of module

            // Information per interval
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, SUMMARY_PER_INTERVAL);
            foreach (SingularPauseAnalysisSummary.IntervalStats intervalTypeInfo in analysisSummary.IntervalInfo.Values)
            {
                XMLWriter.WriteStartElement(BLOCK_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, INTERVAL + intervalTypeInfo.IntervalSegment);
                WriteElement(INTERVAL_START, DateTimeUtils.MsecToClockString(intervalTypeInfo.IntervalStart, false));
                WriteElement(NUMBER_OF_PAUSES, intervalTypeInfo.NumberOfPauses.ToString());
                WriteElement(MEAN_PAUSE_TIME, intervalTypeInfo.NumberOfPauses < 2 ? "" 
                    : (intervalTypeInfo.MeanPauseTime / 1000.0).ToString("F", Nfi));
                WriteElement(MEDIAN_PAUSE_TIME, intervalTypeInfo.NumberOfPauses < 2 ? ""
                    : (intervalTypeInfo.MedianPauseTime/1000.0).ToString("F", Nfi));
                WriteElement(GEOMEAN_PAUSE_TIME, intervalTypeInfo.NumberOfPauses < 2 ? "" : (intervalTypeInfo.GeoMeanPauseTime / 1000.0).ToString("F", Nfi));
                WriteElement(CI_L, intervalTypeInfo.NumberOfPauses < 2
                    ? "" : ((intervalTypeInfo.CI95L/1000.0).ToString("F", Nfi)));
                WriteElement(CI_H, intervalTypeInfo.NumberOfPauses < 2
                    ? "" : ((intervalTypeInfo.CI95H/1000.0).ToString("F", Nfi)));
                  //  + " (ms min. " + intervalTypeInfo.MinPause + " - ms max. " + intervalTypeInfo.MaxPause + ")"));
                WriteElement(CoV, intervalTypeInfo.NumberOfPauses < 2 ? ""
                    : (intervalTypeInfo.CoefVar * 100).ToString("F", Nfi) + " %");
                WriteElement(STANDARD_DEVIATION, intervalTypeInfo.NumberOfPauses < 2 ? ""
                    : (intervalTypeInfo.StDev / 1000.0).ToString("F", Nfi));
                XMLWriter.WriteEndElement(); // end of block
            }
            XMLWriter.WriteEndElement(); // end of module
        }

        /// <summary>
        /// Writes out the statistics of the Miscellaneous Pauses
        /// </summary>
        /// <param name="entry"></param>
        private void WritePause(KeyValuePair<PauseLocation, SingularPauseAnalysisSummary.MiscellaneousStats> entry)
        {
            WriteElement(NUMBER_OF_PAUSES, entry.Value.NumberOfPauses.ToString());
            WriteElement(MEAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? "" 
                : (entry.Value.MeanPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? ""
                : (entry.Value.MedianPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(GEOMEAN_PAUSE_TIME, entry.Value.NumberOfPauses < 2 ? "" 
                : (entry.Value.GeoMeanPauseTime / 1000.0).ToString("F", Nfi));
            WriteElement(CI_L, entry.Value.NumberOfPauses < 2
                ? "" : ((entry.Value.CI95L/1000.0).ToString("F", Nfi)));
           WriteElement(CI_H, entry.Value.NumberOfPauses < 2
                ? "" : ((entry.Value.CI95H/1000.0).ToString("F", Nfi)));
               // + " (ms min. " + entry.Value.MinPause + " - ms max. " + entry.Value.MaxPause + ")"));
            WriteElement(CoV, entry.Value.NumberOfPauses < 2 ? ""
                : (entry.Value.CoefVar * 100).ToString("F", Nfi) + " %");
            WriteElement(STANDARD_DEVIATION, entry.Value.NumberOfPauses < 2 ? "" 
                : (entry.Value.StDev / 1000.0).ToString("F", Nfi));
        }

        /// <summary>
        /// Writes out the statistics of the combined pauses between 'after' and 'before'
        /// </summary>
        /// <param name="entry"></param>
        private void WriteCombinedPauses(KeyValuePair<PauseLocation, SingularPauseAnalysisSummary.CombinedStats> entry)
        {
            WriteElement(NUMBER_OF_PAUSES, entry.Value.CombinedPauses.ToString());
            WriteElement(MEAN_PAUSE_TIME, entry.Value.CombinedPauses < 2 ? "" 
                : (entry.Value.CombinedMean / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PAUSE_TIME, entry.Value.CombinedPauses < 2 ? ""
                : (entry.Value.CombinedMedian / 1000.0).ToString("F", Nfi));
            WriteElement(GEOMEAN_PAUSE_TIME, entry.Value.CombinedPauses < 2 ? "" 
                : (entry.Value.CombinedGeoMean / 1000.0).ToString("F", Nfi));
            WriteElement(CI_L, entry.Value.CombinedPauses < 2
                ? "" : ((entry.Value.CombinedCi95L/1000.0).ToString("F", Nfi)));
            WriteElement(CI_H, entry.Value.CombinedPauses < 2
                ? "" : ((entry.Value.CombinedCi95H/1000.0).ToString("F", Nfi)));
              //  +" (ms min. " + entry.Value.CombinedMinPause + " - ms max. " + entry.Value.CombinedMaxPause + ")"));
            WriteElement(CoV, entry.Value.CombinedPauses < 2 ? ""
                : (entry.Value.CombinedCoefVar * 100).ToString("F", Nfi) + " %");
            WriteElement(STANDARD_DEVIATION, entry.Value.CombinedPauses < 2 ? "" 
                : (entry.Value.CombinedStDev / 1000.0).ToString("F", Nfi));
        }

        /// <summary>
        /// Writes out the 'before' statistics
        /// </summary>
        /// <param name="entry"></param>
        private void WriteBefore(KeyValuePair<PauseLocation, SingularPauseAnalysisSummary.BetweenBeforeStats> entry)
        {
            WriteElement(NUMBER_OF_PAUSES, entry.Value.BetweenBeforePauses.ToString());
            WriteElement(MEAN_PAUSE_TIME, entry.Value.BetweenBeforePauses < 2 ? "" 
                : (entry.Value.BeforeMean / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PAUSE_TIME, entry.Value.BetweenBeforePauses < 2 ? ""
                : (entry.Value.BeforeMedian / 1000.0).ToString("F", Nfi));
            WriteElement(GEOMEAN_PAUSE_TIME, entry.Value.BetweenBeforePauses < 2 ? "" 
                : (entry.Value.BeforeGeoMean / 1000.0).ToString("F", Nfi));
            WriteElement(CI_L, entry.Value.BetweenBeforePauses < 2
                ? "" : ((entry.Value.BeforeCi95L/1000.0).ToString("F", Nfi)));
            WriteElement(CI_H,entry.Value.BetweenBeforePauses < 2
                ? "" : ((entry.Value.BeforeCi95H / 1000.0).ToString("F", Nfi)));
              //  + " (ms min. " + entry.Value.BeforeMinPause + " - ms max. " + entry.Value.BeforeMaxPause + ")"));
            WriteElement(CoV, entry.Value.BetweenBeforePauses < 2 ? ""
                : (entry.Value.BeforeCoefVar * 100).ToString("F", Nfi) + " %");
            WriteElement(STANDARD_DEVIATION, entry.Value.BetweenBeforePauses < 2 ? "" 
                : (entry.Value.BeforeStDev / 1000.0).ToString("F", Nfi));
        }

        /// <summary>
        ///  Writes out the 'after' statistics
        /// </summary>
        /// <param name="entry"></param>
        private void WriteAfter(KeyValuePair<PauseLocation, SingularPauseAnalysisSummary.BetweenAfterStats> entry)
        {
            WriteElement(NUMBER_OF_PAUSES, entry.Value.BetweenAfterPauses.ToString());
            WriteElement(MEAN_PAUSE_TIME, entry.Value.BetweenAfterPauses < 2 ? ""
                : (entry.Value.AfterMean / 1000.0).ToString("F", Nfi));
            WriteElement(MEDIAN_PAUSE_TIME, entry.Value.BetweenAfterPauses < 2 ? ""
                : (entry.Value.AfterMedian / 1000.0).ToString("F", Nfi));
            WriteElement(GEOMEAN_PAUSE_TIME, entry.Value.BetweenAfterPauses < 2 ? "" 
                : (entry.Value.AfterGeoMean / 1000.0).ToString("F", Nfi));
            WriteElement(CI_L, entry.Value.BetweenAfterPauses < 2
                ? "" : ((entry.Value.AfterCi95L/1000.0).ToString("F", Nfi)));
            WriteElement(CI_H, entry.Value.BetweenAfterPauses < 2
              ? "" : ((entry.Value.AfterCi95H / 1000.0).ToString("F", Nfi)));
               // + " (ms min. " + entry.Value.AfterMinPause + " - ms max. " + entry.Value.AfterMaxPause + ")"));
            WriteElement(CoV, entry.Value.BetweenAfterPauses < 2 ? "" 
                : (entry.Value.AfterCoefVar * 100).ToString("F", Nfi) + " %");
            WriteElement(STANDARD_DEVIATION, entry.Value.BetweenAfterPauses < 2 ? ""
                : (entry.Value.AfterStDev / 1000.0).ToString("F", Nfi));
        }

        /// <summary>
        /// Writes out an element to the xmlstream.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">Value of the element.</param>
        private void WriteElement(string elementName, string elementValue)
        {
            XMLWriter.WriteStartElement(ELEMENT_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, elementName);
            XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, elementValue);
            XMLWriter.WriteEndElement();
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