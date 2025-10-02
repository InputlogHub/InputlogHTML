using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    class RevisionMatrixAnalysisXMLWriter: AbstractAnalysisXMLWriter
    {
        #region Fields

        /// <summary>
        /// Location of the stylesheet for the analysis
        /// </summary>
        private const string STYLESHEET_HREF = "revisionmatrix_analysis.xsl";

        /// <summary>
        ///  Element names
        /// </summary>
        private const string SUMMARY_TAG = "summary";
        private const string REVISION_TAG = "revision";

        // R-Burst Constants
        private const string RBURST_TAG = "rbursts";
        private const string NUMBER_OF_RBURSTS = "Number Of R-Bursts";
        private const string MEAN_RBURST_TIME = "Mean RBurst Time (s)";
        private const string MEDIAN_RBURST_TIME = "Median R-Burst Time (s)";
        private const string STDEV_RBURST_TIME = "StDev R-Burst Time (s)";
        private const string MEAN_RBURST_CHARS = "Mean R-Burst Chars";
        private const string MEDIAN_RBURST_CHARS = "Median R-Burst Chars";
        private const string STDEV_RBURST_CHARS = "StDev R-Burst Chars";

        private const string REVISIONTYPE = "type";
        private const string REVISIONNUMBER = "revisionNumber";
        private const string REVISIONCONTENT = "content";
        private const string NUMBEROFEDITS = "edits";
        private const string REVISIONSTART = "start";
        private const string REVISIONEND = "end";
        private const string REVISIONDURATION = "duration";
        private const string REVISIONBEGINPOS = "beginPos";
        private const string REVISIONENDPOS = "endPos";
        private const string REVISIONLENGTH = "length";
        private const string REVISIONCHAR = "chars";
        private const string REVISIONCHARWOSPACE = "charWithoutSpace";
        private const string REVISIONWORDS = "words";

        // The last start time with a value > 0. Used to fill empty slots.
        private ulong LastStart;
        // The last end time with a value > 0. Used to fill empty slots.
        private ulong LastEnd;
        // Last duration seen. Used to fill empty slots.
        private ulong LastDuration;
        // Formatting a numerical value.
        private static readonly CultureInfo Nfi = new CultureInfo("en-US", false);
        #endregion

        public RevisionMatrixAnalysisXMLWriter(string destinationFilePath) :
            base(destinationFilePath)
        {
        }

        /// <summary>
        /// Write the revision matrix analysis XML document.
        /// </summary>
        /// <param name="sessionIdentification"></param>
        /// <param name="extraInfo"></param>
        /// <param name="summary"></param>
        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string,
           IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is RevisionMatrixSummary))
                throw new AnalysisWriterException("Given summary is not of type RevisionMatrixSummary");
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteMatrix((RevisionMatrixSummary) summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

        private void WriteMatrix(RevisionMatrixSummary summary)
        {
            foreach (RevisionMatrixEntry entry in summary.Entries)
            {
                bool isNormalProduction = entry.TypeOfRevision != null && entry.TypeOfRevision.Equals("Normal Production");

                XMLWriter.WriteStartElement(REVISION_TAG);
                // revision id
                XMLWriter.WriteStartElement(REVISIONNUMBER);
                XMLWriter.WriteString(isNormalProduction ? "0": entry.ID().ToString());
                XMLWriter.WriteEndElement();

                // revision type
                XMLWriter.WriteStartElement(REVISIONTYPE);
                XMLWriter.WriteString(isNormalProduction ? entry.TypeOfRevision : entry.RevisionTypeString());
                XMLWriter.WriteEndElement();

                // revision content
                XMLWriter.WriteStartElement(REVISIONCONTENT);
                XMLWriter.WriteString(isNormalProduction ? StringUtils.ConvertToReadable(entry.Text) : StringUtils.ConvertToReadable(entry.Content()));
                XMLWriter.WriteEndElement();

                // revision #edits
                XMLWriter.WriteStartElement(NUMBEROFEDITS);
                XMLWriter.WriteString(isNormalProduction ? entry.Edits.ToString() : entry.NumberOfEdits().ToString());
                XMLWriter.WriteEndElement();

                // revision start
                XMLWriter.WriteStartElement(REVISIONSTART);
                if (isNormalProduction)
                {
                    if (entry.TimeStart > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.TimeStart, true, false));
                     //   XMLWriter.WriteString((entry.TimeStart / 1000).ToString());
                        LastStart = entry.TimeStart;
                    }
                    else
                    {
                       XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastStart, true, false));
                       // XMLWriter.WriteString((LastStart / 1000).ToString());
                    }
                }
                else
                {
                    if (entry.StartTime() > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.StartTime(), true, false));
                     //   XMLWriter.WriteString((entry.StartTime() / 1000).ToString());
                        LastStart = entry.StartTime();
                    }
                    else
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastStart, true, false));
                      //  XMLWriter.WriteString((LastStart / 1000).ToString());
                    }
                }

                XMLWriter.WriteEndElement();

                // revision end
                XMLWriter.WriteStartElement(REVISIONEND);
                if (isNormalProduction)
                {
                    if (entry.TimeEnd > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.TimeEnd, true, false));
                     //   XMLWriter.WriteString((entry.TimeEnd/1000).ToString());
                        LastEnd = entry.TimeEnd;
                    }
                    else
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastEnd, true, false));
                      //  XMLWriter.WriteString((LastStart/1000).ToString());
                    }
                }
                else
                {
                    if (entry.EndTime() > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.EndTime(), true, false));
                      //  XMLWriter.WriteString((entry.EndTime()/1000).ToString());
                        LastEnd = entry.EndTime();
                    }
                    else
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastEnd, true, false));
                      //  XMLWriter.WriteString((LastEnd/1000).ToString());
                    }
                }
                XMLWriter.WriteEndElement();

                // duration
                XMLWriter.WriteStartElement(REVISIONDURATION);
                if (isNormalProduction)
                {
                    if (entry.RevisionDuration > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.RevisionDuration, true, false));
                        LastDuration = entry.RevisionDuration;
                    }
                    else
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastDuration, true, false));
                    }
                }
                else
                {
                    if (entry.Duration() > 0)
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(entry.Duration(), true, false));
                        LastDuration = entry.Duration();
                    }
                    else
                    {
                        XMLWriter.WriteString(DateTimeUtils.MsecToClockString(LastDuration, true, false));
                    }
                }

                XMLWriter.WriteEndElement();

                // length
                XMLWriter.WriteStartElement(REVISIONLENGTH);
                XMLWriter.WriteString(isNormalProduction ? entry.SizeOfRevision.ToString() : entry.RevisionSize().ToString());
                XMLWriter.WriteEndElement();

                // begin pos
                XMLWriter.WriteStartElement(REVISIONBEGINPOS);
                XMLWriter.WriteString(isNormalProduction ? entry.PosStart.ToString() : entry.StartPosition().ToString());
                XMLWriter.WriteEndElement();

                // end pos
                XMLWriter.WriteStartElement(REVISIONENDPOS);
                XMLWriter.WriteString(isNormalProduction ? entry.PosEnd.ToString() : entry.EndPosition().ToString());
                XMLWriter.WriteEndElement();

                // characters
                XMLWriter.WriteStartElement(REVISIONCHAR);
                XMLWriter.WriteString(isNormalProduction ? entry.Chars.ToString() : entry.Characters().ToString());
                XMLWriter.WriteEndElement();

                // characters without space
                XMLWriter.WriteStartElement(REVISIONCHARWOSPACE);
                XMLWriter.WriteString(isNormalProduction ? entry.CharsWithoutSpace.ToString() : entry.CharWithoutSpace().ToString());
                XMLWriter.WriteEndElement();

                // words
                XMLWriter.WriteStartElement(REVISIONWORDS);
                XMLWriter.WriteString(isNormalProduction ? entry.NumberOfWords.ToString() : entry.Words().ToString());
                XMLWriter.WriteEndElement();

                XMLWriter.WriteEndElement();
            }
            WriteModule(SUMMARY_TAG, () => WriteSummary(summary));
            WriteModule(RBURST_TAG, () => WriteRBurst(summary));
        }

        private void WriteSummary(RevisionMatrixSummary summary)
        {
            foreach (string t in summary.Summaries.Keys)
            {
                var t1 = t;
                bool skip = t1.Equals("Total Processing Time");

                WriteModuleBlock(t, delegate
                {
                    WriteModuleElement(
                        "Revisions", skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.REVISIONS_NR].ToString()
                    );

                    WriteModuleElement(
                        "Edits", skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.EDITS].ToString()
                    );
                    WriteModuleElement(
                        "Duration",
                        DateTimeUtils.MsecToClockString((ulong) summary.Summaries[t1][RevisionMatrixSummary.DURATION],
                            true, false)
                    );
                    WriteModuleElement(
                        "Length", skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.LENGTH].ToString()
                    );
                    WriteModuleElement(
                        "Chars", skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.CHARS].ToString()
                    );
                    WriteModuleElement(
                        "CharsWithoutSpace",
                        skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.CHARS_WITHOUT_SPACE].ToString()
                    );
                    WriteModuleElement(
                        "Words", skip ? "" : summary.Summaries[t1][RevisionMatrixSummary.WORDS].ToString()
                    );
                });
            }
        }

        /// <summary>
        /// Collecting R-Burst statistics. Timed data in seconds.
        /// </summary>
        private void WriteRBurst(RevisionMatrixSummary summary)
        {
            CultureInfo thisNfi = (CultureInfo)Nfi.Clone();
            thisNfi.NumberFormat.NumberDecimalDigits = 3;

            WriteModuleBlock(RBURST_TAG, delegate
            {
                WriteModuleElement(NUMBER_OF_RBURSTS, summary.NumberOfBursts.ToString());
                WriteModuleElement(MEAN_RBURST_TIME, summary.MeanRBurstTime.ToString("F", thisNfi));
                WriteModuleElement(MEDIAN_RBURST_TIME, summary.MedianRBurstTime.ToString());
                WriteModuleElement(STDEV_RBURST_TIME, summary.StdevRburstTime.ToString("F", thisNfi));
                WriteModuleElement(MEAN_RBURST_CHARS, summary.MeanRburstChars.ToString("F", thisNfi));
                WriteModuleElement(MEDIAN_RBURST_CHARS, summary.MedianRburstChars.ToString("F", thisNfi));
                WriteModuleElement(STDEV_RBURST_CHARS, summary.StdevRburstChars.ToString("F", thisNfi));
            });
        }


        /// <summary>
        /// Empty abstract method implementation.
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }
    }
}
