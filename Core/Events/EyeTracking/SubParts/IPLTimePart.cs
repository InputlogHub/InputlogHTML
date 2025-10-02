using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	/// <summary>
	/// The IPLTimePart is the timepart that is used whenever an idfx with eyetracking information has
	/// been read. It contains the already processed time information of the eyetrack events, while the
	/// TimePart is used only during the merging of the eyetrack information with the inputlog information.
	/// </summary>
	public class IPLTimePart: ISubPart
	{
		public string StartTimeIplReferenced { get; set; }
		public string EndTimeIplReferenced { get; set; }
		public string Duration { get { return InterpretData().Duration.ToString(); } }

		/// <summary>
		/// The full local timestamp, dd/mm/yyyy hh:mm:ss,xxx
		/// </summary>
		public string FullLocalTimestamp { get; set; }

		public Interpret Interpreter;

		public IPLTimePart()
		{
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData() 
		{
			return Interpreter;
		}

		#region ISubPart
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			throw new NotImplementedException();
		}

		public void Merge(ISubPart other)
		{
			throw new NotImplementedException();
		}

		public bool ContainsData()
		{
			throw new NotImplementedException();
		}

		public bool AllowsDataMerge()
		{
			return false;
		}
		#endregion

		public class Interpret
		{
			private IPLTimePart Source;
			public ulong StartTimeIplReferenced { get { return (string.IsNullOrEmpty(Source.StartTimeIplReferenced) ? 0ul : ulong.Parse(Source.StartTimeIplReferenced)); } }
			public ulong EndTimeIplReferenced { get { return (string.IsNullOrEmpty(Source.EndTimeIplReferenced) ? 0ul : ulong.Parse(Source.EndTimeIplReferenced)); } }
			public ulong Duration { get { return checked(EndTimeIplReferenced - StartTimeIplReferenced); } }
			public string FullLocalTimestamp { get { return Source.FullLocalTimestamp; } }

			public Interpret(IPLTimePart source)
			{
				Source = source;
			}
		}
	}
}
