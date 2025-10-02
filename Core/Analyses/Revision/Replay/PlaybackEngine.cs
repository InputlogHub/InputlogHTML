using System;
using System.Collections.Generic;
using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;
using Microsoft.Office.Interop.Word;

namespace InputLog.Core.Analyses.Revision.Replay
{
    /// <summary>
    /// This class bundles a document to a list of revisions. The document given in the constructor represents the
    /// document in its original state and the list of revisions are the steps needed to get to the final state.
    /// This class provides methods to advance one step (in which a step is either a single edit or a whole revision) and to go back a step. It is also possible to get the current version number of the document.
    /// </summary>
    public class PlaybackEngine : IDisposable
    {
        #region Fields
        /// <summary>
        /// The list of revisions that can be replayed.
        /// </summary>
        private readonly IList<IRevision> _revisions;

        /// <summary>
        /// The document in the current state (ie, all the edits that lie before NextEdit are executed on it).
        /// </summary>
        private InputlogDocument _document;

        /// <summary>
        /// The index of the index we are currently replaying.
        /// </summary>
        private int _currentRevisionIndex;

        /// <summary>
        /// The revision we are currently replaying.
        /// </summary>
        private IRevision CurrentRevision { get { return _revisions[_currentRevisionIndex]; } }

        /// <summary>
        /// The list of edits of the current revision.
        /// </summary>
        private IList<IEdit> CurrentEdits { get { return CurrentRevision.Edits; } }

        /// <summary>
        /// The index of the first following edit in line for being replayed.
        /// </summary>
        private int _nextEditIndex;

        /// <summary>
		/// Keeps track of the number of relevant edits in the previous in each revision.
		/// This list gets filled in through successive ToNext() calls. It is not populated
		/// from the start of the Playback, but should be completely populated by the end
		/// of the playback.
		/// </summary>
		private readonly int[] _relevantEditsPerRevision; 

        /// <summary>
        /// The first following edit in line for being replayed.
        /// </summary>
        private IEdit NextEdit { get { return CurrentEdits[_nextEditIndex]; } }

        /// <summary>
        /// True if there are edits left that have not been replayed, false if not.
        /// </summary>
        public bool HasNext
        {
            get
            {
                // We also count a non-existing revision such that when having n revisions and
                // when having executed the last edit of the last revision (which is revision n-1),
                // CurrentRevisionIndex is set to n. This way, the version number is now 'n.0'.
                // As long as we are not in this last artificial revision, there is an edit to execute
                // and thus we have the following check
                return _revisions.Count > _currentRevisionIndex;
            }
        }

        /// <summary>
        /// False if at least one edit has been replayed and if thus you can go back to at least one earlier version.
        /// </summary>
        public bool HasPrevious { get { return _nextEditIndex > 0 || _currentRevisionIndex > 0; } }

        /// <summary>
        /// A string representing the current version of the document.
        /// The version will be represented by x.y in which y is the 0-based rang order of the next edit in line
        /// to be executed on the document. x is the 0-based rang order of the revision to which this next edit
        /// belongs. As a consequence, 0.0 is the document in its original form and eg version 1.0 is the version
        /// number of the document state where the last edit of the first revision is executed, but the first edit of
        /// the second revision not.
        /// </summary>
        public string CurrentVersion 
		{ 
			get 
			{
				return string.Format("{0}.{1}", 
					ActiveRevisionNumber(_currentRevisionIndex),
					ActiveNumberOfEdits(_currentRevisionIndex)); 
			} 
		}

		/// <summary>
		/// An integer representing the current revision that is being playbacked in the document.
		/// The version starts with 0 and increases through next versions. Every new revision the 
		/// revision number increases, except during NormalProductions. In which case the revisionNumber
		/// of the last actual revision is used. 
		/// </summary>
		public int CurrentRevisionNumber { get { return ActiveRevisionNumber(_currentRevisionIndex); } }

		/// <summary>
		/// An integer representing the current relevant edit in the revision being playbacked. Relevant
		/// edits mean all edits that actually change the documnent, position changes are not included in this.
		/// The edits increase with every small step you make and at the start of each 'revision' it is resetted to 0.
		/// However note that when a Revision is followed by NormalProduction, which is a new 'Revision', although the
		/// revision number will not increase, the edit number will be reset to 0.
		/// </summary>
		public int CurrentEditNumber { get { return ActiveNumberOfEdits(_currentRevisionIndex); } }

		/// <summary>
		/// The current position of the cursor in the document.
		/// </summary>
		public int CurrentPosition { get { return _document.Position; } }

		/// <summary>
		/// The current length of the document
		/// </summary>
		public int CurrentDocLength { get { return _document.Length; } }

