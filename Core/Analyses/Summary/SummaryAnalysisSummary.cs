using System;
using System.Collections.Generic;
using System.Globalization;
using InputLog.Core.Reporting;

namespace InputLog.Core.Analyses.Summary
{
    public class SummaryAnalysisSummary : AbstractAnalysisSummary
    {
        public class ProductInformationClass
        {
            /// <summary>
            /// Count of characters excluding spaces.
            /// </summary>
            public int CharCountWithoutSpaces;
            /// <summary>
            /// Count of characters including spaces.
            /// </summary>
            public int CharCountWithSpaces;
            /// <summary>
            /// Count of characters including spaces in the document before logging.
            /// </summary>
            public int StartCharCountWithSpaces;
            /// <summary>
            /// Count of characters excluding spaces in the document before logging.
            /// </summary>
            public int StartCharCountWithoutSpaces;
            /// <summary>
            /// Count of Far East characters.
            /// </summary>
            // public int FarEastCharCount;
            /// <summary>
            /// Count of lines.
            /// </summary>
            public int LineCount;
            /// <summary>
            /// Count of pages.
            /// </summary>
            public int PageCount;
            /// <summary>
            /// Count of paragraphs.
            /// </summary>
            public int ParagraphCount;
            /// <summary>
            /// Count of words.
            /// </summary>
            public int WordCount;
            /// <summary>
            /// Count of words in the document before logging.
            /// </summary>
            public int StartWordCount;
            /// <summary>
            /// True if the stats were found in the logging, false if not.
            /// </summary>
            public bool StatsFound;
        }

        public class ProcessInformationClass
        {
            public class WordsClass
            {
                public int TotalCharsProduced;
                public int TotalCharsCopied;
                public int TotalCharsReplaced;
                public int FormattingKeys;
                public ulong TotalCharactersWithoutSpaces;
                public ulong TotalCharactersWithSpaces;
                public ulong TotalWords;
                public double AvgWordLength;
                public double StandardDeviation;
                public double MedianWordLength;
            }

            public class SentencesClass
            {
                public ulong TotalSentences;
                public double AvgCharactersPerSentence;
                public double StandardDeviationCs;
                public double AvgWordsPerSentence;
                public double StandardDeviationWs;
                public double MedianCharactersPerSentence;
                public double MedianWordsPerSentence;
            }

            public class ParagraphsClass
            {
                public ulong TotalParagraphs;
                public double AvgCharactersPerParagraph;
                public double StandardDeviationCp;
                public double AvgWordsPerParagraph;
                public double StandardDeviationWp;
                public double AvgSentencePerParagraph;
                public double StandardDeviationSp;
                public double MedianCharactersPerParagraph;
                public double MedianWordsPerParagraph;
                public double MedianSentencePerParagraph;
            }

            public readonly WordsClass Words = new WordsClass();
            public readonly SentencesClass Sentences = new SentencesClass();
            public readonly ParagraphsClass Paragraphs = new ParagraphsClass();
        }

        public class ProductProcessRatioClass
        {
            /// <summary>
            /// Ratio of characters excluding spaces.
            /// </summary>
            public double CharWithoutSpacesRatio;
            /// <summary>
            /// Ratio of characters including spaces.
            /// </summary>
            public double CharWithSpacesRatio;
            /// <summary>
            /// Ratio of total chars produced (incl. copy)
            /// </summary>
            public double ProdWithSpacesRatio;
            /// <summary>
            /// Ratio of words.
            /// </summary>
            public double WordRatio;
        }

        public class ProcessTimeClass
        {
            public class GeneralClass
            {
                public ulong NumberOfSegments;
                public ulong TotalProcessTime;
                public ulong AvgProcessTime;
                public double StandardDeviation;
                public double AvgProcessChars;
                public double StandardDeviationChars;
                public double MedianProcessTime;
                public double MedianProcessChars;
                public double PBurstsPerMinute;
            }

            public readonly GeneralClass General = new GeneralClass();
        }

        public class WritingModeClass
        {
            public class WritingModeEntryClass
            {
                public ulong TotalTime;
                public ulong NumberOfClusters;
                public ulong AvgTimeClusters;
                public double StandardDeviationClusters;
                public ulong NumberOfSegments;
                public ulong AvgTimeSegments;
                public double StandardDeviationSegments;

                public readonly IDictionary<string, ulong> Switches = new Dictionary<string, ulong>();
                public double MedianTimeClusters;
                public double MedianTimeSegments;
            }

            public readonly IDictionary<string, WritingModeEntryClass> Entries = new Dictionary<string, WritingModeEntryClass>();
        }

        /// <summary>
        /// Calculate the number of characters - with spaces - produced per minute. This
        /// looks at the entire process.
        /// </summary>
        public double CharactersProcessPerMinute
        {
            get
            {
                var totalTimeInMilliseconds = this.ProcessTime.General.TotalProcessTime;
                var totalTimeInMinutes = TimeSpan.FromMilliseconds(totalTimeInMilliseconds).TotalMinutes;
                return (this.ProcessInformation.Words.TotalCharactersWithSpaces / totalTimeInMinutes);
            }
        }

        /// <summary>
        /// Calculate the number of characters - with spaces -  per minute. This
        /// looks at the entire process.
        /// </summary>
        public double CharactersProductPerMinute
        {
            get
            {
                var totalTimeInMilliseconds = this.ProcessTime.General.TotalProcessTime;
                var totalTimeInMinutes = TimeSpan.FromMilliseconds(totalTimeInMilliseconds).TotalMinutes;
                return (this.ProductInformation.CharCountWithSpaces / totalTimeInMinutes);
            }
        }

