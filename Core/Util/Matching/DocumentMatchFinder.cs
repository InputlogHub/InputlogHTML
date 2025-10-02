using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util.Matching.MatchPolicies;
using InputLog.Core.Util.Matching.TraversalPolicies;
using System.IO;
using System.Threading;

namespace InputLog.Core.Util.Matching
{
	/// <summary>
	/// Class that finds sets of documents that match together in some way or another 
	/// according to a specifed IMatchPolicy. The top directory is traversed for to find 
	/// matches using a specified ITraversalPolicy. <br />
	/// The end result of the match searching is a list of IMatch's that have been found.<br />
	/// <br />
	/// <br />
	/// The DocumentMatchFinder starts a new MatchingSession on each directory entry, and ends it when
	/// the directory has been completed. It passes the path to the directory to the matching policy to
	/// discover matches. <b>Make sure that the traversal and the matching policy are compatible with this
	/// method of working. </b>
	/// <br />
	/// </summary>
	/// <typeparam name="M">The traversal policy parameter. Note that it must the input types it receives to traverse
	/// on must be strings. These strings are paths to folders.</typeparam>
	/// <typeparam name="T">The matching policy paramter. The input type for a match is a string, and the match
	/// output type is a list of strings.</typeparam>
	public class DocumentMatchFinder<T,M> 
		where T: ITraversalPolicy<string>, new()
		where M: IMatchPolicy<string, string>, new()
	{
		#region private_fields
		/// <summary>
		/// List of all the encountered matches.
		/// </summary>
		List<IMatch<string>> Matches;
		#endregion

		#region public_fields
		/// <summary>
		/// An instance of the traversal policy.
		/// </summary>
		public T Traverser;

		/// <summary>
		/// An instance of the matching policy.
		/// </summary>
		public M Matcher;
		#endregion

		/// <summary>
		/// Construct the document match finder. The default constructor may only be used
		/// if both policies have a default constructor that can fully initialize the 
		/// the policies without any further parameters.
		/// </summary>
		public DocumentMatchFinder()
		{
			Traverser = new T();
			Matcher = new M();
			Matches = new List<IMatch<string>>();
		}

		/// <summary>
		/// Create the document matcher with given traversalPolicy and matchPolicy. 
		/// Both policies have already been created and initialized.
		/// </summary>
		/// <param name="traversalPolicy">Policy used for traversing the document trees.</param>
		/// <param name="matchPolicy">Policy for finding matches while traversing the trees.</param>
		public DocumentMatchFinder(T traversalPolicy, M matchPolicy)
		{
			Traverser = traversalPolicy;
			Matcher = matchPolicy;
			Matches = new List<IMatch<string>>();
		}

		/// <summary>
		/// Find all matches that can be discovered starting from the startPath using the
		/// given traversal and match policy.
		/// </summary>
		/// <param name="startPath">Path to the directory to start searching for matches</param>
		/// <returns>A list of all matches encountered.</returns>
		public List<IMatch<string>> GetMatches(string startPath)
		{
			Traverser.StartPoint = startPath;

			int attemptCounter = 0;
			const int maxAttempts = 5;
			bool done = false;

			while (attemptCounter < maxAttempts && !done)
			{
				try
				{
					foreach (string dir in Traverser)
					{
						// Match for every subdirectory found by the traverser.
						Matcher.BeginMatchingSession();
						AddToMatchesIfNotNull(Matcher.FoundItem(dir));
						AddToMatchesIfNotNull(Matcher.EndMatchingSession());
					}
					AddToMatchesIfNotNull(Matcher.EndMatching());
					done = true;
				}
				// If the reading does not function at one point, try again after a little while.
				catch (IOException ioexc)
				{
					if (attemptCounter == maxAttempts)
					{
						throw ioexc;
					}

					attemptCounter++;
					Matches.Clear();

					// Sleep for 100ms to give other applications that might be busy accessing
					// the files a chance to free their acquired resources before we try again.
					Thread.Sleep(100);
				}
			}

			return Matches;
		}

		/// <summary>
		/// Add the found matches to the list of all Matches, unless if they are 
		/// null.
		/// </summary>
		/// <param name="tmpMatches">Matches to be added to the Matches, if parameter
		/// is null, nothing will happen.</param>
		private void AddToMatchesIfNotNull(List<IMatch<string>> tmpMatches)
		{
			if (tmpMatches != null && tmpMatches.Count > 0)
			{
				Matches.AddRange(tmpMatches);
			}
		}
	}
}