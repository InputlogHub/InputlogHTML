namespace GUI.Tabs.Analyze.Reporting
{
    partial class AnalysisReportingTargets
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
            this.OuterLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TitleLayout = new System.Windows.Forms.TableLayoutPanel();
            this.AnalysisTitle = new System.Windows.Forms.Label();
            this.CheckBox = new System.Windows.Forms.CheckBox();
            this.AnalysisBlock = new System.Windows.Forms.ComboBox();
            this.Data = new System.Windows.Forms.FlowLayoutPanel();
            this.OuterLayoutPanel.SuspendLayout();
            this.TitleLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // OuterLayoutPanel
            // 
            this.OuterLayoutPanel.ColumnCount = 1;
            this.OuterLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.OuterLayoutPanel.Controls.Add(this.TitleLayout, 0, 0);
            this.OuterLayoutPanel.Controls.Add(this.Data, 0, 1);
            this.OuterLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.OuterLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.OuterLayoutPanel.Name = "OuterLayoutPanel";
            this.OuterLayoutPanel.RowCount = 2;
            this.OuterLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.OuterLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OuterLayoutPanel.Size = new System.Drawing.Size(735, 50);
            this.OuterLayoutPanel.TabIndex = 0;
            // 
            // TitleLayout
            // 
            this.TitleLayout.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TitleLayout.ColumnCount = 3;
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TitleLayout.Controls.Add(this.AnalysisTitle, 1, 0);
            this.TitleLayout.Controls.Add(this.CheckBox, 0, 0);
            this.TitleLayout.Controls.Add(this.AnalysisBlock, 2, 0);
            this.TitleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleLayout.Location = new System.Drawing.Point(0, 0);
            this.TitleLayout.Margin = new System.Windows.Forms.Padding(0);
            this.TitleLayout.Name = "TitleLayout";
            this.TitleLayout.RowCount = 1;
            this.TitleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TitleLayout.Size = new System.Drawing.Size(735, 30);
            this.TitleLayout.TabIndex = 0;
            // 
            // AnalysisTitle
            // 
            this.AnalysisTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalysisTitle.AutoSize = true;
            this.AnalysisTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnalysisTitle.Location = new System.Drawing.Point(78, 7);
            this.AnalysisTitle.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.AnalysisTitle.MinimumSize = new System.Drawing.Size(200, 16);
            this.AnalysisTitle.Name = "AnalysisTitle";
            this.AnalysisTitle.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.AnalysisTitle.Size = new System.Drawing.Size(399, 16);
            this.AnalysisTitle.TabIndex = 0;
            this.AnalysisTitle.Text = "Title of Analysis";
            // 
            // CheckBox
            // 
            this.CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBox.AutoSize = true;
            this.CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox.Location = new System.Drawing.Point(0, 8);
            this.CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.CheckBox.Size = new System.Drawing.Size(73, 14);
            this.CheckBox.TabIndex = 2;
            this.CheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox.UseVisualStyleBackColor = true;
            // 
            // AnalysisBlock
            // 
            this.AnalysisBlock.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.AnalysisBlock.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.AnalysisBlock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnalysisBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AnalysisBlock.Location = new System.Drawing.Point(482, 5);
            this.AnalysisBlock.Margin = new System.Windows.Forms.Padding(5, 5, 16, 5);
            this.AnalysisBlock.Name = "AnalysisBlock";
            this.AnalysisBlock.Size = new System.Drawing.Size(237, 21);
            this.AnalysisBlock.Sorted = true;
            this.AnalysisBlock.TabIndex = 3;
            // 
            // Data
            // 
            this.Data.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Data.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.Data.Location = new System.Drawing.Point(0, 30);
            this.Data.Margin = new System.Windows.Forms.Padding(0);
            this.Data.Name = "Data";
            this.Data.Size = new System.Drawing.Size(735, 20);
            this.Data.TabIndex = 1;
            this.Data.WrapContents = false;
            // 
            // AnalysisReportingTargets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.OuterLayoutPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(735, 2);
            this.Name = "AnalysisReportingTargets";
            this.Size = new System.Drawing.Size(735, 50);
            this.OuterLayoutPanel.ResumeLayout(false);
            this.TitleLayout.ResumeLayout(false);
            this.TitleLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel OuterLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel TitleLayout;
        private System.Windows.Forms.Label AnalysisTitle;
        private System.Windows.Forms.FlowLayoutPanel Data;
        private System.Windows.Forms.CheckBox CheckBox;
        private System.Windows.Forms.ComboBox AnalysisBlock;
    }
}
