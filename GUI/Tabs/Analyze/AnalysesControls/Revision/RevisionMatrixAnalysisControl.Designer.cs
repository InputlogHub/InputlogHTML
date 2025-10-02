namespace GUI.Tabs.Analyze.AnalysesControls.Revision
{
	partial class RevisionMatrixAnalysisControl
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
            this.NameLabel = new System.Windows.Forms.Label();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.IncludeHeatmap = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(69, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Revision";
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(51, 13);
            this.MoreInfoLabel.TabIndex = 4;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More info";
            // 
            // IncludeHeatmap
            // 
            this.IncludeHeatmap.AutoSize = true;
            this.IncludeHeatmap.Location = new System.Drawing.Point(17, 41);
            this.IncludeHeatmap.Name = "IncludeHeatmap";
            this.IncludeHeatmap.Size = new System.Drawing.Size(107, 17);
            this.IncludeHeatmap.TabIndex = 53;
            this.IncludeHeatmap.Text = "Include Heatmap";
            this.IncludeHeatmap.UseVisualStyleBackColor = true;
            this.IncludeHeatmap.Visible = false;
            // 
            // RevisionMatrixAnalysisControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.IncludeHeatmap);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "RevisionMatrixAnalysisControl";
            this.Size = new System.Drawing.Size(285, 61);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.CheckBox IncludeHeatmap;
	}
}
