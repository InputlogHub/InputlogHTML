using System;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    /// A decorator around a normal ComboBox that manages its ComboBox contents.
    /// It takes care of the automatic updating of the ComboBox when the ReportingBlock
    /// contents have been updated. Elements may be added to ReportingBlock, deleted, or 
    /// renamed. 
    /// This decorator makes sure that the ComboBox contents stay up to date and correctly
    /// reflect all the changes. All updating of the ComboBox will be handled by the decorator
    /// and should _only_ be handled by the decorator in order to keep consistency among 
    /// all the other ReportingBlock ComboBox's. 
    /// </summary>
    internal class ReportingBlockCBDecorator
    {
        /// <summary>
        /// The combo box decorated by this class. Its selected item
        /// represents the ReportingBlock it belongs too.
        /// </summary>
        protected ComboBox Block;

        /// <summary>
        /// The block that is currently selected in the ComboBox.
        /// </summary>
        public string SelectedBlock
        {
            // Return the currently selected block
            get
            {
                return (string)this.Block.SelectedItem;
            }
            // Set the currently selected item if it exists
            set
            {
                if (this.Block.Items.Contains(value))
                {
                    this.Block.SelectedItem = value;
                }
            }
        }

        /// <summary>
        /// The default block of this ComboBox.
        /// </summary>
        private string DefaultBlock;

        /// <summary>
        /// Create a new Decorator for the ReportingBlock combobox. You pass the ComboBox
        /// to be decorated as source to this class.
        /// </summary>
        /// <param name="source">The ComboBox to be decorated.</param>
        /// <param name="defaultBlock">The default value for this ComboBox. If the value
        /// does not exist yet, it will be added. Then the value will be selected in the
        /// ComboBox. If no default value is passed or the value is empty,
        /// the default value is set to the ReportingBlock Fallback block.</param>
        public ReportingBlockCBDecorator(ComboBox source, string defaultBlock = null)
        {
            this.Block = source;

            // Initialize the ComboBox with the initial list of ReportingBlocks
            this.Block.Items.AddRange(ReportingBlocks.Instance.GetAllBlocks().ToArray<string>());

            // Register this class for changes on ReportingBlocks
            ReportingBlocks.Instance.BlocksChanged += BlocksChanged;

            // Add the defaultBlock to the ReportingBlocks and set it as default selected
            // block.
            this.DefaultBlock = defaultBlock;
            this.RestoreDefaultSelection();
        }

        /// <summary>
        /// Restore the selection of this combobox to its original default block, or 
        /// to the fallback block, if it never had a default specified.
        /// </summary>
        protected void RestoreDefaultSelection()
        {
            if (!String.IsNullOrWhiteSpace(this.DefaultBlock))
            {
                ReportingBlocks.Instance.Add(this.DefaultBlock);
                this.SelectedBlock = this.DefaultBlock;
            }
            else
            {
                this.SelectedBlock = this.GetFallbackBlock();
            }
        }

        /// <summary>
        /// Returns the block that is default in the ReportingBlocks. It is the one
        /// block that can never be removed. 
        /// </summary>
        /// <returns>The fallback block for ReportingBlocks</returns>
        private string GetFallbackBlock()
        {
            return ReportingBlocks.Instance.Fallback;
        }

        /// <summary>
        /// Update our BlockLists whenever we receive an update of the ReportingBlocks. Blocks
        /// may have been added, deleted or renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlocksChanged(object sender, ReportingBlocks.BlocksChangedEventArgs e)
        {
            // There's three updates possible: 
            // 1. Addition
            // 2. Deletion 
            // 3. Renaming
            switch (e.Type)
            {
                case ReportingBlocks.BlocksChangedEventArgs.ChangeType.ADD:
                    this.HandleBlockAddition(e);
                    break;

                case ReportingBlocks.BlocksChangedEventArgs.ChangeType.DELETE:
                    this.HandleBlockDeletion(e);
                    break;

                case ReportingBlocks.BlocksChangedEventArgs.ChangeType.RENAME:
                    this.HandleBlockRenaming(e);
                    break;
                default:
                    throw new ArgumentException("Unhandled ChangeType detected when updating Blocks");
            }
        }

        /// <summary>
        /// A block has been renamed. If our currently selected block was the block
        /// that was renamed, we must maintain our selection of the new, renamed block.
        /// </summary>
        /// <param name="e">The change event arguments containing the change details</param>
        private void HandleBlockRenaming(ReportingBlocks.BlocksChangedEventArgs e)
        {
            string oldBlockName = e.TargetBlock;
            string newBlockName = e.NewValue;
            string currentBlockName = this.SelectedBlock;

            this.Block.Items.Remove(oldBlockName);
            this.Block.Items.Add(newBlockName);

            if (currentBlockName == oldBlockName)
            {
                this.Block.SelectedItem = newBlockName;
            }
        }

        /// <summary>
        /// A block has been deleted. If our currently selected block was the block
        /// that was deleted we set our currently selected block to the Undefined 
        /// block
        /// </summary>
        /// <param name="e">The change event arguments containing the change details</param>
        private void HandleBlockDeletion(ReportingBlocks.BlocksChangedEventArgs e)
        {
            string deletedBlockName = e.TargetBlock;
            string currentBlockName = this.SelectedBlock;

            this.Block.Items.Remove(deletedBlockName);

            if (currentBlockName == deletedBlockName)
            {
                this.Block.SelectedItem = this.GetFallbackBlock();
            }
        }

        /// <summary>
        /// A block has been added. We only have to add this block to our current list of 
        /// Blocks
        /// </summary>
        /// <param name="e">The change event arguments containing the change details</param>
        private void HandleBlockAddition(ReportingBlocks.BlocksChangedEventArgs e)
        {
            string newBlockName = e.NewValue;
            this.Block.Items.Add(newBlockName);
        }
    }
}
