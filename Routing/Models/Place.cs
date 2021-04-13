using System.Runtime.Serialization;

namespace Routing.Models
{
    [DataContract]
    public class Place
    {
        public string display_name { get; set; }
        public double lat { get; set; }

        public double lon { get; set; }
        public double importance { get; set; }
    }
}
