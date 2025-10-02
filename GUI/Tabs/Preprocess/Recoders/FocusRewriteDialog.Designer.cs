namespace GUI.Tabs.Preprocess.Recoders
{
	partial class FocusRewriteDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FocusRewriteDialog));
            this.Ungrouped = new System.Windows.Forms.ListBox();
            this.UngroupedLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Groups = new System.Windows.Forms.ListView();
            this.GroupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NumberOfEntries = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CurrentGroupContainer = new System.Windows.Forms.GroupBox();
            this.RemoveFromGroupButton = new System.Windows.Forms.Button();
            this.AddToGroupButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ActiveGroupListbox = new System.Windows.Forms.ListBox();
            this.GroupNameField = new System.Windows.Forms.TextBox();
            this.NewGroupButton = new System.Windows.Forms.Button();
            this.DeleteGroupButton = new System.Windows.Forms.Button();
            this.SaveGroupButton = new System.Windows.Forms.Button();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.SearchLabel = new System.Windows.Forms.Label();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.SearchTxtBx = new System.Windows.Forms.TextBox();
            this.SearchGrpBx = new System.Windows.Forms.GroupBox();
            this.UngroupedTooTip = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SourceGrpBx = new System.Windows.Forms.GroupBox();
            this.CurrentGroupContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SearchPanel.SuspendLayout();
            this.SearchGrpBx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SourceGrpBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // Ungrouped
            // 
            this.Ungrouped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Ungrouped.FormattingEnabled = true;
            this.Ungrouped.HorizontalScrollbar = true;
            this.Ungrouped.Location = new System.Drawing.Point(0, 0);
            this.Ungrouped.Margin = new System.Windows.Forms.Padding(8);
            this.Ungrouped.Name = "Ungrouped";
            this.Ungrouped.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.Ungrouped.Size = new System.Drawing.Size(250, 378);
            this.Ungrouped.Sorted = true;
            this.Ungrouped.TabIndex = 0;
            // 
            // UngroupedLabel
            // 
            this.UngroupedLabel.AutoSize = true;
            this.UngroupedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UngroupedLabel.Location = new System.Drawing.Point(3, 21);
            this.UngroupedLabel.Name = "UngroupedLabel";
            this.UngroupedLabel.Size = new System.Drawing.Size(119, 13);
            this.UngroupedLabel.TabIndex = 1;
            this.UngroupedLabel.Text = "Ungrouped Sources";
            this.UngroupedTooTip.SetToolTip(this.UngroupedLabel, "Main document names are surrounded by asterisks.");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(683, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Existing Groups";
            // 
            // Groups
            // 
            this.Groups.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.Groups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GroupName,
            this.NumberOfEntries});
            this.Groups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Groups.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Groups.HideSelection = false;
            this.Groups.Location = new System.Drawing.Point(0, 0);
            this.Groups.MultiSelect = false;
            this.Groups.Name = "Groups";
            this.Groups.ShowGroups = false;
            this.Groups.Size = new System.Drawing.Size(240, 378);
            this.Groups.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.Groups.TabIndex = 8;
            this.Groups.UseCompatibleStateImageBehavior = false;
            this.Groups.View = System.Windows.Forms.View.Details;
            this.Groups.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.GroupedSelectionChangeEvent);
            // 
            // GroupName
            // 
            this.GroupName.Text = "Groupname";
            this.GroupName.Width = 130;
            // 
            // NumberOfEntries
            // 
            this.NumberOfEntries.Text = "# entries";
            this.NumberOfEntries.Width = 70;
            // 
            // CurrentGroupContainer
            // 
            this.CurrentGroupContainer.Controls.Add(this.RemoveFromGroupButton);
            this.CurrentGroupContainer.Controls.Add(this.AddToGroupButton);
            this.CurrentGroupContainer.Controls.Add(this.label2);
            this.CurrentGroupContainer.Controls.Add(this.ActiveGroupListbox);
            this.CurrentGroupContainer.Controls.Add(this.GroupNameField);
            this.CurrentGroupContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrentGroupContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentGroupContainer.Location = new System.Drawing.Point(0, 0);
            this.CurrentGroupContainer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.CurrentGroupContainer.Name = "CurrentGroupContainer";
            this.CurrentGroupContainer.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.CurrentGroupContainer.Size = new System.Drawing.Size(274, 378);
            this.CurrentGroupContainer.TabIndex = 9;
            this.CurrentGroupContainer.TabStop = false;
            // 
            // RemoveFromGroupButton
            // 
            this.RemoveFromGroupButton.Location = new System.Drawing.Point(6, 183);
            this.RemoveFromGroupButton.Name = "RemoveFromGroupButton";
            this.RemoveFromGroupButton.Size = new System.Drawing.Size(32, 32);
            this.RemoveFromGroupButton.TabIndex = 6;
            this.RemoveFromGroupButton.Text = "<";
            this.RemoveFromGroupButton.UseVisualStyleBackColor = true;
            this.RemoveFromGroupButton.Click += new System.EventHandler(this.RemoveFromGroupButtonClick);
            // 
            // AddToGroupButton
            // 
            this.AddToGroupButton.Location = new System.Drawing.Point(6, 145);
            this.AddToGroupButton.Name = "AddToGroupButton";
            this.AddToGroupButton.Size = new System.Drawing.Size(32, 32);
            this.AddToGroupButton.TabIndex = 5;
            this.AddToGroupButton.Text = ">";
            this.AddToGroupButton.UseVisualStyleBackColor = true;
            this.AddToGroupButton.Click += new System.EventHandler(this.AddToGroupButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Group name:";
            // 
            // ActiveGroupListbox
            // 
            this.ActiveGroupListbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ActiveGroupListbox.FormattingEnabled = true;
            this.ActiveGroupListbox.HorizontalScrollbar = true;
            this.ActiveGroupListbox.Location = new System.Drawing.Point(44, 62);
            this.ActiveGroupListbox.Name = "ActiveGroupListbox";
            this.ActiveGroupListbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ActiveGroupListbox.Size = new System.Drawing.Size(176, 238);
            this.ActiveGroupListbox.TabIndex = 3;
            // 
            // GroupNameField
            // 
            this.GroupNameField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupNameField.Location = new System.Drawing.Point(44, 36);
            this.GroupNameField.Name = "GroupNameField";
            this.GroupNameField.Size = new System.Drawing.Size(176, 20);
            this.GroupNameField.TabIndex = 0;
            this.GroupNameField.TextChanged += new System.EventHandler(this.GroupNameFieldTextChanged);
            // 
            // NewGroupButton
            // 
            this.NewGroupButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NewGroupButton.Location = new System.Drawing.Point(303, 8);
            this.NewGroupButton.Name = "NewGroupButton";
            this.NewGroupButton.Size = new System.Drawing.Size(167, 30);
            this.NewGroupButton.TabIndex = 10;
            this.NewGroupButton.Text = "New Group";
            this.NewGroupButton.UseVisualStyleBackColor = true;
            this.NewGroupButton.Click += new System.EventHandler(this.NewGroupButtonClick);
            // 
            // DeleteGroupButton
            // 
            this.DeleteGroupButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DeleteGroupButton.Location = new System.Drawing.Point(303, 44);
            this.DeleteGroupButton.Name = "DeleteGroupButton";
            this.DeleteGroupButton.Size = new System.Drawing.Size(166, 30);
            this.DeleteGroupButton.TabIndex = 11;
            this.DeleteGroupButton.Text = "Delete Group";
            this.DeleteGroupButton.UseVisualStyleBackColor = true;
            this.DeleteGroupButton.Click += new System.EventHandler(this.DeleteGroupButtonClick);
            // 
            // SaveGroupButton
            // 
            this.SaveGroupButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SaveGroupButton.Location = new System.Drawing.Point(303, 80);
            this.SaveGroupButton.Name = "SaveGroupButton";
            this.SaveGroupButton.Size = new System.Drawing.Size(166, 30);
            this.SaveGroupButton.TabIndex = 12;
            this.SaveGroupButton.Text = "Save Group";
            this.SaveGroupButton.UseVisualStyleBackColor = true;
            this.SaveGroupButton.Click += new System.EventHandler(this.SaveGroupButtonClick);
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.ContainerControl = this;
            // 
            // SearchLabel
            // 
            this.SearchLabel.Location = new System.Drawing.Point(12, 12);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(179, 53);
            this.SearchLabel.TabIndex = 14;
            this.SearchLabel.Text = "Wildcard Search \r\nUse ? for one character \r\nand * for zero or many.";
            // 
            // SearchPanel
            // 
            this.SearchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchPanel.Controls.Add(this.SearchBtn);
            this.SearchPanel.Controls.Add(this.SearchTxtBx);
            this.SearchPanel.Location = new System.Drawing.Point(12, 68);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(190, 32);
            this.SearchPanel.TabIndex = 15;
            // 
            // SearchBtn
            // 
            this.SearchBtn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.SearchBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.SearchBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.SearchBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.SearchBtn.Image = global::GUI.Properties.Resources.searchT;
            this.SearchBtn.Location = new System.Drawing.Point(159, 2);
            this.SearchBtn.Margin = new System.Windows.Forms.Padding(0);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(31, 28);
            this.SearchBtn.TabIndex = 2;
            this.SearchBtn.UseVisualStyleBackColor = false;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtnClick);
            // 
            // SearchTxtBx
            // 
            this.SearchTxtBx.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SearchTxtBx.Location = new System.Drawing.Point(3, 3);
            this.SearchTxtBx.Name = "SearchTxtBx";
            this.SearchTxtBx.Size = new System.Drawing.Size(156, 20);
            this.SearchTxtBx.TabIndex = 1;
            // 
            // SearchGrpBx
            // 
            this.SearchGrpBx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchGrpBx.Controls.Add(this.SearchLabel);
            this.SearchGrpBx.Controls.Add(this.SearchPanel);
            this.SearchGrpBx.Location = new System.Drawing.Point(0, 9);
            this.SearchGrpBx.Name = "SearchGrpBx";
            this.SearchGrpBx.Size = new System.Drawing.Size(205, 115);
            this.SearchGrpBx.TabIndex = 16;
            this.SearchGrpBx.TabStop = false;
            // 
            // UngroupedTooTip
            // 
            this.UngroupedTooTip.AutoPopDelay = 6000;
            this.UngroupedTooTip.InitialDelay = 500;
            this.UngroupedTooTip.IsBalloon = true;
            this.UngroupedTooTip.ReshowDelay = 100;
            this.UngroupedTooTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.UngroupedTooTip.ToolTipTitle = "Main Document";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(686, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 30);
            this.button1.TabIndex = 17;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 53);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AccessibleName = "SourceSplitter";
            this.splitContainer1.Panel1.Controls.Add(this.Ungrouped);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(772, 378);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 18;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.CurrentGroupContainer);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.Groups);
            this.splitContainer2.Size = new System.Drawing.Size(518, 378);
            this.splitContainer2.SplitterDistance = 274;
            this.splitContainer2.TabIndex = 0;
            // 
            // SourceGrpBx
            // 
            this.SourceGrpBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SourceGrpBx.Controls.Add(this.button1);
            this.SourceGrpBx.Controls.Add(this.NewGroupButton);
            this.SourceGrpBx.Controls.Add(this.SaveGroupButton);
            this.SourceGrpBx.Controls.Add(this.DeleteGroupButton);
            this.SourceGrpBx.Controls.Add(this.SearchGrpBx);
            this.SourceGrpBx.Location = new System.Drawing.Point(6, 426);
            this.SourceGrpBx.Name = "SourceGrpBx";
            this.SourceGrpBx.Size = new System.Drawing.Size(772, 121);
            this.SourceGrpBx.TabIndex = 19;
            this.SourceGrpBx.TabStop = false;
            // 
            // FocusRewriteDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.SourceGrpBx);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UngroupedLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FocusRewriteDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Focus Event Groups";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseForm);
            this.CurrentGroupContainer.ResumeLayout(false);
            this.CurrentGroupContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.SearchGrpBx.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.SourceGrpBx.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.ListBox Ungrouped;
		private System.Windows.Forms.Label UngroupedLabel;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.ListView Groups;
		private System.Windows.Forms.ColumnHeader GroupName;
		private System.Windows.Forms.ColumnHeader NumberOfEntries;
		private System.Windows.Forms.GroupBox CurrentGroupContainer;
		private System.Windows.Forms.Button RemoveFromGroupButton;
		private System.Windows.Forms.Button AddToGroupButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox ActiveGroupListbox;
		private System.Windows.Forms.TextBox GroupNameField;
		private System.Windows.Forms.Button NewGroupButton;
		private System.Windows.Forms.Button DeleteGroupButton;
		private System.Windows.Forms.Button SaveGroupButton;
		private System.Windows.Forms.ErrorProvider ErrorProvider;
        private System.Windows.Forms.Panel SearchPanel;
        private System.Windows.Forms.TextBox SearchTxtBx;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.GroupBox SearchGrpBx;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.ToolTip UngroupedTooTip;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox SourceGrpBx;
    }
}