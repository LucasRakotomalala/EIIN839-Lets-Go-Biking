using Proxy;
using Routing;
using System;
using System.ServiceModel;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost routingHost = new ServiceHost(typeof(RoutingService));
            ServiceHost proxyHost = new ServiceHost(typeof(JCDecaux));

            routingHost.Open();
            proxyHost.Open();

            do
            {
                Console.Clear();
                Console.WriteLine("Le service \"Routing\" est prêt à l'adresse : {0}", routingHost.BaseAddresses[0]);
                Console.WriteLine("Le service \"Proxy\" est prêt à l'adresse : {0}", proxyHost.BaseAddresses[0]);

                Console.WriteLine("\nAppuyez sur 'Entrée' pour fermer la fenêtre ...\n");
            } while (Console.ReadKey().Key != ConsoleKey.Enter);

            routingHost.Close();
            proxyHost.Close();
        }
    }
}
