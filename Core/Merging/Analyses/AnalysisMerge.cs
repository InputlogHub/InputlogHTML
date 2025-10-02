using System;
using System.Collections.Generic;
using System.IO;
using InputLog.Core.IO.AnalysisXML.Input;
using InputLog.Core.IO.AnalysisXML.XML.Parts;
using InputLog.Core.Util;
using System.Threading;
using InputLog.Core.Merging.Analyses.Processors;

namespace InputLog.Core.Merging.Analyses
{
	public class AnalysisMerge
	{
		#region Fields

		/// <summary>
		/// Direction of merging. This can be either horizontal or vertical.
		/// </summary>
		public enum Direction
		{
			HORIZONTAL,
			VERTICAL,
		}

		/// <summary>
		/// Files that have to be merged.
		/// </summary>
		private readonly List<string> ToMerge;

		/// <summary>
		/// Factory for creating analysis readers
		/// </summary>
		// ReSharper disable once NotAccessedField.Local
		private readonly AnalysisReaderFactory ReaderFactory;

		/// <summary>
		/// Reader used for reading the analysis xml files.
		/// </summary>
		private readonly AnalysisXMLReader FileReader;

        /// <summary>
        /// Reader to extract the timestamp from the files to merge.
        /// </summary>
	    private readonly LogCreationReader TimeReader;

		/// <summary>
		/// The number of files that has already been read;
		/// </summary>
		public int Processed { private set; get; }

		/// <summary>
		/// Bool that gets set if you want to stop the processing.
		/// </summary>
		private volatile bool Stop;

		/// <summary>
		/// Directory we store the result files in and in which we can
		/// create some temporary files.
		/// </summary>
		private readonly string WorkingDirectory;

        private readonly List<string> Errors;
        public static string CurrentFile { private set; get; }
		#endregion

		/// <summary>
		/// Construct the analysis xml merging class. 
		/// </summary>
		/// <param name="filePaths">List with filepath to all the files that
		/// have to be merged.</param>
        /// <param name="workDir">The directory to put the csv file in</param>
		public AnalysisMerge(List<string> filePaths, string workDir)
		{
			if (filePaths == null)
			{
				throw new ArgumentNullException();
			}
			if (filePaths.Count == 0)
			{
				throw new ArgumentException("List of files to merge is empty");
			}
			ToMerge = filePaths;

			// Initialize other datamembers
			ReaderFactory = new AnalysisReaderFactory();
			FileReader = new AnalysisXMLReader();
            TimeReader = new LogCreationReader();
			Processed = 0;
			Stop = false;
			WorkingDirectory = workDir;
            Errors = new List<string>();
            CurrentFile = "";
		}

		//---------------------------------------------------------------------------
		//  Merge analysis files.
		//

		public void Merge(Direction direction)
		{
			// Preprocess files to order them on per analysis type basis and on the timestamp of their creation.
            Dictionary<AnalysisType, SortedDictionary<DateTime, string >> filePerType = PreprocessFiles(ToMerge);

			// Create the processor
			BasicProcessor processor = null;
            try
            {
				// Per analysis type
                foreach (KeyValuePair<AnalysisType, SortedDictionary<DateTime, string>> typePair in filePerType)
				{
					// Create the processor
					processor = ProcessorFactory.Create(direction, WorkingDirectory);

                    // The path to the file to process
				    var path = "";

					// For each file of that type
					foreach (KeyValuePair<DateTime, string> pathPair in typePair.Value)
					{
                        try
			            {
						    // Exit loop
						    if (Stop)
						    {
							    break;
						    }

						    // Read the file.
					        path = pathPair.Value;
                            CurrentFile = path;
						    FileReader.Read(path, typePair.Key, pathPair.Key);

						    // Process the file.
						    if (FileReader.Successful)
						    {
                                Process(processor, typePair.Key.ToString());
						    }
                        }
                        catch (Exception e)
			            {
                            AddError(CurrentFile);
				            MessageLogger.CatchException(this, e, Severity.ERROR, e.Message);
			            }

                        Processed++;
					}
					// Exit loop
					if (Stop)
					{
						break;
					}

					// Get filename to save file too.
                    // If there is only one file, the original 'xml' extension is changed into 'csv'.
				    string fileName;
                    if(typePair.Value.Count == 1)
                    {
                        fileName = Path.ChangeExtension(path, ".csv");
                    }
                    else
                    {
                        fileName = typePair.Key + "_MERGED.csv";
                    }
					string filePath = Path.Combine(WorkingDirectory, fileName);
					filePath = PathSanitizer.Uniquify(filePath);

					processor.Write(filePath);
					processor.Clean();
                }
			}
			catch (ThreadAbortException)
			{
				// We ignore this error. The user only cancelled the merging action, no need to panick.
			}
			finally
			{
				// Cleanup...
				if (processor != null)
				{
					processor.Clean();
				}
			}
		}

