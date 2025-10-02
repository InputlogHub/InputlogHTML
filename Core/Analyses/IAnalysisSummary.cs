using System;
using System.Xml.Serialization;
using InputLog.Core.Reporting;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Tagging interface for AnalysisSummaries.
    /// </summary>
    public interface IAnalysisSummary : IXmlSerializable, IDisposable, IReportData
    { }
}