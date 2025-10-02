namespace GUI.Tabs.Preprocess.Conversion
{
    partial class ConversionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Convert ConvertControl;
        private new System.Windows.Forms.Button AcceptButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversionForm));
            this.ConvertControl = new GUI.Tabs.Preprocess.Conversion.Convert();
            this.AcceptButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConvertControl
            // 
            this.ConvertControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.ConvertControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ConvertControl.Location = new System.Drawing.Point(12, 22);
            this.ConvertControl.Margin = new System.Windows.Forms.Padding(0);
            this.ConvertControl.Name = "ConvertControl";
            this.ConvertControl.Size = new System.Drawing.Size(797, 273);
            this.ConvertControl.TabIndex = 0;
            // 
            // AcceptButton
            // 
            this.AcceptButton.Location = new System.Drawing.Point(669, 258);
            this.AcceptButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AcceptButton.Name = "AcceptButton";
            this.AcceptButton.Size = new System.Drawing.Size(133, 34);
            this.AcceptButton.TabIndex = 1;
            this.AcceptButton.Text = "Accept";
            this.AcceptButton.UseVisualStyleBackColor = true;
            this.AcceptButton.Click += new System.EventHandler(this.AcceptButtonClick);
            // 
            // ConversionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(819, 308);
            this.Controls.Add(this.AcceptButton);
            this.Controls.Add(this.ConvertControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ConversionForm";
            this.Text = "ConversionForm";
            this.ResumeLayout(false);

        }

        #endregion
    }
}