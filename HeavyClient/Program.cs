using HeavyClient.JCDecaux;
using System;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            JCDecauxClient client = new JCDecauxClient();

            foreach (var station in client.GetAllStationsFromCity("lyon"))
                Console.WriteLine(station.address);

            client.Close();

            Console.ReadLine();
        }
    }
}
