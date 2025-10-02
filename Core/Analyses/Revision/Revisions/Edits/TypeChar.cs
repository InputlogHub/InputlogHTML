using InputLog.Core.Analyses.Revision.RevisionAnalysis;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Plugin.WordLog;
using InputLog.Core.Util.KeyConversion;
using System.Xml;

namespace InputLog.Core.Analyses.Revision.Revisions.Edits
{
    /// <summary>
    /// Command that types the character(s) corresponding to the given Keypress into the document 
    /// if needed (the command has internal knowledge about when a Replacement will occur)
    /// </summary>
    public class TypeChar : AbstractEdit
    {
        #region Fields
        /// <summary>
        /// WinLog part of the keypress event.
        /// </summary>
        public readonly KeyPress WinKey;

        /// <summary>
        /// WordLog part of the keypress event.
        /// </summary>
        public readonly Keypress WordKey;

        /// <summary>
        /// Returns true if the typed character results in a deletion (IncludeInReplay being ignored).
        /// </summary>
        public bool IsDeletion => Lexical.HasRevisionKey(WinKey); // == KeysEx.VK_BACK || WinKey.Key == KeysEx.VK_DELETE;

        /// <summary>
        /// Returns true if the TypeChar should be included in the replay or false if not.
        /// </summary>
        public bool IncludeInReplay => WordKey.IncludeInReplay;

        /// <summary>
        /// The type of revision to which the edit belongs (insert, delete, ...).
        /// </summary>
        protected override RevisionType RevisionType => IsDeletion ? RevisionType.DELETE : RevisionType.INSERT;

        /// <summary>
        /// The text that was deleted during the last call to Execute.
        /// </summary>
        private string _deletedText;

		/// <summary>
		/// The end position of the cursor after the insertion.
		/// Only valid to be called after the Execute() method has been called.
		/// </summary>
		public override int EndPos
		{
			get
			{
			    if (IsDeletion)
				{
					return WordKey.Position - Length;
				}
			    return WordKey.Position + Length;
			}
		}

		/// <summary>
		/// The position of the cursor before the character is inserted/deleted.
		/// 
		/// </summary>
		public override int StartPos => WordKey.Position;

        /// <summary>
		/// Returns the length of the inserted/deleted text.
		/// </summary>
		public override int Length
		{
			get
			{
			    // DeletedText can be null
                var deleted = _deletedText ?? string.Empty;
                //return IsDeletion ? deleted.Length : WinKey.Value.Length;
                if (IsDeletion)
                {
                    return deleted.Equals("\r\n") ? 1 : deleted.Length;
                }
                return WinKey.Value.Equals("\r\n") ? 1 : WinKey.Value.Length;
            }
		}

		/// <summary>
		/// Returns the start time of the keyboard action that is represented by this type char.
		/// </summary>
		public override ulong StartTime { get; set; }

        /// <summary>
		/// Returns the end time of the keyboard action that is represented by this type char.
		/// </summary>
		public override ulong EndTime { get; set; }

