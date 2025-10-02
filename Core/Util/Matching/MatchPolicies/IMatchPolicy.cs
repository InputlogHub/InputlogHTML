using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util.Matching.MatchPolicies
{
	/// <summary>
	/// A match policy is a class that tries to match objects. It finds its matches
	/// based on its input objects of type T. The matches it finds are of type R and thus
	/// it returns items in the form of a List&lt;IMatch&lt;R&gt;&gt;
	/// </summary>
	public interface IMatchPolicy<T,R>
	{
		/// <summary>
		/// Begin a match-finding session. A policy can only find matches within a one
		/// single matching session, not spanning multiple matching-sessions.
		/// </summary>
		void BeginMatchingSession();

		/// <summary>
		/// Called when a new input item within the matching session has been discovered.
		/// If any matches have already been created, they may be returned.
		/// </summary>
		/// <param name="item">An input item that has been found which can be used
		/// for finding valid matches</param>
		/// <returns>A list of all matches so far created, but not yet returned. If no new 
		/// mathces have been made by adding this item, this method may return null</returns>
		List<IMatch<R>> FoundItem(T item);

		/// <summary>
		/// End the current matching session and return all Matches that have been
		/// discovered.
		/// </summary>
		List<IMatch<R>> EndMatchingSession();

		/// <summary>
		/// End all of the matching. If any matches remain to be returned, return them
		/// here.
		/// </summary>
		/// <returns>The list of all matches that have been found but have not been returned
		/// so far.</returns>
		List<IMatch<R>> EndMatching();
	}
}
