using System;
using System.Collections;
using System.Collections.Generic;
using InputLog.Core.Events;
using InputLog.Core.IO;

namespace InputLog.Core.Preprocessing
{
	/// <summary>
	/// A class that preprocesses a list of event. Preprocessing the list of
	/// events may alter some of the events, as described by the preprocessor in question.
	/// </summary>
	public abstract class Preprocessor: IDisposable
	{
		#region Fields

		/// <summary>
		/// Indicates whether this instance of this class has been disposed.
		/// </summary>
		private bool Disposed;

		/// <summary>
		/// A user friendly name for this event filter.
		/// </summary>
		public string PreprocessorName;
		#endregion

	    /// <summary>
	    /// Preprocess the the eventList, this may alter the eventList. Events may be altered
	    /// or removed. Returns the processed list of events.
	    /// </summary>
	    /// <param name="inputEvents">Events to process (this list may be altered during the processing)</param>
        /// <param name="sessionId">SessionIdentification contains metadata such as the main document name.</param>
	    /// <returns>The processed list of events.</returns>
        public abstract List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId);

	    /// <summary>
	    /// Allows to control on file level if the filtering worked.
	    /// Implemented in EventIDFilter.
	    /// </summary>
	    /// <returns>ArrayList with the IDFxID (string), StartTime (ulong) and EndTime (ulong)</returns>
	    public virtual ArrayList GetFilterResult()
	    {
	        return null;
	    }

		 /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                //dispose
            }
            Disposed = true;
        }

        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// </summary>
		~Preprocessor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this class implements a finalizer.
            GC.SuppressFinalize(this);
        }

		/// <summary>
		/// Returns an extra info object that contains some extra information about this preprocessor.
		/// This can be used to inform the user of involved parameters of the preprocessors.
		/// </summary>
		public virtual IDictionary<string, IDictionary<string, object>> GetExtraInfo()
		{
			return null;
		}

	}
}
