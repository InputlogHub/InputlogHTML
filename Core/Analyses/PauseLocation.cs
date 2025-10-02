namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Enumeration of the different kinds of pauselocations.
    /// </summary>
    /// <remarks>It is imperative that no element in this enumeration is bound to 0,
    ///  to guarantee the correctness of the PauseLocationMarker class.</remarks>
    public enum PauseLocation
    {
        UNDETERMINED = -1,
        WITHIN_WORDS = 1,
        BEFORE_WORDS = 2,
        AFTER_WORDS = 3,
        BEFORE_SENTENCES = 4,
        AFTER_SENTENCES = 5,
        BEFORE_PARAGRAPHS = 6,
        AFTER_PARAGRAPHS = 7,
        INITIAL = 8,
        END = 9,
        CHANGE = 10,
        REVISION = 11,
        COMBINATION_KEY = 12,
        EYETRACK = 13,
        SPEECH = 14,
        UNKNOWN = 15,
        MOUSE = 16
    }
}