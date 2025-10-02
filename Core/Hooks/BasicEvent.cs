using System;

namespace InputLog.Core.Hooks
{
    /// <summary>
    /// Base of the eventz created by InputLogCore.
    /// Every BasicEvent has a sequence number that represents the 
    /// </summary>
    public class BasicEvent : EventArgs
    {
        #region Fields
        /// <summary>
        /// Sequence number of the next generated event.
        /// </summary>
        private static ulong _nextSeqNr;

        /// <summary>
        /// Lock used for protecting nextSeqNr against concurrent access.
        /// </summary>
        private static readonly object NextSeqNrLock = new object();

        /// <summary>
        /// Property that represents the sequence number of the next generated event.
        /// </summary>
        private static ulong NextSequenceNumber
        {
            get
            {
                lock (NextSeqNrLock)
                {
                    return _nextSeqNr++; // make it auto-increasing
                }
            }
        }

        /// <summary>
        /// The sequence number of the event.
        /// </summary>
        public ulong SequenceNumber { get; private set; }
        #endregion

        /// <summary>
        /// Constructor, constructs a new BasicEvent with a sequencnumber
        /// that is one unit higher than the last constructed BasicEvent.
        /// </summary>
        protected BasicEvent()
        {
            SequenceNumber = NextSequenceNumber;
        }
    }
}