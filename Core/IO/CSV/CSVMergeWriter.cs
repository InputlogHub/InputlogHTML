using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.IO.CSV
{

    public class CSVMergeWriter
    {
        /// <summary>
        /// The line getters used to get parts of information for each line.
        /// </summary>
        private ICSVLineGetter[] LineGetters;

        /// <summary>
        /// Headers to be used for all lines.
        /// </summary>
        private Dictionary<ICSVLineGetter, HashSet<string>> HeadersMap;

        private HashSet<string> AllHeaders;

        /// <summary>
        /// Separator character
        /// </summary>
        private readonly string Separator;

        /// <summary>
        /// Create a new CSVMergeWriter. Initialize with the linegetters used for all parts of each line.
        /// </summary>
        /// <param name="lineGetters"></param>
        public CSVMergeWriter(IEnumerable<ICSVLineGetter> lineGetters, string seperator = ";")
        {
            Separator = seperator;
            LineGetters = new ICSVLineGetter[lineGetters.Count()];
            var i = 0;
            foreach (var lgetter in lineGetters)
            {
                LineGetters[i] = lgetter;
                i++;
            }

            HeadersMap = new Dictionary<ICSVLineGetter, HashSet<string>>();
            AllHeaders = new HashSet<string>();
            foreach (var lgetter in LineGetters)
            {
                foreach (var header in lgetter.Headers)
                {
                    if (!AllHeaders.Contains(header))
                    {
                        AllHeaders.Add(header);
                        if (!HeadersMap.ContainsKey(lgetter))
                        {
                            HeadersMap.Add(lgetter, new HashSet<string>());
                        }
                        HeadersMap[lgetter].Add(header);
                    }
                }
            }
        }

        public void WriteToFile(string filePath, bool allowOverwrite = false)
        {
            FileInfo fInfo = new FileInfo(filePath);
            if (fInfo.Exists && !allowOverwrite)
            {
                throw new InvalidOperationException("File already exists and may not be overwritten.");
            }

            ResetLineGetters();


            using (StreamWriter writer = new StreamWriter(fInfo.FullName))
            {
                WriteHeaders(writer);
                // Iterate once over each LineGetter so we are sure to get all the lines of ever LineGetter.
                // These are only iterated over once, because once a line is consumed from a lineGetter it
                // does not appear anymore in subsequent iterations.
                foreach (var lgetter in LineGetters)
                {
                    // Iterate over all fkeys not consumed yet in this line getter.
                    while (lgetter.HasNext())
                    {
                        var linemap = _ConstructLine();
                        var lineBuilder = new StringBuilder();

                        // For each key, add its value to the line, in consistent order.
                        foreach (var header in AllHeaders)
                        {
                            lineBuilder.Append(linemap[header]);
                            lineBuilder.Append(Separator);
                        }
                        // Remove last separator
                        lineBuilder.Remove(lineBuilder.Length - Separator.Length, Separator.Length);

                        // Write Line.
                        writer.WriteLine(lineBuilder.ToString());
                    } 
                }
            }
        }

        private void WriteHeaders(StreamWriter writer)
        {
            StringBuilder headerLine = new StringBuilder();
            foreach (var headerPair in HeadersMap)
            {
                var headers = headerPair.Value;
                foreach (var header in headers)
                {
                    headerLine.Append(header);
                    headerLine.Append(Separator);
                }
            }
            headerLine.Remove(headerLine.Length - Separator.Length, Separator.Length);
            writer.WriteLine(headerLine.ToString());
        }

        private void ResetLineGetters()
        {
            foreach (var lgetter in LineGetters)
            {
                lgetter.Reset();
            }
        }

        /// <summary>
        /// Construct a single line based on a foreign key.
        /// </summary>
        /// <param name="lineGetter"></param>
        /// <param name="fkey"></param>
        /// <returns></returns>
        private Dictionary<string, string> _ConstructLine()
        {
            // For each header we have to fill, retrieve the information associated
            // with the header from the line getter that has information of that header.
            Dictionary<string, string> Line = new Dictionary<string, string>();

            // Header map contains each line getter exactly once, or a line getter is not included
            // if it did not have any unique headers. In which case it does not contribute any 
            // novel data.
            foreach (var headerPair in HeadersMap)
            {
                var lineGetter = headerPair.Key;
                var headers = headerPair.Value;
                var parts = lineGetter.Current;
                foreach (var header in headers)
                {
                    Line.Add(header, parts[header]);
                }
                lineGetter.MoveNext();
            }
            return Line;
        }
    }
}
