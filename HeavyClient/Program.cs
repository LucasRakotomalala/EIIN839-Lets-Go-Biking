using HeavyClient.Routing;
using System;

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

            var pathStartToNearestStartStation = client.GetPath(startAddressPosition.latitude, startAddressPosition.longitude, nearestStationFromStartPosition.position.latitude, nearestStationFromStartPosition.position.longitude); ;
            var pathStationStartToStationEnd = client.GetPath(nearestStationFromStartPosition.position.latitude, nearestStationFromStartPosition.position.longitude, nearestStationFromEndPosition.position.latitude, nearestStationFromEndPosition.position.longitude);
            var pathNearestEndStationToEnd = client.GetPath(nearestStationFromEndPosition.position.latitude, nearestStationFromEndPosition.position.longitude, endAddressPosition.latitude, endAddressPosition.longitude);

            Console.WriteLine(pathStartToNearestStartStation);
            Console.WriteLine(pathStationStartToStationEnd);
            Console.WriteLine(pathNearestEndStationToEnd);

            client.Close();

            Console.ReadLine();
        }
    }
}
