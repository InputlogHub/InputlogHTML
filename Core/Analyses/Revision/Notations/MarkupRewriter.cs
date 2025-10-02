using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;

namespace InputLog.Core.Analyses.Revision.Notations
{
    /// <summary>
    ///  Changes W-Notation markup symbols into S-Notation to enhance readability of the W-Notation report.
    /// </summary>  
    internal class MarkupRewriter
    {
        /// <summary>
        /// List of the markup symbols and text in this summary.
        /// </summary>
        private static LinkedList<SymbolNode> SymbolList { get; set; }

        /// <summary>
        /// Iterating over the W-Notation symbols to buid a new list with S-Notation markup
        /// that enhances the readability of the W-Notation report.
        /// </summary>
        /// <param name="symbols">List with the text and the W-Notation markup symbols</param>
        /// <returns>New list with S-Notation markup symbols.</returns>
        public static LinkedList<SymbolNode> RewriteSymbolList(IEnumerable<SymbolNode> symbols)
        {
            SymbolList = new LinkedList<SymbolNode>();

            foreach (var symbol in symbols)
            {
                var rewrite = symbol;

                if (symbol.PositionCount == 0)
                {
                    rewrite = NewNode(symbol.Symbol, symbol.Revision.RevisionNumber);
                }
                SymbolList.AddLast(rewrite);
            }
            return SymbolList;
        }

        /// <summary>
        /// Rewrites a W-Notation markup symbol with its S-Notation equivalent. (Note: this
        /// doesn't utilise the RevisionSnapshots, as it just changes symbols in a pre-constructed
        /// notation.)
        /// </summary>
        /// <param name="m">String with the markup.</param>
        /// <param name="revNumber"></param>
        /// <returns>A Markup node with S-Notation replacing the W-Notation symbol.</returns>
        private static MarkupNode NewNode(string m, int? revNumber = null)
        {
            // No sequence number needed here.
            if (m.Contains("{")) return new InsertOpenTag("{");
            if (m.Contains("[")) return new DeleteOpenTag("[");

            // Getting the sequence number first for the other cases.
            var seqNumber = new StringBuilder(m.Length);
            foreach (var cr in m.Where(Char.IsDigit))
            {
                seqNumber.Append(cr);
            }

            // Checking and returning the markup node.
            if (m.Contains("^")) return new BreakNode("|" + 
                ((revNumber == null) ? int.Parse(seqNumber.ToString()) : (int)revNumber));
            if (m.Contains("}")) return new InsertCloseTag("}" + 
                ((revNumber == null) ? int.Parse(seqNumber.ToString()) : (int)revNumber));
            return m.Contains("]") ? new DeleteCloseTag("]" + 
                ((revNumber == null) ? int.Parse(seqNumber.ToString()) : (int)revNumber)) : new MarkupNode(m);
        }
    }
}
