using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Recode;

namespace GUI.Tabs.Preprocess.Recoders
{
    internal class IdfxRecoder : ProcessControl
    {
        /// <summary>
        ///     Name of the post processer.
        /// </summary>
        public const string NAME = "Idfx Recoder";

        private IContainer components;
        private ToolTip IdfxRecoderTip;
        private LinkLabel MoreInfoLabel;
        private Label NameLabel;
        private Label WarningLbl;

        /// <summary>
        /// </summary>
        private readonly List<string> IDFXList;

        /// <summary>
        /// </summary>
        private readonly RecoderIdfx RecodeIdfx;

        /// <summary>
        /// </summary>
        public IdfxRecoder() : base("Idfx Recoder", "recodeIdfx")
        {
            InitializeComponent();
            RecodeIdfx = new RecoderIdfx();
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
            if (sources == null || IDFXList == null) return;

            // The next line of code is commented out to prevent the recoding of the same file twice: 
            // once here when clicking the 'Add' button in the EventProcessor panel and again 
            // when clicking the 'Process' buttton in the PreProcess panel.
           // RecodeIdfx.Recode(sources);
        }

        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            return RecodeIdfx;
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
            IdfxRecoderTip = new ToolTip(components);
            SuspendLayout();
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NameLabel.Location = new Point(3, 3);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(121, 25);
            NameLabel.TabIndex = 6;
            NameLabel.Text = "Idfx Recoder";
            // 
            // MoreInfoLabel
            // 
            MoreInfoLabel.AutoSize = true;
            MoreInfoLabel.Enabled = false;
            MoreInfoLabel.Font = new Font("Microsoft Sans Serif", 8.25F);
            MoreInfoLabel.Location = new Point(225, 9);
            MoreInfoLabel.Name = "MoreInfoLabel";
            MoreInfoLabel.Size = new Size(67, 17);
            MoreInfoLabel.TabIndex = 7;
            MoreInfoLabel.TabStop = true;
            MoreInfoLabel.Text = "More Info";
            // 
            // WarningLbl
            // 
            WarningLbl.AutoSize = true;
            WarningLbl.Location = new Point(8, 32);
            WarningLbl.Name = "WarningLbl";
            WarningLbl.Size = new Size(128, 17);
            WarningLbl.TabIndex = 8;
            WarningLbl.Text = "(experimental)";
            // 
            // IdfxRecoderTip
            // 
            IdfxRecoderTip.AutoPopDelay = 10000;
            IdfxRecoderTip.InitialDelay = 100;
            IdfxRecoderTip.IsBalloon = true;
            IdfxRecoderTip.ReshowDelay = 100;
            IdfxRecoderTip.ToolTipIcon = ToolTipIcon.Info;
            IdfxRecoderTip.ToolTipTitle = "Idfx Recoder";
            IdfxRecoderTip.SetToolTip(this,"The idfx-recoder changes the content of a copy of the idfx-file." +
                                           "\nUse this feature on a case-by-case basis\nafter consultation with the Inputlog staff.");
            // 
            // IdfxRecoder
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            Controls.Add(WarningLbl);
            Controls.Add(MoreInfoLabel);
            Controls.Add(NameLabel);
            Name = "IdfxRecoder";
            Size = new Size(295, 58);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}