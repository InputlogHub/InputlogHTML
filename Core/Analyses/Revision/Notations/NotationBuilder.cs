using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Analyses.Revision.Revisions;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;
using InputLog.Core.Analyses.Revision.Revisions.Edits;

namespace InputLog.Core.Analyses.Revision.Notations
{
    /// <summary>
    /// Helper class to build the representation of the revisions using a given syntax (represented by a SymbolFactory).
    /// </summary>
    internal class NotationBuilder
    {
        #region Fields
        /// <summary>
        /// The main list with the symbols.
        /// </summary>
        public LinkedList<SymbolNode> Symbols { get; }

        /// <summary>
        /// The factory that defines the syntax that is used to build the representation of the revisions.
        /// </summary>
        public ISymbolFactory Syntax { get; }

        /// <summary>
        /// The current position in the text.
        /// </summary>
        private LinkedListNode<SymbolNode> _current;

		/// <summary>
		/// A list that keeps all the Snapshots currently present. The key of the
		/// snapshot object is the revisionNumber of the RevisionSnapshot.
		/// </summary>
		private readonly Dictionary<int, AbstractRSnapshot> _snapshots;
        /// <summary>
        /// The file now being processed.
        /// </summary>
        private readonly string _file ;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="syntax">The syntax that defines which notation is used (W-notation, S-notation, ...).</param>
        /// <param name="filePath">The file now being processed.</param>
        /// <param name="originalContent">The original content in the document before any changes have been made to the document</param>
        public NotationBuilder(ISymbolFactory syntax, string filePath, string originalContent = null)
        {
            Symbols = new LinkedList<SymbolNode>();
			_snapshots = new Dictionary<int, AbstractRSnapshot>();
            Syntax = syntax;
            _file = filePath;

			// The initial snapshot, is a normal production snapshot.
			var initialSnap = new NormalProductionSnapshot(0);
			_snapshots.Add(0, initialSnap);

			// Initialize the notation builder with the same content the document originally contained
			// before the editing started.
			if (originalContent != null)
			{
				AppendText(originalContent, initialSnap, true);
			}
        }

        /// <summary>
        /// InsertText. Space separators are changed into a middle dot to enhance readability.
        /// Line and paragraph separators are replaced by a line feed to accomodate a shallow parser (NLP-tool).
        /// 
        /// Characters are added without markup symbols when a normal production revision is encountered. A normal production
        /// is an Insert where the position is at the end of the document and hence is not considered to be
        /// a revision but a normal text production (optionally followed by trailing whitespace).
        /// 
        /// </summary>
        /// <param name="rev"></param>
		/// 
		public void InsertText(InsertRevision rev)
		{
			// Find the correct insert positions.
			LinkedListNode<SymbolNode> position;
			try
			{
				position = SkipToPosition(rev.Position, rev is NormalProductionRevision);
			}
			catch (ArgumentOutOfRangeException exc)
			{
                _current = null;
                throw new ArgumentOutOfRangeException($"The insertion position {rev.Position} lies beyond" +
                                                      $" the current text length.The POU is at position {rev.CurrentPOU}", exc);
			}

			// Get or Create the snapshot for the insertion.
			AbstractRSnapshot snapshot = GetSnapshot(rev);

			if (!(rev is NormalProductionRevision))
			{
				// If revision is not NormalProduction put a break symbol.
				PutBreak(_current, snapshot);

				// Insert the text.
				// As insert-markup cannot jump any other markup anyway we can just add them
				// in an orderly fashion without any difficulties.
				InsertBefore(Syntax.GetLeftInsertionSymbol(snapshot), position, snapshot);
				InsertBefore(rev, position, snapshot);
				InsertBefore(Syntax.GetRightInsertionSymbol(snapshot), position, snapshot);

				// Update Current
				_current = position.Previous ?? Symbols.First;
			}
			// NormalProduction, don't put a break symbol.
			else
			{
				// It is an initial insertion, just insert the text as last nodes.
				// If it is an insertion at the end of the text, just append as last nodes.
				if (position == null)
					// || position == Symbols.Last) // This is commented out to support the \r that is present by standard.
				{
					AppendText(rev.Text, snapshot);
					_current = Symbols.Last;
				}
				// There is trailing whitespace in following nodes, insert before trailing whitespace.
				else
				{
					InsertBefore(rev, position, snapshot);
					_current = position.Previous ?? Symbols.First;
				}
			}
		}

