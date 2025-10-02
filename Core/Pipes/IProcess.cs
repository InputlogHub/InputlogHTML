using System.Data;

namespace InputLog.Core.Pipes
{
    /// <summary>
    /// Interface for a specific process.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Executing the process at hand.
        /// </summary>
        DataSet Executing(DataSet linguisticProcess);

        /// <summary>
        /// Registering the next process.
        /// </summary>
        /// <param name="nextProcess">The next process to register</param>
        void Register(IProcess nextProcess);
    }
}
