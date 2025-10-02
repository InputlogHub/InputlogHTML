using System.Collections.Generic;

namespace InputLog.Core.Analyses.Pause
{
    /// <summary>
    ///     Summary of the Pause analysis.
    ///     Contains general pause information info, pause info per pausetype (pauselocation)
    ///     and pause info per interval.
    /// </summary>
    public class SingularPauseAnalysisSummary : AbstractAnalysisSummary
    {
        
        /// <summary>
        ///     Constructs a PauseAnalysisSummary.
        /// </summary>
        public SingularPauseAnalysisSummary()
        {
            GeneralInformation = new GeneralInformationType();
            IntervalInfo = new Dictionary<ulong, IntervalStats>();
            BasePauseTypes = new Dictionary<PauseLocation, BaseStats>();
            CombinedTypes = new Dictionary<PauseLocation, CombinedStats>();
            BetweenBeforeTypes = new Dictionary<PauseLocation, BetweenBeforeStats>();
            BetweenAfterTypes = new Dictionary<PauseLocation, BetweenAfterStats>();
            MiscellaneousTypes = new Dictionary<PauseLocation, MiscellaneousStats>();
        }

        #region Classes

        /// <summary>
        ///     The general pause information (gathered from all events).
        /// </summary>
        public class GeneralInformationType
        {
            /// <summary>
            ///     The type of pause analysis that was performed (using a fixed number of intervals,
            ///     fixed interval length, ...).
            /// </summary>
            public PauseAnalysis.IntervalType AnalysisType;
            /// <summary>
            ///     95% confidence interval of the mean:
            ///     Low CI95L = mean - 1.96 * stdev ; high CI95H = mean + 1.96 * stdev
            /// </summary>
            public double CI95H;
            /// <summary>
            ///     95% confidence interval of the mean:
            ///     Low CI95L = mean - 1.96 * stdev ; high CI95H = mean + 1.96 * stdev
            /// </summary>
            public double CI95L;
            /// <summary>
            ///     The id of the first keyboard event.
            /// </summary>
            public int FirstKeyPressId;
            /// <summary>
            ///     The start time of the first keyboard event.
            /// </summary>
            public ulong FirstStartKeyTime;
            /// <summary>
            ///     The size of the intervals (in msec).
            /// </summary>
            public ulong IntervalLength;
            /// <summary>
            /// The smallest element form a list of pause times.
            /// </summary>
            public ulong MinPause;
            /// <summary>
            /// The largest element form a list of pause times.
            /// </summary>
            public ulong MaxPause;
            /// <summary>
            ///     The mean pause time ( = total pause time/number of pauses).
            /// </summary>
            public double MeanPauseTime;
            /// <summary>
            ///     The geometric mean of the pause time.
            /// </summary>
            public double GeoMeanPauseTime;
            /// <summary>
            /// Coefficient of Variation (CoefVar), the ratio of standard deviation to the mean.  
            /// CoefVar = sqrt(exp(SD^2)-1)
            /// </summary>
            public double CoefVar;
            /// <summary>
            ///     The median of an array of pause times: from a sorted array it picks the
            ///     middle element when the number of elements is odd or the average of
            ///     the two middles when the number is even.
            /// </summary>
            public double MedianPauseTime;
            /// <summary>
            ///     The number of intervals.
            /// </summary>
            public ulong NumberOfIntervals;
            /// <summary>
            ///     Pause time Standard Deviation (compared to MeanPauseTime).
            /// </summary>
            public double StDev;
            /// <summary>
            ///     The total number of pauses (larger than the threshold) that occured in the log.
            /// </summary>
            public ulong TotalNumberOfPauses;
            /// <summary>
            ///     Sum of all pauses that are larger than the pause threshold.
            /// </summary>
            public ulong TotalPauseTime;
            /// <summary>
            ///     The total process time.
            /// </summary>
            public ulong TotalProcessTime;
            /// <summary>
            /// Ratio total pause time on total process time
            /// </summary>
            public double PauseTimeProportion;
            /// <summary>
            /// The total process time minus the total pause time.
            /// </summary>
            public ulong TotalWritingTime;
            /// <summary>
            /// P-Burst statistics
            /// </summary>
            public ulong NumberOfSegments;
            public ulong AvgProcessTime;
            public double StandardDeviation;
            public double AvgProcessChars;
            public double StandardDeviationChars;
            public double MedianProcessTime;
            public double MedianProcessChars;
            public double PBurstsPerMinute;
            public ulong PBurstTreshold;
        }

