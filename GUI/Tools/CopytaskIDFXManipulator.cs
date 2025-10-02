using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace GUI.Tools
{
    /// <summary>
    ///     A hidden developer tool that allows to select some IDFX files and a copy task
    ///     and the IDFX files will all have the copy task embedded into their IDFX file.
    ///     Furthermore, the first focus event, will be set to the title of the copyTask
    ///     that has been selected.
    /// </summary>
    public partial class CopytaskIDFXManipulator : Form
    {
        private const string DEST_FOLDER_NAME = "manipulated-idfx";

        private XmlDocument CopytaskXML;
        private string DestFolderPath;
        private string SourceIDFXDir;
        private readonly BackgroundWorker Worker;

        public CopytaskIDFXManipulator()
        {
            InitializeComponent();
            Worker = new BackgroundWorker
                     {
                         WorkerSupportsCancellation = true,
                         WorkerReportsProgress = true
                     };
            Worker.ProgressChanged += WorkerProgressChanged;
            Worker.DoWork += WorkerDoWork;
            Worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
        }

        public string[] IDFXFiles
        {
            get { return DialogIDFX.FileNames; }
        }

        public int IDFXFilesCount
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

        public string CopytaskFile
        {
            get { return DialogCopytask.FileName; }
        }

        public string Session
        {
            get { return TextboxSession.Text; }
        }

        private void BrowseIDFXClick(object sender, EventArgs e)
        {
            if (DialogIDFX.ShowDialog() == DialogResult.OK)
            {
                if (IDFXFilesCount < 1)
                {
                    TextboxIDFX.Text = "";
                    return;
                }
                if (IDFXFilesCount == 1)
                {
                    TextboxIDFX.Text = DialogIDFX.FileName;
                }
                else
                {
                    TextboxIDFX.Text = IDFXFilesCount + " files selected...";
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

        private void BrowseCopytaskClick(object sender, EventArgs e)
        {
            if (DialogCopytask.ShowDialog() == DialogResult.OK)
            {
                TextboxCopytask.Text = DialogCopytask.FileName;
                ReadCopytask();
            }
        }

        private void ReadCopytask()
        {
            CopytaskXML = new XmlDocument();
            CopytaskXML.Load(CopytaskFile);
        }

        private void ProcessButtonClick(object sender, EventArgs e)
        {
            if (DestFolderPath.IsNullOrEmpty()) return;
            Worker.RunWorkerAsync();
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

            for (var i = 0; i < IDFXFilesCount; ++i)
            {
                var filepath = IDFXFiles[i];
                try
                {
                    // Read each file from xml, alter it and save it again.
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);

                    AlterFile(xmlDoc);

                    // Create destination filename
                    var filename = Path.GetFileName(filepath);
                    if (filename != null)
                    {
                        var destFilePath = Path.Combine(DestFolderPath, filename);
                        xmlDoc.Save(destFilePath);
                    }

                    var progressPercentage = (int) (i/dIDFXCount*100);
                    Worker.ReportProgress(progressPercentage);
                }
                catch (IOException ioe)
                {
                    MessageLogger.CatchException(this, ioe, Severity.ERROR,
                        "Could not read or write to file: \"" + filepath + "\"");
                }
            }
        }

        private void AlterFile(XmlDocument idfx)
        {
            // Create a node for the copyTask.
            XmlNode entryNode = idfx.CreateElement("entry");

            XmlNode keyNode = idfx.CreateElement("key");
            keyNode.InnerText = SessionIdentification.META_COPYTASK;
            entryNode.AppendChild(keyNode);

            XmlNode valueNode = idfx.CreateElement("value");
            valueNode.InnerXml = CopytaskXML.InnerXml;
            entryNode.AppendChild(valueNode);

            var metaNode = idfx.SelectSingleNode(@"/log/meta");
            DeleteExistingCopytask(metaNode);
            if (metaNode != null) metaNode.AppendChild(entryNode);

            // Change session identifier
            var sessionNode = idfx.SelectSingleNode(@"/log/session/entry/key[text()='Session']");
            if (sessionNode != null)
            {
                if (sessionNode.ParentNode != null)
                {
                    var sessionValueNode = sessionNode.ParentNode.SelectSingleNode("value");
                    if (sessionValueNode != null) sessionValueNode.InnerText = Session;
                }
            }

            // Add session keyboard info
            //XmlNode kbNode = idfx.CreateElement("entry");
            //XmlNode kbNode_key = idfx.CreateElement("key");
            //kbNode_key.InnerText = "Keyboard";
            //kbNode.AppendChild(kbNode_key);
            //XmlNode kbNode_value = idfx.CreateElement("value");
            //kbNode_value.InnerText = "AZERTY";
            //kbNode.AppendChild(kbNode_value);

            //var sessionIdNode = idfx.SelectSingleNode(@"/log/session");
            //sessionIdNode.AppendChild(kbNode);

            // Change first focus event value to first task title.
            var copytaskFirstTask = CopytaskXML.SelectSingleNode("/copyTask/task[1]");
            if (copytaskFirstTask != null)
            {
                if (copytaskFirstTask.Attributes != null)
                {
                    var firstTaskTitle = copytaskFirstTask.Attributes["title"].InnerText;
                    var firstFocusNode = idfx.SelectSingleNode(@"/log/event[@type='focus']");
                    if (firstFocusNode != null)
                    {
                        var titleNode = firstFocusNode.SelectSingleNode("part/title");
                        if (titleNode != null) titleNode.InnerText = firstTaskTitle;
                    }
                }
            }
        }

        /// <summary>
        ///     Checks whether there already exists a node with copyTask
        ///     information in the idfx and deletes the exisiting node, if such
        ///     a node is present.
        /// </summary>
        /// <param name="metaNode">
        ///     The node containing all of the
        ///     meta data.
        /// </param>
        private void DeleteExistingCopytask(XmlNode metaNode)
        {
            ICollection<XmlNode> nodesToRemove = new List<XmlNode>();

            // Find __copytask nodes in meta data
            var xmlNodeList = metaNode.SelectNodes("entry");
            if (xmlNodeList != null)
                foreach (XmlNode entryNode in xmlNodeList)
                {
                    var selectSingleNode = entryNode.SelectSingleNode("key");
                    if (selectSingleNode != null)
                    {
                        var key = selectSingleNode.InnerText;
                        if (key == SessionIdentification.META_COPYTASK)
                        {
                            nodesToRemove.Add(entryNode);
                        }
                    }
                }

            // Remove the nodes
            foreach (var nodeToRemove in nodesToRemove)
            {
                metaNode.RemoveChild(nodeToRemove);
            }
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
                Invoke((MethodInvoker)CloseThisForm);
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