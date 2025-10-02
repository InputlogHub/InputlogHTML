using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InputLog.Core.IO.AnalysisXML.Output;
using InputLog.Core.Util;

namespace InputLog.Core.Merging.Analyses.Processors
{
    /// <summary>
    ///     Class that processes analysis data in a way that allows it to
    ///     write the merged data in a CSV format that vertically expands
    ///     on the in the analysis data.
    ///     E.g:
    ///     ----------------DATA-------------------------  ----------- HEADER -----------
    ///     | title | key1 | key2 | title2 | key3 | ... |  | keys ....                  | Filename |
    ///     -----------------------------------------------------------------------------------------
    ///     | ////  | val  | val  | ////// | val  | ... |  | values ....                | value    |
    ///     | ////  | val  | val  | ////// | val  | ... |  | /////////////////////////  | //////// |
    ///     | ////  | val  | val  | ////// | val  | ... |  | /////////////////////////  | //////// |
    /// </summary>
    internal class VerticalCSVProcessor : BasicProcessor
    {
        /// <summary>
        ///     Constructs the processor.
        /// </summary>
        /// <param name="workDir">
        ///     Directory that can be used
        ///     to write some temporary files and to write the result
        ///     files too.
        /// </param>
        public VerticalCSVProcessor(string workDir) : base(workDir)
        {
            ExtraKeyOrder = new List<int>();
        }

        #region Fields

        //-----------------------------
        // Datamembers for writing the data
        //

        /// <summary>
        ///     List containing the order of in which the the values have
        ///     to be retrieved (based on keyId), in order for the right
        ///     values to match the right keys in the header fields of the extraInfo
        /// </summary>
        private readonly List<int> ExtraKeyOrder;

        /// <summary>
        ///     LineBuilder used for constructing the CSV lines.
        /// </summary>
        private CSVLineBuilder LineBuilder;

        private const string MBE_SEP = "::";

        /// <summary>
        ///     Array with all column elements of the vertical header;
        /// </summary>
        private string[] FullHeader;

        #endregion

        #region Write

        public override void Write(string filePath)
        {
            // Constructs the linebuilder
            LineBuilder = new CSVLineBuilder();

            // Creates the output stream we will write to.
            var output = new StreamWriter(filePath, false, Encoding.Unicode);

            // Writes the header.
            WriteHeader(ref output);

            // Writes the data for each processed file.
            for (var fileId = 0; fileId < FilesProcessed; fileId++)
            {
                WriteFile(ref output, fileId);
            }

            // Closes the output stream.
            output.Close();
        }

        /// <summary>
        ///     Collecting header elements and writing them.
        /// </summary>
        /// <param name="output"></param>
        private void WriteHeader(ref StreamWriter output)
        {
            LineBuilder.BeginLine();

            var keyCombiner = new StringBuilder();

            // Event keys
            foreach (var key in EventData.Keys)
            {
                keyCombiner.Append("event");
                keyCombiner.Append(':');
                keyCombiner.Append(key);
                LineBuilder.Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                keyCombiner.Clear();
            }

            // Header keys
            foreach (var headerKey in HeaderKeys)
            {
                keyCombiner.Append("sessionID");
                keyCombiner.Append(':');
                keyCombiner.Append(headerKey);
                LineBuilder.Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                keyCombiner.Clear();
            }

            // Extra keys
            foreach (var extraTitle in ExtraKeys.Keys)
            {
                LineBuilder.Append(StringUtils.FilterSPSSInvalid(extraTitle, MBE_SEP));
                ExtraKeyOrder.Add(-1); // empty keyId

                foreach (var pair in ExtraKeys[extraTitle])
                {
                    keyCombiner.Append(extraTitle);
                    keyCombiner.Append(':');
                    keyCombiner.Append(pair.Key);
                    LineBuilder.Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                    ExtraKeyOrder.Add(pair.Value);
                    keyCombiner.Clear();
                }
            }

            output.WriteLine(LineBuilder.EndLine());
        }

