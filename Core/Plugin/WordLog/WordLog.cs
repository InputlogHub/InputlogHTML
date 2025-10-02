using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using InputLog.Core.Events;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Hooks.Keyboard;
using InputLog.Core.Plugin.WordLog.Keys;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using Microsoft.Office.Interop.Word;
using static InputLog.Core.Plugin.WinLog.FocusLogger;
using Wrd = Microsoft.Office.Interop.Word;
using Replacement = InputLog.Core.Events.WordLog.Replacement;
using CoreSettings = InputLog.Core.Util.Settings;

namespace InputLog.Core.Plugin.WordLog
{
   /// <summary>
    /// This is the WordLog plugin, enables logging extra data from Word
    /// (position, markup, ...).
    /// </summary>
    [Serializable]
    public class WordLog : AbstractPlugin
    {
        #region Fields

        /// <summary>
        /// This will convert the pressed key into its Unicode value
        /// (taking previously pressed keys into account).
        /// </summary>
        private readonly KeyConverter _converter = new KeyConverter();

        /// <summary>
        /// The LoggedDocument, contains the opened (and logged) Word-Document and -Application
        /// and provides some methods/properties to access it.
        /// </summary>
        private static LoggedDocument _document;

        /// <summary>
        /// The previous document length.
        /// </summary>
        private int _oldLength;

        /// <summary>
        /// The last selected range.
        /// </summary>
        private TextRange _oldRange;

        /// <summary>
        /// Counter keeping track of the current position in the document.
        /// </summary>
        private int _position;

        /// <summary>
        /// Path to the template document. Mind: this is not the reporting template, used to build a user report,
        /// but an existing document
        /// This document will be copied to the path given by WorkPath and the resulting
        /// document will be opened and edited. (So the source document acts as a template.)
        /// If UpdateTemplate equals false, this document will not be altered and will solely
        /// act as a template, if it equals true, this document will be updated every time
        /// the opened document is saved.
        /// </summary>
        public string Template
        {
            private get { return _thisTemplate; }
            set
            {
                ChangeAllowed(); // Throw exception if needed
                _thisTemplate = value;
            }
        }

        private string _thisTemplate = "";

        /// <summary>
        /// Path to where the opened document may be saved.
        /// </summary>
        public string WorkPath
        {
            get => _thisWorkpath;
            set
            {
                ChangeAllowed(); // Throw exception if needed
                _thisWorkpath = value;
            }
        }

        private string _thisWorkpath = "";

        /// <summary>
        /// True if the template document should be updated after the editing has finished.
        /// UpdateTemplate should not be false if Template equals WorkPath or
        /// the WordLog plugin will not start properly.
        /// </summary>
        public bool UpdateTemplate
        {
            private get { return _updateThisTemplate; }
            set
            {
                ChangeAllowed(); // Throw exception if needed
                _updateThisTemplate = value;
            }
        }

        private bool _updateThisTemplate;

        /// <summary>
        /// True if the logged document has outstanding changes that are
        /// not yet saved, false if not.
        /// </summary>
        public bool LoggedDocHasUnsavedChanges => !_document.Document.Saved;

        /// <summary>
        /// The title of the Inputlog main document.
        /// </summary>
        public static string MainDocTitle { get; private set; }

        /// <summary>
        /// Path where the original version of the opened document is copied to
        /// (or an empty file is created if no file was opened but a new one was created).
        /// </summary>
        public string OriginalDocPath { get; private set; }

        public event EventHandler<EventArgs> WordCloseAttempt;

        /// <summary>
        /// Text statistics variables from the Word document before the actual logging starts, 
        /// capturing the word and sentence count of the template or the reused document.
        /// When the document is a new empty one, these variables will be empty.
        /// </summary>
        private Statistics _templateStats;

        /// <summary>
        /// A timer to save versions of the current document at timed intervals. 
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// Counter of the #versions
        /// </summary>
        private int _nDocVersion;

        /// <summary>
        /// True if a keypress user interval is expected.
        /// </summary>
        private bool _isUserActionInterval;
        /// <summary>
        /// Placeholder for missing refs
        /// </summary>
        private static object _missing;
        #endregion

