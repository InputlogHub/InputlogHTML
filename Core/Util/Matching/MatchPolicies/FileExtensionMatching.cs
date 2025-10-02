using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InputLog.Core.Util.Matching.MatchPolicies
{
	/// <summary>
	/// Matching policy that matches files based on their extensions. 
	/// The resulting Matches are IMatch&lt;string&gt;. This is a match that presents a unique
	/// list of all the files that match each other. They are selected solely based on their extensions.<br />
	/// The idea is that all files matching the extensions that are in the same folder match the each other.
	/// </summary>
	public class FileExtensionMatching: IMatchPolicy<string, string>
	{
		#region private_fields
		/// <summary>
		/// List of all the matches found so far, within this matching session.
		/// </summary>
		private ISet<string> TmpMatches = new HashSet<string>();
		#endregion

		#region public_fields
		/// <summary>
		/// Set of extensions to match on.
		/// </summary>
		public ISet<string> Extensions { get; set; }
		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public FileExtensionMatching() 
		{
			Extensions = new HashSet<string>();
		}

		/// <summary>
		/// Create the file extension matching policy, initialized with the
		/// extensions to match on.
		/// </summary>
		/// <param name="extensions">Group of extensions to match on.</param>
		public FileExtensionMatching(IEnumerable<string> extensions)
		{
			Extensions = new HashSet<string>(extensions);
		}

		/// <summary>
		/// Begin a match-finding session. For this policy it should be called upon
		/// entering each new folder.
		/// </summary>
		public void BeginMatchingSession()
		{
			RequireInitialized();
			TmpMatches.Clear();
		}

		/// <summary>
		/// Found a file within a folder. 
		/// </summary>
		/// <param name="item">A file has been found. Match it if possible</param>
		/// <returns>Null, this policy does not return any matches until the matching
		/// session has been ended.</returns>
		public List<IMatch<string>> FoundItem(string item)
		{
			RequireInitialized();

			// If the item has the correct extension we add it to the match.
			if(Extensions.Contains(Path.GetExtension(item)))
			{
				TmpMatches.Add(item);
			}

			return null;
		}

		/// <summary>
		/// End the current matching session and return all Matches that have been
		/// discovered.
		/// </summary>
		/// <returns>A list of all matches found during this matching session</returns>
		public List<IMatch<string>> EndMatchingSession()
		{
			if (TmpMatches.Count > 0)
			{
				IMatch<string> foundMatch = new Match<string>(TmpMatches);
				TmpMatches = new HashSet<string>();
				List<IMatch<string>> resultList = new List<IMatch<string>>(1);
				resultList.Add(foundMatch);

				return resultList;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// End all of the matching. If any matches remain to be returned, return them
		/// here.
		/// </summary>
		/// <returns>The list of all matches that have been found but have not been returned
		/// so far.</returns>
		public List<IMatch<string>> EndMatching()
		{
			return null;
		}

		/// <summary>
		/// Method that checks whether or not the extensions are already initialized or not.
		/// If they have not yet been initialized, we throw an exception. We can not start matching
		/// with our policy if we do not know which extensions to look for.
		/// </summary>
		private void RequireInitialized()
		{
			if (Extensions.Count == 0)
			{
				throw new OperationCanceledException("Can not start matching policy if no extensions have been provided for the policy to match on.");
			}
		}
	}
}
