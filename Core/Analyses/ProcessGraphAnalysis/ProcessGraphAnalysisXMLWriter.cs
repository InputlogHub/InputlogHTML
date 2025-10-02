using System;
using System.Collections.Generic;

namespace InputLog.Core.Analyses.ProcessGraphAnalysis
{
    /// <summary>
    /// Empty writer... because nothing should be written to XML.
    /// </summary>
    public class ProcessGraphAnalysisXMLWriter: AbstractAnalysisXMLWriter
    {
        public ProcessGraphAnalysisXMLWriter(string destination): base(destination) { }

        public override void WriteDocument(IO.SessionIdentification sessionIdentification, 
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }

        public override System.Xml.XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }
    }

    

}
