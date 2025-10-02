namespace GUI.Tabs.Preprocess.Filters.TimeFilterHelp
{
	partial class TimeConfigurationInfoPanel
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
            this.InfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.EndRightLbl = new System.Windows.Forms.Label();
            this.StartRightLbl = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.EmptyLabel = new System.Windows.Forms.Label();
            this.StartLeftTbx = new System.Windows.Forms.TextBox();
            this.EndLeftTbx = new System.Windows.Forms.TextBox();
            this.EndLeftLbl = new System.Windows.Forms.Label();
            this.StartLeftLbl = new System.Windows.Forms.Label();
            this.InfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // InfoPanel
            // 
            this.InfoPanel.ColumnCount = 3;
            this.InfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoPanel.Controls.Add(this.EndRightLbl, 2, 3);
            this.InfoPanel.Controls.Add(this.StartRightLbl, 2, 2);
            this.InfoPanel.Controls.Add(this.Title, 0, 0);
            this.InfoPanel.Controls.Add(this.EmptyLabel, 0, 1);
            this.InfoPanel.Controls.Add(this.StartLeftTbx, 1, 2);
            this.InfoPanel.Controls.Add(this.StartLeftLbl, 0, 2);
            this.InfoPanel.Controls.Add(this.EndLeftLbl, 0, 3);
            this.InfoPanel.Controls.Add(this.EndLeftTbx, 1, 3);
            this.InfoPanel.Location = new System.Drawing.Point(0, 0);
            this.InfoPanel.Margin = new System.Windows.Forms.Padding(0);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.RowCount = 4;
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.InfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.InfoPanel.Size = new System.Drawing.Size(548, 110);
            this.InfoPanel.TabIndex = 0;
            // 
            // EndRightLbl
            // 
            this.EndRightLbl.Enabled = false;
            this.EndRightLbl.Location = new System.Drawing.Point(229, 45);
            this.EndRightLbl.Margin = new System.Windows.Forms.Padding(7, 0, 11, 0);
            this.EndRightLbl.Name = "EndRightLbl";
            this.EndRightLbl.Size = new System.Drawing.Size(160, 30);
            this.EndRightLbl.TabIndex = 4;
            this.EndRightLbl.Text = "endRight";
            this.EndRightLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StartRightLbl
            // 
            this.StartRightLbl.Enabled = false;
            this.StartRightLbl.Location = new System.Drawing.Point(229, 17);
            this.StartRightLbl.Margin = new System.Windows.Forms.Padding(7, 0, 11, 0);
            this.StartRightLbl.Name = "StartRightLbl";
            this.StartRightLbl.Size = new System.Drawing.Size(160, 28);
            this.StartRightLbl.TabIndex = 3;
            this.StartRightLbl.Text = "startRight";
            this.StartRightLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.InfoPanel.SetColumnSpan(this.Title, 3);
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(4, 0);
            this.Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(40, 17);
            this.Title.TabIndex = 0;
            this.Title.Text = "Title";
            //
            // EmptyLabel
            //
            this.EmptyLabel.AutoSize = true;
            this.InfoPanel.SetColumnSpan(this.EmptyLabel, 3);
            this.EmptyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmptyLabel.Location = new System.Drawing.Point(4, 0);
            this.EmptyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.EmptyLabel.Name = "EmptyLabel";
            this.EmptyLabel.Size = new System.Drawing.Size(40, 17);
            this.EmptyLabel.TabIndex = 0;
            // 
            // StartLeftTbx
            // 
            this.StartLeftTbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.StartLeftTbx.Enabled = false;
            this.StartLeftTbx.Location = new System.Drawing.Point(111, 20);
            this.StartLeftTbx.Margin = new System.Windows.Forms.Padding(11, 0, 11, 0);
            this.StartLeftTbx.Name = "StartLeftTbx";
            this.StartLeftTbx.Size = new System.Drawing.Size(100, 22);
            this.StartLeftTbx.TabIndex = 5;
            // 
            // EndLeftTbx
            // 
            this.EndLeftTbx.Enabled = false;
            this.EndLeftTbx.Location = new System.Drawing.Point(111, 48);
            this.EndLeftTbx.Margin = new System.Windows.Forms.Padding(11, 3, 11, 0);
            this.EndLeftTbx.Name = "EndLeftTbx";
            this.EndLeftTbx.Size = new System.Drawing.Size(100, 22);
            this.EndLeftTbx.TabIndex = 7;
            // 
            // EndLeftLbl
            // 
            this.EndLeftLbl.Location = new System.Drawing.Point(3, 45);
            this.EndLeftLbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.EndLeftLbl.Name = "EndLeftLbl";
            this.EndLeftLbl.Size = new System.Drawing.Size(97, 30);
            this.EndLeftLbl.TabIndex = 2;
            this.EndLeftLbl.Text = "endLeft";
            this.EndLeftLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StartLeftLbl
            // 
            this.StartLeftLbl.Location = new System.Drawing.Point(3, 17);
            this.StartLeftLbl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.StartLeftLbl.Name = "StartLeftLbl";
            this.StartLeftLbl.Size = new System.Drawing.Size(83, 28);
            this.StartLeftLbl.TabIndex = 1;
            this.StartLeftLbl.Text = "startLeft";
            this.StartLeftLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TimeConfigurationInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.InfoPanel);
            this.Margin = new System.Windows.Forms.Padding(11, 4, 11, 10);
            this.Name = "TimeConfigurationInfoPanel";
            this.Size = new System.Drawing.Size(400, 110);
            this.InfoPanel.ResumeLayout(false);
            this.InfoPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.TableLayoutPanel InfoPanel;
		protected System.Windows.Forms.Label EndRightLbl;
		protected System.Windows.Forms.Label StartRightLbl;
		protected System.Windows.Forms.Label EndLeftLbl;
		protected System.Windows.Forms.Label Title;
        protected System.Windows.Forms.Label EmptyLabel;
		protected System.Windows.Forms.Label StartLeftLbl;
		protected System.Windows.Forms.TextBox StartLeftTbx;
		protected System.Windows.Forms.TextBox EndLeftTbx;
	}
}
