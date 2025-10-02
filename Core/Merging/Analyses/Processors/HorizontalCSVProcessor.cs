using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InputLog.Core.IO.AnalysisXML.Output;
using InputLog.Core.Util;

namespace InputLog.Core.Merging.Analyses.Processors
{
    /// <summary>
    /// Class that processes analysis data in a way that allows it to 
    /// write the merged data in a CSV format that horizontally expands 
    /// on the analysis data.
    /// E.g:
    /// -----------------------------------------------------------------------
    /// //       (keys)  (values from file 1)     (values from file 4)
    /// -----------------------------------------------------------------------
    /// D        | Key1 | val11 | val 12 | val13 | val14 | val14 | ....
    /// A        | Key2 | val21 | val 22 | val23 | val24 | val24 | ....
    /// T        | Key3 | val31 | val 32 | val33 | val34 | val34 | ....
    /// A        | Key4 | val41 | val 42 | val43 | val44 | val44 | ....
    /// -----------------------------------------------------------------------
    /// H        | Key1 | val11 | val 12 | val13 | val14 | val14 | ....
    /// E        | Key2 | val21 | val 22 | val23 | val24 | val24 | ....
    /// A        | Key3 | val31 | val 32 | val33 | val34 | val34 | ....
    /// D        | Key4 | val41 | val 42 | val43 | val44 | val44 | ....
    /// E        | Key5 | val51 | val 52 | val53 | val54 | val54 | ....
    /// R        | Key6 | val61 | val 62 | val63 | val64 | val64 | ....
    /// +Filename| file | val71 | val 72 | val73 | val74 | val74 | ....
    /// ...
    /// -----------------------------------------------------------------------
    /// A new 'file' starts when the header information is filled in for that column.
    /// Otherwise it is just an event within the same file. It could be that in that
    /// case the other keys don't have a value for that column.
    /// E.g. when merging GeneralAnalysis files horizontally
    /// </summary>
    internal class HorizontalCSVProcessor : BasicProcessor
    {
        #region Fields
        /// <summary>
        /// Maps keys to line numbers.
        /// </summary>
        private Dictionary<string, Dictionary<int, int>> KeyToLine;

        /// <summary>
        /// Keeps the information to save to each line.
        /// </summary>
        private List<CSVLineBuilder> Lines;

        /// <summary>
        /// List that keeps track of the ids of all the lines that are not
        /// titles, but are not event lines either
        /// </summary>
        private List<int> NonEventLines;

        private const string MBE_SEP = "::";

        #endregion

        /// <summary>
        /// Construct the processor.
        /// </summary>
        /// <param name="workDir">Directory that can be used
        /// to write some temporary files and to write the result
        /// files too.</param>
        public HorizontalCSVProcessor(string workDir) : base(workDir)
        {
        }

        public override void Write(string filePath)
        {
            // Create the output file.
            var output = new StreamWriter(filePath, false, Encoding.Unicode);

            try
            {
                // Initialize the CSV builders etc.
                int maxSize = ((ChunkFiles.Count > 0) ? MAX_EVENT_ELEMENTS : 0) + ModuleKeyWidth + ExtraKeyWidth + HeaderWidth;
                KeyToLine = new Dictionary<string, Dictionary<int, int>>();
                Lines = new List<CSVLineBuilder>(maxSize);
                for (int i = 0; i < maxSize; i++)
                {
                    Lines.Add(new CSVLineBuilder());
                }

                // Write keys and create keyToLine map.
                WriteKeys();

                // Write the actual data.
                WriteData();

                // Save all the lines to the output file.
                foreach (var lineBuilder in Lines)
                {
                    output.WriteLine(lineBuilder.EndLine());
                }
                output.Close();
            }
            catch (Exception e)
            {
                var myMessage = "Horizontal merging failed\n";
                if (e.Message.Contains("Sequence contains no elements"))
                {
                    myMessage += "Please, rerun the Pause Analysis with a recent Inputlog version.\n" +
                              "Horizontal merging failed possibly because of a deprecated Pause Analysis format.\n" +
                                "The merging process will end with an empty *csv file.\n";
                }
                myMessage += "Failed file: '" + StringUtils.ShortenPathname(AnalysisMerge.CurrentFile, 50) + "'";
                MessageLogger.CatchException(this, e, Severity.ERROR, myMessage);
            }
        }

