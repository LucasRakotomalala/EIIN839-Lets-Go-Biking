using Proxy.Cache;
using Proxy.Models;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Proxy
{
    public class JCDecaux : IJCDecaux
    {
        private static readonly string KEY = "stations";

        private Cache<JCDecauxItem> cache = new Cache<JCDecauxItem>();
        private readonly double EXPIRATION_TIME = 60;

        public JCDecauxItem GetAllStations()
        {
            return cache.Get(KEY, ObjectCache.InfiniteAbsoluteExpiration, null);
        }

        public JCDecauxItem GetAllStationsFromCity(string city)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("city", city);
            return cache.Get(KEY + "?contract=" + city, EXPIRATION_TIME, dictionary);
        }
    }
}
