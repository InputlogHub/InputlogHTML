namespace GUI.Tabs.Analyze.ImportExport {
    partial class ExportAnalysisConfiguration {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.ExportButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.DestinationFolderTB = new System.Windows.Forms.TextBox();
			this.DestinationFolderLbl = new System.Windows.Forms.Label();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.SaveFiltersCheckbox = new System.Windows.Forms.CheckBox();
			this.SaveSrcCheckbox = new System.Windows.Forms.CheckBox();
			this.StatusLabel = new System.Windows.Forms.Label();
			this.SaveDstCheckbox = new System.Windows.Forms.CheckBox();
			this.DestinationFileTB = new System.Windows.Forms.TextBox();
			this.DestinationFileLbl = new System.Windows.Forms.Label();
			this.destinationFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SaveAnalysisCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// ExportButton
			// 
			this.ExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ExportButton.Location = new System.Drawing.Point(474, 250);
			this.ExportButton.Margin = new System.Windows.Forms.Padding(4);
			this.ExportButton.Name = "ExportButton";
			this.ExportButton.Size = new System.Drawing.Size(100, 30);
			this.ExportButton.TabIndex = 0;
			this.ExportButton.Text = "Export";
			this.ExportButton.UseVisualStyleBackColor = true;
			this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(275, 250);
			this.CancelButton.Margin = new System.Windows.Forms.Padding(4);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(181, 30);
			this.CancelButton.TabIndex = 1;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// DestinationFolderTB
			// 
			this.DestinationFolderTB.Location = new System.Drawing.Point(161, 55);
			this.DestinationFolderTB.Margin = new System.Windows.Forms.Padding(4);
			this.DestinationFolderTB.Name = "DestinationFolderTB";
			this.DestinationFolderTB.Size = new System.Drawing.Size(320, 20);
			this.DestinationFolderTB.TabIndex = 2;
			// 
			// DestinationFolderLbl
			// 
			this.DestinationFolderLbl.AutoSize = true;
			this.DestinationFolderLbl.Location = new System.Drawing.Point(15, 58);
			this.DestinationFolderLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.DestinationFolderLbl.Name = "DestinationFolderLbl";
			this.DestinationFolderLbl.Size = new System.Drawing.Size(105, 13);
			this.DestinationFolderLbl.TabIndex = 3;
			this.DestinationFolderLbl.Text = "Destination Directory";
			// 
			// BrowseButton
			// 
			this.BrowseButton.BackgroundImage = global::GUI.Properties.Resources.folder_explore;
			this.BrowseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.BrowseButton.Location = new System.Drawing.Point(498, 52);
			this.BrowseButton.Margin = new System.Windows.Forms.Padding(4);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(70, 28);
			this.BrowseButton.TabIndex = 4;
			this.BrowseButton.UseVisualStyleBackColor = true;
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
			// 
			// SaveFiltersCheckbox
			// 
			this.SaveFiltersCheckbox.AutoSize = true;
			this.SaveFiltersCheckbox.Location = new System.Drawing.Point(18, 148);
			this.SaveFiltersCheckbox.Margin = new System.Windows.Forms.Padding(4);
			this.SaveFiltersCheckbox.Name = "SaveFiltersCheckbox";
			this.SaveFiltersCheckbox.Size = new System.Drawing.Size(118, 17);
			this.SaveFiltersCheckbox.TabIndex = 5;
			this.SaveFiltersCheckbox.Text = "Include event filters";
			this.SaveFiltersCheckbox.UseVisualStyleBackColor = true;
			// 
			// SaveSrcCheckbox
			// 
			this.SaveSrcCheckbox.AutoSize = true;
			this.SaveSrcCheckbox.Location = new System.Drawing.Point(18, 177);
			this.SaveSrcCheckbox.Margin = new System.Windows.Forms.Padding(4);
			this.SaveSrcCheckbox.Name = "SaveSrcCheckbox";
			this.SaveSrcCheckbox.Size = new System.Drawing.Size(141, 17);
			this.SaveSrcCheckbox.TabIndex = 6;
			this.SaveSrcCheckbox.Text = "Include source file paths";
			this.SaveSrcCheckbox.UseVisualStyleBackColor = true;
			// 
			// StatusLabel
			// 
			this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.StatusLabel.AutoSize = true;
			this.StatusLabel.Location = new System.Drawing.Point(45, 257);
			this.StatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(61, 13);
			this.StatusLabel.TabIndex = 7;
			this.StatusLabel.Text = "statusLabel";
			// 
			// SaveDstCheckbox
			// 
			this.SaveDstCheckbox.AutoSize = true;
			this.SaveDstCheckbox.Checked = true;
			this.SaveDstCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SaveDstCheckbox.Location = new System.Drawing.Point(18, 206);
			this.SaveDstCheckbox.Margin = new System.Windows.Forms.Padding(4);
			this.SaveDstCheckbox.Name = "SaveDstCheckbox";
			this.SaveDstCheckbox.Size = new System.Drawing.Size(184, 17);
			this.SaveDstCheckbox.TabIndex = 8;
			this.SaveDstCheckbox.Text = "Include analysis destination paths";
			this.SaveDstCheckbox.UseVisualStyleBackColor = true;
			// 
			// DestinationFileTB
			// 
			this.DestinationFileTB.Location = new System.Drawing.Point(161, 24);
			this.DestinationFileTB.Margin = new System.Windows.Forms.Padding(4);
			this.DestinationFileTB.Name = "DestinationFileTB";
			this.DestinationFileTB.Size = new System.Drawing.Size(320, 20);
			this.DestinationFileTB.TabIndex = 9;
			// 
			// DestinationFileLbl
			// 
			this.DestinationFileLbl.AutoSize = true;
			this.DestinationFileLbl.Location = new System.Drawing.Point(15, 27);
			this.DestinationFileLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.DestinationFileLbl.Name = "DestinationFileLbl";
			this.DestinationFileLbl.Size = new System.Drawing.Size(105, 13);
			this.DestinationFileLbl.TabIndex = 10;
			this.DestinationFileLbl.Text = "Destination Filename";
			// 
			// SaveAnalysisCheckBox
			// 
			this.SaveAnalysisCheckBox.AutoSize = true;
			this.SaveAnalysisCheckBox.Checked = true;
			this.SaveAnalysisCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SaveAnalysisCheckBox.Location = new System.Drawing.Point(18, 120);
			this.SaveAnalysisCheckBox.Name = "SaveAnalysisCheckBox";
			this.SaveAnalysisCheckBox.Size = new System.Drawing.Size(140, 17);
			this.SaveAnalysisCheckBox.TabIndex = 11;
			this.SaveAnalysisCheckBox.Text = "Include analysis settings";
			this.SaveAnalysisCheckBox.UseVisualStyleBackColor = true;
			// 
			// ExportAnalysisConfiguration
			// 
			this.AcceptButton = this.ExportButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(626, 313);
			this.Controls.Add(this.SaveAnalysisCheckBox);
			this.Controls.Add(this.DestinationFileLbl);
			this.Controls.Add(this.DestinationFileTB);
			this.Controls.Add(this.SaveDstCheckbox);
			this.Controls.Add(this.StatusLabel);
			this.Controls.Add(this.SaveSrcCheckbox);
			this.Controls.Add(this.SaveFiltersCheckbox);
			this.Controls.Add(this.BrowseButton);
			this.Controls.Add(this.DestinationFolderLbl);
			this.Controls.Add(this.DestinationFolderTB);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.ExportButton);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExportAnalysisConfiguration";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export Analysis Configuration";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExportButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.TextBox DestinationFolderTB;
        private System.Windows.Forms.Label DestinationFolderLbl;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.CheckBox SaveFiltersCheckbox;
        private System.Windows.Forms.CheckBox SaveSrcCheckbox;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.CheckBox SaveDstCheckbox;
        private System.Windows.Forms.TextBox DestinationFileTB;
        private System.Windows.Forms.Label DestinationFileLbl;
        private System.Windows.Forms.FolderBrowserDialog destinationFolderDialog;
        private System.Windows.Forms.CheckBox SaveAnalysisCheckBox;
    }
}