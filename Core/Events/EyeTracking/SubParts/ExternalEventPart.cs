using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class ExternalEventPart: ISubPart
	{
		//
		// Original tobii information
		//
		private string ExternalEventIndex { get; set; }
		public string ExternalEvent { get; set; }
		public string ExternalEventValue { get; set; }

		private Interpret Interpreter;

		//
		// Constructors and standard methods
		//
		public ExternalEventPart() 
		{
			Interpreter = new Interpret(this);
		}

		public ExternalEventPart(string eventIndex, string @event, string eventValue)
		{
			ExternalEventIndex = eventIndex;
			ExternalEvent = @event;
			ExternalEventValue = eventValue;
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		// 
		// ISubPart Interface Implementation
		//
		#region ISubPart

		/// <summary>
		/// Initialize the ExternalEventPart based on the data array we get
		/// and the indexmap
		/// </summary>
		/// <param name="data">Data array containing the information</param>
		/// <param name="index">Indexmap mapping the different Tobii tags to the right indices 
		/// in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (index.ContainsKey(TAGS.RE_ExternalEventIndex))
			{
				ExternalEvent = data[index[TAGS.RE_ExternalEvent]];
				ExternalEventIndex = data[index[TAGS.RE_ExternalEventIndex]];
				ExternalEventValue = data[index[TAGS.RE_ExternalEventValue]];
			}
		}

		/// <summary>
		/// Merge this ExternalEventPart with another ExternalEventPart.
		/// This results in concatenated EventData.
		/// </summary>
		/// <param name="other">The other external event part to merge with.</param>
		public void Merge(ISubPart other)
		{
			ExternalEventPart otherPart = other as ExternalEventPart;
			if (otherPart == null)
			{
				throw new ArgumentException("ExternalEventPart can only be merged with other ExternalEventParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if (this.ContainsData())
				{
					ExternalEvent += "; ";
					ExternalEventValue += "; ";
				}
				ExternalEvent += otherPart.ExternalEvent;
				ExternalEventValue += otherPart.ExternalEventValue;
			}
		}

		/// <summary>
		/// Returns true if this external event part contains data.
		/// The item contains data if it has a non-null event.
		/// </summary>
		/// <returns></returns>
		public bool ContainsData()
		{
			return !String.IsNullOrEmpty(ExternalEvent);
		}

		/// <summary>
		/// Merging of ExternalEventParts leads to a concatenated ExternalEventPart.
		/// </summary>
		/// <returns>true</returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		public class Interpret
		{
			public ExternalEventPart Source;
			public string ExternalEventIndex { get { return Source.ExternalEventIndex; } }
			public string ExternalEvent { get { return Source.ExternalEvent; } }
			public string ExternalEventValue { get { return Source.ExternalEventValue; } }

			public Interpret(ExternalEventPart source)
			{
				Source = source;
			}
		}
	}
}
