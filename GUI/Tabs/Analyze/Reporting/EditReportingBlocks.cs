using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    ///     A form that allows the user to edit the blocks. This means
    ///     adding new blocks, deleting existing ones and renaming existing
    ///     ones. The results are not made permanent until the user accepts
    ///     the changes and closes the dialog.
    /// </summary>
    public partial class EditReportingBlocks : Form
    {
        /// <summary>
        ///     Create the editing form.
        /// </summary>
        public EditReportingBlocks()
        {
            InitializeComponent();

            // Fill the Grid with the initial data.
            SetUpInitialBlockConfiguration();

            // Add Event listeners
            FormClosing += EditReportingBlocksFormClosing;
            FormClosed += EditReportingBlocksFormClosed;
        }

        /// <summary>
        ///     Fills the grid with all the blocks that have currently
        ///     been defined and keeps track of the initial values
        ///     of all the blocks.
        /// </summary>
        private void SetUpInitialBlockConfiguration()
        {
            var allBlocks = ReportingBlocks.Instance.GetAllBlocks().ToArray();
            Array.Sort(allBlocks);

            // For each existing block we add an EditBlock user control to our form.
            foreach (var block in allBlocks)
            {
                CreateAndAddEditBlock(block);
            }
        }

        /// <summary>
        ///     Create a new EditBlock and add it to the list of EditBlock controls we currently
        ///     have.
        /// </summary>
        /// <param name="blockName">
        ///     The name of the block. If the block is a new block
        ///     then the name should not be given. It will default to null.
        /// </param>
        private void CreateAndAddEditBlock(string blockName = null)
        {
            var editBlockControl = new EditBlock(blockName);
            editBlockControl.ProposedBlockChange += EditBlockControlProposedBlockChange;
            Blocks.Controls.Add(editBlockControl);
        }

        /// <summary>
        ///     If the AddButton has been clicked we add a new EditBlock to the list of controls.
        ///     The user can then define a new block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButtonClick(object sender, EventArgs e)
        {
            CreateAndAddEditBlock();
        }

        #region Fields

        #endregion

        #region Handle the proposed changing of block names

        /// <summary>
        ///     If the edit block has a proposed name change for a block, either the
        ///     addition of a new block, or the renaming of an existing block it proposes
        ///     these changes. After we the EditReportingBlocks has checked whether such
        ///     a change is allowed or not it either accepts or rejects the change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="blockChange"></param>
        private void EditBlockControlProposedBlockChange(object sender,
            EditBlock.ProposedBlockChangeEventArgs blockChange)
        {
            // Identify the requester.
            var requester = sender as EditBlock;
            Debug.Assert(requester != null);

            if (BlockChangeIsAcceptable(requester, blockChange))
            {
                if (requester != null) requester.AcceptChange();
            }
            else
            {
                if (requester != null) requester.RejectChange();
            }
        }

        /// <summary>
        ///     A proposed block change is an acceptable block change if it does not collide with
        ///     any of the already existing blocks, or any of the newly defined blocks, or
        ///     renamings.
        /// </summary>
        /// <param name="requester">The EditBlock that requested the change.</param>
        /// <param name="blockChange">The change to be evaluated for acceptance.</param>
        /// <returns>True if the change is an acceptable change. False if the change is invalid.</returns>
        private bool BlockChangeIsAcceptable(EditBlock requester, EditBlock.ProposedBlockChangeEventArgs blockChange)
        {
            return ChangeIsNoDuplicateOfOldValues(requester, blockChange) &&
                   ChangeIsNoDuplicateOfNewValues(requester, blockChange);
        }

        /// <summary>
        ///     Checks whether the change collides with any of the already accepted new values for blocks.
        ///     This only takes into account new values of blocks that are currently active - that is
        ///     not marked for deletion. A block that was previously marked as deleted, that is enabled again
        ///     will trigger it's own proposed change that will be rejected if its name now clashes with a
        ///     different block.
        /// </summary>
        /// <param name="requester">The EditBlock that requested the change.</param>
        /// <param name="blockChange">The proposed change that contains the proposed new value for a block</param>
        /// <returns>True if the change does not collide with any of the values of other, active blocks</returns>
        private bool ChangeIsNoDuplicateOfNewValues(EditBlock requester,
            EditBlock.ProposedBlockChangeEventArgs blockChange)
        {
            foreach (EditBlock block in Blocks.Controls)
            {
                if (block != requester &&
                    (block.NewValue == blockChange.ProposedNewValue))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Checks whether the change collides with any of the old values for blocks. Regardless of a
        ///     block's pending status for deletion or not. It is never allowed for a new value to be equal
        ///     to the value of an already existing block because this might lead to inconsistent behaviour.
        ///     The order in which the blockchanges would be persisted would influence the results of the
        ///     changes.
        /// </summary>
        /// <param name="requester">The EditBlock that requested the change.</param>
        /// <param name="blockChange">The proposed change that contains the prosed new value for a block</param>
        /// <returns>
        ///     True if the change does not collide with any values of the already existing blocks,
        ///     active or not.
        /// </returns>
        private bool ChangeIsNoDuplicateOfOldValues(EditBlock requester,
            EditBlock.ProposedBlockChangeEventArgs blockChange)
        {
            foreach (EditBlock block in Blocks.Controls)
            {
                if (block != requester &&
                    (block.OldValue == blockChange.ProposedNewValue))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Form Closing Logic

        /// <summary>
        ///     When the user is closing the form with DialogResult.OK
        ///     we must check whether there are any Errors left on any of the EditBlocks. If the form is
        ///     closed by Cancel, this is not needed.
        ///     If any errors are left we on close (with OK) we alert the user of this fact and ask
        ///     whether he wish to continue regardless.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditReportingBlocksFormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK &&
                BlocksAreInErrorState())
            {
                // These errors are really just 'soft errors'. It means that a change was 
                // rejected, automatically corrected. The user might however not have
                // noticed this correction yet, if he goes straight from entering the invalid change
                // in the NewValue of the EditBlock to clicking the DoneButton. 
                // In this case the LostFocus event that triggers the invalid change detection will be run right before
                // the form closes, there will not be any actual errors, but the user will not have 
                // had time to be alerted of this unnaceptable edit he performed.
                // We may now notify the user of this error correction that happened and 
                // and allow him to continue, or change the values. In order to make sure the
                // confirmation dialog is only triggered once, we `ResetAllErrors()`
                ResetAllErrors();

                var confirmationResult = MessageBox.Show(
                    "Your last change was invalid and has been automatically corrected. Are you sure you wish to accept the " +
                    "automatic correction and close this window?",
                    "Continue?",
                    MessageBoxButtons.YesNo
                    );

                if (confirmationResult == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///     Checks if there are any blocks that are still in error state.
        /// </summary>
        /// <returns>
        ///     True if any EditBlock is still in error state. False if
        ///     all the blocks are error free.
        /// </returns>
        private bool BlocksAreInErrorState()
        {
            foreach (EditBlock block in Blocks.Controls)
            {
                if (block.HasUnresolvedErrors)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Resets the errors on all the blocks.
        /// </summary>
        private void ResetAllErrors()
        {
            foreach (EditBlock block in Blocks.Controls)
            {
                block.ResetErrors();
            }
        }

        /// <summary>
        ///     When we close the form with DialogResult ok we persist all the changes to
        ///     the blocks that the user made.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditReportingBlocksFormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                foreach (EditBlock block in Blocks.Controls)
                {
                    block.PersistChanges();
                }
            }
        }

        #endregion
    }
}