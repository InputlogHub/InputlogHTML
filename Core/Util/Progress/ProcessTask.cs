namespace InputLog.Core.Util.Progress
{
	/// <summary>
	/// A task that can be executed with a run function. That is a 
	/// progress trackable action.
	/// </summary>
	public abstract class ProcessTask: ProgressTrackableAction
	{
		/// <summary>
		/// Do the process work... <br />
		/// The Run method must have a top-level ThreadAbortException
		/// catch clause that catches any ThreadAbortExceptions! <br />
		/// In the case of a ThreadAbortException the Process must be cleanly
		/// shut down by passing along a ProcessFailed event!
		/// </summary>
		public abstract void Run();
	}
}
