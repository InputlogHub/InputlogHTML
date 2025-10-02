using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util.Matching
{
	/// <summary>
	/// A single match contains a few objects that match according to the used
	/// policy to find the matches. The matches contain objects of type T
	/// </summary>
	public interface IMatch<T>
	{
		/// <summary>
		/// Add an item to the match set. If the item was already in the match set it will not be added again.
		/// </summary>
		/// <param name="item">Item to be added to the match.</param>
		void Add(T item);

		/// <summary>
		/// Returns a list of all the items that are part of this match.
		/// </summary>
		/// <returns>All the items that are part of this match</returns>
		ISet<T> Items();

		/// <summary>
		/// Returns all the items in the match that have been selected. 
		/// Items are selected by default.
		/// </summary>
		/// <returns>A set of all the selected items in the match.</returns>
		ISet<T> SelectedItems();

		/// <summary>
		/// Change the selection of an item to the passed along 'selected' value.
		/// </summary>
		/// <param name="item">Item to (de)selected</param>
		/// <param name="selected">True if the item is to be selected, false if it is not.</param>
		/// <returns>True if the changing of the item was successful, false if no such item was found.</returns>
		bool ChangeItemSelection(T item, bool selected);
	}
}