        /// <summary>
        /// Launches the WordLog plugin, opening the document and logging the user input.
        /// </summary>
        /// <exception cref="PluginException">If dstDoc equals srcDoc and updateSrc equals false,
        /// if the destination document could not be created or some unknown exception occurred.</exception>
        protected override void Start()
        {
            if (!string.IsNullOrWhiteSpace(Template) && Template.Equals(WorkPath) && !UpdateTemplate)
            {
                throw new PluginException("Source equals Destination (" + Template + ") and UpdateSource is false.");
            }

            try
            {
                // If there is a template, copy it to the work path so LoggedDocument will find it as its source.
                OriginalDocPath = Path.Combine(Path.GetDirectoryName(WorkPath),
                    Path.GetFileNameWithoutExtension(WorkPath)
                    + "_original" + Path.GetExtension(WorkPath));

                if (!string.IsNullOrWhiteSpace(Template) && File.Exists(Template))
                {
                    File.Copy(Template, WorkPath, true);

                    // Also copy it as the original document
                    File.Copy(Template, OriginalDocPath, true);
                }
                else
                {
                    File.Create(OriginalDocPath).Close();
                }
            }
            catch (Exception e)
            {
                throw new PluginException("Unable to copy template to working directory.", e);
            }

            try
            {
                // Creating the logged document, starting word etc.
                _document = UpdateTemplate ? LoggedDocument.Open(WorkPath, Template) : LoggedDocument.Open(WorkPath);
                _document.WordCloseAttempt += delegate { OnWordCloseAttempt(EventArgs.Empty); };

                // Storing the Word statistics of the start document before any new user activity. 
                // The different variables might be empty.
                _templateStats = new Statistics(_document.Document.Content);

                // Ensuring that we recognize this template as a MainDocument.
                var fullDocName = Path.GetFileName(_document.Document.FullName);
                MainDocTitle = fullDocName;
                FocusMainDoc = fullDocName;

                // Logging of selection changes (and disabling discontinuous selections)
                _document.Word.WindowSelectionChange += SelectionChanged;
                // Adjusting current position & document length
                _position = _document.Position;
                _oldLength = _document.Length;

                // Logging of keystrokes
                SysLog.KeyboardEvent += KeyboardHook;

                if (CoreSettings.TimebasedIntervalSave > 0)
                {
                    _isUserActionInterval = false;
                    IntervalSave(CoreSettings.TimebasedIntervalSave);
                }
                // The keypress that triggers the creation of a version of the current document
                if (!CoreSettings.UserActionIntervalSave.IsNullOrEmpty())
                {
                    _isUserActionInterval = true;                  
                }
            }
            catch (Exception e)
            {
                _isUserActionInterval = false;
                throw new PluginException("Unknown exception caught.", e);
            }
        }

        /// <summary>
        /// Terminates the Word plugin.
        /// </summary>
        /// <exception cref="PluginException">If editing the source document is enabled and
        /// updating the source document failed.</exception>
        protected override void Stop()
        {
            AuthorComment comment = null;
            if (CoreSettings.AddComment)
            {
                string localInputText = "";
                if (GetAuthorComment("Author Comment", "Please, enter your comment. Max 250 char.", ref localInputText))
                {
                    comment = new AuthorComment(localInputText);
                }
            }

            // Log word count etc
            var alias = SysLog.StartEvent(EventType.STATISTICS);
            var alias2 = SysLog.StartEvent(EventType.DOCPATH);
            var alias3 = SysLog.StartEvent(EventType.AUTHORCOMMENT);

            try
            {
                if (_document != null)
                {
                    SysLog.Write(new Statistics(_document.Document.Content, _templateStats), alias);
                    //Adding the workPath (to the final document)
                    //SysLog.Write(new DocPath(workpath), alias2);
                }
                if (comment != null)
                {
                    SysLog.Write(comment, alias3);
                }
            }
            finally
            {
                StopIntervalTimer();
                SysLog.EndEvent(alias);
                SysLog.EndEvent(alias2);
                SysLog.EndEvent(alias3);
                _isUserActionInterval = false;
                // Deregister placed hooks
                SysLog.KeyboardEvent -= KeyboardHook;
                if (_document != null)
                {
                    _document.Word.WindowSelectionChange -= SelectionChanged;

                    // Dispose LoggedDocument
                    _document.Close();
                }
            }
        }

