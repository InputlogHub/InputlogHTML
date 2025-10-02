using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using InputLog.Core.Analyses.General;
using InputLog.Core.Reporting;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Bigram
{
    /// <summary>
    ///     Summary of the Linear analysis.
    ///     Contains a list of Periods. Each Period contains Events that represent a keypress or pause.
    /// </summary>
    public class BigramAnalysisSummary : AbstractAnalysisSummary
    {
        public enum BigramClass
        {
            CHAR_CHAR,
           // CHAR_CHAR_SYLLABLE_BOUNDARY,
            CHAR_SPACE,
            SPACE_CHAR,
            CHAR_PUNCT_INTER,
            CHAR_PUNCT_INTRA,
            CHAR_REV,
            REV_CHAR,
            SHIFT_CHAR,
            CAP_CHAR,
            NR_NR,
            UNSPECIFIED
        }
        #region Nested type: Bigram
        /// <summary>
        /// 
        /// </summary>
        public class Bigram
        {
            private readonly GeneralAnalysisSummary.GeneralAnalysisEvent First;
            private readonly GeneralAnalysisSummary.GeneralAnalysisEvent Second;
            public readonly bool IsNormalAlpha;
            private readonly string StringRepr;
            public readonly BigramClass BigramClass;

            public Bigram(GeneralAnalysisSummary.GeneralAnalysisEvent first, 
                GeneralAnalysisSummary.GeneralAnalysisEvent second)
            {
                First = first;
                Second = second;
                string f = First.Output.Length == 1 ? First.Output.ToLower() : First.Output;
                string s = Second.Output.Length == 1 ? Second.Output.ToLower() : Second.Output;
                bool fIsChar = Regex.IsMatch(f.Trim(), @"^[a-z]+$");
                bool sIsChar = Regex.IsMatch(s.Trim(), @"^[a-z]+$");
                if (fIsChar && sIsChar)
                {
                    IsNormalAlpha = true;
                    StringRepr = f + s;
                }
                else
                {
                    IsNormalAlpha = false;
                    StringRepr = f + "+" + s;
                }
                bool sIsPunctuationIntra = Regex.IsMatch(s.Trim(), @"^[,;\-:]$");
                bool sIsPunctuationInter = Regex.IsMatch(s.Trim(), @"^[\.!\?]$");
                BigramClass = BigramClass.UNSPECIFIED;
                if (IsNormalAlpha)
                {
                    if ((!f.Equals(first.Output)) && s.Equals(second.Output))
                        BigramClass = BigramClass.CAP_CHAR;
                    else
                        BigramClass = BigramClass.CHAR_CHAR;
                    //TODO Syllable boundary
                }
                else if (fIsChar)
                {
                    if (s.Equals("SPACE"))
                        BigramClass = BigramClass.CHAR_SPACE;
                    else if (sIsPunctuationInter)
                        BigramClass = BigramClass.CHAR_PUNCT_INTER;
                    else if (sIsPunctuationIntra)
                        BigramClass = BigramClass.CHAR_PUNCT_INTRA;
                    else if (s.Equals("BACK") || s.Equals("DEL"))
                        BigramClass = BigramClass.CHAR_REV;
                }
                else if (sIsChar)
                {
                    if (f.Equals("SPACE"))
                        BigramClass = BigramClass.SPACE_CHAR;
                    if (f.Equals("SHIFT") || f.Equals("LSHIFT") || f.Equals("RSHIFT"))
                        BigramClass = BigramClass.SHIFT_CHAR;
                    if (f.Equals("BACK") || f.Equals("DEL"))
                        BigramClass = BigramClass.REV_CHAR;
                }
                else if (Regex.IsMatch(f.Trim(), @"^\d$") && Regex.IsMatch(s.Trim(), @"^\d$"))
                {
                    BigramClass = BigramClass.NR_NR;
                }
            }

            public bool Equals(Bigram rhs)
            {
                return rhs.ToString().Equals(ToString());
            }

            public override string ToString()
            {
                return StringRepr;
            }

            public ulong ActionTime()
            {
                if (null != Second.ActionTime && null != First.ActionTime)
                {
                    return (ulong) Second.ActionTime + (ulong) First.ActionTime;
                }
                return 0;
            }
        }
        #endregion

        public class AlphaBigramInfo : SpecialBigramInfo
        {
            Dictionary<string, string> ExtraInfo;

            public AlphaBigramInfo(string key, Dictionary<string, string> extraInfo): base(key)
            {
                ExtraInfo = extraInfo;
            }
        }

        public class SpecialBigramInfo
        {
            public int Count;
            public ulong TotalActionTime;
            public ulong SqTatSum;
            public readonly List<ulong> ActionTimes;
            public readonly List<double> LogActionTimes;
            public ulong MinActionTime;
            public ulong MaxActionTime;
            public readonly string Key;
            public static Func<SpecialBigramInfo, double> GetAverage = null;

            public SpecialBigramInfo(string key)
            {
                Count = 0;
                TotalActionTime = 0;
                SqTatSum = 0;
                MaxActionTime = ulong.MinValue;
                MinActionTime = ulong.MaxValue;
                ActionTimes = new List<ulong>();
                LogActionTimes = new List<double>();
                Key = key;
            }

            public void Add(Bigram gram)
            {
                Count++;
                ulong at = gram.ActionTime();
                TotalActionTime += at;
                SqTatSum += (at * at);
                if (at < MinActionTime) MinActionTime = at;
                if (at > MaxActionTime) MaxActionTime = at;
                ActionTimes.Add(at);
                LogActionTimes.Add(Math.Log(Convert.ToDouble(at)));
            }

            public string GetKey()
            {
                return Key;
            }

            public int GetCount()
            {
                return Count;
            }

            public double GetMean()
            {
                return MathExt.Mean(Count, TotalActionTime);
            }

            public double GetMedian()
            {
                ActionTimes.Sort();
                return MathExt.FindMedian(ActionTimes);
            }

            /// <summary>
            /// Calculates a Confidence Interval (CI) around the mean from a list of pause values
            /// with the Student T distribution with n-1 degrees of freedom.
            /// CI = Mean +- Student T * SE, where SE (Standard Error) = stDev/SQRT(n)
            /// </summary>
            /// <returns>a tuple with as first element the low interval boundary (double),
            /// as second the (geometric) mean, thirdly the high interval boundary (double), and
            /// as fourth element the (geometric) standard deviation for the given confidence level.</returns>
            public Tuple<double, double, double, double> GetInterval()
            {
                return MathExt.CalculateInterval(LogActionTimes, .95);
            }

            public double GetStdDev()
            {
                return MathExt.StandardDeviationSample(Count, SqTatSum, TotalActionTime);
            }

            public double Get95PctLow()
            {
                if (Count == 0) return 0;
                double res = GetMean() - 1.96 * (GetStdDev() / Math.Sqrt(Count));
                return res > 0 ? res : 0;
            }

            public double Get95PctHigh()
            {
                if (Count == 0) return 0;
                double res = GetMean() + 1.96 * (GetStdDev() / Math.Sqrt(Count));
                return res > 0 ? res : 0;
            }

            public double GetMin()
            {
                return Count > 0 ? MinActionTime : 0;
            }

            public double GetMax()
            {
                return MaxActionTime;
            }

        }

        #region fields
        public readonly List<Bigram> Bigrams;
        public readonly Dictionary<string, AlphaBigramInfo> AlphaGrams;
        public readonly Dictionary<string, SpecialBigramInfo> SpecialGrams;
        public readonly Dictionary<string, SpecialBigramInfo> ClassGrams;
        public readonly List<AlphaBigramInfo> Top5Fastest;
        public readonly List<AlphaBigramInfo> Top5Slowest;
        public readonly List<AlphaBigramInfo> Top5Freq;
        public readonly List<AlphaBigramInfo> Bot5Freq;
        public readonly List<AlphaBigramInfo> Top5Freq2;
        public readonly List<AlphaBigramInfo> Bot5Freq2;

        private int Count;
        private ulong TotalActionTime;
        private ulong SqTatSum;
        private readonly List<ulong> ActionTimes;
        private ulong MinActionTime;
        private ulong MaxActionTime;
        #endregion

        /// <summary>
        ///     Construct the summary.
        /// </summary>
        public BigramAnalysisSummary()
        {
            Bigrams = new List<Bigram>();
            AlphaGrams = new Dictionary<string, AlphaBigramInfo>();
            SpecialGrams = new Dictionary<string, SpecialBigramInfo>();
            ClassGrams = new Dictionary<string, SpecialBigramInfo>();
            Top5Fastest = new List<AlphaBigramInfo>();
            Top5Slowest = new List<AlphaBigramInfo>();
            Top5Freq = new List<AlphaBigramInfo>();
            Bot5Freq = new List<AlphaBigramInfo>();
            Top5Freq2 = new List<AlphaBigramInfo>();
            Bot5Freq2 = new List<AlphaBigramInfo>();

            Count = 0;
            TotalActionTime = 0;
            SqTatSum = 0;
            MaxActionTime = ulong.MinValue;
            MinActionTime = ulong.MaxValue;
            ActionTimes = new List<ulong>();
        }

        public void GetBigrams(GeneralAnalysisSummary gaSumm)
        {
            for (int i = 0; i < gaSumm.Events.Count - 1; i++)
            {
                GeneralAnalysisSummary.GeneralAnalysisEvent thisEven = gaSumm.Events[i];
                if (thisEven.Type.Equals("keyboard"))
                {
                    GeneralAnalysisSummary.GeneralAnalysisEvent nextEven = gaSumm.Events[i + 1];
                    if (nextEven.Type.Equals("keyboard"))
                    {
                        Bigrams.Add(new Bigram(thisEven, nextEven));
                    }
                }
            }
        }

        public void MakeSummary(Dictionary<string, Dictionary<string, string>> bigramInfo)
        {
            List<KeyValuePair<string, AlphaBigramInfo>> myList = AlphaGrams.ToList();
            List<KeyValuePair<string, AlphaBigramInfo>> myListWc = new List<KeyValuePair<string, AlphaBigramInfo>>();
            foreach (KeyValuePair<string, AlphaBigramInfo> kvp in myList)
            {
                if (kvp.Value.Count > 4)
                {
                    myListWc.Add(kvp);
                }
            }
            myListWc.Sort((firstPair, nextPair) =>
                {
                    double avg1 = SpecialBigramInfo.GetAverage(firstPair.Value);
                    double avg2 = SpecialBigramInfo.GetAverage(nextPair.Value);
                    return avg1.CompareTo(avg2);
                });
            for (int i = 0; i < 5 && i < myListWc.Count; i++)
            {
                Top5Fastest.Add(myListWc[i].Value);
                int bi = myListWc.Count - 1 - i;
                Top5Slowest.Add(myListWc[bi].Value);
            }
            int maxFreq = 0;
            Dictionary<string, int> freqs = new Dictionary<string, int>();
            foreach (string s in bigramInfo.Keys)
            {
                int freq = int.Parse(bigramInfo[s]["freq"]);
                if (freq == 0) freq = 1;
                freqs.Add(s, freq);
                if (freq > maxFreq) maxFreq = freq;
            }
            myListWc.Sort((firstPair, nextPair) =>
            {
                double c1 = Math.Sqrt(nextPair.Value.Count) * freqs[nextPair.Key] ;// maxFreq;
                double c2 = Math.Sqrt(firstPair.Value.Count) * freqs[firstPair.Key];// maxFreq;
                return c1.CompareTo(c2);
            });
            for (int i = 0; i < 5 && i < myListWc.Count; i++)
            {
                Top5Freq.Add(myListWc[i].Value);
                int bi = myListWc.Count - 1 - i;
                Bot5Freq.Add(myListWc[bi].Value);
            }
            myListWc.Sort((firstPair, nextPair) =>
            {
                int c1 = nextPair.Value.Count > 0 ? freqs[nextPair.Key] : 0;
                int c2 = firstPair.Value.Count > 0 ? freqs[firstPair.Key] : 0;
                return c1.CompareTo(c2);
            });
            for (int i = 0; i < 5 && i < myListWc.Count; i++)
            {
                Top5Freq2.Add(myListWc[i].Value);
                int bi = myListWc.Count - 1 - i;
                Bot5Freq2.Add(myListWc[bi].Value);
            }

            foreach (AlphaBigramInfo big in AlphaGrams.Values)
            {
                Count += big.Count;
                TotalActionTime += big.TotalActionTime;
                SqTatSum += big.SqTatSum;
                ActionTimes.AddRange(big.ActionTimes);
                if (big.MinActionTime < MinActionTime)
                    MinActionTime = big.MinActionTime;
                if (big.MaxActionTime > MaxActionTime)
                    MaxActionTime = big.MaxActionTime;
            }

        }

        public int GetCount()
        {
            return Count;
        }

        public double GetMean()
        {
            return MathExt.Mean(Count, TotalActionTime);
        }

        public double GetMedian()
        {
            ActionTimes.Sort();
            return MathExt.FindMedian(ActionTimes);
        }

        public double GetStdDev()
        {
            return MathExt.StandardDeviation(Count, SqTatSum, TotalActionTime);
        }

        public double Get95Pct()
        {
            return Count > 0 ? (1.96 * GetStdDev()) : 0;
        }

        public double GetMin()
        {
            return Count > 0 ? MinActionTime : 0;
        }

        public double GetMax()
        {
            return MaxActionTime;
        }


        /*
         * IMPORTANT NOTE: 
         * These methods are being referenced in the reporting functionality, by name!
         * Do not change these method names without changing the references
         * in the required resource files as well.
         * 
         * Resource File: Core.Reporting.Resources.ReportMappingResources.resx
         * 
         * Note: The report_ prefix must be kept! The methods are also reflectively discovered
         * in the GetBoundTargets() method of the AbstractAnalysisSummary class!
         */
        #region Reporting

        CultureInfo culture = new CultureInfo("en-US");

        public ReportValue report_TransitionTime_CharChar_Avg()
        {
            string key = BigramClass.CHAR_CHAR.ToString();
            double time = this.ClassGrams[key].GetMean();

            const string RESOURCE_ID = "Bigram_TransitionTime_CharChar_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                time.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_TransitionTime_CharChar_Stdev()
        {
            string key = BigramClass.CHAR_CHAR.ToString();
            double stdev = this.ClassGrams[key].GetStdDev();

            const string RESOURCE_ID = "Bigram_TransitionTime_CharChar_Stdev";
            return new LabeledValue(
                RESOURCE_ID,
                stdev.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5FastestBigrams_Avg()
        {
            IEnumerable<double> actionTimes = this.Top5Fastest.Select<AlphaBigramInfo, double>(bg => bg.GetMean());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5FastestBigrams_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5SlowestBigrams_Avg()
        {
            IEnumerable<double> actionTimes = this.Top5Slowest.Select<AlphaBigramInfo, double>(bg => bg.GetMean());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5SlowestBigrams_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5FastestBigrams_Stdev()
        {
            IEnumerable<double> actionTimes = this.Top5Fastest.Select<AlphaBigramInfo, double>(bg => bg.GetStdDev());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5FastestBigrams_Stdev";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5SlowestBigrams_Stdev()
        {
            IEnumerable<double> actionTimes = this.Top5Slowest.Select<AlphaBigramInfo, double>(bg => bg.GetStdDev());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5SlowestBigrams_Stdev";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5HFBigrams_Avg()
        {
            IEnumerable<double> actionTimes = this.Top5Freq.Select<AlphaBigramInfo, double>(bg => bg.GetMean());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5HFBigrams_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5HFBigrams_Stdev()
        {
            IEnumerable<double> actionTimes = this.Top5Freq.Select<AlphaBigramInfo, double>(bg => bg.GetStdDev());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5HFBigrams_Stdev";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5LFBigrams_Avg()
        {
            IEnumerable<double> actionTimes = this.Bot5Freq.Select<AlphaBigramInfo, double>(bg => bg.GetMean());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5LFBigrams_Avg";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }

        public ReportValue report_5LFBigrams_Stdev()
        {
            IEnumerable<double> actionTimes = this.Bot5Freq.Select<AlphaBigramInfo, double>(bg => bg.GetStdDev());
            double avg = actionTimes.Average();

            const string RESOURCE_ID = "Bigram_5LFBigrams_Stdev";
            return new LabeledValue(
                RESOURCE_ID,
                avg.ToString("F", culture.NumberFormat)
            );
        }




        #endregion
    }
}