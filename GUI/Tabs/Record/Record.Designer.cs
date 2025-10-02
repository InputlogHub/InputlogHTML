using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GUI.Tabs.Record.Plugin;

namespace GUI.Tabs.Record
{
    partial class Record
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.Recording)
            {
                this.StopLogging();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            foreach (AbstractPlugin plugin in this._settings.Plugins)
            {
                plugin.Dispose();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.RecordPanel = new System.Windows.Forms.Panel();
            this.ALGroupbox = new System.Windows.Forms.GroupBox();
            this.AL_CopytaskRBTN = new System.Windows.Forms.RadioButton();
            this.AL_NoneRBTN = new System.Windows.Forms.RadioButton();
            this.RecordSessionIdentification = new System.Windows.Forms.DataGridView();
            this.colKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordSessionIdentificationLabel = new System.Windows.Forms.Label();
            this.RecordPluginOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.OpenWithDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RecordButton = new System.Windows.Forms.Button();
            this.ScriptBox = new System.Windows.Forms.GroupBox();
            this.radioScriptChinese = new System.Windows.Forms.RadioButton();
            this.radioScriptLatin = new System.Windows.Forms.RadioButton();
            this.RecordPanel.SuspendLayout();
            this.ALGroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RecordSessionIdentification)).BeginInit();
            this.panel1.SuspendLayout();
            this.ScriptBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // RecordPanel
            // 
            this.RecordPanel.AutoSize = true;
            this.RecordPanel.Controls.Add(this.ALGroupbox);
            this.RecordPanel.Controls.Add(this.RecordSessionIdentification);
            this.RecordPanel.Controls.Add(this.RecordSessionIdentificationLabel);
            this.RecordPanel.Controls.Add(this.RecordPluginOptions);
            this.RecordPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.RecordPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.RecordPanel.Location = new System.Drawing.Point(13, 13);
            this.RecordPanel.Margin = new System.Windows.Forms.Padding(0);
            this.RecordPanel.Name = "RecordPanel";
            this.RecordPanel.Size = new System.Drawing.Size(857, 315);
            this.RecordPanel.TabIndex = 1;
            // 
            // ALGroupbox
            // 
            this.ALGroupbox.Controls.Add(this.AL_CopytaskRBTN);
            this.ALGroupbox.Controls.Add(this.AL_NoneRBTN);
            this.ALGroupbox.Location = new System.Drawing.Point(0, 244);
            this.ALGroupbox.Name = "ALGroupbox";
            this.ALGroupbox.Size = new System.Drawing.Size(500, 68);
            this.ALGroupbox.TabIndex = 26;
            this.ALGroupbox.TabStop = false;
            this.ALGroupbox.Text = "Other Logging";
            // 
            // AL_CopytaskRBTN
            // 
            this.AL_CopytaskRBTN.AutoSize = true;
            this.AL_CopytaskRBTN.Location = new System.Drawing.Point(7, 43);
            this.AL_CopytaskRBTN.Name = "AL_CopytaskRBTN";
            this.AL_CopytaskRBTN.Size = new System.Drawing.Size(72, 17);
            this.AL_CopytaskRBTN.TabIndex = 1;
            this.AL_CopytaskRBTN.Text = "Copy task";
            this.AL_CopytaskRBTN.UseVisualStyleBackColor = true;
            this.AL_CopytaskRBTN.CheckedChanged += new System.EventHandler(this.AL_CopytaskRBTN_CheckedChanged);
            // 
            // AL_NoneRBTN
            // 
            this.AL_NoneRBTN.AutoSize = true;
            this.AL_NoneRBTN.Checked = true;
            this.AL_NoneRBTN.Location = new System.Drawing.Point(7, 20);
            this.AL_NoneRBTN.Name = "AL_NoneRBTN";
            this.AL_NoneRBTN.Size = new System.Drawing.Size(51, 17);
            this.AL_NoneRBTN.TabIndex = 0;
            this.AL_NoneRBTN.TabStop = true;
            this.AL_NoneRBTN.Text = "None";
            this.AL_NoneRBTN.UseVisualStyleBackColor = true;
            this.AL_NoneRBTN.CheckedChanged += new System.EventHandler(this.AL_NoneRBTN_CheckedChanged);
            // 
            // RecordSessionIdentification
            // 
            this.RecordSessionIdentification.BackgroundColor = System.Drawing.SystemColors.Window;
            this.RecordSessionIdentification.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.RecordSessionIdentification.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.RecordSessionIdentification.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RecordSessionIdentification.ColumnHeadersVisible = false;
            this.RecordSessionIdentification.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colKey,
            this.Value});
            this.RecordSessionIdentification.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.RecordSessionIdentification.EnableHeadersVisualStyles = false;
            this.RecordSessionIdentification.Location = new System.Drawing.Point(614, 32);
            this.RecordSessionIdentification.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.RecordSessionIdentification.Name = "RecordSessionIdentification";
            this.RecordSessionIdentification.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.RecordSessionIdentification.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.RecordSessionIdentification.RowHeadersVisible = false;
            this.RecordSessionIdentification.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.RecordSessionIdentification.RowTemplate.Height = 24;
            this.RecordSessionIdentification.Size = new System.Drawing.Size(225, 218);
            this.RecordSessionIdentification.TabIndex = 24;
            // 
            // colKey
            // 
            this.colKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colKey.HeaderText = "Key";
            this.colKey.Name = "colKey";
            this.colKey.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // RecordSessionIdentificationLabel
            // 
            this.RecordSessionIdentificationLabel.AutoSize = true;
            this.RecordSessionIdentificationLabel.ForeColor = System.Drawing.SystemColors.MenuText;
            this.RecordSessionIdentificationLabel.Location = new System.Drawing.Point(611, 0);
            this.RecordSessionIdentificationLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.RecordSessionIdentificationLabel.Name = "RecordSessionIdentificationLabel";
            this.RecordSessionIdentificationLabel.Size = new System.Drawing.Size(107, 13);
            this.RecordSessionIdentificationLabel.TabIndex = 25;
            this.RecordSessionIdentificationLabel.Text = "Session Identification";
            // 
            // RecordPluginOptions
            // 
            this.RecordPluginOptions.AutoSize = true;
            this.RecordPluginOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.RecordPluginOptions.Location = new System.Drawing.Point(0, 0);
            this.RecordPluginOptions.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.RecordPluginOptions.MaximumSize = new System.Drawing.Size(500, 0);
            this.RecordPluginOptions.MinimumSize = new System.Drawing.Size(500, 0);
            this.RecordPluginOptions.Name = "RecordPluginOptions";
            this.RecordPluginOptions.Size = new System.Drawing.Size(500, 26);
            this.RecordPluginOptions.TabIndex = 15;
            // 
            // OpenWithDialog
            // 
            this.OpenWithDialog.Filter = "Applications|*.exe";
            this.OpenWithDialog.InitialDirectory = "\"C:\\\\Program Files\\\"";
            this.OpenWithDialog.Title = "Choose the application to open the logfile with";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RecordButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(13, 399);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(857, 90);
            this.panel1.TabIndex = 3;
            // 
            // RecordButton
            // 
            this.RecordButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.RecordButton.Location = new System.Drawing.Point(679, 37);
            this.RecordButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 13);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(160, 28);
            this.RecordButton.TabIndex = 3;
            this.RecordButton.Text = "Record";
            this.RecordButton.UseVisualStyleBackColor = true;
            this.RecordButton.Click += new System.EventHandler(this.RecordButtonClick);
            // 
            // ScriptBox
            // 
            this.ScriptBox.Controls.Add(this.radioScriptChinese);
            this.ScriptBox.Controls.Add(this.radioScriptLatin);
            this.ScriptBox.Location = new System.Drawing.Point(13, 331);
            this.ScriptBox.Name = "ScriptBox";
            this.ScriptBox.Size = new System.Drawing.Size(500, 68);
            this.ScriptBox.TabIndex = 27;
            this.ScriptBox.TabStop = false;
            this.ScriptBox.Text = "Script";
            this.ScriptBox.Visible = false;
            // 
            // radioScriptChinese
            // 
            this.radioScriptChinese.AutoSize = true;
            this.radioScriptChinese.Location = new System.Drawing.Point(7, 43);
            this.radioScriptChinese.Name = "radioScriptChinese";
            this.radioScriptChinese.Size = new System.Drawing.Size(63, 17);
            this.radioScriptChinese.TabIndex = 1;
            this.radioScriptChinese.Text = "Chinese";
            this.radioScriptChinese.UseVisualStyleBackColor = true;
            // 
            // radioScriptLatin
            // 
            this.radioScriptLatin.AutoSize = true;
            this.radioScriptLatin.Checked = true;
            this.radioScriptLatin.Location = new System.Drawing.Point(7, 20);
            this.radioScriptLatin.Name = "radioScriptLatin";
            this.radioScriptLatin.Size = new System.Drawing.Size(76, 17);
            this.radioScriptLatin.TabIndex = 0;
            this.radioScriptLatin.TabStop = true;
            this.radioScriptLatin.Text = "Latin script";
            this.radioScriptLatin.UseVisualStyleBackColor = true;
            // 
            // Record
            // 
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.ScriptBox);
            this.Controls.Add(this.RecordPanel);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ForeColor = System.Drawing.SystemColors.MenuText;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Record";
            this.Padding = new System.Windows.Forms.Padding(13, 13, 13, 15);
            this.Size = new System.Drawing.Size(883, 504);
            this.RecordPanel.ResumeLayout(false);
            this.RecordPanel.PerformLayout();
            this.ALGroupbox.ResumeLayout(false);
            this.ALGroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RecordSessionIdentification)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ScriptBox.ResumeLayout(false);
            this.ScriptBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel RecordPanel;
        private FlowLayoutPanel RecordPluginOptions;
        private OpenFileDialog OpenWithDialog;
        private Panel panel1;
        private Button RecordButton;
        private GroupBox ALGroupbox;
        private DataGridView RecordSessionIdentification;
        private DataGridViewTextBoxColumn colKey;
        private DataGridViewTextBoxColumn Value;
        private Label RecordSessionIdentificationLabel;
        private RadioButton AL_NoneRBTN;
        private RadioButton AL_CopytaskRBTN;
        private GroupBox ScriptBox;
        private RadioButton radioScriptChinese;
        private RadioButton radioScriptLatin;
    }
}
