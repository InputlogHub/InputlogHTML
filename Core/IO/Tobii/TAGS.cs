using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.IO.Tobii
{
	/// <summary>
	/// Class containing all TAG names for tobii information etc.
	/// </summary>
	public static class TAGS
	{
		// General Data
		public const string GN_ExportDate = "ExportDate";
		public const string GN_StudioVersionRec = "StudioVersionRec";
		public const string GN_StudioProjectName = "StudioProjectName";
		public const string GN_StudioTestName = "StudioTestName";
		public const string GN_ParticipantName = "ParticipantName";
		public const string GN_Value = "[*]Value";
		public const string GN_RecordingName = "RecordingName";
		public const string GN_RecordingDate = "RecordingDate";
		public const string GN_RecordingDuration = "RecordingDuration";
		public const string GN_RecordingResolution = "RecordingResolution";
		public const string GN_FixationFilter = "FixationFilter";

		// Media
		public const string MEDIA_Name = "MediaName";
		public const string MEDIA_PosX = "MediaPosX (ADCSpx)";
		public const string MEDIA_PosY = "MediaPosY (ADCSpx)";
		public const string MEDIA_Width = "MediaWidth";
		public const string MEDIA_Height = "MediaHeight";

		// Segment Scene
		public const string SEGM_Name = "SegmentName";
		public const string SEGM_Start = "SegmentStart";
		public const string SEGM_End = "SegmentEnd";
		public const string SEGM_Duration = "SegmentDuration";
		public const string SC_Name = "SceneName";
		public const string SC_SegmentStart = "SceneSegmentStart";
		public const string SC_SegmentEnd = "SceneSegmentEnd";
		public const string SC_SegmentDuration = "SceneSegmentDuration";

		// Timestamp
		public const string TS_Recording = "RecordingTimestamp";
		public const string TS_Local = "LocalTimeStamp";
		public const string TS_EyeTracker = "EyeTrackerTimestamp";

		// Recording Events
		public const string RE_MouseEventIndex = "MouseEventIndex";
		public const string RE_MouseEvent = "MouseEvent";
		public const string RE_MouseEventX_ADCSpx = "MouseEventX (ADCSpx)";
		public const string RE_MouseEventY_ADCSpx = "MouseEventY (ADCSpx)";
		public const string RE_MouseEventX_MCSpx = "MouseEventX (MCSpx)";
		public const string RE_MouseEventY_MCSpx = "MouseEventY (MCSpx)";
		public const string RE_KeyPressEventIndex = "KeyPressEventIndex";
		public const string RE_KeyPressEvent = "KeyPressEvent";
		public const string RE_StudioEventIndex = "StudioEventIndex";
		public const string RE_StudioEvent = "StudioEvent";
		public const string RE_StudioEventData = "StudioEventData";
		public const string RE_ExternalEventIndex = "ExternalEventIndex";
		public const string RE_ExternalEvent = "ExternalEvent";
		public const string RE_ExternalEventValue = "ExternalEventValue";
		
		// GeneralData
		public const string GD_EventMarkerValue = "EventMarkerValue";

		// Gaze Event
		public const string GE_SaccadeIndex = "SaccadeIndex";
		public const string GE_FixationIndex = "FixationIndex";
		public const string GE_GazeEventType = "GazeEventType";
		public const string GE_GazeEventDuration = "GazeEventDuration";
		public const string GE_FixationPointX_MCSpx = "FixationPointX (MCSpx)";
		public const string GE_FixationPointY_MCSpx = "FixationPointY (MCSpx)";
		public const string GE_AOIHit = "AOI[*]Hit";

		// Gaze Tracking
		public const string GT_GazeSampleIndex = "GazePointIndex";
		public const string GT_GazePointLeftX_ADCSpx = "GazePointLeftX (ADCSpx)";
		public const string GT_GazePointLeftY_ADCSpx = "GazePointLeftY (ADCSpx)";
		public const string GT_GazePointRightX_ADCSpx = "GazePointRightX (ADCSpx)";
		public const string GT_GazePointRightY_ADCSpx = "GazePointRightY (ADCSpx)";
		public const string GT_GazePointX_ADCSpx = "GazePointX (ADCSpx)";
		public const string GT_GazePointY_ADCSpx = "GazePointY (ADCSpx)";
		public const string GT_GazePointX_MCSpx = "GazePointX (MCSpx)";
		public const string GT_GazePointY_MCSpx = "GazePointY (MCSpx)";
		public const string GT_GazePointLeftX_ADCSmm = "GazePointLeftX (ADCSmm)";
		public const string GT_GazePointLeftY_ADCSmm = "GazePointLeftY (ADCSmm)";
		public const string GT_GazePointRightX_ADCSmm = "GazePointRightX (ADCSmm)";
		public const string GT_GazePointRightY_ADCSmm = "GazePointRightY (ADCSmm)";
		public const string GT_StrictAverageGazePointX_ADCSmm = "StrictAverageGazePointX (ADCSmm)";
		public const string GT_StrictAverageGazePointY_ADCSmm = "StrictAverageGazePointY (ADCSmm)";

		// Eye Tracking
		public const string ET_EyePosLeftX_ADCSmm = "EyePosLeftX (ADCSmm)";
		public const string ET_EyePosLeftY_ADCSmm = "EyePosLeftY (ADCSmm)";
		public const string ET_EyePosLeftZ_ADCSmm = "EyePosLeftZ (ADCSmm)";
		public const string ET_EyePosRightX_ADCSmm = "EyePosRightX (ADCSmm)";
		public const string ET_EyePosRightY_ADCSmm = "EyePosRightY (ADCSmm)";
		public const string ET_EyePosRightZ_ADCSmm = "EyePosRightZ (ADCSmm)";
		public const string ET_DistanceLeft = "DistanceLeft";
		public const string ET_DistanceRight = "DistanceRight";
		public const string ET_CamLeftX = "CamLeftX";
		public const string ET_CamLeftY = "CamLeftY";
		public const string ET_CamRightX = "CamRightX";
		public const string ET_CamRightY = "CamRightY";
		public const string ET_PupilLeft = "PupilLeft";
		public const string ET_PupilRight = "PupilRight";
		public const string ET_ValidityLeft = "ValidityLeft";
		public const string ET_ValidityRight = "ValidityRight";

		/// <summary>
		/// Contains all the different tags from 'general information' 
		/// and whether they are required for merging with Tobii data or not.
		/// </summary>
		public static Dictionary<string, bool> General = new Dictionary<string, bool>()
		{
			// General Data
			{ GN_ExportDate, false },
			{ GN_StudioVersionRec, false },
			{ GN_StudioProjectName, false},
			{ GN_StudioTestName, false},
			{ GN_ParticipantName, false},
			{ GN_Value, false},
			{ GN_RecordingName, false},
			{ GN_RecordingDate, false},
			{ GN_RecordingDuration, false},
			{ GN_RecordingResolution, true },
			{ GN_FixationFilter, false},
		};

		public static Dictionary<string, bool> Media = new Dictionary<string, bool>()
		{
			{ MEDIA_Name, true},
			{ MEDIA_PosX, true},
			{ MEDIA_PosY, true},
			{ MEDIA_Width, true},
			{ MEDIA_Height, true},
		};

		public static Dictionary<string, bool> SegmentScene = new Dictionary<string, bool>()
		{
			{ SEGM_Name, false},
			{ SEGM_Start, false},
			{ SEGM_End, false},
			{ SEGM_Duration, false},
			{ SC_Name, false},
			{ SC_SegmentStart, false},
			{ SC_SegmentEnd, false},
			{ SC_SegmentDuration, false},
		};

		public static Dictionary<string, bool> Timestamp = new Dictionary<string, bool>()
		{
			{ TS_Recording, true},
			{ TS_Local, true},
			{ TS_EyeTracker, true},
		};

		public static Dictionary<string, bool> RecordingEvent = new Dictionary<string, bool>()
		{
			{ RE_MouseEventIndex, true},
			{ RE_MouseEvent, true},
			{ RE_MouseEventX_ADCSpx, true},
			{ RE_MouseEventY_ADCSpx, true},
			{ RE_MouseEventX_MCSpx, true},
			{ RE_MouseEventY_MCSpx, true},
			{ RE_KeyPressEventIndex, true},
			{ RE_KeyPressEvent, true},
			{ RE_StudioEventIndex, true},
			{ RE_StudioEvent, true},
			{ RE_StudioEventData, true},
			{ RE_ExternalEventIndex, true},
			{ RE_ExternalEvent, true},
			{ RE_ExternalEventValue, true},
		};

		public static Dictionary<string, bool> GeneralData = new Dictionary<string, bool>()
		{
			{ GD_EventMarkerValue, false},
		};

		/// <summary>
		/// [*] means that this can be expanded to [whatever normal characters and - and _ etc...]
		/// </summary>
		public static Dictionary<string, bool> GazeEvent = new Dictionary<string, bool>()
		{
			{ GE_SaccadeIndex, true},
			{ GE_FixationIndex, true},
			{ GE_GazeEventType, true},
			{ GE_GazeEventDuration, true},
			{ GE_FixationPointX_MCSpx, true},
			{ GE_FixationPointY_MCSpx, true},
			{ GE_AOIHit, false},
		};

		public static Dictionary<string, bool> GazeTracking = new Dictionary<string, bool>()
		{
			{ GT_GazeSampleIndex, true},
			{ GT_GazePointLeftX_ADCSpx, true},
			{ GT_GazePointLeftY_ADCSpx, true},
			{ GT_GazePointRightX_ADCSpx, true},
			{ GT_GazePointRightY_ADCSpx, true},
			{ GT_GazePointX_ADCSpx, true},
			{ GT_GazePointY_ADCSpx, true},
			{ GT_GazePointX_MCSpx, true},
			{ GT_GazePointY_MCSpx, true},
			{ GT_GazePointLeftX_ADCSmm, true},
			{ GT_GazePointLeftY_ADCSmm, true},
			{ GT_GazePointRightX_ADCSmm, true},
			{ GT_GazePointRightY_ADCSmm, true},
			{ GT_StrictAverageGazePointX_ADCSmm, true},
			{ GT_StrictAverageGazePointY_ADCSmm, true},
		};

		public static Dictionary<string, bool> EyeTracking = new Dictionary<string, bool>()
		{
			{ ET_EyePosLeftX_ADCSmm, true},
			{ ET_EyePosLeftY_ADCSmm, true},
			{ ET_EyePosLeftZ_ADCSmm, true},
			{ ET_EyePosRightX_ADCSmm, true},
			{ ET_EyePosRightY_ADCSmm, true},
			{ ET_EyePosRightZ_ADCSmm, true},
			{ ET_DistanceLeft, true},
			{ ET_DistanceRight, true},
			{ ET_CamLeftX, false},
			{ ET_CamLeftY, false},
			{ ET_CamRightX, false},
			{ ET_CamRightY, false},
			{ ET_PupilLeft, true},
			{ ET_PupilRight, true},
			{ ET_ValidityLeft, true},
			{ ET_ValidityRight, true},
		};
	}
}
