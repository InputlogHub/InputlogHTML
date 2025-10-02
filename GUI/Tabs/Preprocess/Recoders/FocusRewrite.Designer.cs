using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GUI.Properties;

namespace GUI.Tabs.Preprocess.Recoders
{
	partial class FocusRewrite
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FocusRewrite));
            this.Ungrouped = new ListBox();
            this.UngroupedLabel = new Label();
            this.NewGroupButton = new Button();
            this.GroupsLabel = new Label();
            this.nameLabel = new Label();
            this.MoreInfoLabel = new LinkLabel();
            this.Grouped = new ListView();
            this.GroupName = ((ColumnHeader)(new ColumnHeader()));
            this.NumberOfEntries = ((ColumnHeader)(new ColumnHeader()));
            this.SelectSavedLogFile = new OpenFileDialog();
            this.LoadBtn = new Button();
            this.SuspendLayout();
            // 
            // Ungrouped
            // 
            this.Ungrouped.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.Ungrouped.FormattingEnabled = true;
            this.Ungrouped.Location = new Point(8, 63);
            this.Ungrouped.Margin = new Padding(8);
            this.Ungrouped.Name = "Ungrouped";
            this.Ungrouped.Size = new Size(284, 108);
            this.Ungrouped.TabIndex = 0;
            this.Ungrouped.DoubleClick += new EventHandler(this.UngroupedDoubleClicked);
            // 
            // UngroupedLabel
            // 
            this.UngroupedLabel.AutoSize = true;
            this.UngroupedLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            this.UngroupedLabel.Location = new Point(8, 41);
            this.UngroupedLabel.Name = "UngroupedLabel";
            this.UngroupedLabel.Size = new Size(69, 13);
            this.UngroupedLabel.TabIndex = 1;
            this.UngroupedLabel.Text = "Ungrouped";
            // 
            // NewGroupButton
            // 
            this.NewGroupButton.Image = ((Image)(resources.GetObject("NewGroupButton.Image")));
            this.NewGroupButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.NewGroupButton.Location = new Point(83, 36);
            this.NewGroupButton.Name = "NewGroupButton";
            this.NewGroupButton.Size = new Size(96, 23);
            this.NewGroupButton.TabIndex = 2;
            this.NewGroupButton.Text = "New Group";
            this.NewGroupButton.TextAlign = ContentAlignment.MiddleRight;
            this.NewGroupButton.UseVisualStyleBackColor = true;
            this.NewGroupButton.Click += new EventHandler(this.NewGroupButtonClick);
            // 
            // GroupsLabel
            // 
            this.GroupsLabel.Anchor = AnchorStyles.Left;
            this.GroupsLabel.AutoSize = true;
            this.GroupsLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            this.GroupsLabel.Location = new Point(8, 171);
            this.GroupsLabel.Name = "GroupsLabel";
            this.GroupsLabel.Size = new Size(47, 13);
            this.GroupsLabel.TabIndex = 4;
            this.GroupsLabel.Text = "Groups";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new Point(7, 3);
            this.nameLabel.Margin = new Padding(2, 0, 2, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new Size(161, 20);
            this.nameLabel.TabIndex = 5;
            this.nameLabel.Text = "Focus Event Rewriter";
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Enabled = false;
            this.MoreInfoLabel.Font = new Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new Point(165, 8);
            this.MoreInfoLabel.Margin = new Padding(2, 0, 2, 0);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new Size(52, 13);
            this.MoreInfoLabel.TabIndex = 6;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More Info";
            // 
            // Grouped
            // 
            this.Grouped.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.Grouped.Columns.AddRange(new ColumnHeader[] {
            this.GroupName,
            this.NumberOfEntries});
            this.Grouped.GridLines = true;
            this.Grouped.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.Grouped.Location = new Point(8, 189);
            this.Grouped.MultiSelect = false;
            this.Grouped.Name = "Grouped";
            this.Grouped.ShowGroups = false;
            this.Grouped.Size = new Size(284, 122);
            this.Grouped.Sorting = SortOrder.Ascending;
            this.Grouped.TabIndex = 7;
            this.Grouped.UseCompatibleStateImageBehavior = false;
            this.Grouped.View = View.Details;
            this.Grouped.DoubleClick += new EventHandler(this.GroupedItemDoubleClicked);
            // 
            // GroupName
            // 
            this.GroupName.Text = "Groupname";
            this.GroupName.Width = 204;
            // 
            // NumberOfEntries
            // 
            this.NumberOfEntries.Text = "# entries";
            this.NumberOfEntries.Width = 87;
            // 
            // SelectSavedLogFile
            // 
            this.SelectSavedLogFile.FileName = "openFileDialog1";
            // 
            // LoadBtn
            // 
            this.LoadBtn.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.LoadBtn.Image = Resources.folder_explore;
            this.LoadBtn.ImageAlign = ContentAlignment.MiddleLeft;
            this.LoadBtn.Location = new Point(185, 36);
            this.LoadBtn.Name = "LoadBtn";
            this.LoadBtn.Size = new Size(96, 23);
            this.LoadBtn.TabIndex = 8;
            this.LoadBtn.Text = "Load Log";
            this.LoadBtn.TextAlign = ContentAlignment.MiddleRight;
            this.LoadBtn.UseVisualStyleBackColor = true;
            this.LoadBtn.Click += new EventHandler(this.LoadBtn_Click_1);
            // 
            // FocusRewrite
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.LoadBtn);
            this.Controls.Add(this.Grouped);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.GroupsLabel);
            this.Controls.Add(this.NewGroupButton);
            this.Controls.Add(this.UngroupedLabel);
            this.Controls.Add(this.Ungrouped);
            this.Name = "FocusRewrite";
            this.Size = new Size(300, 353);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public ListBox Ungrouped;
		public Label UngroupedLabel;
		private Button NewGroupButton;
		public Label GroupsLabel;
		private Label nameLabel;
		private LinkLabel MoreInfoLabel;
		public ListView Grouped;
		private ColumnHeader GroupName;
		private ColumnHeader NumberOfEntries;
        private OpenFileDialog SelectSavedLogFile;
        private Button LoadBtn;
	}
}
