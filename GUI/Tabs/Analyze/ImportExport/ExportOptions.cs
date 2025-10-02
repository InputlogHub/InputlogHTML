namespace GUI.Tabs.Analyze.ImportExport
{

    /// <summary>
    /// Class that can specify options for the exportation of analysis configurations. This can be used to pass to a 
    /// specific analysis control to let it know if certain fields should be exported or not.
    /// </summary>
    public class ExportOptions
    {

        #region Fields

        /// <summary>
        /// Indicates whether the target destination location should be exported or not (defaults to false);
        /// </summary>
        public bool IncludeDestination { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExportOptions()
        {
            IncludeDestination = false;
        }

    }
}
