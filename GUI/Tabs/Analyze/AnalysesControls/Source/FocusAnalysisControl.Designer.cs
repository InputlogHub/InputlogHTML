using System.Drawing;

namespace GUI.Tabs.Analyze.AnalysesControls.Source
{
    partial class FocusAnalysisControl
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
            this.MoreInfoLabel = new System.Windows.Forms.LinkLabel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.AddPajekFileCBx = new System.Windows.Forms.CheckBox();
            this.FixedNumberOfIntervalsPanel = new System.Windows.Forms.Panel();
            this.NumberOfIntervalsField = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IntervalNmbrErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.PajekTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.FixedNumberOfIntervalsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // MoreInfoLabel
            // 
            this.MoreInfoLabel.AutoSize = true;
            this.MoreInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoreInfoLabel.Location = new System.Drawing.Point(230, 10);
            this.MoreInfoLabel.Margin = new System.Windows.Forms.Padding(4);
            this.MoreInfoLabel.Name = "MoreInfoLabel";
            this.MoreInfoLabel.Size = new System.Drawing.Size(51, 13);
            this.MoreInfoLabel.TabIndex = 3;
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
            this.NameLabel.Size = new System.Drawing.Size(60, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Source";
            // 
            // AddPajekFileCBx
            // 
            this.AddPajekFileCBx.AutoSize = true;
            this.AddPajekFileCBx.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AddPajekFileCBx.Location = new System.Drawing.Point(4, 66);
            this.AddPajekFileCBx.Name = "AddPajekFileCBx";
            this.AddPajekFileCBx.Size = new System.Drawing.Size(224, 17);
            this.AddPajekFileCBx.TabIndex = 4;
            this.AddPajekFileCBx.Text = "Add a visualization file in Pajek format       ";
            this.AddPajekFileCBx.UseVisualStyleBackColor = true;
            // 
            // FixedNumberOfIntervalsPanel
            // 
            this.FixedNumberOfIntervalsPanel.AutoSize = true;
            this.FixedNumberOfIntervalsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.NumberOfIntervalsField);
            this.FixedNumberOfIntervalsPanel.Controls.Add(this.label2);
            this.FixedNumberOfIntervalsPanel.Location = new System.Drawing.Point(0, 32);
            this.FixedNumberOfIntervalsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.FixedNumberOfIntervalsPanel.Name = "FixedNumberOfIntervalsPanel";
            this.FixedNumberOfIntervalsPanel.Size = new System.Drawing.Size(186, 29);
            this.FixedNumberOfIntervalsPanel.TabIndex = 25;
            // 
            // NumberOfIntervalsField
            // 
            this.NumberOfIntervalsField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.NumberOfIntervalsField.FormattingEnabled = true;
            this.NumberOfIntervalsField.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.NumberOfIntervalsField.Location = new System.Drawing.Point(115, 4);
            this.NumberOfIntervalsField.Margin = new System.Windows.Forms.Padding(4);
            this.NumberOfIntervalsField.Name = "NumberOfIntervalsField";
            this.NumberOfIntervalsField.Size = new System.Drawing.Size(67, 21);
            this.NumberOfIntervalsField.TabIndex = 16;
            this.NumberOfIntervalsField.Text = "5";
            this.NumberOfIntervalsField.Validating += new System.ComponentModel.CancelEventHandler(this.NumberOfIntervalsFieldValidating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(5, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Number of Intervals";
            // 
            // IntervalNmbrErrorProvider
            // 
            this.IntervalNmbrErrorProvider.ContainerControl = this;
            // 
            // PajekTooltip
            // 
            this.PajekTooltip.AutoPopDelay = 10000;
            this.PajekTooltip.InitialDelay = 500;
            this.PajekTooltip.IsBalloon = true;
            this.PajekTooltip.ReshowDelay = 100;
            this.PajekTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.PajekTooltip.ToolTipTitle = "Pajek Graph";
            // 
            // FocusAnalysisControl
            // 
            this.Controls.Add(this.FixedNumberOfIntervalsPanel);
            this.Controls.Add(this.AddPajekFileCBx);
            this.Controls.Add(this.MoreInfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "FocusAnalysisControl";
            this.Size = new System.Drawing.Size(285, 86);
            this.FixedNumberOfIntervalsPanel.ResumeLayout(false);
            this.FixedNumberOfIntervalsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IntervalNmbrErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel MoreInfoLabel;
        private System.Windows.Forms.CheckBox AddPajekFileCBx;
        private System.Windows.Forms.Panel FixedNumberOfIntervalsPanel;
        private System.Windows.Forms.ComboBox NumberOfIntervalsField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ErrorProvider IntervalNmbrErrorProvider;
        private System.Windows.Forms.ToolTip PajekTooltip;
    }
}
