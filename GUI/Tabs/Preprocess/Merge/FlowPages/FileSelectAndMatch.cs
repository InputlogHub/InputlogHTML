using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GUI.Flow;
using InputLog.Core.Util;
using InputLog.Core.Util.Matching;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
    /// <summary>
    ///     UserControl that allows a user to select a folder and confirm/deselect the found matches
    ///     for further processing.
    /// </summary>
#if DEBUG
    public partial class FileSelectAndMatch : MiddleClass
#else
	public partial class FileSelectAndMatch : AFileSelectionControl
	#endif
    {
        #region public_events

        /// <summary>
        ///     Enumerator specifying the different zones where changes can occur in the UserControl
        ///     that require handling by the FlowPage.
        /// </summary>
        public enum Zones
        {
            SELECTED_DIRECTORY
        };

        /// <summary>
        ///     Event that gets called when data has been changed in a zone of the user control that
        ///     requires interaction by the flow page.
        /// </summary>
        public event ControlDataChange<Zones> DataChanged;

        #endregion

        /// <summary>
        ///     Actual value of the selected path.
        /// </summary>
        private string ThisSelectedPath;

        /// <summary>
        ///     Path selected by the user.
        ///     This is the Property that adds some extra processing functionality.
        /// </summary>
        public string SelectedPath
        {
            get { return ThisSelectedPath; }
            private set
            {
                ThisSelectedPath = value;
                PathTxtBox.Text = value;
                OnDataChanged(Zones.SELECTED_DIRECTORY);
            }
        }

        /// <summary>
        ///     Total number items, selected or unselected. This counts the matches, not the number
        ///     of individual files.
        /// </summary>
        public override int ItemCount
        {
            get { return GetFlowWrappers().Count(); }
        }

        /// <summary>
        ///     Total number of selected items. This counts the flow wrappers matches, not the
        ///     number of individual files.
        /// </summary>
        public override int SelectedItemsCount
        {
            get
            {
                var wrappers = GetFlowWrappers();
                return wrappers.Aggregate(0, (sum, c) => sum + (c.Selected ? 1 : 0));
            }
        }

        private readonly string UnselectPattern;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public FileSelectAndMatch(string unselectPattern)
        {
            InitializeComponent();
            UnselectPattern = unselectPattern;
        }

        #region button_click_handlers

        /// <summary>
        ///     Select all button has been clicked.
        ///     Select all the FlowControlWrappers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllButtonClick(object sender, EventArgs e)
        {
            foreach (var wrapper in GetFlowWrappers())
            {
                wrapper.ChangeSelection(this, true);
            }
        }

        /// <summary>
        ///     Deselect all button has been clicked.
        ///     Deselect all the FlowControlWrappers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeselectAllButtonClick(object sender, EventArgs e)
        {
            foreach (var wrapper in GetFlowWrappers())
            {
                wrapper.ChangeSelection(this, false);
            }
        }

        #endregion

        //
        // Selection of the startpath for selecting and matching files.
        // 

        #region folder_selection

        /// <summary>
        ///     The user has clicked the BrowseButton. We display the BrowserDialog and
        ///     update the path if it changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButtonClick(object sender, EventArgs e)
        {
            FolderBrowserDialog.SelectedPath = (string) RegistryTools.GetSetting("Inputlog", "DirPath",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = FolderBrowserDialog.SelectedPath;
            }
        }

        #endregion

        /// <summary>
        ///     Fire the dataChanged event if there are listeners
        /// </summary>
        /// <param name="changeZone">Zone where the change has occurred.</param>
        private void OnDataChanged(Zones changeZone)
        {
            if (DataChanged != null)
            {
                DataChanged(changeZone);
            }
        }

        /// <summary>
        ///     Clear whichever matches are currently listed in the MatchPanel
        /// </summary>
        public void ClearMatches()
        {
            FlowPanelClearMatchesTS();
        }

        /// <summary>
        ///     Display the new matches within the MatchPanel.
        /// </summary>
        /// <param name="matches">List of all the discovered matches</param>
        public void NewMatches(List<IMatch<string>> matches)
        {
            // Create and add FlowControlWrappers to the FlowPanel for each match.
            SuspendLayout();
            foreach (var match in matches)
            {
                foreach (var file in match.Items())
                {
                    if (Regex.IsMatch(file, UnselectPattern)) match.ChangeItemSelection(file, false);
                }
                var wrapperContent = new FileExtSelectionControl(match.Items(), match.SelectedItems());
                var wrapper = new FlowControlWrapper(FlowPanel, wrapperContent, CloseWrapper);
                wrapper.SelectionChanged += HandleSelectionChanged;
                wrapperContent.RegisterSizeChangeListener(wrapper);
                FlowPanelAddControl(wrapper);
            }
            ResumeLayoutTS();
        }

        private void HandleSelectionChanged(object sender, bool selected, List<string> files)
        {
            OnSelectionChanged(sender, selected, files);
        }

        /// <summary>
        ///     Handles the closing of a wrapper.
        /// </summary>
        /// <param name="sender">The </param>
        /// <param name="args"></param>
        private void CloseWrapper(object sender, EventArgs args)
        {
            var fWrapper = sender as FlowControlWrapper;
            if (fWrapper == null) return;
            FlowPanel.Controls.Remove(fWrapper);
            fWrapper.ChangeSelection(this, false);
        }

        #region thread_safe_helpers

        /// <summary>
        ///     FlowPanel.Controls.Clear() - thread safe
        /// </summary>
        private delegate void FlowPanelClearMatchesDelegate();

        private void FlowPanelClearMatchesTS()
        {
            if (FlowPanel.InvokeRequired)
            {
                FlowPanelClearMatchesDelegate d = FlowPanelClearMatchesTS;
                Invoke(d);
            }
            else
            {
                FlowPanel.Controls.Clear();
            }
        }


        /// <summary>
        ///     FlowPanel.ResumeLayout() thread safe
        /// </summary>
        private delegate void FlowPanelResumeLayoutDelegate();

        private void ResumeLayoutTS()
        {
            if (FlowPanel.InvokeRequired)
            {
                FlowPanelResumeLayoutDelegate d = ResumeLayoutTS;
                Invoke(d);
            }
            else
            {
                ResumeLayout();
            }
        }

        /// <summary>
        ///     Add a control to the flowPanel in a thread safe manner.
        /// </summary>
        /// <param name="control">Control to add to the FlowPanel</param>
        private delegate void FlowPanelControlAddDelegate(Control control);

        private void FlowPanelAddControl(Control control)
        {
            if (FlowPanel.InvokeRequired)
            {
                FlowPanelControlAddDelegate d = FlowPanelAddControl;
                Invoke(d, control);
            }
            else
            {
                FlowPanel.Controls.Add(control);
            }
        }

        #endregion

        #region FileSelection

        /// <summary>
        ///     Change the selection of some files in this control. If no list of files is specified,
        ///     it involves all files.
        /// </summary>
        /// <param name="sender">Sender of the selection change. Use this to make sure you do not loop selections and events.</param>
        /// <param name="selected">Whether the files are selected (=true) or deselected (=false)</param>
        /// <param name="files">The list of files that will have their selection changed. Or null if it involves all files.</param>
        public override void ChangeSelection(object sender, bool selected, List<string> files = null)
        {
            foreach (Control control in FlowPanel.Controls)
            {
                if (!(control is AFileSelectionControl)) continue;
                var selectionControl = control as AFileSelectionControl;
                selectionControl.ChangeSelection(sender, selected, files);
            }
        }

        /// <summary>
        ///     Method that gets called when there are some files that have issues. These fils
        ///     will be unselected. Optionally the control may display some sort of error
        ///     marking the files as problem files.
        /// </summary>
        /// <param name="files">List of files that has problems and should be unselected</param>
        public override void MarkProblemFiles(List<string> files)
        {
            foreach (Control control in FlowPanel.Controls)
            {
                if (!(control is AFileSelectionControl)) continue;
                var selectionControl = control as AFileSelectionControl;
                selectionControl.MarkProblemFiles(files);
            }
        }

        #endregion

        private IEnumerable<FlowControlWrapper> GetFlowWrappers()
        {
            return FlowPanel.Controls.OfType<FlowControlWrapper>();
        }
    }
}