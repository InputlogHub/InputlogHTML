using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using GUI.Tabs.Analyze.AnalysesControls;
using InputLog.Core.IO;
using InputLog.Core.Preprocessing;
using InputLog.Core.Util.Progress;

namespace GUI.Tabs.Preprocess
{
    /// <summary>
    /// Common base class for filter controls to implement.
    /// An filterControl is a visual representation of an event filter in the GUI.
    /// 
    /// There should always be 2 representations: a collapsed and expanded representation.
    /// Typically, the expanded view allows the user to make changes to the specifics of a filter, 
    /// the collapsed view should give a concise overview of the filter.
    /// </summary>
    public partial class ProcessControl : UserControl
	{
		#region fields

		/// <summary>
        /// Actual ProgressTrackableAction that does the progresstracking
        /// (composition used, no multiple inheritance in C#).
        /// </summary>
        private readonly ProgressTrackableAction Progress = new ProgressTrackableAction();

        /// <summary>
        /// Name of the Analysis used by the Control.
        /// </summary>
        protected string FilterName { get; set; }

        /// <summary>
        /// The abbreviation of the analysis name.
        /// </summary>
        public string FilterAbbreviation { get; protected set; }

        /// <summary>
        /// The number of steps the analysis (that is part of this control) will need to analyse the logfile.
        /// </summary>
        public int NumberOfSteps
        {
            get { return Progress.NumberOfSteps; }
            private set { Progress.NumberOfSteps = value; }
        }

		/// <summary>
		/// Returns whether this process control can handle multiple files at the same
		/// time or can only process one time at a time.
		/// </summary>
		public virtual bool MultipleFileCompatible { get { return false;} }

        /// <summary>
        /// A reference to the MoreInfoLabel of the subclass.
        /// </summary>
        private LinkLabel MoreInfoLabel;

        /// <summary>
        /// URL where more info about the analysis can be found.
        /// </summary>
        private string MoreInfoLink;

        /// <summary>
        /// The collapsed view of the Control.
        /// </summary>
		public CollapsedView CollapsedView { get; private set; }

		/// <summary>
		/// List of all paths to files that have been selected to be processed by
		/// this control.
		/// </summary>
		private List<string> ThisFilePaths;

		/// <summary>
		/// List of all paths to files that have been selected to be processed by
		/// this control.
		/// </summary>
		public List<string> FilePaths
		{
			get
			{
				return ThisFilePaths;
			}
			set
			{
				OnFileSelectionChange(value);
				ThisFilePaths = value;
			}
		}

        #endregion

        /// <summary>
        /// DO NOT USE THIS CONSTRUCTOR!
        /// It is only provided because the designer does not work otherwise.
        /// </summary>
        public ProcessControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs an AnalysisControl.
        /// </summary>
        /// <param name="filterName">The name of the analysis.</param>
        /// <param name="filterAbbr">The abbreviation of the name of the filter. This will be used in ExtendFileName.</param>
        /// <param name="infolink">URL where more info about the filter can be found.</param>
        protected ProcessControl(string filterName, string filterAbbr, string infolink = "http://www.inputlog.net")
        {
            NumberOfSteps = 3;
            FilterName = filterName;
            FilterAbbreviation = filterAbbr;
            MoreInfoLink = infolink;
			FilePaths = new List<string>();

            InitializeComponent();
        }

        /// <summary>
        /// Initializes a link to the Inputlog Manual.
        /// Every subclass should call this method at the end of its constructor providing the
        /// needed arguments.
        /// </summary>
        /// <param name="moreInfo">A reference to the NameLabel of the subclass.</param>
        /// <param name="name">A reference to the NameLabel of the subclass.</param>
        protected void InitHelp(LinkLabel moreInfo, string name)
        {
            MoreInfoLabel = moreInfo;
            MoreInfoLabel.Enabled = true;
            MoreInfoLink = ConfigurationManager.AppSettings["MoreInfo_" + name];
            MoreInfoLabel.LinkClicked += MoreInfoLabelLinkClicked;
            CollapsedView = new CollapsedView(MoreInfoLabel, MoreInfoLabel.Location);
        }

        /// <summary>
        /// This method should be implemented by the subclasses so that it returns the correct EventFilter 
        /// for performing the actual analysis.
        /// 
        /// Note that this method should normally be abstract in order to make sure that all deriving class implement this method.
        /// This is however not possible, since this would require the entire class to be abstract.
        /// When doing this, the Design Mode of VS can no longer be used to design any derived (from this class) user control,
        /// because VS must be able to instantiate any (user control) class that is edited in Designer mode.
        /// Clearly, by making this class abstract, that is no longer possible and so you won't be able to use the Designer Mode 
        /// to edit this class or any derived classes.
        /// For this reason, it was decided to make this method virtual instead, and require (by convention!) that every
        /// deriving class overrides this method.
        /// </summary>
        /// <returns>The EventFilter that implements the actual event filter.</returns>
        public virtual Preprocessor GetPreprocessor(SessionIdentification sessionID) {
            throw new NotSupportedException("This method cannot be called on this class." +
            " You should override this method in deriving classes.");
        }


        /// <summary>
        /// Serializes this filterControl.
        /// Deriving classes should implement this method. See runAnalysis() for more info.
        /// </summary>
        public virtual void Serialize()
        {
            // Deriving class should override this method
            throw new NotSupportedException("This method cannot be called on this class."
            + " You should override this method in deriving classes.");
        }


        /// <summary>
        /// Click Eventlistener for the "More info" link.
        /// Opens a link to more information about the revision analysis.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void MoreInfoLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(MoreInfoLink);
        }

        /// <summary>
        /// Exports this filterControl into a configuration.
        /// Deriving classes should implement this method. See GetEventFilter() for more 
        /// info on why this method is not abstract.
        /// </summary>
        /// <param name="options">Options specifying whether certain fields should be exported or not.</param>
        /// <returns>An FilterConfiguration that reflects the configuration of the filter that is represented 
        /// by this control. An filter of the same type should be able to be initialized to 
        /// the same configuration by passing this FilterConfiguration to its Import() method.</returns>
        public virtual FilterConfiguration Export(FilterExportOptions options) {
            // Deriving class should override this method
            throw new NotSupportedException("This method cannot be called on this class."
            + " You should override this method in deriving classes.");
        }

        /// <summary>
        /// Imports a configuration into this filterControl.
        /// Deriving classes should implement this method. See GetEventFilter() for more 
        /// info on why this method is not abstract.
        /// </summary>
        /// <param name="configuration">An FilterConfiguration that reflects the configuration of an filter 
        /// that can be represented by this control. </param>
        public virtual void Import(FilterConfiguration configuration) {
            // Deriving class should override this method
            throw new NotSupportedException("This method cannot be called on this class."
            + " You should override this method in deriving classes.");
        }

		/// <summary>
		/// Callback method for when the selection of files to be processed by
		/// this processControl has been changed.
		/// </summary>
		/// <param name="filePaths">List of all paths of selected files.</param>
		public virtual void OnFileSelectionChange(List<string> filePaths)
		{
			ThisFilePaths = filePaths;
            
		}
    }
}