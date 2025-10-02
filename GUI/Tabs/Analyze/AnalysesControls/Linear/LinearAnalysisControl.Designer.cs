namespace GUI.Tabs.Analyze.AnalysesControls.Linear
{
    partial class LinearAnalysisControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            this.SpecialCBx = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.FocusIntervals = new System.Windows.Forms.RadioButton();
            this.RevisionIntervals = new System.Windows.Forms.RadioButton();
            this.IntervalSizeErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.IntervalNmbrErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.PauseIntervals = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).BeginInit();
            this.FixedNumberOfIntervalsPanel.SuspendLayout();
            this.FixedIntervalSizePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.Location = new System.Drawing.Point(5, 36);
            this.label4.Margin = new System.Windows.Forms.Padding(4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 20;
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
            this.PauseThresholdField.Location = new System.Drawing.Point(161, 34);
            this.PauseThresholdField.Margin = new System.Windows.Forms.Padding(4);
            this.PauseThresholdField.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.PauseThresholdField.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PauseThresholdField.Name = "PauseThresholdField";
            this.PauseThresholdField.Size = new System.Drawing.Size(95, 20);
            this.PauseThresholdField.TabIndex = 19;
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
            this.FixedNumberOfIntervalsPanel.Location = new System.Drawing.Point(161, 102);
            this.FixedNumberOfIntervalsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsPanel.Name = "FixedNumberOfIntervalsPanel";
            this.FixedNumberOfIntervalsPanel.Size = new System.Drawing.Size(215, 29);
            this.FixedNumberOfIntervalsPanel.TabIndex = 18;
            // 
            // NumberOfIntervalsField
            // 
            this.NumberOfIntervalsField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.NumberOfIntervalsField.FormattingEnabled = true;
            this.NumberOfIntervalsField.Items.AddRange(new object[] {
            "1",
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
            this.NumberOfIntervalsField.TabIndex = 15;
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
            this.FixedIntervalSizePanel.Location = new System.Drawing.Point(161, 65);
            this.FixedIntervalSizePanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizePanel.Name = "FixedIntervalSizePanel";
            this.FixedIntervalSizePanel.Size = new System.Drawing.Size(224, 29);
            this.FixedIntervalSizePanel.TabIndex = 17;
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
            this.IntervalSizeField.TabIndex = 21;
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
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Interval Size (sec)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(220, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 16;
            // 
            // FixedNumberOfIntervalsRadioButton
            // 
            this.FixedNumberOfIntervalsRadioButton.AutoSize = true;
            this.FixedNumberOfIntervalsRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedNumberOfIntervalsRadioButton.Location = new System.Drawing.Point(9, 107);
            this.FixedNumberOfIntervalsRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsRadioButton.Name = "FixedNumberOfIntervalsRadioButton";
            this.FixedNumberOfIntervalsRadioButton.Size = new System.Drawing.Size(145, 17);
            this.FixedNumberOfIntervalsRadioButton.TabIndex = 13;
            this.FixedNumberOfIntervalsRadioButton.Text = "Fixed Number of Intervals";
            this.FixedNumberOfIntervalsRadioButton.UseVisualStyleBackColor = true;
            this.FixedNumberOfIntervalsRadioButton.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // FixedIntervalSizeRadioButton
            // 
            this.FixedIntervalSizeRadioButton.AutoSize = true;
            this.FixedIntervalSizeRadioButton.Checked = true;
            this.FixedIntervalSizeRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedIntervalSizeRadioButton.Location = new System.Drawing.Point(9, 70);
            this.FixedIntervalSizeRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizeRadioButton.Name = "FixedIntervalSizeRadioButton";
            this.FixedIntervalSizeRadioButton.Size = new System.Drawing.Size(111, 17);
            this.FixedIntervalSizeRadioButton.TabIndex = 10;
            this.FixedIntervalSizeRadioButton.TabStop = true;
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
            this.MoreInfoLabel.TabIndex = 7;
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
            this.NameLabel.Size = new System.Drawing.Size(53, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Linear";
            // 
            // SpecialCBx
            // 
            this.SpecialCBx.AutoSize = true;
            this.SpecialCBx.Location = new System.Drawing.Point(9, 211);
            this.SpecialCBx.Name = "SpecialCBx";
            this.SpecialCBx.Size = new System.Drawing.Size(141, 17);
            this.SpecialCBx.TabIndex = 21;
            this.SpecialCBx.Text = "Add condensed analysis";
            this.toolTip1.SetToolTip(this.SpecialCBx, "Adds a less verbose Linear Analysis suitable for analysis in Excel");
            this.SpecialCBx.UseVisualStyleBackColor = true;
            // 
            // FocusIntervals
            // 
            this.FocusIntervals.AutoSize = true;
            this.FocusIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusIntervals.Location = new System.Drawing.Point(9, 131);
            this.FocusIntervals.Name = "FocusIntervals";
            this.FocusIntervals.Size = new System.Drawing.Size(129, 17);
            this.FocusIntervals.TabIndex = 22;
            this.FocusIntervals.TabStop = true;
            this.FocusIntervals.Text = "Focus-based Intervals";
            this.FocusIntervals.UseVisualStyleBackColor = true;
            this.FocusIntervals.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // RevisionIntervals
            // 
            this.RevisionIntervals.AutoSize = true;
            this.RevisionIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RevisionIntervals.Location = new System.Drawing.Point(8, 154);
            this.RevisionIntervals.Name = "RevisionIntervals";
            this.RevisionIntervals.Size = new System.Drawing.Size(141, 17);
            this.RevisionIntervals.TabIndex = 23;
            this.RevisionIntervals.TabStop = true;
            this.RevisionIntervals.Text = "Revision-based Intervals";
            this.RevisionIntervals.UseVisualStyleBackColor = true;
            this.RevisionIntervals.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // IntervalSizeErrorProvider
            // 
            this.IntervalSizeErrorProvider.ContainerControl = this;
            // 
            // IntervalNmbrErrorProvider
            // 
            this.IntervalNmbrErrorProvider.ContainerControl = this;
            // 
            // PauseIntervals
            // 
            this.PauseIntervals.AutoSize = true;
            this.PauseIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PauseIntervals.Location = new System.Drawing.Point(8, 177);
            this.PauseIntervals.Name = "PauseIntervals";
            this.PauseIntervals.Size = new System.Drawing.Size(130, 17);
            this.PauseIntervals.TabIndex = 24;
            this.PauseIntervals.TabStop = true;
            this.PauseIntervals.Text = "Pause-based Intervals";
            this.PauseIntervals.UseVisualStyleBackColor = true;
            // 
            // LinearAnalysisControl
            // 
            this.Controls.Add(this.PauseIntervals);
            this.Controls.Add(this.RevisionIntervals);
            this.Controls.Add(this.FocusIntervals);
            this.Controls.Add(this.SpecialCBx);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PauseThresholdField);
            this.Controls.Add(this.FixedNumberOfIntervalsPanel);
            this.Controls.Add(this.FixedIntervalSizePanel);
            this.Controls.Add(this.FixedNumberOfIntervalsRadioButton);
            this.Controls.Add(this.FixedIntervalSizeRadioButton);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "LinearAnalysisControl";
            this.Size = new System.Drawing.Size(389, 231);
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).EndInit();
            this.FixedNumberOfIntervalsPanel.ResumeLayout(false);
            this.FixedNumberOfIntervalsPanel.PerformLayout();
            this.FixedIntervalSizePanel.ResumeLayout(false);
            this.FixedIntervalSizePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.RadioButton FixedIntervalSizeRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton FixedNumberOfIntervalsRadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel FixedIntervalSizePanel;
        private System.Windows.Forms.Panel FixedNumberOfIntervalsPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown PauseThresholdField;
        private System.Windows.Forms.ComboBox IntervalSizeField;
        private System.Windows.Forms.ComboBox NumberOfIntervalsField;
        private System.Windows.Forms.CheckBox SpecialCBx;
        private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.RadioButton FocusIntervals;
		private System.Windows.Forms.RadioButton RevisionIntervals;
        private System.Windows.Forms.ErrorProvider IntervalSizeErrorProvider;
        private System.Windows.Forms.ErrorProvider IntervalNmbrErrorProvider;
        private System.Windows.Forms.RadioButton PauseIntervals;
    }
}