		/// <summary>
		/// Put a break symbol at the insertion point.
		/// </summary>
		/// <param name="insertionPoint">Insertion point for the break symbol, the break symbol will be added somewhere
		/// after the insertionpoint, according to the rules of break jumping (See BreakNode.JumpsOver()).</param>
		/// <param name="currentSnapshot">Snapshot of the current revision for contextual information.</param>
		private void PutBreak(LinkedListNode<SymbolNode> insertionPoint, AbstractRSnapshot currentSnapshot)
		{
			var breakNode = (BreakNode)Syntax.GetBreakSymbol(currentSnapshot);
			bool insertedMarkup = false;

			// There is no Current position yet, most likely we have an Insert as the first revision in a document
			// that was initially already filled with text. We put the break at the end.
			if (insertionPoint == null)
			{
				SymbolsAddLast(breakNode);
				return;
			}

			// If the next node in line is not a markupNode we just add the break behind the insertionPoint
			while(insertionPoint.Next != null && !insertedMarkup)
			{
			    // The next node is a markupNode, let's see if the breakSymbol can jump over any markupNodes.
			    var node = insertionPoint.Next.Value as MarkupNode;
			    if(node != null)
				{
					var markup = node;
					if (breakNode.JumpsOver(markup))
					{
						insertionPoint = insertionPoint.Next;
					}
					else
					{
						// Can't jump any more, insert break and leave loop.
						SymbolsAddAfter(insertionPoint, breakNode);
						insertedMarkup = true;
					}
				} 
				// Next is not a markupNode, insert break after the current node and leave loop
				else 
				{
					SymbolsAddAfter(insertionPoint, breakNode);
					insertedMarkup = true;
				}
			}

		    // There are no next nodes, either we have inserted the markup or we haven't yet.
			// In the last case we still have to add the break symbol as last element in the Symbols list.
			if (!insertedMarkup)
			{
				SymbolsAddLast(breakNode);
			}
		}

		/// <summary>
		/// Gets a snapshot for given revision. Or if the snapshot does not exist yet it gets created, 
		/// if and only if the type is also specified. If the snapshot does not exist and no type is
		/// given this function will return null.
		/// </summary>
		/// <param name="rev">Revision linked to the snapshot</param>
		/// <returns>The snapshot linked to the revision, or null if the revision was of an invalid type.</returns>
		private AbstractRSnapshot GetSnapshot(IRevision rev)
		{
		    if (!_snapshots.ContainsKey(rev.RevisionNumber))
			{
			    AbstractRSnapshot snapshot;
			    if(rev is NormalProductionRevision)
				{
					snapshot = new NormalProductionSnapshot(rev.RevisionNumber);
				} 
				else if (rev is InsertRevision) 
				{
					snapshot = new ProductionSnapshot(rev.RevisionNumber);
				} 
				else
				{
				    var revision = rev as DeleteRevision;
				    if (revision != null)
				    {
				        snapshot = new DeletionSnapshot(rev.RevisionNumber, revision.TotalLength);
				    } 
				    else 
				    {
				        throw new ArgumentException($"Revision {rev.RevisionNumber} is unknown - " +
				                                    "Cannot create a fitting snapshot for revision.");
				    }
				}
			    _snapshots.Add(rev.RevisionNumber, snapshot);
			}

			return GetSnapshot(rev.RevisionNumber);
		}

		/// <summary>
		/// Return a snapshot based on the revisionNumber. If the snapshot does not exist
		/// yet, this function will throw an ArgumentException.
		/// </summary>
		/// <param name="revisionNumber">RevisionNumber of the RevisionSnapshot to return.</param>
		/// <returns>The snapshot linked to the revision with given revisionNumber.</returns>
		private AbstractRSnapshot GetSnapshot(int revisionNumber)
		{
		    if (!_snapshots.ContainsKey(revisionNumber))
			{
				throw new ArgumentException ($"No snapshot with revisionNumber {revisionNumber} exists, " +
				                             "cannot request new Snapshots on solely the revisionNumber");
			}
		    return _snapshots[revisionNumber];
		}

