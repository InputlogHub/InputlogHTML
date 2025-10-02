using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputLog.Core.Analyses.General;
using InputLog.Core.Events;
using InputLog.Core.IO;
using InputLog.Core.Util.KeyConversion;
using log4net;

namespace InputLog.Core.Analyses.Bigram
{
    /// <summary>
    /// A Fluency Analysis.
    /// Measures Fluency based on strokes per minute during intervals, compared to maxima.
    /// </summary>
    public class BigramAnalysis : Analysis
    {
        public enum Average
        {
            MEAN, MEDIAN
        }
        #region Fields

        private readonly GeneralAnalysis General;

        private readonly Dictionary<string, Dictionary<string, string>> BigramInfo;

        private readonly Average AvgStat;

        private readonly int SpeedThreshold;

        // Log4Net MessageLogger.
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// Constructs a new BigramAnalysis. 
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the list of events.</param>
        /// <param name="bigramInfo"></param>
        /// <param name="controlKeys">List of controlKeys that need to be taken into account 
        /// (controlkeys are displayed differently in the analysis).</param>
        /// <param name="extraParameters">Dictionary containing extra parameters depending on the linearAnalysisType</param>
        /// <param name="avgStat"></param>
        /// <param name="speedThreshold"></param>
        public BigramAnalysis(List<Event> events, SessionIdentification sessionID, Average avgStat,
            int speedThreshold, Dictionary<string, Dictionary<string, string>> bigramInfo, 
            List<KeysEx> controlKeys = null, Dictionary<string,object> extraParameters = null)
            : base("BA", events, sessionID)
        {
            int numberOfIntervals = 0;
            ulong intervalSize = 0;
            General = new GeneralAnalysis(events, sessionID, "BA", numberOfIntervals, intervalSize);
            BigramInfo = bigramInfo;
            AvgStat = avgStat;
            SpeedThreshold = speedThreshold;
        }

        /// <summary>
        /// Performs the actual Bigram Analysis on the periods found in the linear analysis
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            var summary = new BigramAnalysisSummary();
            try
            {
                GeneralAnalysisSummary gaSumm = (GeneralAnalysisSummary) General.DoAnalysis();
                summary.GetBigrams(gaSumm);
                switch (AvgStat)
                {
                    case Average.MEAN:
                        BigramAnalysisSummary.SpecialBigramInfo.GetAverage = x => x.GetMean();
                        break;
                    case Average.MEDIAN:
                        BigramAnalysisSummary.SpecialBigramInfo.GetAverage = x => x.GetMedian();
                        break;
                }
                foreach (string s in BigramInfo.Keys)
                {
                    summary.AlphaGrams.Add(s, new BigramAnalysisSummary.AlphaBigramInfo(s, BigramInfo[s]));
                }
                foreach (BigramAnalysisSummary.BigramClass bigClass
                    in Enum.GetValues(typeof(BigramAnalysisSummary.BigramClass)))
                {
                    summary.ClassGrams.Add(bigClass.ToString(), 
                        new BigramAnalysisSummary.SpecialBigramInfo(bigClass.ToString()));
                }
                summary.ClassGrams.Add("Total", new BigramAnalysisSummary.SpecialBigramInfo("Total"));

                foreach (BigramAnalysisSummary.Bigram bi in summary.Bigrams)
                {
                    if (bi.ActionTime() < (ulong)SpeedThreshold) continue;
                    string biStr = bi.ToString();
                    if (bi.IsNormalAlpha && summary.AlphaGrams.Keys.Contains(biStr))
                    {
                        summary.AlphaGrams[biStr].Add(bi);
                    }
                    else
                    {
                        if (!summary.SpecialGrams.Keys.Contains(biStr))
                        {
                            summary.SpecialGrams.Add(biStr, new BigramAnalysisSummary.SpecialBigramInfo(biStr));
                        }
                        summary.SpecialGrams[biStr].Add(bi);
                    }
                    summary.ClassGrams[bi.BigramClass.ToString()].Add(bi);
                    summary.ClassGrams["Total"].Add(bi);
                }
                summary.MakeSummary(BigramInfo);
            }
            catch (AnalysisException e)
            {
                Log.Warn(e); // exception while analyzing an event, log it and skip it...
            }
            return summary;
        }

    }
}