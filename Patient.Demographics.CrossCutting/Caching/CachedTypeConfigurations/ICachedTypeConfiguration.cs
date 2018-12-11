using System.Collections.Generic;

namespace BIWorldwide.GPSM.CrossCutting.Caching.CachedTypeConfigurations
{
    public interface ICachedTypeConfiguration
    {
        string Type { get; }

        IReadOnlyCollection<string> IgnoredProperties { get; }
    }
}