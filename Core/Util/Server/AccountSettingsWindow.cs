using System;
using System.Windows.Forms;

namespace InputLog.Core.Util.Server
{
    /// <summary>
    /// Form for storing credentials and settings for interaction with InputLog server
    /// </summary>
    public partial class AccountSettingsWindow : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountSettingsWindow()
        {
            InitializeComponent();
            AccountSettings settings = AccountSettings.LoadSettings();
            AccountAutologin.Checked = settings.AutoLogin;
            AccountUsername.Text = settings.ID;
            Accountpassword.Text = settings.Pass;
        }

        /// <summary>
        /// Callback for the OK button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OKButtonClick(object sender, EventArgs e)
        {
            StatusLabel.Text = @"Saving...";
            var settings = new AccountSettings(AccountUsername.Text, Accountpassword.Text, 
                AccountAutologin.Checked);
            settings.SaveSettings();
            StatusLabel.Text = @"Done";
            Close();
        }

        /// <summary>
        /// Callback for the Cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

    }
}