        /// <summary>
		/// Returns true if this edit has timing information. False if no timing information is available
		/// in this edit.
		/// </summary>
		public override bool HasTiming => true;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="winkey">The WinLog part of the event.</param>
        /// <param name="wordkey">The WordLog part of the event.</param>
		/// <param name="id">Id of the event associated with this edit</param>
        public TypeChar(KeyPress winkey, Keypress wordkey, int id)
            : base(EditType.TypeChar, id)
        {
            InputlogDocument.IsRevision = true;
            WinKey = winkey;
            WordKey = wordkey;

			StartTime = WinKey.StartTime;
			EndTime = WinKey.EndTime;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public TypeChar()
        {
            InputlogDocument.IsRevision = true;
            WinKey = new KeyPress();
            WordKey = new Keypress();
        }

        /// <summary>
        /// Types the character at the current position in the given document.
        /// (So in order to type it at the correct place, the cursor (and thus the selection)
        /// should be at the correct position in the document. We could set the selection to
        /// the correct position, but for efficiency reasons we expect this is done not by this
        /// command and just once in the beginning such that consecutive keypresses can take
        /// advantage of this and only have to use the TypeText method of the selection in the
        /// given document.)
        /// </summary>
        /// <param name="doc">The document where to LinearAnalysisType the character in.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public override bool Execute(InputlogDocument doc)
        {
            bool changed = false;
            if (IncludeInReplay)
            {
                doc.Selection.SetRange(WordKey.Position, WordKey.Position); // Make sure we are on the correct position

                if (WinKey.Key == KeysEx.VK_BACK)
                {
                    // If control was pressed, IncludeInReplay should be false and thus this is handled already
                    _deletedText = doc.Range(WordKey.Position - 1, WordKey.Position).Text;
                    doc.Selection.TypeBackspace();

                    changed = !string.IsNullOrEmpty(_deletedText);
                }
                else if (WinKey.Key == KeysEx.VK_RETURN)
                {
                    if (WinKey.KeyboardState.Contains(KeysEx.VK_SHIFT)
                        || WinKey.KeyboardState.Contains(KeysEx.VK_RSHIFT)
                        || WinKey.KeyboardState.Contains(KeysEx.VK_LSHIFT))
                    {
                        // TODO: Word 2007 uses this character to denote shift + return, does this work in other versions?
                        doc.Selection.TypeText("\v");
                    }
                    else
                    {
                        doc.Selection.TypeParagraph();
                    }

                    changed = true;
                }
                else
                {
                    doc.Selection.TypeText(WinKey.Value);
                    changed = !string.IsNullOrEmpty(WinKey.Value);
                }
            }

            return changed;
        }

        /// <summary>
        /// Undoes the command in the given Document.
        /// This method should only be called after the edit was executed on the same document.
        /// </summary>
        /// <param name="doc">The document where to undo the command in.</param>
        /// <returns>True if the execution changed the document state (change in selection, content has changed, ...), false if not.</returns>
        public override bool Undo(InputlogDocument doc)
        {
            bool changed = false;

            if (IncludeInReplay)
            {
                if (WinKey.Key == KeysEx.VK_BACK)
                {
                    doc.Range(WordKey.Position - 1, WordKey.Position - 1).Range.Text = _deletedText;

                    changed = !string.IsNullOrEmpty(_deletedText);
                }
                else if (WinKey.Key == KeysEx.VK_RETURN)
                {
                    doc.Selection.SetRange(WordKey.Position + 1, WordKey.Position + 1);
                    doc.Selection.TypeBackspace();

                    changed = true;
                }
                else
                {
                    doc.Selection.SetRange(WordKey.Position + WinKey.Value.Length, WordKey.Position + WinKey.Value.Length);
                    foreach (var chr in WinKey.Value)
                    {
                        doc.Selection.TypeBackspace();
                    }

                    changed = !string.IsNullOrEmpty(WinKey.Value);
                }
            }

            return changed;
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
			// 20120426
			// Support NormalProductionRevisions (needs no changes as NormalProductionRevision c InsertRevision)

            return ((IsDeletion && rev is DeleteRevision || !IsDeletion && rev is InsertRevision)
                && (rev.CurrentPOU == AbstractRevision.Defaultval || rev.CurrentPOU == WordKey.Position));
        }

        /// <summary>
        /// Adds the edit to the given RevisionAnalysisSummary. It check whether there is a current
        /// revision and whether the edit belongs to it (using BelongsTo). If there is not current
        /// revision or if the edit does not belong to it, a new revision of type RevisionType will
        /// created and the edit will be added to that revision, otherwise, the edit will be added
        /// to the current revision.
        /// </summary>
        /// <param name="summary">The RevisionAnalysisSummary that contains the current
        /// state of the RevisionAnalysis.</param>
        public override void AddTo(RevisionAnalysisSummary summary)
        {
			// If the typechar is not relevant in replay it becomes an 'Ignored' edit type.
			if (!WordKey.IncludeInReplay)
			{
				Type = EditType.Ignored;
				summary.AddUnrelevantEdit(this);
			}
			else
			{
                base.AddTo(summary);
			}
        }

		/// <summary>
		/// This method returns the effect of an edit. This may be either the text that has been added by the edit,
		/// the text that has been removed or text that has been selected. Only call this method after the 'Execute' method
		/// has been called.
		/// </summary>
		/// <returns>A string containing the text that is the focus of this Edit.</returns>
		public override string Effect()
		{
		    if (IsDeletion)
			{
				return _deletedText;
			}
		    return WinKey.Value;
		}

        #region Xml Serialization Infrastructure
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("DeletedText", _deletedText);
            writer.WriteStartElement("WinKey");
            WinKey.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteStartElement("WordKey");
            WordKey.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            _deletedText = reader.ReadElementString("DeletedText");
            reader.ReadStartElement("WinKey");
            WinKey.ReadXml(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("WordKey");
            WordKey.ReadXml(reader);
            reader.ReadEndElement();
        }
        #endregion
    }
}