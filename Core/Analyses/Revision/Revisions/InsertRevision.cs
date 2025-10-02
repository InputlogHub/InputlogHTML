using System.Text;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// A revision in which text/characters are added.
    /// </summary>
    class InsertRevision : AbstractRevision
    {
        #region Fields
        /// <summary>
        /// Internal buffer used for maintaining the inserted text.
        /// </summary>
        private readonly StringBuilder _buf = new StringBuilder();

        /// <summary>
        /// If all the edits would be executed, the text that would be inserted at the start position.
        /// </summary>
        public string Text { get { return _buf.ToString(); } }

        /// <summary>
        /// The position where the text is inserted. Is '0' when this is the first insert
        /// Only valid if at least one Insertion or TypeChar was added, otherwise this property returns -1.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Returns the current POU (Point Of Utterance), i.e. the position after the last character in the StringBuilder 'Buf'
        /// This is only a valid property if at least one Deletion, Insertion or TypeChar was added to the revision,
        /// otherwise the property equals -1.
        /// </summary>
        public override int CurrentPOU { get { return Position + _buf.ToString().Length; } }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        protected InsertRevision()
            : base(RevisionType.INSERT)
        {
            InputlogDocument.IsRevision = true;
            Position = Defaultval;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revisionnumber">The revision number of this revision.</param>
        public InsertRevision(int revisionnumber)
            : base(revisionnumber, RevisionType.INSERT)
        {
            InputlogDocument.IsRevision = true;
            Position = Defaultval;
        }

        /// <summary>
        /// Adds the given edit to the list of edits if it belongs to this revision, throws an exception otherwise.
        /// </summary>
        /// <param name="edit">The edit to add.</param>
        /// <param name="doc">The document in the state right before the edit is executed.
        /// When the control is returned, the edit will be executed on the document.</param>
        /// <exception cref="NotInRevisionException">Whenever the given edit does not belong in the revision.</exception>
        public override void Add(IEdit edit, InputlogDocument doc)
        {
            var insertion = edit as Insertion;
            if (insertion != null)
            {
                var ins = insertion;

                if (Position == Defaultval)
                {
                    Position = ins.GetInsertPosition(doc);
                }
                _buf.Append(ins.Text);
            }
            else
            {
                var c = edit as TypeChar;
                if (c != null)
                {
                    var type = c;
                    if (type.IncludeInReplay)
                    {
                        if (type.IsDeletion)
                        {
                            throw new NotInRevisionException(
                                $"The given edit (of type TypeChar) is not an instertion: {type}");
                        }

                        if (Position == Defaultval)
                        {
                            Position = type.WordKey.Position;
                        }

                        // 14 Jun 2012: 
                        // As normal productions can have characters being added before a trailing
                        // whitespace, we do not always append characters. Sometimes we have to
                        // add them in the middle of the buffer.
                        //
                        if (type.WordKey.Position < _buf.Length)
                        {
                            _buf.Insert(type.WordKey.Position, type.WinKey.Value);
                        }
                        if (type.WinKey.Key == KeysEx.VK_RETURN)
                        {
                            _buf.AppendLine();
                        }
                        else
                        {
                            _buf.Append(type.WinKey.Value);
                        }
                    }
                }
                else if (!(edit is SelectionChange))
                {
                    throw new NotInRevisionException($"The given edit is not an Insertion or a TypeChar: {edit}");
                }
            }

            base.Add(edit, doc);
        }

		/// <summary>
		/// Returns the combined effect of all the edits in this revision. This may be for example, all the text that has been 
		/// added/deleted/selected.
		/// </summary>
		/// <returns>The combined effect of all events in this revision</returns>
		public override string Effect()
		{
			return Text;
		}
    }
}