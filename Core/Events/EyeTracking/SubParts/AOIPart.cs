using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	/// <summary>
	/// Keeps track of the Area-of-interest hits.
	/// </summary>
	public class AOIPart: ISubPart
	{
		/// <summary>
		/// For each AOI this part tells whether a certain EventPart has a hit for 
		/// a given AOI or not.
		/// </summary>
		public Dictionary<string, string> AOIHits { get; private set; }

		/// <summary>
		/// Map that contains the different AOI's available in the current tobii file's
		/// indexMap, and their index in the data arrays.
		/// </summary>
		private static Dictionary<string, int> AOIIndexMap;

		public AOIPart()
		{
			AOIHits = new Dictionary<string, string>();
		}

		/// <summary>
		/// Reset the AOI index map that gets constructed per tobii file.
		/// This should be reset before every new tobii file is parsed.
		/// </summary>
		public static void ResetAOIs()
		{
			AOIIndexMap = null;
		}

		/// <summary>
		/// Add an area of interest hit to the AOIPart.
		/// </summary>
		/// <param name="aoi">Area of Interest</param>
		/// <param name="hit">Hit is "1" if the aoi has been hit, or "0" if it has not been hit.</param>
		public void AddAOI(string aoi, string hit)
		{
			AOIHits.Add(aoi, hit);
		}

		//
		// ISubPart Interface implementation
		#region ISubPart

		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (AOIIndexMap == null)
			{
				ConstructAOIMap(index);
			}
			foreach (string key in AOIIndexMap.Keys)
			{
				AOIHits.Add(key, data[AOIIndexMap[key]]);
			}
		}

		/// <summary>
		/// Detect all AOI hits, and saves them in a separate indexMap for
		/// performance considerations
		/// </summary>
		/// <param name="index">The full index map of the tobii file.</param>
		private void ConstructAOIMap(Dictionary<string, int> index)
		{
			AOIIndexMap = new Dictionary<string, int>();
			foreach (string key in index.Keys)
			{
				if (key.StartsWith("AOI[") && key.EndsWith("]Hit"))
				{
					if (AOIIndexMap.ContainsKey(key))
					{
						AOIIndexMap[key] = index[key];
					}
					else
					{
						AOIIndexMap.Add(key, index[key]);
					}
				}
			}
		}

		public void Merge(ISubPart other)
		{
			AOIPart otherPart = other as AOIPart;
			if (otherPart == null)
			{
				throw new ArgumentException("AOIPart can only be merged with other AOIParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				foreach (KeyValuePair<string, string> pair in otherPart.AOIHits)
				{
					// We only have to overwrite 0's to 1's. We don't overwrite 1's.
					if (AOIHits[pair.Key] == "0")
					{
						AOIHits[pair.Key] = otherPart.AOIHits[pair.Key];
					}
				}
			}
		}

		public bool ContainsData()
		{
			return AOIHits.Count > 0;
		}

		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion
	}
}