        private void WriteKeys()
        {
            int lineCounter = 0;
            NonEventLines = new List<int>();

            // Write header data keys
            var headerMap = new Dictionary<int, int>();
            KeyToLine.Add("header", headerMap);

            var keyCombiner = new StringBuilder();

            for (int keyIndex = 0; keyIndex < HeaderWidth; keyIndex++)
            {
                keyCombiner.Append("sessionID" + MBE_SEP);
                keyCombiner.Append(HeaderKeys[keyIndex]);
                // Write key and make mapping
                Lines[lineCounter].Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                headerMap.Add(keyIndex, lineCounter);
                NonEventLines.Add(lineCounter);
                lineCounter++;
                keyCombiner.Clear();
            }

            // Write Extra info data keys
            var extraMap = new Dictionary<int, int>();
            KeyToLine.Add("extra", extraMap);

            foreach (string extraTitle in ExtraKeys.Keys)
            {
                // Write key and make mapping
                Lines[lineCounter].Append(StringUtils.FilterSPSSInvalid(extraTitle, MBE_SEP));
                lineCounter++;

                foreach (var pair in ExtraKeys[extraTitle])
                {
                    keyCombiner.Append(extraTitle);
                    keyCombiner.Append(MBE_SEP);
                    keyCombiner.Append(pair.Key);
                    Lines[lineCounter].Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                    extraMap.Add(pair.Value, lineCounter);
                    NonEventLines.Add(lineCounter);
                    lineCounter++;
                    keyCombiner.Clear();
                }
            }

            // Write Module data keys
            var moduleMap = new Dictionary<int, int>();
            KeyToLine.Add("module", moduleMap);

            foreach (string moduleTitle in ModuleKeys.Keys)
            {
                Lines[lineCounter].Append(StringUtils.FilterSPSSInvalid(moduleTitle, MBE_SEP));
                lineCounter++;

                foreach (string blockTitle in ModuleKeys[moduleTitle].Keys)
                {
                    keyCombiner.Append(moduleTitle);
                    keyCombiner.Append(MBE_SEP);
                    keyCombiner.Append(blockTitle);
                    var l = Lines[lineCounter];
                    var kc = keyCombiner.ToString();
                    var s = StringUtils.FilterSPSSInvalid(kc, MBE_SEP);
                    l.Append(s);
                    moduleMap.Add(ModuleTitleKeys[moduleTitle][blockTitle], lineCounter);
                    lineCounter++;
                    keyCombiner.Clear();

                    foreach (var pair in ModuleKeys[moduleTitle][blockTitle])
                    {
                        keyCombiner.Append(moduleTitle);
                        keyCombiner.Append(MBE_SEP);
                        keyCombiner.Append(blockTitle);
                        keyCombiner.Append(MBE_SEP);
                        keyCombiner.Append(pair.Key);
                        Lines[lineCounter].Append(StringUtils.FilterSPSSInvalid(keyCombiner.ToString(), MBE_SEP));
                        moduleMap.Add(pair.Value, lineCounter);
                        NonEventLines.Add(lineCounter);
                        lineCounter++;
                        keyCombiner.Clear();
                    }
                }
            }
        }

        private void WriteData()
        {
            // Write data for each file
            for (int fileId = 0; fileId < FilesProcessed; fileId++)
            {
                // Write HeaderData
                WriteHeaderData(fileId);

                // Write ExtraInfoData
                WriteExtraData(fileId);

                // Write ModuleData
                WriteModuleData(fileId);

                // Write EventData
                //WriteEventData(fileId);
            }
        }

        private void WriteHeaderData(int fileId)
        {
            var headerMap = KeyToLine["header"];
            foreach (var headerPair in headerMap)
            {
                Lines[headerPair.Value].Append(HeaderData[headerPair.Key].ContainsKey(fileId)
                    ? HeaderData[headerPair.Key][fileId]
                    : EMPTY);
            }
        }

