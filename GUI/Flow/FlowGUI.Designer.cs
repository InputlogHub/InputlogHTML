namespace GUI.Flow
{
	partial class FlowGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowGUI));
            this.Layout = new System.Windows.Forms.TableLayoutPanel();
            this.InfoLayout = new System.Windows.Forms.TableLayoutPanel();
            this.InfoLbl = new System.Windows.Forms.Label();
            this.ProcessInfoLbl = new System.Windows.Forms.Label();
            this.ProcessLbl = new System.Windows.Forms.Label();
            this.FlowLayout = new System.Windows.Forms.TableLayoutPanel();
            this.NextButton = new System.Windows.Forms.Button();
            this.PreviousButton = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.Layout.SuspendLayout();
            this.InfoLayout.SuspendLayout();
            this.FlowLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Layout.ColumnCount = 1;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Layout.Controls.Add(this.InfoLayout, 0, 0);
            this.Layout.Controls.Add(this.FlowLayout, 0, 2);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Margin = new System.Windows.Forms.Padding(8, 8, 3, 15);
            this.Layout.Name = "Layout";
            this.Layout.RowCount = 3;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.Layout.Size = new System.Drawing.Size(934, 664);
            this.Layout.TabIndex = 0;
            // 
            // InfoLayout
            // 
            this.InfoLayout.ColumnCount = 3;
            this.InfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.InfoLayout.Controls.Add(this.InfoLbl, 0, 0);
            this.InfoLayout.Controls.Add(this.ProcessInfoLbl, 1, 0);
            this.InfoLayout.Controls.Add(this.ProcessLbl, 2, 0);
            this.InfoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoLayout.Location = new System.Drawing.Point(0, 0);
            this.InfoLayout.Margin = new System.Windows.Forms.Padding(0);
            this.InfoLayout.Name = "InfoLayout";
            this.InfoLayout.RowCount = 1;
            this.InfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoLayout.Size = new System.Drawing.Size(934, 25);
            this.InfoLayout.TabIndex = 0;
            // 
            // InfoLbl
            // 
            this.InfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.InfoLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.InfoLbl.Location = new System.Drawing.Point(3, 0);
            this.InfoLbl.Name = "InfoLbl";
            this.InfoLbl.Size = new System.Drawing.Size(761, 25);
            this.InfoLbl.TabIndex = 0;
            this.InfoLbl.Text = "Info";
            this.InfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProcessInfoLbl
            // 
            this.ProcessInfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessInfoLbl.AutoSize = true;
            this.ProcessInfoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProcessInfoLbl.Location = new System.Drawing.Point(770, 6);
            this.ProcessInfoLbl.Name = "ProcessInfoLbl";
            this.ProcessInfoLbl.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.ProcessInfoLbl.Size = new System.Drawing.Size(97, 13);
            this.ProcessInfoLbl.TabIndex = 1;
            this.ProcessInfoLbl.Text = "Processing data...";
            this.ProcessInfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProcessLbl
            // 
            this.ProcessLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessLbl.Location = new System.Drawing.Point(873, 1);
            this.ProcessLbl.Name = "ProcessLbl";
            this.ProcessLbl.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.ProcessLbl.Size = new System.Drawing.Size(58, 23);
            this.ProcessLbl.TabIndex = 2;
            this.ProcessLbl.Text = "50%";
            this.ProcessLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FlowLayout
            // 
            this.FlowLayout.ColumnCount = 3;
            this.FlowLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FlowLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.FlowLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.FlowLayout.Controls.Add(this.NextButton, 2, 0);
            this.FlowLayout.Controls.Add(this.PreviousButton, 1, 0);
            this.FlowLayout.Controls.Add(this.ProgressBar, 0, 0);
            this.FlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlowLayout.Location = new System.Drawing.Point(0, 629);
            this.FlowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.FlowLayout.Name = "FlowLayout";
            this.FlowLayout.RowCount = 1;
            this.FlowLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FlowLayout.Size = new System.Drawing.Size(934, 35);
            this.FlowLayout.TabIndex = 1;
            // 
            // NextButton
            // 
            this.NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.NextButton.Location = new System.Drawing.Point(837, 4);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(94, 27);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // PreviousButton
            // 
            this.PreviousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviousButton.Location = new System.Drawing.Point(737, 4);
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new System.Drawing.Size(94, 27);
            this.PreviousButton.TabIndex = 1;
            this.PreviousButton.Text = "Previous";
            this.PreviousButton.UseVisualStyleBackColor = true;
            this.PreviousButton.Click += new System.EventHandler(this.PreviousButton_Click);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(3, 5);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(728, 25);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 2;
            // 
            // FlowGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 664);
            this.Controls.Add(this.Layout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FlowGUI";
            this.Text = "FlowGUI";
            this.Layout.ResumeLayout(false);
            this.InfoLayout.ResumeLayout(false);
            this.InfoLayout.PerformLayout();
            this.FlowLayout.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		private System.Windows.Forms.TableLayoutPanel InfoLayout;
		private System.Windows.Forms.TableLayoutPanel FlowLayout;
		private System.Windows.Forms.Button NextButton;
		private System.Windows.Forms.Button PreviousButton;
		private System.Windows.Forms.ProgressBar ProgressBar;
		private System.Windows.Forms.Label InfoLbl;
		private System.Windows.Forms.Label ProcessInfoLbl;
		private System.Windows.Forms.Label ProcessLbl;
	}
}