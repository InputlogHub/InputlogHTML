
using System.Data;

namespace InputLog.Core.Pipes
{
    /// <summary>
    /// The pipeline manager is in charge of registering and executing the different processes. 
    /// Called by the WNotationSummary class.
    /// </summary>
    public class ProcessPipeline : IProcessChain
    {
        #region Fields
        // The processed material to return.
        public DataSet Output { get; private set; }

        // A registered operation.
        private IProcess Root;
        #endregion

        /// <summary>
        /// Instruction to execute the processes.
        /// </summary>
        /// <param name="linguisticProcess">The dataSet with the components to process.</param>
        public void Execute(DataSet linguisticProcess)
        {
            Root.Executing(linguisticProcess);
            Output = BaseProcess.ReturnValue;
        }

        /// <summary>
        /// Chaining the different operations in order.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public IProcessChain Register(IProcess process)
        {
            if (Root == null)
            {
                Root = process;
            }
            else
            {
                Root.Register(process);
            }
            return this;
        }
    }
}
