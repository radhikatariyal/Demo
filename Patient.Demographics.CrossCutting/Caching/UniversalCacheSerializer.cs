using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CacheManager.Core;
using CacheManager.Core.Internal;
using UniversalSerializerLib3;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public class UniversalCacheSerializer : ICacheSerializer
    { 
        public byte[] Serialize<T>(T value)
        {
            if (value == null || !ShouldCache(value.GetType()))
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var parameters = new Parameters()
                {
                    Stream = memoryStream,
                    SerializerFormatter = SerializerFormatters.BinarySerializationFormatter,
                    
                };
                using (var serializer = new UniversalSerializer(parameters))
                {
                    serializer.Serialize(value);
                }
                return memoryStream.ToArray();
            }
        }

        public object Deserialize(byte[] data, Type targetType)
        {
            if (data == null || !ShouldCache(targetType))
                return null;
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                var parameters = new Parameters()
                {
                    Stream = memoryStream,
                    SerializerFormatter = SerializerFormatters.BinarySerializationFormatter
                };

                using (var serializer = new UniversalSerializer(parameters))
                {
                    return serializer.Deserialize();
                }
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
            return !targetType.Name.Contains("GroupedEnumerable") &&
                   !targetType.FullName.Contains("System.Linq.IGrouping") &&
                   targetType.GetGenericTypeDefinition().Name.Equals("Grouping") == false;
        }

    }

    public class CustomFiltersTestModifier : CustomModifiers
    {
        /// <summary>
        /// Tells the serializer to add some certain private fields to store the type.
        /// </summary>
        static FieldInfo[] PrivateFieldsAdder(Type type)
        {
            if (type.IsAbstract && type.IsSealed)
                //ignore static classes //|| t.IsValueType || t.IsPrimitive // IsSimple(t) ||
                return null;

            bool isList = (
                typeof(IEnumerable).IsAssignableFrom(type) ||
                type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
            if (isList)
                return null;

            Type baseType = type;

            List<FieldInfo> fields = new List<FieldInfo>();

            while (baseType != null && baseType != typeof(object) && baseType != typeof(ValueType))
            {
                if (baseType != typeof(object) && baseType != typeof(ValueType))
                {
                    fields.AddRange(baseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic
                                                       | BindingFlags.GetProperty
                                                       | BindingFlags.GetField
                    ));
                }
                baseType = baseType.BaseType;
            }

            return fields.Distinct().ToArray();
        }

        /// <summary>
        /// Returns 'false' if this type should not be serialized at all.
        /// That will let the default value created by the constructor of its container class/structure.
        /// </summary>
        static bool ShouldSerialize(Type type)
        {
            if (type.IsAbstract && type.IsSealed) //static classes are ignored
                return false;

            return true;
        }

        public CustomFiltersTestModifier()
            : base(FilterSets: new FilterSet[]
            {
                new FilterSet()
                {
                    AdditionalPrivateFieldsAdder = PrivateFieldsAdder,
                    TypeSerializationValidator = ShouldSerialize
                }
            })
        {

        }
    }
}
