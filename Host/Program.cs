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
            routingHost.Open();

            Console.WriteLine("The routing service is ready at {0}", routingHost.BaseAddresses[0]);
            Console.WriteLine("Press any key to close ...");
            Console.ReadLine();

            routingHost.Close();
        }
    }
}
