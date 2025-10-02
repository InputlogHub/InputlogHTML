using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using InputLog.Core.Analyses.Copytask.Bigrams;

namespace InputLog.Core.Analyses.Copytask.Elements
{
    /// <summary>
    /// A single component of the copy task. Components are separated in the idfx by 
    /// focus events with different names.
    /// Fields determined by the structure of the copyTask. They contain
    /// information regarding the nature of the components.
    /// </summary>
    public class ComponentStructure
    {
        #region Constants

        /// <summary>
        /// Constant value timelimit, for a task that does 
        /// not have a timelimit set.
        /// </summary>
        public const int NO_TIMELIMIT = 0;

        /// <summary>
        /// Constant value repetitions, for a task that does
        /// not have any repetitions specified, and is thus a 
        /// non-repetitive task
        /// </summary>
        public const int NO_REPETITIONS = 1;
        #endregion

        #region Structural Fields
        /// <summary>
        /// Title of the component
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// How many repetitions the task requires. Repetitions < 2 means
        /// the task is not repetitive
        /// </summary>
        public int Repetitions
        {
            get;
            private set;
        }

        /// <summary>
        /// True when the task is a repetitive task, false if it is not.
        /// This is determined by the number of repetitions for the task.
        /// </summary>
        public bool IsRepetitive
        {
            get
            {
                return this.Repetitions > 1;
            }
        }

        /// <summary>
        /// True if the task has a set timelimit, false if it does
        /// not have such a timelimit set.
        /// </summary>
        public bool HasTimeLimit
        {
            get
            {
                return this.TimeLimit > 0;
            }

        }

        /// <summary>
        /// The timelimit in seconds.
        /// </summary>
        public int TimeLimit
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether this tasks should be included in a special synthesis component 
        /// in the analysis.
        /// </summary>
        public bool IncludeInSynthesis
        {
            get;
            private set;
        }

        /// <summary>
        /// The target sentence for this component. This will be used
        /// to determine what the target bigrams are for this component.
        /// </summary>
        public string Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether this component serves an exmaple function in the 
        /// copyTask or not. Example components do not allow the user
        /// to add any text.
        /// </summary>
        public bool IsExample
        {
            get;
            private set;
        }

        /// <summary>
        /// If this is set, then the component does not have any set number
        /// of repetitions. The user may use the enter key as many times as 
        /// he or she likes without ever hitting a limit.
        /// </summary>
        public bool IsUnlimited
        {
            get;
            private set;
        }


        /// <summary>
        /// True if this component is a practice component. False if it is not. Practice
        /// components should be ignored during analysis.
        /// </summary>
        public bool IsPractice
        {
            get;
            private set;
        }

        /// <summary>
        /// A set of all the bigrams that are being targeted by 
        /// this component. This set is based on the target string.
        /// </summary>
        private HashSet<string> targetedBigrams;
        #endregion

        /// <summary>
        /// Create a new component and initialize it with its data.
        /// </summary>
        /// <param name="title">Title of the component</param>
        /// <param name="target">The target sentence for this component. This will be 
        /// used to determine which are the targeted bigrams for this component.</param>
        /// <param name="isExample">True if the component serves an example function,
        /// false if it does not.</param>
        /// <param name="isUnlimited">True if the task is unlimited, false if not. An unlimted 
        /// task is a task where the usage of 'too many' enter keys does not apply. The 
        /// user may use the enter key as he or she wishes.</param>
        /// <param name="synthesize">Set to true if this component should be included
        /// in a special synthesize analysis component outputted in the copyTask analysis.</param>
        /// <param name="isPractice">This is a practice component (if set to true). Practice components
        /// are ignored for the analysis.</param>
        public ComponentStructure(string title,
            string target,
            bool isExample,
            bool isUnlimited,
            bool synthesize,
            bool isPractice
        )
        {
            this._init(title, target, isExample, isUnlimited, synthesize, isPractice);
            this.Repetitions = NO_REPETITIONS;
            this.TimeLimit = NO_TIMELIMIT;
        }

        /// <summary>
        /// Create a new component and initialize it with its data.
        /// </summary>
        /// <param name="title">Title of the component</param>
        /// <param name="target">The target sentence for this component. This will be 
        /// used to determine which are the targeted bigrams for this component.</param>
        /// <param name="isExample">True if the component serves an example function,
        /// false if it does not.</param>
        /// <param name="isUnlimited">True if the task is unlimited, false if not. An unlimted 
        /// task is a task where the usage of 'too many' enter keys does not apply. The 
        /// user may use the enter key as he or she wishes.</param>
        /// <param name="synthesize">Set to true if this component should be included
        /// in a special synthesize analysis component outputed in the copyTask analysis.</param>
        /// <param name="isPractice">This is a practice component (if set to true). Practice components
        /// are ignored for the analysis.</param>
        /// <param name="repetitions">The number of repetitions required. This defaults to 1. If the task
        /// requires more than 1 repetition it is considered a repetitive task.</param>
        public ComponentStructure(string title,
            string target,
            bool isExample,
            bool isUnlimited,
            bool synthesize,
            bool isPractice,
            int repetitions
        )
        {
            Debug.Assert(!this._isImpossibleRepetitionUnlimitedCombo(isUnlimited, repetitions));
            this._throwExceptionIfImpossibleCombo(isUnlimited, repetitions);

            this._init(title, target, isExample, isUnlimited, synthesize, isPractice);
            this.TimeLimit = NO_TIMELIMIT;
            this.Repetitions = repetitions;
        }

