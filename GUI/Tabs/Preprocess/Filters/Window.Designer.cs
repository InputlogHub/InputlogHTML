namespace GUI.Tabs.Preprocess.Filters
{

    partial class Window {
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
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.WindowTitleLabel = new System.Windows.Forms.Label();
            this.WindowTitleList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Enabled = false;
            this.MoreInfoLabel.Location = new System.Drawing.Point(181, 11);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.MoreInfoLabel.TabIndex = 9;
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
            this.NameLabel.Size = new System.Drawing.Size(130, 25);
            this.NameLabel.TabIndex = 8;
            this.NameLabel.Text = "Window Filter";
            // 
            // WindowTitleLabel
            // 
            this.WindowTitleLabel.AutoSize = true;
            this.WindowTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WindowTitleLabel.Location = new System.Drawing.Point(5, 57);
            this.WindowTitleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WindowTitleLabel.Name = "WindowTitleLabel";
            this.WindowTitleLabel.Size = new System.Drawing.Size(88, 17);
            this.WindowTitleLabel.TabIndex = 12;
            this.WindowTitleLabel.Text = "Window Title";
            // 
            // WindowTitleList
            // 
            this.WindowTitleList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WindowTitleList.FormattingEnabled = true;
            this.WindowTitleList.Items.AddRange(new object[] {
            "*Word*",
            "*Firefox*",
            "*Chrome*",
            "*Internet Explorer*"});
            this.WindowTitleList.Location = new System.Drawing.Point(113, 53);
            this.WindowTitleList.Margin = new System.Windows.Forms.Padding(4);
            this.WindowTitleList.Name = "WindowTitleList";
            this.WindowTitleList.Size = new System.Drawing.Size(160, 25);
            this.WindowTitleList.TabIndex = 16;
            this.WindowTitleList.Text = "*Word*";
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.Controls.Add(this.WindowTitleList);
            this.Controls.Add(this.WindowTitleLabel);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "Window";
            this.Size = new System.Drawing.Size(400, 97);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label WindowTitleLabel;
        private System.Windows.Forms.ComboBox WindowTitleList;
    }
}
