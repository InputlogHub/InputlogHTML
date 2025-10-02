using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InputLog.Core.IO.AnalysisXML.Output;
using InputLog.Core.IO.AnalysisXML.XML;
using InputLog.Core.IO.AnalysisXML.XML.Parts;

namespace InputLog.Core.Merging.Analyses.Processors
{
    /// <summary>
    /// Interface for the processors. The processors organize the analysis data from 
    /// in an orderly way. 
    /// </summary>
    internal abstract class BasicProcessor
    {
        #region Fields

        /// <summary>
        /// The max number of tags 'events' can have. 
        /// </summary>
        protected const int MAX_EVENT_ELEMENTS = AnalysisXML.Session.Event.MAXELEMENTS;

        /// <summary>
        /// First part of the name for temporary chunk files.
        /// </summary>
        protected const string CHUNK_NAME = "Chunk-";

        /// <summary>
        /// Extension for chunk files.
        /// </summary>
        protected const string CHUNK_EXT = ".csv";

        /// <summary>
        /// Name of temporary directory used to store chunks
        /// </summary>
        private const string TMP_DIR_NAME = "tmp";

        protected const string EMPTY = "";

        /// <summary>
        /// Get current culture.
        /// </summary>
        private readonly CultureInfo Culture = CultureInfo.CurrentCulture;

        /// <summary>
        /// List that keeps track of the which processed files had chunkfiles.
        /// Whenever a processed file has a chunkfile it is saved with it's index
        /// being equal to the index of its header information.
        /// </summary>
        protected readonly HashSet<int> ChunkFiles;

        /// <summary>
        /// Counter to keep track of how many events have already been processed.
        /// </summary>
        private int EventCount;

        /// <summary>
        /// Event data of a single file. The event data can be quite large and therefore
        /// is not kept for all the files but saved on a file per file basis. This data
        /// is saved in a chunk file after the file has been completely processed. 
        /// </summary>
        protected readonly Dictionary<string, List<string>> EventData;

        /// <summary>
        /// TODO check comments
        /// The extra info data. This data persists through the different file calls, as 
        /// due to efficiency considerations it is not considered a big impact on the memory
        /// to keep the extraInfo data in the memory through the different file calls.
        /// (Note: we assume that extra info data is not more than a few values per file
        /// to be merged. A few being no more than 150.)
        /// (Note2: We use a dictionary as not every extraKeyId has values... e.g.
        /// Title keys do not have corresponding values and thus are never set
        /// in the dictionary)
        /// </summary>
        protected readonly Dictionary<int, Dictionary<int, string>> ExtraData;

        /// <summary>
        /// Width of all the extra info keys. This includes
        /// titles as well as actual key tags.
        /// </summary>
        protected int ExtraKeyWidth;

        /// <summary>
        /// Dictionary of all the keys of extra info. They are hierachically organized
        /// on a per extraInfoTitle basis. Each key within an extra info with given title
        /// is assigned a unique id. This extraKeyId is the index in the data array for 
        /// all the data read for that particular key.
        /// </summary>
        protected readonly Dictionary<string, Dictionary<string, int>> ExtraKeys;

        /// <summary>
        /// The number of files processed so far.
        /// </summary>
        protected int FilesProcessed;

        /// <summary>
        /// TODO check comments
        /// All the headers information so far. The index of the dictionary
        /// corresponds to the index of the key in the headerkey list. The 
        /// list (ordered on a per key basis) can have empty values, if that 
        /// specific headerkey was not set for the given header.
        /// </summary>
        protected readonly Dictionary<int, Dictionary<int, string>> HeaderData;

        /// <summary>
        /// All the tags currently encountered in header objects.
        /// </summary>
        protected readonly List<string> HeaderKeys;

        /// <summary>
        /// The width of the header. This is equal to the largest number 
        /// of keys a header has had till now.
        /// </summary>
        protected int HeaderWidth;

        /// <summary>
        /// TODO check comments
        /// Module data accross the different files. The data encountered in the files is mapped
        /// to the specific moduleKeyId within the list. Values can be null.
        /// The value of a specific files entry for the given module key can be accessed by 
        /// using the header index for that file in the list of 'module data per moduleKeyId'.
        /// This data is kept in memory for all files to process as its not considered a big
        /// hit to memory usage since module values are limited to 1 value per key per file and
        /// we don't expect more than a few hundred values per file.
        /// (Note: We use a dictionary as not every moduleKeyId has values... e.g.
        /// Title keys do not have corresponding values and thus are never set
        /// in the dictionary)
        /// </summary>
        protected readonly Dictionary<int, Dictionary<int, string>> ModuleData;

