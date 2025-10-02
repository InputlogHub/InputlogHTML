using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Events.EyeTracking
{
	/// <summary>
	/// This class was created after most of the TobiiMerging so is not consistently used.
	/// Though it can be used for every tobii/inputlogfile. If used correctly.
	/// <br/>
	/// <br />
	/// This class' Initialize() method should be called once per Tobii-Idfx file combination.
	/// If a tobii file merges with multiple idfx files, the initialze method should be called with all 5 parameters
	/// only on the first call at the start of that merging set. The subsequent idfx's (for the same tobii file) should
	/// only call the initialize method with the adjusted idfx timestamp parameters, but need not change the tobii parameters again.
	/// <br />
	/// <br />
	/// When starting a new tobii/idfx set for merging, the initialize method should be called again with that tobii files first line
	/// information as parameters, and the first idfx files inputlog parameters. Then again, for each subsequent idfx file only the
	/// inputlog parameters should be updated.
	/// </summary>
	static class IPLTimeConverter
	{
		/// <summary>
		/// The start recording time of inputlog, in milliseconds (converted from the dd/mm/yyyy fulltimestamp format)
		/// </summary>
		static private long IPLStartRecordingMS;

		/// <summary>
		/// The startoffset for the inputlog file.
		/// </summary>
		static public long IPLStartOffsetMS { get; private set; }

		/// <summary>
		/// The recording date of the tobii file.
		/// </summary>
		static private string RecordingDate;

		/// <summary>
		/// The offset calculated based on the first line of the tobii file.
		/// This offset is used to change tobii timestamps (in ms) referenced to the RecordingTimestamp
		/// of tobii -> to inputlog timestamps (in milliseconds).
		/// </summary>
		static private long Offset_TobiiRecTS_To_IplTS_MS;

		/// <summary>
		/// Arbirtrary offset. The offset between the (possibly) newly defined IPL startoffset
		/// and the original one must be be bigger or equal to this OFFSET.
		/// </summary>
		static private readonly int ARBIRTRARY_STARTOFFSET_OFFSET = 4;

		/// <summary>
		/// Initialize the variables based on the first line of the tobii file. If this class has already been initialized with the the info of one tobii/inputlog file,
		/// and another inputlog file is later merged with the SAME tobii file. Only the first 2 - inputlog specific - parameters have to be provided.
		/// </summary>
		/// <param param name="iplStartRecording">The start date of the recording in the inputlog file.</param>
		/// <param name="iplStartOffset">The relative log creation date of the inputlogFile</param>
		/// <param name="RecordingDate">RecordingDate value of the first line of the tobii file</param>
		/// <param name="LocalTimestamp">LocalTimestamp value of the first line of the tobii file</param>
		/// <param name="RecordingTimestamp">RecordingTimestamp value of the first line of the tobii file</param>
		static public void Initialize(string iplStartRecording, string iplStartOffset, string recordingDate = "", string localTimestamp = "", string recordingTimestamp = "")
		{
			if (!string.IsNullOrEmpty(recordingDate))
			{
				RecordingDate = recordingDate;
				if (string.IsNullOrEmpty(localTimestamp) || string.IsNullOrEmpty(recordingTimestamp))
				{
					throw new ArgumentException("If at least one of the Tobii parameters is supplied - all the parameters must be supplied");
				}
			}

			// Save inputlog specific data
			IPLStartRecordingMS = FullTimestampToMilliseconds(iplStartRecording);
			IPLStartOffsetMS = long.Parse(iplStartOffset);

			// Convert tobii time to milliseconds
			long tobiiTime = FullTimestampToMilliseconds(recordingDate + " " + localTimestamp);

			// Calculate the offset between the tobii recordingTimestamp and an Inputlog timestamp
			// in milliseconds - of the same time.
			Offset_TobiiRecTS_To_IplTS_MS = tobiiTime - long.Parse(recordingTimestamp);
		}

		/// <summary>
		/// Initialize the variables based on the first line of the tobii file. If this class has already been initialized with the the info of one tobii/inputlog file,
		/// and another inputlog file is later merged with the SAME tobii file. Only the first 2 - inputlog specific - parameters have to be provided. <br />
		/// <br />
		/// Based on the first tobii RealWorldTime event, define the new IPL StartOffset and return it.
		/// </summary>
		/// <param param name="iplStartRecording">The start date of the recording in the inputlog file.</param>
		/// <param name="iplStartOffset">The relative log creation date of the inputlogFile</param>
		/// <param name="RecordingDate">RecordingDate value of the first line of the tobii file</param>
		/// <param name="LocalTimestamp">LocalTimestamp value of the first line of the tobii file</param>
		/// <param name="RecordingTimestamp">RecordingTimestamp value of the first line of the tobii file</param>
		/// <returns>The new start idfx startoffset to be used for interpreting the data.</returns>
		static public long Initialize(ulong iplStartRecordingInMS, ulong iplStartOffset, string recordingDate = "", string localTimestamp = "", string recordingTimestamp = "")
		{
			if (!string.IsNullOrEmpty(recordingDate))
			{
				RecordingDate = recordingDate;
				if (string.IsNullOrEmpty(localTimestamp) || string.IsNullOrEmpty(recordingTimestamp))
				{
					throw new ArgumentException("If at least one of the Tobii parameters is supplied - all the parameters must be supplied");
				}
			}

			// Save inputlog specific data
			IPLStartRecordingMS = (long)iplStartRecordingInMS;
			IPLStartOffsetMS = (long) iplStartOffset;

			/* Tobii events timing can be easily converted to RealWorldTime
			 * 
			 * RealWorldTime[TOBII] = RecordingTimestamp[TOBII] + Offset[TOBII]
			 * <=> Offset[TOBII] = RealWorldTime[TOBII] - RecordingTimeStamp[TOBII] (1)
			 * 
			 * From this we deduce the offset to be used when transforming a recordingTimestamp
			 * to a TobiiEvent-In-RealWorldTime
			 * Offset[TOBII] is now _known_ and _constant_ for one tobii/idfx combination
			 */
			long recordingTimestampTOBII = long.Parse(recordingTimestamp);
			long realworldTimeTOBII = FullTimestampToMilliseconds(recordingDate + " " + localTimestamp);
			long offsetTOBII = realworldTimeTOBII - recordingTimestampTOBII;
		
			/* When converting an Inputlog-Timed-Event to RealWorldTime we must
			 * deduct the InputLog-StartOffset and add the InputLog-StartRecordingTime...
			 * 
			 * Both these values are specified in Milliseconds.
			 * Time[IPL] is a known factor here. It is the time that is specified in an idfx file for a certain IPL event.
			 * 
			 * RealWorldTime[IPL] = Time[IPL] - StartOffset[IPL] + StartRecording[IPL] (2)
			 */

			/*
			 * First convert the realWorldTime[TOBII] to IPLTime, to see if it's a number smaller than ARBIRTRARY_STARTOFFSET_OFFSET.
			 * If it is a negative number we set the new startoffset to the newly calculated (lower) one.
			 */

			long tobiiConvertedToIPLTime = realworldTimeTOBII + IPLStartOffsetMS - IPLStartRecordingMS - ARBIRTRARY_STARTOFFSET_OFFSET;

			if (tobiiConvertedToIPLTime < IPLStartOffsetMS)
			{
				IPLStartOffsetMS = tobiiConvertedToIPLTime;
			}

			/*
			 * RealWorldTime[TOBII] and RealWorldTime[IPL] are in the same time reference frame.
			 * With this information we can determine the offset that is required to convert
			 * a:
			 * RecordingTimeStamp[TOBII] to Time[IPL]
			 * 
			 * As follows:
			 * From combining (1) and (2)
			 * RecordingTimeStamp[TOBII] + Offset[TOBII] = Time[IPL] - StartOffset[IPL] + StartRecording[IPL] (3)
			 * 
			 * We wish to deduce a combined offset to add to the RecordingTimeStamp[TOBII] so that it is equal
			 * to it's inputlog-time-referenced variant: Time[IPL]
			 * Continuing from (3):
			 * 
			 * RecordingTimeStamp[TOBII] + Offset[TOBII] = Time[IPL] - StartOffset[IPL] + StartRecording[IPL]
			 * <=> RecordingTimeStamp[TOBII] + Offset[TOBII] + StartOffset[IPL] - StartRecording[IPL] = Time[IPL]
			 * 
			 * The total offset to be added to a RecordingTimestamp[TOBII] is equal to:
			 * Offset[TOBII] + StartOffset[IPL] - StartRecording[IPL] (3)
			 * 
			 * Note that Offset[TOBII] will remain constant for any single TOBII/IDFX combination and 
			 * thus only needs to be calculated once in order to convert subsequent RecordingTimestamps[TOBII].
			 * This is handy as we don't want can not just convert every RecordingTimestamp[TOBII] (which is in milliseconds) 
			 * to its RealWorldTime-referenced equal time. We do not have the necessary information to calculate
			 * the RealWorldTime-referenced time in those cases.
			 * Just knowing the constant Offset[TOBII] however is the enough to convert to IPL-Referenced-Time.
			 * 
			 * Whenever we want to convert a RecordingTimestamp[TOBII] to Time[IPL] we just add
			 * the _total_ offset as shown in (3).
			 */

			Offset_TobiiRecTS_To_IplTS_MS = offsetTOBII + IPLStartOffsetMS - IPLStartRecordingMS;

			return IPLStartOffsetMS;
		}

		/// <summary>
		/// Convert a timestamp string to a millisecond value (long).
		/// </summary>
		/// <param name="fullTimestamp">timestamp string.</param>
		/// <returns>The millseconds equivalent of the given timestamp (long).</returns>
		static private long FullTimestampToMilliseconds(string fullTimestamp)
		{
			return (long)new TimeSpan(DateTime.Parse(fullTimestamp).Ticks).TotalMilliseconds;
		}

		/// <summary>
		/// Convert a given full tobii timestamp to an inputlog timestamp.
		/// </summary>
		/// <param name="tobiiTimeStamp">Full tobii timestamp consists of (RecordingDate + " " + LocalTimestamp)</param>
		/// <returns>The inputlog equivalent of the timestamp, in milliseconds (ulong)</returns>
		static public ulong TobiiFullTS_ToIPL(string tobiiTimeStamp)
		{
			long tobiiMs = FullTimestampToMilliseconds(tobiiTimeStamp);
			return (ulong) (tobiiMs + IPLStartOffsetMS - IPLStartRecordingMS);
		}

		/// <summary>
		/// Convert a given local tobii timestamp to an inputlog timestamp. This uses the recordingDate set in the
		/// initialization step.
		/// </summary>
		/// <param name="tobiiTimeStamp">Full tobii timestamp consists of (RecordingDate + " " + LocalTimestamp)</param>
		/// <returns>The inputlog equivalent of the timestamp, in milliseconds (ulong)</returns>
		static public ulong TobiiLocalTS_ToIPL(string tobiiLocalTimestamp)
		{
			long tobiiMs = FullTimestampToMilliseconds(RecordingDate + " " + tobiiLocalTimestamp);
			return (ulong)(tobiiMs + IPLStartOffsetMS - IPLStartRecordingMS);
		}

		/// <summary>
		/// Convert a tobii milliseconds timestamp - referenced to the tobii recording timestamp (RecordingTimestamp) - to 
		/// an inputlog timestamp in milliseconds (ulong). 
		/// This uses the information from the initialization method
		/// </summary>
		/// <param name="tobiiRecordingTimestamp">RecordingTimestamp in tobii</param>
		/// <returns>The timestamp in milliseconds converted to an inputlog-referenced timeframe</returns>
		static public ulong TobiiRecordingTS_To_IPL(long tobiiRecordingTimestamp)
		{
			return (ulong)(tobiiRecordingTimestamp + Offset_TobiiRecTS_To_IplTS_MS);
		}
	}
}
