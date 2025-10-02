namespace GUI.Tabs.Preprocess.Filters.TimeFilterHelp
{
	partial class TimeConfiguration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeConfiguration));
            this.beginID2Tbx = new System.Windows.Forms.TextBox();
            this.infoLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fixedLabel1 = new System.Windows.Forms.Label();
            this.fixedLabel2 = new System.Windows.Forms.Label();
            this.beginID2Lbl = new System.Windows.Forms.Label();
            this.endID3Lbl = new System.Windows.Forms.Label();
            this.endID2Lbl = new System.Windows.Forms.Label();
            this.endID1Lbl = new System.Windows.Forms.Label();
            this.beginIDTbx = new System.Windows.Forms.TextBox();
            this.endID2Tbx = new System.Windows.Forms.TextBox();
            this.startTime2Lbl = new System.Windows.Forms.Label();
            this.endIDLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.fixedPanel = new System.Windows.Forms.Panel();
            this.valueLbl = new System.Windows.Forms.Label();
            this.eventValueCBx = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.fixedSkipToFinalKeyRb = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.fixedFinalIDRb = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.fixedInitialIDRb = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.fixedSkipToFirstKeyRb = new System.Windows.Forms.RadioButton();
            this.fixedLabel3 = new System.Windows.Forms.Label();
            this.fixedIdGbx2 = new System.Windows.Forms.GroupBox();
            this.fixedKeepTimeRb = new System.Windows.Forms.RadioButton();
            this.fixedZeroTimeRb = new System.Windows.Forms.RadioButton();
            this.idPanel = new System.Windows.Forms.Panel();
            this.idGbx2 = new System.Windows.Forms.GroupBox();
            this.endTime2Lbl = new System.Windows.Forms.Label();
            this.id1Gbx = new System.Windows.Forms.GroupBox();
            this.beginID1Lbl = new System.Windows.Forms.Label();
            this.endIDTbx = new System.Windows.Forms.TextBox();
            this.startTimeLbl = new System.Windows.Forms.Label();
            this.endTimeLbl = new System.Windows.Forms.Label();
            this.selectTimeRb = new System.Windows.Forms.RadioButton();
            this.selectIDRb = new System.Windows.Forms.RadioButton();
            this.EventErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.AcceptBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.timePanel = new System.Windows.Forms.Panel();
            this.timeGbx2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.beginTime2Tbx = new System.Windows.Forms.TextBox();
            this.event2Lbl = new System.Windows.Forms.Label();
            this.event3Lbl = new System.Windows.Forms.Label();
            this.endTime2Tbx = new System.Windows.Forms.TextBox();
            this.startID2Lbl = new System.Windows.Forms.Label();
            this.timeGbx1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.beginTimeTbx = new System.Windows.Forms.TextBox();
            this.eventLbl2 = new System.Windows.Forms.Label();
            this.eventLbl = new System.Windows.Forms.Label();
            this.endTimeTbx = new System.Windows.Forms.TextBox();
            this.startIDLbl = new System.Windows.Forms.Label();
            this.fixedPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.fixedIdGbx2.SuspendLayout();
            this.idPanel.SuspendLayout();
            this.idGbx2.SuspendLayout();
            this.id1Gbx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EventErrorProvider)).BeginInit();
            this.timePanel.SuspendLayout();
            this.timeGbx2.SuspendLayout();
            this.timeGbx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // beginID2Tbx
            // 
            this.beginID2Tbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginID2Tbx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.beginID2Tbx.Location = new System.Drawing.Point(135, 32);
            this.beginID2Tbx.Margin = new System.Windows.Forms.Padding(2);
            this.beginID2Tbx.MaxLength = 7;
            this.beginID2Tbx.Name = "beginID2Tbx";
            this.beginID2Tbx.Size = new System.Drawing.Size(70, 20);
            this.beginID2Tbx.TabIndex = 19;
            this.beginID2Tbx.Text = "0";
            this.beginID2Tbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateStartID);
            this.beginID2Tbx.Validated += new System.EventHandler(this.StartIDValidated);
            // 
            // infoLbl
            // 
            this.infoLbl.AutoSize = true;
            this.infoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.infoLbl.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.infoLbl.Location = new System.Drawing.Point(16, 9);
            this.infoLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.infoLbl.Name = "infoLbl";
            this.infoLbl.Size = new System.Drawing.Size(291, 13);
            this.infoLbl.TabIndex = 30;
            this.infoLbl.Text = "Changes the Start and/or End ID of the IDFX-File.";
            this.infoLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Keeps the initial start time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Resets the start time to zero";
            // 
            // fixedLabel1
            // 
            this.fixedLabel1.AutoSize = true;
            this.fixedLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedLabel1.Location = new System.Drawing.Point(41, 8);
            this.fixedLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fixedLabel1.Name = "fixedLabel1";
            this.fixedLabel1.Size = new System.Drawing.Size(53, 13);
            this.fixedLabel1.TabIndex = 0;
            this.fixedLabel1.Text = "Start at:";
            // 
            // fixedLabel2
            // 
            this.fixedLabel2.AutoSize = true;
            this.fixedLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedLabel2.Location = new System.Drawing.Point(5, 13);
            this.fixedLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fixedLabel2.Name = "fixedLabel2";
            this.fixedLabel2.Size = new System.Drawing.Size(65, 13);
            this.fixedLabel2.TabIndex = 16;
            this.fixedLabel2.Text = "Start Time";
            // 
            // beginID2Lbl
            // 
            this.beginID2Lbl.AutoSize = true;
            this.beginID2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginID2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.beginID2Lbl.Location = new System.Drawing.Point(3, 36);
            this.beginID2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.beginID2Lbl.Name = "beginID2Lbl";
            this.beginID2Lbl.Size = new System.Drawing.Size(46, 13);
            this.beginID2Lbl.TabIndex = 17;
            this.beginID2Lbl.Text = "Start ID:";
            // 
            // endID3Lbl
            // 
            this.endID3Lbl.AutoSize = true;
            this.endID3Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endID3Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endID3Lbl.Location = new System.Drawing.Point(3, 59);
            this.endID3Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endID3Lbl.Name = "endID3Lbl";
            this.endID3Lbl.Size = new System.Drawing.Size(46, 13);
            this.endID3Lbl.TabIndex = 18;
            this.endID3Lbl.Text = "End ID: ";
            // 
            // endID2Lbl
            // 
            this.endID2Lbl.AutoSize = true;
            this.endID2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endID2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endID2Lbl.Location = new System.Drawing.Point(224, 59);
            this.endID2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endID2Lbl.Name = "endID2Lbl";
            this.endID2Lbl.Size = new System.Drawing.Size(0, 13);
            this.endID2Lbl.TabIndex = 18;
            // 
            // endID1Lbl
            // 
            this.endID1Lbl.AutoSize = true;
            this.endID1Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endID1Lbl.Location = new System.Drawing.Point(3, 59);
            this.endID1Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endID1Lbl.Name = "endID1Lbl";
            this.endID1Lbl.Size = new System.Drawing.Size(46, 13);
            this.endID1Lbl.TabIndex = 18;
            this.endID1Lbl.Text = "End ID: ";
            // 
            // beginIDTbx
            // 
            this.beginIDTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginIDTbx.Location = new System.Drawing.Point(135, 36);
            this.beginIDTbx.Margin = new System.Windows.Forms.Padding(2);
            this.beginIDTbx.MaxLength = 7;
            this.beginIDTbx.Name = "beginIDTbx";
            this.beginIDTbx.Size = new System.Drawing.Size(70, 20);
            this.beginIDTbx.TabIndex = 4;
            this.beginIDTbx.Text = "0";
            this.beginIDTbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateStartID);
            this.beginIDTbx.Validated += new System.EventHandler(this.StartIDValidated);
            // 
            // endID2Tbx
            // 
            this.endID2Tbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endID2Tbx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endID2Tbx.Location = new System.Drawing.Point(135, 57);
            this.endID2Tbx.Margin = new System.Windows.Forms.Padding(2);
            this.endID2Tbx.MaxLength = 7;
            this.endID2Tbx.Name = "endID2Tbx";
            this.endID2Tbx.Size = new System.Drawing.Size(70, 20);
            this.endID2Tbx.TabIndex = 20;
            this.endID2Tbx.Text = "0";
            this.endID2Tbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateEndID);
            this.endID2Tbx.Validated += new System.EventHandler(this.EndIDValidated);
            // 
            // startTime2Lbl
            // 
            this.startTime2Lbl.AutoSize = true;
            this.startTime2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startTime2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.startTime2Lbl.Location = new System.Drawing.Point(215, 34);
            this.startTime2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startTime2Lbl.Name = "startTime2Lbl";
            this.startTime2Lbl.Size = new System.Drawing.Size(0, 13);
            this.startTime2Lbl.TabIndex = 21;
            // 
            // endIDLbl
            // 
            this.endIDLbl.AutoSize = true;
            this.endIDLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endIDLbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endIDLbl.Location = new System.Drawing.Point(224, 59);
            this.endIDLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endIDLbl.Name = "endIDLbl";
            this.endIDLbl.Size = new System.Drawing.Size(0, 13);
            this.endIDLbl.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(16, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Select";
            // 
            // fixedPanel
            // 
            this.fixedPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fixedPanel.Controls.Add(this.valueLbl);
            this.fixedPanel.Controls.Add(this.eventValueCBx);
            this.fixedPanel.Controls.Add(this.pictureBox1);
            this.fixedPanel.Controls.Add(this.panel2);
            this.fixedPanel.Controls.Add(this.panel1);
            this.fixedPanel.Controls.Add(this.fixedLabel3);
            this.fixedPanel.Controls.Add(this.fixedLabel1);
            this.fixedPanel.Controls.Add(this.fixedIdGbx2);
            this.fixedPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.fixedPanel.Location = new System.Drawing.Point(15, 83);
            this.fixedPanel.Margin = new System.Windows.Forms.Padding(0);
            this.fixedPanel.Name = "fixedPanel";
            this.fixedPanel.Size = new System.Drawing.Size(384, 232);
            this.fixedPanel.TabIndex = 29;
            // 
            // valueLbl
            // 
            this.valueLbl.AutoSize = true;
            this.valueLbl.Location = new System.Drawing.Point(295, 137);
            this.valueLbl.Name = "valueLbl";
            this.valueLbl.Size = new System.Drawing.Size(78, 13);
            this.valueLbl.TabIndex = 18;
            this.valueLbl.Text = "Last Key Value";
            // 
            // eventValueCBx
            // 
            this.eventValueCBx.FormattingEnabled = true;
            this.eventValueCBx.Location = new System.Drawing.Point(214, 134);
            this.eventValueCBx.Name = "eventValueCBx";
            this.eventValueCBx.Size = new System.Drawing.Size(75, 21);
            this.eventValueCBx.TabIndex = 19;
            this.eventValueCBx.SelectedValueChanged += new System.EventHandler(this.SelectValueChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GUI.Properties.Resources.keyLines;
            this.pictureBox1.Location = new System.Drawing.Point(6, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(363, 44);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.fixedSkipToFinalKeyRb);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.fixedFinalIDRb);
            this.panel2.Location = new System.Drawing.Point(215, 75);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(167, 52);
            this.panel2.TabIndex = 26;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(105, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Original ID";
            // 
            // fixedSkipToFinalKeyRb
            // 
            this.fixedSkipToFinalKeyRb.AutoSize = true;
            this.fixedSkipToFinalKeyRb.Checked = true;
            this.fixedSkipToFinalKeyRb.Location = new System.Drawing.Point(29, 2);
            this.fixedSkipToFinalKeyRb.Name = "fixedSkipToFinalKeyRb";
            this.fixedSkipToFinalKeyRb.Size = new System.Drawing.Size(14, 13);
            this.fixedSkipToFinalKeyRb.TabIndex = 0;
            this.fixedSkipToFinalKeyRb.TabStop = true;
            this.fixedSkipToFinalKeyRb.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.fixedSkipToFinalKeyRb.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Last Key";
            // 
            // fixedFinalIDRb
            // 
            this.fixedFinalIDRb.AutoSize = true;
            this.fixedFinalIDRb.Location = new System.Drawing.Point(144, 4);
            this.fixedFinalIDRb.Name = "fixedFinalIDRb";
            this.fixedFinalIDRb.Size = new System.Drawing.Size(14, 13);
            this.fixedFinalIDRb.TabIndex = 1;
            this.fixedFinalIDRb.TabStop = true;
            this.fixedFinalIDRb.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.fixedInitialIDRb);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.fixedSkipToFirstKeyRb);
            this.panel1.Location = new System.Drawing.Point(0, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(168, 49);
            this.panel1.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "First Key";
            // 
            // fixedInitialIDRb
            // 
            this.fixedInitialIDRb.AutoSize = true;
            this.fixedInitialIDRb.Location = new System.Drawing.Point(4, 3);
            this.fixedInitialIDRb.Name = "fixedInitialIDRb";
            this.fixedInitialIDRb.Size = new System.Drawing.Size(14, 13);
            this.fixedInitialIDRb.TabIndex = 3;
            this.fixedInitialIDRb.TabStop = true;
            this.fixedInitialIDRb.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.fixedInitialIDRb.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Original ID";
            // 
            // fixedSkipToFirstKeyRb
            // 
            this.fixedSkipToFirstKeyRb.AutoSize = true;
            this.fixedSkipToFirstKeyRb.Checked = true;
            this.fixedSkipToFirstKeyRb.Location = new System.Drawing.Point(114, 3);
            this.fixedSkipToFirstKeyRb.Name = "fixedSkipToFirstKeyRb";
            this.fixedSkipToFirstKeyRb.Size = new System.Drawing.Size(14, 13);
            this.fixedSkipToFirstKeyRb.TabIndex = 1;
            this.fixedSkipToFirstKeyRb.TabStop = true;
            this.fixedSkipToFirstKeyRb.UseVisualStyleBackColor = true;
            // 
            // fixedLabel3
            // 
            this.fixedLabel3.AutoSize = true;
            this.fixedLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.fixedLabel3.Location = new System.Drawing.Point(289, 8);
            this.fixedLabel3.Name = "fixedLabel3";
            this.fixedLabel3.Size = new System.Drawing.Size(48, 13);
            this.fixedLabel3.TabIndex = 2;
            this.fixedLabel3.Text = "End at:";
            // 
            // fixedIdGbx2
            // 
            this.fixedIdGbx2.Controls.Add(this.fixedKeepTimeRb);
            this.fixedIdGbx2.Controls.Add(this.fixedZeroTimeRb);
            this.fixedIdGbx2.Controls.Add(this.fixedLabel2);
            this.fixedIdGbx2.Location = new System.Drawing.Point(4, 158);
            this.fixedIdGbx2.Margin = new System.Windows.Forms.Padding(0);
            this.fixedIdGbx2.Name = "fixedIdGbx2";
            this.fixedIdGbx2.Padding = new System.Windows.Forms.Padding(0);
            this.fixedIdGbx2.Size = new System.Drawing.Size(376, 64);
            this.fixedIdGbx2.TabIndex = 24;
            this.fixedIdGbx2.TabStop = false;
            // 
            // fixedKeepTimeRb
            // 
            this.fixedKeepTimeRb.AutoSize = true;
            this.fixedKeepTimeRb.Checked = true;
            this.fixedKeepTimeRb.Location = new System.Drawing.Point(7, 30);
            this.fixedKeepTimeRb.Name = "fixedKeepTimeRb";
            this.fixedKeepTimeRb.Size = new System.Drawing.Size(126, 17);
            this.fixedKeepTimeRb.TabIndex = 17;
            this.fixedKeepTimeRb.TabStop = true;
            this.fixedKeepTimeRb.Text = "Keep time of first key ";
            this.fixedKeepTimeRb.UseVisualStyleBackColor = true;
            this.fixedKeepTimeRb.CheckedChanged += new System.EventHandler(this.FixedKeepTimeRbCheckedChanged);
            // 
            // fixedZeroTimeRb
            // 
            this.fixedZeroTimeRb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fixedZeroTimeRb.AutoSize = true;
            this.fixedZeroTimeRb.Location = new System.Drawing.Point(211, 30);
            this.fixedZeroTimeRb.Name = "fixedZeroTimeRb";
            this.fixedZeroTimeRb.Size = new System.Drawing.Size(139, 17);
            this.fixedZeroTimeRb.TabIndex = 2;
            this.fixedZeroTimeRb.Text = "Set the start time to zero";
            this.fixedZeroTimeRb.UseVisualStyleBackColor = true;
            this.fixedZeroTimeRb.CheckedChanged += new System.EventHandler(this.FixedZeroTimeRbCheckedChanged);
            // 
            // idPanel
            // 
            this.idPanel.Controls.Add(this.idGbx2);
            this.idPanel.Controls.Add(this.id1Gbx);
            this.idPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.idPanel.Location = new System.Drawing.Point(15, 96);
            this.idPanel.Margin = new System.Windows.Forms.Padding(0);
            this.idPanel.Name = "idPanel";
            this.idPanel.Size = new System.Drawing.Size(384, 219);
            this.idPanel.TabIndex = 29;
            // 
            // idGbx2
            // 
            this.idGbx2.Controls.Add(this.label2);
            this.idGbx2.Controls.Add(this.beginID2Tbx);
            this.idGbx2.Controls.Add(this.beginID2Lbl);
            this.idGbx2.Controls.Add(this.endID3Lbl);
            this.idGbx2.Controls.Add(this.endID2Tbx);
            this.idGbx2.Controls.Add(this.startTime2Lbl);
            this.idGbx2.Controls.Add(this.endTime2Lbl);
            this.idGbx2.Location = new System.Drawing.Point(11, 115);
            this.idGbx2.Margin = new System.Windows.Forms.Padding(2);
            this.idGbx2.Name = "idGbx2";
            this.idGbx2.Padding = new System.Windows.Forms.Padding(2);
            this.idGbx2.Size = new System.Drawing.Size(371, 94);
            this.idGbx2.TabIndex = 24;
            this.idGbx2.TabStop = false;
            this.idGbx2.Enter += new System.EventHandler(this.IdGbx2Enter);
            // 
            // endTime2Lbl
            // 
            this.endTime2Lbl.AutoSize = true;
            this.endTime2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endTime2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endTime2Lbl.Location = new System.Drawing.Point(215, 59);
            this.endTime2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endTime2Lbl.Name = "endTime2Lbl";
            this.endTime2Lbl.Size = new System.Drawing.Size(0, 13);
            this.endTime2Lbl.TabIndex = 22;
            // 
            // id1Gbx
            // 
            this.id1Gbx.Controls.Add(this.label1);
            this.id1Gbx.Controls.Add(this.beginIDTbx);
            this.id1Gbx.Controls.Add(this.beginID1Lbl);
            this.id1Gbx.Controls.Add(this.endID1Lbl);
            this.id1Gbx.Controls.Add(this.endIDTbx);
            this.id1Gbx.Controls.Add(this.startTimeLbl);
            this.id1Gbx.Controls.Add(this.endTimeLbl);
            this.id1Gbx.Location = new System.Drawing.Point(11, 16);
            this.id1Gbx.Margin = new System.Windows.Forms.Padding(2);
            this.id1Gbx.Name = "id1Gbx";
            this.id1Gbx.Padding = new System.Windows.Forms.Padding(2);
            this.id1Gbx.Size = new System.Drawing.Size(371, 86);
            this.id1Gbx.TabIndex = 23;
            this.id1Gbx.TabStop = false;
            this.id1Gbx.Enter += new System.EventHandler(this.Id1GbxEnter);
            // 
            // beginID1Lbl
            // 
            this.beginID1Lbl.AutoSize = true;
            this.beginID1Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginID1Lbl.Location = new System.Drawing.Point(3, 36);
            this.beginID1Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.beginID1Lbl.Name = "beginID1Lbl";
            this.beginID1Lbl.Size = new System.Drawing.Size(46, 13);
            this.beginID1Lbl.TabIndex = 2;
            this.beginID1Lbl.Text = "Start ID:";
            // 
            // endIDTbx
            // 
            this.endIDTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endIDTbx.Location = new System.Drawing.Point(135, 59);
            this.endIDTbx.Margin = new System.Windows.Forms.Padding(2);
            this.endIDTbx.MaxLength = 7;
            this.endIDTbx.Name = "endIDTbx";
            this.endIDTbx.Size = new System.Drawing.Size(70, 20);
            this.endIDTbx.TabIndex = 5;
            this.endIDTbx.Text = "0";
            this.endIDTbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateEndID);
            this.endIDTbx.Validated += new System.EventHandler(this.EndIDValidated);
            // 
            // startTimeLbl
            // 
            this.startTimeLbl.AutoSize = true;
            this.startTimeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startTimeLbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.startTimeLbl.Location = new System.Drawing.Point(215, 38);
            this.startTimeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startTimeLbl.Name = "startTimeLbl";
            this.startTimeLbl.Size = new System.Drawing.Size(0, 13);
            this.startTimeLbl.TabIndex = 8;
            // 
            // endTimeLbl
            // 
            this.endTimeLbl.AutoSize = true;
            this.endTimeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endTimeLbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endTimeLbl.Location = new System.Drawing.Point(215, 61);
            this.endTimeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endTimeLbl.Name = "endTimeLbl";
            this.endTimeLbl.Size = new System.Drawing.Size(0, 13);
            this.endTimeLbl.TabIndex = 19;
            // 
            // selectTimeRb
            // 
            this.selectTimeRb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.selectTimeRb.Location = new System.Drawing.Point(87, 55);
            this.selectTimeRb.Margin = new System.Windows.Forms.Padding(2);
            this.selectTimeRb.Name = "selectTimeRb";
            this.selectTimeRb.Size = new System.Drawing.Size(67, 26);
            this.selectTimeRb.TabIndex = 28;
            this.selectTimeRb.Text = "Time";
            this.selectTimeRb.UseVisualStyleBackColor = true;
            this.selectTimeRb.CheckedChanged += new System.EventHandler(this.SelectTimeRbCheckedChanged);
            // 
            // selectIDRb
            // 
            this.selectIDRb.Checked = true;
            this.selectIDRb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.selectIDRb.Location = new System.Drawing.Point(20, 55);
            this.selectIDRb.Margin = new System.Windows.Forms.Padding(2);
            this.selectIDRb.Name = "selectIDRb";
            this.selectIDRb.Size = new System.Drawing.Size(47, 26);
            this.selectIDRb.TabIndex = 27;
            this.selectIDRb.TabStop = true;
            this.selectIDRb.Text = "ID";
            this.selectIDRb.UseVisualStyleBackColor = true;
            this.selectIDRb.CheckedChanged += new System.EventHandler(this.SelectIDRbCheckedChanged);
            // 
            // EventErrorProvider
            // 
            this.EventErrorProvider.ContainerControl = this;
            // 
            // AcceptBtn
            // 
            this.AcceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AcceptBtn.Location = new System.Drawing.Point(229, 325);
            this.AcceptBtn.Name = "AcceptBtn";
            this.AcceptBtn.Size = new System.Drawing.Size(75, 32);
            this.AcceptBtn.TabIndex = 32;
            this.AcceptBtn.Text = "Accept";
            this.AcceptBtn.UseVisualStyleBackColor = true;
            this.AcceptBtn.Click += new System.EventHandler(this.AcceptBtnClick);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(322, 325);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 32);
            this.CancelBtn.TabIndex = 33;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
            // 
            // timePanel
            // 
            this.timePanel.Controls.Add(this.timeGbx2);
            this.timePanel.Controls.Add(this.timeGbx1);
            this.timePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.timePanel.Location = new System.Drawing.Point(15, 96);
            this.timePanel.Margin = new System.Windows.Forms.Padding(0);
            this.timePanel.Name = "timePanel";
            this.timePanel.Size = new System.Drawing.Size(384, 219);
            this.timePanel.TabIndex = 34;
            // 
            // timeGbx2
            // 
            this.timeGbx2.Controls.Add(this.label8);
            this.timeGbx2.Controls.Add(this.beginTime2Tbx);
            this.timeGbx2.Controls.Add(this.event2Lbl);
            this.timeGbx2.Controls.Add(this.event3Lbl);
            this.timeGbx2.Controls.Add(this.endTime2Tbx);
            this.timeGbx2.Controls.Add(this.startID2Lbl);
            this.timeGbx2.Controls.Add(this.endID2Lbl);
            this.timeGbx2.Location = new System.Drawing.Point(12, 115);
            this.timeGbx2.Margin = new System.Windows.Forms.Padding(2);
            this.timeGbx2.Name = "timeGbx2";
            this.timeGbx2.Padding = new System.Windows.Forms.Padding(2);
            this.timeGbx2.Size = new System.Drawing.Size(370, 93);
            this.timeGbx2.TabIndex = 27;
            this.timeGbx2.TabStop = false;
            this.timeGbx2.Enter += new System.EventHandler(this.TimeGbx2Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(3, 14);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(167, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Resets the start time to zero";
            // 
            // beginTime2Tbx
            // 
            this.beginTime2Tbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginTime2Tbx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.beginTime2Tbx.Location = new System.Drawing.Point(135, 32);
            this.beginTime2Tbx.Margin = new System.Windows.Forms.Padding(2);
            this.beginTime2Tbx.Name = "beginTime2Tbx";
            this.beginTime2Tbx.Size = new System.Drawing.Size(74, 20);
            this.beginTime2Tbx.TabIndex = 21;
            this.beginTime2Tbx.Text = "0";
            this.beginTime2Tbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateStartTime);
            this.beginTime2Tbx.Validated += new System.EventHandler(this.StartTimeValidated);
            // 
            // event2Lbl
            // 
            this.event2Lbl.AutoSize = true;
            this.event2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.event2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.event2Lbl.Location = new System.Drawing.Point(3, 36);
            this.event2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.event2Lbl.Name = "event2Lbl";
            this.event2Lbl.Size = new System.Drawing.Size(58, 13);
            this.event2Lbl.TabIndex = 20;
            this.event2Lbl.Text = "Start Time:";
            // 
            // event3Lbl
            // 
            this.event3Lbl.AutoSize = true;
            this.event3Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.event3Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.event3Lbl.Location = new System.Drawing.Point(3, 59);
            this.event3Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.event3Lbl.Name = "event3Lbl";
            this.event3Lbl.Size = new System.Drawing.Size(55, 13);
            this.event3Lbl.TabIndex = 19;
            this.event3Lbl.Text = "End Time:";
            // 
            // endTime2Tbx
            // 
            this.endTime2Tbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endTime2Tbx.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.endTime2Tbx.Location = new System.Drawing.Point(135, 57);
            this.endTime2Tbx.Margin = new System.Windows.Forms.Padding(2);
            this.endTime2Tbx.Name = "endTime2Tbx";
            this.endTime2Tbx.Size = new System.Drawing.Size(74, 20);
            this.endTime2Tbx.TabIndex = 22;
            this.endTime2Tbx.Text = "0";
            this.endTime2Tbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateEndTime);
            this.endTime2Tbx.Validated += new System.EventHandler(this.EndTimeValidated);
            // 
            // startID2Lbl
            // 
            this.startID2Lbl.AutoSize = true;
            this.startID2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startID2Lbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.startID2Lbl.Location = new System.Drawing.Point(224, 34);
            this.startID2Lbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startID2Lbl.Name = "startID2Lbl";
            this.startID2Lbl.Size = new System.Drawing.Size(0, 13);
            this.startID2Lbl.TabIndex = 23;
            // 
            // timeGbx1
            // 
            this.timeGbx1.Controls.Add(this.label7);
            this.timeGbx1.Controls.Add(this.beginTimeTbx);
            this.timeGbx1.Controls.Add(this.eventLbl2);
            this.timeGbx1.Controls.Add(this.eventLbl);
            this.timeGbx1.Controls.Add(this.endTimeTbx);
            this.timeGbx1.Controls.Add(this.startIDLbl);
            this.timeGbx1.Controls.Add(this.endIDLbl);
            this.timeGbx1.Location = new System.Drawing.Point(12, 16);
            this.timeGbx1.Margin = new System.Windows.Forms.Padding(2);
            this.timeGbx1.Name = "timeGbx1";
            this.timeGbx1.Padding = new System.Windows.Forms.Padding(2);
            this.timeGbx1.Size = new System.Drawing.Size(370, 86);
            this.timeGbx1.TabIndex = 26;
            this.timeGbx1.TabStop = false;
            this.timeGbx1.Enter += new System.EventHandler(this.TimeGbx1Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(3, 14);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(154, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Keeps the initial start time";
            // 
            // beginTimeTbx
            // 
            this.beginTimeTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.beginTimeTbx.Location = new System.Drawing.Point(135, 35);
            this.beginTimeTbx.Margin = new System.Windows.Forms.Padding(2);
            this.beginTimeTbx.Name = "beginTimeTbx";
            this.beginTimeTbx.Size = new System.Drawing.Size(74, 20);
            this.beginTimeTbx.TabIndex = 11;
            this.beginTimeTbx.Text = "0";
            this.beginTimeTbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateStartTime);
            this.beginTimeTbx.Validated += new System.EventHandler(this.StartTimeValidated);
            // 
            // eventLbl2
            // 
            this.eventLbl2.AutoSize = true;
            this.eventLbl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.eventLbl2.Location = new System.Drawing.Point(3, 36);
            this.eventLbl2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.eventLbl2.Name = "eventLbl2";
            this.eventLbl2.Size = new System.Drawing.Size(58, 13);
            this.eventLbl2.TabIndex = 7;
            this.eventLbl2.Text = "Start Time:";
            // 
            // eventLbl
            // 
            this.eventLbl.AutoSize = true;
            this.eventLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.eventLbl.Location = new System.Drawing.Point(3, 59);
            this.eventLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.eventLbl.Name = "eventLbl";
            this.eventLbl.Size = new System.Drawing.Size(55, 13);
            this.eventLbl.TabIndex = 6;
            this.eventLbl.Text = "End Time:";
            // 
            // endTimeTbx
            // 
            this.endTimeTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.endTimeTbx.Location = new System.Drawing.Point(135, 59);
            this.endTimeTbx.Margin = new System.Windows.Forms.Padding(2);
            this.endTimeTbx.Name = "endTimeTbx";
            this.endTimeTbx.Size = new System.Drawing.Size(74, 20);
            this.endTimeTbx.TabIndex = 12;
            this.endTimeTbx.Text = "0";
            this.endTimeTbx.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateEndTime);
            this.endTimeTbx.Validated += new System.EventHandler(this.EndTimeValidated);
            // 
            // startIDLbl
            // 
            this.startIDLbl.AutoSize = true;
            this.startIDLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startIDLbl.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.startIDLbl.Location = new System.Drawing.Point(224, 35);
            this.startIDLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startIDLbl.Name = "startIDLbl";
            this.startIDLbl.Size = new System.Drawing.Size(0, 13);
            this.startIDLbl.TabIndex = 16;
            // 
            // TimeConfiguration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(427, 370);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.AcceptBtn);
            this.Controls.Add(this.infoLbl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.selectTimeRb);
            this.Controls.Add(this.selectIDRb);
            this.Controls.Add(this.fixedPanel);
            this.Controls.Add(this.idPanel);
            this.Controls.Add(this.timePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimeConfiguration";
            this.ShowInTaskbar = false;
            this.Text = "ID & Time Configuration";
            this.fixedPanel.ResumeLayout(false);
            this.fixedPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.fixedIdGbx2.ResumeLayout(false);
            this.fixedIdGbx2.PerformLayout();
            this.idPanel.ResumeLayout(false);
            this.idGbx2.ResumeLayout(false);
            this.idGbx2.PerformLayout();
            this.id1Gbx.ResumeLayout(false);
            this.id1Gbx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EventErrorProvider)).EndInit();
            this.timePanel.ResumeLayout(false);
            this.timeGbx2.ResumeLayout(false);
            this.timeGbx2.PerformLayout();
            this.timeGbx1.ResumeLayout(false);
            this.timeGbx1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox beginID2Tbx;
		private System.Windows.Forms.Label infoLbl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	    private System.Windows.Forms.Label fixedLabel1;
        private System.Windows.Forms.Label fixedLabel2;
		private System.Windows.Forms.Label beginID2Lbl;
        private System.Windows.Forms.Label endID1Lbl;
		private System.Windows.Forms.Label endID2Lbl;
        private System.Windows.Forms.Label endID3Lbl;
		private System.Windows.Forms.TextBox beginIDTbx;
		private System.Windows.Forms.TextBox endID2Tbx;
		private System.Windows.Forms.Label startTime2Lbl;
		private System.Windows.Forms.Label endIDLbl;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel idPanel;
        private System.Windows.Forms.Panel fixedPanel;
		private System.Windows.Forms.GroupBox idGbx2;
        private System.Windows.Forms.GroupBox fixedIdGbx2;
		private System.Windows.Forms.Label endTime2Lbl;
        private System.Windows.Forms.GroupBox id1Gbx;
		private System.Windows.Forms.Label beginID1Lbl;
		private System.Windows.Forms.TextBox endIDTbx;
		private System.Windows.Forms.Label startTimeLbl;
		private System.Windows.Forms.Label endTimeLbl;
		private System.Windows.Forms.RadioButton selectTimeRb;
		private System.Windows.Forms.RadioButton selectIDRb;
		private System.Windows.Forms.ErrorProvider EventErrorProvider;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button AcceptBtn;
		private System.Windows.Forms.Panel timePanel;
		private System.Windows.Forms.GroupBox timeGbx2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox beginTime2Tbx;
		private System.Windows.Forms.Label event3Lbl;
		private System.Windows.Forms.Label event2Lbl;
		private System.Windows.Forms.TextBox endTime2Tbx;
		private System.Windows.Forms.Label startID2Lbl;
		private System.Windows.Forms.GroupBox timeGbx1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox beginTimeTbx;
        private System.Windows.Forms.Label eventLbl;
        private System.Windows.Forms.Label eventLbl2;
		private System.Windows.Forms.TextBox endTimeTbx;
        private System.Windows.Forms.Label startIDLbl;
        private System.Windows.Forms.RadioButton fixedSkipToFirstKeyRb;
        private System.Windows.Forms.RadioButton fixedZeroTimeRb;
        private System.Windows.Forms.RadioButton fixedInitialIDRb;
        private System.Windows.Forms.RadioButton fixedKeepTimeRb;
        private System.Windows.Forms.RadioButton fixedFinalIDRb;
        private System.Windows.Forms.RadioButton fixedSkipToFinalKeyRb;
        private System.Windows.Forms.Label fixedLabel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox eventValueCBx;
        private System.Windows.Forms.Label valueLbl;
    }
}