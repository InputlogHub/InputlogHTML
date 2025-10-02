using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.IO;
using InputLog.Core.Pipes;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.WordPauses
{
    /// <summary>
    /// Word Pauses Analysis presents action and pause information on word level.
    /// Words are reconstructed from the idfx log file and the revision analysis.
    /// </summary>
    public class WordPausesAnalysis : Analysis
    {
        #region Fields

        /// <summary>
        /// List of input events on which to perform the analysis.
        /// </summary>
        private readonly IList<IRevision> _revisions;

        /// <summary>
        /// Path to the CSV-file with the target words and the participant-target list.
        /// Id of the author of this idfx.
        /// </summary>
        private readonly string[] _targetPaths;

        private readonly string _participant;

        /// <summary>
        /// The Damerau-Levenshtein distance between taget and production.
        /// </summary>
        private readonly int _wordDistance;

        /// <summary>
        /// Used for building the representation.
        /// </summary>
        private readonly NotationBuilder _builder;

        private readonly string _analysisAbbr;
        private readonly string _language;

        #endregion

        /// <summary>
        /// Word Pause Analysis Constructor
        /// </summary>
        /// <param name="events">Events from the logfile on which this analysis is performed.</param>
        /// <param name="docPath">The path of the document in the original state (before the logging happened). 
        ///     If the file pointed to by the given path does not exists, an empty document will be used.</param>
        /// <param name="sessionID"></param>
        /// <param name="abbrv">The abbreviation of the analysis that ordered this analysis.</param>
        /// <param name="targets">Optional csv-files containing the target words to look up in the reconstructed text and
        /// an optional csv-file containing the names of the participants and target words assigned to them.</param>
        /// <param name="distance">The Damerau-Levenshtein distance between taget and production</param>
        public WordPausesAnalysis(List<Event> events, string docPath, SessionIdentification sessionID, string abbrv,
            string[] targets, int distance)
            : base(abbrv, events, sessionID)
        {
            var revAnalysis = new RevisionAnalysis(events, sessionID, docPath);
            var revisionSummary = (RevisionAnalysisSummary) revAnalysis.DoAnalysis();
            _builder = new NotationBuilder(new WNotationMarkup(), sessionID.GetFileName(), revAnalysis.OriginalContent);
            _revisions = revisionSummary.Revisions;
            _analysisAbbr = abbrv;
            _language = sessionID.GetLanguage();
            _targetPaths = targets;
            _wordDistance = distance;
            if (!_targetPaths.IsNullOrEmpty())
            {
                _participant = sessionID.GetParticipant();
                Dictionary<string, string> info = sessionID.SessionInfo;
                string session;
                if (info.TryGetValue("Session", out session))
                {
                    _participant = _participant + "_" + session;
                }
                string task;
                if (info.TryGetValue("Task", out task))
                {
                    _participant = _participant + "_" + task;
                }
            }
        }

        public override IAnalysisSummary DoAnalysis()
        {
            int count = 0;
            foreach (var revision in _revisions)
            {
                var rev = revision as InsertRevision;
                count++;
                if (rev != null)
                {
                    // Checking the last revision         
                    _builder.InsertText(count == _revisions.Count ? CheckRevision(rev) : rev);
                }
                else
                {
                    var deleteRevision = revision as DeleteRevision;
                    if (deleteRevision != null)
                    {
                        _builder.DeleteRange(deleteRevision);
                    }
                }
            }
            var thisSummary = new WNotationSummary(_builder.Symbols, new SNotationMarkup(), _builder.ToString());

            var pipeline = new ProcessPipeline();
            var processResults = new DataSet("wordPauses");

            {
                pipeline.Register(new MarkupRemover(thisSummary)).
                    Register(new WordsReconstruction(thisSummary.RevisionTxt, thisSummary.SymbolList,
                        NewStartOffset, _language, _analysisAbbr, _targetPaths, _participant, _wordDistance)).
                    Execute(processResults);
                processResults = pipeline.Output;
                return new WordPausesAnalysisSummary(thisSummary, processResults);
            }
        }

        /// <summary>
        /// Checking if the last insert revision ends with a word boundary containing timed information.
        /// If not, one will be added. When the last action was a mouse movement or a focus change, it was ignored
        /// by the revision analysis, but mouse or focus time is necessary to calculate a correct after word pause.
        /// </summary>
        /// <param name="rev">the last revision</param>
        /// <returns>The revision with an added word boundary node if necessary</returns>
        private InsertRevision CheckRevision(InsertRevision rev)
        {
            InsertRevision tmpRev = rev;
            if (rev.Text.EndsWith("\r\n") 
                || rev.Text.EndsWith("\u003B") // ; semi colon
                || rev.Text.EndsWith("\u2A74") // : colon
                || rev.Text.EndsWith("\u201D") // " right double quotation
                || rev.Text.EndsWith("\u2019") // " right single quotation
                || rev.Text.EndsWith("\u00B7")) // middle dot
            {
                return tmpRev;
            }
            
            var oldEdit = tmpRev.Edits[rev.Text.Length - 1];

            // Collecting timed information from a focus or mouse event that follows the last character of the last word.
            Event inputEvent = null;
            if (oldEdit.Id < InputEvents.Count - 1)
            {
                inputEvent = InputEvents[oldEdit.Id + 1];
            }

            IEdit newEdit = new Insertion();
            if (inputEvent != null)
            {
                switch (inputEvent.Type)
                {
                    case EventType.FOCUS:
                    {
                        var eventPart = Event.GetFirstEventPart<FocusChange>(inputEvent);
                        newEdit.StartTime = eventPart.StartTime;
                        newEdit.EndTime = eventPart.EndTime;
                        break;
                    }
                    case EventType.MOUSE:
                    {
                        foreach (var winlogPart in inputEvent.Parts.OfType<AbstractMouseEvent>())
                        {
                            newEdit.StartTime = winlogPart.StartTime;
                            newEdit.EndTime = winlogPart.EndTime;
                        }

                        break;
                    }
                    default:
                    {
                        // Ignore other cases
                        return tmpRev;
                    }
                }
            }
            else
            {
                return tmpRev;
            }
  
            // Creating a bogus event with timed information from the focus change or mouse movement, and with an artificial 'return'.
            // Position and document length of the previous real edit are used and incremented with 1.
            var winKey = new KeyPress { Key = Util.KeyConversion.KeysEx.VK_RETURN, Value = "\n", EndTime = newEdit.EndTime,StartTime = newEdit.StartTime};
            Keypress wrdKey = null;
            var o = oldEdit as TypeChar;           
            if (o != null)
            {
                wrdKey = new Keypress { Position = o.WordKey.Position + 1, DocumentLength = o.WordKey.DocumentLength + 1, IncludeInReplay = false};
            }
            newEdit = new TypeChar(winKey, wrdKey, Convert.ToInt32(inputEvent.Properties["id"]))
            {
                StartPos = oldEdit.EndPos,
                EndPos = oldEdit.StartPos + 1,
            };

            // Returning the full list with a new edit replacing an empty 'NoEdit' revision.
            tmpRev.Edits[rev.Text.Length] = newEdit;          
            return tmpRev;
        }
    }
}