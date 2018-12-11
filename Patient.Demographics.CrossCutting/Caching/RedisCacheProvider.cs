using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CacheManager.Core;


namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public class RedisCacheProvider : ICachingProvider
    {
        private readonly ICacheManager<object> _cacheManager;

        private string _serializerType;

        public string SerializerType
        {
            get
            {
                if (string.IsNullOrEmpty(_serializerType))
                    _serializerType = _cacheManager.Configuration.SerializerType.Name;
                return _serializerType;
            }
        }

        public RedisCacheProvider(IRedisCacheManagerFactory cacheManagerFactory)
        {
            Debug.WriteLine($"Initialising Redis Cache Provider");
            _cacheManager = cacheManagerFactory.Create();
        }

        public async Task AddAsync(string key, object toBeCached, TimeSpan timeSpan, string dependancyKey = null)
        {
            await Task.Run(() => Add(key, toBeCached, timeSpan, dependancyKey));
        }

        public void Add(string key, object toBeCached, TimeSpan timeSpan, string dependancyKey = null)
        {
            if (toBeCached == null || !ShouldCache(toBeCached.GetType()) )
            {
                return;
            }
            if (dependancyKey != null)
            {
                Debug.WriteLine($"Adding {key} to cache with dependancy {dependancyKey}");
                var cacheItem = new CacheItem<object>(key, dependancyKey, toBeCached, ExpirationMode.Absolute, timeSpan);
                _cacheManager.Add(cacheItem);
            }
            else
            {
                Debug.WriteLine($"Adding {key} to cache");
                var cacheItem = new CacheItem<object>(key, toBeCached, ExpirationMode.Absolute, timeSpan);
                _cacheManager.Add(cacheItem);
            }
        }

        public Task<object> GetAsync(string key, string dependancyKey = null)
        {
            return Task.FromResult(Get(key, dependancyKey));
        }

        public object Get(string key, string dependancyKey = null)
        {
            if (dependancyKey != null)
            {
                Debug.WriteLine($"Getting {key} from cache with dependancy {dependancyKey}");
                try
                {
                    return _cacheManager.Get(key, dependancyKey);
                }
                catch
                {
                    Remove(key, dependancyKey);
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"Getting {key} from cache");
                try
                {
                    return _cacheManager.Get(key);
                }
                catch
                {
                    Remove(key);
                    return null;
                }
            }
        }

        public async Task RemoveAsync(string key, string dependancyKey = null)
        {
            await Task.Run(() => Remove(key, dependancyKey));
        }

        public void Remove(string key, string dependancyKey = null)
        {
            if (dependancyKey != null)
            {
                Debug.WriteLine($"Removing {key} from cache with dependancy {dependancyKey}");
                _cacheManager.Remove(key, dependancyKey);
            }
            else
            {
                Debug.WriteLine($"Removing {key} from cache");
                _cacheManager.Remove(key);
            }
        }

        public async Task InvalidateDependancyAsync(string dependancyKey)
        {
            await Task.Run(() => InvalidateDependancy(dependancyKey));
        }

        public void InvalidateDependancy(string dependancyKey)
        {
            Debug.WriteLine($"Removing dependancy {dependancyKey} from cache");
            _cacheManager.ClearRegion(dependancyKey);
        }


        private static bool ShouldCache(Type targetType)
        {
            if (!targetType.IsGenericType)
                return true;
            return !targetType.Name.Contains("GroupedEnumerable") &&
                   !targetType.FullName.Contains("System.Linq.IGrouping") &&
                   targetType.GetGenericTypeDefinition().Name.Equals("Grouping") == false;
        }

    }
}