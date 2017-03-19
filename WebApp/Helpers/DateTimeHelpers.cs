using System;

namespace WebApp.Helpers
{
    public static class DateTimeHelpers
    {
        public static int GetUnixTimeStamp(DateTime date)
        {
            int unixTimestamp = (int)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }
    }
}