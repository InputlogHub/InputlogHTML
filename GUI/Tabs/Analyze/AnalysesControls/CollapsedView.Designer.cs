namespace GUI.Tabs.Analyze.AnalysesControls {
    partial class CollapsedView {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.TitleLabel = new System.Windows.Forms.Label();
            this.DestinationLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.TitleLabel.Location = new System.Drawing.Point(3, 3);
            this.TitleLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(38, 20);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Title";
            // 
            // DestinationLink
            // 
            this.DestinationLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DestinationLink.AutoEllipsis = true;
            this.DestinationLink.AutoSize = true;
            this.DestinationLink.Location = new System.Drawing.Point(486, 7);
            this.DestinationLink.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.DestinationLink.Name = "DestinationLink";
            this.DestinationLink.Size = new System.Drawing.Size(60, 13);
            this.DestinationLink.TabIndex = 2;
            this.DestinationLink.TabStop = true;
            this.DestinationLink.Text = "Destination";
            // 
            // CollapsedView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.DestinationLink);
            this.Controls.Add(this.TitleLabel);
            this.DoubleBuffered = true;
            this.Name = "CollapsedView";
            this.Size = new System.Drawing.Size(273, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.LinkLabel DestinationLink;
    }
}
