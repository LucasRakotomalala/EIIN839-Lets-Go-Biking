using Proxy.Models;
using Routing.Models;
using System;
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
        private List<Station> allStations;
        private readonly int THRESHOLD_AVAILABLE_BIKES = 2;
        private readonly int THRESHOLD_AVAILABLE_BIKES_STANDS = 2;

        public RoutingService()
        {
            allStations = CallJCDecaux("https://api.jcdecaux.com/vls/v2/stations?apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76").Result;
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public List<Station> GetAllStations()
        {
            return allStations;
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
            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = null;
            double distance = double.MaxValue;

            foreach (Station station in allStations)
            {
                if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance)
                {
                    Station potentialBestStation = CallProxy("https://api.jcdecaux.com/vls/v2/stations/" + station.number + "?contract=" + station.contract_name + "&apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76").Result;
                    if (potentialBestStation.available_bikes >= THRESHOLD_AVAILABLE_BIKES)
                    {
                        nearestStation = potentialBestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialBestStation.position.latitude, potentialBestStation.position.longitude));
                    }
                }
            }

            return nearestStation;
        }

        public Station FindNearestStationFromEnd(double latitude, double longitude)
        {
            GeoCoordinate userPosition = new GeoCoordinate(latitude, longitude);
            Station nearestStation = null;
            double distance = double.MaxValue;

            foreach (Station station in allStations)
            {
                if (userPosition.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < distance)
                {
                    Station potentialBestStation = CallProxy("https://api.jcdecaux.com/vls/v2/stations/" + station.number + "?contract=" + station.contract_name + "&apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76").Result;
                    if (potentialBestStation.available_bike_stands >= THRESHOLD_AVAILABLE_BIKES_STANDS)
                    {
                        nearestStation = potentialBestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialBestStation.position.latitude, potentialBestStation.position.longitude));
                    }
                }
            }

            return nearestStation;
        }

        private static async Task<List<Station>> CallJCDecaux(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Station>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return new List<Station>();
            }
        }

        private static async Task<Station> CallProxy(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<Station>(responseBody);
            }
            catch (HttpRequestException)
            {
                return new Station();
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
