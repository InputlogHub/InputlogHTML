using System;
using System.Xml;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Plugin.WordLog;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// An edit that changes the selection of the document.
    /// </summary>
    class SelectionChange : AbstractEdit
    {
        #region Fields
        /// <summary>
        /// The actual SelectionChange event.
        /// </summary>
        private readonly Events.WordLog.SelectionChange Change;

        /// <summary>
        /// The type of revision to which the edit belongs (insert, delete, ...).
        /// </summary>
        protected override RevisionType RevisionType { get { return RevisionType.SELECTION_CHANGE; } }

        /// <summary>
        /// The new start position of the selection.
        /// </summary>
        public override int StartPos { get { return Change.Start; } }

        /// <summary>
        /// The new end position of the selection.
        /// </summary>
        public override int EndPos { get { return Change.End; } }

		/// <summary>
		/// Returs
		/// </summary>
		public override int Length
		{
			get
			{
				return EndPos - StartPos;
			}
		}

        /// <summary>
        /// The selection prior to the lastly called Execute.
        /// JR - 15082013: Ranges aren't serializable, changed this to storing only used attributes of Range
        /// </summary>
        //private Range PreviousRange;
        private int PreviousRangeStart;
        private int PreviousRangeEnd;

		/// <summary>
		/// The currently selected text, caused by this selection Change event.
		/// </summary>
		private string SelectedText;

        /// <summary>
        /// Returns true if the selection prior to the lastly called Execute was different from the new selection.
        /// </summary>
        private bool SelectionChanged { get { return PreviousRangeStart != StartPos || PreviousRangeEnd != EndPos; } }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="change">The actual selectionchange event.</param>
		/// <param name="id">Id of the event associated with this edit</param>
        public SelectionChange(Events.WordLog.SelectionChange change, int id)
            : base(EditType.SelectionChange, id)
        {
            InputlogDocument.IsRevision = true;
            Change = change;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public SelectionChange()
        {
            InputlogDocument.IsRevision = true;
            Change = new Events.WordLog.SelectionChange();
        }

        /// <summary>
        /// Replaces the range as denoted in Replacement by the text denoted in Replacement
        /// in the given document.
        /// </summary>
        /// <param name="doc">The document where to replace the range in.</param>
        /// <returns>True if the execution changed the selection, false if not.</returns>
        public override bool Execute(InputlogDocument doc)
        {
            PreviousRangeStart = doc.Selection.Range.Start;
            PreviousRangeEnd = doc.Selection.Range.End;
            doc.Selection.SetRange(Change.Start, Change.End);
			SelectedText = doc.Selection.Range.Text;

            return SelectionChanged;
        }

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the execution changed the selection, false if not.</returns>
        public override bool Undo(InputlogDocument doc)
        {
            doc.Selection.SetRange(PreviousRangeStart, PreviousRangeEnd);

            return SelectionChanged;
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
            // A selectionchange always belongs to the previous revision as it is not really a revision on its own
            return true;
        }

        /// <summary>
        /// Adds the edit to the given RevisionAnalysisSummary. It check whether there is a current
        /// revision and whether the edit belongs to it (using BelongsTo). If there is not current
        /// revision or if the edit does not belong to it, a new revision of type RevisionType will
        /// created and the edit will be added to that revision, otherwise, the edit will be added
        /// to the current revision.
        /// </summary>
        /// <param name="summary">The RevisionAnalysisSummary that contains the current
        /// state of the RevisionAnalysis.</param>
        public override void AddTo(RevisionAnalysisSummary summary)
        {
            summary.AddUnrelevantEdit(this);
        }

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public override string Effect()
		{
			return SelectedText;
		}

        #region Xml Serialization Infrastructure
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SelectedText", SelectedText);
            writer.WriteElementString("PreviousRangeStart", PreviousRangeStart.ToString());
            writer.WriteElementString("PreviousRangeEnd", PreviousRangeEnd.ToString());
            writer.WriteStartElement("Change");
            Change.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            SelectedText = reader.ReadElementString("SelectedText");
            PreviousRangeStart = Int32.Parse(reader.ReadElementString("PreviousRangeStart"));
            PreviousRangeEnd = Int32.Parse(reader.ReadElementString("PreviousRangeEnd"));
            reader.ReadStartElement("Change");
            Change.ReadXml(reader);
            reader.ReadEndElement();
        }
        #endregion
    }
}