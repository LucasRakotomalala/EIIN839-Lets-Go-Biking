using HeavyClient.Models;
using HeavyClient.Routing;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RoutingClient client = new RoutingClient("SOAPEndPoint");

            Console.Write("Adresse de départ : ");
            string startAddress = Console.ReadLine().Replace(" ", "+");

            Console.Write("Adresse d'arrivée : ");
            string endAddress = Console.ReadLine().Replace(" ", "+");

            var startAddressPosition = client.GetPosition(startAddress);
            var endAddressPosition = client.GetPosition(endAddress);

            var nearestStationFromStartPosition = client.FindNearestStationFromStart(startAddressPosition.latitude, startAddressPosition.longitude);
            var nearestStationFromEndPosition = client.FindNearestStationFromEnd(endAddressPosition.latitude, endAddressPosition.longitude);

            GeoJson pathStartToNearestStartStation = JsonSerializer.Deserialize<GeoJson>(client.GetPath(startAddressPosition.latitude, startAddressPosition.longitude, nearestStationFromStartPosition.position.latitude, nearestStationFromStartPosition.position.longitude));
            GeoJson pathStationStartToStationEnd = JsonSerializer.Deserialize<GeoJson>(client.GetPath(nearestStationFromStartPosition.position.latitude, nearestStationFromStartPosition.position.longitude, nearestStationFromEndPosition.position.latitude, nearestStationFromEndPosition.position.longitude));
            GeoJson pathNearestEndStationToEnd = JsonSerializer.Deserialize<GeoJson>(client.GetPath(nearestStationFromEndPosition.position.latitude, nearestStationFromEndPosition.position.longitude, endAddressPosition.latitude, endAddressPosition.longitude));

            /*Console.WriteLine(pathStartToNearestStartStation);
            Console.WriteLine(pathStationStartToStationEnd);
            Console.WriteLine(pathNearestEndStationToEnd);*/

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\nParcours pour se rendre à la station la plus proche de {0}", startAddress.Replace("+", " "));
            WriteStepsInstruction(pathStartToNearestStartStation.features);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\nParcours pour se rendre de la station {0} à la station {1}", nearestStationFromStartPosition, nearestStationFromEndPosition);
            WriteStepsInstruction(pathStationStartToStationEnd.features);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\nParcours pour se rendre à l'adresse {0}", endAddress.Replace("+", " "));
            WriteStepsInstruction(pathNearestEndStationToEnd.features);

            client.Close();

            Console.ReadLine();
        }

        private static void WriteStepsInstruction(List<Feature> features)
        {
            Console.ResetColor();
            foreach (Feature feature in features)
            {
                foreach (Segment segment in feature.properties.segments)
                {
                    foreach (Step step in segment.steps)
                    {
                        Console.WriteLine(step.instruction);
                    }
                }
            }
        }
    }
}
