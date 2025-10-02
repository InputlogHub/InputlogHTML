using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InputLog.Core.Preprocessing;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util.Server;
using Timer = System.Timers.Timer;

namespace GUI.Tabs.Analyze.AnalysesControls
{
    /// <summary>
    ///     Base class to be used by all analyses that will be executed on server. See developer docs for more info.
    /// </summary>
    public class ServerAnalysisControl : AnalysisControl
    {
        private ServerAnalysisLauncher Launcher;
        private string OutputDir;
        private Timer PollTimer;

        /// <summary>
        ///     DO NOT USE THIS CONSTRUCTOR!
        ///     It is only provided because the designer does not work otherwise.
        /// </summary>
        protected ServerAnalysisControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "ServerAnalysisControl";

            Launcher = new ServerAnalysisLauncher();
            PollTimer = new Timer(5000) {SynchronizingObject = Parent};
            PollTimer.Elapsed += PollTimerTick;
            PollTimer.AutoReset = true;
            //PollTimer = new Timer(new Container()) { Interval = 5000 };

            ResumeLayout(false);
        }

        public override void Analyze(AnalyzeModel model)
        {
            // Things that normally get done in model.Analyze()
            ResetUI();
            ProduceAnalyzer();

            OutputDir = model.DestinationPath;

            // TODO remove preprocessor list.
            string task;
            if (!Launcher.LaunchAnalysis(model, Analyzer, new List<Preprocessor>(), this, out task))
            {
                return;
            }
            PollTimer.Start();

            RunParamTS(delegate(string value)
                       {
                           GetIDLabel().Text = "ID: " + value;
                           GetIDLabel().Visible = true;
                           GetStatusLabel().Text = "Starting...";
                           GetStatusLabel().Visible = true;
                       }, task);

            Analyzer.ReportProgress(this, new ProgressEventArgs(
                "Analysis sent to server. Waiting for response..."));

            RunTS(delegate
                  {
                      GetProgressBar().PerformStep();
                      ChangeUItoFinished();
                  });
        }


        private void PollTimerTick(object sender, EventArgs e)
        {
            string status;
            List<ServerAnalysisException> exceptions;

            if (Launcher.IsAnalysisCompleted(Analyzer, OutputDir, out status, out exceptions))
            {
                PollTimer.Stop();
                RunTS(delegate
                      {
                          GetProgressBar().PerformStep();
                          AddOpenContainingFolderLink();
                      });

                if (exceptions != null)
                {
                    foreach (var ex in exceptions)
                    {
                        RunTS(delegate
                              {
                                  MessageBox.Show("Error processing file: \"" + ex.Filename + "\": " + ex,
                                      "Analysis error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                              });
                    }
                }
            }
            else
            {
                // Increase interval time, but don't make the interval larger than 1 minute.
                if (PollTimer.Interval < 30000)
                {
                    PollTimer.Interval *= 2;
                    PollTimer.Interval = Math.Min(30000, PollTimer.Interval);
                }
            }

            RunParamTS(delegate(string value) { GetStatusLabel().Text = value; }, status);
        }

        public override void Close()
        {
            base.Close();
            PollTimer.Stop();
        }

        /// <summary>
        ///     Adapts the UI so to represent that the analysis has been finished.
        ///     Does nothing since the actual analysis is still pending on the server.
        /// </summary>
        protected override void ChangeUItoFinished()
        {
        }

        public virtual ProgressBar GetProgressBar()
        {
            return null;
        }

        protected virtual Label GetStatusLabel()
        {
            return null;
        }

        protected virtual Label GetIDLabel()
        {
            return null;
        }
    }
}