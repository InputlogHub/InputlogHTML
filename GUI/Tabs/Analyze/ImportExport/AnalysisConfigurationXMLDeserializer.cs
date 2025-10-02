using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GUI.Tabs.Preprocess;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.ImportExport
{
    /// <summary>
    /// Class that can deserialize XML files into AnalysisConfiguration's.
    /// </summary>
    public class AnalysisConfigurationXMLDeserializer : IDisposable
    {
        #region Fields

        /// <summary>
        /// Indicates whether this instance of this class has been disposed.
        /// </summary>
        private bool Disposed;

        /// <summary>
        /// XmlDocument used to read xml from the analysis configuration file.
        /// </summary>
        protected XmlDocument XMLDocument;

        #endregion

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="sourceFilePath">File to read XML from.</param>
        public AnalysisConfigurationXMLDeserializer(string sourceFilePath)
        {
            XMLDocument = new XmlDocument {PreserveWhitespace = true};
            XMLDocument.Load(sourceFilePath);
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this LinearAnalysisType implements a finalizer.
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Deserializes a list of analaysis configurations from an XML file.
        /// </summary>
        /// <returns>A list of AnalysisConfigurations that are read from the XML file.</returns>
        public List<AnalysisConfiguration> DeserializeAnalysisConfiguration()
        {
            var configurations = new List<AnalysisConfiguration>();
            var analysesConfigTag = XMLDocument[AnalysisConfigurationXMLSerializer.CONFIG_TAG];
            if (analysesConfigTag != null)
            {
                try
                {
                    var analyses = analysesConfigTag.GetElementsByTagName(AnalysisConfigurationXMLSerializer.ANALYSIS_TAG);

                    // read the different analyses
                    foreach (var analysis in analyses.OfType<XmlElement>())
                    {
                        var analysisName = analysis.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                        var configuration = new AnalysisConfiguration(analysisName);

                        // read parameters for this configuration
                        foreach (XmlElement param in analysis.GetElementsByTagName(AnalysisConfigurationXMLSerializer.PARAM_TAG))
                        {
                            var name = param.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                            var value = param.Attributes[AnalysisConfigurationXMLSerializer.VALUE_ATTRIBUTE].Value;
                            configuration.Parameters[name] = value;
                        }
                        configurations.Add(configuration);
                    }
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR,
                                                 "Unable to import an analysis configuration. Continuing with other analyses.");
                }
            }

            return configurations;
        }

        /// <summary>
        /// Deserializes a list of filter configurations from an XML file.
        /// </summary>
        /// <returns>A list of FilterConfigurations that are read from the XML file.</returns>
        public List<FilterConfiguration> DeserializeFilterConfiguration()
        {
            var configurations = new List<FilterConfiguration>();
            var analysesConfigTag = XMLDocument[AnalysisConfigurationXMLSerializer.CONFIG_TAG];
            if (analysesConfigTag != null)
            {
                try
                {
                    var filters = analysesConfigTag.GetElementsByTagName(AnalysisConfigurationXMLSerializer.FILTER_TAG);

                    // Get the different filters
                    foreach (var analysis in filters.OfType<XmlElement>())
                    {
                        var filterName = analysis.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                        var filterConfiguration = new FilterConfiguration(filterName);

                        // read parameters for this configuration
                        foreach ( XmlElement param in analysis.GetElementsByTagName(AnalysisConfigurationXMLSerializer.PARAM_TAG))
                        {
                            var name = param.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                            var value = param.Attributes[AnalysisConfigurationXMLSerializer.VALUE_ATTRIBUTE].Value;
                            filterConfiguration.Parameters[name] = value;
                        }
                        configurations.Add(filterConfiguration);
                    }
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR,
                                                 "Unable to import a filter configuration. Continuing with other filters.");
                }
            }
            return configurations;
        }

        /// <summary>
        /// Deserializes a list of source paths from an XML file.
        /// </summary>
        /// <returns>A list of source paths that are read from the XML file.</returns>
        public List<String> DeserializeSources()
        {
            var sources = new List<String>();
            var analysesConfigTag = XMLDocument[AnalysisConfigurationXMLSerializer.CONFIG_TAG];
            if (analysesConfigTag != null)
            {
                try
                {
                    var sourceElements = analysesConfigTag.GetElementsByTagName(AnalysisConfigurationXMLSerializer.SOURCE_TAG);

                    // Get the different source paths
                    foreach (var element in sourceElements.OfType<XmlElement>())
                    {
                        var sourceName = element.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                        sources.Add(sourceName);
                    }
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR,
                                                 "Unable to import a source file path. Continuing with other sources.");
                }
            }
            return sources;
        }

        /// <summary>
        /// Deserializes the destination dir from an XML file.
        /// </summary>
        /// <returns>The destination dir read from the XML file.</returns>
        public String DeserializeDestination()
        {
            var destination = String.Empty;
            var destinationTag = XMLDocument.DocumentElement[AnalysisConfigurationXMLSerializer.DESTINATION_TAG];
            if (destinationTag != null)
            {
                try
                {
                    destination = destinationTag.Attributes[AnalysisConfigurationXMLSerializer.NAME_ATTRIBUTE].Value;
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to import the destination dir");
                }
            }
            return destination;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                // nothing to do
            }

            Disposed = true;
        }

        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// Do not provide any destructors in types derived from this class.
        /// </summary>
        ~AnalysisConfigurationXMLDeserializer()
        {
            Dispose(false);
        }
    }
}