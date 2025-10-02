using System;
using System.Reflection;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Factory that can create analysisWriters (these can write the result of an analysis to file)
    /// given an analysis and a fileformat.
    /// </summary>
    public class AnalysisWriterFactory
    {
        #region Format enum

        /// <summary>
        /// Different outputformats that are currently supported by this Factory.
        /// </summary>
        public enum Format
        {
            XML,
            MEMORY
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private const string WRITER_CLASS_SUFFIX = "Writer";

        /// <summary>
        /// Creates an AnalysisWriter for a given analysis type and a given output format.
        /// </summary>
        /// <param name="analysisType">The type of analysis for which to create an AnalysisWriter.</param>
        /// <param name="destinationFilePath">The path where the analysis document should be written by the AnalysisWriter.</param>
        /// <param name="format">The outputFormat of the analysis document.</param>
        /// <returns>An AnalysisWriter that can write analysis documents for the given analysis
        /// in the given outputformat or null if no such analysiswriter exists.</returns>
        private IAnalysisWriter Create(Type analysisType, string destinationFilePath, Format format = Format.XML)
        {
            string writerName = analysisType.FullName + format.ToString() + WRITER_CLASS_SUFFIX;
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type writerType = currentAssembly.GetType(writerName);
            return (IAnalysisWriter) Activator.CreateInstance(writerType, new object[] {destinationFilePath});
        }

        /// <summary>
        /// Convenience method for AnalysisWriterFactory.create(analysis.GetType(), destinationFilePath, format);
        /// </summary>
        /// <param name="analysis">The type of analysis for which to create an AnalysisWriter.</param>
        /// <param name="destinationFilePath">The path where the analysis document should be written by the AnalysisWriter.</param>
        /// <param name="format">The outputFormat of the analysis document.</param>
        /// <returns>An AnalysisWriter that can write analysis documents for the given analysis
        /// in the given outputformat or null if no such analysiswriter exists.</returns>
        public IAnalysisWriter Create(Analysis analysis, string destinationFilePath, Format format = Format.XML)
        {
            return Create(analysis.GetType(), destinationFilePath, format);
        }
    }
}