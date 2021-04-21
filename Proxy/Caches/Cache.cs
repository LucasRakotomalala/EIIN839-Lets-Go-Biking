using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Proxy.Cache
{
    public class Cache<T> : ICache<T> where T : new()
    {
        private readonly ObjectCache cache;
        private DateTimeOffset Dt_default { get; set; }

        public Cache()
        {
            cache = MemoryCache.Default;
            Dt_default = ObjectCache.InfiniteAbsoluteExpiration;
        }

        public T Get(string CacheItem, Dictionary<string, string> dictionary)
        {
            return Get(CacheItem, Dt_default, dictionary);
        }

        public T Get(string CacheItem, double dt_seconds, Dictionary<string, string> dictionary)
        {
            return Get(CacheItem, DateTimeOffset.Now.AddSeconds(dt_seconds), dictionary);
        }

        public T Get(string CacheItem, DateTimeOffset dt, Dictionary<string, string> dictionary)
        {
            if (cache.Get(CacheItem) == null)
            {
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = dt
                };
                if (dictionary == null)
                    cache.Add(CacheItem, new T(), cacheItemPolicy);
                else
                    cache.Add(CacheItem, (T) Activator.CreateInstance(typeof(T), dictionary), cacheItemPolicy);
            }
            return (T) cache.Get(CacheItem);
        }
    }
}
