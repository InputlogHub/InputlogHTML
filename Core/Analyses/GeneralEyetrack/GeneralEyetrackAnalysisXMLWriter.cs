using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using InputLog.Core.Util;
using InputLog.Core.Analyses.General;
using InputLog.Core.Util.Xml;

namespace InputLog.Core.Analyses.GeneralEyetrack
{
    // ReSharper disable UnusedMember.Global
	public class GeneralEyetrackAnalysisXMLWriter: GeneralAnalysisXMLWriter
    // ReSharper restore UnusedMember.Global
	{
		// Location to the stylesheet.
		private const string STYLESHEET_HREF_LOC = "general_eyetrack_analysis.xsl";
        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

		///////////////////////////////////////////////////////////////////////
		// Extra variables for the general eyetrack analysis summary
		///////////////////////////////////////////////////////////////////////
		//

		private const string ET_GAZEEVENTNROF_PERTYPE = "gazeEventType_nrOf";

		// maxDistanceX/Y - distanceX/Y - max/min/avg
		private const string ET_MAX_MAXDISTANCEX = "maxMaxDistanceX";
		private const string ET_MIN_MAXDISTANCEX = "minMaxDistanceX";
		private const string ET_AVG_MAXDISTANCEX = "avgMaxDistanceX";
		private const string ET_MAX_MAXDISTANCEY = "maxMaxDistanceY";
		private const string ET_MIN_MAXDISTANCEY = "minMaxDistanceY";
		private const string ET_AVG_MAXDISTANCEY = "avgMaxDistanceY";
		private const string ET_MAX_DISTANCEX = "maxDistanceX";
		private const string ET_MIN_DISTANCEX = "minDistanceX";
		private const string ET_AVG_DISTANCEX = "avgDistanceX";
		private const string ET_MAX_DISTANCEY = "maxDistanceY";
		private const string ET_MIN_DISTANCEY = "minDistanceY";
		private const string ET_AVG_DISTANCEY = "avgDistanceY";

		// start/end GazePointX/Y - Min/Max
		private const string ET_MINSTARTGAZEPOINTX = "minStartGazePointX";
		private const string ET_MAXSTARTGAZEPOINTX = "maxStartGazePointX";
		private const string ET_MINSTARTGAZEPOINTY = "minStartGazePointY";
		private const string ET_MAXSTARTGAZEPOINTY = "maxStartGazePointY";
		private const string ET_MINENDGAZEPOINTX = "minEndGazePointX";
		private const string ET_MAXENDGAZEPOINTX = "maxEndGazePointX";
		private const string ET_MINENDGAZEPOINTY = "minEndGazePointY";
		private const string ET_MAXENDGAZEPOINTY = "maxEndGazePointY";

		//
		///////////////////////////////////////////////////////////////////////

		public GeneralEyetrackAnalysisXMLWriter(string destinationFilePath):
			base(destinationFilePath, STYLESHEET_HREF_LOC)
		{
			IncludeEyetracking = true;
		}

		protected override void WriteEvents(IEnumerable<GeneralAnalysisSummary.GeneralAnalysisEvent> events)
		{
            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 3;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";

			var aoiNames = new List<string>();
			var includeAOIs = false;
			var numberOfAOIs = 0;
		    var generalAnalysisEvents = events as IList<GeneralAnalysisSummary.GeneralAnalysisEvent> ?? events.ToList();
		    
            foreach (Dictionary<string, string> aois 
                in generalAnalysisEvents.OfType<GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent>().
                Select(output => output.Get_AOIs()))
		    {
		        if (aois.Count > 0)
		        {
		            includeAOIs = true;
		            numberOfAOIs = aois.Count;
		            aoiNames.AddRange(aois.Keys);
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

			base.WriteEvents(generalAnalysisEvents);
		}

		protected override void WriteEyetracking(XmlTextWriter xmlTxtWriter, 
            GeneralAnalysisSummary.GeneralAnalysisEvent pOutputEvent)
		{
		    var outputEvent = pOutputEvent as GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent;
		    
            if (outputEvent == null)
		    {
		        throw new System.ArgumentException("Expected an outputEvent of type GeneralEyetrackAnalysisEvent");
		    }

		    for (var indexType = 0; 
                indexType < GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent.NR_OF_TYPES;
                indexType++)
		    {
		        // Get the prefix for the current type.
		        var prefix = GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent.PREFIX[indexType];
		        var type = GeneralEyetrackAnalysisSummary.GeneralEyetrackAnalysisEvent.INDEX_TO_TYPE(indexType);

		        // Write gazeTypes & Events
		        xmlTxtWriter.WriteElementString(prefix + ET_GAZEEVENTNROF_PERTYPE, 
                    outputEvent.Get_GazeEventTypeNumberOf(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_GAZEEVENTDURATION, 
                    outputEvent.Get_GazeEventDuration(type).ToString());
		        
                switch (type)
		        {
		            case "Fixation":
		                xmlTxtWriter.WriteElementString(ET_FIXATIONINDEX, outputEvent.Get_FixationIndices());
		                break;
		            case "Saccade":
		                xmlTxtWriter.WriteElementString(ET_SACCADEINDEX, outputEvent.Get_SaccadeIndices());
		                break;
		        }

		        // Print averages
		        var averageGazePointXAdcSpx = outputEvent.Get_AverageGazePointX_ADCSpx(type);
		        if (averageGazePointXAdcSpx != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEGAZEPOINTX,
                        averageGazePointXAdcSpx.Value.ToString("F", Nfi));
		        var averageGazePointYAdcSpx = outputEvent.Get_AverageGazePointY_ADCSpx(type);
		        if (averageGazePointYAdcSpx != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEGAZEPOINTY,
                        averageGazePointYAdcSpx.Value.ToString("F", Nfi));
		        var averagePupilLeft = outputEvent.Get_AveragePupilLeft(type);
		        if (averagePupilLeft != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEPUPILLEFT,
                        averagePupilLeft.Value.ToString("F", Nfi));
		        var averagePupilRight = outputEvent.Get_AveragePupilRight(type);
		        if (averagePupilRight != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEPUPILRIGHT,
                        averagePupilRight.Value.ToString("F", Nfi));
		        var averageValidityLeft = outputEvent.Get_AverageValidityLeft(type);
		        if (averageValidityLeft != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEVALIDITYLEFT,
                        averageValidityLeft.Value.ToString("F", Nfi));
		        var averageValidityRight = outputEvent.Get_AverageValidityRight(type);
		        if (averageValidityRight != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVERAGEVALIDITYRIGHT,
                        averageValidityRight.Value.ToString("F", Nfi));

		        // Offscreen Time & Samples
		        xmlTxtWriter.WriteElementString(prefix + ET_OFFSCREENTIME,
                    outputEvent.Get_OffscreenTime(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_NUMBEROFSAMPLES,
                    outputEvent.Get_NrOfSamples(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_NUMBEROFVALIDSAMPLES,
                    outputEvent.Get_NrOfValidSamples(type).ToString());

		        // GazePoint - X/Y - ADCSpx/MCSpx - Min/Max
		        xmlTxtWriter.WriteElementString(prefix + ET_MINGAZEPOINTX_ADCSPX,
                    outputEvent.Get_MinGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINGAZEPOINTX_MCSPX,
                    outputEvent.Get_MinGazePointX_MCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINGAZEPOINTY_ADCSPX,
                    outputEvent.Get_MinGazePointY_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINGAZEPOINTY_MCSPX,
                    outputEvent.Get_MinGazePointY_MCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXGAZEPOINTX_ADCSPX,
                    outputEvent.Get_MaxGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXGAZEPOINTX_MCSPX,
                    outputEvent.Get_MaxGazePointX_MCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXGAZEPOINTY_ADCSPX,
                    outputEvent.Get_MaxGazePointY_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXGAZEPOINTY_MCSPX,
                    outputEvent.Get_MaxGazePointY_MCSpx(type).ToString());

		        // MaxDistance/Distance - X/Y - Min/Max/Avg
		        xmlTxtWriter.WriteElementString(prefix + ET_MAX_MAXDISTANCEX,
                    outputEvent.Get_MaxMaxDistanceX(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MIN_MAXDISTANCEX,
                    outputEvent.Get_MinMaxDistanceX(type).ToString());
		        var avgMaxDistanceX = outputEvent.Get_AvgMaxDistanceX(type);
		        if (avgMaxDistanceX != null)
		            xmlTxtWriter.WriteElementString(prefix + ET_AVG_MAXDISTANCEX,
                        avgMaxDistanceX.HasValue ? avgMaxDistanceX.Value.ToString("F", Nfi) : "");
		        xmlTxtWriter.WriteElementString(prefix + ET_MAX_MAXDISTANCEY,
                    outputEvent.Get_MaxMaxDistanceY(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MIN_MAXDISTANCEY,
                    outputEvent.Get_MinMaxDistanceY(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_AVG_MAXDISTANCEY,
                    outputEvent.Get_AvgMaxDistanceY(type).HasValue
                    ? outputEvent.Get_AvgMaxDistanceY(type).Value.ToString("F", Nfi) : "");
		        xmlTxtWriter.WriteElementString(prefix + ET_MAX_DISTANCEX,
                    outputEvent.Get_MaxDistanceX(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MIN_DISTANCEX,
                    outputEvent.Get_MinDistanceX(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_AVG_DISTANCEX,
                    outputEvent.Get_AvgDistanceX(type).HasValue 
                    ? outputEvent.Get_AvgDistanceX(type).Value.ToString("F", Nfi) : "");
		        xmlTxtWriter.WriteElementString(prefix + ET_MAX_DISTANCEY,
                    outputEvent.Get_MaxDistanceY(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MIN_DISTANCEY,
                    outputEvent.Get_MinDistanceY(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_AVG_DISTANCEY,
                    outputEvent.Get_AvgDistanceY(type).HasValue 
                    ? outputEvent.Get_AvgDistanceY(type).Value.ToString("F", Nfi) : "");

		        // Start/End - GazePointX/Y - Min/Max
		        xmlTxtWriter.WriteElementString(prefix + ET_MINSTARTGAZEPOINTX,
                    outputEvent.Get_MinStartGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINSTARTGAZEPOINTY,
                    outputEvent.Get_MinStartGazePointY_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXSTARTGAZEPOINTX,
                    outputEvent.Get_MaxStartGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXSTARTGAZEPOINTY,
                    outputEvent.Get_MaxStartGazePointY_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINENDGAZEPOINTX,
                    outputEvent.Get_MinEndGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MINENDGAZEPOINTY,
                    outputEvent.Get_MinEndGazePointY_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXENDGAZEPOINTX,
                    outputEvent.Get_MaxEndGazePointX_ADCSpx(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MAXENDGAZEPOINTY,
                    outputEvent.Get_MaxEndGazePointY_ADCSpx(type).ToString());

		        // CumAbsDistances - X/X_LEFT/X_RIGHT/Y/Y_UP/Y_DOWN
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEX,
                    outputEvent.Get_CumAbsDistanceX(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEX_LEFT,
                    outputEvent.Get_CumAbsDistanceX_Left(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEX_RIGHT,
                    outputEvent.Get_CumAbsDistanceX_Right(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEY,
                    outputEvent.Get_CumAbsDistanceY(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEY_DOWN,
                    outputEvent.Get_CumAbsDistanceY_Down(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_CUMABSDISTANCEY_UP,
                    outputEvent.Get_CumAbsDistanceY_Up(type).ToString());

		        // Eyepos distances - Left/Right - Min/Max
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSDISTANCELEFT_MAX,
                    outputEvent.Get_DistanceLeft_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSDISTANCELEFT_MIN,
                    outputEvent.Get_DistanceLeft_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSDISTANCERIGHT_MAX,
                    outputEvent.Get_DistanceRight_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSDISTANCERIGHT_MIN,
                    outputEvent.Get_DistanceRight_Min(type).ToString());

		        // Eyepos Left/Right - X/Y/Z - Min/Max
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTX_MAX,
                    outputEvent.Get_EyePosLeftX_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTX_MIN,
                    outputEvent.Get_EyePosLeftX_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTY_MAX,
                    outputEvent.Get_EyePosLeftY_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTY_MIN,
                    outputEvent.Get_EyePosLeftY_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTZ_MAX,
                    outputEvent.Get_EyePosLeftZ_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSLEFTZ_MIN,
                    outputEvent.Get_EyePosLeftZ_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTX_MAX,
                    outputEvent.Get_EyePosRightX_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTX_MIN,
                    outputEvent.Get_EyePosRightX_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTY_MAX,
                    outputEvent.Get_EyePosRightY_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTY_MIN,
                    outputEvent.Get_EyePosRightY_Min(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTZ_MAX,
                    outputEvent.Get_EyePosRightZ_Max(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_EYEPOSRIGHTZ_MIN,
                    outputEvent.Get_EyePosRightZ_Min(type).ToString());

		        // Mouse
		        xmlTxtWriter.WriteElementString(prefix + ET_MOUSEEVENTNUMBER,
                    outputEvent.Get_MouseNumberOfEvents(type).ToString());
		        xmlTxtWriter.WriteElementString(prefix + ET_MOUSEEVENT,
                    outputEvent.Get_MouseEvents(type));

		        // Keyboard
		        xmlTxtWriter.WriteElementString(prefix + ET_KEYBOARDEVENT,
                    outputEvent.Get_KeyboardEvents(type));
		        xmlTxtWriter.WriteElementString(prefix + ET_KEYBOARDEVENTNUMBER,
                    outputEvent.Get_KeyboardNumberOfEvents(type).ToString());

		        // Studio
		        xmlTxtWriter.WriteElementString(prefix + ET_STUDIOEVENT,
                    outputEvent.Get_StudioEvents(type));
		        xmlTxtWriter.WriteElementString(prefix + ET_STUDIOEVENTVALUE,
                    outputEvent.Get_StudioEventValues(type));

		        // External
		        xmlTxtWriter.WriteElementString(prefix + ET_EXTERNALEVENT,
                    outputEvent.Get_ExternalEvents(type));
		        xmlTxtWriter.WriteElementString(prefix + ET_EXTERNALEVENTVALUE,
                    outputEvent.Get_ExternalEventValues(type));
		    }

		    // Write AOI information once (Not per type)
		    Dictionary<string, string> aois = outputEvent.Get_AOIs();


		    // IMPORTANT:
		    // The values are written in the same order as the headers of the
		    // general table (containing the AOI corresponding names for the hits) have
		    // been printed. Because this order is consistent, the same correct value should be 
		    // printed at their correct aoiHit
		    // If the order is changed, either here, or in the printing of the titles, the order
		    // must be changed consistently, so that the right AOIHits are printed in the right
		    // columns
		    // NOTE THAT THE HEADERS ARE WRITTEN IN THE BASECLASSES' WRITE-EVENTS METHOD.

		    xmlTxtWriter.WriteStartElement(ET_AOIHITS);
		    foreach (string aoiHit in aois.Values)
		    {
		        xmlTxtWriter.WriteElementString(ET_AOIHIT, aoiHit);
		    }
		    xmlTxtWriter.WriteEndElement();
		}
	}
}
