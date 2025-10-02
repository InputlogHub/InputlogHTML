namespace GUI.Tabs.Postprocess.WizardPages.Form
{
	partial class FileResultProcessPage
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
            this.OpenFolderLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(8, 414);
            // 
            // OpenFolderLink
            // 
            this.OpenFolderLink.ActiveLinkColor = System.Drawing.Color.Blue;
            this.OpenFolderLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OpenFolderLink.Enabled = false;
            this.OpenFolderLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.OpenFolderLink.Location = new System.Drawing.Point(562, 383);
            this.OpenFolderLink.Name = "OpenFolderLink";
            this.OpenFolderLink.Size = new System.Drawing.Size(100, 23);
            this.OpenFolderLink.TabIndex = 3;
            this.OpenFolderLink.TabStop = true;
            this.OpenFolderLink.Text = "Open Folder";
            this.OpenFolderLink.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.OpenFolderLink.Visible = false;
            this.OpenFolderLink.VisitedLinkColor = System.Drawing.Color.Blue;
            this.OpenFolderLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenFolderLink_LinkClicked);
            // 
            // FileResultProcessPage
            // 
            this.Controls.Add(this.OpenFolderLink);
            this.Name = "FileResultProcessPage";
            this.Size = new System.Drawing.Size(670, 447);
            this.Controls.SetChildIndex(this.ProgressBar, 0);
            this.Controls.SetChildIndex(this.OpenFolderLink, 0);
            this.Controls.SetChildIndex(this.Output, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.LinkLabel OpenFolderLink;
	}
}
