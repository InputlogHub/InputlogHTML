namespace InputLog.Core.Analyses.Revision.Revisions
{
	/// <summary>
	/// A normal production revision is an insert revision at the end of the document or when
	/// the characters following the insert at the specified location in the document are solely 
	/// whitespace characters.
	/// </summary>
	class NormalProductionRevision: InsertRevision
	{
		public NormalProductionRevision(){}
		public NormalProductionRevision(int revisionNumber)
			: base(revisionNumber) {}
	}
}
