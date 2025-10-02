namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	partial class OffsetControler
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
            this.TableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.InfoLbl = new System.Windows.Forms.Label();
            this.TableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayout
            // 
            this.TableLayout.ColumnCount = 1;
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayout.Controls.Add(this.FlowPanel, 0, 1);
            this.TableLayout.Controls.Add(this.InfoLbl, 0, 0);
            this.TableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.TableLayout.Location = new System.Drawing.Point(0, 0);
            this.TableLayout.Margin = new System.Windows.Forms.Padding(3, 25, 3, 10);
            this.TableLayout.Name = "Layout";
            this.TableLayout.RowCount = 2;
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayout.Size = new System.Drawing.Size(925, 604);
            this.TableLayout.TabIndex = 0;
            // 
            // FlowPanel
            // 
            this.FlowPanel.AutoScroll = true;
            this.FlowPanel.AutoScrollMargin = new System.Drawing.Size(8, 8);
            this.FlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlowPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FlowPanel.Location = new System.Drawing.Point(0, 49);
            this.FlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FlowPanel.Name = "FlowPanel";
            this.FlowPanel.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.FlowPanel.Size = new System.Drawing.Size(925, 555);
            this.FlowPanel.TabIndex = 0;
            this.FlowPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.FlowPanel_Paint);
            // 
            // InfoLbl
            // 
            this.InfoLbl.AutoSize = true;
            this.InfoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoLbl.Location = new System.Drawing.Point(3, 0);
            this.InfoLbl.Name = "InfoLbl";
            this.InfoLbl.Size = new System.Drawing.Size(925, 40);
            this.InfoLbl.TabIndex = 1;
            this.InfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OffsetControler
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.TableLayout);
            this.Name = "OffsetControler";
            this.Size = new System.Drawing.Size(925, 604);
            this.TableLayout.ResumeLayout(false);
            this.TableLayout.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel TableLayout;
		public System.Windows.Forms.FlowLayoutPanel FlowPanel;
		private System.Windows.Forms.Label InfoLbl;
	}
}
