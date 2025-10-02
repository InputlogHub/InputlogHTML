using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;

namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// Enumeration that contains the different types of revisions.
    /// </summary>
    public enum RevisionType { DELETE, INSERT, SELECTION_CHANGE, NORMAL_PRODUCTION, IGNORED }

    /// <summary>
    /// Represents a single revision.
    /// </summary>
    public interface IRevision
    {
        #region Fields
        /// <summary>
        /// List of edits that needs to be performed to transition
        /// the document from the previous revision to the current one.
        /// </summary>
        List<IEdit> Edits { get; }

        /// <summary>
        /// The revision number of this revision.
        /// </summary>
        int RevisionNumber { get; }

        /// <summary>
        /// Returns the type of the revision (Insert, Delete or SelectionChange).
        /// </summary>
        RevisionType Type { get; }

        /// <summary>
        /// Returns the current POU.
        /// This is only a valid property if at least one Deletion, Insertion or TypeChar was added to the revision,
        /// otherwise the property equals -1.
        /// </summary>
        int CurrentPOU { get; }

		/// <summary>
		/// The start time of the first timed edit of the revision. 
		/// </summary>
		ulong StartTime { get; }

		/// <summary>
		/// The end time of the last timed edit in the revision.
		/// </summary>
		ulong EndTime { get; }

		/// <summary>
		/// The start position of the first relevant edit in the revision.
		/// A relevant edit is an edit that has replay value (not of the Edits.EditType.Ignored type).
		/// </summary>
		int StartPos { get; }

		/// <summary>
		/// The end position of the last relevant edit in the revision.
		/// A relevant edit is an edit that has replay value (not of the Edits.EditType.Ignored type).
		/// </summary>
		int EndPos { get; }
        #endregion

        /// <summary>
        /// Updates the given document, that should be of the previous revision,
        /// to the current revision.
        /// If the given document is not of the previous revision, the behaviour is undefined.
        /// </summary>
        /// <param name="doc">The document to update from the previous revision to the current one.</param>
        void Update(InputlogDocument doc);

        /// <summary>
        /// Adds the given edit to the list of edits and updates the POU.
        /// </summary>
        /// <param name="edit">The edit to add.</param>
        /// <param name="doc">The document in the state right before the edit is executed.
        /// When the control is returned, the edit will be executed on the document.</param>
        /// <exception cref="NotInRevisionException">Whenever the given edit does not belong in the revision.</exception>
        void Add(IEdit edit, InputlogDocument doc);

		/// <summary>
		/// Returns the combined effect of all the edits in this revision. This may be for example, all the text that has been 
		/// added/deleted/selected.
		/// </summary>
		/// <returns>The combined effect of all events in this revision</returns>
		string Effect();

    }
}