        /// <summary>
        /// True if the object is disposed, false if not.
        /// </summary>
        protected bool Disposed { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="summary">The RevisionAnalysisSummary that contains the list of revisions that determine the replay.</param>
        /// <param name="doc">The original version of the document on which the revisions will be executed.</param>
        public PlaybackEngine(RevisionAnalysisSummary summary, Document doc)
        {
            _revisions = summary.Revisions;
            _currentRevisionIndex = 0;
            _nextEditIndex = 0;
            _document = new InputlogDocument(doc);
            _document.Word.Visible = false;
			_relevantEditsPerRevision = new int[_revisions.Count];
        }

		/// <summary>
		/// Returns the revision number to be shown during playback. This is special because normal production
		/// revisions have revision number 0. However, during playback this revision number is just ignored. And the
		/// edits just keep on counting as though the normal production was part of the previous revision.
		/// </summary>
		/// <param name="currentInd">Index of the current revision</param>
		/// <returns>The revision number to be displayed during playback</returns>
		private int ActiveRevisionNumber(int currentInd)
		{
			if (_revisions.Count == 0) return 0;

			if (currentInd >= _revisions.Count && currentInd > 0)
			{
				return ActiveRevisionNumber(currentInd - 1);
			}
			if (_revisions[currentInd].RevisionNumber == 0 && currentInd > 0)
			{
				return ActiveRevisionNumber(currentInd - 1);
			}
			return _revisions[currentInd].RevisionNumber;
		}

		/// <summary>
		/// Returns the number of relevant edits to be shown during playback. This is a special number when an insert
		/// or delete revision is followed by a normalproduction revision the 'RelevantEditCount' should continue the
		/// counting as if no change of revision has occurred.
		/// This method calculates what is the correct number to be displayed.
		/// </summary>
		/// <param name="currentInd">Index of the current revision</param>
		/// <returns>The relevant edit number to be displayed during playback</returns>
		private int ActiveNumberOfEdits(int currentInd)
		{
			if (_relevantEditsPerRevision.Length == 0) return 0;

			if ((currentInd >= _revisions.Count && currentInd > 0))
				//|| (Revisions[CurrentInd].RevisionNumber == 0 && CurrentInd > 0))
			{
				return ActiveNumberOfEdits(currentInd - 1);
			}
			return _relevantEditsPerRevision[currentInd];
		}

        /// <summary>
        /// Minor version update - Advances to the next version of the document by advancing one edit.
        /// </summary>
        public void ToNext()
        {
            if (!HasNext)
            {
                throw new InvalidOperationException("Final version of the document has been reached.");
            }

            bool changed = false;
            while (!changed && HasNext)
            {
                // First execute first following edit that changes the document state and
				// is not an Ignored type of edit.
				if (NextEdit.Type != EditType.Ignored)
				{
					changed = NextEdit.Execute(_document);

					// Don't count selection changes as actual edits to the document.
					if (NextEdit.Type != EditType.SelectionChange)
					{
						_relevantEditsPerRevision[_currentRevisionIndex]++;
					}
				}

                // Then select the next edit in line
                _nextEditIndex++;
                if (_nextEditIndex == CurrentEdits.Count)
                {
                    _nextEditIndex = 0;
                    _currentRevisionIndex++;
                }
            }
        }

        /// <summary>
        /// Minor version downgrade - Downgrades to the previous version of the document by undoing the last executed edit.
        /// </summary>
        public void ToPrevious()
        {
            if (!HasPrevious)
            {
                throw new InvalidOperationException("Original version of the document has been reached.");
            }

            bool changed = false;
            while (!changed && HasPrevious)
            {
                // Select the last executed edit
                _nextEditIndex--;
                if (_nextEditIndex < 0)
                {
                    _currentRevisionIndex--;
                    _nextEditIndex = CurrentEdits.Count - 1;
                }

                // Then undo it
				if (NextEdit.Type != EditType.Ignored)
				{
					changed = NextEdit.Undo(_document);

					// Don't count selection changes as actual edits to the document.
					if (NextEdit.Type != EditType.SelectionChange)
					{
						_relevantEditsPerRevision[_currentRevisionIndex]--;
					}
				}
            }
        }

        /// <summary>
        /// Major version update - Advances to the next version of the document by advancing a whole revision.
        /// </summary>
        public void ToNextRevision()
        {
            if (!HasNext)
            {
                throw new InvalidOperationException("Final version of the document has been reached.");
            }

            var oldrevidx = _currentRevisionIndex;
            while (oldrevidx == _currentRevisionIndex)
            {
                ToNext();
            }
        }

        /// <summary>
        /// Major version downgrade - Downgrades to the previous version of the document by the whole current revision.
        /// </summary>
        public void ToPreviousRevision()
        {
            if (!HasPrevious)
            {
                throw new InvalidOperationException("Original version of the document has been reached.");
            }

            if (_nextEditIndex == 0)
            {
                ToPrevious();
            }

			int old = CurrentRevisionNumber;

            while (_nextEditIndex != 0 && CurrentRevisionNumber >= old)
            {
                ToPrevious();
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
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the managed resources should also be disposed, false if not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            try
            {
                if (disposing)
                {
                    _document.Dispose();
                }
                _document = null;
            }
            finally
            {
                Disposed = true;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~PlaybackEngine()
        {
            Dispose(false);
        }
    }
}