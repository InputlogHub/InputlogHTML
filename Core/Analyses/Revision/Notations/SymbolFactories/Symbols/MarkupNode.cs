using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots;
using System.Xml;
using System;
namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols
{
    /// <summary>
    /// Represents a symbol containing markup. As markup symbols are meta characters, they do not add value
    /// to the text length / position. As such, the PositionCount property will be 0.
    /// </summary>
    public class MarkupNode : SymbolNode
    {
        /// <summary>
        /// The sequence number of the MarkupNode (break number, ...) if there is one, null otherwise.
        /// </summary>
        public int? SequenceNumber { get; protected set; }

		/// <summary>
		/// Returns whether or not the markup in this node should be numbered markup or not. E.g. in 
		/// S-Notation the end of a delete or insert has it's sequence number following it, thus it is
		/// a numbered markup, likewise for the break.
		/// </summary>
		public virtual bool NumberedMarkup { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="symbol">The markup symbol to contain. (The string/character(s) this node represents.)</param>
		/// <param name="snapshot">Snapshot of the revisions current state, of the revision this symbol belongs too.</param>
		/// <param name="numberedMarkup">Whether the markup has extra numbering information in its string presentation or not.</param>
		public MarkupNode(string symbol, AbstractRSnapshot snapshot = null, bool numberedMarkup = false)
			: base(0, symbol, null, snapshot)
		{
			if (snapshot == null)
			{
				numberedMarkup = false;
				SequenceNumber = null;
			}
			else
			{
				NumberedMarkup = numberedMarkup;
				SequenceNumber = snapshot.RevisionNumber;
			}
		}

        /// <summary>
        /// Default constructor for use with serialization
        /// </summary>
        public MarkupNode()
        {
        }

		/// <summary>
		/// Function that determines whether given node can jump over the 
		/// target markup node or not. 
		/// </summary>
		/// <param name="target">MarkupNode we wish to jump over.</param>
		/// <returns>True if this node can jump over the target, false if not.</returns>
		public virtual bool JumpsOver(SymbolNode target)
		{
			return false;
		}

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("NumberedMarkup", NumberedMarkup.ToString());
            if (SequenceNumber != null) writer.WriteElementString("SequenceNumber", SequenceNumber.ToString());
            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            NumberedMarkup = bool.Parse(reader.ReadElementString("NumberedMarkup"));
            if (reader.IsStartElement("SequenceNumber"))
                SequenceNumber = Int32.Parse(reader.ReadElementString("SequenceNumber"));
            base.ReadXml(reader);
        }

    }
}
