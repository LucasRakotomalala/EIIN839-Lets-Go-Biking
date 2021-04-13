using System.Runtime.Serialization;

namespace Routing.Models
{
    [DataContract]
    public class Place
    {
        public string display_name { get; set; }

        public string lat { get; set; }

        public string lon { get; set; }

        public double importance { get; set; }
    }
}
