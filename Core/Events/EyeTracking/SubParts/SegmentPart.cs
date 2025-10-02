using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class SegmentPart: ISubPart
	{
		//
		// Original tobii data
		//

		public string SegmentName { get; set; }
		public string SegmentStart { set; private get; }
		public string SegmentEnd { set; private get; }
		public string SegmentDuration  { get; set; }
		public string SceneName { get; set; }
		public string SceneSegmentStart { set; private get; }
		public string SceneSegmentEnd { set; private get; }
		public string SceneSegmentDuration { get; set; }

		// 
		// IPL time-referenced information
		//
		public string SegmentStartIPLReferenced 
		{
			get { return (InterpretData().SegmentStartIPLReferenced.HasValue ? InterpretData().SegmentStartIPLReferenced.Value.ToString() : ""); }
			set { InterpretData().SegmentStartIPLReferenced = string.IsNullOrEmpty(value) ? (ulong?)null : ulong.Parse(value); }
		}
		public string SegmentEndIPLReferenced
		{
			get { return (InterpretData().SegmentEndIPLReferenced.HasValue ? InterpretData().SegmentEndIPLReferenced.Value.ToString() : ""); }
			set { InterpretData().SegmentEndIPLReferenced = string.IsNullOrEmpty(value) ? (ulong?)null : ulong.Parse(value); }
		}
		public string SceneSegmentStartIPLReferenced
		{
			get { return (InterpretData().SceneSegmentStartIPLReferenced.HasValue ? InterpretData().SceneSegmentStartIPLReferenced.Value.ToString() : ""); }
			set { InterpretData().SceneSegmentStartIPLReferenced = string.IsNullOrEmpty(value) ? (ulong?)null : ulong.Parse(value); }
		}
		public string SceneSegmentEndIPLReferenced
		{
			get { return (InterpretData().SceneSegmentEndIPLReferenced.HasValue ? InterpretData().SceneSegmentEndIPLReferenced.Value.ToString() : ""); }
			set { InterpretData().SceneSegmentEndIPLReferenced = string.IsNullOrEmpty(value) ? (ulong?)null : ulong.Parse(value); }
		}

		private Interpret Interpreter;

		//
		// Constructor & Base methods
		//
		public SegmentPart()
		{
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// ISubPart implementation
		//
		#region ISubPart Members

		/// <summary>
		/// Construct this Segment part, based on the informatino in the data array
		/// and the indexmap.
		/// </summary>
		/// <param name="data">Array containing the data</param>
		/// <param name="index">Indexmap mapping all the datafields by name to their
		/// index in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (!index.ContainsKey(TAGS.SEGM_Name) || !index.ContainsKey(TAGS.SC_Name))
			{
				return;
			}

			SegmentName = data[index[TAGS.SEGM_Name]];
			SegmentStart = data[index[TAGS.SEGM_Start]];
			SegmentEnd = data[index[TAGS.SEGM_End]];
			SegmentDuration = data[index[TAGS.SEGM_Duration]];
			SceneName = data[index[TAGS.SC_Name]];
			SceneSegmentDuration = data[index[TAGS.SC_SegmentDuration]];
			SceneSegmentEnd = data[index[TAGS.SC_SegmentEnd]];
			SceneSegmentStart = data[index[TAGS.SC_SegmentStart]];
		}

		/// <summary>
		/// Merging two SegmentParts does not alter the data. Unless the other segment part 
		/// has a different name from the current name. 
		/// If the name is different and it is an empty name, we retain the old information.
		/// if the name is different and it is not an empty name, we use the new information.
		/// </summary>
		/// <param name="other">Mediapart to merge with.</param>
		public void Merge(ISubPart other)
		{
			SegmentPart otherPart = other as SegmentPart;
			if (otherPart == null)
			{
				throw new ArgumentException("SegmentPart can only be merged with other SegmentParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if (this.ContainsData() && (this.SegmentName == otherPart.SegmentName || string.IsNullOrEmpty(otherPart.SegmentName)) &&
					(this.SceneName == otherPart.SceneName || string.IsNullOrEmpty(otherPart.SceneName)))
				{
					return;
				}
				else
				{
					SceneName = otherPart.SceneName;
					SceneSegmentDuration = otherPart.SceneSegmentDuration;
					SceneSegmentEnd = otherPart.SceneSegmentEnd;
					SceneSegmentStart = otherPart.SceneSegmentStart;
					SegmentDuration = otherPart.SegmentDuration;
					SegmentEnd = otherPart.SegmentEnd;
					SegmentName = otherPart.SegmentName;
					SegmentStart = otherPart.SegmentStart;
				}
			}
		}

		/// <summary>
		/// Returns true if this part contains actual data.
		/// </summary>
		/// <returns>True if this SegmentPart contains data, false if it does not.</returns>
		public bool ContainsData()
		{
			return !String.IsNullOrEmpty(SegmentName) || !String.IsNullOrEmpty(SceneName)
				|| !String.IsNullOrEmpty(SegmentStart) || !String.IsNullOrEmpty(SceneSegmentStart);
		}

		/// <summary>
		/// Segment parts being merged means that if the segment part already 
		/// contained different information from the to be merged part, this will
		/// be overwritten with the last-merged part.
		/// </summary>
		/// <returns></returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		/// <summary>
		/// Inner class used for interpreting the data from the 
		/// SegmentPart.
		/// </summary>
		public class Interpret
		{
			private SegmentPart Source;
			public string SegmentName { get { return Source.SegmentName; } }
			public string SceneName { get { return Source.SceneName; } }
			public ulong? SegmentStart { get { return string.IsNullOrEmpty(Source.SegmentStart) ? (ulong?)null : ulong.Parse(Source.SegmentStart); } }
			public ulong? SegmentEnd { get { return string.IsNullOrEmpty(Source.SegmentEnd) ? (ulong?)null : ulong.Parse(Source.SegmentEnd); } }
			public ulong? SegmentDuration { get { return string.IsNullOrEmpty(Source.SegmentDuration) ? (ulong?)null : ulong.Parse(Source.SegmentDuration); } }
			public ulong? SceneSegmentStart { get { return string.IsNullOrEmpty(Source.SceneSegmentStart) ? (ulong?)null : ulong.Parse(Source.SceneSegmentStart); } }
			public ulong? SceneSegmentEnd { get { return string.IsNullOrEmpty(Source.SceneSegmentEnd) ? (ulong?)null : ulong.Parse(Source.SceneSegmentEnd); } }
			public ulong? SceneSegmentDuration { get { return string.IsNullOrEmpty(Source.SceneSegmentDuration) ? (ulong?)null : ulong.Parse(Source.SceneSegmentDuration); } }

			public ulong? SegmentStartIPLReferenced 
			{ 
				get 
				{ 
					if(!_SegmentStartIPLReferenced.HasValue)
					{
						if (SegmentStart.HasValue)
						{
							_SegmentStartIPLReferenced = IPLTimeConverter.TobiiRecordingTS_To_IPL((long)SegmentStart);
						}
						else
						{
							_SegmentStartIPLReferenced = (ulong?)null;
						}
						
					}
					return _SegmentStartIPLReferenced;
				}
				set
				{
					_SegmentStartIPLReferenced = value;
				}
			}
			public ulong? SegmentEndIPLReferenced
			{
				get
				{
					if (!_SegmentEndIPLReferenced.HasValue)
					{
						if (SegmentEnd.HasValue)
						{
							_SegmentEndIPLReferenced = IPLTimeConverter.TobiiRecordingTS_To_IPL((long)SegmentEnd);
						}
						else
						{
							_SegmentEndIPLReferenced = (ulong?)null;
						}

					}
					return _SegmentEndIPLReferenced;
				}
				set
				{
					_SegmentEndIPLReferenced = value;
				}
			}
			public ulong? SceneSegmentStartIPLReferenced
			{
				get
				{
					if (!_SceneSegmentStartIPLReferenced.HasValue)
					{
						if (SceneSegmentStart.HasValue)
						{
							_SceneSegmentStartIPLReferenced = IPLTimeConverter.TobiiRecordingTS_To_IPL((long)SceneSegmentStart);
						}
						else
						{
							_SceneSegmentStartIPLReferenced = (ulong?)null;
						}

					}
					return _SceneSegmentStartIPLReferenced;
				}
				set
				{
					_SceneSegmentStartIPLReferenced = value;
				}
			}
			public ulong? SceneSegmentEndIPLReferenced
			{
				get
				{
					if (!_SceneSegmentEndIPLReferenced.HasValue)
					{
						if (SceneSegmentEnd.HasValue)
						{
							_SceneSegmentEndIPLReferenced = IPLTimeConverter.TobiiRecordingTS_To_IPL((long)SceneSegmentEnd);
						}
						else
						{
							_SceneSegmentEndIPLReferenced = (ulong?)null;
						}

					}
					return _SceneSegmentEndIPLReferenced;
				}
				set
				{
					_SceneSegmentEndIPLReferenced = value;
				}
			}

			private ulong? _SegmentStartIPLReferenced = null;
			private ulong? _SegmentEndIPLReferenced = null;
			private ulong? _SceneSegmentStartIPLReferenced = null;
			private ulong? _SceneSegmentEndIPLReferenced = null;

			public Interpret(SegmentPart source)
			{
				Source = source;
			}
		}
	}
}
