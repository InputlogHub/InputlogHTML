namespace InputLog.Core.Events.WinLog
{
    /// <summary>
    /// Question eventPart added at the bottom of copyTask idfx's
    /// </summary>
    public sealed class Questions: IEventPart
    {
        #region Fields
        /// <summary>
        /// Handedness score for a person
        /// </summary>
        public string Handedness;

        /// <summary>
        /// Computer type used for the test
        /// </summary>
        public string Computer;

        /// <summary>
        /// Keyboard familiarity
        /// </summary>
        public string Keyboard;

        /// <summary>
        /// Browser used
        /// </summary>
        public string Browser;

        /// <summary>
        ///  Dominant language of subject
        /// </summary>
        public string Language;

        /// <summary>
        /// If the subject has a disorder or not
        /// </summary>
        public bool Disorder;

        /// <summary>
        /// Education level of the subject
        /// </summary>
        public string Education;

        /// <summary>
        /// Has the subject already done this test before?
        /// </summary>
        public bool Repetition;
        #endregion

        public Questions() { }

        public Questions( string handedness, string computer, string keyboard, string browser,
            string language, bool disorder,
            string education, bool repetition)
        {
            Handedness = handedness;
            Computer = computer;
            Keyboard = keyboard;
            Browser = browser;
            Language = language;
            Disorder = disorder;
            Education = education;
            Repetition = repetition;
        }
    }
}
