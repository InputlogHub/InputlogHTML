namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    partial class FileWizardPageForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.BrowseFolders = new System.Windows.Forms.Button();
            this.FolderSearcher = new System.DirectoryServices.DirectorySearcher();
            this.label2 = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.AddAllButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.RemoveAllButton = new System.Windows.Forms.Button();
            this.SelectedFiles = new System.Windows.Forms.ListBox();
            this.folderSelectTip = new System.Windows.Forms.ToolTip(this.components);
            this.FilesInFolderTabs = new System.Windows.Forms.TabControl();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Files in selected folder";
            // 
            // BrowseFolders
            // 
            this.BrowseFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseFolders.Image = global::GUI.Properties.Resources.folder_explore;
            this.BrowseFolders.Location = new System.Drawing.Point(310, 50);
            this.BrowseFolders.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.BrowseFolders.Name = "BrowseFolders";
            this.BrowseFolders.Size = new System.Drawing.Size(50, 30);
            this.BrowseFolders.TabIndex = 2;
            this.folderSelectTip.SetToolTip(this.BrowseFolders, "Please confirm your choice by selecting one file as an example.");
            this.BrowseFolders.UseVisualStyleBackColor = true;
            this.BrowseFolders.Click += new System.EventHandler(this.BrowseFoldersClick);
            // 
            // FolderSearcher
            // 
            this.FolderSearcher.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.FolderSearcher.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.FolderSearcher.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Files to be merged";
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddButton.Location = new System.Drawing.Point(310, 200);
            this.AddButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(50, 30);
            this.AddButton.TabIndex = 5;
            this.AddButton.Text = ">";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // AddAllButton
            // 
            this.AddAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddAllButton.Location = new System.Drawing.Point(310, 160);
            this.AddAllButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.AddAllButton.Name = "AddAllButton";
            this.AddAllButton.Size = new System.Drawing.Size(50, 30);
            this.AddAllButton.TabIndex = 6;
            this.AddAllButton.Text = ">>";
            this.AddAllButton.UseVisualStyleBackColor = true;
            this.AddAllButton.Click += new System.EventHandler(this.AddAllButtonClick);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveButton.Location = new System.Drawing.Point(310, 240);
            this.RemoveButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RemoveButton.Size = new System.Drawing.Size(50, 30);
            this.RemoveButton.TabIndex = 7;
            this.RemoveButton.Text = "<";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButtonClick);
            // 
            // RemoveAllButton
            // 
            this.RemoveAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveAllButton.Location = new System.Drawing.Point(310, 280);
            this.RemoveAllButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RemoveAllButton.Name = "RemoveAllButton";
            this.RemoveAllButton.Size = new System.Drawing.Size(50, 30);
            this.RemoveAllButton.TabIndex = 8;
            this.RemoveAllButton.Text = "<<";
            this.RemoveAllButton.UseVisualStyleBackColor = true;
            this.RemoveAllButton.Click += new System.EventHandler(this.RemoveAllButtonClick);
            // 
            // SelectedFiles
            // 
            this.SelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedFiles.HorizontalScrollbar = true;
            this.SelectedFiles.Location = new System.Drawing.Point(402, 50);
            this.SelectedFiles.Name = "SelectedFiles";
            this.SelectedFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.SelectedFiles.Size = new System.Drawing.Size(260, 329);
            this.SelectedFiles.TabIndex = 10;
            // 
            // folderSelectTip
            // 
            this.folderSelectTip.IsBalloon = true;
            // 
            // FilesInFolderTabs
            // 
            this.FilesInFolderTabs.Location = new System.Drawing.Point(8, 29);
            this.FilesInFolderTabs.Name = "FilesInFolderTabs";
            this.FilesInFolderTabs.SelectedIndex = 0;
            this.FilesInFolderTabs.Size = new System.Drawing.Size(265, 355);
            this.FilesInFolderTabs.TabIndex = 11;
            // 
            // FileWizardPageForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.FilesInFolderTabs);
            this.Controls.Add(this.SelectedFiles);
            this.Controls.Add(this.RemoveAllButton);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.AddAllButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BrowseFolders);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Name = "FileWizardPageForm";
            this.Size = new System.Drawing.Size(670, 420);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BrowseFolders;
        private System.DirectoryServices.DirectorySearcher FolderSearcher;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button AddAllButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button RemoveAllButton;
        private System.Windows.Forms.ListBox SelectedFiles;
        private System.Windows.Forms.ToolTip folderSelectTip;
        private System.Windows.Forms.TabControl FilesInFolderTabs;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;



    }
}