        /// <summary>
        ///     Reads and writes the event data for the file, if there are any.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="fileId"></param>
        private void WriteFile(ref StreamWriter output, int fileId)
        {
            StreamReader chunkReader = null;
            if (ChunkFiles.Contains(fileId))
            {
                var filePath = Path.Combine(TmpDir, CHUNK_NAME + fileId + CHUNK_EXT);
                chunkReader = new StreamReader(filePath);
            }

            // Writes events, if there are any among the processed files
            if (ChunkFiles.Count > 0)
            {
                // Array with the column labels from all chunks.
                FullHeader = EventData.Keys.ToArray();

                if (chunkReader != null)
                {
                    string eventLine;

                    // Catching the header line from the chunkReader and checking if it contains all the columns.
                    var missingCols = FindMissingColumns(chunkReader.ReadLine());
                    // Reading the next lines with the event data.
                    while ((eventLine = chunkReader.ReadLine()) != null)
                    {
                        if (!missingCols.IsNullOrEmpty())
                        {
                            var tmp = eventLine;
                            // Counts through all instances of the eventLine and inserts an empty string
                            // at the position of the missing column.
                            var count = -1;
                            var i = 0;
                            while ((i = tmp.IndexOf(';', i)) != -1)
                            {
                                i++;
                                count++;
                                if (missingCols.Contains(count))
                                {
                                    eventLine = eventLine.Insert(i, "\"\";");
                                    missingCols.Remove((count));
                                }
                            }
                            // If missingCols still has elements, concatenate them at the end of the eventLine.
                            eventLine = missingCols.Aggregate(eventLine, (current, position) => current + ";\"\"");
                        }
                        LineBuilder.BeginLine();
                        LineBuilder.Append(eventLine, MAX_EVENT_ELEMENTS);
                        AppendNonEventString(fileId);
                        output.WriteLine(LineBuilder.EndLine());
                    }
                }
                // If this file has no events, fill it with empty values.
                else
                {
                    LineBuilder.BeginLine();
                    for (var i = 0; i < MAX_EVENT_ELEMENTS; i++)
                    {
                        LineBuilder.Append(EMPTY);
                    }
                    AppendNonEventString(fileId);
                    output.WriteLine(LineBuilder.EndLine());
                }
            }
            else
            {
                LineBuilder.BeginLine();
                AppendNonEventString(fileId);
                output.WriteLine(LineBuilder.EndLine());
            }
        }

        /// <summary>
        ///     Checking if a chunk misses column headers and returning their position if true.
        /// </summary>
        /// <param name="readLine">The first line of a chunk with the column labels.</param>
        private List<int> FindMissingColumns(string readLine)
        {
            // Preparing a list of chunk column headers.
            var colPosition = new List<int>();
            var parts = readLine.Split(';');
            var chunkHeader = new List<string>();
            chunkHeader.AddRange(parts.Select(head => head.Trim('"')));

            // If both headers (full and chunk) have the same length, we assume 
            // that all the columns are present, that the column labels are similar, 
            // and similarly ordered.
            if (FullHeader.Length == chunkHeader.Count)
            {
                return colPosition;
            }
            // Column headers missing inside the chunkHeader list are inserted.
            // This increases the chunkHeader count.
            // The colPosition list has the position of the missing columns.
            var j = 0;
            for (; j < chunkHeader.Count - 1; j++)
            {
                if (!FullHeader[j].Equals(chunkHeader[j]))
                {
                    chunkHeader.Insert(j, ""); // Inserting a placeholder for FullHeader[j].
                    colPosition.Add(j);
                }
            }
            // If the chunkHeader list is still short of some columns,
            // they are added at the end of the list. 
            // The colPosition list has the position of the missing columns.
            var colDiff = FullHeader.Length - chunkHeader.Count;
            if (colDiff == 0)
            {
                return colPosition;
            }
            j++;
            for (var i = j; i < j + colDiff; i++)
            {
                chunkHeader.Add(""); // Inserting a placeholder for FullHeader[i].
                colPosition.Add(i);
            }

            return colPosition;
        }

        private void AppendNonEventString(int fileId)
        {
            AppendHeaderValues(fileId);
            AppendExtraValues(fileId);
        }

        /// <summary>
        ///     Writes extra info values
        /// </summary>
        /// <param name="fileId"></param>
        private void AppendExtraValues(int fileId)
        {
            foreach (var keyId in ExtraKeyOrder)
            {
                if (ExtraData.ContainsKey(keyId) && ExtraData[keyId].ContainsKey(fileId))
                {
                    LineBuilder.Append(ExtraData[keyId][fileId]);
                }
                else
                {
                    LineBuilder.Append(EMPTY);
                }
            }
        }

        /// <summary>
        ///     Writes header values.
        /// </summary>
        /// <param name="fileId"></param>
        private void AppendHeaderValues(int fileId)
        {
            for (var i = 0; i < HeaderWidth; i++)
            {
                if (HeaderData.Keys.Contains(i) && HeaderData[i].ContainsKey(fileId))
                {
                    LineBuilder.Append(HeaderData[i][fileId]);
                }
                else
                {
                    LineBuilder.Append(EMPTY);
                }
            }
        }

        #endregion
    }
}