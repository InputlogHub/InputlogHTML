namespace GUI.Tabs.Replay
{
    partial class Play
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
            if (disposing)
            {
                if (Replaying)
                {
                    Engine.Dispose();
                }

                if (components != null)
                {
                    components.Dispose();
                }

                if (PlayTimer != null)
                {
                    PlayTimer.Dispose();
                }
            }

            Engine = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Play));
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ToEndButton = new System.Windows.Forms.Button();
            this.ToBeginningButton = new System.Windows.Forms.Button();
            this.NextRevisionButton = new System.Windows.Forms.Button();
            this.PlayPauseButton = new System.Windows.Forms.Button();
            this.PlaySpeedBar = new System.Windows.Forms.TrackBar();
            this.PreviousRevisionButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.PreviousButton = new System.Windows.Forms.Button();
            this.SrcDocLabel = new System.Windows.Forms.Label();
            this.PlayTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.EmbeddedWord = new GUI.Util.EmbeddedWord();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.DocLengthBox = new System.Windows.Forms.TextBox();
            this.PositionBox = new System.Windows.Forms.TextBox();
            this.EditsBox = new System.Windows.Forms.TextBox();
            this.DocLengthLabel = new System.Windows.Forms.Label();
            this.PositionLabel = new System.Windows.Forms.Label();
            this.EditsLabel = new System.Windows.Forms.Label();
            this.RevisionBox = new System.Windows.Forms.TextBox();
            this.RevisionLabel = new System.Windows.Forms.Label();
            this.SrcFilePanel = new System.Windows.Forms.Panel();
            this.SrcDocTextField = new System.Windows.Forms.TextBox();
            this.SrcDocButton = new System.Windows.Forms.Button();
            this.SrcFileTextField = new System.Windows.Forms.TextBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.SrcFileButton = new System.Windows.Forms.Button();
            this.SrcFileLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PlaySpeedBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.ControlPanel.SuspendLayout();
            this.SrcFilePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileDialog
            // 
            this.FileDialog.DefaultExt = "idfx";
            this.FileDialog.Filter = "Log files (*.idfx)|*.idfx";
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorLabel.Location = new System.Drawing.Point(29, 25);
            this.ErrorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ErrorLabel.MaximumSize = new System.Drawing.Size(800, 492);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(37, 16);
            this.ErrorLabel.TabIndex = 44;
            this.ErrorLabel.Text = "Error";
            this.ErrorLabel.Visible = false;
            // 
            // ToEndButton
            // 
            this.ToEndButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ToEndButton.Image = ((System.Drawing.Image)(resources.GetObject("ToEndButton.Image")));
            this.ToEndButton.Location = new System.Drawing.Point(533, 20);
            this.ToEndButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ToEndButton.Name = "ToEndButton";
            this.ToEndButton.Size = new System.Drawing.Size(39, 36);
            this.ToEndButton.TabIndex = 6;
            this.Tooltip.SetToolTip(this.ToEndButton, "Go to the final revision");
            this.ToEndButton.UseVisualStyleBackColor = true;
            this.ToEndButton.Click += new System.EventHandler(this.ToEndButtonClick);
            // 
            // ToBeginningButton
            // 
            this.ToBeginningButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ToBeginningButton.Image = ((System.Drawing.Image)(resources.GetObject("ToBeginningButton.Image")));
            this.ToBeginningButton.Location = new System.Drawing.Point(253, 20);
            this.ToBeginningButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ToBeginningButton.Name = "ToBeginningButton";
            this.ToBeginningButton.Size = new System.Drawing.Size(39, 36);
            this.ToBeginningButton.TabIndex = 5;
            this.Tooltip.SetToolTip(this.ToBeginningButton, "Reload the original document");
            this.ToBeginningButton.UseVisualStyleBackColor = true;
            this.ToBeginningButton.Click += new System.EventHandler(this.ToBeginningButtonClick);
            // 
            // NextRevisionButton
            // 
            this.NextRevisionButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NextRevisionButton.Image = global::GUI.Properties.Resources.media_skip_forward;
            this.NextRevisionButton.Location = new System.Drawing.Point(486, 20);
            this.NextRevisionButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NextRevisionButton.Name = "NextRevisionButton";
            this.NextRevisionButton.Size = new System.Drawing.Size(39, 36);
            this.NextRevisionButton.TabIndex = 2;
            this.Tooltip.SetToolTip(this.NextRevisionButton, "Advance one revision");
            this.NextRevisionButton.UseVisualStyleBackColor = true;
            this.NextRevisionButton.Click += new System.EventHandler(this.NextRevisionButtonClick);
            // 
            // PlayPauseButton
            // 
            this.PlayPauseButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PlayPauseButton.Image = global::GUI.Properties.Resources.media_playback_start;
            this.PlayPauseButton.Location = new System.Drawing.Point(393, 20);
            this.PlayPauseButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayPauseButton.Name = "PlayPauseButton";
            this.PlayPauseButton.Size = new System.Drawing.Size(39, 36);
            this.PlayPauseButton.TabIndex = 0;
            this.Tooltip.SetToolTip(this.PlayPauseButton, "Advance automatically over the edits");
            this.PlayPauseButton.UseVisualStyleBackColor = true;
            this.PlayPauseButton.Click += new System.EventHandler(this.PlayPauseButtonClick);
            // 
            // PlaySpeedBar
            // 
            this.PlaySpeedBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PlaySpeedBar.AutoSize = false;
            this.PlaySpeedBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.PlaySpeedBar.LargeChange = 10;
            this.PlaySpeedBar.Location = new System.Drawing.Point(300, 66);
            this.PlaySpeedBar.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.PlaySpeedBar.Maximum = 100;
            this.PlaySpeedBar.Name = "PlaySpeedBar";
            this.PlaySpeedBar.Size = new System.Drawing.Size(225, 25);
            this.PlaySpeedBar.TabIndex = 2;
            this.PlaySpeedBar.TickFrequency = 10;
            this.Tooltip.SetToolTip(this.PlaySpeedBar, "Play speed");
            this.PlaySpeedBar.Value = 50;
            // 
            // PreviousRevisionButton
            // 
            this.PreviousRevisionButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PreviousRevisionButton.Image = global::GUI.Properties.Resources.media_skip_backward;
            this.PreviousRevisionButton.Location = new System.Drawing.Point(300, 20);
            this.PreviousRevisionButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PreviousRevisionButton.Name = "PreviousRevisionButton";
            this.PreviousRevisionButton.Size = new System.Drawing.Size(39, 36);
            this.PreviousRevisionButton.TabIndex = 4;
            this.Tooltip.SetToolTip(this.PreviousRevisionButton, "Go back one revision");
            this.PreviousRevisionButton.UseVisualStyleBackColor = true;
            this.PreviousRevisionButton.Click += new System.EventHandler(this.PreviousRevisionButtonClick);
            // 
            // NextButton
            // 
            this.NextButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NextButton.Image = global::GUI.Properties.Resources.media_seek_forward;
            this.NextButton.Location = new System.Drawing.Point(440, 20);
            this.NextButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(39, 36);
            this.NextButton.TabIndex = 1;
            this.Tooltip.SetToolTip(this.NextButton, "Advance one edit");
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButtonClick);
            // 
            // PreviousButton
            // 
            this.PreviousButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PreviousButton.Image = global::GUI.Properties.Resources.media_seek_backward;
            this.PreviousButton.Location = new System.Drawing.Point(346, 20);
            this.PreviousButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new System.Drawing.Size(39, 36);
            this.PreviousButton.TabIndex = 3;
            this.Tooltip.SetToolTip(this.PreviousButton, "Go back one edit");
            this.PreviousButton.UseVisualStyleBackColor = true;
            this.PreviousButton.Click += new System.EventHandler(this.PreviousButtonClick);
            // 
            // SrcDocLabel
            // 
            this.SrcDocLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SrcDocLabel.AutoSize = true;
            this.SrcDocLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrcDocLabel.Location = new System.Drawing.Point(-2, 60);
            this.SrcDocLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.SrcDocLabel.Name = "SrcDocLabel";
            this.SrcDocLabel.Size = new System.Drawing.Size(171, 13);
            this.SrcDocLabel.TabIndex = 36;
            this.SrcDocLabel.Text = "Original Word Document - Optional";
            this.Tooltip.SetToolTip(this.SrcDocLabel, "The original document is only needed\nif it was changed by this log file.");
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.EmbeddedWord);
            this.panel1.Controls.Add(this.ControlPanel);
            this.panel1.Controls.Add(this.SrcFilePanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(25, 25);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(822, 487);
            this.panel1.TabIndex = 45;
            // 
            // EmbeddedWord
            // 
            this.EmbeddedWord.AutoSize = true;
            this.EmbeddedWord.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.EmbeddedWord.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.EmbeddedWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EmbeddedWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.EmbeddedWord.Location = new System.Drawing.Point(0, 130);
            this.EmbeddedWord.Margin = new System.Windows.Forms.Padding(0);
            this.EmbeddedWord.Name = "EmbeddedWord";
            this.EmbeddedWord.Padding = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.EmbeddedWord.Size = new System.Drawing.Size(822, 256);
            this.EmbeddedWord.TabIndex = 46;
            this.EmbeddedWord.Load += new System.EventHandler(this.EmbeddedWord_Load);
            // 
            // ControlPanel
            // 
            this.ControlPanel.AutoSize = true;
            this.ControlPanel.Controls.Add(this.DocLengthBox);
            this.ControlPanel.Controls.Add(this.PositionBox);
            this.ControlPanel.Controls.Add(this.EditsBox);
            this.ControlPanel.Controls.Add(this.DocLengthLabel);
            this.ControlPanel.Controls.Add(this.PositionLabel);
            this.ControlPanel.Controls.Add(this.EditsLabel);
            this.ControlPanel.Controls.Add(this.RevisionBox);
            this.ControlPanel.Controls.Add(this.ToEndButton);
            this.ControlPanel.Controls.Add(this.ToBeginningButton);
            this.ControlPanel.Controls.Add(this.NextRevisionButton);
            this.ControlPanel.Controls.Add(this.RevisionLabel);
            this.ControlPanel.Controls.Add(this.PlayPauseButton);
            this.ControlPanel.Controls.Add(this.PlaySpeedBar);
            this.ControlPanel.Controls.Add(this.PreviousRevisionButton);
            this.ControlPanel.Controls.Add(this.NextButton);
            this.ControlPanel.Controls.Add(this.PreviousButton);
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ControlPanel.Location = new System.Drawing.Point(0, 386);
            this.ControlPanel.Margin = new System.Windows.Forms.Padding(25, 25, 25, 50);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Padding = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.ControlPanel.Size = new System.Drawing.Size(822, 101);
            this.ControlPanel.TabIndex = 45;
            // 
            // DocLengthBox
            // 
            this.DocLengthBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DocLengthBox.Enabled = false;
            this.DocLengthBox.Location = new System.Drawing.Point(762, 71);
            this.DocLengthBox.Margin = new System.Windows.Forms.Padding(4, 4, 5, 4);
            this.DocLengthBox.MaxLength = 10;
            this.DocLengthBox.Name = "DocLengthBox";
            this.DocLengthBox.Size = new System.Drawing.Size(60, 20);
            this.DocLengthBox.TabIndex = 12;
            // 
            // PositionBox
            // 
            this.PositionBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.PositionBox.Enabled = false;
            this.PositionBox.Location = new System.Drawing.Point(691, 71);
            this.PositionBox.Margin = new System.Windows.Forms.Padding(4, 4, 5, 4);
            this.PositionBox.MaxLength = 10;
            this.PositionBox.Name = "PositionBox";
            this.PositionBox.Size = new System.Drawing.Size(60, 20);
            this.PositionBox.TabIndex = 11;
            // 
            // EditsBox
            // 
            this.EditsBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.EditsBox.Enabled = false;
            this.EditsBox.Location = new System.Drawing.Point(762, 27);
            this.EditsBox.Margin = new System.Windows.Forms.Padding(4, 4, 5, 4);
            this.EditsBox.MaxLength = 10;
            this.EditsBox.Name = "EditsBox";
            this.EditsBox.Size = new System.Drawing.Size(60, 20);
            this.EditsBox.TabIndex = 10;
            // 
            // DocLengthLabel
            // 
            this.DocLengthLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DocLengthLabel.AutoSize = true;
            this.DocLengthLabel.Location = new System.Drawing.Point(758, 54);
            this.DocLengthLabel.Name = "DocLengthLabel";
            this.DocLengthLabel.Size = new System.Drawing.Size(60, 13);
            this.DocLengthLabel.TabIndex = 9;
            this.DocLengthLabel.Text = "DocLength";
            // 
            // PositionLabel
            // 
            this.PositionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.PositionLabel.AutoSize = true;
            this.PositionLabel.Location = new System.Drawing.Point(688, 54);
            this.PositionLabel.Name = "PositionLabel";
            this.PositionLabel.Size = new System.Drawing.Size(44, 13);
            this.PositionLabel.TabIndex = 8;
            this.PositionLabel.Text = "Position";
            // 
            // EditsLabel
            // 
            this.EditsLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.EditsLabel.AutoSize = true;
            this.EditsLabel.Location = new System.Drawing.Point(758, 9);
            this.EditsLabel.Name = "EditsLabel";
            this.EditsLabel.Size = new System.Drawing.Size(30, 13);
            this.EditsLabel.TabIndex = 7;
            this.EditsLabel.Text = "Edits";
            // 
            // RevisionBox
            // 
            this.RevisionBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.RevisionBox.Enabled = false;
            this.RevisionBox.Location = new System.Drawing.Point(691, 27);
            this.RevisionBox.Margin = new System.Windows.Forms.Padding(4, 4, 5, 4);
            this.RevisionBox.MaxLength = 10;
            this.RevisionBox.Name = "RevisionBox";
            this.RevisionBox.Size = new System.Drawing.Size(60, 20);
            this.RevisionBox.TabIndex = 1;
            // 
            // RevisionLabel
            // 
            this.RevisionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.RevisionLabel.AutoSize = true;
            this.RevisionLabel.Location = new System.Drawing.Point(689, 9);
            this.RevisionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RevisionLabel.Name = "RevisionLabel";
            this.RevisionLabel.Size = new System.Drawing.Size(48, 13);
            this.RevisionLabel.TabIndex = 0;
            this.RevisionLabel.Text = "Revision";
            // 
            // SrcFilePanel
            // 
            this.SrcFilePanel.Controls.Add(this.SrcDocLabel);
            this.SrcFilePanel.Controls.Add(this.SrcDocTextField);
            this.SrcFilePanel.Controls.Add(this.SrcDocButton);
            this.SrcFilePanel.Controls.Add(this.SrcFileTextField);
            this.SrcFilePanel.Controls.Add(this.StartButton);
            this.SrcFilePanel.Controls.Add(this.SrcFileButton);
            this.SrcFilePanel.Controls.Add(this.SrcFileLabel);
            this.SrcFilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SrcFilePanel.Location = new System.Drawing.Point(0, 0);
            this.SrcFilePanel.Margin = new System.Windows.Forms.Padding(25);
            this.SrcFilePanel.Name = "SrcFilePanel";
            this.SrcFilePanel.Size = new System.Drawing.Size(822, 130);
            this.SrcFilePanel.TabIndex = 44;
            // 
            // SrcDocTextField
            // 
            this.SrcDocTextField.Location = new System.Drawing.Point(1, 85);
            this.SrcDocTextField.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.SrcDocTextField.Name = "SrcDocTextField";
            this.SrcDocTextField.Size = new System.Drawing.Size(321, 20);
            this.SrcDocTextField.TabIndex = 35;
            // 
            // SrcDocButton
            // 
            this.SrcDocButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.SrcDocButton.Location = new System.Drawing.Point(329, 81);
            this.SrcDocButton.Margin = new System.Windows.Forms.Padding(0);
            this.SrcDocButton.Name = "SrcDocButton";
            this.SrcDocButton.Size = new System.Drawing.Size(70, 28);
            this.SrcDocButton.TabIndex = 34;
            this.SrcDocButton.UseVisualStyleBackColor = true;
            this.SrcDocButton.Click += new System.EventHandler(this.SrcDocButtonClick);
            // 
            // SrcFileTextField
            // 
            this.SrcFileTextField.AllowDrop = true;
            this.SrcFileTextField.Location = new System.Drawing.Point(1, 25);
            this.SrcFileTextField.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.SrcFileTextField.Name = "SrcFileTextField";
            this.SrcFileTextField.Size = new System.Drawing.Size(321, 20);
            this.SrcFileTextField.TabIndex = 31;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(480, 80);
            this.StartButton.Margin = new System.Windows.Forms.Padding(4);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(100, 30);
            this.StartButton.TabIndex = 33;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // SrcFileButton
            // 
            this.SrcFileButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.SrcFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrcFileButton.Location = new System.Drawing.Point(329, 21);
            this.SrcFileButton.Margin = new System.Windows.Forms.Padding(0);
            this.SrcFileButton.Name = "SrcFileButton";
            this.SrcFileButton.Size = new System.Drawing.Size(70, 28);
            this.SrcFileButton.TabIndex = 32;
            this.SrcFileButton.UseVisualStyleBackColor = true;
            this.SrcFileButton.Click += new System.EventHandler(this.SrcFileButtonClick);
            // 
            // SrcFileLabel
            // 
            this.SrcFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SrcFileLabel.AutoSize = true;
            this.SrcFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrcFileLabel.Location = new System.Drawing.Point(-3, 0);
            this.SrcFileLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.SrcFileLabel.Name = "SrcFileLabel";
            this.SrcFileLabel.Size = new System.Drawing.Size(64, 13);
            this.SrcFileLabel.TabIndex = 30;
            this.SrcFileLabel.Text = "Logging File";
            // 
            // Play
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Play";
            this.Padding = new System.Windows.Forms.Padding(25);
            this.Size = new System.Drawing.Size(872, 537);
            this.Load += new System.EventHandler(this.PlayLoad);
            ((System.ComponentModel.ISupportInitialize)(this.PlaySpeedBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ControlPanel.ResumeLayout(false);
            this.ControlPanel.PerformLayout();
            this.SrcFilePanel.ResumeLayout(false);
            this.SrcFilePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.ToolTip Tooltip;
        private System.Windows.Forms.Timer PlayTimer;
        private System.Windows.Forms.Panel panel1;
        private Util.EmbeddedWord EmbeddedWord;
        private System.Windows.Forms.Panel ControlPanel;
        private System.Windows.Forms.TextBox DocLengthBox;
        private System.Windows.Forms.TextBox PositionBox;
        private System.Windows.Forms.TextBox EditsBox;
        private System.Windows.Forms.Label DocLengthLabel;
        private System.Windows.Forms.Label PositionLabel;
        private System.Windows.Forms.Label EditsLabel;
        private System.Windows.Forms.TextBox RevisionBox;
        private System.Windows.Forms.Button ToEndButton;
        private System.Windows.Forms.Button ToBeginningButton;
        private System.Windows.Forms.Button NextRevisionButton;
        private System.Windows.Forms.Label RevisionLabel;
        private System.Windows.Forms.Button PlayPauseButton;
        private System.Windows.Forms.TrackBar PlaySpeedBar;
        private System.Windows.Forms.Button PreviousRevisionButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button PreviousButton;
        private System.Windows.Forms.Panel SrcFilePanel;
        private System.Windows.Forms.Label SrcDocLabel;
        private System.Windows.Forms.TextBox SrcDocTextField;
        private System.Windows.Forms.Button SrcDocButton;
        public System.Windows.Forms.TextBox SrcFileTextField;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button SrcFileButton;
        private System.Windows.Forms.Label SrcFileLabel;


    }
}