        /// <summary>
        /// Create a new component and initialize it with its data.
        /// </summary>
        /// <param name="title">Title of the component</param>
        /// <param name="target">The target sentence for this component. This will be 
        /// used to determine which are the targeted bigrams for this component.</param>
        /// <param name="isExample">True if the component serves an example function,
        /// false if it does not.</param>
        /// <param name="isUnlimited">True if the task is unlimited, false if not. An unlimted 
        /// task is a task where the usage of 'too many' enter keys does not apply. The 
        /// user may use the enter key as he or she wishes.</param>
        /// <param name="synthesize">Set to true if this component should be included
        /// in a special synthesize analysis component outputed in the copyTask analysis.</param>
        /// <param name="isPractice">This is a practice component (if set to true). Practice components
        /// are ignored for the analysis.</param>
        /// <param name="repetitions">The number of repetitions required. This defaults to 1. If the task
        /// requires more than 1 repetition it is considered a repetitive task.</param>
        /// <param name="timelimit">The timelimit allotted to perform the task. This must be a non-negative integer, 
        /// larger than 0</param>
        public ComponentStructure(string title,
            string target,
            bool isExample,
            bool isUnlimited,
            bool synthesize,
            bool isPractice,
            int repetitions,
            int timelimit
        )
        {
            Debug.Assert(!this._isImpossibleRepetitionUnlimitedCombo(isUnlimited, repetitions));
            this._throwExceptionIfImpossibleCombo(isUnlimited, repetitions);

            this._init(title, target, isExample, isUnlimited, synthesize, isPractice);
            this.TimeLimit = timelimit;
            this.Repetitions = repetitions;
        }

        // Requests from the compiler to inline this function.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _init(string title, string target, bool isExample, bool isUnlimited, bool synthesize, bool isPractice)
        {
            this.Title = title;
            this.Target = target;
            this.IsExample = isExample;
            this.IsUnlimited = isUnlimited;
            this.IncludeInSynthesis = synthesize;
            this.IsPractice = isPractice;

            this._discoverBigrams();
        }

        /// <summary>
        /// Checks whether this combination of the Unlimited attribute, together with
        /// the specified number of repetitions is an impossible combination or not.
        /// </summary>
        /// <param name="isUnlimited">Whether this component is marked as being unlimited or not.</param>
        /// <param name="repetitions">The number of repetitions this component expects</param>
        /// <returns>true if the combination is impossible, false if the combination 
        /// is allowable.</returns>
        // Requests from the compiler to inline this function.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool _isImpossibleRepetitionUnlimitedCombo(bool isUnlimited, int repetitions)
        {
            bool unlimitedWithRepetitions = isUnlimited && repetitions > NO_REPETITIONS;
            return unlimitedWithRepetitions;
        }

        /// <summary>
        /// Throws an exception if the combination of unlimited & repetition is invalid.
        /// This method will make sure that even if we're not running in debug mode, the
        /// analysis will still fail with a clear message in case of an invalid combination
        /// of these attributes
        /// </summary>
        /// <param name="isUnlimited">The value for the unlimited attribute of this component.</param>
        /// <param name="repetitions">The number of repetitions for this component.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _throwExceptionIfImpossibleCombo(bool isUnlimited, int repetitions)
        {
            if (this._isImpossibleRepetitionUnlimitedCombo(isUnlimited, repetitions))
            {
                throw new ArgumentException(
                    "[Invalid attribute combination] " +
                        "Unlimited: (\"" + isUnlimited.ToString() + "\"; " +
                        "Repetitions: (\"" + repetitions.ToString() + "\""
                );
            }
        }

        /// <summary>
        /// Discover all the targeted bigrams of this component based
        /// on the provided target string.
        /// </summary>
        private void _discoverBigrams()
        {
            this.targetedBigrams = BigramParser.FindBigrams(this.Target);
        }

        /// <summary>
        /// Returns whether the specified bigram is being targeted
        /// by this component or not. 
        /// </summary>
        /// <param name="bigram">The bigram that we wish to evaluate. If a bigram
        /// is larger than 2 characters this method will always return false.</param>
        /// <returns>True if the bigram is a targeted bigram for this component, 
        /// false if the bigram is not being targeted.</returns>
        public bool IsTargetedBigram(string bigram)
        {
            // Handling bad situations. In debug, we want this to just crash our application.
            // This situation should never occur!
            Debug.Assert(bigram.Length == 2);
            if (bigram.Length != 2)
            {
                return false;
            }

            return this.targetedBigrams.Contains(bigram);
        }
    }
}
