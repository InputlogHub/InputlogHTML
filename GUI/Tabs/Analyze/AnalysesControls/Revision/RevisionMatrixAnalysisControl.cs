using System.Drawing;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;

namespace GUI.Tabs.Analyze.AnalysesControls.Revision
{
    public partial class RevisionMatrixAnalysisControl : AnalysisControl
    {
        
        #region Fields
        protected override string NAME { get { return RevisionMatrixAnalyzer.NAME; } }
        #endregion

        public RevisionMatrixAnalysisControl()
        {
            InitializeComponent();

            // Heatmap temporarily out of order
            IncludeHeatmap.Enabled = false;

            Init(NameLabel, MoreInfoLabel, RevisionMatrixAnalyzer.ABBR);
        }

        protected override void ProduceAnalyzer()
        {
            Analyzer = new RevisionMatrixAnalyzer(
                RunGetTS(delegate { return IncludeHeatmap.Checked; })
            );
        }

        /// <summary>
        /// Exports the configuration of this GeneralAnalysis Control to a AnalysisConfiguration.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>A dictionary containing key-value pairs that define the configuration 
        /// (= the input fields of this control) of this analysis control. </returns>
        public override AnalysisConfiguration Export(ExportOptions options)
        {
            var config = new AnalysisConfiguration(NAME);

            return config;
        }

        /// <summary>
        /// Imports a configuration into this GeneralAnalysis Control from an AnalysisConfiguration.
        /// </summary>
        public override void Import(AnalysisConfiguration configuration) { }

        /// <summary>
        /// Adapts the UI so to represent that the analysis has been finished.
        /// FluencyAnalysisControl adds a "Show Graph" link in the GUI
        /// If analysis was unsuccessful (probably no intervals found), UI is *not* changed
        /// </summary>
        protected override void ChangeUItoFinished()
        {
            base.ChangeUItoFinished();
            if (IncludeHeatmap.Checked)
            {
                LinkLabel hmLink = new LinkLabel();
                hmLink.Text = "Open heatmap";
                LinkLabel fileLink = (LinkLabel)DynamicControls[FILE_LINK_ID];
                int x = fileLink.Location.X + fileLink.Width;
                int y = fileLink.Location.Y;
                hmLink.Location = new Point(x, y);
                hmLink.Visible = true;
                hmLink.Tag = OutputFilePath;
                hmLink.Click += delegate
                {
                    OpenBrowser(((RevisionMatrixAnalyzer)Analyzer).HMPath);
                };
                DynamicControls["HeatmapLink"] = hmLink;
                Controls.Add(hmLink);
            }
        }

    }
}
