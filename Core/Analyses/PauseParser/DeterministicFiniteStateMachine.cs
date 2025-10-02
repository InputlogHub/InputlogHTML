using System;
using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Analyses.PauseParser
{

    /// <summary>
    /// Class template representing a deterministic finite state machine (DFSM) or finite automaton (DFA).
    /// </summary>
    /// <remarks>The determinism of this class is guaranteed by the system, by blocking duplicate additions.</remarks>
    public class DeterministicFiniteStateMachine<T> : AFiniteStateMachine<T>
    {

        #region Fields

        /// <summary>
        /// Representation of the FSM itself, keeps transitions associated with a specific state
        /// </summary>
        private readonly Dictionary<FSMState<T>, List<FSMTransition<T>>> FSM;

        /// <summary>
        /// Current state of the system
        /// </summary>
        private FSMState<T> CurrentState;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for type T</param>
        /// <remarks>Setting the equals-function to null results in the usage of the default equals-operator of type T.</remarks>
        public DeterministicFiniteStateMachine(IEqualityComparer<T> equalityComparer = null)
            : base(equalityComparer)
        {
            CurrentState = null;
            FSM = new Dictionary<FSMState<T>, List<FSMTransition<T>>>();
        }

        /// <summary>
        /// Retrieves the current state of the FSM.
        /// </summary>
        /// <returns>Current state the FSM is in</returns>
        /// <remarks>If the current state is not set, the default value for type T is returned.</remarks>
        public T GetCurrentState()
        {
            if (CurrentState != null)
            {
                return CurrentState.GetIdentifier();
            }
            return default(T);

        }

        /// <summary>
        /// Adds a state to this FSM.
        /// </summary>
        /// <param name="stateIdentifier">Identifier for state to be added</param>
        /// <param name="entryActions">Actions that should be performed whenever entering this state</param>
        /// <param name="exitActions">Actions that should be performed whenever exiting this state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, based on their identifier</param>
        /// <remarks>Does not overwrite if state already exists.</remarks>
        public override void AddState(T stateIdentifier, List<Action> entryActions = null, List<Action> exitActions = null,
            IEqualityComparer<T> equalityComparer = null)
        {
            if (!Contains(stateIdentifier, equalityComparer))
            {
                var state = new FSMState<T>(stateIdentifier, entryActions, exitActions);
                FSM.Add(state, new List<FSMTransition<T>>());
            }
        }

        /// <summary>
        /// Adds a transition to this FSM.
        /// </summary>
        /// <param name="prevState">Previous state of the transition</param>
        /// <param name="events">Test predicates (aka trigger events): if these (all) hold, the transition is followed</param>
        /// <param name="nextState">State to transition to</param>
        /// <param name="actions">List of actions to perform _before_ transitioning</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <remarks>Precondition: Exists(prevState)</remarks>
        /// <remarks>Transition is not added when preconditions not met</remarks>
        /// <remarks>Action is performed _before_ transitioning to the nextState state</remarks>
        /// <remarks>As this is a deterministic automaton, adding an already existing transition
        ///  (i.e. transitions containing the same trigger eventset and next state) will be rejected entirely.</remarks>
        public override void AddTransition(T prevState, ISet<Func<bool>> events, T nextState, 
            List<Action> actions, IEqualityComparer<T> equalityComparer = null)
        {
            if (Contains(prevState, equalityComparer))
            {
                //Bundle into FSMTransitions
                var newTransition = new FSMTransition<T>(events, actions, nextState);

                Func<FSMTransition<T>, bool> predicate = trans => trans != null 
                    && FSMUtil.SameContent(trans.Events, events);

                var oldTransition = GetFSMTransitions(prevState).FirstOrDefault(predicate);

                if(oldTransition != default(FSMTransition<T>))//If an old transition on the same trigger events was found
                {
                    //Don't add the new one, because this is a deterministic automaton!
                    return;
                }

                List<FSMTransition<T>> value;

                if (FSM.TryGetValue(FindState(prevState, equalityComparer), out value))
                {
                    value.Add(newTransition);
                    FSM[FindState(prevState, equalityComparer)] = value;
                }
            }
        }

        /// <summary>
        /// Starts the automaton.
        /// </summary>
        public override void Start()
        {
            //If we somehow ended up in some kind of default
            if (CurrentState == default(FSMState<T>))
            {
                GoToStartState();
            }
        }

        /// <summary>
        /// Go to the start state
        /// </summary>
        private void GoToStartState()
        {
            if(StartState != null)
            {
                StartState.PerformEntryActions();
                CurrentState = StartState;
            }
        }

        /// <summary>
        /// Resets to the original start state.
        /// </summary>
        public override void Reset()
        {
            GoToStartState();
        }

        /// <summary>
        /// Perform 1 transition. 
        /// This transition is chosen as the first viable transition, starting from the current state 
        /// of the finite state machine. (First: in order of addition; viable: trigger event set evaluates to true)
        /// The exit actions of the old state and actions of the chosen transition are executed (in this order)
        /// before transitioning. The entry actions of the new state are executed after transitioning.
        /// </summary>
        public override void PerformTransition()
        {
            FSMTransition<T> transition = FindViableTransition(CurrentState);
            if (transition != null)
            {
                FSMState<T> nextState = FindState(transition.GetNextState());
                if (nextState != null)
                {
                    CurrentState.PerformExitActions();
                    transition.PerformActions();
                    CurrentState = nextState;
                    nextState.PerformEntryActions();
 
                    return;
                }
            }
            Reset();
            
        }

        /// <summary>
        /// Finds the transition to be followed from the current state, based on the transition tests 
        /// (given when adding transitions).This transition is the first transition, 
        /// going from the given state to any other state, that is viable.
        /// (First: in order of addition; viable: trigger event set evaluates to true)
        /// </summary>
        /// <param name="currentState">Current state</param>
        /// <returns>Transition to be followed</returns>
        private FSMTransition<T> FindViableTransition(FSMState<T> currentState)
        {
            if (currentState != null && FSM.ContainsKey(currentState))
            {
                var transList = FSM[currentState];
                return transList.FirstOrDefault(t => t.EvaluateEvents());
            }
            return null;

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
                return FSM[FindState(state, equalityComparer)];
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
        /// <param name="stateToStringFunc">Custom ToString function for a specific type</param>
        /// <returns>String representation of the current state</returns>
        public override string CurrentStateToString(Func<T,string> stateToStringFunc)
        {
            return null != GetCurrentState() ? stateToStringFunc.Invoke(GetCurrentState()) : string.Empty;
        }

        /// <summary>
        /// Builds a cache of transition, speeding up the execution.
        /// </summary>
        public override void BuildCache()
        {
            /*
             * Nothing to do here.
             */
        }

        #region NDFA2DFA
        /**
         * The functions within this region implement the construction of equivalent deterministic 
         * finite automata/state machines (DFA/DFSM) out of (epsilon) nondeterministic finite 
         * automata/state machines (eNFA/eNFSM).
         * 
         * Essentially, this consists of grouping states in sets (subsets of the set of states of the eNFA).
         * Additional care should be taken when dealing with epsilon transitions. 
         * These are transitions requiring no events to trigger them.
         * This is called the powerset or subset construction (https://en.wikipedia.org/wiki/Powerset_construction)
         */

        /// <summary>
        /// Constructs a DFA from an eNFA.
        /// </summary>
        /// <param name="endfa">Epsilon nondeterministic finite automaton</param>
        /// <returns>Equivalent deterministic finite automaton</returns>
        /// <remarks>This implementation uses the powerset or subset construction</remarks>
        public static DeterministicFiniteStateMachine<ISet<T>> ConstructDFA(NonDeterministicFiniteStateMachine<T> endfa)
        {
            if (endfa == null)
            {
                return null;
            }

            //Newly constructed DFA
            var dfa = new DeterministicFiniteStateMachine<ISet<T>>(new SetComparer<T>());

            //Sort of caches all eclosures, to avoid recomputation
            var eClosureCache = new Dictionary<T, ISet<T>>();

            T startState = endfa.GetStartState();

            if (startState == null || !endfa.Contains(startState))
            {
                return dfa;
            }

            //1)Startstate q_D = ECLOSE(q0) (q0 is startstate of the endfa)
            T q0 = startState;
            ISet<T> qD = endfa.ECLOSE(q0);
            AddSetState(dfa, endfa, qD);
            dfa.SetStartState(qD);
            eClosureCache.Add(q0, qD);

            //2)F_D = {all subsets of stateset of endfa containing at least 1 accepting state}
            dfa.SetEndPredicate(endfa.EndPredicate);

            //3)d_D(S,a) is computed as follows:
            //  *Let S = {p1,...,p_k}
            //  *Compute {r1,..,rm} = union d_E(p_i,a)   (i=1,...,k)
            //  *d_D(S,a) = ECLOSE({r1,r2,...,rm})
            var workList = new Queue<ISet<T>>();//Simulates a queue
            workList.Enqueue(qD);

            while (workList.Count > 0)
            {
                //Subset of states of endfa
                ISet<T> states = workList.Peek();
                
                if (dfa.FindState(states, new SetComparer<T>()) != null)
                {
                    foreach (ISet<Func<bool>> viableEventSet in endfa.GatherViableEvents(states))
                    {
                        //Get transition details for 1 possible event set
                        ISet<T> nextStateSet = endfa.ECLOSE(endfa.DetermineNextStateSet(states, viableEventSet), eClosureCache);
                        List<Action> actionList = endfa.DetermineActionList(states, viableEventSet, nextStateSet);

                        //Add the transition itself
                        dfa.AddTransition(states, viableEventSet, nextStateSet, actionList);
                        
                        //Avoids computing transitions for _all_ subsets of the original set of states
                        //Instead,only consider states that are accessible from the 
                        // current set of subsets of the original DFA-stateset
                        if (!dfa.Contains(nextStateSet))
                        {
                            AddSetState(dfa, endfa, nextStateSet);
                            if (!FSMUtil.IEnumerableContainsSet(workList, nextStateSet))
                            {
                                workList.Enqueue(nextStateSet);
                            }
                        }
                    }
                }
                workList.Dequeue();
            }
            return dfa;
        }

        /// <summary>
        /// Adds a subset of states from an eNFA to a DFA that is being constructed.
        /// </summary>
        /// <param name="dfa">Detereministic finite automaton</param>
        /// <param name="endfa">Epsilon nondeterministic finite automaton</param>
        /// <param name="stateSet">Set of states to add to the DFA</param>
        private static void AddSetState(DeterministicFiniteStateMachine<ISet<T>> dfa, 
            NonDeterministicFiniteStateMachine<T> endfa, ISet<T> stateSet)
        {
            var entryActions = new List<Action>();
            var exitActions = new List<Action>();

            foreach (T subState in stateSet)
            {
                if (endfa.Contains(subState))
                {
                    //Add all the right entry/exit actions too
                    entryActions.AddRange(endfa.GetEntryActions(subState));
                    exitActions.AddRange(endfa.GetExitActions(subState));
                    //Add epsilon-transition actions as entry actions of this state
                    foreach (FSMTransition<T> trans in endfa.GetFSMTransitions(subState))
                    {
                        if (NonDeterministicFiniteStateMachine<T>.IsEpsilon(trans) && stateSet.Contains(trans.GetNextState()))
                        {
                            if (trans.Actions != null)
                            {
                                entryActions.AddRange(trans.Actions);
                            }
                        }
                    }
                }

            }
            dfa.AddState(stateSet, entryActions, exitActions);
        }
        
        #endregion

    }
}
