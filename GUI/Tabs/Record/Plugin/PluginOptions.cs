using System.Windows.Forms;
using GUI.Session;

namespace GUI.Tabs.Record.Plugin
{
    /// <summary>
    /// Empty option panel.
    /// </summary>
    public partial class PluginOptions : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PluginOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Modifies the properties (and the GUI) such that it reflects the metadata contained in the given RecordSession.
        /// Subclasses should override this method if the empty implementation if not wanted.
        /// <param name="session">The session data.</param>
        /// </summary>
        virtual public void SetSessionData(RecordSession session) { }

        /// <summary>
        /// Adds the session information currently stated in the GUI to the given RecordSession.
        /// Subclasses should override this method if the empty implementation if not wanted.
        /// </summary>
        /// <param name="session">The object in which the session data should be saved.</param>
        virtual public void GetSessionInfo(RecordSession session) { }
    }
}