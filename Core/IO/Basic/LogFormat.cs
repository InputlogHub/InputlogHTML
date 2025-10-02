using System;

namespace InputLog.Core.IO.Basic
{
    /// <summary>
    /// Log output formats that are supported. 
    /// When a new format is implemented, it should be added to this enumeration.
    /// </summary>
    [Serializable]
    public static class LogFormat
    {
        public const string TXT = "Txt";
        public const string IDF = "Idf";
        public const string XML = "Xml";
        public const string LEGACY_XML = "LegacyXml";
        public const string TRANSLOG_XML = "TransLogXml";
    };

    /// <summary>
    /// Supported hooks.
    /// When a new hooktype is implemented, it should be added to this enumeration.
    /// </summary>
    [Serializable]
    public enum HookType
    {
        KEYBOARD = 0,
        MOUSE = 1,
        FOCUSCHANGE = 2
    }
}