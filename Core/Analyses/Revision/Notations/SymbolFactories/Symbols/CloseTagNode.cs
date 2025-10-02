using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols
{
	abstract class CloseTagNode: MarkupNode
	{
		/// <summary>
		/// Returns whether or not the markup in this node should be numbered markup or not. E.g. in 
		/// S-Notation the end of a delete or insert has it's sequence number following it, thus it is
		/// a numbered markup, likewise for the break.
		/// </summary>
		public override bool NumberedMarkup { get { return true; } }

		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public CloseTagNode(string symbol, AbstractRSnapshot snapshot, bool numberedMarkup)
            : base(symbol, snapshot, numberedMarkup) {}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public CloseTagNode()
        {
        }
	}

	class InsertCloseTag : CloseTagNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
        /// <param name="seqnr">The sequence number of the break node (if there is one).</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public InsertCloseTag(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
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
        public InsertCloseTag()
        {
        }
	}

	class DeleteCloseTag: CloseTagNode
	{
		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		public DeleteCloseTag(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
			: base(symbol, snapshot, numberedMarkup) { }

		/// <summary>
		/// A DeleteCloseTag jumps when:
		/// 1- there's a positive number of active characters left to delete (ToDelete>0).
		/// 2- breaks that are lower than itself.
		/// 3- other deleteTags 
		/// 4 -or symbol nodes that have no symbol value (deleted symbols).
		/// 5- insertionTags if the insertRevision has no more active symbols left.
		/// 
		/// Thus, a DeleteCloseTag jumps over everything except:
		/// A. SymbolNodes with positionCount > 0 when there's no more characters to be deleted.
		/// B. BreakSymbols with RevisionNumber >= It's Own revisionNumber
		/// C. InsertTags where the Revision has more than 0 active characters
		/// </summary>
		/// <param name="target">SymbolNode we wish to jump over.</param>
		/// <returns>True if this node can jump over the target, false if not.</returns>
		public override bool JumpsOver(SymbolNode target)
		{
			if (Revision == null) return false;

			//A.
			if (!(target is MarkupNode))
			{
				return target.PositionCount == 0 || (Revision is DeletionSnapshot && ((DeletionSnapshot)Revision).ToDelete > 0);
			}
			else
			{
				// B.
				if (target is BreakNode && target.Revision.RevisionNumber >= Revision.RevisionNumber)
				{
					return false;
				}
				// C.
				return !((target is InsertCloseTag || target is InsertOpenTag) && ((ProductionSnapshot)target.Revision).ActiveCharacters > 0);
			}
		}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public DeleteCloseTag()
        {
        }
	}
}
