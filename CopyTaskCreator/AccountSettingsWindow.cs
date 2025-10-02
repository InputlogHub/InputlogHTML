using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApp.Controllers;

namespace CopyTaskCreator
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

            AccountAutologin.Checked = Properties.Settings.Default.autoLogin;
            AccountUsername.Text = Properties.Settings.Default.username;
            Accountpassword.Text = Properties.Settings.Default.password;
        }

        /// <summary>
        /// Callback for the OK button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OKButtonClick(object sender, EventArgs e)
        {
            lblStatus.Text = @"Saving...";
            Properties.Settings.Default.username = AccountUsername.Text;
            Properties.Settings.Default.password = Accountpassword.Text;
            Properties.Settings.Default.autoLogin = AccountAutologin.Checked;
            Properties.Settings.Default.Save();
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

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            var isLoginSuccessful = await ServerConnection.checkCredentials(AccountUsername.Text, Accountpassword.Text);

            if (isLoginSuccessful)
            {   
                lblStatus.Text = @"Login succesful.";
            }
            else
            {
                lblStatus.Text = @"Wrong credentials.";
            }
        }
    }
}