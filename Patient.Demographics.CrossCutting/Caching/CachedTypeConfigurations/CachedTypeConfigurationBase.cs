using System;
using System.Collections.Generic;

namespace BIWorldwide.GPSM.CrossCutting.Caching.CachedTypeConfigurations
{
    public abstract class CachedTypeConfigurationBase : ICachedTypeConfiguration
    {
        protected CachedTypeConfigurationBase(string type, params string[] ignoredProperties)
        {
            Type = type;

            var ignoredPropertiesAndBackingFields = GenerateIgnoredPropertiesIncludingBackingFields(ignoredProperties);
            IgnoredProperties = new HashSet<string>(ignoredPropertiesAndBackingFields, StringComparer.OrdinalIgnoreCase);
        }

        public string Type { get; }

        public IReadOnlyCollection<string> IgnoredProperties { get; }

        private IEnumerable<string> GenerateIgnoredPropertiesIncludingBackingFields(IEnumerable<string> ignoredProperties)
        {
            foreach (var ignoredProperty in ignoredProperties)
            {
                yield return ignoredProperty;

                var backingFieldName = $"<{ignoredProperty}>k__BackingField";
                yield return backingFieldName;

                var privateFieldName = "_" + ignoredProperty;
                yield return privateFieldName;
            }
        }
    }
}