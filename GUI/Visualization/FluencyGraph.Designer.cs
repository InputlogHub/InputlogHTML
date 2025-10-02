namespace GUI.Visualization
{
    partial class FluencyGraph
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FluencyGraph));
            this.VisualSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.VisualSaveButton = new System.Windows.Forms.Button();
            this.SaveToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.maxTypeSelect = new System.Windows.Forms.ComboBox();
            this.maxTypeLabel = new System.Windows.Forms.Label();
            this.dummyToggleLink = new System.Windows.Forms.LinkLabel();
            this.ParameterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ProcessTimeLbl = new System.Windows.Forms.Label();
            this.IntervalLbl = new System.Windows.Forms.Label();
            this.TaskLbl = new System.Windows.Forms.Label();
            this.PersonalLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.ParameterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // VisualSaveFileDialog
            // 
            this.VisualSaveFileDialog.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg|TIFF|*.tiff|EMF|*.emf";
            // 
            // VisualSaveButton
            // 
            this.VisualSaveButton.Location = new System.Drawing.Point(1123, 636);
            this.VisualSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.VisualSaveButton.Name = "VisualSaveButton";
            this.VisualSaveButton.Size = new System.Drawing.Size(82, 28);
            this.VisualSaveButton.TabIndex = 1;
            this.VisualSaveButton.Text = "Save";
            this.SaveToolTip.SetToolTip(this.VisualSaveButton, "The original chart has been saved automatically." +
                                                               " \r\nUse \'Save\' button when the chart has been changed," +
                                                               "\nor when you need a different format.");
            this.VisualSaveButton.UseVisualStyleBackColor = true;
            this.VisualSaveButton.Click += new System.EventHandler(this.VisualSaveButtonClick);
            // 
            // SaveToolTip
            // 
            this.SaveToolTip.IsBalloon = true;
            this.SaveToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.SaveToolTip.ToolTipTitle = "Image File";
            // 
            // chart1
            // 
            this.chart1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.AcrossAxis;
            chartArea1.AxisX.Maximum = 10D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX2.MajorGrid.Enabled = false;
            chartArea1.AxisX2.MajorTickMark.Enabled = false;
            chartArea1.AxisY.Crossing = -1.7976931348623157E+308D;
            chartArea1.AxisY.Interval = 10D;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.Title = "Percentage";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.IsLabelAutoFit = false;
            chartArea1.AxisY2.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY2.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.Title = "Characters";
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "MainArea";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 86F;
            chartArea1.Position.Width = 80F;
            chartArea1.Position.X = 1F;
            chartArea1.Position.Y = 3F;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Margin = new System.Windows.Forms.Padding(4);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1217, 696);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // maxTypeSelect
            // 
            this.maxTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maxTypeSelect.FormattingEnabled = true;
            this.maxTypeSelect.Location = new System.Drawing.Point(1084, 196);
            this.maxTypeSelect.Name = "maxTypeSelect";
            this.maxTypeSelect.Size = new System.Drawing.Size(121, 25);
            this.maxTypeSelect.TabIndex = 2;
            this.maxTypeSelect.SelectedIndexChanged += new System.EventHandler(this.OptTypeSelectSelectedIndexChanged);
            // 
            // maxTypeLabel
            // 
            this.maxTypeLabel.AutoSize = true;
            this.maxTypeLabel.BackColor = System.Drawing.SystemColors.Window;
            this.maxTypeLabel.Location = new System.Drawing.Point(1094, 176);
            this.maxTypeLabel.Name = "maxTypeLabel";
            this.maxTypeLabel.Size = new System.Drawing.Size(102, 17);
            this.maxTypeLabel.TabIndex = 3;
            this.maxTypeLabel.Text = "Maximum Type";
            // 
            // dummyToggleLink
            // 
            this.dummyToggleLink.AutoSize = true;
            this.dummyToggleLink.BackColor = System.Drawing.SystemColors.Window;
            this.dummyToggleLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.dummyToggleLink.Location = new System.Drawing.Point(1155, 23);
            this.dummyToggleLink.Name = "dummyToggleLink";
            this.dummyToggleLink.Size = new System.Drawing.Size(41, 15);
            this.dummyToggleLink.TabIndex = 4;
            this.dummyToggleLink.TabStop = true;
            this.dummyToggleLink.Text = "(Hide)";
            this.dummyToggleLink.Visible = false;
            // 
            // ParameterPanel
            // 
            this.ParameterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ParameterPanel.BackColor = System.Drawing.SystemColors.Window;
            this.ParameterPanel.ColumnCount = 2;
            this.ParameterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.08531F));
            this.ParameterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.91469F));
            this.ParameterPanel.Controls.Add(this.label1, 0, 0);
            this.ParameterPanel.Controls.Add(this.label2, 0, 1);
            this.ParameterPanel.Controls.Add(this.label3, 0, 2);
            this.ParameterPanel.Controls.Add(this.label4, 0, 3);
            this.ParameterPanel.Controls.Add(this.ProcessTimeLbl, 1, 0);
            this.ParameterPanel.Controls.Add(this.IntervalLbl, 1, 1);
            this.ParameterPanel.Controls.Add(this.TaskLbl, 1, 2);
            this.ParameterPanel.Controls.Add(this.PersonalLbl, 1, 3);
            this.ParameterPanel.Location = new System.Drawing.Point(990, 492);
            this.ParameterPanel.Name = "ParameterPanel";
            this.ParameterPanel.RowCount = 4;
            this.ParameterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ParameterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ParameterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ParameterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ParameterPanel.Size = new System.Drawing.Size(215, 107);
            this.ParameterPanel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Time (sec)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Interval Duration";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Task Maximum";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Personal Maximum";
            // 
            // ProcessTimeLbl
            // 
            this.ProcessTimeLbl.AutoSize = true;
            this.ProcessTimeLbl.Location = new System.Drawing.Point(136, 0);
            this.ProcessTimeLbl.Name = "ProcessTimeLbl";
            this.ProcessTimeLbl.Size = new System.Drawing.Size(58, 17);
            this.ProcessTimeLbl.TabIndex = 4;
            this.ProcessTimeLbl.Text = "process";
            // 
            // IntervalLbl
            // 
            this.IntervalLbl.AutoSize = true;
            this.IntervalLbl.Location = new System.Drawing.Point(136, 26);
            this.IntervalLbl.Name = "IntervalLbl";
            this.IntervalLbl.Size = new System.Drawing.Size(54, 17);
            this.IntervalLbl.TabIndex = 5;
            this.IntervalLbl.Text = "interval";
            // 
            // TaskLbl
            // 
            this.TaskLbl.AutoSize = true;
            this.TaskLbl.Location = new System.Drawing.Point(136, 52);
            this.TaskLbl.Name = "TaskLbl";
            this.TaskLbl.Size = new System.Drawing.Size(34, 17);
            this.TaskLbl.TabIndex = 6;
            this.TaskLbl.Text = "task";
            // 
            // PersonalLbl
            // 
            this.PersonalLbl.AutoSize = true;
            this.PersonalLbl.Location = new System.Drawing.Point(136, 78);
            this.PersonalLbl.Name = "PersonalLbl";
            this.PersonalLbl.Size = new System.Drawing.Size(63, 17);
            this.PersonalLbl.TabIndex = 7;
            this.PersonalLbl.Text = "personal";
            // 
            // FluencyGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1217, 696);
            this.Controls.Add(this.ParameterPanel);
            this.Controls.Add(this.dummyToggleLink);
            this.Controls.Add(this.maxTypeLabel);
            this.Controls.Add(this.maxTypeSelect);
            this.Controls.Add(this.VisualSaveButton);
            this.Controls.Add(this.chart1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FluencyGraph";
            this.Text = "Fluency Graph";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ParameterPanel.ResumeLayout(false);
            this.ParameterPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog VisualSaveFileDialog;
        private System.Windows.Forms.ToolTip SaveToolTip;
        private System.Windows.Forms.Label maxTypeLabel;
        private System.Windows.Forms.LinkLabel dummyToggleLink;
        protected System.Windows.Forms.ComboBox maxTypeSelect;
        protected System.Windows.Forms.Button VisualSaveButton;
        protected System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TableLayoutPanel ParameterPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ProcessTimeLbl;
        private System.Windows.Forms.Label IntervalLbl;
        private System.Windows.Forms.Label TaskLbl;
        private System.Windows.Forms.Label PersonalLbl;
    }
}