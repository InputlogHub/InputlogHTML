using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerConfig
{
    static class ServerConfigProgram
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
                Application.Run(new ServerConfig());
            }
            else
            {
                try
                {
                    var conf = new ServerConfig();
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
