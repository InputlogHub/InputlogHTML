using System;
using System.Collections.Generic;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;

namespace InputLog.Core.Analyses.GeneralEyetrack
{
	public class GeneralEyetrackAnalysis: General.GeneralAnalysis
	{
        static int numberOfIntervals = 0;
        static ulong intervalSize = 0;
        public GeneralEyetrackAnalysis(List<Event> events, SessionIdentification sessionID, string abbreviation,
            List<Util.KeyConversion.KeysEx> controlKeys) :base (events, sessionID, abbreviation, numberOfIntervals,
             intervalSize, controlKeys)
		{
			Summary = new GeneralEyetrackAnalysisSummary();
		}

		protected override void AnalyzeEyetrackEvent(Event even, General.GeneralAnalysisSummary.GeneralAnalysisEvent pOutputEvent)
		{
		    var outputEvent = pOutputEvent as GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent;
		    if (outputEvent == null)
		    {
		        throw new ArgumentException("The General Eyetrack Analysis expects " +
		                                    "GeneralEyetrackAnalysisEvents as outputEventParameter");
		    }
		    var eyetrackPart = Event.GetFirstEventPart<EyetrackPart>(even);
		    var gaze = eyetrackPart.GetFirstSubPart<GazeEventPart>();
		    var eyepos = eyetrackPart.GetFirstSubPart<EyePositionPart>();
		    var keyboard = eyetrackPart.GetFirstSubPart<KeyboardEventPart>();
		    var mouse = eyetrackPart.GetFirstSubPart<MouseEventPart>();
		    var studio = eyetrackPart.GetFirstSubPart<StudioEventPart>();
		    var external = eyetrackPart.GetFirstSubPart<ExternalEventPart>();
		    var aoi = eyetrackPart.GetFirstSubPart<AOIPart>();

		    string type = gaze.GazeEventType;
		    GazeEventPart.Interpret gazeI = gaze.InterpretData();
		    EyePositionPart.Interpret eyeposI = (eyepos != null) ? eyepos.InterpretData() : null;

		    if (gazeI != null)
		    {
		        // Gaze event types && duration
		        switch (gaze.GazeEventType)
		        {
		            case "Fixation":
		                outputEvent.Merge_FixationIndices(gaze.FixationIndex);
		                break;
		            case "Saccade":
		                outputEvent.Merge_SaccadeIndices(gaze.SaccadeIndex);
		                break;
		        }
		        outputEvent.Merge_GazeEventTypeNumberOf(type);
		        outputEvent.Merge_GazeEventDuration(type, gazeI.GazeEventDuration);

		        // Averages
		        outputEvent.Merge_AverageGazePointX_ADCSpx_Merge(type, gazeI.AverageGazePointX_ADCSpx,
		                                                         gazeI.NumberOfMerges);
		        outputEvent.Merge_AverageGazePointY_ADCSpx_Merge(type, gazeI.AverageGazePointY_ADCSpx,
		                                                         gazeI.NumberOfMerges);
		        outputEvent.Merge_AveragePupilLeft(type, gazeI.AveragePupilLeft, gazeI.NumberOfMerges);
		        outputEvent.Merge_AveragePupilRight(type, gazeI.AveragePupilRight, gazeI.NumberOfMerges);
		        outputEvent.Merge_AverageValidityLeft(type, gazeI.AverageValidityLeft, gazeI.NumberOfSamples);
		        outputEvent.Merge_AverageValidityRight(type, gazeI.AverageValidityRight, gazeI.NumberOfSamples);

		        // Offscreen time & samples
		        outputEvent.Merge_OffscreenTime(type, gazeI.OffscreenTime);
		        outputEvent.Merge_NrOfSamples(type, gazeI.NumberOfSamples);
		        outputEvent.Merge_NrOfValidSamples(type, gazeI.NumberOfMerges);

		        // GazePoints X, Y - Min/Max
		        outputEvent.Merge_MinGazePointX_ADCSpx(type, gazeI.MinGazePointX_ADCSpx);
		        outputEvent.Merge_MinGazePointX_MCSpx(type, gazeI.MinGazePointX_MCSpx);
		        outputEvent.Merge_MinGazePointY_ADCSpx(type, gazeI.MinGazePointY_ADCSpx);
		        outputEvent.Merge_MinGazePointY_MCSpx(type, gazeI.MinGazePointY_MCSpx);
		        outputEvent.Merge_MaxGazePointX_ADCSpx(type, gazeI.MaxGazePointX_ADCSpx);
		        outputEvent.Merge_MaxGazePointX_MCSpx(type, gazeI.MaxGazePointX_MCSpx);
		        outputEvent.Merge_MaxGazePointY_ADCSpx(type, gazeI.MaxGazePointY_ADCSpx);
		        outputEvent.Merge_MaxGazePointY_MCSpx(type, gazeI.MaxGazePointY_MCSpx);

		        // MaxDistanceX, Y, DistanceX, Y - Min/Max/Avg
		        outputEvent.Merge_MaxMaxDistanceX(type, gazeI.MaxDistanceX);
		        outputEvent.Merge_MinMaxDistanceX(type, gazeI.MaxDistanceX);
		        outputEvent.Merge_AvgMaxDistanceX(type, gazeI.MaxDistanceX);
		        outputEvent.Merge_MaxMaxDistanceY(type, gazeI.MaxDistanceY);
		        outputEvent.Merge_MinMaxDistanceY(type, gazeI.MaxDistanceY);
		        outputEvent.Merge_AvgMaxDistanceY(type, gazeI.MaxDistanceY);
		        outputEvent.Merge_MaxDistanceX(type, gazeI.DistanceX);
		        outputEvent.Merge_MinDistanceX(type, gazeI.DistanceX);
		        outputEvent.Merge_AvgDistanceX(type, gazeI.DistanceX);
		        outputEvent.Merge_MaxDistanceY(type, gazeI.DistanceY);
		        outputEvent.Merge_MinDistanceY(type, gazeI.DistanceY);
		        outputEvent.Merge_AvgDistanceY(type, gazeI.DistanceY);

		        // Start & EndGazePoint - X, Y - Min/Max
		        outputEvent.Merge_MinStartGazePointX_ADCSpx(type, gazeI.StartGazePointX_ADCSpx);
		        outputEvent.Merge_MinStartGazePointY_ADCSpx(type, gazeI.StartGazePointY_ADCSpx);
		        outputEvent.Merge_MaxStartGazePointX_ADCSpx(type, gazeI.StartGazePointX_ADCSpx);
		        outputEvent.Merge_MaxStartGazePointY_ADCSpx(type, gazeI.StartGazePointY_ADCSpx);
		        outputEvent.Merge_MinEndGazePointX_ADCSpx(type, gazeI.EndGazePointX_ADCSpx);
		        outputEvent.Merge_MinEndGazePointY_ADCSpx(type, gazeI.EndGazePointY_ADCSpx);
		        outputEvent.Merge_MaxEndGazePointX_ADCSpx(type, gazeI.EndGazePointX_ADCSpx);
		        outputEvent.Merge_MaxEndGazePointY_ADCSpx(type, gazeI.EndGazePointY_ADCSpx);

		        // CumAbsDistances - X, XLeft, XRight, Y, YUp, YDown
		        outputEvent.Merge_CumAbsDistanceX(type, gazeI.CumulativeAbsoluteDistanceX);
		        outputEvent.Merge_CumAbsDistanceY(type, gazeI.CumulativeAbsoluteDistanceY);
		        outputEvent.Merge_CumAbsDistanceX_Left(type, gazeI.CumulativeAbsoluteDistance_LeftX);
		        outputEvent.Merge_CumAbsDistanceX_Right(type, gazeI.CumulativeAbsoluteDistance_RightX);
		        outputEvent.Merge_CumAbsDistanceY_Down(type, gazeI.CumulativeAbsoluteDistance_DownY);
		        outputEvent.Merge_CumAbsDistanceY_Up(type, gazeI.CumulativeAbsoluteDistance_UpY);
		    }
		    if (eyeposI != null)
		    {
		        // Distances of the eye
		        outputEvent.Merge_DistanceLeft_Max(type, eyeposI.DistanceLeft_MAX);
		        outputEvent.Merge_DistanceLeft_Min(type, eyeposI.DistanceLeft_MIN);
		        outputEvent.Merge_DistanceRight_Max(type, eyeposI.DistanceRight_MAX);
		        outputEvent.Merge_DistanceRight_Min(type, eyeposI.DistanceRight_MIN);

		        // Eyepositions - X, Y, Z - Min/Max
		        outputEvent.Merge_EyePosLeftX_Max(type, eyeposI.EyePosLeftX_ADCSmm_MAX);
		        outputEvent.Merge_EyePosLeftX_Min(type, eyeposI.EyePosLeftX_ADCSmm_MIN);
		        outputEvent.Merge_EyePosLeftY_Max(type, eyeposI.EyePosLeftY_ADCSmm_MAX);
		        outputEvent.Merge_EyePosLeftY_Min(type, eyeposI.EyePosLeftY_ADCSmm_MIN);
		        outputEvent.Merge_EyePosLeftZ_Max(type, eyeposI.EyePosLeftZ_ADCSmm_MAX);
		        outputEvent.Merge_EyePosLeftZ_Min(type, eyeposI.EyePosLeftZ_ADCSmm_MIN);
		        outputEvent.Merge_EyePosRightX_Max(type, eyeposI.EyePosRightX_ADCSmm_MAX);
		        outputEvent.Merge_EyePosRightX_Min(type, eyeposI.EyePosRightX_ADCSmm_MIN);
		        outputEvent.Merge_EyePosRightY_Max(type, eyeposI.EyePosRightY_ADCSmm_MAX);
		        outputEvent.Merge_EyePosRightY_Min(type, eyeposI.EyePosRightY_ADCSmm_MIN);
		        outputEvent.Merge_EyePosRightZ_Max(type, eyeposI.EyePosRightZ_ADCSmm_MAX);
		        outputEvent.Merge_EyePosRightZ_Min(type, eyeposI.EyePosRightZ_ADCSmm_MIN);
		    }

		    // Mouse - Keyboard - Studio - External
		    if (mouse != null)
		    {
		        outputEvent.Merge_MouseEvents(type, mouse.MouseEvent);
		    }
		    if (keyboard != null)
		    {
		        outputEvent.Merge_KeyboardEvents(type, keyboard.KeyPressEvent);
		    }
		    if (studio != null)
		    {
		        outputEvent.Merge_StudioEvents(type, studio.StudioEvent);
		        outputEvent.Merge_StudioEventValues(type, studio.StudioEventData);
		    }
		    if (external != null)
		    {
		        outputEvent.Merge_ExternalEvents(type, external.ExternalEvent);
		        outputEvent.Merge_ExternalEventValues(type, external.ExternalEventValue);
		    }

		    // Merge AOIs
		    if (aoi != null)
		    {
		        outputEvent.Merge_AOIs(aoi.AOIHits);
		    }

		    // Don't complete the outputEvent when completing the processing of an eyetrack event.
			// We wish to merge multiple eyetrack events.
		}
	}
}
