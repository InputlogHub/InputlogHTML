namespace GUI.Tabs.Analyze.AnalysesControls.Token
{
    partial class TokenAnalysisControl
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
            this.moreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.setCSVFileLbl = new System.Windows.Forms.Label();
            this.CSVField = new System.Windows.Forms.TextBox();
            this.csvSelectionBtn = new System.Windows.Forms.Button();
            this.SelectCSVDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(118, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Token Analyzer";
            // 
            // moreInfoLabel
            // 
            this.moreInfoLabel.AutoSize = true;
            this.moreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.moreInfoLabel.Location = new System.Drawing.Point(172, 8);
            this.moreInfoLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.moreInfoLabel.Name = "moreInfoLabel";
            this.moreInfoLabel.Size = new System.Drawing.Size(52, 13);
            this.moreInfoLabel.TabIndex = 1;
            this.moreInfoLabel.TabStop = true;
            this.moreInfoLabel.Text = "More Info";
            // 
            // setCSVFileLbl
            // 
            this.setCSVFileLbl.AutoSize = true;
            this.setCSVFileLbl.Location = new System.Drawing.Point(5, 43);
            this.setCSVFileLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.setCSVFileLbl.Name = "setCSVFileLbl";
            this.setCSVFileLbl.Size = new System.Drawing.Size(86, 13);
            this.setCSVFileLbl.TabIndex = 2;
            this.setCSVFileLbl.Text = "Select a CSV-file";
            // 
            // CSVField
            // 
            this.CSVField.Location = new System.Drawing.Point(104, 41);
            this.CSVField.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CSVField.Name = "CSVField";
            this.CSVField.Size = new System.Drawing.Size(151, 20);
            this.CSVField.TabIndex = 3;
            // 
            // csvSelectionBtn
            // 
            this.csvSelectionBtn.Image = global::GUI.Properties.Resources.folder_explore;
            this.csvSelectionBtn.Location = new System.Drawing.Point(265, 41);
            this.csvSelectionBtn.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.csvSelectionBtn.Name = "csvSelectionBtn";
            this.csvSelectionBtn.Size = new System.Drawing.Size(52, 21);
            this.csvSelectionBtn.TabIndex = 4;
            this.csvSelectionBtn.UseVisualStyleBackColor = true;
            this.csvSelectionBtn.Click += new System.EventHandler(this.CSVSelectionBtnClick);
            // 
            // SelectCSVDialog
            // 
            this.SelectCSVDialog.Filter = "CSV files|*.csv";
            // 
            // TokenAnalysisControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.csvSelectionBtn);
            this.Controls.Add(this.CSVField);
            this.Controls.Add(this.setCSVFileLbl);
            this.Controls.Add(this.moreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "TokenAnalysisControl";
            this.Size = new System.Drawing.Size(325, 70);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel moreInfoLabel;
        private System.Windows.Forms.Label setCSVFileLbl;
        private System.Windows.Forms.TextBox CSVField;
        private System.Windows.Forms.Button csvSelectionBtn;
        private System.Windows.Forms.OpenFileDialog SelectCSVDialog;
    }
}
