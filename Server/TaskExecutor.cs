using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using InputLog.Core.Util;
using InputLog.Core.Analyses;
using System.ServiceModel;
using InputLog.Core.Util.Server;
using InputLog.Core.IO;
using System.Configuration;
using InputLog.Core.Util.Xml;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace Server
{
    /// <summary>
    /// Class that actually executes Tasks, in a separate Thread.
    /// </summary>
    internal class TaskExecutor
    {
        private readonly TaskWrapper _task;
        private readonly DirectoryInfo TaskDir;
        private readonly Thread Thread;
        private readonly string GUID;

        public TaskWrapper Task
        {
            get { return _task; }
        }

        private int Count;
        private bool Finished;
        private bool ServerDown;
        private bool Restart;
        private AnalysisDescription CurrentDescr;
        private IAnalysisSummary CurrentSummary;
        private XMLSerializableList<ServerAnalysisException> Exceptions;
        public TaskResult Result
        {
            get;
            private set;
        }
        public bool IsRerun
        {
            get;
            set;
        }

        public TaskExecutor(TaskWrapper task, bool isRerun = false)
        {
            _task = task;
            Thread = new Thread(Start);
            GUID = task.GetTaskGuid();
            string path = Path.Combine(ConfigurationManager.AppSettings["TaskDir"], task.GetUserID(), task.GetTaskGuid());
            TaskDir = new DirectoryInfo(path);
            IsRerun = isRerun;
        }

        public void Start()
        {
            Exceptions = new XMLSerializableList<ServerAnalysisException>();
            Finished = false;
            Restart = false;
            ServerDown = false;
            Execute();
            if (!Finished)
            {
                while (!Restart)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(600));
                }
                Start();
            }
        }

        private void Execute()
        {
            try
            {
                List<FileInfo> inputFiles = GetInputFiles();

                Count = 1;
                foreach (FileInfo file in inputFiles)
                {
                    ServerAnalysisException e = ProcessInputFile(file);
                    if (e != null)
                    {
                        Exceptions.Add(e);
                    }
                    Count++;
                }
                SerializeExceptions();
                Result = GetTaskResult();
                ZipResults();


                // Remove unnecessary files - e.g. Summaries.zip, Summary.xml
                CleanUp();
                Finished = true;

            }
            catch (ServerDownException e)
            {
                Console.WriteLine("EXCEPTION: " + e.Message);
                Result = new TaskResult(this.GUID, TaskResult.ResCode.FAILED);
            }
            catch (Exception e)
            {
                // This should not really happen ...
                Finished = true;
                Console.WriteLine("EXCEPTION: " + e.Message);
                Result = new TaskResult(this.GUID, TaskResult.ResCode.FAILED);
            }
        }

        private List<FileInfo> GetInputFiles()
        {
            string inputDirPath = Path.Combine(TaskDir.FullName, "Input");
            // If the Input dir already exists this is a rerun and we have already 
            // extracted the zipfiles on the previous run.
            if (!Directory.Exists(inputDirPath))
            {
                ExtractZip();
            }

            // Foreach result file in the TaskDir remove that file from the list of 
            // input files.
            const string RESULT_FILE_PATTERN = "_LG.xml"; // or _LG.html
            const string INPUT_FILE_PATTERN = "input_";
            List<FileInfo> inputFiles = new List<FileInfo>(TaskDir.EnumerateFiles(INPUT_FILE_PATTERN + "*"));
            List<FileInfo> resultFiles = new List<FileInfo>(TaskDir.EnumerateFiles("*" + RESULT_FILE_PATTERN));

            Console.WriteLine("- Inputfiles [original]: " + inputFiles.Count.ToString());

            //foreach (FileInfo resultFile in TaskDir.EnumerateFiles("*" + RESULT_FILE_PATTERN))
            //{
            //    string originalFileName = resultFile.Name;
            //    string baseFileName = originalFileName.Substring(0, originalFileName.Length - RESULT_FILE_PATTERN.Length);
            //    string derivedInputFileName = baseFileName + ".idfx.xml.gz";
            //    inputFiles.Remove(inputFiles.Single(info => info.Name == derivedInputFileName));
            //}

            for (int i = 0; i < inputFiles.Count; )
            {
                FileInfo inputFile = inputFiles[i];

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(inputFile.FullName);
                XmlNode logFileNameSiblingNode = xmlDoc.SelectSingleNode("//log/meta/entry/key[text() = '__LogFileName']");

                if (logFileNameSiblingNode != null)
                {
                    string origInputFileName = logFileNameSiblingNode.NextSibling.InnerText;
                    string baseFileName = origInputFileName.Substring(0, origInputFileName.Length - ".idfx".Length);
                    string derivedResultFileName = baseFileName + RESULT_FILE_PATTERN;

                    // if there exists a resultfile such that its name equals what would be the 
                    // resultFileName of the current input file, then we remove that inputFile 
                    // from the list of "files to process", as it has already been processed - its
                    // resultFile being a testimony to this.
                    if (resultFiles.Any(resultInfo => resultInfo.Name == derivedResultFileName))
                    {
                        inputFiles.Remove(inputFile);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            Console.WriteLine("- Inputfiles [rerun]: " + inputFiles.Count.ToString());
            return inputFiles;
        }

        /// <summary>
        /// Sets the task result of this task. If all files were processed correctly (no exceptions), then
        /// the task is completed. If some files were processed correctly then it is Partially Complete, and
        /// if none of the tasks completed then the task has Failed.
        /// </summary>
        /// <returns>The task result</returns>
        private TaskResult GetTaskResult()
        {
            Console.WriteLine("> TaskResult [BEGIN]");
            // Get nr of Completed and nr of Input files and whether exceptions occurred.
            int nrOfCompletedFiles = TaskDir.EnumerateFiles("*_LG.xml").Count();
            string inputDirPath = Path.Combine(TaskDir.FullName, "Input");
            DirectoryInfo inputDir = new DirectoryInfo(inputDirPath);
            int nrOfInputFiles = inputDir.EnumerateFiles("*").Count();

            Console.WriteLine("#completed: " + nrOfCompletedFiles);
            Console.WriteLine("#inputfiles: " + nrOfInputFiles);

            if (nrOfCompletedFiles == nrOfInputFiles)
            {
                Console.WriteLine("> TaskResult - COMPLETED [END]");
                return new TaskResult(this.GUID, TaskResult.ResCode.COMPLETED);
            }
            else
            {
                if (nrOfCompletedFiles == 0)
                {
                    Console.WriteLine("> TaskResult FAILED [END]");
                    return new TaskResult(this.GUID, TaskResult.ResCode.FAILED);
                }
                else
                {
                    Console.WriteLine("> TaskResult PARTIALLY_COMPLETED [END]");
                    return new TaskResult(this.GUID, TaskResult.ResCode.PARTIALLY_COMPLETED);
                }
            }
        }

        /// <summary>
        /// Delete unnecessary files after the analysis. Deletes
        /// summary files and the Exception.gz file.
        /// </summary>
        private void CleanUp()
        {
            foreach (FileInfo file in TaskDir.EnumerateFiles("Summar*"))
            {
                file.Delete();
            }
            foreach (FileInfo file in TaskDir.EnumerateFiles("Exception*"))
            {
                file.Delete();
            }
        }

        private ServerAnalysisException ProcessInputFile(FileInfo file)
        {
            Console.WriteLine("- Processing file #" + Count);
            Console.WriteLine("- filename: " + file.Name);

            // Always returns null for LinguisticAnalysis.
            GetDescription(file);
            Analysis an = CurrentDescr.GetAnalysis();

            CurrentSummary = null;
            try
            {
                // Always skipped for linguistic analysis.
                if (an != null)
                {
                    CurrentSummary = an.DoAnalysis();
                }
                Console.WriteLine("- Commence linguistic");
                //Console.SetOut(new StringWriter());
                CurrentDescr.LinguisticProcess(an, ref CurrentSummary);
                //Console.SetOut(Console.Out);
                Console.WriteLine("- End linguistic");
            }
            catch (CommunicationException e)
            {
                ServerDown = true;
                Console.WriteLine("Caught CommunicationException: " + e.Message); 
            }
            catch (RemoteCallException e)
            {
                Console.WriteLine("Caught RemoteCallException: " + e.Error);
                return new ServerAnalysisException(CurrentDescr.SessionID.MetaInfo[SessionIdentification.META_LOGFILE], e.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Exception: " + e.Message); 
                return new ServerAnalysisException(CurrentDescr.SessionID.MetaInfo[SessionIdentification.META_LOGFILE], e.Message);
            }

            string outputFileName = CurrentDescr.SessionID.GetFileName();
            int extensionBeginning = outputFileName.LastIndexOf('.');
            outputFileName = outputFileName.Substring(0, extensionBeginning) + "_LG.xml";
            String outFilePath = Path.Combine(TaskDir.FullName, outputFileName);
            WriteOutput(outFilePath);
            Console.WriteLine("- Analysis completed");
            return null;
        }

        /// <summary>
        /// Load the description file stored at given location. This file is gunzipped.
        /// </summary>
        /// <param name="file"></param>
        private void GetDescription(FileInfo file)
        {
            CurrentDescr = (AnalysisDescription)CustomXMLSerializer.Deserialize(File.OpenRead(file.FullName));
            CurrentDescr.SetWorkingDirPath(TaskDir.FullName);
        }

        private void WriteOutput(string outFilePath)
        {
            // Write output and summary
            IAnalysisWriter writer = CurrentDescr.GetAnalysisWriter(outFilePath);
            writer.WriteDocument(CurrentDescr.SessionID, new Dictionary<string, IDictionary<string, object>>(), CurrentSummary);
            String summOutFilePath = Path.Combine(TaskDir.FullName, "Summary" + Count + ".xml");
            String summOutFilePathGz = Path.Combine(TaskDir.FullName, "Summary" + Count + ".xml.gz");
            var summOutFile = new FileStream(summOutFilePath, FileMode.Create);
            var summOutFileGz = new FileStream(summOutFilePathGz, FileMode.Create);
            CustomXMLSerializer.Serialize(summOutFile, CurrentSummary);
            summOutFile.Close();
            summOutFile = new FileStream(summOutFilePath, FileMode.Open);
            var compress = new GZipStream(summOutFileGz, CompressionMode.Compress);
            summOutFile.Seek(0, SeekOrigin.Begin);
            summOutFile.CopyTo(compress);
            compress.Flush();
            compress.Close();
            summOutFile.Close();
            if (CurrentSummary != null)
            {
                CurrentSummary.Dispose();
                CurrentSummary = null;
            }
            writer.Dispose();
        }

        private void ExtractZip()
        {
            // Extract linguisticProcess Zip:
            // gzipped analysis descriptions go to directory Input/
            // Work directory is extracted separately (we don't want that as a subdirectory of Input/)
            var inZip = new FastZip();
            const string excludeWorkFilter = @"+\.*;-Work\\\.*";
            const string includeWorkFilter = @"Work\\*\.*";
            inZip.ExtractZip(Path.Combine(TaskDir.FullName, "Input.zip"), Path.Combine(TaskDir.FullName, "Input"), excludeWorkFilter);
            inZip.ExtractZip(Path.Combine(TaskDir.FullName, "Input.zip"), Path.Combine(TaskDir.FullName, ""), includeWorkFilter);

            string inputDirPath = Path.Combine(TaskDir.FullName, "Input");
            DirectoryInfo inputDir = new DirectoryInfo(inputDirPath);

            foreach (FileInfo file in inputDir.EnumerateFiles())
            {
                String inFilePathGz = file.FullName;
                string inputFileNameNoExt = file.Name.Substring(0, file.Name.IndexOf('.'));
                string inputFileName = "input_" + inputFileNameNoExt + ".xml";
                String inputFilePath = Path.Combine(TaskDir.FullName, inputFileName);

                var decompress = new GZipStream(new FileStream(inFilePathGz, FileMode.Open), CompressionMode.Decompress);
                var inFile = new FileStream(inputFilePath, FileMode.Create);
                decompress.CopyTo(inFile);
                inFile.Seek(0, SeekOrigin.Begin);
                decompress.Close();
                inFile.Close();
            }
        }

        private void ZipResults()
        {
            // Zip results, both real output and summaries
            // Everything but the summaries and linguisticProcess files, the result zip itself, and the Work directory
            string resultsPath = Path.Combine(TaskDir.FullName, "Results.zip");
            if (File.Exists(resultsPath))
            {
                File.Delete(resultsPath);
            }

            // Include net HTML files as well.
            var outZip = new FastZip();
            const string outFilter = @"+\.*$;-Results\.zip$;-Summar.*\.xml.*$;-input_.*\.xml.*;-Input\.zip$;-Work\\\.*;-Input\\\.*";
            outZip.CreateZip(Path.Combine(TaskDir.FullName, "Results.zip"), TaskDir.FullName, true, outFilter);
            //var summZip = new FastZip();
            //const string summFilter = @"Summary.*\.xml\.gz.*;+Exceptions.*";
            //summZip.CreateZip(Path.Combine(TaskDir.FullName, "Summaries.zip"), TaskDir.FullName, false, summFilter);
        }

        private void SerializeExceptions()
        {
            if (Exceptions != null && Exceptions.Count > 0)
            {
                string serFile = Path.Combine(TaskDir.FullName, "Exceptions.xml");
                string outFile = Path.Combine(TaskDir.FullName, "Exceptions.xml.gz");
                var writer = new FileStream(serFile, FileMode.Create);
                CustomXMLSerializer.Serialize(writer, Exceptions);
                var compress = new GZipStream(new FileStream(outFile, FileMode.Create), CompressionMode.Compress);
                writer.Seek(0, SeekOrigin.Begin);
                writer.CopyTo(compress);
                writer.Close();
                compress.Close();
                File.Delete(serFile);
            }
        }

        public void DoRestart()
        {
            Restart = true;
        }

        public void Stop()
        {
            Thread.Abort();
        }

        public bool IsFinished()
        {
            return Finished;
        }

        public bool IsServerDown()
        {
            return ServerDown;
        }
    }
}