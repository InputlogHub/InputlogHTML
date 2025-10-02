namespace GUI.Settings.Tabs {
    partial class General {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.WorkspaceTextField = new System.Windows.Forms.TextBox();
            this.WorkspaceBrowseButton = new System.Windows.Forms.Button();
            this.WorkspaceLabel = new System.Windows.Forms.Label();
            this.WorkspaceBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.RecordHookSelectionLabel = new System.Windows.Forms.Label();
            this.RecordHookSelectionBox = new System.Windows.Forms.CheckedListBox();
            this.RecordLoggingFormatLabel = new System.Windows.Forms.Label();
            this.RecordLoggingFormatList = new System.Windows.Forms.ComboBox();
            this.RecordPluginSelectionLabel = new System.Windows.Forms.Label();
            this.RecordPluginSelectionBox = new System.Windows.Forms.CheckedListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.WorkspaceTextField);
            this.panel1.Controls.Add(this.WorkspaceBrowseButton);
            this.panel1.Controls.Add(this.WorkspaceLabel);
            this.panel1.Location = new System.Drawing.Point(3, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(415, 50);
            this.panel1.TabIndex = 16;
            // 
            // WorkspaceTextField
            // 
            this.WorkspaceTextField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WorkspaceTextField.Location = new System.Drawing.Point(3, 16);
            this.WorkspaceTextField.Name = "WorkspaceTextField";
            this.WorkspaceTextField.Size = new System.Drawing.Size(311, 20);
            this.WorkspaceTextField.TabIndex = 6;
            // 
            // WorkspaceBrowseButton
            // 
            this.WorkspaceBrowseButton.BackgroundImage = global::GUI.Properties.Resources.folder_explore;
            this.WorkspaceBrowseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.WorkspaceBrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WorkspaceBrowseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.WorkspaceBrowseButton.Location = new System.Drawing.Point(320, 16);
            this.WorkspaceBrowseButton.Name = "WorkspaceBrowseButton";
            this.WorkspaceBrowseButton.Size = new System.Drawing.Size(52, 21);
            this.WorkspaceBrowseButton.TabIndex = 7;
            this.WorkspaceBrowseButton.UseVisualStyleBackColor = true;
            this.WorkspaceBrowseButton.Click += new System.EventHandler(this.WorkspaceBrowseButtonClick);
            // 
            // WorkspaceLabel
            // 
            this.WorkspaceLabel.AutoSize = true;
            this.WorkspaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WorkspaceLabel.Location = new System.Drawing.Point(3, 0);
            this.WorkspaceLabel.Name = "WorkspaceLabel";
            this.WorkspaceLabel.Size = new System.Drawing.Size(108, 13);
            this.WorkspaceLabel.TabIndex = 0;
            this.WorkspaceLabel.Text = "Workspace directory:";
            // 
            // RecordHookSelectionLabel
            // 
            this.RecordHookSelectionLabel.AutoSize = true;
            this.RecordHookSelectionLabel.Location = new System.Drawing.Point(6, 68);
            this.RecordHookSelectionLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RecordHookSelectionLabel.Name = "RecordHookSelectionLabel";
            this.RecordHookSelectionLabel.Size = new System.Drawing.Size(82, 13);
            this.RecordHookSelectionLabel.TabIndex = 10;
            this.RecordHookSelectionLabel.Text = "Event Selection";
            // 
            // RecordHookSelectionBox
            // 
            this.RecordHookSelectionBox.CheckOnClick = true;
            this.RecordHookSelectionBox.FormattingEnabled = true;
            this.RecordHookSelectionBox.Items.AddRange(new object[] {
            "Focus Events",
            "Keyboard Events",
            "Mouse Events"});
            this.RecordHookSelectionBox.Location = new System.Drawing.Point(6, 84);
            this.RecordHookSelectionBox.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.RecordHookSelectionBox.Name = "RecordHookSelectionBox";
            this.RecordHookSelectionBox.Size = new System.Drawing.Size(311, 64);
            this.RecordHookSelectionBox.TabIndex = 9;
            // 
            // RecordLoggingFormatLabel
            // 
            this.RecordLoggingFormatLabel.AutoSize = true;
            this.RecordLoggingFormatLabel.Location = new System.Drawing.Point(6, 162);
            this.RecordLoggingFormatLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RecordLoggingFormatLabel.Name = "RecordLoggingFormatLabel";
            this.RecordLoggingFormatLabel.Size = new System.Drawing.Size(80, 13);
            this.RecordLoggingFormatLabel.TabIndex = 17;
            this.RecordLoggingFormatLabel.Text = "Logging Format";
            // 
            // RecordLoggingFormatList
            // 
            this.RecordLoggingFormatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RecordLoggingFormatList.Enabled = false;
            this.RecordLoggingFormatList.FormattingEnabled = true;
            this.RecordLoggingFormatList.Items.AddRange(new object[] {
            "Inputlog XML (*.xml)",
            "Text (*.txt)"});
            this.RecordLoggingFormatList.Location = new System.Drawing.Point(6, 178);
            this.RecordLoggingFormatList.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.RecordLoggingFormatList.Name = "RecordLoggingFormatList";
            this.RecordLoggingFormatList.Size = new System.Drawing.Size(311, 21);
            this.RecordLoggingFormatList.TabIndex = 18;
            // 
            // RecordPluginSelectionLabel
            // 
            this.RecordPluginSelectionLabel.AutoSize = true;
            this.RecordPluginSelectionLabel.Location = new System.Drawing.Point(6, 208);
            this.RecordPluginSelectionLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RecordPluginSelectionLabel.Name = "RecordPluginSelectionLabel";
            this.RecordPluginSelectionLabel.Size = new System.Drawing.Size(83, 13);
            this.RecordPluginSelectionLabel.TabIndex = 19;
            this.RecordPluginSelectionLabel.Text = "Plugin Selection";
            // 
            // RecordPluginSelectionBox
            // 
            this.RecordPluginSelectionBox.CheckOnClick = true;
            this.RecordPluginSelectionBox.FormattingEnabled = true;
            this.RecordPluginSelectionBox.Items.AddRange(new object[] {
            "WinLog - System Logging",
            "WordLog - Starts MS Word, logs position and doc. length"});
            this.RecordPluginSelectionBox.Location = new System.Drawing.Point(6, 224);
            this.RecordPluginSelectionBox.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.RecordPluginSelectionBox.Name = "RecordPluginSelectionBox";
            this.RecordPluginSelectionBox.Size = new System.Drawing.Size(311, 49);
            this.RecordPluginSelectionBox.TabIndex = 20;
            // 
            // General
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoSize = true;
            this.Controls.Add(this.RecordPluginSelectionLabel);
            this.Controls.Add(this.RecordPluginSelectionBox);
            this.Controls.Add(this.RecordLoggingFormatLabel);
            this.Controls.Add(this.RecordLoggingFormatList);
            this.Controls.Add(this.RecordHookSelectionLabel);
            this.Controls.Add(this.RecordHookSelectionBox);
            this.Controls.Add(this.panel1);
            this.Name = "General";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(464, 316);
            this.Load += new System.EventHandler(this.GeneralLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox WorkspaceTextField;
        private System.Windows.Forms.Button WorkspaceBrowseButton;
        private System.Windows.Forms.Label WorkspaceLabel;
        private System.Windows.Forms.FolderBrowserDialog WorkspaceBrowserDialog;
        private System.Windows.Forms.Label RecordHookSelectionLabel;
        private System.Windows.Forms.CheckedListBox RecordHookSelectionBox;
        private System.Windows.Forms.Label RecordLoggingFormatLabel;
        private System.Windows.Forms.ComboBox RecordLoggingFormatList;
        private System.Windows.Forms.Label RecordPluginSelectionLabel;
        private System.Windows.Forms.CheckedListBox RecordPluginSelectionBox;
    }
}
