using System.Collections.Generic;

namespace GUI.Tabs.Analyze.ImportExport
{
    /// <summary>
    /// Class representing a configuration of an analysis (= configuration of its parameters).
    /// </summary>
    public class AnalysisConfiguration
    {
        #region

        /// <summary>
        /// Name of the analysis for which this is a configuration.
        /// </summary>
        public string AnalysisName { get; set; }

        /// <summary>
        /// Actual configuration parameters represented as key-value pairs.
        /// </summary>
        public IDictionary<string, string> Parameters { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysisName">Name of the analysis for which this is a configuration.</param>
        public AnalysisConfiguration(string analysisName)
        {
            AnalysisName = analysisName;
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Tries to get the value for a given parameter name. If there is no parameter with the given name, 
        /// a default value is returned.
        /// </summary>
        /// <param name="name">Name of the parameter for which to return its value.</param>
        /// <param name="defaultValue">Default value to return in case there is no parameter with the given name.</param>
        /// <returns>The value for the parameter with the given name. If there is no parameter with the given name, 
        /// the default value is returned.</returns>
        public string TryGetParameter(string name, string defaultValue)
        {
            string value;
            Parameters.TryGetValue(name, out value);
            return value;
        }
    }
}