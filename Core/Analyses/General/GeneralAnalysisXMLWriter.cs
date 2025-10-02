using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Merging.Analyses;
using InputLog.Core.Util;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;

namespace InputLog.Core.Analyses.General
{
    /// <summary>
    /// Analysis XMLTxtWriter for the General Analysis.
    /// </summary>
    public class GeneralAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Fields
        // The last start time with a value > 0. Used in events without a time stamp.
        private ulong LastStartTime;
        // The last end time with a value > 0. Used in events without a time stamp.
        private ulong LastEndTime;
        // Last position seen. Used to fill empty position slots.
        private int LastPosition;
        // Last doclength seen? Used to fill empty doc length slots.
        private int LastDoclength;
        // A comma separated value file is generated when 'true'
        public static bool GenerateCSV { private get; set; }
        #endregion

        #region Constants
        // Tags and tag-values used in the analysis XML document.
        // Not const so that extending classes may provide a different .xsl
        private readonly string StylesheetHref = "general_analysis.xsl"; 

        private const string ID_TAG = "id";
        private const string EVENT_TAG = "event";
        private const string TYPE_TAG = "type";
        private const string OUTPUT_TAG = "output";
        private const string POSITION_TAG = "position";
        private const string POSITION_FULL_TAG = "positionFull";
        private const string DOCLENGTH_TAG = "doclength";
        private const string DOCLENGTH_FULL_TAG = "doclengthFull";
        private const string START_TIME_TAG = "startTime";
        private const string START_CLOCK_TAG = "startClock";
        private const string END_TIME_TAG = "endTime";
        private const string END_CLOCK_TAG = "endClock";
        private const string ACTION_TIME_TAG = "actionTime";
        private const string PAUSE_TIME_TAG = "pauseTime";
        private const string PAUSE_LOCATION_TAG = "pauseLocation";
        private const string PAUSE_LOCATION_FULL_TAG = "pauseLocationFull";
        private const string CHAR_PRODUCTION_TAG = "charProduction";
        //private const string RANGE_LENGTH_TAG = "rangeLength"; //deprecated
        private const string RESOURCE_TAG = "resource";
        private const string INTERVALSIZE_TAG = "intervalSize";
        private const string INTERVALNUMBER_TAG = "intervalNumber";
        private const string X_TAG = "x";
        private const string Y_TAG = "y";      
        private readonly string FilePath;

