namespace ServerConfig
{
    partial class ServerConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConfig));
            this.SettingsButton = new System.Windows.Forms.Button();
            this.SendMailsButton = new System.Windows.Forms.Button();
            this.CleanupButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(56, 16);
            this.SettingsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(167, 48);
            this.SettingsButton.TabIndex = 0;
            this.SettingsButton.Text = "Settings";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButtonClick);
            // 
            // SendMailsButton
            // 
            this.SendMailsButton.Location = new System.Drawing.Point(56, 90);
            this.SendMailsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SendMailsButton.Name = "SendMailsButton";
            this.SendMailsButton.Size = new System.Drawing.Size(167, 48);
            this.SendMailsButton.TabIndex = 1;
            this.SendMailsButton.Text = "Send Mails";
            this.SendMailsButton.UseVisualStyleBackColor = true;
            this.SendMailsButton.Click += new System.EventHandler(this.SendMailsButtonClick);
            // 
            // CleanupButton
            // 
            this.CleanupButton.Location = new System.Drawing.Point(56, 166);
            this.CleanupButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CleanupButton.Name = "CleanupButton";
            this.CleanupButton.Size = new System.Drawing.Size(167, 48);
            this.CleanupButton.TabIndex = 2;
            this.CleanupButton.Text = "Cleanup";
            this.CleanupButton.UseVisualStyleBackColor = true;
            this.CleanupButton.Click += new System.EventHandler(this.CleanupButtonClick);
            // 
            // ServerConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 246);
            this.Controls.Add(this.CleanupButton);
            this.Controls.Add(this.SendMailsButton);
            this.Controls.Add(this.SettingsButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ServerConfig";
            this.Text = "ServerConfig";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button SendMailsButton;
        private System.Windows.Forms.Button CleanupButton;
    }
}

