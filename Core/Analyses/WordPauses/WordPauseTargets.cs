using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.WordPauses
{
    public class WordPauseTargets
    {
        #region Fields

        /// <summary>
        /// Dictionary with Tokens.
        /// </summary>
        private readonly Dictionary<int, Token> TokenDictionary;

        /// <summary>
        /// Target words not linked to any word in the produced text.
        /// </summary>
        public static HashSet<string> UnusedTargets { get; private set; }
        public static HashSet<string> MatchedTargets { get; private set; }
        /// <summary>
        /// Counting the number of successfully matched target words.
        /// </summary>
        public static int MatchCount { get; private set; }

        /// <summary>
        /// The maximum acceptable Damerau-Levenshtein distance between target and word.
        /// </summary>
        public static int WordDiff { get; private set; }

        /// <summary>
        /// The file name of the csv-file with target words used in this session.
        /// </summary>
        public static string TargetFile { get; private set; }

        /// <summary>
        /// The file name of the csv-file target words assigned to the participants.
        /// </summary>y>
        public static string ParticipantFile { get; private set; }
        public static string[] ParticipantTargets { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="diff"></param>
        public WordPauseTargets(Dictionary<int, Token> dictionary, int diff)
        {
            TokenDictionary = dictionary;
            WordDiff = diff;
            MatchCount = 0;
        }

        /// <summary>
        /// Reading user selected csv-files with target words for this log session.
        /// </summary>
        /// <param name="csvFiles">One path to the targets available for this test and one with a list of paricipants.</param>
        /// <param name="participant">The author of this idfx.</param>
        private void ReadCsvFile(string[] csvFiles, string participant)
        {
            UnusedTargets = new HashSet<string>();
            MatchedTargets = new HashSet<string>();
            char[] delimiters = {';'};
            bool participantFound = false;

            // Getting the target words assigned to this participant.
            ParticipantTargets = new string[] {};
            var allParticipantTargets = new List<string[]>();
            using (var participantReader = new StreamReader(csvFiles[1]))
            {
                while (true)
                {
                    string line = participantReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    line = Regex.Replace(line, @"\s+", "");
                    if(line.Contains(participant))
                    {
                        participantFound = true;
                    }
                    allParticipantTargets.Add(line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            // Converting the TokenDictionary values (Token) into an index accessible array
            int dictSize = TokenDictionary.Count;
            ICollection tValues = TokenDictionary.Values;
            var tokenValues = new Token[dictSize];

            tValues.CopyTo(tokenValues, 0);

            using (var targetReader = new StreamReader(csvFiles[0]))
            {
                while (true)
                {
                    string line = targetReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string[] targets = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // First adding all targets, then when a target is found, it will be removed from this set.
                    foreach (var t in targets)
                    {
                        UnusedTargets.Add(t);
                    }

                    // Reconstructed words get a target id: '0' is not a target word; '1' is a correct target; 
                    // '2' is a variant of the target; '3' a target word from a different participant set.
                    for (int i = 0; i < dictSize; i++)
                    {
                        if (null == tokenValues[i]) continue;
                        Token token0 = tokenValues[i];
                        token0.TargetWord = "0";
                       
                        string word0 = Regex.Replace(token0.Reconstruction, @"\p{P}", "").ToLower().Trim();

                        // First try: the reconstructed word is found in the set of target words.
                        if (Array.FindIndex(targets, x => x == word0) > -1)
                        {
                            // The first word in the targets array has the official target name.
                            // - The word matches exactly the first target and is also part of the particpant target set '1'.
                            // - The word matches exactly the target but is part of another participant's target set '3'. 
                            // - The word is a variant of the target '2'
                            // - The word is identified with the Damerau-Levensthein method: 4.
                            if (targets[0].Equals(word0))
                            {
                                foreach (string[] parts in allParticipantTargets)
                                {
                                    if (parts[0].Equals(participant))
                                    {
                                        ParticipantTargets = parts;
                                        if (Array.FindIndex(ParticipantTargets, x => x == word0) > -1)
                                        {
                                            token0.TargetWord = "1";
                                            break;
                                        }
                                    }
                                    if (Array.FindIndex(parts, x => x == word0) > -1)
                                    {
                                        token0.TargetWord = "3";
                                    }
                                }
                            }
                            else
                            {
                                token0.TargetWord = "2";
                            }

                            // Counting the successful matches.
                            MatchedTargets.Add(word0);
                            MatchCount++;
                            // When successful, remove token from the tokenValue array.
                            tokenValues[i] = null;
                            // If a target was found, it is removed from the 'UnusedTargets' set.
                            foreach (var t in targets)
                            {
                                UnusedTargets.Remove(t);
                            }
                        }

                        if (WordDiff <= 0) continue;
                        {
                            // Second try: the distance of the words <= WordDiff characters.
                            if (targets.Any(t => word0.DistanceTo(t) <= WordDiff))
                            {
                                token0.TargetWord = "4";
                                // When successful, remove token from the tokenValue array.
                                tokenValues[i] = null;
                                // Counting the successful matches.
                                MatchedTargets.Add(word0);
                                MatchCount++;
                                // If a target was found, it is removed from the 'UnusedTargets' set.
                                foreach (var t in targets)
                                {
                                    UnusedTargets.Remove(t);
                                }
                            }
                        }
                    }
                }
            }

            // Participant not found in this csv-file.
            if (ParticipantTargets.IsNullOrEmpty())
            {
                if (participantFound && TokenDictionary.IsNullOrEmpty())
                {
                    throw new WordPauseTargetException("Participant '" + participant
                                                  + "' and csv-file '" +
                                                  StringUtils.ShortenPathname(csvFiles[1], 50) 
                                                  + "' available but reconstruction failed.");
                }
                throw new WordPauseTargetException("Could not find participant '" + participant
                                                   + "' in the given csv-file '" +
                                                   StringUtils.ShortenPathname(csvFiles[1], 50));
            }
        }

        /// <summary>
        /// Setting the target words for this session.
        /// </summary>
        /// <param name="csvPaths">Array with one path to the targets available for this test and one with a list of paricipants</param>
        /// <param name="participant">Author of this idfx</param>
        public void SetTargetToken(string[] csvPaths, string participant)
        {
            TargetFile = Path.GetFileName(csvPaths[0]);
            ParticipantFile = Path.GetFileName(csvPaths[1]);
            ReadCsvFile(csvPaths, participant);
        }
    }

    public class WordPauseTargetException : Exception
    {
        public WordPauseTargetException(string message) :
            base(message)
        {
        }
    }
}