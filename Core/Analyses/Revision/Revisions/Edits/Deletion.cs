using InputLog.Core.Plugin.WordLog;
using WordLog = InputLog.Core.Events.WordLog;
using System.Xml;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// Command that removes the given range from the given document.
    /// </summary>
    sealed class Deletion : AbstractEdit
    {
        #region Fields

        /// <summary>
        /// The type of revision to which the edit belongs (insert, delete, ...).
        /// </summary>
        protected override RevisionType RevisionType { get { return RevisionType.DELETE; } }

        /// <summary>
        /// Contains the text that was deleted when the Edit was executed.
        /// </summary>
        private string DeletedText;

		/// <summary>
		/// Length of the edit. Only valid after a call to the execute() method has been made.
		/// </summary>
		public override int Length
		{
			get { return (DeletedText != null) ? DeletedText.Length : 0; }
		}

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="replacement">A Replacement that defines the characters to delete in the document</param>
        /// <param name="id">Id of the event associated with this edit</param>
        public Deletion(WordLog.Replacement replacement, int id)
            : this(replacement.Start, replacement.End, id)
        {
            InputlogDocument.IsRevision = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">The start position of the range to delete.</param>
		/// <param name="end">The end position of the range to delete.</param>
		/// <param name="id">Id of the event associated with this edit</param>
        private Deletion(int start, int end, int id)
            : base(EditType.Deletion, id)
        {
            InputlogDocument.IsRevision = true;
            StartPos = start;
            EndPos = end;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Deletion()
        {
            InputlogDocument.IsRevision = true;
        }

        /// <summary>
        /// Removes the range from the given document.
        /// </summary>
        /// <param name="doc">The document where to remove the range from.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public override bool Execute(InputlogDocument doc)
        {
            TextRange range = doc.Range(StartPos, EndPos);
            DeletedText = range.Text;
            range.Range.Delete();
			doc.Selection.SetRange(StartPos, StartPos);

			return StartPos != EndPos;
        }

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public override bool Undo(InputlogDocument doc)
        {
            doc.Range(StartPos, StartPos).Range.Text = DeletedText;
			return StartPos != EndPos;
        }

        /// <summary>
        /// Returns true if the edit belongs to the given revision.
        /// The given document should contain the version of the logged document right before
        /// the instance of IEdit is executed on it (i.e. all previously seen edits should be
        /// executed on it).
        /// </summary>
        /// <param name="rev">The revision.</param>
        /// <param name="doc">The document on which all the edits until this one are executed (this one excluded).</param>
        /// <returns>True if this instance belongs to the given revision, false if not.</returns>
        protected override bool BelongsTo(IRevision rev, InputlogDocument doc)
        {
            var revision = rev as DeleteRevision;
            if (revision == null) return false;
            var delrev = revision;
            var rslt = (delrev.Edits.Count == 0) || (StartPos <= delrev.Start && delrev.Start <= EndPos);

            return rslt;
        }

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public override string Effect()
		{
			return DeletedText;
		}

        #region Xml Serialization Infrastructure
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("DeletedText", DeletedText);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            DeletedText = reader.ReadElementString("DeletedText");
        }
        #endregion
    }
}