using Proxy.Cache;
using Proxy.Models;
using System.Collections.Generic;

namespace Proxy
{
    public class JCDecaux : IJCDecaux
    {
        private static readonly string KEY = "station";

        private Cache<JCDecauxItem> cache = new Cache<JCDecauxItem>();
        private readonly double EXPIRATION_TIME = 60;

        public JCDecauxItem GetStationDefault(string city, string number)
        {
            return GetStation(city, number, EXPIRATION_TIME);
        }

        public JCDecauxItem GetStation(string city, string number, double duration)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "city", city },
                { "number", number.ToString() }
            };
            return cache.Get(KEY + "?contract=" + city + "&number=" + number, duration, dictionary);
        }
    }
}
