using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Preprocessing.Recode;

namespace GUI.Tabs.Preprocess.Recoders
{
    internal class RemoveTaskbar : ProcessControl
    {
        /// <summary>
        ///     Name of the post processer.
        /// </summary>
        public const string NAME = "Taskbar Remover";

        /// <summary>
        /// </summary>
        private readonly List<string> IDFXList;

        /// <summary>
        /// </summary>
        private readonly TaskbarRemover TaskbarIdfx;

        private IContainer components;
        private LinkLabel MoreInfoLabel;
        private Label NameLabel;
        private ToolTip TaskBarRemoveTip;

        /// <summary>
        /// </summary>
        public RemoveTaskbar() : base("Taskbar Remover", "taskbarIdfx")
        {
            InitializeComponent();
            TaskbarIdfx = new TaskbarRemover();
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
        }

        public override Preprocessor GetPreprocessor(SessionIdentification sessionID)
        {
            return TaskbarIdfx;
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
            TaskBarRemoveTip = new ToolTip(components);
            SuspendLayout();
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NameLabel.Location = new Point(2, 2);
            NameLabel.Margin = new Padding(2, 0, 2, 0);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(152, 20);
            NameLabel.TabIndex = 6;
            NameLabel.Text = "TASKBAR Remover";
            // 
            // MoreInfoLabel
            // 
            MoreInfoLabel.AutoSize = true;
            MoreInfoLabel.Enabled = false;
            MoreInfoLabel.Font = new Font("Microsoft Sans Serif", 8.25F);
            MoreInfoLabel.Location = new Point(169, 7);
            MoreInfoLabel.Margin = new Padding(2, 0, 2, 0);
            MoreInfoLabel.Name = "MoreInfoLabel";
            MoreInfoLabel.Size = new Size(52, 13);
            MoreInfoLabel.TabIndex = 7;
            MoreInfoLabel.TabStop = true;
            MoreInfoLabel.Text = "More Info";
            // 
            // TaskbarRemoveTip
            // 
            TaskBarRemoveTip.AutoPopDelay = 10000;
            TaskBarRemoveTip.InitialDelay = 100;
            TaskBarRemoveTip.IsBalloon = true;
            TaskBarRemoveTip.ReshowDelay = 100;
            TaskBarRemoveTip.ToolTipIcon = ToolTipIcon.Info;
            TaskBarRemoveTip.ToolTipTitle = "Taskbar Remover";
            TaskBarRemoveTip.SetToolTip(this, "TASKBAR Remover changes the content of a copy of the idfx-file." +
                                              "\nThe 'TASKBAR' and similar empty sources are removed as focus changes.\nTheir pause time is added to the nearest real source.");
            // 
            // RemoveTaskbar
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            Controls.Add(MoreInfoLabel);
            Controls.Add(NameLabel);
            Name = "RemoveTaskbar";
            Size = new Size(221, 44);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}