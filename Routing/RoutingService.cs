using Proxy.Models;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static Proxy.Models.JCDecauxItem;

namespace Routing
{
    public class RoutingService : IRouting
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Station> stations;

        public RoutingService()
        {
            stations = GetAllStations();
        }
        public List<Station> GetAllStations()
        {
            return CallProxy("http://localhost:8733/API/JCDecaux/stations").Result;
        }

        public List<Station> GetAllStationsFromCity(string city)
        {
            return CallProxy("http://localhost:8733/API/JCDecaux/stations?city=" + city).Result;
        }

        public Station FindNearestStation(double latitude, double longitude)
        {
            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = stations[0];
            double distance = double.MaxValue;

            foreach (Station station in stations)
            {
                if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance)
                {
                    nearestStation = station;
                    distance = userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));
                }
            }

            return nearestStation;
        }

        private static async Task<List<Station>> CallProxy(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<JCDecauxItem>(responseBody).stations;
            }
            catch (HttpRequestException)
            {
                return new List<Station>();
            }
        }
    }
}
