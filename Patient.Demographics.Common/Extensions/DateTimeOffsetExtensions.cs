using System;

namespace Patient.Demographics.Common.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset EndOfDay(this DateTimeOffset date)
        {
            return date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
    }
}