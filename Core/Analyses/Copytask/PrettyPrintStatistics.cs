using System;
using System.Globalization;

namespace InputLog.Core.Analyses.Copytask
{
    /// <summary>
    /// This class Encapsulates the StatisticsAccumulator and 
    /// makes sure that all the output from that class is consistently
    /// and correctly converted to readable output values.
    /// </summary>
    public class PrettyPrintStatistics
    {
        /// <summary>
        /// The statistics instance we are wrapping
        /// </summary>
        protected StatisticsAccumulator statistics;

        /// <summary>
        /// Number formatting to be used.
        /// </summary>
        protected NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

        public string CountTargeted
        {
            get { return this.statistics.CountTargetted.ToString(); }
        }

        public string CountNotTargeted
        {
            get { return this.statistics.CountNotTargetted.ToString(); }
        }

        public string Mean
        {
            get { return this.statistics.Mean.ToString("F", Nfi); }
        }

        public string StdDev
        {
            get { return this.statistics.StdDev.ToString("F", Nfi); }
        }

        public string Median
        {
            get { return this.statistics.Median.ToString("F0", Nfi); }
        }

        /// <summary>
        /// First take anti log before outputting the value.
        /// </summary>
        public string TrimmedLogMean
        {
            get
            {
                double antiLogMean = Math.Exp(this.statistics.TrimmedLogMean);
                return antiLogMean.ToString("F", Nfi);
            }
        }

        public string CoefficientOfVariance
        {
            get
            {
                // Is reported in %
                double cov = this.statistics.CoefficientOfVariance * 100;
                return cov.ToString("F", Nfi) + " %";
            }
        }

        public string CPM
        {
            get { return this.statistics.CPM.ToString("F0", Nfi); }
        }

        private static string[] _keys = new string[] {
            "Count (targeted)",
            "Count (not targeted)",
            "Mean IKI",
            "StdDev",
            "Median",
            "LogMean (trimmed)",
            "Coef. of Variation",
            "CPM"
        };

        /// <summary>
        /// Create a new instance of a pretty print statistics class that
        /// wraps around the given statistics class to provide clean, consistent
        /// output.
        /// </summary>
        /// <param name="statistics">The statistics</param>
        public PrettyPrintStatistics(StatisticsAccumulator statistics)
        {
            this.statistics = statistics;
            this.Nfi.NumberDecimalDigits = 1;
            this.Nfi.NumberGroupSeparator = " ";
        }

        /// <summary>
        /// Return the names of each element that can be requested in the 
        /// pretty print statistics class. For example the property LogMean 
        /// will have a key named "LogMean", etc.
        /// The order maintained in the keys is the same order the values will
        /// be returned in. So the first element of the key item will correspond
        /// with the name of the value of the first property returned in the values
        /// array.
        /// </summary>
        /// <returns>An array of keys in a specific order.</returns>
        public virtual string[] Keys()
        {
            return _keys;
        }

        /// <summary>
        /// Return the values available in the PrettyPrintStatistics, in the same
        /// order as the keys have been returned.
        /// </summary>
        /// <returns>The values of all items in this instance.</returns>
        public virtual string[] Values()
        {
            return new[] {
                this.CountTargeted,
                this.CountNotTargeted,
                this.Mean,
                this.StdDev,
                this.Median,
                this.TrimmedLogMean,
                this.CoefficientOfVariance,
                this.CPM
            };
        }
    }

    /// <summary>
    /// Extends the PrettyPrintStatistics by adding some extra statistics that
    /// can be printed, specifically for components.
    /// </summary>
    public class PrettyPrintComponentStats : PrettyPrintStatistics
    {
        /// <summary>
        /// Return the absolute CPM for the component. An empty value, or 'no-value' 
        /// will be returned as a dash '-'.
        /// </summary>
        public string AbsoluteCPM
        {
            get
            {
                double cpm = ((ComponentStatisticsAccumulator)this.statistics).AbsoluteCPM;
                return cpm.ToString("F0", this.Nfi);
            }
        }


        /// <summary>
        /// Create an instance of the PrettyPrintComponentStats class 
        /// by giving it a reference to a componentStatisticsAccumulator.
        /// </summary>
        /// <param name="stats">The stats class of the component.</param>
        public PrettyPrintComponentStats(ComponentStatisticsAccumulator stats) :
            base(stats) { }


