using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util;
using InputLog.Core.IO.Tobii;

namespace InputLog.Core.Events.EyeTracking.SubParts
{
	/// <summary>
	/// Timing information from the eyetracker
	/// <br/>
	/// <br />
	/// <code>
	/// 1366274854.560082  						TOBI ORIGINAL MICROSECONDS
	/// 1366299514.521							TOBI  LOCAL TIMESTAMP
	/// 1366299514.505							INPUTLOG TIMESTAMP
	/// 1366299505.613 + 885789 - 876897		INPUTLOG CALCULATION + IL_TIME_START - IL_RELATIVE CREATIONDATE
	/// </code>
	/// Using the tobii local timestamp is the most accurate way of working.
	/// <br/>
	/// <br/>
	/// This class is only used during the merging of inputlog and tobii. When an idfx file with 
	/// eyetrack information has been read by inputlog the in-memory eyetrackparts will never
	/// contain TimeParts. The time in those cases will always be presented in an IPLTimePart.
	/// </summary>
	public class TimePart: ISubPart
	{
		//
		// Original Fields
		//
		/// <summary>
		/// Timestamp from the start of the recording (0 ms) in milliseconds
		/// </summary>
		public string RecordingTS { private get; set; }

		/// <summary>
		/// Timstamp of the internal computer clock. Hour:Minute:Seconds,Milliseconds 
		/// </summary>
		private string _LocalTS;
		private bool _LocalTS_Changed;
		public string LocalTS
		{
			get { return _LocalTS; }
			set
			{
				if (_LocalTS != value)
				{
					_LocalTS = value;
					_LocalTS_Changed = true;
				}
			}
		}

		/// <summary>
		/// Timetamp of the eye tracker, from an unknown point of time, in microseconds.
		/// </summary>
		public string EyeTrackerTS { private get; set; }

		/// <summary>
		/// The date of the tobii recording.
		/// </summary>
		public string RecordingDate { private get; set; }

		//
		// Calculated properties.
		//

		public ulong IdfxStartRecordingMs { set; private get; }
		public ulong IdfxStartOffsetMs { set; private get; }

		/// <summary>
		/// The start time of the timepart, in milliseconds, using the time reference of the idfx files.
		/// </summary>
		public string StartTimeIplReferenced { get { return InterpretData().StartTimeIplReferenced.ToString(); } }

		/// <summary>
		/// The end time of the timepart, in milliseconds, using the time reference of the idfx files.
		/// </summary>
		public string EndTimeIplReferenced { get { return InterpretData().EndTimeIplReferenced.ToString(); } }

		/// <summary>
		/// Start Time of the TimePart, this remains the lowest time of the TimePart, accross
		/// merging.
		/// </summary>
		public string StartTime { get { return InterpretData().StartTime.ToString(); } }

		/// <summary>
		/// EndTime of the TimePart, this remains the highest time of the TimePart, accross
		/// merging.
		/// </summary>
		public string EndTime { get { return InterpretData().EndTime.ToString(); } }

		/// <summary>
		/// Duration for the TimePart, or duration for the total time covered by the
		/// merged timepart.
		/// </summary>
		public string Duration { get { return InterpretData().Duration.ToString(); } }

		/// <summary>
		/// This is the Full Time in Milliseconds of the original TimePart, or the last
		/// TimePart that has been merged into this TimePart, in case this part is the
		/// result of a merge.
		/// </summary>
		public string FullTimeInMs { get { return InterpretData().FullTimeInMs.ToString(); } }

		/// <summary>
		/// The full local timestamp, dd/mm/yyyy hh:mm:ss,xxx
		/// </summary>
		public string FullLocalTimestamp { get { return InterpretData().FullLocalTimestamp; } }

		// 
		// Data interpretation
		// 

		// Interprets the data.
		private Interpret Interpreter;

		// True if the interpreter has already been initialized, false if not.
		private bool InterpreterInitialized;

		//
		// Constructors.
		//
		public TimePart()
		{
			Interpreter = new Interpret(this);
			InterpreterInitialized = false;
		}

		public TimePart(string recordingDate, string recording, string local, string eyetracker)
		{
			RecordingDate = recordingDate;
			RecordingTS = recording;
			LocalTS = local;
			EyeTrackerTS = eyetracker;
			Interpreter = new Interpret(this);
			Interpreter.Initialize();
			InterpreterInitialized = true;
		}

		public Interpret InterpretData()
		{
			return Interpreter;
		}

		//
		// ISubPart interface implementation
		// 

		#region ISubPart Members
		public void Initialize(string[] data, Dictionary<string, int> index)
		{
			RecordingDate = data[index[TAGS.GN_RecordingDate]];
			LocalTS = data[index[TAGS.TS_Local]];
			EyeTrackerTS = data[index[TAGS.TS_EyeTracker]];
			RecordingTS = data[index[TAGS.TS_Recording]];
			Interpreter.Initialize();
			InterpreterInitialized = true;
		}

