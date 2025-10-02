using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using InputLog.Core.IO.Txt.CsvReader;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// Matching the target words (words with the correct orthography) with the
    /// produced words. Using Damerau-Levenshtein to calculate the distance between
    /// taget and production.
    /// </summary>
    public class TokenTarget
    {
        #region Fields
        /// <summary>
        /// Dictionary with Tokens.
        /// </summary>
        private readonly Dictionary<int, Token> TokenDictionary;
        /// <summary>
        /// List of target words from a csv-file.
        /// </summary>
        private List<string> Columns;
        /// <summary>
        /// Target words not linked to any word in the produced text.
        /// </summary>
        public static HashSet<string> MissedTargets { get; private set; }
        /// <summary>
        /// Counting the number of successfully matched target words.
        /// </summary>
        public static int MatchCount { get; private set; }
        /// <summary>
        /// The maximum acceptable Damerau-Levenshtein distance between target and word.
        /// </summary>
        public static int WordDiff { get; private set; }
        /// <summary>
        /// The file name of the csv-file used in this session.
        /// </summary>
        public static string CsvFile { get; private set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary">TokenDictionary, a structure holding all Token instances.</param>
        /// <param name="diff">The Levenshtein distance.</param>
        public TokenTarget(Dictionary<int, Token> dictionary, int diff = 1)
        {
            TokenDictionary = dictionary;
            WordDiff = diff;
            MatchCount = 0;   
        }

        /// <summary>
        /// Reading a user selected csv-file with target words for this log session.
        /// </summary>
        /// <param name="csvFile"></param>
        private void ReadCsvFile(string csvFile)
        {
            // Columns in the csv-file
            Columns = new List<string>();
            MissedTargets = new HashSet<string>();

            // Converting the TokenDictionary values (Token) into an index accessible array
            int dictSize = TokenDictionary.Count;
            ICollection tValues = TokenDictionary.Values;
            var tokenValues = new Token[dictSize];
            tValues.CopyTo(tokenValues, 0);

            using (var reader = new CsvFileReader(csvFile))
            {
                while (reader.ReadRow(Columns))
                {
                    // When the target is eventually found, it will be removed from this set.
                    MissedTargets.Add(Columns[1]);

                    // Extracting three tokens to match the three columns in the csv-file.
                    // These columns hold: a word to the left of the target, the target itself, 
                    // and a word to the right of the target.
                    for (int i = 0; i < dictSize - 3; i++)
                    {                 
                        if (null == tokenValues[i]) continue;
                        var token0 = tokenValues[i];
                        if (null == tokenValues[i + 1] ) continue;
                        var token1 = tokenValues[i + 1];
                        if (null == tokenValues[i + 2]) continue;
                        var token2 = tokenValues[i + 2];
                        
                        var word0 = Regex.Replace(token0.Reconstruction, @"\p{P}", "");
                        var word1 = Regex.Replace(token1.Reconstruction, @"\p{P}", "");
                        var word2 = Regex.Replace(token2.Reconstruction, @"\p{P}", "");

                        // First try: the target word matches the reconstructed word right away
                        // when both words are equally spelled or because the distance <= WordDiff characters.
                        if (word1.Trim().ToLower().Equals(Columns[1]) 
                            || word1.Trim().ToLower().DistanceTo(Columns[1]) <= WordDiff)
                        {
                            token1.TargetWord = Columns[1];
                            // When successful, remove token from the tokenValue array.
                            tokenValues[i] = null;
                            // Counting the successful matches.
                            MatchCount++;
                            MissedTargets.Remove(Columns[1]);
                            break;
                        }

                        // Second try: the words preceding and following the target should both 
                        // match or the distance of the words <= WordDiff characters.
                        if ((word0.Trim().ToLower().Equals(Columns[0]) 
                            || word0.Trim().ToLower().DistanceTo(Columns[0]) <= WordDiff)
                            && (word2.Trim().ToLower().Equals(Columns[2]) 
                            || word2.Trim().ToLower().DistanceTo(Columns[2]) <= WordDiff))                        
                        {
                            token1.TargetWord = Columns[1];
                            // When successful, remove token from the tokenValue array.
                            tokenValues[i + 1] = null;
                            // Counting the successful matches.
                            MatchCount++;
                            MissedTargets.Remove(Columns[1]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setting the target words for this session.
        /// </summary>
        /// <param name="csvPath">Path to the csv-file</param>
        public void SetTargetToken(string csvPath)
        {
            CsvFile = Path.GetFileName(csvPath);
            ReadCsvFile(csvPath);
        }
    }
}