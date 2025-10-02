using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.PauseParser
{

    /// <summary>
    /// Class of general utility functions for this compilation unit.
    /// </summary>
    public class FSMUtil
    {
        /// <summary>
        /// Checks whether 2 sets have the same contents.
        /// </summary>
        /// <param name="set1">First set</param>
        /// <param name="set2">Second set</param>
        /// <param name="comparer">Custom equality comparer. The default comparer is used when 
        /// this argument is null or missing.</param>
        /// <returns>True when they both have the same contents, otherwise false</returns>
        public static bool SameContent<T>(ISet<T> set1, ISet<T> set2, IEqualityComparer<T> comparer = null)
        {
            if(set1.Count == set2.Count)
            {
                return set1.All(it1 => set2.Contains(it1, comparer));
            }
            return false;
        }

        /// <summary>
        /// Transforms elements of a list from one type to another
        /// </summary>
        /// <typeparam name="IT">Input type</typeparam>
        /// <typeparam name="OT">Output Type</typeparam>
        /// <param name="transform">Transformation function</param>
        /// <param name="inList">Input list</param>
        /// <returns>List containing transformed elements</returns>
        /// <remarks>The order of elements is preserved.</remarks>
        public static List<OT> TransformList<IT, OT>(Func<IT, OT> transform, List<IT> inList)
        {
            var outList = new List<OT>();
            TransformCollection(transform, inList, outList);
            return outList;
        }

        /// <summary>
        /// Transforms elements of a set from one type to another
        /// </summary>
        /// <typeparam name="IT">Input type</typeparam>
        /// <typeparam name="OT">Output Type</typeparam>
        /// <param name="transform">Transformation function</param>
        /// <param name="inSet">Input set</param>
        /// <returns>Set containing transformed elements</returns>
        public static HashSet<OT> TransformHashSet<IT, OT>(Func<IT, OT> transform, HashSet<IT> inSet)
        {
            var outSet = new HashSet<OT>();
            TransformCollection<IT, OT>(transform, inSet, outSet);
            return outSet;
        }

        /// <summary>
        /// Transforms elements of a generic collection and adds them to another generic collection
        /// </summary>
        /// <typeparam name="IT">Input type</typeparam>
        /// <typeparam name="OT">Output type</typeparam>
        /// <param name="transform">Transformation function</param>
        /// <param name="inColl">Input collection, containing elements of type IT</param>
        /// <param name="outColl">Output collection; transformed elements are added here by 
        /// invoking Add on this collection</param>
        /// <remarks>If inColl or outColl are null, no actions will be performed.</remarks>
        public static void TransformCollection<IT, OT>(Func<IT, OT> transform, ICollection<IT> inColl, 
            ICollection<OT> outColl)
        {
            if (inColl != null && outColl != null)
            {
                foreach (IT inItem in inColl)
                {
                    OT outItem = transform.Invoke(inItem);
                    outColl.Add(outItem);
                }

            }
        }

        /// <summary>
        /// Checks whether a given type implements IEnumerable.
        /// </summary>
        /// <param name="type">The type of an object</param>
        /// <returns>True if IEnumerable is listed as interface of this type, otherwise false</returns>
        public static bool IsIEnumerable(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        /// <summary>
        /// Prints the contents of an IENumerable to a string.
        /// </summary>
        /// <param name="enumerable">IEnumerable object</param>
        /// <returns>String representation of the contents</returns>
        public static string IEnumerableToString(IEnumerable enumerable)
        {
            string description = "{";
            //Count items
            int count = enumerable.Cast<object>().Count();
            //Make description
            int it = 0;
            foreach (object item in enumerable)
            {
                description += item.ToString();
                if (it < count - 1)
                {
                    description += ",";
                }
                it++;
            }
            description += "}";
            return description;
        }

        /// <summary>
        /// Prints the contents of an IENumerable to a string.
        /// </summary>
        /// <typeparam name="S">Type of the items in the IEnumerable</typeparam>
        /// <param name="enumerable">IEnumerable object</param>
        /// <returns>String representation of the contents</returns>
        public static string IEnumerableToString<S>(IEnumerable<S> enumerable)
        {
            return IEnumerableToString<S>(enumerable, (S item) => item.ToString());
        }


        /// <summary>
        /// Prints the contents of an IENumerable to a string.
        /// </summary>
        /// <typeparam name="S">Type of the items in the IEnumerable</typeparam>
        /// <param name="enumerable">IEnumerable object</param>
        /// <param name="typeToStringFunc">Custom ToString function for the items in the IEnumerable</param>
        /// <returns>String representation of the contents</returns>
        public static string IEnumerableToString<S>(IEnumerable<S> enumerable, Func<S,string> typeToStringFunc)
        {
            string description = "{";
            //Count items
            int count = enumerable.Count();
            
            //Make description
            int it = 0;
            foreach (S item in enumerable)
            {
                description += typeToStringFunc(item);
                if (it < count - 1)
                {
                    description += ",";
                }
                it++;
            }
            description += "}";
            return description;
        }

        /// <summary>
        /// Checks whether a set is an element of an IEnumerable of sets.
        /// </summary>
        /// <typeparam name="S">Type of set elements</typeparam>
        /// <param name="workList">Collection of sets of a specific type</param>
        /// <param name="querySet">Set that is queried</param>
        /// <returns>True if any set in the IEnumerable has exactly the same contents as the query</returns>
        /// <remarks>This also returns true if any set contained in the IEnumerable has the
        ///  same contents as the queryset.</remarks>
        /// <remarks>It does not check for the exact same object being in the set.</remarks>
        public static bool IEnumerableContainsSet<S>(IEnumerable<ISet<S>> workList, ISet<S> querySet)
        {
            return workList.Any(s => SameContent(s, querySet));
        }
    }

    /// <summary>
    /// This class implements an IEqualityComparer for sets.
    /// The special behaviour of this class is that it checks the contents of the sets.
    /// </summary>
    /// <typeparam name="T">Type of the set contents</typeparam>
    /// <remarks>Implemented according to guidelines at MSDN (http://msdn.microsoft.com/en-us/library/ms132151.aspx)</remarks>
    public class SetComparer<T> : IEqualityComparer<ISet<T>>
    {
        private readonly IEqualityComparer<T> ItemComparer;

        public SetComparer(IEqualityComparer<T> itemComparer = null)
        {
            ItemComparer = itemComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(ISet<T> set1, ISet<T> set2)
        {
            return FSMUtil.SameContent<T>(set1,set2, ItemComparer);
        }

        public int GetHashCode(ISet<T> set)
        {
            int sum = set.Sum(elem => elem.GetHashCode());
            return sum.GetHashCode();
        }

    }

    /// <summary>
    /// Class template representing an (epsilon non-deterministic) finite state machine (eNFSM) or finite automaton (eNDFA).
    /// </summary>
    /// <remarks>The determinism of this class is not guaranteed by the system, but left to the user.</remarks>
    /// <remarks>Methods to transform any automaton to an equivalent deterministic one are available, 
    /// but as of yet untested.</remarks>
    /// <remarks>When multiple transitions are possible, the first one encountered is followed. 
    /// The order of these transitions is equal to the order of addition.</remarks>
    public abstract class AFiniteStateMachine<T>
    {
        /// <summary>
        /// Class template representing a state in a finite state machine. 
        /// With each state, a (unique) identifier is associated.
        /// </summary>
        /// <typeparam name="ST">Identifier type</typeparam>
        protected internal class FSMState<ST>
        {
            /// <summary>
            /// Identifier/key to find this state with
            /// </summary>
            private readonly ST Identifier;

            /// <summary>
            /// List of actions to be performed when entering this state
            /// </summary>
            private List<Action> EntryActions;

            /// <summary>
            /// List of actions to be performed when exiting this state
            /// </summary>
            private List<Action> ExitActions;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="identifier">Unique identifier for this state</param>
            /// <param name="entryActions"></param>
            /// <param name="exitActions"></param>
            public FSMState(ST identifier, List<Action> entryActions = null, List<Action> exitActions = null)
            {
                this.Identifier = identifier;
                SetEntryActions(entryActions);
                SetExitActions(exitActions);
            }

            /// <summary>
            /// Returns this state's identifier
            /// </summary>
            /// <returns>Identifier of this state</returns>
            public ST GetIdentifier()
            {
                return Identifier;
            }

            /// <summary>
            /// Sets the entry actions for this state
            /// </summary>
            /// <param name="entryActions">Actions to be performed when entering the state</param>
            private void SetEntryActions(List<Action> entryActions)
            {
                if (entryActions != null)
                {
                    EntryActions = entryActions;
                }
                else
                {
                    EntryActions = new List<Action>();
                }
            }


            /// <summary>
            /// Gets the entry actions for this state
            /// </summary>
            /// <returns>Actions to be performed when entering the state</returns>
            public List<Action> GetEntryActions()
            {
                if (EntryActions == null)
                {
                    EntryActions = new List<Action>();
                }
                return EntryActions;
            }

            /// <summary>
            /// Sets the exit actions for this state
            /// </summary>
            /// <param name="exitActions">Actions to be performed when exiting the state</param>
            private void SetExitActions(List<Action> exitActions)
            {
                if (exitActions != null)
                {
                    ExitActions = exitActions;
                }
                else
                {
                    ExitActions = new List<Action>();
                }
            }

            /// <summary>
            /// Gets the exit actions for this state
            /// </summary>
            /// <returns>Actions to be performed when exiting the state</returns>
            public List<Action> GetExitActions()
            {
                if (ExitActions == null)
                {
                    ExitActions = new List<Action>();
                }
                return ExitActions;
            }

            /// <summary>
            /// Adds 1 entry action to the current entry actions.
            /// </summary>
            /// <param name="extraEntryAction">Additional entry action</param>
            public void AddEntryAction(Action extraEntryAction)
            {
                if (extraEntryAction != null)
                {
                    this.AddEntryActions(new List<Action>() { extraEntryAction });
                }
            }

            /// <summary>
            /// Adds 1 exit action to the current exit actions.
            /// </summary>
            /// <param name="extraExitAction">Additional exit action</param>
            public void AddExitAction(Action extraExitAction)
            {
                if (extraExitAction != null)
                {
                    this.AddExitActions(new List<Action>() { extraExitAction });
                }
            }

            /// <summary>
            /// Adds entry actions to the current entry actions.
            /// </summary>
            /// <param name="extraEntryActions">Additional entry actions</param>
            public void AddEntryActions(List<Action> extraEntryActions)
            {
                if (EntryActions == null)
                {
                    EntryActions = new List<Action>();
                }
                if (extraEntryActions != null)
                {
                    EntryActions.AddRange(extraEntryActions);
                }
            }

            /// <summary>
            /// Adds exit actions to the current exit actions.
            /// </summary>
            /// <param name="extraExitActions">Additional exit actions</param>
            public void AddExitActions(List<Action> extraExitActions)
            {
                if (ExitActions == null)
                {
                    ExitActions = new List<Action>();
                }
                if (extraExitActions != null)
                {
                    ExitActions.AddRange(extraExitActions);
                }
            }

            /// <summary>
            /// Perform entry actions of this state
            /// </summary>
            /// <remarks>Actions are invoked in the original order.</remarks>
            public void PerformEntryActions()
            {
                if (EntryActions == null || EntryActions.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < EntryActions.Count; i++)
                {
                    EntryActions[i].Invoke();
                }
            }

            /// <summary>
            /// Perform exit actions of this state
            /// </summary>
            /// <remarks>Actions are invoked in the original order.</remarks>
            public void PerformExitActions()
            {
                if (ExitActions == null || ExitActions.Count == 0)
                {
                    return;
                }
                foreach (Action t in ExitActions)
                {
                    t.Invoke();
                }
            }

            /// <summary>
            /// Returns a string representation of this state.
            /// </summary>
            /// <returns>Description of the state, entry and exit actions (in this order)</returns>
            public override string ToString()
            {
                return ToString((ST item) => item.ToString());
            }

            /// <summary>
            /// Returns a string representation of this state.
            /// </summary>
            /// <param name="stateToStringFunc">Custom ToString function for the state identifiers</param>
            /// <returns>Description of the state, entry and exit actions (in this order)</returns>
            public string ToString(Func<ST, string> stateToStringFunc)
            {
                string description = stateToStringFunc(Identifier);
                description += "::";
                int i = 0;
                foreach (Action act in EntryActions)
                {
                    description += act.Method.Name;
                    i++;
                    if (i < EntryActions.Count)
                    {
                        description += ",";
                    }
                }
                description += "::";
                i = 0;
                foreach (Action act in EntryActions)
                {
                    description += act.Method.Name;
                    i++;
                    if (i < EntryActions.Count)
                    {
                        description += ",";
                    }
                }

                return description;
            }
        }

        /// <summary>
        /// Class template representing a transition in a finite state machine.
        /// </summary>
        protected internal class FSMTransition<TT>
        {

            #region Fields

            /// <summary>
            /// Set of "triggers" or "events" for this transition. 
            /// This transition is only to be followed when all these events evaluate to true.
            /// </summary>
            internal ISet<Func<bool>> Events;

            /// <summary>
            /// List of Actions that will be executed (in order of the list!) before following the transition.
            /// Note that this means that the current state when executing these actions is, i
            /// n general, not equal to the NextState data member. 
            /// </summary>
            internal List<Action> Actions;

            /// <summary>
            /// Represents the next state. This should become the current state of the finite state machine 
            /// after the transition has been followed.
            /// </summary>
            private readonly TT NextState;

            #endregion

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="events">Set of requirements to follow this transition</param>
            /// <param name="actions">List of actions to associate with this transition</param>
            /// <param name="nextState">Next state, after the transition</param>
            public FSMTransition(ISet<Func<bool>> events, List<Action> actions, TT nextState)
            {
                NextState = nextState;
                Actions = actions ?? new List<Action>();
                Events = events;
            }

            /// <summary>
            /// Evaluate the set of trigger events.
            /// </summary>
            /// <returns>Returns true if all required events evaluate to true, or false otherwise
            ///  (i.e. returns false whenever at least 1 evaluates to false).</returns>
            public bool EvaluateEvents()
            {
                return Events.Aggregate(true, (current, evt) => current & evt.Invoke());
            }

            /// <summary>
            /// Perform the actions associated with this transition.
            /// </summary>
            public void PerformActions()
            {
                foreach (Action act in Actions)
                {
                    act.Invoke();
                }
            }

            /// <summary>
            /// Returns the next state when this transition is followed.
            /// </summary>
            /// <returns>State to go to when following this transition.</returns>
            public TT GetNextState()
            {
                return NextState;
            }

            /// <summary>
            /// Checks whether the transition parameter has the same trigger event-set as this instance.
            /// </summary>
            /// <param name="trans">Transition to check events of.</param>
            /// <returns>True if both event sets have the same content, otherwise false.</returns>
            public bool EqualEvents(FSMTransition<TT> trans)
            {
                if (this.Events.Count != trans.Events.Count)
                {
                    return false;
                }
                return FSMUtil.SameContent<Func<bool>>(Events, trans.Events);
            }

            /// <summary>
            /// Returns a string representation of this transition.
            /// </summary>
            /// <returns>Description of the next state, events and actions (in that order)</returns>
            public override string ToString()
            {
                return ToString(item => item.ToString());
                            
            }

            /// <summary>
            /// Returns a string representation of this transition.
            /// </summary>
            /// <param name="stateToStringFunc">Custom ToString function for the state identifiers</param>
            /// <returns>Description of the next state, events and actions (in that order)</returns>
            public string ToString(Func<TT, string> stateToStringFunc)
            {
                string description = stateToStringFunc(NextState);
                
                description += "::";
                int i = 0;
                if (Events != null && Events.Count > 0)
                {
                    foreach (Func<bool> evt in Events)
                    {
                        description += evt.Method.Name;
                        i++;
                        if (i < Events.Count)
                        {
                            description += ",";
                        }
                    }
                }

                description += "::";
                i = 0;
                if (Actions != null && Actions.Count > 0)
                {
                    foreach (Action act in Actions)
                    {
                        description += act.Method.Name;
                        i++;
                        if (i < Actions.Count)
                        {
                            description += ",";
                        }
                    }
                }
                return description;
            }
        }

        #region Fields

        /// <summary>
        /// Start state of the FSM
        /// </summary>
        protected internal FSMState<T> StartState;

        /// <summary>
        /// Predicate to signal when execution of the FSM must stop.
        /// When invoking this evaluates to true, FSM halts.
        /// </summary>
        protected internal Func<bool> EndPredicate;

        /// <summary>
        /// Comparer for 2 state identifiers, to check whether 2 states are equal.
        /// </summary>
        protected IEqualityComparer<T> EqualStates;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for type T</param>
        /// <remarks>Setting the comparer to null results in the usage of the default equality-comperarer for type ST.</remarks>
        protected AFiniteStateMachine(IEqualityComparer<T> equalityComparer = null)
        {
            StartState = null;
            SetEndPredicate(() => true);
            //Use default if none is provided
            EqualStates = equalityComparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Sets a new start state for this FSM. Is set to null if state with identifier startState does not exist yet.
        /// </summary>
        /// <param name="startState">New startstate</param>
        public void SetStartState(T startState, IEqualityComparer<T> equalityComparer = null)
        {
            if (this.Contains(startState, equalityComparer))
            {
                this.StartState = FindState(startState, equalityComparer);
            }
            else
            {
                this.StartState = null;
            }

        }

        /// <summary>
        /// Sets a new endpredicate.
        /// </summary>
        /// <param name="endPredicate">New endpredicate identifier</param>
        public void SetEndPredicate(Func<bool> endPredicate)
        {
            EndPredicate = endPredicate;
        }

        /// <summary>
        /// Evaluates the endpredicate
        /// </summary>
        /// <returns>Returns true if endpredicate returns true, otherwise false</returns>
        public bool EvaluateEndPredicate()
        {
            return EndPredicate.Invoke();
        }

        /// <summary>
        /// Adds entry actions to a specific state.
        /// </summary>
        /// <param name="stateIdentifier">State identifier for the state to modify</param>
        /// <param name="entryActions">Entry actions to add to the state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal,
        ///  based on their identifier</param>
        /// <remarks>These actions are added to the back of the already existing actions!</remarks>
        public void AddEntryActions(T stateIdentifier, List<Action> entryActions, IEqualityComparer<T> equalityComparer = null)
        {
            if (this.Contains(stateIdentifier, equalityComparer))
            {
                this.FindState(stateIdentifier, equalityComparer).AddEntryActions(entryActions);
            }
        }

        /// <summary>
        /// Retrieves the entry actions of a specific state.
        /// </summary>
        /// <param name="stateIdentifier">Identifier of a state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <returns>List of actions that should be performed when entering the state, 
        /// or null if the state does not exist</returns>
        public List<Action> GetEntryActions(T stateIdentifier, IEqualityComparer<T> equalityComparer = null)
        {
            if (Contains(stateIdentifier, equalityComparer))
            {
                return FindState(stateIdentifier, equalityComparer).GetEntryActions();
            }
            return null;
        }

        /// <summary>
        /// Adds exit actions to a specific state.
        /// </summary>
        /// <param name="stateIdentifier">State identifier for the state to modify</param>
        /// <param name="exitActions">Exit actions to add to the state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <remarks>These actions are added to the back of the already existing actions!</remarks>
        public void AddExitActions(T stateIdentifier, List<Action> exitActions, IEqualityComparer<T> equalityComparer = null)
        {
            if (Contains(stateIdentifier, equalityComparer))
            {
                FindState(stateIdentifier, equalityComparer).AddExitActions(exitActions);
            }
        }

        /// <summary>
        /// Retrieves the exit actions of a specific state.
        /// </summary>
        /// <param name="stateIdentifier">Identifier of a state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <returns>List of actions that should be performed when exiting the state, 
        /// or null if the state does not exist</returns>
        public List<Action> GetExitActions(T stateIdentifier, IEqualityComparer<T> equalityComparer = null)
        {
            if (Contains(stateIdentifier, equalityComparer))
            {
                return FindState(stateIdentifier, equalityComparer).GetExitActions();
            }
            return null;
        }

        /// <summary>
        /// Checks whether a state exists.
        /// </summary>
        /// <param name="stateIdentifier"></param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal,
        ///  based on their identifier</param>
        /// <returns>True if state exists in this FSM, otherwise false</returns>
        public bool Contains(T stateIdentifier, IEqualityComparer<T> equalityComparer = null)
        {
            return FindState(stateIdentifier, equalityComparer) != default(FSMState<T>);
        }

        /// <summary>
        /// Checks whether a transition exists.
        /// </summary>
        /// <param name="prevState">Identifier of previous state (from which the transition starts)</param>
        /// <param name="nextState">Next state of the transition</param>
        /// <param name="events">Set of events that trigger the transition</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <returns>True if transition exists in this FSM, otherwise false</returns>
        /// <remarks>An exception is thrown when the previous state does not exist!</remarks>
        /// <remarks>A transition already exists when there is another transition with 
        /// the same events and the same next state, starting from the same previous state 
        /// (regardless of transition actions).</remarks>
        public bool Contains(T prevState, T nextState, ISet<Func<bool>> events, IEqualityComparer<T> equalityComparer = null)
        {
            IEqualityComparer<T> compareStates = equalityComparer ?? EqualStates;

            if (this.Contains(prevState, compareStates))
            {
                return GetFSMTransitions(prevState).Any(trans => compareStates.Equals(trans.GetNextState(), nextState) 
                    && FSMUtil.SameContent(trans.Events, events));
            }
            throw new Exception("State " + prevState + " does not exist");
        }

        #region Abstract
        /// <summary>
        /// Adds a state to this FSM.
        /// </summary>
        /// <param name="stateIdentifier">Identifier for state to be added</param>
        /// <param name="entryActions">Actions that should be performed whenever entering this state</param>
        /// <param name="exitActions">Actions that should be performed whenever exiting this state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <remarks>This method should be idempotent (and does not overwrite if state already exists).</remarks>
        public abstract void AddState(T stateIdentifier, List<Action> entryActions = null, List<Action> exitActions = null,
            IEqualityComparer<T> equalityComparer = null);

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
        /// <remarks>Actions are performed _before_ transitioning to the nextState state</remarks>
        public abstract void AddTransition(T prevState, ISet<Func<bool>> events, T nextState, List<Action> actions,
            IEqualityComparer<T> equalityComparer = null);

        /// <summary>
        /// Start the automaton.
        /// </summary>
        public abstract void Start();
        
        /// <summary>
        /// Reset the automaton.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Perform 1 transition. 
        /// This transition is chosen as the first viable transition, starting from the current 
        /// state of the finite state machine. 
        /// (First: in order of addition; viable: trigger event set evaluates to true)
        /// The exit actions of the old state and actions of the chosen transition are executed 
        /// (in this order) before transitioning.
        /// The entry actions of the new state are executed after transitioning.
        /// </summary>
        public abstract void PerformTransition();
        
        /// <summary>
        /// Get all states in this finite automaton.
        /// </summary>
        /// <returns>Set containing the identifiers of all states</returns>
        protected internal abstract ISet<FSMState<T>> GetFSMStates(); 
        
        /// <summary>
        /// Get all transitions from a given state.
        /// </summary>
        /// <param name="state">Identifier of a state</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, 
        /// based on their identifier</param>
        /// <returns>List of all outgoing transitions from the state or null if the state does not exist</returns>
        protected internal abstract List<FSMTransition<T>> GetFSMTransitions(T state, 
            IEqualityComparer<T> equalityComparer = null); 



        #endregion

        /// <summary>
        /// Tries to find a state with the given identifier.
        /// </summary>
        /// <param name="stateIdentifier">State identifier</param>
        /// <param name="equalityComparer">Equality comparer to compare whether 2 states are equal, based on their identifier</param>
        /// <returns>A state where the identifier matches the parameter, or the default of return type</returns>
        /// <remarks>If the equality comparer parameter is null, the field EqualStates is used.</remarks>
        /// <seealso>
        ///     <cref>AFiniteStateMachine</cref>
        /// </seealso>
        protected internal virtual FSMState<T> FindState(T stateIdentifier, IEqualityComparer<T> equalityComparer = null)
        {
            IEqualityComparer<T> compareStates = equalityComparer ?? EqualStates;
            Func<FSMState<T>, bool> predicate = (FSMState<T> state) => state != null 
                && compareStates.Equals(state.GetIdentifier(), stateIdentifier);
            return GetFSMStates().FirstOrDefault<FSMState<T>>(predicate);
        }

        /// <summary>
        /// Get all states in this finite automaton.
        /// </summary>
        /// <returns>Set containing the identifiers of all states</returns>
        public ISet<T> GetStates()
        {
            ISet<T> stateSet = new HashSet<T>();
            foreach (FSMState<T> state in GetFSMStates())
            {
                if (state != null)
                {
                    stateSet.Add(state.GetIdentifier());
                }
            }
            return stateSet;
        }

        

        /// <summary>
        /// Get all transitions from a given state.
        /// </summary>
        /// <param name="state">Identifier of a state</param>
        /// <returns>List of all outgoing transitions from the state or null if
        ///  the state does not exist</returns>
        public List<Triple<T, ISet<Func<bool>>, List<Action>>> GetTransitions(T state)
        {
            if (this.Contains(state, EqualStates))
            {
                return GetFSMTransitions(state).Select(trans => new Triple<T, ISet<Func<bool>>, 
                    List<Action>>(trans.GetNextState(), trans.Events, trans.Actions)).ToList();
            }
            else
            {
                return null;
            }
        }

        

        /// <summary>
        /// Returns a description of this finite state machine.
        /// </summary>
        /// <returns>Description of the states and transitions</returns>
        public override string ToString()
        {
            return ToString((T item) => item.ToString());
        }

        /// <summary>
        /// Returns a description of this finite state machine.
        /// </summary>
        /// <param name="stateToStringFunc">Custom ToString function for the state identifiers</param>
        /// <returns>Description of the states and transitions</returns>
        public string ToString(Func<T,string> stateToStringFunc)
        {
            string description = "";
            if (StartState != null)
            {
                description += "Startstate: " + StartState.ToString(stateToStringFunc) + Environment.NewLine;
            }

            foreach (T state in GetStates())
            {
                description += FindState(state, EqualStates).ToString(stateToStringFunc) + Environment.NewLine;

                List<FSMTransition<T>> transitions = GetFSMTransitions(state);
                if(transitions != null){
                    description = transitions.Aggregate(description, 
                        (current, trans) => current + ("\t" + trans.ToString(stateToStringFunc) + Environment.NewLine));
                }
            }
            return description;
        }

        /// <summary>
        /// Retrieves a string representation of the current state.
        /// </summary>
        /// <returns>String representation of the current state</returns>
        public abstract string CurrentStateToString();

        /// <summary>
        /// Retrieves a string representation of the current state.
        /// </summary>
        /// <param name="stateToStringFunc">Custom ToString function for the state identifiers</param>
        /// <returns>String representation of the current state</returns>
        public abstract string CurrentStateToString(Func<T,string> stateToStringFunc);

        /// <summary>
        /// Returns the default start state identifier.
        /// </summary>
        /// <returns>The startstate as given by the user, or default value of type T if it does not exist.</returns>
        public T GetStartState()
        {
            if (StartState != null)
            {
                return StartState.GetIdentifier();
            }
            return default(T);
        }

        /// <summary>
        /// Builds a cache of transition, speeding up the execution.
        /// </summary>
        public abstract void BuildCache();

    }
}
