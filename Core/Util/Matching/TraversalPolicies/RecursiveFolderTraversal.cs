using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InputLog.Core.Util.Matching.TraversalPolicies
{
	/// <summary>
	/// This is a traversal policy that recursively traverses folders.
	/// The traversal is depth first. The first folder to be returned is lowest 
	/// folder of the every first (recursive) sub folder.
	/// </summary>
	public class RecursiveFolderTraversal: ITraversalPolicy<string>
	{
		#region private_fields
		/// <summary>
		/// The startPoint for the recursive search2
		/// </summary>
		private string _startPoint;
		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public RecursiveFolderTraversal() {}

		/// <summary>
		/// Create the recursive folder traverser, already knowing the startpoint.
		/// </summary>
		/// <param name="startPoint">Startpoint to start the traversal.</param>
		public RecursiveFolderTraversal(string startPoint) 
		{
			StartPoint = startPoint;
		}

		#region public_fields
		/// <summary>
		/// Set the start point for the traversal. This is the top level directory. 
		/// From this directory on the search2 will go deeper. Siblings will not be searched.
		/// </summary>
		public string StartPoint
		{
			get
			{
				return _startPoint;
			}
			set
			{
				_startPoint = value;
			}
		}
		#endregion

		//
		// Implementation of the traversalPolicy interface.
		//
		#region traversal_policy_interface
		/// <summary>
		/// Traverse the folders recursively using the iterator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<string> GetEnumerator()
		{
			if (StartPoint == null || StartPoint == "")
			{
				throw new OperationCanceledException("RecursiveFolderTraversal not initialized correctly. Can not recursively iterate folders if no start point is specified!");
			}

			// Start over my own subdirectories
			foreach(string subDir in Directory.GetDirectories(StartPoint))
			{
				// Go depth first over my own SubDirs by using a recursive call to a RecursiveFolderTraversal.
				RecursiveFolderTraversal subDirTraverser = new RecursiveFolderTraversal(subDir);
				foreach (string subSubDir in subDirTraverser)
				{
					// The first folder ever to be returned will be the lowest lvl folder
					yield return subSubDir;
				}
			}
			// Return myself.
			yield return StartPoint;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}
}
