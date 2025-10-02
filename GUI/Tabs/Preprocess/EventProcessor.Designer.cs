namespace GUI.Tabs.Preprocess
{
	partial class EventProcessor
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
            this.Panel = new System.Windows.Forms.FlowLayoutPanel();
            this.AddButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.UpButton = new System.Windows.Forms.Button();
            this.DropDown = new System.Windows.Forms.ComboBox();
            this.Layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.ColumnCount = 4;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.Layout.Controls.Add(this.Panel, 0, 1);
            this.Layout.Controls.Add(this.AddButton, 3, 0);
            this.Layout.Controls.Add(this.DownButton, 2, 0);
            this.Layout.Controls.Add(this.UpButton, 1, 0);
            this.Layout.Controls.Add(this.DropDown, 0, 0);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Layout.Name = "Layout";
            this.Layout.RowCount = 2;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Layout.Size = new System.Drawing.Size(428, 221);
            this.Layout.TabIndex = 0;
            // 
            // Panel
            // 
            this.Panel.AutoScroll = true;
            this.Panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Layout.SetColumnSpan(this.Panel, 4);
            this.Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Panel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Panel.Location = new System.Drawing.Point(0, 37);
            this.Panel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(428, 181);
            this.Panel.TabIndex = 57;
            this.Panel.WrapContents = false;
            // 
            // AddButton
            // 
            this.AddButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AddButton.Image = global::GUI.Properties.Resources.add;
            this.AddButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddButton.Location = new System.Drawing.Point(333, 3);
            this.AddButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(95, 28);
            this.AddButton.TabIndex = 56;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // DownButton
            // 
            this.DownButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DownButton.Image = global::GUI.Properties.Resources.down;
            this.DownButton.Location = new System.Drawing.Point(283, 3);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(44, 28);
            this.DownButton.TabIndex = 55;
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButtonClick);
            // 
            // UpButton
            // 
            this.UpButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpButton.Image = global::GUI.Properties.Resources.up;
            this.UpButton.Location = new System.Drawing.Point(233, 3);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(44, 28);
            this.UpButton.TabIndex = 54;
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButtonClick);
            // 
            // DropDown
            // 
            this.DropDown.BackColor = System.Drawing.SystemColors.Window;
            this.DropDown.Dock = System.Windows.Forms.DockStyle.Left;
            this.DropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DropDown.FormattingEnabled = true;
            this.DropDown.Location = new System.Drawing.Point(0, 6);
            this.DropDown.Margin = new System.Windows.Forms.Padding(0, 6, 0, 3);
            this.DropDown.Name = "DropDown";
            this.DropDown.Size = new System.Drawing.Size(220, 21);
            this.DropDown.TabIndex = 50;
            // 
            // EventProcessor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Layout);
            this.Margin = new System.Windows.Forms.Padding(6, 3, 4, 3);
            this.Name = "EventProcessor";
            this.Size = new System.Drawing.Size(428, 221);
            this.EnabledChanged += new System.EventHandler(this.EventProcessorEnabledChanged);
            this.Layout.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private new System.Windows.Forms.TableLayoutPanel Layout;
		protected System.Windows.Forms.ComboBox DropDown;
		protected System.Windows.Forms.Button UpButton;
		protected System.Windows.Forms.Button DownButton;
		protected System.Windows.Forms.Button AddButton;
		protected System.Windows.Forms.FlowLayoutPanel Panel;
	}
}
