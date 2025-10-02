using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;

using Part = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part;
using EyetrackXML = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part.Eyetrack;
using InputLog.Core.Util;
using System.Security;
using System.Xml;

namespace InputLog.Core.IO.Xml.Output
{
	public class XmlEyetrackWriter
	{
		/// <summary>
		/// Write the eyetrack eventpart.
		/// </summary>
		/// <param name="e">The eventpart to write</param>
		public static void WriteEyetrackPart(EyetrackPart e, XmlWriter writer)
		{
            writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.Eyetracker);

			foreach (ISubPart subPart in e.Parts)
			{
				if (subPart is AOIPart)
				{
					WritePart((AOIPart)subPart, writer);
				}
				else if (subPart is ExternalEventPart)
				{
                    WritePart((ExternalEventPart)subPart, writer);
				}
				else if (subPart is EyePositionPart)
				{
                    WritePart((EyePositionPart)subPart, writer);
				}
				else if (subPart is GazeEventPart)
				{
                    WritePart((GazeEventPart)subPart, writer);
				}
				else if (subPart is KeyboardEventPart)
				{
                    WritePart((KeyboardEventPart)subPart, writer);
				}
				else if (subPart is MediaPart)
				{
                    WritePart((MediaPart)subPart, writer);
				}
				else if (subPart is MouseEventPart)
				{
                    WritePart((MouseEventPart)subPart, writer);
				}
				else if (subPart is SegmentPart)
				{
                    WritePart((SegmentPart)subPart, writer);
				}
				else if (subPart is StudioEventPart)
				{
                    WritePart((StudioEventPart)subPart, writer);
				} 
				else if (subPart is TimePart)
				{
                    WritePart((TimePart)subPart, writer);
				}
				else if (subPart is IPLTimePart)
				{
                    WritePart((IPLTimePart)subPart, writer);
				}
			}
			writer.WriteEndElement();
		}

