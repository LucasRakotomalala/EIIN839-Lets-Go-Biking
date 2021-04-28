using Proxy.Cache;
using Proxy.Models;
using System.Collections.Generic;
using System.ServiceModel;

namespace Proxy
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class JCDecaux : IJCDecaux
    {
        private static readonly string KEY = "station";

        private readonly Cache<JCDecauxItem> cache = new Cache<JCDecauxItem>();
        private readonly double EXPIRATION_TIME = 60;

        public JCDecauxItem GetStationDefault(string city, string stationNumber)
        {
            return GetStation(city, stationNumber, EXPIRATION_TIME);
        }

        public JCDecauxItem GetStation(string city, string stationNumber, double duration)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "city", city },
                { "number", stationNumber }
            };
            return cache.Get(KEY + "?contract=" + city + "&number=" + stationNumber, duration, dictionary);
        }
    }
}
