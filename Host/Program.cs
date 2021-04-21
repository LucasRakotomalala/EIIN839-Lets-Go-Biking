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

            Console.WriteLine("The routing service is ready at {0}", routingHost.BaseAddresses[0]);
            Console.WriteLine("The proxy service is ready at {0}", proxyHost.BaseAddresses[0]);
            Console.WriteLine("\nPress any key to close ...\n");
            Console.ReadLine();

            routingHost.Close();
            proxyHost.Close();
        }
    }
}
