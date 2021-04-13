using HeavyClient.Routing;
using System;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RoutingClient client = new RoutingClient("SOAPEndPoint");

            //JCDecauxItem item = client.GetAllStations();
            var itemLyon = client.GetAllStationsFromCity("lyon");

            foreach (var station in itemLyon)
                Console.WriteLine(station.position.latitude + " " + station.position.longitude);

            client.Close();

            Console.ReadLine();
        }
    }
}
