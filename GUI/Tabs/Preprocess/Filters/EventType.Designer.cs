namespace GUI.Tabs.Preprocess.Filters
{
    partial class EventType {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.EventValueList = new System.Windows.Forms.ComboBox();
            this.EventTypeList = new System.Windows.Forms.ComboBox();
            this.ActionList = new System.Windows.Forms.ComboBox();
            this.ActionLabel = new System.Windows.Forms.Label();
            this.EventTypeLabel = new System.Windows.Forms.Label();
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(282, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Event Value";
            // 
            // EventValueList
            // 
            this.EventValueList.Enabled = false;
            this.EventValueList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.EventValueList.FormattingEnabled = true;
            this.EventValueList.Location = new System.Drawing.Point(285, 70);
            this.EventValueList.Margin = new System.Windows.Forms.Padding(4);
            this.EventValueList.Name = "EventValueList";
            this.EventValueList.Size = new System.Drawing.Size(113, 25);
            this.EventValueList.TabIndex = 16;
            this.EventValueList.Text = "Not used yet";
            // 
            // EventTypeList
            // 
            this.EventTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EventTypeList.FormattingEnabled = true;
            this.EventTypeList.IntegralHeight = false;
            this.EventTypeList.Location = new System.Drawing.Point(100, 70);
            this.EventTypeList.Margin = new System.Windows.Forms.Padding(4);
            this.EventTypeList.Name = "EventTypeList";
            this.EventTypeList.Size = new System.Drawing.Size(160, 25);
            this.EventTypeList.TabIndex = 15;
            // 
            // ActionList
            // 
            this.ActionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActionList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ActionList.FormattingEnabled = true;
            this.ActionList.Location = new System.Drawing.Point(100, 117);
            this.ActionList.Margin = new System.Windows.Forms.Padding(4);
            this.ActionList.Name = "ActionList";
            this.ActionList.Size = new System.Drawing.Size(225, 25);
            this.ActionList.TabIndex = 14;
            // 
            // ActionLabel
            // 
            this.ActionLabel.AutoSize = true;
            this.ActionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ActionLabel.Location = new System.Drawing.Point(43, 117);
            this.ActionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ActionLabel.Name = "ActionLabel";
            this.ActionLabel.Size = new System.Drawing.Size(47, 17);
            this.ActionLabel.TabIndex = 11;
            this.ActionLabel.Text = "Action";
            // 
            // EventTypeLabel
            // 
            this.EventTypeLabel.AutoSize = true;
            this.EventTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.EventTypeLabel.Location = new System.Drawing.Point(9, 70);
            this.EventTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.EventTypeLabel.Name = "EventTypeLabel";
            this.EventTypeLabel.Size = new System.Drawing.Size(80, 17);
            this.EventTypeLabel.TabIndex = 10;
            this.EventTypeLabel.Text = "Event Type";
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Enabled = false;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(181, 11);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(67, 17);
            this.MoreInfoLabel.TabIndex = 9;
            this.MoreInfoLabel.TabStop = true;
            this.MoreInfoLabel.Text = "More info";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(4, 4);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(159, 25);
            this.NameLabel.TabIndex = 8;
            this.NameLabel.Text = "Event Type Filter";
            // 
            // EventType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventValueList);
            this.Controls.Add(this.EventTypeList);
            this.Controls.Add(this.ActionList);
            this.Controls.Add(this.ActionLabel);
            this.Controls.Add(this.EventTypeLabel);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "EventType";
            this.Size = new System.Drawing.Size(400, 158);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label EventTypeLabel;
        private System.Windows.Forms.Label ActionLabel;
        private System.Windows.Forms.ComboBox ActionList;
        private System.Windows.Forms.ComboBox EventTypeList;
        private System.Windows.Forms.ComboBox EventValueList;
        private System.Windows.Forms.Label label1;
    }
}
