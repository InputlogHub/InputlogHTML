using System;
using System.IO;
using System.Runtime.InteropServices;
using InputLog.Core.Util;
using Wrd = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Text;

namespace InputLog.Core.Plugin.WordLog
{
    /// <summary>
    /// Aggregates a Word Document and a Word Application and provides some
    /// properties and methods to access them.
    /// </summary>
    public class InputlogDocument : IDisposable
    {
        #region Fields
        /// <summary>
        /// The actual word application.
        /// </summary>
        public Wrd.Application Word { get; }

        /// <summary>
        /// The actual document.
        /// </summary>
        public Wrd.Document Document { get; }

        /// <summary>
        /// Returns the current cursor position in the active window of
        /// the document.
        /// </summary>
        public int Position => Document.ActiveWindow.Selection.Range.Characters.First.Start;

        /// <summary>
        /// Returns the position of the last character in the document.
        /// </summary>
        public int End => Document.Content.Characters.Last.End;

        /// <summary>
        /// Returns the length of the Document.
        /// </summary>
        public int Length => Document.Content.Text.Length;

        /// <summary>
        /// Debug property to show the contents of the entire document
        /// </summary>
        public string FullDocument => Range(0, End).Text;

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Returns the selection of the active window.
        /// </summary>
        public Wrd.Selection Selection => Document.ActiveWindow.Selection;

        /// <summary>
        /// Final path of the logged document.
        /// </summary>
        public static string LoggedDocPath { get; protected set; }
        /// <summary>
        /// Makes a doc invisible when a revision is made.
        /// </summary>
        public static bool IsRevision { get; set; }

        /// <summary>
        /// When opening a document to recreate a logging, the start position as
        /// reported in the idfx may be larger than 0, e.g. when an image or
        /// other object stands before the start of the writing space.
        /// </summary>
        private int StartPositionOffset { get;  set; }

        /// <summary>
        /// The Word default document file format. As from Microsoft Office Word 2007 on this is the *.docx format.
        /// </summary>
        private const Wrd.WdSaveFormat FORMAT = Wrd.WdSaveFormat.wdFormatDocumentDefault;

        /// <summary>
        /// Using Microsoft InteropServices to define the window in focus.
        /// </summary>
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        #endregion

        /// <summary>
        /// Creates a Document out of an existing (and opened) Word Document.
        /// </summary>
        /// <param name="doc">The Word Document.</param>
        public InputlogDocument(Wrd.Document doc)
        {
            Disposed = false;

            try
            {
                Document = doc;
                Word = Document.Application;
           //     if (IsRevision) isVisible = false;

                // Disable unwanted options only when the document is not protected from editing.
                if (Word.ActiveDocument.ProtectionType.Equals(Microsoft.Office.Interop.Word.WdProtectionType.wdNoProtection))
                {
                    WordDocumentTools.DisableUnwantedOptions(Word);
                }

                Word.DocumentBeforeClose += BeforeClose; // Disable closing of logged document               
              //  Word.Visible = isVisible;

				// Make sure the content in the document ends with \r, regardless of content in there.
				if (FullDocument == null || !FullDocument.EndsWith("\r"))
				{
					Selection.SetRange(End, End);
					Selection.TypeText("\r");
				}
				
            }
            catch (Exception e)
            {
                throw new PluginException("Unknown exception caught.", e);
            }
        }

        public static InputlogDocument Create(object path)
        {
            Wrd.Application app = new Wrd.Application();
            object optional = Missing.Value;
            Wrd.Document doc = app.Documents.Add(ref optional, ref optional, ref optional, ref optional);
            doc.SaveAs(ref path, FORMAT);
            doc.Activate();
            
            return new InputlogDocument(doc);
        }

        /// <summary>
        /// Creates a new InputlogDocument opening the document on the given path.
        /// </summary>
        /// <param name="path">The path to the document to open.</param>
        /// <param name="isRevision">Doc used for revision = true</param>
        public InputlogDocument(string path, bool isRevision)
            : this(string.IsNullOrEmpty(path) || !File.Exists(path)
                ? new Wrd.Document()
                : new Wrd.Application().Documents.Open(path))
        {
            IsRevision = isRevision;
        }

