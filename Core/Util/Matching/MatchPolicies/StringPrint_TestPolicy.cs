using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace InputLog.Core.Util.Matching.MatchPolicies
{
	public class StringPrint_TestPolicy: IMatchPolicy<string, string>
	{

		#region IMatchPolicy<string,string> Members

		public void BeginMatchingSession()
		{
			// nothing
		}

		public List<IMatch<string>> FoundItem(string item)
		{
			Debug.Print(item);
			return null;
		}

		public List<IMatch<string>> EndMatchingSession()
		{
			return null;
		}

		public List<IMatch<string>> EndMatching()
		{
			return null;
		}

		#endregion
	}
}
