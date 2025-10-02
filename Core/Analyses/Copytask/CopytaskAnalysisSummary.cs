using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputLog.Core.Analyses.Copytask.Elements;
using InputLog.Core.Events;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Copytask
{
    /// <summary>
    /// Create the summary of the copyTask, keeping the statistics information of every
    /// group and for each value of the group.
    /// </summary>
    public class CopytaskAnalysisSummary: AbstractAnalysisSummary
    {
        #region Fields
        /// <summary>
        /// List of the raw bigram data.
        /// </summary>
        public List<BigramContext> RawBigrams;

        /// <summary>
        /// Maps a groups name to its statistics data.
        /// </summary>
        private Dictionary<string, GroupStatistics> _groupData;

        /// <summary>
        /// Return the statistics of a group with given name.
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <returns>Returns the statistics data of the requested group.</returns>
        public GroupStatistics this[string groupName]
        {
            get
            {
                return this._groupData[groupName];
            }
        }

        /// <summary>
        /// Statistics accross groups, a set of meta-statistics about the general
        /// correctness of the task. 
        /// </summary>
        public readonly Dictionary<string, CorrectnessAccumulator> CorrectnessStatistics;

        /// <summary>
        /// Structure to keep the correctness information for a single group
        /// </summary>
        public class CorrectnessEntry
        {
            public int CountTargetted;
            public int CountNotTargetted;
            public double CorrectnessScore => (double)CountTargetted/(CountTargetted+CountNotTargetted);

            public PrettyPrintCorrectnessEntry GetPrettyPrinter()
            {
                return new PrettyPrintCorrectnessEntry(this);
            }
        }

        /// <summary>
        /// Keeps the correctness information for each component
        /// </summary>
        public readonly Dictionary<string, CorrectnessEntry> CorrectnessEntries;
        #endregion

        /// <summary>
        /// The answers to the questions asked at the end of a copyTask logging session
        /// </summary>
        public QuestionsData QuestionsData { get; set; }

        /// <summary>
        /// Create a new copyTask analysis summary.
        /// </summary>
        public CopytaskAnalysisSummary()
        {
            this._groupData = new Dictionary<string, GroupStatistics>();
            this.CorrectnessEntries = new Dictionary<string, CorrectnessEntry>();
            this.CorrectnessStatistics = new Dictionary<string, CorrectnessAccumulator>();
        }

        /// <summary>
        /// Add a group to the summary. Groups must have unique group names!
        /// The group is only added if it contains relevant information (= not empty).
        /// </summary>
        /// <param name="group">The data of the group.</param>
        /// <param name="weight">The weight of the group. Weights have an impact
        /// on when the groups are requested in a weighted fashion. Lighter weights
        /// mean the group shall be returned earlier. A heavier weight will 
        /// make sure the group will be returned later on. Default weight is 50.</param>
        /// <param name="groupingTitle">The grouping title for GroupStatistics. Multiple
        /// GroupStatistics may have the same grouping title, which means they all together
        /// belong to a bigger grouping.</param>
        /// <remarks>If the groupname is not unique the behavior is not defined.</remarks>
        public void AddGroup(GroupStatistics group, int weight = 50, string groupingTitle = null)
        {
            if (group.ContainsInformation)
            {
                this._groupData.Add(group.GroupName, group);
                group.Weight = weight;
                group.GroupingTitle = groupingTitle;
            }
        }

        /// <summary>
        /// Iterate over the groups in a weighted order. Groups with lighter
        /// weights will be returned first, groups with heavier weights will be
        /// returned later on.
        /// </summary>
        /// <returns>All the groups in order of their ascending weight</returns>
        public IEnumerable<GroupStatistics> WeightedIterate()
        {
            FilteredGroupedListAccumulator<GroupStatistics, int> groups =
                new FilteredGroupedListAccumulator<GroupStatistics, int>(item => item.Weight);

            groups.Accumulate(this._groupData.Values);
            int[] sortedWeights = groups.Keys.ToArray();
            Array.Sort(sortedWeights);

            foreach (int weight in sortedWeights)
            {
                foreach (GroupStatistics data in groups[weight])
                {
                    yield return data;
                }
            }
            yield break;
        }



        #region GroupStatistics Implementation
        /// <summary>
        /// Group all the statistics of one set of groups that belongs together
        /// </summary>
        public class GroupStatistics
        {
            /// <summary>
            /// If the grouping title is set this will insert a title before the
            /// XML Output of the groupstatistics of this element.
            /// </summary>
            public string GroupingTitle;

            /// <summary>
            /// Name of the 'big' grouping of the items. E.g frequency, components, adjacency, etc...
            /// </summary>
            public string GroupName
            {
                get;
                private set;
            }

            /// <summary>
            /// Weight of the group.
            /// </summary>
            internal int Weight
            {
                get;
                set;
            }

            /// <summary>
            /// Contains the statistics for each value in the group, e.g.:
            /// for adjacency: true, false
            /// for frequency: HF, LF, Indeterminate
            /// etc...
            /// </summary>
            private Dictionary<string, StatisticsAccumulator> _items;

            /// <summary>
            /// Return the different group values that have been encountered.
            /// Each group value has it's own set of statistics.
            /// </summary>
            public IEnumerable<string> Values
            {
                get
                {
                    return this._items.Keys;
                }
            }

            /// <summary>
            /// True only if this group contains any information, if it is an empty group
            /// this returns false.
            /// </summary>
            public bool ContainsInformation
            {
                get
                {
                    return this._items.Count > 0;
                }
            }

            /// <summary>
            /// Get the statistics information for a certain value of the group.
            /// </summary>
            /// <param name="value">The value of an item from this group.</param>
            /// <returns>The statistics associated with the value.</returns>
            public StatisticsAccumulator this[string value]
            {
                get
                {
                    return this._items[value];
                }
            }

            
            /// <summary>
            /// Create a new group of statistics.
            /// </summary>
            /// <param name="groupName">The name for the group.</param>
            public GroupStatistics(string groupName)
            {
                this.GroupName = char.ToUpper(groupName.ElementAt(0)) + groupName.Substring(1);
                this._items = new Dictionary<string, StatisticsAccumulator>();
                this.GroupingTitle = null;
            }

            /// <summary>
            /// Add a new value to the group together with the statistics for that
            /// group value.
            /// </summary>
            /// <param name="value">The value of the item in the group.</param>
            /// <param name="statistics">The statistics for that value</param>
            public void AddValue(string value, StatisticsAccumulator statistics)
            {
                this._items.Add(value, statistics);
            }

        #endregion

        }
    }
}
