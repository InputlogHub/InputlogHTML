using System.Collections.Generic;

namespace InputLog.Core.Views
{
	abstract class CompoundView: BasicView 
	{
		// Publicly declared data members
		#region public_data_members
		/// <summary>
		/// The type of the view, this should be overwritten in every 
		/// subclass.
		/// </summary>
		public override ViewType TYPE
		{
			get { return ViewType.COMPOUND_VIEW; }
		}
		#endregion

		// Datamembers inherited by all other views
		#region protected_data_members
		#endregion

		// Datamembers privately owned by this view
		#region private_data_members
		/// <summary>
		/// The list of all the components of this compound view.
		/// </summary>
		private readonly List<BasicView> Components;
		#endregion

		/// <summary>
		/// Create a compound view. A compound requires a name.
		/// </summary>
		protected CompoundView() 
		{
			Components = new List<BasicView>();
		}

		/// <summary>
		/// Enumerates the components of this compoundView. 
		/// </summary>
		/// <returns>Each element in this compoundView, one at a time, using an iterator.</returns>
		public virtual IEnumerator<BasicView> GetEnumerator()
		{
		    return ((IEnumerable<BasicView>) Components).GetEnumerator();
		}
	}
}