        private void AddError(string currentFile)
        {
            Errors.Add(currentFile);
        }


	    /// <summary>
	    /// Process a single file. The information that has been read from the file is
	    /// currently stored in the FileReader datamembers. We will use this
	    /// information to process the file.
	    /// </summary>
	    /// <param name="processor">Vertical or horizontal processor</param>
	    /// <param name="analysisType">The analysis type to process</param>
	    private void Process(BasicProcessor processor, string analysisType)
		{
            processor.BeginFile(FileReader.Header);

			if (FileReader.Extras != null && FileReader.Extras.Count > 0)
			{
				foreach (ExtraInfo extra in FileReader.Extras)
				{
					processor.ProcessExtraInfo(extra);
				}
			}
			if (FileReader.Modules != null && FileReader.Modules.Count > 0)
			{
				foreach(Module module in FileReader.Modules)
				{
					processor.ProcessModule(module);
				}
			}
			if (FileReader.Events != null && FileReader.Events.Count > 0)
			{
			    string output = CheckOutput(analysisType);
				foreach (Event even in FileReader.Events)
				{
					processor.ProcessEvent(even, output);
				}
			}

			processor.EndFile();
		}

        /// <summary>
        /// Double quotes in the text are replaced with single quotes to prevent interference with the csv-builder
        /// who sees the double quote as a text delimiter.
        /// Different analyses have different event properties containing the output string, hence a check on type.
        /// </summary>
        /// <param name="type">The type of analysis to process</param>
        /// <returns>The name of the event property holding the text string</returns>
        private static string CheckOutput(string type)
	    {
            string output;
            switch(type)
            {
                case "GENERAL":
                case "GENERAL_EYETRACK":
                    output = "output";
                    break;
                case "REVISION":
                    output = "content";
                    break;
                case "LINGUISTIC":
                case "WORD_PAUSES":
                    output = "CharsProduced";
                    break;
                default:  
                    output = "none";
                    break;
            }
	        return output;
	    }
           
		/// <summary>
		/// Organize the files on analysis type. All the files of the same analysis
		/// type are put together in a list. They are also sorted in ascending order based on the creation date
		/// of the log file, so as to have the oldest files first.
		/// </summary>
		/// <param name="filePaths">List of filepaths to all the files that have to be merged.</param>
		/// <returns>A dictionary based keyed on analysis type. The Values in the dictionary
		/// is a list of all the files of that analysis type, that have to be merged.</returns>
        private Dictionary<AnalysisType, SortedDictionary< DateTime, string>> PreprocessFiles(IEnumerable<string> filePaths)
		{
            // Init
            var filePerType = new Dictionary<AnalysisType, SortedDictionary<DateTime, string>>();
            var r = new Random();
            // Organize files according to their type
            foreach (var path in filePaths)
            {
                try
                {
                    CurrentFile = path;
                    var timeStamp = TimeReader.GetCreationTime(path);
                    var type = AnalysisReaderFactory.GetType(path);

                    // If a list of given type already exists, just add
                    if (filePerType.ContainsKey(type))
                    {
                        try
                        {
                            filePerType[type].Add(timeStamp, path);
                        }
                        // When files originating from the same person and the same session
                        // have different pause thresholds, we need to tweek the timeStamp...
                        catch (ArgumentException)
                        {
                            filePerType[type].Add(timeStamp.AddSeconds(r.NextDouble()), path);
                        }
                    }
                    // Otherwise create a new list and add it with type as key.
                    else
                    {
                        var files = new SortedDictionary<DateTime, string> { { timeStamp, path } };
                        filePerType.Add(type, files);
                    }
                }
                catch (Exception)
                {
                    Processed++;
                    AddError(CurrentFile);
                }
            }
            return filePerType;
		}

		//---------------------------------------------------------------------------
		// Stopping the merging
		//

		public void StopProcessing()
		{
			Stop = true;
		}

        public bool HasErrors()
        {
            return Errors.Count > 0;
        }

        public IEnumerable<string> GetErrors()
        {
            return Errors;
        }
    }
}
