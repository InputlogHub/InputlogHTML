using GUI.Tabs.Preprocess.Filters;
using GUI.Tabs.Preprocess.Recoders;

namespace GUI.Tabs.Preprocess
{
    partial class PostProcessDialog
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
            this.ManipulatorList = new System.Windows.Forms.ComboBox();
            this.AddFilterButton = new System.Windows.Forms.Button();
            this.AnalyzeFiltersLabel = new System.Windows.Forms.Label();
            this.SelectedManipulatorsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ManipulatorList
            // 
            this.ManipulatorList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ManipulatorList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ManipulatorList.FormattingEnabled = true;
            this.ManipulatorList.Location = new System.Drawing.Point(24, 23);
            this.ManipulatorList.Margin = new System.Windows.Forms.Padding(11);
            this.ManipulatorList.Name = "ManipulatorList";
            this.ManipulatorList.Size = new System.Drawing.Size(406, 25);
            this.ManipulatorList.TabIndex = 43;
            // 
            // AddFilterButton
            // 
            this.AddFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddFilterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AddFilterButton.Image = global::GUI.Properties.Resources.add;
            this.AddFilterButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddFilterButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddFilterButton.Location = new System.Drawing.Point(562, 23);
            this.AddFilterButton.Margin = new System.Windows.Forms.Padding(11);
            this.AddFilterButton.Name = "AddFilterButton";
            this.AddFilterButton.Size = new System.Drawing.Size(149, 32);
            this.AddFilterButton.TabIndex = 44;
            this.AddFilterButton.Text = "Add";
            this.AddFilterButton.UseVisualStyleBackColor = true;
            this.AddFilterButton.Click += new System.EventHandler(this.AddManipulatorButtonClick);
            // 
            // AnalyzeFiltersLabel
            // 
            this.AnalyzeFiltersLabel.AutoSize = true;
            this.AnalyzeFiltersLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AnalyzeFiltersLabel.Location = new System.Drawing.Point(-128, 38);
            this.AnalyzeFiltersLabel.Margin = new System.Windows.Forms.Padding(11, 11, 11, 0);
            this.AnalyzeFiltersLabel.Name = "AnalyzeFiltersLabel";
            this.AnalyzeFiltersLabel.Size = new System.Drawing.Size(46, 17);
            this.AnalyzeFiltersLabel.TabIndex = 42;
            this.AnalyzeFiltersLabel.Text = "Filters";
            // 
            // SelectedManipulatorsPanel
            // 
            this.SelectedManipulatorsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedManipulatorsPanel.AutoScroll = true;
            this.SelectedManipulatorsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SelectedManipulatorsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SelectedManipulatorsPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SelectedManipulatorsPanel.Location = new System.Drawing.Point(24, 70);
            this.SelectedManipulatorsPanel.Margin = new System.Windows.Forms.Padding(11);
            this.SelectedManipulatorsPanel.Name = "SelectedManipulatorsPanel";
            this.SelectedManipulatorsPanel.Size = new System.Drawing.Size(687, 452);
            this.SelectedManipulatorsPanel.TabIndex = 46;
            this.SelectedManipulatorsPanel.WrapContents = false;
            // 
            // ProcessButton
            // 
            this.ProcessButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProcessButton.Location = new System.Drawing.Point(611, 537);
            this.ProcessButton.Margin = new System.Windows.Forms.Padding(4);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(100, 32);
            this.ProcessButton.TabIndex = 47;
            this.ProcessButton.Text = "Confirm";
            this.ProcessButton.UseCompatibleTextRendering = true;
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.OKButtonClick);
            // 
            // UpButton
            // 
            this.UpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpButton.Enabled = false;
            this.UpButton.Image = global::GUI.Properties.Resources.up;
            this.UpButton.Location = new System.Drawing.Point(726, 332);
            this.UpButton.Margin = new System.Windows.Forms.Padding(4);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(37, 36);
            this.UpButton.TabIndex = 48;
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButtonClick);
            // 
            // DownButton
            // 
            this.DownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DownButton.Enabled = false;
            this.DownButton.Image = global::GUI.Properties.Resources.down;
            this.DownButton.Location = new System.Drawing.Point(726, 376);
            this.DownButton.Margin = new System.Windows.Forms.Padding(4);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(37, 36);
            this.DownButton.TabIndex = 49;
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButtonClick);
            // 
            // PostProcessDialog
            // 
            this.AcceptButton = this.ProcessButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 582);
            this.Controls.Add(this.DownButton);
            this.Controls.Add(this.UpButton);
            this.Controls.Add(this.ProcessButton);
            this.Controls.Add(this.SelectedManipulatorsPanel);
            this.Controls.Add(this.ManipulatorList);
            this.Controls.Add(this.AddFilterButton);
            this.Controls.Add(this.AnalyzeFiltersLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PostProcessDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Post Processing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ManipulatorList;
        private System.Windows.Forms.Button AddFilterButton;
        private System.Windows.Forms.Label AnalyzeFiltersLabel;
        public System.Windows.Forms.FlowLayoutPanel SelectedManipulatorsPanel;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.Button DownButton;

        /// <summary>
        /// The wrapper of the manipulator that is currently selected (or null if no manipulator is selected). Use 
        /// the SelectedManipulator property to access this member in order to enable/disable the up and down 
        /// buttons automatically according to the position of the selected manipulator in the list.
        /// </summary>
        private FilterWrapper selectedManipulator;
    }
}