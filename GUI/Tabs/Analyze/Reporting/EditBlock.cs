using System;
using System.Diagnostics;
using System.Windows.Forms;
using InputLog.Core.Reporting;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    ///     Allows the editing of a single Block. It can be selected or deselected
    ///     in order to add or delete it. And the values can be changed in order
    ///     to rename the block.
    /// </summary>
    public partial class EditBlock : UserControl
    {
        /// <summary>
        ///     Create a new ReportingBlockChanges instances that tracks the changes
        ///     that have been made to a certain block.
        /// </summary>
        /// <param name="originalName">
        ///     The name of the block before any changes
        ///     have been allowed.
        /// </param>
        public EditBlock(string originalName)
        {
            InitializeComponent();

            // Set GUI elements initially
            CheckBox.Checked = true;
            OldValueField.ReadOnly = true;

            // Set data members
            OldValue = originalName;

            // Register event listeners
            Load += EditBlockLoad;
            NewValueField.LostFocus += NewValueFieldLostFocus;
            NewValueField.GotFocus += NewValueFieldGotFocus;

            // Special behavior to disable any editing if this 
            // EditBlock represents the fallback block. 
            if (OldValue == ReportingBlocks.Instance.Fallback)
            {
                DisableEditing();
            }
        }

        /// <summary>
        ///     Disables the editing possibilities for this EditBlock. This may
        ///     be done if the block is a block that should not allow any editing, as
        ///     may be the case with standard blocks.
        /// </summary>
        private void DisableEditing()
        {
            CheckBox.Enabled = false;
            OldValueField.Enabled = false;
            NewValueField.Enabled = false;
        }

        /// <summary>
        ///     Save the changes saved in this EditBlock so that the actual
        ///     changes get pushed to the ReportingBlocks and all of their
        ///     listeners.
        /// </summary>
        public void PersistChanges()
        {
            Debug.Assert(!HasPendingChanges,
                "EditBlock.PersistChanges() - Can not persist changes when unapproved changes are still pending.");

            if (!IsUnchanged())
            {
                if (HasBeenAdded())
                {
                    ReportingBlocks.Instance.Add(NewValue);
                }
                else if (HasBeenDeleted())
                {
                    ReportingBlocks.Instance.Delete(OldValue);
                }
                else if (HasBeenRenamed())
                {
                    ReportingBlocks.Instance.Rename(OldValue, NewValue);
                }
                else
                {
                    Debug.Assert(false, "EditBlock.PersistChanges() - Inconsistent state detected");
                }
            }
        }

        #region Fields

        /// <summary>
        ///     Size offset because our Formlayout has a 1px border.
        /// </summary>
        private static readonly int PIXEL_BORDER_OFFSET = 2;

        /// <summary>
        ///     The original value of the block. This value can not change.
        /// </summary>
        public string OldValue
        {
            get { return OldValueField.Text; }
            private set { OldValueField.Text = value; }
        }

        /// <summary>
        ///     The new value for the block.
        /// </summary>
        public string NewValue
        {
            get
            {
                // If the EditBlock is not selected, it is currently not active and thus
                // its NewValue is 'non-existant' until marked as active again. 
                if (IsSelected())
                {
                    return NewValueField.Text;
                }
                return null;
            }
            private set { NewValueField.Text = value; }
        }

        /// <summary>
        ///     A temporary buffer that keeps the value of the NewValueField, right
        ///     before it gets focus. If the field loses focus and we can see that the
        ///     Field has a new value, different from the one stored here. The new
        ///     change will be Submitted for acceptance.
        ///     If the change is accepted, the new value becomes accepted, otherwise
        ///     the new value is overwritten the value saved in the PreChangeBuffer.
        /// </summary>
        private string PreChangeBuffer;

        /// <summary>
        ///     This bool is set to true if a proposed change event has been fired, but
        ///     the change has not yet been accepted or rejected. During the period
        ///     where this EditBock has pending changes, the changes can not be
        ///     persisted.
        ///     if this EditBlock has no more pending changes, this bool is set to false.
        /// </summary>
        private bool HasPendingChanges;

        /// <summary>
        ///     Boolean that is set to true if the EditBlock has errors that are not yet resolved.
        ///     This is the case when a change is rejected, until then a change is accepted, or the
        ///     errors are reset.
        /// </summary>
        public bool HasUnresolvedErrors { get; private set; }

        #endregion

        #region Resize Logic

        /// <summary>
        ///     On load, changes the visual characteristics of this user control
        ///     in order to match the parent.
        ///     - Set width equal to parent container.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditBlockLoad(object sender, EventArgs e)
        {
            MatchParentWidth();
        }

        /// <summary>
        ///     Adjust the width of this control according to the width of the
        ///     parent container.
        /// </summary>
        private void MatchParentWidth()
        {
            Width = Parent.Width - Margin.Left - Margin.Right - PIXEL_BORDER_OFFSET;
        }

        #endregion

        #region Detecting Changes to the Blocks

        /// <summary>
        ///     Returns whether this ReportingBlock is a new block which
        ///     did not exist before yet.
        /// </summary>
        /// <returns>True if the block is a new block, false if not.</returns>
        private bool HasBeenAdded()
        {
            return IsSelected() &&
                   (string.IsNullOrWhiteSpace(OldValue) && (!string.IsNullOrWhiteSpace(NewValue)));
        }

        /// <summary>
        ///     Returns whether this ReportingBlock with original value OldValue
        ///     has now been deleted.
        /// </summary>
        /// <returns>
        ///     True if the block has been deleted. False if
        ///     it hasn't.
        /// </returns>
        private bool HasBeenDeleted()
        {
            // You are not selected and not a new one. 
            return !IsSelected() && (!string.IsNullOrWhiteSpace(OldValue));
        }

        /// <summary>
        ///     The ReportingBlock has had its name changed to something new.
        /// </summary>
        /// <returns>True if the block has been renamed.</returns>
        private bool HasBeenRenamed()
        {
            // You are not selected, you have an oldValue (otherwise you are new),
            // You have a newValue (otherwise you have not been renamed) and
            // The values for your old and new values are different.
            return IsSelected() &&
                   (!string.IsNullOrWhiteSpace(NewValue)) &&
                   (!string.IsNullOrWhiteSpace(OldValue)) &&
                   (NewValue != OldValue);
        }

        /// <summary>
        ///     The reporting block has not changed.
        /// </summary>
        /// <returns>
        ///     True if the reporting block has remained unchanged. False if
        ///     it has been changed.
        /// </returns>
        private bool IsUnchanged()
        {
            // You either are selected, but your value's haven't changed.
            // You are old and unchanged
            var oldAndUnchanged =
                IsSelected() &&
                (
                    OldValue == NewValue || // Your values are unchanged
                    string.IsNullOrWhiteSpace(NewValue) // Your values are unchanged - no new value has been provided.
                    );

            // You are new, but have been excluded
            var newAndExcluded = !IsSelected() && (string.IsNullOrWhiteSpace(OldValue));

            return oldAndUnchanged || newAndExcluded;
        }

        /// <summary>
        ///     Returns whether this reporting block has been selected. An element
        ///     that is selected is 'active'. This means it is definitely not being
        ///     deleted. It might still be 'added', 'renamed' or remain 'unchanged'.
        /// </summary>
        /// <returns></returns>
        private bool IsSelected()
        {
            return CheckBox.Checked;
        }

        #endregion

        #region Handling Change Events to Blocks

        // 
        // Changing the 'active' status of a block. Setting to inactive means
        // marking it for deletion
        // 

        /// <summary>
        ///     When the EditBlock becomes unselected it maintains its state
        ///     but is - until selected once again - marked as `HasBeenDeleted`.
        ///     However, for any checks of duplicate block-names etc it will still
        ///     be taken into consideration.
        ///     Visually the EditBlock will be clearly inactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox.Checked)
            {
                ActivateEditBox();
            }
            else
            {
                DisableEditBox();
            }
        }

        /// <summary>
        ///     Enable this box - makes it clear to the user that
        ///     changes in this edit box will be reflected later on.
        ///     If the box is switched from inactive to active a ProposedBlockChangeEvent
        ///     is fired to make sure that changes that happened while the block was inactive
        ///     have not cause the Blocks current NewValue to be an invalid value.
        /// </summary>
        private void ActivateEditBox()
        {
            ChangeToActiveStatus(true);

            var blockChange =
                new ProposedBlockChangeEventArgs(NewValue);
            OnProposedBlockChange(blockChange);
        }

        /// <summary>
        ///     Disable this box - makes it clear to the user
        ///     that this editbox is no longer being used.
        /// </summary>
        private void DisableEditBox()
        {
            ChangeToActiveStatus(false);
        }

        /// <summary>
        ///     Update the 'active' status of our text fields. Setting to active means
        ///     that the textboxes wil be enabled. Setting to inactive means we will
        ///     disable them, so they appear grayed out.
        /// </summary>
        /// <param name="newStatus">
        ///     True if we cange them to active.
        ///     False if we should change them to inactive instead.
        /// </param>
        private void ChangeToActiveStatus(bool newStatus)
        {
            OldValueField.Enabled = newStatus;
            NewValueField.Enabled = newStatus;
        }

        // 
        // Notify listeners that our NewValueTextField has been 
        // changed. Our listeners will then either Accept or 
        // Reject the change.
        //

        /// <summary>
        ///     When our NewValueField loses focus we check to see whether any possible
        ///     changes that might have been made are valid or not. If they are not
        ///     we do not accept the changes and put the value back to the value this
        ///     field had before it gained focus. If the change however is a valid change
        ///     then the update is accepted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewValueFieldLostFocus(object sender, EventArgs e)
        {
            // A change of NewValue to an empty string is always allowed. These
            // changes do not have to be proposed. They are just the undoing of a 
            // rename and thus have no further consequences.
            //
            if (PreChangeBuffer != NewValue &&
                !string.IsNullOrWhiteSpace(NewValue))
            {
                var proposedChanges =
                    new ProposedBlockChangeEventArgs(NewValue);
                OnProposedBlockChange(proposedChanges);
            }
        }

        /// <summary>
        ///     When our NewValueField gains focus we save its current value in a temporary
        ///     change buffer. The change buffer is discarded on a loss of focus the new value
        ///     for the field is accepted. However, if the change is not accepted the older
        ///     value for the field will overwrite the new value, and the changes
        ///     will be undone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewValueFieldGotFocus(object sender, EventArgs e)
        {
            PreChangeBuffer = NewValue;
        }

        #region Submitting changes for acceptance

        /// <summary>
        ///     Tells listeners of the proposed changes for a block. This class should contain
        ///     all the information from an EditBlock required by the listeners to perform its
        ///     acceptance/rejection logic.
        /// </summary>
        public class ProposedBlockChangeEventArgs : EventArgs
        {
            /// <summary>
            ///     Create the ChangeEventArgs for a new proposed name for a block.
            /// </summary>
            /// <param name="proposedNewValue">
            ///     The proposed name for the block, if it is a
            ///     new block, or the proposed new name for the block after it would be
            ///     renamed.
            /// </param>
            public ProposedBlockChangeEventArgs(string proposedNewValue)
            {
                ProposedNewValue = proposedNewValue;
            }

            /// <summary>
            ///     The new value that is being proposed for a block. This may be the new name of
            ///     a block after renaming, or the name of an entirely new block.
            /// </summary>
            public string ProposedNewValue { get; private set; }
        }

        /// <summary>
        ///     Event that gets triggered if this reporting block has a proposed block change.
        ///     A proposed block change means that the new name for a block (NewValue) has been
        ///     changed. This change has to be verified to be valid before being accepted however.
        ///     This event notifies its listeners of the change, and allows them to react to
        ///     accept or reject the change.
        /// </summary>
        public event EventHandler<ProposedBlockChangeEventArgs> ProposedBlockChange;

        /// <summary>
        ///     Method gets called on a proposed block change and notifies all the listeners of
        ///     the proposed change.
        /// </summary>
        /// <param name="e">
        ///     Arguments that contain more detailed information about
        ///     the proposed change.
        /// </param>
        private void OnProposedBlockChange(ProposedBlockChangeEventArgs e)
        {
            if (ProposedBlockChange != null)
            {
                HasPendingChanges = true;
                ProposedBlockChange(this, e);
            }
        }

        /// <summary>
        ///     We accept the new value for the block. The change has been accepted and
        ///     it can be persisted.
        /// </summary>
        public void AcceptChange()
        {
            ClearErrorProviders();
            ResetErrors();
            ProvidedInputValid.SetError(NewValueField, "This is a valid change.");
            EmptyPreChangeBuffer();
            HasPendingChanges = false;
        }

        /// <summary>
        ///     The new value for the block has been rejected, the change will be undone. The
        ///     value of the NewValue that we had before this change was proposed will become
        ///     the new value for the block again.
        /// </summary>
        public void RejectChange()
        {
            ClearErrorProviders();
            EncounteredError();
            ProvidedInputInvalid.SetError(NewValueField,
                string.Format("A name change to \"{0}\" is not accepted. A block with this name might already exist.",
                    NewValue)
                );
            NewValue = PreChangeBuffer;
            EmptyPreChangeBuffer();
            HasPendingChanges = false;
        }

        /// <summary>
        ///     Empty our buffer of Pre-Changes.
        /// </summary>
        private void EmptyPreChangeBuffer()
        {
            PreChangeBuffer = null;
        }

        /// <summary>
        ///     Clear the error providers. This unsets any icons as well.
        /// </summary>
        private void ClearErrorProviders()
        {
            ProvidedInputValid.Clear();
            ProvidedInputInvalid.Clear();
        }

        /// <summary>
        ///     Gets called if a change has been rejected. This error remais until it is
        ///     fixed, or reset.
        /// </summary>
        private void EncounteredError()
        {
            HasUnresolvedErrors = true;
        }

        /// <summary>
        ///     Reset any errors that still remain on this form.
        /// </summary>
        public void ResetErrors()
        {
            HasUnresolvedErrors = false;
        }

        #endregion

        #endregion

        #region ToolTip Info

        // 
        // TODO: Add Tooltip info
        //

        #endregion
    }
}