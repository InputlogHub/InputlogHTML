namespace GUI.Tabs.Analyze.AnalysesControls.Copytask
{
    partial class CopytaskAnalysisControl
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
            this.cbRaw = new System.Windows.Forms.CheckBox();
            this.cbSynthesis = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(83, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Copy Task";
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
            // cbRaw
            // 
            this.cbRaw.AutoSize = true;
            this.cbRaw.Location = new System.Drawing.Point(8, 62);
            this.cbRaw.Name = "cbRaw";
            this.cbRaw.Size = new System.Drawing.Size(109, 17);
            this.cbRaw.TabIndex = 5;
            this.cbRaw.Text = "Raw data as .csv";
            this.cbRaw.UseVisualStyleBackColor = true;
            // 
            // cbSynthesis
            // 
            this.cbSynthesis.AutoSize = true;
            this.cbSynthesis.Checked = true;
            this.cbSynthesis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSynthesis.Location = new System.Drawing.Point(8, 39);
            this.cbSynthesis.Name = "cbSynthesis";
            this.cbSynthesis.Size = new System.Drawing.Size(71, 17);
            this.cbSynthesis.TabIndex = 6;
            this.cbSynthesis.Text = "Synthesis";
            this.cbSynthesis.UseVisualStyleBackColor = true;
            // 
            // CopytaskAnalysisControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.Controls.Add(this.cbSynthesis);
            this.Controls.Add(this.cbRaw);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "CopytaskAnalysisControl";
            this.Size = new System.Drawing.Size(325, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.CheckBox cbRaw;
        private System.Windows.Forms.CheckBox cbSynthesis;
    }
}