        /// <summary>
        /// Saving a copy of the current logged document. Two possibilities: one based on a timed interval 
        /// in minutes. When the interval value is > 0 a copy is saved repeatedly for as long as the recording runs 
        /// at timed intervals. Second option with _IsUserActionInterval 'true', a user action is expected
        /// and the document copy is saved at once based on a user action, i.e. typing a specific key 
        /// such as 'esc' (escape).
        /// </summary>
        /// <param name="interval">interval length in minutes</param>
        private void IntervalSave(ulong interval)
        {
            try
            {
                if (_isUserActionInterval)
                {
                    // Making a version based on an user action
                    CreateVersion("_Action_#", ++_nDocVersion);
                }
                else if (interval > 0)
                {
                    _timer = new System.Timers.Timer();
                    StartIntervalTimer(interval);
                }
            }
            catch (Exception e)
            {
                throw new PluginException("Unable to save an interval copy to the working directory.", e);
            }
        }

        /// <summary>
        /// Every x minutes the timer saves a copy of the current document
        /// for as long as the recording session runs.
        /// </summary>
        /// <param name="interval">the writing interval in minutes</param>
        private void StartIntervalTimer(ulong interval)
        {
            //Converts minutes to millisecs.
            var thisInterval = interval*60000; // minutes
            _timer.Interval = thisInterval;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Elapsed event handler is called after each interval and the document is saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Making a timed version 
           CreateVersion("_Timed_#", ++_nDocVersion);
        }

        /// <summary>
        /// Method to create a version of the current document during the writing process.
        /// The trigger to save a version is either a timed interval, or a specific key press. 
        /// </summary>
        /// <param name="versionType">Timed or user Action (keypress)</param>
        /// <param name="version">number of the version</param>
        private static void CreateVersion(string versionType, int version)
        {
            var wordApp = new Wrd.Application
            {
                Visible = false,
                DisplayAlerts = WdAlertLevel.wdAlertsNone
            };
            Document newWordDocument = null;
            try
            {              
                string docName = _document.Document.FullName;
                string intervalDocPath = Path.Combine(Path.GetDirectoryName(docName),
                    Path.GetFileNameWithoutExtension(docName)
                    + versionType + version + Path.GetExtension(docName));

                newWordDocument = wordApp.Documents.Add();                
                newWordDocument.Content.SetRange(0, 0);             
                newWordDocument.Content.Text = _document.Document.Content.Text;       
                newWordDocument.SaveAs(intervalDocPath);
            }
            catch (COMException ce)
            {
                throw new COMException("Saving an interval document failed, ", ce);
            }
            finally
            {
                if (newWordDocument != null)
                {
                    newWordDocument.Close(ref _missing, ref _missing, ref _missing);
                    wordApp.Quit(ref _missing, ref _missing, ref _missing);
                    Marshal.FinalReleaseComObject(newWordDocument);
                }
            }
        }

