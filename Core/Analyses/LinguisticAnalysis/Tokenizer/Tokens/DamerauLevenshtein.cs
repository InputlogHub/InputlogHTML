using System;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// Damerau-Levenshtein distance is a metric defining the difference between two strings,
    /// given by counting the minimum number of operations needed to transform one string into the other,
    /// where an operation is defined as an insertion, deletion, or substitution of a single character,
    /// or a transposition of two adjacent characters.
    /// http://mihkeltt.blogspot.be/2009/04/dameraulevenshtein-distance.html
    /// </summary>
    public static class DamerauLevenshtein
    {
        public static int DistanceTo(this string @string, string targetString)
        {
            return Distance(@string, targetString);
        }

        private static int Distance(string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1))
            {
                return !string.IsNullOrEmpty(string2) ? string2.Length : 0;
            }
            if (string.IsNullOrEmpty(string2))
            {
                return !string.IsNullOrEmpty(string1) ? string1.Length : 0;
            }
            var length1 = string1.Length;
            var length2 = string2.Length;
            var d = new int[length1 + 1,length2 + 1];
            for (var i = 0; i <= d.GetUpperBound(0); i++) d[i, 0] = i;
            for (var i = 0; i <= d.GetUpperBound(1); i++) d[0, i] = i;
            for (var i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (var j = 1; j <= d.GetUpperBound(1); j++)
                {
                    var cost = string1[i - 1] == string2[j - 1] ? 0 : 1;
                    var del = d[i - 1, j] + 1;
                    var ins = d[i, j - 1] + 1;
                    var sub = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(del, Math.Min(ins, sub));
                    if (i > 1 && j > 1 && string1[i - 1] == string2[j - 2] && string1[i - 2] == string2[j - 1])
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
    }
}