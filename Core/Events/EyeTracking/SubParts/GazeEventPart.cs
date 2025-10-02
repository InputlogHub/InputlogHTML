using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	public class GazeEventPart: ISubPart
	{
		//
		// Variables available in the Eyetrack data
		//
		// public gets
		public string FixationIndex { get; set; }
		public string SaccadeIndex { get; set; }
		public string GazeEventType { get; set; }
		public string GazeEventDuration { get; set; }
		// privates gets
		public string FixationPointX_MCSpx { private get; set; }
		public string FixationPointY_MCSpx { private get; set; }
		public string GazePointIndex { private get; set; }
		public string GazePointLeftX_ADCSpx { private get; set; }
		public string GazePointLeftY_ADCSpx { private get; set; }
		public string GazePointRightX_ADCSpx { private get; set; }
		public string GazePointRightY_ADCSpx { private get; set; }
		public string GazePointX_ADCSpx { private get; set; }
		public string GazePointY_ADCSpx { private get; set; }
		public string GazePointX_MCSpx { private get; set; }
		public string GazePointY_MCSpx { private get; set; }
		public string GazePointLeftX_ADCSmm { private get; set; }
		public string GazePointLeftY_ADCSmm { private get; set; }
		public string GazePointRightX_ADCSmm { private get; set; }
		public string GazePointRightY_ADCSmm { private get; set; }
		public string StrictAverageGazePointX_ADCSmm { private get; set; }
		public string StrictAverageGazePointY_ADCSmm { private get; set; }
		public string PupilLeft { private get; set; }
		public string PupilRight { private get; set; }
		public string ValidityLeft { private get; set; }
		public string ValidityRight { private get; set; }

		// helper variables
		private ulong MilliSecondsTS;
		private bool PreviousSampleOffscreen = false;
		private int AverageSampleValidity;

		//
		//Variables (to be) available in idfx log files.
		//
		public string AverageGazePointX_ADCSpx { get { return Interpreter.AverageGazePointX_ADCSpx.ToString(); } }
		public string AverageGazePointY_ADCSpx { get { return Interpreter.AverageGazePointY_ADCSpx.ToString(); } }
		public string AverageValidityLeft { get { return Interpreter.AverageValidityLeft.ToString(); } }
		public string AverageValidityRight { get { return Interpreter.AverageValidityRight.ToString(); } }
		public string AveragePupilLeft { get { return Interpreter.AveragePupilLeft.ToString(); } }
		public string AveragePupilRight { get { return Interpreter.AveragePupilRight.ToString(); } }
		public string OffscreenTime 
		{ 
			get { return Interpreter.OffscreenTime.ToString(); }
			set { InterpretData().OffscreenTime = int.Parse(value); }
		}
		public string NumberOfSamples { get { return Interpreter.NumberOfSamples.ToString(); } }
		public string NumberOfValidSamples { get { return Interpreter.NumberOfMerges.ToString(); } }


		// X calculations
		public string MinGazePointX_MCSpx 
		{ 
			get { return Interpreter.MinGazePointX_MCSpx.ToString(); }
			set { Interpreter.MinGazePointX_MCSpx = int.Parse(value); }
		}
		public string MaxGazePointX_MCSpx 
		{ 
			get { return Interpreter.MaxGazePointX_MCSpx.ToString(); }
			set { Interpreter.MaxGazePointX_MCSpx = int.Parse(value); }
		}
		public string MinGazePointX_ADCSpx 
		{ 
			get { return Interpreter.MinGazePointX_ADCSpx.ToString(); }
			set { Interpreter.MinGazePointX_ADCSpx = int.Parse(value); }
		}
		public string MaxGazePointX_ADCSpx 
		{ 
			get { return Interpreter.MaxGazePointX_ADCSpx.ToString(); }
			set { Interpreter.MaxGazePointX_ADCSpx = int.Parse(value); }
		}
		public string StartGazePointX_ADCSpx 
		{ 
			get { return Interpreter.StartGazePointX_ADCSpx.ToString(); }
			set { Interpreter.StartGazePointX_ADCSpx = int.Parse(value); }
		}
		public string EndGazePointX_ADCSpx 
		{ 
			get { return Interpreter.EndGazePointX_ADCSpx.ToString(); }
			set { Interpreter.EndGazePointX_ADCSpx = int.Parse(value); }
		}
		public string MaxDistanceX 
		{ 
			get { return Interpreter.MaxDistanceX.ToString(); }
			set { /* this is a calculated value and does not have to be set */ }
		}
		public string DistanceX 
		{ 
			get { return Interpreter.DistanceX.ToString(); }
			set { /* this is a calculated value and does not have to be set */ } 
		}
		public string CumulativeAbsoluteDistanceX 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistanceX.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistanceX = int.Parse(value); }
		}
		public string CumulativeAbsoluteDistance_LeftX 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistance_LeftX.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistance_LeftX = int.Parse(value); }
		}
		public string CumulativeAbsoluteDistance_RightX 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistance_RightX.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistance_RightX = int.Parse(value); }
		}

		// Y calculations
		public string MinGazePointY_MCSpx 
		{ 
			get { return Interpreter.MinGazePointY_MCSpx.ToString(); }
			set { Interpreter.MinGazePointY_MCSpx = int.Parse(value); }
		}
		public string MaxGazePointY_MCSpx 
		{ 
			get { return Interpreter.MaxGazePointY_MCSpx.ToString(); }
			set { Interpreter.MaxGazePointY_MCSpx = int.Parse(value); }
		}
		public string MinGazePointY_ADCSpx 
		{ 
			get { return Interpreter.MinGazePointY_ADCSpx.ToString(); }
			set { Interpreter.MinGazePointY_ADCSpx = int.Parse(value); }
		}
		public string MaxGazePointY_ADCSpx 
		{ 
			get { return Interpreter.MaxGazePointY_ADCSpx.ToString(); }
			set { Interpreter.MaxGazePointY_ADCSpx = int.Parse(value); }
		}
		public string StartGazePointY_ADCSpx 
		{ 
			get { return Interpreter.StartGazePointY_ADCSpx.ToString(); }
			set { Interpreter.StartGazePointY_ADCSpx = int.Parse(value); }
		}
		public string EndGazePointY_ADCSpx 
		{ 
			get { return Interpreter.EndGazePointY_ADCSpx.ToString(); }
			set { Interpreter.EndGazePointY_ADCSpx = int.Parse(value); }
		}
		public string MaxDistanceY 
		{ 
			get { return Interpreter.MaxDistanceY.ToString(); }
			set { /* this is a calculated value and does not have to be set */ }
		}
		public string DistanceY 
		{ 
			get { return Interpreter.DistanceY.ToString(); }
			set { /* this is a calculated value and does not have to be set */ }
		}
		public string CumulativeAbsoluteDistanceY 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistanceY.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistanceY = int.Parse(value); }
		}
		public string CumulativeAbsoluteDistance_UpY 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistance_UpY.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistance_UpY = int.Parse(value); }
		}
		public string CumulativeAbsoluteDistance_DownY 
		{ 
			get { return Interpreter.CumulativeAbsoluteDistance_DownY.ToString(); }
			set { Interpreter.CumulativeAbsoluteDistance_DownY = int.Parse(value); }
		}

		//
		// Other variables
		//
		private Interpret Interpreter;
		private bool InterpreterInitialized;

		//
		// Private constants
		//
		private const string __FIXATION = "Fixation";
		private const string __SACCADE = "Saccade";
		private const string __UNCLASSIFIED = "Unclassified";

		/// <summary>
		/// Construct a new Gaze Event Part.
		/// </summary>
		public GazeEventPart() 
		{
			Interpreter = new Interpret(this);
			InterpreterInitialized = false;
		}

		/// <summary>
		/// Set the values of the averages of a gaze event part.
		/// This method can be called if the averages and all the surrounding information have 
		/// already been calculated at some point and can be provided by that source.
		/// </summary>
		/// <param name="nrOfSamples">Number of all samples</param>
		/// <param name="nrOfMerges">Number of all valid samples</param>
		/// <param name="averageGazePointX_ADCSpx"></param>
		/// <param name="averageGazePointY_ADCSpx"></param>
		/// <param name="averageValidityLeft"></param>
		/// <param name="averageValidityRight"></param>
		/// <param name="averagePupilLeft"></param>
		/// <param name="averagePupilRight"></param>
		public void SetAverageValues(int nrOfSamples,
								int nrOfMerges,
								double averageGazePointX_ADCSpx,
								double averageGazePointY_ADCSpx,
								double averageValidityLeft,
								double averageValidityRight,
								double averagePupilLeft,
								double averagePupilRight)
		{
			Interpreter.NumberOfMerges = nrOfMerges;
			Interpreter.NumberOfSamples = nrOfSamples;
			Interpreter.SumGazePointX_ADCSpx = averageGazePointX_ADCSpx * nrOfMerges;
			Interpreter.SumGazePointY_ADCSpx = averageGazePointY_ADCSpx * nrOfMerges;
			Interpreter.SumValidityLeft = averageValidityLeft * nrOfSamples;
			Interpreter.SumValidityRight = averageValidityRight * nrOfSamples;
			Interpreter.SumPupilLeft = averagePupilLeft * nrOfMerges;
			Interpreter.SumPupilRight = averagePupilRight * nrOfMerges;
		}

		/// <summary>
		/// Set the values of the averages of a gaze event part.
		/// This method can be called if the averages and all the surrounding information have 
		/// already been calculated at some point and can be provided by that source.
		/// </summary>
		/// <param name="nrOfSamples">Number of all samples</param>
		/// <param name="nrOfMerges">Number of all valid samples</param>
		/// <param name="averageFixationPointX_MDSpx"></param>
		/// <param name="averageFixationPointY_MDSpx"></param>
		/// <param name="averageValidityLeft"></param>
		/// <param name="averageValidityRight"></param>
		/// <param name="averagePupilLeft"></param>
		/// <param name="averagePupilRight"></param>
		public void SetAverageValues(string nrOfSamples,
								string nrOfMerges,
								string averageFixationPointX_MDSpx,
								string averageFixationPointY_MDSpx,
								string averageValidityLeft,
								string averageValidityRight,
								string averagePupilLeft,
								string averagePupilRight)
		{
			SetAverageValues(int.Parse(nrOfSamples),
				int.Parse(nrOfMerges),
				double.Parse(averageFixationPointX_MDSpx),
				double.Parse(averageFixationPointY_MDSpx),
				double.Parse(averageValidityLeft),
				double.Parse(averageValidityRight),
				double.Parse(averagePupilLeft),
				double.Parse(averagePupilRight));
		}
								

		/// <summary>
		/// Does a gaze event belong to this gaze event or is it a new gaze event?
		/// </summary>
		/// <param name="gazeEvent">The gaze Event that might also be part of this gazeEvent.</param>
		/// <returns>True if the gaze event belongs to this gaze event, false if not.</returns>
		public bool BelongsToGazeEvent(GazeEventPart gazeEvent)
		{
			string message = "";
			return CanMerge(gazeEvent, out message);
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// Implementation of the ISubPart interface
		//
		#region ISubPart
		/// <summary>
		/// Initialize the gaze event part based on the data array and the index map.
		/// </summary>
		/// <param name="data">Data array containing all the data of the current sample.</param>
		/// <param name="index">Index mapping the names of the information of the 
		/// gaze data to the index in the data array.</param>
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			if (data[index[TAGS.ET_ValidityLeft]] != "")
			{
				FixationIndex = data[index[TAGS.GE_FixationIndex]];
				SaccadeIndex = data[index[TAGS.GE_SaccadeIndex]];
				GazeEventType = data[index[TAGS.GE_GazeEventType]];
				GazeEventDuration = data[index[TAGS.GE_GazeEventDuration]];
				ValidityLeft = data[index[TAGS.ET_ValidityLeft]];
				ValidityRight = data[index[TAGS.ET_ValidityRight]];

				AverageSampleValidity = (Interpreter.SampleValidityLeft + Interpreter.SampleValidityRight) / 2;
				if (AverageSampleValidity < 2)
				{
					FixationPointX_MCSpx = data[index[TAGS.GE_FixationPointX_MCSpx]];
					FixationPointY_MCSpx = data[index[TAGS.GE_FixationPointY_MCSpx]];
					GazePointIndex = data[index[TAGS.GT_GazeSampleIndex]];
					GazePointLeftX_ADCSpx = data[index[TAGS.GT_GazePointLeftX_ADCSpx]];
					GazePointLeftY_ADCSpx = data[index[TAGS.GT_GazePointLeftY_ADCSpx]];
					GazePointRightX_ADCSpx = data[index[TAGS.GT_GazePointRightX_ADCSpx]];
					GazePointRightY_ADCSpx = data[index[TAGS.GT_GazePointRightY_ADCSpx]];
					GazePointX_ADCSpx = data[index[TAGS.GT_GazePointX_ADCSpx]];
					GazePointY_ADCSpx = data[index[TAGS.GT_GazePointY_ADCSpx]];
					GazePointX_MCSpx = data[index[TAGS.GT_GazePointX_MCSpx]];
					GazePointY_MCSpx = data[index[TAGS.GT_GazePointY_MCSpx]];
					GazePointLeftX_ADCSmm = data[index[TAGS.GT_GazePointLeftX_ADCSmm]];
					GazePointLeftY_ADCSmm = data[index[TAGS.GT_GazePointLeftY_ADCSmm]];
					GazePointRightX_ADCSmm = data[index[TAGS.GT_GazePointRightX_ADCSmm]];
					GazePointRightY_ADCSmm = data[index[TAGS.GT_GazePointRightY_ADCSmm]];
					StrictAverageGazePointX_ADCSmm = data[index[TAGS.GT_StrictAverageGazePointX_ADCSmm]];
					StrictAverageGazePointY_ADCSmm = data[index[TAGS.GT_StrictAverageGazePointY_ADCSmm]];
					PupilLeft = data[index[TAGS.ET_PupilLeft]];
					PupilRight = data[index[TAGS.ET_PupilRight]];
					PreviousSampleOffscreen = false;
				}
				else
				{
					PreviousSampleOffscreen = true;
				}
				MilliSecondsTS = ulong.Parse(data[index[TAGS.TS_Recording]]);
				Interpreter.Initialize();
				InterpreterInitialized = AverageSampleValidity < 2;
			}
		}


		public void Merge(ISubPart other)
		{
			GazeEventPart otherPart = other as GazeEventPart;
			if (otherPart == null)
			{
				throw new ArgumentException("GazeEventPart can only be merged with other GazeEventParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			string message = "";
			if (!CanMerge(otherPart, out message))
			{
				throw new ArgumentException(message);
			}

			// My sample validity
			AverageSampleValidity = (Interpreter.SampleValidityLeft + Interpreter.SampleValidityRight) / 2;

			if (!InterpreterInitialized)
			{
				Interpreter.Initialize();
				InterpreterInitialized = AverageSampleValidity < 2;
			}
			Interpreter.Merge(otherPart);

			// Copy the data from other, to this GazeEventPart
			// This is done so that we can calculate Cummulative distances in the interpreter upon
			// following merges.
			FixationIndex = otherPart.FixationIndex;
			SaccadeIndex = otherPart.SaccadeIndex;
			GazeEventType = otherPart.GazeEventType;
			GazeEventDuration = otherPart.GazeEventDuration;
			ValidityLeft = otherPart.ValidityLeft;
			ValidityRight = otherPart.ValidityRight;

			// This timing information is used for calculation 'offscreen-time'
			// The offscreenstartTime for a follow-up offscreen sample would 
			// start from the end time of this offscreen sample.
			this.MilliSecondsTS = otherPart.MilliSecondsTS;

			if (otherPart.AverageSampleValidity < 2)
			{
				FixationPointX_MCSpx = otherPart.FixationPointX_MCSpx;
				FixationPointY_MCSpx = otherPart.FixationPointY_MCSpx;
				GazePointIndex = otherPart.GazePointIndex;
				GazePointLeftX_ADCSpx = otherPart.GazePointLeftX_ADCSpx;
				GazePointLeftY_ADCSpx = otherPart.GazePointLeftY_ADCSpx;
				GazePointRightX_ADCSpx = otherPart.GazePointRightX_ADCSpx;
				GazePointRightY_ADCSpx = otherPart.GazePointRightY_ADCSpx;
				GazePointX_ADCSpx = otherPart.GazePointX_ADCSpx;
				GazePointY_ADCSpx = otherPart.GazePointY_ADCSpx;
				GazePointX_MCSpx = otherPart.GazePointX_MCSpx;
				GazePointY_MCSpx = otherPart.GazePointY_MCSpx;
				GazePointLeftX_ADCSmm = otherPart.GazePointLeftX_ADCSmm;
				GazePointLeftY_ADCSmm = otherPart.GazePointLeftY_ADCSmm;
				GazePointRightX_ADCSmm = otherPart.GazePointRightX_ADCSmm;
				GazePointRightY_ADCSmm = otherPart.GazePointRightY_ADCSmm;
				StrictAverageGazePointX_ADCSmm = otherPart.StrictAverageGazePointX_ADCSmm;
				StrictAverageGazePointY_ADCSmm = otherPart.StrictAverageGazePointY_ADCSmm;
				PupilLeft = otherPart.PupilLeft;
				PupilRight = otherPart.PupilRight;
				PreviousSampleOffscreen = false;
			}
			else
			{
				PreviousSampleOffscreen = true;
				// Don't overwrite this data. Else the values we get for the distance etc are incorrect.
				/*
				FixationPointX_MCSpx = "";
				FixationPointY_MCSpx = "";
				GazePointIndex = "";
				GazePointLeftX_ADCSpx = "";
				GazePointLeftY_ADCSpx = "";
				GazePointRightX_ADCSpx = "";
				GazePointRightY_ADCSpx = "";
				GazePointX_ADCSpx = "";
				GazePointY_ADCSpx = "";
				GazePointX_MCSpx = "";
				GazePointY_MCSpx = "";
				GazePointLeftX_ADCSmm = "";
				GazePointLeftY_ADCSmm = "";
				GazePointRightX_ADCSmm = "";
				GazePointRightY_ADCSmm = "";
				StrictAverageGazePointX_ADCSmm = "";
				StrictAverageGazePointY_ADCSmm = "";
				PupilLeft = "";
				PupilRight = "";*/
			}
		}

		/// <summary>
		/// Closes off the gaze event.
		/// </summary>
		/// <param name="recordingTS"></param>
		public void CloseOfEvent(ulong recordingTS)
		{
			Interpreter.CloseOfEvent(recordingTS);
		}

		/// <summary>
		/// Checks if two gaze event parts can be merged together and returns true if they can.
		/// If they can not this method will specify the reason and return false.
		/// </summary>
		/// <param name="other">Other gaze event, to merge with.</param>
		/// <param name="message">Reason why the merging can not be done, if there are issues. In case
		/// this method returns true, the message is left blank.</param>
		/// <returns>True if the otherPart can be merged with this part, false if not.</returns>
		private bool CanMerge(GazeEventPart other, out string message)
		{
			if (!this.ContainsData())
			{
				message = "";
				return true;
			}

			switch (GazeEventType)
			{
				case __FIXATION:
					if (this.FixationIndex != other.FixationIndex)
					{
						message = "Can not merge GazeEventParts that do not belong to the same GazeEvent [FixationID UNEQUAL]!";
						return false;
					}
					break;
				case __SACCADE:
					if (this.SaccadeIndex != other.SaccadeIndex)
					{
						message = "Can not merge GazeEventParts that do not belong to the same GazeEvent [SaccadeID UNEQUAL]!";
						return false;
					}
					break;
				case __UNCLASSIFIED:
					if (other.GazeEventType != __UNCLASSIFIED)
					{
						message = "Can not merge GazeEventParts that do not belong to the same GazeEvent [GazeEventType UNEQUAL]!";
						return false;
					}
					if (this.GazeEventDuration != other.GazeEventDuration)
					{
						message = "Can not merge GazeEventParts of type UNCLASSIFIED if they do not have the same GazeEventDuration [GazeEventDuration UNEQUAL]!";
						return false;
					}
					break;
				default:
					if (this.GazeEventType != other.GazeEventType)
					{
						message = "Can not merge GazeEventParts that do not belong to the same GazeEvent [GazeEventType UNEQUAL]!";
						return false;
					}
					break;
			}
			message = "";
			return true;
		}

		public bool ContainsData()
		{
			return !String.IsNullOrEmpty(GazeEventType);
		}

		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion

		/// <summary>
		/// Internal class interpretes the original data from the tobii file (conversions) and calculates
		/// extra information
		/// </summary>
		public class Interpret
		{
			//
			// Original tobii data
			//
			private GazeEventPart Source;
			public int FixationIndex { get { return int.Parse(Source.FixationIndex); } }
			public int SacadeIndex { get { return int.Parse(Source.SaccadeIndex); } }
			public string GazeEventType { get { return Source.GazeEventType; } }
			// Not so useful tobii data - at least not useful in the constructed idfx file.
			public int GazeEventDuration { get { return int.Parse(Source.GazeEventDuration); } }
			private int FixationPointX_MCSpx { get { return String.IsNullOrEmpty(Source.FixationPointX_MCSpx) ? 0 : int.Parse(Source.FixationPointX_MCSpx); } }
			private int FixationPointY_MCSpx { get { return String.IsNullOrEmpty(Source.FixationPointY_MCSpx) ? 0 : int.Parse(Source.FixationPointY_MCSpx); } }
			private int GazePointIndex { get { return int.Parse(Source.GazePointIndex); } }
			private int GazePointLeftX_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointLeftX_ADCSpx) ? 0 : int.Parse(Source.GazePointLeftX_ADCSpx); } }
			private int GazePointLeftY_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointLeftY_ADCSpx) ? 0 : int.Parse(Source.GazePointLeftY_ADCSpx); } }
			private int GazePointRightX_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointRightX_ADCSpx) ? 0 : int.Parse(Source.GazePointRightX_ADCSpx); } }
			private int GazePointRightY_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointRightY_ADCSpx) ? 0 : int.Parse(Source.GazePointRightY_ADCSpx); } }
			private int GazePointX_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointX_ADCSpx) ? 0 : int.Parse(Source.GazePointX_ADCSpx); } }
			private int GazePointY_ADCSpx { get { return String.IsNullOrEmpty(Source.GazePointY_ADCSpx) ? 0 : int.Parse(Source.GazePointY_ADCSpx); } }
			private int GazePointX_MCSpx { get { return String.IsNullOrEmpty(Source.GazePointX_MCSpx) ? 0 : int.Parse(Source.GazePointX_MCSpx); } }
			private int GazePointY_MCSpx { get { return String.IsNullOrEmpty(Source.GazePointY_MCSpx) ? 0 : int.Parse(Source.GazePointY_MCSpx); } }
			private double GazePointLeftX_ADCSmm { get { return String.IsNullOrEmpty(Source.GazePointLeftX_ADCSmm) ? 0.0d : double.Parse(Source.GazePointLeftX_ADCSmm); } }
			private double GazePointLeftY_ADCSmm { get { return String.IsNullOrEmpty(Source.GazePointLeftY_ADCSmm) ? 0.0d : double.Parse(Source.GazePointLeftY_ADCSmm); } }
			private double GazePointRightX_ADCSmm { get { return String.IsNullOrEmpty(Source.GazePointRightX_ADCSmm) ? 0.0d : double.Parse(Source.GazePointRightX_ADCSmm); } }
			private double GazePointRightY_ADCSmm { get { return String.IsNullOrEmpty(Source.GazePointRightY_ADCSmm) ? 0.0d : double.Parse(Source.GazePointRightY_ADCSmm); } }
			private double StrictAverageGazePointX_ADCSmm { get { return String.IsNullOrEmpty(Source.StrictAverageGazePointX_ADCSmm) ? 0.0d : double.Parse(Source.StrictAverageGazePointX_ADCSmm); } }
			private double StrictAverageGazePointY_ADCSmm { get { return String.IsNullOrEmpty(Source.StrictAverageGazePointY_ADCSmm) ? 0.0d : double.Parse(Source.StrictAverageGazePointY_ADCSmm); } }
			private double PupilLeft { get { return String.IsNullOrEmpty(Source.PupilLeft) ? 0.0d : double.Parse(Source.PupilLeft); } }
			private double PupilRight { get { return String.IsNullOrEmpty(Source.PupilRight) ? 0.0d : double.Parse(Source.PupilRight); } }
			public int SampleValidityLeft { get { return int.Parse(Source.ValidityLeft); } }
			public int SampleValidityRight { get { return int.Parse(Source.ValidityRight); } }

			//
			//Variables (to be) available in idfx log files.
			//
			public double AverageGazePointX_ADCSpx { get { return SumGazePointX_ADCSpx / NumberOfMerges; } }
			public double AverageGazePointY_ADCSpx { get { return SumGazePointY_ADCSpx / NumberOfMerges; } }
			public double AverageValidityLeft { get { return SumValidityLeft / NumberOfSamples; } }
			public double AverageValidityRight { get { return SumValidityRight / NumberOfSamples; } }
			public double AveragePupilLeft { get { return SumPupilLeft / NumberOfMerges; } }
			public double AveragePupilRight { get { return SumPupilRight / NumberOfMerges; } }
			public int OffscreenTime = 0;

			// Helpers for the averages:
			public int NumberOfMerges { get; set; } // valid samples
			public int NumberOfSamples { get; set; } // all samples
			public double SumGazePointX_ADCSpx
			{
				private get 
				{
					if (!_SumGazePointX_ADCSpx.HasValue)
					{
						_SumGazePointX_ADCSpx = GazePointX_ADCSpx;
					}
					return _SumGazePointX_ADCSpx.Value;
				}
				set
				{
					_SumGazePointX_ADCSpx = value;
				}
			}
			public double SumGazePointY_ADCSpx
			{
				private get
				{
					if (!_SumGazePointY_ADCSpx.HasValue)
					{
						_SumGazePointY_ADCSpx = GazePointY_ADCSpx;
					}
					return _SumGazePointY_ADCSpx.Value;
				}
				set
				{
					_SumGazePointY_ADCSpx = value;
				}
			}
			public double SumValidityLeft
			{
				private get
				{
					if (!_SumValidityLeft.HasValue)
					{
						_SumValidityLeft = SampleValidityLeft;
					}
					return _SumValidityLeft.Value;
				}
				set
				{
					_SumValidityLeft = value;
				}
			}
			public double SumValidityRight
			{
				private get
				{
					if (!_SumValidityRight.HasValue)
					{
						_SumValidityRight = SampleValidityRight;
					}
					return _SumValidityRight.Value;
				}
				set
				{
					_SumValidityRight = value;
				}
			}
			public double SumPupilLeft
			{
				private get
				{
					if (!_SumPupilLeft.HasValue)
					{
						_SumPupilLeft = PupilLeft;
					}
					return _SumPupilLeft.Value;
				}
				set
				{
					_SumPupilLeft = value;
				}
			}
			public double SumPupilRight
			{
				private get
				{
					if (!_SumPupilRight.HasValue)
					{
						_SumPupilRight = PupilRight;
					}
					return _SumPupilRight.Value;
				}
				set
				{
					_SumPupilRight = value;
				}
			}

			private double? _SumGazePointX_ADCSpx = null;
			private double? _SumGazePointY_ADCSpx = null;
			private double? _SumValidityLeft = null;
			private double? _SumValidityRight = null;
			private double? _SumPupilLeft = null;
			private double? _SumPupilRight = null;

			// X calculations
			public int MinGazePointX_MCSpx
			{
				get
				{
					if (!_MinGazePointX_MCSpx.HasValue)
					{
						_MinGazePointX_MCSpx = GazePointX_MCSpx;
					}
					return _MinGazePointX_MCSpx.Value;
				}
				set
				{
					_MinGazePointX_MCSpx = value;
				}
			}
			public int MaxGazePointX_MCSpx
			{
				get
				{
					if (!_MaxGazePointX_MCSpx.HasValue)
					{
						_MaxGazePointX_MCSpx = GazePointX_MCSpx;
					}
					return _MaxGazePointX_MCSpx.Value;
				}
				set
				{
					_MaxGazePointX_MCSpx = value;
				}
			}
			public int MinGazePointX_ADCSpx
			{
				get
				{
					if (!_MinGazePointX_ADCSpx.HasValue)
					{
						_MinGazePointX_ADCSpx = GazePointX_ADCSpx;
					}
					return _MinGazePointX_ADCSpx.Value;
				}
				set
				{
					_MinGazePointX_ADCSpx = value;
				}
			}
			public int MaxGazePointX_ADCSpx
			{
				get
				{
					if (!_MaxGazePointX_ADCSpx.HasValue)
					{
						_MaxGazePointX_ADCSpx = GazePointX_ADCSpx;
					}
					return _MaxGazePointX_ADCSpx.Value;
				}
				set
				{
					_MaxGazePointX_ADCSpx = value;
				}
			}
			public int StartGazePointX_ADCSpx
			{
				get
				{
					if (!_StartGazePointX_ADCSpx.HasValue)
					{
						_StartGazePointX_ADCSpx = GazePointX_ADCSpx;
					}
					return _StartGazePointX_ADCSpx.Value;
				}
				set
				{
					_StartGazePointX_ADCSpx = value;
				}
			}
			public int EndGazePointX_ADCSpx;
			public int MaxDistanceX
			{
				get
				{
					return MaxGazePointX_ADCSpx - MinGazePointX_ADCSpx;
				}
			}
			public int DistanceX
			{
				get
				{
					return EndGazePointX_ADCSpx - StartGazePointX_ADCSpx;
				}
			}
			public int CumulativeAbsoluteDistanceX;
			public int CumulativeAbsoluteDistance_LeftX;
			public int CumulativeAbsoluteDistance_RightX;

			// Helpers for the X-Calculations:
			private int? _MinGazePointX_MCSpx = null;
			private int? _MaxGazePointX_MCSpx = null;
			private int? _MinGazePointX_ADCSpx = null;
			private int? _MaxGazePointX_ADCSpx = null;
			private int? _StartGazePointX_ADCSpx = null;

			// Y calculations
			public int MinGazePointY_MCSpx
			{
				get
				{
					if (!_MinGazePointY_MCSpx.HasValue)
					{
						_MinGazePointY_MCSpx = GazePointY_MCSpx;
					}
					return _MinGazePointY_MCSpx.Value;
				}
				set
				{
					_MinGazePointY_MCSpx = value;
				}
			}
			public int MaxGazePointY_MCSpx
			{
				get
				{
					if (!_MaxGazePointY_MCSpx.HasValue)
					{
						_MaxGazePointY_MCSpx = GazePointY_MCSpx;
					}
					return _MaxGazePointY_MCSpx.Value;
				}
				set
				{
					_MaxGazePointY_MCSpx = value;
				}
			}
			public int MinGazePointY_ADCSpx
			{
				get
				{
					if (!_MinGazePointY_ADCSpx.HasValue)
					{
						_MinGazePointY_ADCSpx = GazePointY_ADCSpx;
					}
					return _MinGazePointY_ADCSpx.Value;
				}
				set
				{
					_MinGazePointY_ADCSpx = value;
				}
			}
			public int MaxGazePointY_ADCSpx
			{
				get
				{
					if (!_MaxGazePointY_ADCSpx.HasValue)
					{
						_MaxGazePointY_ADCSpx = GazePointY_ADCSpx;
					}
					return _MaxGazePointY_ADCSpx.Value;
				}
				set
				{
					_MaxGazePointY_ADCSpx = value;
				}
			}
			public int StartGazePointY_ADCSpx
			{
				get
				{
					if (!_StartGazePointY_ADCSpx.HasValue)
					{
						_StartGazePointY_ADCSpx = GazePointY_ADCSpx;
					}
					return _StartGazePointY_ADCSpx.Value;
				}
				set
				{
					_StartGazePointY_ADCSpx = value;
				}
			}
			public int EndGazePointY_ADCSpx;
			public int MaxDistanceY
			{
				get
				{
					return MaxGazePointY_ADCSpx - MinGazePointY_ADCSpx;
				}
			}
			public int DistanceY
			{
				get
				{
					return EndGazePointY_ADCSpx - StartGazePointY_ADCSpx;
				}
			}
			public int CumulativeAbsoluteDistanceY;
			public int CumulativeAbsoluteDistance_UpY;
			public int CumulativeAbsoluteDistance_DownY;

			// Helpers for the Y-Calculations:
			private int? _MinGazePointY_MCSpx = null;
			private int? _MaxGazePointY_MCSpx = null;
			private int? _MinGazePointY_ADCSpx = null;
			private int? _MaxGazePointY_ADCSpx = null;
			private int? _StartGazePointY_ADCSpx = null;

			public Interpret(GazeEventPart source)
			{
				Source = source;
				NumberOfMerges = 1;
				NumberOfSamples = 1;
			}

			internal void Merge(GazeEventPart otherPart)
			{
				Interpret other = otherPart.InterpretData();
				NumberOfSamples += 1;

				SumValidityLeft += other.SumValidityLeft;
				SumValidityRight += other.SumValidityRight;

				if (this.Source.PreviousSampleOffscreen)
				{
					ulong offscreenStart = this.Source.MilliSecondsTS;
					ulong offscreenEnd = otherPart.MilliSecondsTS;
					OffscreenTime = OffscreenTime + (int)checked(offscreenEnd - offscreenStart);
				}

				if (otherPart.AverageSampleValidity < 2)
				{
					// Only merge the data from the otherSample if its data is valid.

					// For the averages
					NumberOfMerges += other.NumberOfMerges;
					SumGazePointX_ADCSpx += other.SumGazePointX_ADCSpx;
					SumGazePointY_ADCSpx += other.SumGazePointY_ADCSpx;
					SumPupilLeft += other.SumPupilLeft;
					SumPupilRight += other.SumPupilRight;

					// minimums
					MinGazePointX_ADCSpx = Math.Min(MinGazePointX_ADCSpx, other.MinGazePointX_ADCSpx);
					MinGazePointX_MCSpx = Math.Min(MinGazePointX_MCSpx, other.MinGazePointX_MCSpx);
					MinGazePointY_ADCSpx = Math.Min(MinGazePointY_ADCSpx, other.MinGazePointY_ADCSpx);
					MinGazePointY_MCSpx = Math.Min(MinGazePointY_MCSpx, other.MinGazePointY_MCSpx);

					// maximums
					MaxGazePointX_ADCSpx = Math.Max(MaxGazePointX_ADCSpx, other.MaxGazePointX_ADCSpx);
					MaxGazePointX_MCSpx = Math.Max(MaxGazePointX_MCSpx, other.MaxGazePointX_MCSpx);
					MaxGazePointY_ADCSpx = Math.Max(MaxGazePointY_ADCSpx, other.MaxGazePointY_ADCSpx);
					MaxGazePointY_MCSpx = Math.Max(MaxGazePointY_MCSpx, other.MaxGazePointY_MCSpx);

					// Start point is set to my own gazepoint. It can only be set once.
					// This is done during the initialization of the Interpreter.
					// End point is overwritten every single time.
					EndGazePointX_ADCSpx = other.GazePointX_ADCSpx;
					EndGazePointY_ADCSpx = other.GazePointY_ADCSpx;

					// Start point value, in the set method only allows to be set once. 
					// These statements will have no effect if the StartGazePoint already had a value.
					StartGazePointX_ADCSpx = GazePointX_ADCSpx;
					StartGazePointY_ADCSpx = GazePointY_ADCSpx;

					CumulativeAbsoluteDistanceX += Math.Abs(GazePointX_ADCSpx - other.GazePointX_ADCSpx);
					CumulativeAbsoluteDistanceY += Math.Abs(GazePointY_ADCSpx - other.GazePointY_ADCSpx);
					CumulativeAbsoluteDistance_LeftX += ((other.GazePointX_ADCSpx < GazePointX_ADCSpx) ? GazePointX_ADCSpx - other.GazePointX_ADCSpx : 0);
					CumulativeAbsoluteDistance_RightX += ((other.GazePointX_ADCSpx > GazePointX_ADCSpx) ? other.GazePointX_ADCSpx - GazePointX_ADCSpx : 0);
					CumulativeAbsoluteDistance_DownY += ((other.GazePointY_ADCSpx > GazePointY_ADCSpx) ? other.GazePointY_ADCSpx - GazePointY_ADCSpx : 0); // Y starts 0 at the top
					CumulativeAbsoluteDistance_UpY += ((other.GazePointY_ADCSpx < GazePointY_ADCSpx) ? GazePointY_ADCSpx - other.GazePointY_ADCSpx : 0); // Y highest value is the bottom
				}
			}

			internal void Initialize()
			{
				if (Source.AverageSampleValidity < 2)
				{
					StartGazePointX_ADCSpx = GazePointX_ADCSpx;
					StartGazePointY_ADCSpx = GazePointY_ADCSpx;
					MinGazePointX_ADCSpx = GazePointX_ADCSpx;
					MinGazePointX_MCSpx = GazePointX_MCSpx;
					MinGazePointY_ADCSpx = GazePointY_ADCSpx;
					MinGazePointY_MCSpx = GazePointY_MCSpx;
					MaxGazePointX_ADCSpx = GazePointX_ADCSpx;
					MaxGazePointX_MCSpx = GazePointX_MCSpx;
					MaxGazePointY_ADCSpx = GazePointY_ADCSpx;
					MaxGazePointY_MCSpx = GazePointY_MCSpx;
				}
			}

			internal void CloseOfEvent(ulong recordingTS)
			{
				if (this.Source.AverageSampleValidity >= 2)
				{
					ulong offscreenStart = this.Source.MilliSecondsTS;
					ulong offscreenEnd = recordingTS;
					OffscreenTime = OffscreenTime + (int)checked(offscreenEnd - offscreenStart);
				}
			}
		}
	}
}
