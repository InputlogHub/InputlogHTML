using System;
using System.Collections.Generic;
namespace InputLog.Core.IO.CSV
{
    /// <summary>
    /// Interface for line getters for CSVWriters.
    /// </summary>
    public interface ICSVLineGetter: IEnumerator<Dictionary<string, string>>
    {
        IReadOnlyCollection<string> Headers { get; }
        bool HasNext();
    }
}
