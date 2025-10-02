namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// A revision that changes the current selection.
    /// </summary>
    class SelectionChangeRevision : AbstractRevision
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SelectionChangeRevision() : base(RevisionType.SELECTION_CHANGE) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revisionnumber">The revision number of this revision.</param>
        public SelectionChangeRevision(int revisionnumber) : base(revisionnumber, RevisionType.SELECTION_CHANGE) { }

		/// <summary>
		/// Returns the combined effect of all the edits in this revision. This may be for example, all the text that has been 
		/// added/deleted/selected.
		/// </summary>
		/// <returns>The combined effect of all events in this revision</returns>
		public override string Effect()
		{
			if (LastEdit != null)
			{
				return LastEdit.Effect();
			}
			return "";
		}
    }
}