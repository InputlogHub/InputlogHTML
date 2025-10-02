using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GUI.Tabs.Postprocess.WizardPages.Form;
using GUI.Wizard;
using InputLog.Core.Merging.Analyses;

namespace GUI.Tabs.Postprocess.WizardPages.Page
{
    internal class MergeAnalysisProcessPage : WizardPage
    {
        #region Fields

        /// <summary>
        /// The number of files that has to be processed
        /// </summary>
        private int NumberOfFiles;

        /// <summary>
        /// Worker thread where the actual processing happens.
        /// </summary>
        private Thread Worker;

        #endregion

        /// <summary>
        /// Construction
        /// </summary>
        public MergeAnalysisProcessPage()
        {
            Control = new FileResultProcessPage();
            Next = null;
        }

        public override bool IsProcessingPage()
        {
            return true;
        }

        public override bool Validate()
        {
            return Status == ProcessCode.PROCESS_SUCCESS;
        }

        /// <summary>
        ///  Process the information
        /// </summary>
        public override void Process()
        {
            // Start processing
            OnProcessStart();

            // Get the wizards collected information
            Dictionary<string, object> wizardData = GetData();

            // Get data from the dictionary
            object obj;
            wizardData.TryGetValue(FileSelectPage.PATH, out obj);
            var path = (string) obj;
            wizardData.TryGetValue(FileSelectPage.FILES, out obj);
            var thisFiles = (List<string>) obj;
            wizardData.TryGetValue(MergeAnalysis_Intro.ANALYSIS_KEY, out obj);
            if (obj == null) return;
            var analysisType = (AnalysisMerge.Direction) obj;

            // Cast the process page control to something more usable.
            var page = (FileResultProcessPage) Control;

            // Check data.
            if (path == null)
            {
                throw new ArgumentException("MergeAnalysis: FilePath not set.");
            }
            if (thisFiles == null || thisFiles.Count == 0)
            {
                throw new ArgumentException("MergeAnalysis: No files selected.");
            }

            // Update progress bar settings
            page.SetProgressMaximum(125);

            // Begin merging of files.
            page.AppendLine("Merging " + thisFiles.Count + " files. Please, be patient."
                            + "Processing can take a few minutes.");
            NumberOfFiles = thisFiles.Count;

            // Create the dictionary of file paths, key corresponds to the analysis type.
            for (int i = 0; i < thisFiles.Count; i++)
            {
                thisFiles[i] = path + Path.DirectorySeparatorChar + thisFiles[i];
            }

            // Merge the files
            MergeFiles(page, thisFiles, analysisType, path);

            //Done
            if (Stop)
            {
                OnProcessFail();
            }
            else
            {
                OnProcessSuccess();

                // Hide progress bar, show the opendirectory link
                page.SetVisibility(page.ProgressBar, false);
                page.SetVisibility(page.OpenFolderLink, true);
                page.SetEnabled(page.ProgressBar, false);
                page.SetEnabled(page.OpenFolderLink, true);
                page.FileDirectory = path;
                page.AppendLine("Merging completed...");
            }
        }

        private void MergeFiles(ProcessPage page, List<string> filePaths, 
            AnalysisMerge.Direction mergeDirection, string workDir)
        {
            page.AppendLine("Processing files...");

            // Create a thread for our reader (with parameters)
            var merger = new AnalysisMerge(filePaths, workDir);
            ThreadStart workStarter = () => merger.Merge(mergeDirection);
            Worker = new Thread(workStarter);
            Worker.Start();

            // Progressbar increments
            double increment = 100/(double) NumberOfFiles;

            int previousState = 0;
            while (merger.Processed < NumberOfFiles && !Stop && Worker.IsAlive)
            {
                if (merger.Processed > previousState)
                {
                    for (; previousState < merger.Processed; previousState++)
                    {
                        page.SetProgressValue((int)((previousState+1) * increment));
                        page.AppendLine("\tProcessing file " + (previousState+1) + "/" + NumberOfFiles);
                    }
                }
            }
            if (merger.HasErrors())
            {
                page.AppendLine("");
                page.AppendLine("\tErrors occured while processing files:");
                foreach (string e in merger.GetErrors())
                {
                    page.AppendLine("\t\t" + e);
                }
                MessageBox.Show("Errors occured while merging. Check log for more information",
                    "Merge Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (Stop)
            {
                merger.StopProcessing();
                // Wait for the merging to stop.
                Thread.Sleep(75);
                if (Worker.IsAlive)
                {
                    try
                    {
                        Worker.Abort();
                    }
                    catch (ThreadAbortException)
                    {
                        // We ignore this exception, it was an intended action.
                    }
                }
                page.AppendLine("> Processing Aborted");
            }
            else
            {
                page.AppendLine("Processing files completed.");
                page.AppendLine("Writing merged file.");

                // Wait until the worker is completely finished.
                while (Worker.IsAlive)
                {
                    Thread.Sleep(50);
                }
                page.AppendLine("Writing completed.");
                page.SetProgressValue(125);
            }
        }

    }
}