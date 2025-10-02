using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Util;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util.Server;
using InputLog.Core.Util.Xml;

namespace GUI.Tabs.Analyze.AnalysesControls
{
    /// <summary>
    /// Preprocesses and zips input files and launches analyses on the server.
    /// </summary>
    public class ServerAnalysisLauncher
    {
        private readonly string TempDir;
        private readonly string WorkDir;
        private int TaskId;

        public ServerAnalysisLauncher()
        {
            TempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            WorkDir = Path.Combine(TempDir, "Work");
        }

        public bool ConnectToServer()
        {
            Directory.CreateDirectory(TempDir);
            Directory.CreateDirectory(WorkDir);
            return ServerUtils.Login();
        }

        private void CompressInput(string sourcePath, AnalyzeModel model, AbstractAnalyzer analyzer)
        {
            /*SessionIdentification sessionId;
            model.ReadEvents(sourcePath, out sessionId);
            if (sessionId == null) return;*/

            var description = analyzer.GetDescription(WorkDir);
            description.PreProcess(description.GetAnalysis());

            string path = Path.Combine(TempDir, description.SessionID.GetFileName() + ".xml");
            var outFile = File.Create(path);
            var compressed = new GZipStream(File.Create(path + ".gz"),
                CompressionMode.Compress);

            CustomXMLSerializer.Serialize(outFile, description);
            outFile.Seek(0, SeekOrigin.Begin);
            outFile.CopyTo(compressed);
            outFile.Close();
            compressed.Close();
        }

        public bool LaunchAnalysis(AnalyzeModel model, AbstractAnalyzer analyzer,
            List<Preprocessor> preprocessors, ServerAnalysisControl saControl, out string task)
        {
            saControl.RunTS(delegate
            {
                saControl.GetProgressBar().PerformStep();
            });
            analyzer.ReportProgress(this, new ProgressEventArgs("Connecting to server..."));

            if (!ConnectToServer())
            {
                analyzer.ReportProgress(this, new ProgressEventArgs("Failed to connect to server"));
                saControl.RunTS(delegate
                {
                    saControl.GetProgressBar().PerformStep();
                });
                task = "null";
                return false;
            }

            analyzer.ReportProgress(this, new ProgressEventArgs("Connected to server"));
            saControl.RunTS(delegate
            {
                saControl.GetProgressBar().PerformStep();
            });

            //model.Analyze(analyzer, preprocessors, preprocessOnly: true);
            // NOTE: This logic is copied from AnalyzeModel
            model.OriginalDocumentPaths.Add("");

            // Iterate over all source files and their respective original documents.
            foreach (var it in model.SourcePaths.Zip(model.OriginalDocumentPaths,
                (s, o) => new { SourcePath = s, OriginalDocumentPath = o }))
            {
                try
                {
                    SessionIdentification sessionId;
                    var events = model.ReadEvents(it.SourcePath, out sessionId);
                    if (sessionId == null || events == null) continue;
                    AnalyzeModel.PreprocessEvents(sessionId, events);

                    analyzer.StartSession(sessionId, events, model.IsReportGeneration);
                    analyzer.SetPaths(it.SourcePath, model.DestinationPath, it.OriginalDocumentPath);


                    CompressInput(it.SourcePath, model, analyzer);
                }
                catch (Exception exc)
                {
                    if (exc.Message.Contains("Empty path"))
                    {
                        MessageLogger.CatchException(this, exc, Severity.ERROR, "Path not found.");
                    }
                    else if (exc.Message.Contains("The process"))
                    {
                        MessageLogger.CatchException(this, exc, Severity.ERROR, "The file is being used by another process.");
                    }
                    else
                    {
                        string message = "The analysis could not be executed due to an error.";
                        if (!String.IsNullOrEmpty(RevisionAnalysis.BadEventIds)) 
                        {
                            message += "Bad events detected with ids [" + RevisionAnalysis.BadEventIds + "]";
                        }
                        MessageLogger.CatchException(this, 
                            new Exception(message, exc),
                            Severity.ERROR
                        );
                    }
                }
            }

            var zip = new FastZip();
            string zipPath = Path.Combine(TempDir, "Input.zip");

            DirectoryInfo workingDir = new DirectoryInfo(TempDir);
            if (workingDir.EnumerateFiles(@"*.xml.gz").Any())
            {

                zip.CreateZip(zipPath, TempDir, true, @"+\.xml\.gz.*$;+Work\\*\.*");

                var taskDetails = ServerUtils.LaunchAnalysis(analyzer.Name, zipPath);
                TaskId = taskDetails.Item1;
                task = taskDetails.Item2;
                Directory.Delete(TempDir, true);
                return true;
            }
            else
            {
                task = "no task created";
                Directory.Delete(TempDir, true);
                return false;
            }

        }

        public bool IsAnalysisCompleted(AbstractAnalyzer analyzer, string outputDir,
            out string status, out List<ServerAnalysisException> exceptions)
        {
            bool completed;
            exceptions = null;
            status = ServerUtils.GetTaskStatus(TaskId, out completed);

            if (completed)
            {
                //int i = 0;
                //foreach (var summary in ServerUtils.GetSummaries(TaskId, out exceptions))
                //{
                //    var writer = Descriptions.ElementAt(i).
                //        GetAnalysisWriter(analyzer.DestinationFiles.ElementAt(i));
                //    writer.WriteDocument(Descriptions.ElementAt(i).SessionID,
                //        new Dictionary<string, IDictionary<string, object>>(), summary);
                //    writer.Dispose();
                //    i++;
                //}
                ServerUtils.DownloadResults(TaskId, outputDir, out exceptions);
            }
            else
            {
                int n = ServerUtils.GetTaskQueueLength(TaskId);
                if (n > 0) status += " (" + status + ")";
            }

            return completed;
        }
    }
}