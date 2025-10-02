namespace GUI.Tabs.Analyze.Reporting
{
    partial class GenerateReports
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
            this.OuterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ImportButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.CommunicationLabel = new System.Windows.Forms.Label();
            this.SelectButtonLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.EditBlocks = new System.Windows.Forms.Button();
            this.InnerLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OuterLayout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SelectButtonLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // OuterLayout
            // 
            this.OuterLayout.ColumnCount = 1;
            this.OuterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OuterLayout.Controls.Add(this.tableLayoutPanel1, 0, 3);
            this.OuterLayout.Controls.Add(this.CommunicationLabel, 0, 0);
            this.OuterLayout.Controls.Add(this.SelectButtonLayout, 0, 1);
            this.OuterLayout.Controls.Add(this.InnerLayoutPanel, 0, 2);
            this.OuterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.OuterLayout.Location = new System.Drawing.Point(0, 0);
            this.OuterLayout.Name = "OuterLayout";
            this.OuterLayout.RowCount = 4;
            this.OuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.71429F));
            this.OuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.OuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.28571F));
            this.OuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.OuterLayout.Size = new System.Drawing.Size(784, 562);
            this.OuterLayout.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Controls.Add(this.ImportButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ExportButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.OkButton, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.CancelButton, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 526);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(15, 0, 15, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(754, 29);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(0, 0);
            this.ImportButton.Margin = new System.Windows.Forms.Padding(0);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(115, 28);
            this.ImportButton.TabIndex = 0;
            this.ImportButton.Text = "Import Configuration";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButtonClick);
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(125, 0);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(115, 28);
            this.ExportButton.TabIndex = 1;
            this.ExportButton.Text = "Export Configuration";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButtonClick);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(599, 1);
            this.OkButton.Margin = new System.Windows.Forms.Padding(0);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 28);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(679, 1);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 28);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // CommunicationLabel
            // 
            this.CommunicationLabel.AutoSize = true;
            this.CommunicationLabel.BackColor = System.Drawing.Color.LightBlue;
            this.CommunicationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CommunicationLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommunicationLabel.Image = global::GUI.Properties.Resources.information_icon;
            this.CommunicationLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CommunicationLabel.Location = new System.Drawing.Point(15, 5);
            this.CommunicationLabel.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
            this.CommunicationLabel.Name = "CommunicationLabel";
            this.CommunicationLabel.Size = new System.Drawing.Size(754, 41);
            this.CommunicationLabel.TabIndex = 0;
            this.CommunicationLabel.Text = "This is a description of what you must do to generate reports.";
            this.CommunicationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CommunicationLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.CommunicationLabelPaint);
            // 
            // SelectButtonLayout
            // 
            this.SelectButtonLayout.ColumnCount = 5;
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.SelectButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SelectButtonLayout.Controls.Add(this.SelectAllButton, 0, 0);
            this.SelectButtonLayout.Controls.Add(this.DeselectAllButton, 1, 0);
            this.SelectButtonLayout.Controls.Add(this.EditBlocks, 4, 0);
            this.SelectButtonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectButtonLayout.Location = new System.Drawing.Point(15, 66);
            this.SelectButtonLayout.Margin = new System.Windows.Forms.Padding(15, 15, 15, 0);
            this.SelectButtonLayout.Name = "SelectButtonLayout";
            this.SelectButtonLayout.RowCount = 1;
            this.SelectButtonLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SelectButtonLayout.Size = new System.Drawing.Size(754, 30);
            this.SelectButtonLayout.TabIndex = 1;
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Location = new System.Drawing.Point(0, 0);
            this.SelectAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(75, 28);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButtonClick);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(80, 0);
            this.DeselectAllButton.Margin = new System.Windows.Forms.Padding(0);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(75, 28);
            this.DeselectAllButton.TabIndex = 1;
            this.DeselectAllButton.Text = "Deselect All";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButtonClick);
            // 
            // EditBlocks
            // 
            this.EditBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.EditBlocks.Location = new System.Drawing.Point(594, 1);
            this.EditBlocks.Margin = new System.Windows.Forms.Padding(0);
            this.EditBlocks.Name = "EditBlocks";
            this.EditBlocks.Size = new System.Drawing.Size(160, 28);
            this.EditBlocks.TabIndex = 2;
            this.EditBlocks.Text = "Edit Blocks";
            this.EditBlocks.UseVisualStyleBackColor = true;
            this.EditBlocks.Click += new System.EventHandler(this.EditBlocksClick);
            // 
            // InnerLayoutPanel
            // 
            this.InnerLayoutPanel.AutoScroll = true;
            this.InnerLayoutPanel.AutoScrollMinSize = new System.Drawing.Size(600, 300);
            this.InnerLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.InnerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.InnerLayoutPanel.Location = new System.Drawing.Point(15, 101);
            this.InnerLayoutPanel.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
            this.InnerLayoutPanel.Name = "InnerLayoutPanel";
            this.InnerLayoutPanel.Size = new System.Drawing.Size(754, 420);
            this.InnerLayoutPanel.TabIndex = 3;
            this.InnerLayoutPanel.WrapContents = false;
            // 
            // GenerateReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.OuterLayout);
            this.MinimumSize = new System.Drawing.Size(500, 450);
            this.Name = "GenerateReports";
            this.ShowIcon = false;
            this.Text = "Generate reports";
            this.OuterLayout.ResumeLayout(false);
            this.OuterLayout.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.SelectButtonLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel OuterLayout;
        private System.Windows.Forms.Label CommunicationLabel;
        private System.Windows.Forms.TableLayoutPanel SelectButtonLayout;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.FlowLayoutPanel InnerLayoutPanel;
        private System.Windows.Forms.Button EditBlocks;
    }
}