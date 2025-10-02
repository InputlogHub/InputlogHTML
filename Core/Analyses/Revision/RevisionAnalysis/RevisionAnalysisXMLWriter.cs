using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.IO;
using InputLog.Core.Util;
using log4net;

namespace InputLog.Core.Analyses.Revision.RevisionAnalysis
{
    public class RevisionAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants
        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "revision_analysis.xslt";

        private const string REVISION_TAG = "revision";
        private const string TYPE_TAG = "type";
        private const string EVENT_TAG = "event";
        private const string START_POS_TAG = "start";
        private const string END_POS_TAG = "end";
       // private const string LENGTH_TAG = "length";
        private const string BEFORE_TAG = "usebefore";
        private const string INCLUDE_REPLAY_TAG = "includeInReplay";
        private const string KEY_TAG = "key";
        private const string KEYBOARDSTATE_TAG = "keyboardstate";


        private const string OUTPUT_TAG = "output";
        private const string POSITION_TAG = "position";
        //private const string DOCLENGTH_TAG = "doclength";
        //private const string START_TIME_TAG = "startTime";
        //private const string START_CLOCK_TAG = "startClock";
        //private const string END_TIME_TAG = "endTime";
        //private const string END_CLOCK_TAG = "endClock";
        //private const string ACTION_TIME_TAG = "actionTime";
        //private const string PAUSE_TIME_TAG = "pauseTime";
        //private const string PAUSE_LOCATION_TAG = "pauseLocation";
        //private const string RANGE_LENGTH_TAG = "rangeLength";

        //private const string SEQUENCE_TAG = "sequence";
        private const string ID_TAG = "id";
        //private const string EVENT_LOCATION = "eventLocation";

        //public const string PAUSE_THRESHOLD_PARAMETER = "Pause Threshold (ms)";
        //public const string TYPE_PARAMETER = "Revision Analysis Type";
        #endregion

        #region Fields
        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// Constructs a RevisionAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public RevisionAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath) { }

