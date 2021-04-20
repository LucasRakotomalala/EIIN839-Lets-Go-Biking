using HeavyClient.Routing;
using System;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RoutingClient client = new RoutingClient("SOAPEndPoint");

            string input;

            do
            {
                Console.Clear();
                Console.WriteLine("Bienvenue sur le client .Net de Let's Go Biking !");
                Console.WriteLine("\nChoisissez ce que vous voulez faire :");
                Console.WriteLine("\t- route : Établir un itinéraire entre 2 adresses");
                Console.WriteLine("\t- stats : Voir les utilisations des différentes stations de JCDecaux depuis notre serveur");
                Console.WriteLine("\t- quit : Quitter le client .Net");
                Console.Write("\nChoix : ");
                input = Console.ReadLine();

                if (input.Equals("route"))
                {
                    SearchRoute(client);
                }
                else if (input.Equals("stats"))
                {
                    Console.WriteLine("YES");
                    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu principal ...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Mauvaise entrée, veuillez réessayer ...");
                }
            } while (!input.Equals("quit"));

            client.Close();
        }

        private static void SearchRoute(RoutingClient client)
        {
            do
            {
                Console.Write("Adresse de départ : ");
                string startAddress = Console.ReadLine().Replace(" ", "+"); // 11 Rue Saint Jacques, Lyon

                Console.Write("Adresse d'arrivée : ");
                string endAddress = Console.ReadLine().Replace(" ", "+"); // 19 Place Louis Pradel, Lyon

                Position startAddressPosition = client.GetPosition(startAddress);
                Position endAddressPosition = client.GetPosition(endAddress);

                if (startAddressPosition == null || endAddressPosition == null)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("\nAu moins une adresse est incorrecte ...");
                }
                else
                {
                    Station nearestStationFromStartPosition = client.FindNearestStationFromStart(startAddressPosition.latitude, startAddressPosition.longitude);
                    Station nearestStationFromEndPosition = client.FindNearestStationFromEnd(endAddressPosition.latitude, endAddressPosition.longitude);

                    Position[] positions = new Position[] { startAddressPosition, nearestStationFromStartPosition.position, nearestStationFromEndPosition.position, endAddressPosition };

                    GeoJson completePath = client.GetPath(positions);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("\nParcours complet à suivre :\n");
                    WriteStepsInstruction(completePath.features);

                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
               
                Console.WriteLine("\nAppuyez sur une touche pour refaire une requête ou taper 'quit' pour revenir au menu principal ...");
                Console.ResetColor();
            } while (Console.ReadLine() != "quit");
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
