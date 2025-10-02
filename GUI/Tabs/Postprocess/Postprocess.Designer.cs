namespace GUI.Tabs.Postprocess
{
    partial class Postprocess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Postprocess));
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ConvertProgressMessage = new System.Windows.Forms.Label();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.ConvertProgressBar = new System.Windows.Forms.ProgressBar();
            this.MergeAnalysis = new System.Windows.Forms.RadioButton();
            this.MergeOtherInfo = new System.Windows.Forms.Label();
            this.MergeOther = new System.Windows.Forms.RadioButton();
            this.MergeAnalysisInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(18, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 52);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // radioButton1
            // 
            this.radioButton1.AutoCheck = false;
            this.radioButton1.AutoSize = true;
            this.radioButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton1.Location = new System.Drawing.Point(8, 90);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(8);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(296, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Combine simultaneously logged observations into one *file";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(18, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(246, 39);
            this.label2.TabIndex = 1;
            this.label2.Text = "- Two or more Inputlog General Analysis XML files\r\n- Two or more Inputlog Summary" +
    " Analysis XML files\r\n- Two or more Inputlog Pause Analysis XML files";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoCheck = false;
            this.radioButton2.AutoSize = true;
            this.radioButton2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton2.Location = new System.Drawing.Point(8, 16);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(8);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(303, 17);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Merge consecutively logged observations into one XML file";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 380F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ConvertProgressMessage, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.ConvertButton, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.ConvertProgressBar, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.MergeAnalysis, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MergeOtherInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.MergeOther, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.MergeAnalysisInfo, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(22, 22);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(828, 460);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // ConvertProgressMessage
            // 
            this.ConvertProgressMessage.AutoSize = true;
            this.ConvertProgressMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertProgressMessage.Location = new System.Drawing.Point(11, 401);
            this.ConvertProgressMessage.Margin = new System.Windows.Forms.Padding(11, 11, 11, 1);
            this.ConvertProgressMessage.Name = "ConvertProgressMessage";
            this.ConvertProgressMessage.Size = new System.Drawing.Size(43, 13);
            this.ConvertProgressMessage.TabIndex = 13;
            this.ConvertProgressMessage.Text = "Waiting";
            // 
            // ConvertButton
            // 
            this.ConvertButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.ConvertButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertButton.Location = new System.Drawing.Point(662, 432);
            this.ConvertButton.Margin = new System.Windows.Forms.Padding(0, 7, 6, 1);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(160, 27);
            this.ConvertButton.TabIndex = 12;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            // 
            // ConvertProgressBar
            // 
            this.ConvertProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertProgressBar.Location = new System.Drawing.Point(0, 432);
            this.ConvertProgressBar.Margin = new System.Windows.Forms.Padding(0, 7, 0, 1);
            this.ConvertProgressBar.Name = "ConvertProgressBar";
            this.ConvertProgressBar.Size = new System.Drawing.Size(380, 27);
            this.ConvertProgressBar.TabIndex = 11;
            // 
            // MergeAnalysis
            // 
            this.MergeAnalysis.Appearance = System.Windows.Forms.Appearance.Button;
            this.MergeAnalysis.AutoSize = true;
            this.MergeAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MergeAnalysis.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MergeAnalysis.Location = new System.Drawing.Point(0, 0);
            this.MergeAnalysis.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.MergeAnalysis.Name = "MergeAnalysis";
            this.MergeAnalysis.Size = new System.Drawing.Size(380, 29);
            this.MergeAnalysis.TabIndex = 10;
            this.MergeAnalysis.Text = "Merge consecutively logged observations into one CSV file";
            this.MergeAnalysis.UseVisualStyleBackColor = true;
            this.MergeAnalysis.CheckedChanged += new System.EventHandler(this.MergeAnalysis_CheckedChanged);
            // 
            // MergeOtherInfo
            // 
            this.MergeOtherInfo.AutoSize = true;
            this.MergeOtherInfo.Enabled = false;
            this.MergeOtherInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MergeOtherInfo.Location = new System.Drawing.Point(8, 254);
            this.MergeOtherInfo.Margin = new System.Windows.Forms.Padding(8);
            this.MergeOtherInfo.Name = "MergeOtherInfo";
            this.MergeOtherInfo.Size = new System.Drawing.Size(312, 60);
            this.MergeOtherInfo.TabIndex = 8;
            this.MergeOtherInfo.Text = resources.GetString("MergeOtherInfo.Text");
            // 
            // MergeOther
            // 
            this.MergeOther.Appearance = System.Windows.Forms.Appearance.Button;
            this.MergeOther.AutoSize = true;
            this.MergeOther.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MergeOther.Enabled = false;
            this.MergeOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MergeOther.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MergeOther.Location = new System.Drawing.Point(0, 211);
            this.MergeOther.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.MergeOther.Name = "MergeOther";
            this.MergeOther.Size = new System.Drawing.Size(380, 29);
            this.MergeOther.TabIndex = 6;
            this.MergeOther.Text = "Combine simultaneously logged observations into one *file";
            this.MergeOther.UseVisualStyleBackColor = true;
            // 
            // MergeAnalysisInfo
            // 
            this.MergeAnalysisInfo.AutoSize = true;
            this.MergeAnalysisInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MergeAnalysisInfo.Location = new System.Drawing.Point(8, 43);
            this.MergeAnalysisInfo.Margin = new System.Windows.Forms.Padding(8);
            this.MergeAnalysisInfo.Name = "MergeAnalysisInfo";
            this.MergeAnalysisInfo.Size = new System.Drawing.Size(265, 160);
            this.MergeAnalysisInfo.TabIndex = 5;
            this.MergeAnalysisInfo.Text = resources.GetString("MergeAnalysisInfo.Text");
            // 
            // Postprocess
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.28F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Postprocess";
            this.Padding = new System.Windows.Forms.Padding(22, 22, 22, 23);
            this.Size = new System.Drawing.Size(872, 505);
            this.Load += new System.EventHandler(this.ConvertDEV_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.RadioButton MergeAnalysis;
		private System.Windows.Forms.Label MergeOtherInfo;
		private System.Windows.Forms.RadioButton MergeOther;
		private System.Windows.Forms.Label MergeAnalysisInfo;
		public System.Windows.Forms.Label ConvertProgressMessage;
		private System.Windows.Forms.Button ConvertButton;
		private System.Windows.Forms.ProgressBar ConvertProgressBar;
    }
}
