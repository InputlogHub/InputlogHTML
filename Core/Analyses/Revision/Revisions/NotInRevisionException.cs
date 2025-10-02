using System;

namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// Class thrown when one tries to add an event to a revision,
    /// and the revision detects that the event cannot belong to it
    /// (ie, the event alters the document in such a way that it is
    /// not a linear edit).
    /// </summary>
    [Serializable]
    public class NotInRevisionException : AnalysisException
    {
        public NotInRevisionException() { }
        public NotInRevisionException(string message) : base(message) { }
        public NotInRevisionException(string message, Exception inner) : base(message, inner) { }
        protected NotInRevisionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