        /// <summary>
		/// Adds the given text at the end of the symbol list.
		/// <b>Only use this method to add the text that was present in a document BEFORE the start of the
		/// editing. This should not be used to add text produced by edits on the document!!</b>
		/// All symbols from the given text get an explicit 'noEdit'-edit to simplify the construction of data grids in an Linguistic of
		/// WourdPause analysis.
		/// </summary>
		/// <param name="text">Text to be added.</param>
		/// <param name="snapshot"></param>
		/// <param name="overrideSmartLogic">Boolean that overrides the special behaviour reserved for adding
		/// as 'last' symbol. When override_smart_logic is false a symbol is only added as last when the current
		/// last symbol is not a \r. Otherwise it adds it before the last character. Setting override_smart_logic to true
		/// overrides this behaviour. Overriding this behaviour is usefull for initialization.</param>
		private void AppendText(string text, AbstractRSnapshot snapshot, bool overrideSmartLogic = false)
		{
			if (snapshot is ProductionSnapshot)
			{
				snapshot.AddCharacters(text.Length);
			}
			else
			{
				throw new ArgumentException("Text can only be inserted in ProductionSnapshots, " +
											"given snapshot is not a ProductionSnapshot.");
			}

            IEdit noEdit = new NoEdit();

            foreach (var chr in text)
			{
				var charCat = char.GetUnicodeCategory(chr);
				switch (charCat)
				{
					// Converting all line separators into a line feed.
					case System.Globalization.UnicodeCategory.LineSeparator:
						SymbolsAddLast(new SymbolNode((char)10, noEdit, snapshot), overrideSmartLogic);
						break;
					// Converting all paragraph separators into a line feed.
					case System.Globalization.UnicodeCategory.ParagraphSeparator:
						SymbolsAddLast(new SymbolNode((char)10, noEdit, snapshot), overrideSmartLogic);
						break;
					// Changing space representations into 'middle dot'
					case System.Globalization.UnicodeCategory.SpaceSeparator:
						SymbolsAddLast(new SymbolNode((char)183, noEdit, snapshot), overrideSmartLogic);
						break;
                    // Changing Unicode Control into 'middle dot'
                    case System.Globalization.UnicodeCategory.Control:
                        SymbolsAddLast(new SymbolNode((char)183, noEdit, snapshot), overrideSmartLogic);
                        break;
					default:
						SymbolsAddLast(new SymbolNode(chr, noEdit, snapshot), overrideSmartLogic);
						break;
				}
			}
		}

