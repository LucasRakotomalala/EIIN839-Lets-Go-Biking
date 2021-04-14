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
        private readonly double EXPIRATION_TIME = 120;

        public JCDecauxItem GetStation(string city, string number)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("city", city);
            dictionary.Add("number", number.ToString());
            return cache.Get(KEY + "?contract=" + city + "&number=" + number, EXPIRATION_TIME, dictionary);
        }
    }
}
