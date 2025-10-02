namespace GUI.Tabs.Analyze.Reporting
{
    partial class EditReportingBlocks
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
            this.InnerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.Blocks = new System.Windows.Forms.FlowLayoutPanel();
            this.AddButton = new System.Windows.Forms.Button();
            this.DoneButton = new System.Windows.Forms.Button();
            this.InnerLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // InnerLayout
            // 
            this.InnerLayout.ColumnCount = 2;
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.InnerLayout.Controls.Add(this.InfoLabel, 0, 0);
            this.InnerLayout.Controls.Add(this.CancelButton, 1, 2);
            this.InnerLayout.Controls.Add(this.Blocks, 0, 1);
            this.InnerLayout.Controls.Add(this.AddButton, 1, 0);
            this.InnerLayout.Controls.Add(this.DoneButton, 0, 2);
            this.InnerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerLayout.Location = new System.Drawing.Point(0, 0);
            this.InnerLayout.Margin = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.InnerLayout.Name = "InnerLayout";
            this.InnerLayout.RowCount = 3;
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.InnerLayout.Size = new System.Drawing.Size(364, 406);
            this.InnerLayout.TabIndex = 1;
            // 
            // InfoLabel
            // 
            this.InfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Location = new System.Drawing.Point(15, 9);
            this.InfoLabel.Margin = new System.Windows.Forms.Padding(15, 7, 15, 0);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(234, 39);
            this.InfoLabel.TabIndex = 0;
            this.InfoLabel.Text = "You may alter the list of blocks here. You can add new blocks, delete blocks and " +
    "rename blocks.";
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(274, 374);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 28);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // Blocks
            // 
            this.Blocks.AutoScroll = true;
            this.Blocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InnerLayout.SetColumnSpan(this.Blocks, 2);
            this.Blocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Blocks.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Blocks.Location = new System.Drawing.Point(15, 57);
            this.Blocks.Margin = new System.Windows.Forms.Padding(15, 7, 15, 7);
            this.Blocks.Name = "Blocks";
            this.Blocks.Size = new System.Drawing.Size(334, 307);
            this.Blocks.TabIndex = 3;
            this.Blocks.WrapContents = false;
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Image = global::GUI.Properties.Resources.add;
            this.AddButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddButton.Location = new System.Drawing.Point(274, 19);
            this.AddButton.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 28);
            this.AddButton.TabIndex = 4;
            this.AddButton.Text = "New";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // DoneButton
            // 
            this.DoneButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DoneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DoneButton.Location = new System.Drawing.Point(184, 374);
            this.DoneButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(75, 28);
            this.DoneButton.TabIndex = 5;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            // 
            // EditReportingBlocks
            // 
            this.AcceptButton = this.DoneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 406);
            this.Controls.Add(this.InnerLayout);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 380);
            this.Name = "EditReportingBlocks";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit blocks";
            this.InnerLayout.ResumeLayout(false);
            this.InnerLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel InnerLayout;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.FlowLayoutPanel Blocks;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DoneButton;

    }
}