        /// <summary>
        /// Insert given text before the given node.
        /// </summary>
        /// <param name="rev">Revision containing the text being inserted.</param>
        /// <param name="insertionPoint">Node in the symbolNode list before which the text is inserted.</param>
        /// <param name="snapshot"></param>
        private void InsertBefore(InsertRevision rev, LinkedListNode<SymbolNode> insertionPoint, AbstractRSnapshot snapshot)
		{
            var productionSnapshot = snapshot as ProductionSnapshot;
            if (productionSnapshot != null)
			{
			    ProductionSnapshot parent = insertionPoint.Value.Revision as ProductionSnapshot;
			    if (parent != null && !(snapshot is NormalProductionSnapshot))
				{
					productionSnapshot.Link(parent);
				}
                // A 'Return' occupies one position in the idfx, but Microsoft renders it with two characters
                // this confuses the revision and edit machinery.
                if (rev.Text.Contains("\r\n"))
			    {
                    productionSnapshot.AddCharacters(rev.Text.Length - 1);
                }
			    else
			    {
                    productionSnapshot.AddCharacters(rev.Text.Length);
                }
			   
			}
			else
			{
				throw new ArgumentException("InsertionPoint Symbol should always belong to a ProductionSnapshot.");
			}

            // We should not get any deletes in this branch of the code.
            foreach (IEdit edit in rev.GetRelevantEdits())
			{
				string content;
			    var c = edit as TypeChar;
			    if (c != null)
				{
					content = c.WinKey.Value;

                }
				else
			    {
			        var insertion = edit as Insertion;
			        if (insertion != null)
			        {
			            content = insertion.Text;
			        }
			        else
			        {
			            // This shouldn't occur...
			            continue;
			        }
			    }

			    foreach (var chr in content)
				{
				    var charCat = content.Equals(Environment.NewLine) ? UnicodeCategory.ParagraphSeparator : char.GetUnicodeCategory(chr);
                   
					switch (charCat)
					{
						// Converting all line separators into a line feed.
						case System.Globalization.UnicodeCategory.LineSeparator:
							SymbolsAddBefore(insertionPoint, new SymbolNode((char)10, edit, snapshot));
							break;
						// Converting all paragraph separators into a line feed.
						case System.Globalization.UnicodeCategory.ParagraphSeparator:
							SymbolsAddBefore(insertionPoint, new SymbolNode((char)10, edit, snapshot));
							break;
						// Changing all space representations into 'middle dot'
						case System.Globalization.UnicodeCategory.SpaceSeparator:
							SymbolsAddBefore(insertionPoint, new SymbolNode((char)183, edit, snapshot));
							break;
                        // Changing Unicode Control into 'middle dot'
                        case System.Globalization.UnicodeCategory.Control:
                            SymbolsAddLast(new SymbolNode((char)183, edit, snapshot));
                            break;
						default:
							SymbolsAddBefore(insertionPoint, new SymbolNode(chr, edit, snapshot));
							break;
					}
                    // Preventing to read the second char of the "\r\n" symbol. It would as a consequence insert 
                    // extra content in the node.
				    if (content.Contains(Environment.NewLine))
				    {
				        break;
				    }
				}
			}
		}

        /// <summary>
        /// Insert a symbol node before a certain position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="insertionPoint">Node in the symbolNode list before which the text is inserted.</param>
        /// <param name="snapshot"></param>
        private void InsertBefore(SymbolNode node, LinkedListNode<SymbolNode> insertionPoint, AbstractRSnapshot snapshot)
		{
            var productionSnapshot = snapshot as ProductionSnapshot;
            if (productionSnapshot != null)
			{
			    var parent = insertionPoint.Value.Revision as ProductionSnapshot;
			    if (parent != null)
				{
					productionSnapshot.Link(parent);
				}
			}
            snapshot.AddCharacters(node.PositionCount);
			SymbolsAddBefore(insertionPoint, node);
		}

        /// <summary>
        /// Adds the node at the end of the list, except if the last node is a \r, 
        /// in which case it adds the node before the \r node.
        /// </summary>
        /// <param name="node">Node to add to the list.</param>
        /// <param name="overrideSmartLogic">Boolean that overrides the special behaviour reserved for adding
        /// as 'last' symbol. When override_smart_logic is false a symbol is only added as last when the current
        /// last symbol is not a \r. Otherwise it adds it before the last character. Setting override_smart_logic to true
        /// overrides this behaviour. Overriding this behaviour is usefull for initialization.</param>
        /// 
        private void SymbolsAddLast(SymbolNode node, bool overrideSmartLogic = false)
        {
            if (Symbols.Last != null)
            {
                if (!overrideSmartLogic && Symbols.Last.Value.Symbol.Equals("\r"))
                {
                    Symbols.AddBefore(Symbols.Last, node);
                }
                else if (Symbols.Last.Value.Symbol.Equals("\u00B7") && node.Symbol.Equals("\u00B7"))
                {
                    // Preventing duplicate middle dots to accumulate at the end of the symbol list.
                    // Not clear at this moment why it happens and no time to find out.
                    if (!Symbols.Last.Value.Edit.Id.Equals(node.Edit.Id))
                    {
                        Symbols.AddLast(node);
                    }
                }
            }
            else
            {
                Symbols.AddLast(node);
            }
        }

