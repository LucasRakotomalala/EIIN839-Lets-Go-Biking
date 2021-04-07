using System;
using System.ServiceModel;
using System.Threading;

namespace Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(JCDecaux));
            host.Open();

            Console.WriteLine("The service is ready at {0}", host.BaseAddresses[0]);
            Console.WriteLine("Press any key to close ...");
            Console.ReadLine();

            host.Close();
        }
    }
}
