using System;
using System.Threading;
using System.Windows.Forms;
using InputLog.Core.Util;

namespace GUI
{
    /// <summary>
    /// The main class of the GUI project, launches the GUI.
    /// </summary>
    static class GuiMain
    {
        /// <summary>
        /// Main Exception Handler.
        /// Displays a messagebox with more information about the exception that occured.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="t">Event argument.</param>
        private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs t)
        {
            // Report the exception to our logger.
            MessageLogger.CatchException(sender, t.Exception, Severity.ERROR);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The CLI arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Use thread exception handler instead of try-catch block.
            // This allows the application to recover (wrapping the Application.Run() statement 
            // in a try-catch block will end the program).
            Application.ThreadException += ThreadExceptionHandler;
            Application.Run(Gui.GetInstance(args));
        }
    }
}