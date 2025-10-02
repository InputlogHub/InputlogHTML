namespace GUI.Settings
{
    /// <summary>
    ///  Array of two-letter code text languages according to ISO 639-1.
    /// See : https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
    /// </summary>
    public static class AvailableLanguages
    {
        public static object[] Languages => new object[]
        {
            "EN", "NL", "CA", "CY", "FR", "FI", "DA", "DE", "ES", "GR", "IT", "NB", "NN", "NO", "PL", "PT", "SE", "SV",
            "TR", "ZH", "Other"
        };
    }
}
