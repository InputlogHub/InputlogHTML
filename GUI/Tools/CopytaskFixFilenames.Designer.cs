namespace GUI.Tools
{
    partial class CopytaskFixFilenames
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopytaskFixFilenames));
            this.Layout = new System.Windows.Forms.TableLayoutPanel();
            this.IdfxLabel = new System.Windows.Forms.Label();
            this.IdfxTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.Layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.ColumnCount = 3;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.Layout.Controls.Add(this.IdfxLabel, 0, 0);
            this.Layout.Controls.Add(this.IdfxTextBox, 1, 0);
            this.Layout.Controls.Add(this.BrowseButton, 2, 0);
            this.Layout.Controls.Add(this.ProgressBar, 0, 1);
            this.Layout.Controls.Add(this.ProcessButton, 2, 1);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Name = "Layout";
            this.Layout.RowCount = 2;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Layout.Size = new System.Drawing.Size(614, 113);
            this.Layout.TabIndex = 0;
            // 
            // IdfxLabel
            // 
            this.IdfxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.IdfxLabel.AutoSize = true;
            this.IdfxLabel.Location = new System.Drawing.Point(5, 21);
            this.IdfxLabel.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
            this.IdfxLabel.Name = "IdfxLabel";
            this.IdfxLabel.Size = new System.Drawing.Size(92, 13);
            this.IdfxLabel.TabIndex = 0;
            this.IdfxLabel.Text = "Idfx Files:";
            // 
            // IdfxTextBox
            // 
            this.IdfxTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.IdfxTextBox.Enabled = false;
            this.IdfxTextBox.Location = new System.Drawing.Point(103, 18);
            this.IdfxTextBox.Name = "IdfxTextBox";
            this.IdfxTextBox.Size = new System.Drawing.Size(383, 20);
            this.IdfxTextBox.TabIndex = 1;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(499, 16);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(105, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Layout.SetColumnSpan(this.ProgressBar, 2);
            this.ProgressBar.Location = new System.Drawing.Point(10, 73);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(476, 23);
            this.ProgressBar.TabIndex = 3;
            // 
            // ProcessButton
            // 
            this.ProcessButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessButton.Location = new System.Drawing.Point(499, 73);
            this.ProcessButton.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(105, 23);
            this.ProcessButton.TabIndex = 4;
            this.ProcessButton.Text = "Restore names";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButtonClick);
            // 
            // CopytaskFixFilenames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 113);
            this.Controls.Add(this.Layout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CopytaskFixFilenames";
            this.Text = "Restore IDFX original logfile name";
            this.Layout.ResumeLayout(false);
            this.Layout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Layout;
        private System.Windows.Forms.Label IdfxLabel;
        private System.Windows.Forms.TextBox IdfxTextBox;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button ProcessButton;
    }
}