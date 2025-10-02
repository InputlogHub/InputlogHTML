
namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots
{
    internal class ProductionSnapshot: AbstractRSnapshot
	{
		#region fields
		/// <summary>
		/// Return the number of active characters remaining in this production snapshot.
		/// </summary>
		public int ActiveCharacters => Count;

	    /// <summary>
		/// If this snapshot is contained within another production snapshot it will be set 
		/// here as its parent. Any changes in ActiveCharacter count this snapshot gets will
		/// cascade through to its parents.
		/// </summary>
		private ProductionSnapshot _parent; 
		#endregion

		/// <summary>
		/// Construct a new snapshot of a productionrevision with given revisionNumber
		/// </summary>
		/// <param name="revisionNumber">ID of the revision</param>
		public ProductionSnapshot(int revisionNumber)
			: base(revisionNumber) {}

        /// <summary>
        /// Default constructor for use with serialization.
        /// Constructor should be public, not protected.
        /// </summary>
        public ProductionSnapshot()
        {
        }

		/// <summary>
		/// Link this productionsnapshot to another productionsnapshot. Semantically this means
		/// that this production snapshot is contained within the parent Production Snapshot. So any active
		/// characters added to this snapshot also increase the count of its parent. Any changes in count
		/// will posibly cascade through a chain of parents.
		/// </summary>
		/// <param name="parent">The containing production snapshot, a.k.a. the parent</param>
		public void Link(ProductionSnapshot parent)
		{
			if (parent != this)
			{
				_parent = parent;
			}
		}

		/// <summary>
		/// Update the count by adding the specified amount. The update cascades through
		/// the parent chain.
		/// </summary>
		/// <param name="add">Amount to add to the count</param>
		protected override void UpdateCount(int add)
		{
			base.UpdateCount(add);
		    _parent?.UpdateCount(add);
		}
	}

	class NormalProductionSnapshot : ProductionSnapshot
	{
		/// <summary>
		/// Construct a new snapshot of a productionrevision with given revisionNumber
		/// </summary>
		/// <param name="revisionNumber">ID of the revision</param>
		public NormalProductionSnapshot(int revisionNumber)
			: base(revisionNumber) { }

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public NormalProductionSnapshot()
        {
        }
	}
}
