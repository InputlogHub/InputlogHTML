using System.Collections.Generic;
using InputLog.Core.Events;

namespace GUI.Tabs.Preprocess.Filters
{
    partial class Time
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.SummaryPanel = new System.Windows.Forms.Panel();
            this.configSaveBtn = new System.Windows.Forms.Button();
            this.EditConfiguration = new System.Windows.Forms.Button();
            this.openConfigFile = new System.Windows.Forms.Button();
            this.InitialIDPanel = new System.Windows.Forms.Panel();
            this.infoLbl = new System.Windows.Forms.Label();
            this.AutoBtn = new System.Windows.Forms.Button();
            this.ManualBtn = new System.Windows.Forms.Button();
            this.Summary = new GUI.Tabs.Preprocess.Filters.TimeFilterHelp.TimeConfigurationInfoPanel();
            this.configFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveConfigDialog = new System.Windows.Forms.SaveFileDialog();
            this.SummaryPanel.SuspendLayout();
            this.InitialIDPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(3, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(145, 25);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "ID && Time Filter";
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Enabled = false;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(181, 11);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.MoreInfoLabel.TabIndex = 1;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More Info";
            // 
            // SummaryPanel
            // 
            this.SummaryPanel.Controls.Add(this.InitialIDPanel);
            this.SummaryPanel.Controls.Add(this.Summary);
            this.SummaryPanel.Controls.Add(this.EditConfiguration);
            this.SummaryPanel.Controls.Add(this.openConfigFile);
            this.SummaryPanel.Controls.Add(this.configSaveBtn);
            this.SummaryPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SummaryPanel.Location = new System.Drawing.Point(0, 50);
            this.SummaryPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SummaryPanel.Name = "SummaryPanel";
            this.SummaryPanel.Size = new System.Drawing.Size(580, 139);
            this.SummaryPanel.TabIndex = 26;
            // 
            // configSaveBtn
            // 
            this.configSaveBtn.Location = new System.Drawing.Point(253, 109);
            this.configSaveBtn.Margin = new System.Windows.Forms.Padding(4);
            this.configSaveBtn.Name = "configSaveBtn";
            this.configSaveBtn.Size = new System.Drawing.Size(140, 30);
            this.configSaveBtn.TabIndex = 28;
            this.configSaveBtn.Text = "Save Configuration";
            this.configSaveBtn.UseVisualStyleBackColor = true;
            this.configSaveBtn.Click += new System.EventHandler(this.ConfigSaveBtnClick);
            // 
            // EditConfiguration
            // 
            this.EditConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.EditConfiguration.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EditConfiguration.Location = new System.Drawing.Point(8, 109);
            this.EditConfiguration.Margin = new System.Windows.Forms.Padding(4);
            this.EditConfiguration.Name = "EditConfiguration";
            this.EditConfiguration.Size = new System.Drawing.Size(89, 30);
            this.EditConfiguration.TabIndex = 27;
            this.EditConfiguration.Text = "Configure";
            this.EditConfiguration.UseVisualStyleBackColor = true;
            this.EditConfiguration.Click += new System.EventHandler(this.EditConfigurationButtonClick);
            // 
            // openConfigFile
            // 
            this.openConfigFile.Location = new System.Drawing.Point(105, 109);
            this.openConfigFile.Margin = new System.Windows.Forms.Padding(4);
            this.openConfigFile.Name = "openConfigFile";
            this.openConfigFile.Size = new System.Drawing.Size(140, 30);
            this.openConfigFile.TabIndex = 1;
            this.openConfigFile.Text = "Load Configuration";
            this.openConfigFile.UseVisualStyleBackColor = true;
            this.openConfigFile.Click += new System.EventHandler(this.OpenConfigFileClick);
            // 
            // InitialIDPanel
            // 
            this.InitialIDPanel.Controls.Add(this.infoLbl);
            this.InitialIDPanel.Controls.Add(this.AutoBtn);
            this.InitialIDPanel.Controls.Add(this.ManualBtn);
            this.InitialIDPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InitialIDPanel.Location = new System.Drawing.Point(0, 0);
            this.InitialIDPanel.Name = "InitialIDPanel";
            this.InitialIDPanel.Size = new System.Drawing.Size(580, 139);
            this.InitialIDPanel.TabIndex = 29;
            // 
            // infoLbl
            // 
            this.infoLbl.AutoSize = true;
            this.infoLbl.Location = new System.Drawing.Point(5, 28);
            this.infoLbl.Name = "infoLbl";
            this.infoLbl.Size = new System.Drawing.Size(303, 17);
            this.infoLbl.TabIndex = 2;
            this.infoLbl.Text = "Change the Start and/or End ID of the IDFX-file";
            // 
            // AutoBtn
            // 
            this.AutoBtn.Location = new System.Drawing.Point(114, 72);
            this.AutoBtn.Name = "AutoBtn";
            this.AutoBtn.Size = new System.Drawing.Size(100, 30);
            this.AutoBtn.TabIndex = 1;
            this.AutoBtn.Text = "Automatic";
            this.AutoBtn.UseVisualStyleBackColor = true;
            this.AutoBtn.Click += new System.EventHandler(this.AutoBtnClick);
            // 
            // ManualBtn
            // 
            this.ManualBtn.Location = new System.Drawing.Point(8, 72);
            this.ManualBtn.Name = "ManualBtn";
            this.ManualBtn.Size = new System.Drawing.Size(100, 30);
            this.ManualBtn.TabIndex = 0;
            this.ManualBtn.Text = "Manual";
            this.ManualBtn.UseVisualStyleBackColor = true;
            this.ManualBtn.Click += new System.EventHandler(this.ManualBtnClick);
            // 
            // Summary
            // 
            this.Summary.BackColor = System.Drawing.Color.Transparent;
            this.Summary.Dock = System.Windows.Forms.DockStyle.Top;
            this.Summary.Location = new System.Drawing.Point(0, 0);
            this.Summary.Margin = new System.Windows.Forms.Padding(0);
            this.Summary.Name = "Summary";
            this.Summary.Size = new System.Drawing.Size(580, 139);
            this.Summary.TabIndex = 0;
            // 
            // configFileDialog
            // 
            this.configFileDialog.DefaultExt = "txt";
            this.configFileDialog.FileName = "timeConfig";
            this.configFileDialog.Filter = "Text files|*.txt";
            // 
            // saveConfigDialog
            // 
            this.saveConfigDialog.DefaultExt = "txt";
            this.saveConfigDialog.FileName = "timeConfig";
            this.saveConfigDialog.Filter = "Text files|*.txt";
            // 
            // Time
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SummaryPanel);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.nameLabel);
            this.Name = "Time";
            this.Size = new System.Drawing.Size(580, 189);
            this.SummaryPanel.ResumeLayout(false);
            this.InitialIDPanel.ResumeLayout(false);
            this.InitialIDPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
		public System.Windows.Forms.Panel SummaryPanel;
		private TimeFilterHelp.TimeConfigurationInfoPanel Summary;
		private System.Windows.Forms.Button EditConfiguration;
        private System.Windows.Forms.Button openConfigFile;
        private System.Windows.Forms.OpenFileDialog configFileDialog;
        private System.Windows.Forms.Button configSaveBtn;
        private System.Windows.Forms.SaveFileDialog saveConfigDialog;
        private System.Windows.Forms.Panel InitialIDPanel;
        private System.Windows.Forms.Label infoLbl;
        private System.Windows.Forms.Button AutoBtn;
        private System.Windows.Forms.Button ManualBtn;
    }
}