        //-----------------------------
        // Module fields
        //

        /// <summary>
        /// The number of module keys encountered so far, this includes titles of
        /// modules and blocks.
        /// </summary>
        protected int ModuleKeyWidth;

        /// <summary>
        /// The module keys that have been encountered so far. They are categorized first
        /// on module title, then on block title. And lastly the keys within that specific block
        /// map to their moduleKeyId, which is then used for accessing the data in the ModuleData
        /// datamember.
        /// </summary>
        protected readonly Dictionary<string, Dictionary<string, Dictionary<string, int>>> ModuleKeys;

        /// <summary>
        /// Separate dictionary to map the keys of blocks to their keyId that can 
        /// be used as keyId in the value array.
        /// </summary>
        protected readonly Dictionary<string, Dictionary<string, int>> ModuleTitleKeys;

        /// <summary>
        /// Complete path to the temporary working directory.
        /// </summary>
        protected readonly string TmpDir;

        /// <summary>
        /// Directory that can be used to write some temporary files and to write the result
        /// files too. 
        /// </summary>
        private string WorkDir;

        #endregion

        /// <summary>
        /// Construct the processor.
        /// </summary>
        /// <param name="workDir">
        /// Directory that can be used to write some temporary files 
        /// and to write the result files too.
        /// </param>
        /// <summary>
        /// Construct the processor.
        /// </summary>
        protected BasicProcessor(string workDir)
        {
            WorkDir = workDir;

            // Create tmp working directory
            TmpDir = Path.Combine(workDir, TMP_DIR_NAME);
            Directory.CreateDirectory(TmpDir);

            // Initialize datamembers
            HeaderWidth = 0;
            HeaderKeys = new List<string>();
            HeaderData = new Dictionary<int, Dictionary<int, string>>();
            ExtraKeyWidth = 0;
            ExtraKeys = new Dictionary<string, Dictionary<string, int>>();
            ExtraData = new Dictionary<int, Dictionary<int, string>>();
            ModuleKeyWidth = 0;
            ModuleKeys = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            ModuleTitleKeys = new Dictionary<string, Dictionary<string, int>>();
            ModuleData = new Dictionary<int, Dictionary<int, string>>();
            EventData = new Dictionary<string, List<string>>();
            ChunkFiles = new HashSet<int>();
        }

        #region BeginFile

        /// <summary>
        /// Preparing a file
        /// </summary>
        /// <param name="header">Session information for this file</param>
        public void BeginFile(Header header)
        {
            // Reset event data. (except the keys)
            EventCount = 0;
            List<string> eventKeys = EventData.Keys.ToList();
            foreach (string eventKey in eventKeys)
            {
                EventData[eventKey] = new List<string>();
            }

            // Process Header
            if (HeaderWidth < header.Properties.Count)
            {
                CreateNewHeaderKeys(header);
            }

            // When users are allowed to write the Session keys, 
            // we have little control on how they handle the case of the first letter.
            for (int keyId = 0; keyId < HeaderWidth; keyId++)
            {
                string key = HeaderKeys[keyId];
                string lKey = key.ToLower(Culture);
                string uKey = UppercaseFirst(key);

                // Add value of the key to the value list
                if (header.Properties.ContainsKey(key))
                {
                    HeaderData[keyId].Add(FilesProcessed, header.Properties[key]); 
                }
                else if (header.Properties.ContainsKey(lKey))
                {
                    HeaderData[keyId].Add(FilesProcessed, header.Properties[lKey]); 
                }
                else if (header.Properties.ContainsKey(uKey))
                {
                    HeaderData[keyId].Add(FilesProcessed, header.Properties[uKey]); 
                }            
            }
        }

        /// <summary>
        /// Uppercasing the first letter of a word.
        /// </summary>
        /// <param name="s">The original key</param>
        /// <returns>The key with first letter uppercased.</returns>
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Checks which keys in a header are new keys and adds them to the key
        /// list. It also takes care of filling in empty information for all the files
        /// that have already been processed before the new keys have been added.
        /// </summary>
        /// <param name="header">Header object containing new keys.</param>
        private void CreateNewHeaderKeys(AnalysisData header)
        {
            // Detect new header keys
            var newKeys = header.Properties.Keys.Where(newKey => !HeaderKeys.Contains(newKey)).ToList();

            foreach (string newKey in newKeys)
            {
                // Add the new key properties to the headerKeyList
                int newKeyIndex = HeaderKeys.Count;
                HeaderKeys.Add(newKey);

                // Create a new dictionary for storing the values associated with the key.
                HeaderData.Add(newKeyIndex, new Dictionary<int, string>());
            }
            HeaderWidth += newKeys.Count;
        }

