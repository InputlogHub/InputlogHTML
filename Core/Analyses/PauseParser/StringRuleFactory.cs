using System.Collections.Generic;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{
    public class StringRuleFactory : IRuleFactory<string>
    {
        #region UnaryRules
        public ARegexRule<string> MakeSymbolRule(List<Triple<string,bool,int>> events, List<string> actions)
        {
            return new SymbolRule<string>(events, actions);
        }

        public ARegexRule<string> MakeParenthesesRule(ARegexRule<string> subRule, List<string> actions)
        {
            return new ParenthesesRule<string>(subRule, actions);
        }

        public ARegexRule<string> MakeKleeneStarRule(ARegexRule<string> subRule, List<string> actions)
        {
            return new KleeneStarRule<string>(subRule, actions);
        }

        public ARegexRule<string> MakePlusRule(ARegexRule<string> subRule, List<string> actions)
        {
            return new PlusRule<string>(subRule, actions);
        }

        public ARegexRule<string> MakeNumberRule(ARegexRule<string> subRule, int number, List<string> actions)
        {
            return new NumberRule<string>(subRule, number, actions);
        }
        
        #endregion

        #region BinaryRules
        public ARegexRule<string> MakeConcatRule(ARegexRule<string> subRule1, ARegexRule<string> subRule2, List<string> actions)
        {
            return new ConcatRule<string>(subRule1, subRule2, actions);
        }

        public ARegexRule<string> MakeUnionRule(ARegexRule<string> subRule1, ARegexRule<string> subRule2, List<string> actions)
        {
            return new UnionRule<string>(subRule1, subRule2, actions);
        }
        #endregion
        
    }

}
