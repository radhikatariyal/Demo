using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Demographics.Common
{
    /// <summary>
    /// Code handily lifted from NHibernate
    /// </summary>
    public static class Identifier
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        public static Guid NewId()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string 
            TimeSpan days = new TimeSpan(now.Ticks - BaseDateTicks);
            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// Orders all identifiers by the comb value
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> OrderByComb<TSource>(this IEnumerable<TSource> list, Func<TSource, Guid> keySelector)
        {
            return list.OrderBy(g => keySelector(g).ToString().Substring(24));
        }
    }
}
