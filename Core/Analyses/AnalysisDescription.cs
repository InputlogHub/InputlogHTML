using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO.Xml.Output;
using System.Text;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Base class for a serializable analysis description, to be executed on the server.
    /// </summary>
    public class AnalysisDescription : IXmlSerializable
    {
        private readonly Guid Guid;
        private XmlDocument IdfxDoc;
        protected string WorkingDirPath;

		protected ulong NewStartOffset { get; set; }
        protected string AnalysisAbbr;

        private Dictionary<string, string> Properties { get; set; }
        protected List<Event> Events { get; private set; }
        public SessionIdentification SessionID { get; set; }

        // NOT SERIALIZED YET SINCE IT IS NOT USED FOR CURRENT SERVER ANALYSES
        //public IDictionary<String, IDictionary<String, Object>> ExtraInfo { get; set; }

        protected AnalysisDescription()
        {
            Properties = new Dictionary<string, string>();
            Events = new List<Event>();
            SessionID = new SessionIdentification();
            Guid = Guid.NewGuid();
        }

        protected AnalysisDescription(List<Event> events, SessionIdentification sessionID, string abbrv)
        {
            Properties = new Dictionary<String, String>();
            Events = events;
			// CHANGED 6 march 2013 by Tom Pauwaert - SessionID is the actual sessionID that comes
			// from the original IDFX file. 
			// REASON: Consistency for determining the startTimeOffset.
            //SessionID = new SessionIdentification();
			SessionID = sessionID;
            Guid = Guid.NewGuid();
            AnalysisAbbr = abbrv;

			// Startoffset according to session id.
            var startTimeOffset = sessionID.HasRelativeCreationTime() ? sessionID.GetRelativeCreationTime() : 0;

			// If the first event has a startTime == 0, where first_event.startTime != startTimeOffset then this means
			// that the list of events actually has a resetted start time, and the startTimeOffset should be == 0.
			TimedEventPart firstTimedEvent = FindFirstTimedEvent();
			if (firstTimedEvent != null && firstTimedEvent.StartTime == 0)
			{
				startTimeOffset = 0;
			}

			NewStartOffset = startTimeOffset;
		}

		/// <summary>
		/// Finds the first event in the list that has timing information.
		/// </summary>
		/// <returns>Returns the timing information of the first event with such information.</returns>
		private TimedEventPart FindFirstTimedEvent()
		{
		    // go over events in from first to last.
		    return Events.SelectMany(e => e.Parts).OfType<TimedEventPart>().FirstOrDefault();
		}

        /// <summary>
        /// Sets working dir path, so analyses can retrieve files that were sent along.
        /// Should be overridden by subclasses to update their paths
        /// </summary>
        public virtual void SetWorkingDirPath(string path)
        {
            WorkingDirPath = path;
        }

        /// <summary>
        /// Returns the name of the Analysis to be executed
        /// </summary>
        /// <returns></returns>
        public virtual string GetName()
        {
            return "";
        }

        private void WriteIdfx()
        {
            string outFilename = Guid + ".idfx";
            var outFile = new FileStream(outFilename, FileMode.Create);

			var settings = new XmlWriterSettings
			{
				Indent = true,
				Encoding = Encoding.UTF8,
				CheckCharacters = false,
				CloseOutput = true,
			};

            XmlWriter writer = XmlWriter.Create(outFile, settings);
            IdfxDoc.WriteTo(writer);
            writer.Close();
        }

        /// <summary>
        /// Base classes should overwrite this method to store their properties
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteCustomXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Base classes should overwrite this method to read their properties
        /// </summary>
        /// <param name="reader"> </param>
        protected virtual void ReadCustomXml(XmlReader reader)
        {
        }

        public virtual Analysis GetAnalysis()
        {
            return null;
        }

        public virtual void PreProcess(Analysis an)
        {
        }

        /// <summary>
        /// Override this method to add Processes to the LinguisticProcess pipeline.
        /// </summary>
        public virtual void LinguisticProcess(Analysis an, ref IAnalysisSummary summ)
        {
        }

        /// <summary>
        /// Override this method to return a custom Analysis Writer
        /// </summary>
        /// <param name="outpath"></param>
        public virtual IAnalysisWriter GetAnalysisWriter(string outpath)
        {
            return new AnalysisWriterFactory().Create(GetAnalysis(), outpath);
        }

        #region Xml Serialization Infrastructure

        public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("properties");
			foreach (var prop in Properties)
			{
				writer.WriteStartElement("entry");
				writer.WriteElementString("key", prop.Key);
				writer.WriteElementString("value", prop.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
            writer.WriteElementString("AnalysisAbbr", AnalysisAbbr);
            writer.WriteElementString("NewStartOffset", NewStartOffset.ToString());

			WriteCustomXml(writer);
			var xmlWriter = new XmlEventWriter(writer);
			xmlWriter.Start(SessionID);
			Events.ForEach(xmlWriter.Write);
			xmlWriter.Stop();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("properties");
            while (reader.IsStartElement("entry"))
            {
                reader.ReadStartElement("entry");
                var key = reader.ReadElementString("key");
                var value = reader.ReadElementString("value");
                Properties[key] = value;
                reader.ReadEndElement();
            }
            try
            {
                reader.ReadEndElement();
            }
            catch
            {
            }
            AnalysisAbbr = reader.ReadElementString("AnalysisAbbr");
            NewStartOffset = ulong.Parse(reader.ReadElementString("NewStartOffset"));
            ReadCustomXml(reader);
            reader.ReadToFollowing("log");
            XmlReader logTree = reader.ReadSubtree();
            IdfxDoc = new XmlDocument();
            IdfxDoc.Load(logTree);
            WriteIdfx();
            var eventLogReader = EventLogFactory.CreateFileEventLogReader(Guid + ".idfx", LogFormat.XML);
            SessionID = eventLogReader.ReadSessionIdentification();
            Events = eventLogReader.ReadEvents();
            eventLogReader.Close();
            File.Delete(Guid + ".idfx");
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion

    }
}