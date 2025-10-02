using System;

namespace InputLog.Core.Analyses
{

    /// <summary>
    /// Exception that can be thrown by an Analysis.
    /// </summary>
    [Serializable]
    public class AnalysisException : Exception
    {
        public AnalysisException() { }
        public AnalysisException(string message) : base(message) { }
        public AnalysisException(string message, Exception inner) : base(message, inner) { }
        protected AnalysisException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}