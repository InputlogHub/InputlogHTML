namespace InputLog.Core.Util.Progress
{
    /// <summary>
    /// Listener for Progress events.
    /// </summary>
    /// <param name="sender">The originater of the event.</param>
    /// <param name="eventArgs">Contains the data of the event.</param>
    public delegate void ProgressListener(object sender, ProgressEventArgs eventArgs);

    /// <summary>
    /// All classes implementing this interface should perform some kind of action that should be trackable by an external class.
    /// A typical use case is a UI that needs to update elements of its interface (e.g. ProgressBar) 
    /// while an other class performs work.
    /// The UI can than determine the number of steps the action will take by reading the NumberOfSteps property and use this 
    /// to initialize its UI elements.
    /// </summary>
    public interface IProcessTrackableAction
    {

        /// <summary>
        /// Event delegates that will receive notifications during the different steps of the process.
        /// </summary>
        event ProgressListener ProcessListeners;

        /// <summary>
        /// The (approximate) number of steps it will take for the action to finish.
        /// This can be used to estimate how many times the ProgressListener-callback will be called 
        /// (e.g. to initialize a progressbar).
        /// </summary>
        int NumberOfSteps { get; }

        /// <summary>
		/// Reports progress to the ProcessListeners.
        /// </summary>
        /// <param name="sender">Sender of the ProgressEvent.</param>
        /// <param name="args">Arguments of the ProgressEvent.</param>
        void ReportProgress(object sender, ProgressEventArgs args);
    }

	/// <summary>
	/// This interface is identical to the IProcessTrackableAction with a sole semantic difference.
	/// The nature of the processing done by an IPreprocessTrackableAction as opposed to its otherwise identical 
	/// twin is that the processing is Preprocessing. <br />
	/// This means that it can do preprocessing before any other actual processing is done. It can do preprocessing 
	/// followed by normal processing or not. But the distinction lies in the nature of the processing to be done.
	/// </summary>
	public interface IPreprocessTrackableAction
	{

		/// <summary>
		/// Event delegates that will receive notifications during the different steps of the process.
		/// </summary>
		event ProgressListener PreprocessListeners;

		/// <summary>
		/// The (approximate) number of steps it will take for the action to finish.
		/// This can be used to estimate how many times the ProgressListener-callback will be called 
		/// (e.g. to initialize a progressbar).
		/// </summary>
		int NumberOfSteps { get; }

		/// <summary>
		/// Reports progress to the PreprocessListeners.
		/// </summary>
		/// <param name="sender">Sender of the ProgressEvent.</param>
		/// <param name="args">Arguments of the ProgressEvent.</param>
		void ReportProgress(object sender, ProgressEventArgs args);
	}
}