		/// <summary>
		/// Upon the merging of timeparts the 'current time' of this timePart will be updated
		/// to represent the timing information of the other part it is being merged with.
		/// The Start, EndTimes and Duration will be altered according to the previous information,
		/// and the information gathered from the merging.
		/// </summary>
		/// <param name="other">The other timepart to be merged with this part.</param>
		public void Merge(ISubPart other)
		{
			TimePart otherPart = other as TimePart;
			if (otherPart == null)
			{
				throw new ArgumentException("TimePart can only be merged with other timeParts. Parameter type was \""
					+ other.GetType().ToString() + "\"", "other");
			}

			// Make sure the interpreter has definitely initialized those values.
			if (!InterpreterInitialized)
			{
				Interpreter.Initialize();
			}

			LocalTS = otherPart.LocalTS;
			EyeTrackerTS = otherPart.EyeTrackerTS;
			RecordingTS = otherPart.RecordingTS;
			Interpreter.Merge(otherPart);
		}

		/// <summary>
		/// TimePart is a required field that is always present in every sample.
		/// A TimePart therefore will always contain data.
		/// </summary>
		/// <returns>True</returns>
		public bool ContainsData()
		{
			return true;
		}

		/// <summary>
		/// A TimePart allows for data to be merged, so that the time of multiple
		/// samples may be incorporated into one TimePart with specified end/start times
		/// and durations.
		/// </summary>
		/// <returns>True</returns>
		public bool AllowsDataMerge()
		{
			return true;
		}
		#endregion


		/// <summary>
		/// Class that interprets and converts the data from Tobii.
		/// </summary>
		public class Interpret
		{
			private TimePart Source;

			public Interpret(TimePart source)
			{
				Source = source;
			}
			
			// Original TimePart data
			public ulong RecordingTS { get { return ulong.Parse(Source.RecordingTS); } }
			public ulong LocalTS { get { return StringUtils.ClockStringToMsec(Source.RecordingDate + " " + Source.LocalTS); } }
			public ulong EyeTrackerTS { get { return (string.IsNullOrEmpty(Source.EyeTrackerTS)) ? 0 : ulong.Parse(Source.EyeTrackerTS); } }
			public string FullLocalTimestamp { get { return Source.RecordingDate + " " + Source.LocalTS; } }


			// Calculated TimePart Data

			/// <summary>
			/// Get the time of this TimePart in milliseconds
			/// </summary>
			public ulong FullTimeInMs
			{
				get
				{
					if (Source._LocalTS_Changed || _TimeSpanNotSet)
					{
						_TimeSpan = new TimeSpan(DateTime.Parse(FullLocalTimestamp).Ticks);
						_TimeSpanNotSet = false;
					}
					return checked((ulong) _TimeSpan.TotalMilliseconds);
				}
			}
			private TimeSpan _TimeSpan;
			private bool _TimeSpanNotSet = true;

			/// <summary>
			/// Get the start time, in milliseconds, in the reference time used by the idfx files.
			/// </summary>
			public ulong StartTimeIplReferenced { get { return StartTime + Source.IdfxStartOffsetMs - Source.IdfxStartRecordingMs; } }

			/// <summary>
			/// Get the end time, in milliseconds, in the reference time used by the idfx files.
			/// </summary>
			public ulong EndTimeIplReferenced { get { return EndTime + Source.IdfxStartOffsetMs - Source.IdfxStartRecordingMs; } }

			/// <summary>
			/// StartTime is the lowest time in milliseconds of the all TimeParts merged into this part.
			/// </summary>
			public ulong StartTime
			{
				private set
				{
					_StartTime = value;
				}

				get
				{
					if (!_StartTime.HasValue)
					{
						_StartTime = FullTimeInMs;
					}
					return _StartTime.Value;
				}
			}
			private ulong? _StartTime = null; 

			/// <summary>
			/// EndTime is the highest time in milliseconds of all the TimeParts merged into this part.
			/// </summary>
			public ulong EndTime
			{
				private set
				{
					_EndTime = value;
				}

				get
				{
					if (!_EndTime.HasValue)
					{
						_EndTime = FullTimeInMs;
					}
					return _EndTime.Value;
				}
			}
			private ulong? _EndTime = null;

			// A checked expression throws an exception if there's over -or underflow.
			public ulong Duration { get { return checked( EndTime - StartTime ); } }

			/// <summary>
			/// Initialize some values of the Interpeter, the Initialize method must be
			/// called before a merge method.
			/// </summary>
			public void Initialize()
			{
				ulong tmp = StartTime;
				tmp = EndTime;
			}

			/// <summary>
			/// Merge this TimePart data interpreter with the otherParts 
			/// data interpeter. Make sure that the Initialize method has already been called.
			/// </summary>
			/// <param name="otherPart">Otherpart, to merge with.</param>
			public void Merge(TimePart otherPart)
			{
				StartTime = Math.Min(StartTime, otherPart.InterpretData().StartTime);
				EndTime = Math.Max(EndTime, otherPart.InterpretData().EndTime);
			}
		}
	}
}
