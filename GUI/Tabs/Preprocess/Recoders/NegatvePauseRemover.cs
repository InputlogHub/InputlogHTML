using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Recode;

namespace GUI.Tabs.Preprocess.Recoders
{
    internal class NegtivePauseRemover : ProcessControl
    {
        /// <summary>
        ///     Name of the post processer.
        /// </summary>
        public const string NAME = "Negative Pause Remover";

        /// <summary>
        /// </summary>
        private readonly NegativePauseRemover _removeNegativePauses;

        /// <summary>
        /// </summary>
        private readonly List<string> IDFXList;

        private IContainer components;
        private LinkLabel MoreInfoLabel;
        private Label NameLabel;
        private ToolTip NegativePauseTip;
        private Label WarningLbl;

        /// <summary>
        /// </summary>
        public NegtivePauseRemover() : base("Negative Pause Remover", "removeNegativePauses")
        {
            InitializeComponent();
            _removeNegativePauses = new NegativePauseRemover();
            IDFXList = new List<string>();
        }

        /// <summary>
        ///     Returns whether this process control can handle multiple files at the same
        ///     time or can only process one time at a time.
        /// </summary>
        public override bool MultipleFileCompatible => true;

        private void UpdateEventLists()
        {
            // The sources are the filePaths.
            var sources = FilePaths;

            // The initial case when there are no sources selected yet.
            if ((sources == null) || (IDFXList == null)) return;

            // The next line of code is commented out to prevent the recoding of the same file twice: 
            // once here when clicking the 'Add' button in the EventProcessor panel and again 
            // when clicking the 'Process' buttton in the PreProcess panel.
            // RecodeIdfx.Recode(sources);
        }

        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            return _removeNegativePauses;
        }

        public override void OnFileSelectionChange(List<string> filePaths)
        {
            base.OnFileSelectionChange(filePaths);
            UpdateEventLists();
        }

        private void InitializeComponent()
        {
            components = new Container();
            NameLabel = new Label();
            MoreInfoLabel = new LinkLabel();
            WarningLbl = new Label();
            NegativePauseTip = new ToolTip(components);
            SuspendLayout();
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular,
                GraphicsUnit.Point, 0);
            NameLabel.Location = new Point(15, 2);
            NameLabel.Margin = new Padding(2, 0, 2, 0);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(188, 20);
            NameLabel.TabIndex = 6;
            NameLabel.Text = "Negative Pause Remover";
            // 
            // MoreInfoLabel
            // 
            MoreInfoLabel.AutoSize = true;
            MoreInfoLabel.Enabled = false;
            MoreInfoLabel.Font = new Font("Microsoft Sans Serif", 8.25F);
            MoreInfoLabel.Location = new Point(224, 7);
            MoreInfoLabel.Margin = new Padding(2, 0, 2, 0);
            MoreInfoLabel.Name = "MoreInfoLabel";
            MoreInfoLabel.Size = new Size(52, 13);
            MoreInfoLabel.TabIndex = 7;
            MoreInfoLabel.TabStop = true;
            MoreInfoLabel.Text = "More Info";
            // 
            // WarningLbl
            // 
            WarningLbl.AutoSize = true;
            WarningLbl.Location = new Point(16, 31);
            WarningLbl.Margin = new Padding(2, 0, 2, 0);
            WarningLbl.Name = "WarningLbl";
            WarningLbl.Size = new Size(235, 13);
            WarningLbl.TabIndex = 8;
            WarningLbl.Text = "Changes the start and end time of faulty mouse events";
            // 
            // NegativePauseTip
            // 
            NegativePauseTip.AutoPopDelay = 10000;
            NegativePauseTip.InitialDelay = 100;
            NegativePauseTip.IsBalloon = true;
            NegativePauseTip.ReshowDelay = 100;
            NegativePauseTip.ToolTipIcon = ToolTipIcon.Info;
            NegativePauseTip.ToolTipTitle = "Negative Pause Remover";
            // 
            // NegtivePauseRemover
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            Controls.Add(WarningLbl);
            Controls.Add(MoreInfoLabel);
            Controls.Add(NameLabel);
            Name = "NegtivePauseRemover";
            Size = new Size(278, 56);
            NegativePauseTip.SetToolTip(this,
                "The Negative Pause Remover changes the start and end time of faulty mouse events\n" +
                "in a copy of the idfx-file. Use this feature on a case-by-case basis\n" +
                "after consultation with the Inputlog staff.");
            ResumeLayout(false);
            PerformLayout();
        }
    }
}