
namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots
{
    internal class DeletionSnapshot : AbstractRSnapshot
	{
		#region fields
		/// <summary>
		/// Return the number of characters that still have to be deleted
		/// before having fully executed this deletion snapshot.
		/// </summary>
		public int ToDelete => Count;

	    #endregion

		/// <summary>
		/// Construct a new snapshot of a productionrevision with given revisionNumber
		/// </summary>
		/// <param name="revisionNumber">ID of the revision</param>
		/// <param name="count">The number of characters to be deleted in the snapshot.</param>
		public DeletionSnapshot(int revisionNumber, int count)
			: base(revisionNumber) 
		{
			Count = count;
		}

        /// <summary>
        /// String representation of a DeletionSnapshot
        /// </summary>
        /// <returns>Revision number and number of characters</returns>
	    public override string ToString() => "Revision " + RevisionNumber + " CharCount " + Count;

	    /// <summary>
        /// Default constructor for use with serialization.
        /// Constructor should be public, not protected.
        /// </summary>
        public DeletionSnapshot()
        {
        }
	}
}
