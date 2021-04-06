using Proxy.Cache;
using Proxy.Models;
using System.Collections.Generic;
using System.Device.Location;
using static Proxy.Models.JCDecauxItem;

namespace Proxy
{
    public class JCDecaux : IJCDecaux
    {
        private static readonly string KEY = "stations";

        private Cache<JCDecauxItem> cache = new Cache<JCDecauxItem>();
        private readonly double EXPIRATION_TIME = 30;

        public JCDecauxItem GetAllStations()
        {
            return cache.Get(KEY, EXPIRATION_TIME, null);
        }

        public JCDecauxItem GetAllStationsFromCity(string city)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("city", city);
            return cache.Get(KEY + "?contract=" + city, EXPIRATION_TIME, dictionary);
        }

        public Station FindNearestStation(double latitude, double longitude)
        {
            List<Station> stations = cache.Get(KEY + "?lat=" + (int) latitude + "&lng=" + (int) longitude, EXPIRATION_TIME, null).stations;

            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = null;
            double distance = 0;

            foreach (Station station in stations)
            {
                if (nearestStation == null)
                {
                    nearestStation = station;
                    distance = userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));
                }
                else if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance)
                {
                    nearestStation = station;
                    distance = userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));

                }
            }

            return nearestStation;
        }
    }
}
