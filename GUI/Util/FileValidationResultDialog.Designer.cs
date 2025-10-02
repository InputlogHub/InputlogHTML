namespace GUI.Util
{
	partial class ControlDialog
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
			this.Layout = new System.Windows.Forms.TableLayoutPanel();
			this.ButtonLayout = new System.Windows.Forms.TableLayoutPanel();
			this.OkButton = new System.Windows.Forms.Button();
			this.ExplanationLbl = new System.Windows.Forms.Label();
			this.DataList = new System.Windows.Forms.ListView();
			this.FileColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.IssueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ExceptionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Layout.SuspendLayout();
			this.ButtonLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// Layout
			// 
			this.Layout.ColumnCount = 1;
			this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Layout.Controls.Add(this.DataList, 0, 1);
			this.Layout.Controls.Add(this.ButtonLayout, 0, 2);
			this.Layout.Controls.Add(this.ExplanationLbl, 0, 0);
			this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Layout.Location = new System.Drawing.Point(0, 0);
			this.Layout.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
			this.Layout.Name = "Layout";
			this.Layout.RowCount = 3;
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.Layout.Size = new System.Drawing.Size(1006, 387);
			this.Layout.TabIndex = 0;
			// 
			// ButtonLayout
			// 
			this.ButtonLayout.ColumnCount = 2;
			this.ButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
			this.ButtonLayout.Controls.Add(this.OkButton, 1, 0);
			this.ButtonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonLayout.Location = new System.Drawing.Point(0, 352);
			this.ButtonLayout.Margin = new System.Windows.Forms.Padding(0);
			this.ButtonLayout.Name = "ButtonLayout";
			this.ButtonLayout.RowCount = 1;
			this.ButtonLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ButtonLayout.Size = new System.Drawing.Size(1006, 35);
			this.ButtonLayout.TabIndex = 0;
			// 
			// OkButton
			// 
			this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkButton.Location = new System.Drawing.Point(896, 2);
			this.OkButton.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(100, 30);
			this.OkButton.TabIndex = 1;
			this.OkButton.Text = "Continue";
			this.OkButton.UseVisualStyleBackColor = true;
			// 
			// ExplanationLbl
			// 
			this.ExplanationLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ExplanationLbl.AutoSize = true;
			this.ExplanationLbl.Location = new System.Drawing.Point(3, 8);
			this.ExplanationLbl.Name = "ExplanationLbl";
			this.ExplanationLbl.Size = new System.Drawing.Size(1000, 13);
			this.ExplanationLbl.TabIndex = 2;
			this.ExplanationLbl.Text = "Explanation Text";
			this.ExplanationLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// DataList
			// 
			this.DataList.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.DataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileColumn,
            this.IssueColumn,
            this.ExceptionColumn});
			this.DataList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataList.GridLines = true;
			this.DataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.DataList.HoverSelection = true;
			this.DataList.Location = new System.Drawing.Point(0, 30);
			this.DataList.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.DataList.Name = "DataList";
			this.DataList.Size = new System.Drawing.Size(1006, 317);
			this.DataList.TabIndex = 3;
			this.DataList.UseCompatibleStateImageBehavior = false;
			this.DataList.View = System.Windows.Forms.View.Details;
			// 
			// FileColumn
			// 
			this.FileColumn.Text = "File";
			this.FileColumn.Width = 250;
			// 
			// IssueColumn
			// 
			this.IssueColumn.Text = "Issue";
			this.IssueColumn.Width = 250;
			// 
			// ExceptionColumn
			// 
			this.ExceptionColumn.Text = "Errors";
			this.ExceptionColumn.Width = 500;
			// 
			// ControlDialog
			// 
			this.AcceptButton = this.OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1006, 387);
			this.ControlBox = false;
			this.Controls.Add(this.Layout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControlDialog";
			this.ShowInTaskbar = false;
			this.Text = "File Issues";
			this.Layout.ResumeLayout(false);
			this.Layout.PerformLayout();
			this.ButtonLayout.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		private System.Windows.Forms.TableLayoutPanel ButtonLayout;
		public System.Windows.Forms.Button OkButton;
		public System.Windows.Forms.ListView DataList;
		private System.Windows.Forms.ColumnHeader FileColumn;
		private System.Windows.Forms.ColumnHeader IssueColumn;
		private System.Windows.Forms.ColumnHeader ExceptionColumn;
		public System.Windows.Forms.Label ExplanationLbl;
	}
}