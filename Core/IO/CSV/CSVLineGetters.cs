using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.IO.CSV
{
    /// <summary>
    /// Class that returns a line given a foreign key for the requested line. 
    /// </summary>
    public class CSVLineGetter : InputLog.Core.IO.CSV.ICSVLineGetter
    {
        /// <summary>
        /// All the CSVItems to be stored.
        /// </summary>
        private IReadOnlyCollection<CSVItem> Values;

        /// <summary>
        /// Index of next line to return.
        /// </summary>
        private int CurrentIndex = 0;

        /// <summary>
        /// Array of all the headers for the items.
        /// </summary>
        public IReadOnlyCollection<string> Headers
        {
            get { return _Headers.AsReadOnly(); }
        }
        public List<string> _Headers;

        public CSVLineGetter( IReadOnlyCollection<CSVItem> values)
        {
            this.Values = values;
            this.ConstructHeaders();
        }

        /// <summary>
        /// Construct the index from the items to their keys.
        /// </summary>
        private void ConstructHeaders()
        {
            // Create Headers.
            _Headers = new List<string>();
            if (this.Values.Count == 0)
            {
                return;
            }

            var firstItem = this.Values.First();
            foreach (var delegatePair in firstItem.GetDelegates)
            {
                _Headers.Add(delegatePair.Key);
            }
        }

        public Dictionary<string, string> Current
        {
            get 
            {
                if (CurrentIndex >= Values.Count)
                {
                    return EmptyMap();
                }
                var item = Values.ElementAt(CurrentIndex);
                var values = new Dictionary<string, string>(_Headers.Count);
                for (var i = 0; i < Headers.Count; ++i)
                {
                    var currentHeader = _Headers[i];
                    values.Add(currentHeader, item.GetDelegates[currentHeader]());
                }
                return values;
            }
        }

        private Dictionary<string, string> EmptyMap()
        {
            var values = new Dictionary<string, string>(_Headers.Count);
            for (var i = 0; i < Headers.Count; ++i)
            {
                values.Add(_Headers[i], "");
            }
            return values;
            
        }

        public void Dispose() {}

        object System.Collections.IEnumerator.Current
        {
            get 
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            CurrentIndex++;
            return HasNext();
        }

        public bool HasNext()
        {
            return CurrentIndex < Values.Count;
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }
    }


    /// <summary>
    /// Class that repeats the same line over and over irrespective of the 
    /// foreign key associated with it.
    /// </summary>
    public class CSVRepeatGetter : ICSVLineGetter
    {
        private CSVItem RepeatItem;

        /// <summary>
        /// The static information for the single line of information that is always retrieved. 
        /// </summary>
        private Dictionary<string, string> Line;

        /// <summary>
        /// The headers in this item.
        /// </summary>
        private List<string> _Headers;

        /// <summary>
        /// List all the headers for this item.
        /// </summary>
        public IReadOnlyCollection<string> Headers
        {
            get { return _Headers.AsReadOnly(); }
        }


        /// <summary>
        /// Create a new repeat getter which always repeats the same single item
        /// irrespective of the foreign key used.
        /// </summary>
        /// <param name="repeatItem">The item which will always be repeated.</param>
        public CSVRepeatGetter(CSVItem repeatItem)
        {
            this.RepeatItem = repeatItem;

            Line = new Dictionary<string, string>(repeatItem.GetDelegates.Count);
            _Headers = new List<string>(repeatItem.GetDelegates.Count);

            foreach (var delegatePair in repeatItem.GetDelegates)
            {
                Line.Add(delegatePair.Key, delegatePair.Value());
                _Headers.Add(delegatePair.Key);
            }
        }

        public Dictionary<string, string> Current
        {
            get 
            {
                return Line;
            }
        }

        public void Dispose() { }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset() { }

        public bool HasNext()
        {
            return false;
        }
    }
}
