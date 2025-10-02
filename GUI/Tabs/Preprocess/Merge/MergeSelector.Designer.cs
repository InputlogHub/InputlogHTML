namespace GUI.Tabs.Preprocess.Merge
{
	partial class MergeSelector
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
            this.InfoLbl = new System.Windows.Forms.Label();
            this.MergeButton = new System.Windows.Forms.Button();
            this.Layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.ColumnCount = 2;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Layout.Controls.Add(this.InfoLbl, 0, 0);
            this.Layout.Controls.Add(this.MergeButton, 1, 1);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Name = "Layout";
            this.Layout.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Layout.RowCount = 2;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.Layout.Size = new System.Drawing.Size(598, 87);
            this.Layout.TabIndex = 0;
            // 
            // InfoLbl
            // 
            this.InfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.InfoLbl.Location = new System.Drawing.Point(6, 6);
            this.InfoLbl.Name = "InfoLbl";
            this.InfoLbl.Padding = new System.Windows.Forms.Padding(35, 0, 0, 0);
            this.InfoLbl.Size = new System.Drawing.Size(427, 23);
            this.InfoLbl.TabIndex = 1;
            this.InfoLbl.Text = "Merge .idfx files with:";
            this.InfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MergeButton
            // 
            this.MergeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MergeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MergeButton.Location = new System.Drawing.Point(456, 45);
            this.MergeButton.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
            this.MergeButton.Name = "MergeButton";
            this.Layout.SetRowSpan(this.MergeButton, 2);
            this.MergeButton.Size = new System.Drawing.Size(119, 27);
            this.MergeButton.TabIndex = 0;
            this.MergeButton.Text = "Merge";
            this.MergeButton.UseVisualStyleBackColor = true;
            this.MergeButton.Click += new System.EventHandler(this.MergeButton_Click);
            // 
            // MergeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Layout);
            this.Name = "MergeSelector";
            this.Size = new System.Drawing.Size(598, 87);
            this.Layout.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		private System.Windows.Forms.Button MergeButton;
        private System.Windows.Forms.Label InfoLbl;
	}
}
