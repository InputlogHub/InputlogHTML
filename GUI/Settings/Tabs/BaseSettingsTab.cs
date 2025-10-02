using System;
using System.Windows.Forms;

namespace GUI.Settings.Tabs
{
    /// <summary>
    /// Common base class for tabs of the settings dialog.
    /// </summary>
    public partial class BaseSettingsTab : UserControl
    {
        #region Fields

        /// <summary>
        /// SettingsWindow class to save the settings to.
        /// </summary>
        protected readonly Properties.Settings Settings = Properties.Settings.Default;

        /// <summary>
        /// A reference back to the Gui.
        /// </summary>
        public Gui GUI { get; set; }

        #endregion

        /// <summary>
        /// Constructs the BaseSettingsTab.
        /// </summary>
        protected BaseSettingsTab()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Apply the settings as shown by the GUI (save them to SettingsWindow).
        /// 
        /// Note that this method should normally be abstract in order to make sure that all deriving class implement this method.
        /// This is however not possible, since this would require the entire class to be abstract.
        /// When doing this, the Design Mode of VS can no longer be used to design any derived (from this class) user control,
        /// because VS must be able to instantiate any (user control) class that is edited in Designer mode.
        /// Clearly, by making this class abstract, that is no longer possible and so you won't be able to use the Designer Mode 
        /// to edit this class or any derived classes.
        /// For this reason, it was decided to make this method virtual instead, and require (by convention!) that every
        /// deriving class overrides this method.
        /// 
        /// When you do call this method directly, an NotSupportedException will be thrown.
        /// </summary>
        public virtual void ApplySettings()
        {
            throw new NotSupportedException("ApplySettings not supported by BaseSettingsTab");
        }

        /// <summary>
        /// Reload the stored settings and update the GUI to represent the new values.
        /// 
        /// Note that this method should normally be abstract in order to make sure that all deriving class implement this method.
        /// This is however not possible, since this would require the entire class to be abstract.
        /// When doing this, the Design Mode of VS can no longer be used to design any derived (from this class) user control,
        /// because VS must be able to instantiate any (user control) class that is edited in Designer mode.
        /// Clearly, by making this class abstract, that is no longer possible and so you won't be able to use the Designer Mode 
        /// to edit this class or any derived classes.
        /// For this reason, it was decided to make this method virtual instead, and require (by convention!) that every
        /// deriving class overrides this method.
        /// 
        /// When you do call this method directly, an NotSupportedException will be thrown.
        /// </summary>
        public virtual void ReloadSettings()
        {
            throw new NotSupportedException("ReloadSettings not supported by BaseSettingsTab");
        }
    }
}