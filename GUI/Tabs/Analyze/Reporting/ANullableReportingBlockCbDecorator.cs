using System.Windows.Forms;

namespace GUI.Tabs.Analyze.Reporting
{
    /// <summary>
    /// This class is the same as it's superclass the ReportingBlockCBDecorator in 
    /// that it's a wrapper around a ComboBox that handles all the updating of blocks.
    /// The only difference with this class and its superclass is that this class also
    /// introduces a standard 'null' selection. For when there's no way to make a uniform block
    /// selection.
    /// This will be used by the AnalysisReportingTargets. For instance, when one target of the 
    /// analysis is assigned block A and another target is assigned block B, the block of the 
    /// entire analysis becomes 'null'. 
    /// If all reporting targets for the analysis belong to block B then the analysis
    /// itself will be marked as block B too.
    /// 
    /// NOTE: THIS WILL NOT WORK IF THE BASE CLASS ALSO ALLOWS AN EMPTY STRING
    /// TO BE ADDED AS A VALID OPTION. This class operates under the assumption that 
    /// the base class will never have any empty strings as options.
    /// </summary>
    internal class NullableReportingBlockCbDecorator: ReportingBlockCBDecorator
    {
        #region Fields

        /// <summary>
        /// The selection option for when this combobox is set to a null state.
        /// </summary>
        private const string NULL_SELECT = "";

        #endregion

        /// <summary>
        /// Create a new Nullable decorator for a ReportingBlock combobox. You pass
        /// the combobox to be decorated as a source to this class.
        /// </summary>
        /// <param name="source">The ComboBox to be decorated.</param>
        /// <param name="defaultBlock">The default value for this ComboBox. If the value
        /// does not exist yet, it will be added. Then the value will be selected in the
        /// ComboBox. If no default value is passed, the default value is set to the 
        /// ReportingBlock Fallback block</param>
        public NullableReportingBlockCbDecorator(ComboBox source, string defaultBlock = null):
            base(source, defaultBlock)
        {
            Block.Items.Add(NULL_SELECT);
            RestoreDefaultSelection();
        }

        /// <summary>
        /// Set the selected item for this combobox to the null item.
        /// </summary>
        public void SetToNull()
        {
            SelectedBlock = NULL_SELECT;
        }

        /// <summary>
        /// Check whether the currently selected block is the Null block. 
        /// </summary>
        /// <returns>True if the currently selected block is the null 
        /// block, false if it is a different block.</returns>
        public bool IsSetToNull() 
        {
            return SelectedBlock == NULL_SELECT;   
        }

    }
}
