using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class KeyboardEventPart: ISubPart
	{
		//
		// Standard tobii information
		//
		private string KeyPressEventIndex { get; set; }
		public string KeyPressEvent { get; set; }
		
		// 
		// Calculated data
		//
		/// <summary>
		/// Shows how many keyboard events are encased in this keyboard
		/// event. Merging keyboard events can increase the number of keyboard
		/// events in the KeyboardEventPart
		/// </summary>
		public string KeyboardEventNumber 
		{ 
			get { return _KeyboardEventNumber.ToString(); }
			set { _KeyboardEventNumber = int.Parse(value); }
		}
		private int _KeyboardEventNumber;

		private Interpret Interpreter;

		//
		// constructors and standard methods
		public KeyboardEventPart()
		{
			Interpreter = new Interpret(this);
			_KeyboardEventNumber = 0;
		}

		public KeyboardEventPart(string kpEventIndex, string kpEvent)
		{
			KeyPressEventIndex = kpEventIndex;
			KeyPressEvent = kpEvent;
			_KeyboardEventNumber = 1;
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// Implementation of the ISubPart interface

		#region ISubPart
		/// <summary>
		/// Initialize the tobii keyboard event from the data array, using
		/// the index map.
		/// </summary>
		/// <param name="data">Data of the tobii sample</param>
		/// <param name="index">Indexmap mapping the tobii values to their index in the data array</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			KeyPressEvent = data[index[TAGS.RE_KeyPressEvent]];
			KeyPressEventIndex = data[index[TAGS.RE_KeyPressEventIndex]];
			_KeyboardEventNumber = (string.IsNullOrEmpty(KeyPressEventIndex) ? 0 : 1);
		}

		/// <summary>
		/// Merges two keyboard event parts. When they are merged, the KeyboardEventNumber counter
		/// goes up one (if the other event containsData()), and their events are
		/// concatenated.
		/// </summary>
		/// <param name="other">The other event to merge.</param>
		public void Merge(ISubPart other)
		{
			KeyboardEventPart otherPart = other as KeyboardEventPart;
			if (otherPart == null)
			{
				throw new ArgumentException("KeyboardEventPart can only be merged with other KeyboardEventParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if (this.ContainsData())
				{
					KeyPressEventIndex += "; ";
					KeyPressEvent += "; ";
				}
				_KeyboardEventNumber += 1;
				KeyPressEventIndex += otherPart.KeyPressEventIndex;
				KeyPressEvent += otherPart.KeyPressEvent;
			}
		}

		/// <summary>
		/// Returns true if this part contains at least one keyboard event.
		/// </summary>
		/// <returns></returns>
		public bool ContainsData()
		{
			return _KeyboardEventNumber > 0;
		}

		/// <summary>
		/// When keyboardEventParts are merged they concatenate the
		/// their respective KeyboardEvents.
		/// </summary>
		/// <returns></returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		public class Interpret
		{
			private KeyboardEventPart Source;
			private int KeyPressEventIndex { get { return int.Parse(Source.KeyPressEventIndex); } }
			private string KeyPressEvent { get { return Source.KeyPressEvent; } }

			public Interpret(KeyboardEventPart source)
			{
				Source = source;
			}
		}
	}
}
