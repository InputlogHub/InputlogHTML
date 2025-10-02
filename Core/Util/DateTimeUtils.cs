using System;

namespace InputLog.Core.Util
{
    public static class DateTimeUtils
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Converts a DateTime to a Timestamp in milliseconds
        /// </summary>
        /// <returns>Timestamp in milliseconds</returns>
        public static ulong ConvertToTimestamp(this DateTime value)
        {
            TimeSpan elapsedTime = value.ToUniversalTime() - Epoch;
            return (ulong)elapsedTime.TotalMilliseconds;
        }

        public static string ToXESRoundTrip(this DateTime value)
        {
            return value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        public static DateTime FromTimestamp(ulong msecs)
        {
            return Epoch.AddMilliseconds(msecs).ToLocalTime();
        }

        /// <summary>
        /// Converts a given timestamp in msec to a string representation "hh:mm:ss.thousands".
        /// Note that the given timestamp does not need to be a unix timestamp. 
        /// This methods just extracts the number of hours,
        /// minutes and seconds (+ thousands of seconds) that are expressed in milliseconds by the given parameter.
        /// </summary>
        /// <param name="msec">milliseconds to get a "hh:mm:ss.thousands" string representation for.</param>
        /// <param name="inclThousands">(Optional) If true (=default value), the thousands' will be added 
        /// to the string, if false not.</param>
        /// <param name="inclHours"></param>
        /// <returns>"hh:mm:ss.thousands" string representation of the parameter.</returns>
        public static string MsecToClockString(ulong msec, bool inclThousands = true, bool inclHours = true)
        {
            var time = TimeSpan.FromMilliseconds(msec);
            if(time.Hours > 0) inclHours = true;
            string format = @"mm\:ss";
            if (inclHours)
                format = @"hh\:" + format;
            if (inclThousands)
                format = format + @"\.fff";
            return time.ToString(format);
        }
    }
}