        /// <summary>
        /// Writes out the analysis document using a given RevisionAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which
        ///  the General Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is RevisionAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type RevisionAnalysisSummary");
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteRevisions((RevisionAnalysisSummary)summary);
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });
        }

        /// <summary>
        /// Writes out the RevisionAnalysisEvents.
        /// </summary>
        /// <param name="summary">List of RevisionAnalysisEvent's to write to the analysis document.s</param>
        private void WriteRevisions(RevisionAnalysisSummary summary)
        {
            foreach (var rev in summary.Revisions)
            {
                XMLWriter.WriteStartElement(REVISION_TAG);
                XMLWriter.WriteAttributeString(TYPE_TAG, rev.Type.ToString());
                XMLWriter.WriteAttributeString(ID_TAG, rev.RevisionNumber.ToString());

                foreach (var edit in rev.Edits)
                {
                    XMLWriter.WriteStartElement(EVENT_TAG);
                    XMLWriter.WriteAttributeString(TYPE_TAG, edit.Type.ToString());
                    switch (edit.Type)
                    {
                        case EditType.Deletion:
                            ProcessDelete(edit);
                            break;
                        case EditType.Insertion:
                            ProcessInsertion(edit);
                            break;
                        case EditType.TypeChar:
                            ProcessTypeChar(edit);
                            break;
                        default:
                            Log.Warn(string.Format("Edit type not recognized: {0}", edit.Type.ToString()));
                            break;
                    }
                    XMLWriter.WriteEndElement();
                }

                XMLWriter.WriteEndElement();
                //XMLWriter.WriteStartElement(EVENT_TAG);
                //WriteIfValueNotNull(TYPE_TAG, outputEvent.Type);
                //WriteIfValueNotNull(ID_TAG, outputEvent.Id);
                //WriteIfValueNotNull(SEQUENCE_TAG, outputEvent.Sequence);
                //WriteIfValueNotNull(EVENT_LOCATION, outputEvent.EventLocation);
                //WriteIfValueNotNull(OUTPUT_TAG, outputEvent.Output);
                //WriteIfValueNotNull(POSITION_TAG, outputEvent.Position);
                //WriteIfValueNotNull(DOCLENGTH_TAG, outputEvent.DocLength);
                //WriteIfValueNotNull(START_TIME_TAG, outputEvent.StartTime);

                //// determine and write out the start- and endclock using the start- and endtime.
                //if (outputEvent.StartTime.HasValue)
                //{
                //    WriteIfValueNotNull(START_CLOCK_TAG, MsecToClockString(outputEvent.StartTime.Value));
                //}
                //WriteIfValueNotNull(END_TIME_TAG, outputEvent.EndTime);

                //if (outputEvent.EndTime.HasValue)
                //{
                //    WriteIfValueNotNull(END_CLOCK_TAG, MsecToClockString(outputEvent.EndTime.Value));
                //}
                //WriteIfValueNotNull(ACTION_TIME_TAG, outputEvent.ActionTime);
                //WriteIfValueNotNull(PAUSE_TIME_TAG, outputEvent.PauseTime, "Unable to set a pausetime");

                //if (outputEvent.PauseLocation != PauseLocation.UNDETERMINED)
                //{
                //    WriteIfValueNotNull(PAUSE_LOCATION_TAG, ((int)outputEvent.PauseLocation).ToString());
                //}
                //WriteIfValueNotNull(RANGE_LENGTH_TAG, outputEvent.RangeLength);
                //XMLWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Processes the given edit and writes the result using the XMLWriter.
        /// </summary>
        /// <param name="edit">The edit to process.</param>
        private void ProcessDelete(IEdit edit)
        {
            var deletion = edit as Deletion;
            if (deletion != null)
            {
                var del = deletion;
                XMLWriter.WriteElementString(START_POS_TAG, del.StartPos.ToString());
                XMLWriter.WriteElementString(END_POS_TAG, del.EndPos.ToString());
            }
            else
            {
                Log.Warn("The given edit is not of type Deletion, skipped it");
            }
        }

        /// <summary>
        /// Processes the given edit and writes the result using the XMLWriter.
        /// </summary>
        /// <param name="edit">The edit to process.</param>
        private void ProcessInsertion(IEdit edit)
        {
            var insertion = edit as Insertion;
            if (insertion != null)
            {
                var ins = insertion;
                XMLWriter.WriteElementString(START_POS_TAG, ins.InsertPosition.ToString());
				XMLWriter.WriteElementString(BEFORE_TAG, StringUtils.ConvertToReadable((!ins.UseAfter).ToString()));
                XMLWriter.WriteElementString(OUTPUT_TAG, ins.Text);
            }
            else
            {
                Log.Warn("The given edit is not of type Insertion, skipped it");
            }
        }

        ///// <summary>
        ///// Processes the given edit and writes the result using the XMLWriter.
        ///// </summary>
        ///// <param name="edit">The edit to process.</param>
        //private void ProcessSelectionChange(IEdit edit)
        //{
        //    var change = edit as SelectionChange;
        //    if (change != null)
        //    {
        //        var sel = change;
        //        XMLWriter.WriteElementString(START_POS_TAG, sel.StartPos.ToString());
        //        XMLWriter.WriteElementString(END_POS_TAG, sel.EndPos.ToString());
        //    }
        //    else
        //    {
        //        log.Warn("The given edit is not of type SelectionChange, skipped it");
        //    }
        //}

        /// <summary>
        /// Processes the given edit and writes the result using the XMLWriter.
        /// </summary>
        /// <param name="edit">The edit to process.</param>
        private void ProcessTypeChar(IEdit edit)
        {
            var c = edit as TypeChar;
            if (c != null)
            {
                var typ = c;
                XMLWriter.WriteElementString(KEY_TAG, typ.WinKey.Key.ToString());
                XMLWriter.WriteStartElement(KEYBOARDSTATE_TAG);
                foreach (var key in typ.WinKey.KeyboardState)
                {
                    XMLWriter.WriteElementString(KEY_TAG, key.ToString());
                }
                XMLWriter.WriteEndElement();
                XMLWriter.WriteElementString(OUTPUT_TAG, typ.WinKey.Value);
                XMLWriter.WriteElementString(INCLUDE_REPLAY_TAG, typ.WordKey.IncludeInReplay.ToString());
                XMLWriter.WriteElementString(POSITION_TAG, typ.WordKey.Position.ToString());
            }
            else
            {
                Log.Warn("The given edit is not of type TypeChar, skipped it");
            }
        }

        /// <summary>
        /// Empty abstract method implementation
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }

    }
}