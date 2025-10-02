using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Util
{
    public static class IsNullOrEmptyExtension
    {
        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                return !source.Cast<object>().Any();
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return !source.Any();
            }
            return true;
        }
    }
}
