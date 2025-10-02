using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;
using System.Collections.Generic;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories
{
    /// S-Notation 
    /// S-notation is a formal method for representing the successive text editing actions made during a writing session.
    /// (Kollberg, P. (1996) S-notation as a tool for analysing the episodic structure of revisions. Paper presented at 
    /// the European Writing Conferences, Barcelona, October 1996)
    /// S-notation can be derived from a keystroke logfile and provides a record of the writer's revisions (insertions and deletions),
    /// presented at their place in the text. S-notation also shows in what order the revisions occurred during the writing session.
    /// Revisions and breaks are numbered according to the order of their occurrence in the writing process.
    /// The S-notation uses the following symbols:
    /// |i                  The break (vertical bar/pipe symbol) with a sequential number i indicates the location of the revision
    /// {inserted text}i    An insertion (curled braces) following a break at # i 
    /// [deleted text]i     A deletion (square brackets) following a break at # i 
    /// This basic info includes: event analysisType, output, pausetime, actiontime, start- and endtime, pauselocation, etc
    internal class SNotationMarkup : ISymbolFactory
	{
		#region fields
		/// <summary>
		/// break character
		/// </summary>
        private const string BREAK = "|";

		/// <summary>
		/// Get the break character.
		/// </summary>
        public string Break { get { return BREAK; } }
		#endregion

		public MarkupNode GetBreakSymbol(AbstractRSnapshot rev)
        {
			return new BreakNode(BREAK, rev, true);
        }

		public MarkupNode GetLeftInsertionSymbol(AbstractRSnapshot rev)
        {
            return new InsertOpenTag("{", rev, false);
        }

		public MarkupNode GetRightInsertionSymbol(AbstractRSnapshot rev)
        {
            return new InsertCloseTag("}", rev, true);
        }

		public MarkupNode GetLeftDeletionSymbol(AbstractRSnapshot rev)
        {
            return new DeleteOpenTag("[", rev, false);
        }

		public MarkupNode GetRightDeletionSymbol(AbstractRSnapshot rev)
        {
            return new DeleteCloseTag("]", rev, true);
        }
    }
}