        /// <summary>
        ///     Returns total process time in minutes (double)
        /// </summary>
        public double ProcessTimeInMinutes
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(this.ProcessTime.General.TotalProcessTime);
                return ts.TotalMinutes;
            }
        }

        /// <summary>
        ///     Returns total process time in millisecs
        /// </summary>
        public double ProcessMillis
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(this.ProcessTime.General.TotalProcessTime);
                return ts.TotalMilliseconds;
            }
        }


        public readonly ProductInformationClass ProductInformation = new ProductInformationClass();
        public readonly ProcessInformationClass ProcessInformation = new ProcessInformationClass();
        public readonly ProductProcessRatioClass ProductProcessRatio = new ProductProcessRatioClass();
        public readonly ProcessTimeClass ProcessTime = new ProcessTimeClass();
        public readonly WritingModeClass WritingMode = new WritingModeClass();


        /*
         * IMPORTANT NOTE: 
         * These methods are being referenced in the reporting functionality, by name!
         * Do not change these method names without changing the references
         * in the required resource files as well.
         * 
         * Resource File: Core.Reporting.Resources.ReportMappingResources.resx
         * 
         * Note: The report_ prefix must be kept! The methods are also reflectively discovered
         * in the GetBoundTargets() method of the AbstractAnalysisSummary class!
         */
        #region Reporting Methods
        CultureInfo culture = new CultureInfo("en-US", false);

        public ReportValue report_Process_NrOfWords()
        {
            const string RESOURCE_ID = "Summary_Process_NrOfWords";
            return new LabeledValue(
                RESOURCE_ID,
                this.ProcessInformation.Words.TotalWords.ToString()
            );
        }

        public ReportValue report_Process_CharactersPerMinute()
        {
            const string RESOURCE_ID = "Summary_Process_CPM";
            return new LabeledValue(
                RESOURCE_ID,
                this.CharactersProcessPerMinute.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_Product_NrOfWords()
        {
            const string RESOURCE_ID = "Summary_Product_NrOfWords";
            return new LabeledValue(
                RESOURCE_ID,
                this.ProductInformation.WordCount.ToString()
            );
        }


        public ReportValue report_Product_CharactersPerMinute()
        {
            const string RESOURCE_ID = "Summary_Product_CPM";
            return new LabeledValue(
                RESOURCE_ID,
                this.CharactersProductPerMinute.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_Process_TotalTime()
        {
            const string RESOURCE_ID = "Summary_Process_TotalTime";
            TimeSpan ts = TimeSpan.FromMilliseconds(this.ProcessTime.General.TotalProcessTime);
            return new LabeledValue(
                RESOURCE_ID,
                ts.ToString(@"hh\:mm\:ss")
            );
        }
        
        public ReportValue report_Process_WritingPausing()
        {
            const string RESOURCE_ID = "Summary_Process_WritingPausing";
            double wp = ProcessTime.General.AvgProcessChars;
            return new LabeledValue(RESOURCE_ID, wp.ToString("F", culture.NumberFormat));
         
        }

        public ReportValue report_Product_CharInclSpaces()
        {
            const string RESOURCE_ID = "Summary_Product_CharactersInclSpaces";
            return new LabeledValue(
                RESOURCE_ID,
                this.ProductInformation.CharCountWithSpaces.ToString()
            );
        }

        public ReportValue report_Process_CharInclSpaces()
        {
            const string RESOURCE_ID = "Summary_Process_CharactersInclSpaces";
            return new LabeledValue(
                RESOURCE_ID,
                this.ProcessInformation.Words.TotalCharactersWithSpaces.ToString()
            );
        }

        public ReportValue report_Process_WPM()
        {
            const string RESOURCE_ID = "Summary_Process_WPM";
            double WPM = this.ProcessInformation.Words.TotalWords / this.ProcessTimeInMinutes;
            return new LabeledValue(RESOURCE_ID, WPM.ToString("F", culture.NumberFormat));
        }

        public ReportValue report_Product_TotalWords()
        {
            const string RESOURCE_ID = "Summary_Product_TotalWords";
            return new LabeledValue(RESOURCE_ID, this.ProductInformation.WordCount.ToString());
        }

        public ReportValue report_Product_WPM()
        {
            const string RESOURCE_ID = "Summary_Product_WPM";
            double WPM = ProductInformation.WordCount / this.ProcessTimeInMinutes;
            return new LabeledValue(RESOURCE_ID, WPM.ToString("F", culture.NumberFormat));
        }


        public ReportValue report_Process_AvgWordLength()
        {
            const string RESOURCE_ID = "Summary_Process_AverageWordLength";
            return new LabeledValue(
                RESOURCE_ID, 
                this.ProcessInformation.Words.AvgWordLength.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_Process_AvgWordLength_STDEV()
        {
            const string RESOURCE_ID = "Summary_Process_AverageWordLength_STDEV";
            return new LabeledValue(
                RESOURCE_ID, 
                this.ProcessInformation.Words.StandardDeviation.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_RevisionProportionInCharacters()
        {
            const string RESOURCE_ID = "Summary_RevisionProportionInChar";
            double ratio = this.ProductProcessRatio.CharWithSpacesRatio;
            return new LabeledValue(
                RESOURCE_ID,
                String.Format("{0}%", ratio.ToString("F", culture.NumberFormat))
            );
        }

        public ReportValue report_RevisionRatioInCharacters()
        {
            const string RESOURCE_ID = "Summary_RevisionRatioInChar";
            double ratio = this.ProductProcessRatio.ProdWithSpacesRatio * 100.0;
            return new LabeledValue(
                RESOURCE_ID,
                String.Format("{0}%", ratio.ToString("F", culture.NumberFormat))
            );
        }
        #endregion
    }
}