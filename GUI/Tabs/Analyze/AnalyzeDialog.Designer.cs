namespace GUI.Tabs.Analyze
{
    partial class AnalyzeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalyzeDialog));
            this.destinationLbl = new System.Windows.Forms.Label();
            this.analysisDestinationTbx = new System.Windows.Forms.TextBox();
            this.analysisConfigLbl = new System.Windows.Forms.Label();
            this.yesBtn = new System.Windows.Forms.Button();
            this.noBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.analysisGbx = new System.Windows.Forms.GroupBox();
            this.analysisGbx.SuspendLayout();
            this.SuspendLayout();
            // 
            // destinationLbl
            // 
            this.destinationLbl.AutoSize = true;
            this.destinationLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.destinationLbl.Location = new System.Drawing.Point(17, 19);
            this.destinationLbl.Name = "destinationLbl";
            this.destinationLbl.Size = new System.Drawing.Size(263, 34);
            this.destinationLbl.TabIndex = 0;
            this.destinationLbl.Text = "Enter the name of a folder if you want\nto save your results in a separate directory.";
            // 
            // analysisDestinationTbx
            // 
            this.analysisDestinationTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.analysisDestinationTbx.Location = new System.Drawing.Point(20, 72);
            this.analysisDestinationTbx.Name = "analysisDestinationTbx";
            this.analysisDestinationTbx.Size = new System.Drawing.Size(262, 23);
            this.analysisDestinationTbx.TabIndex = 1;
            this.analysisDestinationTbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateInput);
            this.analysisDestinationTbx.Validated += new System.EventHandler(this.InputValidated);
            // 
            // analysisConfigLbl
            // 
            this.analysisConfigLbl.AutoSize = true;
            this.analysisConfigLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.analysisConfigLbl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.analysisConfigLbl.Location = new System.Drawing.Point(17, 137);
            this.analysisConfigLbl.Name = "analysisConfigLbl";
            this.analysisConfigLbl.Size = new System.Drawing.Size(292, 17);
            this.analysisConfigLbl.TabIndex = 3;
            this.analysisConfigLbl.Text = "Do you want to save the filter configurations?";
            // 
            // yesBtn
            // 
            this.yesBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.yesBtn.Location = new System.Drawing.Point(20, 166);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Size = new System.Drawing.Size(70, 30);
            this.yesBtn.TabIndex = 4;
            this.yesBtn.Text = "Yes";
            this.yesBtn.UseVisualStyleBackColor = true;
            this.yesBtn.Click += new System.EventHandler(this.YesBtnClick);
            // 
            // noBtn
            // 
            this.noBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.noBtn.Location = new System.Drawing.Point(118, 166);
            this.noBtn.Name = "noBtn";
            this.noBtn.Size = new System.Drawing.Size(70, 30);
            this.noBtn.TabIndex = 5;
            this.noBtn.Text = "No";
            this.noBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cancelBtn.Location = new System.Drawing.Point(32, 253);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(70, 30);
            this.cancelBtn.TabIndex = 6;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
            // 
            // okBtn
            // 
            this.okBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.okBtn.Location = new System.Drawing.Point(130, 253);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(81, 30);
            this.okBtn.TabIndex = 7;
            this.okBtn.Text = "Continue";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtnClick);
            // 
            // analysisGbx
            // 
            this.analysisGbx.Controls.Add(this.analysisDestinationTbx);
            this.analysisGbx.Controls.Add(this.destinationLbl);
            this.analysisGbx.Controls.Add(this.analysisConfigLbl);
            this.analysisGbx.Controls.Add(this.noBtn);
            this.analysisGbx.Controls.Add(this.yesBtn);
            this.analysisGbx.Location = new System.Drawing.Point(12, 12);
            this.analysisGbx.Name = "analysisGbx";
            this.analysisGbx.Size = new System.Drawing.Size(340, 216);
            this.analysisGbx.TabIndex = 8;
            this.analysisGbx.TabStop = false;
            // 
            // AnalyzeDialog
            // 
            this.AcceptButton = this.cancelBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 309);
            this.Controls.Add(this.analysisGbx);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnalyzeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Analysis Saving";
            this.analysisGbx.ResumeLayout(false);
            this.analysisGbx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label destinationLbl;
        private System.Windows.Forms.TextBox analysisDestinationTbx;
        private System.Windows.Forms.Label analysisConfigLbl;
        private System.Windows.Forms.Button yesBtn;
        private System.Windows.Forms.Button noBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.GroupBox analysisGbx;
    }
}