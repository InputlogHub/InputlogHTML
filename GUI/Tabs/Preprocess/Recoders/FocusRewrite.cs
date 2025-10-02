using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using InputLog.Core.Util;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.IO.Basic;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Recode;
using InputLog.Core.IO;

namespace GUI.Tabs.Preprocess.Recoders
{
	public partial class FocusRewrite : ProcessControl
	{
		#region Fields
		/// <summary>
		/// Name of the post processer.
		/// </summary>
		public const string NAME = "Sources";
        /// <summary>
        /// XmlDocument used to read xml from the analysis configuration file.
        /// </summary>
        private XmlDocument XMLDocument;

        /// <summary>
        /// Returns whether this process control can handle multiple files at the same
        /// time or can only process one time at a time.
        /// </summary>
        public override bool MultipleFileCompatible => true;

	    /// <summary>
		/// The focus rewriter preprocessor that does the actual processing
		/// </summary>
		public  FocusRewriter FRewriter { get; private set; }
        public static List<string> SavedFiles { get; private set; }
        private FocusRewriter.Group _newGroup;

        /// <summary>
        /// List of all idfx files currently in the 'focusRewriters' collection.
        /// </summary>
        private readonly List<string> IDFXList;
       
        /// <summary>
        /// Tags used in the saved grouped sources.
        /// </summary>
	    private static string SOURCELOG_TAG = "Preprocessing";
        private static string FILES_TAG = "Files";
	    private static string FOCUSGROUP_TAG = "FocusGroup";
	    private static string FOCUSID_ATTRIBUTE = "ID";
        private static string UNGROUPED_TAG = "Ungrouped";

	    public List<string> UngroupedSources { get; set; }
        public List<KeyValuePair<string, List<string>>> GroupedSources { get; set; }
	    public static bool LoadFromLogFile;

        #endregion

        /// <summary>
        /// Create a focus rewrite post processing control.
        /// </summary>
        public FocusRewrite() : base("Focus Rewriter","focusRewrite")
		{
			InitializeComponent();
            FilterAbbreviation = "recode";
            InitHelp(MoreInfoLabel, FilterAbbreviation);
			FRewriter = new FocusRewriter();
			IDFXList = new List<string>();
        }

		/// <summary>
		/// Update the in memory list of all focus events in all the source files. This removes from 'EventList'
		/// any source file no longer in the sources list, and adds any focus events from source files that have
		/// been added to the source list.
		/// </summary>
		private void UpdateEventLists()
		{
			// The sources are the filePaths.
			var sources = FilePaths;

			// The initial case when there are no sources selected yet.
			if (sources == null || IDFXList == null) return;

			// We only need focus events.
			EventTypeFilter focusTypeFilter = new EventTypeFilter(new[] { "focus" });

			// Read every source file.
			foreach (string source in sources)
			{
				// If the source file has not been read yet, read & filter it.
				if (!IDFXList.Contains(source))
				{
					var eventLogReader = EventLogFactory.CreateFileEventLogReader(source, LogFormat.XML);
                    SessionIdentification sessionId = eventLogReader.ReadSessionIdentification();
					List<Event> sourceEvents = eventLogReader.ReadEvents();

				    //Filter the sourceEvents on focus events.
					sourceEvents.Filter(focusTypeFilter);

					// Get the list of window titles of the focus events.
                    List<string> focusEvents = new List<string>();
                    // Returns the main document name from the session information if it exists.
                    var mainDoc = sessionId.GetMainDocument().IsNullOrEmpty() ? "UNKNOWN" : sessionId.GetMainDocument();

                    foreach (Event e in sourceEvents)
                    {
                        string windowTitle = ((FocusChange)e.Parts.Single()).WindowTitle;
                        focusEvents.Add(windowTitle);
                    }

                    FRewriter.AddIdfx(source, focusEvents);
                    FRewriter.AddMainDocs(source, mainDoc);
				}
			}

			// Remove the eventLists that are no longer in the Source List.
			foreach (string idfx in IDFXList)
			{
				if (!sources.Contains(idfx))
				{
					FRewriter.RemoveIdfx(idfx);
				}
			}

			UpdateCompactLists();
		}

		/// <summary>
		/// Returns the preprocessor associated with the FocusRewriting Preprocessor control.
		/// </summary>
		/// <returns>The preprocessor of this control.</returns>
        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
		{
			return FRewriter;
		}

		/// <summary>
		/// Serializes this filterControl.
		/// Deriving classes should implement this method. See runAnalysis() for more info.
		/// </summary>
		public override void Serialize()
		{
			// Deriving class should override this method
			throw new NotSupportedException("This method cannot be called on this class."
			+ " You should override this method in deriving classes.");
		}

		/// <summary>
		/// Exports this filterControl into a configuration.
		/// Deriving classes should implement this method. See GetEventFilter() for more 
		/// info on why this method is not abstract.
		/// </summary>
		/// <param name="options">Options specifying whether certain fields should be exported or not.</param>
		/// <returns>An FilterConfiguration that reflects the configuration of the filter that is represented 
		/// by this control. An filter of the same type should be able to be initialized to 
		/// the same configuration by passing this FilterConfiguration to its Import() method.</returns>
		public override FilterConfiguration Export(FilterExportOptions options)
		{
			// Deriving class should override this method
			throw new NotSupportedException("This method cannot be called on this class."
			+ " You should override this method in deriving classes.");
		}

