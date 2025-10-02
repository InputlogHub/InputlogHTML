using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Plugin.WordLog;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// An abstract implementation of the common parts of the different edits.
    /// </summary>
    public abstract class AbstractEdit : IEdit
    {
        #region Fields
        /// <summary>
        /// The type of revision to which the edit belongs (insert, delete, ...).
        /// </summary>
        protected abstract RevisionType RevisionType { get; }

        /// <summary>
        /// The type of the edit.
        /// </summary>
        public EditType Type { get; set; }

        /// <summary>
        /// StartTime of the event.
        /// </summary>
		public virtual ulong StartTime { get; set; }

        /// <summary>
        /// EndTime of the event.
        /// </summary>
		public virtual ulong EndTime { get; set; }

		/// <summary>
		/// Returns true if the edit has timing information available, or false if not.
		/// </summary>
		public virtual bool HasTiming
		{
		    get
			{
				return false;
			}
		    set { throw new NotImplementedException(); }
		}

        /// <summary>
        /// ActionTime of the event (=EndTime - StartTime);
        /// </summary>
        public ulong ActionTime
        {
            get
            {
                return EndTime > StartTime ? EndTime - StartTime : 0;
            }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
		/// Start position of the event, if any.
		/// </summary>
		public virtual int StartPos { get; set; }

		/// <summary>
		/// End position of the event, if any.
		/// </summary>
		public virtual int EndPos { get; set; }

		/// <summary>
		/// Length of the edit. This might equal Endpos - Startpos, but in case of inserted or deleted text it might
		/// be that Startpos == Endpos and that Length != Endpos - Startpos
		/// </summary>
		public virtual int Length => EndPos - StartPos;

        /// <summary>
        /// PauseTime of the event.
        /// </summary>
        public ulong? PauseTime { get; set; }

        /// <summary>
        /// PauseLocation of the event.
        /// </summary>
        public PauseLocation PauseLocation { get; set; }

        /// <summary>
        /// Returns the current POU (Point Of Utterance - last position of the cursor in the document).
        /// This is only a valid property if at least one Deletion, Insertion or TypeChar was added to the revision,
        /// otherwise the property equals -1.
        /// </summary>
        public readonly int CurrentPOU = -1;

		/// <summary>
		/// Event id associated with this edit.
		/// </summary>
		public int Id { get; set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of the edit.</param>
		/// <param name="id">The id of the event associated with this edit.</param>
        protected AbstractEdit(EditType type, int id)
        {
            InputlogDocument.IsRevision = true;
            Type = type;
			Id = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of the edit.</param>
        protected AbstractEdit(EditType type)
        {
            InputlogDocument.IsRevision = true;
            Type = type;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public AbstractEdit()
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
		protected abstract bool BelongsTo(IRevision rev, InputlogDocument doc);

        /// <summary>
        /// Adds the edit to the given RevisionAnalysisSummary. It check whether there is a current
        /// revision and whether the edit belongs to it (using BelongsTo). If there is not current
        /// revision or if the edit does not belong to it, a new revision of type RevisionType will
        /// created and the edit will be added to that revision, otherwise, the edit will be added
        /// to the current revision.
        /// </summary>
        /// <param name="summary">The RevisionAnalysisSummary that contains the current
        /// state of the RevisionAnalysis.</param>
        public virtual void AddTo(RevisionAnalysisSummary summary)
        {
            var rev = summary.Current;
            var doc = summary.Document;

			// 14 Jun 2012: We execute the edit before we check which revision it belongs too and
			// add it somewhere. We need the information set during execution of the edit to correctly determine
			// which revision type the edit belongs too.
			//
			// SelectionEdits are executed on a different code path and never enter the AbstractEdit.AddTo() method,
			// therefore we need not take them into account here.
			//
			Execute(doc);
            if (rev == null || !BelongsTo(rev, doc))
            {
                rev = summary.CreateRevision(RevisionType,this);
            }

            // First add the previous SelectionChange events to the revision to which this edit belongs.
            summary.FlushUnrelevantEdits();

            rev.Add(this, doc);
        }

        /// <summary>
        /// Executes the command over the given Document.
        /// </summary>
        /// <param name="doc">The document where to execute the command over.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public abstract bool Execute(InputlogDocument doc);

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public abstract bool Undo(InputlogDocument doc);

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public abstract string Effect();

        #region Xml Serialization Infrastructure
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("ID", Id.ToString());
            writer.WriteElementString("EditType", Type.ToString());
            writer.WriteElementString("StartTime", StartTime.ToString());
            writer.WriteElementString("EndTime", EndTime.ToString());
            writer.WriteElementString("StartPos", StartPos.ToString());
            writer.WriteElementString("EndPos", EndPos.ToString());
            writer.WriteElementString("PauseLocation", PauseLocation.ToString());
            if (PauseTime != null) writer.WriteElementString("PauseTime", PauseTime.ToString());
        }

        public virtual void ReadXml(XmlReader reader)
        {
            Id = Int32.Parse(reader.ReadElementString("ID"));
            Type = (EditType) Enum.Parse(typeof(EditType), reader.ReadElementString("EditType"));
            StartTime = ulong.Parse(reader.ReadElementString("StartTime"));
            EndTime = ulong.Parse(reader.ReadElementString("EndTime"));
            StartPos = Int32.Parse(reader.ReadElementString("StartPos"));
            EndPos = Int32.Parse(reader.ReadElementString("EndPos"));
            PauseLocation = (PauseLocation)Enum.Parse(typeof(PauseLocation), reader.ReadElementString("PauseLocation"));
            if (reader.IsStartElement("PauseTime"))
                PauseTime = ulong.Parse(reader.ReadElementString("PauseTime"));
        }
        #endregion
    }
}