using System;
using System.Windows.Forms;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    ///     Shows a single Report Element that the user can choose to include or
    ///     exclude from the final Report. Each report element may be assigned a block
    ///     signifying how different elements on the report should be grouped together.
    /// </summary>
    public partial class ReportElementControl : UserControl
    {
        /// <summary>
        ///     Create a new UserControl for ReportElements. This allows users to
        ///     select whether they wish to include this reportElement in their report or not.
        ///     It also allows a user to assign a block to the ReportElement, which helps
        ///     defining final structure of the report.
        /// </summary>
        /// <param name="reportMethodIdentifier">
        ///     Non-user facing identifier for
        ///     the ReportElement represented by this control.
        /// </param>
        /// <param name="reportElementName">
        ///     User facing label/name for the ReportElement
        ///     represented by this control.
        /// </param>
        /// <param name="defaultBlock">
        ///     The default block that should be selected for this
        ///     ReportElement. The default block is set by the parent control, for instance
        ///     the analysis this ReportElement targets.
        /// </param>
        public ReportElementControl(string reportMethodIdentifier,
            string reportElementName,
            string defaultBlock)
        {
            InitializeComponent();

            // Initialize data members
            ReportMethodIdentifier = reportMethodIdentifier;
            ReportElementName = reportElementName;

            // Bind Event Handlers
            Load += ReportElementUserControlLoad;
            CheckBox.CheckedChanged += CheckBoxCheckedChanged;

            // Handle the block updating of the combo box.
            ReportingBlockComboBox = new ReportingBlockCBDecorator(Block, defaultBlock);
            Block.SelectedValueChanged += BlockSelectedValueChanged;
        }

        /// <summary>
        ///     Return the information about this ReportElement in the form
        ///     of a ReportingTarget. The returned reportingTarget does not
        ///     have the Analysis information set.
        /// </summary>
        /// <returns>
        ///     The ReportingTarget of this ReportElement. It contains the
        ///     information about which actual method _target_ is being targetted
        ///     and the block to which this element should be added. The analysis
        ///     information is not set by this method.
        /// </returns>
        public ReportingTarget GetReportingTarget()
        {
            return new ReportingTarget(
                ReportMethodIdentifier,
                ReportingBlockComboBox.SelectedBlock
                );
        }

        #region Public Fields

        /// <summary>
        ///     Is this ReportElement currently selected or not?
        /// </summary>
        public bool IsSelected
        {
            get { return CheckBox.Checked; }
            set
            {
                // If the value of the select is changed, 
                // we fire the OnSelectedStatusChange event.
                var oldValue = CheckBox.Checked;
                if (value != oldValue)
                {
                    CheckBox.Checked = value;
                    OnSelectedStatusChanged(new SelectedStatusChangedEventArgs());
                }
            }
        }

        /// <summary>
        ///     Set and get the Block that is currently selected for this ReportElement.
        /// </summary>
        public string SelectedBlock
        {
            get { return ReportingBlockComboBox.SelectedBlock; }
            set
            {
                // If the value of the selected block has changed,
                // we fire the BlockSelectionChanged event.
                var oldValue = SelectedBlock;
                if (oldValue != value)
                {
                    ReportingBlockComboBox.SelectedBlock = value;
                    OnBlockSelectionChanged(new BlockSelectionChangedEventArgs());
                }
            }
        }

        /// <summary>
        ///     User facing name of the current report element.
        /// </summary>
        public string ReportElementName
        {
            get { return Label.Text; }
            private set { Label.Text = value; }
        }

        /// <summary>
        ///     The exact identifier of the reporting method - not user facing -
        ///     represented by this user control
        /// </summary>
        public string ReportMethodIdentifier { get; private set; }

        /// <summary>
        ///     Returns the height of the ReportElementUserControl.
        /// </summary>
        public int ReportElementControlHeight
        {
            get { return Height; }
        }

        /// <summary>
        ///     A reference to the Decorator surrounding our ComboBox. The decorator
        ///     keeps the ComboBox entirely up to date and responds to any changes
        ///     that occur to the ReportingBlock lists.
        /// </summary>
        private readonly ReportingBlockCBDecorator ReportingBlockComboBox;

        #endregion

        #region Selection Event Handling

        /// <summary>
        ///     Event Arguments used when the selected status of this event
        ///     have been changed.
        /// </summary>
        public class SelectedStatusChangedEventArgs : EventArgs
        {
        }

        /// <summary>
        ///     Event that is fired whenever the selected status of this element has been changed.
        /// </summary>
        public event EventHandler<SelectedStatusChangedEventArgs> SelectedStatusChanged;

        /// <summary>
        ///     When our selection status has been changed, we fire the SelectedStatusChanged event
        ///     passing the parameters of the change that have been passed to us.
        /// </summary>
        /// <param name="e">Arguments with details about the nature of the status change.</param>
        protected virtual void OnSelectedStatusChanged(SelectedStatusChangedEventArgs e)
        {
            if (SelectedStatusChanged != null)
            {
                SelectedStatusChanged(this, e);
            }
        }

        /// <summary>
        ///     This method gets triggered if our CheckBox, and thus our own selected state (e.g. this.Selected)
        ///     has changed. We must propagate the update to any listeners listening for when our
        ///     selected status has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            OnSelectedStatusChanged(new SelectedStatusChangedEventArgs());
        }

        #endregion

        #region Block change Event Handling

        /// <summary>
        ///     Event arguments used when the Block Selection has changed for this report
        ///     element.
        /// </summary>
        public class BlockSelectionChangedEventArgs : EventArgs
        {
        }

        /// <summary>
        ///     Event that is fired whenever the selected block for this report element has changed.
        /// </summary>
        public event EventHandler<BlockSelectionChangedEventArgs> BlockSelectionChanged;

        /// <summary>
        ///     When the block selection has changed, we update all our listeners of the change
        /// </summary>
        /// <param name="e">Parameters containing the details of the selection change.</param>
        protected virtual void OnBlockSelectionChanged(BlockSelectionChangedEventArgs e)
        {
            if (BlockSelectionChanged != null)
            {
                BlockSelectionChanged(this, e);
            }
        }

        /// <summary>
        ///     If our block has been changed then we must update our own block listeners of the
        ///     change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockSelectedValueChanged(object sender, EventArgs e)
        {
            OnBlockSelectionChanged(new BlockSelectionChangedEventArgs());
        }

        #endregion

        #region Size Updating Logic

        /// <summary>
        ///     When the parent's size has changed, copy the size.
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
        ///     Adjust the elements visual properties before it is updated. We will
        ///     match the elements width to the width of its parent container.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportElementUserControlLoad(object sender, EventArgs e)
        {
            CopyParentWidth();

            // Listen to any changes in the parent size.
            Parent.SizeChanged += ParentSizeChanged;
        }

        /// <summary>
        ///     Set this elements width equal to the width of the parent container.
        /// </summary>
        private void CopyParentWidth()
        {
            Width = Parent.Width - Margin.Left - Margin.Right;
        }

        #endregion
    }
}