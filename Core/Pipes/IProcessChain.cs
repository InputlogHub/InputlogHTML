
using System.Data;

namespace InputLog.Core.Pipes
{
    public interface IProcessChain
    {
        void Execute(DataSet linguisticProcess);
        IProcessChain Register(IProcess process);
    }
}
