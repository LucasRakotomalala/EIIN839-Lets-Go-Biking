using Proxy.Models;
using Routing.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text.Json;
using System.Threading.Tasks;
using static Proxy.Models.JCDecauxItem;

namespace Routing
{
    public class RoutingService : IRouting
    {
        private static readonly HttpClient client = new HttpClient();

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
            ReverseGeoCode reverseGeoCode = CallOSM(request.Replace(",", ".")).Result;
            return reverseGeoCode.address.city;
        }

        public string GetPath(double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd)
        {
            string LatStart = latitudeStart.ToString().Replace(",", ".");
            string LngStart = longitudeStart.ToString().Replace(",", ".");
            string LatEnd = latitudeEnd.ToString().Replace(",", ".");
            string LngEnd = longitudeEnd.ToString().Replace(",", ".");
            string request = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf6248c20bc76cf8e34fd9b3413bf70ae6877d&start=" + LngStart + "," + LatStart + "&end=" + LngEnd + "," + LatEnd;
            return CallORS(request).Result;
        }

        public Station FindNearestStation(double latitude, double longitude)
        {
            List<Station> stations = GetAllStationsFromCity(GetCityName(latitude, longitude));

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

        private static async Task<ReverseGeoCode> CallOSM(string request)
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

        private static async Task<string> CallORS(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
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
