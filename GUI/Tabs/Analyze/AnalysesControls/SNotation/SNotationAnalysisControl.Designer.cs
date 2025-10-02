namespace GUI.Tabs.Analyze.AnalysesControls.SNotation
{
    partial class SNotationAnalysisControl
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
            this.moreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // moreInfoLabel
            // 
            this.moreInfoLabel.AutoSize = true;
            this.moreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.moreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.moreInfoLabel.Margin = new System.Windows.Forms.Padding(3);
            this.moreInfoLabel.Name = "moreInfoLabel";
            this.moreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.moreInfoLabel.TabIndex = 1;
            this.moreInfoLabel.TabStop = true;
            this.moreInfoLabel.Text = "More Info";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(105, 25);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "S-Notation";
            // 
            // SNotationAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.moreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "SNotationAnalysis";
            this.Size = new System.Drawing.Size(327, 32);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel moreInfoLabel;
    }
}
