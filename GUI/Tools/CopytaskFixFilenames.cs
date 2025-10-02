using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.Util;

namespace GUI.Tools
{
    public partial class CopytaskFixFilenames : Form
    {
        private const string DEST_FOLDER_NAME = "restored-idfx";

        /// <summary>
        ///     Does the background work of fixing the file names.
        /// </summary>
        private readonly BackgroundWorker Worker;

        private string DestFolderPath;

        /// <summary>
        ///     Dialog used to open / select the idfx files.
        /// </summary>
        private readonly OpenFileDialog DialogIDFX;

        private string SourceIDFXDir;

        public CopytaskFixFilenames()
        {
            InitializeComponent();

            Worker = new BackgroundWorker
                     {
                         WorkerReportsProgress = true,
                         WorkerSupportsCancellation = true
                     };
            Worker.DoWork += WorkerDoWork;
            Worker.ProgressChanged += WorkerProgressChanged;
            Worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            DialogIDFX = new OpenFileDialog
                         {
                             CheckPathExists = true,
                             RestoreDirectory = true,
                             Multiselect = true
                         };
        }

        private string[] IDFXFiles
        {
            get { return DialogIDFX.FileNames; }
        }

        private int IDFXFilesCount
        {
            get
            {
                if (IDFXFiles != null)
                {
                    return IDFXFiles.Length;
                }
                return 0;
            }
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseForm();
            Process.Start(DestFolderPath);
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var destDirInfo = new DirectoryInfo(DestFolderPath);
            if (!destDirInfo.Exists)
            {
                destDirInfo.Create();
            }

            double dIDFXCount = IDFXFilesCount;

            for (var i = 0; i < IDFXFilesCount; i++)
            {
                var filepath = IDFXFiles[i];
                try
                {
                    var eventReader = EventLogFactory.CreateFileEventLogReader(filepath, LogFormat.XML);
                    var sessionID = eventReader.ReadSessionIdentification();

                    var originalFilename = "Unknown.idfx";
                    if (sessionID.MetaInfo.ContainsKey(SessionIdentification.META_LOGFILE))
                    {
                        originalFilename = sessionID.MetaInfo[SessionIdentification.META_LOGFILE];
                    }

                    eventReader.Close();

                    var newPath = Path.Combine(DestFolderPath, originalFilename);
                    newPath = PathSanitizer.Uniquify(newPath);

                    //File.Move(filepath, new_path);
                    File.Copy(filepath, newPath);

                    var progressPercentage = (int) (i/dIDFXCount*100);
                    Worker.ReportProgress(progressPercentage);
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR, "Could not process file: " + filepath);
                }
            }
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            if (DialogIDFX.ShowDialog() == DialogResult.OK)
            {
                if (IDFXFilesCount < 1)
                {
                    IdfxTextBox.Text = "";
                    return;
                }
                if (IDFXFilesCount == 1)
                {
                    IdfxTextBox.Text = DialogIDFX.FileName;
                }
                else
                {
                    IdfxTextBox.Text = IDFXFilesCount + " files selected...";
                }

                var filename = DialogIDFX.FileName;
                SourceIDFXDir = Path.GetDirectoryName(filename);
                if (SourceIDFXDir != null)
                {
                    var destinationPath = Path.Combine(SourceIDFXDir, DEST_FOLDER_NAME);
                    DestFolderPath = PathSanitizer.Uniquify(destinationPath);
                }
            }
        }

        private void ProcessButtonClick(object sender, EventArgs e)
        {
            if (DestFolderPath.IsNullOrEmpty()) return;
            Worker.RunWorkerAsync();
        }

        #region Threadsafe GUI updating

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressBar.InvokeRequired)
            {
                ProgressBar.Invoke((MethodInvoker) delegate { UpdateProgressBar(e.ProgressPercentage); });
            }
            else
            {
                UpdateProgressBar(e.ProgressPercentage);
            }
        }

        private void CloseForm()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) CloseThisForm);
            }
            else
            {
                CloseThisForm();
            }
        }

        private void CloseThisForm()
        {
            if (Worker.IsBusy)
            {
                Worker.CancelAsync();
            }
            Close();
        }

        private void UpdateProgressBar(int currentPercentage)
        {
            ProgressBar.Value = currentPercentage;
        }

        private void ClearProgressBar()
        {
            if (ProgressBar.InvokeRequired)
            {
                ProgressBar.Invoke((MethodInvoker)ClearThisProgressBar);
            }
            else
            {
                ClearThisProgressBar();
            }
        }

        private void ClearThisProgressBar()
        {
            ProgressBar.Value = 0;
            ProgressBar.Maximum = 100;
        }

        #endregion
    }
}