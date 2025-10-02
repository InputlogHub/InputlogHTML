using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class MediaPart: ISubPart
	{
		//
		// Standard tobii data
		//
		public string MediaName { get; set; }
		public string MediaPosX { get; set; }
		public string MediaPosY { get; set; }
		public string MediaWidth { get; set; }
		public string MediaHeight { get; set; }

		public Interpret Interpreter;

		// 
		// Constructors and standard functions.
		//
		public MediaPart()
		{
			Interpreter = new Interpret(this);
		}

		public MediaPart(string mName, string mPosX, string mPosY, string mW, string mH)
		{
			MediaName = mName;
			MediaPosX = mPosX;
			MediaPosY = mPosY;
			MediaWidth = mW;
			MediaHeight = mH;
			Interpreter = new Interpret(this);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// Implementation of ISubPart interface
		//
		#region ISubPart Members
		/// <summary>
		/// Initialize the MediaPart based on the data array we get
		/// and the indexmap
		/// </summary>
		/// <param name="data">Data array containing the information</param>
		/// <param name="index">Indexmap mapping the different Tobii tags to the right indices 
		/// in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (index.ContainsKey(TAGS.MEDIA_Name))
			{
				MediaName = data[index[TAGS.MEDIA_Name]];
				MediaHeight = data[index[TAGS.MEDIA_Height]];
				MediaPosX = data[index[TAGS.MEDIA_PosX]];
				MediaPosY = data[index[TAGS.MEDIA_PosY]];
				MediaWidth = data[index[TAGS.MEDIA_Width]];
			}
		}

		/// <summary>
		/// Merging two MediaParts does not alter the data. Unless the other media part 
		/// has a different name from the current name. 
		/// If the name is different and it is an empty name, we retain the old information.
		/// if the name is different and it is not an empty name, we use the new information.
		/// </summary>
		/// <param name="other">Mediapart to merge with.</param>
		public void Merge(ISubPart other)
		{
			MediaPart otherPart = other as MediaPart;
			if (otherPart == null)
			{
				throw new ArgumentException("MediaPart can only be merged with other MediaParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			if (otherPart.ContainsData())
			{
				if (this.ContainsData() && (this.MediaName == otherPart.MediaName || string.IsNullOrEmpty(otherPart.MediaName)))
				{
					return;
				}
				else
				{
					MediaHeight = otherPart.MediaHeight;
					MediaName = otherPart.MediaName;
					MediaPosX = otherPart.MediaPosX;
					MediaPosY = otherPart.MediaPosY;
					MediaWidth = otherPart.MediaWidth;
				}
			}
		}

		public bool ContainsData()
		{
			return !String.IsNullOrEmpty(MediaName);
		}

		public bool AllowsDataMerge()
		{
			return true;
		}

		#endregion

		public class Interpret
		{
			private MediaPart Source;
			public string MediaName { get { return Source.MediaName; } }
			public int MediaPosX { get { return int.Parse(Source.MediaPosX); } }
			public int MediaPosY { get { return int.Parse(Source.MediaPosY); } }
			public int MediaWidth { get { return int.Parse(Source.MediaWidth); } }
			public int MediaHeight { get { return int.Parse(Source.MediaHeight); } }

			public Interpret(MediaPart source)
			{
				Source = source;
			}
		}
	}
}
