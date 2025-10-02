using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.AnalysesControls.Bigram;
using GUI.Tabs.Analyze.AnalysesControls.Copytask;
using GUI.Tabs.Analyze.AnalysesControls.Fluency;
using GUI.Tabs.Analyze.AnalysesControls.General;
using GUI.Tabs.Analyze.AnalysesControls.Linguistic;
using GUI.Tabs.Analyze.AnalysesControls.Pause;
using GUI.Tabs.Analyze.AnalysesControls.Revision;
using GUI.Tabs.Analyze.AnalysesControls.Source;
using GUI.Tabs.Analyze.AnalysesControls.Summary;
using GUI.Tabs.Analyze.AnalysesControls.WordPauses;
using InputLog.Core.Util;

namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    public partial class FileWizardPageForm : UserControl
    {
        public FileWizardPageForm()
        {
            InitializeComponent();
            Path = null;
            TabAbbreviations = new Dictionary<TabPage, string>();
        }

        /// <summary>
        ///     Get the filenames of all files in the 'SelectedFiles' listbox of
        ///     the form.
        /// </summary>
        /// <returns>A list of the filenames of all the selected items in the form. </returns>
        public List<string> GetSelectedFiles()
        {
            var fileNames = new List<string>(SelectedFiles.Items.Count);
            fileNames.AddRange(from ListItem fileName in SelectedFiles.Items select fileName.Value);
            return fileNames;
        }

        public string GetRelativePath(string currentDir, FileInfo file)
        {
            var directory = new DirectoryInfo(currentDir);

            var fullDirectory = directory.FullName;
            var fullFile = file.FullName;

            return fullFile.Substring(fullDirectory.Length + 1);
        }

        /// <summary>
        ///     Return the path to the selected folder in the page.
        /// </summary>
        /// <returns>The complete path to the selected folder</returns>
        public string GetPath()
        {
            return Path;
        }

        /// <summary>
        /// Event handling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Create the ToolTip and associate with the Form container.
            var toolTip = new ToolTip
                          {
                              IsBalloon = true,
                              AutoPopDelay = 5000,
                              InitialDelay = 1000,
                              ReshowDelay = 500,
                              ShowAlways = true
                          };

            // Set up the delays for the ToolTip.
            // Force the ToolTip text to be displayed whether or not the form is active.

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip.SetToolTip(AddAllButton, "Add all the files in the selected folder to the 'to be merged' box.");
            toolTip.SetToolTip(AddButton, "Add the selected files in selected folder to the 'to be merged' box.");
            toolTip.SetToolTip(RemoveAllButton, "Remove all the files currently from the 'to be merged' box.");
            toolTip.SetToolTip(RemoveButton, "Remove the selected files from the 'to be merged' box.");
        }

        private void BrowseFoldersClick(object sender, EventArgs e)
        {
            try
            {
                var folderDialog = new FolderBrowserDialog
                                   {
                                       SelectedPath = (string) RegistryTools.GetSetting("Inputlog", "DirPath",
                                           Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                                   };
                // Show only files matching the filter. 
                // Vertical merge MergeDirection 'true': General Analysis 
                // else horizontal merge with Pause Analysis or Summary Analysis
                //folderDialog.Filter = MergeAnalysis_Intro_Form.MergeDirection ? "General|*GA*.xml|All Files|*.*" 
                //    : "Summary|*SU*.xml|Pause|*PA*.xml|All Files|*.*";
                //folderDialog.Multiselect = true;

                // Retrieve previously saved path from the Registry.
                // FolderDialog will autoscroll to this location or to MyDocuments as default.

                var result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Remove the items currently in the list
                    FilesInFolderTabs.TabPages.Clear();

                    var dir = new DirectoryInfo(folderDialog.SelectedPath);
                    Path = dir.ToString();
                    var c = 0;
                    foreach (var abbr in Abbreviations[MergeAnalysis_Intro_Form.MergeDirection])
                    {
                        var fileList = dir.GetFiles("*_" + abbr + "*.xml", SearchOption.AllDirectories);
                        if (fileList.Length > 0)
                        {
                            AddFileListTab(abbr, abbr, fileList, c);
                            c++;
                        }
                    }
                    var fileListAll = dir.GetFiles("*.xml", SearchOption.AllDirectories);
                    AddFileListTab("All", "_", fileListAll, c);

                    // Save actual path to the directory in the Registry for reuse later.
                    RegistryTools.SaveSetting("Inputlog", "DirPath", folderDialog.SelectedPath);
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        private void AddFileListTab(string name, string abbr, FileInfo[] fileList, int c)
        {
            // Retain only the relevant files for merging.
            FilesInFolderTabs.TabPages.Add(name);
            var filesInFolderArea = FilesInFolderTabs.TabPages[c];
            TabAbbreviations[filesInFolderArea] = abbr;
            filesInFolderArea.BackColor = Color.White;
            var filesInFolder = CreateListBox();
            filesInFolderArea.Controls.Add(filesInFolder);

            var sc = new SuffixComparer(abbr);
            Array.Sort(fileList, sc);
            foreach (var file in fileList)
            {
                filesInFolder.Items.Add(new ListItem {Text = file.Name, Value = GetRelativePath(Path, file)});
            }
        }

        private ListBox CreateListBox()
        {
            var result = new ListBox
                         {
                             Size = new Size(255, 340),
                             HorizontalScrollbar = true,
                             SelectionMode = SelectionMode.MultiExtended,
                             TabIndex = 10,
                             Margin = new Padding(0, 0, 0, 0)
                         };
            return result;
        }

        private void AddAllButtonClick(object sender, EventArgs e)
        {
            var items = FilesInFolder.Items.Cast<ListItem>().ToList();
            AddItems(items);
            FilesInFolder.Items.Clear();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            var items = new List<ListItem>();
            while (FilesInFolder.SelectedItems.Count > 0)
            {
                items.Add((ListItem) FilesInFolder.SelectedItems[0]);
                FilesInFolder.Items.Remove(FilesInFolder.SelectedItems[0]);
            }
            AddItems(items);
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            var items = new List<ListItem>();
            while (SelectedFiles.SelectedItems.Count > 0)
            {
                items.Add((ListItem) SelectedFiles.SelectedItems[0]);
                SelectedFiles.Items.Remove(SelectedFiles.SelectedItems[0]);
            }
            RemoveItems(items);
        }

        private string GetCurrentTabAbbreviation()
        {
            return TabAbbreviations[FilesInFolderTabs.SelectedTab];
        }

        private void RemoveAllButtonClick(object sender, EventArgs e)
        {
            var items = SelectedFiles.Items.Cast<ListItem>().ToList();
            RemoveItems(items);
            SelectedFiles.Items.Clear();
        }

        private void AddItems(List<ListItem> itemsToAdd)
        {
            var items = new ListItem[SelectedFiles.Items.Count + itemsToAdd.Count];
            var c = 0;
            foreach (ListItem listItem in SelectedFiles.Items)
            {
                items[c] = listItem;
                c++;
            }
            foreach (var item in itemsToAdd)
            {
                items[c] = item;
                c++;
            }
            var sc = new SuffixComparer(GetCurrentTabAbbreviation());
            Array.Sort(items, sc);
            SelectedFiles.Items.Clear();
            foreach (var item in items)
            {
                SelectedFiles.Items.Add(item);
            }
        }

        private void RemoveItems(List<ListItem> itemsToRemove)
        {
            var items = new ListItem[FilesInFolder.Items.Count + itemsToRemove.Count];
            var c = 0;
            foreach (ListItem listItem in FilesInFolder.Items)
            {
                items[c] = listItem;
                c++;
            }
            foreach (var item in itemsToRemove)
            {
                items[c] = item;
                c++;
            }
            var sc = new SuffixComparer(GetCurrentTabAbbreviation());
            Array.Sort(items, sc);
            FilesInFolder.Items.Clear();
            foreach (var item in items)
            {
                FilesInFolder.Items.Add(item);
            }
        }

        public void Reset()
        {
            FilesInFolderTabs.TabPages.Clear();
            SelectedFiles.Items.Clear();
        }

        public class ListItem
        {
            public string Text;
            public string Value;

            public override string ToString()
            {
                return Text;
            }
        }

        public class SuffixComparer : IComparer<FileInfo>, IComparer<ListItem>, IComparer<string>
        {
            private readonly string ABBR;

            public SuffixComparer(string abbr)
            {
                ABBR = abbr;
            }

            public int Compare(FileInfo x, FileInfo y)
            {
                return Compare(x.Name, y.Name);
            }

            public int Compare(ListItem x, ListItem y)
            {
                return Compare(x.Text, y.Text);
            }

            public int Compare(string x, string y)
            {
                string xSuff;
                string ySuff;
                if (ABBR.Equals("_"))
                {
                    if (x.IndexOfOccurrence(ABBR, 3) < 0) return 1;
                    if (y.IndexOfOccurrence(ABBR, 3) < 0) return -1;
                    xSuff = x.Substring(x.IndexOfOccurrence(ABBR, 3));
                    ySuff = y.Substring(y.IndexOfOccurrence(ABBR, 3));
                }
                else
                {
                    if (x.LastIndexOf(ABBR) < 0) return 1;
                    if (y.LastIndexOf(ABBR) < 0) return -1;
                    xSuff = x.Substring(x.LastIndexOf(ABBR));
                    ySuff = y.Substring(y.LastIndexOf(ABBR));
                }
                var c = String.Compare(xSuff, ySuff, StringComparison.Ordinal);
                if (c == 0) return String.Compare(x, y, StringComparison.Ordinal);
                return c;
            }
        }

        #region Fields

        /// <summary>
        ///     Path to the selected folder.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        ///     Abbreviations of the analysis suitable for merging.
        ///     Vertical: GA; Horizontal: SU, PA
        /// </summary>
        private const string GA = GeneralAnalyzer.ABBR;

        private const string SU = SummaryAnalyzer.ABBR;
        private const string PA = PauseAnalyzer.ABBR;
        private const string FLUA = FluencyAnalyzer.ABBR;
        private const string SO = FocusAnalyzer.ABBR;
        private const string BA = BigramAnalyzer.ABBR;
        private const string LG = LinguisticAnalyzer.ABBR;
        private const string GEA = GeneralEyetrackAnalyzer.ABBR;
        private const string RM = RevisionMatrixAnalyzer.ABBR;
        private const string CT = CopytaskAnalyzer.ABBR;
        private const string WP = WordPausesAnalyzer.ABBR;
        private const string REP = InputLog.Core.Reporting.Output.XmlFormatter.AFFIX;

        public Dictionary<bool, string[]> Abbreviations = new Dictionary<bool, string[]>
                                                          {
                                                              {true, new[] {GA, GEA, LG, WP, RM}},
                                                              {false, new[] {SU, PA, FLUA, SO, BA, RM, CT, REP}}
                                                          };

        public Dictionary<TabPage, string> TabAbbreviations;

        private ListBox FilesInFolder
        {
            get { return (ListBox) FilesInFolderTabs.SelectedTab.Controls[0]; }
        }

        #endregion
    }
}