        #endregion

        #region ExtraInfo

        public void ProcessExtraInfo(ExtraInfo extra)
        {
            // Check if there are new keys in this extraInfo part, 
            // and if so, add them to the key list.
            CheckForNewExtraKeys(extra);

            // Add values to the extraData list.
            foreach (var entry in extra.Properties)
            {
                int keyId = ExtraKeys[extra.Title][entry.Key];
                // FilesProcess is the Index at which data values have to be saved in their respective key lists.
                ExtraData[keyId].Add(FilesProcessed, entry.Value);
            }
        }

        private void CheckForNewExtraKeys(AnalysisData extra)
        {
            Dictionary<string, int> titleKeys;

            // This title already exists, get the keys associated with it.
            if (ExtraKeys.ContainsKey(extra.Title))
            {
                titleKeys = ExtraKeys[extra.Title];
            }
            // This title extra part title does not yet exist, we add it.
            else
            {
                titleKeys = new Dictionary<string, int>();
                ExtraKeys.Add(extra.Title, titleKeys);
                ExtraKeyWidth++;
            }

            // Check if there are any new keys within the title.
            // We get all the keys in the extra object and remove any keys
            // that we already know of. This leaves us with a collection 
            // of the new keys under the 'title'.
            ICollection<string> extraNewKeys = extra.Properties.Keys.ToList();
            foreach (string existingKey in titleKeys.Keys)
            {
                if (extraNewKeys.Contains(existingKey))
                {
                    extraNewKeys.Remove(existingKey);
                }
            }

            // There are new keys in this extra info object.
            if (extraNewKeys.Count <= 0) return;
            foreach (string newKey in extraNewKeys)
            {
                titleKeys.Add(newKey, ExtraKeyWidth);
                ExtraData.Add(ExtraKeyWidth, new Dictionary<int, string>());
                ExtraKeyWidth++;
            }
        }

        #endregion

        #region Module

        public void ProcessModule(Module module)
        {
            if (IgnoreForMerge(module)) return;
            // Check if there's any new keys in the module
            CheckForNewModuleKeys(module);


            // Add the values of the module in the list.
            foreach (AnalysisData block in module.Children)
            {
                if (IgnoreForMerge(block)) continue;
                var aBlock = (Block) block;
                if (aBlock.Value != "")
                {
                    int keyId = ModuleTitleKeys[module.Title][block.Title];
                    ModuleData[keyId].Add(FilesProcessed, aBlock.Value);
                }

                foreach (var entry in block.Properties)
                {
                    int keyId = ModuleKeys[module.Title][block.Title][entry.Key];
                    try
                    {
                        ModuleData[keyId].Add(FilesProcessed, entry.Value);
                    }
                    catch (Exception)
                    {
                        // Jump over a faulty file.
                        Debug.WriteLine(" KeyId " + keyId);
                    }
                }
            }
        }

        private static bool IgnoreForMerge(AnalysisData moduleOrBlock)
        {
            return (moduleOrBlock.Attributes.ContainsKey("ignoreForMerge") && moduleOrBlock.Attributes["ignoreForMerge"] == "1");
        }

        private void CheckForNewModuleKeys(Module module)
        {
            // Check if the title is already in the keyset.
            Dictionary<string, Dictionary<string, int>> blockKeys;
            Dictionary<string, int> blockTitleKeys;
            if (ModuleKeys.ContainsKey(module.Title))
            {
                blockKeys = ModuleKeys[module.Title];
                blockTitleKeys = ModuleTitleKeys[module.Title];
            }
            else
            {
                blockKeys = new Dictionary<string, Dictionary<string, int>>();
                blockTitleKeys = new Dictionary<string, int>();
                ModuleKeys.Add(module.Title, blockKeys);
                ModuleTitleKeys.Add(module.Title, blockTitleKeys);
                ModuleKeyWidth++;
            }

            // Check if the title of the blocks already exist in the module's keyset.
            foreach (AnalysisData block in module.Children)
            {
                if (IgnoreForMerge(block)) continue;
                Dictionary<string, int> keys;
                if (blockKeys.ContainsKey(block.Title))
                {
                    keys = blockKeys[block.Title];
                }
                else
                {
                    keys = new Dictionary<string, int>();
                    blockKeys.Add(block.Title, keys);
                    blockTitleKeys.Add(block.Title, ModuleKeyWidth);
                    ModuleData.Add(ModuleKeyWidth, new Dictionary<int, string>());
                    ModuleKeyWidth++;
                }

                // Check if the keys inside the block are already in the keylist.
                foreach (string key in block.Properties.Keys)
                {
                    if (!keys.ContainsKey(key))
                    {
                        keys.Add(key, ModuleKeyWidth);
                        ModuleData.Add(ModuleKeyWidth, new Dictionary<int, string>());
                        ModuleKeyWidth++;
                    }
                }
            }
        }

