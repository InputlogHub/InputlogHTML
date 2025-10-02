using System.Data;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Pipes
{
    /// <summary>
    /// Abstract generic processing class to be inherited by the actual postprocessing classes.
    /// ProgressTrackableAction: a listener for Progress events to update the progress bar in the GUI.
    /// IProcess: interface for a process.
    /// </summary>
    public abstract class BaseProcess : ProgressTrackableAction, IProcess
    {
        #region Fields
        // Next process in a chain of operations.
        private IProcess _nextProcess;

        // The outcome of a process.
        public static DataSet ReturnValue { get; private set; }
        #endregion

        /// <summary>
        /// Abstract methods to be performed by the processes in the pipeline.
        /// </summary>
        /// <param name="linguisticProcess">The DataSet with results of the linguistic processing.</param>
        /// <returns>The DataSet with results added.</returns>
        protected abstract DataSet Process(DataSet linguisticProcess);

        /// <summary>
        /// The (recursive) processing pipeline.
        /// </summary>
        public DataSet Executing(DataSet linguisticProcess)
        {
            ReturnValue = Process(linguisticProcess);

            if (_nextProcess != null)
            {
                ReturnValue = _nextProcess.Executing(ReturnValue);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Registering the consecutive processes 
        /// </summary>
        /// <param name="aProcess">The next operation to register</param>
        public void Register(IProcess aProcess)
        {
            if (_nextProcess == null)
            {
                _nextProcess = aProcess;
            }
            else
            {
                _nextProcess.Register(aProcess);
            }
        }
    }
}
