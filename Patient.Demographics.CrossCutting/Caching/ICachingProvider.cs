using System;
using System.Threading.Tasks;

namespace BIWorldwide.GPSM.CrossCutting.Caching
{
    public interface ICachingProvider
    {
        string SerializerType { get; }
        Task AddAsync(string key, object toBeCached, TimeSpan timeSpan, string dependancyKey = null);
        void Add(string key, object toBeCached, TimeSpan timeSpan, string dependancyKey = null);
        Task<object> GetAsync(string key, string dependancyKey = null);
        object Get(string key, string dependancyKey = null);
        Task RemoveAsync(string key, string dependancyKey = null);
        void Remove(string key, string dependancyKey = null);
        Task InvalidateDependancyAsync(string key);
        void InvalidateDependancy(string key);
    }
}