        private static void WritePart(TimePart timePart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.Time.TAG);
			writer.WriteElementString(EyetrackXML.Time.Start.TAG, timePart.StartTimeIplReferenced);
			writer.WriteElementString(EyetrackXML.Time.End.TAG, timePart.EndTimeIplReferenced);
			writer.WriteElementString(EyetrackXML.Time.Duration.TAG, timePart.Duration);
			writer.WriteElementString(EyetrackXML.Time.FullTimestamp.TAG, timePart.FullLocalTimestamp);
			writer.WriteEndElement();
		}

        private static void WritePart(IPLTimePart timePart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.Time.TAG);
			writer.WriteElementString(EyetrackXML.Time.Start.TAG, timePart.StartTimeIplReferenced);
			writer.WriteElementString(EyetrackXML.Time.End.TAG, timePart.EndTimeIplReferenced);
			writer.WriteElementString(EyetrackXML.Time.Duration.TAG, timePart.Duration);
			writer.WriteElementString(EyetrackXML.Time.FullTimestamp.TAG, timePart.FullLocalTimestamp);
			writer.WriteEndElement();
		}

        private static void WritePart(StudioEventPart studioEventPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.StudioEvent.TAG);
			writer.WriteElementString(EyetrackXML.StudioEvent.Event.TAG, studioEventPart.StudioEvent);
			writer.WriteElementString(EyetrackXML.StudioEvent.EventData.TAG, studioEventPart.StudioEventData);
			writer.WriteEndElement();
		}

        private static void WritePart(SegmentPart segmentPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.Segment.TAG);
			writer.WriteElementString(EyetrackXML.Segment.SegmentDuration.TAG, segmentPart.SegmentDuration);
			writer.WriteElementString(EyetrackXML.Segment.SegmentEnd.TAG, segmentPart.SegmentEndIPLReferenced);
			writer.WriteElementString(EyetrackXML.Segment.SegmentName.TAG, segmentPart.SegmentName);
			writer.WriteElementString(EyetrackXML.Segment.SegmentStart.TAG, segmentPart.SegmentStartIPLReferenced);

			writer.WriteElementString(EyetrackXML.Segment.SceneDuration.TAG, segmentPart.SceneSegmentDuration);
			writer.WriteElementString(EyetrackXML.Segment.SceneEnd.TAG, segmentPart.SceneSegmentEndIPLReferenced);
			writer.WriteElementString(EyetrackXML.Segment.SceneName.TAG, segmentPart.SceneName);
			writer.WriteElementString(EyetrackXML.Segment.SceneStart.TAG, segmentPart.SceneSegmentStartIPLReferenced);
			writer.WriteEndElement();
		}

        private static void WritePart(MouseEventPart mouseEventPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.MouseEvent.TAG);
			writer.WriteElementString(EyetrackXML.MouseEvent.Event.TAG, mouseEventPart.MouseEvent);
			writer.WriteElementString(EyetrackXML.MouseEvent.EventNumber.TAG, mouseEventPart.MouseEventNumber);
			writer.WriteEndElement();
		}

        private static void WritePart(MediaPart mediaPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.Media.TAG);
			writer.WriteElementString(EyetrackXML.Media.MediaName.TAG, mediaPart.MediaName);
			writer.WriteElementString(EyetrackXML.Media.MediaHeight.TAG, mediaPart.MediaHeight);
			writer.WriteElementString(EyetrackXML.Media.MediaWidth.TAG, mediaPart.MediaWidth);
			writer.WriteElementString(EyetrackXML.Media.MediaPosX.TAG, mediaPart.MediaPosX);
			writer.WriteElementString(EyetrackXML.Media.MediaPosY.TAG, mediaPart.MediaPosY);
			writer.WriteEndElement();
		}

        private static void WritePart(KeyboardEventPart keyboardEventPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.KeyboardEvent.TAG);
			writer.WriteElementString(EyetrackXML.KeyboardEvent.Event.TAG, keyboardEventPart.KeyPressEvent);
			writer.WriteElementString(EyetrackXML.KeyboardEvent.EventNumber.TAG, keyboardEventPart.KeyboardEventNumber);
			writer.WriteEndElement();
		}

        private static void WritePart(GazeEventPart gazeEventPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.Gaze.TAG);
			// basic
			writer.WriteElementString(EyetrackXML.Gaze.FixationIndex.TAG, gazeEventPart.FixationIndex);
			writer.WriteElementString(EyetrackXML.Gaze.SaccadeIndex.TAG, gazeEventPart.SaccadeIndex);
			writer.WriteElementString(EyetrackXML.Gaze.GazeEventType.TAG, gazeEventPart.GazeEventType);
			writer.WriteElementString(EyetrackXML.Gaze.GazeEventDuration.TAG, gazeEventPart.GazeEventDuration);

			// averages
			writer.WriteElementString(EyetrackXML.Gaze.AverageGazePointX.TAG, gazeEventPart.AverageGazePointX_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.AverageGazePointY.TAG, gazeEventPart.AverageGazePointY_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.AveragePupilLeft.TAG, gazeEventPart.AveragePupilLeft);
			writer.WriteElementString(EyetrackXML.Gaze.AveragePupilRight.TAG, gazeEventPart.AveragePupilRight);
			writer.WriteElementString(EyetrackXML.Gaze.AverageValidityLeft.TAG, gazeEventPart.AverageValidityLeft);
			writer.WriteElementString(EyetrackXML.Gaze.AverageValidityRight.TAG, gazeEventPart.AverageValidityRight);
			writer.WriteElementString(EyetrackXML.Gaze.OffscreenTime.TAG, gazeEventPart.OffscreenTime);
			writer.WriteElementString(EyetrackXML.Gaze.NrOfSamples.TAG, gazeEventPart.NumberOfSamples);
			writer.WriteElementString(EyetrackXML.Gaze.NrOfValidSamples.TAG, gazeEventPart.NumberOfValidSamples);

			// minimums
			writer.WriteElementString(EyetrackXML.Gaze.MinGazePointX_ADSCpx.TAG, gazeEventPart.MinGazePointX_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MinGazePointX_MDSpx.TAG, gazeEventPart.MinGazePointX_MCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MinGazePointY_ADSCpx.TAG, gazeEventPart.MinGazePointY_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MinGazePointY_MDSpx.TAG, gazeEventPart.MinGazePointY_MCSpx);
			
			// maximums
			writer.WriteElementString(EyetrackXML.Gaze.MaxDistanceX.TAG, gazeEventPart.MaxDistanceX);
			writer.WriteElementString(EyetrackXML.Gaze.MaxDistanceY.TAG, gazeEventPart.MaxDistanceY);
			writer.WriteElementString(EyetrackXML.Gaze.MaxGazePointX_ADSCpx.TAG, gazeEventPart.MaxGazePointX_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MaxGazePointX_MDSpx.TAG, gazeEventPart.MaxGazePointX_MCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MaxGazePointY_ADSCpx.TAG, gazeEventPart.MaxGazePointY_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.MaxGazePointY_MDSpx.TAG, gazeEventPart.MaxGazePointY_MCSpx);

			// start ends
			writer.WriteElementString(EyetrackXML.Gaze.StartGazePointX_ADSCpx.TAG, gazeEventPart.StartGazePointX_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.StartGazePointY_ADSCpx.TAG, gazeEventPart.StartGazePointY_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.EndGazePointX_ADSCpx.TAG, gazeEventPart.EndGazePointX_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.EndGazePointY_ADSCpx.TAG, gazeEventPart.EndGazePointY_ADCSpx);
			writer.WriteElementString(EyetrackXML.Gaze.DistanceX.TAG, gazeEventPart.DistanceX);
			writer.WriteElementString(EyetrackXML.Gaze.DistanceY.TAG, gazeEventPart.DistanceY);

			// cumulative
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceX.TAG, gazeEventPart.CumulativeAbsoluteDistanceX);
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceY.TAG, gazeEventPart.CumulativeAbsoluteDistanceY);
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceX_Left.TAG, gazeEventPart.CumulativeAbsoluteDistance_LeftX);
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceX_Right.TAG, gazeEventPart.CumulativeAbsoluteDistance_RightX);
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceY_Down.TAG, gazeEventPart.CumulativeAbsoluteDistance_DownY);
			writer.WriteElementString(EyetrackXML.Gaze.CumulativeAbsoluteDistanceY_Up.TAG, gazeEventPart.CumulativeAbsoluteDistance_UpY);

			writer.WriteEndElement();
		}

        private static void WritePart(EyePositionPart eyePositionPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.EyePosition.TAG);
			writer.WriteElementString(EyetrackXML.EyePosition.DistanceLeft_MAX.TAG, eyePositionPart.DistanceLeft_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.DistanceLeft_MIN.TAG, eyePositionPart.DistanceLeft_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.DistanceRight_MAX.TAG, eyePositionPart.DistanceRight_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.DistanceRight_MIN.TAG, eyePositionPart.DistanceRight_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftX_MAX.TAG, eyePositionPart.EyePosLeftX_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftX_MIN.TAG, eyePositionPart.EyePosLeftX_ADCSmm_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftY_MAX.TAG, eyePositionPart.EyePosLeftY_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftY_MIN.TAG, eyePositionPart.EyePosLeftY_ADCSmm_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftZ_MAX.TAG, eyePositionPart.EyePosLeftZ_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosLeftZ_MIN.TAG, eyePositionPart.EyePosLeftZ_ADCSmm_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightX_MAX.TAG, eyePositionPart.EyePosRightX_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightX_MIN.TAG, eyePositionPart.EyePosRightX_ADCSmm_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightY_MAX.TAG, eyePositionPart.EyePosRightY_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightY_MIN.TAG, eyePositionPart.EyePosRightY_ADCSmm_MIN);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightZ_MAX.TAG, eyePositionPart.EyePosRightZ_ADCSmm_MAX);
			writer.WriteElementString(EyetrackXML.EyePosition.EyePosRightZ_MIN.TAG, eyePositionPart.EyePosRightZ_ADCSmm_MIN);
			writer.WriteEndElement();
		}

        private static void WritePart(ExternalEventPart externalEventPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.ExternalEvent.TAG);
			writer.WriteElementString(EyetrackXML.ExternalEvent.Event.TAG, externalEventPart.ExternalEvent);
			writer.WriteElementString(EyetrackXML.ExternalEvent.EventValue.TAG, externalEventPart.ExternalEventValue);
			writer.WriteEndElement();
		}

        private static void WritePart(AOIPart aOIPart, XmlWriter writer)
		{
			writer.WriteStartElement(EyetrackXML.AOI.TAG);
			foreach (KeyValuePair<string, string> aoiHit in aOIPart.AOIHits)
			{
				writer.WriteStartElement(EyetrackXML.AOI.AOI_HIT.TAG);
				writer.WriteAttributeString(EyetrackXML.AOI.AOI_HIT.ATTRIBUTES.NAME.KEY, SecurityElement.Escape(aoiHit.Key));
				writer.WriteString(aoiHit.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
