
namespace GUI.TemplateBuilder
{
    partial class Block
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
            this.lbl_title = new System.Windows.Forms.Label();
            this.text_title = new System.Windows.Forms.TextBox();
            this.btn_up = new System.Windows.Forms.Button();
            this.btn_down = new System.Windows.Forms.Button();
            this.btn_delete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.text_intro = new System.Windows.Forms.TextBox();
            this.text_elements = new System.Windows.Forms.RichTextBox();
            this.lbl_data = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Location = new System.Drawing.Point(90, 12);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(27, 13);
            this.lbl_title.TabIndex = 0;
            this.lbl_title.Text = "Title";
            // 
            // text_title
            // 
            this.text_title.Location = new System.Drawing.Point(138, 9);
            this.text_title.Name = "text_title";
            this.text_title.Size = new System.Drawing.Size(378, 20);
            this.text_title.TabIndex = 1;
            // 
            // btn_up
            // 
            this.btn_up.Location = new System.Drawing.Point(3, 7);
            this.btn_up.Name = "btn_up";
            this.btn_up.Size = new System.Drawing.Size(30, 30);
            this.btn_up.TabIndex = 2;
            this.btn_up.Text = "up";
            this.btn_up.UseVisualStyleBackColor = true;
            this.btn_up.Click += new System.EventHandler(this.btn_up_Click);
            // 
            // btn_down
            // 
            this.btn_down.Location = new System.Drawing.Point(3, 43);
            this.btn_down.Name = "btn_down";
            this.btn_down.Size = new System.Drawing.Size(30, 30);
            this.btn_down.TabIndex = 3;
            this.btn_down.Text = "down";
            this.btn_down.UseVisualStyleBackColor = true;
            this.btn_down.Click += new System.EventHandler(this.btn_down_Click);
            // 
            // btn_delete
            // 
            this.btn_delete.Location = new System.Drawing.Point(595, 0);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(25, 25);
            this.btn_delete.TabIndex = 4;
            this.btn_delete.Text = "X";
            this.btn_delete.UseVisualStyleBackColor = true;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Introduction";
            // 
            // text_intro
            // 
            this.text_intro.Location = new System.Drawing.Point(138, 40);
            this.text_intro.Multiline = true;
            this.text_intro.Name = "text_intro";
            this.text_intro.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.text_intro.Size = new System.Drawing.Size(378, 52);
            this.text_intro.TabIndex = 6;
            // 
            // text_elements
            // 
            this.text_elements.Location = new System.Drawing.Point(138, 107);
            this.text_elements.Multiline = true;
            this.text_elements.Name = "text_elements";
            this.text_elements.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.text_elements.Size = new System.Drawing.Size(378, 73);
            this.text_elements.TabIndex = 7;
            this.text_elements.TextChanged += Text_elements_TextChanged;
            // 
            // lbl_data
            // 
            this.lbl_data.AutoSize = true;
            this.lbl_data.Location = new System.Drawing.Point(82, 136);
            this.lbl_data.Name = "lbl_data";
            this.lbl_data.Size = new System.Drawing.Size(30, 13);
            this.lbl_data.TabIndex = 8;
            this.lbl_data.Text = "Data";
            // 
            // Block
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbl_data);
            this.Controls.Add(this.text_elements);
            this.Controls.Add(this.text_intro);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.btn_down);
            this.Controls.Add(this.btn_up);
            this.Controls.Add(this.text_title);
            this.Controls.Add(this.lbl_title);
            this.Name = "Block";
            this.Size = new System.Drawing.Size(618, 198);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.TextBox text_title;
        private System.Windows.Forms.Button btn_up;
        private System.Windows.Forms.Button btn_down;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox text_intro;
        private System.Windows.Forms.RichTextBox text_elements;
        private System.Windows.Forms.Label lbl_data;

        public string Title { 
            get {
                return text_title.Text;
            } 
        }

        public string Introduction { 
            get
            {
                return text_intro.Text;
            }
        }

        public string Elements 
        {
            get 
            {
                return text_elements.Text;
            }
        }
    }
}