		/// <summary>
		/// Imports a configuration into this filterControl.
		/// Deriving classes should implement this method. See GetEventFilter() for more 
		/// info on why this method is not abstract.
		/// </summary>
		/// <param name="configuration">An FilterConfiguration that reflects the configuration of an filter 
		/// that can be represented by this control. </param>
		public override void Import(FilterConfiguration configuration)
		{
			// Deriving class should override this method
			throw new NotSupportedException("This method cannot be called on this class."
			+ " You should override this method in deriving classes.");
		}

		//-----------------------------------------------------------------------------------------
		// GUI Actions handling.
		//

		private void HandleEditGroupDialog()
		{
			// Get selected index (if any)
			string selectedGroup = null;
			if (Grouped.SelectedItems.Count > 0)
			{
				selectedGroup = Grouped.SelectedItems[0].Text;
			}

			FocusRewriteDialog editDialog = new FocusRewriteDialog(this, FRewriter, selectedGroup);
			editDialog.ShowDialog();

			UpdateCompactLists();
		}

		private void GroupedItemDoubleClicked(object sender, EventArgs e)
		{
			HandleEditGroupDialog();
		}

		private void NewGroupButtonClick(object sender, EventArgs e)
		{
			HandleEditGroupDialog();
		}

		private void UngroupedDoubleClicked(object sender, EventArgs e)
		{
			HandleEditGroupDialog();
		}

        /// <summary>
        /// Loading an xml file containing all the sources that have been collected into group files,
        /// and repopulating the FocusRewriteDialog with this information. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBtn_Click_1(object sender, EventArgs e)
        {
            if (SelectSavedLogFile.ShowDialog() == DialogResult.OK)
            {
                var fileName = SelectSavedLogFile.FileName;
                var file = Path.GetFileNameWithoutExtension(fileName);
                if (file != null)
                {
                    DeserializeSourceLog(fileName);                    
                }
                else
                {
                    MessageBox.Show("Could not open file containing grouped source information.", "File Exception",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //-----------------------------------------------------------------------------------------
        // GUI Content handling
        //

	    /// <summary>
	    /// Deserializes a list of Focus groups from an XML file.
	    /// </summary>
	    /// <param name="fileName"></param>
	    /// <returns>A list of files and focus groups read from the XML file.</returns>
	    private void DeserializeSourceLog(string fileName)
        {
            XMLDocument = new XmlDocument { PreserveWhitespace = true };
            XMLDocument.Load(fileName);

            var logTag = XMLDocument[SOURCELOG_TAG];
            if (logTag != null)
            {
                try
                {
                    var filesGroup = logTag.GetElementsByTagName(FILES_TAG);                  
                    var focusGroups = logTag.GetElementsByTagName(FOCUSGROUP_TAG);
                    var ungroupedGroup = logTag.GetElementsByTagName(UNGROUPED_TAG);
                    var sources = new List<string>();
                    // Get the files
                    foreach (var files in filesGroup.OfType<XmlElement>())
                    {
                        foreach (var source in files.OfType<XmlElement>())
                        {
                            sources.Add(source.InnerText);                      
                        }
                    }

                    try
                    {
                        FilePaths = sources;
                        SavedFiles = sources;
                    }
                    catch(IOException)
                    {
                        MessageBox.Show("Please, ensure that the sources files are accessible", "Could Not Find Files",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    LoadFromLogFile = true;
                    UngroupedSources = new List<string>();
                    GroupedSources = new List<KeyValuePair<string, List<string>>>();
                    // Get the different groups
                    foreach (var focus in focusGroups.OfType<XmlElement>())
                    {
                        var id = focus.Attributes[FOCUSID_ATTRIBUTE].Value;                      
                        var sourceList = new List<string>();
                        foreach (var source in focus.OfType<XmlElement>())
                        {
                            sourceList.Add(source.InnerText);
                        }
                        var groupInfo = new KeyValuePair<string, List<string>>(id, sourceList);
                        GroupedSources.Add(groupInfo);

                        // Set the new group name.
                        _newGroup = new FocusRewriter.Group(FRewriter) {Name = id};
                        // Save the group first, then add the items.
                        FRewriter.SaveNewGroup(_newGroup);                
                        // Insert the new items
                        foreach (string windowTitle in sourceList)
                        {
                            _newGroup.AddWindowTitle(windowTitle);
                        }
                    }

                    // Get the ungrouped files
                    foreach (var ungrouped in ungroupedGroup.OfType<XmlElement>())
                    {
                        foreach (var source in ungrouped.OfType<XmlElement>())
                        {
                            UngroupedSources.Add(source.InnerText);
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to import a focus group log file.");
                }
                HandleEditGroupDialog();
            }
            else
            {
                MessageBox.Show("This is not a file containing grouped source information.", "Invalid File",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCompactLists()
		{
			// Clear the current lists.
			Ungrouped.Items.Clear();
			Grouped.Items.Clear();

			// Get lists
			List<FocusRewriter.Group> groups = FRewriter.GetGroups();
			IEnumerable<string> ungrouped = FRewriter.GetUngrouped();

			// ungrouped items.
			foreach(string item in ungrouped)
			{
				Ungrouped.Items.Add(item);
			}

			// grouped items.
			foreach (FocusRewriter.Group group in groups)
			{
			    var row = new ListViewItem {Text = @group.Name};
			    row.SubItems.Add(group.Count.ToString());
				Grouped.Items.Add(row);
			}
		}

		public override void OnFileSelectionChange(List<string> filePaths)
		{
			base.OnFileSelectionChange(filePaths);

			// Update the FocusRewriter with the new files.
			UpdateEventLists();
		}
	}
}
