using System;
using System.Collections.Generic;

namespace Proxy.Cache
{
    public interface ICache<T>
    {
        T Get(string CacheItem, Dictionary<string, string> dictionary);

        T Get(string CacheItem, double dt_seconds, Dictionary<string, string> dictionary);

        T Get(string CacheItem, DateTimeOffset dt, Dictionary<string, string> dictionary);
    }
}
