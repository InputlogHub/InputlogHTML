namespace GUI.Tabs.Analyze.AnalysesControls.WordPauses
{
    partial class WordPausesAnalysisControl
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
            this.components = new System.ComponentModel.Container();
            this.moreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.TargetLbl = new System.Windows.Forms.Label();
            this.ParticipantLbl = new System.Windows.Forms.Label();
            this.TargetField = new System.Windows.Forms.TextBox();
            this.ParticipantField = new System.Windows.Forms.TextBox();
            this.TargetSelectionBtn = new System.Windows.Forms.Button();
            this.ParticipantSelectionBtn = new System.Windows.Forms.Button();
            this.SelectTargetDialog = new System.Windows.Forms.OpenFileDialog();
            this.SelectParticipantDialog = new System.Windows.Forms.OpenFileDialog();
            this.TargetTip = new System.Windows.Forms.ToolTip(this.components);
            this.DistanceLbl = new System.Windows.Forms.Label();
            this.DistanceUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // moreInfoLabel
            // 
            this.moreInfoLabel.AutoSize = true;
            this.moreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.moreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.moreInfoLabel.Margin = new System.Windows.Forms.Padding(3);
            this.moreInfoLabel.Name = "moreInfoLabel";
            this.moreInfoLabel.Size = new System.Drawing.Size(52, 13);
            this.moreInfoLabel.TabIndex = 1;
            this.moreInfoLabel.TabStop = true;
            this.moreInfoLabel.Text = "More Info";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(166, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Word Pauses Analysis";
            // 
            // TargetLbl
            // 
            this.TargetLbl.AutoSize = true;
            this.TargetLbl.Location = new System.Drawing.Point(5, 41);
            this.TargetLbl.Name = "TargetLbl";
            this.TargetLbl.Size = new System.Drawing.Size(117, 13);
            this.TargetLbl.TabIndex = 2;
            this.TargetLbl.Text = "Select a target word list";
            // 
            // ParticipantLbl
            // 
            this.ParticipantLbl.AutoSize = true;
            this.ParticipantLbl.Location = new System.Drawing.Point(5, 67);
            this.ParticipantLbl.Name = "ParticipantLbl";
            this.ParticipantLbl.Size = new System.Drawing.Size(146, 13);
            this.ParticipantLbl.TabIndex = 3;
            this.ParticipantLbl.Text = "Select a participant- target list";
            // 
            // TargetField
            // 
            this.TargetField.Location = new System.Drawing.Point(171, 38);
            this.TargetField.Name = "TargetField";
            this.TargetField.Size = new System.Drawing.Size(151, 20);
            this.TargetField.TabIndex = 4;
            // 
            // ParticipantField
            // 
            this.ParticipantField.Location = new System.Drawing.Point(171, 64);
            this.ParticipantField.Name = "ParticipantField";
            this.ParticipantField.Size = new System.Drawing.Size(151, 20);
            this.ParticipantField.TabIndex = 5;
            // 
            // TargetSelectionBtn
            // 
            this.TargetSelectionBtn.Image = global::GUI.Properties.Resources.folder_explore;
            this.TargetSelectionBtn.Location = new System.Drawing.Point(328, 37);
            this.TargetSelectionBtn.Name = "TargetSelectionBtn";
            this.TargetSelectionBtn.Size = new System.Drawing.Size(52, 21);
            this.TargetSelectionBtn.TabIndex = 6;
            this.TargetSelectionBtn.UseVisualStyleBackColor = true;
            this.TargetSelectionBtn.Click += new System.EventHandler(this.TargetSelectionBtnClick);
            // 
            // ParticipantSelectionBtn
            // 
            this.ParticipantSelectionBtn.Image = global::GUI.Properties.Resources.folder_explore;
            this.ParticipantSelectionBtn.Location = new System.Drawing.Point(328, 62);
            this.ParticipantSelectionBtn.Name = "ParticipantSelectionBtn";
            this.ParticipantSelectionBtn.Size = new System.Drawing.Size(52, 21);
            this.ParticipantSelectionBtn.TabIndex = 7;
            this.ParticipantSelectionBtn.UseVisualStyleBackColor = true;
            this.ParticipantSelectionBtn.Click += new System.EventHandler(this.ParticipantSelectionBtnClick);
            // 
            // SelectTargetDialog
            // 
            this.SelectTargetDialog.Filter = "CSV files|*.csv";
            // 
            // SelectParticipantDialog
            // 
            this.SelectParticipantDialog.Filter = "CSV files|*.csv";
            // 
            // TargetTip
            // 
            this.TargetTip.IsBalloon = true;
            this.TargetTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // DistanceLbl
            // 
            this.DistanceLbl.AutoSize = true;
            this.DistanceLbl.Location = new System.Drawing.Point(5, 92);
            this.DistanceLbl.Name = "DistanceLbl";
            this.DistanceLbl.Size = new System.Drawing.Size(154, 13);
            this.DistanceLbl.TabIndex = 8;
            this.DistanceLbl.Text = "Damerau-Levenshtein distance";
            // 
            // DistanceUpDown
            // 
            this.DistanceUpDown.Location = new System.Drawing.Point(171, 90);
            this.DistanceUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.DistanceUpDown.Name = "DistanceUpDown";
            this.DistanceUpDown.Size = new System.Drawing.Size(33, 20);
            this.DistanceUpDown.TabIndex = 9;
            // 
            // WordPausesAnalysisControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.DistanceUpDown);
            this.Controls.Add(this.DistanceLbl);
            this.Controls.Add(this.ParticipantSelectionBtn);
            this.Controls.Add(this.TargetSelectionBtn);
            this.Controls.Add(this.ParticipantField);
            this.Controls.Add(this.TargetField);
            this.Controls.Add(this.ParticipantLbl);
            this.Controls.Add(this.TargetLbl);
            this.Controls.Add(this.moreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "WordPausesAnalysisControl";
            this.Size = new System.Drawing.Size(383, 113);
            ((System.ComponentModel.ISupportInitialize)(this.DistanceUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel moreInfoLabel;
        private System.Windows.Forms.Label TargetLbl;
        private System.Windows.Forms.Label ParticipantLbl;
        private System.Windows.Forms.TextBox TargetField;
        private System.Windows.Forms.TextBox ParticipantField;
        private System.Windows.Forms.Button TargetSelectionBtn;
        private System.Windows.Forms.Button ParticipantSelectionBtn;
        private System.Windows.Forms.OpenFileDialog SelectTargetDialog;
        private System.Windows.Forms.OpenFileDialog SelectParticipantDialog;
        private System.Windows.Forms.ToolTip TargetTip;
        private System.Windows.Forms.Label DistanceLbl;
        private System.Windows.Forms.NumericUpDown DistanceUpDown;
    }
}
