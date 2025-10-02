using System;
using System.Collections.Generic;
using System.Xml;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Tagging interface representing an AnalysisWriter.
    /// An AnalysisWriter is a class that provides functionality to write the result of an analysis.
    /// </summary>
    public interface IAnalysisWriter : IDisposable
    {
        void WriteDocument(SessionIdentification sessionIdentification, IDictionary<string, IDictionary<string, object>> extraInfo,
            IAnalysisSummary summary);
        XmlDocument WriteMemory(IAnalysisSummary summary);
        void Abort();
    }
}