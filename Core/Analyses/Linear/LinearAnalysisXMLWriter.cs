using System.Collections.Generic;
using System.Text;
using System.Xml;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Linear
{
    /// <summary>
    /// Analysis XMLWriter for the Linear Analysis.
    /// </summary>
    public class LinearAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Fields
        // Adds a version of the Summary Analysis without pauzes, without dots, and with hash tags ('#') 
        // for repetitive deletes and backspaces. This version can be copied directly into Excel 
        // for further analysis.  
        public static bool IsSpecialLinear { private get; set; }

        #endregion

        #region Constants

        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "linear_analysis.xsl";
        private const string PERIOD_TAG = "Period";
        private const string PERIOD_EVENT_TAG = "PeriodEvent";

        // Period/Event attributes
        private const string PERIOD_ID_ATTRIBUTE = "period_id";
        private const string PERIOD_TIME_ATTRIBUTE = "periodTime";
        private const string LINK_ATTRIBUTE = "link";

        public const string PAUSE_THRESHOLD_PARAMETER = "Pause Threshold (ms)";
        public const string TYPE_PARAMETER = "Linear Analysis Type";

        public const string SPACE = "SPACE";
        public const string DELETE = "DELETE";
        public const string RIGHT = "RIGHT";
        public const string BACK = "BACK";
        public const string LEFT = "LEFT";
        
        #endregion

        /// <summary>
        /// Constructs a LinearAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public LinearAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
        }

        /// <summary>
        /// Writes out the analysis document using a given LinearAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which 
        /// the Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string,
            IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is LinearAnalysisSummary)) throw new AnalysisWriterException("Given summary is not of type" +
                " LinearAnalysisSummary");
            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary((LinearAnalysisSummary)summary);
            if(IsSpecialLinear)
            {
                AddSpecialSummary((LinearAnalysisSummary)summary);
            }
            XMLWriter.WriteEndElement();
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_CSS, COMMON_XSL }, new[] { INPUTLOG_LOGO });
        }

        /// <summary>
        /// Writes a periodEvent to the xmlstream if value!=null.
        /// </summary>
        /// <param name="value">value of the periodEvent</param>
        private void WritePeriodEvent(object value)
        {
            if (value != null)
            {
                XMLWriter.WriteStartElement(PERIOD_EVENT_TAG);
                XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, value.ToString());
                XMLWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes a special period event to the xmlstream.
        /// Special events are wrapped in square brackets [] and include the count of the event.
        /// if value.ToString()=="[SPACE]", then &#8203;&#183; is <code>count</code>
        ///  times written to the stream (round dot).
        /// e.g.: [RETURN 2] represents 2 consecutive returns.
        /// When the count smaller/equal to 1, it is not written out: e.g.: [ENTER]
        /// </summary>
        /// <param name="value">value of the event</param>
        /// <param name="count">count of the event</param>
        private void WriteSpecialPeriodEvent(string value, ulong count)
        {
            switch (value)
            {
                case SPACE:
                    for (ulong i = 0; i < count; i++)
                    {
                        XMLWriter.WriteStartElement(PERIOD_EVENT_TAG);
                        XMLWriter.WriteStartAttribute(VALUE_ATTRIBUTE);
                        XMLWriter.WriteRaw("&#8203;&#183;");
                        XMLWriter.WriteEndAttribute();
                        XMLWriter.WriteEndElement();
                    }
                    break;
                default:
                    if (count <= 1)
                    {
                        WritePeriodEvent("[" + value + "]");
                    }
                    else
                    {
                        WritePeriodEvent("[" + value + " " + count + "]");
                    }
                    break;
            }

        }

        /// <summary>
        /// Writes a pause event to the xmlstream.
        /// Pause events are wrapped in curly brackets {}.
        /// e.g.: {2045} represents a pause of 2045 msec.
        /// </summary>
        /// <param name="pauseTime">pauseTime to write to the xmlstream</param>
        private void WritePauseEvent(ulong pauseTime)
        {
            WritePeriodEvent("{" + pauseTime + "}");
        }

        /// <summary>
        /// Writes the summary to the stream.
        /// Loops over all the periods in the stream, writes out the start and periodtime for each of them.
        /// Then loops over all the events within a period and writes out the events (based on there type, 
        /// they are represented differently).
        /// </summary>
        /// <param name="summary">The analysis summary</param>
        private void WriteSummary(LinearAnalysisSummary summary)
        {
            foreach (var period in summary.Periods)
            {
                XMLWriter.WriteStartElement(PERIOD_TAG);
                XMLWriter.WriteAttributeString(PERIOD_ID_ATTRIBUTE, period.ID);
                XMLWriter.WriteAttributeString(PERIOD_TIME_ATTRIBUTE, period.PeriodStartTime.ToString());
                XMLWriter.WriteAttributeString(LINK_ATTRIBUTE, period.Link);
                foreach (var periodEvent in period)
                {
                    if (periodEvent is LinearAnalysisSummary.RegularEvent)
                    {
                        WritePeriodEvent(((LinearAnalysisSummary.RegularEvent)periodEvent).Value);
                    }
                    else if (periodEvent is LinearAnalysisSummary.ReplaceEvent)
                    {
                        WritePeriodEvent(((LinearAnalysisSummary.ReplaceEvent)periodEvent).Value);
                    }
                    else if (periodEvent is LinearAnalysisSummary.SpecialEvent)
                    {
                        var specialPeriodEvent = (LinearAnalysisSummary.SpecialEvent)periodEvent;
                        WriteSpecialPeriodEvent(specialPeriodEvent.Value, specialPeriodEvent.Count);
                    }
                    else if (periodEvent is LinearAnalysisSummary.PauseEvent)
                    {
                        WritePauseEvent(((LinearAnalysisSummary.PauseEvent)periodEvent).PauseTime);
                    }
                }
                XMLWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds a special version of the Linear Analysis to the report without pauzes, 
        /// without dots, and with hash tags ('#') for repetitive deletes and backspaces. 
        /// This version can be copied directly into Excel for further analysis.
        /// </summary>
        /// <param name="summary"></param>
        private void AddSpecialSummary(LinearAnalysisSummary summary)
        {
            XMLWriter.WriteStartElement(PERIOD_TAG);
            XMLWriter.WriteAttributeString(PERIOD_ID_ATTRIBUTE, "*CONDENSED*");
            XMLWriter.WriteEndElement();

            foreach (var period in summary.Periods)
            {
                XMLWriter.WriteStartElement(PERIOD_TAG);
                XMLWriter.WriteAttributeString(PERIOD_ID_ATTRIBUTE, period.ID);
                XMLWriter.WriteAttributeString(PERIOD_TIME_ATTRIBUTE, period.PeriodStartTime.ToString());
                XMLWriter.WriteAttributeString(LINK_ATTRIBUTE, period.Link);
                foreach (var periodEvent in period)
                {
                    if (periodEvent is LinearAnalysisSummary.RegularEvent)
                    {
                        WritePeriodEvent(((LinearAnalysisSummary.RegularEvent)periodEvent).Value);
                    }
                    else if (periodEvent is LinearAnalysisSummary.SpecialEvent)
                    {
                        var specialPeriodEvent = (LinearAnalysisSummary.SpecialEvent)periodEvent;
                        WriteCondensedPeriodEvent(specialPeriodEvent.Value, specialPeriodEvent.Count);
                    }
                }
                XMLWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Writing a period event without dots for spaces and without pause time
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        private void WriteCondensedPeriodEvent(string value, ulong count)
        {
            switch (value)
            {
                case SPACE:
                    for (ulong i = 0; i < count; i++)
                    {
                        XMLWriter.WriteStartElement(PERIOD_EVENT_TAG);
                        XMLWriter.WriteStartAttribute(VALUE_ATTRIBUTE);
                        XMLWriter.WriteRaw(" ");
                        XMLWriter.WriteEndAttribute();
                        XMLWriter.WriteEndElement();
                    }
                    break;

                case DELETE: case BACK: case RIGHT: case LEFT: 
                    if (count <= 1)
                    {
                        WritePeriodEvent("#");
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for(int i = 0; i < (int) count; i++)
                        {
                            sb.Append("#");
                        }
                        WritePeriodEvent(sb);
                    }
                    break;
            }

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
    }
}
