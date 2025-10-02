using System.Collections.Generic;
using System.Xml;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.IO;
using log4net;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    class SNotationAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "snotation_analysis.xsl";
        private const string MARKUP_TAG = "markup";
        private const string BREAK = "break";
        private const string TARGET = "target";

        // Event attributes
        private const string START_TIME_ATTRIBUTE = "startTime";
        private const string ACTION_TIME_TAG = "actionTime";
        private const string PAUSE_TIME_TAG = "pauseTime";

        public const string PAUSE_THRESHOLD_PARAMETER = "Pause Threshold (ms)";
        public const string TYPE_PARAMETER = "SNotation Analysis Type";
        #endregion

        #region Fields
        // Log4Net MessageLogger.
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public SNotationAnalysisXMLWriter(string destinationFilePath)
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
            if (!(summary is SNotationSummary))
                throw new AnalysisWriterException("Given summary is not of type SNotationSummary");
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteMarkup((SNotationSummary)summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

        /// <summary>
        /// Writes out the MarkupAnalysisEvents.
        /// </summary>
        /// <param name="summary">List of text and markups to write to the xml document</param>
        private void WriteMarkup(SNotationSummary summary)
        {
            XMLWriter.WriteStartElement(MARKUP_TAG);

            foreach (var symbol in summary.SymbolList)
            {
				var tmpString = StringUtils.ReplaceNonPrintableCharacters(symbol.Symbol);
                XMLWriter.WriteString(tmpString);
                if (symbol is MarkupNode && ((MarkupNode)symbol).SequenceNumber != null && ((MarkupNode)symbol).NumberedMarkup)
                {
                    var markup = (MarkupNode)symbol;
                    XMLWriter.WriteStartElement(summary.Syntax.Break.Equals(markup.Symbol) ? BREAK : TARGET);
                    XMLWriter.WriteString(((MarkupNode)symbol).SequenceNumber.ToString());
                    XMLWriter.WriteEndElement();
                }
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
            throw new System.NotImplementedException();
        }
    }
}