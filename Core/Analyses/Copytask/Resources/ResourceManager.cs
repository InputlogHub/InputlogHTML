using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.IO.CSV;

namespace InputLog.Core.Analyses.Copytask.Resources
{
    /// <summary>
    ///     Handles the copyTask resources files. These are all the bigram files (by language)
    ///     and the different keyboard layouts available. 
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        ///     Keeps the bigram data for a language/keyboard setting stored in 
        ///     memory for future calls.
        /// </summary>
        private Dictionary<string, Dictionary<string, Bigrams.Bigram>> _bigramData;

        /// <summary>
        ///     Set to true when the class is initialized.
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        ///     List of the languages that are supported. These are 
        ///     two letter, official acronyms of the languages.
        ///     For each language in the list we have a bigram file with frequency
        ///     information etc. The file is indexed by bigram.
        /// </summary>
        private List<string> _supported_languages;

        /// <summary>
        ///     List of the languages that are supported. These are 
        ///     two letter, official acronyms of the languages.
        ///     For each language in the list we have a bigram file with frequency
        ///     information etc. The file is indexed by bigram.
        /// </summary>
        public IEnumerable<string> SupportedLanguages => _supported_languages.AsReadOnly();

        /// <summary>
        ///     List of the keyboard layouts that are supported.
        ///     These are the keyboard layout identifiers and for each layout
        ///     in this list, we have a bigram file with the adjacency, 
        ///     repetition and hand combination information. The file
        ///     is indexed by bigram.
        /// </summary>
        private List<string> _supported_layouts;

        /// <summary>
        ///     List of the keyboard layouts that are supported.
        ///     These are the keyboard layout identifiers and for each layout
        ///     in this list, we have a bigram file with the adjacency, 
        ///     repition and hand combination information. The file
        ///     is indexed by bigram.
        /// </summary>
        public IEnumerable<string> SupportedLayouts => this._supported_layouts.AsReadOnly();

        private const string LANGUAGE_PREFIX = "bigr_";
        private const string LAYOUT_PREFIX = "kb_";
        private const string RESOURCES_FOLDER = @"Analyses\Copytask\Resources";
        private const string RESOURCE_EXTENSION = ".csv";
        private const string FOREIGN_KEY = "bigram";
        private const char CSV_SEP = ';';
        private string EXECUTION_PATH;

        /// <summary>
        ///     Create a new resource manager. 
        /// </summary>
        public ResourceManager()
        {
            this._bigramData = new Dictionary<string, Dictionary<string, Bigrams.Bigram>>();

            this.EXECUTION_PATH = AppContext.BaseDirectory;
            string resourcePath = Path.Combine(this.EXECUTION_PATH, RESOURCES_FOLDER);

            this._supported_languages = _DetectPostfixInFileNames(resourcePath, LANGUAGE_PREFIX);
            this._supported_layouts = _DetectPostfixInFileNames(resourcePath, LAYOUT_PREFIX);
        }

        /// <summary>
        ///     Detect all the files in given folder that begin with given
        ///     prefix. Return a list of postfixes of all the filenames (without extension) 
        ///     found that begin with given prefix, and then removing that
        ///     prefix from the filename.
        /// </summary>
        /// <param name="parentFolder">The folder in which we will search for files.</param>
        /// <param name="prefix">The prefix we will search for, and then remove from
        /// the filename.</param>
        /// <returns>A list of files that started with a given prefix, have their prefix filtered
        /// off, and their extension removed.</returns>
        private List<string> _DetectPostfixInFileNames(string parentFolderPath, string prefix)
        {
            DirectoryInfo parentFolder = new DirectoryInfo(parentFolderPath);
            Debug.Assert(parentFolder.Exists, "Bigram resources can not be found");

            List<string> returnValue = new List<string>();

            FileInfo[] files = parentFolder.GetFiles(prefix + "*");
            foreach (FileInfo file in files)
            {
                string originalFilenameNoExt = Path.GetFileNameWithoutExtension(file.Name);
                string filteredName = originalFilenameNoExt.Substring(prefix.Length);
                returnValue.Add(filteredName);
            }

            return returnValue;
        }


        /// <summary>
        ///     Get the bigram resource for the given language and keyboard
        ///     layout combination. If the combination does not exist. This 
        ///     method will throw an exception.
        /// </summary>
        /// <param name="language">Language code for the bigrams.</param>
        /// <param name="layout">Keyboard layout code for the bigrams.</param>
        /// <returns>A dictionary containing all the bigrams with the specific information
        /// for the specified language/layout combination.</returns>
        public Dictionary<string, Bigrams.Bigram> GetBigrams(string language, string layout)
        {
            // Check requested data.
            if (!_supported_languages.Contains(language))
            {
                throw new FileNotFoundException("Language (" + language + ") is not a supported copyTask language.");
            }

            if (!_supported_layouts.Contains(layout))
            {
                throw new FileNotFoundException("Layout (" + layout + ") is not a supported copyTask keyboard layout.");
            }

            // Load the bigram data if it isn't loaded yet.
            string identifier = language + "_" + layout;
            if (!_bigramData.ContainsKey(identifier))
            {
                Dictionary<string, Bigrams.Bigram> data = this._loadBigramData(language, layout);
                _bigramData.Add(identifier, data);
            }
            return _bigramData[identifier];
        }
        
        /// <summary>
        ///     Load the bigram data for given language/layout combination.
        /// </summary>
        /// <param name="language">The requested language</param>
        /// <param name="layout">The requested keyboard layout</param>
        /// <returns>The bigram data with all the characteristics for givne
        /// combination of language and keyboard layout.</returns>
        private Dictionary<string, Bigrams.Bigram> _loadBigramData(string language, string layout)
        {
            string languageFilename = LANGUAGE_PREFIX + language + RESOURCE_EXTENSION;
            string languageFilePath = Path.Combine(EXECUTION_PATH, RESOURCES_FOLDER, languageFilename);

            CSVMergeReader<Bigrams.Bigram> reader = new CSVMergeReader<Bigrams.Bigram>(FOREIGN_KEY, CSV_SEP);
            reader.ReadFile(languageFilePath);

            string layoutFilename = LAYOUT_PREFIX + layout + RESOURCE_EXTENSION;
            string layoutFilePath = Path.Combine(EXECUTION_PATH, RESOURCES_FOLDER, layoutFilename);
            reader.ReadFile(layoutFilePath);

            var fileContents = reader.GetFileContent();
            return fileContents.ItemsAsDictionary;
        }
    }
}
