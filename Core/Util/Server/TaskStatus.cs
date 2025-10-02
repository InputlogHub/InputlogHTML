using System;

namespace InputLog.Core.Util.Server
{
    public class TaskStatus
    {
        public const int STATUS_ON_HOLD = 0;
        public const int STATUS_UNSTARTED = 1;
        public const int STATUS_PENDING = 2;
        public const int STATUS_COMPLETED = 3;
        public const int STATUS_PRUNED = 4;
        public const int STATUS_DELAY = 5;
        public const int STATUS_FAILED = 6;
        public const int STATUS_PARTIALLY_COMPLETED = 7;

        /// <summary>
        /// Turns a Status int into a descriptive String
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static String StatusString(int status)
        {
            switch (status)
            {
                case STATUS_ON_HOLD:
                    return "On hold";
                case STATUS_UNSTARTED:
                    return "Not Started";
                case STATUS_PENDING:
                    return "Processing";
                case STATUS_COMPLETED:
                    return "Completed";
                case STATUS_PRUNED:
                    return "Pruned";
                case STATUS_DELAY:
                    return "Delayed";
                case STATUS_FAILED:
                    return "Failed";
                case STATUS_PARTIALLY_COMPLETED:
                    return "Partial - Some Errors";
                default:
                    return "Rejected";
            }
        }
    }
}
