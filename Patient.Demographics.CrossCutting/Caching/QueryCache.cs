using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public class QueryCache
    {
        public static string GenerateCacheKey(string repository, string name, object[] arguments)
        {
            string key = repository + "-" + name;
            if (arguments == null || arguments.Length == 0)
                return key;
            return key + "-" + string.Join("-", arguments.Select(ArgumentKey).ToArray());
        }

        public static string GenerateCacheKey(string repository, string name, object argument)
        {
            return GenerateCacheKey(repository, name, new[] {argument});
        }

        private static string ArgumentKey(object argument)
        {
            if (argument == null)
            {
                return "";
            }

            if (IsSimple(argument.GetType()) || argument.GetType().Namespace == "System.Linq.Expressions")
            {
                return argument.ToString();
            }

            var sb = new StringBuilder();

            var enumerable = argument as IEnumerable;

            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append("-");
                    }

                    sb.Append(ArgumentKey(item));
                }
            }
            else
            {
                foreach (var pInfo in argument.GetType().GetProperties())
                {
                    if (pInfo.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType))
                    {
                        foreach (var pInfoSingle in (IEnumerable)pInfo.GetValue(argument))
                        {
                            sb.Append(ArgumentKey(pInfoSingle));
                        }
                    }
                    else
                    {
                        var value = pInfo.GetValue(argument);

                        if (value != null && !IsSimple(value.GetType()))
                        {
                            sb.Append(ArgumentKey(value));
                        }
                        else
                        {
                            var strValue = value?.ToString();

                            if (!string.IsNullOrWhiteSpace(strValue) && strValue != Guid.Empty.ToString())
                            {
                                sb.Append(pInfo.Name + ":" + strValue);
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(Guid)
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(DateTimeOffset?);
        }

        public static string GenerateDependancyKey(string repository, string aggregateId)
        {
            string key = repository;
            if (string.IsNullOrEmpty(aggregateId))
                return key;
            return key + "-" + aggregateId;
        }

        public static string GenerateDependancyKey(string repository, object[] arguments)
        {
            string key = repository;
            if (arguments == null || arguments.Length == 0)
                return key;
            return key + "-" + string.Join("-", arguments.Select(a => a.ToString()).ToArray());
        }
    }
}