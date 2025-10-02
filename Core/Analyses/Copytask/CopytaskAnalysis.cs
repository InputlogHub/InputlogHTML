using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using InputLog.Core.Analyses.Copytask.Elements;
using InputLog.Core.Analyses.Copytask.Resources;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Copytask
{
    /// <summary>
    /// The copyTask analysis analyzes the data from copytasks by first reading the bigram data for
    /// the task from a specific file, and then using the bigram data to analyze and summarize
    /// the pause information related to different components and differen types of bigrams.
    /// </summary>
    public class CopytaskAnalysis : Analysis
    {
        #region Constants
        /// <summary>
        /// Seperator used in the csv file.
        /// </summary>

        /// <summary>
        /// When filtering trials to ignore the first x trials, we use
        /// the trial_cut to determine the value of x.
        /// </summary>
        private const int TRIAL_CUT = 2;

        /// <summary>
        /// The percentage of time to filter out of a component.
        /// </summary>
        public const double TIME_FILTER_PERC = 0.10;

        /// <summary>
        /// Any bigram that has a pausetime of less than or equal to 
        /// the pause treshold will be filtered out from the results.
        /// </summary>
        private const int PAUSE_TRESHOLD = 30;


        private const string COMPONENT_GROUPING = "Components";
        private const string CHARACTERISTICS_GROUPING = "Characteristics";
        #endregion

        #region Fields
        /// <summary>
        /// Dictionary storing all the known bigrams and their characteristics.
        /// </summary>
        private Dictionary<string, Bigrams.Bigram> BigramData;

        /// <summary>
        /// Dictionary mapping path to a bigram file with the in memory, processed bigram data - if it has 
        /// already been read once. The bigram data itself is in the form
        /// key: bigram
        /// value: bigram data (frequency, bigram, percentile, repetitive, ...)
        /// </summary>
        private static Dictionary<string, Dictionary<string, Bigrams.Bigram>> LoadedBigramData =
            new Dictionary<string, Dictionary<string, Bigrams.Bigram>>();

        /// <summary>
        /// The structure of the copyTask being analysed in this analysis, built
        /// from the XML structure of the copyTask, present in the meta information
        /// of the sessionID.
        /// </summary>
        private CopytaskExecution CopytaskExecution;

        /// <summary>
        ///     Resource manager keeps track of the bigram language and layout
        ///     files that have already been processed.
        /// </summary>
        private static ResourceManager RM;

        #endregion
        /// <summary>
        /// Create a new CopytaskAnalysis.
        /// </summary>
        /// <param name="abbr">Abbreviation of the copyTask</param>
        /// <param name="events">The list of inputlog events</param>
        /// <param name="sessionID">The session identification of the log file</param>
        public CopytaskAnalysis(string abbr, List<Event> events, SessionIdentification sessionID) :
            base(abbr, events, sessionID)
        {
            this.LoadCopytaskStructure(sessionID.MetaInfo[SessionIdentification.META_COPYTASK]);
            if (RM == null)
            {
                RM = new ResourceManager();
            }
            string language = this.CopytaskExecution.Language;
            string keyboardLayout = null;

            // If the session keyboard is missing an attempt is made to extract it from the file title.
            try
            {
                keyboardLayout = sessionID.SessionInfo[SessionIdentification.SESSION_KEYBOARD];
            }
            catch(KeyNotFoundException)
            {
                foreach (var layout in RM.SupportedLayouts)
                {
                    if (CopytaskExecution.Title.ToUpper().Contains(layout))
                    {
                        keyboardLayout = layout;
                        sessionID.SessionInfo[SessionIdentification.SESSION_KEYBOARD] = layout;
                    }
                }
            }

            this.BigramData = this.LoadBigramData(language, keyboardLayout);
        }

        /// <summary>
        /// Load the copyTask structure based on the XML information of the copyTask
        /// that is present in the meta information of the session ID.
        /// </summary>
        /// <param name="copytaskXmlString">A string containing the copyTask structure
        /// in xml format</param>
        private void LoadCopytaskStructure(string copytaskXmlString)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(copytaskXmlString);

                this.CopytaskExecution = new CopytaskExecution();
                this.CopytaskExecution.LoadStructure(xml);
            }
            catch (Exception e)
            {
                throw new CopytaskAnalysisException(
                    "Could not correctly load copyTask description in idfx. " +
                        "[Message: " + e.Message + "]",
                    e
                );
            }
        }

        public override IAnalysisSummary DoAnalysis()
        {
            this.ReplayCopytaskExecution();
            List<BigramContext> bigrams = this.CopytaskExecution.RetrieveBigrams(this.BigramData, this.InputEvents.Count);
            CopytaskAnalysisSummary summary = this.GatherData(bigrams);
            summary.RawBigrams = bigrams;

            var questions = this.InputEvents.Last();
            if (questions.Type == EventType.QUESTIONS)
            {
                var data = questions.Parts.OfType<Questions>().First();
                summary.QuestionsData = new QuestionsData(
                    data.Handedness,
                    data.Computer,
                    data.Keyboard,
                    data.Browser,
                    data.Language,
                    data.Disorder,
                    data.Education,
                    data.Repetition);
            }

            return summary;
        }

        /// <summary>
        /// Gather the required data over the list of all the detected bigrams. This method takes the list of 
        /// bigrams and calculates all the statistics for all the different views copytaskanalysis. All statistiscs
        /// within one group form one view of the data and will be outputted as one view within the analysis.
        /// </summary>
        private CopytaskAnalysisSummary GatherData(ICollection<BigramContext> bigrams)
        {
            CopytaskAnalysisSummary summary = new CopytaskAnalysisSummary();

            int synthesis_weight = -5;
            int component_weight = 0;
            int characteristics_weight = 50;

            Dictionary<string, bool> synthesisComponents = new Dictionary<string, bool>();
            foreach (var bigramContext in bigrams)
            {
                synthesisComponents[bigramContext.Component.Title] = bigramContext.Component.IncludeInSynthesis;
            }

            // Dictionary of all the groups. 
            Dictionary<string, FilteredGroupedListAccumulator<BigramContext, string>> groups = this._getGroupAccumulators();

            // Add a special synthesis component that gives a quick overview of some selected
            // tasks and a few basic numbers.
            this._calculateSynthesisStatistics(bigrams, summary, synthesis_weight);

            // Handle the component groupings differently from the rest.
            // 1. Print overview over components - using a different statistics calculator
            // 2. Extra subdivision - for each component subdivision per trial
            // 3. Special weights for fixed ordering of components.
            this._handleComponentGroupings(bigrams, summary, component_weight, synthesisComponents);

            // Calculate meta statistics accross groups
            this._getCorrectnessScores(bigrams, summary, synthesisComponents);

            // weights for the groups. We want component and component_TC to appear first (weights from 0).
            // followed by the components per trial analysis (weights starting from 10)
            // Ending with the other groups (weights from 100) where the we always first have the
            // normal group, followed by the TC group.
            int weight = characteristics_weight;

            // Handle the remaining groups - non component groups
            // E.g. frequency, adjacency groups...
            foreach (var group in groups)
            {
                string groupName = group.Key;
                FilteredGroupedListAccumulator<BigramContext, string> groupAcc = group.Value;

                // Calculate the statistics for all the bigrams.
                groupAcc.Accumulate(bigrams);
                summary.AddGroup(
                    this._calculateGroupStatistics<StatisticsAccumulator>(groupName, groupAcc, synthesisComponents),
                    weight++,
                    CHARACTERISTICS_GROUPING
                );
            }

            return summary;
        }

        /// <summary>
        /// Calculates a small synthesis overview of the data. This handles a few different views 
        /// and should always be outputted first.
        /// </summary>
        /// <param name="bigrams">The list of bigrams on which to calculate the statistics.</param>
        /// <param name="summary">The summary to which to add the statistics results</param>
        /// <param name="weight">Weight of the synthesis group.</param>
        private void _calculateSynthesisStatistics(IEnumerable<BigramContext> bigrams, CopytaskAnalysisSummary summary, int weight)
        {
            // We create a group of statistics that form the synthesis component.
            CopytaskAnalysisSummary.GroupStatistics synthesis =
                new CopytaskAnalysisSummary.GroupStatistics("InterKey Intervals (IKI)");

            // 1. Overall component - over all targetted bigrams.
            StatisticsAccumulator overallTargeted = new StatisticsAccumulator();
            overallTargeted.Filter = 
                this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters());
            overallTargeted.Accumulate(bigrams);
            synthesis.AddValue("Targeted Bigrams", overallTargeted);

            // 2. Overall component - over all HF bigrams.
            StatisticsAccumulator overall_HF = new StatisticsAccumulator();
            overall_HF.Filter = 
                new ChainAndFilter<BigramContext>(
                    item => item.Bigram.FreqClass == Bigrams.Bigram.FrequencyClass.HF)
                .Chain(this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters()));
            overall_HF.Accumulate(bigrams);
            synthesis.AddValue("High Frequency Bigrams", overall_HF);

            // 3. Overall synthesis components - all components that have the
            // IncludeInSynthesis flag set.
            StatisticsAccumulator overall_synthesis = new StatisticsAccumulator();
            overall_synthesis.Filter = 
                new ChainAndFilter<BigramContext>(
                    item => item.Component.IncludeInSynthesis)
                .Chain(this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters()));
            overall_synthesis.Accumulate(bigrams);
            synthesis.AddValue("Selected Components' Bigrams", overall_synthesis);

            summary.AddGroup(synthesis, weight, "Overall Synthesis");
        }

        private void _getCorrectnessScores(IEnumerable<BigramContext> bigrams, CopytaskAnalysisSummary summary, Dictionary<string, bool> synthesisComponents)
        {
            // Group in the components first, but this time only those in the synthesis
            var componentAcc = new FilteredGroupedListAccumulator<BigramContext, string>(item => item.Component.Title);
            componentAcc.Filter = this._getPauseTimeFilter().Chain(this._getPerpetualFilters());
            componentAcc.Accumulate(bigrams);

            // Next we calculate the statistics for each group.
            // This includes groups not in the synthesis
            var overallStatistics = new List<StatisticsAccumulator>();
            foreach (var groupName in componentAcc.Keys)
            {
                bool addSuffix = (
                    synthesisComponents != null &&
                    synthesisComponents.ContainsKey(groupName) &&
                    synthesisComponents[groupName]);
                var groupTitle = groupName + (addSuffix ? "*" : string.Empty);
                var groupItems = componentAcc[groupName];
                var statsAcc = new StatisticsAccumulator();
                statsAcc.Accumulate(groupItems);
                overallStatistics.Add(statsAcc);
                summary.CorrectnessEntries.Add(groupTitle, new CopytaskAnalysisSummary.CorrectnessEntry
                {
                    CountTargetted = statsAcc.CountTargetted,
                    CountNotTargetted = statsAcc.CountNotTargetted
                });
            }

            // Accumulate accross the accumulated statistics, creating meta statistics
            summary.CorrectnessStatistics.Add("Overall", new CorrectnessAccumulator());
            summary.CorrectnessStatistics["Overall"].Accumulate(overallStatistics);

            // Group in the components first, but this time only those in the synthesis
            var componentInSynthesisAcc = new FilteredGroupedListAccumulator<BigramContext, string>(item => item.Component.Title);
            componentInSynthesisAcc.Filter = new ChainAndFilter<BigramContext>(item => item.Component.IncludeInSynthesis)
                .Chain(this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters()));
            componentInSynthesisAcc.Accumulate(bigrams);

            // Next we calculate the statistics for each group.
            var synthesisStatistics = new List<StatisticsAccumulator>();
            foreach (var groupName in componentInSynthesisAcc.Keys)
            {
                var groupItems = componentInSynthesisAcc[groupName];
                var statsAcc = new StatisticsAccumulator();
                statsAcc.Accumulate(groupItems);
                synthesisStatistics.Add(statsAcc);
            }

            // Accumulate accross the accumulated statistics, creating meta statistics
            summary.CorrectnessStatistics.Add("Selected Components", new CorrectnessAccumulator());
            summary.CorrectnessStatistics["Selected Components"].Accumulate(synthesisStatistics);
        }

        /// <summary>
        ///  Handle the groupings of components. This requires a different treatment
        ///  from the other groupings.
        /// </summary>
        /// <param name="bigrams">The list of bigrams on which to calculate the statistics.</param>
        /// <param name="summary">The summary to which to add the statistics results</param>
        /// <param name="weight">Weight of the component groupings.</param>
        /// <param name="synthesisComponents">Names of the components and whether they are included in the synthesis or not</param>
        private void _handleComponentGroupings(
            IEnumerable<BigramContext> bigrams,
            CopytaskAnalysisSummary summary,
            int weight,
            Dictionary<string, bool> synthesisComponents
        )
        {
            // Define weights: First we print components, then components (trial cut) then 
            // individual components grouped by trial.
            int weight_component = weight;
            int weight_component_TC = weight + 5;
            int weight_component_TF = weight + 10;
            int weight_individualComponent = weight + 15;

            // Group by component
            FilteredGroupedListAccumulator<BigramContext, string> componentAcc =
                new FilteredGroupedListAccumulator<BigramContext, string>(item => item.Component.Title);

            // Calculate statistics for the componentGroup - both normal & trial filtered.
            // 1. First calculate Trial Cut statistics of the components.
            componentAcc.Filter = 
                this._getTrialCutFilter()
                .Chain(this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters()));
            componentAcc.Accumulate(bigrams);
            summary.AddGroup(
                this._calculateGroupStatistics<ComponentStatisticsAccumulator>("Trial Cut >2", componentAcc, synthesisComponents),
                weight_component_TC,
                COMPONENT_GROUPING
            );

            // 2. Calculate statistics of components with both normal filtering & time % filtering
            componentAcc.Filter = 
                this._getTimePercentageFilter(TIME_FILTER_PERC)
                .Chain(this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters()));
            componentAcc.Accumulate(bigrams);
            summary.AddGroup(
                this._calculateGroupStatistics<ComponentStatisticsAccumulator>("Time Filtered 10%", componentAcc, synthesisComponents),
                weight_component_TF,
                COMPONENT_GROUPING
            );

            // 3. Calculate statistics of components with only basic filtering
            componentAcc.Filter = 
                this._getPauseTimeFilter()
                .Chain(this._getPerpetualFilters());
            componentAcc.Accumulate(bigrams);
            summary.AddGroup(
                this._calculateGroupStatistics<ComponentStatisticsAccumulator>("Overall", componentAcc, synthesisComponents),
                weight_component,
                COMPONENT_GROUPING
            );


            // 4. Calculate the statistics on a per trial basis, for each component separately.
            // IMPORTANT NOTE: [ORDER IS IMPORTANT] the accumulator at this point still holds the 
            // component groups gotten from only basic filtering.
            this._calculateComponentPerTrialStatistics(
                componentAcc,
                summary,
                weight_individualComponent,
                synthesisComponents
            );
        }

        /// <summary>
        /// Given an accumulator with a grouping, calculate the statistics
        /// for each separate group and add all of the results to a GroupStatistics class
        /// of the summary
        /// </summary>
        /// <typeparam name="StatsAccType">The type of the accumulator to be used
        /// to calculate statistics for each group, within this grouping. This type should
        /// extend StatisticsAccumulator and have a default constructor</typeparam>
        /// <param name="groupName">Name for the group that's being processed.</param>
        /// <param name="group">The accumulator that has accumulated the groups.</param>
        /// <param name="synthesisComponents">Names of the components and whether they are included in the synthesis or not</param>
        /// <param name="subGroupPrefix">A prefix used for the names of the subgroups. 
        /// By default this is empty.</param>
        /// <returns>The calculated statistics for each group, saved in a GroupStatistics.</returns>
        private CopytaskAnalysisSummary.GroupStatistics _calculateGroupStatistics<StatsAccType>(
            string groupName,
            FilteredGroupedListAccumulator<BigramContext, string> group,
            Dictionary<string, bool> synthesisComponents = null,
            string subGroupPrefix = ""
        )
            where StatsAccType : StatisticsAccumulator, new()
        {
            groupName = groupName + (
                synthesisComponents != null &&
                synthesisComponents.ContainsKey(groupName) && 
                synthesisComponents[groupName] ? "*" : string.Empty);
            CopytaskAnalysisSummary.GroupStatistics groupStats =
                new CopytaskAnalysisSummary.GroupStatistics(groupName);

            foreach (string componentName in group.Keys)
            {
                bool addRequiredSuffix = (
                    synthesisComponents != null && 
                    synthesisComponents.ContainsKey(componentName) && 
                    synthesisComponents[componentName]);
                StatsAccType statsAcc = new StatsAccType();
                statsAcc.Accumulate(group[componentName]);
                string subGroupName = subGroupPrefix + componentName + (addRequiredSuffix ? "*" : string.Empty);
                groupStats.AddValue(subGroupName, statsAcc);
            }
            return groupStats;
        }

        /// <summary>
        /// Calculate the statistics for separately for each component, per trial (as in
        /// grouped by trial). 
        /// </summary>
        /// <param name="componentAcc">The accumulator containing the bigrams grouped by component</param>
        /// <param name="summary">Summary to which we add the results.</param>
        /// <param name="initialWeight">The weight at which the per component statistics are started.</param>
        private void _calculateComponentPerTrialStatistics(
            FilteredGroupedListAccumulator<BigramContext, string> componentAcc,
            CopytaskAnalysisSummary summary,
            int initialWeight,
            Dictionary<string, bool> synthesisComponents = null
        )
        {
            FilteredGroupedListAccumulator<BigramContext, string> trialGrouping =
                new FilteredGroupedListAccumulator<BigramContext, string>(item => item.Trial.ToString());

            // For each component, we get the bigrams in that component and group them
            // by trial. For each such trial group we save the statistics and save the entire
            // lot of them together in a CopytaskAnalysisSummary.GroupStatistics group for that
            // component specifically.
            foreach (string componentName in componentAcc.Keys)
            {
                trialGrouping.Accumulate(componentAcc[componentName]);

                summary.AddGroup(
                    this._calculateGroupStatistics<StatisticsAccumulator>(componentName, trialGrouping, synthesisComponents, "Trial "),
                    initialWeight++,
                    COMPONENT_GROUPING
                );
            }
        }

        /// <summary>
        /// Create the set of group accumulators that are required to calculate all 
        /// the required statistics and data.
        /// </summary>
        /// <returns>A dictionary containing the groupAccumulators that should be used 
        /// to group and accumulate the required data.</returns>
        private Dictionary<string, FilteredGroupedListAccumulator<BigramContext, string>> _getGroupAccumulators()
        {
            Dictionary<string, FilteredGroupedListAccumulator<BigramContext, string>> groups =
                new Dictionary<string, FilteredGroupedListAccumulator<BigramContext, string>>();

            // Define recurring filters.
            ChainAndFilter<BigramContext> perpetualFilters = _getPerpetualFilters();
            ChainAndFilter<BigramContext> pauseTimeTresholdFilter = _getPauseTimeFilter().Chain(perpetualFilters);
            ChainAndFilter<BigramContext> TrialCutAndPauseTimeFilter = _getTrialCutFilter().Chain(pauseTimeTresholdFilter);

            // Group by bigram characteristics.
            // Group by frequency (all tasks)
            var groupFrequency = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.FreqClass.ToString()
            );
            // Filter: 
            // 1. No indeterminate 
            // 2. pauseTime
            groupFrequency.Filter = new ChainAndFilter<BigramContext>(
                item => item.Bigram.FreqClass != Bigrams.Bigram.FrequencyClass.Indeterminate
            ).Chain(pauseTimeTresholdFilter);
            groups.Add("Frequency", groupFrequency);

            //
            // Group by Frequency (trial filtered)
            //
            var groupFrequency_TC = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.FreqClass.ToString()
            );
            // Filter: 
            // 1. NoIndeterminate 
            // 2. Trial excl.
            // 3. pauseTime
            groupFrequency_TC.Filter =
                new ChainAndFilter<BigramContext>(
                    item => item.Bigram.FreqClass != Bigrams.Bigram.FrequencyClass.Indeterminate)
                .Chain(TrialCutAndPauseTimeFilter);
            groups.Add("Frequency (Trial excl. > 2)", groupFrequency_TC);

            //
            // Group by frequency (only repetitive tasks) & ignore Indetermined & trial cut.
            //
            var groupFrequencyRepetitive = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.FreqClass.ToString()
            );
            // Filter:
            // 1. Only repetitive tasks
            // 2. No Indeterminate
            // 3. Trial excl.
            // 4. Pause Filter.
            groupFrequencyRepetitive.Filter =
                new ChainAndFilter<BigramContext>(item => (item.Component.IsRepetitive || item.Component.IsUnlimited))
                    .Chain(new ChainAndFilter<BigramContext>(item => item.Bigram.FreqClass != Bigrams.Bigram.FrequencyClass.Indeterminate)
                    .Chain(TrialCutAndPauseTimeFilter)
                );
            groups.Add("Frequency (Only repetitions & Trial excl. >2)", groupFrequencyRepetitive);


            //
            // Group by hand combination (Ignore unknown)
            //
            var groupHand = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Hand.ToString()
            );
            // Filter:
            // 1. No Unknown 
            // 2. pauseTime
            groupHand.Filter =
                new ChainAndFilter<BigramContext>(item => item.Bigram.Hand != Bigrams.Bigram.HandCombination.Unknown)
                    .Chain(pauseTimeTresholdFilter);
            groups.Add("Hand combination", groupHand);


            //
            // Group by hand combination (Trial Cut & Ignore Unknown)
            //
            var groupHand_TC = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Hand.ToString()
            );
            // Filter: 
            // 1. Ignore Unknown
            // 2. Trial excl.
            // 3. Pause Time
            groupHand_TC.Filter =
                new ChainAndFilter<BigramContext>(item => item.Bigram.Hand != Bigrams.Bigram.HandCombination.Unknown)
                    .Chain(TrialCutAndPauseTimeFilter);
            groups.Add("Hand combination (Trial excl. >2)", groupHand_TC);

            //
            // Group by adjacency
            //
            var groupAdjacent = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Adjacent.ToString()
            );
            // Filter:
            // 1. Pause Time
            groupAdjacent.Filter = pauseTimeTresholdFilter;
            groups.Add("Adjacency", groupAdjacent);

            // 
            // Group by adjacency (Trial excl. >2)
            // 
            var groupAdjacent_TC = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Adjacent.ToString()
            );
            // Filter:
            // 1. Trial Cut
            // 2. Pause Time
            groupAdjacent_TC.Filter = TrialCutAndPauseTimeFilter;
            groups.Add("Adjacency (Trial excl. >2)", groupAdjacent_TC);

            //
            // Group by repetitiveness.
            //
            var groupRepetitive = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Repetitive.ToString()
            );
            // Filter:
            // 1. Pause Time
            groupRepetitive.Filter = pauseTimeTresholdFilter;
            groups.Add("Repetition", groupRepetitive);

            //
            // Group by repetitiveness (trial excl.)
            //
            var groupRepetitive_TC = new FilteredGroupedListAccumulator<BigramContext, string>(
                item => item.Bigram.Repetitive.ToString()
            );
            // Filter:
            // 1. Trial Cut
            // 2. Pause Time
            groupRepetitive_TC.Filter = TrialCutAndPauseTimeFilter;
            groups.Add("Repetition (Trial excl. >2)", groupRepetitive_TC);
            return groups;
        }

        /// <summary>
        /// Returns a filter that contains all the basic filtering options that are applied to all
        /// the bigrams.
        /// </summary>
        /// <returns>A base filter used on all bigrams</returns>
        private ChainAndFilter<BigramContext> _getPerpetualFilters()
        {
            ChainAndFilter<BigramContext> noPracticeFilter = new ChainAndFilter<BigramContext>((item) => (!item.Component.IsPractice));
            return noPracticeFilter;
        }

        /// <summary>
        /// Return a filter to use for filtering on trial cut. This is a filter that 
        /// can be used in chain.
        /// </summary>
        /// <returns>A filter to use for trial cutting. The filter can be used in a chain.</returns>
        private ChainAndFilter<BigramContext> _getTrialCutFilter()
        {
            ChainAndFilter<BigramContext> trialCutFilter = new ChainAndFilter<BigramContext>(
                (item) => (
                    !(item.Component.IsUnlimited || item.Component.IsRepetitive) ||
                    (item.Trial > TRIAL_CUT)
            ));
            return trialCutFilter;
        }

        /// <summary>
        /// Get a filter to use for pauseTimeTreshold filtering
        /// </summary>
        /// <returns>The filter to use for pauseTimeTreshold filtering.</returns>
        private ChainAndFilter<BigramContext> _getPauseTimeFilter()
        {
            ChainAndFilter<BigramContext> pauseTimeTresholdFilter =
                new ChainAndFilter<BigramContext>(item => item.PauseTime > PAUSE_TRESHOLD);
            return pauseTimeTresholdFilter;
        }

        /// <summary>
        /// Return a filter that filters out the elements of within the first specified % of time
        /// of a component, and the last specified % of time.
        /// </summary>
        /// <param name="percentage">The percentage that should be taken from
        /// the start and the end.</param>
        /// <returns>A filter that filters a time-based percentage at the front and the
        /// end of the component's execution.</returns>
        private ChainAndFilter<BigramContext> _getTimePercentageFilter(double percentage)
        {
            ChainAndFilter<BigramContext> timePercentageFilter =
                new ChainAndFilter<BigramContext>(item => item.IsWithinTimeFilteredRange(percentage));
            return timePercentageFilter;
        }


        /// <summary>
        /// Replay the copyTask execution to build an in-memory representation
        /// of how the copyTask was performed, what events are part of what component
        /// or trial in the copyTask. This is needed for the correct subdivisions into analysis components.
        /// 
        /// For determining what the targetted bigrams were, pause performance of bigrams, 
        /// to make subdivisions of the data based on component, trial, etc.
        /// </summary>
        private void ReplayCopytaskExecution()
        {
            foreach (Event currentEvent in this.InputEvents)
            {
                if (currentEvent.Type != EventType.QUESTIONS)
                {
                    this.CopytaskExecution.AddEvent(currentEvent);
                }
            }
        }


        #region Static bigram data handling
        /// <summary>
        /// Load the bigram data from the CSV file at given bigramPath. The read bigram 
        /// information is stored for later retrieval of the same information without having
        /// to read and process the file again.
        /// </summary>
        /// <param name="language">The language for the bigrams</param>
        /// <param name="layout">The keyboard layout used in the copyTask.</param>
        /// <returns>The bigram data if it could be read correctly.</returns>
        private Dictionary<string, Bigrams.Bigram> LoadBigramData(string language, string layout)
        {
            Dictionary<string, Bigrams.Bigram> bigrams = RM.GetBigrams(language, layout);
            return bigrams;
        }
        #endregion
    }

    public class CopytaskAnalysisException : Exception
    {
        public CopytaskAnalysisException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
