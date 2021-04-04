using Proxy.Cache;
using Proxy.Models;
using System.Collections.Generic;

namespace Proxy
{
    public class JCDecaux : IJCDecaux
    {
        private static readonly string URL = "https://api.jcdecaux.com/vls/v2/";
        private static readonly string DATA = "stations";
        private static readonly string API_KEY = "ff987c28b1313700e2c97651cec164bd6cb4ed76";

        private Cache<List<Station>> stations = new Cache<List<Station>>();
        private readonly double EXPIRATION_TIME = 30;

        List<Station> IJCDecaux.GetAllStationsFromCity(string city)
        {
            return stations.Get(URL + DATA + "?contract=" + city + "&apiKey=" + API_KEY, EXPIRATION_TIME);
        }

    }
}
