using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{

    /// <summary>
    /// Class template representing an (epsilon non-deterministic) finite state machine (eNFSM) or finite automaton (eNDFA).
    /// </summary>
    /// <remarks>The determinism of this class is not guaranteed by the system, but left to the user.</remarks>
    /// <remarks>Methods to transform any automaton to an equivalent deterministic one are available, 
    /// but as of yet untested.</remarks>
    /// <remarks>When multiple transitions are possible, the first one encountered is followed. 
    /// The order of these transitions is equal to the order of addition.</remarks>
    public class NonDeterministicFiniteStateMachine<T> : AFiniteStateMachine<T>
    {

        private class PairComparer : IComparer<Pair<FSMTransition<T>, int>>
        {
            public int Compare(Pair<FSMTransition<T>, int> pair1, Pair<FSMTransition<T>,int> pair2)
            {
                return pair1.Second - pair2.Second;
            }
        }

        #region Fields

        /// <summary>
        /// Representation of the FSM itself, keeps transitions associated with a specific state
        /// The integer in the pair is a priorityfield.
        /// </summary>
        private readonly Dictionary<FSMState<T>, List<Pair<FSMTransition<T>,int>>> FSM;

        /// <summary>
        /// Cache for identifier-state translation
        /// </summary>
        private Dictionary<T, FSMState<T>> StateCache;

        /// <summary>
        /// Current state of the system
        /// </summary>
        private ISet<FSMState<T>> CurrentState;

        /// <summary>
        /// Epsilon-closure caches, helps to speed things up a bit, at the cost of memory.
        ///     Since most machines have sufficient memory, this is the way to go.
        /// </summary>
        private Dictionary<FSMState<T>, ISet<FSMState<T>>> EClosureCache;
        private Dictionary<FSMState<T>, List<Pair<FSMTransition<T>,int>>> ETransitionCache;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for type T</param>
        /// <remarks>Setting the equals-function to null results in the usage of the default equals-operator of type T.</remarks>
        public NonDeterministicFiniteStateMachine(IEqualityComparer<T> equalityComparer = null)
            : base(equalityComparer)
        {
            CurrentState = default(ISet<FSMState<T>>);
            FSM = new Dictionary<FSMState<T>, List<Pair<FSMTransition<T>, int>>>();
            EClosureCache = null;
            ETransitionCache = null;
        }

        /// <summary>
        /// Retrieves the current state of the FSM.
        /// </summary>
        /// <returns>Current state the FSM is in</returns>
        /// <remarks>If the current state is not set, the default value for the return type is returned.</remarks>
        /// <remarks>As this is a nondeterministic automaton, the state is actually a set of states.</remarks>
        public ISet<T> GetCurrentState()
        {

            if (CurrentState != default(ISet<T>))
            {
                ISet<T> states = new HashSet<T>(); 
                foreach(FSMState<T> state in CurrentState){
                    states.Add(state.GetIdentifier());
                }
                return states;
            }
            return default(ISet<T>);

        }

        /// <summary>
        /// Adds a state to this FSM.
        /// </summary>
        /// <param name="stateIdentifier">Identifier for state to be added</param>
        /// <param name="entryActions">Actions that should be performed whenever entering this state</param>
        /// <param name="exitActions">Actions that should be performed whenever exiting this state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <remarks>Does not overwrite if state already exists.</remarks>
        public override void AddState(T stateIdentifier, List<Action> entryActions = null, List<Action> exitActions = null,
            IEqualityComparer<T> equalityComparer = null)
        {
            if (!Contains(stateIdentifier, equalityComparer))
            {
                var state = new FSMState<T>(stateIdentifier, entryActions, exitActions);
                FSM.Add(state, new List<Pair<FSMTransition<T>,int>>());
            }
        }


        /// <summary>
        /// Adds a transition to this FSM.
        /// </summary>
        /// <param name="prevState">Previous state of the transition</param>
        /// <param name="events">Test predicates (aka trigger events): if these (all) hold, the transition is followed</param>
        /// <param name="nextState">State to transition to</param>
        /// <param name="actions">List of actions to perform _before_ transitioning</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, based on their identifier</param>
        /// <remarks>Precondition: Exists(prevState)</remarks>
        /// <remarks>Transition is not added when preconditions not met</remarks>
        /// <remarks>Action is performed _before_ transitioning to the nextState state</remarks>
        public override void AddTransition(T prevState, ISet<Func<bool>> events, T nextState, List<Action> actions,
            IEqualityComparer<T> equalityComparer = null)
        {
            AddTransition(prevState, events, nextState, actions, int.MaxValue, equalityComparer);
        }


        public void AddTransition(T prevState, ISet<Func<bool>> events, T nextState, List<Action> actions, int priority,
            IEqualityComparer<T> equalityComparer = null)
        {
            //Bundle into FSMTransitions
            var newTransition = new FSMTransition<T>(events, actions, nextState);

            List<Pair<FSMTransition<T>,int>> value;
            if (FSM.TryGetValue(FindState(prevState, equalityComparer), out value))
            {
                value.Add(new Pair<FSMTransition<T>,int>(newTransition, priority));
                FSM[FindState(prevState, equalityComparer)] = value;
            }
        }

        /// <summary>
        /// Start the FSM.
        /// </summary>
        public override void Start()
        {
            if (CurrentState == default(ISet<FSMState<T>>))
            {
                GoToStartState();
            }
        }

        /// <summary>
        /// Resets the FSM.
        /// </summary>
        public override void Reset()
        {
            GoToStartState();
        }

        protected void GoToStartState()
        {
            ISet<FSMState<T>> startStateEClosure = new HashSet<FSMState<T>> { StartState };
            if (EClosureCache != null && !EClosureCache.TryGetValue(StartState, out startStateEClosure))
            {
                startStateEClosure = ECLOSE(StartState);
                EClosureCache[StartState] = startStateEClosure;
            }
            else if (EClosureCache == null)
            {
                startStateEClosure = ECLOSE(StartState);
            }
            
            CurrentState = startStateEClosure;
            foreach (FSMState<T> state in CurrentState)
            {
                state.PerformEntryActions();
            }

        }

        /// <summary>
        /// Perform 1 transition. 
        /// This transition is chosen as the first viable transition, starting from the current state of the finite state machine. 
        /// (First: in order of addition; viable: trigger event set evaluates to true)
        /// The exit actions of the old state and actions of the chosen transition are executed (in this order) before transitioning.
        /// The entry actions of the new state are executed after transitioning.
        /// </summary>
        public override void PerformTransition()
        {
            var enabledTrans = new List<Pair<FSMTransition<T>, int>>();
            foreach (FSMState<T> state in CurrentState)
            {
                enabledTrans.AddRange(FindEnabledTransitions(state));
                state.PerformExitActions();
            }
            
            ISet<FSMState<T>> nextStateSet = FindNextStates(enabledTrans);
            if ((enabledTrans.Count > 0) && (nextStateSet != null && nextStateSet.Count > 0))
            {
                foreach (FSMState<T> state in nextStateSet)
                {
                    enabledTrans.AddRange(FindEpsilonTransitions(state));
                }
                var priorityQueue = new PriorityQueue<Pair<FSMTransition<T>, int>>
                    (enabledTrans, new PairComparer());
                while (!priorityQueue.IsEmpty())
                {
                    (priorityQueue.Dequeue()).First.PerformActions();
                }
                
                foreach (FSMState<T> state in nextStateSet)
                {
                    state.PerformEntryActions();
                }
                CurrentState = nextStateSet;
                
            }
            else
            {
                Reset();
            }

        }

        private ISet<FSMState<T>> FindNextStates(IEnumerable<Pair<FSMTransition<T>, int>> enabledTrans)
        {
            ISet<FSMState<T>> nextStates = new HashSet<FSMState<T>>();
            foreach (Pair<FSMTransition<T>, int> transPair in enabledTrans)
            {
                var nextState = FindState(transPair.First.GetNextState());
                if (nextState != null)
                {
                    ISet<FSMState<T>> nextStateEClosure = new HashSet<FSMState<T>> { nextState };
                    if(EClosureCache != null && !EClosureCache.TryGetValue(nextState, out nextStateEClosure)){
                        nextStateEClosure = ECLOSE(nextState);
                        EClosureCache[nextState] = nextStateEClosure;
                    }
                    else if (EClosureCache == null)
                    {
                        nextStateEClosure = ECLOSE(nextState);
                    }
                    nextStates.UnionWith(nextStateEClosure);
                }
            }
            return nextStates;
        }

        private List<Pair<FSMTransition<T>, int>> FindEpsilonTransitions(FSMState<T> state)
        {

            List<Pair<FSMTransition<T>, int>> epsilonTransitions = null;
            if(
                (ETransitionCache == null) || (ETransitionCache != null 
                && !ETransitionCache.TryGetValue(state, out epsilonTransitions))
              )
            {
                epsilonTransitions = FSM[state].FindAll(transPair => IsEpsilon(transPair.First)); 
                if (ETransitionCache != null)
                {
                    ETransitionCache[state] = epsilonTransitions;
                }
                
            }//else: correct value was looked up from cache and is now stored in epsilonTransitions
            return epsilonTransitions;
        }

        private IEnumerable<Pair<FSMTransition<T>, int>> FindEnabledTransitions(FSMState<T> currentState)
        {
            return FSM[currentState].FindAll(transPair => !IsEpsilon(transPair.First) 
                && transPair.First.EvaluateEvents());
        }

        /// <summary>
        /// Get all states in this finite automaton.
        /// </summary>
        /// <returns>Set containing the identifiers of all states</returns>
        protected internal override ISet<FSMState<T>> GetFSMStates()
        {
            return new HashSet<FSMState<T>>(FSM.Keys);
        }

        /// <summary>
        /// Get all transitions from a given state.
        /// </summary>
        /// <param name="state">Identifier of a state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <returns>List of all outgoing transitions from the state or null if the state does not exist</returns>
        protected internal override List<FSMTransition<T>> GetFSMTransitions(T state, IEqualityComparer<T> equalityComparer = null)
        {
            if (Contains(state, equalityComparer))
            {
                return FSM[FindState(state, equalityComparer)].Select(transPair => transPair.First).ToList();
            }
            return null;
        }

        /// <summary>
        /// Retrieves a string representation of the current state.
        /// </summary>
        /// <returns>String representation of the current state</returns>
        public override string CurrentStateToString()
        {
            return CurrentStateToString(item => item.ToString());
        }

        /// <summary>
        /// Retrieves a string representation of the current state.
        /// </summary>
        /// <param name="stateToStringFunc">Custom ToString function for the state identifiers</param>
        /// <returns>String representation of the current state</returns>
        public override string CurrentStateToString(Func<T,string> stateToStringFunc)
        {
            if (GetCurrentState() != default(ISet<T>))
            {
                return FSMUtil.IEnumerableToString(GetCurrentState(), stateToStringFunc);
            }
            return string.Empty;
        }

        /// <summary>
        /// Builds a cache of transition, speeding up the execution.
        /// </summary>
        public override void BuildCache()
        {
            //States
            StateCache = new Dictionary<T, FSMState<T>>();
            //Transitions
            EClosureCache = new Dictionary<FSMState<T>, ISet<FSMState<T>>>();
            ETransitionCache = new Dictionary<FSMState<T>, List<Pair<FSMTransition<T>,int>>>();
            foreach (FSMState<T> state in FSM.Keys)
            {
                StateCache[state.GetIdentifier()] = state;
                EClosureCache[state] = ECLOSE(state);
                ETransitionCache[state] = FindEpsilonTransitions(state);
            }

        }

        protected internal override FSMState<T> FindState(T identifier, IEqualityComparer<T> equalityComparer = null)
        {
            FSMState<T> state = null;
            if (StateCache != null && !StateCache.TryGetValue(identifier, out state))
            {
                state = base.FindState(identifier, equalityComparer);
                StateCache[identifier] = state;
            }
            else if (StateCache == null)
            {
                state = base.FindState(identifier, equalityComparer);
            }
            return state;
        }

        #region NFA2DFA

        /// <summary>
        /// Special Epsilon function, signifying an epsilon transition
        /// </summary>
        /// <returns>Always returns true</returns>
        public static bool Epsilon()
        {
            return true;
        }

        /// <summary>
        /// Tests whether the transition is an epsilon transition.
        /// </summary>
        /// <param name="trans">Transition</param>
        /// <returns>True when all trigger events of this transition are Epsilon, otherwise false</returns>
        internal static bool IsEpsilon(FSMTransition<T> trans)
        {
            Func<Func<bool>, bool> pred = evt => evt != Epsilon;
            return (trans.Events.FirstOrDefault(pred) == default(Func<bool>));
        }

        /// <summary>
        /// This functions returns the epsilon-closure of a specific state.
        /// Let "state" be a state in an eNFA, then the e-closure would be the following (recursive definition):
        ///     base-case:
        ///         ECLOSE(state) = {state}
        ///     recursion:
        ///          ECLOSE(state) = {all states t of eNFA where there exists an epsilon transition s=>t, with s in ECLOSE(state)}
        /// </summary>
        /// <param name="state">State identifier</param>
        /// <returns>Epsilon-closure of this state or null if a state with this identifier does not exist</returns>
        public ISet<T> ECLOSE(T state)
        {
            ISet<T> eClosureIdentifiers = null;
            if (Contains(state))
            {
                eClosureIdentifiers = new HashSet<T>();
                foreach (FSMState<T> ecloseState in ECLOSE(FindState(state)))
                {
                    eClosureIdentifiers.Add(ecloseState.GetIdentifier());
                }
            }
            return eClosureIdentifiers;
        }

        /// <summary>
        /// This functions returns the epsilon-closure of a specific state.
        /// Let "state" be a state in an eNFA, then the e-closure would be the following (recursive definition):
        ///     base-case:
        ///         ECLOSE(state) = {state}
        ///     recursion:
        ///          ECLOSE(state) = {all states t of eNFA where there exists an epsilon transition s=>t, with s in ECLOSE(state)}
        /// </summary>
        /// <param name="state">State</param>
        /// <returns>Epsilon-closure of this state</returns>
        /// <remarks>Precondition: Exists(state.GetIdentifer())</remarks>
        /// <remarks>Postcondition: Exist(s.GetIdentifier()) for all s in return value</remarks>
        private ISet<FSMState<T>> ECLOSE(FSMState<T> state){
            ISet<FSMState<T>> eClosure = new HashSet<FSMState<T>>();//Base case of recursive definition

            var workList = new Queue<FSMState<T>>();
            workList.Enqueue(state);
            while (workList.Count > 0) //While more states in the epsilon-closure of "state" are found
            {
                /*
                 * Robin Verschoren - 27/08/2013
                 * This list avoids modifying the eClosure set in the foreach-body.
                 * Not using this kind of trick (with a list or other collection), throws an InvalidOperationException 
                 *  and this seems to be one of the cleanest solutions (although it looks a bit silly).
                 */
                var candidateECloseStates = new List<FSMState<T>>();

                FSMState<T> workingState = workList.Peek();
                if (workingState != null && !eClosure.Contains(workingState))
                {
                    candidateECloseStates.AddRange(from transPair in FSM[workingState]
                                                   select transPair.First
                                                   into trans let nextState = FindState(trans.GetNextState()) 
                                                   where IsEpsilon(trans) && nextState != null 
                                                   && Contains(nextState.GetIdentifier()) 
                                                   && !eClosure.Contains(nextState) 
                                                   && !workList.Contains(nextState) select nextState);
                }


                foreach (FSMState<T> candidate in candidateECloseStates)
                {
                    if (!eClosure.Contains(candidate)) //If it is a state new to the closure
                    {
                        if (!workList.Contains(candidate))
                        {
                            workList.Enqueue(candidate);
                        }
                    }
                }
                eClosure.Add(workList.Dequeue());

            }
            
            return eClosure;
        }

        /// <summary>
        /// Returns the epsilon-closure of a set of states.
        /// This is simply the union of the e-closures of every state in the set.
        /// </summary>
        /// <param name="stateSet">A set of states</param>
        /// <param name="eClosureCache">Cache of e-closures (this avoids recomputation)</param>
        /// <returns>Epsilon-closure of the set of states</returns>
        public ISet<T> ECLOSE(ISet<T> stateSet, Dictionary<T,ISet<T>> eClosureCache = null)
        {
            ISet<T> eClosure = new HashSet<T>();
            foreach (T subState in stateSet)
            {
                ISet<T> eClosureCandidate = null;
                if (eClosureCache != null && !eClosureCache.TryGetValue(subState, out eClosureCandidate))
                {
                    eClosureCandidate = ECLOSE(subState);
                    eClosureCache.Add(subState, eClosureCandidate);
                }
                if (eClosureCandidate != null) eClosure.UnionWith(eClosureCandidate);
            }
            return eClosure;
        }

        /// <summary>
        /// Gathers all the actions that have to be performed when transitioning from a set of states to another.
        /// </summary>
        /// <param name="eventsSet">Event set that could trigger a transition</param>
        /// <param name="nextStateSet">Set of states that will become the curent state after transitioning</param>
        /// <returns>List of actions that should be performed when transitioning</returns>
        public List<Action> DetermineActionList(ISet<Func<bool>> eventsSet, ISet<T> nextStateSet)
        {
            return DetermineActionList(null, eventsSet, nextStateSet);
        }

        /// <summary>
        /// Gathers all the actions that have to be performed when transitioning from a set of states to another.
        /// </summary>
        /// <param name="states">Current set of states</param>
        /// <param name="eventsSet">Event set that could trigger a transition</param>
        /// <param name="nextStateSet">Set of states that will become the curent state after transitioning</param>
        /// <returns>List of actions that should be performed when transitioning</returns>
        public List<Action> DetermineActionList(ISet<T> states, ISet<Func<bool>> eventsSet, ISet<T> nextStateSet)
        {
            if (states == null) throw new ArgumentNullException("states");
            var actionList = new List<Action>();
            foreach (var trans in from s in states 
                                  select FindState(s) 
                                  into state
                                  where state != null 
                                  from transPair in FSM[state] 
                                  select transPair.First into trans 
                                  where nextStateSet.Contains(trans.GetNextState()) 
                                  && (FSMUtil.SameContent(trans.Events, eventsSet)
                                  || IsEpsilon(trans)) 
                                  where trans != null && trans.Actions != null select trans)
            {
                actionList.AddRange(trans.Actions);
            }
            return actionList;
        }

        /// <summary>
        /// Determines the next state, based on current set of states, if all predicates from the event set hold true.
        /// </summary>
        /// <param name="states">Current set of states of the system</param>
        /// <param name="eventSet">Set of events</param>
        /// <returns>Next system state after transitioning</returns>
        public ISet<T> DetermineNextStateSet(ISet<T> states, ISet<Func<bool>> eventSet)
        {
            ISet<T> nextStateSet = new HashSet<T>();
            foreach (var trans in from s in states 
                                  select FindState(s) 
                                  into state where state != null 
                                  from transPair in FSM[state] 
                                  select transPair.First into trans
                                  where FSMUtil.SameContent(trans.Events, eventSet) select trans)
            {
                nextStateSet.Add(trans.GetNextState());
            }
            return nextStateSet;
        }

        /// <summary>
        /// Gather all viable events from a specific set of states.
        /// </summary>
        /// <param name="states">Set of states</param>
        /// <returns>A set of event sets that can trigger a transition from one of the states in the set</returns>
        public ISet<ISet<Func<bool>>> GatherViableEvents(ISet<T> states)
        {
            ISet<ISet<Func<bool>>> eventsSet = new HashSet<ISet<Func<bool>>>();

            foreach (FSMState<T> state in states.Select(s => FindState(s)).Where(state => state != null))
            {
                for (int index = 0; index < FSM[state].Count; index++)
                {
                    Pair<FSMTransition<T>, int> transPair = FSM[state][index];
                    FSMTransition<T> trans = transPair.First;
                    if (!IsEpsilon(trans) && !FSMUtil.IEnumerableContainsSet(eventsSet, trans.Events))
                    {
                        eventsSet.Add(trans.Events);
                    }
                }
            }
            return eventsSet;
        }

        #endregion
    }
}
