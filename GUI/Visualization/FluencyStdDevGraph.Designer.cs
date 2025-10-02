namespace GUI.Visualization
{
    partial class FluencyStdDevGraph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FluencyStdDevGraph));
            this.VisualSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.VisualSaveButton = new System.Windows.Forms.Button();
            this.SaveToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // VisualSaveFileDialog
            // 
            this.VisualSaveFileDialog.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg|TIFF|*.tiff|EMF|*.emf";
            // 
            // VisualSaveButton
            // 
            this.VisualSaveButton.Location = new System.Drawing.Point(1098, 636);
            this.VisualSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.VisualSaveButton.Name = "VisualSaveButton";
            this.VisualSaveButton.Size = new System.Drawing.Size(82, 28);
            this.VisualSaveButton.TabIndex = 1;
            this.VisualSaveButton.Text = "Save";
            this.SaveToolTip.SetToolTip(this.VisualSaveButton, "The original chart has been saved automatically. \r\nUse \'Save\' button when the cha" +
        "rt has been changed,\nor when you need a different format.");
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
            chartArea1.AxisX.Interval = 10D;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.AcrossAxis;
            chartArea1.AxisX.Maximum = 100D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "Pers. optimum %";
            chartArea1.AxisX2.MajorGrid.Enabled = false;
            chartArea1.AxisX2.MajorTickMark.Enabled = false;
            chartArea1.AxisY.Crossing = -1.7976931348623157E+308D;
            chartArea1.AxisY.Interval = 10D;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.Title = "Sd";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.IsLabelAutoFit = false;
            chartArea1.AxisY2.LabelStyle.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY2.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.Title = "Characters";
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "MainArea";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 86F;
            chartArea1.Position.Width = 80F;
            chartArea1.Position.X = 1F;
            chartArea1.Position.Y = 3F;
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(13, 13);
            this.chart1.Margin = new System.Windows.Forms.Padding(4);
            this.chart1.Name = "chart1";
            series1.ChartArea = "MainArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.Legend = "Legend1";
            series1.MarkerSize = 10;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series1.Name = "Plot";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1188, 615);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // FluencyStdDevGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1217, 696);
            this.Controls.Add(this.VisualSaveButton);
            this.Controls.Add(this.chart1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FluencyStdDevGraph";
            this.Text = "Fluency Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VisualizationFormFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog VisualSaveFileDialog;
        private System.Windows.Forms.ToolTip SaveToolTip;
        protected System.Windows.Forms.Button VisualSaveButton;
        protected System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

