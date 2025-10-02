namespace GUI.Tabs.Analyze.AnalysesControls.ProcessGraph
{
    partial class ProcessGraphAnalysisControl
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
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.IncludeProcess = new System.Windows.Forms.CheckBox();
            this.IncludeProduct = new System.Windows.Forms.CheckBox();
            this.IncludePosition = new System.Windows.Forms.CheckBox();
            this.IncludePauses = new System.Windows.Forms.CheckBox();
            this.IncludeFocus = new System.Windows.Forms.CheckBox();
            this.FileTypeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PauseThreshold = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.OverrideY1TxtBx = new System.Windows.Forms.MaskedTextBox();
            this.OverrideY1Lbl = new System.Windows.Forms.Label();
            this.OverrideY2Lbl = new System.Windows.Forms.Label();
            this.OverrideY2TxtBx = new System.Windows.Forms.MaskedTextBox();
            this.OverrideY1Tip = new System.Windows.Forms.ToolTip(this.components);
            this.OverrideY2Tip = new System.Windows.Forms.ToolTip(this.components);
            this.AxisGrpBx = new System.Windows.Forms.GroupBox();
            this.IncludeOutlierBx = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PauseThreshold)).BeginInit();
            this.AxisGrpBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.MoreInfoLabel.TabIndex = 3;
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
            this.NameLabel.Size = new System.Drawing.Size(142, 25);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Process Graph";
            // 
            // IncludeProcess
            // 
            this.IncludeProcess.AutoSize = true;
            this.IncludeProcess.Checked = true;
            this.IncludeProcess.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludeProcess.Location = new System.Drawing.Point(10, 47);
            this.IncludeProcess.Name = "IncludeProcess";
            this.IncludeProcess.Size = new System.Drawing.Size(81, 21);
            this.IncludeProcess.TabIndex = 4;
            this.IncludeProcess.Text = "Process";
            this.IncludeProcess.UseVisualStyleBackColor = true;
            // 
            // IncludeProduct
            // 
            this.IncludeProduct.AutoSize = true;
            this.IncludeProduct.Checked = true;
            this.IncludeProduct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludeProduct.Location = new System.Drawing.Point(10, 125);
            this.IncludeProduct.Name = "IncludeProduct";
            this.IncludeProduct.Size = new System.Drawing.Size(79, 21);
            this.IncludeProduct.TabIndex = 5;
            this.IncludeProduct.Text = "Product";
            this.IncludeProduct.UseVisualStyleBackColor = true;
            // 
            // IncludePosition
            // 
            this.IncludePosition.AutoSize = true;
            this.IncludePosition.Checked = true;
            this.IncludePosition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludePosition.Location = new System.Drawing.Point(9, 151);
            this.IncludePosition.Name = "IncludePosition";
            this.IncludePosition.Size = new System.Drawing.Size(80, 21);
            this.IncludePosition.TabIndex = 6;
            this.IncludePosition.Text = "Position";
            this.IncludePosition.UseVisualStyleBackColor = true;
            // 
            // IncludePauses
            // 
            this.IncludePauses.AutoSize = true;
            this.IncludePauses.Checked = true;
            this.IncludePauses.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludePauses.Location = new System.Drawing.Point(9, 73);
            this.IncludePauses.Name = "IncludePauses";
            this.IncludePauses.Size = new System.Drawing.Size(77, 21);
            this.IncludePauses.TabIndex = 7;
            this.IncludePauses.Text = "Pauses";
            this.IncludePauses.UseVisualStyleBackColor = true;
            // 
            // IncludeFocus
            // 
            this.IncludeFocus.AutoSize = true;
            this.IncludeFocus.Checked = true;
            this.IncludeFocus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludeFocus.Location = new System.Drawing.Point(10, 99);
            this.IncludeFocus.Name = "IncludeFocus";
            this.IncludeFocus.Size = new System.Drawing.Size(68, 21);
            this.IncludeFocus.TabIndex = 8;
            this.IncludeFocus.Text = "Focus";
            this.IncludeFocus.UseVisualStyleBackColor = true;
            // 
            // FileTypeList
            // 
            this.FileTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FileTypeList.FormattingEnabled = true;
            this.FileTypeList.Items.AddRange(new object[] {
            "PNG",
            "BMP",
            "EMF",
            "JPEG",
            "TIFF"});
            this.FileTypeList.Location = new System.Drawing.Point(380, 177);
            this.FileTypeList.Name = "FileTypeList";
            this.FileTypeList.Size = new System.Drawing.Size(70, 25);
            this.FileTypeList.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(289, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Save as ...";
            // 
            // PauseThreshold
            // 
            this.PauseThreshold.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.PauseThreshold.Location = new System.Drawing.Point(380, 141);
            this.PauseThreshold.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.PauseThreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PauseThreshold.Name = "PauseThreshold";
            this.PauseThreshold.Size = new System.Drawing.Size(70, 23);
            this.PauseThreshold.TabIndex = 11;
            this.PauseThreshold.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(221, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Pause Threshold (ms)";
            // 
            // OverrideY1TxtBx
            // 
            this.OverrideY1TxtBx.Location = new System.Drawing.Point(287, 19);
            this.OverrideY1TxtBx.Mask = "0000000";
            this.OverrideY1TxtBx.Name = "OverrideY1TxtBx";
            this.OverrideY1TxtBx.Size = new System.Drawing.Size(52, 23);
            this.OverrideY1TxtBx.TabIndex = 13;
            this.OverrideY1TxtBx.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.OverrideY1TxtBx.ValidatingType = typeof(int);
            // 
            // OverrideY1Lbl
            // 
            this.OverrideY1Lbl.AutoSize = true;
            this.OverrideY1Lbl.Location = new System.Drawing.Point(6, 22);
            this.OverrideY1Lbl.Name = "OverrideY1Lbl";
            this.OverrideY1Lbl.Size = new System.Drawing.Size(270, 17);
            this.OverrideY1Lbl.TabIndex = 14;
            this.OverrideY1Lbl.Text = "Override Default Left Axis Max.Value (ms)";
            // 
            // OverrideY2Lbl
            // 
            this.OverrideY2Lbl.AutoSize = true;
            this.OverrideY2Lbl.Location = new System.Drawing.Point(6, 55);
            this.OverrideY2Lbl.Name = "OverrideY2Lbl";
            this.OverrideY2Lbl.Size = new System.Drawing.Size(269, 17);
            this.OverrideY2Lbl.TabIndex = 15;
            this.OverrideY2Lbl.Text = "Override Default Right Axis Max.Value (#)";
            // 
            // OverrideY2TxtBx
            // 
            this.OverrideY2TxtBx.Location = new System.Drawing.Point(287, 52);
            this.OverrideY2TxtBx.Mask = "0000000";
            this.OverrideY2TxtBx.Name = "OverrideY2TxtBx";
            this.OverrideY2TxtBx.Size = new System.Drawing.Size(52, 23);
            this.OverrideY2TxtBx.TabIndex = 16;
            this.OverrideY2TxtBx.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.OverrideY2TxtBx.ValidatingType = typeof(int);
            // 
            // OverrideY1Tip
            // 
            this.OverrideY1Tip.AutoPopDelay = 5000;
            this.OverrideY1Tip.InitialDelay = 500;
            this.OverrideY1Tip.IsBalloon = true;
            this.OverrideY1Tip.ReshowDelay = 50;
            this.OverrideY1Tip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.OverrideY1Tip.ToolTipTitle = "Override the Primary Y-Axis";
            // 
            // OverrideY2Tip
            // 
            this.OverrideY2Tip.AutoPopDelay = 5000;
            this.OverrideY2Tip.InitialDelay = 500;
            this.OverrideY2Tip.IsBalloon = true;
            this.OverrideY2Tip.ReshowDelay = 50;
            this.OverrideY2Tip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.OverrideY2Tip.ToolTipTitle = "Override the Secondary Y-Axis";
            // 
            // AxisGrpBx
            // 
            this.AxisGrpBx.Controls.Add(this.OverrideY2TxtBx);
            this.AxisGrpBx.Controls.Add(this.OverrideY2Lbl);
            this.AxisGrpBx.Controls.Add(this.OverrideY1Lbl);
            this.AxisGrpBx.Controls.Add(this.OverrideY1TxtBx);
            this.AxisGrpBx.Location = new System.Drawing.Point(111, 47);
            this.AxisGrpBx.Name = "AxisGrpBx";
            this.AxisGrpBx.Size = new System.Drawing.Size(361, 88);
            this.AxisGrpBx.TabIndex = 17;
            this.AxisGrpBx.TabStop = false;
            this.AxisGrpBx.Leave += new System.EventHandler(this.ValidateInputY1Y2);
            // 
            // IncludeOutlierBx
            // 
            this.IncludeOutlierBx.AutoSize = true;
            this.IncludeOutlierBx.Location = new System.Drawing.Point(10, 177);
            this.IncludeOutlierBx.Name = "IncludeOutlierBx";
            this.IncludeOutlierBx.Size = new System.Drawing.Size(79, 21);
            this.IncludeOutlierBx.TabIndex = 18;
            this.IncludeOutlierBx.Text = "Outliers";
            this.IncludeOutlierBx.UseVisualStyleBackColor = true;
            // 
            // ProcessGraphAnalysisControl
            // 
            this.Controls.Add(this.IncludeOutlierBx);
            this.Controls.Add(this.AxisGrpBx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PauseThreshold);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileTypeList);
            this.Controls.Add(this.IncludeFocus);
            this.Controls.Add(this.IncludePauses);
            this.Controls.Add(this.IncludePosition);
            this.Controls.Add(this.IncludeProduct);
            this.Controls.Add(this.IncludeProcess);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "ProcessGraphAnalysisControl";
            this.Size = new System.Drawing.Size(475, 205);
            ((System.ComponentModel.ISupportInitialize)(this.PauseThreshold)).EndInit();
            this.AxisGrpBx.ResumeLayout(false);
            this.AxisGrpBx.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.CheckBox IncludeProcess;
        private System.Windows.Forms.CheckBox IncludeProduct;
        private System.Windows.Forms.CheckBox IncludePosition;
        private System.Windows.Forms.CheckBox IncludePauses;
        private System.Windows.Forms.CheckBox IncludeFocus;
        private System.Windows.Forms.ComboBox FileTypeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown PauseThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox OverrideY1TxtBx;
        private System.Windows.Forms.Label OverrideY1Lbl;
        private System.Windows.Forms.Label OverrideY2Lbl;
        private System.Windows.Forms.MaskedTextBox OverrideY2TxtBx;
        private System.Windows.Forms.ToolTip OverrideY1Tip;
        private System.Windows.Forms.ToolTip OverrideY2Tip;
        private System.Windows.Forms.GroupBox AxisGrpBx;
        private System.Windows.Forms.CheckBox IncludeOutlierBx;
    }
}
