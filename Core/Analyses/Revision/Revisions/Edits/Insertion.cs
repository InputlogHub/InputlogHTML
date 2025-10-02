using System;
using InputLog.Core.Plugin.WordLog;
using WordLog = InputLog.Core.Events.WordLog;
using System.Xml;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// Command that performs the given Insert in the given document.
    /// </summary>
    class Insertion : AbstractEdit
    {
        #region Fields
        /// <summary>
        /// The insert to perform in the document.
        /// </summary>
        private readonly WordLog.Insert _insert;

        /// <summary>
        /// Length of the inserted text.
        /// The length of Insert.After can be different from the length of Insert.Before
        /// if the LoggedDocument.Range method trimmed the start or end because it was
        /// smaller than 0 or greater than the document length.
        /// As such, the actual length of the inserted text is the maximum of the both.
        /// </summary>
        public override int Length => Math.Max(_insert.After.Length, _insert.Before.Length);

        /// <summary>
		/// Start position of the insertion. Start position can only be correctly determined after the
		/// UseBefore(document) or GetInsertPosition(document) has been called.
		/// It is not necessarily equal to the Position of the cursor after the insertion
		/// has been performed.
		/// </summary>
		public override int StartPos
		{
			get
			{
			    if (InsertPosition != null) return (int)InsertPosition;
			    return 0;
			}
		    set
			{
				base.StartPos = value;
			}
		}

		/// <summary>
		/// End position of the insertion, this is the position of the last character that has been inserted
		/// by the insertion. It is not necessarily equal to the Position of the cursor after the insertion
		/// has been performed.
		/// </summary>
		public override int EndPos
		{
			get 
            { if (InsertPosition != null) return (int)InsertPosition + Length;
                return Length;
            }
		    set
			{
				base.EndPos = value;
			}
		}

        /// <summary>
        /// The type of revision to which the edit belongs (insert, delete, ...).
        /// </summary>
        protected override RevisionType RevisionType { get { return RevisionType.INSERT; } }

        /// <summary>
        /// The position where the text is either inserted before or after.
        /// </summary>
        public int Position => _insert.Position;

        /// <summary>
        /// True if the text is inserted after the position denoted by Position, false otherwise.
        /// Use this method only after UseBefore(document) or GetInsertPostion(document) was called,
        /// otherwise its value is undefined.
        /// </summary>
        public bool? UseAfter { get; private set; }

        /// <summary>
        /// The text that is inserted by this Insertion.
        /// Use this method only after UseBefore(document) or GetInsertPostion(document) was called,
        /// otherwise its value is undefined.
        /// </summary>
        public string Text
        {
            get
            {
                if (UseAfter == true) { return _insert.After; }
                return UseAfter == false ? _insert.Before : null;
            }
        }

        /// <summary>
        /// Returns the start position where the text should be inserted.
        /// This position differs from Position in that Position contains the position of the cursor after
        /// the insertion has happened while this property contains the start position where the text is
        /// inserted after.
        /// Use this method only after UseBefore(document) or GetInsertPostion(document) was called,
        /// otherwise its value is undefined.
        /// </summary>
        public int? InsertPosition
        {
            get
            {
                if (UseAfter == true) { return Position; }
                if (UseAfter == false) { return Position - Text.Length; }
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="insert">The Insert to perform in the document.</param>
		/// <param name="id">Id of the event associated with this edit</param>
        public Insertion(WordLog.Insert insert, int id)
            : base(EditType.Insertion, id)
        {
            InputlogDocument.IsRevision = true;
            _insert = insert;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Insertion()
        {
            InputlogDocument.IsRevision = true;
            _insert = new WordLog.Insert();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">Position where to insert the text.</param>
        /// <param name="text">The text to insert.</param>
		/// <param name="id">Id of the event associated with this edit</param>
        public Insertion(int position, string text, int id)
            // text.Length should be added to the position because when using GetInsertPosition(),
            // it will be subtracted from it.
            : this(new WordLog.Insert(position + text.Length, text, ""), id) { }

        /// <summary>
        /// Inserts the text denoted by the Insert given during construction in the given document.
        /// </summary>
        /// <param name="doc">The document where to replace the range in.</param>
        /// <returns>True if the execution changed the document state (change in selection, 
        /// content has changed, ...), false if not.</returns>
        public override bool Execute(InputlogDocument doc)
        {
            string text = UseBefore(doc) ? _insert.Before : _insert.After;
            doc.Range(GetInsertPosition(doc), GetInsertPosition(doc)).Range.Text = text;
            // TODO This should be set to GetInsertPosition(doc) + Length?
            //doc.Selection.SetRange(insert.Position, insert.Position);
            doc.Selection.SetRange(GetInsertPosition(doc) + Length, GetInsertPosition(doc) + Length);

            return Length != 0;
        }

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the execution changed the document state (change in selection, 
        /// content has changed, ...), false if not.</returns>
        public override bool Undo(InputlogDocument doc)
        {
            if (InsertPosition != null) doc.Range((int)InsertPosition, (int)InsertPosition + Length).Range.Delete();

            return Length != 0;
        }

        /// <summary>
        /// Returns true if the text contained in Before was inserted before the position or
        /// false if the text contained in After was inserted after the position contained in Position.
        /// </summary>
        /// <param name="doc">The document where the text will be inserted in.</param>
        /// <returns>
        /// Returns true if the text contained in Before was inserted before the position or
        /// false if the text contained in After was inserted after the position contained in Position.
        /// </returns>
        private bool UseBefore(InputlogDocument doc)
        {
            // First check if this method was run before and if so, return old result
            if (UseAfter != null)
            {
                return UseAfter != true;
            }

            // If it was not yet run before, determine result etc.
            bool rslt = false;
            if (_insert.After.Length != Length) // After was cropped => we should use Before
            {
                rslt = true;
            }
            else if (_insert.Before.Length != Length) // Before was cropped => we should use After
            {}
            else if (!_insert.Before.Equals(doc.Range(_insert.Position - Length + 1, _insert.Position).Text))
            // Before differs from the current text before the position => we should use Before
            {
                rslt = true;
            }
            else if (!_insert.After.Equals(doc.Range(_insert.Position, _insert.Position + Length - 1).Text))
            // Before equals the current text before the position and After differs from the current 
            // text after the position => we should use After
            // Note that in the case that we have xa°abx (° denotes the current position and x denotes unknown charachters)
            // in which the user presses backspace (resulting in x°abx) and then undoes its action (resulting back in xa°abx
            // and in Before = After = a), we will return false due to this if check (during reconstruction we would have
            // xa°bx in which Before equals the text before the position, but After does not equal the text after the position).
            // However, this does not yield incorrect results as Before = After and the POU after the replay is also same.
            {}
            else if (_insert.Before.Equals(_insert.After))
            // It is indistinguishable. Its either an undo action (in which case both Before and After
            // are plausible) or its a pasted piece of text in which case it was inserted before the position.
            // Two out of three use Before => we return true.
            {
                rslt = true;
            }

            UseAfter = !rslt;

            return rslt;
        }

        /// <summary>
        /// Returns the actual position where the text would be inserted when Execute(doc) would be called.
        /// </summary>
        /// <param name="doc">The document where the text would be inserted.</param>
        /// <returns>The actual position where the text would be inserted when Execute(doc) would be called.</returns>
        public int GetInsertPosition(InputlogDocument doc)
        {
            return UseBefore(doc) ? _insert.Position - Length : _insert.Position;
        }

        /// <summary>
        /// Returns true if the edit belongs to the given revision.
        /// The given document should contain the version of the logged document right before
        /// the instance of IEdit is executed on it (i.e. all previously seen edits should be
        /// executed on it).
        /// </summary>
        /// <param name="rev">The revision.</param>
        /// <param name="doc">The document on which all the edits until this one are executed (this one excluded).</param>
        /// <returns>True if this instance belongs to the given revision, false if not.</returns>
        protected override bool BelongsTo(IRevision rev, InputlogDocument doc)
        {
			return (rev is InsertRevision && (rev.CurrentPOU == AbstractRevision.Defaultval
                || rev.CurrentPOU == GetInsertPosition(doc)));
        }

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public override string Effect()
		{
			return Text;
		}

        #region Xml Serialization Infrastructure
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("Insert");
            _insert.WriteXml(writer);
            if (UseAfter != null)
                writer.WriteElementString("UseAfter", UseAfter.ToString());
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            reader.ReadStartElement("Insert");
            _insert.ReadXml(reader);
            if (reader.IsStartElement("UseAfter"))
                UseAfter = bool.Parse(reader.ReadElementString("UseAfter"));
            reader.ReadEndElement();
        }
        #endregion
    }
}