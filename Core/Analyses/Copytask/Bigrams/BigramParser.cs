using System.Collections.Generic;
using System.Text;

namespace InputLog.Core.Analyses.Copytask.Bigrams
{
    /// <summary>
    /// BigramParser can parse bigrams.
    /// This means parsing strings to a set of all the bigrams occuring within 
    /// that specific bigram, or taking a set of inputlog events and determining
    /// whether they are a bigram or not, and if they are, what bigram they form together.
    /// </summary>
    internal class BigramParser
    {
        /// <summary>
        /// Take a string and find all the possible bigrams in the list. A character
        /// following or followed by whitespace is ignored. 
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <returns>The set of all the bigrams encountered.</returns>
        public static HashSet<string> FindBigrams(string input)
        {
            HashSet<string> bigrams = new HashSet<string>();

            // We need at least 2 characters to find bigrams.
            if (input.Length < 2)
            {
                return bigrams;
            }

            StringBuilder sb = new StringBuilder(2);
            char first =  input[0];
            for (int i = 1; i < input.Length; ++i)
            {
                char second = input[i];

                // If one of the characters is not a letter or digit, we can't have a bigram
                // combination, so we just skip these characters.
                //
                if (!char.IsLetterOrDigit(first) || !char.IsLetterOrDigit(second))
                {
                    first = second;
                    continue;
                }

                sb.Append(first);
                sb.Append(second);
                bigrams.Add(sb.ToString());
                sb.Clear();

                first = second;
            }
            return bigrams;
        }
    }
}