        /// <summary>
        /// When recording stops, this timer needs to stop too.
        /// </summary>
        private void StopIntervalTimer()
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Callback for keyboard eventz.
        /// </summary>
        /// <param name="sender">The originater of the event.</param>
        /// <param name="e">The data of the event, mostly a wrapper around the information provided by Windows.</param>
        /// <param name="unicode">The unicode representation of the key (if there is one), taking into account
        /// previously pressed dead keys. (A maximum of one dead key is taken into account, ie chaining
        /// of dead keys is not supported.)</param>
        /// <param name="alias">The alias used for referencing the event in the Log.</param>
        /// <param name="kbState">Enumeration containing all the keyboard buttons currently pressed - excluding the button
        /// which lead to the callback.</param>
        private void KeyboardHook(object sender, KeyboardEvent e, string unicode, string alias,
            ICollection<KeysEx> kbState)
        {
            try
            {
                // 1/04/2013 - Hack temporarily disabled as this causes Inputlog to not function
                // correctly with the Office 2013 version of Word.
                //
                // HACK FOR TWITTER VERSION:
                // CHECK DOCUMENT ACTIVITY BASED ON WINDOWS FOCUS EVENTS
                // THIS MAY HAVE INCORRECT RESULTS WHEN SAVING/USING OTHER WORD WINDOWS
                // OR SIMPLY WHEN TWO DOCUMENTS WITH THE SAME NAME ARE OPEN AT THE SAME TIME.

                //if (!_document.Active)
                //{
                //    Console.WriteLine("** KEY Doc NOT Active **");
                //    return;
                //}
                //else
                //{
                //    Console.WriteLine("** KEY Doc Active **");
                //}

                lock (_document)
                {
                    // Only process on keydown events
                    if (_document.Active && (e.Type == KeyboardMessages.WM_KEYDOWN || e.Type == KeyboardMessages.WM_SYSKEYDOWN))
                    {
                        // the leaveIt boolean determines whether or not an event should be replayed.
                        // Don't replay a backspace at the beginning of the document, this is an 
                        // action that may be ignored.
                        bool leaveIt = DontReplayThisKeyboardEvent(e, kbState) || e.Key == KeysEx.VK_BACK && _position == 0;

                        // Saving a version of the current document based on an user action, pressing the esc-key in this case.
                        // Other possibilities such as CTRL-P, CTRL-C or CTRL-V are not activated at this time.
                        //|| (e.Key == KeysEx.VK_LCONTROL || e.Key == KeysEx.VK_RCONTRO) &&
                        //  (KeyPress.KeyboardState.Contains(KeysEx.VK_C) || KeyPress.KeyboardState.Contains(KeysEx.VK_P))) 
                        try
                        {
                            if (_isUserActionInterval && e.Key == KeysEx.VK_ESCAPE)
                            {
                                IntervalSave(0);
                            }
                        }
                        catch (Exception)
                        {                         
                           // ignore
                        }

                        SysLog.Write(new Keypress(_position, _oldLength, !leaveIt), alias);
                        //  Debug.WriteLine("Keypress: {0} - {1}: \"{2}\"", position, leaveIt, unicode);

                        // Only update position if it will not intefere with a SelectionChanged event.
                        // (In particular, do not update if (ctrl +) del or ctrl + backspace is pressed
                        // or if there is a current selection. This way, we will not update the length
                        // etc. when e.g. the A key is pressed while there was a selection. As such, we
                        // leave it up to the SelectionChanged event to handle it.)
                        if (leaveIt) return;
                        if (e.Key == KeysEx.VK_BACK)
                        {
                            // Whenever backspace is pressed, resync with Word.
                            // The maximum is needed for the special case that the cursor
                            // position already is 0 so that it could not be set to -1.

                            // 20120510 Arbitrarily reduce the length of the document and the position
                            // in the document by one, when backspace is pressed. This is done because 
                            // when backspace is held, the syncing of the position with word becomes incorrect
                            // due to the high response time required for correct syncing.
                            //Position = Math.Max(Document.Position, 0);
                            //OldLength = Document.Length;

                            _position = Math.Max(_position - 1, 0);
                            _oldLength = _oldLength - 1;
                        }
                        else
                        {
                            // Update position with the length of the unicode string as
                            // this is the string Word will print on screen
                            _position += unicode.Length;

                            // Same for document length
                            _oldLength += unicode.Length;
                        }
                    }
                }
            }
            finally
            {
                // EndEvent needs to be called on all events
                SysLog.EndEvent(alias);
            }
        }

        /// <summary>
        /// Returns true if the given keyboard event should be left for the
        /// SelectionChanged event to handle the document changes, false if not. 
		/// The same applies for keys that have no replay value, e.g. pressing SHIFT
		/// without producing any output is not relevant for replaying, thus the replay
		/// for this key is FALSE.
        /// </summary>
        /// <param name="e">The keyboard event.</param>
        /// <param name="kbState">The keyboard state.</param>
        /// <returns>True if the given keyboard event has replay value and if the key is not handled by
		/// the selection changed event.</returns>
        private bool DontReplayThisKeyboardEvent(KeyboardEvent e, ICollection<KeysEx> kbState)
        {
			var state = new HashSet<KeysEx>(kbState);
			return (_oldRange != null && _oldRange.Start != _oldRange.End) // There is a selection
				   || (e.Key == KeysEx.VK_DELETE) // It was delete that was pressed
				   // It is a key that moves the cursor or has no direct output. 
                   // It is a 'navigation key'. This includes CTRL+Backspace
				   || NavigationKey.isNavigation(e.Key, _converter.Convert(e.Key, state), kbState);
        }