        /// <summary>
        /// Returns a TextRange over Document.
        /// </summary>
        /// <param name="start">The start position of the range.</param>
        /// <param name="end">The end position of the range.</param>
        /// <returns>The created TextRange.</returns>
        public TextRange Range(int start, int end)
        { 
           if (start > 1  && Position == 1)
           {
               StartPositionOffset = start - 3;
           }

           int startPos;
           int endPos;
           if (start < end)
           {
               startPos = start;
               endPos = end;
           }
           else
           {
               startPos = end;
               endPos = start;
           }
           if (StartPositionOffset > 0)
           {
               startPos = startPos - StartPositionOffset;
               endPos = endPos - StartPositionOffset;

           }
           if (startPos == endPos)
           {
               if (endPos < End) endPos++;
               else startPos--;
           }
           if (startPos >= End)
           {
               startPos = End - 1;
           }

           // If we have an endPos of 1 past the 'End' of the document this means we add the carriage return \r
           // in the selection. 
           // We should take into account that the position at start of the logging in a document can be different from '0',
           // e.g. when there is an image of on other object before the typing space. The position in the document could be 100
           // while the document length is only 10. This is relevant when an InputlogDocument instance is created to build a revision.
           // Note that the 'End' property returns the length of the document WITHOUT this carriage return.
           TextRange newRange;
           try
           {
               newRange = new TextRange(Math.Max(0, startPos), Math.Min(endPos, End), Document);
           }
           // Can recover we the right end and start position?
           catch (COMException)
           {
               newRange = new TextRange(Document);
           }

           return newRange;
        }

        /// <summary>
        /// Returns a TextRange over build from the given range..
        /// </summary>
        /// <param name="range">The range where to create a TextRange from.</param>
        /// <returns>The created TextRange.</returns>
        public static TextRange Range(Wrd.Range range)
        {
            return new TextRange(range);
        }

        /// <summary>
        /// Callback when a document is closed, used for canceling the closing of the logged document.
        /// </summary>
        /// <param name="doc">The document that's being closed.</param>
        /// <param name="cancel">Will be false on entrance of the procedure, assign true to cancel.</param>
        protected virtual void BeforeClose(Wrd.Document doc, ref bool cancel)
        {
            try
            {
                if (doc.FullName.Equals(Document.FullName))
                {
                    cancel = true;
                    MessageLogger.LogMessage(this, "Unable to close document",
                        "You can't close a document that is being logged or replayed.",
                        Severity.WARNING);
                }
                else
                {
                    cancel = false;
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object (closes the document).
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            try
            {
                if (disposing)
                {
                    if (Document != null)
                    {
                        Word.DocumentBeforeClose -= BeforeClose;
                    }
                }
            }
            finally
            {
                Disposed = true;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~InputlogDocument()
        {
            Dispose(false);
        }

        /// <summary>
        /// Closes and disposes the Word document.
        /// Word interop - how to close one Word window without closing all of them?
        /// See: https://stackoverflow.com/questions/7712955/word-interop-how-to-close-one-word-window-without-closing-all-of-them
        /// </summary>
        public void Close()
        {
            Dispose();
            Document?.Close(Wrd.WdSaveOptions.wdDoNotSaveChanges);

            // Quit Word if no other documents are open.
            if (Word != null && Word.Documents.Count == 0)
            {
                Word.Quit(Wrd.WdSaveOptions.wdDoNotSaveChanges);
                //Marshal.FinalReleaseComObject(Word);
            }
        }

        /// <summary>
        /// Word interop - how to close one Word window without closing all of them?
        /// See: https://stackoverflow.com/questions/7712955/word-interop-how-to-close-one-word-window-without-closing-all-of-them
        /// </summary>
        public void SaveAndClose()
        {
            Dispose();
            Document?.Close(Wrd.WdSaveOptions.wdSaveChanges);

            // Quit Word if no other documents are open.
            Word?.Quit(Wrd.WdSaveOptions.wdDoNotSaveChanges);
            //  Marshal.FinalReleaseComObject(Word);
        }

        /// <summary>
        /// A function that checks whether or not all characters behind the current
        /// cursor position are whitespace characters or not.
        /// </summary>
        /// <returns>True if the range from Position to End only contains whiteChar, false if not.</returns>
        public bool FollowedByWhiteSpace(int position)
        {
            var followUpText = Range(position, End).Text;
            return string.IsNullOrWhiteSpace(followUpText);
        }

        internal TextRange Range()
        {
            return Range(0, End);
        }
    }
}