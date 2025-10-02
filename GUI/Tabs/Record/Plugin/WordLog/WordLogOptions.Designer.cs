using System.Windows.Forms;

namespace GUI.Tabs.Record.Plugin.WordLog {
	partial class WordLogOptions {
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
            this.WordDocDialog = new System.Windows.Forms.OpenFileDialog();
            this.ExistingDocBrowseButton = new System.Windows.Forms.Button();
            this.WordNewDoc = new System.Windows.Forms.RadioButton();
            this.PreviousDocBtn = new System.Windows.Forms.RadioButton();
            this.PreviousDocTextBox = new System.Windows.Forms.TextBox();
            this.ExistingDocTextField = new System.Windows.Forms.TextBox();
            this.OpenDocBtn = new System.Windows.Forms.RadioButton();
            this.DocumentBox = new System.Windows.Forms.GroupBox();
            this.DocumentBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // WordDocDialog
            // 
            this.WordDocDialog.CheckFileExists = false;
            this.WordDocDialog.DefaultExt = "docx";
            this.WordDocDialog.Filter = "Word Documents | *.doc;*.docx; *.docm";
            this.WordDocDialog.Title = "Select a Word document to edit";
            // 
            // ExistingDocBrowseButton
            // 
            this.ExistingDocBrowseButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.ExistingDocBrowseButton.Location = new System.Drawing.Point(424, 81);
            this.ExistingDocBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.ExistingDocBrowseButton.Name = "ExistingDocBrowseButton";
            this.ExistingDocBrowseButton.Size = new System.Drawing.Size(68, 28);
            this.ExistingDocBrowseButton.TabIndex = 3;
            this.ExistingDocBrowseButton.UseVisualStyleBackColor = true;
            this.ExistingDocBrowseButton.Click += new System.EventHandler(this.ExistingDocBrowseButtonClick);
            // 
            // WordNewDoc
            // 
            this.WordNewDoc.AutoSize = true;
            this.WordNewDoc.Checked = true;
            this.WordNewDoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.WordNewDoc.Location = new System.Drawing.Point(12, 27);
            this.WordNewDoc.Margin = new System.Windows.Forms.Padding(4);
            this.WordNewDoc.Name = "WordNewDoc";
            this.WordNewDoc.Size = new System.Drawing.Size(97, 17);
            this.WordNewDoc.TabIndex = 4;
            this.WordNewDoc.TabStop = true;
            this.WordNewDoc.Text = "New document";
            this.WordNewDoc.UseVisualStyleBackColor = true;
            this.WordNewDoc.CheckedChanged += new System.EventHandler(this.WordNewDocCheckedChanged);
            // 
            // PreviousDocBtn
            // 
            this.PreviousDocBtn.Location = new System.Drawing.Point(12, 115);
            this.PreviousDocBtn.Name = "PreviousDocBtn";
            this.PreviousDocBtn.Size = new System.Drawing.Size(149, 24);
            this.PreviousDocBtn.TabIndex = 5;
            this.PreviousDocBtn.Text = "Previous document";
            this.PreviousDocBtn.CheckedChanged += new System.EventHandler(this.PreviousDocBtnCheckedChanged);
            // 
            // PreviousDocTextBox
            // 
            this.PreviousDocTextBox.Location = new System.Drawing.Point(11, 145);
            this.PreviousDocTextBox.Name = "PreviousDocTextBox";
            this.PreviousDocTextBox.Size = new System.Drawing.Size(382, 20);
            this.PreviousDocTextBox.TabIndex = 6;
            // 
            // ExistingDocTextField
            // 
            this.ExistingDocTextField.Location = new System.Drawing.Point(11, 86);
            this.ExistingDocTextField.Name = "ExistingDocTextField";
            this.ExistingDocTextField.Size = new System.Drawing.Size(382, 20);
            this.ExistingDocTextField.TabIndex = 7;
            // 
            // OpenDocBtn
            // 
            this.OpenDocBtn.Location = new System.Drawing.Point(12, 56);
            this.OpenDocBtn.Name = "OpenDocBtn";
            this.OpenDocBtn.Size = new System.Drawing.Size(149, 24);
            this.OpenDocBtn.TabIndex = 8;
            this.OpenDocBtn.Text = "Existing document";
            this.OpenDocBtn.CheckedChanged += new System.EventHandler(this.OpenDocBtnCheckedChanged);
            // 
            // DocumentBox
            // 
            this.DocumentBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DocumentBox.Controls.Add(this.WordNewDoc);
            this.DocumentBox.Controls.Add(this.ExistingDocBrowseButton);
            this.DocumentBox.Controls.Add(this.PreviousDocBtn);
            this.DocumentBox.Controls.Add(this.PreviousDocTextBox);
            this.DocumentBox.Controls.Add(this.ExistingDocTextField);
            this.DocumentBox.Controls.Add(this.OpenDocBtn);
            this.DocumentBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DocumentBox.Location = new System.Drawing.Point(0, 0);
            this.DocumentBox.Margin = new System.Windows.Forms.Padding(0);
            this.DocumentBox.Name = "DocumentBox";
            this.DocumentBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.DocumentBox.Size = new System.Drawing.Size(500, 200);
            this.DocumentBox.TabIndex = 7;
            this.DocumentBox.TabStop = false;
            this.DocumentBox.Text = "Document Logging";
            // 
            // WordLogOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.DocumentBox);
            this.Enabled = true;
            this.Name = "WordLogOptions";
            this.Size = new System.Drawing.Size(500, 200);
            this.DocumentBox.ResumeLayout(false);
            this.DocumentBox.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog WordDocDialog;
        private System.Windows.Forms.Button ExistingDocBrowseButton;
        private System.Windows.Forms.RadioButton PreviousDocBtn;
        private System.Windows.Forms.GroupBox DocumentBox;
        private System.Windows.Forms.TextBox PreviousDocTextBox;
	    public  System.Windows.Forms.TextBox ExistingDocTextField;
        private System.Windows.Forms.RadioButton WordNewDoc;
	    public System.Windows.Forms.RadioButton OpenDocBtn;
	}
}
