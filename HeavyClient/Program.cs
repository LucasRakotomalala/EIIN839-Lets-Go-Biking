using HeavyClient.Routing;
using System;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RoutingClient client = new RoutingClient("SOAPEndPoint");

            do
            {
                Console.Clear();
                Console.ResetColor();

                Console.Write("Adresse de départ : ");
                string startAddress = Console.ReadLine().Replace(" ", "+"); // 11 Rue Saint Jacques, Lyon

                Console.Write("Adresse d'arrivée : ");
                string endAddress = Console.ReadLine().Replace(" ", "+"); // 19 Place Louis Pradel, Lyon

                var startAddressPosition = client.GetPosition(startAddress);
                var endAddressPosition = client.GetPosition(endAddress);

                var nearestStationFromStartPosition = client.FindNearestStationFromStart(startAddressPosition.latitude, startAddressPosition.longitude);
                var nearestStationFromEndPosition = client.FindNearestStationFromEnd(endAddressPosition.latitude, endAddressPosition.longitude);

                Position[] positions = new Position[] { startAddressPosition, nearestStationFromStartPosition.position, nearestStationFromEndPosition.position, endAddressPosition };

                GeoJson completePath = client.GetPath(positions);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\nParcours complet à suivre :\n");
                WriteStepsInstruction(completePath.features);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\nAppuyez sur une touche pour refaire une requête ...");
            } while (Console.ReadLine() != "quit");

            client.Close();

        }

        private static void WriteStepsInstruction(Feature[] features)
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
