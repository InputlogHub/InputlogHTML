using System.Xml;

namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Including the path to the final doc as an event
    /// </summary>
    public sealed class DocPath : IEventPart
    {
        public string FinalPath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fPath"></param>
        public DocPath(string fPath)
        {
            FinalPath = fPath;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public DocPath()
        {
        }
    }
}
