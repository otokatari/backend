using System;

namespace OtokatariBackend.Utils
{
    public static class DateUtil
    {
        public static long DateToUnix(this DateTime dt) => (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;

        public static DateTime UnixToDate(long unix) => new DateTime(1970, 1, 1).AddSeconds(unix);

        public static long NowToUnix() => DateToUnix(DateTime.Now);
    }
}
