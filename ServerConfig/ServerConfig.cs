using System;
using System.Windows.Forms;
using InputLog.Core.Util.Server;

namespace ServerConfig
{
    public partial class ServerConfig : Form
    {
        public ServerConfig()
        {
            InitializeComponent();
            ServerUtils.Initialize();
        }

        private void SettingsButtonClick(object sender, EventArgs e)
        {
            var options = new AccountSettingsWindow();
            try
            {
                options.ShowDialog();
            }
            catch (Exception)
            {

            }
            finally
            {
                options.Dispose();
            }
        }

        public void SendMailsButtonClick(object sender, EventArgs e)
        {
            if (ServerUtils.Login(true))
                ServerUtils.SendMails();
        }

        public void CleanupButtonClick(object sender, EventArgs e)
        {
            if (ServerUtils.Login(true))
                ServerUtils.Cleanup();
        }
    }
}