        /// <summary>
        ///     Basic statistics
        /// </summary>
        public class BaseStats
        {
            /// <summary>
            ///     95% confidence interval of the mean calculated with the log of the pause values,
            ///     because the data are mostly left skewed.
            ///     CI95H = mean + 1.96 * stdev (high)
            /// </summary>
            public readonly double CI95H;
            /// <summary>
            ///     95% confidence interval of the mean calculated with the log of the pause values,
            ///     because the data are mostly left skewed.
            ///     CI95L = mean - 1.96 * stdev (low) 
            /// </summary>
            public readonly double CI95L;
            /// <summary>
            /// Coefficient of Variation (CoefVar), the ratio of standard deviation to the mean.  
            /// CoefVar = sqrt(exp(SD^2)-1)
            /// </summary>
            public readonly double CoefVar;
            /// <summary>
            /// The largest pause length found in a list.
            /// </summary>
            public readonly ulong MaxPause;
            /// <summary>
            ///     The arithmetic mean
            /// </summary>
            public readonly double MeanPauseTime;
            /// <summary>
            ///     The geometric mean
            /// </summary>
            public readonly double GeoMeanPauseTime;
            /// <summary>
            ///     The median of an array of pause times: the middle
            ///     element of a sorted array of pause times.
            /// </summary>
            public readonly double MedianPauseTime;
            /// <summary>
            /// The smallest pause length found in a list.
            /// </summary>
            public readonly ulong MinPause;
            /// <summary>
            ///     Number of pauses that occured.
            /// </summary>
            public readonly ulong NumberOfPauses;
            /// <summary>
            ///     Standard Deviation
            /// </summary>
            public readonly double StDev;

            /// <summary>
            ///     First quartile
            /// </summary>
            public readonly double FirstQuartile;

            /// <summary>
            ///     Third Quartile
            /// </summary>
            public readonly double ThirdQuartile;

            /// <summary>
            ///     Minimum value on a boxplot
            /// </summary>
            public readonly double BoxPlotMin;

            /// <summary>
            ///     Maximum value to be shown on a boxplot
            /// </summary>
            public readonly double BoxPlotMax;

            /// <summary>
            ///     Constructs a BaseStats.
            /// </summary>
            /// <param name="numberOfPauses">Number of pauses that occured.</param>
            /// <param name="meanPauseTime"></param>
            /// <param name="ci95H"></param>
            /// <param name="maxPause"></param>
            /// <param name="coefVar"></param>
            /// <param name="medianPauseTime"></param>
            /// <param name="stDev"></param>
            /// <param name="geoMeanPauseTime"></param>
            /// <param name="ci95L"></param>
            /// <param name="minPause"></param>
            /// <param name="firstQ"></param>
            /// <param name="thirdQ"></param>
            /// <param name="boxplotMin"></param>
            /// <param name="boxplotMax"></param>
            public BaseStats(ulong numberOfPauses, double meanPauseTime, double geoMeanPauseTime, double ci95L,
                double ci95H, ulong minPause, ulong maxPause, double coefVar, double medianPauseTime, double stDev,
                double firstQ, double thirdQ, double boxplotMin, double boxplotMax)
            {
                NumberOfPauses = numberOfPauses;
                MeanPauseTime = meanPauseTime;
                GeoMeanPauseTime = geoMeanPauseTime;
                CI95L = ci95L;
                CI95H = ci95H;
                MinPause = minPause;
                MaxPause = maxPause;
                CoefVar = coefVar;
                MedianPauseTime = medianPauseTime;
                StDev = stDev;
                FirstQuartile = firstQ;
                ThirdQuartile = thirdQ;
                BoxPlotMin = boxplotMin;
                BoxPlotMax = boxplotMax;
            }
        }

        /// <summary>
        ///  Combined Statistics, stores statistics for the Combination Pause,
        ///  the transition between BEFORE-AFTER on word, sentence and paragraph level.
        /// </summary>
        public class CombinedStats
        {
            public readonly double CombinedCi95H;
            public readonly double CombinedCi95L;
            public readonly ulong CombinedMaxPause;
            public readonly double CombinedMean;
            public readonly double CombinedGeoMean;
            public readonly double CombinedCoefVar;
            public readonly double CombinedMedian;
            public readonly double CombinedFirstQuartile;
            public readonly double CombinedThirdQuartile;
            public readonly double CombinedBoxPlotMin;
            public readonly double CombinedBoxPlotMax;
            public readonly ulong CombinedMinPause;
            public readonly ulong CombinedPauses;
            public readonly double CombinedStDev;

