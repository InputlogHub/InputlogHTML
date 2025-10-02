using System;
using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
	internal static class TobiiUtilities
	{
		/// <summary>
		/// Renames all duplicate AOI hits names. This is done be appending _*,
		/// where * is the number of occurences for that header name.
		/// </summary>
		/// <param name="originalHeaders">The original array of all headers.</param>
		/// <returns>An array of headers containing without duplicate AOI hits.</returns>
		public static string[] RenameDuplicateAOIs(string[] originalHeaders)
		{
			var newHeaders = new string[originalHeaders.Length];
			var count = 0;
			var headerOccurenceCounter = new Dictionary<string, int>();

			var patternHeaders = new[] 
            { 
				new[] { "AOI[", "]Hit" },
				new[] { "[", "]Value" }
            };

			foreach (string header in originalHeaders)
			{
				int oldCount = count;

				foreach (string[] patternedHeader in patternHeaders)
				{
					string prefixPattern = patternedHeader[0];
					string postfixPattern = patternedHeader[1];

					// probably an AOI hit - header.
				    if (!header.StartsWith(prefixPattern)) continue;
				    var tokenParts = header.Split(new[] { prefixPattern, postfixPattern }, StringSplitOptions.None);
				    if (originalHeaders.Count(token => token.Equals(header)) > 1)
				    {
				        // The token has a duplicate, rename this one by appending _*,
				        // where * is the number of previous encounters of this token.
				        if (!headerOccurenceCounter.ContainsKey(header))
				        {
				            headerOccurenceCounter.Add(header, 0);
				        }
				        headerOccurenceCounter[header]++;
				        newHeaders[count++] = prefixPattern 
				                              + tokenParts[1] 
				                              + "_OCC" 
				                              + headerOccurenceCounter[header].ToString() 
				                              + postfixPattern;
				    }
				    else
				    {
				        newHeaders[count++] = header;
				    }
				}

				// The header was unmatched by any prefixPattern and thus not stored yet in the new headers array.
				if (oldCount == count)
				{
					newHeaders[count++] = header;
				}
			}

			return newHeaders;
		}

	}
}
