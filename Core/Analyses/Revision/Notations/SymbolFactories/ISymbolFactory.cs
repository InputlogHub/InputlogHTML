using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories
{
    /// <summary>
    /// ISymbolFactory
    /// </summary>
    public interface ISymbolFactory
    {
        string Break { get; }

        MarkupNode GetBreakSymbol(AbstractRSnapshot rev);

		MarkupNode GetLeftInsertionSymbol(AbstractRSnapshot rev);

		MarkupNode GetRightInsertionSymbol(AbstractRSnapshot rev);

		MarkupNode GetLeftDeletionSymbol(AbstractRSnapshot rev);

		MarkupNode GetRightDeletionSymbol(AbstractRSnapshot rev);
    }
}
