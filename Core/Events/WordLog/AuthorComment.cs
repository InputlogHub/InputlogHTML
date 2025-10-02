namespace InputLog.Core.Events.WordLog
{
    /// <summary>
    /// Short text provided by the author at the end of the current writing session.
    /// </summary>
    public sealed class AuthorComment : IEventPart
    {
        /// <summary>
        /// Content
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comment"> string with the comment text </param>
        public AuthorComment(string comment )
        {
            Comment = comment;
        }
        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public AuthorComment()
        {
        }
    }
}