namespace InputLog.Core.Merging.Analyses.Processors
{
    /// <summary>
    /// Factory creates the different analysis processors based on the
    /// merging directions
    /// </summary>
    internal class ProcessorFactory
    {
        /// <summary>
        /// Create an analysis file processor based on the merging
        /// direction requested by the user. 
        /// </summary>
        /// <param name="direction">Direction of the merging</param>
        /// <param name="workDir">Working directory the processor uses for temporary files
        /// and to store the result files.</param>
        /// <returns>A processor that can be used for processing the
        /// analysis files.</returns>
        public static BasicProcessor Create(AnalysisMerge.Direction direction, string workDir)
        {
            switch (direction)
            {
                case AnalysisMerge.Direction.HORIZONTAL:
                    return new HorizontalCSVProcessor(workDir);

                case AnalysisMerge.Direction.VERTICAL:
                    return new VerticalCSVProcessor(workDir);

                default:
                    return null;
            }
        }
    }
}