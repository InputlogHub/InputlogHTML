namespace InputLog.Core.Events
{
    /// <summary>
    /// The EventPart interface represents extra information a specific application
    /// (e.g. word, (core)) adds to an event.
    /// Examples may include:
    ///  - character position in a text (KEYBOARD EVENT)
    ///  - markup of a character (KEYBOARD EVENT)
    ///  - document information (FOCUS CHANGE EVENT)
    ///  - ...
    /// </summary>
    public interface IEventPart
    {}
}