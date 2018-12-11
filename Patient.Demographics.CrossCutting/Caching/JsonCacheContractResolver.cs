using BIWorldwide.GPSM.CrossCutting.Caching.CachedTypeConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    /// <summary>
    /// Special contract resolver to create objects bypassing constructor call.
    /// Adapted from http://stackoverflow.com/questions/13947997/how-to-deserialize-class-without-calling-a-constructor
    /// </summary>
    public class JsonCacheContractResolver : DefaultContractResolver
    {
        private readonly IEnumerable<ICachedTypeConfiguration> _cachedTypeConfigurations;

        //TODO: cache reflection information for types [SY]
        public JsonCacheContractResolver(IEnumerable<ICachedTypeConfiguration> cachedTypeConfigurations)
        {
            _cachedTypeConfigurations = cachedTypeConfigurations;

            SerializeCompilerGeneratedMembers = true;
            //TODO: override GetSerializableMembersand remove obsolete property assignment [SY]
            DefaultMembersSearchFlags |= BindingFlags.NonPublic;
        }
         
        public static bool CanSetMemberValue(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Property:
                    var property = member as PropertyInfo;
                    return property?.GetSetMethod(true) != null;
                default:
                    return false;
            }
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);

            var cachedTypeConfiguration = _cachedTypeConfigurations.SingleOrDefault(c => c.Type == objectType.Name);
            if (cachedTypeConfiguration != null)
            {
                members = members.Where(member => !cachedTypeConfiguration.IgnoredProperties.Contains(member.Name)).ToList();
            }

            return members;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            var cachedTypeConfiguration = _cachedTypeConfigurations.SingleOrDefault(c => c.Type == type.Name);
            if (cachedTypeConfiguration != null)
            {
                properties = properties.Where(property => !cachedTypeConfiguration.IgnoredProperties.Contains(property.PropertyName)).ToList();
            }

            return properties;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = CanSetMemberValue(member);
            return jProperty;
        }
        
        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract"/> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract"/> for the given type.
        /// </returns>
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var constructor = objectType.GetConstructor( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            //If there is a default constructor, use the default contract
            //if (objectType.GetConstructor(Type.EmptyTypes) != null)
            if (constructor != null)
            {
                return base.CreateObjectContract(objectType);
            }

            // prepare contract using default resolver
            var objectContract = base.CreateObjectContract(objectType);

            // if type has constructor marked with JsonConstructor attribute or can't be instantiated, return default contract
            if (objectContract.OverrideCreator != null || objectContract.CreatedType.IsInterface || objectContract.CreatedType.IsAbstract)
                return objectContract;

            // prepare function to check that specified constructor parameter corresponds to non writable property on a type
            Func<JsonProperty, bool> isParameterForNonWritableProperty =
                parameter =>
                {
                    var propertyForParameter = objectContract.Properties.FirstOrDefault(property => property.PropertyName == parameter.PropertyName);

                    if (propertyForParameter == null)
                        return false;

                    return !propertyForParameter.Writable;
                };

            // if type has parameterized constructor and any of constructor parameters corresponds to non writable property, return default contract
            // this is needed to handle special cases for types that can be initialized only via constructor, i.e. Tuple<>
            if (objectContract.OverrideCreator != null
                && objectContract.CreatorParameters.Any(parameter => isParameterForNonWritableProperty(parameter)))
                return objectContract;

            // override default creation method to create object without constructor call
            objectContract.DefaultCreatorNonPublic = false;

            objectContract.DefaultCreator =
                () => FormatterServices.GetUninitializedObject(objectContract.CreatedType);

            return objectContract;
        }
    }
}