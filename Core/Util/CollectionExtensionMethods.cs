using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InputLog.Core.Util
{

    /// <summary>
    /// Class provinding some extension methods for C# Collections.
    /// </summary>
    public static class CollectionExtensionMethods
    {

        /// <summary>
        /// Extension method that provides filter capabilities to System.Collections.Generic.List.
        /// Concretely, this method iterates over the list and verifies a predicate (StatefullPredicate) for each 
        /// element in the list. If the predicate is not met for a given element, it is removed from the list.
        /// Otherwise, it stays in the list.<br />
		/// THIS FUNCTION ALTERS THE INPUT ORIGINAL LIST!
        /// </summary>
        /// <typeparam name="T">Type of the elements in the list.</typeparam>
        /// <param name="list">The list on which this extension method operates.</param>
        /// <param name="predicate">The predicate to verify on each element in the list.</param>
        /// <returns>The list on which this extension method operates.</returns>
        public static List<T> Filter<T>(this List<T> list, IStatefullPredicate<T> predicate)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (predicate.Check(list[i]))
                {
                    i++;
                }
                else
                {
                    list.RemoveAt(i);
                }

            }
            return list;
        }

		/// <summary>
		/// Extension method that provides filter capabilities to System.Collections.Generic.List.
		/// Concretely, this method iterates over the list and verifies a predicate (StatefullPredicate) for each 
		/// element in the list. If the predicate is not met for a given element, it is removed from the list.
		/// Otherwise, it stays in the list.<br />
		/// THIS FUNCTION _DOES NOT_ ALTER THE ORIGINAL INPUT LIST! BUT IT DOES NOT MAKE A DEEP COPY EITHER!
		/// </summary>
		/// <typeparam name="T">Type of the elements in the list.</typeparam>
		/// <param name="list">The list on which this extension method operates. This list is not altered</param>
		/// <param name="predicate">The predicate to verify on each element in the list.</param>
		/// <returns>A non-deep copy of the list containing all the element that have not been filtered out.</returns>
		public static List<T> FilterWithoutAltering<T>(this List<T> list, IStatefullPredicate<T> predicate)
		{
			List<T> returnList = new List<T>();
			foreach (T item in list)
			{
				if (predicate.Check(item))
				{
					returnList.Add(item);
				}
			}
			return returnList;
		}

        /// <summary>
        /// Extension method that adds an element to a given list and returns the list 
        /// (the default Add method does not return the list).
        /// </summary>
        /// <typeparam name="T">Type of the elements in the list.</typeparam>
        /// <param name="list">The list on which this extension method operates.</param>
        /// <param name="obj">The element that needs to be added to the list.</param>
        /// <returns>The list on which this extension method operates.</returns>
        public static List<T> AddEx<T>(this List<T> list, T obj)
        {
            list.Add(obj);
            return list;
        }

		/// <summary>
		/// Merge two dictionaries. The "source" is merged into "target" by replacing any values that already existed in 
		/// the target with it's own values. Any values from source that did not yet exist in target are added to the dictionary.
		/// </summary>
		/// <typeparam name="TKey">Type of the keys</typeparam>
		/// <typeparam name="TValue">Type of the values</typeparam>
		/// <param name="target">Target dictionary for the merging. This dictionary may have been altered after the merging.</param>
		/// <param name="source">Source dictionary to merge into the "target". On key collision the source's value will
		/// overwrite the target's original value.</param>
		/// <returns>The target dictionary with the values of the source dictionary merged into it. The target
		/// dictionary will be altered.</returns>
		public static IDictionary<TKey, TValue> Merge<TKey,TValue>(this IDictionary<TKey, TValue> target, 
            IDictionary<TKey, TValue> source)
		{
			if (target == null)
			{
				throw new ArgumentNullException("Target dictionary may not be null.");
			}

			if (source == null)
			{
				return target;
			}

			foreach (KeyValuePair<TKey, TValue> pair in source)
			{
				if (target.ContainsKey(pair.Key))
				{
					target[pair.Key] = pair.Value;
				}
				else
				{
					target.Add(pair.Key, pair.Value);
				}
			}

			return target;
		}

        public static bool SameContents<KT,VT>(this IDictionary<KT, VT> lhs, IDictionary<KT, VT> rhs)
        {
            if (lhs.Keys.Count != rhs.Keys.Count) return false;
            HashSet<KT> keys = new HashSet<KT>(lhs.Keys);
            keys.UnionWith(rhs.Keys);
            foreach (KT key in keys)
            {
                if (!(lhs.ContainsKey(key) && rhs.ContainsKey(key))) return false;
                if (!lhs[key].Equals(rhs[key])) return false;
            }
            return true;
        }

        public static bool SameContents<VT>(this List<VT> lhs, List<VT> rhs)
        {
            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; i++)
            {
                if (!(lhs[i].Equals(rhs[i]))) return false;
            }
            return true;
        }

        public static void FromSerialString(this IDictionary<string, string> target, string s)
        {
            string pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(s, pattern);
            foreach (Match m in matches)
            {
                string entry = m.Groups[1].ToString();
                string[] parts = entry.Split(';');
                string key = parts[0];
                string value = parts[1];
                target[key] = value;
            }
        }

        public static void FromSerialString<T>(this IDictionary<string, T> target, string s, Func<string, T> f)
        {
            string pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(s, pattern);
            foreach (Match m in matches)
            {
                string entry = m.Groups[1].ToString();
                string[] parts = entry.Split(';');
                string key = parts[0];
                string value = parts[1];
                target[key] = f(value);
            }
        }

        public static string ToSerialString(this IDictionary<string, string> target)
        {
            StringBuilder res = new StringBuilder();
            foreach (string key in target.Keys)
            {
                res.Append("{" + key + ";" + target[key] + "}");
            }
            return res.ToString();
        }

        public static string ToSerialString<T>(this IDictionary<string, T> target, Func<T,string> f)
        {
            StringBuilder res = new StringBuilder();
            foreach (string key in target.Keys)
            {
                res.Append("{" + key + ";" + f(target[key]) + "}");
            }
            return res.ToString();
        }

    }
}
