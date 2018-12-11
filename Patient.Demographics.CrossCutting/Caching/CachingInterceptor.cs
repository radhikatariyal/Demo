using System;
using System.Linq;
using System.Threading.Tasks;
using BIWorldwide.GPSM.Common;
using BIWorldwide.GPSM.Common.Settings;
using Castle.DynamicProxy;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    [Serializable]
    public class CachingInterceptor : IInterceptor
    {
        private readonly ICachingProvider _cachingProvider;
        private readonly IDataSettings _dataSettings;

        public CachingInterceptor(ICachingProvider cachingProvider, IDataSettings dataSettings)
        {
            _cachingProvider = cachingProvider;
            _dataSettings = dataSettings;
        }

        public void Intercept(IInvocation invocation)
        {
            string repositoryName = string.Empty;
            string cacheKey = string.Empty;

            if (_dataSettings.CacheQueries)
            {
                var interfaces = invocation.TargetType.GetInterfaces();
                if (interfaces.Any())
                {
                    repositoryName = interfaces.First().Name;
                    string method = invocation.Method.GetMethodDescription();
                  
                    cacheKey = QueryCache.GenerateCacheKey(repositoryName, method, invocation.Arguments);

                    var item = _cachingProvider.Get(cacheKey, repositoryName);
                    
                    if (item != null)
                    {
                        if (invocation.Method.ReturnType.BaseType == typeof(Task))
                        {
                            var returnType = invocation.Method.ReturnType.GenericTypeArguments[0];
                            var taskMethod = typeof(Task).GetMethod("FromResult");
                            var taskGenericMethod = taskMethod.MakeGenericMethod(returnType);
                            invocation.ReturnValue = taskGenericMethod.Invoke(null, new[] {item});
                        }
                        else
                        {
                            invocation.ReturnValue = item;
                        }
                        return;
                    }
                }
            }

            invocation.Proceed();
            
            if (!string.IsNullOrEmpty(repositoryName))
            {
                var cacheUntil = CacheSettings.MaxTimeout;
                var cacheForAttribute = invocation.Method.GetCustomAttributes(typeof(CacheForAttribute), true);
                if (cacheForAttribute.Any())
                {
                    var seconds = (cacheForAttribute.Single() as CacheForAttribute).Seconds;
                    cacheUntil = TimeSpan.FromSeconds(seconds);
                }

                if (invocation.ReturnValue is Task)
                {
                    var task = invocation.ReturnValue as Task;
                    if (!task.IsFaulted)
                    {
                        task.ContinueWith((action) =>
                        {
                            var result = action.GetType()
                                .GetProperty("Result")
                                .GetValue(action, null);
                            if (result != null)
                            {
                                if(cacheUntil.TotalSeconds > 0)
                                {
                                    _cachingProvider.Add(cacheKey, result, cacheUntil, repositoryName);
                                }
                            }

                        });
                    }
                }
                else
                {
                    if (cacheUntil.TotalSeconds > 0)
                    {
                        _cachingProvider.Add(cacheKey, invocation.ReturnValue, cacheUntil, repositoryName);
                    }
                }
            }
        }
    }
}