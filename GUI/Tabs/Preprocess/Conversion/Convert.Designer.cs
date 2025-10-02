namespace GUI.Tabs.Preprocess.Conversion
{
    partial class Convert {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ConvertOutputFormatList = new System.Windows.Forms.ComboBox();
            this.ConvertOutpuFormatLabel = new System.Windows.Forms.Label();
            this.ConvertInputFormatList = new System.Windows.Forms.ComboBox();
            this.ConvertInputFormatLabel = new System.Windows.Forms.Label();
            this.ConvertDestFileButton = new System.Windows.Forms.Button();
            this.ConvertDestFileLabel = new System.Windows.Forms.Label();
            this.ConvertDestFileTextField = new System.Windows.Forms.TextBox();
            this.ConvertSrcFileButton = new System.Windows.Forms.Button();
            this.ConvertSrcFileLabel = new System.Windows.Forms.Label();
            this.ConvertSrcFileTextField = new System.Windows.Forms.TextBox();
            this.ConvertSrcFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ConvertDestFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SelectionPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConvertOutputFormatList
            // 
            this.ConvertOutputFormatList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConvertOutputFormatList.DisplayMember = "Key";
            this.ConvertOutputFormatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConvertOutputFormatList.FormattingEnabled = true;
            this.ConvertOutputFormatList.Location = new System.Drawing.Point(117, 132);
            this.ConvertOutputFormatList.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.ConvertOutputFormatList.Name = "ConvertOutputFormatList";
            this.ConvertOutputFormatList.Size = new System.Drawing.Size(200, 25);
            this.ConvertOutputFormatList.TabIndex = 29;
            this.ConvertOutputFormatList.ValueMember = "Value";
            // 
            // ConvertOutpuFormatLabel
            // 
            this.ConvertOutpuFormatLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertOutpuFormatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertOutpuFormatLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertOutpuFormatLabel.Location = new System.Drawing.Point(6, 138);
            this.ConvertOutpuFormatLabel.Margin = new System.Windows.Forms.Padding(6, 12, 0, 0);
            this.ConvertOutpuFormatLabel.Name = "ConvertOutpuFormatLabel";
            this.ConvertOutpuFormatLabel.Size = new System.Drawing.Size(111, 32);
            this.ConvertOutpuFormatLabel.TabIndex = 28;
            this.ConvertOutpuFormatLabel.Text = "Format";
            // 
            // ConvertInputFormatList
            // 
            this.ConvertInputFormatList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConvertInputFormatList.DisplayMember = "Key";
            this.ConvertInputFormatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConvertInputFormatList.FormattingEnabled = true;
            this.ConvertInputFormatList.Location = new System.Drawing.Point(117, 39);
            this.ConvertInputFormatList.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.ConvertInputFormatList.Name = "ConvertInputFormatList";
            this.ConvertInputFormatList.Size = new System.Drawing.Size(200, 25);
            this.ConvertInputFormatList.TabIndex = 27;
            this.ConvertInputFormatList.ValueMember = "Value";
            // 
            // ConvertInputFormatLabel
            // 
            this.ConvertInputFormatLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertInputFormatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertInputFormatLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertInputFormatLabel.Location = new System.Drawing.Point(6, 48);
            this.ConvertInputFormatLabel.Margin = new System.Windows.Forms.Padding(6, 12, 0, 0);
            this.ConvertInputFormatLabel.Name = "ConvertInputFormatLabel";
            this.ConvertInputFormatLabel.Size = new System.Drawing.Size(111, 35);
            this.ConvertInputFormatLabel.TabIndex = 26;
            this.ConvertInputFormatLabel.Text = "Format";
            // 
            // ConvertDestFileButton
            // 
            this.ConvertDestFileButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConvertDestFileButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.ConvertDestFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertDestFileButton.Location = new System.Drawing.Point(501, 90);
            this.ConvertDestFileButton.Name = "ConvertDestFileButton";
            this.ConvertDestFileButton.Size = new System.Drawing.Size(82, 28);
            this.ConvertDestFileButton.TabIndex = 24;
            this.ConvertDestFileButton.UseVisualStyleBackColor = true;
            this.ConvertDestFileButton.Click += new System.EventHandler(this.ConvertDestFileButtonClick);
            // 
            // ConvertDestFileLabel
            // 
            this.ConvertDestFileLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertDestFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertDestFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertDestFileLabel.Location = new System.Drawing.Point(6, 95);
            this.ConvertDestFileLabel.Margin = new System.Windows.Forms.Padding(6, 12, 0, 0);
            this.ConvertDestFileLabel.Name = "ConvertDestFileLabel";
            this.ConvertDestFileLabel.Size = new System.Drawing.Size(111, 31);
            this.ConvertDestFileLabel.TabIndex = 22;
            this.ConvertDestFileLabel.Text = "Destination";
            // 
            // ConvertDestFileTextField
            // 
            this.ConvertDestFileTextField.AllowDrop = true;
            this.ConvertDestFileTextField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ConvertDestFileTextField.Location = new System.Drawing.Point(117, 93);
            this.ConvertDestFileTextField.Margin = new System.Windows.Forms.Padding(0);
            this.ConvertDestFileTextField.Name = "ConvertDestFileTextField";
            this.ConvertDestFileTextField.Size = new System.Drawing.Size(351, 23);
            this.ConvertDestFileTextField.TabIndex = 23;
            // 
            // ConvertSrcFileButton
            // 
            this.ConvertSrcFileButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConvertSrcFileButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.ConvertSrcFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertSrcFileButton.Location = new System.Drawing.Point(501, 4);
            this.ConvertSrcFileButton.Name = "ConvertSrcFileButton";
            this.ConvertSrcFileButton.Size = new System.Drawing.Size(82, 27);
            this.ConvertSrcFileButton.TabIndex = 21;
            this.ConvertSrcFileButton.UseVisualStyleBackColor = true;
            this.ConvertSrcFileButton.Click += new System.EventHandler(this.ConvertSrcFileButtonClick);
            // 
            // ConvertSrcFileLabel
            // 
            this.ConvertSrcFileLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConvertSrcFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertSrcFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConvertSrcFileLabel.Location = new System.Drawing.Point(6, 12);
            this.ConvertSrcFileLabel.Margin = new System.Windows.Forms.Padding(6, 12, 3, 3);
            this.ConvertSrcFileLabel.Name = "ConvertSrcFileLabel";
            this.ConvertSrcFileLabel.Size = new System.Drawing.Size(108, 21);
            this.ConvertSrcFileLabel.TabIndex = 19;
            this.ConvertSrcFileLabel.Text = "Source";
            // 
            // ConvertSrcFileTextField
            // 
            this.ConvertSrcFileTextField.AllowDrop = true;
            this.ConvertSrcFileTextField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ConvertSrcFileTextField.Location = new System.Drawing.Point(117, 6);
            this.ConvertSrcFileTextField.Margin = new System.Windows.Forms.Padding(0);
            this.ConvertSrcFileTextField.Name = "ConvertSrcFileTextField";
            this.ConvertSrcFileTextField.Size = new System.Drawing.Size(351, 23);
            this.ConvertSrcFileTextField.TabIndex = 20;
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.ColumnCount = 3;
            this.SelectionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.SelectionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.SelectionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.SelectionPanel.Controls.Add(this.ConvertSrcFileLabel, 0, 0);
            this.SelectionPanel.Controls.Add(this.ConvertSrcFileTextField, 1, 0);
            this.SelectionPanel.Controls.Add(this.ConvertSrcFileButton, 2, 0);
            this.SelectionPanel.Controls.Add(this.ConvertInputFormatLabel, 0, 1);
            this.SelectionPanel.Controls.Add(this.ConvertInputFormatList, 1, 1);
            this.SelectionPanel.Controls.Add(this.ConvertDestFileLabel, 0, 2);
            this.SelectionPanel.Controls.Add(this.ConvertDestFileTextField, 1, 2);
            this.SelectionPanel.Controls.Add(this.ConvertDestFileButton, 2, 2);
            this.SelectionPanel.Controls.Add(this.ConvertOutpuFormatLabel, 0, 3);
            this.SelectionPanel.Controls.Add(this.ConvertOutputFormatList, 1, 3);
            this.SelectionPanel.Location = new System.Drawing.Point(0, 0);
            this.SelectionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.RowCount = 4;
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.SelectionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.SelectionPanel.Size = new System.Drawing.Size(586, 170);
            this.SelectionPanel.TabIndex = 32;
            // 
            // Convert
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.SelectionPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Convert";
            this.Size = new System.Drawing.Size(598, 232);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ConvertDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ConvertDragEnter);
            this.SelectionPanel.ResumeLayout(false);
            this.SelectionPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ComboBox ConvertOutputFormatList;
        private System.Windows.Forms.Label ConvertOutpuFormatLabel;
        private System.Windows.Forms.ComboBox ConvertInputFormatList;
		private System.Windows.Forms.Label ConvertInputFormatLabel;
        private System.Windows.Forms.Button ConvertDestFileButton;
        private System.Windows.Forms.Label ConvertDestFileLabel;
        private System.Windows.Forms.TextBox ConvertDestFileTextField;
        private System.Windows.Forms.Button ConvertSrcFileButton;
        private System.Windows.Forms.Label ConvertSrcFileLabel;
        private System.Windows.Forms.TextBox ConvertSrcFileTextField;
        private System.Windows.Forms.OpenFileDialog ConvertSrcFileDialog;
        private System.Windows.Forms.SaveFileDialog ConvertDestFileDialog;
		private System.Windows.Forms.TableLayoutPanel SelectionPanel;
    }
}
