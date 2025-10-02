using System.Diagnostics;

namespace InputLog.Core.Util
{
    /// <summary>
    /// Filters work on a certain type and use a delegate method to decide whether
    /// an item should be filtered out or kept. Filters can be chained together.
    /// </summary>
    /// <typeparam name="Type">The type of the item the filter works on.</typeparam>
    public class BaseFilter<Type>
    {
        /// <summary>
        /// The delegate filter method used for filtering.
        /// </summary>
        /// <param name="item">The item to be filtered or not.</param>
        /// <returns>True if the item should be kept, false if the item should not be kept.</returns>
        public delegate bool FilterFunction(Type item);

        /// <summary>
        /// The filterfunction to be used to filter items.
        /// </summary>
        private FilterFunction _function;

        /// <summary>
        /// Create a new base filter. You provide the delegate to be 
        /// used to filter the items.
        /// </summary>
        /// <param name="function">Function used to filter the items. It should return
        /// true if the item is to be kept, false if the item is to filtered out.</param>
        public BaseFilter(FilterFunction function)
        {
            Debug.Assert(function != null);
            this._function = function;
        }

        /// <summary>
        /// Filter the given item using the filterfunction. 
        /// </summary>
        /// <param name="item">The item to filter. The item may not be null!</param>
        /// <returns>True if the item is to be kept, false if it is to be removed.</returns>
        public virtual bool Filter(Type item)
        {
            Debug.Assert(item != null);
            return this._function(item);
        }
    }

    /// <summary>
    /// Create a filter that can be chained together with other filters. All filters
    /// in this chain must decide to keep the item. If one filter in the chain decides
    /// against keeping the item, the result will be to not keep it.
    /// </summary>
    /// <typeparam name="Type">The type of the items to filter.</typeparam>
    public class ChainAndFilter<Type> : BaseFilter<Type>
    {
        /// <summary>
        /// The next filter in the chain.
        /// </summary>
        private BaseFilter<Type> _next;

        /// <summary>
        /// Create a new ChainAndFilter. This filter class allows chaining filters
        /// with the AND logical combination.
        /// </summary>
        /// <param name="function">The function used for filtering.</param>
        public ChainAndFilter(FilterFunction function) :
            base(function)
        {
            this._next = null;
        }

        /// <summary>
        /// Chain a filter after this one. Note, you can only chain one filter after another
        /// filter. Attempting to chain a second filter to the same first filter will result
        /// in undefined behavior.
        /// </summary>
        /// <param name="next">The filter to put in the chain behind this filter.</param>
        /// <returns>Returns a reference to this filter</returns>
        public ChainAndFilter<Type> Chain(BaseFilter<Type> next)
        {
            Debug.Assert(this._next == null, "Error: Overwriting filters in filter chain!");
            this._next = next;
            return this;
        }

        /// <summary>
        /// Remove the next element in the chain. This breaks the chain.
        /// </summary>
        public void Unchain()
        {
            this._next = null;
        }

        /// <summary>
        /// Filter the given item using the filterfunction. 
        /// </summary>
        /// <param name="item">The item to filter. The item may not be null!</param>
        /// <returns>True if the item is to be kept, false if it is to be removed.</returns>
        public override bool Filter(Type item)
        {
            if (this._next == null)
            {
                return base.Filter(item);
            }
            return base.Filter(item) && this._next.Filter(item);
        }
    }


    /// <summary>
    /// Create a filter that can be chained together with other filters. Only one filter
    /// in this chain must decide to keep the item. If one filter in the chain decides
    /// against to keep the item, the result will be to keep it.
    /// </summary>
    /// <typeparam name="Type">The type of the items to filter.</typeparam>
    public class ChainOrFilter<Type> : BaseFilter<Type>
    {
        /// <summary>
        /// The next filter in the chain.
        /// </summary>
        private BaseFilter<Type> _next;

        /// <summary>
        /// Create a new ChainOrFilter. This filter class allows chaining filters
        /// with the OR logical combination.
        /// </summary>
        /// <param name="function">The function used for filtering.</param>
        public ChainOrFilter(FilterFunction function) :
            base(function)
        {
            this._next = null;
        }

        /// <summary>
        /// Chain a filter after this one. Note, you can only chain one filter after another
        /// filter. Attempting to chain a second filter to the same first filter will result
        /// in undefined behavior.
        /// </summary>
        /// <param name="next">The filter to put in the chain behind this filter.</param>
        /// <returns>Reference to this filter.</returns>
        public ChainOrFilter<Type> Chain(BaseFilter<Type> next)
        {
            Debug.Assert(this._next == null, "Error: Overwriting filters in filter chain!");
            this._next = next;
            return this;
        }

        /// <summary>
        /// Filter the given item using the filterfunction. 
        /// </summary>
        /// <param name="item">The item to filter. The item may not be null!</param>
        /// <returns>True if the item is to be kept, false if it is to be removed.</returns>
        public override bool Filter(Type item)
        {
            if (this._next == null)
            {
                return base.Filter(item);
            }
            return base.Filter(item) || this._next.Filter(item);
        }
    }
}