        //private void SymbolsAddLast(SymbolNode node, bool overrideSmartLogic = false)
        //{
        //    if (!overrideSmartLogic && Symbols.Last != null && Symbols.Last.Value.Symbol == "\r")
        //    {
        //        Symbols.AddBefore(Symbols.Last, node);
        //    }
        //    else
        //    {
        //        Symbols.AddLast(node);
        //    }
        //}

        /// <summary>
        /// Add a node after a given insertionPoint, unless the insertionPoint is the last, and has value \r.
        /// </summary>
        /// <param name="insertionPoint">Point to insert to the value after</param>
        /// <param name="node">Node to insert.</param>
        private void SymbolsAddAfter(LinkedListNode<SymbolNode> insertionPoint, SymbolNode node)
		{
			if (insertionPoint.Value.Symbol == "\r" && insertionPoint == Symbols.Last)
			{
				Symbols.AddBefore(insertionPoint, node);
			}
			else
			{
				Symbols.AddAfter(insertionPoint, node);
			}
		}

		/// <summary>
		/// Add a node before given insertionPoint
		/// </summary>
		/// <param name="insertionPoint">Insertion point in the list</param>
		/// <param name="node">Node to be inserterd</param>
		private void SymbolsAddBefore(LinkedListNode<SymbolNode> insertionPoint, SymbolNode node)
		{
			Symbols.AddBefore(insertionPoint, node);
		}

        /// <summary>
        /// DeleteRange
        /// </summary>
        /// <param name="rev">Revision specifying the delete</param>
		/// 
		public void DeleteRange(DeleteRevision rev)
		{
			// Prelimanary check
			if (rev.RevisionNumber == 0)
			{
				throw new ArgumentException("Invalid revision number: " +
				                            "a delete revision can never have revision number 0.");
			}

			// Skip to the correct start position.
			LinkedListNode<SymbolNode> position;
			try
			{
				position = SkipToPosition(rev.Start,false);
			}
			catch (ArgumentOutOfRangeException exc)
			{
				_current = null;
				throw new ArgumentOutOfRangeException(
				    $"Bad file: '{_file}'. The start position {rev.Start} " +
				    $"of the range to-be-deleted of length {rev.TotalLength} " + "lies beyond the current text length.", exc);
			}

			LinkedListNode<SymbolNode> target = position;
			var snapshot = (DeletionSnapshot)GetSnapshot(rev);

			// Put the break symbol
			PutBreak(_current, snapshot);

			// Create the open/close tags.
			MarkupNode openTag = Syntax.GetLeftDeletionSymbol(snapshot);
			MarkupNode closeTag = Syntax.GetRightDeletionSymbol(snapshot);
			
			// Delete the range. This part deletes the range to the right, 
            // by jumping the closing tag over w/e can be
			// jumped and deleting all the normal characters it jumps over.
			// 
			// If the target node (=next node) is not null and there are still characters 
            // to be deleted, or the node can be skipped anyway
			// we enter the while loop, either deleting a character, or jumping over the markup.
			while
				(target != null 
                && (snapshot.ToDelete > 0 
                || (closeTag.JumpsOver(target.Value)))
				)
			{
				// We still have to delete characters, just jump markup and delete normal symbol nodes.
				if (snapshot.ToDelete > 0)
				{
					// Not markup, delete the node (set PositionCount to 0)
					// If the symbolNode has positionCount we set it to and substract it of our 'toDelete' in the snapshot
					if (!(target.Value is MarkupNode || target.Value.PositionCount == 0))
					{
						snapshot.DeleteCharacters(target.Value.PositionCount);
						target.Value.PositionCount = 0;
						target.Value.Revision.DeleteCharacter();
					}
				}
				target = target.Next;
			}

			// We cannot move any further to the right with the closing tag. A few options apply:
			// 1. We have deleted the entire range -> Just add the closing tag BEFORE the current position (= target) as we 
			//  could no longer jump any nodes, we cannot jump target, thus may not put After, and must put Before.
			//  -> if: target == null -> add as last symbol.
			// 2. We have not deleted the entire range, but target == null -> Exception.

			//1. 
			if (snapshot.ToDelete == 0)
			{
				if (target == null)
				{
					SymbolsAddLast(closeTag);
					_current = Symbols.Last;
				}
				else
				{
					SymbolsAddBefore(target, closeTag);
					_current = target.Previous ?? target;
				}
			}

			//2.
			else
			{
                throw new InvalidOperationException("DeleteRevision out of range\n" +
                                        "Range-to-delete to long for current revision: " + snapshot.RevisionNumber +
                                        " Chars to delete: " + snapshot.ToDelete + ". Break at position: " + rev.CurrentPOU);
            }

            // Place the left deletion tag, jumping over all markup that can be skipped.
            target = position;
			while (target != null && openTag.JumpsOver(target.Value))
			{
				target = target.Previous;
			}

			// We can no longer jump to the left, two reasons are possible:
			// 1. There are no more nodes to the left. -> Add the openTag as first symbol.
			// 2. There are still nodes to the left, but we can no longer jump them. -> Add OpenTag AFTER the target,
			//  as the target is already equal to the node that cannot be jumped to the left, we should add the tag to the right (=after)
			if (target == null)
			{
				Symbols.AddFirst(openTag);
			}
			else
			{
				SymbolsAddAfter(target, openTag);
			}
		}

