using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.Analyses.Copytask.Elements
{
    /// <summary>
    /// This class keeps track of a bigram within its context within the 
    /// copyTask. This allows us to add extra information to a bigram, such
    /// as pause information between events, whether the bigram is a targetted
    /// bigram or not, etc. This information can not be stored in the Bigrams.Bigram class
    /// because it has only one instance for every bigram, with information that is 
    /// shared across all those instances.
    /// </summary>
    public class BigramContext: InputLog.Core.IO.CSV.CSVItem
    {
        /// <summary>
        /// The bigram containing the shared information about the bigram such
        /// as frequency, adjacency, etc...
        /// </summary>
        public Bigrams.Bigram Bigram
        {
            get;
            private set;
        }

        /// <summary>
        /// The trial number of this bigram. If it's a bigram typed on 
        /// a sescond trial, trial will be 2.
        /// Counting starts from 1, not 0.
        /// </summary>
        public int Trial
        {
            get;
            private set;
        }

        /// <summary>
        /// The pause between the starting time of the two characters
        /// in the bigram.
        /// </summary>
        public ulong PauseTime
        {
            get;
            set;
        }

        /// <summary>
        /// Start time of the bigram. (Start time of the
        /// first character of the bigram.)
        /// </summary>
        public ulong StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// End time of the bigram is equal to the start time 
        /// of the second character in the bigram.
        /// </summary>
        public ulong EndTime
        {
            get
            {
                return this.StartTime + this.PauseTime;
            }
        }


        /// <summary>
        /// The component this Bigram belongs to. This contains all the information about
        /// the component. Timing information, repetitions, targeted bigrams, etc...
        /// </summary>
        public ComponentStructure Component
        {
            get;
            private set;
        }

        #region ComponentExecution Timing information fields
        /// <summary>
        /// The start time of the execution of the component
        /// that this bigram is a part of.
        /// </summary>
        public ulong ComponentStartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The end time of the execution of the component
        /// that this bigram is a part of.
        /// </summary>
        public ulong ComponentEndTime
        {
            get;
            private set;
        }

        /// <summary>
        /// How long the execution of the entire component, to completion, 
        /// took. Specified in seconds.
        /// </summary>
        public double ComponentExecutionTime
        {
            get
            {
                double difference = this.ComponentEndTime - this.ComponentStartTime;
                Debug.Assert(difference >= 0);

                // both endTime and startTime are in ms, we need them in seconds:
                return difference / 1000;
            }
        }
        #endregion


        /// <summary>
        /// Make a new BigramContext. Using this constructor assumes the component is a 
        /// non-repetitive component without different trials.
        /// </summary>
        /// <param name="bigram">Bigram characteristics</param>
        /// <param name="component">The component this bigram belongs too.</param>
        public BigramContext(Bigrams.Bigram bigram, 
            ComponentStructure component
        )
        {
            Debug.Assert(component != null);

            this.Bigram = bigram;
            this.Component = component;

            this.Trial = 1;
        }

        /// <summary>
        /// Make a new BigramContext. This constructor assumes the component is a repetitive
        /// component if the parameter is not changed. You may assign the trial to the 
        /// bigramcontext. Note however, that a trial may only be larger than 1 if 
        /// the bigram is also part of a repetitive component.
        /// </summary>
        /// <param name="bigram">Shared bigram characteristics</param>
        /// <param name="component">The component this bigram belongs too.</param>
        /// <param name="trial">Trial number this bigram is part of.</param>
        public BigramContext(Bigrams.Bigram bigram, 
            ComponentStructure component,
            int trial
        )
        {
            Debug.Assert(component != null);
            Debug.Assert(trial >= 1);
            Debug.Assert((trial == 1) || component.IsRepetitive || component.IsUnlimited);

            this.Bigram = bigram;
            this.Component = component;

            this.Trial = trial;
        }

        /// <summary>
        /// Returns whether or not this bigram was a bigram targeted by
        /// the component it appears in. if it is not a targeted bigram it
        /// is safe to assume that the user made a mistake inputting the
        /// target string of the component.
        /// </summary>
        /// <returns>True if this bigram is a target bigram for the component, 
        /// false if not.</returns>
        public bool IsTargetBigram()
        {
            return this.Component.IsTargetedBigram(this.Bigram.Value);
        }

        /// <summary>
        /// The window of time in which the entire component that this
        /// Bigram is a part of was executed. The start/end time are bound
        /// by the ComponentExecution.
        /// </summary>
        /// <param name="startTime">The beginning time of the execution the component.</param>
        /// <param name="endTime">The end time of the execution of the component.</param>
        public void SetExecutionWindow(ulong startTime, ulong endTime)
        {
            this.ComponentStartTime = startTime;
            this.ComponentEndTime = endTime;
        }

        /// <summary>
        /// Returns whether the current bigram should be filtered out or not when
        /// bigrams within the first x-percentage and last x-percentage of the 
        /// execution time are ignored. So, when percentage equals 10 all bigrams
        /// formed within the first 10% and the last 10% of the components execution
        /// will NOT be in the 'TimeFilteredRange'. All elements within the first 10% 
        /// and the last 10% WILL be in the 'TimeFilteredRange'.
        /// </summary>
        /// <param name="percentage">The percentage of items to filter from the front
        /// and from the back of the time range. Percentage must be a value > 0, and 
        /// smaller than or equal to 0.5 (which means all items will be within the 
        /// time filtered range).</param>
        /// <returns>True if the bigram is not within the specfied percentage time% 
        /// of items to be ignored, and false if the bigram is within the percentage time%
        /// elements as specified by the percentage parameter </returns>
        public bool IsWithinTimeFilteredRange(double percentage)
        {
            Debug.Assert(percentage > 0 && percentage <= 0.5);

            // offset equal to the %, in ms, is the % of the execution time in ms.
            double offset = (this.ComponentEndTime - this.ComponentStartTime) * percentage;

            double lowerbound = this.ComponentStartTime + offset;
            double upperbound = this.ComponentEndTime - offset;

            if (this.StartTime < lowerbound)
            {
                return false;
            }
            else if (this.StartTime > upperbound)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Implementation of get delegates for CSV serialization
        protected override void InitGetDelegates()
        {
            this._getDelegates.Add("component", this.GetComponent);
            this._getDelegates.Add("com_trial", this.GetTrial);
            this._getDelegates.Add("com_is_practice", this.GetComponentIsPractice);
            this._getDelegates.Add("com_is_synthesized", this.GetComponentIsSynthesized);
            this._getDelegates.Add("com_is_timed", this.GetComponentIsTimed);
            this._getDelegates.Add("com_is_repeated", this.GetComponentIsRepeated);
            this._getDelegates.Add("bigram", this.GetBigram);
            this._getDelegates.Add("bigr_pausetime", this.GetPauseTime);
            this._getDelegates.Add("bigr_start_time", this.GetStartTime);
            this._getDelegates.Add("bigr_is_targetted", this.GetIsTargetted);
            this._getDelegates.Add("bigr_is_within_timerange", this.GetIsWithinTimeRange);
            this._getDelegates.Add("bigr_frequency_class", this.GetFrequencyClass);
            this._getDelegates.Add("bigr_hand_comb", this.GetHandCombination);
            this._getDelegates.Add("bigr_adjacent", this.GetAdjacent);
            this._getDelegates.Add("bigr_repetitive", this.GetRepetitive);
        }

        public string GetComponent()
        {
            return Component.Title;
        }

        public string GetTrial()
        {
            return Trial.ToString();
        }

        public string GetComponentIsPractice()
        {
            return Component.IsPractice ? "1" : "0";
        }

        public string GetComponentIsSynthesized()
        {
            return Component.IncludeInSynthesis ? "1" : "0";
        }

        public string GetComponentIsTimed()
        {
            return Component.HasTimeLimit ? "1" : "0";
        }

        public string GetComponentIsRepeated()
        {
            return Component.IsUnlimited ? "2" :
                Component.IsRepetitive ? "1" : "0";
        }

        public string GetBigram()
        {
            return Bigram.GetBigram();
        }

        public string GetPauseTime()
        {
            return PauseTime.ToString();
        }

        public string GetStartTime()
        {
            return StartTime.ToString();
        }

        public string GetIsTargetted()
        {
            return IsTargetBigram() ? "1" : "0";
        }

        public string GetIsWithinTimeRange()
        {
            return IsWithinTimeFilteredRange(Copytask.CopytaskAnalysis.TIME_FILTER_PERC) ? "1" : "0";
        }

        public string GetFrequencyClass()
        {
            return Bigram.GetFrequencyClass();
        }

        public string GetHandCombination()
        {
            return Bigram.GetHandComb();
        }

        public string GetAdjacent()
        {
            return Bigram.GetAdjacent();
        }

        public string GetRepetitive()
        {
            return Bigram.GetRepetitive();
        }

        #endregion

        protected override void InitSetDelegates()
        {
            /* has no set delegates */
        }

    }
}
