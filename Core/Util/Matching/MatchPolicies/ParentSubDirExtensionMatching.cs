using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InputLog.Core.Util.Matching.MatchPolicies
{
	/// <summary>
	/// Match on extensions in a ParentDir > SubDir fashion. <br />
	/// The input and output types of the MatchPolicy are strings, the paths to the directories, 
	/// and the paths to the matched files in an IMatch&lt;string&gt;.<br />
	/// This matching policy matches at least one, or all files with 'parent-extensions' in the parent
	/// directory with any or all files with 'subdir' extensions in the parents subfolders.<br />
	/// <br />
	/// <b>The matching policy requires that the traversal of the directories happens in a step-wise, depth first manner.</b>
	/// </summary>
	public class ParentSubDirExtensionMatching: IMatchPolicy<string, string>
	{
		//
		// The extensions and their matchAll boolean flags.
		//
		#region private_extensions
		/// <summary>
		/// Set of extensions for files in the parent folder
		/// </summary>
		private ISet<string> ParentExt;

		/// <summary>
		/// Set of extensions for files in the subdirs of the parent folder
		/// </summary>
		private ISet<string> ChildExt;

		/// <summary>
		/// Boolean that is true if the parentdir should have at least 
		/// one file for each of the extensions in the ParentExt sets.
		/// Null if it is not initialized.
		/// </summary>
		private bool? MatchAllParents;

		/// <summary>
		/// Boolean that is true if the subdir should have at least 
		/// one file for each of the extensions in the ChildExt sets.
		/// Null if it is not initialized.
		/// </summary>
		private bool? MatchAllChildren;
		#endregion

		#region public_fields
		#endregion

		public ParentSubDirExtensionMatching() 
		{
			MatchAllChildren = null;
			MatchAllParents = null;
		}

		/// <summary>
		/// Create the matching policy with the required parameters.
		/// </summary>
		/// <param name="parentExtensions">file extensions that should match in the top folder</param>
		/// <param name="childExtensions">file extensions that should match in the parents subfolders</param>
		/// <param name="matchAllParents">True if all the parent extensions should match, false if only one match is required</param>
		/// <param name="matchAllChildren">True if all the children extensions should match, false if only one match is required</param>
		public ParentSubDirExtensionMatching(ISet<string> parentExtensions, ISet<string> childExtensions, 
			bool matchAllParents = false, bool matchAllChildren = false)
		{
			ParentExt = parentExtensions;
			ChildExt = childExtensions;
			MatchAllParents = matchAllParents;
			MatchAllChildren = matchAllChildren;
		}

		//
		// Initialization methods
		//
		#region initialize

		/// <summary>
		/// Initialize parent extensions information
		/// </summary>
		/// <param name="parentExtensions">The extensions to match on in the parent dir.</param>
		/// <param name="matchAll">True if at least one file of each of the listed extensions should be found for a match.</param>
		public void InitializeParentExtensions(ISet<string> parentExtensions, bool matchAll)
		{
			ParentExt = new HashSet<string>(parentExtensions);
			MatchAllParents = matchAll;
		}

		/// <summary>
		/// Initialize children extensions information
		/// </summary>
		/// <param name="childExtensions">The extensions to match on in the child dir.</param>
		/// <param name="matchAll">True if at least one file of each of the listed extensions should be found for a match.</param>
		public void InitializeChildExtensions(ISet<string> childExtensions, bool matchAll)
		{
			ChildExt = new HashSet<string>(childExtensions);
			MatchAllChildren = matchAll;
		}
		#endregion

		//
		// Matching Policy methods
		// 
		#region IMatchPolicy<string,string> Members

		/// <summary>
		/// Call this method when we enter a new directory.
		/// </summary>
		public void BeginMatchingSession()
		{
			RequireInitialized();
		}

		/// <summary>
		/// Call this method when you have discovered an a directory that has to be
		/// checked for possible matches.
		/// </summary>
		/// <param name="item">Path to the directory to be inspected.</param>
		/// <returns>A list of all the matches that have been found so far, or null if nothing conclusive
		/// has been found so far.</returns>
		public List<IMatch<string>> FoundItem(string item)
		{
			RequireInitialized();

			// If we don't have subdirectories we have nothing to search2 here.
			string[] subDirs = Directory.GetDirectories(item);
			if(subDirs == null || subDirs.Count() == 0) 
			{
				return null;
			}

			// Get all files in the directory that match the extension matching criteria.
			string[] files = Directory.GetFiles(item);
			string[] parentFiles = Array.FindAll<string>(files, file => ParentExt.Contains(Path.GetExtension(file)));

			// Check matchAll criteria for the parrent.
			if (!CheckMatchAll(parentFiles, ParentExt, (bool)MatchAllParents)) return null;

			// Get a list of all the subfiles that are relevant in this match.
			List<string> relevantSubFiles = new List<string>();
			foreach (string subdir in subDirs)
			{
				files = Directory.GetFiles(subdir);
				string[] childrenFiles = Array.FindAll<string>(files, file => ChildExt.Contains(Path.GetExtension(file)));
				relevantSubFiles.AddRange(childrenFiles);
			}

			// Check matchAll criteria for the children.
			if (!CheckMatchAll(relevantSubFiles, ChildExt, (bool)MatchAllChildren)) return null;

			// Create a match which has all the children and parent files in it.
			Match<string> match = new Match<string>();
			Array.ForEach<string>(parentFiles, file => match.Add(file));
			relevantSubFiles.ForEach(file => match.Add(file));

			// Return the match.
			return new List<IMatch<string>>(new Match<string>[] { match });
		}

		/// <summary>
		/// Check if the files match all the extension if this is required.
		/// </summary>
		/// <param name="files">List of files with matching extensions</param>
		/// <param name="ext">Set of all the extensions that may be found.</param>
		/// <param name="matchAll">Should we find at least one file for each extension or not?</param>
		/// <returns>True if the extensions are as expected, or false if they don't all match or not enough extensions
		/// are found.</returns>
		private bool CheckMatchAll(IEnumerable<string> files, ISet<string> extensions, bool matchAll)
		{
			// there's files and they shouldn't have all extensions so > true.
			if (files == null || files.Count() == 0) return false;
			if (!matchAll) return true;

			// otherwise, check the extensions.
			return extensions.All<string>(ext => files.Any<string>(file => Path.GetExtension(file) == ext));
		}


		/// <summary>
		/// Call this method when we are done processing a certain directory.
		/// </summary>
		/// <returns>All matches that have been found after processing the directory, but have not
		/// yet been returned yet. Or null if no such matches have been found.</returns>
		public List<IMatch<string>> EndMatchingSession()
		{
			return null;
		}

		/// <summary>
		/// Call this method if there are no more items to be inspected 
		/// for matching. 
		/// </summary>
		/// <returns>All matches that have been discovered but have not yet been returned. Or null if 
		/// no such matches have been found.</returns>
		public List<IMatch<string>> EndMatching()
		{

			return null; 
		}
		#endregion

		//
		// Helper functions
		// 
		/// <summary>
		/// Checks if the class is correctly initialized, otherwise it throws an exception that cancels the operation
		/// that was in execution.
		/// </summary>
		private void RequireInitialized()
		{
			if(MatchAllChildren == null || MatchAllParents == null || 
				(ParentExt == null || ChildExt == null) || 
				ParentExt.Count == 0 || ChildExt.Count == 0)
			{
				throw new OperationCanceledException("Matching policy requires Extension and MatchBoolean initialization before matching is possible. " 
					+ "Please initialize class.");
			}
		}
	}
}
