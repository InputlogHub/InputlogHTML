using System;
using System.Xml;

namespace InputLog.Core.Analyses.Revision.Notations.SymbolFactories.RevisionSnapshots
{
	/// <summary>
	/// The different types of snapshot available.
	/// </summary>
	public enum SnapshotTypes { UNKNOWN, NORMAL, INSERT, DELETE };

	/// <summary>
	/// Snapshot of a revision that counts the number of active characters currently in a 
	/// revision, during a reconstructive simulation of the revisions.
	/// </summary>
	public abstract class AbstractRSnapshot
	{
		#region fields
		/// <summary>
		/// The revision number of the revision this snapshot is presenting.
		/// </summary>
		public int RevisionNumber { get; private set; }

		/// <summary>
		/// The number of characters that are still 'active' in the revision. 
		/// Active characters are characters that still have a positive position count.
		/// </summary>
		protected int Count { get; set; }
		#endregion

		/// <summary>
		/// Construct a new snapshot of a revision with given revisionNumber
		/// </summary>
		/// <param name="revisionNumber">ID of the revision</param>
		protected AbstractRSnapshot(int revisionNumber)
		{
			RevisionNumber = revisionNumber;
		}

        /// <summary>
        /// Default constructor for use with serialization.
        /// Constructor should be public, not protected.
        /// </summary>
        public AbstractRSnapshot()
        {
        }

		/// <summary>
		/// Increases the number of active characters in the revision by 1. 
		/// An active character is a character that has a position count greater than 0.
		/// </summary>
		public void AddCharacter() 
		{
			UpdateCount(1);
		}

		/// <summary>
		/// Increases the number of active characters in the revision by count. 
		/// An active character is a character that has a position count greater than 0.
		/// </summary>
		/// <param name="count">count must be greater than or equal to 0</param>
		public void AddCharacters(int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("Can not add a negative amount of characters to a snapshot");
			}
			UpdateCount(count);
		}

		/// <summary>
		/// Decreases the number of active characters in the revision by 1. 
		/// An active character is a character that has a position count greater than 0.
		/// </summary>
		public void DeleteCharacter()
		{
			if (Count > 0)
			{
				UpdateCount(-1);
			}
			else
			{
				throw new InvalidOperationException("No characters left in deletion to delete, negative value not allowed.");
			}
		}

		/// <summary>
		/// Decreases the number of active characters in the revision by count. 
		/// An active character is a character that has a position count greater than 0.
		/// </summary>
		/// <param name="count">count must be greater than or equal to 0</param>
		public void DeleteCharacters(int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("Can not delete a negative amount of characters.");
			}
			if (count > 0 && Count - count >= 0)
			{
				UpdateCount(-count);
			}	
			else
			{
                throw new InvalidOperationException("No characters left in deletion to delete, negative value not allowed.");
			}
		}

		/// <summary>
		/// Update the count by adding the specified amount.
		/// </summary>
		/// <param name="add">Amount to add to the count</param>
		protected virtual void UpdateCount(int add)
		{
			Count += add;
			if (Count < 0)
			{
				throw new ArgumentException("Can not decrease count past 0");
			}
		}

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("RevisionNumber", RevisionNumber.ToString());
            writer.WriteElementString("CharCount", Count.ToString());
        }

        public void ReadXml(XmlReader reader)
        {
            RevisionNumber = int.Parse(reader.ReadElementString("RevisionNumber"));
            Count = int.Parse(reader.ReadElementString("CharCount"));
        }
	}
}
