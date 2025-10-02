namespace InputLog.Core.Events
{

    /// <summary>
    /// This class lists the different types of events that are supported by the current version of Inputlog.
    /// Note that the use of strings is deliberate (not an enum!), this allows the developer to easily add other types
    /// without needing to modify this class (without being dependent on an enumeration).
    /// </summary>
    public class EventType
    {
        public const string KEYBOARD = "keyboard";
        public const string MOUSE = "mouse";
        public const string FOCUS = "focus";
        public const string DOCPATH = "docpath";

        //WORDLOG SPECIFIC EVENTS
        public const string SELECTION = "selection";
        public const string REPLACEMENT = "replacement";
        public const string INSERT = "insert";
        public const string STATISTICS = "statistics";
        public const string PLACEHOLDER = "placeholder";
        public const string AUTHORCOMMENT = "authorcomment";

        //EYETRACK SPECIFIC EVENTS
        public const string EYETRACK = "eyetrack";

		//DRAGON NATURAL SPEAKING EVENTS
        public const string DRAGONNS = "dragonns";
        
        // COPYTASK SPECIFIC EVENTS
        public const string QUESTIONS = "questions";
    }
}
