
namespace InputLog.Core.Util
{
    /// <summary>
    /// Class exporting the public settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Reference to the actual Settings.
        /// </summary>
        private static readonly Properties.Settings Backend = Properties.Settings.Default;

        /// <summary>
        /// Time out to wait in EventLog.Stop() before a stop is forced.
        /// </summary>
        public static int StopTimeout
        {
            get { return Backend.Stop_Timeout; }
            set { Backend.Stop_Timeout = value; }
        }

        /// <summary>
        /// True if the original word document is allowed to be overridden,
        /// false if not.
        /// </summary>
        public static bool WordLogOverrideDoc
        {
            get { return Backend.WordLog_OverrideDoc; }
            set { Backend.WordLog_OverrideDoc = value; }
        }

        /// <summary>
        /// If true, no event logging outside the main Word document is allowed, except for the title of the window.
        /// The outcommented setter & getter hereafter is used in the Cardiff version, together 
        /// with the 'Wordlog Restricted' button set to 'Visible = False' in Logging.cs[Design]. 
        /// This setting imposes the restricted logging as default while refusing the user to change it.
        /// </summary>
        public static bool WinLogRestricted
        {
            get { return Backend.WinLog_Restricted; }
            set { Backend.WinLog_Restricted = value; }
        }

        /// <summary>
        /// Threshold for pausetimes between mouse-movements.
        /// When the difference between the end of a mouse-movement and the startTime of the next mouse-movement exceeds this 
        /// threshold, the 2 mouse events will not be accumulated but treated as 2 seperate events.
        /// </summary>
        public static ulong WinLogMouseMovementPauseThreshold
        {
            get { return Backend.WinLog_MouseMovementPauseThreshold; }
            set { Backend.WinLog_MouseMovementPauseThreshold = value; }
        }

        /// <summary>
        /// If set to true, all add-ins in Word will be unloaded when WordLog is started.
        /// </summary>
        public static bool WordLogDisableWordAddins
        {
            get { return Backend.WordLog_DisableWordAddins; }
            set { Backend.WordLog_DisableWordAddins = value; }
        }

        /// <summary>
        /// If set to true, the idfx will be segmented according to the key
        /// specified in the KeyDelimiter setting.
        /// </summary>
        public static bool Segmentation_KeyDelimiterActive
        {
            get { return Backend.Segmentation_KeyDelimiterIsActive; }
            set { Backend.Segmentation_KeyDelimiterIsActive = value; }
        }

        /// <summary>
        /// Set the key to be used as the delimiter key for idfx segmentation
        /// </summary>
        public static string Segmentation_KeyDelimiter
        {
            get { return Backend.Segmentation_KeyDelimiter; }
            set { Backend.Segmentation_KeyDelimiter = value; }
        }

        /// <summary>
        /// If true to include the initial pause time.
        /// </summary>
        public static bool Segmentation_IncludeInitialPause
        {
            get { return Backend.Segmentation_IncludeInitialPause; }
            set { Backend.Segmentation_IncludeInitialPause = value; }
        }

        /// <summary>
        /// Sets the number of minutes between each document version save action during a writing session.
        /// </summary>
        public static ulong TimebasedIntervalSave
        {
            get {return Backend.TimebasedIntervalSave; }
            set { Backend.TimebasedIntervalSave = value; }
        }

        /// <summary>
        /// Defines the keyboard action that triggers a document version save during a writing session.
        /// </summary>
        public static string UserActionIntervalSave
        {
            get { return Backend.UserActionIntervalSave; }
            set { Backend.UserActionIntervalSave = value; }
        }

        /// <summary>
        /// Allows the user to add a comment at the end of the writing session.
        /// </summary>
        public static bool AddComment
        {
            get { return Backend.AddComment; }
            set { Backend.AddComment = value; }
        }

        /// <summary>
        /// Contains settings for the analyses.
        /// </summary>
        public static class Analysis
        {
            /// <summary>
            /// Pausetime threshold that indicates whether 2 consecutive mouse clicks should be considered 
            /// as a double click or not.
            /// </summary>
            public static ulong DoubleClickThreshold
            {
                get { return Backend.Analysis_DoubleClickThreshold; }
                set { Backend.Analysis_DoubleClickThreshold = value; }
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void Save()
        {
            Backend.Save();
        }

        /// <summary>
        /// Reloads the settings.
        /// </summary>
        public static void Reload()
        {
            Backend.Reload();
        }

        /// <summary>
        /// Resets the settings.
        /// </summary>
        public static void Reset()
        {
            Backend.Reset();
        }
    }
}