            public CombinedStats(ulong combinedPauses, double combinedMean, double combinedGeoMean, 
                double combinedCi95L, double combinedCi95H, ulong combinedMinPause, ulong combinedMaxPause, 
                double combinedCoefVar, double combinedMedian, double combinedStDev, 
                double combinedFirstQuartile, double combinedThirdQuartile, double combinedPBMin, double combinedPBMax)
            {
                CombinedPauses = combinedPauses;
                CombinedMean = combinedMean;
                CombinedGeoMean = combinedGeoMean;
                CombinedCi95L = combinedCi95L;
                CombinedCi95H = combinedCi95H;
                CombinedMinPause = combinedMinPause;
                CombinedMaxPause = combinedMaxPause;
                CombinedCoefVar = combinedCoefVar;
                CombinedMedian = combinedMedian;
                CombinedStDev = combinedStDev;
                CombinedFirstQuartile = combinedFirstQuartile;
                CombinedThirdQuartile = combinedThirdQuartile;
                CombinedBoxPlotMax = combinedPBMax;
                CombinedBoxPlotMin = combinedPBMin;
            }
        }

        /// <summary>
        ///     BEFORE Statistics, stores statistics for a BEFORE type of pause.
        /// </summary>
        public class BetweenBeforeStats
        {
            public readonly double BeforeCi95H;
            public readonly double BeforeCi95L;
            public readonly ulong BeforeMaxPause;
            public readonly double BeforeMean;
            public readonly double BeforeGeoMean;
            public readonly double BeforeMedian;
            public readonly double BeforeCoefVar;
            public readonly ulong BeforeMinPause;
            public readonly double BeforeStDev;
            public readonly ulong BetweenBeforePauses;

            public BetweenBeforeStats(ulong betweenBeforePauses, double beforeMean, double beforeGeoMean,
                double beforeCi95L, double beforeCi95H, ulong beforeMinPause, ulong beforeMaxPause,
                double beforeCoefVar, double beforeMedian, double beforeStDev)
            {
                BetweenBeforePauses = betweenBeforePauses;
                BeforeMean = beforeMean;
                BeforeGeoMean = beforeGeoMean;
                BeforeCi95L = beforeCi95L;
                BeforeCi95H = beforeCi95H;
                BeforeMinPause = beforeMinPause;
                BeforeMaxPause = beforeMaxPause;
                BeforeCoefVar = beforeCoefVar;
                BeforeMedian = beforeMedian;
                BeforeStDev = beforeStDev;
            }
        }

        /// <summary>
        ///    AFTER Statistics, stores statistics for a AFTER type of pause.
        /// </summary>
        public class BetweenAfterStats
        {
            public readonly double AfterCi95H;
            public readonly double AfterCi95L;
            public readonly ulong AfterMaxPause;
            public readonly double AfterMean;
            public readonly double AfterGeoMean;
            public readonly double AfterMedian;
            public readonly double AfterCoefVar;
            public readonly ulong AfterMinPause;
            public readonly double AfterStDev;          
            public readonly ulong BetweenAfterPauses;

            public BetweenAfterStats(ulong betweenAfterPauses, double afterMean, double afterGeoMean,
                double afterCi95L, double afterCi95H, ulong afterMinPause, ulong afterMaxPause,
                double afterCoefVar, double afterMedian, double afterStDev)
            {
                BetweenAfterPauses = betweenAfterPauses;
                AfterMean = afterMean;
                AfterGeoMean = afterGeoMean;
                AfterCi95L = afterCi95L;
                AfterCi95H = afterCi95H;
                AfterMinPause = afterMinPause;
                AfterMaxPause = afterMaxPause;
                AfterCoefVar = afterCoefVar;
                AfterMedian = afterMedian;
                AfterStDev = afterStDev;
            }
        }

