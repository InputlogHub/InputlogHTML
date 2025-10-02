using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.IO.CSV
{
    /// <summary>
    /// A single item in the CSV file. This represents a single row. The data will be automatically
    /// set based on which headers correspond to which delegate functions. 
    /// NOTE: Any class subclassing CSVItem must have a DEFAULT constructor!
    /// </summary>
    public abstract class CSVItem
    {
        /// <summary>
        /// Delegate for setting CSVItem data based on the headers. The data
        /// is the data for a specific element.
        /// </summary>
        /// <param name="data">Data for an item, read from the CSV file.</param>
        public delegate void SetDataDelegate(string data);

        /// <summary>
        /// Matches header names to delegates so that based on the header name
        /// the correct attribute in the CSV item can be set using its public
        /// set method.
        /// </summary>
        public Dictionary<string, SetDataDelegate> SetDelegates
        {
            get
            {
                return this._setDelegates;
            }
        }
        protected readonly Dictionary<string, SetDataDelegate> _setDelegates;

        /// <summary>
        /// Delegate for getting the information of a CSV item associated with 
        /// a certain header.
        /// </summary>
        /// <returns>The data element returned by the delegate</returns>
        public delegate string GetDataDelegate();

        /// <summary>
        /// Matches header names to delegates so that based on the heade rname
        /// the correct attribute in the CSVItem may be retrieved, using its public
        /// get method.
        /// </summary>
        public Dictionary<string, GetDataDelegate> GetDelegates
        {
            get
            {
                return this._getDelegates;
            }
        }
        protected readonly Dictionary<string, GetDataDelegate> _getDelegates;


        /// <summary>
        /// Specifies the line number in the CSV file this CSVItem was 
        /// created from.
        /// </summary>
        internal int LineNo
        {
            get;
            set;
        }

        /// <summary>
        /// Deal with left over data. This is data that did not have
        /// any delegates associated with it.
        /// </summary>
        public ReadOnlyDictionary<string, string> LeftOverData
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this._leftOverData);
            }
        }
        protected Dictionary<string, string> _leftOverData;

        /// <summary>
        /// True if there is any left over data set in this item.
        /// </summary>
        public bool HasLeftOverData
        {
            get
            {
                return (this._leftOverData != null) &&
                        (this._leftOverData.Count > 0);
            }
        }

        /// <summary>
        /// Construct a new CSVItem
        /// </summary>
        public CSVItem()
        {
            this._setDelegates = new Dictionary<string, SetDataDelegate>();
            this._getDelegates = new Dictionary<string, GetDataDelegate>();
            this.InitSetDelegates();
            this.InitGetDelegates();
        }

        /// <summary>
        /// Use this method to initialize the delegate dictionary. This method is 
        /// called during the construction of the CSVItem.
        /// </summary>
        protected abstract void InitSetDelegates();

        /// <summary>
        /// Use this method to initialize the get delegate dictionary. This method is
        /// called during the construction of the CSVItem.
        /// </summary>
        protected abstract void InitGetDelegates();

        /// <summary>
        /// Add a leftover data item.
        /// </summary>
        /// <param name="header">the header associated with the data.</param>
        /// <param name="data">data item to add.</param>
        public void AddLeftoverData(string header, string data)
        {
            if (this._leftOverData == null)
            {
                this._leftOverData = new Dictionary<string, string>();
            }
            this._leftOverData.Add(header, data);
        }

    }
}