        /// <summary>
        /// Returns the node containing the character on given position, taking into account
        /// that certain symbols may not add value to the position (markup symbols, deleted characters, ...)
        /// </summary>
        /// <param name="position">The position of the Node that should be returned.</param>
        /// <param name="isNormalProduction"></param>
        /// <returns>The node at the given position.</returns>
        private LinkedListNode<SymbolNode> SkipToPosition(int position, bool isNormalProduction)
		{
			var curNode = Symbols.First;
			var curPosition = 0;

			// Skip to position.
			if (curNode != null)
			{
				while (curPosition < position && curNode.Next != null)
				{
					curPosition += curNode.Value.PositionCount;
					curNode = curNode.Next;
				}
			}

			// Skip markup or deleted characters, but no insert tags should be passed here, 
            // unless they have an ActiveCharacter count of 0
		    try
		    {
		        while (curNode != null 
		               && (curNode.Next != null 
		                   && (isNormalProduction && curNode.Next.Value.PositionCount == 0) 
		                   || !((curNode.Value is InsertCloseTag || curNode.Value is InsertOpenTag) 
		                        && ((ProductionSnapshot) curNode.Value.Revision).ActiveCharacters > 0) 
		                   && curNode.Value.PositionCount == 0))
		        {
		            curNode = curNode.Next;
		        }
		    }
		    catch (InvalidCastException i)
		    {	        
		        throw new InvalidCastException("Cast to ProductionSnapshot is invalid\n" + i.StackTrace);
		    }

            // Test outcommented on Oct 14 2016 by EVH. Reason: the methods 'InsertText' (line 86) and 'DeleteRange (line 533)
            // have their own ArgumentOutOfRangeException and apparently more S-Notation files are generated without this one.
            //
            // Did we move far enough?
            //if ((position > 0 && curNode == null) || curNode != null && (curPosition < position && curNode.Next == null))
            //    {
            //    throw new ArgumentOutOfRangeException(position.ToString(),
            //        $"Bad file: '{_file}'. Insertion point {position} of node \"{curNode.Value}\" is beyond text length of {curPosition}.");
            //}

            return curNode;
		}

        /// <summary>
        /// Use this method to get the final representation of the revisions in the syntax given during construction.
        /// The 'skip' property ignores the symbols from text found in the original document but absent in the idfx of the current analysis.
        /// </summary>
        /// <returns>The final representation of the revisions using the syntax given during construction.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder(Symbols.Count);
            foreach (var symbol in Symbols)
            {
                if (symbol.SymbolChar.Equals((char)10) || symbol.SymbolChar.Equals((char)183))
                {
                    buf.Append("\u00B7");
                }
                else if (symbol.Edit == null || symbol.Edit.Type != EditType.Skip)
                {
                    buf.Append(symbol);
                }
            }
            return buf.ToString();
        }
    }
}