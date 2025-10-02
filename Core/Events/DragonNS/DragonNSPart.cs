using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events.WinLog;

namespace InputLog.Core.Events.DragonNS
{
    /// <summary>
	/// The event part for for a Dragon Naturally Speaking event.
	/// </summary>
    public class DragonNSPart : TimedEventPart
	{
        public int ID;
        public ulong TotalStartTime;
        public ulong TotalEndTime;
        public string Guid;
        public string Text;
        public string WavePath;

        public DragonNSPart()
        {
        }

        public DragonNSPart(int id, ulong startTime, ulong endTime, string guid, string text, string wavePath)
		{
            ID = id;
            TotalStartTime = startTime;
            TotalEndTime = endTime;
            Guid = guid;
            Text = text;
            WavePath = wavePath;
		}

        public void AddTiming(ulong startTime, ulong endTime)
        {
            base.StartTime = startTime;
            base.EndTime = endTime;
        }
    }
}
