namespace GUI.Tabs.Analyze.AnalysesControls.General
{
    partial class GeneralAnalysisControl
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.CsvCBx = new System.Windows.Forms.CheckBox();
            this.RevisionCBX = new System.Windows.Forms.CheckBox();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.fixedInterval = new System.Windows.Forms.NumericUpDown();
            this.numberOfIntervals = new System.Windows.Forms.NumericUpDown();
            this.fixedIntevalLbl = new System.Windows.Forms.Label();
            this.numberIntervalsLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fixedInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfIntervals)).BeginInit();
            this.SuspendLayout();
            // 
            // CsvCBx
            // 
            this.CsvCBx.AutoSize = true;
            this.CsvCBx.Location = new System.Drawing.Point(233, 34);
            this.CsvCBx.Name = "CsvCBx";
            this.CsvCBx.Size = new System.Drawing.Size(119, 17);
            this.CsvCBx.TabIndex = 4;
            this.CsvCBx.Text = "Generate a CSV-file";
            this.toolTip1.SetToolTip(this.CsvCBx, "Allowing the import of the analysis data\r\ndirectly into Excel.");
            this.CsvCBx.UseVisualStyleBackColor = true;
            // 
            // RevisionCBX
            // 
            this.RevisionCBX.AutoSize = true;
            this.RevisionCBX.Location = new System.Drawing.Point(233, 62);
            this.RevisionCBX.Name = "RevisionCBX";
            this.RevisionCBX.Size = new System.Drawing.Size(126, 17);
            this.RevisionCBX.TabIndex = 5;
            this.RevisionCBX.Text = "Include Revision Info";
            this.RevisionCBX.UseVisualStyleBackColor = true;
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(51, 13);
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
            this.NameLabel.Size = new System.Drawing.Size(66, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "General";
            // 
            // fixedInterval
            // 
            this.fixedInterval.Location = new System.Drawing.Point(8, 33);
            this.fixedInterval.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.fixedInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fixedInterval.Name = "fixedInterval";
            this.fixedInterval.Size = new System.Drawing.Size(37, 20);
            this.fixedInterval.TabIndex = 6;
            this.fixedInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numberOfIntervals
            // 
            this.numberOfIntervals.Location = new System.Drawing.Point(8, 59);
            this.numberOfIntervals.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numberOfIntervals.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numberOfIntervals.Name = "numberOfIntervals";
            this.numberOfIntervals.Size = new System.Drawing.Size(37, 20);
            this.numberOfIntervals.TabIndex = 7;
            this.numberOfIntervals.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // fixedIntevalLbl
            // 
            this.fixedIntevalLbl.AutoSize = true;
            this.fixedIntevalLbl.Location = new System.Drawing.Point(52, 34);
            this.fixedIntevalLbl.Name = "fixedIntevalLbl";
            this.fixedIntevalLbl.Size = new System.Drawing.Size(118, 13);
            this.fixedIntevalLbl.TabIndex = 8;
            this.fixedIntevalLbl.Text = "Fixed interval size (min.)";
            // 
            // numberIntervalsLbl
            // 
            this.numberIntervalsLbl.AutoSize = true;
            this.numberIntervalsLbl.Location = new System.Drawing.Point(52, 62);
            this.numberIntervalsLbl.Name = "numberIntervalsLbl";
            this.numberIntervalsLbl.Size = new System.Drawing.Size(98, 13);
            this.numberIntervalsLbl.TabIndex = 9;
            this.numberIntervalsLbl.Text = "Number of intervals";
            // 
            // GeneralAnalysisControl
            // 
            this.Controls.Add(this.numberIntervalsLbl);
            this.Controls.Add(this.fixedIntevalLbl);
            this.Controls.Add(this.numberOfIntervals);
            this.Controls.Add(this.fixedInterval);
            this.Controls.Add(this.RevisionCBX);
            this.Controls.Add(this.CsvCBx);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "GeneralAnalysisControl";
            this.Size = new System.Drawing.Size(362, 82);
            ((System.ComponentModel.ISupportInitialize)(this.fixedInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfIntervals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.CheckBox CsvCBx;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox RevisionCBX;
		protected System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.NumericUpDown fixedInterval;
        private System.Windows.Forms.NumericUpDown numberOfIntervals;
        private System.Windows.Forms.Label fixedIntevalLbl;
        private System.Windows.Forms.Label numberIntervalsLbl;
    }
}
