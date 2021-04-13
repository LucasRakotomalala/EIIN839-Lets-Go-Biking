using System.Runtime.Serialization;

namespace Routing.Models
{
    [DataContract]
    public class ReverseGeoCode
    {
        [DataMember]
        public string display_name { get; set; }

        [DataMember]
        public Address address { get; set; }

        public class Address
        {
            [DataMember]
            public string city { get; set; }

            [DataMember]
            public string municipality { get; set; }

            [DataMember]
            public string county { get; set; }

            [DataMember]
            public string state_district { get; set; }

            [DataMember]
            public string state { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public string country_code { get; set; }
        }
    }
}
