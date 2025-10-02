using System;
using System.Windows.Forms;

namespace Server.ServerConfig
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new global::ServerConfig.ServerConfig());
            }
            else
            {
                try
                {
                    var conf = new global::ServerConfig.ServerConfig();
                    if (args[0].Equals("--sendmails"))
                    {
                        conf.SendMailsButtonClick(null, null);
                    }
                    if (args[0].Equals("--cleanup"))
                    {
                        conf.CleanupButtonClick(null, null);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
