
namespace GUI.TemplateBuilder
{
    partial class TemplateBuilder
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
            this.btn_loadExisting = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.text_filename = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_browse = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_add_block = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.text_title = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_loadExisting
            // 
            this.btn_loadExisting.Location = new System.Drawing.Point(336, 19);
            this.btn_loadExisting.Name = "btn_loadExisting";
            this.btn_loadExisting.Size = new System.Drawing.Size(75, 30);
            this.btn_loadExisting.TabIndex = 0;
            this.btn_loadExisting.Text = "Load";
            this.btn_loadExisting.UseVisualStyleBackColor = true;
            this.btn_loadExisting.Click += new System.EventHandler(this.btn_loadExisting_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.CheckPathExists = false;
            this.openFileDialog1.Filter = "report template files (*.template)|*.template";
            this.openFileDialog1.InitialDirectory = "./";
            // 
            // text_filename
            // 
            this.text_filename.Location = new System.Drawing.Point(60, 25);
            this.text_filename.Name = "text_filename";
            this.text_filename.Size = new System.Drawing.Size(213, 20);
            this.text_filename.TabIndex = 1;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(429, 19);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 30);
            this.btn_save.TabIndex = 2;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btn_browse);
            this.panel1.Controls.Add(this.btn_loadExisting);
            this.panel1.Controls.Add(this.btn_save);
            this.panel1.Controls.Add(this.text_filename);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(927, 59);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filename";
            // 
            // btn_browse
            // 
            this.btn_browse.Image = global::GUI.Properties.Resources.folder_explore;
            this.btn_browse.Location = new System.Drawing.Point(279, 19);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(36, 30);
            this.btn_browse.TabIndex = 3;
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(12, 148);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(640, 389);
            this.panel2.TabIndex = 4;
            // 
            // btn_add_block
            // 
            this.btn_add_block.Location = new System.Drawing.Point(12, 556);
            this.btn_add_block.Name = "btn_add_block";
            this.btn_add_block.Size = new System.Drawing.Size(100, 31);
            this.btn_add_block.TabIndex = 5;
            this.btn_add_block.Text = "Add Block";
            this.btn_add_block.UseVisualStyleBackColor = true;
            this.btn_add_block.Click += new System.EventHandler(this.btn_add_block_Click);
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.Location = new System.Drawing.Point(672, 148);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(273, 389);
            this.panel3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Report Title";
            // 
            // text_title
            // 
            this.text_title.Location = new System.Drawing.Point(85, 97);
            this.text_title.Name = "text_title";
            this.text_title.Size = new System.Drawing.Size(272, 20);
            this.text_title.TabIndex = 8;
            // 
            // TemplateBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 599);
            this.Controls.Add(this.text_title);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btn_add_block);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TemplateBuilder";
            this.Text = "TemplateBuilder";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_loadExisting;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox text_filename;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_browse;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_add_block;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox text_title;
    }
}