using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;

namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// Abstract implementation of a Revision that implements the common parts.
    /// </summary>
    internal abstract class AbstractRevision : IRevision
    {
        #region Fields
        /// <summary>
        /// List of edits that needs to be performed to transition
        /// the document from the previous revision to the current one.
        /// </summary>
        readonly List<IEdit> _thisEdits = new List<IEdit>();
        public List<IEdit> Edits
        {
            get { return _thisEdits; }
        }

        /// <summary>
        /// The revision number of this revision.
        /// CHANGED: RevisionNumber may be equal to NULL now.
        /// </summary>
        public int RevisionNumber { get; }

        /// <summary>
        /// Returns the last edit currently known to be part of the revision or null if no edits are part of the revision.
        /// </summary>
        protected IEdit LastEdit => Edits.Last();

        /// <summary>
        /// Returns the type of the revision (Insert, Delete or SelectionChange).
        /// </summary>
        public RevisionType Type { get; }

        /// <summary>
        /// Returns the current POU (Point Of Utterance - last position of the cursor in the document).
        /// This is only a valid property if at least one Deletion, Insertion or TypeChar was added to the revision,
        /// otherwise the property equals -1.
        /// </summary>
        public virtual int CurrentPOU => Defaultval;

        /// <summary>
        /// Default value for int properties which arent correctly initialized yet.
        /// </summary>
        public const int Defaultval = -1;

		/// <summary>
		/// The start time of the first timed edit of the revision. 
		/// Returns 0 if no timed event can be found in the revision.
		/// </summary>
		private ulong? _thisStartTime;
		public virtual ulong StartTime
		{
		    get
			{
				if (_thisStartTime != null)
				{
					return (ulong)_thisStartTime;
				}
				// we still have to find the first timed event.
			    foreach (IEdit edit in Edits)
			    {
			        if (edit.HasTiming)
			        {
			            _thisStartTime = edit.StartTime;
			            return edit.StartTime;
			        }
			    }
			    return 0;
			}
		}

        // Helpers for getting EndTime
		private ulong? _thisEndTime;
		private int _editsLastEndTime;

		/// <summary>
		/// The end time of the last timed edit in the revision.
		/// </summary>
		public virtual ulong EndTime
		{
		    get
			{
				if (_thisEndTime != null && _editsLastEndTime == Edits.Count)
				{
					return (ulong)_thisEndTime;
				}
			    for (int i = Edits.Count - 1; i >= 0; i--)
			    {
			        if (Edits[i].HasTiming)
			        {
			            _thisEndTime = Edits[i].EndTime;
			            _editsLastEndTime = Edits.Count;
			            return (ulong)_thisEndTime;
			        }
			    }
			    return 0;
			}
		}

        /// <summary>
		/// The start position of the first relevant edit in the revision.
		/// A relevant edit is an edit that has replay value (not of the Edits.EditType.Ignored type).
		/// </summary>
		private int _thisStartPos = -1;
		public virtual int StartPos
		{
		    get
			{
				if (_thisStartPos != -1)
				{
					return _thisStartPos;
				}
			    foreach (IEdit edit in Edits)
			    {
			        if (edit.Type != EditType.Ignored && edit.Type != EditType.SelectionChange)
			        {
			            _thisStartPos = edit.StartPos;
			            return _thisStartPos;
			        }
			    }
			    return _thisStartPos;
			}
		}

        // Helpers for the EndPos property
		private int _thisEndPos = -1;
		private int _editsLastEndPos = -1;

		/// <summary>
		/// The end position of the last relevant edit in the revision.
		/// A relevant edit is an edit that has replay value (not of the Edits.EditType.Ignored type).
		/// </summary>
		public virtual int EndPos
		{
		    get
			{
				if (_thisEndPos != -1 && _editsLastEndPos == Edits.Count)
				{
					return _thisEndPos;
				}
			    for (int i = Edits.Count - 1; i >= 0; i--)
			    {
			        if (Edits[i].Type != EditType.Ignored && Edits[i].Type != EditType.SelectionChange)
			        {
			            _thisEndPos = Edits[i].EndPos;
			            _editsLastEndPos = Edits.Count;
			            return _thisEndPos;
			        }
			    }
			    return 0;
			}
		}

        /// <summary>
		/// Return the lenght of the revision.
		/// </summary>
		public int Length => EndPos - StartPos;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revisionnumber">The revision number of this revision.</param>
        /// <param name="type">The type of the revision (Delete, Insert or SelectionChange).</param>
        protected AbstractRevision(int revisionnumber, RevisionType type)
        {
            InputlogDocument.IsRevision = true;
            RevisionNumber = revisionnumber;
            Type = type;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of the revision (Delete, Insert or SelectionChange).</param>
        protected AbstractRevision(RevisionType type)
        {
            InputlogDocument.IsRevision = true;
            Type = type;
            RevisionNumber = -1;
        }

        /// <summary>
        /// Updates the given document, that should be of the previous revision, to the current revision.
        /// If the given document is not of the previous revision, the behaviour is undefined.
        /// </summary>
        /// <param name="doc">The document to update from the previous revision to the current one.</param>
        public virtual void Update(InputlogDocument doc)
        {
            foreach (IEdit edit in Edits)
            {
                edit.Execute(doc);
            }
        }

        /// <summary>
        /// Adds the given edit to the list of edits or throws an exception if the given edit does not belong in the revision.
        /// Subclasses should override this method in order to provide correct functionality.
        /// </summary>
        /// <param name="edit">The edit to add.</param>
        /// <param name="doc">The document in the state right before the edit is executed.
        /// When the control is returned, the edit will be executed on the document.</param>
        /// <exception cref="NotInRevisionException">Whenever the given edit does not belong in the revision.</exception>
        public virtual void Add(IEdit edit, InputlogDocument doc)
        {
            Edits.Add(edit);
            
			// 14 Jun 2012: Changed the order. First we execute an edit on the document, then we see what type of
			// revision the edit would belong too. We need the information in the edit that will only be set during
			// execution to determine in what type of revision the edit belongs
			//
			//edit.Execute(doc);
        }

		/// <summary>
		/// Returns the combined effect of all the edits in this revision. This may be for example, all the text that has been 
		/// added/deleted/selected.
		/// </summary>
		/// <returns>The combined effect of all events in this revision</returns>
		public abstract string Effect();

		/// <summary>
		/// Retuns a list of all the relevant edits in the revision, in the right order they have been added
		/// to the revision. Relevant edits are edits that have changed the document in some way. By this we mean
		/// edits that have added or deleted value: insertions, deletions and typechars with replay.
		/// </summary>
		/// <returns>A list of all relevant edits in this revision.</returns>
		public List<IEdit> GetRelevantEdits()
		{
		    return Edits.Where(edit => edit.Type != EditType.Ignored && edit.Type != EditType.SelectionChange).ToList();
		}

    }
}