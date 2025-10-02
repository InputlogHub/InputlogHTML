using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Plugin.WordLog;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
	class NoEdit: AbstractEdit
	{
		protected override RevisionType RevisionType
		{
			get { return RevisionType.IGNORED; }
		}

	    /// <summary>
	    /// Constructor.
	    /// </summary>
	    /// <param name="id">The id of the event associated with this edit.</param>
	    public NoEdit(int id)
	        : base(EditType.Ignored, id)
	    {
            InputlogDocument.IsRevision = true;
        }

	    /// <summary>
	    /// Constructor Overload
	    /// </summary>
	    public NoEdit()
	        : base(EditType.Skip)
	    {
            InputlogDocument.IsRevision = true;
        }


        /// <summary>
        /// Returns true if the edit belongs to the given revision.
        /// The given document should contain the version of the logged document right before
        /// the instance of IEdit is executed on it (i.e. all previously seen edits should be executed on it).
        /// </summary>
        /// <param name="rev">The revision.</param>
        /// <param name="doc">The document on which all the edits until this one are executed (this one excluded).</param>
        /// <returns>True if this instance belongs to the given revision, false if not.</returns>
		protected override bool BelongsTo(IRevision rev, InputlogDocument doc)
		{
			return true;
		}

        /// <summary>
        /// Executes the command over the given Document.
        /// </summary>
        /// <param name="doc">The document where to execute the command over.</param>
        /// <returns>True if the exetion changed the document state (change in selection, content has changed, ...), false if not.</returns>
		public override bool Execute(InputlogDocument doc)
		{
			return true;
		}

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the exetion changed the document state (change in selection, content has changed, ...), false if not.</returns>
		public override bool Undo(InputlogDocument doc)
		{
			return true;
		}

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public override string Effect()
		{
			return "";
		}
	}
}