        private void WriteExtraData(int fileId)
        {
            var extraMap = KeyToLine["extra"];
            foreach (var extraPair in extraMap)
            {
                Lines[extraPair.Value].Append(ExtraData[extraPair.Key].ContainsKey(fileId)
                    ? ExtraData[extraPair.Key][fileId]
                    : EMPTY);
            }
        }

        private void WriteModuleData(int fileId)
        {
            var moduleMap = KeyToLine["module"];
            foreach (var modulePair in moduleMap)
            {
                Lines[modulePair.Value].Append(ModuleData[modulePair.Key].ContainsKey(fileId)
                    ? ModuleData[modulePair.Key][fileId]
                    : EMPTY);
            }
        }

        private void WriteEventData(int fileId)
        {
            // If there are any events for this file
            if (ChunkFiles.Contains(fileId))
            {
                // Open the chunkfile, process it line per line.
                string filePath = Path.Combine(TmpDir, CHUNK_NAME + fileId + CHUNK_EXT);
                var chunkReader = new StreamReader(filePath, Encoding.Unicode);

                bool firstEventLine = true;
                string chunkLine;

                // First chunkLine contains the headers.
                var chunkEventHeaders = CleanChunkLine(chunkReader.ReadLine()).ToList();
                while ((chunkLine = chunkReader.ReadLine()) != null)
                {
                    WriteSingleEvent(chunkLine, ref chunkEventHeaders, firstEventLine, fileId);
                    if (firstEventLine)
                    {
                        firstEventLine = false;
                    }
                }
            }
        }

        private void WriteSingleEvent(string chunkLine, ref List<string> chunkHeaders, bool firstEventLine, int fileId)
        {
            // Split the line & clean
            string[] chunkPieces = CleanChunkLine(chunkLine);
            Dictionary<int, int> eventMap = KeyToLine["event"];

            for (int keyId = 0; keyId < EventData.Keys.Count; keyId++)
            {
                // Line to save the content too
                int lineId = eventMap[keyId];

                // Content to save.
                if (chunkHeaders.Contains(EventData.Keys.ElementAt(keyId)))
                {
                    int idInChunk = chunkHeaders.IndexOf(EventData.Keys.ElementAt(keyId));
                    Lines[lineId].Append(chunkPieces[idInChunk]);
                }
                else
                {
                    Lines[lineId].Append(EMPTY);
                }
            }

            // If this is not the first event line we have to add emtpy
            // buffer values to all the other lines.
            if (!firstEventLine)
            {
                // Write HeaderData
                WriteHeaderData(fileId);

                // Write ExtraInfoData
                WriteExtraData(fileId);

                // Write ModuleData
                WriteModuleData(fileId);
            }
        }


        private static string[] CleanChunkLine(string chunkLine)
        {
            string[] chunkPieces = chunkLine.Split(CSVLineBuilder.SEPARATOR);
            // doesn't matter that it can be larger than the actual ammount of pieces in it.
            var cleanedPieces = new string[chunkPieces.Length];

            int index = 0;
            for (int i = 0; i < chunkPieces.Length; i++)
            {
                // Remove the enclosing marks from the string. (standard: ")
                // Detect a faulty split on an 'actual ;'
                // Cases:
                // 1. "...";";";"..." -> ["..."]["]["]["..."]  (lenght = 1 -> merge with next one )
                // 2. "... text ; some more text" --> ["... text][some more text"] (does not end with " -> merge with next one)
                // 3. "tekst";";tekst" --> ["tekst"]["][tekst"] (lenght = 1 -> mrege with next one)

                if (chunkPieces[i].Length == 1)
                {
                    cleanedPieces[index] = chunkPieces[i] + chunkPieces[++i];
                }
                else if (chunkPieces[i].ElementAt(chunkPieces[i].Length - 1) != '"')
                {
                    cleanedPieces[index] = chunkPieces[i] + chunkPieces[++i];
                }
                else
                {
                    cleanedPieces[index] = chunkPieces[i];
                }
                cleanedPieces[index] = cleanedPieces[index].Substring(1, cleanedPieces[index].Length - 2);

                index++;
            }
            return cleanedPieces;
        }
    }
}