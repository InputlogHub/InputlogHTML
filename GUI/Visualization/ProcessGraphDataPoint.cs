using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InputLog.Core.Analyses.General;
using InputLog.Core.Util;

namespace GUI.Visualization
{
    /// <summary>
    /// Contains data from events in a General Analysis to be used in a chart.
    /// </summary>
    public class ProcessGraphDataPoint
    {
        #region Fields

        public int StartTime;
        public int DocLength;
        public int Position;
        public int Characters;
        public int PauseTime;
        public string Focus;
        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ProcessGraphDataPoint rhs)) return false;
            return StartTime == rhs.StartTime
                   && DocLength == rhs.DocLength
                   && Position == rhs.Position
                   && Characters == rhs.Characters
                   && PauseTime == rhs.PauseTime
                   && (Focus == null && rhs.Focus == null
                       || Focus != null && Focus.Equals(rhs.Focus));
        }

        /// <summary>
        /// Reads a list of VisualizationDataPoints from a General Analysis.
        /// </summary>
        /// <returns></returns>
        public static List<ProcessGraphDataPoint> ReadFromGeneralAnalysisSummary(GeneralAnalysisSummary summ, string thisMainDoc)
        {
            var points = new List<ProcessGraphDataPoint>();
            var lastPosition = 0;
            var lastDocLength = 0;
            var lastStartTime = 0UL;
            var prevFocus = "Focus Unknown";
            int separator = thisMainDoc.IndexOf(".", StringComparison.Ordinal);
            var mainDoc = thisMainDoc;
            if (separator != -1)
            {
                mainDoc = thisMainDoc.Substring(0, separator - 1).ToLower();
            }
            GeneralAnalysisSummary.GeneralAnalysisEvent placeHolderEvent = null;
            //  GeneralAnalysisSummary.GeneralAnalysisEvent prevEvent = null;

            foreach (var outputEvent in summ.Events)
            {
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

                var point = new ProcessGraphDataPoint();

                if (outputEvent.Position.HasValue)
                {
                    lastPosition = (int)outputEvent.Position;
                }
                point.Position = lastPosition;

                if (outputEvent.DocLength.HasValue)
                {
                    lastDocLength = (int)outputEvent.DocLength;
                }
                point.DocLength = lastDocLength;

                // Total number of characters produced so far.
                if (outputEvent.CharProduction != null) point.Characters = (int)outputEvent.CharProduction;

                // Determine and write out the start- and endClock using the Start- and EndTime.
                if (outputEvent.StartTime.HasValue && outputEvent.StartTime > 0)
                {
                    lastStartTime = (ulong)outputEvent.StartTime;
                }
                point.StartTime = (int) Convert.ToInt64(lastStartTime);

                try
                {
                    if (outputEvent.PauseTime != null) point.PauseTime = (int) Convert.ToInt64(outputEvent.PauseTime);
                }
                catch (Exception)
                {
                    point.PauseTime = 0;
                }

                if (outputEvent.Type != null)
                {
                    if (outputEvent.Type.Equals("focus"))
                    {
                        var output = StringUtils.ReplaceNonPrintableCharacters(outputEvent.Output);
                        if (null != output)
                        {
                            point.Focus = output.ToLower().Contains(mainDoc) ? "wordlog" : output;
                            if (point.Focus.Equals(string.Empty))
                            {
                                point.Focus = "Focus Unknown";
                            }
                        }
                    }
                    else
                    {
                        point.Focus = prevFocus;
                    } 
                }
                prevFocus = point.Focus;
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// If all fields are mutable and we have to override a GetHashCode method, 
        /// this is the implementation we need to have.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1;
        }
    }

}