using System;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
using InputLog.Core.Plugin.WordLog;
using System.Text;

namespace InputLog.Core.Analyses.Revision.Revisions
{
    /// <summary>
    /// A revision in which a certain range is deleted.
    /// </summary>
    class DeleteRevision : AbstractRevision
    {
        #region Fields
        /// <summary>
        /// If all the edits would be executed, the range that would be deleted would start at this position.
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// If all the edits would be executed, the range that would be deleted would end at this position.
        /// </summary>
        public int End => Start + TotalLength - 1;

        /// <summary>
        /// Contains the total length of the range that would be deleted.
        /// </summary>
        public int TotalLength { get; private set; }

        /// <summary>
        /// Returns the current POU (Point Of Utterance).
        /// This is only a valid property if at least one Deletion or TypeChar was added to the revision,
        /// otherwise the property equals -1.
        /// </summary>
        public override int CurrentPOU => Start;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteRevision()
            : base(RevisionType.DELETE)
        {
            InputlogDocument.IsRevision = true;
            Start = Defaultval;
            TotalLength = Defaultval;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revisionnumber">The revision number of this revision.</param>
        public DeleteRevision(int revisionnumber)
            : base(revisionnumber, RevisionType.DELETE)
        {
            InputlogDocument.IsRevision = true;
            Start = Defaultval;
            TotalLength = Defaultval;
        }

        /// <summary>
        /// 20110830 Combining the length of consecutive deletes(backspace)
        /// Adds the given edit to the list of edits if it belongs to this revision, throws an exception otherwise.
        /// </summary>
        /// <param name="edit">The edit to add.</param>
        /// <param name="doc">The document in the state right before the edit is executed.
        /// When the control is returned, the edit will be executed on the document.</param>
        /// <exception cref="NotInRevisionException">Whenever the given edit does not belong in the revision.</exception>
        public override void Add(IEdit edit, InputlogDocument doc)
        {
            // Edit must be executed before it contains the correct information, length wise etc.
            var deletion = edit as Deletion;
            if (deletion != null)
			{
				base.Add(deletion, doc);

                var del = deletion;
                if (TotalLength == Defaultval) // This means no deletion was added yet, at most only SelectionChanges were added
                {
                    Start = del.StartPos;
                    TotalLength = del.Length;
                }
                else if (del.StartPos <= Start && Start <= del.EndPos)
                {
                    Start = Math.Min(del.StartPos, Start);
                    TotalLength += del.Length;
                }
                else
                {
                    throw new NotInRevisionException(
                        $"Given Deletion (range {del.StartPos}-{del.EndPos}) does not overlap with current start {Start}");
                }
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
                            base.Add(c, doc);

                            // If (control + ) delete or control + backspace was pressed,
                            // IncludeInReplay should was false and thus this is handled by another event.
                            // Therefore, we need only to take a regular backspace into account here.
                            // 'TotalLength == DEFAULTVAL' means no deletion was added yet, only SelectionChanges.
                            if (TotalLength == Defaultval)
                            {
                                // Initialize as an empty delete revision, starting at the current position with a length of 0.
                                Start = type.WordKey.Position;
                                TotalLength = 0;

                                // then delete the character before the cursor
                                DeletePreviousCharacter();
                            }
                            else
                            {
                                DeletePreviousCharacter();
                            }
                        }
                        else
                        {
                            throw new NotInRevisionException(
                                $"The given edit (of type TypeChar) is not a deletion: {type}");
                        }
                    }
                }
                else if (!(edit is SelectionChange))
                {
                    base.Add(edit, doc);
                    throw new NotInRevisionException("The given edit is not a Deletion, " +
                                                     $"SelectionChange or a TypeChar: {edit}");
                }
            }
        }

        /// <summary>
        /// Includes the character in front of this delete revision in this revision and thereby deletes it.
        /// If there is no such character, i.e. Start == 0, nothing happens.
        /// </summary>
        private void DeletePreviousCharacter()
        {
            if (Start <= 0) return;
            Start--;
            TotalLength++;
        }

		/// <summary>
		/// Returns the combined effect of all the edits in this revision.
		/// This may be for example, all the text that has been 
		/// added/deleted/selected.
		/// </summary>
		/// <returns>The combined effect of all events in this revision</returns>
		public override string Effect()
		{
			if (Edits.Count <= 0)
			{
				return "";
			}

			var sb = new StringBuilder();
			foreach (IEdit edit in Edits)
			{
				if(!(edit is SelectionChange)){
					sb.Append(edit.Effect());
				}
			}
			return sb.ToString();
		}
    }
}