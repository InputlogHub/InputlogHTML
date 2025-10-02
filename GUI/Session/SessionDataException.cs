using System;

namespace GUI.Session
{
    /// <summary>
    /// Class thrown when an illegal request is made to a *SessionMetaData class.
    /// </summary>
    [Serializable]
    public class SessionDataException : Exception
    {
        public SessionDataException() { }
        public SessionDataException(string message) : base(message) { }
        public SessionDataException(string message, Exception inner) : base(message, inner) { }
        protected SessionDataException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
