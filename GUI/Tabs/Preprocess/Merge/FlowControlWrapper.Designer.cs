namespace GUI.Tabs.Preprocess.Merge
{
	partial class FlowControlWrapper
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
			this.WrapperLayout = new System.Windows.Forms.TableLayoutPanel();
			this.ClosePicture = new System.Windows.Forms.PictureBox();
			this.Checkbox = new System.Windows.Forms.CheckBox();
			this.Layout.SuspendLayout();
			this.WrapperLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ClosePicture)).BeginInit();
			this.SuspendLayout();
			// 
			// Layout
			// 
			this.Layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Layout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.Layout.ColumnCount = 1;
			this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Layout.Controls.Add(this.WrapperLayout, 0, 0);
			this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Layout.Location = new System.Drawing.Point(0, 0);
			this.Layout.Margin = new System.Windows.Forms.Padding(0);
			this.Layout.Name = "Layout";
			this.Layout.RowCount = 2;
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.Layout.Size = new System.Drawing.Size(150, 32);
			this.Layout.TabIndex = 0;
			// 
			// WrapperLayout
			// 
			this.WrapperLayout.BackColor = System.Drawing.SystemColors.ControlLight;
			this.WrapperLayout.ColumnCount = 2;
			this.WrapperLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.WrapperLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.WrapperLayout.Controls.Add(this.ClosePicture, 1, 0);
			this.WrapperLayout.Controls.Add(this.Checkbox, 0, 0);
			this.WrapperLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WrapperLayout.Location = new System.Drawing.Point(1, 1);
			this.WrapperLayout.Margin = new System.Windows.Forms.Padding(0);
			this.WrapperLayout.Name = "WrapperLayout";
			this.WrapperLayout.RowCount = 1;
			this.WrapperLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.WrapperLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.WrapperLayout.Size = new System.Drawing.Size(148, 35);
			this.WrapperLayout.TabIndex = 0;
			// 
			// ClosePicture
			// 
			this.ClosePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ClosePicture.Image = global::GUI.Properties.Resources.cross_grey;
			this.ClosePicture.InitialImage = global::GUI.Properties.Resources.cross_grey;
			this.ClosePicture.Location = new System.Drawing.Point(119, 3);
			this.ClosePicture.Name = "ClosePicture";
			this.ClosePicture.Size = new System.Drawing.Size(26, 25);
			this.ClosePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.ClosePicture.TabIndex = 1;
			this.ClosePicture.TabStop = false;
			this.ClosePicture.Click += new System.EventHandler(this.PictClose_Click);
			this.ClosePicture.MouseEnter += new System.EventHandler(this.PictClose_MouseEnter);
			this.ClosePicture.MouseLeave += new System.EventHandler(this.PictClose_MouseLeave);
			// 
			// Checkbox
			// 
			this.Checkbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.Checkbox.AutoSize = true;
			this.Checkbox.Checked = true;
			this.Checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.Checkbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Checkbox.Location = new System.Drawing.Point(6, 9);
			this.Checkbox.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this.Checkbox.Name = "Checkbox";
			this.Checkbox.Size = new System.Drawing.Size(61, 17);
			this.Checkbox.TabIndex = 2;
			this.Checkbox.Text = "Match";
			this.Checkbox.UseVisualStyleBackColor = true;
			this.Checkbox.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// FlowControlWrapper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.Layout);
			this.Name = "FlowControlWrapper";
			this.Size = new System.Drawing.Size(150, 32);
			this.Layout.ResumeLayout(false);
			this.WrapperLayout.ResumeLayout(false);
			this.WrapperLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ClosePicture)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		private System.Windows.Forms.TableLayoutPanel WrapperLayout;
		private System.Windows.Forms.PictureBox ClosePicture;
		private System.Windows.Forms.CheckBox Checkbox;

	}
}
