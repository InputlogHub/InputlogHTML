using System.Collections.Generic;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{
    public abstract class ARegexRule<T>
    {

        private List<string> Actions;

        /// <summary>
        /// Concstructor
        /// </summary>
        /// <param name="actions">List of actions to perform after recognizing this rule</param>
        protected ARegexRule(List<string> actions)
        {
            Actions = actions;
        }

        public abstract Pair<T,T> AddToFSM(AFSMFactory<T> factory);

        /// <summary>
        /// Gets a list of actions to perform upon recognizing this rule.
        /// </summary>
        /// <returns>List of actions</returns>
        public List<string> GetActions()
        {
            return Actions ?? (Actions = new List<string>());
        }
    }

    #region UnaryRules
    public class SymbolRule<T> : ARegexRule<T>
    {

        private readonly List<Triple<string,bool,int>> Events;

        public SymbolRule(List<Triple<string,bool,int>> events, List<string> actions)
            : base(actions)
        {
            Events = events;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            return factory.MakeSymbol(Events, GetActions());
        }
    }
 
    public class ParenthesesRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule;

        public ParenthesesRule(ARegexRule<T> subRule, List<string> actions)
            : base(actions)
        {
            SubRule = subRule;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            Pair<T, T> pair = SubRule.AddToFSM(factory);
            return factory.MakeParentheses(pair.First, pair.Second, GetActions());
        }
    }

    public class KleeneStarRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule;

        public KleeneStarRule(ARegexRule<T> subRule, List<string> actions)
            : base(actions)
        {
            SubRule = subRule;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            Pair<T, T> pair = SubRule.AddToFSM(factory);
            return factory.MakeKleeneStar(pair.First, pair.Second, GetActions());
        }
    }

    public class PlusRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule;

        public PlusRule(ARegexRule<T> subRule, List<string> actions)
            : base(actions)
        {
            SubRule = subRule;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            Pair<T, T> mandatorySubRule = SubRule.AddToFSM(factory);
            Pair<T, T> optionalSubRule = SubRule.AddToFSM(factory);
            Pair<T, T> optionalKleeneStar = factory.MakeKleeneStar(optionalSubRule.First,
                optionalSubRule.Second, new List<string>());
            return factory.MakeConcat(mandatorySubRule.First, mandatorySubRule.Second,
                optionalKleeneStar.First, optionalKleeneStar.Second, GetActions());
        }
    }

    public class NumberRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule;
        private readonly int Number;

        public NumberRule(ARegexRule<T> subRule, int number, List<string> actions)
            : base(actions)
        {
            SubRule = subRule;
            Number = number;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            if (Number >= 1)
            {
                Pair<T, T> pair1 = SubRule.AddToFSM(factory);
                Pair<T, T> pair2;
                T start = pair1.First;

                for (int i = 1; i < Number; i++)
                {
                    pair2 = SubRule.AddToFSM(factory);
                    //Do not add the actions of this rule here, or they will be repeated _every_time_!
                    factory.MakeConcat(pair1.First, pair1.Second, pair2.First, pair2.Second, new List<string>());
                    //Shift to next pair
                    pair1 = pair2;
                }
                //Make epsilon-concatenation, to add actions of this rule
                pair2 = factory.MakeEpsilon(new List<string>());
                //Very condensed statement here: the end state of this rule is the end state of the concatenation of:
                //  * the several concatenations of the same rule (i.e. the subrule concatenated Number-1 times with itself)
                //  and 
                //  * an epsilon-transition
                T end = factory.MakeConcat(pair1.First, pair1.Second, pair2.First, pair2.Second, GetActions()).Second;
                return new Pair<T, T>(start, end);
            }
            // \todo What do we do?
            return new Pair<T, T>(default(T), default(T));
        }
    }


    #endregion

    #region BinaryRules
    public class ConcatRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule1;
        private readonly ARegexRule<T> SubRule2;

        public ConcatRule(ARegexRule<T> subRule1, ARegexRule<T> subRule2, List<string> actions)
            : base(actions)
        {
            SubRule1 = subRule1;
            SubRule2 = subRule2;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            Pair<T,T> pair1 = SubRule1.AddToFSM(factory);
            Pair<T,T> pair2 = SubRule2.AddToFSM(factory);
            return factory.MakeConcat(pair1.First, pair1.Second, pair2.First, pair2.Second, GetActions());
        }
    }

    public class UnionRule<T> : ARegexRule<T>
    {

        private readonly ARegexRule<T> SubRule1;
        private readonly ARegexRule<T> SubRule2;

        public UnionRule(ARegexRule<T> subRule1, ARegexRule<T> subRule2, List<string> actions)
            : base(actions)
        {
            SubRule1 = subRule1;
            SubRule2 = subRule2;
        }

        public override Pair<T, T> AddToFSM(AFSMFactory<T> factory)
        {
            Pair<T, T> pair1 = SubRule1.AddToFSM(factory);
            Pair<T, T> pair2 = SubRule2.AddToFSM(factory);
            return factory.MakeUnion(pair1.First, pair1.Second, pair2.First, pair2.Second, GetActions());
        }
    }
    #endregion
}
