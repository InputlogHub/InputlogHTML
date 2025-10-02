using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Plugin.WordLog.Keys;
using InputLog.Core.Preprocessing.Filter;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;
using log4net;
using EventTypeFilter = InputLog.Core.Preprocessing.Filter.EventTypeFilter;
using WrdEvent = InputLog.Core.Events.WordLog;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.Revision.RevisionAnalysis
{
    /// <summary>
    /// </summary>
    public class RevisionAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        ///     Log4Net MessageLogger.
        /// </summary>
        public static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Event types we wish to do the analysis on (all other types are discarded).
        /// </summary>
        private static readonly string[] EventTypes =
            {
                EventType.KEYBOARD, EventType.FOCUS, EventType.MOUSE,
                EventType.INSERT, EventType.REPLACEMENT, EventType.SELECTION, EventType.DRAGONNS
            };

        /// <summary>
        ///     Keystrokes should not be processed while there is a current selection in the document,
        ///     they should always be preceded by a ReplacementEvent.
        ///     This queue is used for buffering keystrokes detected there was a current selection.
        /// </summary>
        /// CHANGED: Edits.TypeChar > Edits.AbstractEdit (When implementing CursorEdits)
        private readonly Queue<AbstractEdit> BufferedKeyStrokes = new Queue<AbstractEdit>();

        /// <summary>
        ///     List of controlKeys that need to be taken into account.
        /// </summary>
        private readonly List<KeysEx> ControlKeys;

        /// <summary>
        ///     The path of the document in the original state (before the logging happened).
        /// </summary>
        private readonly string DocumentPath;

        /// <summary>
        ///     The used pause threshold (how long the pause between 2 events should be before we actually
        ///     mark it as a pause).
        /// </summary>
        private readonly ulong PauseThreshold;

        /// <summary>
        ///     A boolean that is set to true when we encounter a delete or backspace typechar
        ///     without replay. This means that the deletechar was used during a selection. This
        ///     is important as it alters the behaviour of full paragraph deletions. (In the case of a
        ///     full paragraph deletion with a deletechar, the \r at the end of the paragraph is also deleted,
        ///     if it's an overwrite with other text, the \r is not deleted).
        /// </summary>
        private bool FoundNoReplayBackspace;

        /// <summary>
        ///     The original content of the document before all the changes were made to the document.
        /// </summary>
        public string OriginalContent { get; private set; }

        /// <summary>
        ///  String with event ids that cause problems during preprocessing.
        /// </summary>
        public static string BadEventIds { get; private set; }

        #endregion

        /// <summary>
        ///     Constructor.
        ///     Performs a revision analysis.
        /// </summary>
        /// <param name="events">List of events on which to perform the analysis.</param>
        /// <param name="sessionID">Session identification of the event list</param>
        /// <param name="docpath">
        ///     The path of the document in the original state (before the logging happened).
        ///     Leave empty for an empty doc. If the file pointed to by the given path does not exists,
        ///     an empty document will be used.
        /// </param>
        /// <param name="pauseThreshold">
        ///     The used pause threshold (how long the pause between
        ///     2 events should be before we actually mark it as a pause)
        /// </param>
        /// <param name="controlKeys">
        ///     List of controlKeys that need to be taken into account
        ///     (Control keys are displayed differently in the analysis)
        /// </param>
        public RevisionAnalysis(List<Event> events, SessionIdentification sessionID, 
            string docpath = null, ulong pauseThreshold = 0ul, List<KeysEx> controlKeys = null)
            : base("RA", events, sessionID)
        {
            DocumentPath = docpath;
            ControlKeys = controlKeys;
            PauseThreshold = pauseThreshold;

            // Do some more filtering before starting the analysis.
            // This filtering changes the original list of events.
            EventFilter filter = new EventTypeFilter(EventTypes);
            InputEvents = events.FilterWithoutAltering(filter);
        }

        /// <summary>
        ///     Performs the actual Revision Analysis on the recorded events
        /// </summary>
        /// <returns>A summary of the analysis.</returns>
        public override IAnalysisSummary DoAnalysis()
        {
            // Determine some extra info / do some preprocessing
            if (ControlKeys != null)
            {
                RemoveDuplicateControlKeys(InputEvents, ControlKeys);
            }
            // discover double clicks if the threshold > 0
            if (Settings.Analysis.DoubleClickThreshold > 0)
            {
                RecognizeDoubleClicks(InputEvents, Settings.Analysis.DoubleClickThreshold);
            }

            // Determine the pause locations of the events.
            var pauseLocationMarker = new PauseLocationMarker();
            List<Pair<Event, PauseLocation>> eventPauseList = pauseLocationMarker.Start(InputEvents);

            // process actual events (+ write to analysis-document).
            return AnalyzeEvents(eventPauseList);
        }

        /// <summary>
        ///     This method performs the actual analysis on a given list of events (and their pause location).
        /// </summary>
        /// <param name="eventPauseList">List containing pairs of events and their corresponding pause location.</param>
        /// <returns>A summary of the analysis.</returns>
        private RevisionAnalysisSummary AnalyzeEvents(IEnumerable<Pair<Event, PauseLocation>> eventPauseList)
        {
            var summary = new RevisionAnalysisSummary(DocumentPath);
            OriginalContent = summary.Document.FullDocument;
            try
            {
                // looping over all events
                foreach (var eventPair in eventPauseList)
                {
                    Event even = eventPair.First;

                    // delegate the events based on their analysisType.
                    switch (even.Type)
                    {
                        case EventType.REPLACEMENT:
                            {
                                AnalyzeReplacementEvent(even, summary);
                                break;
                            }
                        case EventType.INSERT:
                            {
                                AnalyzeInsertEvent(even, summary);
                                break;
                            }
                        case EventType.SELECTION:
                            {
                                AnalyzeSelectionEvent(even, summary);
                                break;
                            }
                        case EventType.KEYBOARD:
                            {
                                AnalyzeKeyboardEvent(even, summary);
                                break;
                            }
                        case EventType.MOUSE:
                        case EventType.FOCUS:
                        case EventType.PLACEHOLDER:
                            var edit = new NoEdit(int.Parse(even.Properties["id"]));
                            summary.AddUnrelevantEdit(edit);
                            break;
                    }
                }
            }
            finally
            {
                // Summary can already be disposed here, it just disposes the resources only needed during the analysis.
                summary.Dispose();
            }
            return summary;
        }

        /// <summary>
        ///     Analyzes a Replacement event.
        /// </summary>
        /// <param name="even">The event to analyze.</param>
        /// <param name="summary">The summary where to add the result to.</param>
        private void AnalyzeReplacementEvent(Event even, RevisionAnalysisSummary summary)
        {
            foreach (WrdEvent.Replacement repl in even.Parts.OfType<WrdEvent.Replacement>())
            {
                // Only take replacements into account in which text is actually replaced
                string text = summary.Document.Range(repl.Start, repl.End).Text;
                if (repl.NewText.Equals(text)) continue;
                // If the last character of the selected text, in OUR version of the document ends
                // with a \r, then we have a full paragraph selection. Word, when selecting a full 
                // paragraph, selects an extra whitespace (that does not actually exist in our document) to delete
                // However, the \r in the ACTUAL word document is not deleted.
                // The length of the selection in the ACTUAL document (with the extra non-existent space) however
                // increases the length of the selection, so that in OUR version of the document the \r is also 
                // in the selection. 
                // Thus deleting the full range of the logged selection, in a full-paragraph selection, in OUR document
                // requires us to reduce the length of the deletion by 1, so that we account for the extra selected non-existent
                // space that word adds, so it does not delete our \r, that should remain.
                //
                WrdEvent.Replacement standinRep = repl;
                /*if (text.EndsWith("\r") && (!Found_NoReplayBackspace || text.Length == repl.NewText.Length+1))
                    {
                        standinRep = new WrdEvent.Replacement(repl.Start, repl.End - 1, repl.NewText);
                    }*/

                // By convention, a replacement is first a delete and then an insertion, divided over 2 revisions.
                new Deletion(standinRep, int.Parse(even.Properties["id"])).AddTo(summary);
                if (standinRep.NewText.Length != 0)
                {
                    new Insertion(standinRep.Start, standinRep.NewText, int.Parse(even.Properties["id"])).AddTo(
                        summary);
                }
            }

            // Whenever the selection changes, the buffered keystrokes should be flushed, but after a replacement event,
            // a SelectionChange event follows and the buffered keystrokes should be flushed after this event and not here.
        }

        /// <summary>
        ///     Analyzes an Insert event.
        /// </summary>
        /// <param name="even">The event to analyze.</param>
        /// <param name="summary">The summary where to add the result to.</param>
        private void AnalyzeInsertEvent(Event even, RevisionAnalysisSummary summary)
        {
            foreach (WrdEvent.Insert insertPart in even.Parts.OfType<WrdEvent.Insert>())
            {
                new Insertion(insertPart, int.Parse(even.Properties["id"])).AddTo(summary);
            }

            FlushBufferedKeystrokes(summary); // Whenever the selection changes, this should be flushed
        }

        /// <summary>
        ///     Analyzes a SelectionChange event.
        /// </summary>
        /// <param name="even">The event to analyze.</param>
        /// <param name="summary">The summary where to add the result to.</param>
        private void AnalyzeSelectionEvent(Event even, RevisionAnalysisSummary summary)
        {
            foreach (WrdEvent.SelectionChange sel in even.Parts.OfType<WrdEvent.SelectionChange>())
            {
                new SelectionChange(sel, int.Parse(even.Properties["id"])).AddTo(summary);
            }

            FlushBufferedKeystrokes(summary); // Whenever the selection changes, this should be flushed
        }

        /// <summary>
        ///     Composite state of the current key captured
        ///     Analyzes a keyboard event and writes the results to the analysis document.
        /// </summary>
        /// <param name="even">The first part of an event pair (the event proper)</param>
        /// <param name="summary">The summary where to add the result to.</param>
        private void AnalyzeKeyboardEvent(Event even, RevisionAnalysisSummary summary)
        {
            KeyPress winKey = even.Parts.OfType<KeyPress>().FirstOrDefault();
            WrdEvent.Keypress wrdKey = even.Parts.OfType<WrdEvent.Keypress>().FirstOrDefault();

            if (winKey == null || wrdKey == null) return;
            var type = new TypeChar(winKey, wrdKey, int.Parse(even.Properties["id"]));

            // Check if it's a no-replay backspace
            FoundNoReplayBackspace = !wrdKey.IncludeInReplay && Lexical.HasRevisionKey(winKey);
                       
            // Keystrokes should only be processed when there is no selection
            if (summary.Document.Selection.Start == summary.Document.Selection.End)
            {
                type.AddTo(summary);
            }
            else
            {
                BufferedKeyStrokes.Enqueue(type);
            }
        }

        /// <summary>
        ///     Flushes the currently buffered keystrokes.
        /// </summary>
        /// <param name="summary">The summary where the events should be added to.</param>
        private void FlushBufferedKeystrokes(RevisionAnalysisSummary summary)
        {
            while (BufferedKeyStrokes.Count > 0)
            {
                BufferedKeyStrokes.Dequeue().AddTo(summary);
            }
        }

        /// <summary>
        /// Preprocess the event lists to change the replay values of some events. Navigation keys
        /// are now ignored. The delete key is not replayed as it is handled differently and
        ///  backspace keys at the beginning of the file are also, not replayed.
        /// </summary>
        /// <param name="events">The list of events to be processed.</param>
        /// <param name="versionNumber">The version number</param>
        public static void PreprocessEvents(List<Event> events, int versionNumber)
        {
            // Auto convert 1
            // Rationale: RevisionAnalysis rewrite created some changes in the logging
            // Effect: The replay value of certain events (left, right, shift, backspace at the front, etc...)
            // is set to false, to be ignored during the actual RevisionAnalysis
            if (versionNumber >= 50128) return;
            //var converter = new KeyConverter();
            var sb = new StringBuilder();
            foreach (Event e in events)
            {
                if (e.Type != EventType.KEYBOARD) continue;
                WrdEvent.Keypress wordLog = e.Parts.OfType<WrdEvent.Keypress>().FirstOrDefault();
                KeyPress winLog = e.Parts.OfType<KeyPress>().FirstOrDefault();
                if (wordLog == null || winLog == null) continue;
                //var kbState = new HashSet<KeysEx>(winLog.KeyboardState);
                bool replay = false;       
                try
                {
                    replay = wordLog.IncludeInReplay && // replay can only be true if it originally was true!
                             !(
                                 (winLog.Key == KeysEx.VK_DELETE) ||
                                 // don't replay deletes, they are handled by selectionChanged
                                 (winLog.Key == KeysEx.VK_BACK && wordLog.Position == 0) ||
                                 // don't replay backspace at front of text
                                 //(NavigationKey.isNavigation(winLog.Key, converter.Convert(winLog.Key, kbState),winLog.KeyboardState))
                                 // Don't replay navigation keys
                                 (NavigationKey.isNavigation(winLog.Key, winLog.Value, winLog.KeyboardState)) // Don't replay navigation keys
                                 );
                }
                catch (Exception exception)
                {
                    string idx;
                    if(e.Properties.TryGetValue("id", out idx))
                    {
                        Debug.Print("Exception at index: {0} - DocumentLength: {1} - Exception: {2}", idx, 
                            wordLog.DocumentLength, exception);
                        sb.Append(idx).Append("-");
                    }
                }
                int wordLogIndex = e.Parts.IndexOf(wordLog);
                ((WrdEvent.Keypress) e.Parts[wordLogIndex]).IncludeInReplay = replay;
            }
            BadEventIds = sb.ToString();
        }
    }
}