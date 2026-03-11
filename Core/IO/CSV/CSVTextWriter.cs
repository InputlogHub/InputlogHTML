using System.Collections;
using System.Collections.Generic;
using System.IO;
using InputLog.Core.Util;

namespace InputLog.Core.IO.CSV
{
    public class CSVTextWriter
    {
        /// <summary>
        /// Separator character
        /// </summary>
        private string _separator;

        /// <summary>
        /// Writing the elements of an arrayList to an CSV file.
        /// </summary>
        /// <param name="list">List with ArrayLists</param>
        /// <param name="filePath">Path to write to</param>
        /// <param name="separator">Field separator to use</param>
        public void WriteToFile(List<ArrayList> list, string filePath, string separator)
        {
            _separator = separator;
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var item in list)
                {
                    if(item.IsNullOrEmpty()) continue;
                    foreach (var element in item)
                    {
                        writer.WriteLine(string.Join(_separator, (string[])element));
                    }
                   
                }
            }
        }
    }
}
