using System;
using System.Collections.Generic;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{
    /// <summary>
    /// Class template factory to build finite state machines.
    /// </summary>
    /// <typeparam name="T">Type of the state identifiers of the finite state machines that are built</typeparam>
    public abstract class AFSMFactory<T>
    {
        #region Fields
        
        /// <summary>
        /// Built finite state machine
        /// </summary>
        /// <remarks>This is strictly an epsilon nondeterministic FSM (eNFSM), 
        /// as Thompson's construction algorithm (regex->FSM) is built on epsilon transitions, 
        /// only found in eNFSMs.</remarks>
        private NonDeterministicFiniteStateMachine<T> FSM;
        
        #endregion

        #region AbstractMethods

        /// <summary>
        /// Finds a predicate based on a name or identifier.
        /// </summary>
        /// <param name="name">Identifier of the predicate</param>
        /// <param name="inverse">Indicates whether the inverse of the function has to be taken or not</param>
        /// <param name="peekPosition">Indicates whether we need to inspect the current event 
        /// or one of the following events</param>
        /// <returns>Specified predicate or null</returns>
        /// <remarks>Peekposition defaults to 0, the current event. When its value is i > 0, 
        /// the ith next event will be considered.</remarks>
        protected abstract Func<bool> FindPredicate(string name, bool inverse = false, int peekPosition = 0);

        /// <summary>
        /// Finds an action based on a name or identifier.
        /// </summary>
        /// <param name="name">Identifier of the action</param>
        /// <returns>Specified action or null</returns>
        protected abstract Action FindAction(string name);

        ///<summary>
        ///Generates a unique identifier.
        ///</summary>
        ///<returns>A unique identifier</returns>
        protected abstract T GenerateIdentifier();

        #endregion

        #region Public interface
        /// <summary>
        /// Start building a new finite state machine, as opposed to adding rules to a previous machine.
        /// </summary>
        public void NewFiniteStateMachine()
        {
            FSM = new NonDeterministicFiniteStateMachine<T>();
        }

        /// <summary>
        /// Builds the specified finite state machine.
        /// </summary>
        /// <returns>Finite state machine as it was specified</returns>
        public NonDeterministicFiniteStateMachine<T> BuildFiniteStateMachine()
        {
            return FSM;
        }

        #region TCA
        /***************************************************************************
         *                                                                         * 
         * The following functions help to implement Thompson's construction       *
         *  algorithm (TCA).                                                       *
         *  For more information:                                                  *
         *	(https://en.wikipedia.org/wiki/Thompson%27s_construction_algorithm)    *
         *	                                                                       *
         *                                                                         *
         * It should not be necessary to implement new rules here.                 *
         *                                                                         *
         ***************************************************************************/

        /// <summary>
        /// Constructs a rule that recognizes a symbol.
        /// </summary>
        /// <param name="events">Events that recognize the symbol</param>
        /// <param name="actionList">Actions to perform when symbol is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        public Pair<T,T> MakeSymbol(ICollection<Triple<string,bool,int>> events, ICollection<string> actionList)
        {
            T start = GenerateIdentifier();
            T end = GenerateIdentifier();
            AddState(start);
            AddState(end);
            AddTransition(start, FindPredicates(events), end, FindActions(actionList));
            return new Pair<T, T>(start, end);
        }

        /// <summary>
        /// Constructs a rule that recognizes epsilon.
        /// </summary>
        /// <param name="actionList">Actions to perform when epsilon is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        public Pair<T,T> MakeEpsilon(ICollection<string> actionList)
        {
            T start = GenerateIdentifier();
            T end = GenerateIdentifier();
            AddState(start);
            AddState(end);
            AddTransition(start, MakeEpsilon(), end, FindActions(actionList));
            return new Pair<T, T>(start, end);
        }

        /// <summary>
        /// Constructs a rule that recognizes the Kleene star (i.e. arbitrary number of repetitions) of a subexpression.
        /// </summary>
        /// <param name="start">Startstate identifier of subexpression</param>
        /// <param name="end">Endstate identifier of subexpression</param>
        /// <param name="actionList">Actions to perform when the expression is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        /// <remarks>Due to the construction of the Kleene star, the specified actions
        ///  will be executed every time the loop is visited.</remarks>
        public Pair<T, T> MakeKleeneStar(T start, T end, ICollection<string> actionList)
        {
            
            T newStart = GenerateIdentifier();
            T endOfRule = GenerateIdentifier();
            AddState(newStart);
            AddState(endOfRule);

            AddTransition(newStart, MakeEpsilon(), start, null);
            AddTransition(newStart, MakeEpsilon(), endOfRule, null);
            AddTransition(end, MakeEpsilon(), start, null);
            AddTransition(end, MakeEpsilon(), endOfRule, null);

            //Modified from TCA to better accomodate the actions we use
            //  Otherwise, action would be executed every loop
            T newEnd = GenerateIdentifier();
            AddState(newEnd);
            AddTransition(endOfRule, MakeEpsilon(), newEnd, FindActions(actionList));


            return new Pair<T,T>(newStart, newEnd);
            
        }

        /// <summary>
        /// Constructs a rule that recognizes a subexpression that was surrounded by parentheses.
        /// </summary>
        /// <param name="start">Startstate identifier of subexpression</param>
        /// <param name="end">Endstate identifier of subexpression</param>
        /// <param name="actionList">Actions to perform when the expression is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        public Pair<T, T> MakeParentheses(T start, T end, ICollection<string> actionList)
        {
            T newEnd = GenerateIdentifier();
            AddState(newEnd);

            AddTransition(end, MakeEpsilon(), newEnd, FindActions(actionList));

            return new Pair<T, T>(start, newEnd);
        }

        /// <summary>
        /// Constructs a rule that recognizes the concatenation of 2 subexpressions.
        /// </summary>
        /// <param name="start1">Startstate identifier of the first subexpression</param>
        /// <param name="end1">Endstate identifier of the first subexpression</param>
        /// <param name="start1">Startstate identifier of the second subexpression</param>
        /// <param name="end1">Endstate identifier of the second subexpression</param>
        /// <param name="actionList">Actions to perform when the expression is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        public Pair<T, T> MakeConcat(T start1, T end1, T start2, T end2, ICollection<string> actionList)
        {
            T start = GenerateIdentifier();
            T end = GenerateIdentifier();
            AddState(start);
            AddState(end);

            AddTransition(start, MakeEpsilon(), start1, null);
            AddTransition(end1, MakeEpsilon(), start2, null);
            AddTransition(end2, MakeEpsilon(), end, FindActions(actionList));

            return new Pair<T, T>(start, end);
        }

        /// <summary>
        /// Constructs a rule that recognizes the union of 2 subexpressions.
        /// </summary>
        /// <param name="start1">Startstate identifier of the first subexpression</param>
        /// <param name="end1">Endstate identifier of the first subexpression</param>
        /// <param name="start1">Startstate identifier of the second subexpression</param>
        /// <param name="end1">Endstate identifier of the second subexpression</param>
        /// <param name="actionList">Actions to perform when the expression is recognized</param>
        /// <returns>Pair of startstate and endstate identifiers</returns>
        public Pair<T,T> MakeUnion(T start1, T end1, T start2, T end2, ICollection<string> actionList)
        {
            T start = GenerateIdentifier();
            T end = GenerateIdentifier();
            AddState(start);
            AddState(end);

            AddTransition(start, MakeEpsilon(), start1, null);
            AddTransition(start, MakeEpsilon(), start2, null);

            AddTransition(end1, MakeEpsilon(), end, FindActions(actionList));
            AddTransition(end2, MakeEpsilon(), end, FindActions(actionList));

            return new Pair<T,T>(start, end);
        }
        #endregion
        #endregion

        #region Helper

        ///<summary>
        ///Generates an event set for transitions that only contains the epsilon-event.
        ///</summary>
        ///<returns>An event set containing only epsilon</returns>
        private ISet<Func<bool>> MakeEpsilon()
        {
            return new HashSet<Func<bool>>() { NonDeterministicFiniteStateMachine<T>.Epsilon };
        }

        /// <summary>
        /// Adds a transition to the finite state machine.
        /// </summary>
        /// <param name="prevState">State from which the transition starts</param>
        /// <param name="events">Trigger events (requirements) that trigger the transition if all evaluate to true</param>
        /// <param name="nextState">State to go to after transitioning</param>
        /// <param name="actionList">List of actions to perform after following the transition</param>
        private void AddTransition(T prevState, ISet<Func<bool>> events, T nextState, List<Action> actionList)
        {
            if (FSM != null)
            {
                FSM.AddTransition(prevState, events, nextState, actionList);
            }
        }

        /// <summary>
        /// Adds a state to the finite state machine.
        /// </summary>
        /// <param name="stateIdentifier">State identifier</param>
        /// <param name="entryActions">Actions to perform when entering the state</param>
        /// <param name="exitActions">Actions to perform when exiting the state</param>
        private void AddState(T stateIdentifier, List<Action> entryActions = null, List<Action> exitActions = null)
        {
            if (FSM != null)
            {
                FSM.AddState(stateIdentifier, entryActions, exitActions);
            }
        }

        /// <summary>
        /// Generates a set of boolean-returning functions from a list of strings
        /// </summary>
        /// <param name="nameInverseList">List of methodnames,inverse-indications and peekpositions</param>
        /// <returns>Set of predicates corresponding to the names of the methods</returns>
        /// <seealso cref="FindPredicate"/>
        private ISet<Func<bool>> FindPredicates(ICollection<Triple<string,bool,int>> nameInverseList)
        {
            ISet<Func<bool>> eventSet = new HashSet<Func<bool>>();
            if (nameInverseList != null)
            {
                foreach (Triple<string, bool, int> methodNameInversePeekTriple in nameInverseList)
                {
                    string methodName = methodNameInversePeekTriple.First;
                    bool inverse = methodNameInversePeekTriple.Second;
                    int peekPosition = methodNameInversePeekTriple.Third;
                    Func<bool> evt = FindPredicate(methodName, inverse, peekPosition);
                    if (evt != null)
                    {
                        eventSet.Add(evt);
                    }
                }
            }
            return eventSet;
        }

        /// <summary>
        /// Generates a list of Actions from a list of strings
        /// </summary>
        /// <param name="nameList">List of methodnames</param>
        /// <returns>List of Actions corresponding to the names of the methods</returns>
        private List<Action> FindActions(ICollection<string> nameList)
        {
            List<Action> actionList = new List<Action>();
            if (nameList != null)
            {
                foreach (string methodName in nameList)
                {
                    Action act = FindAction(methodName);
                    if (act != null)
                    {
                        actionList.Add(act);
                    }
                }
            }
            return actionList;
        }

        #endregion

        
    }
}