		// Eyetracking
		protected const string INCLUDE_ET = "IncludeEyetracking";
		protected const string INCLUDE_ET_AOI = "IncludeAOIs";
		// MEDIA
		private const string ET_MEDIANAME = "mediaName";
		private const string ET_MEDIAPOSX = "mediaPosX_ADCSpx";
		private const string ET_MEDIAPOSY = "mediaPosY_ADCSpx";
		private const string ET_MEDIAWIDTH = "mediaWidth";
		private const string ET_MEDIAHEIGHT = "mediaHeight";
		// SEGMENTS & SCENES
		private const string ET_SEGMENTNAME = "segmentName";
		private const string ET_SEGMENTSTART = "segmentStart";
		private const string ET_SEGMENTEND = "segmentEnd";
		private const string ET_SEGMENTDURATION = "segmentDuration";
		private const string ET_SCENENAME = "sceneName";
		private const string ET_SCENESTART = "sceneStart";
		private const string ET_SCENEEND = "sceneEnd";
		private const string ET_SCENEDURATION = "sceneDuration";
		// TIME IS PLACED WHERE IPL TIME IS PLACED AND REPEATED HERE
		private const string ET_TIMESTART = "et_startTime";
		private const string ET_TIMEEND = "et_endTime";
		private const string ET_TIMEDURATION = "et_duration";
		private const string ET_TIMEFULLSTAMP = "et_fullTimestamp";
		// MOUSE
		protected const string ET_MOUSEEVENT = "mouseEvents";
		protected const string ET_MOUSEEVENTNUMBER = "mouseNumberOfEvents";
		// KEYBOARD
		protected const string ET_KEYBOARDEVENT = "keyboardEvents";
		protected const string ET_KEYBOARDEVENTNUMBER = "keyboardNumberOfEvents";
		// STUDIO
		protected const string ET_STUDIOEVENT = "studioEvent";
		protected const string ET_STUDIOEVENTVALUE = "studioEventValue";
		// EXTERNAL
		protected const string ET_EXTERNALEVENT = "externalEvent";
		protected const string ET_EXTERNALEVENTVALUE = "externalEventValue";
		// GAZE
		protected const string ET_FIXATIONINDEX = "fixationIndex";
		protected const string ET_SACCADEINDEX = "saccadeIndex";
		protected const string ET_GAZEEVENTTYPE = "gazeEventType";
		protected const string ET_GAZEEVENTDURATION = "gazeEventDuration";
		protected const string ET_AVERAGEGAZEPOINTX = "averageGazePointX_ADCSpx";
		protected const string ET_AVERAGEGAZEPOINTY = "averageGazePointY_ADCSpx";
		protected const string ET_AVERAGEVALIDITYLEFT = "averageValidityLeft";
		protected const string ET_AVERAGEVALIDITYRIGHT = "averageValidityRight";
		protected const string ET_AVERAGEPUPILLEFT = "averagePupilLeft";
		protected const string ET_AVERAGEPUPILRIGHT = "averagePupilRight";
		protected const string ET_OFFSCREENTIME = "offscreenTime";
		protected const string ET_NUMBEROFSAMPLES = "nrOfSamples";
		protected const string ET_NUMBEROFVALIDSAMPLES = "nrOfValidSamples";
		protected const string ET_MINGAZEPOINTX_MCSPX = "minGazePointX_MCSpx";
		protected const string ET_MAXGAZEPOINTX_MCSPX = "maxGazePointX_MCSpx";
		protected const string ET_MINGAZEPOINTX_ADCSPX = "minGazePointX_ADCSpx";
		protected const string ET_MAXGAZEPOINTX_ADCSPX = "maxGazePointX_ADCSpx";
		protected const string ET_STARTGAZEPOINTX = "startGazePointX_ADCSpx";
		protected const string ET_ENDGAZEPOINTX = "endGazePointX_ADCSpx";
		protected const string ET_MAXDISTANCEX = "maxDistanceX";
		protected const string ET_DISTANCEX = "distanceX";
		protected const string ET_CUMABSDISTANCEX = "cumAbsDistanceX";
		protected const string ET_CUMABSDISTANCEX_LEFT = "cumAbsDistanceX_Left";
		protected const string ET_CUMABSDISTANCEX_RIGHT = "cumAbsDistanceX_Right";
		protected const string ET_MINGAZEPOINTY_MCSPX = "minGazePointY_MCSpx";
		protected const string ET_MAXGAZEPOINTY_MCSPX = "maxGazePointY_MCSpx";
		protected const string ET_MINGAZEPOINTY_ADCSPX = "minGazePointY_ADCSpx";
		protected const string ET_MAXGAZEPOINTY_ADCSPX = "maxGazePointY_ADCSpx";
		protected const string ET_STARTGAZEPOINTY = "startGazePointY_ADCSpx";
		protected const string ET_ENDGAZEPOINTY = "endGazePointY_ADCSpx";
		protected const string ET_MAXDISTANCEY = "maxDistanceY";
		protected const string ET_DISTANCEY = "distanceY";
		protected const string ET_CUMABSDISTANCEY = "cumAbsDistanceY";
		protected const string ET_CUMABSDISTANCEY_UP = "cumAbsDistanceY_Up";
		protected const string ET_CUMABSDISTANCEY_DOWN = "cumAbsDistanceY_Down";
		// EYEPOSITION
		protected const string ET_EYEPOSLEFTX_MIN = "eyePosLeftX_Min";
		protected const string ET_EYEPOSLEFTY_MIN = "eyePosLeftY_Min";
		protected const string ET_EYEPOSLEFTZ_MIN = "eyePosLeftZ_Min";
		protected const string ET_EYEPOSRIGHTX_MIN = "eyePosRightX_Min";
		protected const string ET_EYEPOSRIGHTY_MIN = "eyePosRightY_Min";
		protected const string ET_EYEPOSRIGHTZ_MIN = "eyePosRightZ_Min";
		protected const string ET_EYEPOSDISTANCELEFT_MIN = "distanceLeft_Min";
		protected const string ET_EYEPOSDISTANCERIGHT_MIN = "distanceRight_Min";
		protected const string ET_EYEPOSLEFTX_MAX = "eyePosLeftX_Max";
		protected const string ET_EYEPOSLEFTY_MAX = "eyePosLeftY_Max";
		protected const string ET_EYEPOSLEFTZ_MAX = "eyePosLeftZ_Max";
		protected const string ET_EYEPOSRIGHTX_MAX = "eyePosRightX_Max";
		protected const string ET_EYEPOSRIGHTY_MAX = "eyePosRightY_Max";
		protected const string ET_EYEPOSRIGHTZ_MAX = "eyePosRightZ_Max";
		protected const string ET_EYEPOSDISTANCELEFT_MAX = "distanceLeft_Max";
		protected const string ET_EYEPOSDISTANCERIGHT_MAX = "distanceRight_Max";
		// AOIS
		protected const string ET_AOINAMES = "aoiNames";
		protected const string ET_AOINAME = "aoiName";
		protected const string ET_AOINUMBER = "aoiNumberOf";
		protected const string ET_AOIHITS = "aoiHits";
		protected const string ET_AOIHIT = "aoi";

