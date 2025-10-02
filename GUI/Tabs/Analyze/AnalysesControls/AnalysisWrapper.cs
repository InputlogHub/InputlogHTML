using System;
using System.Drawing;
using System.Windows.Forms;
using GUI.Properties;

namespace GUI.Tabs.Analyze.AnalysesControls
{
    /// <summary>
    /// A wrapper around AnalysisControl that adds a close button.
    /// </summary>
    public partial class AnalysisWrapper : UserControl
    {
        #region Fields

        /// <summary>
        /// Called whenever the AnalysisWrapper is closed.
        /// </summary>
        private readonly EventHandler CloseHandle;

        /// <summary>
        /// The parent Control.
        /// </summary>
        private readonly Control ParentControl;

        /// <summary>
        /// Indicates whether the control is currently collapsed or not.
        /// </summary>
        private bool Collapsed;

        /// <summary>
        /// The contained Control.
        /// </summary>
        public AnalysisControl Control { get; private set; }

        /// <summary>
        /// The collapsed view of the contained control.
        /// </summary>
        private CollapsedView CollapsedView { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentControl">The Control containing the wrapper.</param>
        /// <param name="analysis">The AnalysisControl to be contained by the wrapper.</param>
        /// <param name="closeHandle">The handler to call when the wrapper is closed.</param>
        public AnalysisWrapper(Control parentControl, AnalysisControl analysis, EventHandler closeHandle)
        {
            InitializeComponent();
            ParentControl = parentControl;

            Control = analysis;
            Control.Dock = DockStyle.Bottom;

            CollapsedView = analysis.GetCollapsedView();
            CollapsedView.Dock = DockStyle.Bottom;
            CollapsedView.Hide();

            Controls.Add(Control);
            Controls.Add(CollapsedView);

            if (closeHandle != null)
            {
                CloseHandle += closeHandle;
            }
            else
            {
                DisableClosing();
            }

            ParentControl.Layout += OnLayout;
            Paint += Repaint;
            AdjustSize();
        }

        /// <summary>
        /// Disallows the user to close this wrapper. It hides the
        /// close picture and prevents the CloseHandle from being called.
        /// </summary>
        private void DisableClosing()
        {
            pictClose.Visible = false;
        }

        /// <summary>
        /// Callback when the layout of the parent control is changed.
        /// We will need to adjust our width to the new layout.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLayout(object sender, LayoutEventArgs e)
        {
            AdjustSize();
        }

        /// <summary>
        /// Callback for the paint event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Repaint(object sender, PaintEventArgs e)
        {
            // Draw grey line under AnalysisControl
            var pen = new Pen(Color.Gray);
            try
            {
                int y = Height - 1;
                e.Graphics.DrawLine(pen, 0, y, Width, y);
            }
            finally
            {
                pen.Dispose();
            }
        }

        /// <summary>
        /// Adjusts the size of the wrapper to the size of the parent control.
        /// </summary>
        private void AdjustSize()
        {
            // Make this container as large as the parent container
            Width = ParentControl.ClientSize.Width;
            MaximumSize = new Size(ParentControl.ClientSize.Width, 0);
        }

        /// <summary>
        /// Click handler for the close picture.
        /// Calls the close handle.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PictCloseClick(object sender, EventArgs e)
        {
            if (CloseHandle != null)
            {
                CloseHandle(this, new EventArgs());
            }
        }

        /// <summary>
        /// Mouse leave handler for the close picture.
        /// (changes the picture).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PictCloseMouseLeave(object sender, EventArgs e)
        {
            pictClose.Image = Resources.cross_grey;
        }

        /// <summary>
        /// Mouse enter handler for the close picture.
        /// (changes the picture).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PictCloseMouseEnter(object sender, EventArgs e)
        {
            pictClose.Image = Resources.cross;
        }

        /// <summary>
        /// Click handler for the collapse picture.
        /// Collapses of Expands the AnalysisControl, depending whether the control is currently expanded or collapsed.
        /// Also adjusts the collapse icon to represent the action if it is clicked.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PictCollapseClick(object sender, EventArgs e)
        {
            if (Collapsed)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }

        /// <summary>
        /// Collapses the Analysis Control.
        /// </summary>
        private void Collapse()
        {
            Control.SuspendLayout();
            PictCollapse.Image = Resources.bullet_toggle_plus;
            Collapsed = true;
            Control.Hide();
            CollapsedView.Show();
            Control.ResumeLayout();
        }

        /// <summary>
        /// Expands the Analysis Control.
        /// </summary>
        private void Expand()
        {
            Control.SuspendLayout();
            PictCollapse.Image = Resources.bullet_toggle_minus;
            Collapsed = false;
            CollapsedView.Hide();
            Control.Show();
            Control.ResumeLayout();
        }
    }
}