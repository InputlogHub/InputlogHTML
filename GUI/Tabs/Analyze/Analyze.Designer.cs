namespace GUI.Tabs.Analyze
{
    partial class Analyze
    {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            PresentationControls.CheckBoxProperties checkBoxProperties2 = new PresentationControls.CheckBoxProperties();
            this.SrcFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ImportConfigurationDialog = new System.Windows.Forms.OpenFileDialog();
            this.DstDirectoryDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.OrgDocLabel = new System.Windows.Forms.Label();
            this.OrgDocFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.VisualizeAnalysisFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectedAnalysesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DisablePanel = new System.Windows.Forms.Panel();
            this.ReportOutputCList = new PresentationControls.CheckBoxComboBox();
            this.ReportRadioBttn = new System.Windows.Forms.RadioButton();
            this.AnalysesRadioBttn = new System.Windows.Forms.RadioButton();
            this.ReportsList = new System.Windows.Forms.ComboBox();
            this.AddReportButton = new System.Windows.Forms.Button();
            this.OrgDocButton = new System.Windows.Forms.Button();
            this.OrgDocTextField = new System.Windows.Forms.TextBox();
            this.NumberSelectedSourceFilesLabel = new System.Windows.Forms.Label();
            this.DstDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.DstDirLabel = new System.Windows.Forms.Label();
            this.DstFileTextField = new System.Windows.Forms.TextBox();
            this.AnalysesList = new PresentationControls.CheckBoxComboBox();
            this.AddAnalysesButton = new System.Windows.Forms.Button();
            this.SrcFileTextField = new System.Windows.Forms.TextBox();
            this.AnalyzeSrcFileLabel = new System.Windows.Forms.Label();
            this.AnalyzeSelectedAnalysesLabel = new System.Windows.Forms.Label();
            this.SrcFileButton = new System.Windows.Forms.Button();
            this.ButtonPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.ImportConfigurationButton = new System.Windows.Forms.Button();
            this.ExportConfigurationButton = new System.Windows.Forms.Button();
            this.ClearAnalysesButton = new System.Windows.Forms.Button();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.AnalyzeButton = new System.Windows.Forms.Button();
            this.OpenTemplateDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.DisablePanel.SuspendLayout();
            this.ButtonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SrcFileDialog
            // 
            // change the Filter to include an "Other files" choice (All files)
            this.SrcFileDialog.DefaultExt = "idfx";
            this.SrcFileDialog.Filter = "Log files (*.idfx)|*.idfx|Other files (*.*)|*.*";
            this.SrcFileDialog.FilterIndex = 1; // keep "Log files" selected by default
            this.SrcFileDialog.Multiselect = true;
            // 
            // ImportConfigurationDialog
            // 
            this.ImportConfigurationDialog.Filter = "Inputlog Analysis Configuration Files|*.iafx|All Files|*.*";
            // 
            // OrgDocLabel
            // 
            this.OrgDocLabel.AutoSize = true;
            this.OrgDocLabel.Location = new System.Drawing.Point(-2, 60);
            this.OrgDocLabel.Name = "OrgDocLabel";
            this.OrgDocLabel.Size = new System.Drawing.Size(171, 13);
            this.OrgDocLabel.TabIndex = 49;
            this.OrgDocLabel.Text = "Original Word Document - Optional";
            this.toolTip1.SetToolTip(this.OrgDocLabel, "Use the original document if you perform an S-Notation\r\nor Linguistic Analysis an" +
        "d if the content of the \r\noriginal document has been changed in this session.");
            // 
            // OrgDocFileDialog
            // 
            this.OrgDocFileDialog.DefaultExt = "docx";
            this.OrgDocFileDialog.Filter = "Doc files (*.docx;*.doc)|*.docx;*.doc";
            this.OrgDocFileDialog.Multiselect = true;
            // 
            // VisualizeAnalysisFileDialog
            // 
            this.VisualizeAnalysisFileDialog.FileName = "analysis";
            this.VisualizeAnalysisFileDialog.Filter = "Analysis files|*.xml";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.SelectedAnalysesPanel);
            this.panel1.Controls.Add(this.DisablePanel);
            this.panel1.Controls.Add(this.ButtonPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(25, 25);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 454);
            this.panel1.TabIndex = 0;
            // 
            // SelectedAnalysesPanel
            // 
            this.SelectedAnalysesPanel.AutoScroll = true;
            this.SelectedAnalysesPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SelectedAnalysesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedAnalysesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SelectedAnalysesPanel.Location = new System.Drawing.Point(0, 200);
            this.SelectedAnalysesPanel.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.SelectedAnalysesPanel.Name = "SelectedAnalysesPanel";
            this.SelectedAnalysesPanel.Padding = new System.Windows.Forms.Padding(0, 0, 11, 0);
            this.SelectedAnalysesPanel.Size = new System.Drawing.Size(888, 180);
            this.SelectedAnalysesPanel.TabIndex = 48;
            this.SelectedAnalysesPanel.WrapContents = false;
            // 
            // DisablePanel
            // 
            this.DisablePanel.AllowDrop = true;
            this.DisablePanel.Controls.Add(this.ReportOutputCList);
            this.DisablePanel.Controls.Add(this.ReportRadioBttn);
            this.DisablePanel.Controls.Add(this.AnalysesRadioBttn);
            this.DisablePanel.Controls.Add(this.ReportsList);
            this.DisablePanel.Controls.Add(this.AddReportButton);
            this.DisablePanel.Controls.Add(this.OrgDocButton);
            this.DisablePanel.Controls.Add(this.OrgDocLabel);
            this.DisablePanel.Controls.Add(this.OrgDocTextField);
            this.DisablePanel.Controls.Add(this.NumberSelectedSourceFilesLabel);
            this.DisablePanel.Controls.Add(this.DstDirectoryBrowseButton);
            this.DisablePanel.Controls.Add(this.DstDirLabel);
            this.DisablePanel.Controls.Add(this.DstFileTextField);
            this.DisablePanel.Controls.Add(this.AnalysesList);
            this.DisablePanel.Controls.Add(this.AddAnalysesButton);
            this.DisablePanel.Controls.Add(this.SrcFileTextField);
            this.DisablePanel.Controls.Add(this.AnalyzeSrcFileLabel);
            this.DisablePanel.Controls.Add(this.AnalyzeSelectedAnalysesLabel);
            this.DisablePanel.Controls.Add(this.SrcFileButton);
            this.DisablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.DisablePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DisablePanel.Location = new System.Drawing.Point(0, 0);
            this.DisablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.DisablePanel.Name = "DisablePanel";
            this.DisablePanel.Padding = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.DisablePanel.Size = new System.Drawing.Size(888, 200);
            this.DisablePanel.TabIndex = 46;
            // 
            // ReportOutputCList
            // 
            checkBoxProperties1.AutoSize = true;
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ReportOutputCList.CheckBoxProperties = checkBoxProperties1;
            this.ReportOutputCList.DisplayMemberSingleItem = "";
            this.ReportOutputCList.FormattingEnabled = true;
            this.ReportOutputCList.Location = new System.Drawing.Point(442, 148);
            this.ReportOutputCList.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.ReportOutputCList.MaxDropDownItems = 16;
            this.ReportOutputCList.Name = "ReportOutputCList";
            this.ReportOutputCList.Size = new System.Drawing.Size(106, 21);
            this.ReportOutputCList.TabIndex = 57;
            // 
            // ReportRadioBttn
            // 
            this.ReportRadioBttn.AutoSize = true;
            this.ReportRadioBttn.Location = new System.Drawing.Point(445, 119);
            this.ReportRadioBttn.Name = "ReportRadioBttn";
            this.ReportRadioBttn.Size = new System.Drawing.Size(82, 17);
            this.ReportRadioBttn.TabIndex = 56;
            this.ReportRadioBttn.Text = "User Report";
            this.ReportRadioBttn.UseVisualStyleBackColor = true;
            // 
            // AnalysesRadioBttn
            // 
            this.AnalysesRadioBttn.AutoSize = true;
            this.AnalysesRadioBttn.Checked = true;
            this.AnalysesRadioBttn.Location = new System.Drawing.Point(1, 119);
            this.AnalysesRadioBttn.Name = "AnalysesRadioBttn";
            this.AnalysesRadioBttn.Size = new System.Drawing.Size(67, 17);
            this.AnalysesRadioBttn.TabIndex = 55;
            this.AnalysesRadioBttn.TabStop = true;
            this.AnalysesRadioBttn.Text = "Analyses";
            this.AnalysesRadioBttn.UseVisualStyleBackColor = true;
            this.AnalysesRadioBttn.CheckedChanged += new System.EventHandler(this.AnalysesRadioBttn_CheckedChanged);
            // 
            // ReportsList
            // 
            this.ReportsList.FormattingEnabled = true;
            this.ReportsList.Location = new System.Drawing.Point(559, 147);
            this.ReportsList.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.ReportsList.Name = "ReportsList";
            this.ReportsList.Size = new System.Drawing.Size(183, 21);
            this.ReportsList.TabIndex = 52;
            // 
            // AddReportButton
            // 
            this.AddReportButton.Image = global::GUI.Properties.Resources.add;
            this.AddReportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddReportButton.Location = new System.Drawing.Point(752, 142);
            this.AddReportButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.AddReportButton.Name = "AddReportButton";
            this.AddReportButton.Size = new System.Drawing.Size(70, 30);
            this.AddReportButton.TabIndex = 51;
            this.AddReportButton.Text = "Add";
            this.AddReportButton.UseVisualStyleBackColor = true;
            this.AddReportButton.Click += new System.EventHandler(this.AddReportClick);
            // 
            // OrgDocButton
            // 
            this.OrgDocButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.OrgDocButton.Location = new System.Drawing.Point(310, 81);
            this.OrgDocButton.Name = "OrgDocButton";
            this.OrgDocButton.Size = new System.Drawing.Size(70, 31);
            this.OrgDocButton.TabIndex = 50;
            this.OrgDocButton.UseVisualStyleBackColor = true;
            this.OrgDocButton.Click += new System.EventHandler(this.OrgDocButtonClick);
            // 
            // OrgDocTextField
            // 
            this.OrgDocTextField.AllowDrop = true;
            this.OrgDocTextField.Location = new System.Drawing.Point(1, 85);
            this.OrgDocTextField.Name = "OrgDocTextField";
            this.OrgDocTextField.Size = new System.Drawing.Size(300, 20);
            this.OrgDocTextField.TabIndex = 48;
            this.OrgDocTextField.Leave += new System.EventHandler(this.OrgDocTextFieldLeave);
            // 
            // NumberSelectedSourceFilesLabel
            // 
            this.NumberSelectedSourceFilesLabel.AutoSize = true;
            this.NumberSelectedSourceFilesLabel.Location = new System.Drawing.Point(99, 2);
            this.NumberSelectedSourceFilesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NumberSelectedSourceFilesLabel.Name = "NumberSelectedSourceFilesLabel";
            this.NumberSelectedSourceFilesLabel.Size = new System.Drawing.Size(152, 13);
            this.NumberSelectedSourceFilesLabel.TabIndex = 45;
            this.NumberSelectedSourceFilesLabel.Text = "NumberOfSelectedSourceFiles";
            // 
            // DstDirectoryBrowseButton
            // 
            this.DstDirectoryBrowseButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.DstDirectoryBrowseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DstDirectoryBrowseButton.Location = new System.Drawing.Point(752, 21);
            this.DstDirectoryBrowseButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.DstDirectoryBrowseButton.Name = "DstDirectoryBrowseButton";
            this.DstDirectoryBrowseButton.Size = new System.Drawing.Size(70, 31);
            this.DstDirectoryBrowseButton.TabIndex = 44;
            this.DstDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.DstDirectoryBrowseButton.Click += new System.EventHandler(this.DstDirectoryBrowseButtonClick);
            // 
            // DstDirLabel
            // 
            this.DstDirLabel.AutoSize = true;
            this.DstDirLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DstDirLabel.Location = new System.Drawing.Point(442, 2);
            this.DstDirLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.DstDirLabel.Name = "DstDirLabel";
            this.DstDirLabel.Size = new System.Drawing.Size(127, 13);
            this.DstDirLabel.TabIndex = 43;
            this.DstDirLabel.Text = "DestinationPath Directory";
            // 
            // DstFileTextField
            // 
            this.DstFileTextField.AllowDrop = true;
            this.DstFileTextField.Location = new System.Drawing.Point(442, 25);
            this.DstFileTextField.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.DstFileTextField.Name = "DstFileTextField";
            this.DstFileTextField.Size = new System.Drawing.Size(300, 20);
            this.DstFileTextField.TabIndex = 42;
            this.DstFileTextField.Leave += new System.EventHandler(this.DstFileTextFieldLeave);
            // 
            // AnalysesList
            // 
            checkBoxProperties2.AutoSize = true;
            checkBoxProperties2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.AnalysesList.CheckBoxProperties = checkBoxProperties2;
            this.AnalysesList.DisplayMemberSingleItem = "";
            this.AnalysesList.FormattingEnabled = true;
            this.AnalysesList.Location = new System.Drawing.Point(0, 145);
            this.AnalysesList.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.AnalysesList.MaxDropDownItems = 20;
            this.AnalysesList.Name = "AnalysesList";
            this.AnalysesList.Size = new System.Drawing.Size(300, 21);
            this.AnalysesList.TabIndex = 33;
            // 
            // AddAnalysesButton
            // 
            this.AddAnalysesButton.Image = global::GUI.Properties.Resources.add;
            this.AddAnalysesButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddAnalysesButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddAnalysesButton.Location = new System.Drawing.Point(310, 141);
            this.AddAnalysesButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.AddAnalysesButton.Name = "AddAnalysesButton";
            this.AddAnalysesButton.Size = new System.Drawing.Size(70, 31);
            this.AddAnalysesButton.TabIndex = 34;
            this.AddAnalysesButton.Text = "Add";
            this.AddAnalysesButton.UseVisualStyleBackColor = true;
            this.AddAnalysesButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // SrcFileTextField
            // 
            this.SrcFileTextField.AllowDrop = true;
            this.SrcFileTextField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SrcFileTextField.Location = new System.Drawing.Point(1, 25);
            this.SrcFileTextField.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.SrcFileTextField.Name = "SrcFileTextField";
            this.SrcFileTextField.Size = new System.Drawing.Size(300, 20);
            this.SrcFileTextField.TabIndex = 28;
            this.SrcFileTextField.Leave += new System.EventHandler(this.SrcFileTextFieldLeave);
            // 
            // AnalyzeSrcFileLabel
            // 
            this.AnalyzeSrcFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalyzeSrcFileLabel.AutoSize = true;
            this.AnalyzeSrcFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AnalyzeSrcFileLabel.Location = new System.Drawing.Point(-2, 2);
            this.AnalyzeSrcFileLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.AnalyzeSrcFileLabel.Name = "AnalyzeSrcFileLabel";
            this.AnalyzeSrcFileLabel.Size = new System.Drawing.Size(71, 13);
            this.AnalyzeSrcFileLabel.TabIndex = 27;
            this.AnalyzeSrcFileLabel.Text = "Source File(s)";
            // 
            // AnalyzeSelectedAnalysesLabel
            // 
            this.AnalyzeSelectedAnalysesLabel.AutoSize = true;
            this.AnalyzeSelectedAnalysesLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AnalyzeSelectedAnalysesLabel.Location = new System.Drawing.Point(-2, 180);
            this.AnalyzeSelectedAnalysesLabel.Margin = new System.Windows.Forms.Padding(11, 10, 11, 0);
            this.AnalyzeSelectedAnalysesLabel.Name = "AnalyzeSelectedAnalysesLabel";
            this.AnalyzeSelectedAnalysesLabel.Size = new System.Drawing.Size(94, 13);
            this.AnalyzeSelectedAnalysesLabel.TabIndex = 36;
            this.AnalyzeSelectedAnalysesLabel.Text = "Selected Analyses";
            // 
            // SrcFileButton
            // 
            this.SrcFileButton.Image = global::GUI.Properties.Resources.folder_explore;
            this.SrcFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrcFileButton.Location = new System.Drawing.Point(310, 21);
            this.SrcFileButton.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.SrcFileButton.Name = "SrcFileButton";
            this.SrcFileButton.Size = new System.Drawing.Size(70, 31);
            this.SrcFileButton.TabIndex = 29;
            this.SrcFileButton.UseVisualStyleBackColor = true;
            this.SrcFileButton.Click += new System.EventHandler(this.SrcFileButtonClick);
            // 
            // ButtonPanel1
            // 
            this.ButtonPanel1.ColumnCount = 3;
            this.ButtonPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 442F));
            this.ButtonPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.ButtonPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.ButtonPanel1.Controls.Add(this.ProgressBar, 0, 1);
            this.ButtonPanel1.Controls.Add(this.ImportConfigurationButton, 1, 1);
            this.ButtonPanel1.Controls.Add(this.ExportConfigurationButton, 1, 0);
            this.ButtonPanel1.Controls.Add(this.ClearAnalysesButton, 2, 1);
            this.ButtonPanel1.Controls.Add(this.ProgressLabel, 0, 0);
            this.ButtonPanel1.Controls.Add(this.AnalyzeButton, 2, 0);
            this.ButtonPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel1.Location = new System.Drawing.Point(0, 380);
            this.ButtonPanel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 25);
            this.ButtonPanel1.Name = "ButtonPanel1";
            this.ButtonPanel1.Padding = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.ButtonPanel1.RowCount = 2;
            this.ButtonPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ButtonPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ButtonPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ButtonPanel1.Size = new System.Drawing.Size(888, 74);
            this.ButtonPanel1.TabIndex = 47;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(0, 43);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(0, 1, 0, 4);
            this.ProgressBar.MarqueeAnimationSpeed = 50;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(380, 28);
            this.ProgressBar.TabIndex = 38;
            // 
            // ImportConfigurationButton
            // 
            this.ImportConfigurationButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ImportConfigurationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportConfigurationButton.Location = new System.Drawing.Point(442, 43);
            this.ImportConfigurationButton.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.ImportConfigurationButton.Name = "ImportConfigurationButton";
            this.ImportConfigurationButton.Size = new System.Drawing.Size(160, 28);
            this.ImportConfigurationButton.TabIndex = 43;
            this.ImportConfigurationButton.Text = "Import Configuration";
            this.ImportConfigurationButton.UseVisualStyleBackColor = true;
            this.ImportConfigurationButton.Click += new System.EventHandler(this.ImportConfigurationButtonClick);
            // 
            // ExportConfigurationButton
            // 
            this.ExportConfigurationButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExportConfigurationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ExportConfigurationButton.Location = new System.Drawing.Point(442, 14);
            this.ExportConfigurationButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExportConfigurationButton.Name = "ExportConfigurationButton";
            this.ExportConfigurationButton.Size = new System.Drawing.Size(160, 28);
            this.ExportConfigurationButton.TabIndex = 42;
            this.ExportConfigurationButton.Text = "Export Configuration";
            this.ExportConfigurationButton.UseVisualStyleBackColor = true;
            this.ExportConfigurationButton.Click += new System.EventHandler(this.ExportConfigurationButtonClick);
            // 
            // ClearAnalysesButton
            // 
            this.ClearAnalysesButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ClearAnalysesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ClearAnalysesButton.Location = new System.Drawing.Point(728, 43);
            this.ClearAnalysesButton.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.ClearAnalysesButton.Name = "ClearAnalysesButton";
            this.ClearAnalysesButton.Size = new System.Drawing.Size(160, 28);
            this.ClearAnalysesButton.TabIndex = 41;
            this.ClearAnalysesButton.Text = "Clear";
            this.ClearAnalysesButton.UseVisualStyleBackColor = true;
            this.ClearAnalysesButton.Click += new System.EventHandler(this.ClearAnalysesButtonClick);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProgressLabel.Location = new System.Drawing.Point(11, 14);
            this.ProgressLabel.Margin = new System.Windows.Forms.Padding(11, 0, 0, 1);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(88, 13);
            this.ProgressLabel.TabIndex = 39;
            this.ProgressLabel.Text = "Nothing done yet";
            // 
            // AnalyzeButton
            // 
            this.AnalyzeButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AnalyzeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AnalyzeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AnalyzeButton.Location = new System.Drawing.Point(728, 14);
            this.AnalyzeButton.Margin = new System.Windows.Forms.Padding(0);
            this.AnalyzeButton.Name = "AnalyzeButton";
            this.AnalyzeButton.Size = new System.Drawing.Size(160, 28);
            this.AnalyzeButton.TabIndex = 37;
            this.AnalyzeButton.Text = "Analyze";
            this.AnalyzeButton.UseVisualStyleBackColor = true;
            this.AnalyzeButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AnalyzeButtonClick);
            // 
            // OpenTemplateDialog
            // 
            this.OpenTemplateDialog.Filter = "Template files|*.template";
            this.OpenTemplateDialog.RestoreDirectory = true;
            this.OpenTemplateDialog.Title = "Select a report template";
            // 
            // Analyze
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Analyze";
            this.Padding = new System.Windows.Forms.Padding(25, 25, 25, 26);
            this.Size = new System.Drawing.Size(938, 505);
            this.panel1.ResumeLayout(false);
            this.DisablePanel.ResumeLayout(false);
            this.DisablePanel.PerformLayout();
            this.ButtonPanel1.ResumeLayout(false);
            this.ButtonPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog SrcFileDialog;
        private System.Windows.Forms.OpenFileDialog ImportConfigurationDialog;
        private System.Windows.Forms.FolderBrowserDialog DstDirectoryDialog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog OrgDocFileDialog;
        private System.Windows.Forms.OpenFileDialog VisualizeAnalysisFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel SelectedAnalysesPanel;
        private System.Windows.Forms.Panel DisablePanel;
        private System.Windows.Forms.Button OrgDocButton;
        private System.Windows.Forms.Label OrgDocLabel;
        private System.Windows.Forms.TextBox OrgDocTextField;
        private System.Windows.Forms.Label NumberSelectedSourceFilesLabel;
        private System.Windows.Forms.Button DstDirectoryBrowseButton;
        private System.Windows.Forms.Label DstDirLabel;
        public System.Windows.Forms.TextBox DstFileTextField;
        private System.Windows.Forms.Button AddAnalysesButton;
        public System.Windows.Forms.TextBox SrcFileTextField;
        private System.Windows.Forms.Label AnalyzeSrcFileLabel;
        private System.Windows.Forms.Label AnalyzeSelectedAnalysesLabel;
        private System.Windows.Forms.Button SrcFileButton;
        private System.Windows.Forms.TableLayoutPanel ButtonPanel1;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button ImportConfigurationButton;
        private System.Windows.Forms.Button ExportConfigurationButton;
        private System.Windows.Forms.Button ClearAnalysesButton;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Button AnalyzeButton;
        private PresentationControls.CheckBoxComboBox AnalysesList;
        private System.Windows.Forms.Button AddReportButton;
        private System.Windows.Forms.ComboBox ReportsList;
        private System.Windows.Forms.RadioButton ReportRadioBttn;
        private System.Windows.Forms.RadioButton AnalysesRadioBttn;
        private PresentationControls.CheckBoxComboBox ReportOutputCList;
        private System.Windows.Forms.OpenFileDialog OpenTemplateDialog;
    }
}
