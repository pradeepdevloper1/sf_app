using System; 

namespace XF.APP.DATA
{
    public static class DataConverters
    {
        public static long GetCurrentEpochTime()
        {
            var epoch = (long)(GetIndianDate(DateTime.UtcNow) -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            return epoch;
        }

        public static DateTime GetIndianDate(DateTime dateTime = new DateTime())
        {
            TimeZoneInfo ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime;
            if (dateTime.Year == 0)
                indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZONE);
            else
                indianTime = TimeZoneInfo.ConvertTime(dateTime, ZONE);
            return indianTime;
        }
    }
}
