using System.Runtime.Serialization;

namespace Proxy.Models
{
    [DataContract]
    class Availabilities
    {
        [DataMember]
        public int bikes { get; set; }

        [DataMember]
        public int stands { get; set; }

        [DataMember]
        public int mechanicalBikes { get; set; }

        [DataMember]
        public int electricalBikes { get; set; }

        [DataMember]
        public int electricalInternalBatteryBikes { get; set; }

        [DataMember]
        public int electricalRemovableBatteryBikes { get; set; }

        public override string ToString()
        {
            return
                "Bikes: " + bikes + "\n" +
                "Stands: " + this.stands + "\n" +
                "Mechanical Bikes: " + mechanicalBikes + "\n" +
                "Electrical Bikes: " + electricalBikes + "\n" +
                "Electrical Internal Battery Bikes: " + electricalInternalBatteryBikes + "\n" +
                "Electrical Removable Battery Bikes: " + electricalRemovableBatteryBikes + "\n";
        }
    }
}