		protected bool IncludeEyetracking;
		protected bool FinishEyetrackEvents = true;

        #endregion

        /// <summary>
        /// Constructs a GeneralAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public GeneralAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
            FilePath = destinationFilePath;
        }

		protected GeneralAnalysisXMLWriter(string destinationFilePath, string stylesheetHref)
			:base(destinationFilePath)
		{
			FilePath = destinationFilePath;
			StylesheetHref = stylesheetHref;
		}

        /// <summary>
        /// Writes out the analysis document using a given GeneralAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which
        ///  the General Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is GeneralAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type GeneralAnalysisSummary");
            WriteHeader(StylesheetHref);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);

            // Information regarding a possible author comment.
            if (Settings.AddComment)
            {
                extraInfo["Additional info"] = new Dictionary<string, object>
                {
                    ["Author Comment"] = "The idfx-file of this participant contains a comment"
                };
            }

            WriteExtraInfo(extraInfo);
            WriteEvents(((GeneralAnalysisSummary)summary).Events);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { StylesheetHref, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
            CopyText(HTML_README);
            XMLWriter.Close();

            // A csv version of the xml is generated if the user has checked the checkbox in the GUI.
            if (!GenerateCSV) return;
            var generalXML = new List<string> {FilePath};
            var csvMaker = new AnalysisMerge(generalXML, Path.GetDirectoryName(FilePath));
            csvMaker.Merge(AnalysisMerge.Direction.VERTICAL);
        }

        /// <summary>
        /// Writes out the GeneralAnalysisEvents.
        /// </summary>
        /// <param name="events">List of GeneralAnalysisEvent's to write to the analysis document.</param>
        protected virtual void WriteEvents(IEnumerable<GeneralAnalysisSummary.GeneralAnalysisEvent> events)
        {
			// Check if there's eyetrack information somewhere in any of the events.
			// Check if there's areas of interest defined
			// Count the areas of interest and get all the different aoi names.
			bool includeAOIs = false;
			int numberOfAOIs = 0;
			var aoiNames = new List<string>();
            var generalAnalysisEvents = events as IList<GeneralAnalysisSummary.GeneralAnalysisEvent> ?? events.ToList();
            foreach (var @event in generalAnalysisEvents)
			{
			    if (@event.Eyetrack == null) continue;
			    IncludeEyetracking = true;
			    var aoi = @event.Eyetrack.GetFirstSubPart<AOIPart>();
			    if (aoi != null)
			    {
			        includeAOIs = true;
			        numberOfAOIs = aoi.AOIHits.Count;
			        aoiNames.AddRange(aoi.AOIHits.Keys);
			    }
			    break;
			}
			// Write all this 'preprocessed' knowledge in separate elements to the general file.
			XMLWriter.WriteAttributeElement(INCLUDE_ET, "Value", IncludeEyetracking.ToString());
			XMLWriter.WriteAttributeElement(INCLUDE_ET_AOI, "Value", includeAOIs.ToString());
			XMLWriter.WriteAttributeElement(ET_AOINUMBER, "Value", numberOfAOIs.ToString());
			XMLWriter.WriteStartElement(ET_AOINAMES);
			foreach (string aoiName in aoiNames)
			{
				XMLWriter.WriteElementString(ET_AOINAME, aoiName);
			}
			XMLWriter.WriteEndElement();

            GeneralAnalysisSummary.GeneralAnalysisEvent placeHolderEvent = null;
			//GeneralAnalysisSummary.GeneralAnalysisEvent prevEvent = null;

			bool includeRevisions = Revisions != null;
            var openRevisions = new List<IRevision>();
            IRevision openRev = null;
            int lastRevID = 0;
			XMLWriter.WriteAttributeElement("IncludeRevisions", "Value", includeRevisions.ToString());

			var iterator = generalAnalysisEvents.GetEnumerator();
			iterator.MoveNext();
            var prevEvents = new List<GeneralAnalysisSummary.GeneralAnalysisEvent>();
            while (iterator.Current != null)
            {
                var outputEvent = iterator.Current;

                //if (outputEvent.Id.Equals("169"))
                //{
                //}

                iterator.MoveNext();
                // A 'placeholder' event is not written to the HTML page. 
                if (outputEvent.Type.Equals("placeholder"))
                {
                    placeHolderEvent = outputEvent;
                    continue;
                }
                // The event following a placeholder takes its pauseTime and adds actionTime and PauseTime
                // to its own actionTime.
                if (placeHolderEvent != null && placeHolderEvent.Type.Equals("placeholder"))
                {
                    outputEvent.ActionTime += placeHolderEvent.ActionTime + placeHolderEvent.PauseTime;
                    outputEvent.PauseTime = placeHolderEvent.PauseTime;
                    placeHolderEvent = null;
                }

                // Start writing the events
                XMLWriter.WriteStartElement(EVENT_TAG);
                WriteIfValueNotNull(ID_TAG, outputEvent.Id);
                WriteIfValueNotNull(TYPE_TAG, outputEvent.Type);
                WriteIfValueNotNull(OUTPUT_TAG, StringUtils.ReplaceNonPrintableCharacters(outputEvent.Output));

                if (outputEvent.Position.HasValue)
                {
                    LastPosition = (int) outputEvent.Position;
                    WriteIfValueNotNull(POSITION_TAG, outputEvent.Position);
                    WriteIfValueNotNull(POSITION_FULL_TAG, outputEvent.Position);
                }
                // When an event has no position the last available position is used instead 
                else
                {
                    WriteIfValueNotNull(POSITION_FULL_TAG, LastPosition);
                }

                if(outputEvent.DocLength.HasValue)
                {
                    LastDoclength = (int) outputEvent.DocLength;
                    WriteIfValueNotNull(DOCLENGTH_TAG, outputEvent.DocLength);
                    WriteIfValueNotNull(DOCLENGTH_FULL_TAG, outputEvent.DocLength);
                }
                // When an event has no doclength the last available doclength is used instead 
                else
                {
                    WriteIfValueNotNull(DOCLENGTH_FULL_TAG, LastDoclength);
                }

                // Total number of characters produced so far.
                WriteIfValueNotNull(CHAR_PRODUCTION_TAG, outputEvent.CharProduction);

                // Determine and write out the start- and endclock using the start- and endtime.
                if (outputEvent.StartTime.HasValue && outputEvent.StartTime > 0)
                {
                    LastStartTime = (ulong)outputEvent.StartTime;
                    WriteIfValueNotNull("RawStart", outputEvent.RawStartTime);
                    WriteIfValueNotNull("RawEnd", outputEvent.RawEndTime);
                    WriteIfValueNotNull(START_TIME_TAG, outputEvent.StartTime);
                    WriteIfValueNotNull(START_CLOCK_TAG, DateTimeUtils.MsecToClockString(outputEvent.StartTime.Value));
                }
                // When an event has no startTime the last available startTime of a previous event is taken instead
                else
                {
                    WriteIfValueNotNull(START_TIME_TAG, LastStartTime);
                    WriteIfValueNotNull(START_CLOCK_TAG, DateTimeUtils.MsecToClockString(LastStartTime));
                }

                // Empty EndTime and EndClock is filled with last available value.
                if (outputEvent.EndTime.HasValue && outputEvent.EndTime > 0)
                {
                    LastEndTime = (ulong) outputEvent.EndTime;
                    WriteIfValueNotNull(END_TIME_TAG, outputEvent.EndTime);
                    WriteIfValueNotNull(END_CLOCK_TAG, DateTimeUtils.MsecToClockString(outputEvent.EndTime.Value));
                }
                // When an event has no endTime the last available endTime of a previous event is taken instead
                // and augmented with the pauseTime of the event without the endTime.
                else
                {
                    var newTime = LastEndTime + outputEvent.PauseTime;
                    WriteIfValueNotNull(END_TIME_TAG, newTime);
                    WriteIfValueNotNull(END_CLOCK_TAG, DateTimeUtils.MsecToClockString((ulong)newTime));
                }
                WriteIfValueNotNull(ACTION_TIME_TAG, outputEvent.ActionTime);
                WriteIfValueNotNull(PAUSE_TIME_TAG, outputEvent.PauseTime, "Unable to set a pausetime");

                if (outputEvent.PauseLocation != PauseLocation.UNDETERMINED)
                {
                    WriteIfValueNotNull(PAUSE_LOCATION_TAG, ((int)outputEvent.PauseLocation).ToString(CultureInfo.InvariantCulture));
                    var pauseLocationFull = outputEvent.PauseLocation.ToString();
                    pauseLocationFull = pauseLocationFull.Replace("_", " ");

                    WriteIfValueNotNull(PAUSE_LOCATION_FULL_TAG, pauseLocationFull);
                }
                // WriteIfValueNotNull(RANGE_LENGTH_TAG, outputEvent.RangeLength);
                WriteIfValueNotNull(RESOURCE_TAG, outputEvent.Resource);
                WriteIfValueNotNull(INTERVALSIZE_TAG, outputEvent.FixedSizeInterval);
                WriteIfValueNotNull(INTERVALNUMBER_TAG, outputEvent.FixedNumberInterval);
                WriteIfValueNotNull(X_TAG, outputEvent.X);
                WriteIfValueNotNull(Y_TAG, outputEvent.Y);

                // Link event to revision if necessary
                if (includeRevisions)
                {
                    bool done = false;
                    int newRevId = lastRevID;
                    // Check whether revision starts here
                    foreach (RevisionMatrixEntry revEntry in Revisions)
                    {
                        if (revEntry.TypeOfRevision != null && revEntry.TypeOfRevision.Equals("Normal Production"))
                        {
                            continue;
                        }
                        IRevision rev = revEntry.Revision;
                        int revID = rev.RevisionNumber;
                        bool startsHere = rev.StartTime.Equals(outputEvent.RawStartTime) 
                            && (outputEvent.RawStartTime > 0 || revID == lastRevID+1);
                        ulong lastTimedEventTime = FindLastTimedEventTime(prevEvents);
                        ulong nextTimedEventTime = FindNextTimedEventTime(outputEvent, iterator.Current);
                        bool startsInBetween = lastTimedEventTime > 0 && lastTimedEventTime <= rev.StartTime 
                            && nextTimedEventTime > rev.StartTime;
                        if ((!startsHere && !startsInBetween) || openRev == rev) continue;

                        //if (outputEvent.Id.Equals("1803") || outputEvent.Id.Equals("1806"))
                        //{
                        //}

                        // Single-event revision?
                        if (rev.EndTime <= iterator.Current.RawStartTime)
                        {
                            newRevId = revID;
                            WriteRevisionInfo(XMLWriter, rev, "H");
                        }
                        else
                        {
                            newRevId = revID;
                            openRev = rev;
                            WriteRevisionInfo(XMLWriter, rev, "B");
                        }
                        done = true;
                    }
                    lastRevID = newRevId;

                    if (!done && openRev != null)
                    {
                        // Check whether revision ends here
                        if (iterator.Current == null || openRev.EndTime < iterator.Current.RawStartTime)
                        {
                            WriteRevisionInfo(XMLWriter, openRev, "E");
                            openRev = null;
                            done = true;
                        }
                        if (!done)
                        {
                            WriteRevisionInfo(XMLWriter, openRev, "M");
                            done = true;
                        }
                    }
                    if (!done)
                    {
                        XMLWriter.WriteStartElement("RevisionInfo");
                        XMLWriter.WriteElementString("RevisionNumber", "");
                        XMLWriter.WriteElementString("RevisionPos", "");
                        XMLWriter.WriteElementString("RevisionType", "PRODUCTION");
                        XMLWriter.WriteEndElement();
                    }
                }

				// Write eyetracking information.
				if (IncludeEyetracking)
				{
					WriteEyetracking(XMLWriter, outputEvent);
				}

                XMLWriter.WriteEndElement();
                prevEvents.Add(outputEvent);
            }
        }

        private ulong FindNextTimedEventTime(GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent,
            GeneralAnalysisSummary.GeneralAnalysisEvent nextEvent)
        {
            if (outputEvent.RawStartTime > 0) return outputEvent.RawStartTime;
            if (outputEvent.RawEndTime > 0) return outputEvent.RawEndTime;
            if (nextEvent.RawStartTime > 0) return nextEvent.RawStartTime;
            return nextEvent.RawEndTime > 0 ? nextEvent.RawEndTime : 0;
        }

        private ulong FindLastTimedEventTime(List<GeneralAnalysisSummary.GeneralAnalysisEvent> prevEvents)
        {
            for (int i = prevEvents.Count-1; i >= 0; i--)
            {
                if (prevEvents[i].RawEndTime > 0) return prevEvents[i].RawEndTime;
                if (prevEvents[i].RawStartTime > 0) return prevEvents[i].RawStartTime;
            }
            return 0;
        }


        protected virtual void WriteEyetracking(XmlTextWriter xmlTxtWriter, GeneralAnalysisSummary.GeneralAnalysisEvent outputEvent)
		{
			EyetrackPart eyetrack = outputEvent.Eyetrack;
			AOIPart aoiPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<AOIPart>() : null;
			ExternalEventPart extPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<ExternalEventPart>() : null;
			EyePositionPart eyeposPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<EyePositionPart>() : null;
			GazeEventPart gazePart = (eyetrack != null) ? eyetrack.GetFirstSubPart<GazeEventPart>() : null;
			IPLTimePart timePart = (eyetrack != null) ? eyetrack.GetFirstSubPart<IPLTimePart>() : null;
			KeyboardEventPart keyboardPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<KeyboardEventPart>() : null;
			MediaPart mediaPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<MediaPart>() : null;
			MouseEventPart mousePart = (eyetrack != null) ? eyetrack.GetFirstSubPart<MouseEventPart>() : null;
			SegmentPart segmentPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<SegmentPart>() : null;
			StudioEventPart studioPart = (eyetrack != null) ? eyetrack.GetFirstSubPart<StudioEventPart>() : null;

			if (aoiPart != null)
			{
				xmlTxtWriter.WriteStartElement(ET_AOIHITS);
				// IMPORTANT:
				// The values are written in the same order as the headers of the
				// general table (containing the AOI corresponding names for the hits) have
				// been printed. Because this order is consistent, the same correct value should be 
				// printed at their correct aoiHit
				// If the order is changed, either here, or in the printing of the titles, the order
				// must be changed consistently, so that the right AOIHits are printed in the right
				// columns
				foreach (string aoiHit in aoiPart.AOIHits.Values)
				{
					xmlTxtWriter.WriteElementString(ET_AOIHIT, aoiHit);
				}
				xmlTxtWriter.WriteEndElement();
			}

			if (extPart != null)
			{
				xmlTxtWriter.WriteElementString(ET_EXTERNALEVENT, extPart.ExternalEvent);
				xmlTxtWriter.WriteElementString(ET_EXTERNALEVENTVALUE, extPart.ExternalEventValue);
			}

			if (mediaPart != null)
			{
				xmlTxtWriter.WriteElementString(ET_MEDIANAME, mediaPart.MediaName);
				xmlTxtWriter.WriteElementString(ET_MEDIAPOSX, mediaPart.MediaPosX);
				xmlTxtWriter.WriteElementString(ET_MEDIAPOSY, mediaPart.MediaPosY);
				xmlTxtWriter.WriteElementString(ET_MEDIAHEIGHT, mediaPart.MediaHeight);
				xmlTxtWriter.WriteElementString(ET_MEDIAWIDTH, mediaPart.MediaWidth);
			}

			if (segmentPart != null)
			{
				xmlTxtWriter.WriteElementString(ET_SEGMENTNAME, segmentPart.SegmentName);
				xmlTxtWriter.WriteElementString(ET_SEGMENTDURATION, segmentPart.SegmentDuration);
				xmlTxtWriter.WriteElementString(ET_SEGMENTEND, (!string.IsNullOrEmpty(segmentPart.SegmentEndIPLReferenced) ?
                    DateTimeUtils.MsecToClockString(ulong.Parse(segmentPart.SegmentEndIPLReferenced), false) : ""));
				xmlTxtWriter.WriteElementString(ET_SEGMENTSTART, (!string.IsNullOrEmpty(segmentPart.SegmentStartIPLReferenced) ?
                    DateTimeUtils.MsecToClockString(ulong.Parse(segmentPart.SegmentStartIPLReferenced), false) : ""));

				xmlTxtWriter.WriteElementString(ET_SCENEDURATION, segmentPart.SceneSegmentDuration);
				xmlTxtWriter.WriteElementString(ET_SCENENAME, segmentPart.SceneName);
				xmlTxtWriter.WriteElementString(ET_SCENEEND, (!string.IsNullOrEmpty(segmentPart.SceneSegmentEndIPLReferenced) ?
                    DateTimeUtils.MsecToClockString(ulong.Parse(segmentPart.SceneSegmentEndIPLReferenced), false) : ""));
				xmlTxtWriter.WriteElementString(ET_SCENESTART, (!string.IsNullOrEmpty(segmentPart.SceneSegmentStartIPLReferenced) ?
                    DateTimeUtils.MsecToClockString(ulong.Parse(segmentPart.SceneSegmentStartIPLReferenced), false) : ""));
			}

			if (timePart != null)
			{
				xmlTxtWriter.WriteElementString(ET_TIMEDURATION, timePart.Duration);
				xmlTxtWriter.WriteElementString(ET_TIMEEND, timePart.EndTimeIplReferenced);
				xmlTxtWriter.WriteElementString(ET_TIMEFULLSTAMP, timePart.FullLocalTimestamp);
				xmlTxtWriter.WriteElementString(ET_TIMESTART, timePart.StartTimeIplReferenced);
			}

			if (mousePart != null)
			{
				xmlTxtWriter.WriteElementString(ET_MOUSEEVENT, mousePart.MouseEvent);
				xmlTxtWriter.WriteElementString(ET_MOUSEEVENTNUMBER, mousePart.MouseEventNumber);
			}

			if (keyboardPart != null)
			{
				xmlTxtWriter.WriteElementString(ET_KEYBOARDEVENT, keyboardPart.KeyPressEvent);
				xmlTxtWriter.WriteElementString(ET_KEYBOARDEVENTNUMBER, keyboardPart.KeyboardEventNumber);
			}

			if (studioPart != null)
			{
				xmlTxtWriter.WriteElementString(ET_STUDIOEVENT, studioPart.StudioEvent);
				xmlTxtWriter.WriteElementString(ET_STUDIOEVENTVALUE, studioPart.StudioEventData);
			}

			if (gazePart != null)
			{
				xmlTxtWriter.WriteElementString(ET_FIXATIONINDEX, gazePart.FixationIndex);
				xmlTxtWriter.WriteElementString(ET_SACCADEINDEX, gazePart.SaccadeIndex);
				xmlTxtWriter.WriteElementString(ET_GAZEEVENTTYPE, gazePart.GazeEventType);
				xmlTxtWriter.WriteElementString(ET_GAZEEVENTDURATION, gazePart.GazeEventDuration);

				xmlTxtWriter.WriteElementString(ET_AVERAGEGAZEPOINTX, gazePart.InterpretData().AverageGazePointX_ADCSpx.ToString("F2"));
				xmlTxtWriter.WriteElementString(ET_AVERAGEGAZEPOINTY, gazePart.InterpretData().AverageGazePointY_ADCSpx.ToString("F2"));
				xmlTxtWriter.WriteElementString(ET_AVERAGEPUPILLEFT, 
                    gazePart.InterpretData().AveragePupilLeft.ToString("F2"));
				xmlTxtWriter.WriteElementString(ET_AVERAGEPUPILRIGHT, 
                    gazePart.InterpretData().AveragePupilRight.ToString("F2"));
				xmlTxtWriter.WriteElementString(ET_AVERAGEVALIDITYLEFT, gazePart.InterpretData().AverageValidityLeft.ToString("F2"));
				xmlTxtWriter.WriteElementString(ET_AVERAGEVALIDITYRIGHT, gazePart.InterpretData().AverageValidityRight.ToString("F2"));

				xmlTxtWriter.WriteElementString(ET_OFFSCREENTIME, gazePart.OffscreenTime);
				xmlTxtWriter.WriteElementString(ET_NUMBEROFSAMPLES, gazePart.NumberOfSamples);
				xmlTxtWriter.WriteElementString(ET_NUMBEROFVALIDSAMPLES, gazePart.NumberOfValidSamples);

				xmlTxtWriter.WriteElementString(ET_MINGAZEPOINTX_ADCSPX, gazePart.MinGazePointX_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_MINGAZEPOINTX_MCSPX, gazePart.MinGazePointX_MCSpx);
				xmlTxtWriter.WriteElementString(ET_MINGAZEPOINTY_ADCSPX, gazePart.MinGazePointY_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_MINGAZEPOINTY_MCSPX, gazePart.MinGazePointY_MCSpx);

				xmlTxtWriter.WriteElementString(ET_MAXGAZEPOINTX_ADCSPX, gazePart.MaxGazePointX_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_MAXGAZEPOINTX_MCSPX, gazePart.MaxGazePointX_MCSpx);
				xmlTxtWriter.WriteElementString(ET_MAXGAZEPOINTY_ADCSPX, gazePart.MaxGazePointY_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_MAXGAZEPOINTY_MCSPX, gazePart.MaxGazePointY_MCSpx);

				xmlTxtWriter.WriteElementString(ET_MAXDISTANCEX, gazePart.MaxDistanceX);
				xmlTxtWriter.WriteElementString(ET_MAXDISTANCEY, gazePart.MaxDistanceY);

				xmlTxtWriter.WriteElementString(ET_DISTANCEX, gazePart.DistanceX);
				xmlTxtWriter.WriteElementString(ET_DISTANCEY, gazePart.DistanceY);

				xmlTxtWriter.WriteElementString(ET_STARTGAZEPOINTX, gazePart.StartGazePointX_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_STARTGAZEPOINTY, gazePart.StartGazePointY_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_ENDGAZEPOINTX, gazePart.EndGazePointX_ADCSpx);
				xmlTxtWriter.WriteElementString(ET_ENDGAZEPOINTY, gazePart.EndGazePointY_ADCSpx);

				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEX, gazePart.CumulativeAbsoluteDistanceX);
				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEX_LEFT, gazePart.CumulativeAbsoluteDistance_LeftX);
				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEX_RIGHT, gazePart.CumulativeAbsoluteDistance_RightX);
				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEY, gazePart.CumulativeAbsoluteDistanceY);
				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEY_DOWN, gazePart.CumulativeAbsoluteDistance_DownY);
				xmlTxtWriter.WriteElementString(ET_CUMABSDISTANCEY_UP, gazePart.CumulativeAbsoluteDistance_UpY);
			}

            if (eyeposPart == null) return;
            xmlTxtWriter.WriteElementString(ET_EYEPOSDISTANCELEFT_MAX, eyeposPart.DistanceLeft_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSDISTANCELEFT_MIN, eyeposPart.DistanceLeft_MIN);
            xmlTxtWriter.WriteElementString(ET_EYEPOSDISTANCERIGHT_MAX, eyeposPart.DistanceRight_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSDISTANCERIGHT_MIN, eyeposPart.DistanceRight_MIN);

            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTX_MAX, eyeposPart.EyePosLeftX_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTX_MIN, eyeposPart.EyePosLeftX_ADCSmm_MIN);
            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTY_MAX, eyeposPart.EyePosLeftY_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTY_MIN, eyeposPart.EyePosLeftY_ADCSmm_MIN);
            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTZ_MAX, eyeposPart.EyePosLeftZ_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSLEFTZ_MIN, eyeposPart.EyePosLeftZ_ADCSmm_MIN);

            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTX_MAX, eyeposPart.EyePosRightX_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTX_MIN, eyeposPart.EyePosRightX_ADCSmm_MIN);
            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTY_MAX, eyeposPart.EyePosRightY_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTY_MIN, eyeposPart.EyePosRightY_ADCSmm_MIN);
            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTZ_MAX, eyeposPart.EyePosRightZ_ADCSmm_MAX);
            xmlTxtWriter.WriteElementString(ET_EYEPOSRIGHTZ_MIN, eyeposPart.EyePosRightZ_ADCSmm_MIN);
		}

        private static void WriteRevisionInfo(XmlTextWriter xmlTextWriter, IRevision rev, string p)
        {
            xmlTextWriter.WriteStartElement("RevisionInfo");
            xmlTextWriter.WriteElementString("RevisionNumber", rev.RevisionNumber.ToString());
            xmlTextWriter.WriteElementString("RevisionPos", p);
            xmlTextWriter.WriteElementString("RevisionType", rev.Type.ToString());
            xmlTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Empty abstract method implementation
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new System.NotImplementedException();
        }

        public void AddRevisions(List<RevisionMatrixEntry> revisions)
        {
            Revisions = revisions;
        }

        private List<RevisionMatrixEntry> Revisions { get; set; }
    }
}