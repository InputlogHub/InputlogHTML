using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GUI.Properties;
using GUI.Tabs.Analyze;
using InputLog.Core.Util;
using log4net;
using Microsoft.Win32;

namespace GUI.Util
{
    /// <summary>
    ///     The ExceptionMessageBox is sort of Dialogbox to easily handle exceptions.
    ///     When an ExceptionMessageBox is shown, the information in the accompanying exception is logged and the user can
    ///     choose to either view the details of the error, send an error report or dismiss the error.
    ///     FUTURE: This class contains some logic to extract system information from windows. It might be interesting to
    ///     extract  this logic into a seperate class (so that is can be used when this information is needed in other parts of the
    ///     program).
    ///     TODO: Integrate this with an online bugtracker (let people submit reports from within inputlog).
    /// </summary>
    public partial class ExceptionMessageBox : Form
    {
        /// <summary>
        ///     Constructs and Shows an ExceptionMessagebox.
        /// </summary>
        /// <param name="exc">Exception for which the messagebox should be shown.</param>
        /// <param name="level">Level indicating the severity of the error.</param>
        /// <param name="title">Title of the exception messagebox (if no is given, a default title is used).</param>
        /// <param name="message">Message to show in the messagebox (if no is given, 
        /// the message from the given exception is used).</param>
        private ExceptionMessageBox(Exception exc, Severity level = Severity.ERROR, string title = null,
            string message = null)
        {
            InitializeComponent();
            var file = StringUtils.ShortenPathname(AnalyzeModel.GetCurrentSourcePath(), 80);
            if (null == file || file.Equals(string.Empty))
            {
                file = "";
            }
            else
            {
                file = " File: " + file;
            }

        Exception = exc;

            if (message == null || message.Trim() == string.Empty)
            {
                if (exc.Message.Trim() == string.Empty)
                {
                    ErrorDetailsLabel.Text = Resources.ExceptionMessageBox_StandardMessage + file;
                }
                else
                {
                    ErrorDetailsLabel.Text = exc.Message + file;
                }
            }
            else
            {
                ErrorDetailsLabel.Text = message + file;
            }

            Text = string.IsNullOrEmpty(title) ? Resources.ExceptionMessageBox_StandardTitle : title;

            SetIcon(level);
            Log(level, message + file);
            DetailsTextBox.Text = CreateErrorReport(Exception);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        ///     Shows an ExceptionMessagebox.
        /// </summary>
        /// <param name="exc">Exception for which the messagebox should be shown.</param>
        /// <param name="level">Level indicating the severity of the error.</param>
        /// <param name="title">Title of the exception messagebox (if no is given, a default title is used).</param>
        /// <param name="message">Message to show in the messagebox (if no is given, 
        /// the message from the given exception is used).</param>
        public static void Show(Exception exc, Severity level = Severity.ERROR, string title = null,
            string message = null)
        {
            var box = new ExceptionMessageBox(exc, level, title, message);

            try
            {
                box.ShowDialog();
            }
            finally
            {
                box.Dispose();
            }
        }

        /// <summary>
        ///     Creates an error report based on an exception.
        ///     The report contains:
        ///     * A Stack Trace of the Exception
        ///     * Environment info (windows + .NET versions)
        ///     * The installed version of word
        ///     * Other system information provided by .NET
        /// </summary>
        /// <param name="e">Exception for which to create a report.</param>
        /// <returns>
        ///     An error report for the given exception (string representation,
        ///     ready to be written to file or something similar).
        /// </returns>
        private string CreateErrorReport(Exception e)
        {
            var sb = new StringBuilder("----------------------------\n EXCEPTION\n----------------------------\n");
            sb.Append(e);

            sb.Append("\n\n----------------------------\n STACKTRACE\n----------------------------\n");
            sb.Append(e.StackTrace);

            sb.Append("\n\n------------------------------\n ENVIRONMENT\n------------------------------\n");
            sb.Append("OS:" + GetOSString() + "\n");
            sb.Append(".NET CLR Version:" + Environment.Version);

            sb.Append("\n\n----------------\n WORD\n----------------\n");
            sb.Append(GetWordVersionString());

            sb.Append(
                "\n\n--------------------------------------------------------\n MISC SYSTEM INFORMATION\n--------------------------------------------------------\n");
            var t = typeof (SystemInformation);
            var pi = t.GetProperties();
            foreach (PropertyInfo t1 in pi)
            {
                if (t1.Name != "PowerStatus")
                {
                    sb.Append(t1.Name + ":" + t1.GetValue(null, null) + "\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Returns a string with information about the currently installed version of Word.
        ///     Largely based on code found on codeproject.com:
        ///     Getting Office's Version (By Niskov | 30 May 2008)
        ///     http://www.codeproject.com/KB/office/getting_office_version.aspx
        /// </summary>
        /// <returns>A string contain information about the currently installed 
        /// version of word (version, product name, etc)</returns>
        private static string GetWordVersionString()
        {
            var wordVersionString = "Unable to gather information about MS Word (is MS Word installed?)";

            // Searching for the registry for Word installation-path en start looking in CURRENT_USER
            string wordPath = null;
            var mainKey = Registry.CurrentUser;
            try
            {
                mainKey = mainKey.OpenSubKey(APP_PATH_REG_KEY + "\\winword.exe", false);
                if (mainKey != null)
                {
                    wordPath = mainKey.GetValue(string.Empty).ToString();
                }
            }
            catch
            {
            }

            // If not found, look inside LOCAL_MACHINE.
            mainKey = Registry.LocalMachine;
            if (string.IsNullOrEmpty(wordPath))
            {
                try
                {
                    mainKey = mainKey.OpenSubKey(APP_PATH_REG_KEY + "\\winword.exe", false);
                    if (mainKey != null)
                    {
                        wordPath = mainKey.GetValue(string.Empty).ToString();
                    }
                }
                catch
                {
                } // ignore any exceptions
            }

            // Closing the handle.
            if (mainKey != null)
                mainKey.Close();

            // If a valid path has been found, extract the version information.
            if (wordPath != null && File.Exists(wordPath))
            {
                try
                {
                    var wordVersion = FileVersionInfo.GetVersionInfo(wordPath);
                    wordVersionString = wordVersion.ToString();
                }
                catch
                {
                } // ignore any exceptions
            }

            return wordVersionString;
        }

        /// <summary>
        ///     Getting a string representing the current version of Windows.
        ///     based on:
        ///     http://support.microsoft.com/kb/304283
        ///     http://www.csharp411.com/determine-windows-version-and-edition-with-c/#comment-5347
        /// </summary>
        /// <returns>A string representing the current version of Windows.</returns>
        private string GetOSString()
        {
            var os = Environment.OSVersion;
            var osString = os.ToString();
            // Determines which version of windows is used based on the version numbering.
            switch (os.Version.Major)
            {
                //cases <4: prior to win xp  (unsupported)
                case 5:
                {
                    switch (os.Version.Minor)
                    {
                        case 0:
                            osString += " (Windows 2000)";
                            break;
                        case 1:
                            osString += " (Windows XP)";
                            break;
                    }
                    break;
                }
                case 6:
                {
                    switch (os.Version.Minor)
                    {
                        case 0:
                            osString += " (Windows Vista)";
                            break;
                        case 1:
                            osString += " (Windows 7)";
                            break;
                        case 3:
                            osString += "(Windows Server 2008)";
                            break;
                    }
                    break;
                }
            }
            return osString;
        }

        /// <summary>
        ///     Sets the Icon of the Messagebox based on a given severity level.
        /// </summary>
        /// <param name="level">Level indicating the severity of the error that occured.</param>
        private void SetIcon(Severity level)
        {
            switch (level)
            {
                case Severity.DEBUG:
                    ErrorIcon.Image = SystemIcons.Asterisk.ToBitmap();
                    break;
                case Severity.ERROR:
                    ErrorIcon.Image = SystemIcons.Error.ToBitmap();
                    break;
                case Severity.FATAL:
                    ErrorIcon.Image = SystemIcons.Error.ToBitmap();
                    break;
                case Severity.INFO:
                    ErrorIcon.Image = SystemIcons.Information.ToBitmap();
                    break;
                case Severity.WARNING:
                    ErrorIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        /// <summary>
        ///     Logs an exception to the loggerchain.
        /// </summary>
        /// <param name="level">Level indicating the severity of the exception.</param>
        /// <param name="message">Message giving more details about the exception.</param>
        private void Log(Severity level, string message)
        {
            switch (level)
            {
                case Severity.DEBUG:
                    ThisLog.Debug(message, Exception);
                    break;
                case Severity.ERROR:
                    ThisLog.Error(message, Exception);
                    break;
                case Severity.FATAL:
                    ThisLog.Fatal(message, Exception);
                    break;
                case Severity.INFO:
                    ThisLog.Info(message, Exception);
                    break;
                case Severity.WARNING:
                    ThisLog.Warn(message, Exception);
                    break;
            }
        }

        /// <summary>
        ///     Handles the click event for the show details link label.
        ///     Shows the Details panel when it is invisble, hides it when it is visible.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ShowDetailsLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DetailsTextBox.Visible)
            {
                ShowDetailsLinkLabel.Text = "Show Details"; //FUTURE extract these strings
                DetailsTextBox.Hide();
                AutoSize = true;
            }
            else
            {
                ShowDetailsLinkLabel.Text = "Hide Details";
                DetailsTextBox.Show();
                AutoSize = false; // Make it resizable
            }
        }

        /// <summary>
        ///     Handles the OKButton click event.
        ///     Closes the ExceptionMessageBox.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OKButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Handles the "Send Report" Click event.
        ///     Opens the default mail application (if any), and populates the subject, title, and message fields.
        ///     The message field will contain details about the exception.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SendReportLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Creating email message.
            var builder = new StringBuilder();
            builder.Append("mailto:luuk.vanwaes@uantwerpen.be;marielle.leijten@uantwerpen.be");
            builder.Append("&subject=[InputLog] Error Report: " + ErrorDetailsLabel.Text);
            builder.Append("&body=[Insert own message here]%0D%0A %0D%0A");
            builder.Append("Report Details:%0D%0A");
            // Escaping some charachters in hex
            var errorReport = CreateErrorReport(Exception).Replace("\n", "%0D%0A").Replace("&", "%26");
            //TODO fix this: windows has a short maxlength on commandline parameters, 
            // This is why the error report gets truncated in the emailapplication,
            // see: http://www.weask.us/entry/process-start-parameters-truncated
            builder.Append(errorReport);

            // Starts mail client and populate the fields
            try
            {
                Process.Start(builder.ToString());
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Unable to start default email application. Is there a default email application installed?",
                    "Unable to start email application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Fields

        /// <summary>
        ///     Log4Net MessageLogger.
        /// </summary>
        private static readonly ILog ThisLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Exception for which this ExceptionMessageBox was created.
        /// </summary>
        private readonly Exception Exception;

        /// <summary>
        ///     Registry path to the directory that should contain the path to Word.
        /// </summary>
        private const string APP_PATH_REG_KEY = @"Software\Microsoft\Windows\CurrentVersion\App Paths";

        #endregion
    }
}