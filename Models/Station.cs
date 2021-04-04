using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    class Station
    {
        [DataMember]
        public string contractName { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public int number { get; set; }

        [DataMember]
        public Position position { get; set; }

        [DataMember]
        public bool banking { get; set; }

        [DataMember]
        public bool bonus { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public bool connected { get; set; }

        [DataMember]
        public bool overflow { get; set; }

        [DataMember]
        public string shape { get; set; }

        [DataMember]
        public string overflowStands { get; set; }

        [DataMember]
        public Stand totalStands { get; set; }

        [DataMember]
        public Stand mainStands { get; set; }

        public override string ToString()
        {
            return
                "Contract Name: " + contractName + "\n" +
                "Address: " + address + "\n" +
                "Name: " + name + "\n" +
                "Number: " + number + "\n" +
                position.ToString() +
                "Banking: " + banking + "\n" +
                "Bonus: " + bonus + "\n" +
                "Status: " + status + "\n" +
                "Connected: " + connected + "\n" +
                "Overflow: " + overflow + "\n" +
                "Shape: " + shape + "\n" +
                "Overflow Stands: " + overflowStands + "\n" +
                "Total Stands: " + totalStands + "\n" +
                "Main Stands: " + mainStands + "\n";
        }
    }
}