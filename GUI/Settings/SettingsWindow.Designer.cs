namespace GUI.Settings {
	partial class SettingsWindow {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.TabContainerPanel = new System.Windows.Forms.Panel();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.GeneralTab = new System.Windows.Forms.TabPage();
            this.GeneralSettings = new GUI.Settings.Tabs.General();
            this.LoggingTab = new System.Windows.Forms.TabPage();
            this.LoggingSettings = new GUI.Settings.Tabs.Logging();
            this.AnalysesTab = new System.Windows.Forms.TabPage();
            this.AnalysesSettings = new GUI.Settings.Tabs.Analyses();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ResetToDefaultButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.TabContainerPanel.SuspendLayout();
            this.Tabs.SuspendLayout();
            this.GeneralTab.SuspendLayout();
            this.LoggingTab.SuspendLayout();
            this.AnalysesTab.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CancelBtn.Location = new System.Drawing.Point(199, 0);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 9;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.OKBtn.Location = new System.Drawing.Point(118, 0);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 10;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKButtonClick);
            // 
            // TabContainerPanel
            // 
            this.TabContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabContainerPanel.Controls.Add(this.Tabs);
            this.TabContainerPanel.Location = new System.Drawing.Point(1, 0);
            this.TabContainerPanel.Name = "TabContainerPanel";
            this.TabContainerPanel.Size = new System.Drawing.Size(571, 512);
            this.TabContainerPanel.TabIndex = 11;
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.GeneralTab);
            this.Tabs.Controls.Add(this.LoggingTab);
            this.Tabs.Controls.Add(this.AnalysesTab);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(571, 512);
            this.Tabs.TabIndex = 0;
            // 
            // GeneralTab
            // 
            this.GeneralTab.Controls.Add(this.GeneralSettings);
            this.GeneralTab.Location = new System.Drawing.Point(4, 22);
            this.GeneralTab.Name = "GeneralTab";
            this.GeneralTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.GeneralTab.Size = new System.Drawing.Size(563, 486);
            this.GeneralTab.TabIndex = 0;
            this.GeneralTab.Text = "General";
            this.GeneralTab.UseVisualStyleBackColor = true;
            // 
            // GeneralSettings
            // 
            this.GeneralSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.GeneralSettings.GUI = null;
            this.GeneralSettings.Location = new System.Drawing.Point(2, 8);
            this.GeneralSettings.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.GeneralSettings.Name = "GeneralSettings";
            this.GeneralSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GeneralSettings.Size = new System.Drawing.Size(464, 331);
            this.GeneralSettings.TabIndex = 0;
            // 
            // LoggingTab
            // 
            this.LoggingTab.Controls.Add(this.LoggingSettings);
            this.LoggingTab.Location = new System.Drawing.Point(4, 22);
            this.LoggingTab.Name = "LoggingTab";
            this.LoggingTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.LoggingTab.Size = new System.Drawing.Size(563, 403);
            this.LoggingTab.TabIndex = 1;
            this.LoggingTab.Text = "Logging";
            this.LoggingTab.UseVisualStyleBackColor = true;
            // 
            // LoggingSettings
            // 
            this.LoggingSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoggingSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.LoggingSettings.GUI = null;
            this.LoggingSettings.Location = new System.Drawing.Point(3, 3);
            this.LoggingSettings.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.LoggingSettings.Name = "LoggingSettings";
            this.LoggingSettings.Size = new System.Drawing.Size(557, 397);
            this.LoggingSettings.TabIndex = 0;
            // 
            // AnalysesTab
            // 
            this.AnalysesTab.Controls.Add(this.AnalysesSettings);
            this.AnalysesTab.Location = new System.Drawing.Point(4, 22);
            this.AnalysesTab.Name = "AnalysesTab";
            this.AnalysesTab.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.AnalysesTab.Size = new System.Drawing.Size(563, 403);
            this.AnalysesTab.TabIndex = 2;
            this.AnalysesTab.Text = "Analyses";
            this.AnalysesTab.UseVisualStyleBackColor = true;
            // 
            // AnalysesSettings
            // 
            this.AnalysesSettings.AutoScroll = true;
            this.AnalysesSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnalysesSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AnalysesSettings.GUI = null;
            this.AnalysesSettings.Location = new System.Drawing.Point(3, 3);
            this.AnalysesSettings.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.AnalysesSettings.Name = "AnalysesSettings";
            this.AnalysesSettings.Size = new System.Drawing.Size(557, 397);
            this.AnalysesSettings.TabIndex = 0;
            // 
            // ResetToDefaultButton
            // 
            this.ResetToDefaultButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ResetToDefaultButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ResetToDefaultButton.Location = new System.Drawing.Point(3, 0);
            this.ResetToDefaultButton.Name = "ResetToDefaultButton";
            this.ResetToDefaultButton.Size = new System.Drawing.Size(83, 23);
            this.ResetToDefaultButton.TabIndex = 12;
            this.ResetToDefaultButton.Text = "Reset";
            this.ResetToDefaultButton.UseVisualStyleBackColor = true;
            this.ResetToDefaultButton.Click += new System.EventHandler(this.ResetToDefaultButtonClick);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(12, 526);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 13);
            this.StatusLabel.TabIndex = 13;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPanel.Controls.Add(this.ResetToDefaultButton);
            this.ButtonPanel.Controls.Add(this.OKBtn);
            this.ButtonPanel.Controls.Add(this.CancelBtn);
            this.ButtonPanel.Location = new System.Drawing.Point(2, 517);
            this.ButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(274, 23);
            this.ButtonPanel.TabIndex = 14;
            // 
            // SettingsWindow
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(572, 554);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.TabContainerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.TopMost = true;
            this.TabContainerPanel.ResumeLayout(false);
            this.Tabs.ResumeLayout(false);
            this.GeneralTab.ResumeLayout(false);
            this.LoggingTab.ResumeLayout(false);
            this.AnalysesTab.ResumeLayout(false);
            this.ButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Panel TabContainerPanel;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage GeneralTab;
        private System.Windows.Forms.TabPage LoggingTab;
        private System.Windows.Forms.TabPage AnalysesTab;
        private System.Windows.Forms.ToolTip ToolTip;
        private Tabs.General GeneralSettings;
        private Tabs.Logging LoggingSettings;
        private System.Windows.Forms.Button ResetToDefaultButton;
        private Tabs.Analyses AnalysesSettings;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Panel ButtonPanel;
	}
}