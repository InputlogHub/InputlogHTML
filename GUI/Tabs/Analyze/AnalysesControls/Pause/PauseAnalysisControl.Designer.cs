namespace GUI.Tabs.Analyze.AnalysesControls.Pause
{
    partial class PauseAnalysisControl
    {
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
            this.label4 = new System.Windows.Forms.Label();
            this.PauseThresholdField = new System.Windows.Forms.NumericUpDown();
            this.FixedNumberOfIntervalsPanel = new System.Windows.Forms.Panel();
            this.NumberOfIntervalsField = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FixedIntervalSizePanel = new System.Windows.Forms.Panel();
            this.IntervalSizeField = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.FixedNumberOfIntervalsRadioButton = new System.Windows.Forms.RadioButton();
            this.FixedIntervalSizeRadioButton = new System.Windows.Forms.RadioButton();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.IntervalSizeErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.IntervalNmbrErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.PBurstThresholdField = new System.Windows.Forms.NumericUpDown();
            this.PBurstTooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).BeginInit();
            this.FixedNumberOfIntervalsPanel.SuspendLayout();
            this.FixedIntervalSizePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBurstThresholdField)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.Location = new System.Drawing.Point(4, 36);
            this.label4.Margin = new System.Windows.Forms.Padding(4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Pause Threshold (ms)";
            // 
            // PauseThresholdField
            // 
            this.PauseThresholdField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PauseThresholdField.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.PauseThresholdField.Location = new System.Drawing.Point(166, 34);
            this.PauseThresholdField.Margin = new System.Windows.Forms.Padding(4);
            this.PauseThresholdField.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.PauseThresholdField.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.PauseThresholdField.Name = "PauseThresholdField";
            this.PauseThresholdField.Size = new System.Drawing.Size(68, 20);
            this.PauseThresholdField.TabIndex = 25;
            this.PauseThresholdField.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // FixedNumberOfIntervalsPanel
            // 
            this.FixedNumberOfIntervalsPanel.AutoSize = true;
            this.FixedNumberOfIntervalsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.NumberOfIntervalsField);
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.label2);
            this.FixedNumberOfIntervalsPanel.Location = new System.Drawing.Point(205, 147);
            this.FixedNumberOfIntervalsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsPanel.Name = "FixedNumberOfIntervalsPanel";
            this.FixedNumberOfIntervalsPanel.Size = new System.Drawing.Size(215, 29);
            this.FixedNumberOfIntervalsPanel.TabIndex = 24;
            // 
            // NumberOfIntervalsField
            // 
            this.NumberOfIntervalsField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.NumberOfIntervalsField.FormattingEnabled = true;
            this.NumberOfIntervalsField.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.NumberOfIntervalsField.Location = new System.Drawing.Point(144, 4);
            this.NumberOfIntervalsField.Margin = new System.Windows.Forms.Padding(4);
            this.NumberOfIntervalsField.Name = "NumberOfIntervalsField";
            this.NumberOfIntervalsField.Size = new System.Drawing.Size(67, 21);
            this.NumberOfIntervalsField.TabIndex = 16;
            this.NumberOfIntervalsField.Text = "5";
            this.NumberOfIntervalsField.Validating += new System.ComponentModel.CancelEventHandler(this.NumberOfIntervalsFieldValidating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Number of Intervals";
            // 
            // FixedIntervalSizePanel
            // 
            this.FixedIntervalSizePanel.AutoSize = true;
            this.FixedIntervalSizePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FixedIntervalSizePanel.Controls.Add(this.IntervalSizeField);
            this.FixedIntervalSizePanel.Controls.Add(this.label1);
            this.FixedIntervalSizePanel.Controls.Add(this.label3);
            this.FixedIntervalSizePanel.Location = new System.Drawing.Point(205, 106);
            this.FixedIntervalSizePanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizePanel.Name = "FixedIntervalSizePanel";
            this.FixedIntervalSizePanel.Size = new System.Drawing.Size(271, 29);
            this.FixedIntervalSizePanel.TabIndex = 23;
            // 
            // IntervalSizeField
            // 
            this.IntervalSizeField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.IntervalSizeField.FormattingEnabled = true;
            this.IntervalSizeField.Items.AddRange(new object[] {
            "20",
            "30",
            "60",
            "120"});
            this.IntervalSizeField.Location = new System.Drawing.Point(144, 4);
            this.IntervalSizeField.Margin = new System.Windows.Forms.Padding(4);
            this.IntervalSizeField.Name = "IntervalSizeField";
            this.IntervalSizeField.Size = new System.Drawing.Size(67, 21);
            this.IntervalSizeField.TabIndex = 27;
            this.IntervalSizeField.Text = "60";
            this.IntervalSizeField.Validating += new System.ComponentModel.CancelEventHandler(this.IntervalSizeFieldValidating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Interval Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(220, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "seconds";
            // 
            // FixedNumberOfIntervalsRadioButton
            // 
            this.FixedNumberOfIntervalsRadioButton.AutoSize = true;
            this.FixedNumberOfIntervalsRadioButton.Checked = true;
            this.FixedNumberOfIntervalsRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedNumberOfIntervalsRadioButton.Location = new System.Drawing.Point(8, 151);
            this.FixedNumberOfIntervalsRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsRadioButton.Name = "FixedNumberOfIntervalsRadioButton";
            this.FixedNumberOfIntervalsRadioButton.Size = new System.Drawing.Size(145, 17);
            this.FixedNumberOfIntervalsRadioButton.TabIndex = 22;
            this.FixedNumberOfIntervalsRadioButton.TabStop = true;
            this.FixedNumberOfIntervalsRadioButton.Text = "Fixed Number of Intervals";
            this.FixedNumberOfIntervalsRadioButton.UseVisualStyleBackColor = true;
            this.FixedNumberOfIntervalsRadioButton.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // FixedIntervalSizeRadioButton
            // 
            this.FixedIntervalSizeRadioButton.AutoSize = true;
            this.FixedIntervalSizeRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedIntervalSizeRadioButton.Location = new System.Drawing.Point(8, 111);
            this.FixedIntervalSizeRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizeRadioButton.Name = "FixedIntervalSizeRadioButton";
            this.FixedIntervalSizeRadioButton.Size = new System.Drawing.Size(111, 17);
            this.FixedIntervalSizeRadioButton.TabIndex = 21;
            this.FixedIntervalSizeRadioButton.Text = "Fixed Interval Size";
            this.FixedIntervalSizeRadioButton.UseVisualStyleBackColor = true;
            this.FixedIntervalSizeRadioButton.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(51, 13);
            this.MoreInfoLabel.TabIndex = 5;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More info";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(54, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Pause";
            // 
            // IntervalSizeErrorProvider
            // 
            this.IntervalSizeErrorProvider.ContainerControl = this;
            // 
            // IntervalNmbrErrorProvider
            // 
            this.IntervalNmbrErrorProvider.ContainerControl = this;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "P-Burst Threshold (ms)";
            // 
            // PBurstThresholdField
            // 
            this.PBurstThresholdField.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.PBurstThresholdField.Location = new System.Drawing.Point(166, 66);
            this.PBurstThresholdField.Margin = new System.Windows.Forms.Padding(4);
            this.PBurstThresholdField.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.PBurstThresholdField.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.PBurstThresholdField.Name = "PBurstThresholdField";
            this.PBurstThresholdField.Size = new System.Drawing.Size(68, 20);
            this.PBurstThresholdField.TabIndex = 28;
            this.PBurstThresholdField.UseWaitCursor = true;
            this.PBurstThresholdField.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // PBurstTooltip
            // 
            this.PBurstTooltip.AutoPopDelay = 10000;
            this.PBurstTooltip.InitialDelay = 500;
            this.PBurstTooltip.IsBalloon = true;
            this.PBurstTooltip.ReshowDelay = 100;
            this.PBurstTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.PBurstTooltip.ToolTipTitle = "P-Burst";
            // 
            // PauseAnalysisControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = false;
            this.Controls.Add(this.PBurstThresholdField);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PauseThresholdField);
            this.Controls.Add(this.FixedNumberOfIntervalsPanel);
            this.Controls.Add(this.FixedIntervalSizePanel);
            this.Controls.Add(this.FixedNumberOfIntervalsRadioButton);
            this.Controls.Add(this.FixedIntervalSizeRadioButton);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "PauseAnalysisControl";
            this.Size = new System.Drawing.Size(505, 209);
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).EndInit();
            this.FixedNumberOfIntervalsPanel.ResumeLayout(false);
            this.FixedNumberOfIntervalsPanel.PerformLayout();
            this.FixedIntervalSizePanel.ResumeLayout(false);
            this.FixedIntervalSizePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBurstThresholdField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown PauseThresholdField;
        private System.Windows.Forms.Panel FixedNumberOfIntervalsPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel FixedIntervalSizePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton FixedNumberOfIntervalsRadioButton;
        private System.Windows.Forms.RadioButton FixedIntervalSizeRadioButton;
        private System.Windows.Forms.ComboBox IntervalSizeField;
        private System.Windows.Forms.ComboBox NumberOfIntervalsField;
        private System.Windows.Forms.ErrorProvider IntervalSizeErrorProvider;
        private System.Windows.Forms.ErrorProvider IntervalNmbrErrorProvider;
        private System.Windows.Forms.NumericUpDown PBurstThresholdField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip PBurstTooltip;
    }
}
