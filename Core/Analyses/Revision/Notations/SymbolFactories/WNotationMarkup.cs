using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories
{
    internal class WNotationMarkup : ISymbolFactory
	{
		#region fields
		private const string BREAK = "^";

        public string Break { get { return BREAK; } }
		#endregion

		public MarkupNode GetBreakSymbol(AbstractRSnapshot rev)
        {
            return new BreakNode(Break + "_" + rev.RevisionNumber + "_", rev);
        }

		public MarkupNode GetLeftInsertionSymbol(AbstractRSnapshot rev)
        {
			return new InsertOpenTag("_" + rev.RevisionNumber + "_{", rev);
        }

		public MarkupNode GetRightInsertionSymbol(AbstractRSnapshot rev)
        {
			return new InsertCloseTag("}_" + rev.RevisionNumber + "_", rev);
        }

		public MarkupNode GetLeftDeletionSymbol(AbstractRSnapshot rev)
        {
			return new DeleteOpenTag("_" + rev.RevisionNumber + "_[", rev);
        }

		public MarkupNode GetRightDeletionSymbol(AbstractRSnapshot rev)
        {
			return new DeleteCloseTag("]_" + rev.RevisionNumber + "_", rev);
        }
    }
}
