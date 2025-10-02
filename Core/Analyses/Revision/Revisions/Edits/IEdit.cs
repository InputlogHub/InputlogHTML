using InputLog.Core.Plugin.WordLog;
using System.Xml;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// Enumeration listing the different types of edits.
	/// The Ignored-type may be used for edits that add no value to a revision, that is cursor changing, clicking at
	/// random locations. Keys that solely trigger other types of edits, such as CTRL+V and thus on their own
	/// are not relevant.
    /// </summary>
	/// 20120508 - Removed SelectionChange type as it is basically also an Ignored type.
    public enum EditType { Deletion, Insertion, SelectionChange, TypeChar, Ignored, Skip }

    /// <summary>
    /// Interface for commands that edits a given Document.
    /// Commands can edit a document by adding text, replacing text, ...
    /// </summary>
    public interface IEdit
    {
        #region Fields
        /// <summary>
        /// The type of the edit.
        /// </summary>
        EditType Type { set; get; }

        /// <summary>
        /// StartTime of the event.
        /// </summary>
        ulong StartTime { set; get; }

        /// <summary>
        /// EndTime of the event.
        /// </summary>
        ulong EndTime { set; get; }

		/// <summary>
		/// Returns true if the edit has timing information, false if not.
		/// </summary>
        bool HasTiming { set; get; }

        /// <summary>
        /// ActionTime of the event (=EndTime - StartTime);
        /// </summary>
        ulong ActionTime { set; get; }

        /// <summary>
        /// PauseTime of the event.
        /// </summary>
        ulong? PauseTime { set; get; }

		/// <summary>
		/// Start position of the event, if any.
		/// </summary>
        int StartPos { set; get; }

		/// <summary>
		/// End position of the event, if any.
		/// </summary>
        int EndPos { set; get; }

		/// <summary>
		/// Length of the edit. This is not necessarily EndPos - StartPos
		/// </summary>
		int Length { get; }

        /// <summary>
        /// PauseLocation of the event.
        /// </summary>
        PauseLocation PauseLocation { get; }

		/// <summary>
		/// The id of the that caused this edit.
		/// </summary>
        int Id { set; get; }
        #endregion

        /// <summary>
        /// Executes the command over the given Document.
        /// </summary>
        /// <param name="doc">The document where to execute the command over.</param>
        /// <returns>True if the exetion changed the document state (change in selection, content has changed, ...),
        ///  false if not.</returns>
        bool Execute(InputlogDocument doc);

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the exetion changed the document state (change in selection, content has changed, ...), 
        /// false if not.</returns>
        bool Undo(InputlogDocument doc);

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		string Effect();

        void WriteXml(XmlWriter writer);
        void ReadXml(XmlReader reader);
    }
}