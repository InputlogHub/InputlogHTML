using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols
{
	abstract class OpenTagNode: MarkupNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
        public OpenTagNode(string symbol, AbstractRSnapshot snapshot, bool numberedMarkup)
            : base(symbol, snapshot, numberedMarkup) {}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public OpenTagNode()
        {
        }
	}

	class InsertOpenTag : OpenTagNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public InsertOpenTag(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
            : base(symbol, snapshot, numberedMarkup) {}

		/// <summary>
		/// InsertTags never jump!
		/// </summary>
		/// <param name="target">SymbolNode we wish to jump over.</param>
		/// <returns>True if this node can jump over the target, false if not.</returns>
		public override bool JumpsOver(SymbolNode target)
		{
			return false;
		}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public InsertOpenTag()
        {
        }
	}

	class DeleteOpenTag : OpenTagNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public DeleteOpenTag(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
			: base(symbol, snapshot, numberedMarkup) { }

		/// <summary>
		/// Delete open tag only jumps over an InsertionOpenTag, if the insertion revision related
		/// to that tag has no more active symbols left.
		/// 1. Jump over everything except nodes with PositionCount > 0 or InsertTags with more than 0 active characters.
		/// 2. Jump over empty SymbolNodes
		/// </summary>
		/// <param name="target">SymbolNode we wish to jump over.</param>
		/// <returns>True if this node can jump over the target, false if not.</returns>
		public override bool JumpsOver(SymbolNode target)
		{
			if (Revision == null) return false;

			if (//1.
				((target is InsertOpenTag || target is InsertCloseTag) && ((ProductionSnapshot)target.Revision).ActiveCharacters != 0) ||
				//2.
				(!(target is MarkupNode) && target.PositionCount != 0) ||
				//3. 
				(target is BreakNode && target.Revision.RevisionNumber >= this.Revision.RevisionNumber))
			{
				return false;
			}
			return true;
		}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public DeleteOpenTag()
        {
        }
    }
}
