using System;
using System.Collections.Generic;

namespace Patient.Demographics.Common.Extensions
{
    public static class ListExtensions
    {
        public static int FindIndex<T>(this IList<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}