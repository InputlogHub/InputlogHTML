using System;
using System.IO;
using System.Text;
using InputLog.Core.Util;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace InputLog.Core.Plugin.WordLog
{
    /// <summary>
    /// A document fit to be logged (SaveAs is disabled, ...)
    /// </summary>
    internal sealed class LoggedDocument : InputlogDocument
    {
        #region Fields
        /// <summary>
        /// If not null, the opened document will also be saved to this location every time a save occurs.
        /// </summary>
        private string Destination { get; set; }

        /// <summary>
        /// Returns true if and the Word Application has focus and
        /// the active document in it equals this.Document.
        /// </summary>
        public bool Active { private set; get; }

        /// <summary>
        /// True if we should intercept Save actions, false if we should allow them.
        /// </summary>
        private bool _interceptSave;

        private readonly object _saveLock = new object();

        public event EventHandler<EventArgs> WordCloseAttempt;
        /// <summary>
        /// The Word default document file format. As from Microsoft Office Word 2007 on this is the *.docx format.
        /// </summary>
        private const WdSaveFormat FORMAT = WdSaveFormat.wdFormatDocumentDefault;

        #endregion

        /// <summary>
        /// Constructor, opens a the document pointed to by path in Word and disables
        /// some of Words features.
        /// </summary>
        /// <param name="source">The path to the document to open.</param>
        /// <param name="destination">If not null, every time the document is saved,
        /// the document will also be copied to this path.</param>
        private LoggedDocument(string source, string destination = null)
            : base(new Application().Documents.Open(source))
        {
            Destination = destination;
            Active = true;
            _interceptSave = true; // We always intercept save operations, unless when already intercepting one

            try
            {
                // Callbacks to disable save, close and discontinuous selections
                Word.DocumentBeforeSave += BeforeSave; // Disable save as of logged document

                // If no current selection, MS Word "selects" the current word when right clicking
                Word.WindowBeforeRightClick += BeforeRightClick;

                // Activation listeners
                Word.WindowActivate += WindowActivated;
                Word.WindowDeactivate += WindowDeactivated;
                Word.WindowSize += WindowsSized;
                Word.Visible = true;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                while (!Word.Visible && sw.ElapsedMilliseconds < 1000)
                {
                    // Just wait at most 1sec.
                }
                try
                {
                    Word.Activate();
                    Document.Activate();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            catch (Exception e)
            {
                throw new PluginException("Unknown exception caught.", e.InnerException);
            }
        }

        /// <summary>
        /// Microsoft InteropServices returning the name of the window in focus.
        /// </summary>
        /// <returns>Window in focus</returns>
        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }
            return null;
        }

        protected override void BeforeClose(Document doc, ref bool cancel)
        {
            base.BeforeClose(doc, ref cancel);
            OnWordCloseAttempt(EventArgs.Empty);
        }

        private void OnWordCloseAttempt(EventArgs e)
        {
            // Note the use of a temporary variable here to make the event raising
            // thread-safe; may or may not be necessary in your case.
            var evt = WordCloseAttempt;
            evt?.Invoke(this, e);
        }

        /// <summary>
        /// Callback when a document in our opened Word application is saved.
        /// This is used in order to disable the 'save as' operation of our
        /// logged document.
        /// If we would not disable this function, the user could rename the
        /// document and we cannot compare with with our document path any
        /// more in order to check whether the document is the logged one or not.
        /// </summary>
        /// <param name="doc">The document that is being saved.</param>
        /// <param name="saveAsUi">Set to true if save as should be shown, false if not.</param>
        /// <param name="cancel">Set to true in order to cancel the operation.</param>
        private void BeforeSave(Document doc, ref bool saveAsUi, ref bool cancel)
        {
            try
            {
                if (!_interceptSave || !doc.FullName.Equals(Document.FullName)) return;
                lock (_saveLock)
                {
                    // Do not intercept save operations for as long this method is running
                    _interceptSave = false;

                    var wasActive = Active;
                    try
                    {
                        // During saving the document is not active
                        Active = false;

                        // Current Fullname is the destination in the Inputlog workspace
                        var workspacedoc = Document.FullName;

                        if (saveAsUi || string.IsNullOrWhiteSpace(Destination))
                        {
                            // Show SaveAs dialog, this will enable the user to save the document on a new place
                            Word.Dialogs[WdWordDialog.wdDialogFileSaveAs].Show();

                            // Update the destination to the new path picked by the user
                            Destination = Document.FullName;
                        }
                        else if (!Document.FullName.Equals(Destination))
                        {
                            for (var i = 1; i <= Word.Documents.Count; i++)
                            {
                                // 20110920 EVH Fixed ticket #28
                                var path = Word.Documents[i].FullName;
                                if (string.Equals(path, Destination, StringComparison.OrdinalIgnoreCase))
                                {
                                    MessageLogger.LogMessage(this, "Unable to save document",
                                        $"The document \"{Destination}\"is open in another window," +
                                        "this window must be closed before you can save the logged document.", Severity.ERROR);
                                    return;
                                }
                            }

                            // As it was not a SaveAs operation, we still need to save the document on the destination
                            Document.SaveAs(Destination, FORMAT);
                        }

                        // Save it back in the Inputlog workspace
                        Document.SaveAs(workspacedoc, FORMAT);
                    }
                    catch (Exception e)
                    {
                        MessageLogger.CatchException(this, e, Severity.ERROR, "Unable to save document.");
                    }
                    finally
                    {
                        // Cancel the save operation (the save is already completed)
                        saveAsUi = false;
                        cancel = true;

                        // Start intercepting save operations again.
                        _interceptSave = true;

                        // Set back old active state
                        Active = wasActive;
                    }
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Callback when a right click is done.
        /// </summary>
        /// <param name="sel">The current selection.</param>
        /// <param name="cancel">Will be false on entrance of the procedure, assign true to cancel.</param>
        private void BeforeRightClick(Selection sel, ref bool cancel)
        {
            try
            {
                if (!Active) return;
                if (sel.Start == sel.End)
                {
                    sel.Expand(WdUnits.wdWord);
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Callback when a document window of Word is activated;
        /// sets this. Active to true if the activated document is
        /// the logged document and to false if it is another document.
        /// Using Window InteropServices to decide if Inputlog has focus or not.
        /// </summary>
        /// <param name="doc">The document displayed in the activated window.</param>
        /// <param name="win">The window that's being activated.</param>
        private void WindowActivated(Document doc, Window win)
        {
            try
            {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(doc.Name);
                if (nameWithoutExtension != null && GetActiveWindowTitle().Contains(nameWithoutExtension))
                {
                    Active = true;
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
            // Console.WriteLine("WindowActivated - Inputlog doc is active: " + Active + " short name: " + nameWithoutExtension + " WindowTitle: " + GetActiveWindowTitle());
        }

        /// <summary>
        /// Callback when a document window of Word is deactivated;
        /// sets this.Active to false if the deactivated document is
        /// the logged document.
        /// Using the Application object property 'ActiveWindow.Active' to decide if Inputlog has focus or not.
        /// </summary>
        /// <param name="doc">The document displayed in the deactivated window.</param>
        /// <param name="win">The deactivated window.</param>
        private void WindowDeactivated(Document doc, Window win)
        {
            try
            {
                 Active = false;
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }

            //Console.WriteLine("WindowDeactivated - Inputlog doc is active: " + Active + " doc.name: " + doc.Name + " WindowTitle: " + GetActiveWindowTitle());
        }

        /// <summary>
        /// When a window is resized, including when using the 'minimize' button, the logging doc is no longer active.
        /// This method ensures that the inputlog document resumes the logging when it is back in focus.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="win"></param>
        private void WindowsSized(Document doc, Window win)
        {
            if (!Active)
            {
                WindowActivated(doc, win);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;

            try
            {
                // We should not intercept the next save, the user should already
                // been asked to save if there were outstanding changes and will have
                // saved so if he wanted to
                _interceptSave = false;
                if (Document != null)
                {
                    try
                    {
                        Document.Save();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (disposing)
                {
                    Word.Application.Visible = false;
                    Word.DocumentBeforeSave -= BeforeSave;
                    Word.WindowBeforeRightClick -= BeforeRightClick;
                    Word.WindowActivate -= WindowActivated;
                    Word.WindowDeactivate -= WindowDeactivated;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Save the logged document.
        /// </summary>
        public void Save()
        {
            bool saveAsUi = false;
            bool cancel = false;
            BeforeSave(Document, ref saveAsUi, ref cancel);
        }

        /// <summary>
        /// Constructor, opens a the document pointed to by path in Word and disables
        /// some of Words features. If the document does not yet exist, it is created first.
        /// </summary>
        /// <param name="source">The path to the document to open.</param>
        /// <param name="destination">If not null, every time the document is saved,
        /// the document will also be copied to this path.</param>
        /// <returns>The opened document.</returns>
        public static LoggedDocument Open(string source, string destination = null)
        {
            try
            {
                if (!File.Exists(source))
                {
                    var doc = new Document();
                    doc.SaveAs(source, FORMAT);
                    doc.Close();
                }
            }
            catch (Exception e)
            {
                throw new PluginException("Unable to create new document.", e);
            }
            LoggedDocPath = source;
            return new LoggedDocument(source, destination);
        }
    }
}