using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace InputLog.Core.Merging
{
	public class MergeException : Exception, ISerializable
	{
		public MergeException(): base()
		{
		}
		public MergeException(string message): base(message)
		{
		}
		public MergeException(string message, Exception inner): base(message,inner)
		{
		}
		protected MergeException(SerializationInfo info, StreamingContext context): base(info, context)
		{
		}
	}
}
