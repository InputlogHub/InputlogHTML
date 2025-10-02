using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using WebApp.Controllers;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CopyTaskCreator
{
    public partial class CopyTaskCreator : Form
    {
        public static string fileDirectory;
        public static string title;
        bool isExisting;

        public CopyTaskCreator()
        {
            InitializeComponent();
            this.LanguageComboBox.SelectedItem = "EN";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            TaskControl task = new TaskControl();
            layoutTask(task);
        }

        private void layoutTask(TaskControl task)
        {
            task.SuspendLayout();
            TasksPanel.Controls.Add(task);
            TasksPanel.ScrollControlIntoView(task);
            task.ResumeLayout();
            task.Width = TasksPanel.ClientSize.Width - 10;
        }

        /// <summary>
        /// Creates new or overwrites existing xml document with all tasks defined by the user.
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            saveCopyTask();
        }

        public XElement toXml(){
            XElement copyTask = new XElement("copytask");
            copyTask.Add(new XElement("title", txtGeneralTitle.Text));
            copyTask.Add(new XElement("description", txtGeneralDescription.Text));
            copyTask.Add(new XElement("language", LanguageComboBox.SelectedItem));

            string lang = LanguageComboBox.SelectedItem != null ? LanguageComboBox.SelectedItem.ToString().ToLower() : "en";

            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);

            XElement instructions = new XElement("general_instructions", txtGeneralInstructions.Text);

            copyTask.Add(instructions);

            var taskControls = TasksPanel.Controls.OfType<TaskControl>().ToList();

            foreach (TaskControl taskControl in taskControls)
            {
                XElement task = taskControl.toXml();
                copyTask.Add(task);
            }

            return copyTask;
        }

        private void btnLoadTask_Click(object sender, EventArgs e)
        {
            openCopyTask();
        }

        private void openCopyTask()
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog ofd = new OpenFileDialog();

            // Set filter options and filter index.
            ofd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            var dialogResult = ofd.ShowDialog();

            // Process input if the user clicked OK.
            if (dialogResult == DialogResult.OK)
            {
                clearAllFields();

                // Open the selected file to read.
                XmlDocument tasksXML = new XmlDocument();
                StreamReader inputReader = new StreamReader(ofd.FileName, Encoding.UTF8);
                tasksXML.Load(inputReader);

                fileDirectory = System.IO.Path.GetDirectoryName(ofd.FileName);

                txtGeneralInstructions.Text = XMLUtil.GetChildNodeValue(tasksXML["copytask"], "general_instructions");
                txtGeneralTitle.Text = XMLUtil.GetChildNodeValue(tasksXML["copytask"], "title");
                txtGeneralDescription.Text = XMLUtil.GetChildNodeValue(tasksXML["copytask"], "description");
                LanguageComboBox.SelectedItem = XMLUtil.GetChildNodeValue(tasksXML["copytask"], "language");

                XmlNodeList taskNodes = tasksXML.DocumentElement.SelectNodes("task");

                // Create a TaskControl object for each task node in xml document and add it to the taskspanel.
                foreach (XmlNode node in taskNodes)
                {
                    TaskControl task = new TaskControl(node);
                    TasksPanel.Controls.Add(task);
                    TasksPanel.ScrollControlIntoView(task);
                    task.Width = TasksPanel.ClientSize.Width - 10;
                }

                isExisting = true;
            }
        }

        private void saveCopyTask()
        {

            if(!validateTitles())
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fileDirectory == null)
            {
                fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InputLog");
            }

            fbd.SelectedPath = fileDirectory;
            fbd.Description = "Please select the folder where you wish to save the copy task.";

            title = "copytask";
            if (txtGeneralTitle.Text != "")
                title = txtGeneralTitle.Text;

            string fileName = title + ".txt";

            if (isExisting || fbd.ShowDialog() == DialogResult.OK)
            {
                fileDirectory = isExisting? fileDirectory : fbd.SelectedPath;

                var tempDir = new DirectoryInfo(fileDirectory);
                if (!tempDir.Exists)
                {
                    tempDir.Create();
                }

                var filePath = fileDirectory + "\\" + fileName;

                XElement copyTask = toXml();

                File.WriteAllText(filePath, copyTask.ToString());
                var shortName = Path.GetFileName(fileName);
                MessageBox.Show("Copy task \'" + shortName + "\' saved to folder " + Path.GetDirectoryName(filePath));
                isExisting = true;
            }
        }

        /// <summary>
        /// Save As - for a copy task. Allows the user to select
        /// a different name than for the copy task.
        /// </summary>
        private void saveCopyTaskAs()
        {
            if (!validateTitles())
            {
                return;
            }

            // If a path is not already set, set it to MyDocuments/Inputlog
            if (fileDirectory == null)
            {
                fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InputLog");
            }

            
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.RestoreDirectory = true;
            sfd.CheckPathExists = true;
            sfd.DefaultExt = "txt";
            sfd.Filter = "CopyTask Files (*.txt)|*.txt|All Files(*.*)|*.*";
            sfd.OverwritePrompt = true;
            sfd.InitialDirectory = fileDirectory;

            // If SaveAs = ok, transform this task to XML, then write it to the chosen file.
            StreamWriter targetFileWriter = null;
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Stream targetFileStream = sfd.OpenFile();
                    targetFileWriter = new StreamWriter(targetFileStream, Encoding.UTF8);
                    String copyTaskContent = toXml().ToString();
                    targetFileWriter.Write(copyTaskContent);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Saving document failed: unexpected error caught [" + e.GetType() + "]",
                    "Error",
                    MessageBoxButtons.OK
                );
            }
            finally
            {
                if (targetFileWriter != null)
                {
                    targetFileWriter.Close();
                }
            }

            // At this point the file is an 'already existing file'.
            isExisting = true;
        }

        private bool validateTitles()
        {
            var allTasks = TasksPanel.Controls.OfType<TaskControl>().ToList();
            bool validated = true;
            foreach (var task in allTasks)
            {
                if (task.title() == String.Empty)
                    validated = false;
                var allTitles = allTasks.Select(t => t.title()).Distinct().ToList();
                if(allTitles.Count != allTasks.Count)
                {
                    validated = false;
                }
            }

            if(!validated)
                MessageBox.Show("Each task is required to have a title, and this title must be unique. Please review the titles you have given to your tasks.");

            return validated;
        }

        private void TasksPanel_SizeChanged(object sender, EventArgs e)
        {
            TasksPanel.SuspendLayout();

            var taskControls = TasksPanel.Controls.
                OfType<TaskControl>().ToList();

            foreach (TaskControl task in taskControls)
            {
                task.Width = TasksPanel.ClientSize.Width - 10;
            }

            TasksPanel.ResumeLayout();
        }

        private void AddTextBlockButton_Click(object sender, EventArgs e)
        {
            TaskControl task = new TaskControl();
            task.setInformational();

            layoutTask(task);
        }

        private void AddExampleButton_Click(object sender, EventArgs e)
        {
            TaskControl task = new TaskControl();
            task.setExample();

            layoutTask(task);
        }

        private async void sendToServer()
        {
            saveCopyTask();

            bool connected = false, cancelled = false;
            while (!connected && !cancelled)
            {
                DialogResult dr = Credentials.CredentialsDialog();
                if (dr == DialogResult.OK)
                {
                    connected = await ServerConnection.checkCredentials(Properties.Settings.Default.username, Properties.Settings.Default.password);
                    if (!connected)
                    {
                        var result = MessageBox.Show("Please check if you entered your username and password correctly.", "Wrong credentials", MessageBoxButtons.OKCancel);
                        if (result == DialogResult.Cancel)
                        {
                            cancelled = true;
                        }
                    }
                }
                else
                {
                    cancelled = true;
                }
            }
            if (connected)
            {
                //zip directory
                string startPath = fileDirectory;
                string zipPath = fileDirectory + ".zip";

                for (int i = 1; File.Exists(zipPath); i++)
                {
                    zipPath = fileDirectory + "_" + i + ".zip";
                }

                ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                

                var serverFileName = await ServerConnection.uploadCopyTask(Properties.Settings.Default.username, Properties.Settings.Default.password, zipPath, title);
                if (serverFileName != "")
                {
                    MessageBox.Show("Copy task was successfully sent to server. You will now be redirected to a web page to finish the process.");

                    File.Delete(zipPath);

                    Process.Start("http://localhost:18477/Copy/Manage?creator=" + Properties.Settings.Default.username + "&task=" + serverFileName);

                }
            }
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            sendToServer();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCopyTask();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCopyTask();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCopyTaskAs();
        }

        private void sendToServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sendToServer();
        }

        private void newCopyTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newCopyTask();
        }

        private void newCopyTask()
        {
            var response = MessageBox.Show("Do you wish to save your current copy task?", "Save copy task", MessageBoxButtons.YesNoCancel);

            switch (response)
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.Yes:
                    saveCopyTask();
                    break;
                case DialogResult.No:
                    break;
            }

            clearAllFields();
            isExisting = false;
        }

        private void clearAllFields()
        {
            // Remove all current TaskControls from the copytask form.
            TasksPanel.Controls.OfType<TaskControl>().ToList().ForEach(b => { this.Controls.Remove(b); b.Dispose(); });

            //Clear all other fields;
            txtGeneralTitle.Text = "";
            txtGeneralDescription.Text = "";
            txtGeneralInstructions.Text = "";
            LanguageComboBox.SelectedIndex = -1;
        }
    }
}
