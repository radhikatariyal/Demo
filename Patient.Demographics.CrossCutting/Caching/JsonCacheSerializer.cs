using BIWorldwide.GPSM.CrossCutting.Caching.CachedTypeConfigurations;
using CacheManager.Core;
using CacheManager.Core.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public class JsonCacheSerializer : ICacheSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonCacheSerializer(IEnumerable<ICachedTypeConfiguration> cachedTypeConfigurations)
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonCacheContractResolver(cachedTypeConfigurations),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }

        public byte[]Serialize<T>(T value)
        {
            // ReSharper disable once UseIsOperator.2
            if (value == null || !ShouldCache(value.GetType()))
            {
                return null;
            }

            string json = JsonConvert.SerializeObject(value, _settings);
            var data = Encoding.UTF8.GetBytes(json);
            return data;
        }

        public object Deserialize(byte[] data, Type targetType)
        {
         
            if (data == null || !ShouldCache(targetType))
                return null;

            try
            {
                string json = Encoding.UTF8.GetString(data);
                var result = JsonConvert.DeserializeObject(json, targetType, _settings);
                return result;
            }
            catch (Exception ex)
            {
                //Some interfaces like  System.Linq.IGrouping<K,V> cannot be deserialised (not specific to json serialization)
                //When that happens, the exception will make whole request fail.
                //until a generic opt out approach is implemented,
                //such errors will cause null return to ensure consuming code then queries the DB
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public byte[] SerializeCacheItem<T>(CacheItem<T> value)
        {
            return Serialize(value);
        }

        public CacheItem<T> DeserializeCacheItem<T>(byte[] value, Type valueType)
        {
            return Deserialize(value, typeof(CacheItem<T>)) as CacheItem<T>;
        }

        private static bool ShouldCache(Type targetType)
        {
            if (!targetType.IsGenericType)
                return true;
            return !targetType.FullName.Contains("IGrouping") &&
                   !targetType.GetGenericTypeDefinition().Name.Equals("Grouping");
        }
    }
}
