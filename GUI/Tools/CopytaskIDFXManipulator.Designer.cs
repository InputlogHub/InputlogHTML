namespace GUI.Tools
{
    partial class CopytaskIDFXManipulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopytaskIDFXManipulator));
            this.DialogIDFX = new System.Windows.Forms.OpenFileDialog();
            this.LayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TextboxSession = new System.Windows.Forms.TextBox();
            this.LabelSession = new System.Windows.Forms.Label();
            this.LabelCopytask = new System.Windows.Forms.Label();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.TextboxIDFX = new System.Windows.Forms.TextBox();
            this.TextboxCopytask = new System.Windows.Forms.TextBox();
            this.BrowseCopytask = new System.Windows.Forms.Button();
            this.LabelIDFX = new System.Windows.Forms.Label();
            this.BrowseIDFX = new System.Windows.Forms.Button();
            this.DialogCopytask = new System.Windows.Forms.OpenFileDialog();
            this.LayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DialogIDFX
            // 
            this.DialogIDFX.Filter = "IDFX Files|*.idfx";
            this.DialogIDFX.Multiselect = true;
            this.DialogIDFX.RestoreDirectory = true;
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.ColumnCount = 3;
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.59283F));
            this.LayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.40717F));
            this.LayoutPanel.Controls.Add(this.TextboxSession, 1, 2);
            this.LayoutPanel.Controls.Add(this.LabelSession, 0, 2);
            this.LayoutPanel.Controls.Add(this.LabelCopytask, 0, 1);
            this.LayoutPanel.Controls.Add(this.ProcessButton, 2, 3);
            this.LayoutPanel.Controls.Add(this.ProgressBar, 0, 3);
            this.LayoutPanel.Controls.Add(this.TextboxIDFX, 1, 0);
            this.LayoutPanel.Controls.Add(this.TextboxCopytask, 1, 1);
            this.LayoutPanel.Controls.Add(this.BrowseCopytask, 2, 1);
            this.LayoutPanel.Controls.Add(this.LabelIDFX, 0, 0);
            this.LayoutPanel.Controls.Add(this.BrowseIDFX, 2, 0);
            this.LayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.LayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.RowCount = 4;
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LayoutPanel.Size = new System.Drawing.Size(614, 162);
            this.LayoutPanel.TabIndex = 0;
            // 
            // TextboxSession
            // 
            this.TextboxSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TextboxSession.Location = new System.Drawing.Point(105, 90);
            this.TextboxSession.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
            this.TextboxSession.Name = "TextboxSession";
            this.TextboxSession.Size = new System.Drawing.Size(368, 20);
            this.TextboxSession.TabIndex = 9;
            // 
            // LabelSession
            // 
            this.LabelSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelSession.AutoSize = true;
            this.LabelSession.Location = new System.Drawing.Point(3, 93);
            this.LabelSession.Name = "LabelSession";
            this.LabelSession.Size = new System.Drawing.Size(94, 13);
            this.LabelSession.TabIndex = 8;
            this.LabelSession.Text = "Copytask Session";
            // 
            // LabelCopytask
            // 
            this.LabelCopytask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelCopytask.AutoSize = true;
            this.LabelCopytask.Location = new System.Drawing.Point(3, 53);
            this.LabelCopytask.Name = "LabelCopytask";
            this.LabelCopytask.Size = new System.Drawing.Size(94, 13);
            this.LabelCopytask.TabIndex = 7;
            this.LabelCopytask.Text = "Copytask";
            // 
            // ProcessButton
            // 
            this.ProcessButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessButton.Location = new System.Drawing.Point(493, 127);
            this.ProcessButton.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(111, 28);
            this.ProcessButton.TabIndex = 1;
            this.ProcessButton.Text = "Process";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButtonClick);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LayoutPanel.SetColumnSpan(this.ProgressBar, 2);
            this.ProgressBar.Location = new System.Drawing.Point(5, 127);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(468, 28);
            this.ProgressBar.TabIndex = 2;
            // 
            // TextboxIDFX
            // 
            this.TextboxIDFX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TextboxIDFX.Location = new System.Drawing.Point(105, 10);
            this.TextboxIDFX.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
            this.TextboxIDFX.Name = "TextboxIDFX";
            this.TextboxIDFX.ReadOnly = true;
            this.TextboxIDFX.Size = new System.Drawing.Size(368, 20);
            this.TextboxIDFX.TabIndex = 3;
            // 
            // TextboxCopytask
            // 
            this.TextboxCopytask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TextboxCopytask.Location = new System.Drawing.Point(105, 50);
            this.TextboxCopytask.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
            this.TextboxCopytask.Name = "TextboxCopytask";
            this.TextboxCopytask.ReadOnly = true;
            this.TextboxCopytask.Size = new System.Drawing.Size(368, 20);
            this.TextboxCopytask.TabIndex = 4;
            // 
            // BrowseCopytask
            // 
            this.BrowseCopytask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseCopytask.Location = new System.Drawing.Point(493, 46);
            this.BrowseCopytask.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.BrowseCopytask.Name = "BrowseCopytask";
            this.BrowseCopytask.Size = new System.Drawing.Size(111, 28);
            this.BrowseCopytask.TabIndex = 5;
            this.BrowseCopytask.Text = "Browse";
            this.BrowseCopytask.UseVisualStyleBackColor = true;
            this.BrowseCopytask.Click += new System.EventHandler(this.BrowseCopytaskClick);
            // 
            // LabelIDFX
            // 
            this.LabelIDFX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelIDFX.AutoSize = true;
            this.LabelIDFX.Location = new System.Drawing.Point(3, 13);
            this.LabelIDFX.Name = "LabelIDFX";
            this.LabelIDFX.Size = new System.Drawing.Size(94, 13);
            this.LabelIDFX.TabIndex = 6;
            this.LabelIDFX.Text = "IDFX Files";
            // 
            // BrowseIDFX
            // 
            this.BrowseIDFX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseIDFX.Location = new System.Drawing.Point(493, 6);
            this.BrowseIDFX.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.BrowseIDFX.Name = "BrowseIDFX";
            this.BrowseIDFX.Size = new System.Drawing.Size(111, 28);
            this.BrowseIDFX.TabIndex = 0;
            this.BrowseIDFX.Text = "Browse";
            this.BrowseIDFX.UseVisualStyleBackColor = true;
            this.BrowseIDFX.Click += new System.EventHandler(this.BrowseIDFXClick);
            // 
            // DialogCopytask
            // 
            this.DialogCopytask.Filter = "Copytask file|*.txt";
            // 
            // CopytaskIDFXManipulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 162);
            this.Controls.Add(this.LayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CopytaskIDFXManipulator";
            this.Text = "CopytaskIDFXManipulator";
            this.LayoutPanel.ResumeLayout(false);
            this.LayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog DialogIDFX;
        private System.Windows.Forms.TableLayoutPanel LayoutPanel;
        private System.Windows.Forms.Label LabelCopytask;
        private System.Windows.Forms.Button BrowseIDFX;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.TextBox TextboxIDFX;
        private System.Windows.Forms.TextBox TextboxCopytask;
        private System.Windows.Forms.Button BrowseCopytask;
        private System.Windows.Forms.Label LabelIDFX;
        private System.Windows.Forms.OpenFileDialog DialogCopytask;
        private System.Windows.Forms.TextBox TextboxSession;
        private System.Windows.Forms.Label LabelSession;
    }
}