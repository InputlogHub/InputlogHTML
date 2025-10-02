namespace GUI.Tabs.Preprocess.Merge
{
	public partial class FileExtSelectionControl
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
			this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			// 
			// FlowPanel
			// 
			this.FlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.FlowPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.FlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FlowPanel.Location = new System.Drawing.Point(0, 0);
			this.FlowPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.FlowPanel.Name = "FlowPanel";
			this.FlowPanel.Size = new System.Drawing.Size(159, 18);
			this.FlowPanel.TabIndex = 0;
			// 
			// FileExtSelectionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.FlowPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "FileExtSelectionControl";
			this.Size = new System.Drawing.Size(159, 18);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.FlowLayoutPanel FlowPanel;

	}
}
