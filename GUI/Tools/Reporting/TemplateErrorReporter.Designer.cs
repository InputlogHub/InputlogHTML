namespace GUI.Tools.Reporting
{
    partial class TemplateErrorReporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateErrorReporter));
            this.Layout = new System.Windows.Forms.TableLayoutPanel();
            this.InfoLbl = new System.Windows.Forms.Label();
            this.TemplateNameLbl = new System.Windows.Forms.Label();
            this.TemplateSelector = new System.Windows.Forms.ComboBox();
            this.Data = new System.Windows.Forms.DataGridView();
            this.CloseButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.Layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Data)).BeginInit();
            this.SuspendLayout();
            // 
            // Layout
            // 
            this.Layout.ColumnCount = 2;
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.Controls.Add(this.InfoLbl, 0, 0);
            this.Layout.Controls.Add(this.TemplateNameLbl, 0, 1);
            this.Layout.Controls.Add(this.TemplateSelector, 1, 1);
            this.Layout.Controls.Add(this.Data, 0, 2);
            this.Layout.Controls.Add(this.CloseButton, 1, 3);
            this.Layout.Controls.Add(this.RefreshButton, 0, 3);
            this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Layout.Location = new System.Drawing.Point(0, 0);
            this.Layout.Margin = new System.Windows.Forms.Padding(0);
            this.Layout.Name = "Layout";
            this.Layout.RowCount = 4;
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.Layout.Size = new System.Drawing.Size(750, 468);
            this.Layout.TabIndex = 0;
            // 
            // InfoLbl
            // 
            this.InfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoLbl.AutoSize = true;
            this.Layout.SetColumnSpan(this.InfoLbl, 2);
            this.InfoLbl.Location = new System.Drawing.Point(5, 11);
            this.InfoLbl.Margin = new System.Windows.Forms.Padding(5, 10, 3, 0);
            this.InfoLbl.Name = "InfoLbl";
            this.InfoLbl.Size = new System.Drawing.Size(742, 13);
            this.InfoLbl.TabIndex = 0;
            this.InfoLbl.Text = "This form shows you which errors you have in each of your report template files.";
            // 
            // TemplateNameLbl
            // 
            this.TemplateNameLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TemplateNameLbl.AutoSize = true;
            this.TemplateNameLbl.Location = new System.Drawing.Point(3, 37);
            this.TemplateNameLbl.Name = "TemplateNameLbl";
            this.TemplateNameLbl.Size = new System.Drawing.Size(94, 13);
            this.TemplateNameLbl.TabIndex = 1;
            this.TemplateNameLbl.Text = "Template:";
            // 
            // TemplateSelector
            // 
            this.TemplateSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TemplateSelector.FormattingEnabled = true;
            this.TemplateSelector.Location = new System.Drawing.Point(100, 33);
            this.TemplateSelector.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this.TemplateSelector.Name = "TemplateSelector";
            this.TemplateSelector.Size = new System.Drawing.Size(640, 21);
            this.TemplateSelector.TabIndex = 2;
            // 
            // Data
            // 
            this.Data.AllowUserToAddRows = false;
            this.Data.AllowUserToDeleteRows = false;
            this.Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Layout.SetColumnSpan(this.Data, 2);
            this.Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Data.Location = new System.Drawing.Point(5, 71);
            this.Data.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.Data.Name = "Data";
            this.Data.ReadOnly = true;
            this.Data.Size = new System.Drawing.Size(735, 362);
            this.Data.TabIndex = 3;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(665, 439);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 4;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // RefreshButton
            // 
            this.RefreshButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RefreshButton.Location = new System.Drawing.Point(15, 439);
            this.RefreshButton.Margin = new System.Windows.Forms.Padding(15, 3, 3, 3);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshButton.TabIndex = 5;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // TemplateErrorReporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(750, 468);
            this.Controls.Add(this.Layout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "TemplateErrorReporter";
            this.Text = "Template Errors";
            this.Layout.ResumeLayout(false);
            this.Layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Data)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Layout;
        private System.Windows.Forms.Label InfoLbl;
        private System.Windows.Forms.Label TemplateNameLbl;
        private System.Windows.Forms.ComboBox TemplateSelector;
        private System.Windows.Forms.DataGridView Data;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button RefreshButton;
    }
}