using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GUI.Tabs.Analyze.AnalysesControls;
using GUI.Tabs.Preprocess;
using GUI.Tabs.Preprocess.Recoders;
using GUI.Tabs.Preprocess.Filters;

namespace GUI.Tabs.Analyze.ImportExport
{
    /// <summary>
    /// Class that can serialize sources, analysis and filter configurations to XML.
    /// </summary>
    public class AnalysisConfigurationXMLSerializer : IDisposable
    {
        #region Tags
        public const string CONFIG_TAG = "analysisConfig";

        public const string ANALYSES_TAG = "analyses";
        public const string ANALYSIS_TAG = "analysis";
        public const string PARAM_TAG = "param";

        public const string NAME_ATTRIBUTE = "name";
        public const string VALUE_ATTRIBUTE = "value";

        public const string FILTERS_TAG = "filters";
        public const string FILTER_TAG = "filter";

        public const string SOURCES_TAG = "sources";
        public const string SOURCE_TAG = "source";

        public const string DESTINATION_TAG = "destination";
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether this instance of this class has been disposed.
        /// </summary>
        private bool Disposed;

        /// <summary>
        /// XmlWriter used to writer xml to the analysis configuration file.
        /// </summary>
        protected XmlTextWriter XMLWriter;

        #endregion

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="destinationFilePath">File to write XML to.</param>
        public AnalysisConfigurationXMLSerializer(string destinationFilePath)
        {
            XMLWriter = new XmlTextWriter(destinationFilePath, Encoding.UTF8) {Formatting = Formatting.Indented};
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this AnalysisType implements a finalizer.
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Writes a XML header and root element to the stream.
        /// </summary>
        public void WriteHeader()
        {
            XMLWriter.WriteStartDocument();
            XMLWriter.WriteStartElement(CONFIG_TAG);
        }

        /// <summary>
        /// Writes a XML footer to the stream.
        /// </summary>
        public void WriteFooter()
        {
            XMLWriter.WriteEndElement();
            XMLWriter.WriteEndDocument();
        }

        /// <summary>
        /// Serializes a given analaysis configuration to the stream (using ANALYSIS to wrap the key-value pairs 
        /// of the given dictionary).
        /// </summary>
        /// <param name="controls">Configuration to serialize</param>
        /// <param name="exportOptions"></param>
        public void SerializeAnalysisConfiguration(List<AnalysisControl> controls, ExportOptions exportOptions)
        {
            XMLWriter.WriteStartElement(ANALYSES_TAG);
            foreach (var configuration in controls.Select(control => control.Export(exportOptions)))
            {
                XMLWriter.WriteStartElement(ANALYSIS_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, configuration.AnalysisName);

                foreach (var entry in configuration.Parameters)
                {
                    WriteParam(entry.Key, entry.Value);
                }
                XMLWriter.WriteEndElement();
            }
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes a given filter configuration to the stream (using FILTER to wrap the key-value pairs 
        /// of the given dictionary).
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="filterOptions"></param>
        public void SerializeFilterConfiguration(List<ProcessControl> controls, FilterExportOptions filterOptions)
        {
            XMLWriter.WriteStartElement(FILTERS_TAG);
            foreach (var configuration in controls.Select(control => control.Export(filterOptions)))
            {
                XMLWriter.WriteStartElement(FILTER_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, configuration.FilterName);

                foreach (var entry in configuration.Parameters)
                {
                    WriteParam(entry.Key, entry.Value);
                }
                XMLWriter.WriteEndElement();
            }
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes source paths to the stream.
        /// </summary>
        public void SerializeSource(List<String> sources)
        {
            XMLWriter.WriteStartElement(SOURCES_TAG);
            foreach (var source in sources)
            {
                XMLWriter.WriteStartElement(SOURCE_TAG);
                XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, source);
                XMLWriter.WriteEndElement();
            }
            XMLWriter.WriteFullEndElement();
        }

        /// <summary>
        /// Serializes a destination path to the stream.
        /// </summary>
        public void SerializeDestination(String destination)
        {
            XMLWriter.WriteStartElement(DESTINATION_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, destination);
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes a param to the stream (using the PARAM_TAG).
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">Value of the param.</param>
        private void WriteParam(string paramName, string paramValue)
        {
            XMLWriter.WriteStartElement(PARAM_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, paramName);
            XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, paramValue);
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                XMLWriter.Close();
            }

            Disposed = true;
        }

        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// Do not provide any destructors in types derived from this class.
        /// </summary>
        ~AnalysisConfigurationXMLSerializer()
        {
            Dispose(false);
        }
    }
}