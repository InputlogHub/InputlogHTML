using System;

namespace Server
{
    /// <summary>
    /// Represents the Result of a Task and is passed back to the TaskManager after the Task was finished.
    /// Currently contains just the Task identifier.
    /// </summary>
    class TaskResult
    {
        public string Guid { get; private set; }
        public enum ResCode 
        {
            COMPLETED,
            PARTIALLY_COMPLETED,
            FAILED
        }

        public ResCode Result
        {
            get;
            private set; 
        }

        public TaskResult(string guid, ResCode resultCode)
        {
            Guid = guid;
            Result = resultCode;
        }
    }
}
