using System;
using NodaTime;

namespace Patient.Demographics.Common.Extensions
{
    public static class TimeZoneHelper
    {
        public static string GetCurrentIanaTimeZone()
        {
            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            return tz.ToString();
        }

        public static DateTimeOffset ParseDate(string ianaTimeZone, string dateAsString)
        {
            try
            {
                var localDateTime = ParseLocalDate(dateAsString).AtMidnight();
                
                var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaTimeZone);
                
                var localMapping = timeZone.MapLocal(localDateTime);

                // It's possible that the localDateTime has no value in the target timezone, or that it has two (if daylight saving
                // kicks in at midnight on that day. We just take the first value. 
                var zonedDateTime = localMapping.First();

                return  zonedDateTime.ToDateTimeOffset();
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Could not parse the date '{dateAsString}", e);
            }
        }

        private static LocalDate ParseLocalDate(string dateAsString)
        {
            var dateComponents = dateAsString.Split('/', '-');

            var firstNumber = Int32.Parse(dateComponents[0]);

            int day;
            int month;
            int year;

            if (firstNumber > 1000)
            {
                // Assuming that the date is in the format yyyy/MM/dd
                year = firstNumber;
                month = Int32.Parse(dateComponents[1]);
                day = Int32.Parse(dateComponents[2]);
            }
            else
            {
                // Assuming that the date is in the format dd/MM/yyyy
                day = firstNumber;
                month = Int32.Parse(dateComponents[1]);
                year = Int32.Parse(dateComponents[2]);
            }

            return new LocalDate(year, month, day);
        }
    }
}