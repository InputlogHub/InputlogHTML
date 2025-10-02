using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util.Matching.TraversalPolicies
{
	/// <summary>
	/// Implements a way in which to traverse nodes of type T.
	/// </summary>
	public interface ITraversalPolicy<T>: IEnumerable<T>
	{
		/// <summary>
		/// Set the start point for the traverse.
		/// </summary>
		string StartPoint { get; set; }
	}
}
