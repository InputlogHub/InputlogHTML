namespace GUI.Tabs.Analyze.Reporting
{
    partial class ReportElementControl
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
            this.InnerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.CheckBox = new System.Windows.Forms.CheckBox();
            this.Label = new System.Windows.Forms.Label();
            this.Block = new System.Windows.Forms.ComboBox();
            this.InnerLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // InnerLayout
            // 
            this.InnerLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.InnerLayout.ColumnCount = 3;
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.InnerLayout.Controls.Add(this.CheckBox, 0, 0);
            this.InnerLayout.Controls.Add(this.Label, 1, 0);
            this.InnerLayout.Controls.Add(this.Block, 2, 0);
            this.InnerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerLayout.Location = new System.Drawing.Point(0, 0);
            this.InnerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.InnerLayout.Name = "InnerLayout";
            this.InnerLayout.RowCount = 1;
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.Size = new System.Drawing.Size(575, 28);
            this.InnerLayout.TabIndex = 0;
            // 
            // CheckBox
            // 
            this.CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBox.AutoSize = true;
            this.CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox.Location = new System.Drawing.Point(2, 7);
            this.CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Size = new System.Drawing.Size(56, 14);
            this.CheckBox.TabIndex = 0;
            this.CheckBox.UseVisualStyleBackColor = true;
            // 
            // Label
            // 
            this.Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(63, 7);
            this.Label.MinimumSize = new System.Drawing.Size(200, 13);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(305, 13);
            this.Label.TabIndex = 1;
            this.Label.Text = "Reporting Method Name";
            // 
            // Block
            // 
            this.Block.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Block.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Block.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Block.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Block.Location = new System.Drawing.Point(378, 5);
            this.Block.Margin = new System.Windows.Forms.Padding(5, 3, 15, 0);
            this.Block.MinimumSize = new System.Drawing.Size(180, 0);
            this.Block.Name = "Block";
            this.Block.Size = new System.Drawing.Size(180, 21);
            this.Block.Sorted = true;
            this.Block.TabIndex = 2;
            // 
            // ReportElementUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.InnerLayout);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ReportElementUserControl";
            this.Size = new System.Drawing.Size(575, 28);
            this.InnerLayout.ResumeLayout(false);
            this.InnerLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel InnerLayout;
        private System.Windows.Forms.CheckBox CheckBox;
        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.ComboBox Block;
    }
}
