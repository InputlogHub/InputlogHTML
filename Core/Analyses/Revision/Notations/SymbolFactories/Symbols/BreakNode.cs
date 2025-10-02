using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols
{
	class BreakNode: MarkupNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public BreakNode(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
			: base(symbol, snapshot, numberedMarkup) { }

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public BreakNode()
        {
        }

		/// <summary>
		/// BreakNodes can only jump over other break nodes, if the others have a lower
		/// revision number than the current breaknode.
		/// </summary>
		/// <param name="target">SymbolNode we wish to jump over.</param>
		/// <returns>True if this node can jump over the target, false if not.</returns>
		public override bool JumpsOver(SymbolNode target)
		{
			if (Revision == null) return false;
			return target is BreakNode && target.Revision.RevisionNumber < Revision.RevisionNumber;
		}
	}
}
