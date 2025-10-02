namespace GUI.Util {
    partial class ExceptionMessageBox {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionMessageBox));
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ErrorIcon = new System.Windows.Forms.PictureBox();
            this.ErrorDetailsLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DetailsTextBox = new System.Windows.Forms.RichTextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.ShowDetailsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SendReportLink = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.panel1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(779, 78);
            this.flowLayoutPanel2.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.ErrorIcon);
            this.flowLayoutPanel1.Controls.Add(this.ErrorDetailsLabel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(171, 70);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // ErrorIcon
            // 
            this.ErrorIcon.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ErrorIcon.Location = new System.Drawing.Point(4, 4);
            this.ErrorIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ErrorIcon.Name = "ErrorIcon";
            this.ErrorIcon.Size = new System.Drawing.Size(59, 62);
            this.ErrorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ErrorIcon.TabIndex = 4;
            this.ErrorIcon.TabStop = false;
            // 
            // ErrorDetailsLabel
            // 
            this.ErrorDetailsLabel.AutoSize = true;
            this.ErrorDetailsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorDetailsLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ErrorDetailsLabel.Location = new System.Drawing.Point(80, 0);
            this.ErrorDetailsLabel.Margin = new System.Windows.Forms.Padding(13, 0, 4, 0);
            this.ErrorDetailsLabel.Name = "ErrorDetailsLabel";
            this.ErrorDetailsLabel.Size = new System.Drawing.Size(87, 70);
            this.ErrorDetailsLabel.TabIndex = 1;
            this.ErrorDetailsLabel.Text = "Error Details";
            this.ErrorDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(183, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(0, 0);
            this.panel1.TabIndex = 6;
            // 
            // DetailsTextBox
            // 
            this.DetailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DetailsTextBox.Location = new System.Drawing.Point(0, 141);
            this.DetailsTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DetailsTextBox.Name = "DetailsTextBox";
            this.DetailsTextBox.Size = new System.Drawing.Size(777, 298);
            this.DetailsTextBox.TabIndex = 11;
            this.DetailsTextBox.Text = "DetailsTextBox";
            this.DetailsTextBox.Visible = false;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(641, 104);
            this.OKButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(121, 29);
            this.OKButton.TabIndex = 12;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButtonClick);
            // 
            // ShowDetailsLinkLabel
            // 
            this.ShowDetailsLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowDetailsLinkLabel.AutoSize = true;
            this.ShowDetailsLinkLabel.Location = new System.Drawing.Point(541, 112);
            this.ShowDetailsLinkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ShowDetailsLinkLabel.Name = "ShowDetailsLinkLabel";
            this.ShowDetailsLinkLabel.Size = new System.Drawing.Size(89, 17);
            this.ShowDetailsLinkLabel.TabIndex = 14;
            this.ShowDetailsLinkLabel.TabStop = true;
            this.ShowDetailsLinkLabel.Text = "Show Details";
            this.ShowDetailsLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ShowDetailsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowDetailsLinkLabelLinkClicked);
            // 
            // SendReportLink
            // 
            this.SendReportLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SendReportLink.AutoSize = true;
            this.SendReportLink.Location = new System.Drawing.Point(444, 112);
            this.SendReportLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SendReportLink.Name = "SendReportLink";
            this.SendReportLink.Size = new System.Drawing.Size(88, 17);
            this.SendReportLink.TabIndex = 13;
            this.SendReportLink.TabStop = true;
            this.SendReportLink.Text = "Send Report";
            this.SendReportLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SendReportLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SendReportLinkLinkClicked);
            // 
            // ExceptionMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(779, 441);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.ShowDetailsLinkLabel);
            this.Controls.Add(this.SendReportLink);
            this.Controls.Add(this.DetailsTextBox);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(18, 45);
            this.Name = "ExceptionMessageBox";
            this.Text = "ExceptionMessageBox";
            this.TopMost = true;
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox ErrorIcon;
        private System.Windows.Forms.Label ErrorDetailsLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox DetailsTextBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.LinkLabel ShowDetailsLinkLabel;
        private System.Windows.Forms.LinkLabel SendReportLink;

    }
}