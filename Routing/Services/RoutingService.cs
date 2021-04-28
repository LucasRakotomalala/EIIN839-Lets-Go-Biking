using Proxy.Models;
using Routing.Models;
using Routing.Services.External;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.ServiceModel.Web;
using System.Text.Json;

namespace Routing
{
    public class RoutingService : IRouting
    {
        private static readonly JCDecaux jCDecaux = new JCDecaux();
        private static readonly ProxyService proxy = new ProxyService();

        private readonly OpenStreetMapNomatim openStreetMapNomatim = new OpenStreetMapNomatim();
        private readonly OpenRouteService openRouteService = new OpenRouteService();

        private static readonly List<Station> allStations = jCDecaux.GetStations().Result; //TODO: Update stations when specific ones are queried
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

            List<Place> places = openStreetMapNomatim.GetPlacesFromAddress(address).Result;
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

            GeoJson cyclingRegularPath = GetPath(positions, "cycling-regular");
            GeoJson footWalkingPath = GetPath(new Position[] { positions[0], positions[positions.Length - 1] }, "foot-walking");

            bool chooseFootWalkingPathByDistance = footWalkingPath.features[0].properties.summary.distance < cyclingRegularPath.features[0].properties.summary.distance;
            bool chooseFootWalkingPathByDuration = footWalkingPath.features[0].properties.summary.duration < cyclingRegularPath.features[0].properties.summary.duration;

            Console.WriteLine("Distance à pied plus courte ? {0}", chooseFootWalkingPathByDistance);
            Console.WriteLine("Durée à pied plus courte ? {0}", chooseFootWalkingPathByDuration);

            return (chooseFootWalkingPathByDistance && chooseFootWalkingPathByDuration) ? footWalkingPath : cyclingRegularPath;
        }

        public void PathOptions()
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }

        public GeoJson GoToStation(Position[] positions)
        {
            if (Array.Exists(positions, position => position == null))
            {
                return null;
            }

            return GetPath(positions, "foot-walking");
        }

        public void GoToStationOptions()
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
                    Station potentialNearestStation = proxy.GetJCDecauxItem(station.contract_name, station.number.ToString()).Result.station;
                    if (potentialNearestStation.available_bikes >= THRESHOLD_AVAILABLE_BIKES && potentialNearestStation.status.Equals("OPEN"))
                    {
                        nearestStation = potentialNearestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialNearestStation.position.latitude, potentialNearestStation.position.longitude));
                    }
                }
            }

            if (nearestStation != null)
            {
                logs.Add(string.Format("{0}@&#&#&@{1}", DateTime.Now, new Random().Next()), string.Format("{0}@&#&#&@{1}", nearestStation.contract_name, nearestStation.number));
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
                    Station potentialNearestStation = proxy.GetJCDecauxItem(station.contract_name, station.number.ToString()).Result.station;
                    if (potentialNearestStation.available_bike_stands >= THRESHOLD_AVAILABLE_BIKES_STANDS && potentialNearestStation.status.Equals("OPEN"))
                    {
                        nearestStation = potentialNearestStation;
                        distance = userPosition.GetDistanceTo(new GeoCoordinate(potentialNearestStation.position.latitude, potentialNearestStation.position.longitude));
                    }
                }
            }

            if (nearestStation != null)
            {
                logs.Add(string.Format("{0}@&#&#&@{1}", DateTime.Now, new Random().Next()), string.Format("{0}@&#&#&@{1}", nearestStation.contract_name, nearestStation.number));
            }

            return nearestStation;
        }

        public Dictionary<string, string> GetLogs()
        {
            return logs;
        }

        private GeoJson GetPath(Position[] positions, string profile)
        {
            return JsonSerializer.Deserialize<GeoJson>(openRouteService.PostDirections(profile, positions).Result);
        }
    }
}
