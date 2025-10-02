
namespace GUI.TemplateBuilder
{
    partial class PropertyButton
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
            this.btn_property = new System.Windows.Forms.Button();
            this.PropertyTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btn_property
            // 
            this.btn_property.Location = new System.Drawing.Point(0, 0);
            this.btn_property.Name = "btn_property";
            this.btn_property.Size = new System.Drawing.Size(250, 25);
            this.btn_property.TabIndex = 0;
            this.btn_property.UseVisualStyleBackColor = true;
            this.btn_property.Click += new System.EventHandler(this.btn_property_Click);
            // 
            // PropertyButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_property);
            this.Name = "PropertyButton";
            this.Size = new System.Drawing.Size(250, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_property;
        private System.Windows.Forms.ToolTip PropertyTooltip;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
