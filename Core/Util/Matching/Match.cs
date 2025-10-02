using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util.Matching
{
	/// <summary>
	/// A basic implementation of a match class that implements the 
	/// the IMatch&lt;T&gt; interface.
	/// </summary>
	/// <typeparam name="T">Type of the objects that are part of a match</typeparam>
	class Match<T>: IMatch<T>
	{
		#region protected_fields
		/// <summary>
		/// The list of items that are part of this match.
		/// </summary>
		protected Dictionary<T,bool> Content;
		#endregion

		/// <summary>
		/// Construct the match object.
		/// </summary>
		public Match()
		{
			Content = new Dictionary<T, bool>();
		}

		/// <summary>
		/// Construct a match from an existing match set.
		/// </summary>
		/// <param name="matchSet">Set containing already matched items.</param>
		public Match(ISet<T> matchSet)
		{
			Content = new Dictionary<T, bool>(matchSet.Count);
			foreach (T item in matchSet)
			{
				Content.Add(item, true);
			}
		}

		/// <summary>
		/// Add an item to the match set. If the item was already in the match set it will not be added again.
		/// </summary>
		/// <param name="item">Item to be added to the match.</param>
		public virtual void Add(T item)
		{
			Content.Add(item,true);
		}

		/// <summary>
		/// Returns a list of all the items that are part of this match.
		/// </summary>
		/// <returns>All the items that are part of this match</returns>
		public virtual ISet<T> Items()
		{
			return new HashSet<T>(Content.Keys);
		}

		/// <summary>
		/// Returns a set of all the items that have been selected in this match.
		/// </summary>
		/// <returns>All the selected items in the match.</returns>
		public virtual ISet<T> SelectedItems()
		{
			return new HashSet<T>(Content.Keys.Where(key => Content[key]));
		}

		/// <summary>
		/// Change the selection state of an item in the match. This method
		/// allows one to select or deselect an item in the match. If the item
		/// could not be found in the match it returns false, otherwise it returns
		/// true.
		/// </summary>
		/// <param name="item">Item to be (de)selected</param>
		/// <param name="selected">True if the item is to be selected, false if it is to be
		/// deselected</param>
		/// <returns>True if the item was found in this Match, false if not.</returns>
		public virtual bool ChangeItemSelection(T item, bool selected)
		{
			if (Content.ContainsKey(item))
			{
				Content[item] = selected;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
