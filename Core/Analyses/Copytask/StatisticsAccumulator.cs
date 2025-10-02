using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using InputLog.Core.Analyses.Copytask.Elements;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Copytask
{
    /// <summary>
    /// This class runs the required calculations for the copyTask given 
    /// the list of BigramContexts to work on.
    /// </summary>
    public class StatisticsAccumulator : FilteredAccumulator<BigramContext>
    {
        #region Simple Fields
        /// <summary>
        /// Nr of targetted bigrams encountered.
        /// </summary>
        public int CountTargetted
        {
            get;
            private set;
        }

        /// <summary>
        /// Nr of non-targetted bigrams encountered.
        /// </summary>
        public int CountNotTargetted
        {
            get;
            private set;
        }

        /// <summary>
        /// Sum of the pause times of all targetted bigrams.
        /// </summary>
        private double SumOfTargetted;

        /// <summary>
        /// Sum of the log pause times of all targetted bigrams.
        /// </summary>
        private double LogSumOfTargetted;


        /// <summary>
        /// Sorted list of Pause Times. Keys and values are identical.
        /// </summary>
        private SortedList<double, double> PauseTimes;

        /// <summary>
        /// Sorted list of log transformed pause times. Key and values are identical.
        /// </summary>
        private SortedList<double, double> LogPauseTimes;

        /// <summary>
        /// Start time of the first bigram accumulated.
        /// </summary>
        private ulong StartTime;

        /// <summary>
        /// start time of the last bigram to be accumulated.
        /// </summary>
        private ulong EndTime;
        #endregion

        #region Derived Fields
        public double Mean
        {
            get
            {
                return this.SumOfTargetted / this.CountTargetted;
            }
        }

        public double StdDev
        {
            get;
            private set;
        }

        public double Median
        {
            get;
            private set;
        }

        public double LogMean
        {
            get
            {
                return LogSumOfTargetted / this.CountTargetted;
            }
        }

        public double TrimmedLogMean
        {
            get;
            private set;
        }

        public double CoefficientOfVariance
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of milliseconds in a minute.
        /// </summary>
        private int MILLISECONDS_IN_MINUTE = 60000;

        /// <summary>
        /// Characters per minute, calculated estimate based on 
        /// the mean pause time.
        /// </summary>
        public double CPM
        {
            get
            {
                return MILLISECONDS_IN_MINUTE / this.Mean;
            }
        }

        #endregion

        /// <summary>
        /// Create a new statistics accumulator class.
        /// </summary>
        public StatisticsAccumulator() { }

        /// <summary>
        /// Prepare the accumulator variables to accumulate 
        /// a list of values. Reset from any previous runs.
        /// </summary>
        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();

            this.PauseTimes = new SortedList<double, double>(new DuplicateKeyComparer());
            this.LogPauseTimes = new SortedList<double, double>(new DuplicateKeyComparer());
            this.CountNotTargetted = 0;
            this.CountTargetted = 0;
            this.EndTime = ulong.MinValue;
            this.StartTime = ulong.MaxValue;
            this.SumOfTargetted = 0;
            this.LogSumOfTargetted = 0;

            this.StdDev = 0;
            this.Median = 0;
            this.TrimmedLogMean = 0;
        }

        /// <summary>
        /// Calculate the values that can only be calculated after the values
        /// have been correctly accumulated.
        /// </summary>
        protected override void AfterAccumulate()
        {
            base.AfterAccumulate();

            List<double> pauseTimes = this.PauseTimes.Values.ToList();
            this.Median = MathExt.FindMedian(pauseTimes);
            this.StdDev = MathExt.StDevFromList(pauseTimes);

            List<double> logPauseTimes = this.LogPauseTimes.Values.ToList();
            var interval = MathExt.CalculateInterval(logPauseTimes);
            this.TrimmedLogMean = interval.Item2;
            this.CoefficientOfVariance = interval.Item4;

            this._clear();
        }

        /// <summary>
        /// Clears the lists of pauseTimes to free up the memory space.
        /// We don't need those lists anymore.
        /// </summary>
        private void _clear()
        {
            this.PauseTimes = null;
            this.LogPauseTimes = null;
        }

        /// <summary>
        /// Process a new item that's being added to the accumulator
        /// after having passed all filters etc.
        /// </summary>
        /// <param name="item">The item that's being processed.</param>
        protected override void ProcessNewItem(BigramContext item)
        {
            if (item.IsTargetBigram())
            {
                this.CountTargetted += 1;

                // Add pause times.
                double pauseTime = (double)item.PauseTime;
                this.SumOfTargetted += pauseTime;
                this.PauseTimes.Add(pauseTime, pauseTime);
                double logPauseTime = Math.Log(pauseTime);
                this.LogSumOfTargetted += logPauseTime;
                this.LogPauseTimes.Add(logPauseTime, logPauseTime);

                this.StartTime = Math.Min(this.StartTime, item.StartTime);
                this.EndTime = Math.Max(this.EndTime, item.EndTime);
            }
            else
            {
                this.CountNotTargetted += 1;
            }
        }

        /// <summary>
        /// Return a PrettyPrintStatistics instance that can take care of the
        /// pretty printing of the values of this Accumulator.
        /// </summary>
        /// <returns>A pretty print class that can create the output for this
        /// object.</returns>
        public virtual PrettyPrintStatistics GetPrettyPrinter()
        {
            return new PrettyPrintStatistics(this);
        }
    }

    /// <summary>
    /// Keeps timing information about the events being
    /// accumulated.
    /// </summary>
    public class TimedStatisticsAccumulator : StatisticsAccumulator
    {
        /// <summary>
        /// Start time of the bigram with the lowest start 
        /// time, accumulated in this accumulator.
        /// </summary>
        protected ulong _startTime;

        /// <summary>
        /// End time of the bigram with the highest start time
        /// accumulated in this accumulator.
        /// </summary>
        protected ulong _endTime;

        /// <summary>
        /// Execution time over all the bigrams accumulated 
        /// in the accumulator, in milliseconds.
        /// </summary>
        public ulong ExecutionTimeInMs
        {
            get
            {
                return this._endTime - this._startTime;
            }
        }

        /// <summary>
        /// Execution time over all the bigrams accumulated 
        /// in the accumulator, in seconds.
        /// </summary>
        public double ExecutionTimeInSeconds
        {
            get
            {
                return (double)this.ExecutionTimeInMs / 1000;
            }
        }

        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();
            this._startTime = ulong.MaxValue;
            this._endTime = ulong.MinValue;
        }

        protected override void ProcessNewItem(BigramContext item)
        {
            base.ProcessNewItem(item);

            this._startTime = Math.Min(this._startTime, item.StartTime);
            this._endTime = Math.Max(this._endTime, item.EndTime);
        }

    }


    /// <summary>
    /// Special statistics accumulator that calculates some extra stasistics over components.
    /// </summary>
    public class ComponentStatisticsAccumulator : TimedStatisticsAccumulator
    {
        /// <summary>
        /// The absolute, sustained CPM. The calculation is different from the 
        /// other CPM count. 
        /// </summary>
        public double AbsoluteCPM
        {
            get
            {
                return this.CountTargetted / this.ExecutionTimeInSeconds * SECONDS_IN_A_MINUTE;
            }
        }

        /// <summary>
        /// Name of the component we are calculating statistics over.
        /// This sort of statistics only works for an analysis of BigramContexts
        /// over one and the same component.
        /// </summary>
        private string _componentName;

        private const int SECONDS_IN_A_MINUTE = 60;

        /// <summary>
        /// The time limit set for the component. Set in seconds. Timelimit
        /// may be equal to the timelimit specified for the component in the copyTask structure.
        /// Or if no such timelimit is set it will be equal to the time taken
        /// to execute that component.
        /// </summary>
        //private double _timelimit;

        /// <summary>
        /// Whether the component has an intricinsic time limit set or not.
        /// So a component that doesn't have a timelimit set will still have a 
        /// value for _timelimit, but the value will just be equal to the time
        /// it took to execute the component. Otherwise the value will be equal
        /// to the timelimit specified for the component.
        /// </summary>
        //private bool _hasTimeLimit;

        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();
            this._componentName = String.Empty;
        }

        protected override void ProcessNewItem(BigramContext item)
        {
            // Checks that we are adding only valid items.
            Debug.Assert(_isValidItem(item));
            if (this._isValidItem(item))
            {
                base.ProcessNewItem(item);
            }

            // Keep timing information.
            //if (item.Component.HasTimeLimit && !this._hasTimeLimit)
            //{
            //    this._timelimit = item.Component.TimeLimit;
            //    this._hasTimeLimit = true;
            //}
            //else if (this._timelimit == ComponentStructure.NO_TIMELIMIT)
            //{
            //    this._timelimit = item.ComponentExecutionTime;
            //}
        }

        /// <summary>
        /// Checks whether an item being added is valid in the context of this
        /// StatisticsAccumulator that is specific for component statistics. 
        /// </summary>
        /// <param name="item">Item to be added to the statistics. However,
        /// only items of one and the same component can be added to a single
        /// ComponentStatisticsAccumulator</param>
        /// <returns>True if the item may be added to this accumulator, false if not.</returns>
        private bool _isValidItem(BigramContext item)
        {
            if (this._componentName == String.Empty)
            {
                this._componentName = item.Component.Title;
            }
            bool belongsToSameComponent = this._componentName == item.Component.Title;
            return belongsToSameComponent;
        }

        /// <summary>
        /// Return a PrettyPrintStatistics instance that can take care of the
        /// pretty printing of the values of this Accumulator.
        /// </summary>
        /// <returns>A pretty print class that can create the output for this
        /// object.</returns>
        public override PrettyPrintStatistics GetPrettyPrinter()
        {
            return new PrettyPrintComponentStats(this);
        }
    }

    public class CorrectnessAccumulator : Accumulator<StatisticsAccumulator>
    {
        #region Simple Fields
        /// <summary>
        /// Nr of targetted bigrams encountered.
        /// </summary>
        public List<int> CountTargetted
        {
            get;
            private set;
        }

        /// <summary>
        /// Nr of non-targetted bigrams encountered.
        /// </summary>
        public List<int> CountNotTargetted
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the correctness score per component, based on the counts of targetted and 
        /// non-targetted bigrams in that component
        /// </summary>
        private List<double> CorrectnessPerComponent
        {
            get
            {
                var correctness = new List<double>();
                foreach (var entry in CountTargetted.Zip(CountNotTargetted, (first, second) => new Pair<int,int>(first, second)))
                {
                    var targetted = entry.First;
                    var notTargetted = entry.Second;
                    var perc = (double)targetted/Math.Max((targetted + notTargetted), 1);
                    correctness.Add(perc);
                }
                return correctness;
            }
        }

        /// <summary>
        /// Accross all components the count of all targetted bigrams
        /// </summary>
        public int AggregatedCountTargetted { get; private set; }

        /// <summary>
        /// Accross all components the count of all non-targetted bigrams
        /// </summary>
        public int AggregatedCountNotTargetted { get; private set; }

        /// <summary>
        /// Accross all components an aggregated correctness
        /// </summary>
        public double AggregatedCorrectness { get; private set; }

        /// <summary>
        /// A mean of the correctness scores per component
        /// </summary>
        public double MeanCorrectness { get; private set; }

        /// <summary>
        /// A median of the correctness scores of the separate components
        /// </summary>
        public double MedianCorrectness { get; private set; }

        /// <summary>
        /// Standard deviation on the correctness scores of the separate components
        /// </summary>
        public double StdevCorrectness { get; private set; }

        /// <summary>
        /// Minimum correctness score achieved accross the components
        /// </summary>
        public double MinCorrectness { get; private set; }

        /// <summary>
        /// Maximum correctness score achieved accross the components
        /// </summary>
        public double MaxCorrectness { get; private set; }
        #endregion

        /// <summary>
        /// Create a new correctness accumulator class.
        /// </summary>
        public CorrectnessAccumulator() { }

        /// <summary>
        /// Prepare the accumulator variables to accumulate 
        /// a list of values. Reset from any previous runs.
        /// </summary>
        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();

            this.CountNotTargetted = new List<int>();
            this.CountTargetted = new List<int>();
        }

        /// <summary>
        /// Calculate the values that can only be calculated after the values
        /// have been correctly accumulated.
        /// </summary>
        protected override void AfterAccumulate()
        {
            base.AfterAccumulate();

            var scores = CorrectnessPerComponent;
            scores.Sort();
            this.MaxCorrectness = scores.Max();
            this.MinCorrectness = scores.Min();
            this.MedianCorrectness = MathExt.FindMedian(scores);
            this.MeanCorrectness = MathExt.MeanFromList(scores);
            this.StdevCorrectness = MathExt.StDevFromList(scores);

            // Calculating the aggregated correctness
            this.AggregatedCountTargetted = CountTargetted.Sum();
            this.AggregatedCountNotTargetted = CountNotTargetted.Sum();
            this.AggregatedCorrectness = 
                ((double)this.AggregatedCountTargetted)/
                Math.Max(1.0, this.AggregatedCountTargetted + this.AggregatedCountNotTargetted);
        }

        /// <summary>
        /// Clears the lists of pauseTimes to free up the memory space.
        /// We don't need those lists anymore.
        /// </summary>
        private void _clear()
        {
            this.CountTargetted = null;
            this.CountNotTargetted = null;
        }

        /// <summary>
        /// Process a new item that's being added to the accumulator
        /// after having passed all filters etc.
        /// </summary>
        /// <param name="item">The item that's being processed.</param>
        protected override void ProcessNewItem(StatisticsAccumulator item)
        {
            this.CountTargetted.Add(item.CountTargetted);
            this.CountNotTargetted.Add(item.CountNotTargetted);
        }

        /// <summary>
        /// This pretty printer method is incompatible with this class and can not be used
        /// please use the GetPrettyPrinterCorrectness() method instead
        /// </summary>
        /// <throws>NotImplementedException</throws>
        public virtual PrettyPrintStatistics GetPrettyPrinter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a pretty printer for this accumulator to provide a consistent output
        /// of the different values
        /// </summary>
        /// <returns>Pretty printer for the output</returns>
        public PrettyPrintCorrectnessStatistics GetPrettyPrinterCorrectness()
        {
            return new PrettyPrintCorrectnessStatistics(this);
        }
        
    }

    /// <summary>
    /// Allows the comparing of duplicate keys for the sorted list.
    /// Equal keys are treated as being greater. Because there's no need
    /// to remove keys form the list we can use this comparer.
    /// </summary>
    internal class DuplicateKeyComparer : IComparer<double>
    {
        public int Compare(double d1, double d2)
        {
            if (d1 < d2)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
