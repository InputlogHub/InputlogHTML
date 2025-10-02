using System;

namespace InputLog.Core.IO.Convert
{

    /// <summary>
    /// Exception that can be thrown by the LogFileConvertor.
    /// </summary>
    [Serializable]
    public class ConversionException : Exception
    {
        public ConversionException() { }
        public ConversionException(string message) : base(message) { }
        public ConversionException(string message, Exception inner) : base(message, inner) { }
        protected ConversionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }


}
