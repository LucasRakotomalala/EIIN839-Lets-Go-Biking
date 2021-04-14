using Proxy.Models;
using Routing.Models;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routing
{
    public class RoutingService : IRouting
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly int THRESHOLD_AVAILABLE_BIKES = 2;
        private readonly int THRESHOLD_AVAILABLE_BIKES_STANDS = 2;

        public RoutingService()
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public List<Station> GetAllStations()
        {
            return CallProxy("http://localhost:8733/API/JCDecaux/stations").Result;
        }

        public List<Station> GetAllStationsFromCity(string city)
        {
            return CallProxy("http://localhost:8733/API/JCDecaux/stations?city=" + city).Result;
        }

        public string GetCityName(double latitude, double longitude)
        {
            string request = "https://nominatim.openstreetmap.org/reverse?email=lucas.rakotomalala@etu.univ-cotedazur.fr&zoom=10&format=json&lat=" + latitude + "&lon=" + longitude;
            ReverseGeoCode reverseGeoCode = CallOSMReverse(request.Replace(",", ".")).Result;
            return reverseGeoCode.address.city;
        }

        public Position GetPosition(string address)
        {
            string request = "https://nominatim.openstreetmap.org/search?email=lucas.rakotomalala@etu.univ-cotedazur.fr&format=json&q=" + address;
            List<Place> places = CallOSMPlaces(request).Result;
            Place bestPlace = null;
            double importance = double.MinValue;

            foreach (Place place in places)
            {
                if (place.importance > importance)
                {
                    bestPlace = place;
                    importance = place.importance;
                }
            }

            return (bestPlace == null) ? null : new Position
            {
                latitude = double.Parse(bestPlace.lat.Replace(".", ",")),
                longitude = double.Parse(bestPlace.lon.Replace(".", ","))
            }; ;
        }

        public string GetPath(Position[] positions)
        {
            string data = "{\"coordinates\":[";

            foreach (Position position in positions)
            {
                data += "[" + position.longitude.ToString().Replace(",", ".") + "," + position.latitude.ToString().Replace(",", ".") + "],";
            }

            data += "],\"instructions\":\"true\",\"language\":\"fr\",\"preference\":\"shortest\",\"units\":\"m\"}";
            string request = "https://api.openrouteservice.org/v2/directions/cycling-regular/geojson";
            return CallORS(request, data.Replace("],],", "]],")).Result;
        }

        public void Options()
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }

        public Station FindNearestStationFromStart(double latitude, double longitude)
        {
            List<Station> stations = GetAllStationsFromCity(GetCityName(latitude, longitude));

            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = null;
            double distance = double.MaxValue;

            foreach (Station station in stations)
            {
                if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance && station.available_bikes >= THRESHOLD_AVAILABLE_BIKES)
                {
                    nearestStation = station;
                    distance = userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));
                }
            }

            return nearestStation;
        }

        public Station FindNearestStationFromEnd(double latitude, double longitude)
        {
            List<Station> stations = GetAllStationsFromCity(GetCityName(latitude, longitude));

            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = null;
            double distance = double.MaxValue;

            foreach (Station station in stations)
            {
                if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance && station.available_bike_stands >= THRESHOLD_AVAILABLE_BIKES_STANDS)
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

        private static async Task<ReverseGeoCode> CallOSMReverse(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ReverseGeoCode>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static async Task<List<Place>> CallOSMPlaces(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Place>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static async Task<string> CallORS(string request, string data)
        {
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "5b3ce3597851110001cf6248c20bc76cf8e34fd9b3413bf70ae6877d");
                HttpResponseMessage response = await client.PostAsync(request, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
