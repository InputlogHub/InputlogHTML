namespace GUI.Tabs.Analyze.AnalysesControls {
    partial class AnalysisWrapper {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisWrapper));
            this.pictClose = new System.Windows.Forms.PictureBox();
            this.PictCollapse = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictCollapse)).BeginInit();
            this.SuspendLayout();
            // 
            // pictClose
            // 
            this.pictClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictClose.Image = global::GUI.Properties.Resources.cross_grey;
            this.pictClose.InitialImage = null;
            this.pictClose.Location = new System.Drawing.Point(818, 3);
            this.pictClose.Name = "pictClose";
            this.pictClose.Size = new System.Drawing.Size(17, 17);
            this.pictClose.TabIndex = 0;
            this.pictClose.TabStop = false;
            this.pictClose.Click += new System.EventHandler(this.PictCloseClick);
            this.pictClose.MouseEnter += new System.EventHandler(this.PictCloseMouseEnter);
            this.pictClose.MouseLeave += new System.EventHandler(this.PictCloseMouseLeave);
            // 
            // PictCollapse
            // 
            this.PictCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PictCollapse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PictCollapse.Image = ((System.Drawing.Image)(resources.GetObject("PictCollapse.Image")));
            this.PictCollapse.Location = new System.Drawing.Point(788, 3);
            this.PictCollapse.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.PictCollapse.Name = "PictCollapse";
            this.PictCollapse.Size = new System.Drawing.Size(17, 17);
            this.PictCollapse.TabIndex = 2;
            this.PictCollapse.TabStop = false;
            this.PictCollapse.Click += new System.EventHandler(this.PictCollapseClick);
            // 
            // AnalysisWrapper
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.PictCollapse);
            this.Controls.Add(this.pictClose);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.Name = "AnalysisWrapper";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.Size = new System.Drawing.Size(838, 24);
            ((System.ComponentModel.ISupportInitialize)(this.pictClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictCollapse)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictClose;
        private System.Windows.Forms.PictureBox PictCollapse;
    }
}
