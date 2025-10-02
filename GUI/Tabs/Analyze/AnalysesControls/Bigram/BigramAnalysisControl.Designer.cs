namespace GUI.Tabs.Analyze.AnalysesControls.Bigram
{
    partial class BigramAnalysisControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FileTypeList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PauseThresholdField = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.AvgStatistic = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).BeginInit();
            this.SuspendLayout();
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.MoreInfoLabel.TabIndex = 3;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More info";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(73, 25);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Bigram";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(324, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Save graph as ...";
            // 
            // FileTypeList
            // 
            this.FileTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FileTypeList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FileTypeList.FormattingEnabled = true;
            this.FileTypeList.Items.AddRange(new object[] {
            "PNG",
            "BMP",
            "EMF",
            "JPEG",
            "TIFF"});
            this.FileTypeList.Location = new System.Drawing.Point(319, 68);
            this.FileTypeList.Name = "FileTypeList";
            this.FileTypeList.Size = new System.Drawing.Size(121, 25);
            this.FileTypeList.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.Location = new System.Drawing.Point(10, 42);
            this.label4.Margin = new System.Windows.Forms.Padding(4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 17);
            this.label4.TabIndex = 28;
            this.label4.Text = "Pause Threshold (ms)";
            // 
            // PauseThresholdField
            // 
            this.PauseThresholdField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PauseThresholdField.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.PauseThresholdField.Location = new System.Drawing.Point(166, 40);
            this.PauseThresholdField.Margin = new System.Windows.Forms.Padding(4);
            this.PauseThresholdField.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.PauseThresholdField.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PauseThresholdField.Name = "PauseThresholdField";
            this.PauseThresholdField.ReadOnly = true;
            this.PauseThresholdField.Size = new System.Drawing.Size(95, 23);
            this.PauseThresholdField.TabIndex = 27;
            this.PauseThresholdField.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label5.Location = new System.Drawing.Point(10, 76);
            this.label5.Margin = new System.Windows.Forms.Padding(4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 17);
            this.label5.TabIndex = 37;
            this.label5.Text = "Average statistic";
            // 
            // AvgStatistic
            // 
            this.AvgStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AvgStatistic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AvgStatistic.FormattingEnabled = true;
            this.AvgStatistic.Location = new System.Drawing.Point(166, 73);
            this.AvgStatistic.Name = "AvgStatistic";
            this.AvgStatistic.Size = new System.Drawing.Size(95, 25);
            this.AvgStatistic.TabIndex = 36;
            // 
            // BigramAnalysisControl
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.AvgStatistic);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PauseThresholdField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileTypeList);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "BigramAnalysisControl";
            this.Size = new System.Drawing.Size(443, 101);
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox FileTypeList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown PauseThresholdField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox AvgStatistic;
    }
}