        /// <summary>
        /// Callback when the selection of the document has changed.
        /// This methods shrinks that selection to a contiguous one and
        /// logs the necessary data.
        /// </summary>
        /// <param name="sel">Contains the new Selection.</param>
        private void SelectionChanged(Selection sel)
        {
            try
            {
                // 1/04/2013 - Currently back to the old way of determining if a document is active or not
                // as the DocumentIsActive() way causes Inputlog to not function with the newer version
                // of Office Word - 2013
                //
                // Console.WriteLine(String.Format("Selection changed: {0}-{1} | {2}-{3} {4}",
                // sel.Start, sel.nd, sel.Characters.First.Start, sel.Characters.Last.Start,
                // sel.Characters.Last.End));
                //if (!_document.Active)
                //{
                //    Console.WriteLine("** SELECTION Doc NOT Active **");
                //    return;
                //}
                //else
                //{
                //    Console.WriteLine("** SELECTION Doc Active **");
                //}
                lock (_document)
                {
                    // If a user double clicks in Word, Word selects the word (with the trailing space),
                    // but if the user then overwrites this selection by pressing a (character) key,
                    // word does not replace this space. However, if the user manually selected a word
                    // with the trailing space and then presses a character button, the trailing space
                    // will also be removed/replaced by the character. Wordlog cannot detect the difference
                    // between the former and the latter situation and thus the Revision analysis (and hence
                    // Replay etc) is erroneous in those cases. Word does the same when selecting more than
                    // one word (by clicking and dragging).
                    // The following checks if the selection contains exactly words and if so, it forces Word
                    // to select all of the words together with their trailing spaces and thereby forcing it behave
                    // as the latter case (i.e. if the user now presses a key to overwrite the selection, the
                    // trailing space will now also be removed/replaced in all cases).
                    if (_document.Active && !string.IsNullOrWhiteSpace(sel.Text) && sel.Text.Length > 1)
                    {
                        // Make it a contiguous selection
                        try
                        {
                            sel.ShrinkDiscontiguousSelection();
                        }
                        catch (Exception)
                        {
                            // when using Stava Rex (Swedish spelling program) or perhaps in other cases, 
                            // this will throw an exception. We assume that the selection is contiguous in those cases
                        }
                        // Exception that is needed when placing the cursor before the last character.
                        if (sel.Start != _document.Length - 1 && sel.End != _document.Length - 1)
                        {
                            var buf = new StringBuilder(sel.Text.Length);
                            for (var i = 1; i <= sel.Words.Count; i++)
                            {
                                var wrd = sel.Words[i];
                                buf.Append(wrd.Text);
                            }
                            if (sel.Text.Equals(buf.ToString()))
                            {
                                try
                                {
                                    sel.Expand(WdUnits.wdWord);
                                }
                                catch (Exception e)
                                {
                                    string s = "Please copy the following text ...";
                                    s += "||selt:" + sel.Text;
                                    s += "||buf:" + buf;
                                    s += "||sell:" + sel.Text.Length;
                                    s += "||selwc:" + sel.Words.Count;
                                    s += "||sels:" + sel.Start;
                                    s += "||sele:" + sel.End;
                                    s += "||exc:" + e;
                                    if (e.InnerException != null)
                                    {
                                        s += "||iex:" + e.InnerException;
                                    }
                                }
                            }
                        }
                    }

                    // Word preferences us to use ranges wherever we could, so here we go...
                    var newRange = InputlogDocument.Range(sel.Range);

                    // First log possible changes connected with the selection change
                    if(_document.Active) LogRange(newRange);

                    // Log the change so the position is certain to be correct in the replay
                    var alias = SysLog.StartEvent(EventType.SELECTION);
                    SysLog.Write(new SelectionChange(newRange.Start, newRange.End), alias);
                    SysLog.EndEvent(alias);

                    // Update the cursor position
                    _position = newRange.Start;

                    // Update the old range
                    _oldRange = newRange;

                    // HACK. 'Document.Length' was used in the original code. However, when copy/pasting from 
                    // a web page, there was a difference between Document.End and Document.Length, resulting
                    // in an incomplete rendering of the copied text. 
                    // This hack produces an out-of-range error in the Revision analysis when the copied
                    // text from a web page contains embedded links.

                    // Update the length
                    _oldLength = _document.End; // Length; // length of a doc == last pos of the doc
                }
            }
            catch (Exception e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }
        }

