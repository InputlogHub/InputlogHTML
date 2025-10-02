using System;
using System.Collections.Generic;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.WordPauses
{
    public class WordPausesAnalysisXMLWriter : WNotationAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the process analysis XML document.
        private const string STYLESHEET_HREF = "word_pause_analysis.xsl";
        #endregion

        /// <summary>
        /// Constructs a Word Pause Analysis XMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public WordPausesAnalysisXMLWriter(string destinationFilePath) : base(destinationFilePath)
        { }

        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string,
            IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is WordPausesAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type Word Pauses Analysis");
            var summ = (WordPausesAnalysisSummary) summary;
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            // Meta information.
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);

            try
            {
                // Adding extra information regarding the external *.csv file used in this analysis.
                extraInfo["Target file information"] = new Dictionary<string, object>();
                extraInfo["Target file information"]["Codes used in the 'Target' column: "] = "- 1 - The word matches exactly the first target and is also part of the particpant's target set."
                     + " - 2 - The word is a variant of the target."
                     + " - 3 - The word matches exactly the target but is part of another participant's target set."                         
                     + " - 4 - The word is identified with the Damerau-Levensthein method.";
                extraInfo["Target file information"]["Target file used: "] = WordPauseTargets.TargetFile;
                extraInfo["Target file information"]["Participant file used: "] = WordPauseTargets.ParticipantFile;
                extraInfo["Target file information"]["Total targets in the file: "]
                    = WordPauseTargets.MatchCount + WordPauseTargets.UnusedTargets.Count;
                var sbr = new StringBuilder();
                for (int index = 1; index < WordPauseTargets.ParticipantTargets.Length; index++)
                {
                    string target = WordPauseTargets.ParticipantTargets[index];
                    sbr.Append(target).Append(", ");
                }
                extraInfo["Target file information"]["Targets for this participant: "] = sbr.ToString()
                    .Substring(0, sbr.ToString().LastIndexOf(','));
                sbr.Clear();
                extraInfo["Target file information"]["Total targets matched: "] = WordPauseTargets.MatchCount;
                if (WordPauseTargets.MatchedTargets.Count == 0)
                {
                    sbr.Append("No matching targets found.,");
                }
                else
                {
                    foreach (string target in WordPauseTargets.MatchedTargets)
                    {
                        sbr.Append(target).Append(", ");
                    }
                }
                extraInfo["Target file information"]["Unique targets matched: "] = sbr.ToString()
                    .Substring(0, sbr.ToString().LastIndexOf(','));
                sbr.Clear();
                if (WordPauseTargets.UnusedTargets.Count == 0)
                {
                    sbr.Append("All targets found.,");
                }
                else
                {
                    foreach (string target in WordPauseTargets.UnusedTargets)
                    {
                        sbr.Append(target).Append(", ");
                    }
                }
                extraInfo["Target file information"]["Unused targets: "] = sbr.ToString()
                    .Substring(0, sbr.ToString().LastIndexOf(','));
                extraInfo["Target file information"]["Damerau-Levenshtein distance: "] = WordPauseTargets.WordDiff;

                WriteExtraInfo(extraInfo);
            }
            catch (Exception)
            {
                // ignored
            }

            //  Writes the final text retrieved from the document to the report, the S-Notation and the W-Notation.
            WriteMarkup(summ);

            // Text with all inserts added and all deletions removed.
            XMLWriter.WriteStartElement("reconstruct");
            XMLWriter.WriteString(summ.WordPauseResults.Tables["textStrings"].Rows[0].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // Text showing only the inserts in the context of the normal production.
            XMLWriter.WriteStartElement("inserts");
            XMLWriter.WriteString(summ.WordPauseResults.Tables["textStrings"].Rows[1].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // Text showing only deletions in the context of the normal production.
            XMLWriter.WriteStartElement("deletions");
            XMLWriter.WriteString(summ.WordPauseResults.Tables["textStrings"].Rows[2].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // The text enriched with timed information.
            summ.WordPauseResults.WriteXml(XMLWriter);

            XMLWriter.WriteEndElement();

            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }
    }
}