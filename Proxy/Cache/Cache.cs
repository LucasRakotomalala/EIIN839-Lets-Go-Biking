using System;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy.Cache
{
    public class Cache<T> where T : new()
    {
        private ObjectCache cache;
        private DateTimeOffset dt_default { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public Cache()
        {
            cache = MemoryCache.Default;
            dt_default = ObjectCache.InfiniteAbsoluteExpiration;
        }

        public T Get(string CacheItem)
        {
            return Get(CacheItem, dt_default);
        }

        public T Get(string CacheItem, double dt_seconds)
        {
            return Get(CacheItem, DateTimeOffset.Now.AddSeconds(dt_seconds));
        }

        public T Get(string CacheItem, DateTimeOffset dt)
        {
            if (cache.Get(CacheItem) == null)
            {
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = dt
                };
                cache.Add(CacheItem, CallAPI(CacheItem).Result, cacheItemPolicy);
            }
            return (T) cache.Get(CacheItem);
        }

        private static async Task<T> CallAPI(string request)
        {
            System.Diagnostics.Debug.WriteLine("CallAPI");
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(responseBody);
            }
            catch (HttpRequestException)
            {
                return new T();
            }
        }
    }
}
