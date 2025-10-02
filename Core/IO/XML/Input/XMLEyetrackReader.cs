using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;
using System.Xml;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;

namespace InputLog.Core.IO.Xml.Input
{
	public class XmlEyetrackReader
	{
		/// <summary>
        /// Delegate according to the HookType.
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <returns></returns>
        public static IEventPart ReadEyetrack(XmlElement xmlElement)
		{
			EyetrackPart ePart = new EyetrackPart();

			XmlNodeList nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.Media.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement) nodeList.Item(0);
				ePart.Parts.Add(ReadMedia(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.Segment.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadSegment(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.Time.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadIPLTime(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.MouseEvent.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadMouseEvent(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.KeyboardEvent.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadKeyboardEvent(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.StudioEvent.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadStudioEvent(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.ExternalEvent.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadExternalEvent(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadGaze(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadEyePosition(subPart));
			}

			nodeList = xmlElement.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.AOI.TAG);
			if (nodeList.Count > 0)
			{
				XmlElement subPart = (XmlElement)nodeList.Item(0);
				ePart.Parts.Add(ReadAOI(subPart));
			}

			return ePart;
		}

        private static ISubPart ReadAOI(XmlElement subPart)
		{
			AOIPart aoiPart = new AOIPart();
			XmlNodeList hits = subPart.GetElementsByTagName(XmlElements.Log.Events.Event.Part.Eyetrack.AOI.AOI_HIT.TAG);

			foreach (var hitNode in hits)
			{
				XmlElement hit = hitNode as XmlElement;
				if (hit != null)
				{
					string aoiName = hit.GetAttribute(XmlElements.Log.Events.Event.Part.Eyetrack.AOI.AOI_HIT.ATTRIBUTES.NAME.KEY);
					string hitValue = hit.InnerText;
					aoiPart.AOIHits.Add(aoiName, hitValue);
				}
			}

			return aoiPart;
		}

        private static ISubPart ReadEyePosition(XmlElement subPart)
		{
			EyePositionPart eyePos = new EyePositionPart();

			eyePos.EyePosLeftX_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftX_MAX.TAG].InnerText;
			eyePos.EyePosLeftX_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftX_MIN.TAG].InnerText;
			eyePos.EyePosLeftY_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftY_MAX.TAG].InnerText;
			eyePos.EyePosLeftY_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftY_MIN.TAG].InnerText;
			eyePos.EyePosLeftZ_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftZ_MAX.TAG].InnerText;
			eyePos.EyePosLeftZ_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosLeftZ_MIN.TAG].InnerText;
			eyePos.EyePosRightX_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightX_MAX.TAG].InnerText;
			eyePos.EyePosRightX_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightX_MIN.TAG].InnerText;
			eyePos.EyePosRightY_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightY_MAX.TAG].InnerText;
			eyePos.EyePosRightY_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightY_MIN.TAG].InnerText;
			eyePos.EyePosRightZ_ADCSmm_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightZ_MAX.TAG].InnerText;
			eyePos.EyePosRightZ_ADCSmm_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.EyePosRightZ_MIN.TAG].InnerText;

			eyePos.DistanceLeft_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.DistanceLeft_MAX.TAG].InnerText;
			eyePos.DistanceLeft_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.DistanceLeft_MIN.TAG].InnerText;
			eyePos.DistanceRight_MAX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.DistanceRight_MAX.TAG].InnerText;
			eyePos.DistanceRight_MIN = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.EyePosition.DistanceRight_MIN.TAG].InnerText;

			return eyePos;
		}

        private static ISubPart ReadGaze(XmlElement subPart)
		{
			GazeEventPart gaze = new GazeEventPart();

			gaze.FixationIndex = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.FixationIndex.TAG].InnerText;
			gaze.SaccadeIndex = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.SaccadeIndex.TAG].InnerText;
			gaze.GazeEventType = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.GazeEventType.TAG].InnerText;
			gaze.GazeEventDuration = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.GazeEventDuration.TAG].InnerText;

			gaze.SetAverageValues(
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.NrOfSamples.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.NrOfValidSamples.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AverageGazePointX.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AverageGazePointY.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AverageValidityLeft.TAG].InnerText, 
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AverageValidityRight.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AveragePupilLeft.TAG].InnerText,
				subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.AveragePupilRight.TAG].InnerText
			);
			gaze.OffscreenTime = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.OffscreenTime.TAG].InnerText;

			gaze.MinGazePointX_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MinGazePointX_ADSCpx.TAG].InnerText;
			gaze.MinGazePointX_MCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MinGazePointX_MDSpx.TAG].InnerText;
			gaze.MaxGazePointX_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MaxGazePointX_ADSCpx.TAG].InnerText;
			gaze.MaxGazePointX_MCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MaxGazePointX_MDSpx.TAG].InnerText;
			gaze.StartGazePointX_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.StartGazePointX_ADSCpx.TAG].InnerText;
			gaze.EndGazePointX_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.EndGazePointX_ADSCpx.TAG].InnerText;
			// maxDistanceX: calculated field
			// distanceX: calculated field
			gaze.CumulativeAbsoluteDistanceX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceX.TAG].InnerText;
			gaze.CumulativeAbsoluteDistance_LeftX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceX_Left.TAG].InnerText;
			gaze.CumulativeAbsoluteDistance_RightX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceX_Right.TAG].InnerText;

			gaze.MinGazePointY_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MinGazePointY_ADSCpx.TAG].InnerText;
			gaze.MinGazePointY_MCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MinGazePointY_MDSpx.TAG].InnerText;
			gaze.MaxGazePointY_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MaxGazePointY_ADSCpx.TAG].InnerText;
			gaze.MaxGazePointY_MCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.MaxGazePointY_MDSpx.TAG].InnerText;
			gaze.StartGazePointY_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.StartGazePointY_ADSCpx.TAG].InnerText;
			gaze.EndGazePointY_ADCSpx = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.EndGazePointY_ADSCpx.TAG].InnerText;
			// maxDistanceY: calculated field
			// distanceY: calculated field
			gaze.CumulativeAbsoluteDistanceY = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceY.TAG].InnerText;
			gaze.CumulativeAbsoluteDistance_UpY = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceY_Up.TAG].InnerText;
			gaze.CumulativeAbsoluteDistance_DownY = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Gaze.CumulativeAbsoluteDistanceY_Down.TAG].InnerText;

			return gaze;
		}

        private static ISubPart ReadExternalEvent(XmlElement subPart)
		{
			ExternalEventPart exPart = new ExternalEventPart();

			exPart.ExternalEvent = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.ExternalEvent.Event.TAG].InnerText;
			exPart.ExternalEventValue = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.ExternalEvent.EventValue.TAG].InnerText;

			return exPart;
		}

        private static ISubPart ReadStudioEvent(XmlElement subPart)
		{
			StudioEventPart exPart = new StudioEventPart();

			exPart.StudioEvent = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.StudioEvent.Event.TAG].InnerText;
			exPart.StudioEventData = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.StudioEvent.EventData.TAG].InnerText;

			return exPart;
		}

        private static ISubPart ReadKeyboardEvent(XmlElement subPart)
		{
			KeyboardEventPart kbPart = new KeyboardEventPart();

			kbPart.KeyboardEventNumber = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.KeyboardEvent.EventNumber.TAG].InnerText;
			kbPart.KeyPressEvent = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.KeyboardEvent.Event.TAG].InnerText;

			return kbPart;
		}

        private static ISubPart ReadMouseEvent(XmlElement subPart)
		{
			MouseEventPart mPart = new MouseEventPart();

			mPart.MouseEvent = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.MouseEvent.Event.TAG].InnerText;
			mPart.MouseEventNumber = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.MouseEvent.EventNumber.TAG].InnerText;

			return mPart;
		}

        private static ISubPart ReadIPLTime(XmlElement subPart)
		{
			IPLTimePart timePart = new IPLTimePart();

			timePart.StartTimeIplReferenced = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Time.Start.TAG].InnerText;
			timePart.EndTimeIplReferenced = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Time.End.TAG].InnerText;
			// duration: calculated field
			timePart.FullLocalTimestamp = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Time.FullTimestamp.TAG].InnerText;

			return timePart;
		}

        private static ISubPart ReadSegment(XmlElement subPart)
		{
			SegmentPart segPart = new SegmentPart();

			segPart.SegmentDuration = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentDuration.TAG].InnerText;
			segPart.SegmentEndIPLReferenced = string.IsNullOrEmpty(subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentEnd.TAG].InnerText) ?
				null : subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentEnd.TAG].InnerText;
			segPart.SegmentName = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentName.TAG].InnerText;
			segPart.SegmentStartIPLReferenced = string.IsNullOrEmpty(subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentStart.TAG].InnerText) ?
				null : subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SegmentStart.TAG].InnerText;

			segPart.SceneName = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneName.TAG].InnerText;
			segPart.SceneSegmentDuration = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneDuration.TAG].InnerText;
			segPart.SceneSegmentEndIPLReferenced = string.IsNullOrEmpty(subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneEnd.TAG].InnerText) ? 
				null : subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneEnd.TAG].InnerText;
			segPart.SceneSegmentStartIPLReferenced = string.IsNullOrEmpty(subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneStart.TAG].InnerText) ?
				null : subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Segment.SceneStart.TAG].InnerText;

			return segPart;
		}

        private static ISubPart ReadMedia(XmlElement subPart)
		{
			MediaPart mediaPart = new MediaPart();

			mediaPart.MediaHeight = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Media.MediaHeight.TAG].InnerText;
			mediaPart.MediaName = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Media.MediaName.TAG].InnerText;
			mediaPart.MediaPosX = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Media.MediaPosX.TAG].InnerText;
			mediaPart.MediaPosY = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Media.MediaPosY.TAG].InnerText;
			mediaPart.MediaWidth = subPart[XmlElements.Log.Events.Event.Part.Eyetrack.Media.MediaWidth.TAG].InnerText;

			return mediaPart;
		}
	}
}
