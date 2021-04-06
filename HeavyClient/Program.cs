using HeavyClient.JCDecaux;
using System;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            JCDecauxClient client = new JCDecauxClient("SOAPEndPoint");

            //JCDecauxItem item = client.GetAllStations();
            JCDecauxItem itemLyon = client.GetAllStationsFromCity("lyon");

            foreach (var station in itemLyon.stations)
                Console.WriteLine(station.position.latitude + " " + station.position.longitude);

            client.Close();

            Console.ReadLine();
        }
    }
}