        #endregion

        #region Event

        public void ProcessEvent(Event even, string output)
        {
            // check for new keys.
            CheckForNewEventKeys(even);

            // Double quotes in analyses with text are replaced with single quotes to prevent  
            // the csv-builder using the double quote as a text delimiter.
            if (!output.Equals("none"))
            {
                if (even.Properties[output].Equals("\""))
                {
                    even.Properties[output] = even.Properties[output].Replace("\"", "'");
                }
            }

            // Add values for each key.
            foreach (string key in EventData.Keys)
            {
                EventData[key].Add(even.Properties.ContainsKey(key) ? even.Properties[key] : EMPTY);
            }

            // Increase events processed counters
            EventCount++;
        }

        private void CheckForNewEventKeys(Event even)
        {
            foreach (string eventKey in even.Properties.Keys)
            {
                if (!EventData.ContainsKey(eventKey))
                {
                    var values = new List<string>(EventCount + 1);
                    EventData.Add(eventKey, values);

                    // fill values array with empty values for all the previous events
                    for (var i = 0; i < EventCount; i++)
                    {
                        values.Add(EMPTY);
                    }
                }
            }
        }

        #endregion

        #region endFile

        public void EndFile()
        {
            // If there were any events in this file, we write them to a chunk file.
            if (EventCount > 0)
            {
                WriteChunk();
                // Add the this file id to the list of files having chunks.
                ChunkFiles.Add(FilesProcessed);
            }

            FilesProcessed++;
        }

        private void WriteChunk()
        {
            // Create the chunk file. & stream.
            string fileName = CHUNK_NAME + FilesProcessed + CHUNK_EXT;
            string filePath = Path.Combine(TmpDir, fileName);

            // Holds the chunk file content + 1 header line
            var chunkLines = new string[EventCount + 1];

            // Write header line.
            var lineBuilder = new CSVLineBuilder();//MAX_EVENT_ELEMENTS);
            lineBuilder.BeginLine();
            for (int keyIndex = 0; keyIndex < EventData.Keys.Count; keyIndex++)
            {
                lineBuilder.Append(EventData.Keys.ElementAt(keyIndex));
            }
            chunkLines[0] = lineBuilder.EndLine();

            // Create chunkfile content
            for (int i = 0; i < EventCount; i++)
            {
                Dictionary<string, List<string>>.KeyCollection eventKeys = EventData.Keys;

                // Process values
                lineBuilder.BeginLine();
                for (int keyIndex = 0; keyIndex < eventKeys.Count; keyIndex++)
                {
                    // Remove new lines from the values here. Otherwise we get wrong CSV information.
                    lineBuilder.Append(EventData[eventKeys.ElementAt(keyIndex)][i]);
                }

                chunkLines[i + 1] = lineBuilder.EndLine();
            }

            // Write the content to a file.
            File.WriteAllLines(filePath, chunkLines, Encoding.Unicode);
        }

        #endregion

        //---------------------------------------------------------------------
        // Standard processor methods
        //

        //-----------------------------
        // Process Header
        //

        //-----------------------------
        // Process Extra info
        //

        //-----------------------------
        // Process a module
        //

        //-----------------------------
        // Process an event
        //

        //-----------------------------
        // Ending of the file:
        // Finish any outstanding processing.
        //

        //---------------------------------------------------------------------
        // Write away all the processed data
        //

        /// <summary>
        /// Method to write away all the processed data. 
        /// <param name="filePath">Filepath of the file to save the data to.</param>
        /// </summary>
        public abstract void Write(string filePath);

        #region Clean

        /// <summary>
        /// Clean up any resources used by the processor. Remove any temporary
        /// files and directories created.
        /// </summary>
        public void Clean()
        {
            // Remove the tmp folder and the chunk files.
            try
            {
                if (Directory.Exists(TmpDir))
                {
                    Directory.Delete(TmpDir, true);
                }
            }
            catch
            {
                // Ignore. It's not that important anyway. 
            }
        }

        #endregion

        //-----------------------------
        // Cleanup
        //
    }
}