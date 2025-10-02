namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    partial class ProcessPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Output = new System.Windows.Forms.TextBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.TopLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.AcceptsTab = true;
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.BackColor = System.Drawing.Color.White;
            this.Output.Location = new System.Drawing.Point(8, 29);
            this.Output.Margin = new System.Windows.Forms.Padding(8);
            this.Output.MaxLength = 1048575;
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Output.Size = new System.Drawing.Size(654, 342);
            this.Output.TabIndex = 0;
            this.Output.WordWrap = false;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(8, 387);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(8);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(654, 25);
            this.ProgressBar.TabIndex = 1;
            // 
            // TopLabel
            // 
            this.TopLabel.AutoSize = true;
            this.TopLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.TopLabel.Location = new System.Drawing.Point(12, 8);
            this.TopLabel.Name = "TopLabel";
            this.TopLabel.Size = new System.Drawing.Size(65, 17);
            this.TopLabel.TabIndex = 2;
            this.TopLabel.Text = "Progress";
            // 
            // ProcessPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.TopLabel);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.Output);
            this.DoubleBuffered = true;
            this.Name = "ProcessPage";
            this.Size = new System.Drawing.Size(670, 420);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		public System.Windows.Forms.TextBox Output;
		public System.Windows.Forms.ProgressBar ProgressBar;
		private System.Windows.Forms.Label TopLabel;

	}
}
