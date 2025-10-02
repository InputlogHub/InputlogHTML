namespace InputLog.Core.Util.CommandProcessing
{
    /// <summary>
    /// Basic interface for commands, defines the execute method
    /// to execute the command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}