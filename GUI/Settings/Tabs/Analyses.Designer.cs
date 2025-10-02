namespace GUI.Settings.Tabs
{
    partial class Analyses
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.disablePersMax = new System.Windows.Forms.CheckBox();
            this.resetPersMaxima = new System.Windows.Forms.Button();
            this.personalMaxLabel = new System.Windows.Forms.Label();
            this.personalMax = new System.Windows.Forms.NumericUpDown();
            this.absoluteMaxLabel = new System.Windows.Forms.Label();
            this.absoluteMax = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RecognizeDoubleClicksUpPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.RecognizeDoubleClicksUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.RecognizeDoubleClicksCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AddAllBtn = new System.Windows.Forms.Button();
            this.RemoveAllBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupedToAvailableButton = new System.Windows.Forms.Button();
            this.AvailableToGroupedButton = new System.Windows.Forms.Button();
            this.AvailableKeys = new System.Windows.Forms.ListBox();
            this.GroupedKeys = new System.Windows.Forms.ListBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personalMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.absoluteMax)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.RecognizeDoubleClicksUpPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RecognizeDoubleClicksUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.disablePersMax);
            this.groupBox3.Controls.Add(this.resetPersMaxima);
            this.groupBox3.Controls.Add(this.personalMaxLabel);
            this.groupBox3.Controls.Add(this.personalMax);
            this.groupBox3.Controls.Add(this.absoluteMaxLabel);
            this.groupBox3.Controls.Add(this.absoluteMax);
            this.groupBox3.Location = new System.Drawing.Point(3, 319);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(447, 121);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fluency Analysis";
            // 
            // disablePersMax
            // 
            this.disablePersMax.AutoSize = true;
            this.disablePersMax.Location = new System.Drawing.Point(199, 75);
            this.disablePersMax.Name = "disablePersMax";
            this.disablePersMax.Size = new System.Drawing.Size(188, 17);
            this.disablePersMax.TabIndex = 31;
            this.disablePersMax.Text = "Disable Personal Maximum Storing";
            this.disablePersMax.UseVisualStyleBackColor = true;
            // 
            // resetPersMaxima
            // 
            this.resetPersMaxima.Location = new System.Drawing.Point(24, 75);
            this.resetPersMaxima.Name = "resetPersMaxima";
            this.resetPersMaxima.Size = new System.Drawing.Size(140, 23);
            this.resetPersMaxima.TabIndex = 30;
            this.resetPersMaxima.Text = "Reset Personal Maxima";
            this.resetPersMaxima.UseVisualStyleBackColor = true;
            this.resetPersMaxima.Click += new System.EventHandler(this.resetPersOptima_Click);
            // 
            // personalMaxLabel
            // 
            this.personalMaxLabel.AutoSize = true;
            this.personalMaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.personalMaxLabel.Location = new System.Drawing.Point(196, 20);
            this.personalMaxLabel.Margin = new System.Windows.Forms.Padding(4);
            this.personalMaxLabel.Name = "personalMaxLabel";
            this.personalMaxLabel.Size = new System.Drawing.Size(132, 13);
            this.personalMaxLabel.TabIndex = 29;
            this.personalMaxLabel.Text = "Default Personal Maximum";
            // 
            // personalMax
            // 
            this.personalMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.personalMax.Location = new System.Drawing.Point(199, 37);
            this.personalMax.Margin = new System.Windows.Forms.Padding(4);
            this.personalMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.personalMax.Name = "personalMax";
            this.personalMax.Size = new System.Drawing.Size(95, 20);
            this.personalMax.TabIndex = 28;
            this.personalMax.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // absoluteMaxLabel
            // 
            this.absoluteMaxLabel.AutoSize = true;
            this.absoluteMaxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.absoluteMaxLabel.Location = new System.Drawing.Point(21, 20);
            this.absoluteMaxLabel.Margin = new System.Windows.Forms.Padding(4);
            this.absoluteMaxLabel.Name = "absoluteMaxLabel";
            this.absoluteMaxLabel.Size = new System.Drawing.Size(95, 13);
            this.absoluteMaxLabel.TabIndex = 27;
            this.absoluteMaxLabel.Text = "Absolute Maximum";
            // 
            // absoluteMax
            // 
            this.absoluteMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.absoluteMax.Location = new System.Drawing.Point(24, 37);
            this.absoluteMax.Margin = new System.Windows.Forms.Padding(4);
            this.absoluteMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.absoluteMax.Name = "absoluteMax";
            this.absoluteMax.Size = new System.Drawing.Size(95, 20);
            this.absoluteMax.TabIndex = 26;
            this.absoluteMax.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.RecognizeDoubleClicksUpPanel);
            this.groupBox2.Controls.Add(this.RecognizeDoubleClicksCheckbox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(447, 90);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Event Grouping";
            // 
            // RecognizeDoubleClicksUpPanel
            // 
            this.RecognizeDoubleClicksUpPanel.Controls.Add(this.label3);
            this.RecognizeDoubleClicksUpPanel.Controls.Add(this.RecognizeDoubleClicksUpDown);
            this.RecognizeDoubleClicksUpPanel.Controls.Add(this.label4);
            this.RecognizeDoubleClicksUpPanel.Enabled = false;
            this.RecognizeDoubleClicksUpPanel.Location = new System.Drawing.Point(22, 42);
            this.RecognizeDoubleClicksUpPanel.Name = "RecognizeDoubleClicksUpPanel";
            this.RecognizeDoubleClicksUpPanel.Size = new System.Drawing.Size(186, 31);
            this.RecognizeDoubleClicksUpPanel.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Pause Threshold";
            // 
            // RecognizeDoubleClicksUpDown
            // 
            this.RecognizeDoubleClicksUpDown.Location = new System.Drawing.Point(96, 8);
            this.RecognizeDoubleClicksUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.RecognizeDoubleClicksUpDown.Name = "RecognizeDoubleClicksUpDown";
            this.RecognizeDoubleClicksUpDown.Size = new System.Drawing.Size(55, 20);
            this.RecognizeDoubleClicksUpDown.TabIndex = 0;
            this.RecognizeDoubleClicksUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(157, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "ms";
            // 
            // RecognizeDoubleClicksCheckbox
            // 
            this.RecognizeDoubleClicksCheckbox.AutoSize = true;
            this.RecognizeDoubleClicksCheckbox.Checked = true;
            this.RecognizeDoubleClicksCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RecognizeDoubleClicksCheckbox.Location = new System.Drawing.Point(9, 19);
            this.RecognizeDoubleClicksCheckbox.Name = "RecognizeDoubleClicksCheckbox";
            this.RecognizeDoubleClicksCheckbox.Size = new System.Drawing.Size(142, 17);
            this.RecognizeDoubleClicksCheckbox.TabIndex = 3;
            this.RecognizeDoubleClicksCheckbox.Text = "Recognize double clicks";
            this.RecognizeDoubleClicksCheckbox.UseVisualStyleBackColor = true;
            this.RecognizeDoubleClicksCheckbox.CheckedChanged += new System.EventHandler(this.RecognizeDoubleClicksCheckboxCheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AddAllBtn);
            this.groupBox1.Controls.Add(this.RemoveAllBtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.GroupedToAvailableButton);
            this.groupBox1.Controls.Add(this.AvailableToGroupedButton);
            this.groupBox1.Controls.Add(this.AvailableKeys);
            this.groupBox1.Controls.Add(this.GroupedKeys);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBox1.Location = new System.Drawing.Point(3, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 213);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Consecutive Key Grouping";
            // 
            // AddAllBtn
            // 
            this.AddAllBtn.Location = new System.Drawing.Point(163, 123);
            this.AddAllBtn.Name = "AddAllBtn";
            this.AddAllBtn.Size = new System.Drawing.Size(30, 26);
            this.AddAllBtn.TabIndex = 7;
            this.AddAllBtn.Text = ">>";
            this.AddAllBtn.UseVisualStyleBackColor = true;
            this.AddAllBtn.Click += new System.EventHandler(this.GroupAllButtonClick);
            // 
            // RemoveAllBtn
            // 
            this.RemoveAllBtn.Location = new System.Drawing.Point(163, 79);
            this.RemoveAllBtn.Name = "RemoveAllBtn";
            this.RemoveAllBtn.Size = new System.Drawing.Size(30, 26);
            this.RemoveAllBtn.TabIndex = 6;
            this.RemoveAllBtn.Text = "<<";
            this.RemoveAllBtn.UseVisualStyleBackColor = true;
            this.RemoveAllBtn.Click += new System.EventHandler(this.RemoveAllButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Grouped keys";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Available keys";
            // 
            // GroupedToAvailableButton
            // 
            this.GroupedToAvailableButton.Location = new System.Drawing.Point(163, 47);
            this.GroupedToAvailableButton.Name = "GroupedToAvailableButton";
            this.GroupedToAvailableButton.Size = new System.Drawing.Size(30, 26);
            this.GroupedToAvailableButton.TabIndex = 3;
            this.GroupedToAvailableButton.Text = "<";
            this.GroupedToAvailableButton.UseVisualStyleBackColor = true;
            this.GroupedToAvailableButton.Click += new System.EventHandler(this.GroupedToAvailableButtonClick);
            // 
            // AvailableToGroupedButton
            // 
            this.AvailableToGroupedButton.Location = new System.Drawing.Point(163, 155);
            this.AvailableToGroupedButton.Name = "AvailableToGroupedButton";
            this.AvailableToGroupedButton.Size = new System.Drawing.Size(30, 26);
            this.AvailableToGroupedButton.TabIndex = 2;
            this.AvailableToGroupedButton.Text = ">";
            this.AvailableToGroupedButton.UseVisualStyleBackColor = true;
            this.AvailableToGroupedButton.Click += new System.EventHandler(this.AvailableToGroupedButtonClick);
            // 
            // AvailableKeys
            // 
            this.AvailableKeys.DisplayMember = "Key";
            this.AvailableKeys.FormattingEnabled = true;
            this.AvailableKeys.Location = new System.Drawing.Point(24, 47);
            this.AvailableKeys.Name = "AvailableKeys";
            this.AvailableKeys.Size = new System.Drawing.Size(133, 134);
            this.AvailableKeys.Sorted = true;
            this.AvailableKeys.TabIndex = 1;
            this.AvailableKeys.ValueMember = "Value";
            // 
            // GroupedKeys
            // 
            this.GroupedKeys.DisplayMember = "Key";
            this.GroupedKeys.FormattingEnabled = true;
            this.GroupedKeys.Location = new System.Drawing.Point(199, 47);
            this.GroupedKeys.Name = "GroupedKeys";
            this.GroupedKeys.Size = new System.Drawing.Size(139, 134);
            this.GroupedKeys.Sorted = true;
            this.GroupedKeys.TabIndex = 0;
            this.GroupedKeys.ValueMember = "Value";
            // 
            // Analyses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Analyses";
            this.Size = new System.Drawing.Size(453, 443);
            this.Load += new System.EventHandler(this.AnalysesLoad);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.personalMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.absoluteMax)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.RecognizeDoubleClicksUpPanel.ResumeLayout(false);
            this.RecognizeDoubleClicksUpPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RecognizeDoubleClicksUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GroupedToAvailableButton;
        private System.Windows.Forms.Button AvailableToGroupedButton;
        private System.Windows.Forms.ListBox AvailableKeys;
        private System.Windows.Forms.ListBox GroupedKeys;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel RecognizeDoubleClicksUpPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown RecognizeDoubleClicksUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox RecognizeDoubleClicksCheckbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label personalMaxLabel;
        private System.Windows.Forms.NumericUpDown personalMax;
        private System.Windows.Forms.Label absoluteMaxLabel;
        private System.Windows.Forms.NumericUpDown absoluteMax;
        private System.Windows.Forms.Button resetPersMaxima;
        private System.Windows.Forms.CheckBox disablePersMax;
        private System.Windows.Forms.Button AddAllBtn;
        private System.Windows.Forms.Button RemoveAllBtn;
    }
}
