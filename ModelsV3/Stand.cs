using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    class Stand
    {
        [DataMember]
        public Availabilities availabilities { get; set; }

        [DataMember]
        public int capacity { get; set; }

        public override string ToString()
        {
            return
                "Availabilities: " + availabilities + "\n" +
                "Capacity: " + capacity;
        }
    }
}
