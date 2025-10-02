using System;
using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;
using System.Xml;
using System.Xml.Schema;

namespace InputLog.Core.Analyses.Revision.RevisionAnalysis
{
    /// <summary>
    /// Keeps track of the known revisions by maintaining them in a list.
    /// The last added revision can easily be accessed using CurRevision and new (empty) revisions
    /// are created using CreateRevision. The first revision is created automatically.
    /// </summary>
    public sealed class RevisionAnalysisSummary : IAnalysisSummary
    {
        #region Fields
        /// <summary>
        /// List of revisions known until now.
        /// </summary>
        internal readonly List<IRevision> Revisions = new List<IRevision>();

        /// <summary>
        /// 
        /// This buffer contains all edits that are added to the revision to make it more informative but 
        /// do not add any value to the revision otherwise, such as the deletion or addition of text. These 
        /// edits should be buffered and added either to the current revision or the new revision, depending on what
        /// revision the next non-SelectionChange, non Unrelevant event belongs to.
        /// </summary>
        private readonly Queue<IEdit> UnrelevantEditsBuffer = new Queue<IEdit>();

        /// <summary>
        /// The current revision, by definition the last one in Revisions.
        /// If the list does not contain a Revision yet, a new one will be created, added to the list and returned.
        /// If the property is set instead of read, the new value will be added als the (currently) last revision to
        /// the list.
        /// </summary>
        internal IRevision Current { get { return Revisions.Count == 0 ? null : Revisions.Last(); } }

        /// <summary>
        /// A document that reflects the current state of the reconstructed document.
        /// </summary>
        internal InputlogDocument Document;

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        private bool Disposed { get; set; }

        /// <summary>
        /// The number for the next 'numbered' revision. (This is not the same as the total number of revisions.
        ///  Because Normal Flow revisions do not increase the revision number
        /// </summary>
        private int RevisionNumber { get; set; }
        /// <summary>
        /// Makes a doc invisible when a revision is made.
        /// </summary>
        public bool IsRevision { get; set; }
        #endregion

        #region Reporting Methods

        /// <summary>
        /// The default behavior for AnalysisSummaries is to not support any reporting, as this
        /// requires extra methods to be added (namely those used for reporting). Any method 
        /// that does want to support Reporting may be return a dictionary of ReportingMethods it
        /// supports.
        /// </summary>
        /// <returns>Null: default behavior is to not support reporting.</returns>
        public Dictionary<String, InputLog.Core.Reporting.ReportMethod> GetBoundReportTargets()
        {
            return null;
        }
        
        #endregion

        /// <summary>
        /// Creates a new RevisionTracker to keep track of the known revisions. A first revision is created automatically.
        /// </summary>
        /// <param name="docpath">Path to the original document when the logging just started.</param>
        internal RevisionAnalysisSummary(string docpath)
        {
            Document = new InputlogDocument(docpath, IsRevision);
            Document.Word.Visible = false;
            RevisionNumber = 1;

            // If the document already contained text insert this text in a first revision.
            /*if (Document.Length > 0)
            {
                InsertRevision rev = new InsertRevision();
                Revisions.Add(rev);
                var edit = new Core.Analyses.Revision.Revisions.Edits.Insertion(0, Document.FullDocument);
                rev.RevisionNumber = 0;
                edit.AddTo(this);
            }*/
        }

        /// <summary>
        /// Creates a new revision of the given type, adds it to the collection
        /// of revisions and returns it.
        /// </summary>
        internal IRevision CreateRevision(RevisionType type, IEdit edit)
        {
            IRevision rev;
            switch (type)
            {
                case RevisionType.DELETE:
                    rev = new DeleteRevision(RevisionNumber++);
                    break;
                case RevisionType.INSERT:
                    rev = IsNormalProduction(edit) ? new NormalProductionRevision(0) : new InsertRevision(RevisionNumber++);
                    break;
                case RevisionType.SELECTION_CHANGE:
                    rev = new SelectionChangeRevision(RevisionNumber++);
                    break;
                default:
                    rev = null;
                    break;
            }

            Revisions.Add(rev);
            return rev;
        }

        private bool IsNormalProduction(IEdit edit)
        {
            // What is normal production?
            // Normal production is an insertion of text at the end of the document
            // or near the end when only whitespace follows the insertion location.

            // 14 June 2012: 
            // Document.FollowedByWhitespace: why do we increase the position we use as parameter for this function?
            // REASON: Because the character is already added, before we either create the revision the character will
            // be a part too OR because when the check is called to see if an edit BelongsTo a revision, the character, also,
            // has already been added.
            // Therefore we increase the position by the length of the insertion to make sure we only take into account the
            // trailing characters!
            //
            var c = edit as TypeChar;
            if (c != null)
            {
                var theEdit = c;
                return (theEdit.WordKey.DocumentLength == theEdit.WordKey.Position
                    || Document.FollowedByWhiteSpace(theEdit.WordKey.Position + theEdit.WinKey.Value.Length));
            }
            var insertion = edit as Insertion;
            if (insertion != null)
            {
                // TODO: -1 correct ??
                var theEdit = insertion;
                return theEdit.UseAfter != null && 
                    (theEdit.InsertPosition == Document.Length - 1 || Document.FollowedByWhiteSpace(((bool)theEdit.UseAfter)
                    ? (theEdit.Position + theEdit.Length) : theEdit.Position));
            }
            throw new InvalidOperationException("Normal production is only available for insertion revisions!");
        }

        /// <summary>
        /// Buffers the given Unrelevant event to be added to the current revision whenever
        /// FlushUnrelevantEdits is called.
        /// </summary>
        /// <param name="change">The Unrelevant Edit to buffer.</param>
        internal void AddUnrelevantEdit(IEdit change)
        {
            UnrelevantEditsBuffer.Enqueue(change);

            // 14 June 2012: Every edit will be executed before being added to a revision, to make sure that all the correct
            // data is available in the edit upon deciding what revision it belongs too. The selection edit is not executed
            // through the AbstractEdit.AddTo() method as the other edits are, therefore, it needs to be executed here instead,
            // and therefore this statement is not commented out.
            //

            // Every added event should be executed, even if it is locally buffered and not yet added to a revision
            change.Execute(Document);
        }

        /// <summary>
        /// Flushes the currently buffered SelectionChange events, adding them to the current revision.
        /// </summary>
        internal void FlushUnrelevantEdits()
        {
            if (Current != null)
            {
                while (UnrelevantEditsBuffer.Count > 0)
                {
                    // Do not use the AddTo of the Unrelevant event as this could add it to the buffer again in case
                    // of a SelectionChange event that has been added.
                    // Add it directly to the edits as the Add method of the revision would execute the edit again.
                    Current.Edits.Add(UnrelevantEditsBuffer.Dequeue());
                }
            }
        }

        #region Xml Serialization Infrastructure
        public void WriteXml(XmlWriter writer)
        {

        }

        public void ReadXml(XmlReader reader)
        {

        }

        public XmlSchema GetSchema()
        {
            return (null);
        }
        #endregion

        /// <summary>
        /// Disposes the resources needed while building the summary.
        /// After calling this method, the object can still be used as a data structure,
        /// but it should not be expanded any more.
        /// </summary>
        public void Dispose()
        {
            FlushUnrelevantEdits();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                Document.Close();
            }
            Document = null;

            Disposed = true;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~RevisionAnalysisSummary()
        {
            Dispose(false);
        }
    }
}