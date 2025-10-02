using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;
using System.Xml;
using System;
using InputLog.Core.Analyses.Revision.Revisions.Edits;
namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols
{
    /// <summary>
    /// This class is made to be put into a list so that the S-notation or W-notation can be build. Each node contains 
    /// a symbol string either representing a character in the produced text or a markup in S-notation or W-notation syntax
    /// to stand for breaks, insertions and deletions, and the number of postions it represents in the produced text.
    /// If the symbol of the node is a character of the produced text that is not deleted, this number of positions is 1,
    /// otherwise (if the character was deleted or the symbol represents S-/W-notation markup), the number of positions is 0.
    /// </summary>
    public class SymbolNode
    {
        #region Fields
        /// <summary>
        /// The number of positions this node takes in in a string.
        /// Typically, this is either 1 (if the node contains a character that is not (yet) deleted)
        /// or 0 (if the node contains a deleted symbol or if it contains a meta symbol, i.e. a markup).
		/// A deleted symbolnode will have a position count of 0.
        /// </summary>
        public int PositionCount { get; set; }

        /// <summary>
        /// The actual symbol to display.
        /// </summary>
        public string Symbol { get; set; }
        public char SymbolChar { get; set; }

		/// <summary>
		/// The corresponding edit, responsible for this Symbols creation. 
		/// The edit MAY BE NULL. In the case of a document that had original text in it before
		/// the start of the editing process, the original text will be added as symbols to the list without
		/// those symbols originating from an edit.
		/// Likewise, MarkupNodes inheriting from SymbolNode will have a NULL edit as they are not produced by
		/// edits, and do not represent actual production of text.
		/// </summary>
		public IEdit Edit { get; private set; }

		/// <summary>
		/// Snapshot of the revisions current state. This is contextual information
		/// can be used better construct any notations using the SymbolNodes.
		/// </summary>
		public AbstractRSnapshot Revision { get; protected set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="positionCount">The number of character positions this node will represent 
        /// (see the doc of PositionCount for more information).</param>
        /// <param name="symbol">The actual symbol to contain. (The string/character(s) this node represents.)</param>
		/// <param name="edit">The edit responsible for the creation of this symbol.</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
        protected SymbolNode(int positionCount, string symbol, IEdit edit, AbstractRSnapshot snapshot = null)
        {
            PositionCount = positionCount;
            Symbol = symbol;
			Revision = snapshot;
			Edit = edit;
        }

        /// <summary>
        /// Constructor, constructs a new single node with a symbol containing a single character.
        /// The PositionCount will be set to 1.
        /// </summary>
        /// <param name="symbol">The actual symbol to contain. (The character this node represents.)</param>
		/// <param name="edit">The edit responsible for the creation of this symbol.</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
        public SymbolNode(char symbol, IEdit edit, AbstractRSnapshot snapshot = null)
        {
            PositionCount = 1;
            Symbol = symbol.ToString();
            SymbolChar = symbol;
            Revision = snapshot;
			Edit = edit;
        }

        /// <summary>
        /// Default constructor for use with serialization
        /// Constructor should be public, not protected.
        /// </summary>
        public SymbolNode()
        {
        }

        /// <summary>
        /// Returns the string representation of this object. The string representation consists solely of the contained Symbol.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return Symbol;
        }

        /// <summary>
        /// Call this method to represent that the symbol contained by this node is deleted from the produced text.
        /// </summary>
        public void Delete()
        {
            PositionCount = 0;
        }

        #region Xml Serialization Infrastructure
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("PositionCount", PositionCount.ToString());
            writer.WriteElementString("Symbol", Symbol);
            if (Revision != null)
            {
                writer.WriteStartElement("Revision");
                writer.WriteElementString("FullType", Revision.GetType().FullName);
                Revision.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (Edit != null)
            {
                writer.WriteStartElement("Edit");
                writer.WriteElementString("FullType", Edit.GetType().FullName);
                Edit.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public virtual void ReadXml(XmlReader reader)
        {
            PositionCount = Int32.Parse(reader.ReadElementString("PositionCount"));
            Symbol = reader.ReadElementString("Symbol");
            if (reader.IsStartElement("Revision"))
            {
                reader.ReadStartElement("Revision");
                string TypeStr = reader.ReadElementString("FullType");
                Revision = (AbstractRSnapshot)Activator.CreateInstance(Type.GetType(TypeStr));
                Revision.ReadXml(reader);
                reader.ReadEndElement();
            }
            if (reader.IsStartElement("Edit"))
            {
                reader.ReadStartElement("Edit");
                string TypeStr = reader.ReadElementString("FullType");
                Edit = (IEdit)Activator.CreateInstance(Type.GetType(TypeStr));
                Edit.ReadXml(reader);
                reader.ReadEndElement();
            }
        }
        #endregion
    }
}