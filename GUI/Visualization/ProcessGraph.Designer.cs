namespace GUI.Visualization
{
    partial class ProcessGraph
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
            FormClosing += VisualisationClosing;
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessGraph));
            this.VisualSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SaveToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.VisualSaveButton = new System.Windows.Forms.Button();
            this.LegendPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ProcessToggle = new System.Windows.Forms.LinkLabel();
            this.ProductToggle = new System.Windows.Forms.LinkLabel();
            this.PositionToggle = new System.Windows.Forms.LinkLabel();
            this.PausesToggle = new System.Windows.Forms.LinkLabel();
            this.FocusToggle = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.OutlierToggle = new System.Windows.Forms.LinkLabel();
            this.OutlierBx = new System.Windows.Forms.GroupBox();
            this.OutlierLimitTxtBx = new System.Windows.Forms.MaskedTextBox();
            this.OutlierMaxLbl2 = new System.Windows.Forms.Label();
            this.OutlierMaxLbl = new System.Windows.Forms.Label();
            this.OutlierAvrgLbl2 = new System.Windows.Forms.Label();
            this.OutlierAvrgLbl = new System.Windows.Forms.Label();
            this.OutlierCountLbl2 = new System.Windows.Forms.Label();
            this.OutlierCountLbl = new System.Windows.Forms.Label();
            this.OutlierLimitLbl = new System.Windows.Forms.Label();
            this.RedrawBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.VisPauseThreshold = new System.Windows.Forms.NumericUpDown();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.LegendPanel.SuspendLayout();
            this.OutlierBx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VisPauseThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // VisualSaveFileDialog
            // 
            this.VisualSaveFileDialog.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg|TIFF|*.tiff|EMF|*.emf";
            // 
            // SaveToolTip
            // 
            this.SaveToolTip.IsBalloon = true;
            this.SaveToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.SaveToolTip.ToolTipTitle = "Image File";
            // 
            // VisualSaveButton
            // 
            this.VisualSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.VisualSaveButton.Location = new System.Drawing.Point(1167, 627);
            this.VisualSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.VisualSaveButton.Name = "VisualSaveButton";
            this.VisualSaveButton.Size = new System.Drawing.Size(82, 28);
            this.VisualSaveButton.TabIndex = 1;
            this.VisualSaveButton.Text = "Save";
            this.SaveToolTip.SetToolTip(this.VisualSaveButton, "The original chart has been saved automatically.\nUse the \'Save\' button when the c" +
        "hart has been changed,\nor when you want a different format.");
            this.VisualSaveButton.UseVisualStyleBackColor = true;
            this.VisualSaveButton.Click += new System.EventHandler(this.VisualSaveButtonClick);
            // 
            // LegendPanel
            // 
            this.LegendPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LegendPanel.BackColor = System.Drawing.SystemColors.Window;
            this.LegendPanel.ColumnCount = 2;
            this.LegendPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.10257F));
            this.LegendPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.89743F));
            this.LegendPanel.Controls.Add(this.ProcessToggle, 1, 0);
            this.LegendPanel.Controls.Add(this.ProductToggle, 1, 1);
            this.LegendPanel.Controls.Add(this.PositionToggle, 1, 2);
            this.LegendPanel.Controls.Add(this.PausesToggle, 1, 3);
            this.LegendPanel.Controls.Add(this.FocusToggle, 1, 4);
            this.LegendPanel.Controls.Add(this.label2, 0, 0);
            this.LegendPanel.Controls.Add(this.label3, 0, 1);
            this.LegendPanel.Controls.Add(this.label4, 0, 2);
            this.LegendPanel.Controls.Add(this.label5, 0, 3);
            this.LegendPanel.Controls.Add(this.label6, 0, 4);
            this.LegendPanel.Location = new System.Drawing.Point(1043, 25);
            this.LegendPanel.Name = "LegendPanel";
            this.LegendPanel.RowCount = 5;
            this.LegendPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.LegendPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.LegendPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.LegendPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.LegendPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.LegendPanel.Size = new System.Drawing.Size(206, 99);
            this.LegendPanel.TabIndex = 15;
            // 
            // ProcessToggle
            // 
            this.ProcessToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessToggle.AutoSize = true;
            this.ProcessToggle.BackColor = System.Drawing.Color.White;
            this.ProcessToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessToggle.Location = new System.Drawing.Point(161, 0);
            this.ProcessToggle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.ProcessToggle.Name = "ProcessToggle";
            this.ProcessToggle.Size = new System.Drawing.Size(41, 15);
            this.ProcessToggle.TabIndex = 2;
            this.ProcessToggle.TabStop = true;
            this.ProcessToggle.Text = "(Hide)";
            this.ProcessToggle.Click += new System.EventHandler(this.ProcessToggleClick);
            // 
            // ProductToggle
            // 
            this.ProductToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProductToggle.AutoSize = true;
            this.ProductToggle.BackColor = System.Drawing.Color.White;
            this.ProductToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProductToggle.Location = new System.Drawing.Point(161, 19);
            this.ProductToggle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.ProductToggle.Name = "ProductToggle";
            this.ProductToggle.Size = new System.Drawing.Size(41, 15);
            this.ProductToggle.TabIndex = 3;
            this.ProductToggle.TabStop = true;
            this.ProductToggle.Text = "(Hide)";
            this.ProductToggle.Click += new System.EventHandler(this.ProductToggleClick);
            // 
            // PositionToggle
            // 
            this.PositionToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PositionToggle.AutoSize = true;
            this.PositionToggle.BackColor = System.Drawing.Color.White;
            this.PositionToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PositionToggle.Location = new System.Drawing.Point(161, 38);
            this.PositionToggle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.PositionToggle.Name = "PositionToggle";
            this.PositionToggle.Size = new System.Drawing.Size(41, 15);
            this.PositionToggle.TabIndex = 4;
            this.PositionToggle.TabStop = true;
            this.PositionToggle.Text = "(Hide)";
            this.PositionToggle.Click += new System.EventHandler(this.PositionToggleClick);
            // 
            // PausesToggle
            // 
            this.PausesToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PausesToggle.AutoSize = true;
            this.PausesToggle.BackColor = System.Drawing.Color.White;
            this.PausesToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PausesToggle.Location = new System.Drawing.Point(161, 57);
            this.PausesToggle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.PausesToggle.Name = "PausesToggle";
            this.PausesToggle.Size = new System.Drawing.Size(41, 15);
            this.PausesToggle.TabIndex = 5;
            this.PausesToggle.TabStop = true;
            this.PausesToggle.Text = "(Hide)";
            this.PausesToggle.Click += new System.EventHandler(this.PausesToggleClick);
            // 
            // FocusToggle
            // 
            this.FocusToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FocusToggle.AutoSize = true;
            this.FocusToggle.BackColor = System.Drawing.Color.White;
            this.FocusToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FocusToggle.Location = new System.Drawing.Point(161, 76);
            this.FocusToggle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 5);
            this.FocusToggle.Name = "FocusToggle";
            this.FocusToggle.Size = new System.Drawing.Size(41, 15);
            this.FocusToggle.TabIndex = 6;
            this.FocusToggle.TabStop = true;
            this.FocusToggle.Text = "(Hide)";
            this.FocusToggle.Click += new System.EventHandler(this.FocusToggleClick);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(58, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "— Process";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(60, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "— Product";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(6, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "--- Cursor Position";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label5.Location = new System.Drawing.Point(61, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "•  Pauses";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(71, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "— Focus";
            // 
            // OutlierToggle
            // 
            this.OutlierToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutlierToggle.AutoSize = true;
            this.OutlierToggle.BackColor = System.Drawing.SystemColors.Window;
            this.OutlierToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.OutlierToggle.Location = new System.Drawing.Point(1182, 506);
            this.OutlierToggle.Name = "OutlierToggle";
            this.OutlierToggle.Size = new System.Drawing.Size(41, 15);
            this.OutlierToggle.TabIndex = 12;
            this.OutlierToggle.TabStop = true;
            this.OutlierToggle.Text = "(Hide)";
            this.OutlierToggle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OutlierToggleClick);
            // 
            // OutlierBx
            // 
            this.OutlierBx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OutlierBx.Controls.Add(this.OutlierLimitTxtBx);
            this.OutlierBx.Controls.Add(this.OutlierMaxLbl2);
            this.OutlierBx.Controls.Add(this.OutlierMaxLbl);
            this.OutlierBx.Controls.Add(this.OutlierAvrgLbl2);
            this.OutlierBx.Controls.Add(this.OutlierAvrgLbl);
            this.OutlierBx.Controls.Add(this.OutlierCountLbl2);
            this.OutlierBx.Controls.Add(this.OutlierCountLbl);
            this.OutlierBx.Controls.Add(this.OutlierLimitLbl);
            this.OutlierBx.Location = new System.Drawing.Point(1015, 349);
            this.OutlierBx.Name = "OutlierBx";
            this.OutlierBx.Size = new System.Drawing.Size(234, 154);
            this.OutlierBx.TabIndex = 11;
            this.OutlierBx.TabStop = false;
            this.OutlierBx.Text = "Pause Outliers";
            // 
            // OutlierLimitTxtBx
            // 
            this.OutlierLimitTxtBx.Location = new System.Drawing.Point(137, 31);
            this.OutlierLimitTxtBx.Mask = "0000000";
            this.OutlierLimitTxtBx.Name = "OutlierLimitTxtBx";
            this.OutlierLimitTxtBx.Size = new System.Drawing.Size(55, 23);
            this.OutlierLimitTxtBx.TabIndex = 8;
            this.OutlierLimitTxtBx.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.OutlierLimitTxtBx.ValidatingType = typeof(int);
            this.OutlierLimitTxtBx.Leave += new System.EventHandler(this.LimitChanged);
            // 
            // OutlierMaxLbl2
            // 
            this.OutlierMaxLbl2.AutoSize = true;
            this.OutlierMaxLbl2.Location = new System.Drawing.Point(167, 118);
            this.OutlierMaxLbl2.Name = "OutlierMaxLbl2";
            this.OutlierMaxLbl2.Size = new System.Drawing.Size(16, 17);
            this.OutlierMaxLbl2.TabIndex = 7;
            this.OutlierMaxLbl2.Text = "0";
            this.OutlierMaxLbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // OutlierMaxLbl
            // 
            this.OutlierMaxLbl.AutoSize = true;
            this.OutlierMaxLbl.Location = new System.Drawing.Point(16, 118);
            this.OutlierMaxLbl.Name = "OutlierMaxLbl";
            this.OutlierMaxLbl.Size = new System.Drawing.Size(148, 17);
            this.OutlierMaxLbl.TabIndex = 6;
            this.OutlierMaxLbl.Text = "Outlier Maximum (sec)";
            // 
            // OutlierAvrgLbl2
            // 
            this.OutlierAvrgLbl2.AutoSize = true;
            this.OutlierAvrgLbl2.Location = new System.Drawing.Point(167, 89);
            this.OutlierAvrgLbl2.Name = "OutlierAvrgLbl2";
            this.OutlierAvrgLbl2.Size = new System.Drawing.Size(16, 17);
            this.OutlierAvrgLbl2.TabIndex = 5;
            this.OutlierAvrgLbl2.Text = "0";
            this.OutlierAvrgLbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // OutlierAvrgLbl
            // 
            this.OutlierAvrgLbl.AutoSize = true;
            this.OutlierAvrgLbl.Location = new System.Drawing.Point(16, 89);
            this.OutlierAvrgLbl.Name = "OutlierAvrgLbl";
            this.OutlierAvrgLbl.Size = new System.Drawing.Size(143, 17);
            this.OutlierAvrgLbl.TabIndex = 4;
            this.OutlierAvrgLbl.Text = "Outlier Average (sec)";
            // 
            // OutlierCountLbl2
            // 
            this.OutlierCountLbl2.AutoSize = true;
            this.OutlierCountLbl2.Location = new System.Drawing.Point(134, 61);
            this.OutlierCountLbl2.Name = "OutlierCountLbl2";
            this.OutlierCountLbl2.Size = new System.Drawing.Size(16, 17);
            this.OutlierCountLbl2.TabIndex = 3;
            this.OutlierCountLbl2.Text = "0";
            this.OutlierCountLbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // OutlierCountLbl
            // 
            this.OutlierCountLbl.AutoSize = true;
            this.OutlierCountLbl.Location = new System.Drawing.Point(16, 61);
            this.OutlierCountLbl.Name = "OutlierCountLbl";
            this.OutlierCountLbl.Size = new System.Drawing.Size(102, 17);
            this.OutlierCountLbl.TabIndex = 2;
            this.OutlierCountLbl.Text = "Outliers > Limit";
            // 
            // OutlierLimitLbl
            // 
            this.OutlierLimitLbl.AutoSize = true;
            this.OutlierLimitLbl.Location = new System.Drawing.Point(16, 34);
            this.OutlierLimitLbl.Name = "OutlierLimitLbl";
            this.OutlierLimitLbl.Size = new System.Drawing.Size(115, 17);
            this.OutlierLimitLbl.TabIndex = 0;
            this.OutlierLimitLbl.Text = "Outlier Limit (ms)";
            // 
            // RedrawBtn
            // 
            this.RedrawBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RedrawBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.RedrawBtn.Location = new System.Drawing.Point(1167, 584);
            this.RedrawBtn.Name = "RedrawBtn";
            this.RedrawBtn.Size = new System.Drawing.Size(82, 28);
            this.RedrawBtn.TabIndex = 9;
            this.RedrawBtn.Text = "Redraw";
            this.RedrawBtn.UseVisualStyleBackColor = true;
            this.RedrawBtn.Click += new System.EventHandler(this.RedrawBtnClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1034, 528);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Pause threshold (ms)";
            // 
            // VisPauseThreshold
            // 
            this.VisPauseThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.VisPauseThreshold.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.VisPauseThreshold.Location = new System.Drawing.Point(1185, 526);
            this.VisPauseThreshold.Margin = new System.Windows.Forms.Padding(4);
            this.VisPauseThreshold.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.VisPauseThreshold.Name = "VisPauseThreshold";
            this.VisPauseThreshold.Size = new System.Drawing.Size(58, 23);
            this.VisPauseThreshold.TabIndex = 7;
            this.VisPauseThreshold.ValueChanged += new System.EventHandler(this.VisPauseThresholdValueChanged);
            // 
            // chart1
            // 
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.AcrossAxis;
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX2.MajorGrid.Enabled = false;
            chartArea1.AxisX2.MajorTickMark.Enabled = false;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MajorGrid.Interval = 0D;
            chartArea1.AxisY.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisY.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.ScaleBreakStyle.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.ScaleBreakStyle.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.Title = "Pauses (ms)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            chartArea1.AxisY2.IsLabelAutoFit = false;
            chartArea1.AxisY2.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisY2.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.Title = "Characters";
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "MainArea";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 86F;
            chartArea1.Position.Width = 80F;
            chartArea1.Position.X = 1F;
            chartArea1.Position.Y = 3F;
            chartArea2.AlignWithChartArea = "MainArea";
            chartArea2.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.AxisX.LineColor = System.Drawing.Color.Transparent;
            chartArea2.AxisX.LineWidth = 0;
            chartArea2.AxisX.MajorGrid.LineWidth = 0;
            chartArea2.AxisX.MajorTickMark.Enabled = false;
            chartArea2.AxisX2.LabelStyle.Enabled = false;
            chartArea2.AxisX2.LineColor = System.Drawing.Color.Transparent;
            chartArea2.AxisX2.LineWidth = 0;
            chartArea2.AxisX2.MajorGrid.Enabled = false;
            chartArea2.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LineColor = System.Drawing.Color.Transparent;
            chartArea2.AxisY.LineWidth = 0;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorTickMark.Enabled = false;
            chartArea2.AxisY2.Interval = 1D;
            chartArea2.AxisY2.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.AxisY2.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.None;
            chartArea2.AxisY2.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisY2.LineColor = System.Drawing.Color.Transparent;
            chartArea2.AxisY2.MajorGrid.Enabled = false;
            chartArea2.AxisY2.MajorTickMark.Enabled = false;
            chartArea2.AxisY2.MajorTickMark.Interval = 0D;
            chartArea2.AxisY2.MajorTickMark.IntervalOffset = 0D;
            chartArea2.AxisY2.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisY2.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            chartArea2.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            chartArea2.IsSameFontSizeForAllAxes = true;
            chartArea2.Name = "FocusArea";
            chartArea2.Position.Auto = false;
            chartArea2.Position.Height = 9F;
            chartArea2.Position.Width = 80F;
            chartArea2.Position.X = 3F;
            chartArea2.Position.Y = 90F;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ChartAreas.Add(chartArea2);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Margin = new System.Windows.Forms.Padding(4);
            this.chart1.Name = "chart1";
            series1.ChartArea = "MainArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Blue;
            series1.Legend = "Legend1";
            series1.LegendText = "Process";
            series1.Name = "Process";
            series1.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series2.ChartArea = "MainArea";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Green;
            series2.Legend = "Legend1";
            series2.LegendText = "Product          ";
            series2.Name = "Product";
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series3.BorderColor = System.Drawing.Color.Transparent;
            series3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series3.ChartArea = "MainArea";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Green;
            series3.Label = "     ";
            series3.Legend = "Legend1";
            series3.LegendText = "Cursor Position      ";
            series3.Name = "Cursor Position";
            series3.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series4.ChartArea = "MainArea";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bubble;
            series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series4.CustomProperties = "BubbleMinSize=2, BubbleMaxSize=2";
            series4.Legend = "Legend1";
            series4.LegendText = "Pauses";
            series4.MarkerSize = 8;
            series4.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series4.Name = "Pauses";
            series4.YValuesPerPoint = 2;
            series5.BorderColor = System.Drawing.Color.Transparent;
            series5.BorderWidth = 2;
            series5.ChartArea = "FocusArea";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series5.Color = System.Drawing.Color.Chocolate;
            series5.CustomProperties = "EmptyPointValue=Zero";
            series5.Legend = "Legend1";
            series5.LegendText = "Focus";
            series5.Name = "Focus";
            series5.ShadowColor = System.Drawing.Color.Transparent;
            series5.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series5.YValuesPerPoint = 2;
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Size = new System.Drawing.Size(1301, 668);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseMove);
            // 
            // ProcessGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1301, 668);
            this.Controls.Add(this.LegendPanel);
            this.Controls.Add(this.OutlierToggle);
            this.Controls.Add(this.OutlierBx);
            this.Controls.Add(this.RedrawBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VisPauseThreshold);
            this.Controls.Add(this.VisualSaveButton);
            this.Controls.Add(this.chart1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProcessGraph";
            this.Text = "Process Graph";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart1MouseMove);
            this.LegendPanel.ResumeLayout(false);
            this.LegendPanel.PerformLayout();
            this.OutlierBx.ResumeLayout(false);
            this.OutlierBx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VisPauseThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog VisualSaveFileDialog;
        private System.Windows.Forms.Button VisualSaveButton;
        private System.Windows.Forms.LinkLabel ProcessToggle;
        private System.Windows.Forms.LinkLabel ProductToggle;
        private System.Windows.Forms.LinkLabel PositionToggle;
        private System.Windows.Forms.LinkLabel PausesToggle;
        private System.Windows.Forms.LinkLabel FocusToggle;
        private System.Windows.Forms.NumericUpDown VisPauseThreshold;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ToolTip SaveToolTip;
        private System.Windows.Forms.Button RedrawBtn;
        private System.Windows.Forms.GroupBox OutlierBx;
        private System.Windows.Forms.Label OutlierMaxLbl2;
        private System.Windows.Forms.Label OutlierMaxLbl;
        private System.Windows.Forms.Label OutlierAvrgLbl2;
        private System.Windows.Forms.Label OutlierAvrgLbl;
        private System.Windows.Forms.Label OutlierCountLbl2;
        private System.Windows.Forms.Label OutlierCountLbl;
        private System.Windows.Forms.Label OutlierLimitLbl;
        private System.Windows.Forms.MaskedTextBox OutlierLimitTxtBx;
        private System.Windows.Forms.LinkLabel OutlierToggle;
        private System.Windows.Forms.TableLayoutPanel LegendPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

