using System;
using BIWorldwide.GPSM.Common.Settings;
using BIWorldwide.GPSM.CrossCutting.Caching.CachedTypeConfigurations;
using CacheManager.Core;
using System.Collections.Generic;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public interface IRedisCacheManagerFactory
    {
        ICacheManager<object> Create();
    }

    public class RedisCacheManagerFactory : IRedisCacheManagerFactory
    {
        private readonly IDataSettings _cacheConfiguration;
        private readonly IEnumerable<ICachedTypeConfiguration> _cachedTypeConfigurations;

        public RedisCacheManagerFactory(IDataSettings cacheConfiguration, IEnumerable<ICachedTypeConfiguration> cachedTypeConfigurations)
        {
            _cacheConfiguration = cacheConfiguration;
            _cachedTypeConfigurations = cachedTypeConfigurations;
        }

        public ICacheManager<object> Create()
        {
            return CacheFactory.Build(settings =>
            {
                var endPoints = _cacheConfiguration.CacheServer.Split(new [] { ','}, StringSplitOptions.RemoveEmptyEntries);
                settings
                    .WithRedisConfiguration("redis", config =>
                    {
                        config.WithAllowAdmin()
                            .WithDatabase(_cacheConfiguration.CacheDatabase);
                        foreach (var endPoint in endPoints)
                        {
                            var split = endPoint.Split(':');
                            config.WithEndpoint(split[0].Trim(), int.Parse(split[1]));
                        } 

                        if (!string.IsNullOrEmpty(_cacheConfiguration.CachePassword))
                        {
                            config.WithPassword(_cacheConfiguration.CachePassword);
                        }
                    })
                    // .WithSerializer(typeof(JsonCacheSerializer), _cachedTypeConfigurations)
                    .WithSerializer(typeof(UniversalCacheSerializer), _cachedTypeConfigurations)
                    .WithRetryTimeout(50)
                    .WithMaxRetries(100)
                    .WithRedisBackplane("redis")
                    .WithRedisCacheHandle("redis", true);
            });
        }
    }
}