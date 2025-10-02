using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.IO.CSV
{
    /// <summary>
    ///     This class reads CSV files, but can merge many CSV files into
    ///     a single CSVFileContent. The only requirement is that the CSV files
    ///     share a key which can be used to add link the lines that
    ///     have been read.
    /// </summary>
    /// <typeparam name="T">The type of the items that will be created
    /// and filled by reading CSV content from the files.</typeparam>
    public class CSVMergeReader<T>: CSVReader<T> where T: CSVItem, new()
    {
        #region Fields

        /// <summary>
        ///     Header string that shall function as the id for
        ///     merging content from different CSV files.
        /// </summary>
        private string _foreignKey;

        /// <summary>
        ///     The content container that will store all the information 
        ///     accross merges accross different files.
        /// </summary>
        private CSVMergeFileContent<T> _content;
        #endregion


        /// <summary>
        ///     Create a new CSV reader and specify the format of the text to be read. The default is a 
        ///     comma separated, non-quoted CSV file.
        /// </summary>
        /// <param name="foreignKey">The name of the header that will function 
        /// as the id header for the merging of the files.</param>
        /// <param name="separator">The separator string for values</param>
        public CSVMergeReader(string foreignKey, char separator = ',') : base(separator)
        {
            this._foreignKey = foreignKey;
            this._content = new CSVMergeFileContent<T>(foreignKey);
        }

        /// <summary>
        ///     Create a new CSV reader and specify the format of the text to be read.
        /// </summary>
        /// <param name="foreignKey">The name of the header that will function 
        /// as the id header for the merging of the files.</param>
        /// <param name="separator">The separator string for values</param>
        /// <param name="quote">The string used to quote the values.</param>
        public CSVMergeReader(string foreignKey, char separator, char quote) : base(separator, quote)
        {
            this._foreignKey = foreignKey;
            this._content = new CSVMergeFileContent<T>(foreignKey);
        }

        /// <summary>
        ///     Resets the content that has already been read. This deletes
        ///     any data that has already been processed. After calling this
        ///     method the CSVMergeReader can be used to read a different 
        ///     set of CSV Files not necessarily related to the data that
        ///     was being merged in previous calls.
        ///     This resets the class to its beginning state.
        /// </summary>
        public void Reset()
        {
            this._content = new CSVMergeFileContent<T>(this._foreignKey);
        }

        /// <summary>
        ///     Get the file contents that resulted after reading of 
        ///     the files.
        /// </summary>
        /// <returns>The file content</returns>
        public CSVMergeFileContent<T> GetFileContent()
        {
            return this._content;
        } 

        /// <summary>
        ///     Returns the already existing file container, so more information
        ///     may be added during subsequent ReadFile calls.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected override CSVFileContent<T> GetFileContainer()
        {
            return this._content;
        } 

        /// <summary>
        ///     FileContent container that allows for the merging
        ///     of data from different CSV files.
        /// </summary>
        /// <typeparam name="ItemType">The type of the items that will be created
        /// and filled by reading CSV content from the files.</typeparam>
        public class CSVMergeFileContent<ItemType>: CSVReader<ItemType>.CSVFileContent<ItemType> where ItemType: CSVItem, new()
        {
            /// <summary>
            ///     Name of the header that functions as the key to
            ///     specify which lines across different csv files 
            ///     are about the same items.
            /// </summary>
            private string _foreignKey;

            /// <summary>
            ///     Index of where in the line the foreignKey is situated
            ///     in the headers
            /// </summary>
            private int _foreignKeyIndex;

            /// <summary>
            ///     Dictionary that holds all the items that have been
            ///     read, indexed by their foreign key.
            /// </summary>
            private Dictionary<string, ItemType> _items;
            public virtual IReadOnlyCollection<ItemType> Items
            {
                get
                {
                    return this._items.Values.ToList().AsReadOnly();
                }
            }

            /// <summary>
            ///     Return a dictionary of all the read items, indexed
            ///     by their key.
            /// </summary>
            public Dictionary<string, ItemType> ItemsAsDictionary
            {
                get
                {
                    return this._items;
                }
            }

            /// <summary>
            ///     Create a new CSVMergeFileContent container.
            /// </summary>
            /// <param name="foreignKey">The header in the CSV file that functions
            /// as foreign key across different CSV files to determine which
            /// items belong together</param>
            public CSVMergeFileContent(string foreignKey)
            {
                this._foreignKey = foreignKey;
                this._items = new Dictionary<string, ItemType>();
            }

            /// <summary>
            /// Add an item to the CSV File Contents. Items can only be added after the headers
            /// have been set. This merges data into a single item across CSV files, if a valid
            /// foreign key has been specified.
            /// </summary>
            /// <param name="currentLineNo">Line number at which the item has been found</param>
            /// <param name="lineParts">The line, procesed into parts. The indices of the items
            /// should correspond to the indices of the headers.</param>
            internal override void AddItem(int currentLineNo, string[] lineParts)
            {
                // Check if the item already exists.
                string key = lineParts[this._foreignKeyIndex];
                ItemType item = null;
                if (this._items.ContainsKey(key))
                {
                    item = this._items[key];
                }
                else
                {
                    item = new ItemType();
                    this._items.Add(key, item);
                }

                this.FillItemWithLine(currentLineNo, lineParts, item);
            }

            /// <summary>
            /// Specify the headers. Headers must be set before you start adding items.
            /// Also determines the location of the foreignKey in the header list.
            /// </summary>
            /// <param name="headers">The headers read from the CSV file.</param>
            internal override void SetHeaders(string[] headers)
            {
                base.SetHeaders(headers);
                try
                {
                    this._foreignKeyIndex = Array.IndexOf(
                        headers,
                        headers.Single(header => header == this._foreignKey)
                    );
                }
                catch (Exception e)
                {
                    throw new CSVReaderException(
                        "Foreign key (" + this._foreignKey + ") not found in listed " +
                            "headers: " + String.Join(", ", headers),
                        e
                    );

                }
            }
        }
    }

}
