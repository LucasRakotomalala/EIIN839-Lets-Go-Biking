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
            string startAddress = Console.ReadLine().Replace(" ", "+"); // 11 Rue Saint Jacques, Lyon

            Console.Write("Adresse d'arrivée : ");
            string endAddress = Console.ReadLine().Replace(" ", "+"); // 19 Place Louis Pradel, Lyon

            var startAddressPosition = client.GetPosition(startAddress);
            var endAddressPosition = client.GetPosition(endAddress);
            Console.WriteLine(startAddressPosition.latitude);

            var nearestStationFromStartPosition = client.FindNearestStationFromStart(startAddressPosition.latitude, startAddressPosition.longitude);
            var nearestStationFromEndPosition = client.FindNearestStationFromEnd(endAddressPosition.latitude, endAddressPosition.longitude);

            Position[] positions = new Position[] { startAddressPosition, nearestStationFromStartPosition.position, nearestStationFromEndPosition.position, endAddressPosition};
            string result = client.GetPath(positions);

            GeoJson completePath = JsonSerializer.Deserialize<GeoJson>(result);

            Console.WriteLine("\nParcours complet à suivre :");
            WriteStepsInstruction(completePath.features);

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
