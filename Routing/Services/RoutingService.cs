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
        private static readonly List<Station> allStations = CallJCDecaux("https://api.jcdecaux.com/vls/v2/stations?apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76").Result; //TODO: Update stations when specific ones are queried
        private static readonly Dictionary<string, string> logs = new Dictionary<string, string>();

        private static readonly int THRESHOLD_AVAILABLE_BIKES = 2;
        private static readonly int THRESHOLD_AVAILABLE_BIKES_STANDS = 2;

        public RoutingService()
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public List<Station> GetAllStations()
        {
            return allStations;
        }

        public Position GetPosition(string address)
        {
            address = address.Trim();
            if (address.Equals("null") || address.Equals(""))
            {
                return null;
            }

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
                latitude = double.Parse(bestPlace.lat, new System.Globalization.CultureInfo("en-US")),
                longitude = double.Parse(bestPlace.lon, new System.Globalization.CultureInfo("en-US"))
            };
        }

        public GeoJson GetPath(Position[] positions)
        {
            if (Array.Exists(positions, position => position == null))
            {
                return null;
            }

            string requestCyclingRegular = "https://api.openrouteservice.org/v2/directions/cycling-regular/geojson";
            string requestFootWalking = "https://api.openrouteservice.org/v2/directions/foot-walking/geojson";

            GeoJson cyclingRegularPath = JsonSerializer.Deserialize<GeoJson>(CallORS(requestCyclingRegular, BuildDataORS(positions)).Result);
            GeoJson footWalkingPath = JsonSerializer.Deserialize<GeoJson>(CallORS(requestFootWalking, BuildDataORS(new Position[] { positions[0], positions[positions.Length - 1] })).Result);

            bool chooseFootWalkingPathByDistance = footWalkingPath.features[0].properties.summary.distance < cyclingRegularPath.features[0].properties.summary.distance;
            bool chooseFootWalkingPathByDuration = footWalkingPath.features[0].properties.summary.duration < cyclingRegularPath.features[0].properties.summary.duration;

            Console.WriteLine("Distance à pied plus courte ? {0}", chooseFootWalkingPathByDistance);
            Console.WriteLine("Durée à pied plus courte ? {0}", chooseFootWalkingPathByDuration);

            return (chooseFootWalkingPathByDistance && chooseFootWalkingPathByDuration) ? footWalkingPath : cyclingRegularPath;
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
                    Station potentialNearestStation = CallProxy("http://localhost:8081/API/JCDecaux/station?city=" + station.contract_name + "&number=" + station.number).Result.station;
                    if (potentialNearestStation.available_bikes >= THRESHOLD_AVAILABLE_BIKES && potentialNearestStation.status.Equals("OPEN"))
                    {
                        nearestStation = potentialNearestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialNearestStation.position.latitude, potentialNearestStation.position.longitude));
                    }
                }
            }

            if (nearestStation != null)
            {
                logs.Add(string.Format("{0}@&#&#&@{1}", DateTime.Now.ToString(), new Random().Next().ToString()), string.Format("{0}@&#&#&@{1}", nearestStation.contract_name, nearestStation.number.ToString()));
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
                    Station potentialNearestStation = CallProxy("http://localhost:8743/API/JCDecaux/station?city=" + station.contract_name + "&number=" + station.number).Result.station;
                    if (potentialNearestStation.available_bike_stands >= THRESHOLD_AVAILABLE_BIKES_STANDS && potentialNearestStation.status.Equals("OPEN"))
                    {
                        nearestStation = potentialNearestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialNearestStation.position.latitude, potentialNearestStation.position.longitude));
                    }
                }
            }

            if (nearestStation != null)
            {
                logs.Add(string.Format("{0}@&#&#&@{1}", DateTime.Now.ToString(), new Random().Next().ToString()), string.Format("{0}@&#&#&@{1}", nearestStation.contract_name, nearestStation.number.ToString()));
            }

            return nearestStation;
        }

        public Dictionary<string, string> GetLogs()
        {
            return logs;
        }

        private static async Task<List<Station>> CallJCDecaux(string request)
        {
            try
            {
                Console.WriteLine("[{0}] Requête GET vers JCDecaux pour obtenir toutes les stations avec l'URL : {1}\n", DateTime.Now, request);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Station>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static async Task<JCDecauxItem> CallProxy(string request)
        {
            try
            {
                Console.WriteLine("[{0}] Requête GET vers le Proxy avec l'URL : {1}\n", DateTime.Now, request);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<JCDecauxItem>(responseBody);
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
                Console.WriteLine("[{0}] Requête GET vers OpenStreetMap pour avoir le point le plus près d'une adresse avec l'URL : {1}\n", DateTime.Now, request);
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
                Console.WriteLine("[{0}] Requête POST vers OpenRouteService avec l'URL : {1} et les données : {2}\n", DateTime.Now, request, data);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri(request),
                    Method = HttpMethod.Post,
                    Content = content
                };
                httpRequest.Headers.Add("Authorization", "5b3ce3597851110001cf6248c20bc76cf8e34fd9b3413bf70ae6877d");

                HttpResponseMessage response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private string BuildDataORS(Position[] positions)
        {
            string data = "{\"coordinates\":[";

            foreach (Position position in positions)
            {
                data += "[" + position.longitude.ToString().Replace(",", ".") + "," + position.latitude.ToString().Replace(",", ".") + "],";
            }

            data += "],\"instructions\":\"true\",\"language\":\"fr\",\"preference\":\"shortest\",\"units\":\"m\"}";

            return data.Replace("],],", "]],");
        }
    }
}
