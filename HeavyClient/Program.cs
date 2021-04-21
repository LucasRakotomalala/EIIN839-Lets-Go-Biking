using Proxy.Models;
using Routing;
using Routing.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Environment.GetEnvironmentVariable("host") ?? "localhost:8080";
            string address = string.Format("http://{0}/", host);

            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IRouting> factory = new ChannelFactory<IRouting>(binding, address);
            IRouting client = factory.CreateChannel();

            string input;

            do
            {
                Console.Clear();
                Console.WriteLine("Bienvenue sur le client .Net de Let's Go Biking !");
                Console.WriteLine("\nChoisissez ce que vous voulez faire :");
                Console.WriteLine("\t- path : Établir un itinéraire entre 2 adresses avec des vélos si nécessaire");
                Console.WriteLine("\t- stats : Voir les utilisations des différentes stations de JCDecaux depuis notre serveur");
                Console.WriteLine("\t- quit : Quitter le client .Net");
                Console.Write("\nChoix : ");
                input = Console.ReadLine().ToLower().Trim();

                if (input.Equals("path"))
                {
                    SearchRoute(client);
                }
                else if (input.Equals("stats"))
                {
                    WriteLogs(client);
                    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu principal ...");
                    Console.ReadLine();
                }
                else if (input.Equals("quit"))
                {
                    Console.WriteLine("Fermeture ...");
                }
                else
                {
                    Console.WriteLine("Mauvaise entrée, veuillez réessayer ...");
                }
            } while (!input.Equals("quit"));
        }

        private static void SearchRoute(IRouting client)
        {
            do
            {
                Console.Write("Adresse de départ : ");
                string startAddress = Console.ReadLine().Trim().Replace(" ", "+"); // 11 Rue Saint Jacques, Lyon

                Console.Write("Adresse d'arrivée : ");
                string endAddress = Console.ReadLine().Trim().Replace(" ", "+"); // 19 Place Louis Pradel, Lyon OR 127 Avenue du Prado, Marseille

                if (startAddress.Equals("null") || endAddress.Equals("null") || startAddress.Equals("") || endAddress.Equals(""))
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("\nAu moins une adresse est incorrecte ...");
                }
                else
                {
                    Position startAddressPosition = client.GetPosition(startAddress);
                    Position endAddressPosition = client.GetPosition(endAddress);
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
               
                Console.WriteLine("\nAppuyez sur une touche pour refaire une requête ou tapez 'quit' pour revenir au menu principal ...");
                Console.ResetColor();
            } while (Console.ReadLine().ToLower().Trim() != "quit");
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

        private static void WriteLogs(IRouting client)
        {
            foreach (KeyValuePair<string, string> item in client.GetLogs())
            {
                string[] separator = new string[] { "@&#&#&@" };

                string date = item.Key.Split(separator, StringSplitOptions.None)[0];
                string[] value = item.Value.Split(separator, StringSplitOptions.None);
                string city = value[0];
                string stationNumber = value[1];

                Console.WriteLine("[{0}] {1} - {2}", date, city, stationNumber);
            }
        }
    }
}
