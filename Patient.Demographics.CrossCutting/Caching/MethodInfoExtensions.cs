using System.Linq;
using System.Reflection;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public static class MethodInfoExtensions
    {
        public static string GetMethodDescription(this MethodInfo methodInfo)
        {
            // This is used to generate cache keys. If the method name alone is used, then two methods with the same name, but different generic parameters 
            // and return type can cause a cache key clash. 
            var returnType = methodInfo.ReturnType.Name;
            
            if (methodInfo.IsGenericMethod)
            {
                var genericArgs = methodInfo.GetGenericArguments();
                var genericArgsString = string.Join("-", genericArgs.Select(g => g.Name));
                return $"{returnType}-{genericArgsString}-{methodInfo.Name}";
            }

            return $"{methodInfo.Name}";
        }
    }
}