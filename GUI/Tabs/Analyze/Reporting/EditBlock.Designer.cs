namespace GUI.Tabs.Analyze.Reporting
{
    partial class EditBlock
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditBlock));
            this.InnerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.CheckBox = new System.Windows.Forms.CheckBox();
            this.OldValueField = new System.Windows.Forms.TextBox();
            this.NewValueField = new System.Windows.Forms.TextBox();
            this.ProvidedInputValid = new System.Windows.Forms.ErrorProvider(this.components);
            this.ProvidedInputInvalid = new System.Windows.Forms.ErrorProvider(this.components);
            this.InnerLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProvidedInputValid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProvidedInputInvalid)).BeginInit();
            this.SuspendLayout();
            // 
            // InnerLayout
            // 
            this.InnerLayout.ColumnCount = 4;
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.InnerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.InnerLayout.Controls.Add(this.CheckBox, 0, 0);
            this.InnerLayout.Controls.Add(this.OldValueField, 1, 0);
            this.InnerLayout.Controls.Add(this.NewValueField, 2, 0);
            this.InnerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerLayout.Location = new System.Drawing.Point(0, 0);
            this.InnerLayout.Name = "InnerLayout";
            this.InnerLayout.RowCount = 1;
            this.InnerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InnerLayout.Size = new System.Drawing.Size(516, 28);
            this.InnerLayout.TabIndex = 0;
            // 
            // CheckBox
            // 
            this.CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBox.AutoSize = true;
            this.CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox.Location = new System.Drawing.Point(0, 7);
            this.CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Size = new System.Drawing.Size(25, 14);
            this.CheckBox.TabIndex = 0;
            this.CheckBox.UseVisualStyleBackColor = true;
            this.CheckBox.CheckedChanged += new System.EventHandler(this.CheckBoxCheckedChanged);
            // 
            // OldValueField
            // 
            this.OldValueField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.OldValueField.Location = new System.Drawing.Point(30, 4);
            this.OldValueField.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.OldValueField.Name = "OldValueField";
            this.OldValueField.Size = new System.Drawing.Size(220, 20);
            this.OldValueField.TabIndex = 1;
            // 
            // NewValueField
            // 
            this.NewValueField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.NewValueField.Location = new System.Drawing.Point(260, 4);
            this.NewValueField.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.NewValueField.Name = "NewValueField";
            this.NewValueField.Size = new System.Drawing.Size(220, 20);
            this.NewValueField.TabIndex = 2;
            // 
            // ProvidedInputValid
            // 
            this.ProvidedInputValid.ContainerControl = this;
            this.ProvidedInputValid.Icon = ((System.Drawing.Icon)(resources.GetObject("ProvidedInputValid.Icon")));
            // 
            // ProvidedInputInvalid
            // 
            this.ProvidedInputInvalid.ContainerControl = this;
            // 
            // EditBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.InnerLayout);
            this.Name = "EditBlock";
            this.Size = new System.Drawing.Size(516, 28);
            this.InnerLayout.ResumeLayout(false);
            this.InnerLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProvidedInputValid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProvidedInputInvalid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel InnerLayout;
        private System.Windows.Forms.CheckBox CheckBox;
        private System.Windows.Forms.TextBox OldValueField;
        private System.Windows.Forms.TextBox NewValueField;
        private System.Windows.Forms.ErrorProvider ProvidedInputValid;
        private System.Windows.Forms.ErrorProvider ProvidedInputInvalid;
    }
}