        /// <summary>
        /// Checks whether the selection changed to the given range was because of a
        /// modification was made in the text and if so, logs it.
        /// </summary>
        /// <param name="newRange">The range representing the new selection.</param>
        private void LogRange(TextRange newRange)
        {
            // HACK. 'Document.Length' was used in the original code. However, when copy/pasting from 
            // a web page, there was a difference between Document.End and Document.Length, resulting
            // in an incomplete rendering of the copied text. This hack produces an out-of-range
            // error in the Revision analysis when the copied text from a web page contains embedded links.

           // var delta = Document.Length - OldLength; 
            var delta = _document.End - _oldLength; // Change in document length
            //Console.WriteLine(" " + Document.End + " " + OldLength + " " + newRange.Start 
            // + " " + newRange.End + " " + newRange.Text);
            if (newRange.Start != newRange.End)
            {
                // Either multiple characters inserted due to an undo action
                // or the given range is a new selection.
                // In case of an undo, the given range is the text that was inserted
                // and the textrange that has been replaced is given by
                // [newRange.Start, newRange.End - delta]
                // There are 3 cases which we can distinguish:
                //	1)	If the document length has changed, it was clearly an edit and thus
                //		it was an undo action;
                //	2)	If the document length has not changed, it is possible that it still
                //		was an undo action that replaced a range delText of x characters
                //		by the range newRange (also of length x).
                //		In this case, clearly delText == [newRange.Start, newRange.End]
                //		(which is equal to [newRange.Start - delta, newRange.End] as delta = 0)
                //		and newRange.Text != delText.Text;
                //	3)	In all other cases, it was just an ordinary selection that was made.
                // Unfortunately, we cannot check whether delText.Text != newRange.Text
                // as that would require to have access to the previous version of the Document
                // (which we do not keep due to efficiency reasons).
                // However, if we would always log this kind of selection as a change,
                // worst case we would log a replacement of delText by newRange and both
                // would contain exactly the same text. As such, this would cause a little
                // waste of resources, but it would not introduce incorrect behaviour!

                // log [newRange.Start, newRange.End - delta] as replaced by newRange
                LogReplacement(new Replacement(newRange.Start, Math.Min(_document.End, newRange.End - delta), newRange.Text));
                Debug.WriteLine("Replacement: {0}-{1}: \"{2}\"", newRange.Start, newRange.End - delta, newRange.Text);
            }
            else
            {
                // Either the cursor position has changed because the user intended it to
                // change it by using a mouse click, hitting the home key, ... or the cursor
                // position has changed due to an undo, a paste, a cut or a delete.
                // We can distinguish between these cases by taking the previous selection into
                // account and here's how:
                //	a)	There was no real previous selection (oldRange.Start == oldRange.End).
                //		Suppose delta = newDocLength - oldDocLength, now we have 3 possibilities:
                //	 1)	delta > 0
                //		Text was added. The added text is contained in the range
                //		[newRange.Start - delta, newRange.Start] and was inserted in the previous
                //		version of the Document at position newRange.Start - delta.
                //	 2)	delta < 0
                //		Text was removed. The get to the current version of the Document,
                //		the text contained by the range [newRange.Start, newRange.Start - delta]
                //		was removed from the previous version of the Document.
                //	 3)	delta = 0
                //		No text was added or deleted, it was a repositioning of the cursor.
                //	b)	A range of characters was selected right until this event changed the selection.
                //		If we do not take a possible undo action into account, we have the same three
                //		choices as with a), but know we need to take into account that inserted text may
                //		override the selected text and thereby can decrease the Document length
                //		(ie, delta = newDocLength - oldDocLength < 0) if the text range pasted over the
                //		previous selection (oldRange) contains less character than it. It could also keep
                //		the document length unmodified if the length of the inserted text equals the
                //		length of the selected text, or increase the document length if its length is
                //		greater than the length of the selection.
                //		We can distinguish between these cases using the following checks:
                //	 1)	delta > 0:
                //			There was text inserted over oldRange and that new text can be found in the
                //			range [oldRange.Start, newRange.Start] (again, newRange.Start == newRange.End).
                //	 2)	delta < 0:
                //			Either new text was inserted over the selection and just like delta > 0,
                //			this text is contained by the range [oldRange.Start, newRange.Start] or
                //			the selection was only deleted (and no new text was added). The latter can be
                //			distinguished by checking if newRange.Start == oldRange.Start.
                //			Note: if we would say that in case of delta < 0, the new text is contained by
                //			[oldRange.Start, newRange.Start], this would always be correct as if no new
                //			text is inserted, oldRange.Start == newRange.Start and would the aforementioned
                //			range be empty.
                //	 3)	delta = 0:
                //			Either the text inserted over the selection has the same length as the selection,
                //			or nothing has changed. We can distinguish the former from the latter by checking
                //			if the characters in [oldRange.Start, oldRange.End] have changed. (Note that
                //			in this case, oldRange.End == newRange.Start == newRange.End.)
                //		Note that the inserted text always can be represented by [oldRange.Start, newRange.Start].
                //
                //		If we however do take undo into account, it is possible that text is deleted or inserted
                //		and that the previous selection is not altered. This means that logging the text contained
                //		by [oldRange.Start, newRange.Start] as inserted over the old selection would be incorrect.
                //		The way to handle this is explained in the code below.

                if (delta > 0)
                {
                    // We're sure there was some text inserted
                    // This is either text pasted over the selection or text inserted elsewhere using undo.
                    // If there was no previous selection or newRange.Start - delta != oldRange.End, we are
                    // certain that this is not a text pasted over oldRange but an insertion due to an undo action.
					// (28/6/2012: A paste of copied text also uses this branch of LogRange)

                    // There was no previous selection
                    if (_oldRange == null
                        || _oldRange.Start == _oldRange.End
                        || newRange.Start - delta != _oldRange.End)
                    {
                        // As it is an undo action, we know that the last action (not considering the undo)
                        // was the deletion of the text that undo inserts back again => no changes can be made
                        // to the OldRange (otherwise, undo would've undone those changes and not inserted the
                        // text elsewhere) => we only need to log the inserted text and not the previous selection

                        // We are not certain that the inserted text is in [newRange.Start - delta, newRange.Start];
                        // if the user used ctrl + delete, the inserted text is in [newRange.Start, newRange.Start +
                        // delta] (ie after the current position instead of before).
                        // Because we cannot check this, we log the range [newRange.Start - delta, newRange.Start +
                        // delta] and postpone the decision to the revision analysis.
                        var before = _document.Range(newRange.Start - delta, newRange.Start);
                        var after = _document.Range(newRange.Start, newRange.Start + delta);
                        LogInsert(new Insert(newRange.Start, before.Text, after.Text));
                        Debug.WriteLine("Insertion: {0}: \"{1}|{2}\"", newRange.Start, before.Text, after.Text);
                    }
                    else
                    {
                        // It is possible that the inserted text is pasted over oldRange but it is also possible
                        // that the inserted text is inserted due to an undo action.
                        // As a consequence, we check whether the text in oldRange has changed and if so, we log
                        // the replacement of oldRange with the text now in [oldRange.Start, newRange.Start],
                        // if not, we only log the insertion of [oldRange.End, newRange.Start] at oldRange.End.

                        var text = _document.Range(_oldRange.Start, _oldRange.End);
                        if (_oldRange.HasSameContentAs(text))
                        {
                            // Selection has not changed, it was an insertion after it
                            text = _document.Range(_oldRange.End, newRange.Start);
                            LogReplacement(new Replacement(text.Start, Math.Min(_document.End, text.Start), text.Text));
                            Debug.WriteLine("Insertion: {0}: \"{1}\"", text.Start, text.Text);
                        }
                        else
                        {
                            // Selection has changed
                            text = _document.Range(_oldRange.Start, newRange.Start);
                            LogReplacement(new Replacement(_oldRange.Start, Math.Min(_document.End, _oldRange.End), text.Text));
                            Debug.WriteLine("Replacement: {0}-{1}: \"{2}\"", _oldRange.Start, _oldRange.End,
                                            text.Text);
                        }
                    }
                }
                else if (delta < 0)
                {
                    // We're sure some text was deleted
                    // This was either the deletion of the selected text, the pasting of a smaller text over it
                    // or the deletion of some arbitrary other text due to an undo action.
                    // If there wasn't a previous selection or newRange.Start - delta != oldRange.end,
                    // we are certain that this is the deletion of some text due to an undo action.

                    // There was no previous selection
                    if (_oldRange == null
                        || _oldRange.Start == _oldRange.End
                        || newRange.Start - delta != _oldRange.End)
                    {
                        // As it is an undo action, we know that the last action (not considering the undo)
                        // was the insertion of the text that undo deletes => no changes can be made
                        // to the OldRange (otherwise, undo would've undone those changes and not deleted the
                        // text elsewhere) => We only need to log the deleted text and not the previous selection
                        // Another possibility is that VK_DELETE was pressed with no selection, in which case taken
                        // actions are also correct.

                        LogReplacement(new Replacement(newRange.Start, newRange.Start - delta));
                        Debug.WriteLine("Deletion: {0}-{1}", newRange.Start, Math.Min(_document.End, newRange.Start - delta));
                    }
                    else
                    {
                        // It is possible that selected text was replaced by some new text but it is also possible
                        // that some text was deleted due to an undo action.
                        // We could check to see if the content of the selection has changed, but it is as easy
                        // and efficient just to log thinking it has (and it is not incorrect to do so).

                        var text = _document.Range(_oldRange.Start, newRange.Start);
                        LogReplacement(new Replacement(_oldRange.Start, Math.Min(_document.End, _oldRange.End), text.Text));
                        Debug.WriteLine("Replacement: {0}-{1}: \"{2}\"", _oldRange.Start,
                                        _oldRange.End, text.Text);
                    }
                }
                else
                {
                    // The document length has not changed, but it is possible that the selected text was
                    // replaced by a text of an equal length => We check whether the selected
                    // text has been changed and if so, we log it.
                    if (_oldRange == null || _oldRange.Start == _oldRange.End) return;
                    // There was a selection
                    var text = _document.Range(_oldRange.Start, _oldRange.End);

                    if (_oldRange.HasSameContentAs(text)) return;
                    LogReplacement(new Replacement(text.Start, Math.Min(_document.End, text.End), text.Text));
                    Debug.WriteLine("Replacement: {0}-{1}: \"{2}\"", text.Start, text.End, text.Text);
                }
            }
        }

