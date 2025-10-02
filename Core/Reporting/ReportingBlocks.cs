using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InputLog.Core.Reporting
{
    /// <summary>
    /// This class is responsible for keeping track of all the different blocks 
    /// that have been defined for reporting. Blocks may be added and deleted, a
    /// list of blocks may be requested. All Blocks must have unique names.
    /// The default block is not allowed to be removed. This class will update any
    /// listeners of changes when a block has been added or has been removed.
    /// This class is a _singleton_
    /// </summary>
    public class ReportingBlocks
    {
        #region Fields
        #region Singleton Implementation
        /// <summary>
        /// The unique instance of this class.
        /// </summary>
        private static ReportingBlocks _instance;

        /// <summary>
        /// Key used for synchronizing multithreaded access to the TemplateInstance property
        /// </summary>
        private static readonly object SynchronizeKey = new object();

        /// <summary>
        /// Get the instance of the ReportingBlocks class.
        /// </summary>
        public static ReportingBlocks Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SynchronizeKey)
                    {
                        if (_instance == null)
                        {
                            _instance = new ReportingBlocks();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Internal representation of the different blocks that currently exist
        /// </summary>
        private readonly HashSet<string> Blocks;

        /// <summary>
        /// Returns the number of blocks that currently exist.
        /// </summary>
        public int Count
        {
            get
            {
                return Blocks.Count;
            }
        }

        /// <summary>
        /// The name of the default group.
        /// </summary>
        private readonly string DEFAULT = "< ungrouped >";

        /// <summary>
        /// The block that one can always fallback too. If one does not know
        /// which block to select, the fallback block will never be deleted 
        /// from the block list.
        /// </summary>
        public string Fallback
        {
            get
            {
                return DEFAULT;
            }
        }

        /// <summary>
        /// Access key to make sure multi-threaded access doesn't corrupt the 
        /// ReportingBlock
        /// </summary>
        private readonly object SingleAccessKey = new object();

        #endregion

        /// <summary>
        /// Create a new instance of ReportingBlocks. This will create an empty list
        /// of blocks and initialize it the list by adding the default block to it.
        /// </summary>
        private ReportingBlocks()
        {
            // Initialize members
            Blocks = new HashSet<string> {DEFAULT};
        }

        /// <summary>
        /// Add a new block to the available ReportingBlocks. The name of the 
        /// new block must be unique. If the name already existed in ReportingBlocks,
        /// it will not be added again.
        /// </summary>
        /// <param name="newBlockName">The name of the new block. This name must be
        /// unique, and no other block with the same name as `newBlockName` may already
        /// be available in the ReportingBlocks</param>
        /// <returns>True if the addition of the newBlock was succesful, false if
        /// the block could not be added. E.g: the name of the newBlock already existed
        /// within the ReportingBlocks</returns>
        public bool Add(string newBlockName)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(newBlockName));

            lock (SingleAccessKey)
            {
                if (Blocks.Contains(newBlockName))
                {
                    return false;
                }

                Blocks.Add(newBlockName);
                OnBlocksChanged(new BlocksChangedEventArgs(
                    BlocksChangedEventArgs.ChangeType.ADD,
                    null,
                    newBlockName));
            }
            return true;
        }

        /// <summary>
        /// Delete an existing block from the ReportingBlocks. If the given block name does 
        /// not exist in the ReportingBlocks this operation does nothing. If you are trying to delete
        /// the DEFAULT group - nothing will happen and false will be returned.
        /// </summary>
        /// <param name="blockName">Name of the block to be deleted from ReportingBlocks</param>
        /// <returns>True if the delete was succesful, false if the element could not be deleted. 
        /// E.g.: no block with given name existed.</returns>
        public bool Delete(string blockName)
        {
            lock (SingleAccessKey)
            {
                if (!Blocks.Contains(blockName))
                {
                    return false;
                }

                // The default block can not be deleted.
                if (DEFAULT == blockName)
                {
                    return false;
                }

                Blocks.Remove(blockName);
                OnBlocksChanged(
                    new BlocksChangedEventArgs(
                            BlocksChangedEventArgs.ChangeType.DELETE,
                            blockName,
                            null
                ));
            }
            return true;
        }

        /// <summary>
        /// Rename an existing block. Both parameters must be valid for this operation to be succesful.
        /// </summary>
        /// <param name="oldBlockName">The old name of the block. A block with this name must exist
        /// in order for this operation to be able to execute.</param>
        /// <param name="newBlockName">The new name of the block. This name must be unique and a block
        /// with this name may not already exist in ReportingBlocks.</param>
        /// <returns>Returns true if the operation was succcesful, False if it was not.</returns>
        public bool Rename(string oldBlockName, string newBlockName)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(oldBlockName));
            Debug.Assert(!String.IsNullOrWhiteSpace(newBlockName));

            lock (SingleAccessKey)
            {
                if (!Blocks.Contains(oldBlockName) ||
                    Blocks.Contains(newBlockName) ||
                    oldBlockName == newBlockName)
                {
                    return false;
                }

                Blocks.Remove(oldBlockName);
                Blocks.Add(newBlockName);
                OnBlocksChanged(
                    new BlocksChangedEventArgs(
                        BlocksChangedEventArgs.ChangeType.RENAME,
                        oldBlockName,
                        newBlockName
                ));
            }
            return true;
        }

        /// <summary>
        /// Gets all the blocks in the list.
        /// NOTE: _NEVER_ iterate over this class in response to a BlocksChangedEvent as doing so
        /// will cause a deadlock! The BlocksChangedEventArgs contains all the information
        /// needed to correctly handle the event.
        /// </summary>
        /// <returns>All the blocks that are currently defined in ReportingBlocks</returns>
        public IEnumerable<string> GetAllBlocks()
        {
            return Blocks;
        }

        #region Event Handling

        /// <summary>
        /// Class that contains the nature of the reporting block change
        /// that triggered the update event.
        /// The Event may have been triggered by an
        /// - Addition
        /// - Deletion
        /// - Renaming.
        /// </summary>
        public class BlocksChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The type of change. This determines how to interpret the
            /// values in this instance of BlocksChangedEventArgs
            /// </summary>
            public enum ChangeType
            {
                ADD,
                DELETE,
                RENAME
            }

            /// <summary>
            /// The type of change that caused this event to be fired. 
            /// The type of the change has an influence on how the `TargetBlock` and 
            /// `NewValue` properties have to be interpreted.
            /// </summary>
            public ChangeType Type;

            /// <summary>
            /// The block that caused the change. The values for the different event types 
            /// should be interpreted as follows:
            /// - Add: null (because no existing block has been targeted).
            /// - Delete: The name of the block that was deleted.
            /// - Rename: The old name of the block that was changed.
            /// </summary>
            public string TargetBlock
            {
                get;
                private set;
            }

            /// <summary>
            /// The new value for the block, after the change. The values for the different
            /// event types should be interpreted as follows:
            /// - Add: The name of the block that was added.
            /// - Delete: null (for the block has been deleted)
            /// - Rename: The new name of the block that was changed.
            /// </summary>
            public string NewValue
            {
                get;
                private set;
            }

            /// <summary>
            /// Construct the BlocksChangedEventArgs.
            /// Depending on the type of the change the values for targetBlock and newValue differ. 
            /// How the values are interpreted for each type is the following:
            /// ChangeType.Add: 
            /// - targetBlock is null - if anything else is passed here, it will be ignored.
            /// - newValue is the name of the new block.
            /// ChangeType.Delete:
            /// - targetBlock is the name of the deleted block.
            /// - newValue must be null: if anything else is passed here, it will be ignored.
            /// ChangeType.Rename:
            /// - targetBlock is the old name of the block.
            /// - newValue is the new name of the block.
            /// </summary>
            /// <param name="type">The type of the change.</param>
            /// <param name="targetBlock">The name of the block targetted by the change.</param>
            /// <param name="newValue">The new name of the block after the change.</param>
            public BlocksChangedEventArgs(ChangeType type, string targetBlock, string newValue)
            {
                InitializeDataMembers(type, targetBlock, newValue);
            }

            /// <summary>
            /// Initialize all data members according to the naming conventions stipulated in the
            /// constructors.
            /// </summary>
            /// <param name="type">The change type</param>
            /// <param name="targetBlock">The name of the target block</param>
            /// <param name="newValue">The name of the new value</param>
            private void InitializeDataMembers(ChangeType type, string targetBlock, string newValue = null)
            {
                Type = type;

                switch (type)
                {
                    case ChangeType.ADD:
                        TargetBlock = null;
                        NewValue = newValue;
                        break;
                    case ChangeType.DELETE:
                        TargetBlock = targetBlock;
                        NewValue = null;
                        break;
                    case ChangeType.RENAME:
                        TargetBlock = targetBlock;
                        NewValue = newValue;
                        break;
                    default:
                        throw new ArgumentException("Unhandled ChangeType in BlocksChangedEventArgs");
                }
            }
        }

        /// <summary>
        /// This event gets triggered whenever the ReportBlocks are changed. If 
        /// a block has been added, deleted or renamed all the listeners of this
        /// event will receive a notification and details about the change.
        /// </summary>
        public event EventHandler<BlocksChangedEventArgs> BlocksChanged;

        /// <summary>
        /// Notify all listeners that a block has been changed.
        /// </summary>
        /// <param name="e"></param>
        private void OnBlocksChanged(BlocksChangedEventArgs e)
        {
            if (BlocksChanged != null)
            {
                BlocksChanged(this, e);
            }
        }
        #endregion
    }
}
