namespace GUI.Tabs.Preprocess
{
    sealed partial class Preprocess
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.TooltipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.TooltipWarningFilter = new System.Windows.Forms.ToolTip(this.components);
            this.TooltipWarningRecode = new System.Windows.Forms.ToolTip(this.components);
            this.SelectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FileListTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.DirectoryHandler = new System.DirectoryServices.DirectoryEntry();
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PreProcessSrcFileLabel = new System.Windows.Forms.Label();
            this.SrcFileTextField = new System.Windows.Forms.TextBox();
            this.SrcFileSelect = new System.Windows.Forms.Button();
            this.NrSrcFiles = new System.Windows.Forms.Label();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.PreprocessLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.FilterRadio = new System.Windows.Forms.RadioButton();
            this.FilterPanel = new GUI.Tabs.Preprocess.EventProcessor();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.FileLevelLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.FileLevelRadio = new System.Windows.Forms.RadioButton();
            this.FileLevelButtonLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.FileLevelRadioConvert = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.FileLevelRadioMerge = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.InputFilesDD = new System.Windows.Forms.ComboBox();
            this.FilterLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.RecodePanel = new GUI.Tabs.Preprocess.EventProcessor();
            this.RecodeRadio = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.MergeRadio = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mergeIncludePause = new System.Windows.Forms.CheckBox();
            this.SegmentationLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SegmentationRadio = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.keyDelimiterLabel = new System.Windows.Forms.Label();
            this.segmentInitialPauseCBX = new System.Windows.Forms.CheckBox();
            this.segmentKeyDelimiters = new System.Windows.Forms.ComboBox();
            this.MainLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.PreprocessLayoutPanel.SuspendLayout();
            this.FileLevelLayoutPanel.SuspendLayout();
            this.FileLevelButtonLayoutPanel.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.FilterLayoutPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SegmentationLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TooltipInfo
            // 
            this.TooltipInfo.IsBalloon = true;
            this.TooltipInfo.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TooltipInfo.ToolTipTitle = "Description:";
            // 
            // TooltipWarningFilter
            // 
            this.TooltipWarningFilter.AutomaticDelay = 0;
            this.TooltipWarningFilter.IsBalloon = true;
            this.TooltipWarningFilter.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.TooltipWarningFilter.ToolTipTitle = "Warning:";
            // 
            // TooltipWarningRecode
            // 
            this.TooltipWarningRecode.AutomaticDelay = 0;
            this.TooltipWarningRecode.IsBalloon = true;
            this.TooltipWarningRecode.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.TooltipWarningRecode.ToolTipTitle = "Warning:";
            // 
            // SelectFileDialog
            // 
            this.SelectFileDialog.Filter = "IDFX Files (*.idfx)|*.idfx|All Files|*.*";
            this.SelectFileDialog.Multiselect = true;
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.MainLayoutPanel.AutoSize = true;
            this.MainLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.MainLayoutPanel.ColumnCount = 2;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.MainLayoutPanel.Controls.Add(this.ProcessButton, 1, 3);
            this.MainLayoutPanel.Controls.Add(this.PreprocessLayoutPanel, 0, 1);
            this.MainLayoutPanel.Controls.Add(this.ProgressBar, 0, 3);
            this.MainLayoutPanel.Controls.Add(this.FileLevelLayoutPanel, 1, 2);
            this.MainLayoutPanel.Controls.Add(this.FilterLayoutPanel, 1, 1);
            this.MainLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.MainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MainLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MainLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.MainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.RowCount = 4;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(936, 532);
            this.MainLayoutPanel.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.PreProcessSrcFileLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SrcFileTextField, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SrcFileSelect, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.NrSrcFiles, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(468, 48);
            this.tableLayoutPanel1.TabIndex = 72;
            // 
            // PreProcessSrcFileLabel
            // 
            this.PreProcessSrcFileLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PreProcessSrcFileLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PreProcessSrcFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PreProcessSrcFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PreProcessSrcFileLabel.Location = new System.Drawing.Point(1, 0);
            this.PreProcessSrcFileLabel.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.PreProcessSrcFileLabel.Name = "PreProcessSrcFileLabel";
            this.PreProcessSrcFileLabel.Size = new System.Drawing.Size(267, 20);
            this.PreProcessSrcFileLabel.TabIndex = 75;
            this.PreProcessSrcFileLabel.Text = "Source File(s)";
            // 
            // SrcFileTextField
            // 
            this.SrcFileTextField.AllowDrop = true;
            this.SrcFileTextField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SrcFileTextField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SrcFileTextField.Location = new System.Drawing.Point(1, 23);
            this.SrcFileTextField.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
            this.SrcFileTextField.Name = "SrcFileTextField";
            this.SrcFileTextField.Size = new System.Drawing.Size(267, 20);
            this.SrcFileTextField.TabIndex = 74;
            // 
            // SrcFileSelect
            // 
            this.SrcFileSelect.Dock = System.Windows.Forms.DockStyle.Right;
            this.SrcFileSelect.Image = global::GUI.Properties.Resources.folder_explore;
            this.SrcFileSelect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrcFileSelect.Location = new System.Drawing.Point(363, 20);
            this.SrcFileSelect.Margin = new System.Windows.Forms.Padding(0);
            this.SrcFileSelect.Name = "SrcFileSelect";
            this.SrcFileSelect.Size = new System.Drawing.Size(83, 28);
            this.SrcFileSelect.TabIndex = 72;
            this.SrcFileSelect.UseVisualStyleBackColor = true;
            this.SrcFileSelect.Click += new System.EventHandler(this.FileSelectButtonFilterClick);
            // 
            // NrSrcFiles
            // 
            this.NrSrcFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.NrSrcFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.NrSrcFiles.Location = new System.Drawing.Point(271, 22);
            this.NrSrcFiles.Margin = new System.Windows.Forms.Padding(3, 0, 16, 0);
            this.NrSrcFiles.Name = "NrSrcFiles";
            this.NrSrcFiles.Size = new System.Drawing.Size(70, 23);
            this.NrSrcFiles.TabIndex = 73;
            this.NrSrcFiles.Text = "x File(s)";
            this.NrSrcFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProcessButton
            // 
            this.ProcessButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.ProcessButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProcessButton.Location = new System.Drawing.Point(775, 501);
            this.ProcessButton.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(160, 31);
            this.ProcessButton.TabIndex = 67;
            this.ProcessButton.Text = "Process";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButtonClick);
            // 
            // PreprocessLayoutPanel
            // 
            this.PreprocessLayoutPanel.ColumnCount = 3;
            this.PreprocessLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.PreprocessLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.PreprocessLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.PreprocessLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PreprocessLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PreprocessLayoutPanel.Controls.Add(this.FilterRadio, 0, 0);
            this.PreprocessLayoutPanel.Controls.Add(this.FilterPanel, 0, 1);
            this.PreprocessLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreprocessLayoutPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PreprocessLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PreprocessLayoutPanel.Location = new System.Drawing.Point(0, 48);
            this.PreprocessLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.PreprocessLayoutPanel.Name = "PreprocessLayoutPanel";
            this.PreprocessLayoutPanel.RowCount = 2;
            this.PreprocessLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.PreprocessLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PreprocessLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PreprocessLayoutPanel.Size = new System.Drawing.Size(468, 301);
            this.PreprocessLayoutPanel.TabIndex = 3;
            // 
            // FilterRadio
            // 
            this.FilterRadio.AutoSize = true;
            this.FilterRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.FilterRadio.Location = new System.Drawing.Point(0, 3);
            this.FilterRadio.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.FilterRadio.Name = "FilterRadio";
            this.FilterRadio.Size = new System.Drawing.Size(160, 27);
            this.FilterRadio.TabIndex = 60;
            this.FilterRadio.TabStop = true;
            this.FilterRadio.Text = "Filter";
            this.FilterRadio.UseVisualStyleBackColor = true;
            this.FilterRadio.CheckedChanged += new System.EventHandler(this.FilterRadioCheckedChanged);
            // 
            // FilterPanel
            // 
            this.FilterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PreprocessLayoutPanel.SetColumnSpan(this.FilterPanel, 3);
            this.FilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterPanel.Enabled = false;
            this.FilterPanel.Location = new System.Drawing.Point(0, 36);
            this.FilterPanel.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
            this.FilterPanel.Name = "FilterPanel";
            this.FilterPanel.Size = new System.Drawing.Size(464, 262);
            this.FilterPanel.TabIndex = 63;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.ProgressBar.Location = new System.Drawing.Point(0, 504);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(436, 28);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 65;
            // 
            // FileLevelLayoutPanel
            // 
            this.FileLevelLayoutPanel.ColumnCount = 1;
            this.FileLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.FileLevelLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.FileLevelLayoutPanel.Controls.Add(this.FileLevelRadio, 0, 0);
            this.FileLevelLayoutPanel.Controls.Add(this.FileLevelButtonLayoutPanel, 0, 1);
            this.FileLevelLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileLevelLayoutPanel.Location = new System.Drawing.Point(468, 349);
            this.FileLevelLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FileLevelLayoutPanel.Name = "FileLevelLayoutPanel";
            this.FileLevelLayoutPanel.RowCount = 2;
            this.FileLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.FileLevelLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FileLevelLayoutPanel.Size = new System.Drawing.Size(468, 152);
            this.FileLevelLayoutPanel.TabIndex = 68;
            // 
            // FileLevelRadio
            // 
            this.FileLevelRadio.AutoSize = true;
            this.FileLevelRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileLevelRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileLevelRadio.Location = new System.Drawing.Point(8, 3);
            this.FileLevelRadio.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
            this.FileLevelRadio.Name = "FileLevelRadio";
            this.FileLevelRadio.Size = new System.Drawing.Size(469, 27);
            this.FileLevelRadio.TabIndex = 9;
            this.FileLevelRadio.TabStop = true;
            this.FileLevelRadio.Text = "File-Level Conversion";
            this.FileLevelRadio.UseVisualStyleBackColor = true;
            this.FileLevelRadio.CheckedChanged += new System.EventHandler(this.FileLevelRadioCheckedChanged);
            // 
            // FileLevelButtonLayoutPanel
            // 
            this.FileLevelButtonLayoutPanel.ColumnCount = 1;
            this.FileLevelButtonLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.FileLevelButtonLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.FileLevelButtonLayoutPanel.Controls.Add(this.FileLevelRadioConvert, 0, 0);
            this.FileLevelButtonLayoutPanel.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.FileLevelButtonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileLevelButtonLayoutPanel.Location = new System.Drawing.Point(0, 33);
            this.FileLevelButtonLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FileLevelButtonLayoutPanel.Name = "FileLevelButtonLayoutPanel";
            this.FileLevelButtonLayoutPanel.RowCount = 2;
            this.FileLevelButtonLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.19672F));
            this.FileLevelButtonLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.80328F));
            this.FileLevelButtonLayoutPanel.Size = new System.Drawing.Size(480, 119);
            this.FileLevelButtonLayoutPanel.TabIndex = 10;
            // 
            // FileLevelRadioConvert
            // 
            this.FileLevelRadioConvert.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FileLevelRadioConvert.Appearance = System.Windows.Forms.Appearance.Button;
            this.FileLevelRadioConvert.Enabled = false;
            this.FileLevelRadioConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FileLevelRadioConvert.Location = new System.Drawing.Point(8, 20);
            this.FileLevelRadioConvert.Margin = new System.Windows.Forms.Padding(8, 5, 0, 3);
            this.FileLevelRadioConvert.Name = "FileLevelRadioConvert";
            this.FileLevelRadioConvert.Size = new System.Drawing.Size(282, 30);
            this.FileLevelRadioConvert.TabIndex = 8;
            this.FileLevelRadioConvert.Text = "Convert Translog and older idfx versions";
            this.FileLevelRadioConvert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.FileLevelRadioConvert.UseVisualStyleBackColor = true;
            this.FileLevelRadioConvert.Click += new System.EventHandler(this.RadioVersionConvertCheckedChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.FileLevelRadioMerge, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.InputFilesDD, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 69);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(480, 50);
            this.tableLayoutPanel4.TabIndex = 9;
            // 
            // FileLevelRadioMerge
            // 
            this.FileLevelRadioMerge.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FileLevelRadioMerge.Appearance = System.Windows.Forms.Appearance.Button;
            this.FileLevelRadioMerge.Enabled = false;
            this.FileLevelRadioMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FileLevelRadioMerge.Location = new System.Drawing.Point(8, 10);
            this.FileLevelRadioMerge.Margin = new System.Windows.Forms.Padding(8, 3, 2, 3);
            this.FileLevelRadioMerge.Name = "FileLevelRadioMerge";
            this.FileLevelRadioMerge.Size = new System.Drawing.Size(144, 30);
            this.FileLevelRadioMerge.TabIndex = 10;
            this.FileLevelRadioMerge.Text = "Merge Data";
            this.FileLevelRadioMerge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.FileLevelRadioMerge.UseVisualStyleBackColor = true;
            this.FileLevelRadioMerge.Click += new System.EventHandler(this.FileLevelRadioMergeClick);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Merge *.idfx files with:";
            // 
            // InputFilesDD
            // 
            this.InputFilesDD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InputFilesDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InputFilesDD.Enabled = false;
            this.InputFilesDD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.InputFilesDD.FormattingEnabled = true;
            this.InputFilesDD.Items.AddRange(new object[] {
            "Dragon Naturally Speaking (*.dat)",
            "Tobii/Eyelink files (*.tsv)"});
            this.InputFilesDD.Location = new System.Drawing.Point(274, 12);
            this.InputFilesDD.Margin = new System.Windows.Forms.Padding(5, 12, 0, 3);
            this.InputFilesDD.Name = "InputFilesDD";
            this.InputFilesDD.Size = new System.Drawing.Size(206, 21);
            this.InputFilesDD.TabIndex = 11;
            // 
            // FilterLayoutPanel
            // 
            this.FilterLayoutPanel.ColumnCount = 3;
            this.FilterLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.FilterLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.FilterLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.FilterLayoutPanel.Controls.Add(this.RecodePanel, 0, 1);
            this.FilterLayoutPanel.Controls.Add(this.RecodeRadio, 0, 0);
            this.FilterLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterLayoutPanel.Location = new System.Drawing.Point(468, 48);
            this.FilterLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FilterLayoutPanel.Name = "FilterLayoutPanel";
            this.FilterLayoutPanel.RowCount = 2;
            this.FilterLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.FilterLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FilterLayoutPanel.Size = new System.Drawing.Size(468, 301);
            this.FilterLayoutPanel.TabIndex = 69;
            // 
            // RecodePanel
            // 
            this.RecodePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FilterLayoutPanel.SetColumnSpan(this.RecodePanel, 3);
            this.RecodePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecodePanel.Enabled = false;
            this.RecodePanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RecodePanel.Location = new System.Drawing.Point(8, 36);
            this.RecodePanel.Margin = new System.Windows.Forms.Padding(8, 3, 1, 3);
            this.RecodePanel.Name = "RecodePanel";
            this.RecodePanel.Size = new System.Drawing.Size(459, 262);
            this.RecodePanel.TabIndex = 65;
            // 
            // RecodeRadio
            // 
            this.RecodeRadio.AutoSize = true;
            this.RecodeRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecodeRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.RecodeRadio.Location = new System.Drawing.Point(8, 3);
            this.RecodeRadio.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
            this.RecodeRadio.Name = "RecodeRadio";
            this.RecodeRadio.Size = new System.Drawing.Size(152, 27);
            this.RecodeRadio.TabIndex = 62;
            this.RecodeRadio.TabStop = true;
            this.RecodeRadio.Text = "Recode";
            this.RecodeRadio.UseVisualStyleBackColor = true;
            this.RecodeRadio.CheckedChanged += new System.EventHandler(this.RecodeRadioCheckedChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.SegmentationLayoutPanel, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 349);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(465, 152);
            this.tableLayoutPanel2.TabIndex = 73;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.MergeRadio, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(232, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(231, 152);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // MergeRadio
            // 
            this.MergeRadio.AutoSize = true;
            this.MergeRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MergeRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MergeRadio.Location = new System.Drawing.Point(0, 3);
            this.MergeRadio.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.MergeRadio.Name = "MergeRadio";
            this.MergeRadio.Size = new System.Drawing.Size(229, 27);
            this.MergeRadio.TabIndex = 8;
            this.MergeRadio.TabStop = true;
            this.MergeRadio.Text = "IDFX Merging";
            this.MergeRadio.UseVisualStyleBackColor = true;
            this.MergeRadio.CheckedChanged += new System.EventHandler(this.MergeRadioCheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.mergeIncludePause);
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(232, 115);
            this.panel2.TabIndex = 25;
            // 
            // mergeIncludePause
            // 
            this.mergeIncludePause.AutoSize = true;
            this.mergeIncludePause.Checked = true;
            this.mergeIncludePause.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mergeIncludePause.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.mergeIncludePause.Location = new System.Drawing.Point(0, 4);
            this.mergeIncludePause.Name = "mergeIncludePause";
            this.mergeIncludePause.Size = new System.Drawing.Size(119, 17);
            this.mergeIncludePause.TabIndex = 25;
            this.mergeIncludePause.Text = "Include initial pause";
            this.mergeIncludePause.UseVisualStyleBackColor = true;
            // 
            // SegmentationLayoutPanel
            // 
            this.SegmentationLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SegmentationLayoutPanel.ColumnCount = 1;
            this.SegmentationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SegmentationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SegmentationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SegmentationLayoutPanel.Controls.Add(this.SegmentationRadio, 0, 0);
            this.SegmentationLayoutPanel.Controls.Add(this.panel1, 0, 1);
            this.SegmentationLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SegmentationLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SegmentationLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SegmentationLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SegmentationLayoutPanel.Name = "SegmentationLayoutPanel";
            this.SegmentationLayoutPanel.RowCount = 2;
            this.SegmentationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.SegmentationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SegmentationLayoutPanel.Size = new System.Drawing.Size(232, 152);
            this.SegmentationLayoutPanel.TabIndex = 5;
            // 
            // SegmentationRadio
            // 
            this.SegmentationRadio.AutoSize = true;
            this.SegmentationRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SegmentationRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SegmentationRadio.Location = new System.Drawing.Point(0, 3);
            this.SegmentationRadio.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.SegmentationRadio.Name = "SegmentationRadio";
            this.SegmentationRadio.Size = new System.Drawing.Size(229, 27);
            this.SegmentationRadio.TabIndex = 8;
            this.SegmentationRadio.TabStop = true;
            this.SegmentationRadio.Text = "IDFX Segmentation";
            this.SegmentationRadio.UseVisualStyleBackColor = true;
            this.SegmentationRadio.CheckedChanged += new System.EventHandler(this.SegmentRadioCheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.keyDelimiterLabel);
            this.panel1.Controls.Add(this.segmentInitialPauseCBX);
            this.panel1.Controls.Add(this.segmentKeyDelimiters);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(0, 35);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 115);
            this.panel1.TabIndex = 25;
            // 
            // keyDelimiterLabel
            // 
            this.keyDelimiterLabel.AutoSize = true;
            this.keyDelimiterLabel.Location = new System.Drawing.Point(-4, 27);
            this.keyDelimiterLabel.Name = "keyDelimiterLabel";
            this.keyDelimiterLabel.Size = new System.Drawing.Size(71, 13);
            this.keyDelimiterLabel.TabIndex = 26;
            this.keyDelimiterLabel.Text = "Key Delimiter:";
            // 
            // segmentInitialPauseCBX
            // 
            this.segmentInitialPauseCBX.AutoSize = true;
            this.segmentInitialPauseCBX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.segmentInitialPauseCBX.Location = new System.Drawing.Point(0, 4);
            this.segmentInitialPauseCBX.Name = "segmentInitialPauseCBX";
            this.segmentInitialPauseCBX.Size = new System.Drawing.Size(119, 17);
            this.segmentInitialPauseCBX.TabIndex = 25;
            this.segmentInitialPauseCBX.Text = "Include initial pause";
            this.segmentInitialPauseCBX.UseVisualStyleBackColor = true;
            // 
            // segmentKeyDelimiters
            // 
            this.segmentKeyDelimiters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.segmentKeyDelimiters.FormattingEnabled = true;
            this.segmentKeyDelimiters.Location = new System.Drawing.Point(0, 50);
            this.segmentKeyDelimiters.Name = "segmentKeyDelimiters";
            this.segmentKeyDelimiters.Size = new System.Drawing.Size(134, 21);
            this.segmentKeyDelimiters.TabIndex = 24;
            // 
            // Preprocess
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.Controls.Add(this.MainLayoutPanel);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.Name = "Preprocess";
            this.Size = new System.Drawing.Size(936, 532);
            this.MainLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.PreprocessLayoutPanel.ResumeLayout(false);
            this.PreprocessLayoutPanel.PerformLayout();
            this.FileLevelLayoutPanel.ResumeLayout(false);
            this.FileLevelLayoutPanel.PerformLayout();
            this.FileLevelButtonLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.FilterLayoutPanel.ResumeLayout(false);
            this.FilterLayoutPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.SegmentationLayoutPanel.ResumeLayout(false);
            this.SegmentationLayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip TooltipInfo;
		private System.Windows.Forms.ToolTip TooltipWarningFilter;
		private System.Windows.Forms.ToolTip TooltipWarningRecode;
		private System.Windows.Forms.OpenFileDialog SelectFileDialog;
		private System.Windows.Forms.ToolTip FileListTooltip;
		private System.DirectoryServices.DirectoryEntry DirectoryHandler;
		private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel PreprocessLayoutPanel;
		private System.Windows.Forms.RadioButton FilterRadio;
        private EventProcessor FilterPanel;
        private System.Windows.Forms.TableLayoutPanel SegmentationLayoutPanel;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.RadioButton SegmentationRadio;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox segmentInitialPauseCBX;
        private System.Windows.Forms.ComboBox segmentKeyDelimiters;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.TableLayoutPanel FileLevelLayoutPanel;
        private System.Windows.Forms.RadioButton FileLevelRadio;
        private System.Windows.Forms.TableLayoutPanel FileLevelButtonLayoutPanel;
        private System.Windows.Forms.CheckBox FileLevelRadioConvert;
        private System.Windows.Forms.TableLayoutPanel FilterLayoutPanel;
        private EventProcessor RecodePanel;
        private System.Windows.Forms.RadioButton RecodeRadio;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button SrcFileSelect;
        private System.Windows.Forms.Label NrSrcFiles;
        private System.Windows.Forms.TextBox SrcFileTextField;
        private System.Windows.Forms.Label PreProcessSrcFileLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton MergeRadio;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox mergeIncludePause;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.CheckBox FileLevelRadioMerge;
        private System.Windows.Forms.ComboBox InputFilesDD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label keyDelimiterLabel;

	}
}
