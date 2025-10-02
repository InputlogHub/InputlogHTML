using System.Collections.Generic;
using System.Xml;
using InputLog.Core.Util;
using InputLog.Core.Events.EyeTracking;

namespace InputLog.Core.Analyses.General
{
    /// <summary>
    /// Summary of the General analysis.
    /// Contains a list of events. Each event contains information that was determined during the analysis.
    /// </summary>
    public class GeneralAnalysisSummary : AbstractAnalysisSummary
    {
        /// <summary>
        /// Constructs a new standard GeneralAnalysis Summary.
        /// </summary>
        public GeneralAnalysisSummary()
        {
            Events = new List<GeneralAnalysisEvent>();
        }

		/// <summary>
		/// Create a new Analysis Event.
		/// </summary>
		/// <returns></returns>
		public virtual GeneralAnalysisEvent GetAnalysisEvent()
		{
			return new GeneralAnalysisEvent();
		}

        #region Xml Serialization Infrastructure

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Events");
            foreach (GeneralAnalysisEvent ev in Events)
            {
                writer.WriteStartElement("Event");
                ev.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Events");
            while (reader.IsStartElement("Event"))
            {
                reader.ReadStartElement("Event");
                var ev = new GeneralAnalysisEvent();
                ev.ReadXml(reader);
                Events.Add(ev);
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        #endregion

        /// <summary>
        /// List of the events in this summary.
        /// </summary>
        public List<GeneralAnalysisEvent> Events { private set; get; }

        #region Nested type: GeneralAnalysisEvent

        /// <summary>
        /// An event that was created during the general analysis. Typically this kind event corresponds to an event
        /// logged by the Logging lib, but with additional information added.
        /// </summary>
        public class GeneralAnalysisEvent
        {
			/// <summary>
			/// Specifies whether the GeneralAnalysisEvent has been completed yet or is
			/// waiting for further processing. 
			/// </summary>
			public bool IsCompleted;

            /// <summary>
            /// ActionTime of the event (=EndTime - StartTime);
            /// </summary>
            public ulong? ActionTime;

            /// <summary>
            /// The number of characters actually produced.
            /// Includes deletes and copied text.
            /// </summary>
            public int? CharProduction;

            /// <summary>
            /// Length of the document.
            /// </summary>
            public int? DocLength;

            /// <summary>
            /// EndTime of the event.
            /// </summary>
            public ulong? EndTime;

            /// <summary>
            /// String representation of the id.
            /// </summary>
            public string Id;

            /// <summary>
            /// Output of the event.
            /// </summary>
            public string Output;

            /// <summary>
            /// PauseLocation of the event.
            /// </summary>
            public PauseLocation PauseLocation;

            /// <summary>
            /// PauseTime of the event.
            /// </summary>
            public ulong? PauseTime;

            /// <summary>
            /// Position in the document.
            /// </summary>
            public int? Position;

            /// <summary>
            /// StartTime of the event.
            /// </summary>
            public ulong? StartTime;

            /// <summary>
            /// String representation of the analysisType of the event.
            /// </summary>
            public string Type;

            // Mouse events
            /// <summary>
            /// X coordinate of the mouse.
            /// </summary>
            public int? X;

            /// <summary>
            /// Y coordinate of the mouse.
            /// </summary>
            public int? Y;

			/// <summary>
			/// Eyetrack information. This may be null.
			/// </summary>
			public EyetrackPart Eyetrack;

			/// <summary>
			/// The start offset to be used for interpreting the 'raw' times.
			/// </summary>
			public ulong NewStartOffset;

			private const string ET_PREFIX = "ET_";

            /// <summary>
            /// Current RangeLength in the document. Deprecated.
            /// </summary>
            // public int? RangeLength;

            /// <summary>
            /// Link to an additional resource, f.i. an audio file for dragon
            /// </summary>
            public string Resource;

            /// <summary>
            /// The interval where this event is happening.
            /// </summary>
            public int FixedSizeInterval;
            public int FixedNumberInterval;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="writer"></param>
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("Id", Id);
                writer.WriteElementString("Type", Type);
                writer.WriteElementString("Output", Output);
                writer.WriteElementString("Position", Position.ToString());
                writer.WriteElementString("DocLength", DocLength.ToString());
                writer.WriteElementString("CharProduction", CharProduction.ToString());
                writer.WriteElementString("StartTime", StartTime.ToString());
                writer.WriteElementString("EndTime", EndTime.ToString());
                writer.WriteElementString("ActionTime", ActionTime.ToString());
                writer.WriteElementString("PauseTime", PauseTime.ToString());
                writer.WriteElementString("PauseLocation", ((int) PauseLocation).ToString());
                writer.WriteElementString("FixedSizeInterval", FixedSizeInterval.ToString());
                writer.WriteElementString("FixedNumberInterval", FixedNumberInterval.ToString());
                writer.WriteElementString("X", X.ToString());
                writer.WriteElementString("Y", Y.ToString());


                //  Writer.WriteElementString("RangeLength", RangeLength.ToString());

				if (Eyetrack != null)
				{
					WriteEyetrack(writer);
				}
            }

            private void WriteEyetrack(XmlWriter writer)
			{
				// TODO implement.
			}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            public void ReadXml(XmlReader reader)
            {
                int tmp;
                ulong utmp;
                Id = reader.ReadElementString("Id");
                Type = reader.ReadElementString("Type");
                Output = reader.ReadElementString("Output");
                if (XmlUtils.ReadIntIfPresent(reader, "Position", out tmp)) Position = tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "DocLength", out tmp)) DocLength = tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "CharProduction", out tmp)) CharProduction = tmp;
                if (XmlUtils.ReadUlongIfPresent(reader, "StartTime", out utmp)) StartTime = utmp;
                if (XmlUtils.ReadUlongIfPresent(reader, "EndTime", out utmp)) EndTime = utmp;
                if (XmlUtils.ReadUlongIfPresent(reader, "ActionTime", out utmp)) ActionTime = utmp;
                if (XmlUtils.ReadUlongIfPresent(reader, "PauseTime", out utmp)) PauseTime = utmp;
                if (XmlUtils.ReadIntIfPresent(reader, "PauseLocation", out tmp)) PauseLocation = (PauseLocation) tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "FixedNumberInterval", out tmp)) FixedNumberInterval = tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "FixedSizeInterval", out tmp)) FixedSizeInterval = tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "X", out tmp)) X = tmp;
                if (XmlUtils.ReadIntIfPresent(reader, "Y", out tmp)) Y = tmp;

                // if (XmlUtils.ReadIntIfPresent(Reader, "RangeLength", out tmp)) this.RangeLength = tmp;
            }

            public ulong RawStartTime { get; set; }

            public ulong RawEndTime { get; set; }

        }

        #endregion
    }
}