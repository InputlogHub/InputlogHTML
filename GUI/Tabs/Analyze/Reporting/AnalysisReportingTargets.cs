using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using InputLog.Core.Reporting;
using InputLog.Core.Reporting.Report;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    ///     Shows the different report targets available per analysis. The user may check or uncheck
    ///     the reporting capabilities he or she wishes and then determine which block a single
    ///     reporting element, or all elements of the given analysis will be a part of in the
    ///     final outputted report structure.
    /// </summary>
    public partial class AnalysisReportingTargets : UserControl
    {
        /// <summary>
        ///     Create a new userControl for selecting which reporting information the
        ///     user would like to add, and how - in which blocks - the user would like
        ///     to see that information structured.
        /// </summary>
        /// <param name="analysisName">
        ///     The name of the analysis whose reporting targets
        ///     we are currently listing
        /// </param>
        /// <param name="reportingTargets">
        ///     The available reporting targets for
        ///     this analysis. These are based on the reporting resources.
        ///     They contain the ID, label, method, description, introduction, and anything 
        ///     else that is known about the target.
        /// </param>
        public AnalysisReportingTargets(string analysisName, List<Report.ReportResource> reportingTargets)
        {
            Debug.Assert(reportingTargets != null);
            Debug.Assert(reportingTargets.Count > 0);

            InitializeComponent();

            // Initialize data members
            AnalysisName = analysisName;
            ReportingTargets = reportingTargets;

            // Before loading, fix the layout of this element.
            Load += AnalysisReportingTargetsLoad;

            // Delegate all the handling of the updating of the Reporting ComboBox to
            // a dedicated handler that wraps our ComboBox. The default block for 
            // our analysis' block is the analysis name.
            ReportingBlockComboBox = new NullableReportingBlockCbDecorator(AnalysisBlock, AnalysisName);
        }

        /// <summary>
        ///     Method called before the user control is shown. It initializes its data fields with
        ///     information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisReportingTargetsLoad(object sender, EventArgs e)
        {
            // Set title
            AnalysisTitle.Text = AnalysisName;

            // Add ReportElementUserControls to this UserControl for each available
            // reportingTarget.
            foreach (Report.ReportResource target in ReportingTargets)
            {
                string humanReadableTarget = target.Label;
                string targetId = target.BaseId;
                var reportElement =
                    new ReportElementControl(targetId, humanReadableTarget, AnalysisName);

                Data.Controls.Add(reportElement);
            }

            // Update the size of the control, depending on the size of the list.
            ReportElementControlHeight = ((ReportElementControl) Data.Controls[0]).ReportElementControlHeight;
            UpdateControl();

            // Register Event handlers.
            Parent.SizeChanged += ParentSizeChanged;
            BindSelectionChangeListeners();
            BindBlockChangeListeners();
        }

        /// <summary>
        ///     Get all the selected ReportingTargets in this Control that the user
        ///     wants to include in the report, together with other relevant information about
        ///     the target and how it should appear in the report.
        /// </summary>
        /// <returns>
        ///     A list of all the reporting targets that the user has selected
        ///     including any information about how they should be included in the
        ///     report. This is the (resource) ID of the target.
        /// </returns>
        internal List<ReportingTarget> GetSelectedReportingTargets()
        {
            var targets = new List<ReportingTarget>();

            foreach (ReportElementControl reportElement in Data.Controls)
            {
                if (reportElement.IsSelected)
                {
                    var target = reportElement.GetReportingTarget();
                    target.Analysis = AnalysisName;
                    targets.Add(target);
                }
            }
            return targets;
        }

        #region Fields

        /// <summary>
        ///     Name of the this analysis. This name is shown to the user.
        /// </summary>
        public string AnalysisName { get; private set; }

        /// <summary>
        ///     Targets for reporting that are available for this analysis
        /// </summary>
        private readonly List<Report.ReportResource> ReportingTargets;

        /// <summary>
        ///     Contains the height of the reportElement controls that we add for
        ///     each available reportingTarget.
        /// </summary>
        private int ReportElementControlHeight;

        /// <summary>
        ///     A reference to the Decorator surrounding our ComboBox. The decorator
        ///     keeps the ComboBox entirely up to date and responds to any changes
        ///     that occur to the ReportingBlock lists.
        /// </summary>
        private readonly NullableReportingBlockCbDecorator ReportingBlockComboBox;

        #endregion

        #region Updating Size Information

        /// <summary>
        ///     Update this element if the parent flowpanel changed in size. We will copy the width.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentSizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();
            CopyParentWidth();
            ResumeLayout();
        }

        /// <summary>
        ///     Copy the width of the parent element.
        /// </summary>
        private void CopyParentWidth()
        {
            Width = Parent.Width - Margin.Left - Margin.Right;
        }

        /// <summary>
        ///     Updates properties of the Control. In this case it updates the height of the control
        ///     in order to accomodate any added report target user controls.
        /// </summary>
        private void UpdateControl()
        {
            // Determine height required by the data panel.
            var heightForUserControls = ReportingTargets.Count*ReportElementControlHeight;
            Height = TitleLayout.Height + heightForUserControls;
        }

        #endregion

        #region Select All / Deselect All logic

        /// <summary>
        ///     Select all the ReportElements for this analysis.
        /// </summary>
        public void SelectAll()
        {
            ChangeSelectStatusAll(true);
        }

        /// <summary>
        ///     Deselect all the ReportElements for this analysis.
        /// </summary>
        public void DeselectAll()
        {
            ChangeSelectStatusAll(false);
        }

        /// <summary>
        ///     Change the select status for all ReportElements for this analysis.
        /// </summary>
        /// <param name="select">
        ///     True to select all the ReportElements, False to
        ///     deselect them all.
        /// </param>
        private void ChangeSelectStatusAll(bool select)
        {
            UnbindSelectionChangeListeners();

            CheckBox.Checked = select;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                reportElement.IsSelected = select;
            }

            BindSelectionChangeListeners();
        }

        /// <summary>
        ///     Register for all SelectionChange events of all the reportElements and
        ///     this elements SelectionChange events.
        ///     checkboxes.
        /// </summary>
        private void BindSelectionChangeListeners()
        {
            CheckBox.CheckedChanged += CheckBoxCheckedChanged;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                reportElement.SelectedStatusChanged += ReportElementSelectedStatusChanged;
            }
        }

        /// <summary>
        ///     Unregister from all the SelectionChange events of the reportElements and
        ///     this elements SelectionChange events.
        /// </summary>
        private void UnbindSelectionChangeListeners()
        {
            CheckBox.CheckedChanged -= CheckBoxCheckedChanged;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                reportElement.SelectedStatusChanged -= ReportElementSelectedStatusChanged;
            }
        }

        /// <summary>
        ///     React to a selection status change of one of our ReportElements (children). This requires
        ///     us to possibly update our own selection status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportElementSelectedStatusChanged(object sender,
            ReportElementControl.SelectedStatusChangedEventArgs e)
        {
            UpdateSelectedStatus();
        }

        /// <summary>
        ///     Update our own SelectedStatus based on whether or not all of our ReportElementUserControls
        ///     are selected, or not. If all of them are selected, our SelectedStatus will be selected as well.
        ///     If some of them are selected, our SelectedStatus will be Null. If none of them are selected
        ///     Our SelectedStatus will be false.
        ///     NOTE: Updates performed using this method will not trigger any SelectedStateChanged events.
        /// </summary>
        private void UpdateSelectedStatus()
        {
            var allReportElementsSelected = true;
            var allReportElementsUnselected = true;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                allReportElementsSelected &= reportElement.IsSelected;
                allReportElementsUnselected &= !reportElement.IsSelected;
            }

            // We use checkstate here because we can have one of three values. 
            // 1. CheckState.Checked > true
            // 2. CheckState.Unchecked > false
            // 3. CheckState.Indeterminate > null... some report elements are selected, 
            //    some report elements are unselected.

            if (!allReportElementsUnselected && !allReportElementsSelected)
            {
                // Set to indeterminate.
                ChangeMySelectedState(CheckState.Indeterminate);
            }
            else if (allReportElementsSelected)
            {
                // Set to checked
                ChangeMySelectedState(CheckState.Checked);
            }
            else
            {
                // Set to unchecked
                ChangeMySelectedState(CheckState.Unchecked);
            }
        }

        /// <summary>
        ///     Change the state of this elements SelectedState, without triggering
        ///     any update events for this.
        /// </summary>
        /// <param name="newState">The new selection state for the element.</param>
        private void ChangeMySelectedState(CheckState newState)
        {
            var oldValue = CheckBox.CheckState;

            if (oldValue != newState)
            {
                UnbindSelectionChangeListeners();
                CheckBox.CheckState = newState;
                BindSelectionChangeListeners();
            }
        }

        /// <summary>
        ///     Event handler for when the our SelectedState has changed. If we have been changed
        ///     to Selected then we must Select all our Report Elements too. If we have been changed
        ///     to Unselected then we must Unselect all our Report Elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox.Checked)
            {
                SelectAll();
            }
            else
            {
                DeselectAll();
            }
        }

        #endregion

        #region Block selection / deselection logic

        /// <summary>
        ///     Bind all change listeners that listen to block selection changes.
        /// </summary>
        private void BindBlockChangeListeners()
        {
            AnalysisBlock.SelectedValueChanged += AnalysisBlockSelectedValueChanged;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                reportElement.BlockSelectionChanged += ReportElementBlockSelectionChanged;
            }
        }

        /// <summary>
        ///     Unbind the listeners that listen to block selectoin changes.
        /// </summary>
        private void UnbindBlockChangeListeners()
        {
            AnalysisBlock.SelectedValueChanged -= AnalysisBlockSelectedValueChanged;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                reportElement.BlockSelectionChanged -= ReportElementBlockSelectionChanged;
            }
        }

        /// <summary>
        ///     The Analysis has it's block selection changed. That means we must change
        ///     the block selection of all our reporting elements as well, to the same block
        ///     as our Analysis. EXCEPT if the newly selected block is the NULL block.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisBlockSelectedValueChanged(object sender, EventArgs e)
        {
            if (!ReportingBlockComboBox.IsSetToNull())
            {
                UnbindBlockChangeListeners();

                var selectedBlock = ReportingBlockComboBox.SelectedBlock;
                foreach (ReportElementControl reportElement in Data.Controls)
                {
                    reportElement.SelectedBlock = selectedBlock;
                }

                BindBlockChangeListeners();
            }
        }

        /// <summary>
        ///     One of the report elements has its block selection changed. That means we might
        ///     have to update our own selection as well. If all the blocks have the same selection,
        ///     then we update our block to be the same one as our reportElements' blocks.
        ///     If our reportElements have different blocks assigned, then we update our own
        ///     block to the NULL block.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportElementBlockSelectionChanged(object sender,
            ReportElementControl.BlockSelectionChangedEventArgs e)
        {
            UnbindBlockChangeListeners();

            var uniformlySelectedBlock = IdentifyUniformBlockSelected();
            if (uniformlySelectedBlock != null)
            {
                ReportingBlockComboBox.SelectedBlock = uniformlySelectedBlock;
            }
            else
            {
                ReportingBlockComboBox.SetToNull();
            }

            BindBlockChangeListeners();
        }

        /// <summary>
        ///     Detect whether all the ReportElements have the same block selected or not.
        ///     And if they do have the same block selected, return the name of that block.
        /// </summary>
        /// <returns>
        ///     The name of the block that all ReportElements have selected, if all the
        ///     ReportElements have the same block selected. If the ReportElements do not all have
        ///     the same block selected, this method returns null.
        /// </returns>
        private string IdentifyUniformBlockSelected()
        {
            string selectedBlock = null;
            foreach (ReportElementControl reportElement in Data.Controls)
            {
                // If it's not the first time we go through this loop
                // and the selected block of the next element differs from the previous one: 
                if (selectedBlock != null &&
                    selectedBlock != reportElement.SelectedBlock)
                {
                    return null;
                }
                selectedBlock = reportElement.SelectedBlock;
            }
            return selectedBlock;
        }

        #endregion
    }
}