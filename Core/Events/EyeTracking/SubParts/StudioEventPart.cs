using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class StudioEventPart: ISubPart
	{
		//
		// Original Tobii info
		//
		public string StudioEventIndex { private get; set; }
		public string StudioEvent { get; set; }
		public string StudioEventData { get; set; }

		private Interpret Interpreter;

		//
		// Constructors
		//
		public StudioEventPart() 
		{
			Interpreter = new Interpret(this);
		}

		public StudioEventPart(string seIndex, string se, string seData)
		{
			StudioEventIndex = seIndex;
			StudioEventData = seData;
			StudioEvent = se;

			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// ISubPart Interface implementation
		//
		#region ISubPart Members

		/// <summary>
		/// Initialize the StudioEventPart based on the data array we get
		/// and the indexmap
		/// </summary>
		/// <param name="data">Data array containing the information</param>
		/// <param name="index">Indexmap mapping the different Tobii tags to the right indices 
		/// in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (index.ContainsKey(TAGS.RE_StudioEventIndex))
			{
				StudioEvent = data[index[TAGS.RE_StudioEvent]];
				StudioEventData = data[index[TAGS.RE_StudioEventData]];
				StudioEventIndex = data[index[TAGS.RE_StudioEventIndex]];
			}
		}

		/// <summary>
		/// Merge this StudioEventPart with another StudioEventPart.
		/// This results in concatenated EventData.
		/// </summary>
		/// <param name="other">The other studio event part to merge with.</param>
		public void Merge(ISubPart other)
		{
			StudioEventPart otherPart = other as StudioEventPart;
			if (otherPart == null)
			{
				throw new ArgumentException("StudioEventPart can only be merged with other StudioEventParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if(this.ContainsData())
				{
					StudioEvent += "; ";
					StudioEventData += "; ";
				}
				StudioEvent += otherPart.StudioEvent;
				StudioEventData += otherPart.StudioEventData;
			}
		}

		/// <summary>
		/// Returns true if this studio event part contains data.
		/// The items contains data if it has a non-null event.
		/// </summary>
		/// <returns>True if the event has data, false if it does not 
		/// have any data.</returns>
		public bool ContainsData()
		{
			return !String.IsNullOrEmpty(StudioEvent);
		}

		/// <summary>
		/// Merging of StudioEventParts leads to a concatenated studioEventPart.
		/// </summary>
		/// <returns>true</returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		public class Interpret
		{
			private StudioEventPart Source;
			public int StudioEventIndex { get { return int.Parse(Source.StudioEventIndex); } }
			public string StudioEvent { get { return Source.StudioEvent; } }
			public string StudioEventData { get { return Source.StudioEventData; } }

			public Interpret(StudioEventPart source)
			{
				Source = source;
			}
		}
	}
}
