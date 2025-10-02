using System;

namespace InputLog.Core.Analyses
{

    /// <summary>
    /// Exception that can be thrown by an AnalysisWriter.
    /// </summary>
    [Serializable]
    public class AnalysisWriterException : Exception
    {
        public AnalysisWriterException() { }
        public AnalysisWriterException(string message) : base(message) { }
        public AnalysisWriterException(string message, Exception inner) : base(message, inner) { }
        protected AnalysisWriterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}