        /// <summary>
        ///     IntervalStats, stores statistics for a certain interval period.
        /// </summary>
        public class IntervalStats : BaseStats
        {
            /// <summary>
            ///     IntervalSegment gives a common name to the same interval segments.
            ///     Previously interval names included the start time of each interval segement.
            ///     This prevented the grouping of the same interval segements of different participants,
            ///     because the different individual start times created different segment names.
            /// </summary>
            public string IntervalSegment;

            /// <summary>
            ///     StartTime of the interval.
            /// </summary>
            public ulong IntervalStart;

            /// <summary>
            ///     Constructs a IntervalStat.
            /// </summary>
            /// <param name="intervalSegment">A sequential number for this interval segement.</param>
            /// <param name="intervalStart">StartTime of the interval.</param>
            /// <param name="numberOfPauses">Number of pauses that occured.</param>
            /// <param name="meanPauseTime">The arithmetic mean of the pause times in this interval.</param>
            /// <param name="geoMeanPauseTime">The geometric mean</param>
            /// <param name="ci95L">Low value of the 95% confidence interval around the mean.</param>
            /// <param name="ci95H">High value of the 95% confidence interval around the mean.</param>
            /// <param name="maxPause"></param>
            /// <param name="coefVar">Coefficient of Variation (CoefVar), 
            /// the ratio of standard deviation to the mean. CoefVar = sqrt(exp(SD^2)-1)</param>
            /// <param name="medianPauseTime">
            ///     The median of an array of pause times: the middle
            ///     element of a sorted array of pause times.
            /// </param>
            /// <param name="stDev">The standard dev of the pause times in this interval.</param>
            /// <param name="minPause"></param>
            public IntervalStats(string intervalSegment, ulong intervalStart, ulong numberOfPauses,
                double meanPauseTime, double geoMeanPauseTime, double ci95L, double ci95H, ulong minPause,
                ulong maxPause, double coefVar, double medianPauseTime, double stDev) :
                    base(numberOfPauses, meanPauseTime, geoMeanPauseTime,ci95L, ci95H, minPause, 
                maxPause, coefVar, medianPauseTime, stDev, 0, 0, 0, 0)
            {
                IntervalSegment = intervalSegment;
                IntervalStart = intervalStart;
            }
        }

        /// <summary>
        ///     Pauses of minor importance: intitial, end pauses, revision, transition, combination key, and unknown
        /// </summary>
        public class MiscellaneousStats : BaseStats
        {
            public MiscellaneousStats(ulong numberOfPauses, double meanPauseTime, double geoMmeanPauseTime, 
                double ci95L, double ci95H, ulong minPause, ulong maxPause, double coefVar, 
                double medianPauseTime, double stDev)
                : base(numberOfPauses, meanPauseTime, geoMmeanPauseTime, ci95L, ci95H, minPause, maxPause,
                coefVar, medianPauseTime, stDev, 0, 0, 0, 0)
            {}
        }

        #endregion

        #region Fields

        /// <summary>
        ///     The general information that belongs to this pauseAnalysisSummary.
        /// </summary>
        public GeneralInformationType GeneralInformation { private set; get; }

        /// <summary>
        ///     Mapping basic pause types (within words) to their statistics
        /// </summary>
        public IDictionary<PauseLocation, BaseStats> BasePauseTypes { private set; get; }

        /// <summary>
        ///     Mapping pauses of minor importance to their statistics.
        /// </summary>
        public IDictionary<PauseLocation, MiscellaneousStats> MiscellaneousTypes { private set; get; }

        /// <summary>
        ///     Dictionary mapping the Combined Words, Sentences, Paragraphs pause types to their statistics.
        /// </summary>
        public IDictionary<PauseLocation, CombinedStats> CombinedTypes { private set; get; }

        /// <summary>
        ///     Dictionary mapping the BEFORE Words, Sentences, Paragraphs pause types to their statistics.
        /// </summary>
        public IDictionary<PauseLocation, BetweenBeforeStats> BetweenBeforeTypes { private set; get; }

        /// <summary>
        ///     Dictionary mapping the AFTER Words, Sentences, Paragraph pause types to their statistics.
        /// </summary>
        public IDictionary<PauseLocation, BetweenAfterStats> BetweenAfterTypes { private set; get; }

        /// <summary>
        ///     Dictionary mapping the different intervals to their statistics.
        /// </summary>
        public IDictionary<ulong, IntervalStats> IntervalInfo { private set; get; }

        #endregion
    }
}
