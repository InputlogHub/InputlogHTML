using InputLog.Core.Pipes;

namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer
{
    /// <summary>
    /// Abstract Tokenizer class, inherits a BaseProcess.
    /// </summary>
    public abstract class Inspector : BaseProcess
    {
        public abstract void AddRegexDefinition();
    }
}
