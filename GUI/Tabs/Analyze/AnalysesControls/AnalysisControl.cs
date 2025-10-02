using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Analyze.ImportExport;
using InputLog.Core.Preprocessing;
using InputLog.Core.Util;
using InputLog.Core.Util.Progress;

namespace GUI.Tabs.Analyze.AnalysesControls
{
    /// <summary>
    ///     Common base class for analysis controls to implement.
    ///     An analysisControl is a visual representation of an analysis in the GUI.
    ///     There should always be 2 representations: a collapsed and expanded representation.
    ///     Typically, the expanded view allows the user to make changes to an analysis, the collapsed view should give a
    ///     concise
    ///     overview of the analysis.
    /// </summary>
    public partial class AnalysisControl : UserControl
    {
        protected delegate TReturnType GetDelegate<out TReturnType>();

        protected delegate void ParamDelegate<in TType>(TType value);

        public delegate void RunDelegate();

        /// <summary>
        ///     Constructs an AnalysisControl.
        /// </summary>
        protected AnalysisControl()
        {
            InitializeComponent();
        }

        public void AddProcessListener(ProgressListener listener)
        {
            Analyzer.ProcessListeners += listener;
        }

        public virtual CollapsedView GetCollapsedView()
        {
            return CollapsedView;
        }

        /// <summary>
        ///     Initializes the instance.
        ///     Every subclass should call this method at the end of its constructor providing the needed arguments.
        /// </summary>
        /// <param name="title">A reference to the NameLabel of the subclass.</param>
        /// <param name="moreInfo">A reference to the MoreInfoLabel of the subclass.</param>
        /// <param name="name"></param>
        protected virtual void Init(Label title, LinkLabel moreInfo, string name)
        {
            MoreInfoLabel = moreInfo;
            MoreInfoLabel.Enabled = true;
            MoreInfoLink = ConfigurationManager.AppSettings["MoreInfo_" + name];
            MoreInfoLabel.LinkClicked += MoreInfoLabelLinkClicked;
            CollapsedView = new CollapsedView(title, moreInfo.Location);
        }

        public virtual bool CheckPreconditions()
        {
            return true;
        }

        protected virtual void ProduceAnalyzer()
        {
            Analyzer = null;
        }

        public AbstractAnalyzer GetAnalyzer()
        {
            return Analyzer;
        }

        public virtual void Analyze(AnalyzeModel model)
        {
            ResetUI();
            ProduceAnalyzer();
            model.Analyze(Analyzer, new List<Preprocessor>());
            if (!model.IsReportGeneration)
            {
                RunTS(delegate { ChangeUItoFinished(); });
            }
        }

