using System;
using System.Collections.Generic;
using InputLog.Core.Events.WordLog;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.DragonNS;
using System.Globalization;

namespace InputLog.Core.IO.Basic
{
    public static class EventIOHandlers
    {
        private const string WINLOG = "WinLog";
        private const string WORDLOG = "WordLog";
        private const string EYETRACK = "Eyetrack";
        private const string DRAGON = "DragonNS";
        private const string NAMESPACE = "InputLog.Core.IO";
        private const string NAMESPACE_READ = "Input";
        private const string NAMESPACE_WRITE = "Output";

        public static readonly Dictionary<Guid, string> WriteHandlers = new Dictionary<Guid, string>
        {
            { typeof(Insert).GUID, WORDLOG },
			{ typeof(Keypress).GUID, WORDLOG },
			{ typeof(MouseEvent).GUID, WORDLOG },
			{ typeof(Replacement).GUID, WORDLOG },
			{ typeof(SelectionChange).GUID, WORDLOG },
			{ typeof(Statistics).GUID, WORDLOG },
            { typeof(AuthorComment).GUID, WORDLOG },
            { typeof(Click).GUID, WINLOG },
            { typeof(FocusChange).GUID, WINLOG },
            { typeof(KeyPress).GUID, WINLOG },
            { typeof(MouseMovement).GUID, WINLOG },
            { typeof(Scroll).GUID, WINLOG },
            { typeof(EyetrackPart).GUID, EYETRACK },
            { typeof(DragonNSPart).GUID, DRAGON },
        };

        public static readonly Dictionary<string, string> ReadHandlers = new Dictionary<string, string>
        {
            { WORDLOG.ToLower(), WORDLOG },
			{ WINLOG.ToLower(), WINLOG },
			{ EYETRACK.ToLower(), EYETRACK },
			{ DRAGON.ToLower(), DRAGON },
        };

        public static Tuple<string, string> GetReadHandler(string partType, string eventType, string format)
        {
            string nmspc = NAMESPACE + "." + format + "." + NAMESPACE_READ + ".";
            string handle = nmspc + format + ReadHandlers[partType] + "Reader";
            string func = "Read" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventType);
            return new Tuple<string, string>(handle, func);
        }

        public static Tuple<string, string> GetWriteHandler(Type partType, string eventType, string format)
        {
            string nmspc = NAMESPACE + "." + format + "." + NAMESPACE_WRITE + ".";
            string handle = nmspc + format + WriteHandlers[partType.GUID] + "Writer";
            string func = "Write" + partType.Name;
            return new Tuple<string, string>(handle, func);
        }
    }
}
