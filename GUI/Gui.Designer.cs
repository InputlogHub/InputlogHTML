using GUI.Tabs.Analyze;

namespace GUI
{
    partial class Gui
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
                this.NotifyIcon.Visible = false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gui));
            this.InputlogLogo = new System.Windows.Forms.PictureBox();
            this.tabImages = new System.Windows.Forms.ImageList(this.components);
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenButton = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentFilesButton = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.QuitButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AccountSettingsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.myAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copytaskIDFXManToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copytaskCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logfileNameRestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDefaultValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.inputlogHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputlogTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputlogOnTheWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputlogManualpdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutInputlogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenuRecordItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPlay = new System.Windows.Forms.TabPage();
            this.PlayTab = new GUI.Tabs.Replay.Play();
            this.tabAnalyze = new System.Windows.Forms.TabPage();
            this.AnalyzeTab = new GUI.Tabs.Analyze.Analyze();
            this.tabRecord = new System.Windows.Forms.TabPage();
            this.RecordTab = new GUI.Tabs.Record.Record();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.tabPreprocess = new System.Windows.Forms.TabPage();
            this.PreprocessTab = new GUI.Tabs.Preprocess.Preprocess();
            this.tabMerge = new System.Windows.Forms.TabPage();
            this.PostprocessTab = new GUI.Tabs.Postprocess.Postprocess();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.inspectTemplateErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.InputlogLogo)).BeginInit();
            this.MenuBar.SuspendLayout();
            this.NotifyContextMenuStrip.SuspendLayout();
            this.tabPlay.SuspendLayout();
            this.tabAnalyze.SuspendLayout();
            this.tabRecord.SuspendLayout();
            this.Tabs.SuspendLayout();
            this.tabPreprocess.SuspendLayout();
            this.tabMerge.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputlogLogo
            // 
            resources.ApplyResources(this.InputlogLogo, "InputlogLogo");
            this.InputlogLogo.BackColor = System.Drawing.Color.Transparent;
            this.InputlogLogo.Image = global::GUI.Properties.Resources.InputLogLogo;
            this.InputlogLogo.Name = "InputlogLogo";
            this.InputlogLogo.TabStop = false;
            // 
            // tabImages
            // 
            this.tabImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.tabImages, "tabImages");
            this.tabImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MenuBar
            // 
            this.MenuBar.BackColor = System.Drawing.Color.Transparent;
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.ToolMenu,
            this.toolsToolStripMenuItem,
            this.HelpMenu});
            resources.ApplyResources(this.MenuBar, "MenuBar");
            this.MenuBar.Name = "MenuBar";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenButton,
            this.RecentFilesButton,
            this.OptionsButton,
            this.toolStripMenuItem1,
            this.QuitButton});
            this.FileMenu.Name = "FileMenu";
            resources.ApplyResources(this.FileMenu, "FileMenu");
            // 
            // OpenButton
            // 
            this.OpenButton.Name = "OpenButton";
            resources.ApplyResources(this.OpenButton, "OpenButton");
            this.OpenButton.Click += new System.EventHandler(this.OpenButtonClick);
            // 
            // RecentFilesButton
            // 
            this.RecentFilesButton.Name = "RecentFilesButton";
            resources.ApplyResources(this.RecentFilesButton, "RecentFilesButton");
            // 
            // OptionsButton
            // 
            this.OptionsButton.Name = "OptionsButton";
            resources.ApplyResources(this.OptionsButton, "OptionsButton");
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButtonClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // QuitButton
            // 
            this.QuitButton.Name = "QuitButton";
            resources.ApplyResources(this.QuitButton, "QuitButton");
            this.QuitButton.Click += new System.EventHandler(this.QuitButtonClick);
            // 
            // ToolMenu
            // 
            this.ToolMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AccountSettingsButton,
            this.myAccountToolStripMenuItem});
            this.ToolMenu.Name = "ToolMenu";
            resources.ApplyResources(this.ToolMenu, "ToolMenu");
            // 
            // AccountSettingsButton
            // 
            this.AccountSettingsButton.Name = "AccountSettingsButton";
            resources.ApplyResources(this.AccountSettingsButton, "AccountSettingsButton");
            this.AccountSettingsButton.Click += new System.EventHandler(this.AccountSettingsButtonClick);
            // 
            // myAccountToolStripMenuItem
            // 
            this.myAccountToolStripMenuItem.Name = "myAccountToolStripMenuItem";
            resources.ApplyResources(this.myAccountToolStripMenuItem, "myAccountToolStripMenuItem");
            this.myAccountToolStripMenuItem.Click += new System.EventHandler(this.MyAccountToolStripMenuItemClick);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copytaskIDFXManToolStripMenuItem,
            this.copytaskCreatorToolStripMenuItem,
            this.logfileNameRestoreToolStripMenuItem,
            this.reportingToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // copytaskIDFXManToolStripMenuItem
            // 
            this.copytaskIDFXManToolStripMenuItem.Name = "copytaskIDFXManToolStripMenuItem";
            resources.ApplyResources(this.copytaskIDFXManToolStripMenuItem, "copytaskIDFXManToolStripMenuItem");
            this.copytaskIDFXManToolStripMenuItem.Click += new System.EventHandler(this.CopytaskIDFXManToolStripMenuItemClick);
            // 
            // copytaskCreatorToolStripMenuItem
            // 
            this.copytaskCreatorToolStripMenuItem.Name = "copytaskCreatorToolStripMenuItem";
            resources.ApplyResources(this.copytaskCreatorToolStripMenuItem, "copytaskCreatorToolStripMenuItem");
            this.copytaskCreatorToolStripMenuItem.Click += new System.EventHandler(this.CopytaskCreatorToolStripMenuItemClick);
            // 
            // logfileNameRestoreToolStripMenuItem
            // 
            this.logfileNameRestoreToolStripMenuItem.Name = "logfileNameRestoreToolStripMenuItem";
            resources.ApplyResources(this.logfileNameRestoreToolStripMenuItem, "logfileNameRestoreToolStripMenuItem");
            this.logfileNameRestoreToolStripMenuItem.Click += new System.EventHandler(this.LogfileNameRestoreToolStripMenuItemClick);
            // 
            // reportingToolStripMenuItem
            // 
            this.reportingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDefaultValuesToolStripMenuItem,
            this.inspectTemplateErrorsToolStripMenuItem});
            this.reportingToolStripMenuItem.Name = "reportingToolStripMenuItem";
            resources.ApplyResources(this.reportingToolStripMenuItem, "reportingToolStripMenuItem");
            // 
            // editDefaultValuesToolStripMenuItem
            // 
            this.editDefaultValuesToolStripMenuItem.Name = "editDefaultValuesToolStripMenuItem";
            resources.ApplyResources(this.editDefaultValuesToolStripMenuItem, "editDefaultValuesToolStripMenuItem");
            this.editDefaultValuesToolStripMenuItem.Click += new System.EventHandler(this.editDefaultValuesToolStripMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inputlogHelpToolStripMenuItem,
            this.inputlogTourToolStripMenuItem,
            this.inputlogOnTheWebToolStripMenuItem,
            this.inputlogManualpdfToolStripMenuItem,
            this.toolStripMenuItem2,
            this.checkForUpdatesToolStripMenuItem,
            this.aboutInputlogToolStripMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            resources.ApplyResources(this.HelpMenu, "HelpMenu");
            // 
            // inputlogHelpToolStripMenuItem
            // 
            this.inputlogHelpToolStripMenuItem.Name = "inputlogHelpToolStripMenuItem";
            resources.ApplyResources(this.inputlogHelpToolStripMenuItem, "inputlogHelpToolStripMenuItem");
            this.inputlogHelpToolStripMenuItem.Click += new System.EventHandler(this.InputlogHelpToolStripMenuItemClick);
            // 
            // inputlogTourToolStripMenuItem
            // 
            this.inputlogTourToolStripMenuItem.Name = "inputlogTourToolStripMenuItem";
            resources.ApplyResources(this.inputlogTourToolStripMenuItem, "inputlogTourToolStripMenuItem");
            this.inputlogTourToolStripMenuItem.Click += new System.EventHandler(this.InputlogTourToolStripMenuItemClick);
            // 
            // inputlogOnTheWebToolStripMenuItem
            // 
            this.inputlogOnTheWebToolStripMenuItem.Name = "inputlogOnTheWebToolStripMenuItem";
            resources.ApplyResources(this.inputlogOnTheWebToolStripMenuItem, "inputlogOnTheWebToolStripMenuItem");
            this.inputlogOnTheWebToolStripMenuItem.Click += new System.EventHandler(this.InputlogOnTheWebToolStripMenuItemClick);
            // 
            // inputlogManualpdfToolStripMenuItem
            // 
            this.inputlogManualpdfToolStripMenuItem.Name = "inputlogManualpdfToolStripMenuItem";
            resources.ApplyResources(this.inputlogManualpdfToolStripMenuItem, "inputlogManualpdfToolStripMenuItem");
            this.inputlogManualpdfToolStripMenuItem.Click += new System.EventHandler(this.InputlogManualpdfToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItemClick);
            // 
            // aboutInputlogToolStripMenuItem
            // 
            this.aboutInputlogToolStripMenuItem.Name = "aboutInputlogToolStripMenuItem";
            resources.ApplyResources(this.aboutInputlogToolStripMenuItem, "aboutInputlogToolStripMenuItem");
            this.aboutInputlogToolStripMenuItem.Click += new System.EventHandler(this.AboutInputlogToolStripMenuItemClick);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.ContextMenuStrip = this.NotifyContextMenuStrip;
            resources.ApplyResources(this.NotifyIcon, "NotifyIcon");
            this.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMouseClick);
            // 
            // NotifyContextMenuStrip
            // 
            this.NotifyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenuRecordItem});
            this.NotifyContextMenuStrip.Name = "NotifyContextMenuStrip";
            resources.ApplyResources(this.NotifyContextMenuStrip, "NotifyContextMenuStrip");
            // 
            // ContextMenuRecordItem
            // 
            this.ContextMenuRecordItem.Name = "ContextMenuRecordItem";
            resources.ApplyResources(this.ContextMenuRecordItem, "ContextMenuRecordItem");
            this.ContextMenuRecordItem.Click += new System.EventHandler(this.ContextMenuRecordItemClick);
            // 
            // tabPlay
            // 
            this.tabPlay.Controls.Add(this.PlayTab);
            resources.ApplyResources(this.tabPlay, "tabPlay");
            this.tabPlay.Name = "tabPlay";
            // 
            // PlayTab
            // 
            resources.ApplyResources(this.PlayTab, "PlayTab");
            this.PlayTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.PlayTab.Name = "PlayTab";
            // 
            // tabAnalyze
            // 
            this.tabAnalyze.Controls.Add(this.AnalyzeTab);
            resources.ApplyResources(this.tabAnalyze, "tabAnalyze");
            this.tabAnalyze.Name = "tabAnalyze";
            this.tabAnalyze.UseVisualStyleBackColor = true;
            // 
            // AnalyzeTab
            // 
            resources.ApplyResources(this.AnalyzeTab, "AnalyzeTab");
            this.AnalyzeTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.AnalyzeTab.Name = "AnalyzeTab";
            // 
            // tabRecord
            // 
            this.tabRecord.BackColor = System.Drawing.Color.Transparent;
            this.tabRecord.Controls.Add(this.RecordTab);
            resources.ApplyResources(this.tabRecord, "tabRecord");
            this.tabRecord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.tabRecord.Name = "tabRecord";
            // 
            // RecordTab
            // 
            resources.ApplyResources(this.RecordTab, "RecordTab");
            this.RecordTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.RecordTab.ForeColor = System.Drawing.SystemColors.MenuText;
            this.RecordTab.Name = "RecordTab";
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.tabRecord);
            this.Tabs.Controls.Add(this.tabPreprocess);
            this.Tabs.Controls.Add(this.tabAnalyze);
            this.Tabs.Controls.Add(this.tabMerge);
            this.Tabs.Controls.Add(this.tabPlay);
            resources.ApplyResources(this.Tabs, "Tabs");
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            // 
            // tabPreprocess
            // 
            this.tabPreprocess.Controls.Add(this.PreprocessTab);
            resources.ApplyResources(this.tabPreprocess, "tabPreprocess");
            this.tabPreprocess.Name = "tabPreprocess";
            this.tabPreprocess.UseVisualStyleBackColor = true;
            // 
            // PreprocessTab
            // 
            resources.ApplyResources(this.PreprocessTab, "PreprocessTab");
            this.PreprocessTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.PreprocessTab.Name = "PreprocessTab";
            // 
            // tabMerge
            // 
            this.tabMerge.BackColor = System.Drawing.Color.Transparent;
            this.tabMerge.Controls.Add(this.PostprocessTab);
            resources.ApplyResources(this.tabMerge, "tabMerge");
            this.tabMerge.Name = "tabMerge";
            this.tabMerge.UseVisualStyleBackColor = true;
            // 
            // PostprocessTab
            // 
            resources.ApplyResources(this.PostprocessTab, "PostprocessTab");
            this.PostprocessTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.PostprocessTab.Name = "PostprocessTab";
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "openFileDialog1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // inspectTemplateErrorsToolStripMenuItem
            // 
            this.inspectTemplateErrorsToolStripMenuItem.Name = "inspectTemplateErrorsToolStripMenuItem";
            resources.ApplyResources(this.inspectTemplateErrorsToolStripMenuItem, "inspectTemplateErrorsToolStripMenuItem");
            this.inspectTemplateErrorsToolStripMenuItem.Click += new System.EventHandler(this.inspectTemplateErrorsToolStripMenuItem_Click);
            // 
            // Gui
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(227)))), ((int)(((byte)(237)))));
            this.Controls.Add(this.InputlogLogo);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.MenuBar);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.MenuBar;
            this.Name = "Gui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GuiFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GuiFormClosed);
            this.Load += new System.EventHandler(this.GuiLoad);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.GuiDragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.InputlogLogo)).EndInit();
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.NotifyContextMenuStrip.ResumeLayout(false);
            this.tabPlay.ResumeLayout(false);
            this.tabPlay.PerformLayout();
            this.tabAnalyze.ResumeLayout(false);
            this.tabAnalyze.PerformLayout();
            this.tabRecord.ResumeLayout(false);
            this.tabRecord.PerformLayout();
            this.Tabs.ResumeLayout(false);
            this.tabPreprocess.ResumeLayout(false);
            this.tabPreprocess.PerformLayout();
            this.tabMerge.ResumeLayout(false);
            this.tabMerge.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList tabImages;
        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem QuitButton;
        private System.Windows.Forms.PictureBox InputlogLogo;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.ContextMenuStrip NotifyContextMenuStrip;
        public System.Windows.Forms.ToolStripMenuItem ContextMenuRecordItem;
        private System.Windows.Forms.TabPage tabPlay;
        private Tabs.Replay.Play PlayTab;
        private System.Windows.Forms.TabPage tabAnalyze;
        private Analyze AnalyzeTab;
        private System.Windows.Forms.TabPage tabRecord;
        private Tabs.Record.Record RecordTab;
        public System.Windows.Forms.TabControl Tabs;
        public System.Windows.Forms.TabPage tabMerge;
        private Tabs.Postprocess.Postprocess PostprocessTab;
        private System.Windows.Forms.ToolStripMenuItem AccountSettingsButton;
        private System.Windows.Forms.ToolStripMenuItem OptionsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem myAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputlogHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputlogTourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputlogOnTheWebToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutInputlogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenButton;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.ToolStripMenuItem RecentFilesButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TabPage tabPreprocess;
        private Tabs.Preprocess.Preprocess PreprocessTab;
        private System.Windows.Forms.ToolStripMenuItem inputlogManualpdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytaskIDFXManToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytaskCreatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logfileNameRestoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDefaultValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectTemplateErrorsToolStripMenuItem;
    }
}

