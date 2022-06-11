using System; 

namespace XF.APP.DTO
{
    public static class Converters
    {
        public static long GetCurrentEpochDate()
        {
            var epoch = (long)(DateTime.Now.ToUniversalTime() -
               new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            return epoch;
        }

        public static DateTime EpochToDateString(long epoch)
        {
            if (epoch == 0)
                return new DateTime();

            var utcDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
            TimeZoneInfo ZONE = TimeZoneInfo.FindSystemTimeZoneById(Constants.DATE_TIME_ZONE);
            DateTime date = TimeZoneInfo.ConvertTime(utcDate, ZONE);
            return date;
        }

        public static string EpochToDate(long epoch)
        {
            if (epoch == 0)
                return new DateTime().ToString(Constants.DATE_TIME_FORMAT);

            var utcDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
            TimeZoneInfo ZONE = TimeZoneInfo.FindSystemTimeZoneById(Constants.DATE_TIME_ZONE);
            DateTime date = TimeZoneInfo.ConvertTime(utcDate, ZONE);
            var stringDate = date.ToString(Constants.DATE_TIME_FORMAT);
            return stringDate;
        }

        public static long GetEpochDate(DateTime dateTime)
        {
            var epoch = (long)((dateTime.ToUniversalTime() -
                 new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            return epoch;

        }

    }
}