        /// <summary>
        /// Logs the given replacement with the SystemLogger.
        /// </summary>
        /// <param name="replacement">The replacement to log.</param>
        private void LogReplacement(Replacement replacement)
        {
            var alias = SysLog.StartEvent(EventType.REPLACEMENT);

            try
            {
                SysLog.Write(replacement, alias);
            }
            finally
            {
                SysLog.EndEvent(alias);
            }
        }

        /// <summary>
        /// Logs the given insert with the SystemLogger.
        /// </summary>
        private void LogInsert(IEventPart insert)
        {
            var alias = SysLog.StartEvent(EventType.INSERT);

            try
            {
                SysLog.Write(insert, alias);
            }
            finally
            {
                SysLog.EndEvent(alias);
            }
        }

        /// <summary>
        /// Save the logged document.
        /// </summary>
        public void Save()
        {
            _document.Save();
        }

        private void OnWordCloseAttempt(EventArgs e)
        {
            // Note the use of a temporary variable here to make the event raisin
            // thread-safe; may or may not be necessary in your case.
            var evt = WordCloseAttempt;
            evt?.Invoke(this, e);
        }

        /// <summary>
        /// Dialog to capture a comment from the author of this writing session.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="prompt"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool GetAuthorComment(string caption, string prompt, ref string value)
        {
            Form commentInput = new Form();
            Label labelPrmt = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            commentInput.Text = caption;
            labelPrmt.Text = prompt;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            labelPrmt.SetBounds(21, 20, 372, 13);
            textBox.SetBounds(25, 36, 255, 200);
            textBox.AutoSize = false;
            textBox.Size = new Size(250, 150);
            textBox.MaxLength = 250;
            textBox.WordWrap = true;
            textBox.Multiline = true;
            buttonOk.SetBounds(25, 200, 75, 23);
            buttonCancel.SetBounds(105, 200, 75, 23);

            labelPrmt.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            commentInput.ClientSize = new Size(300, 300);
            commentInput.Controls.AddRange(new Control[] { labelPrmt, textBox, buttonOk, buttonCancel });
            commentInput.ClientSize = new Size(Math.Max(300, labelPrmt.Right + 10), commentInput.ClientSize.Height);
            commentInput.FormBorderStyle = FormBorderStyle.FixedDialog;
            commentInput.StartPosition = FormStartPosition.CenterScreen;
            commentInput.MinimizeBox = false;
            commentInput.MaximizeBox = false;
            commentInput.AcceptButton = buttonOk;
            commentInput.CancelButton = buttonCancel;

            if (commentInput.ShowDialog() == DialogResult.OK)
            {
                value = textBox.Text;
                return true;
            }
            commentInput.Dispose();
            return false;
        }
    }
}