        /// <summary>
        /// Return the keys for all the outputvalues that this 
        /// pretty printer can return.
        /// </summary>
        /// <returns>Array containing the labels for each value that 
        /// will be returned by this array, in the same order that the values
        /// will be returned.</returns>
        public override string[] Keys()
        {
            string[] baseKeys = base.Keys();
            string[] newKeys = new string[baseKeys.Length + 1];
            Array.Copy(baseKeys, newKeys, baseKeys.Length);
            newKeys[newKeys.Length - 1] = "Absolute CPM";
            return newKeys;
        }

        /// <summary>
        /// Return the values available in the PrettyPrintComponentStats, in the same
        /// order as the keys have been returned.
        /// </summary>
        /// <returns>The values of all items in this instance.</returns>
        public override string[] Values()
        {
            string[] baseValues = base.Values();
            string[] newValues = new string[baseValues.Length + 1];
            Array.Copy(baseValues, newValues, baseValues.Length);
            newValues[newValues.Length - 1] = this.AbsoluteCPM;
            return newValues;
        }
    }

    /// <summary>
    /// Extends the PrettyPrintStatistics and changes the keys & values returned so they
    /// suit the statistics for the correctness. 
    /// This class correctly converts to readable output values.
    /// </summary>
    /// <remarks>If you add any new output variables to this class, be sure
    /// to update the NR_OF_ITEMS constant as well to represent the number of
    /// elements that will be returned from this class.</remarks>
    public class PrettyPrintCorrectnessStatistics
    {
        /// <summary>
        /// The statistics instance we are wrapping
        /// </summary>
        private readonly CorrectnessAccumulator statistics;

        /// <summary>
        /// Number formatting to be used.
        /// </summary>
        protected readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

        public string AggregatedCountTargeted => this.statistics.AggregatedCountTargetted.ToString();
        public string AggregatedCountNotTargeted => this.statistics.AggregatedCountNotTargetted.ToString();
        public string AggregatedCorrectness => (this.statistics.AggregatedCorrectness * 100).ToString("F1", Nfi) + "%";
        public string Mean => (this.statistics.MeanCorrectness * 100).ToString("F1", Nfi) + "%";
        public string StdDev => (this.statistics.StdevCorrectness * 100).ToString("F1", Nfi);
        public string Median => (this.statistics.MedianCorrectness * 100).ToString("F1", Nfi) + "%";
        public string Min => (this.statistics.MinCorrectness * 100).ToString("F1", Nfi) + "%";
        public string Max => (this.statistics.MaxCorrectness * 100).ToString("F1", Nfi) + "%";

        private static readonly string[] _keys = {
            "Aggr Targetted",
            "Aggr Not Targetted",
            "Aggr Correctness",
            "Mean",
            "Stdev",
            "Median",
            "Min",
            "Max"
        };

        /// <summary>
        /// Create a new instance of a pretty print statistics class that
        /// wraps around the given accumulator class to provide clean, consistent
        /// output.
        /// </summary>
        /// <param name="statistics">The statistics</param>
        public PrettyPrintCorrectnessStatistics(CorrectnessAccumulator statistics)
        {
            this.statistics = statistics;
            this.Nfi.NumberDecimalDigits = 1;
            this.Nfi.NumberGroupSeparator = " ";
        }

        /// <summary>
        /// Return the names of each element that can be requested in the 
        /// pretty print statistics class. For example the property LogMean 
        /// will have a key named "LogMean", etc.
        /// The order maintained in the keys is the same order the values will
        /// be returned in. So the first element of the key item will correspond
        /// with the name of the value of the first property returned in the values
        /// array.
        /// </summary>
        /// <returns>An array of keys in a specific order.</returns>
        public virtual string[] Keys()
        {
            return _keys;
        }

        /// <summary>
        /// Return the values available in the PrettyPrintStatistics, in the same
        /// order as the keys have been returned.
        /// </summary>
        /// <returns>The values of all items in this instance.</returns>
        public virtual string[] Values()
        {
            return new[] {
                this.AggregatedCountTargeted,
                this.AggregatedCountNotTargeted,
                this.AggregatedCorrectness,
                this.Mean,
                this.StdDev,
                this.Median,
                this.Min,
                this.Max
            };
        }
    }

    public class PrettyPrintCorrectnessEntry
    {
        private readonly CopytaskAnalysisSummary.CorrectnessEntry Entry;
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

        public string CountTargeted => this.Entry.CountTargetted.ToString();
        public string CountNotTargeted => this.Entry.CountNotTargetted.ToString();
        public string Correctness => (this.Entry.CorrectnessScore * 100).ToString("F1", Nfi) + "%";

        public PrettyPrintCorrectnessEntry(CopytaskAnalysisSummary.CorrectnessEntry entry)
        {
            this.Entry = entry;
        }
    }


}
