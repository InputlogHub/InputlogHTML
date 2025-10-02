namespace GUI.Tabs.Analyze.AnalysesControls.Fluency
{
    partial class FluencyAnalysisControl
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
            this.MaximumToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.BatchSettingsGBx = new System.Windows.Forms.GroupBox();
            this.DefaultMaxRBtn = new System.Windows.Forms.RadioButton();
            this.PersMaxRBtn = new System.Windows.Forms.RadioButton();
            this.MultigrphCBx = new System.Windows.Forms.CheckBox();
            this.MaximumLbl = new System.Windows.Forms.Label();
            this.trendDegreeLabel = new System.Windows.Forms.Label();
            this.trendLineDegree = new System.Windows.Forms.ComboBox();
            this.taskMaxTypeLabel = new System.Windows.Forms.Label();
            this.TaskMaximumTypeSelect = new System.Windows.Forms.ComboBox();
            this.CharProd = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.FileTypeList = new System.Windows.Forms.ComboBox();
            this.SavePersMaxLink = new System.Windows.Forms.LinkLabel();
            this.personalMaxLabel = new System.Windows.Forms.Label();
            this.personalMax = new System.Windows.Forms.NumericUpDown();
            this.RevisionIntervals = new System.Windows.Forms.RadioButton();
            this.FocusIntervals = new System.Windows.Forms.RadioButton();
            this.FixedNumberOfIntervalsPanel = new System.Windows.Forms.Panel();
            this.NumberOfIntervalsField = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FixedIntervalSizePanel = new System.Windows.Forms.Panel();
            this.IntervalSizeField = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FixedNumberOfIntervalsRadioButton = new System.Windows.Forms.RadioButton();
            this.FixedIntervalSizeRadioButton = new System.Windows.Forms.RadioButton();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.IntervalSizeErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.IntervalNmbrErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.MultiGraphErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.MaxSettingGBx = new System.Windows.Forms.GroupBox();
            this.AbsMaxLbl = new System.Windows.Forms.Label();
            this.PauseThresholdField = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.BatchSettingsGBx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personalMax)).BeginInit();
            this.FixedNumberOfIntervalsPanel.SuspendLayout();
            this.FixedIntervalSizePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MultiGraphErrorProvider)).BeginInit();
            this.MaxSettingGBx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).BeginInit();
            this.SuspendLayout();
            // 
            // MaximumToolTip
            // 
            this.MaximumToolTip.AutoPopDelay = 10000;
            this.MaximumToolTip.InitialDelay = 200;
            this.MaximumToolTip.IsBalloon = true;
            this.MaximumToolTip.ReshowDelay = 100;
            this.MaximumToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.MaximumToolTip.ToolTipTitle = "Fluency Maximum";
            // 
            // BatchSettingsGBx
            // 
            this.BatchSettingsGBx.Controls.Add(this.DefaultMaxRBtn);
            this.BatchSettingsGBx.Controls.Add(this.PersMaxRBtn);
            this.BatchSettingsGBx.Controls.Add(this.MultigrphCBx);
            this.BatchSettingsGBx.Controls.Add(this.MaximumLbl);
            this.BatchSettingsGBx.Location = new System.Drawing.Point(552, 37);
            this.BatchSettingsGBx.Name = "BatchSettingsGBx";
            this.BatchSettingsGBx.Size = new System.Drawing.Size(248, 170);
            this.BatchSettingsGBx.TabIndex = 38;
            this.BatchSettingsGBx.TabStop = false;
            this.BatchSettingsGBx.Text = "Processing Multiple Files";
            this.MaximumToolTip.SetToolTip(this.BatchSettingsGBx, "When processing multiple files select \'Individual Maximum\'\r\nto save the personal " +
        "maximum of each user or use the default\r\nvalue.");
            this.BatchSettingsGBx.Enter += new System.EventHandler(this.ValidateMultipleFiles);
            // 
            // DefaultMaxRBtn
            // 
            this.DefaultMaxRBtn.AutoSize = true;
            this.DefaultMaxRBtn.Location = new System.Drawing.Point(19, 92);
            this.DefaultMaxRBtn.Name = "DefaultMaxRBtn";
            this.DefaultMaxRBtn.Size = new System.Drawing.Size(165, 21);
            this.DefaultMaxRBtn.TabIndex = 4;
            this.DefaultMaxRBtn.Text = "Use Default Maximum";
            this.DefaultMaxRBtn.UseVisualStyleBackColor = true;
            // 
            // PersMaxRBtn
            // 
            this.PersMaxRBtn.AutoSize = true;
            this.PersMaxRBtn.Checked = true;
            this.PersMaxRBtn.Location = new System.Drawing.Point(19, 59);
            this.PersMaxRBtn.Name = "PersMaxRBtn";
            this.PersMaxRBtn.Size = new System.Drawing.Size(179, 21);
            this.PersMaxRBtn.TabIndex = 3;
            this.PersMaxRBtn.TabStop = true;
            this.PersMaxRBtn.Text = "Use Individual Maximum";
            this.PersMaxRBtn.UseVisualStyleBackColor = true;
            // 
            // MultigrphCBx
            // 
            this.MultigrphCBx.AutoSize = true;
            this.MultigrphCBx.Location = new System.Drawing.Point(19, 131);
            this.MultigrphCBx.Name = "MultigrphCBx";
            this.MultigrphCBx.Size = new System.Drawing.Size(211, 21);
            this.MultigrphCBx.TabIndex = 2;
            this.MultigrphCBx.Text = "Generate Multigraph (max.4)";
            this.MultigrphCBx.UseVisualStyleBackColor = true;
            this.MultigrphCBx.Validating += new System.ComponentModel.CancelEventHandler(this.MultigraphValidating);
            // 
            // MaximumLbl
            // 
            this.MaximumLbl.AutoSize = true;
            this.MaximumLbl.Location = new System.Drawing.Point(16, 32);
            this.MaximumLbl.Name = "MaximumLbl";
            this.MaximumLbl.Size = new System.Drawing.Size(126, 17);
            this.MaximumLbl.TabIndex = 0;
            this.MaximumLbl.Text = "Personal Maximum";
            // 
            // trendDegreeLabel
            // 
            this.trendDegreeLabel.AutoSize = true;
            this.trendDegreeLabel.Location = new System.Drawing.Point(550, 222);
            this.trendDegreeLabel.Name = "trendDegreeLabel";
            this.trendDegreeLabel.Size = new System.Drawing.Size(128, 17);
            this.trendDegreeLabel.TabIndex = 37;
            this.trendDegreeLabel.Text = "Trend Line Degree";
            // 
            // trendLineDegree
            // 
            this.trendLineDegree.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trendLineDegree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.trendLineDegree.FormattingEnabled = true;
            this.trendLineDegree.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.trendLineDegree.Location = new System.Drawing.Point(571, 242);
            this.trendLineDegree.Name = "trendLineDegree";
            this.trendLineDegree.Size = new System.Drawing.Size(95, 25);
            this.trendLineDegree.TabIndex = 36;
            // 
            // taskMaxTypeLabel
            // 
            this.taskMaxTypeLabel.AutoSize = true;
            this.taskMaxTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.taskMaxTypeLabel.Location = new System.Drawing.Point(14, 34);
            this.taskMaxTypeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.taskMaxTypeLabel.Name = "taskMaxTypeLabel";
            this.taskMaxTypeLabel.Size = new System.Drawing.Size(140, 17);
            this.taskMaxTypeLabel.TabIndex = 35;
            this.taskMaxTypeLabel.Text = "Task Maximum Mode";
            // 
            // TaskMaximumTypeSelect
            // 
            this.TaskMaximumTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TaskMaximumTypeSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TaskMaximumTypeSelect.FormattingEnabled = true;
            this.TaskMaximumTypeSelect.Location = new System.Drawing.Point(17, 55);
            this.TaskMaximumTypeSelect.Name = "TaskMaximumTypeSelect";
            this.TaskMaximumTypeSelect.Size = new System.Drawing.Size(186, 25);
            this.TaskMaximumTypeSelect.TabIndex = 34;
            // 
            // CharProd
            // 
            this.CharProd.AutoSize = true;
            this.CharProd.Checked = true;
            this.CharProd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CharProd.Location = new System.Drawing.Point(13, 245);
            this.CharProd.Name = "CharProd";
            this.CharProd.Size = new System.Drawing.Size(201, 21);
            this.CharProd.TabIndex = 33;
            this.CharProd.Text = "Processing only characters";
            this.CharProd.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(684, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 17);
            this.label6.TabIndex = 31;
            this.label6.Text = "Save graph as ...";
            // 
            // FileTypeList
            // 
            this.FileTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FileTypeList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FileTypeList.FormattingEnabled = true;
            this.FileTypeList.Items.AddRange(new object[] {
            "PNG",
            "BMP",
            "EMF",
            "JPEG",
            "TIFF"});
            this.FileTypeList.Location = new System.Drawing.Point(687, 242);
            this.FileTypeList.Name = "FileTypeList";
            this.FileTypeList.Size = new System.Drawing.Size(95, 25);
            this.FileTypeList.TabIndex = 30;
            // 
            // SavePersMaxLink
            // 
            this.SavePersMaxLink.AutoSize = true;
            this.SavePersMaxLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SavePersMaxLink.Location = new System.Drawing.Point(138, 112);
            this.SavePersMaxLink.Margin = new System.Windows.Forms.Padding(4);
            this.SavePersMaxLink.Name = "SavePersMaxLink";
            this.SavePersMaxLink.Size = new System.Drawing.Size(40, 17);
            this.SavePersMaxLink.TabIndex = 29;
            this.SavePersMaxLink.TabStop = true;
            this.SavePersMaxLink.Text = "Save";
            this.SavePersMaxLink.Visible = false;
            this.SavePersMaxLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SavePersOptLinkClicked);
            // 
            // personalMaxLabel
            // 
            this.personalMaxLabel.AutoSize = true;
            this.personalMaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.personalMaxLabel.Location = new System.Drawing.Point(14, 89);
            this.personalMaxLabel.Margin = new System.Windows.Forms.Padding(4);
            this.personalMaxLabel.Name = "personalMaxLabel";
            this.personalMaxLabel.Size = new System.Drawing.Size(175, 17);
            this.personalMaxLabel.TabIndex = 25;
            this.personalMaxLabel.Text = "Default Personal Maximum";
            // 
            // personalMax
            // 
            this.personalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.personalMax.Location = new System.Drawing.Point(17, 110);
            this.personalMax.Margin = new System.Windows.Forms.Padding(4);
            this.personalMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.personalMax.Name = "personalMax";
            this.personalMax.Size = new System.Drawing.Size(95, 23);
            this.personalMax.TabIndex = 24;
            this.personalMax.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.personalMax.ValueChanged += new System.EventHandler(this.PersonalMaxValueChanged);
            // 
            // RevisionIntervals
            // 
            this.RevisionIntervals.AutoSize = true;
            this.RevisionIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RevisionIntervals.Location = new System.Drawing.Point(13, 213);
            this.RevisionIntervals.Name = "RevisionIntervals";
            this.RevisionIntervals.Size = new System.Drawing.Size(184, 21);
            this.RevisionIntervals.TabIndex = 23;
            this.RevisionIntervals.TabStop = true;
            this.RevisionIntervals.Text = "Revision-based Intervals";
            this.RevisionIntervals.UseVisualStyleBackColor = true;
            this.RevisionIntervals.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // FocusIntervals
            // 
            this.FocusIntervals.AutoSize = true;
            this.FocusIntervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusIntervals.Location = new System.Drawing.Point(13, 183);
            this.FocusIntervals.Name = "FocusIntervals";
            this.FocusIntervals.Size = new System.Drawing.Size(168, 21);
            this.FocusIntervals.TabIndex = 22;
            this.FocusIntervals.TabStop = true;
            this.FocusIntervals.Text = "Focus-based Intervals";
            this.FocusIntervals.UseVisualStyleBackColor = true;
            this.FocusIntervals.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // FixedNumberOfIntervalsPanel
            // 
            this.FixedNumberOfIntervalsPanel.AutoSize = true;
            this.FixedNumberOfIntervalsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.NumberOfIntervalsField);
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.label2);
            this.FixedNumberOfIntervalsPanel.Location = new System.Drawing.Point(27, 62);
            this.FixedNumberOfIntervalsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsPanel.Name = "FixedNumberOfIntervalsPanel";
            this.FixedNumberOfIntervalsPanel.Size = new System.Drawing.Size(215, 37);
            this.FixedNumberOfIntervalsPanel.TabIndex = 18;
            // 
            // NumberOfIntervalsField
            // 
            this.NumberOfIntervalsField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.NumberOfIntervalsField.Location = new System.Drawing.Point(144, 8);
            this.NumberOfIntervalsField.Margin = new System.Windows.Forms.Padding(4);
            this.NumberOfIntervalsField.Name = "NumberOfIntervalsField";
            this.NumberOfIntervalsField.Size = new System.Drawing.Size(67, 25);
            this.NumberOfIntervalsField.TabIndex = 15;
            this.NumberOfIntervalsField.Text = "10";
            this.NumberOfIntervalsField.SelectedIndexChanged += new System.EventHandler(this.ResetGraph);
            this.NumberOfIntervalsField.Validating += new System.ComponentModel.CancelEventHandler(this.NumberOfIntervalsFieldValidating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(5, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "Number of Intervals";
            // 
            // FixedIntervalSizePanel
            // 
            this.FixedIntervalSizePanel.AutoSize = true;
            this.FixedIntervalSizePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FixedIntervalSizePanel.Controls.Add(this.IntervalSizeField);
            this.FixedIntervalSizePanel.Controls.Add(this.label1);
            this.FixedIntervalSizePanel.Location = new System.Drawing.Point(27, 136);
            this.FixedIntervalSizePanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizePanel.Name = "FixedIntervalSizePanel";
            this.FixedIntervalSizePanel.Size = new System.Drawing.Size(215, 33);
            this.FixedIntervalSizePanel.TabIndex = 17;
            // 
            // IntervalSizeField
            // 
            this.IntervalSizeField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.IntervalSizeField.Size = new System.Drawing.Size(67, 25);
            this.IntervalSizeField.TabIndex = 21;
            this.IntervalSizeField.Text = "60";
            this.IntervalSizeField.SelectedIndexChanged += new System.EventHandler(this.ResetGraph);
            this.IntervalSizeField.Validating += new System.ComponentModel.CancelEventHandler(this.IntervalSizeFieldValidating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Interval Size (sec)";
            // 
            // FixedNumberOfIntervalsRadioButton
            // 
            this.FixedNumberOfIntervalsRadioButton.AutoSize = true;
            this.FixedNumberOfIntervalsRadioButton.Checked = true;
            this.FixedNumberOfIntervalsRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedNumberOfIntervalsRadioButton.Location = new System.Drawing.Point(13, 37);
            this.FixedNumberOfIntervalsRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsRadioButton.Name = "FixedNumberOfIntervalsRadioButton";
            this.FixedNumberOfIntervalsRadioButton.Size = new System.Drawing.Size(189, 21);
            this.FixedNumberOfIntervalsRadioButton.TabIndex = 13;
            this.FixedNumberOfIntervalsRadioButton.TabStop = true;
            this.FixedNumberOfIntervalsRadioButton.Text = "Fixed Number of Intervals";
            this.FixedNumberOfIntervalsRadioButton.UseVisualStyleBackColor = true;
            this.FixedNumberOfIntervalsRadioButton.CheckedChanged += new System.EventHandler(this.UpdateAnalysisType);
            // 
            // FixedIntervalSizeRadioButton
            // 
            this.FixedIntervalSizeRadioButton.AutoSize = true;
            this.FixedIntervalSizeRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FixedIntervalSizeRadioButton.Location = new System.Drawing.Point(13, 111);
            this.FixedIntervalSizeRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.FixedIntervalSizeRadioButton.Name = "FixedIntervalSizeRadioButton";
            this.FixedIntervalSizeRadioButton.Size = new System.Drawing.Size(143, 21);
            this.FixedIntervalSizeRadioButton.TabIndex = 10;
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
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
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
            this.NameLabel.Size = new System.Drawing.Size(81, 25);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Fluency";
            // 
            // IntervalSizeErrorProvider
            // 
            this.IntervalSizeErrorProvider.ContainerControl = this;
            // 
            // IntervalNmbrErrorProvider
            // 
            this.IntervalNmbrErrorProvider.ContainerControl = this;
            // 
            // MultiGraphErrorProvider
            // 
            this.MultiGraphErrorProvider.ContainerControl = this;
            // 
            // MaxSettingGBx
            // 
            this.MaxSettingGBx.Controls.Add(this.AbsMaxLbl);
            this.MaxSettingGBx.Controls.Add(this.taskMaxTypeLabel);
            this.MaxSettingGBx.Controls.Add(this.TaskMaximumTypeSelect);
            this.MaxSettingGBx.Controls.Add(this.SavePersMaxLink);
            this.MaxSettingGBx.Controls.Add(this.personalMaxLabel);
            this.MaxSettingGBx.Controls.Add(this.personalMax);
            this.MaxSettingGBx.Location = new System.Drawing.Point(282, 37);
            this.MaxSettingGBx.Name = "MaxSettingGBx";
            this.MaxSettingGBx.Size = new System.Drawing.Size(236, 180);
            this.MaxSettingGBx.TabIndex = 39;
            this.MaxSettingGBx.TabStop = false;
            this.MaxSettingGBx.Text = "Maximum Settings";
            // 
            // AbsMaxLbl
            // 
            this.AbsMaxLbl.AutoSize = true;
            this.AbsMaxLbl.Location = new System.Drawing.Point(14, 146);
            this.AbsMaxLbl.Name = "AbsMaxLbl";
            this.AbsMaxLbl.Size = new System.Drawing.Size(157, 17);
            this.AbsMaxLbl.TabIndex = 36;
            this.AbsMaxLbl.Text = "Absolute Maximum: 400";
            // 
            // PauseThresholdField
            // 
            this.PauseThresholdField.Location = new System.Drawing.Point(402, 242);
            this.PauseThresholdField.Margin = new System.Windows.Forms.Padding(4);
            this.PauseThresholdField.Name = "PauseThresholdField";
            this.PauseThresholdField.Size = new System.Drawing.Size(86, 23);
            this.PauseThresholdField.TabIndex = 40;
            this.PauseThresholdField.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.PauseThresholdField.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 244);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 17);
            this.label3.TabIndex = 41;
            this.label3.Text = "Pause Threshold";
            this.label3.Visible = false;
            // 
            // FluencyAnalysisControl
            // 
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PauseThresholdField);
            this.Controls.Add(this.MaxSettingGBx);
            this.Controls.Add(this.BatchSettingsGBx);
            this.Controls.Add(this.trendDegreeLabel);
            this.Controls.Add(this.trendLineDegree);
            this.Controls.Add(this.CharProd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.FileTypeList);
            this.Controls.Add(this.RevisionIntervals);
            this.Controls.Add(this.FocusIntervals);
            this.Controls.Add(this.FixedNumberOfIntervalsPanel);
            this.Controls.Add(this.FixedIntervalSizePanel);
            this.Controls.Add(this.FixedNumberOfIntervalsRadioButton);
            this.Controls.Add(this.FixedIntervalSizeRadioButton);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "FluencyAnalysisControl";
            this.Size = new System.Drawing.Size(803, 270);
            this.BatchSettingsGBx.ResumeLayout(false);
            this.BatchSettingsGBx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personalMax)).EndInit();
            this.FixedNumberOfIntervalsPanel.ResumeLayout(false);
            this.FixedNumberOfIntervalsPanel.PerformLayout();
            this.FixedIntervalSizePanel.ResumeLayout(false);
            this.FixedIntervalSizePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalSizeErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MultiGraphErrorProvider)).EndInit();
            this.MaxSettingGBx.ResumeLayout(false);
            this.MaxSettingGBx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PauseThresholdField)).EndInit();
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
        private System.Windows.Forms.Panel FixedIntervalSizePanel;
        private System.Windows.Forms.Panel FixedNumberOfIntervalsPanel;
        private System.Windows.Forms.ComboBox IntervalSizeField;
        private System.Windows.Forms.ComboBox NumberOfIntervalsField;
        private System.Windows.Forms.ToolTip MaximumToolTip;
        private System.Windows.Forms.RadioButton FocusIntervals;
        private System.Windows.Forms.RadioButton RevisionIntervals;
        private System.Windows.Forms.ErrorProvider IntervalSizeErrorProvider;
        private System.Windows.Forms.ErrorProvider IntervalNmbrErrorProvider;
        private System.Windows.Forms.Label personalMaxLabel;
        private System.Windows.Forms.NumericUpDown personalMax;
        private System.Windows.Forms.LinkLabel SavePersMaxLink;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox FileTypeList;
        private System.Windows.Forms.CheckBox CharProd;
        private System.Windows.Forms.ComboBox TaskMaximumTypeSelect;
        private System.Windows.Forms.Label taskMaxTypeLabel;
        private System.Windows.Forms.Label trendDegreeLabel;
        private System.Windows.Forms.ComboBox trendLineDegree;
        private System.Windows.Forms.GroupBox BatchSettingsGBx;
        private System.Windows.Forms.Label MaximumLbl;
        private System.Windows.Forms.CheckBox MultigrphCBx;
        private System.Windows.Forms.ErrorProvider MultiGraphErrorProvider;
        private System.Windows.Forms.GroupBox MaxSettingGBx;
        private System.Windows.Forms.RadioButton DefaultMaxRBtn;
        private System.Windows.Forms.RadioButton PersMaxRBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown PauseThresholdField;
        private System.Windows.Forms.Label AbsMaxLbl;
    }
}
