using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    public class City
    {
        [DataMember]
        public string name { get; set; }
    }
}