        protected void ResetUI()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) delegate { ResetUIThreadSafe(); });
            }
            else
            {
                ResetUIThreadSafe();
            }
        }

        private void ResetUIThreadSafe()
        {
            Control directoryLink;
            if (DynamicControls.TryGetValue(DIRECTORY_LINK_ID, out directoryLink))
            {
                Controls.Remove(directoryLink);
                directoryLink.Dispose();
            }

            Control fileLink;
            if (DynamicControls.TryGetValue(FILE_LINK_ID, out fileLink))
            {
                Controls.Remove(fileLink);
                fileLink.Dispose();
            }

            Control destinationLink;
            if (DynamicControls.TryGetValue(DESTINATION_LINK_ID, out destinationLink))
            {
                Controls.Remove(destinationLink);
                destinationLink.Dispose();
            }

            Control graphLink;
            if (DynamicControls.TryGetValue(GRAPH_LINK_ID, out graphLink))
            {
                Controls.Remove(graphLink);
                graphLink.Dispose();
            }
        }

        /// <summary>
        ///     Adapts the UI so to represent that the analysis has been finished.
        /// </summary>
        protected virtual void ChangeUItoFinished()
        {
                 AddOpenContainingFolderLink();
        }

        /// <summary>
        ///     Adds an 'Open Containing Folder' link next to the More Info label.
        /// </summary>
        protected virtual void AddOpenContainingFolderLink()
        {
            if (Analyzer == null) return;

            var dirLink = new LinkLabel();
            var fileLink = new LinkLabel();

            try
            {
                dirLink.Text = "Open containing folder";
                var dstDirs = Analyzer.DestinationFiles.GroupBy(Path.GetDirectoryName).ToArray();
                if (dstDirs.Count() > 1) dirLink.Text += "s (" + dstDirs.Count() + ")";

                dirLink.AutoSize = true;
                var x = MoreInfoLabel.Location.X + MoreInfoLabel.Width + 5;
                var y = MoreInfoLabel.Location.Y;
                dirLink.Location = new Point(x, y);

                // Add the directories to the tag of the linklabel. Although it seems that you can also add multiple 
                // links to a single linklabel (linklabel.Links), the implementation only allows for separate linked pieces
                // of text within a single linklabel. That is, you cannot add multiple links to the same piece of 
                // text in the linklabel. See the documentation of LinkLabel for more info.
                dirLink.Tag = dstDirs.ToArray();
                dirLink.Click += OpenLink;

                fileLink.Text = "Open file";

                x = x + dirLink.Width + 55;
                fileLink.Location = new Point(x, y);

                var htmlFilePath = Path.Combine(
                    Path.GetDirectoryName(Analyzer.OutputFilePath) ?? "",
                    Path.GetFileNameWithoutExtension(Analyzer.OutputFilePath) + ".html"
                );
                Analyzer.OutputFilePath = htmlFilePath;
                fileLink.Tag = htmlFilePath;
                fileLink.Click += OpenBrowser;
            }
            finally
            {
                Controls.Add(dirLink);
                DynamicControls[DIRECTORY_LINK_ID] = dirLink;

                Controls.Add(fileLink);
                DynamicControls[FILE_LINK_ID] = fileLink;
            }
        }

        protected void AddGraphLink(EventHandler act)
        {
            var graphLink = new LinkLabel {Text = "Show graph"};
            var fileLink = (LinkLabel) DynamicControls[FILE_LINK_ID];
            var x = fileLink.Location.X + fileLink.Width + 20;
            var y = fileLink.Location.Y;
            graphLink.Location = new Point(x, y);
            graphLink.Visible = true;
            graphLink.Tag = Analyzer.OutputFilePath;
            graphLink.Click += act;
            DynamicControls[GRAPH_LINK_ID] = graphLink;
            Controls.Add(graphLink);
        }

        /// <summary>
        ///     Exports this analysisControl into a configuration.
        ///     Deriving classes should implement this method. See runAnalysis() for more info.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>
        ///     An AnalysisConfiguration that reflects the configuration of the analysis that is represented
        ///     by this control. An analysisControl of the same type should be able to be initialized to
        ///     the same configuration by passing this AnalysisConfiguration to its Import() method.
        /// </returns>
        public virtual AnalysisConfiguration Export(ExportOptions options)
        {
            // Deriving class should override this method
            throw new NotSupportedException("This method cannot be called on this class."
                                            + " You should override this method in deriving classes.");
        }

        /// <summary>
        ///     Imports a configuration into this analysisControl.
        ///     Deriving classes should implement this method. See runAnalysis() for more info.
        /// </summary>
        /// <param name="configuration">
        ///     An AnalysisConfiguration that reflects the configuration of an analysis
        ///     that can be represented by this control.
        /// </param>
        public virtual void Import(AnalysisConfiguration configuration)
        {
            // Deriving class should override this method
            throw new NotSupportedException("This method cannot be called on this class."
                                            + " You should override this method in deriving classes.");
        }

        /// <summary>
        ///     Click event handler. Opens the standard browser with the file of which the path is specified.
        /// </summary>
        /// <param name="sender">Open file link clicked</param>
        /// <param name="args">Event arg</param>
        private void OpenBrowser(object sender, EventArgs args)
        {
            OpenBrowser(Analyzer.OutputFilePath);
        }

        protected void OpenBrowser(string outputFilePath, object sender = null)
        {
            try
            {
                Process.Start(outputFilePath);
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.NativeErrorCode == 2)
                {
                    MessageLogger.CatchException(sender, noBrowser, Severity.ERROR,
                        "No report. The analysis was unsuccessful.");
                }
                else if (noBrowser.ErrorCode == -2147467259)
                {
                    MessageLogger.CatchException(sender, noBrowser, Severity.ERROR,
                        "The standard browser was not found.");
                }
            }
            catch (FileNotFoundException noFile)
            {
                MessageLogger.CatchException(sender, noFile, Severity.ERROR,
                    "Wrong path. The file was not found.");
            }
            catch (Exception other)
            {
                MessageLogger.CatchException(sender, other, Severity.ERROR,
                    "An unexpected error occurred while opening the link.");
            }
        }

        /// <summary>
        ///     Click event handler for a linklabel. Opens the file of which the path is specified by
        ///     the linklabel that sent this event in a new process with the systems default handler for the filetype of
        ///     the file specified by the linklabel.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OpenLink(object sender, EventArgs args)
        {
            try
            {
                var destinationDirs = Analyzer.DestinationFiles.GroupBy(Path.GetDirectoryName);

                foreach (var dst in destinationDirs)
                {
                    Process.Start(dst.Key);
                }
            }
            catch (Exception exc)
            {
                // we need to explicitely catch all exceptions here, as we are starting a new process.
                // if we do not catch all exceptions explicitely, the exceptions that occur in the new process will not be 
                // caught by the toplevel MessageLogger (This has occured in the past in a case where we were trying
                // to open an non-existing file).
                MessageLogger.CatchException(sender, exc, Severity.ERROR,
                    "An unexpected error occured while opening the link.");
            }
        }

        /// <summary>
        ///     Click Eventlistener for the "More info" link.
        ///     Opens a link to more information about the revision analysis.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void MoreInfoLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(MoreInfoLink);
        }

        public virtual void Close()
        {
			ResetUI();
		}

        protected TReturnType RunGetTS<TReturnType>(GetDelegate<TReturnType> del)
        {
            if (InvokeRequired)
            {
                return (TReturnType) Invoke(del);
            }
            return del();
        }

        protected void RunParamTS<TValueType>(ParamDelegate<TValueType> del, TValueType value)
        {
            if (InvokeRequired)
            {
                Invoke(del, value);
            }
            else
            {
                del(value);
            }
        }

        public void RunTS(RunDelegate del)
        {
            if (InvokeRequired)
            {
                Invoke(del);
            }
            else
            {
                del();
            }
        }

        #region Constants

        private const string DIRECTORY_LINK_ID = "directoryLink";
        protected const string FILE_LINK_ID = "fileLink";
        private const string DESTINATION_LINK_ID = "destinationLink";
        private const string GRAPH_LINK_ID = "graphLink";

        protected virtual string NAME
        {
            get { return ""; }
        }

        #endregion

        #region Fields

        protected AbstractAnalyzer Analyzer;

        /// <summary>
        ///     UI Controls that are dynamically added to this analysis control and that need to be maintained in order to be
        ///     accessed at a later time. This can for example be used to add new controls to the UI after an analysis finishes,
        ///     and clean them up (dispose) when resetting the analysis.
        /// </summary>
        protected readonly Dictionary<string, Control> DynamicControls = new Dictionary<string, Control>();

        /// <summary>
        ///     URL where more info about the analysis can be found.
        /// </summary>
        private string MoreInfoLink;

        /// <summary>
        ///     A reference to the MoreInfoLabel of the subclass.
        /// </summary>
        private LinkLabel MoreInfoLabel;

        /// <summary>
        ///     The collapsed view of the Control.
        /// </summary>
        private CollapsedView CollapsedView { get; set; }

        /// <summary>
        ///     Path to the final output of this analysis
        /// </summary>
        protected string OutputFilePath
        {
            get { return Analyzer.OutputFilePath; }
        }

        /// <summary>
        ///     Path to original output
        /// </summary>
        protected string OrgDocPath { get; set; }

        /// <summary>
        ///     Path to a source file (*.idfx).
        /// </summary>
        public string SourcePath
        {
            get { return Analyzer.SourcePath; }
        }

        /// <summary>
        ///     Name of the Analysis used by the Control.
        /// </summary>
        public string AnalysisName
        {
            get { return NAME; }
        }

        /// <summary>
        ///     Number of sources to analyze as selected by the user.
        /// </summary>
        protected int SourceCount
        {
            get { return Tabs.Analyze.Analyze.GetSourceCount(); }
        }

        /// <summary>
        ///     The abbreviation of the analysis name.
        /// </summary>
        protected string AnalysisAbbreviation { get; set; }

        #endregion
    }
}