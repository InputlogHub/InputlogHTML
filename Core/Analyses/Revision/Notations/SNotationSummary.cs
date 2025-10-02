using System;
using System.Collections.Generic;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Reporting;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    /// <summary>
    /// A linkedList with the text and its revisions represented as inserts, deletions, and breaks according to the
    /// S-Notation conventions.
    /// </summary>
    public class SNotationSummary : AbstractAnalysisSummary
    {
        /// <summary>
        /// List of the markup symbols and text in this summary.
        /// </summary>
        public LinkedList<SymbolNode> SymbolList { get; private set; }

        /// <summary>
        /// The syntax used when building the notation.
        /// </summary>
        public ISymbolFactory Syntax { get; private set; }

        /// <summary>
        /// Constructs a new S-Notation Summary.
        /// </summary>
        /// <param name="symbols">The list with the symbols.</param>
        /// <param name="syntax">The syntax used.</param>
        public SNotationSummary(LinkedList<SymbolNode> symbols, ISymbolFactory syntax)
        {
            SymbolList = symbols;
            Syntax = syntax;
        }

        /// <summary>
        ///     Converts the SymbolList into a string with SNotation markup (subscripts
        ///     and superscripts where appropriate)
        /// </summary>
        /// <returns>A string containing the snotation with required suscript
        /// and superscript markup</returns>
        private string GetSNotationString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SymbolNode symbol in SymbolList)
            {
                string symbolContent = StringUtils.ReplaceNonPrintableCharacters(symbol.Symbol);
                sb.Append(symbolContent);

                if (symbol is MarkupNode &&
                    ((MarkupNode)symbol).SequenceNumber != null &&
                    ((MarkupNode)symbol).NumberedMarkup)
                {
                    var markup = (MarkupNode)symbol;
                    if (this.Syntax.Break.Equals(markup.Symbol))
                    {
                        // Break: should be subscript
                        string number = markup.SequenceNumber.ToString();
                        number = number.ToUnicodeSubScript();
                        sb.Append(number);
                    }
                    else
                    {
                        // Target should be superscript
                        string number = markup.SequenceNumber.ToString();
                        number = number.ToUnicodeSuperScript();
                        sb.Append(number);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Get the SNotation result as a unicode string.
        /// </summary>
        public string SNotationString
        {
            get
            {
                return GetSNotationString();
            }
        }

        ///
        /// Reporting methods.
        ///
        #region Reporting

        public ReportValue report_SNotation_Linear_Text()
        {
            const string REPORT_ID = "SNotation_Linear_Text";
            try
            {
                // replace middle dot by space.
                string altered = this.SNotationString.Replace((char)183, (char)32);
                return new LabeledValue( REPORT_ID, altered);
            }
            catch (Exception)
            {
                return new LabeledValue(REPORT_ID, "Error: could not construct snotation");
            }
        }


        #endregion
    }
}
