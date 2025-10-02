using System.Text.RegularExpressions;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    /// The tokens are defined by a regex rule.
    /// </summary>
    public class RegexDefinition
    {
        #region Fields
        public bool IsIgnored { get; private set; }
        public string Rule { get; private set; }
        public string Type { get; private set; }
        public Regex Regex { get; private set; } 
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The kind of regex rule</param>
        /// <param name="rule">The regex as a string</param>
        /// <param name="isIgnored">Boolean 'true' if this regex should not be executed. 
        /// If 'false' the regex is compiled.</param>
        public RegexDefinition(string type, string rule, bool isIgnored)
        {
            Type = type;
            Rule = rule;
            IsIgnored = isIgnored;
            if (!isIgnored) Regex = new Regex(Rule, RegexOptions.Compiled);
        }
    }
}

