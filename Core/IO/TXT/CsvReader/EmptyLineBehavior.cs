namespace InputLog.Core.IO.Txt.CsvReader
{
    /// <summary>
    ///     Determines how empty lines are interpreted when reading CSV files.
    ///     These values do not affect empty lines that occur within quoted fields
    ///     or empty lines that appear at the end of the input file.
    /// </summary>
    public enum EmptyLineBehavior
    {
        /// <summary>
        ///     Empty lines are interpreted as a line with zero columns.
        /// </summary>
        NO_COLUMNS,

        /// <summary>
        ///     Empty lines are interpreted as a line with a single empty column.
        /// </summary>
        EMPTY_COLUMN,

        /// <summary>
        ///     Empty lines are skipped over as though they did not exist.
        /// </summary>
        IGNORE,

        /// <summary>
        ///     An empty line is interpreted as the end of the input file.
        /// </summary>
        END_OF_FILE,
    }
}