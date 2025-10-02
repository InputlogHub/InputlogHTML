using System;
using System.Linq;
using InputLog.Core.Analyses.Revision.Revisions;

namespace InputLog.Core.Analyses.Revision.Notations
{
    /// <summary>
    ///     Wrapper for a Revision class in the RevisionMatrix.
    ///     The wrapper provides some methods that can provide extra information about the
    ///     revision it 'wraps'.
    /// </summary>
    public class RevisionMatrixEntry
    {
        #region Fields

        private ulong NewStartOffset => Analysis.NewStartOffset;

        /// <summary>
        ///     The revision in this entry.
        /// </summary>
        public IRevision Revision { get; }

        /// <summary>
        ///     The content in a revision
        /// </summary>
        private string _revisionContent;

        private string RevisionContent
        {
            get { return _revisionContent ?? (_revisionContent = Revision.Effect()); }
            set { _revisionContent = value; }        
        }

        /// <summary>
        ///     The number of characters in a revision content.
        /// </summary>
        public int Chars;

        /// <summary>
        /// 
        /// </summary>
        public string TypeOfRevision { get; set; }
    
        /// <summary>
        /// 
        /// </summary>
        public int CharsWithoutSpace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Edits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ulong RevisionDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfWords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SizeOfRevision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get { return _revisionContent; }
            set { _revisionContent = value; } }

        /// <summary>
        /// 
        /// </summary>
        public int PosStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PosEnd { get; set; }
   
        /// <summary>
        /// 
        /// </summary>
        public ulong TimeStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ulong TimeEnd { get; set; }

        /// <summary>
        ///     The number of white spaces in a revision content.
        /// </summary>
        private int _spaces;

        public const string NORMAL_REVISION = "Normal Production";
        public const string INSERT_REVISION = "Insertion";
        public const string DELETE_REVISION = "Deletion";
        public const string UNKNOWN_REVISION = "Unknown";

        #endregion


        /// <summary>
        ///     Construct an entry for the revision matrix.
        /// </summary>
        /// <param name="rev">The revision that is part of the entry</param>
        public RevisionMatrixEntry(IRevision rev)
        {
            Revision = rev;
            Chars = -1;

            _initialize();
        }

        /// <summary>
        /// An aggregated Normal Production doesn't have a Revision.
        /// Normal Productions are aggregated when they follow after each other.
        /// Because we can't forsee if that will happen, all Normal Productions
        /// are considered 'aggegated'. Thus, in the RevisionMatrixAnalysis all the 
        /// relevant features concerning the Normal Production are taken from temporary 
        /// variables and not from the Revision proper. 
        /// </summary>
        public RevisionMatrixEntry()
        {
            Revision = null;
            RevisionContent = string.Empty;
        }

        private void _initialize()
        {
            // Get content from revision
            _revisionContent = Revision.Effect();

            // Used to count the content.
            foreach (var cr in RevisionContent)
                if (char.IsLetterOrDigit(cr))
                    Chars++;
                else if (char.IsWhiteSpace(cr))
                    _spaces++;
            // Mind:the increment is necessary to obtain the right result.
            if (Chars >= 0)
                Chars += 1;
            else
                Chars = 0;
        }

        /// <summary>
        /// Returns the number of edits in the revision
        /// </summary>
        /// <returns>The number of edits in the revision</returns>
        public int NumberOfEdits()
        {
            return Revision?.Edits.Count ?? 0;
        }

        public RevisionType GetRevisionType()
        {
            if (Revision is NormalProductionRevision)
                return RevisionType.NORMAL_PRODUCTION;
            if (Revision is InsertRevision)
                return RevisionType.INSERT;
            if (Revision is DeleteRevision)
                return RevisionType.DELETE;
            return RevisionType.IGNORED;
        }

        /// <summary>
        ///     Returns the type of the revision, at the moment this can be either an
        ///     InsertRevision, DeleteRevision or NormalProduction
        /// </summary>
        /// <returns></returns>
        public string RevisionTypeString()
        {
            if (Revision is NormalProductionRevision)
                return NORMAL_REVISION;
            if (Revision is InsertRevision)
                return INSERT_REVISION;
            if (Revision is DeleteRevision)
                return DELETE_REVISION;
            return UNKNOWN_REVISION;
        }

        /// <summary>
        ///     Returns the content of the revision, this either the inserted text, or the deleted
        ///     text.
        /// </summary>
        /// <returns>Text altered by the revision (inserted or deleted)</returns>
        public string Content()
        {
            return RevisionContent;
        }

        /// <summary>
        ///     The endtime of the last edit in a revision.
        /// </summary>
        /// <returns></returns>
        public ulong EndTime()
        {
            if (Revision == null) return 0;
            if (NewStartOffset > Revision.EndTime)
            {
                return Revision.EndTime;               
            }
            return Revision.EndTime - NewStartOffset;
        }

        /// <summary>
        ///     The start time of the first edit in a revision.
        /// </summary>
        /// <returns></returns>
        public ulong StartTime()
        {
            if (Revision == null) return 0;
            if (NewStartOffset > Revision.StartTime)
            {
                return Revision.StartTime;
            }
            return Revision.StartTime - NewStartOffset;
        }

        /// <summary>
        ///     Returns the duration of a revision. This is the duration between the start time of the first edit
        ///     up to the endtime of the last edit in the revision.
        /// </summary>
        /// <returns>Duration of the revision </returns>
        public ulong Duration()
        {
            if (Revision == null) return 0;
            if (Revision.EndTime == 0)
            {
                return 0;
            }
            var duration = Revision.EndTime - Revision.StartTime;
            return duration;
        }

        /// <summary>
        ///     Returns the revisionnumber of the revision.
        /// </summary>
        /// <returns>The revision number</returns>
        public int ID()
        {
            return Revision?.RevisionNumber ?? 0;
        }

        /// <summary>
        ///     Returns the position of the cursor at the beginning of the revision.
        /// </summary>
        /// <returns>Position of the cursor at the beginning of the revision.</returns>
        public int StartPosition()
        {
            return Revision?.StartPos?? 0;
        }

        /// <summary>
        ///     Returns the position of the cursor at the beginning of the revision.
        /// </summary>
        /// <returns>Position of the cursor at the beginning of the revision.</returns>
        public int EndPosition()
        {
            return Revision?.EndPos?? 0;
        }

        /// <summary>
        ///     Returns the number of characters in a revision.
        ///     TODO Number of characters is constantly one short of exact count.
        ///     HACK: adding 1 to char count. Char initial value is -1.
        /// </summary>
        /// <returns>int</returns>
        public int Characters()
        {
            return Chars;
        }

        /// <summary>
        ///     Returns the number of characters without spaces in a revision.
        /// </summary>
        /// <returns>int</returns>
        public int CharWithoutSpace()
        {
           return _spaces > Chars ? 0 : Chars - _spaces;
        }

        /// <summary>
        ///     Returns the number of words in a revision.
        /// </summary>
        /// <returns>int</returns>
        public int Words()
        {
            if (RevisionContent.Length < 2) return 0;
            var count = 0;
            var isPrevChar = false;
            foreach (var isChar in RevisionContent.Select(char.IsLetterOrDigit))
            {
                if (isChar && !isPrevChar)
                {
                    count++;
                }
                isPrevChar = isChar;
            }
            return count;
        }

        /// <summary>
        ///     Returns the total number of deleted or added characters in this revision.
        /// </summary>
        /// <returns>The number of characters added or deleted in this revision</returns>
        public int RevisionSize()
        {
            if (Revision == null) return 0;
            return Math.Abs(Revision.EndPos - Revision.StartPos);
        }
    }
}