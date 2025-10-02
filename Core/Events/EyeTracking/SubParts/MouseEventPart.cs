using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	/// <summary>
	/// The mouse event part is the part for the Mouse events of tobii.
	/// However, it does not save to all the detail that tobii does, because this is
	/// to be used in Inputlog merging, inputlog will provide those details for us.
	/// The only information we supply are the mouse events that have taken place (potentially
	/// concatenated), and how many mouse events have taken place.
	/// </summary>
	public class MouseEventPart: ISubPart
	{
		//
		// Standard information
		//
		private string MouseEventIndex { get; set; }
		public string MouseEvent { get; set; }

		/// <summary>
		/// Shows how many mouse events are encased in this
		/// mouse event part. Merging mouse events can increase
		/// the number of mouse events in this MouseEventPart
		/// </summary>
		public string MouseEventNumber 
		{ 
			get { return _MouseEventNumber.ToString(); }
			set { _MouseEventNumber = int.Parse(value); }
		}
		private int _MouseEventNumber;

		private Interpret Interpreter;

		//
		// Constructor and standard methods
		//
		public MouseEventPart()
		{
			Interpreter = new Interpret(this);
			_MouseEventNumber = 0;
		}

		public MouseEventPart(string meIndex, string me)
		{
			MouseEventIndex = meIndex;
			MouseEvent = me;
			_MouseEventNumber = 1;
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// Implementation of the ISubPart interface
		//
		#region ISubPart Members
		/// <summary>
		/// Initialize the tobii mouse event from the data array, using
		/// the index map.
		/// </summary>
		/// <param name="data">Data of the tobii sample</param>
		/// <param name="index">Indexmap mapping the tobii values to their index in the data array</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			MouseEventIndex = data[index[TAGS.RE_MouseEventIndex]];
			MouseEvent = data[index[TAGS.RE_MouseEvent]];
			_MouseEventNumber = (!String.IsNullOrEmpty(MouseEventIndex)) ? 1 : 0;
		}

		/// <summary>
		/// Merges two mouse event parts. When they are merged, the MouseEventNumber counter
		/// goes up one (if the other event containsData()), and their events are
		/// concatenated.
		/// </summary>
		/// <param name="other">The other event to merge.</param>
		public void Merge(ISubPart other)
		{
			MouseEventPart otherPart = other as MouseEventPart;
			if (otherPart == null)
			{
				throw new ArgumentException("MouseEventPart can only be merged with other MouseEventParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if (this.ContainsData())
				{
					MouseEventIndex += "; ";
					MouseEvent += "; ";
				}
				_MouseEventNumber += 1;
				MouseEventIndex += otherPart.MouseEventIndex;
				MouseEvent += otherPart.MouseEvent;
			}
		}

		/// <summary>
		/// Returns true if this part contains at least mouse event data of 
		/// one mouse event.
		/// </summary>
		/// <returns></returns>
		public bool ContainsData()
		{
			return _MouseEventNumber > 0;
		}

		/// <summary>
		/// Mouse events allow concatenation merging.
		/// </summary>
		/// <returns>true</returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		/// <summary>
		/// Subclass that interpretes the data of the mouseEventPart in the
		/// correct format.
		/// </summary>
		public class Interpret
		{
			// 
			// Standard Tobii information
			//
			private MouseEventPart Source;
			public string MouseEvent { get { return Source.MouseEvent; } }
			public int MouseEventIndex { get { return int.Parse(Source.MouseEventIndex); } }

			public Interpret(MouseEventPart source)
			{
				Source = source;
			}
		}
	}
}
