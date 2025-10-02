using System.Collections.Generic;

namespace InputLog.Core.Util
{
    /// <summary>
    /// A class that accumulates items.
    /// </summary>
    /// <typeparam name="AccumType">Type of the items being accumulated.</typeparam>
    public abstract class Accumulator<AccumType>
    {
        /// <summary>
        /// Create a new accumulator.
        /// </summary>
        public Accumulator()
        { }

        /// <summary>
        /// Add an item to the accumulator
        /// </summary>
        /// <param name="item">Item to add.</param>
        protected abstract void ProcessNewItem(AccumType item);

        /// <summary>
        /// Override this method with however the accumulator should
        /// process the addition of an item
        /// </summary>
        /// <param name="item">The item to be accumulated.</param>
        protected virtual void AccumulateItem(AccumType item)
        {
            this.ProcessNewItem(item);
        }

        /// <summary>
        /// Run the accumulator over the list, accumulating every item
        /// in the list, as specifid by the accumulator.
        /// </summary>
        /// <param name="inputList">The list to accumulate items from.</param>
        public void Accumulate(IEnumerable<AccumType> inputList)
        {
            this.BeforeAccumulate();
            foreach (AccumType item in inputList)
            {
                this.AccumulateItem(item);
            }
            this.AfterAccumulate();
        }

        /// <summary>
        ///  Method gets called before accumulation starts. Any processing
        ///  that needs to be done to set up the accumulator before procesing
        ///  may be executed here.
        /// </summary>
        protected virtual void BeforeAccumulate() { }

        /// <summary>
        /// Signifies to the accumulator that all items have been accumulated.
        /// Any processing that still has to happen may happen afterwords.
        /// </summary>
        protected virtual void AfterAccumulate() { }
    }

    public abstract class FilteredAccumulator<AccumType>: Accumulator<AccumType>
    {
        /// <summary>
        /// Filter to use for filtering. The filter should return true if the
        /// item is to be accumulated, false if it is to be filtered out.
        /// </summary>
        public BaseFilter<AccumType> Filter
        {
            set;
            get;
        }

        /// <summary>
        /// Filter an item. Returns true if the item is to be accumulated,
        /// false if the item should not be accumulated. If no filter is set, the
        /// item is will be accumulated.
        /// </summary>
        /// <param name="item">Item to filter.</param>
        /// <returns>True if the item should be accumulated, false if not.</returns>
        protected bool ShouldAccumulate(AccumType item)
        {
            if (this.Filter != null)
            {
                return this.Filter.Filter(item);
            }
            return true;
        }

        /// <summary>
        /// Add an item to the accumulator, if it passes the filters.
        /// </summary>
        /// <param name="item">Item to add.</param>
        protected override void AccumulateItem(AccumType item)
        {
            if (this.ShouldAccumulate(item))
            {
                this.ProcessNewItem(item);
            }
        }
    }


    /// <summary>
    /// Accumulate items in a list, after filtering out the irrelevant items.
    /// </summary>
    /// <typeparam name="AccumType">Type of the items being listed.</typeparam>
    public class FilteredListAccumulator<AccumType>: FilteredAccumulator<AccumType>
    {
        /// <summary>
        /// The items being listed.
        /// </summary>
        protected List<AccumType> _items;

        /// <summary>
        /// The items accumulated.
        /// </summary>
        public List<AccumType> Items
        {
            get
            {
                return this._items;
            }
        }

        /// <summary>
        /// Create a new ListAccumulator
        /// </summary>
        public FilteredListAccumulator() { }

        /// <summary>
        /// Add items to the list. The items will only be added if they
        /// pass the filter.
        /// </summary>
        /// <param name="item"></param>
        protected override void ProcessNewItem(AccumType item)
        {
            this._items.Add(item);
        }

        /// <summary>
        /// Clean the lists before accumulating a new set of items.
        /// </summary>
        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();
            this._items = new List<AccumType>();
        }
    }


    /// <summary>
    /// A grouped list accumlator is a simple class runs over an IEnumerable and
    /// groups items according to some values, if they pass the filter.
    /// </summary>
    /// <typeparam name="AccumType">The type of the times being accumlated. This
    /// should be the same type as the type being enumerated in your IEnumerable.</typeparam>
    /// <typeparam name="GroupType">The type of the value that will decide to which
    /// group an item belongs. E.g. string for string idenifiers, enums for enum values, etc.</typeparam>
    public class FilteredGroupedListAccumulator<AccumType, GroupType>: FilteredAccumulator<AccumType>
    {
        /// <summary>
        /// The delegate specifying how the grouping function looks. An item of
        /// a given type is handed to the grouping function who returns the items
        /// group value.
        /// </summary>
        /// <param name="item">The item of which we want to determine the group.</param>
        /// <returns>The group to which the item belongs.</returns>
        public delegate GroupType GroupingFunction(AccumType item);

        /// <summary>
        /// The dictionary that keeps collections of all items belonging 
        /// to each group. 
        /// </summary>
        protected Dictionary<GroupType, List<AccumType>> _items;

        /// <summary>
        /// Return all the groups currently in the list.
        /// </summary>
        public IEnumerable<GroupType> Keys
        {
            get
            {
                return this._items.Keys;
            }
        }

        /// <summary>
        /// Return all elements in specified group.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <returns>The items in specified group.</returns>
        public IEnumerable<AccumType> this[GroupType group]
        {
            get
            {
                return this._items[group];
            }
        }

        /// <summary>
        /// The concrete grouping function that will be used to determine
        /// the groups of the different elements.
        /// </summary>
        private GroupingFunction _groupingFunction;

        /// <summary>
        /// Create a new GroupedListAccumulator
        /// </summary>
        /// <param name="function">The concrete function that will be
        /// used for determining the groups of the accumulated items.</param>
        public FilteredGroupedListAccumulator(GroupingFunction function)
        {
            this._groupingFunction = function;
        }

        /// <summary>
        /// Add an item to its corresponding group.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        protected override void ProcessNewItem(AccumType item)
        {
            GroupType group = this._groupingFunction(item);
            if (!this._items.ContainsKey(group))
            {
                this._items.Add(group, new List<AccumType>());
            }
            this._items[group].Add(item);
        }

        /// <summary>
        /// Clear the currently aggregated items in order to be ready
        /// to aggregate a new set of items.
        /// </summary>
        protected override void BeforeAccumulate()
        {
            base.BeforeAccumulate();
            this._items = new Dictionary<GroupType, List<AccumType>>();
        }
    }
}
