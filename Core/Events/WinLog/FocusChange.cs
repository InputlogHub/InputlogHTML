namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Core keyboard information (received from Windows).
    /// @date July 06, 2010
    /// </summary>
    public sealed class FocusChange : TimedEventPart, IFocusChangeEventPart
    {
        /// <summary>
        /// Title of the new active window.
        /// </summary>
        private string _activeWindowTitle;

        public string WindowTitle
        {
            get => _activeWindowTitle;
            set => _activeWindowTitle = string.IsNullOrWhiteSpace(value) ? "TASKBAR" : value;
        }

        /// <summary>
        /// Constructs a WindowsFocusChangeEventPart.
        /// </summary>
        /// <param name="windowTitle">Title of the new active window.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public FocusChange(string windowTitle, ulong startTime, ulong endTime)
            : base(startTime, endTime)
        {
            WindowTitle = windowTitle;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public FocusChange()
        {
        }

        /// <summary>
        /// Provides string representation of an event (overrides default implementation).
        /// </summary>
        /// <returns>String representation of an event.</returns>
        public override string ToString()
        {
            return "[WinLog.FocusChange \"" + _activeWindowTitle + "\"]";
        }
    }
}