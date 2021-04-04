using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    public class Station
    {
        [DataMember]
        public string contract_name { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public int number { get; set; }

        [DataMember]
        public Position position { get; set; }

        [DataMember]
        public int bike_stands { get; set; }

        [DataMember]
        public int available_bike_stands { get; set; }

        [DataMember]
        public int available_bikes { get; set; }

        [DataMember]
        public string status { get; set; }

        public override string ToString()
        {
            return
                "Contract Name: " + contract_name + "\n" +
                "Address: " + address + "\n" +
                "Name: " + name + "\n" +
                "Number: " + number + "\n" +
                position.ToString() +
                "Bike Stands: " + bike_stands + "\n" +
                "Available Bike Stands: " + available_bike_stands + "\n" +
                "Available Bikes: " + available_bikes + "\n" +
                "Status: " + status + "\n";
        }
    }
}