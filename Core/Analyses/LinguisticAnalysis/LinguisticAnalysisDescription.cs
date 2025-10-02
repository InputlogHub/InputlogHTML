using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using InputLog.Core.Analyses.LinguisticAnalysis.CombinedAnalysis;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Events;
using InputLog.Core.Pipes;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.LinguisticAnalysis
{
    /// <summary>
    /// Serializable description for a Linguistic Analysis.
    /// </summary>
    public class LinguisticAnalysisDescription : AnalysisDescription
    {
        public static string DocPath { private set; get; }
        private ulong _pauseThreshold;
        private WNotationSummary _wNotSumm;
        private string Language => SessionID.GetLanguage();

        /// <summary>
        /// Parameterless constructor needed for de/serialization.
        /// </summary>
        public LinguisticAnalysisDescription()
        {

        }

        public LinguisticAnalysisDescription(List<Event> events, SessionIdentification sessionID, 
            string abbrv, string docPath, ulong pauseThreshold) : base(events, sessionID, abbrv)
        {
            DocPath = docPath;
            _pauseThreshold = pauseThreshold;
        }

        public override string GetName()
        {
            return "Linguistic Analysis";
        }

        protected override void WriteCustomXml(XmlWriter writer)
        {
            writer.WriteElementString("DocPath", DocPath);
            writer.WriteElementString("PauseThreshold", _pauseThreshold.ToString());
            _wNotSumm.WriteXml(writer);
        }

        protected override void ReadCustomXml(XmlReader reader)
        {
            DocPath = reader.ReadElementString("DocPath");
            _pauseThreshold = ulong.Parse(reader.ReadElementString("PauseThreshold"));
            _wNotSumm = new WNotationSummary();
            _wNotSumm.ReadXml(reader);
        }

        public override void SetWorkingDirPath(String path)
        {
            base.SetWorkingDirPath(path);
            DocPath = Path.Combine(WorkingDirPath, DocPath);
        }

        public override void LinguisticProcess(Analysis an, ref IAnalysisSummary summ)
        {
            var pipeline = new ProcessPipeline();
            var processResults = new DataSet("linguisticProcess");
            // Performs the actual linguistic analysis
            using (var thisSummary = _wNotSumm)
            {
                if (String.IsNullOrWhiteSpace(thisSummary.RevisionTxt) || thisSummary.RevisionTxt.Length < 3)
                {
                    throw new Exception("Empty S-Notation string");
                }
                pipeline.Register(new MarkupRemover(thisSummary)).
                    // Register(new TokenInspector()).
                         Register(new WordsReconstruction(thisSummary.RevisionTxt,
                             thisSummary.SymbolList, NewStartOffset, Language, AnalysisAbbr)).
                         Register(new LT3Combination(Language)).
                         Execute(processResults);
                processResults = pipeline.Output;
                summ = new LinguisticAnalysisSummary(thisSummary, processResults);
            }
        }

        public override void PreProcess(Analysis an)
        {
            var wNot = new WNotationAnalysis(Events, DocPath, _pauseThreshold, SessionID);
            _wNotSumm = (WNotationSummary) wNot.DoAnalysis();
        }

        public override Analysis GetAnalysis()
        {
            return null;
        }

        public override IAnalysisWriter GetAnalysisWriter(String outpath)
        {
            return new LinguisticAnalysisXMLWriter(outpath);
        }
    }
}