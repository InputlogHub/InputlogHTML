
using System;
using System.Collections.Generic;
using System.Xml;
using InputLog.Core.Events;
using InputLog.Core.Util.KeyConversion;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.General
{
    /// <summary>
    /// Serializable description for a General Analysis.
    /// </summary>
    public class GeneralAnalysisDescription : AnalysisDescription
    {
        protected readonly List<KeysEx> ControlKeys;
		protected readonly string Abbreviation;
        private ulong IntervalSize;
        private int NumberOfIntervals;

        public GeneralAnalysisDescription(): this("GA"){}

        protected GeneralAnalysisDescription(string abbreviation)
        {
            Abbreviation = abbreviation;
            ControlKeys = new List<KeysEx>();
            NewStartOffset = 0ul;
        }

		protected GeneralAnalysisDescription(List<Event> events, SessionIdentification sessionID, string abbreviation) :
			base(events, sessionID, abbreviation) 
		{
			Abbreviation = abbreviation;
		}

        protected GeneralAnalysisDescription(List<Event> events, SessionIdentification sessionID, string abbreviation, 
            List<KeysEx> controlKeys = null)
            : base(events, sessionID, abbreviation)
        {
            Abbreviation = abbreviation;
            ControlKeys = controlKeys;
        }

        public override String GetName()
        {
            return "General Analysis";
        }

        protected override void WriteCustomXml(XmlWriter writer)
        {
            writer.WriteStartElement("ControlKeys");
			if (ControlKeys != null)
			{
				foreach (KeysEx key in ControlKeys)
				{
					writer.WriteElementString("Key", ((int)key).ToString());
				}
			}
            writer.WriteEndElement();
            writer.WriteElementString("StartOffset", NewStartOffset.ToString());
        }

        protected override void ReadCustomXml(XmlReader reader)
        {
            bool end = !reader.IsEmptyElement;
            reader.ReadStartElement("ControlKeys");
            while (reader.IsStartElement("Key"))
            {
                ControlKeys.Add((KeysEx) Int32.Parse(reader.ReadElementString("Key")));
            }
            if (end) reader.ReadEndElement();
            NewStartOffset = ulong.Parse(reader.ReadElementString("StartOffset"));
        }

        public override Analysis GetAnalysis()
        {
            return new GeneralAnalysis(Events, SessionID, Abbreviation, NumberOfIntervals, IntervalSize, ControlKeys);
        }
    }
}