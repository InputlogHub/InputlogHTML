using System.Collections.Generic;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{
    public interface IRuleFactory<T>
    {
        #region UnaryRules
        ARegexRule<T> MakeSymbolRule(List<Triple<string,bool,int>> events, List<string> actions);

        ARegexRule<T> MakeParenthesesRule(ARegexRule<T> subRule, List<string> actions);

        ARegexRule<T> MakeKleeneStarRule(ARegexRule<T> subRule, List<string> actions);

        ARegexRule<T> MakePlusRule(ARegexRule<T> subRule, List<string> actions);

        ARegexRule<T> MakeNumberRule(ARegexRule<T> subRule, int number, List<string> actions);

        #endregion

        #region BinaryRules
        ARegexRule<T> MakeConcatRule(ARegexRule<T> subRule1, ARegexRule<T> subRule2, List<string> actions);
            
        ARegexRule<T> MakeUnionRule(ARegexRule<T> subRule1, ARegexRule<T> subRule2, List<string> actions);
        #endregion
    }

}
