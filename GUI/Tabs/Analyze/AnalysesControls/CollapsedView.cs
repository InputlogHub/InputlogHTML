using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.Tabs.Analyze.AnalysesControls
{

    /// <summary>
    /// Represents the collapsed view of a AnalysisControl.
    /// </summary>
    public partial class CollapsedView : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">Reference to the title label.</param>
        /// <param name="linklocation">Reference to the label pointing to the output file.</param>
        public CollapsedView(Label title, Point linklocation)
        {
            InitializeComponent();

            TitleLabel.Text = title.Text;
            TitleLabel.Location = title.Location;
            TitleLabel.Size = title.Size;
            TitleLabel.Font = title.Font;

            DestinationLink.Location = linklocation;
            DestinationLink.Visible = false;
        }

        /// <summary>
        /// Should be called whenever the analysis is finished and the the label pointing
        /// to the destination file should be upgraded to a link.
        /// </summary>
        /// <param name="analysisfilepath">The path where the resulting file was written.</param>
        public void AnalysisFinished(string analysisfilepath)
        {
            Parent.SuspendLayout();

            DestinationLink.Text = analysisfilepath;
            DestinationLink.LinkClicked += LinkClicked;
            DestinationLink.Visible = false;

            Parent.ResumeLayout();
        }

        /// <summary>
        /// Callback whenever the output link is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(DestinationLink.Text);
        }
    }
}