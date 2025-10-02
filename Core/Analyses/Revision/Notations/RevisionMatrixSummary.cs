using System.Collections.Generic;
using System.Globalization;
using InputLog.Core.Reporting;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class RevisionMatrixSummary : AbstractAnalysisSummary
    {
        /// <summary>
        ///     Create a revsision matrix summary
        /// </summary>
        public RevisionMatrixSummary()
        {
            Entries = new List<RevisionMatrixEntry>();
            _culture = new CultureInfo("en-US") {NumberFormat = {NumberDecimalDigits = 2}};
        }

        /// <summary>
        ///     Add an entry to the revision matrix.
        /// </summary>
        /// <param name="entry">Entry to be added</param>
        public void Add(RevisionMatrixEntry entry)
        {
            if (entry != null)
            {
                Entries.Add(entry);
            }
        }

        #region Fields

        /// <summary>
        ///     List of all the revision entries in the matrix. The oldest revision
        ///     are at the front of the list, and the newer revision are appended to the
        ///     end of the list.
        /// </summary>
        public List<RevisionMatrixEntry> Entries { get; }

        /// <summary>
        ///     Summaries of edits, durations etc over the different types of analysies
        /// </summary>
        public Dictionary<string, int[]> Summaries;


        // Summary indices
        public const int REVISIONS_NR = RevisionMatrixAnalysis.RevisionsNr;
        public const int EDITS = RevisionMatrixAnalysis.Edits;
        public const int DURATION = RevisionMatrixAnalysis.Duration;
        public const int LENGTH = RevisionMatrixAnalysis.Length;
        public const int CHARS = RevisionMatrixAnalysis.Chars;
        public const int CHARS_WITHOUT_SPACE = RevisionMatrixAnalysis.CharsWithoutSpace;
        public const int WORDS = RevisionMatrixAnalysis.Words;

        // Keys for types of revisions in summary
        public const string ALL_REVISIONS = "Production + Revisions";
        public const string DELETE_REVISIONS = RevisionMatrixEntry.DELETE_REVISION;
        public const string NORMAL_REVISIONS = RevisionMatrixEntry.NORMAL_REVISION;
        public const string INSERT_REVISIONS = RevisionMatrixEntry.INSERT_REVISION;

        // R-Burst data
        // Times are in seconds
        public int NumberOfBursts;
        public double MeanRBurstTime;
        public double MedianRBurstTime;
        public double StdevRburstTime;
        public double MeanRburstChars;
        public double MedianRburstChars;
        public double StdevRburstChars;


        private readonly CultureInfo _culture;

        #endregion

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

        #region Reporting methods

        public ReportValue report_RevisionsPer100Words()
        {
            var producedWords = Summaries[ALL_REVISIONS][WORDS] - Summaries[DELETE_REVISIONS][WORDS];
            var nrOfRevisions =
                (Summaries.ContainsKey(DELETE_REVISIONS) ? Summaries[DELETE_REVISIONS][REVISIONS_NR] : 0) +
                (Summaries.ContainsKey(INSERT_REVISIONS) ? Summaries[INSERT_REVISIONS][REVISIONS_NR] : 0);

            var percentage = nrOfRevisions != 0 ? (double) nrOfRevisions / producedWords *100 : 0;

            const string RESOURCE_ID = "Revision_RevisionsPer100Words";
            return new LabeledValue(
                RESOURCE_ID,
                percentage.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_NrDeletions()
        {
            var nrOfDeletions =
                Summaries.ContainsKey(DELETE_REVISIONS)
                    ? Summaries[DELETE_REVISIONS][REVISIONS_NR]
                    : 0;

            const string RESOURCE_ID = "Revision_NrOfDeletions";
            return new LabeledValue(
                RESOURCE_ID,
                nrOfDeletions.ToString()
            );
        }

        public ReportValue report_NrInsertions()
        {
            var nrOfInsertions =
                Summaries.ContainsKey(INSERT_REVISIONS)
                    ? Summaries[INSERT_REVISIONS][REVISIONS_NR]
                    : 0;

            const string RESOURCE_ID = "Revision_NrOfInsertions";
            return new LabeledValue(
                RESOURCE_ID,
                nrOfInsertions.ToString()
            );
        }

        public ReportValue report_CummulativeDeletionLength()
        {
            var length =
                Summaries.ContainsKey(DELETE_REVISIONS)
                    ? Summaries[DELETE_REVISIONS][LENGTH]
                    : 0;

            const string RESOURCE_ID = "Revision_CummulativeDeletionLength";
            return new LabeledValue(
                RESOURCE_ID,
                length.ToString()
            );
        }

        public ReportValue report_AverageDeletionLength()
        {
            var length =
                Summaries.ContainsKey(DELETE_REVISIONS)
                    ? Summaries[DELETE_REVISIONS][LENGTH]
                    : 0;
            var nrOfDeletions =
                Summaries.ContainsKey(DELETE_REVISIONS)
                    ? Summaries[DELETE_REVISIONS][REVISIONS_NR]
                    : 0;
            var ratio =
                nrOfDeletions != 0
                    ? length/(double) nrOfDeletions
                    : 0;

            const string RESOURCE_ID = "Revision_AverageDeletionLength";
            return new LabeledValue(
                RESOURCE_ID,
                ratio.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_CummulativeInsertionLength()
        {
            var length =
                Summaries.ContainsKey(INSERT_REVISIONS)
                    ? Summaries[INSERT_REVISIONS][LENGTH]
                    : 0;

            const string RESOURCE_ID = "Revision_CummulativeInsertionLength";
            return new LabeledValue(
                RESOURCE_ID,
                length.ToString()
            );
        }

        public ReportValue report_AverageInsertionLength()
        {
            var length =
                Summaries.ContainsKey(INSERT_REVISIONS)
                    ? Summaries[INSERT_REVISIONS][LENGTH]
                    : 0;
            var nrOfInsertions =
                Summaries.ContainsKey(INSERT_REVISIONS)
                    ? Summaries[INSERT_REVISIONS][REVISIONS_NR]
                    : 0;
            var ratio =
                nrOfInsertions != 0
                    ? length/(double) nrOfInsertions
                    : 0;

            const string RESOURCE_ID = "Revision_AverageInsertionLength";
            return new LabeledValue(
                RESOURCE_ID,
                ratio.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_RburstNr()
        {
            const string RESOURCE_ID = "Revision_Rburst_Nr";
            return new LabeledValue(
                RESOURCE_ID,
                NumberOfBursts.ToString()
            );
        }

        public ReportValue report_Rburst_AvgTime()
        {
            const string RESOURCE_ID = "Revision_Rburst_AvgTime";
            return new LabeledValue(
                RESOURCE_ID,
                MeanRBurstTime.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Rburst_StdevTime()
        {
            const string RESOURCE_ID = "Revision_Rburst_StdevTime";
            return new LabeledValue(
                RESOURCE_ID,
                StdevRburstTime.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Rburst_MedianTime()
        {
            const string RESOURCE_ID = "Revision_Rburst_MedianTime";
            return new LabeledValue(
                RESOURCE_ID,
                MedianRBurstTime.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Rburst_AvgProduction()
        {
            const string RESOURCE_ID = "Revision_Rburst_AvgProduction";
            return new LabeledValue(
                RESOURCE_ID,
                MeanRburstChars.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Rburst_StdevProduction()
        {
            const string RESOURCE_ID = "Revision_Rburst_StdevProduction";
            return new LabeledValue(
                RESOURCE_ID,
                StdevRburstChars.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_Rburst_MedianProduction()
        {
            const string RESOURCE_ID = "Revision_Rburst_MedianProduction";
            return new LabeledValue(
                RESOURCE_ID,
                MedianRburstChars.ToString("F", _culture.NumberFormat)
            );
        }

        public ReportValue report_RevisionsPerMinute()
        {
            var nrOfDeletions =
                Summaries.ContainsKey(DELETE_REVISIONS)
                    ? Summaries[DELETE_REVISIONS][REVISIONS_NR]
                    : 0;
            var nrOfInsertions =
                Summaries.ContainsKey(INSERT_REVISIONS)
                    ? Summaries[INSERT_REVISIONS][REVISIONS_NR]
                    : 0;
            var totalNrRevisions = nrOfDeletions + nrOfInsertions;
            var rpm = totalNrRevisions/RevisionMatrixAnalysis.TotalProcessTime;

            const string RESOURCE_ID = "Revision_RevisionsPerMinute";
            return new LabeledValue(
                RESOURCE_ID,
                rpm.ToString("F", _culture.NumberFormat)
            );
        }

        #endregion
    }
}