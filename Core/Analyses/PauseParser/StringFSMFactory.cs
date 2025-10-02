using System;

namespace InputLog.Core.Analyses.PauseParser
{
    /// <summary>
    /// Implementation of a factory for finite state machines with strings as state identifiers.
    /// </summary>
    // Used a kind of template specialisation-substitute here
    public class StringFSMFactory : AFSMFactory<string>
    {
        /// <summary>
        /// Lookup class for specific methods (to be used as triggers for the FSM)
        /// </summary>
        private readonly IEventLookup EvtLookup;

        /// <summary>
        /// Lookup class for specific methods (to be used as transition actions for the FSM)
        /// </summary>
        private readonly IActionLookup ActLookup;

        /// <summary>
        /// Used to generate unique identifiers in GenerateIdentifier
        /// </summary>
        private int Identifier;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionLookup">Action-lookup class to lookup transition actions</param>
        /// <param name="eventLookup">Event-lookup class to lookup transition events/triggers</param>
        public StringFSMFactory(IActionLookup actionLookup, IEventLookup eventLookup)
        {
            ActLookup = actionLookup;
            EvtLookup = eventLookup;
            NewFiniteStateMachine();
        }

        /// <summary>
        /// Finds an action based on a name or identifier.
        /// </summary>
        /// <param name="name">Identifier of the action</param>
        /// <returns>Specified action or null</returns>
        protected override Action  FindAction(string name)
        {
            return ActLookup.FindAction(name);
        }

        /// <summary>
        /// Finds a predicate based on a name or identifier.
        /// </summary>
        /// <param name="name">Identifier of the predicate</param>
        /// <param name="inverse">Indicates whether the inverse of the function has to be taken or not</param>
        /// <param name="peekPosition">Indicates whether we need to inspect the current event or 
        /// one of the following events</param>
        /// <returns>Specified predicate or null</returns>
        /// <remarks>Peekposition defaults to 0, the current event. 
        /// When its value is i > 0, the ith next event will be considered.</remarks>
        protected override Func<bool> FindPredicate(string name, bool inverse = false, int peekPosition = 0)
        {
            Func<bool> result = null;
            Func<bool> lookup = EvtLookup.FindPredicate(name, peekPosition);
            if (lookup != null)
            {
                if (inverse)
                {
                    result = () => !lookup();
                }
                else
                {
                    result = lookup;
                }
            }
            return result;
        }

        
        ///<summary>
        ///Generates a unique identifier.
        ///</summary>
        ///<returns>A unique identifier</returns>
        protected override string GenerateIdentifier()
        {
            //OK, this isn't really a unique identifier
            //  but we're fine as long as the amount of states is less than 2^31-1 (which is likely to be the case)
            Identifier++;
            return Identifier.ToString();
        }
    }
}
