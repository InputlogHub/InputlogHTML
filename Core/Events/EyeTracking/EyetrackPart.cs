using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events.EyeTracking.SubParts;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking
{
	/// <summary>
	/// The event part for for an eyetrack event.
	/// </summary>
	public class EyetrackPart: IEventPart
	{
		/// <summary>
		/// The different parts available in the EyetrackPart
		/// </summary>
		public List<ISubPart> Parts { get; private set; }

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public EyetrackPart()
		{
			Parts = new List<ISubPart>();
		}

		/// <summary>
		/// Initialize the eyetrack part based on the data that has been read from the 
		/// tobii file.
		/// </summary>
		/// <param name="data">The data for this eyetrack part.</param>
		/// <param name="index">The index map, mapping the tobii information to its index</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			AOIPart aoiPart = new AOIPart();
			ExternalEventPart extPart = new ExternalEventPart();
			EyePositionPart eyeposPart = new EyePositionPart();
			GazeEventPart gazePart = new GazeEventPart();
			KeyboardEventPart keyPart = new KeyboardEventPart();
			MediaPart mediaPart = new MediaPart();
			MouseEventPart mousePart = new MouseEventPart();
			SegmentPart segPart = new SegmentPart();
			StudioEventPart studioPart = new StudioEventPart();
			TimePart timePart = new TimePart();

			aoiPart.Initialize(data, index);
			extPart.Initialize(data, index);
			eyeposPart.Initialize(data, index);
			gazePart.Initialize(data, index);
			keyPart.Initialize(data, index);
			mediaPart.Initialize(data, index);
			mousePart.Initialize(data, index);
			segPart.Initialize(data, index);
			studioPart.Initialize(data, index);
			timePart.Initialize(data, index);

			if (aoiPart.ContainsData())
			{
				Parts.Add(aoiPart);
			}
			if (extPart.ContainsData())
			{
				Parts.Add(extPart);
			}
			if (eyeposPart.ContainsData())
			{
				Parts.Add(eyeposPart);
			}
			if (keyPart.ContainsData())
			{
				Parts.Add(keyPart);
			}
			if (mediaPart.ContainsData())
			{
				Parts.Add(mediaPart);
			}
			if (mousePart.ContainsData())
			{
				Parts.Add(mousePart);
			}
			if (segPart.ContainsData())
			{
				Parts.Add(segPart);
			}
			if (studioPart.ContainsData())
			{
				Parts.Add(studioPart);
			}
			if (gazePart.ContainsData())
			{
				Parts.Add(gazePart);
			}
			if (timePart.ContainsData())
			{
				Parts.Add(timePart);
			}
		}

		/// <summary>
		/// Merge this EyetrackPart with the other eyetrackpart.
		/// This will merge data, calculate max, mins, averages etc based on the data
		/// of both EyetrackParts.
		/// </summary>
		/// <param name="other">The other eyetrackpart to merge his eyetrack part with. Merging
		/// the eyetrack parts removes all ISubParts form the other EyetrackPart in the process
		/// of merging.</param>
		public void Merge(EyetrackPart other)
		{
			// For each part we both have, merge...
			foreach (ISubPart part in Parts)
			{
				ISubPart otherPart = other.GetFirstSubPartByName(part.GetType().Name);
				if (otherPart != null)
				{
					part.Merge(otherPart);
					other.Parts.Remove(otherPart);
				}
			}
			// All parts still in the other.Parts list are parts we do not have, add them to the parts list.
			foreach (ISubPart part in other.Parts)
			{
				this.Parts.Add(part);
			}
			other.Parts.Clear();
		}

		/// <summary>
		/// Can these two eye track parts be merged together into a single gaze event, or not?
		/// </summary>
		/// <param name="other">The other EyetrackPart.</param>
		/// <returns>True if the two Eyetrackparts can be merged together, false if not.</returns>
		public bool CanMerge(EyetrackPart other)
		{
			GazeEventPart otherGaze = other.GetFirstSubPart<GazeEventPart>();
			GazeEventPart thisGaze = this.GetFirstSubPart<GazeEventPart>();

			if (otherGaze == null || thisGaze == null)
			{
				return true;
			}

			return thisGaze.BelongsToGazeEvent(otherGaze);
		}

		/// <summary>
		/// If the event is closed, this method can be called on it so that 
		/// certain 'on-closure' calculations can still be correctly calculated.
		/// The data passed along is the first line of the next tobii event.
		/// </summary>
		public void CloseOfEvent(string[] parts, Dictionary<string, int> indexMap)
		{
			GazeEventPart gaze = this.GetFirstSubPart<GazeEventPart>();
			if (gaze != null)
			{
				gaze.CloseOfEvent(ulong.Parse(parts[indexMap[TAGS.TS_Recording]]));
			}
		}

		/// <summary>
        ///     Returns the first SubPart of a certain type T.
        /// </summary>
        /// <returns>
        ///     The first SubPart of Type T of the given EyetrackPart,
        ///     or default(T) if there is no such part.
        /// </returns>
		public T GetFirstSubPart<T>()
		{
			foreach (ISubPart subPart in Parts)
			{
				if (subPart is T)
				{
					return ((T)subPart);
				}
			}
			return default(T);
		}

		/// <summary>
		/// Return the first sub part in the parts list, which has the same 'NOT FULLY QUALIFIED'
		/// type name as specified by the parameter.
		/// </summary>
		/// <param name="typeName">The NOT FULLY QUALIFIED type name of the subpart you are looking for.</param>
		/// <returns>The first subpart of the requested type, if it is found, null if it is not present in the list.</returns>
		public ISubPart GetFirstSubPartByName(string typeName)
		{
			foreach (ISubPart subPart in Parts)
			{
				if (subPart.GetType().Name == typeName)
				{
					return subPart;
				}
			}
			return null;
		}
	}
}
