using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using InputLog.Core.Analyses;
using System.Xml;
using InputLog.Core.Util.Xml;

namespace InputLog.Core.Util.Server
{
    /// <summary>
    /// Provides a collection of methods for interacting with the InputLog webserver.
    /// Contains a hidden WebBrowser control.
    /// InputLog server URL is read from config.
    /// </summary>
    public static class ServerUtils
    {
        private static SimpleWebappInteraction _swi;

        /// <summary>
        /// The (hidden) WebBrowser control to be used
        /// </summary>
        public static void Initialize()
        {
            _swi = new SimpleWebappInteraction(ConfigurationManager.AppSettings["InputLogServer"]);
        }

        public static void Initialize(string url)
        {
            _swi = new SimpleWebappInteraction(url);
        }

        /// <summary>
        /// Sends a request to the WebApp to create a new Task on the server.
        /// </summary>
        /// <param name="type">Name of analysis to be executed. SHOULD BE PRESENT IN THE WEBAPP's NEW TASK FORM!</param>
        /// <param name="fileName">Name of input zip file</param>
        /// <returns></returns>
        public static Tuple<int, string> LaunchAnalysis(String type, String fileName)
        {
            var r = _swi.NavigateXml("Task/Create");
            if (r != null)
            {
                XmlElement analysis = r.GetElementByAttrValue("id", type);
                var filePath = Path.GetFullPath(fileName);
                if (analysis != null)
                {
                    var postData = new Dictionary<string,string> {{"analyses", analysis.GetAttribute("value")}};
                    var response = _swi.PostWithFileResponse("Task/Create", filePath, postData);
                    string responseURL = response.ResponseUri.ToString();
                    int id = Int32.Parse(responseURL.Substring(responseURL.LastIndexOf('/') + 1));
                    r = _swi.NavigateXml("Task/Details/" + id);
                    string guid = r.GetElementByAttrValue("id", "TaskGuid").InnerText;
                    return new Tuple<int, string>(id, guid);
                }
            }
            return null;
        }

        /// <summary>
        /// Opens a new browser window and navigates to the Details page of the task with the given ID
        /// </summary>
        /// <param name="id">ID of the Task</param>
        public static void TaskDetails(int id)
        {
            Process.Start(_swi.BaseURL + "Task/Details/" + id);
        }

        public static void DownloadResults(int id, string outputDirectory, 
            out List<ServerAnalysisException> exceptions) 
        {
            string outputPath = Path.Combine(outputDirectory, "Results.zip");
            outputPath = PathSanitizer.Uniquify(outputPath);
            exceptions = null;

            try 
            {
                Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(null, e, Severity.INFO,
                    "Could not store file. Output directory \"" + 
                    outputDirectory + "\" could not be created."
                );
            }

            try 
            {
                _swi.DownloadThisFile("Task/Results/" + id, outputPath);
                RetrieveExceptions(outputPath, out exceptions);
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(null, e, Severity.ERROR,
                    "Could not download Results file. Please download the results file using the website instead.");
            }

        }

        /// <summary>
        /// Fetches a Task's serialized summaries from the WebServer, deserializes them and returns them
        /// </summary>
        /// <param name="resultsPath">path wher ethe results zip is saved</param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private static void RetrieveExceptions(string resultsPath, out List<ServerAnalysisException> exceptions)
        {
            exceptions = new List<ServerAnalysisException>();

            string subDir = Path.Combine(Path.GetTempPath(), "Inputlog-Exception-Retrieval");
            string Exceptions = Path.Combine(subDir, "Exceptions");
            Directory.CreateDirectory(Exceptions);

            try
            {
                var zip = new FastZip();
                zip.ExtractZip(resultsPath, Exceptions, "Exceptions.*");
                var dir = new DirectoryInfo(Exceptions);
                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    var decompress = new GZipStream(new FileStream(file.FullName, FileMode.Open), CompressionMode.Decompress);
                    var inFile = new FileStream(file.FullName + ".tmp", FileMode.Create);
                    decompress.CopyTo(inFile);
                    inFile.Seek(0, SeekOrigin.Begin);
                    exceptions = (List<ServerAnalysisException>)CustomXMLSerializer.Deserialize(inFile);
                    decompress.Close();
                    inFile.Close();
                }
            }
            catch (Exception)
            {
                // Ignore... Exceptions while trying to retrieve the exceptions,
                // it's not _that_ interesting anyway. The data is still in the results zip anyway.
            }
            if (!subDir.IsNullOrEmpty())
            {
                Directory.Delete(subDir, true);
            }
        }

        private static IAnalysisSummary GetSummary(FileInfo file)
        {
            var decompress = new GZipStream(new FileStream(file.FullName, FileMode.Open), CompressionMode.Decompress);
            var inFile = new FileStream(file.FullName + ".tmp", FileMode.Create);
            decompress.CopyTo(inFile);
            inFile.Seek(0, SeekOrigin.Begin);
            var summ = (IAnalysisSummary)CustomXMLSerializer.Deserialize(inFile);
            inFile.Close();
            decompress.Close();
            return summ;
        }

        /// <summary>
        /// Returns a Task's status as a string, and sets a boolean if the Task is completed.
        /// </summary>
        /// <param name="id">ID of the Task</param>
        /// <param name="completed">True if the Task has been completed</param>
        /// <returns>Task status as a String</returns>
        public static string GetTaskStatus(int id, out bool completed)
        {
            var r = _swi.NavigateXml("Task/GetStatus/" + id);
            int statusValue = -1;
            if (r != null)
            {
                XmlElement valueElem = r.GetElementByAttrValue("id", "value");
                if (valueElem != null)
                {
                    statusValue = int.Parse(valueElem.InnerText);
                    completed = (statusValue == TaskStatus.STATUS_COMPLETED || statusValue == TaskStatus.STATUS_PARTIALLY_COMPLETED);
                    return TaskStatus.StatusString(statusValue);
                }
            }
            completed = false;
            return TaskStatus.StatusString(statusValue);
        }

        /// <summary>
        /// Fetches the length of the Task queue on the server
        /// </summary>
        /// <param name="id">ID of the Task</param>
        /// <returns></returns>
        public static int GetTaskQueueLength(int id)
        {
            var r = _swi.NavigateXml("Task/GetPositionInQueue/" + id);
            if (r != null)
            {
                XmlElement valueElem = r.GetElementByAttrValue("id", "Value");
                if (valueElem != null) return int.Parse(valueElem.InnerText);
            }
            return 0;
        }

        public static void SendMails()
        {
            var r = _swi.Navigate("Task/SendNotifications");
            r.Close();
        }

        public static void Cleanup()
        {
            var r = _swi.Navigate("Task/Cleanup");
            r.Close();
        }

        public static bool Login(bool silent = false, AccountSettings accSett = null)
        {
            return _swi.Login(silent, accSett);
        }
    }
}
