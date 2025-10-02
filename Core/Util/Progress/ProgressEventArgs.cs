using System;

namespace InputLog.Core.Util.Progress
{
    /// <summary>
    /// Represent progress event arguments.
    /// Contains a string with a message specifying details about the event.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
		/// <summary>
		/// Progress Codes - This code tells gives extra
		/// information about the ProgressEvent messages.
		/// </summary>
		public enum ProgressCode
		{
			NOT_SET,
			STARTED,
			STEP_COMPLETED,
			FAILED,
			DONE,
		};

        /// <summary>
        /// A message from the originator about the completed step.
        /// </summary>
        public string Message { internal set; get; }

		/// <summary>
		/// The code describing the kind of progress that was made.
		/// </summary>
		public ProgressCode Code { internal set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProgressEventArgs(string message, ProgressCode code = ProgressCode.NOT_SET)
        {
            Message = message;
			Code = code;
        }
    }
}