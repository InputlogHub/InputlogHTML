using System;
using System.Collections.Generic;
using System.Xml;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.IO;
using log4net;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    public class WNotationAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the process analysis XML document.
        private const string STYLESHEET_HREF = "linguistic_analysis.xsl";
        private const string WNOTATION_TAG = "wnotation";
        private const string FINAL_TAG = "final";
        private const string BREAK = "break";
        private const string TARGET = "target";
        private const string MARKUP_TAG = "markup";

        #endregion

        #region Fields
        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.
            GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        protected WNotationAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath) { }

        /// <summary>
        /// WriteDocument
        /// </summary>
        /// <param name="sessionIdentification"></param>
        /// <param name="extraInfo"></param>
        /// <param name="summary"></param>
        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string,
            IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is WNotationSummary))
                throw new AnalysisWriterException("Given summary is not of type WNotationSummary");
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteMarkup((WNotationSummary)summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

        /// <summary>
        /// Writes out the MarkupAnalysisEvents.
        /// </summary>
        /// <param name="summary">List of text and markups to write to the xml document</param>
        protected void WriteMarkup(WNotationSummary summary)
        {
            // Writes the final text retrieved from the document to the report.
            var fDoc = summary.FullText;
            WriteFinal(fDoc);

            // Writes first the string in the canonical S-Notation format.
            var symbols = summary.SNotationList;
            WriteSNotation(symbols);

            // The string in the W-Notation format.
            symbols = summary.SymbolList;
            WriteWNotation(symbols);
        }

        /// <summary>
        /// Writing the final version as saved in the document
        /// </summary>
        /// <param name="final">String with the text from the document</param>
        private void WriteFinal(string final)
        {
            XMLWriter.WriteStartElement(FINAL_TAG);
            XMLWriter.WriteString(final);
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Rendering the S-Notation version
        /// </summary>
        /// <param name="symbolList">LinkedList with text and markups</param>
        private void WriteSNotation(IEnumerable<SymbolNode> symbolList)
        {
            XMLWriter.WriteStartElement(MARKUP_TAG);

            foreach (var symbol in symbolList)
            {
                if (symbol is BreakNode)
                {
                    XMLWriter.WriteString(symbol.ToString().Substring(0, 1));
                    XMLWriter.WriteStartElement(BREAK);
                    XMLWriter.WriteString(symbol.ToString().Substring(1));
                    XMLWriter.WriteEndElement();
                }
                else if (symbol is DeleteCloseTag || symbol is InsertCloseTag)
                {
                    XMLWriter.WriteString(symbol.ToString().Substring(0, 1));
                    XMLWriter.WriteStartElement(TARGET);
                    XMLWriter.WriteString(symbol.ToString().Substring(1));
                    XMLWriter.WriteEndElement();
                }
                else
                {
                    var tmpString = StringUtils.ReplaceNonPrintableCharacters(symbol.Symbol);
                    XMLWriter.WriteString(tmpString);
                }
            }

            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Rendering the W-Notation version
        /// </summary>
        /// <param name="symbolList">LinkedList with text and markups</param>
        private void WriteWNotation(IEnumerable<SymbolNode> symbolList)
        {
            XMLWriter.WriteStartElement(WNOTATION_TAG);
            foreach (var symbol in symbolList)
            {
				XMLWriter.WriteString(StringUtils.ReplaceNonPrintableCharacters(symbol.Symbol));
            }
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