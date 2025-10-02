using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Summary
{
    /// <summary>
    /// Analysis XMLWriter for the Summary Analysis.
    /// </summary>
    public class SummaryAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "summary_analysis.xsl";
        private const string MODULE_TAG = "module";
        private const string BLOCK_TAG = "block";
        private const string ELEMENT_TAG = "element";
        private const string TAB = "- ";

        //Attribute Values
        private const string PRODUCT_INFORMATION_MODULE = "Product Information";
        private const string PROCESS_INFORMATION_MODULE = "Process Information";
        private const string PRODUCT_PROCESS_RATIO_MODULE = "Product/Process";

        private const string INITIALTEXT_BLOCK = "Initial Text";
        private const string WORDS_BLOCK = "Words";
        private const string CHAR_BLOCK = "Keystrokes";
        private const string CHARACTERRATIO_BLOCK = "Ratio";
        private const string PROPORTION_BLOCK = "Proportion";
        private const string TOTAL_TYPED_WITHOUT_SPACES = "Total Typed (excl.spaces)";
        private const string TOTAL_WITHOUT_SPACES = "Total (excl.spaces)";
        private const string TOTAL_CHARS_WITHOUT_SPACES = "Total Characters (excl.spaces)";
        private const string CHARACTERSNS_PER_MINUTE = "Per Minute (excl.spaces)";
        private const string WORDS_PER_MINUTE = "Per Minute";
        private const string TOTAL_TYPED_INCLUDING_SPACES = "Total Typed (incl.spaces)";
        private const string TOTAL_INCLUDING_SPACES = "Total (incl.spaces)";
        private const string TOTAL_CHARS_INCLUDING_SPACES = "Total Characters (incl.spaces)";
        private const string TOTAL_CHARACTERS_COPIED = "Characters Inserted";
        private const string TOTAL_CHARACTERS_REPLACED = "Characters Replaced";
        private const string CHARACTERSWS_PER_MINUTE = "Per Minute (incl. spaces)";
        private const string COMBINATION_KEYS = "Total Non-Character Keys";

        private const string CHARACTERS_WITHOUT_SPACES_RATIO = "Characters (excl.spaces)";
        private const string CHARACTERS_INCLUDING_SPACES_RATIO = "Characters (incl.spaces)";
        private const string PRODUCED_INCLUDING_SPACES_RATIO = "Produced Ratio (incl.spaces)";
        private const string WORD_RATIO = "Words";
       // private const string TOTAL_FAR_EAST_CHARACTERS_INCLUDING_SPACES = "Total Far East Characters";
        private const string AVG_WORD_LENGTH = "Mean Word Length";
        private const string MEDIAN_WORD_LENGTH = "Median Word Length";
        private const string STANDARD_DEVIATION_WL = "Standard Deviation Word Length";

        private const string SENTENCES_BLOCK = "Sentences";
        private const string AVG_CHARACTERS_PER_SENTENCE = "Mean Characters/Sentence ";
        private const string MEDIAN_CHARACTERS_PER_SENTENCE = "Median Characters/Sentence ";
        private const string STANDARD_DEVIATION_CS = "Standard Deviation Characters/Sentence";
        private const string AVG_WORDS_PER_SENTENCES = "Mean Words/Sentence";
        private const string MEDIAN_WORDS_PER_SENTENCES = "Median Words/Sentence";
        private const string STANDARD_DEVIATION_WS = "Standard Deviation Words/Sentence";

        private const string PARAGRAPHS_BLOCK = "Paragraphs";
        private const string AVG_CHARACTERS_PER_PARAGRAPH = "Mean Characters/Paragraph";
        private const string MEDIAN_CHARACTERS_PER_PARAGRAPH = "Median Characters/Paragraph";
        private const string STANDARD_DEVIATION_CP = "Standard Deviation Characters/Paragraph";
        private const string AVG_WORDS_PER_PARAGRAPH = "Mean Words/Paragraph";
        private const string MEDIAN_WORDS_PER_PARAGRAPH = "Median Words/Paragraph";
        private const string STANDARD_DEVIATION_WP = "Standard Deviation Words/Paragraph";
        private const string AVG_SENTENCES_PARAGRAPH = "Mean Sentences/Paragraph";
        private const string MEDIAN_SENTENCES_PARAGRAPH = "Median Sentences/Paragraph";
        private const string STANDARD_DEVIATION_SP = "Standard Deviation Sentences/Paragraph";

        private const string LINES_BLOCK = "Lines";

        private const string PAGES_BLOCK = "Pages";

        private const string PROCESS_TIME_MODULE = "Process Time";

        private const string GENERAL_BLOCK = "General";
        private const string TOTAL_PROCESS_TIME = "Total Process Time (s)";

        private const string WRITING_MODE_MODULE = "Writing Mode";

        private const string TOTAL_TIME = "Total Time (s)";
        private const string NUMBER_OF_CLUSTERS = "Number of Clusters";
        private const string AVG_TIME_CLUSTERS = "Mean Cluster Time";
        private const string MEDIAN_TIME_CLUSTERS = "Median Cluster Time";
        private const string STANDARD_DEVIATION_CLUSTERS = "Standard Deviation Cluster Time";
        private const string NUMBER_OF_MBURSTS = "Number of M-Bursts";
        private const string AVG_TIME_SEGMENTS = "Mean M-Burst Time";
        private const string MEDIAN_TIME_SEGMENTS = "Median M-Burst Time";
        private const string STANDARD_DEVIATION_SEGMENTS = "Standard Deviation M-Burst Time";

        //private const string SWITCHES_KEYBOARD_DNS;
        public const string PAUSE_THRESHOLD_PARAMETER = "Pause Threshold (ms)";

        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        /// <summary>
        /// Constructs a SummaryAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public SummaryAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
        }

        /// <summary>
        /// Writes out the analysis document using a given AnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">Identification of the logging session on which the Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is SummaryAnalysisSummary)) 
                throw new AnalysisWriterException("Given summary is not of type Summary Analysis");

            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 3;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";

            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary((SummaryAnalysisSummary)summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_CSS, COMMON_XSL }, new[] { INPUTLOG_LOGO });
        }

        /// <summary>
        /// Writes the actual analysis summary to the analysis document.
        /// </summary>
        /// <param name="summary">Summary to write out.</param>
        private void WriteSummary(SummaryAnalysisSummary summary)
        {
            WriteProcessInformationModule(summary.ProcessInformation, summary.ProcessTime);
            if (summary.ProductInformation.StatsFound)
            {
                WriteProductInformationModule(summary.ProductInformation, summary.ProcessTime);
            }
            if (summary.ProductInformation.StatsFound)
            {
                WriteProcessProductRatioModule(summary.ProductProcessRatio);
            }
            WriteProcessTimeModule(summary.ProcessTime);
            WriteWritingModeModule(summary.WritingMode);
        }

        /// <summary>
        /// Writes out an element tag to the xmlstream if value!=null.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">Value of the element.</param>
        private void WriteElement(string elementName, object elementValue)
        {
            if (elementValue == null) return;
            XMLWriter.WriteStartElement(ELEMENT_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, elementName);
            XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, elementValue.ToString());
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out the ProcessInformation statistics.
        /// </summary>
        /// <param name="processInformation">ProcessInformation statistics</param>
        /// <param name="processTime">ProcessTime statistics</param>
        private void WriteProcessInformationModule(SummaryAnalysisSummary.ProcessInformationClass processInformation,
            SummaryAnalysisSummary.ProcessTimeClass processTime)
        {
            var totalProcessTime = processTime.General.TotalProcessTime;

            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PROCESS_INFORMATION_MODULE);

            // Characters
            XMLWriter.WriteStartElement(BLOCK_TAG);

            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, CHAR_BLOCK + " Produced in This Session");
            const string descript = "Total {0} incl. Inserted and Replaced Characters in Main Document";
            switch (processInformation.Words.TotalCharsProduced)
            { 
                case 0:
                    WriteElement(string.Format(descript, CHAR_BLOCK), "n.d.");
                    break;
                default:
                    WriteElement(string.Format(descript, CHAR_BLOCK), processInformation.Words.TotalCharsProduced.ToString("F", Nfi));
                    break;
            }
            WriteElement(TAB + COMBINATION_KEYS, processInformation.Words.FormattingKeys.ToString("F", Nfi));
            WriteElement(TAB + TOTAL_CHARACTERS_COPIED, processInformation.Words.TotalCharsCopied.ToString("F", Nfi));
            WriteElement(TAB + TOTAL_CHARACTERS_REPLACED, processInformation.Words.TotalCharsReplaced.ToString("F", Nfi));
            WriteElement(TAB + TOTAL_TYPED_INCLUDING_SPACES, processInformation.Words.TotalCharactersWithSpaces.ToString("F", Nfi));
            WriteElement(TAB + CHARACTERSWS_PER_MINUTE,
                (processInformation.Words.TotalCharactersWithSpaces
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));

            WriteElement(TAB + TOTAL_TYPED_WITHOUT_SPACES, processInformation.Words.TotalCharactersWithoutSpaces);
            WriteElement(TAB + CHARACTERSNS_PER_MINUTE,
                (processInformation.Words.TotalCharactersWithoutSpaces
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));

            XMLWriter.WriteEndElement();

            // Words
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, WORDS_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", WORDS_BLOCK), 
                processInformation.Words.TotalWords.ToString("F", Nfi));
            WriteElement(WORDS_PER_MINUTE, (processInformation.Words.TotalWords 
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));

            WriteElement(AVG_WORD_LENGTH, processInformation.Words.AvgWordLength.ToString("F", Nfi));
            WriteElement(MEDIAN_WORD_LENGTH, processInformation.Words.MedianWordLength.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_WL, processInformation.Words.StandardDeviation.Equals(0)
                ? "": processInformation.Words.StandardDeviation.ToString("F", Nfi));
            XMLWriter.WriteEndElement();

            // Sentences
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, SENTENCES_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", SENTENCES_BLOCK), 
                processInformation.Sentences.TotalSentences.ToString("F", Nfi));
            WriteElement(AVG_CHARACTERS_PER_SENTENCE, processInformation.Sentences.AvgCharactersPerSentence.ToString("F", Nfi));
            WriteElement(MEDIAN_CHARACTERS_PER_SENTENCE, processInformation.Sentences.MedianCharactersPerSentence.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_CS, processInformation.Sentences.StandardDeviationCs.Equals(0) 
                ? "": processInformation.Sentences.StandardDeviationCs.ToString("F", Nfi));
            WriteElement(AVG_WORDS_PER_SENTENCES, processInformation.Sentences.AvgWordsPerSentence.ToString("F", Nfi));
            WriteElement(MEDIAN_WORDS_PER_SENTENCES, processInformation.Sentences.MedianWordsPerSentence.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_WS, processInformation.Sentences.StandardDeviationWs.Equals(0)
                ? "": processInformation.Sentences.StandardDeviationWs.ToString("F", Nfi));
            XMLWriter.WriteEndElement();

            // Paragraphs
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PARAGRAPHS_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", PARAGRAPHS_BLOCK),
                processInformation.Paragraphs.TotalParagraphs.ToString("F", Nfi));
            WriteElement(AVG_CHARACTERS_PER_PARAGRAPH, processInformation.Paragraphs.AvgCharactersPerParagraph.ToString("F", Nfi));
            WriteElement(MEDIAN_CHARACTERS_PER_PARAGRAPH, processInformation.Paragraphs.MedianCharactersPerParagraph.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_CP, processInformation.Paragraphs.StandardDeviationCp.Equals(0) 
                ? "": processInformation.Paragraphs.StandardDeviationCp.ToString("F", Nfi));
            WriteElement(AVG_WORDS_PER_PARAGRAPH, processInformation.Paragraphs.AvgWordsPerParagraph.ToString("F", Nfi));
            WriteElement(MEDIAN_WORDS_PER_PARAGRAPH, processInformation.Paragraphs.MedianWordsPerParagraph.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_WP,processInformation.Paragraphs.StandardDeviationWp.Equals(0)
                ? "": processInformation.Paragraphs.StandardDeviationWp.ToString("F", Nfi));
            WriteElement(AVG_SENTENCES_PARAGRAPH, processInformation.Paragraphs.AvgSentencePerParagraph.ToString("F", Nfi));
            WriteElement(MEDIAN_SENTENCES_PARAGRAPH, processInformation.Paragraphs.MedianSentencePerParagraph.ToString("F", Nfi));
            WriteElement(STANDARD_DEVIATION_SP,processInformation.Paragraphs.StandardDeviationSp.Equals(0)
                ? "": processInformation.Paragraphs.StandardDeviationSp.ToString("F", Nfi));
            XMLWriter.WriteEndElement();// end of block
            XMLWriter.WriteEndElement(); // end of module
        }

        /// <summary>
        /// Writes out the ProductInformation statistics.
        /// </summary>
        /// <param name="productInformation">ProductInformation statistics</param>
        /// <param name="processTime">ProcessTime statistics</param>
        private void WriteProductInformationModule(SummaryAnalysisSummary.ProductInformationClass productInformation,
            SummaryAnalysisSummary.ProcessTimeClass processTime)
        {
            var totalProcessTime = processTime.General.TotalProcessTime;

            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PRODUCT_INFORMATION_MODULE);
            // Initial Text Information
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, INITIALTEXT_BLOCK);
            WriteElement(TOTAL_CHARS_INCLUDING_SPACES, productInformation.StartCharCountWithSpaces < 0 ? "" : 
                productInformation.StartCharCountWithSpaces.ToString("F", Nfi));
            WriteElement(TOTAL_CHARS_WITHOUT_SPACES, productInformation.StartCharCountWithoutSpaces < 0 ? "" :
                productInformation.StartCharCountWithoutSpaces.ToString("F", Nfi));
            WriteElement(string.Format("Total {0} Initially ", WORDS_BLOCK),
                productInformation.StartWordCount < 0 ? "" : productInformation.StartWordCount.ToString("F", Nfi));
            XMLWriter.WriteEndElement();
            // Characters
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, "Characters in Final Text of This Session");
            WriteElement(TOTAL_INCLUDING_SPACES, productInformation.CharCountWithSpaces.ToString("F", Nfi));
            WriteElement(CHARACTERSWS_PER_MINUTE, (productInformation.CharCountWithSpaces 
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));
            WriteElement(TOTAL_WITHOUT_SPACES, productInformation.CharCountWithoutSpaces.ToString("F", Nfi));
            WriteElement(CHARACTERSNS_PER_MINUTE, (productInformation.CharCountWithoutSpaces 
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));
            XMLWriter.WriteEndElement();

            // Words
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, WORDS_BLOCK);
            //   WriteElement(TOTAL_FAR_EAST_CHARACTERS_INCLUDING_SPACES, productInformation.FarEastCharCount);
            WriteElement(string.Format("Total {0} in Main Document", WORDS_BLOCK), 
                productInformation.WordCount.ToString("F", Nfi));
            WriteElement(WORDS_PER_MINUTE, (productInformation.WordCount 
                / TimeSpan.FromMilliseconds(totalProcessTime).TotalMinutes).ToString("F", Nfi));

            XMLWriter.WriteEndElement();

            // Paragraphs
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PARAGRAPHS_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", PARAGRAPHS_BLOCK), 
                productInformation.ParagraphCount);
            XMLWriter.WriteEndElement();

            // Lines
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, LINES_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", LINES_BLOCK), 
                productInformation.LineCount);
            XMLWriter.WriteEndElement();

            // Paragraphs
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PAGES_BLOCK);
            WriteElement(string.Format("Total {0} in Main Document", PAGES_BLOCK), 
                productInformation.PageCount);
            XMLWriter.WriteEndElement();

            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out the ProcessProductRatio statistics.
        /// </summary>
        /// <param name="ratio">Ratio statistics</param>
        private void WriteProcessProductRatioModule(SummaryAnalysisSummary.ProductProcessRatioClass ratio)
        {
            // Ratio
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PRODUCT_PROCESS_RATIO_MODULE);

            // Proportions Characters
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, CHARACTERRATIO_BLOCK);
            WriteElement(PRODUCED_INCLUDING_SPACES_RATIO, ratio.ProdWithSpacesRatio.ToString("F", Nfi));
            XMLWriter.WriteEndElement();

            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PROPORTION_BLOCK);
            WriteElement(CHARACTERS_INCLUDING_SPACES_RATIO, ratio.CharWithSpacesRatio.ToString("F", Nfi));
            WriteElement(CHARACTERS_WITHOUT_SPACES_RATIO, ratio.CharWithoutSpacesRatio.ToString("F", Nfi));

            // Proportions Words
            WriteElement(WORD_RATIO, ratio.WordRatio.ToString("F", Nfi));  

            XMLWriter.WriteEndElement(); // end of block
            XMLWriter.WriteEndElement(); // end of module
        }

        /// <summary>
        /// Writes out the ProcessTime statistics.
        /// </summary>
        /// <param name="processTime">ProcessTime statistics</param>
        private void WriteProcessTimeModule(SummaryAnalysisSummary.ProcessTimeClass processTime)
        {
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, PROCESS_TIME_MODULE);

            // General
            XMLWriter.WriteStartElement(BLOCK_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, GENERAL_BLOCK);
            XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE,
                DateTimeUtils.MsecToClockString(processTime.General.TotalProcessTime, false));
            WriteElement(TOTAL_PROCESS_TIME, (processTime.General.TotalProcessTime / 1000.0).ToString("F", Nfi));
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out the WritingMode statistics.
        /// </summary>
        /// <param name="writingMode">WritingMode statistics to write out.</param>
        private void WriteWritingModeModule(SummaryAnalysisSummary.WritingModeClass writingMode)
        {
            XMLWriter.WriteStartElement(MODULE_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, WRITING_MODE_MODULE);

            foreach (var entry in writingMode.Entries)
            {
                var eventTypeName = char.ToUpper(entry.Key[0]) + entry.Key.Substring(1);
                XMLWriter.WriteStartElement(BLOCK_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, eventTypeName);
                XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, DateTimeUtils.MsecToClockString(entry.Value.TotalTime, false));
                WriteElement(TOTAL_TIME, (entry.Value.TotalTime / 1000.0).ToString("F", Nfi));
                var postFix = " " + eventTypeName;

                WriteElement(NUMBER_OF_CLUSTERS + postFix, entry.Value.NumberOfClusters.ToString("F", Nfi));
                WriteElement(AVG_TIME_CLUSTERS + postFix, (entry.Value.AvgTimeClusters / 1000.0).ToString("F", Nfi));
                WriteElement(MEDIAN_TIME_CLUSTERS + postFix, (entry.Value.MedianTimeClusters / 1000.0).ToString("F", Nfi));
                WriteElement(STANDARD_DEVIATION_CLUSTERS + postFix, entry.Value.StandardDeviationClusters.Equals(0) 
                    ? "": (entry.Value.StandardDeviationClusters/1000.0).ToString("F", Nfi));
                WriteElement(NUMBER_OF_MBURSTS + postFix, entry.Value.NumberOfSegments.ToString("F", Nfi));
                WriteElement(AVG_TIME_SEGMENTS + postFix, (entry.Value.AvgTimeSegments / 1000.0).ToString("F", Nfi));
                WriteElement(MEDIAN_TIME_SEGMENTS + postFix, (entry.Value.MedianTimeSegments / 1000.0).ToString("F", Nfi));
                WriteElement(STANDARD_DEVIATION_SEGMENTS + postFix, entry.Value.StandardDeviationSegments.Equals(0) 
                    ? "": (entry.Value.StandardDeviationSegments/1000.0).ToString("F", Nfi));
                foreach (var switchEntry in entry.Value.Switches)
                {
                    var switchEventTypeName = char.ToUpper(switchEntry.Key[0]) + switchEntry.Key.Substring(1);
                    WriteElement("Switches " + switchEventTypeName + " to " + eventTypeName, switchEntry.Value);
                }
                XMLWriter.WriteEndElement();
            }
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