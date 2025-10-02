using System;
using System.Linq;
using System.Text;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.IO;

namespace GUI.Tabs.Preprocess.Filters
{
    /// <summary>
    /// This class provides the controls for a Window Filter.
    /// InputFields:
    /// - windowTitle
    /// </summary>
    public partial class Window : ProcessControl
    {
        #region fields

        /// <summary>
        /// Characters that need to be escaped in the windowtitle because they have a special meaning in regular expressions.
        /// Note that the '*' is not escaped, as we use it as a wild card.
        /// </summary>
        private static readonly char[] EscapeChars =
            { '+', '?', '.', '(', ')', '{', '}', '"', '\'', '\\', '/' };

        /// <summary>
        /// Name of this Filter.
        /// </summary>
		public const string NAME = "Window Filter";

		/// <summary>
		/// Returns whether this process control can handle multiple files at the same
		/// time or can only process one time at a time.
		/// </summary>
		public override bool MultipleFileCompatible
		{
			get { return true; }
		}

        /// <summary>
        /// Configuration parameter names of this filter.
        /// </summary>
        private static class ConfigurationParameters
        {
            public const string WINDOW_TITLE = "WindowTitle";
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Window(): base("Window Filter", "window")
        {
            InitializeComponent();
            FilterAbbreviation = "window";
            InitHelp(MoreInfoLabel, FilterAbbreviation);
        }

        /// <summary>
        /// Returns the event filter that is represented by this filter control.
        /// </summary>
        /// <returns>The event filter.</returns>
        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            var windowTitle = WindowTitleList.Text;
            var strBuilder = new StringBuilder();

            // Escape regex chars manually, since * should be replaced differently.
            foreach (var ch in windowTitle.ToCharArray())
            {
                if (EscapeChars.Contains(ch))
                {
                    strBuilder.Append('\\');
                    strBuilder.Append(ch);
                }
                else if (ch.Equals('*'))
                {
                    strBuilder.Append("(.*)");
                }
                else
                {
                    strBuilder.Append(ch);
                }
            }

            windowTitle = strBuilder.ToString();
            return new WindowFilter(NAME, windowTitle);
        }

        /// <summary>
        /// Exports the configuration of this Window Control to a FilterConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A FilterConfiguration that defines the configuration (= the input fields) of this filter control. </returns>
        public override FilterConfiguration Export(FilterExportOptions options)
        {
            var config = new FilterConfiguration(NAME);
            config.Parameters.Add(ConfigurationParameters.WINDOW_TITLE, WindowTitleList.Text);
            return config;
        }

        /// <summary>
        /// Imports a configuration into this Window Control from an FilterConfiguration.
        /// </summary>
        public override void Import(FilterConfiguration configuration)
        {
            var defaultVal = WindowTitleList.Items.Count > 0 ? WindowTitleList.Items[0].ToString() : String.Empty;
            WindowTitleList.Text = configuration.TryGetParameter(ConfigurationParameters.WINDOW_TITLE, defaultVal);
        }
    }
}