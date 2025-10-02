namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	partial class FileSelectAndMatch
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
            this.Layout = new System.Windows.Forms.TableLayoutPanel();
            this.InnerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.PathLayout = new System.Windows.Forms.TableLayoutPanel();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.PathTxtBox = new System.Windows.Forms.TextBox();
            this.PathLbl = new System.Windows.Forms.Label();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.Layout.SuspendLayout();
            this.InnerLayout.SuspendLayout();
            this.PathLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Layout.ColumnCount = 1;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Layout.Controls.Add(this.InnerLayout, 0, 0);
            this.Layout.Controls.Add(this.FlowPanel, 0, 1);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Layout.Name = "Layout";
            this.Layout.RowCount = 2;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.Size = new System.Drawing.Size(925, 604);
            this.Layout.TabIndex = 0;
            // 
            // InnerLayout
            // 
            this.InnerLayout.ColumnCount = 3;
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.InnerLayout.Controls.Add(this.PathLayout, 0, 0);
            this.InnerLayout.Controls.Add(this.SelectAllButton, 1, 0);
            this.InnerLayout.Controls.Add(this.DeselectAllButton, 2, 0);
            this.InnerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerLayout.Location = new System.Drawing.Point(0, 0);
            this.InnerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.InnerLayout.Name = "InnerLayout";
            this.InnerLayout.RowCount = 1;
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.Size = new System.Drawing.Size(925, 44);
            this.InnerLayout.TabIndex = 0;
            // 
            // PathLayout
            // 
            this.PathLayout.ColumnCount = 3;
            this.PathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.PathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.PathLayout.Controls.Add(this.BrowseButton, 2, 0);
            this.PathLayout.Controls.Add(this.PathTxtBox, 1, 0);
            this.PathLayout.Controls.Add(this.PathLbl, 0, 0);
            this.PathLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PathLayout.Location = new System.Drawing.Point(0, 0);
            this.PathLayout.Margin = new System.Windows.Forms.Padding(0, 0, 50, 0);
            this.PathLayout.Name = "PathLayout";
            this.PathLayout.RowCount = 1;
            this.PathLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.PathLayout.Size = new System.Drawing.Size(663, 44);
            this.PathLayout.TabIndex = 0;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.BrowseButton.Location = new System.Drawing.Point(589, 6);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(71, 31);
            this.BrowseButton.TabIndex = 0;
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // PathTxtBox
            // 
            this.PathTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.PathTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PathTxtBox.Location = new System.Drawing.Point(58, 10);
            this.PathTxtBox.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.PathTxtBox.Name = "PathTxtBox";
            this.PathTxtBox.ReadOnly = true;
            this.PathTxtBox.Size = new System.Drawing.Size(520, 23);
            this.PathTxtBox.TabIndex = 1;
            // 
            // PathLbl
            // 
            this.PathLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.PathLbl.AutoSize = true;
            this.PathLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PathLbl.Location = new System.Drawing.Point(8, 13);
            this.PathLbl.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.PathLbl.Name = "PathLbl";
            this.PathLbl.Size = new System.Drawing.Size(42, 17);
            this.PathLbl.TabIndex = 2;
            this.PathLbl.Text = "Path";
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SelectAllButton.Location = new System.Drawing.Point(716, 6);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(95, 31);
            this.SelectAllButton.TabIndex = 1;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButtonClick);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.DeselectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DeselectAllButton.Location = new System.Drawing.Point(817, 6);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(105, 31);
            this.DeselectAllButton.TabIndex = 2;
            this.DeselectAllButton.Text = "Deselect All";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButtonClick);
            // 
            // FlowPanel
            // 
            this.FlowPanel.AutoScroll = true;
            this.FlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FlowPanel.BackColor = System.Drawing.SystemColors.Control;
            this.FlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.FlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlowPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FlowPanel.Location = new System.Drawing.Point(3, 64);
            this.FlowPanel.Margin = new System.Windows.Forms.Padding(3, 20, 3, 12);
            this.FlowPanel.Name = "FlowPanel";
            this.FlowPanel.Size = new System.Drawing.Size(919, 528);
            this.FlowPanel.TabIndex = 1;
            // 
            // FolderBrowserDialog
            // 
            this.FolderBrowserDialog.ShowNewFolderButton = false;
            // 
            // FileSelectAndMatch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.Layout);
            this.Name = "FileSelectAndMatch";
            this.Size = new System.Drawing.Size(925, 604);
            this.Layout.ResumeLayout(false);
            this.InnerLayout.ResumeLayout(false);
            this.PathLayout.ResumeLayout(false);
            this.PathLayout.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		private System.Windows.Forms.TableLayoutPanel InnerLayout;
		private System.Windows.Forms.TableLayoutPanel PathLayout;
		private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
		private System.Windows.Forms.Button BrowseButton;
		private System.Windows.Forms.TextBox PathTxtBox;
		private System.Windows.Forms.Label PathLbl;
		private System.Windows.Forms.Button SelectAllButton;
		private System.Windows.Forms.Button DeselectAllButton;
		private System.Windows.Forms.FlowLayoutPanel FlowPanel;
	}
}
