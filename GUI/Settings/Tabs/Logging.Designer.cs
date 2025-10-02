namespace GUI.Settings.Tabs {
    partial class Logging {
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
            this.TimOutUpDown = new System.Windows.Forms.NumericUpDown();
            this.TimeOutLabel = new System.Windows.Forms.Label();
            this.RestrictedTip = new System.Windows.Forms.ToolTip(this.components);
            this.WordLogBox = new System.Windows.Forms.GroupBox();
            this.WLRestrictedLogging = new System.Windows.Forms.CheckBox();
            this.WLDisableAddins = new System.Windows.Forms.CheckBox();
            this.WLOverride = new System.Windows.Forms.CheckBox();
            this.VersionSavingGB = new System.Windows.Forms.GroupBox();
            this.UserActionLBL = new System.Windows.Forms.Label();
            this.TimeIntervalLbL = new System.Windows.Forms.Label();
            this.IntervalL = new System.Windows.Forms.Label();
            this.TimeIntervalUD = new System.Windows.Forms.NumericUpDown();
            this.UserActionDB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.segmentInitialPauseCBX = new System.Windows.Forms.CheckBox();
            this.keyDelimiters = new System.Windows.Forms.ComboBox();
            this.keyDelimitersCBX = new System.Windows.Forms.CheckBox();
            this.HighPrecision = new System.Windows.Forms.CheckBox();
            this.AutoHide = new System.Windows.Forms.CheckBox();
            this.TimeOutPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MouseMovementPauseThresholdPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.MouseMovementPauseThresholdUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.SeperateMouseMovementsCheckbox = new System.Windows.Forms.CheckBox();
            this.VersionTip = new System.Windows.Forms.ToolTip(this.components);
            this.AuthorCommentCBX = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.TimOutUpDown)).BeginInit();
            this.WordLogBox.SuspendLayout();
            this.VersionSavingGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeIntervalUD)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.TimeOutPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.MouseMovementPauseThresholdPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MouseMovementPauseThresholdUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TimOutUpDown
            // 
            this.TimOutUpDown.Location = new System.Drawing.Point(0, 0);
            this.TimOutUpDown.Name = "TimOutUpDown";
            this.TimOutUpDown.Size = new System.Drawing.Size(120, 20);
            this.TimOutUpDown.TabIndex = 0;
            // 
            // TimeOutLabel
            // 
            this.TimeOutLabel.Location = new System.Drawing.Point(0, 0);
            this.TimeOutLabel.Name = "TimeOutLabel";
            this.TimeOutLabel.Size = new System.Drawing.Size(100, 23);
            this.TimeOutLabel.TabIndex = 0;
            // 
            // RestrictedTip
            // 
            this.RestrictedTip.AutoPopDelay = 7500;
            this.RestrictedTip.InitialDelay = 200;
            this.RestrictedTip.IsBalloon = true;
            this.RestrictedTip.ReshowDelay = 100;
            this.RestrictedTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.RestrictedTip.ToolTipTitle = "Restricted Logging";
            // 
            // WordLogBox
            // 
            this.WordLogBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.WordLogBox.Controls.Add(this.WLRestrictedLogging);
            this.WordLogBox.Controls.Add(this.WLDisableAddins);
            this.WordLogBox.Controls.Add(this.WLOverride);
            this.WordLogBox.Location = new System.Drawing.Point(3, 99);
            this.WordLogBox.Name = "WordLogBox";
            this.WordLogBox.Size = new System.Drawing.Size(422, 95);
            this.WordLogBox.TabIndex = 22;
            this.WordLogBox.TabStop = false;
            this.WordLogBox.Text = "WordLog";
            this.RestrictedTip.SetToolTip(this.WordLogBox, "When checked, no keystroke logging outside the initial Word document.\r\nHowever, t" +
        "he title of the other document or window is registered.");
            // 
            // WLRestrictedLogging
            // 
            this.WLRestrictedLogging.AutoSize = true;
            this.WLRestrictedLogging.Location = new System.Drawing.Point(9, 66);
            this.WLRestrictedLogging.Margin = new System.Windows.Forms.Padding(2);
            this.WLRestrictedLogging.Name = "WLRestrictedLogging";
            this.WLRestrictedLogging.Size = new System.Drawing.Size(121, 17);
            this.WLRestrictedLogging.TabIndex = 2;
            this.WLRestrictedLogging.Text = "WordLog Restricted";
            this.WLRestrictedLogging.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.RestrictedTip.SetToolTip(this.WLRestrictedLogging, "When checked: no keystroke logging outside the initial Word document.\r\nHowever, t" +
        "he title of the window in focus is always registered.\r\n");
            this.WLRestrictedLogging.UseVisualStyleBackColor = true;
            // 
            // WLDisableAddins
            // 
            this.WLDisableAddins.AutoSize = true;
            this.WLDisableAddins.Location = new System.Drawing.Point(9, 43);
            this.WLDisableAddins.Name = "WLDisableAddins";
            this.WLDisableAddins.Size = new System.Drawing.Size(128, 17);
            this.WLDisableAddins.TabIndex = 1;
            this.WLDisableAddins.Text = "Disable Word Add-ins";
            this.WLDisableAddins.UseVisualStyleBackColor = true;
            // 
            // WLOverride
            // 
            this.WLOverride.AutoSize = true;
            this.WLOverride.Checked = true;
            this.WLOverride.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WLOverride.Location = new System.Drawing.Point(9, 21);
            this.WLOverride.Name = "WLOverride";
            this.WLOverride.Size = new System.Drawing.Size(179, 17);
            this.WLOverride.TabIndex = 0;
            this.WLOverride.Text = "Edit existing WordLog document";
            this.WLOverride.UseVisualStyleBackColor = true;
            // 
            // VersionSavingGB
            // 
            this.VersionSavingGB.Controls.Add(this.UserActionLBL);
            this.VersionSavingGB.Controls.Add(this.TimeIntervalLbL);
            this.VersionSavingGB.Controls.Add(this.IntervalL);
            this.VersionSavingGB.Controls.Add(this.TimeIntervalUD);
            this.VersionSavingGB.Controls.Add(this.UserActionDB);
            this.VersionSavingGB.Location = new System.Drawing.Point(3, 350);
            this.VersionSavingGB.Name = "VersionSavingGB";
            this.VersionSavingGB.Size = new System.Drawing.Size(422, 77);
            this.VersionSavingGB.TabIndex = 29;
            this.VersionSavingGB.TabStop = false;
            this.VersionSavingGB.Text = "Version Saving Settings";
            this.VersionTip.SetToolTip(this.VersionSavingGB, "The current document can be saved in the background.\r\nat certain intervals, eithe" +
        "r time based or triggerd by a keypress.\r\n");
            // 
            // UserActionLBL
            // 
            this.UserActionLBL.AutoSize = true;
            this.UserActionLBL.Location = new System.Drawing.Point(8, 49);
            this.UserActionLBL.Name = "UserActionLBL";
            this.UserActionLBL.Size = new System.Drawing.Size(98, 13);
            this.UserActionLBL.TabIndex = 6;
            this.UserActionLBL.Text = "User action interval";
            // 
            // TimeIntervalLbL
            // 
            this.TimeIntervalLbL.AutoSize = true;
            this.TimeIntervalLbL.Location = new System.Drawing.Point(7, 24);
            this.TimeIntervalLbL.Name = "TimeIntervalLbL";
            this.TimeIntervalLbL.Size = new System.Drawing.Size(92, 13);
            this.TimeIntervalLbL.TabIndex = 5;
            this.TimeIntervalLbL.Text = "Time laps interval ";
            // 
            // IntervalL
            // 
            this.IntervalL.AutoSize = true;
            this.IntervalL.Location = new System.Drawing.Point(177, 24);
            this.IntervalL.Name = "IntervalL";
            this.IntervalL.Size = new System.Drawing.Size(23, 13);
            this.IntervalL.TabIndex = 4;
            this.IntervalL.Text = "min";
            // 
            // TimeIntervalUD
            // 
            this.TimeIntervalUD.Location = new System.Drawing.Point(126, 22);
            this.TimeIntervalUD.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.TimeIntervalUD.Name = "TimeIntervalUD";
            this.TimeIntervalUD.Size = new System.Drawing.Size(45, 20);
            this.TimeIntervalUD.TabIndex = 3;
            this.TimeIntervalUD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TimeIntervalUD.ValueChanged += new System.EventHandler(this.TimebaseIntervalChanged);
            // 
            // UserActionDB
            // 
            this.UserActionDB.Cursor = System.Windows.Forms.Cursors.Default;
            this.UserActionDB.FormattingEnabled = true;
            this.UserActionDB.Location = new System.Drawing.Point(126, 46);
            this.UserActionDB.Name = "UserActionDB";
            this.UserActionDB.Size = new System.Drawing.Size(84, 21);
            this.UserActionDB.TabIndex = 2;
            this.UserActionDB.SelectedIndexChanged += new System.EventHandler(this.ActionIntervalChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.segmentInitialPauseCBX);
            this.groupBox2.Controls.Add(this.keyDelimiters);
            this.groupBox2.Controls.Add(this.keyDelimitersCBX);
            this.groupBox2.Location = new System.Drawing.Point(3, 264);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(422, 80);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IDFX Segmentation";
            // 
            // segmentInitialPauseCBX
            // 
            this.segmentInitialPauseCBX.AutoSize = true;
            this.segmentInitialPauseCBX.Location = new System.Drawing.Point(11, 46);
            this.segmentInitialPauseCBX.Name = "segmentInitialPauseCBX";
            this.segmentInitialPauseCBX.Size = new System.Drawing.Size(119, 17);
            this.segmentInitialPauseCBX.TabIndex = 26;
            this.segmentInitialPauseCBX.Text = "Include initial pause";
            this.segmentInitialPauseCBX.UseVisualStyleBackColor = true;
            // 
            // keyDelimiters
            // 
            this.keyDelimiters.FormattingEnabled = true;
            this.keyDelimiters.Location = new System.Drawing.Point(126, 22);
            this.keyDelimiters.Name = "keyDelimiters";
            this.keyDelimiters.Size = new System.Drawing.Size(84, 21);
            this.keyDelimiters.TabIndex = 25;
            this.keyDelimiters.SelectedIndexChanged += new System.EventHandler(this.KeyDelimitersSelectedIndexChanged);
            // 
            // keyDelimitersCBX
            // 
            this.keyDelimitersCBX.AutoSize = true;
            this.keyDelimitersCBX.Location = new System.Drawing.Point(11, 23);
            this.keyDelimitersCBX.Name = "keyDelimitersCBX";
            this.keyDelimitersCBX.Size = new System.Drawing.Size(90, 17);
            this.keyDelimitersCBX.TabIndex = 24;
            this.keyDelimitersCBX.Text = "Key delimiters";
            this.keyDelimitersCBX.UseVisualStyleBackColor = true;
            // 
            // HighPrecision
            // 
            this.HighPrecision.AutoSize = true;
            this.HighPrecision.Checked = true;
            this.HighPrecision.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HighPrecision.Location = new System.Drawing.Point(12, 228);
            this.HighPrecision.Name = "HighPrecision";
            this.HighPrecision.Size = new System.Drawing.Size(130, 17);
            this.HighPrecision.TabIndex = 26;
            this.HighPrecision.Text = "High precision logging";
            this.HighPrecision.UseVisualStyleBackColor = true;
            // 
            // AutoHide
            // 
            this.AutoHide.AutoSize = true;
            this.AutoHide.Location = new System.Drawing.Point(12, 205);
            this.AutoHide.Name = "AutoHide";
            this.AutoHide.Size = new System.Drawing.Size(134, 17);
            this.AutoHide.TabIndex = 25;
            this.AutoHide.Text = "Autohide when logging";
            this.AutoHide.UseVisualStyleBackColor = true;
            // 
            // TimeOutPanel
            // 
            this.TimeOutPanel.Controls.Add(this.groupBox1);
            this.TimeOutPanel.Location = new System.Drawing.Point(0, 0);
            this.TimeOutPanel.Name = "TimeOutPanel";
            this.TimeOutPanel.Size = new System.Drawing.Size(200, 100);
            this.TimeOutPanel.TabIndex = 28;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.MouseMovementPauseThresholdPanel);
            this.groupBox1.Controls.Add(this.SeperateMouseMovementsCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(3, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(422, 90);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WinLog";
            // 
            // MouseMovementPauseThresholdPanel
            // 
            this.MouseMovementPauseThresholdPanel.Controls.Add(this.label1);
            this.MouseMovementPauseThresholdPanel.Controls.Add(this.MouseMovementPauseThresholdUpDown);
            this.MouseMovementPauseThresholdPanel.Controls.Add(this.label2);
            this.MouseMovementPauseThresholdPanel.Location = new System.Drawing.Point(9, 41);
            this.MouseMovementPauseThresholdPanel.Name = "MouseMovementPauseThresholdPanel";
            this.MouseMovementPauseThresholdPanel.Size = new System.Drawing.Size(186, 31);
            this.MouseMovementPauseThresholdPanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pause Threshold";
            // 
            // MouseMovementPauseThresholdUpDown
            // 
            this.MouseMovementPauseThresholdUpDown.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MouseMovementPauseThresholdUpDown.Location = new System.Drawing.Point(96, 8);
            this.MouseMovementPauseThresholdUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.MouseMovementPauseThresholdUpDown.Name = "MouseMovementPauseThresholdUpDown";
            this.MouseMovementPauseThresholdUpDown.Size = new System.Drawing.Size(55, 20);
            this.MouseMovementPauseThresholdUpDown.TabIndex = 0;
            this.MouseMovementPauseThresholdUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MouseMovementPauseThresholdUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ms";
            // 
            // SeperateMouseMovementsCheckbox
            // 
            this.SeperateMouseMovementsCheckbox.AutoSize = true;
            this.SeperateMouseMovementsCheckbox.Checked = true;
            this.SeperateMouseMovementsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SeperateMouseMovementsCheckbox.Location = new System.Drawing.Point(9, 18);
            this.SeperateMouseMovementsCheckbox.Name = "SeperateMouseMovementsCheckbox";
            this.SeperateMouseMovementsCheckbox.Size = new System.Drawing.Size(162, 17);
            this.SeperateMouseMovementsCheckbox.TabIndex = 3;
            this.SeperateMouseMovementsCheckbox.Text = "Seperate Mouse Movements";
            this.SeperateMouseMovementsCheckbox.UseVisualStyleBackColor = true;
            this.SeperateMouseMovementsCheckbox.CheckedChanged += new System.EventHandler(this.SeperateMouseMovementsCheckboxCheckedChanged);
            // 
            // VersionTip
            // 
            this.VersionTip.AutoPopDelay = 7500;
            this.VersionTip.InitialDelay = 200;
            this.VersionTip.IsBalloon = true;
            this.VersionTip.ReshowDelay = 100;
            this.VersionTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.VersionTip.ToolTipTitle = "Document Versions";
            // 
            // AuthorCommentCBX
            // 
            this.AuthorCommentCBX.AutoSize = true;
            this.AuthorCommentCBX.Location = new System.Drawing.Point(12, 442);
            this.AuthorCommentCBX.Name = "AuthorCommentCBX";
            this.AuthorCommentCBX.Size = new System.Drawing.Size(295, 17);
            this.AuthorCommentCBX.TabIndex = 30;
            this.AuthorCommentCBX.Text = "Add an author comment at the end of this writing session.";
            this.AuthorCommentCBX.UseVisualStyleBackColor = true;
            // 
            // Logging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoSize = true;
            this.Controls.Add(this.AuthorCommentCBX);
            this.Controls.Add(this.VersionSavingGB);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.HighPrecision);
            this.Controls.Add(this.AutoHide);
            this.Controls.Add(this.TimeOutPanel);
            this.Controls.Add(this.WordLogBox);
            this.Name = "Logging";
            this.Size = new System.Drawing.Size(551, 494);
            this.Load += new System.EventHandler(this.LoggingLoad);
            ((System.ComponentModel.ISupportInitialize)(this.TimOutUpDown)).EndInit();
            this.WordLogBox.ResumeLayout(false);
            this.WordLogBox.PerformLayout();
            this.VersionSavingGB.ResumeLayout(false);
            this.VersionSavingGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeIntervalUD)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.TimeOutPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.MouseMovementPauseThresholdPanel.ResumeLayout(false);
            this.MouseMovementPauseThresholdPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MouseMovementPauseThresholdUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel MouseMovementPauseThresholdPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown MouseMovementPauseThresholdUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox SeperateMouseMovementsCheckbox;
        private System.Windows.Forms.Panel TimeOutPanel;
        private System.Windows.Forms.NumericUpDown TimOutUpDown;
        private System.Windows.Forms.Label TimeOutLabel;
        private System.Windows.Forms.GroupBox WordLogBox;
        private System.Windows.Forms.CheckBox WLDisableAddins;
        private System.Windows.Forms.CheckBox WLOverride;
        private System.Windows.Forms.CheckBox AutoHide;
        private System.Windows.Forms.CheckBox HighPrecision;
        private System.Windows.Forms.CheckBox WLRestrictedLogging;
        private System.Windows.Forms.ToolTip RestrictedTip;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox segmentInitialPauseCBX;
        private System.Windows.Forms.ComboBox keyDelimiters;
        private System.Windows.Forms.CheckBox keyDelimitersCBX;
        private System.Windows.Forms.GroupBox VersionSavingGB;
        private System.Windows.Forms.Label IntervalL;
        private System.Windows.Forms.NumericUpDown TimeIntervalUD;
        private System.Windows.Forms.ComboBox UserActionDB;
        private System.Windows.Forms.Label TimeIntervalLbL;
        private System.Windows.Forms.ToolTip VersionTip;
        private System.Windows.Forms.CheckBox AuthorCommentCBX;
        private System.Windows.Forms.Label UserActionLBL;
    }
}
