namespace InputLog.Core.Util.Progress
{
    /// <summary>
    /// Basic implementation of the IProgressTrackableAction.
    /// </summary>
    public class ProgressTrackableAction : IProcessTrackableAction
    {
        /// <summary>
        /// Event delegates that will receive notifications during the different steps of the process.
        /// </summary>
        public event ProgressListener ProcessListeners;

        /// <summary>
        /// The (approximate) number of steps it will take for the action to finish.
        /// This can be used to estimate how many times the ProgressListener-callback
        /// will be called (e.g. to initialize a progressbar).
        /// </summary>
        public int NumberOfSteps { get; set; }

        /// <summary>
        /// Reports progress to the ProgressListeners.
        /// </summary>
        /// <param name="sender">Sender of the ProgressEvent.</param>
        /// <param name="args">Arguments of the ProgressEvent.</param>
        public virtual void ReportProgress(object sender, ProgressEventArgs args)
        {
            if (ProcessListeners != null)
            {
                ProcessListeners(sender, args);
            }
        }
    }
}