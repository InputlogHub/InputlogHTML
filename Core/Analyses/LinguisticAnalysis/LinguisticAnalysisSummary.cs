using System.Data;
using System.Xml;
using InputLog.Core.Analyses.Revision.Notations;

namespace InputLog.Core.Analyses.LinguisticAnalysis
{
    /// <summary>
    /// A string with the text and its revisions represented as inserts, deletions, and breaks 
    /// according to the W-Notation conventions.
    /// </summary>
    public class LinguisticAnalysisSummary : WNotationSummary
    {
        #region Fields

        public DataSet LinguisticProcessResults;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summ">Summary</param>
        /// <param name="linguisticProcessResults"></param>
        public LinguisticAnalysisSummary(WNotationSummary summ, DataSet linguisticProcessResults)
            : base(summ)
        {
            LinguisticProcessResults = linguisticProcessResults;
        }

        /// <summary>
        /// Parameterless constructor needed for deserialization.
        /// </summary>
        public LinguisticAnalysisSummary()
        {           
        }

        #region Xml Serialization Infrastructure

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            //writer.WriteStartElement("LinguisticProcessResults");
            LinguisticProcessResults.WriteXml(writer);
            //writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            //reader.ReadStartElement("LinguisticProcessResults");
            LinguisticProcessResults = new DataSet("linguisticProcess");
            LinguisticProcessResults.ReadXml(reader);
            //reader.ReadEndElement();
        }

        #endregion
    }
}