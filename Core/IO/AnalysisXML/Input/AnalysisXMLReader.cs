using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
using InputLog.Core.Merging.Analyses;

namespace InputLog.Core.IO.AnalysisXML.Input
{
	/// <summary>
	/// Class that reads analysis XML files and stores them in an in-memory format.
	/// These files can then be traversed on a per file or a per entry basis.
	/// </summary>
	public class AnalysisXMLReader
	{
		#region Fields

		/// <summary>
		/// An XML document used for reading the different files.
		/// </summary>
		private readonly XmlDocument XmlDoc;

		/// <summary>
		/// The header of the file.
		/// </summary>
		public Header Header { get; private set; }

		/// <summary>
		/// Lists all the modules in a file.
		/// </summary>
		public List<Module> Modules { get; private set; }

		/// <summary>
		/// List of all the events in a file.
		/// </summary>
		public List<Event> Events { get; private set; }

		/// <summary>
		/// List of all the extra information in a file.
		/// </summary>
		public List<ExtraInfo> Extras { get; private set; }

		/// <summary>
		///  Bool set to true if the last read was successful, false
		///  if it failed.
		/// </summary>
		public bool Successful { get; private set; }
		#endregion

        /// <summary>
        /// Preserving whitespace gives difficulties when reading the data.
        /// </summary>
		public AnalysisXMLReader()
		{
		    XmlDoc = new XmlDocument {PreserveWhitespace = false};	   
		}

		public void Read(string filePath, AnalysisType type, DateTime timestamp)
		{
		    Successful = false;
			//try
			//{
				// Initialize storage datamembers.
				Header = new Header();
				// TODO what if extras === null?
				Extras = new List<ExtraInfo>();
				Modules = new List<Module>();
				Events = new List<Event>();

				// Get the methods to be used for reading the information.
				// The use of the delegate methods based on the analysis types makes sure
				// that the reading of this information is tailored for the specific
				// analysis type.
				var readerFactory = new AnalysisReaderFactory();
				BasicXMLReader reader = AnalysisReaderFactory.Create(type);
				BasicXMLReader.ReadHeaderMethod readHeader = reader.ReadHeader();
				BasicXMLReader.ReadExtraInfoMethod readExtraInfo = reader.ReadExtraInfo();
				BasicXMLReader.ReadEventsMethod readEvents = reader.ReadEvents();
				BasicXMLReader.ReadModulesMethod readModules = reader.ReadModules();

				var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				XmlDoc.Load(stream);

				// Read the XML file and store in info in memory.
				Header = readHeader(XmlDoc, timestamp.ToString());
				Extras = readExtraInfo(XmlDoc);
				Events = readEvents(XmlDoc);
				Modules = readModules(XmlDoc);

				// Add the filename as property to the Header.
				Header.Properties["filepath"] = filePath;
				Successful = true;
			/*}
			catch (Exception e)
			{
				Successful = false;
				Util.MessageLogger.LogMessage(this, "Reading Error", 
                    "Improperly constructed file, or invalid file type for merging: " +
                    "[" + filePath + "]", Util.Severity.FATAL);
                throw e;
			}
			finally
			{
				// Close the file.
				if (stream != null)
				{
					stream.Close();
				}
			}*/
		}
	}
}
