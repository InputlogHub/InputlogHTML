using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    class TokenAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "token_analysis.xsl";
        private const string BIGRAM_TAG = "bigram";
        private const string CHAR_TAG = "digr";
        private const string TIME_TAG = "pause";
        private const string KEY_ATTR = "key";
        private const string DATA_TAG = "bigrams";
        private const string BREAK = "break";
        private const string TARGET = "target";
        private const string MARKUP_TAG = "markup";
        private string User;
    
        /// <summary>
        /// Constructs a TokenAnalysisXMLWriter
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument will be saved.</param>
        public TokenAnalysisXMLWriter(string destinationFilePath) : base(destinationFilePath)
        {}

        public override void WriteDocument(SessionIdentification sessionIdentification, 
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is TokenAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type TokenAnalysisSummary");
            User = sessionIdentification.GetParticipant();
            var summ = (TokenAnalysisSummary)summary;

            // Adding extra information regarding the external *.csv file used in this analysis.
            extraInfo["Target file information"] = new Dictionary<string, object>();
            extraInfo["Target file information"]["CSV file used: "] = TokenTarget.CsvFile;
            extraInfo["Target file information"]["Total Targets in the file: "] 
                = TokenTarget.MatchCount + TokenTarget.MissedTargets.Count;
            extraInfo["Target file information"]["Matched Targets: "] = TokenTarget.MatchCount;
            var sb = new StringBuilder();
            if (TokenTarget.MissedTargets.Count == 0)
            {
                sb.Append("All targets found.,");
            }
            else
            {
                foreach (string target in TokenTarget.MissedTargets)
                {
                    sb.Append(target).Append(", ");
                }
            }
            extraInfo["Target file information"]["Missed Targets: "] = sb.ToString().Substring(0, sb.ToString().LastIndexOf(','));
            extraInfo["Target file information"]["Damerau-Levenshtein Distance: "] = TokenTarget.WordDiff;

            // Meta info
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);

            // The reconstructed text
            XMLWriter.WriteStartElement("reconstruct");
            XMLWriter.WriteString(summ.LinguisticProcessResults.Tables["textStrings"].Rows[0].ItemArray[1].ToString());
            XMLWriter.WriteEndElement();

            // The S-Notation
            WriteMarkup(summ);

            // The bigrams
            WriteBigrams(summ.LinguisticProcessResults.Tables["bigrams"]);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

        /// <summary>
        /// Writes out the MarkupAnalysisEvents.
        /// </summary>
        /// <param name="summary">List of text and markups to write to the xml document</param>
        private void WriteMarkup(TokenAnalysisSummary summary)
        {
            var symbols = summary.SNotations;
            WriteSNotation(symbols);
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
                else if(symbol is DeleteCloseTag || symbol is InsertCloseTag)
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
        /// Writing the XML for the bigram table.
        /// </summary>
        /// <param name="bigramTable">DataTable with words and their bigram information</param>
        private void WriteBigrams(DataTable bigramTable)
        {
            // Finding the maximum bigram length.
            int maxCol = bigramTable.Columns.Count;

            // Max. number of bigrams
            int biCount = (maxCol - 5) / 2;

            foreach (DataRow row in bigramTable.Rows)
            {
                XMLWriter.WriteStartElement(DATA_TAG); 

                // Fixed columns
                WriteIfValueNotNull("name", User);
                WriteIfValueNotNull("target", row["Target"]);
                WriteIfValueNotNull("produced", row["Produced"]);
                WriteIfValueNotNull("s_notation", row["S-Notation"]);
                WriteIfValueNotNull("revision", row["Revisions"]);

                // Variable columns
                for (var i = 0; i < biCount; i++)
                {
                    XMLWriter.WriteStartElement(BIGRAM_TAG);
                    XMLWriter.WriteAttributeString(KEY_ATTR, i.ToString());
                   
                    WriteIfValueNotNull(CHAR_TAG, row["digr_"+ i]);
                    WriteIfValueNotNull(TIME_TAG, row["pause_" + i]);

                    XMLWriter.WriteEndElement();
                }

                XMLWriter.WriteEndElement();
            }
        }

        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new System.NotImplementedException();
        }
    }
}
