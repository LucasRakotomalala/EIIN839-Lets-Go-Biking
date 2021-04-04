using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    public class Position
    {
        [DataMember]
        public double latitude { get; set; }

        [DataMember]
        public double longitude { get; set; }

        public override string ToString()
        {
            return
                "Latitude: " + latitude + "\n" +
                "Longitude: " + longitude + "\n";
        }
    }
}
