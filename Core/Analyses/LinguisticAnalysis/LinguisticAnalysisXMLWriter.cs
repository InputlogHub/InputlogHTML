using System.Collections.Generic;
using InputLog.Core.IO;
using log4net;
using InputLog.Core.Analyses.Revision.Notations;

namespace InputLog.Core.Analyses.LinguisticAnalysis
{
    public class LinguisticAnalysisXMLWriter : WNotationAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the process analysis XML document.
        private const string STYLESHEET_HREF = "linguistic_analysis.xsl";
        #endregion

        #region Fields
        private string _processResult = "";
        private string _csvResult = "";
        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// Constructs a LinguisticAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public LinguisticAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath) { }

        /// <summary>
        /// Writes out the analysis document using a given LinguisticAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which
        ///  the Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is LinguisticAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type LinguisticAnalysisSummary");
            var summ = (LinguisticAnalysisSummary)summary;
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);

            //// Adding extra information regarding the external data used in this analysis.
            //// Debugging only.
            //_processResult = WordsReconstruction.Succeeded 
            //    ? "Data are processed correctly on " 
            //    : "This report contains incorrect data on ";
            //_csvResult = LT3Combination.CSVOut;

            ////extraInfo["Processing information"] = new Dictionary<string, object>();
            ////extraInfo["Processing information"]["Result "] = processResult;

            //Console.WriteLine("XMLSucceeded - " + _processResult + " " + DateTime.Now);
            //Console.WriteLine("Resulting CSV string: " + _csvResult);

            // Meta information.
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);

            //  Writes the final text retrieved from the document to the report, the S-Notation and the W-Notation.
            WriteMarkup(summ);

            // Text with all inserts added and all deletions removed.
            XMLWriter.WriteStartElement("reconstruct");
            XMLWriter.WriteString(summ.LinguisticProcessResults.Tables["textStrings"].Rows[0].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // Text showing only the inserts in the context of the normal production.
            XMLWriter.WriteStartElement("inserts");
            XMLWriter.WriteString(summ.LinguisticProcessResults.Tables["textStrings"].Rows[1].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // Text showing only deletions in the context of the normal production.
            XMLWriter.WriteStartElement("deletions");
            XMLWriter.WriteString(summ.LinguisticProcessResults.Tables["textStrings"].Rows[2].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // Linguistic analysis of the text enriched with timed information.
            summ.LinguisticProcessResults.WriteXml(XMLWriter);

            XMLWriter.WriteEndElement();

            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

    }
}