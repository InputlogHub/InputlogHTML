namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    partial class MergeAnalysis_Intro_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeAnalysis_Intro_Form));
            this.Info = new System.Windows.Forms.Label();
            this.RadioMergeHorizontal = new System.Windows.Forms.RadioButton();
            this.RadioMergeVertical = new System.Windows.Forms.RadioButton();
            this.ImageHorizontalMerge = new System.Windows.Forms.PictureBox();
            this.ImageVerticalMerge = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageHorizontalMerge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageVerticalMerge)).BeginInit();
            this.SuspendLayout();
            // 
            // Info
            // 
            this.Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Info.Location = new System.Drawing.Point(7, 11);
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(660, 46);
            this.Info.TabIndex = 2;
            this.Info.Text = resources.GetString("Info.Text");
            // 
            // RadioMergeHorizontal
            // 
            this.RadioMergeHorizontal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RadioMergeHorizontal.Location = new System.Drawing.Point(354, 250);
            this.RadioMergeHorizontal.Name = "RadioMergeHorizontal";
            this.RadioMergeHorizontal.Size = new System.Drawing.Size(275, 21);
            this.RadioMergeHorizontal.TabIndex = 8;
            this.RadioMergeHorizontal.TabStop = true;
            this.RadioMergeHorizontal.Text = "Horizontal merging";
            this.RadioMergeHorizontal.UseVisualStyleBackColor = true;
            this.RadioMergeHorizontal.CheckedChanged += new System.EventHandler(this.RadioMergeHorizontal_CheckedChanged);
            // 
            // RadioMergeVertical
            // 
            this.RadioMergeVertical.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RadioMergeVertical.Location = new System.Drawing.Point(354, 75);
            this.RadioMergeVertical.Name = "RadioMergeVertical";
            this.RadioMergeVertical.Size = new System.Drawing.Size(275, 21);
            this.RadioMergeVertical.TabIndex = 7;
            this.RadioMergeVertical.TabStop = true;
            this.RadioMergeVertical.Text = "Vertical merging";
            this.RadioMergeVertical.UseVisualStyleBackColor = true;
            this.RadioMergeVertical.CheckedChanged += new System.EventHandler(this.RadioMergeVertical_CheckedChanged);
            // 
            // ImageHorizontalMerge
            // 
            this.ImageHorizontalMerge.Image = ((System.Drawing.Image)(resources.GetObject("ImageHorizontalMerge.Image")));
            this.ImageHorizontalMerge.Location = new System.Drawing.Point(11, 235);
            this.ImageHorizontalMerge.Name = "ImageHorizontalMerge";
            this.ImageHorizontalMerge.Size = new System.Drawing.Size(289, 157);
            this.ImageHorizontalMerge.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ImageHorizontalMerge.TabIndex = 6;
            this.ImageHorizontalMerge.TabStop = false;
            // 
            // ImageVerticalMerge
            // 
            this.ImageVerticalMerge.Image = ((System.Drawing.Image)(resources.GetObject("ImageVerticalMerge.Image")));
            this.ImageVerticalMerge.Location = new System.Drawing.Point(10, 60);
            this.ImageVerticalMerge.Name = "ImageVerticalMerge";
            this.ImageVerticalMerge.Size = new System.Drawing.Size(289, 157);
            this.ImageVerticalMerge.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ImageVerticalMerge.TabIndex = 5;
            this.ImageVerticalMerge.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(354, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 78);
            this.label1.TabIndex = 9;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(357, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 91);
            this.label2.TabIndex = 10;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // MergeAnalysis_Intro_Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RadioMergeHorizontal);
            this.Controls.Add(this.RadioMergeVertical);
            this.Controls.Add(this.ImageHorizontalMerge);
            this.Controls.Add(this.ImageVerticalMerge);
            this.Controls.Add(this.Info);
            this.DoubleBuffered = true;
            this.Name = "MergeAnalysis_Intro_Form";
            this.Size = new System.Drawing.Size(670, 420);
            ((System.ComponentModel.ISupportInitialize)(this.ImageHorizontalMerge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageVerticalMerge)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label Info;
        private System.Windows.Forms.PictureBox ImageHorizontalMerge;
        private System.Windows.Forms.PictureBox ImageVerticalMerge;
		public System.Windows.Forms.RadioButton RadioMergeHorizontal;
		public System.Windows.Forms.RadioButton RadioMergeVertical;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}
